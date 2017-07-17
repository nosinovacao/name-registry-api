using NAME.Registry.Interfaces;
using System;
using NAME.Registry.Domain;
using Microsoft.Extensions.Logging;
using NAME.Registry.Exceptions;
using System.Collections.Generic;
using System.Linq;
using NAME.Registry.DTOs;

namespace NAME.Registry.Services
{
    /// <summary>
    /// Manages <see cref="RegisteredService"/>s.
    /// </summary>
    public class RegisteredServicesManager : IRegisteredServiceManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<RegisteredServicesManager> logger;

        /// <summary>
        /// The service repository.
        /// </summary>
        private IRegisteredServiceRepository serviceRepository;

        /// <summary>
        /// The session manager.
        /// </summary>
        private IServiceSessionManager sessionManager;

        /// <summary>
        /// The mapper
        /// </summary>
        private IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredServicesManager" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceRepository">The service repository.</param>
        /// <param name="sessionManager">The session manager.</param>
        /// <param name="mapper">The mapper.</param>
        public RegisteredServicesManager(ILogger<RegisteredServicesManager> logger, IRegisteredServiceRepository serviceRepository, IServiceSessionManager sessionManager, IMapper mapper)
        {
            Guard.NotNull(logger, nameof(logger));
            Guard.NotNull(serviceRepository, nameof(serviceRepository));
            Guard.NotNull(sessionManager, nameof(sessionManager));
            Guard.NotNull(mapper, nameof(mapper));

            this.logger = logger;
            this.serviceRepository = serviceRepository;
            this.sessionManager = sessionManager;
            this.mapper = mapper;
        }

        /// <summary>
        /// Registers a service and creates a new session.
        /// <para>If the service is already registered, the already existant service is returned and a new session is generated.</para>
        /// </summary>
        /// <param name="hostname">The hostname.</param>
        /// <param name="nameEndpoint">The name endpoint.</param>
        /// <param name="namePort">The name port.</param>
        /// <param name="appName">Name of the application.</param>
        /// <param name="appVersion">The application version.</param>
        /// <param name="nameVersion">The name version.</param>
        /// <returns>
        /// Returns the <see cref="RegisteredServiceDTO"/>.
        /// </returns>
        /// <exception cref="RepositoryEntityOperationFailedException">RegisteredService</exception>
        public RegisteredServiceDTO RegisterService(
                    string hostname,
                    string nameEndpoint,
                    uint? namePort,
                    string appName,
                    string appVersion,
                    string nameVersion)
        {
            Guard.NotNull(hostname, nameof(hostname));
            Guard.NotNull(nameEndpoint, nameof(nameEndpoint));
            Guard.NotNull(appName, nameof(appName));
            Guard.NotNull(appVersion, nameof(appVersion));
            Guard.NotNull(nameVersion, nameof(nameVersion));

            this.logger.LogInformation($"Registering a {nameof(RegisteredService)}.");

            string generatedId = Generator.RegisteredServiceId(hostname, nameEndpoint, namePort, appName, appVersion, nameVersion);

            var registeredService = this.serviceRepository.GetById(generatedId);

            if (registeredService != null)
            {
                this.logger.LogInformation($"The {nameof(RegisteredService)} already exists. Creating a new {nameof(ServiceSession)}.");
                var session = this.sessionManager.CreateSession(generatedId);
                this.logger.LogDebug($"Setting the {registeredService.CurrentSessionId} to the newly created {nameof(ServiceSession)}'s id ({session.Id}).");

                registeredService.CurrentSessionId = Guid.Parse(session.Id);
                registeredService.Updated = DateTime.UtcNow;

                this.logger.LogDebug($"Updating the {nameof(RegisteredService)} in the repository.");
                if (!this.serviceRepository.Update(registeredService))
                    throw new RepositoryEntityOperationFailedException(registeredService, $"Failed to update the {nameof(RegisteredService)}.");

                this.logger.LogDebug($"Returning the {nameof(RegisteredService)}: {{{registeredService}}} AND {nameof(ServiceSession)}: {{{session}}}.");

                var mappedService = this.mapper.Map<RegisteredService, RegisteredServiceDTO>(registeredService);
                mappedService.CurrentSession = session;
                return mappedService;
            }
            else
            {
                this.logger.LogInformation($"The provided values are new. Creating a new {nameof(RegisteredService)}.");

                this.logger.LogDebug($"Creating a new {nameof(ServiceSession)}.");
                var session = this.sessionManager.CreateSession(generatedId);

                var service = new RegisteredService()
                {
                    AppName = appName,
                    AppVersion = appVersion,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    Hostname = hostname,
                    Id = generatedId,
                    NAMEEndpoint = nameEndpoint,
                    NAMEPort = namePort,
                    NAMEVersion = nameVersion,
                    CurrentSessionId = Guid.Parse(session.Id)
                };

                this.logger.LogDebug($"Creating a new {nameof(RegisteredService)} with the {nameof(service.CurrentSessionId)} set to {session.Id}.");
                this.serviceRepository.Insert(service);

                var mappedService = this.mapper.Map<RegisteredService, RegisteredServiceDTO>(service);
                mappedService.CurrentSession = session;
                return mappedService;
            }
        }

        /// <summary>
        /// Gets all the <see cref="RegisteredServiceDTO" />s.
        /// </summary>
        /// <param name="hostnameFilter">The hostname filter.</param>
        /// <param name="appNameFilter">The application name filter.</param>
        /// <param name="appVersionFilter">The application version filter.</param>
        /// <param name="minimumLastPingFilter">The minimum last ping filter.</param>
        /// <returns>
        /// Returns the <see cref="RegisteredServiceDTO" />s.
        /// </returns>
        public IEnumerable<RegisteredServiceDTO> GetAll(string hostnameFilter = null, string appNameFilter = null, string appVersionFilter = null, DateTime? minimumLastPingFilter = null)
        {
            hostnameFilter = hostnameFilter ?? string.Empty;
            appNameFilter = appNameFilter ?? string.Empty;
            appVersionFilter = appVersionFilter ?? string.Empty;

            var services = this.serviceRepository.GetAll(hostnameFilter, appNameFilter, appVersionFilter);
            if (services.Count() == 0)
                return Enumerable.Empty<RegisteredServiceDTO>();

            var mappedServices = new List<RegisteredServiceDTO>();

            foreach (var service in services)
            {
                var currentSession = this.sessionManager.GetById(service.CurrentSessionId);

                if (minimumLastPingFilter != null && (currentSession.LastPing == null || currentSession.LastPing.Value < minimumLastPingFilter))
                    continue;

                var mappedService = this.mapper.Map<RegisteredService, RegisteredServiceDTO>(service);
                mappedService.CurrentSession = currentSession;
                mappedServices.Add(mappedService);
            }
            return mappedServices;
        }

        /// <summary>
        /// Gets all the <see cref="T:NAME.Registry.Domain.RegisteredService" />s and corresponding <see cref="T:NAME.Registry.Domain.ServiceSession" />.
        /// </summary>
        /// <returns>
        /// Returns the <see cref="T:NAME.Registry.Domain.RegisteredService" />s and <see cref="T:NAME.Registry.Domain.ServiceSession" />.
        /// </returns>
        public IEnumerable<RegisteredServiceWithSessionsDTO> GetAllWithSessions()
        {
            var services = this.serviceRepository.GetAll();

            if (services.Count() == 0)
                return Enumerable.Empty<RegisteredServiceWithSessionsDTO>();


            var sessions = this.sessionManager.GetAll(true).GroupBy(s => s.RegisteredServiceId).ToDictionary(g => g.Key, g => g.AsEnumerable());

            var resultServices = this.mapper.Map<IEnumerable<RegisteredService>, IEnumerable<RegisteredServiceWithSessionsDTO>>(services);
            foreach (var service in resultServices)
            {
                if (sessions.TryGetValue(service.Id, out var serviceSessions))
                {
                    service.CurrentSession = serviceSessions.FirstOrDefault(s => s.Invalidated == null);
                    service.ServiceSessions = serviceSessions;
                }
            }

            return resultServices;
        }

        /// <summary>
        /// Gets the <see cref="RegisteredServiceDTO" /> by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// Returns the <see cref="RegisteredServiceDTO" />.
        /// </returns>
        public RegisteredServiceDTO GetByID(string id)
        {
            Guard.NotNull(id, nameof(id));

            var service = this.serviceRepository.GetById(id);
            if (service == null)
                return null;

            var mappedService = this.mapper.Map<RegisteredService, RegisteredServiceDTO>(service);
            mappedService.CurrentSession = this.sessionManager.GetById(service.CurrentSessionId);

            return mappedService;
        }
    }
}
