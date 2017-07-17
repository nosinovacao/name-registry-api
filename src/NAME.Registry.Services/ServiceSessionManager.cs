using NAME.Registry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NAME.Registry.Domain;
using Microsoft.Extensions.Logging;
using NAME.Registry.Exceptions;
using NAME.Registry.DTOs;

namespace NAME.Registry.Services
{
    /// <summary>
    /// Provides mechanisms to manage <see cref="ServiceSession"/>s.
    /// </summary>
    /// <seealso cref="NAME.Registry.Interfaces.IServiceSessionManager" />
    public class ServiceSessionManager : IServiceSessionManager
    {
        private ILogger<ServiceSessionManager> logger;
        private IServiceSessionRepository sessionRepository;
        private IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceSessionManager" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="sessionRepository">The session repository.</param>
        /// <param name="mapper">The mapper.</param>
        public ServiceSessionManager(ILogger<ServiceSessionManager> logger, IServiceSessionRepository sessionRepository, IMapper mapper)
        {
            Guard.NotNull(logger, nameof(logger));
            Guard.NotNull(sessionRepository, nameof(sessionRepository));
            Guard.NotNull(mapper, nameof(mapper));

            this.logger = logger;
            this.sessionRepository = sessionRepository;
            this.mapper = mapper;
        }

        /// <summary>
        /// Adds the manifest snapshot to a service session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="manifest">The manifest.</param>
        /// <returns>
        /// Returns the added <see cref="T:NAME.Registry.Domain.ManifestSnapshot" />.
        /// </returns>
        /// <exception cref="NAME.Registry.Exceptions.EntityNotFoundException">Happens when the entity is not found.</exception>
        /// <exception cref="RepositoryEntityOperationFailedException">ServiceSession</exception>
        public ManifestSnapshotDTO AddManifestSnapshot(Guid sessionId, string manifest)
        {
            Guard.NotNull(sessionId, nameof(sessionId));
            Guard.NotNull(manifest, nameof(manifest));

            ServiceSession session = this.sessionRepository.GetById(sessionId);
            if (session == null)
            {
                this.logger.LogWarning($"Received a {nameof(this.AddManifestSnapshot)} request for a nonexistent Session.");
                throw new EntityNotFoundException();
            }

            this.logger.LogInformation($"Adding the manifest with to the session {session.Id}.");

            var now = DateTime.UtcNow;
            var snapshot = new ManifestSnapshot() { Manifest = manifest, DateAndTime = now };

            session.LastPing = now;

            session.ManifestSnapshots.Add(snapshot);

            if (!this.sessionRepository.Update(session))
                throw new RepositoryEntityOperationFailedException(session, $"Failed to update the {nameof(ServiceSession)}.");

            return this.mapper.Map<ManifestSnapshot, ManifestSnapshotDTO>(snapshot);
        }

        /// <summary>
        /// Adds the session.
        /// </summary>
        /// <param name="registeredServiceId">The registered service identifier.</param>
        /// <returns>
        /// Returns the created <see cref="T:NAME.Registry.Domain.ServiceSession" />
        /// </returns>
        public ServiceSessionDTO CreateSession(string registeredServiceId)
        {
            Guard.NotNull(registeredServiceId, nameof(registeredServiceId));

            this.logger.LogInformation($"Creating a new session for the {nameof(RegisteredService)} with id {registeredServiceId}.");

            var oldValidSessions = this.sessionRepository.GetAllInRegisteredService(registeredServiceId, false).ToList();

            ServiceSession session = new ServiceSession()
            {
                RegisteredServiceId = registeredServiceId,
                Bootstrapped = DateTime.UtcNow
            };
            this.sessionRepository.Insert(session);

            if (oldValidSessions.Count() > 0)
            {
                foreach (var validSession in oldValidSessions)
                {
                    validSession.Invalidated = DateTime.UtcNow;
                }
                this.sessionRepository.UpdateMany(oldValidSessions);
            }

            return this.MapToDTO(session);
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>
        /// Returns the <see cref="T:NAME.Registry.Domain.ServiceSession" />.
        /// </returns>
        public ServiceSessionDTO GetById(Guid sessionId)
        {
            Guard.NotNull(sessionId, nameof(sessionId));

            this.logger.LogInformation($"Fetching the session with id {sessionId}.");

            var session = this.sessionRepository.GetById(sessionId);
            if (session == null)
                return null;

            return this.MapToDTO(session);
        }

        /// <summary>
        /// Pings the session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <exception cref="EntityNotFoundException">Happens when the entity was not found.</exception>
        /// <exception cref="RepositoryEntityOperationFailedException">ServiceSession</exception>
        public void PingSession(Guid sessionId)
        {
            Guard.NotNull(sessionId, nameof(sessionId));

            ServiceSession session = this.sessionRepository.GetById(sessionId);
            if (session == null)
            {
                this.logger.LogWarning($"Received a {nameof(this.AddManifestSnapshot)} request for a nonexistent Session.");
                throw new EntityNotFoundException();
            }

            var theTime = DateTime.UtcNow;
            this.logger.LogInformation($"Setting the {nameof(session.LastPing)} of the {nameof(ServiceSession)} with id {session.Id} to {theTime}.");

            session.LastPing = DateTime.UtcNow;

            if (!this.sessionRepository.Update(session))
                throw new RepositoryEntityOperationFailedException(session, $"Failed to update the {nameof(ServiceSession)}.");
        }

        /// <summary>
        /// Gets all the <see cref="T:NAME.Registry.Domain.ServiceSession" />s.
        /// </summary>
        /// <param name="includeInvalidated">if set to <c>true</c> [include invalidated].</param>
        /// <returns>
        /// Returns the <see cref="T:NAME.Registry.Domain.ServiceSession" />s.
        /// </returns>
        public IEnumerable<ServiceSessionDTO> GetAll(bool includeInvalidated = false)
        {
            return this.mapper.Map<IEnumerable<ServiceSession>, IEnumerable<ServiceSessionDTO>>(this.sessionRepository.GetAll(includeInvalidated));
        }

        /// <summary>
        /// Gets all the <see cref="T:NAME.Registry.Domain.ServiceSession" />s of the specified <see cref="T:NAME.Registry.Domain.RegisteredService" /> id.
        /// </summary>
        /// <param name="registeredServiceId">The <see cref="T:NAME.Registry.Domain.RegisteredService" /> identifier.</param>
        /// <param name="includeInvalidated">if set to <c>true</c> [include invalidated].</param>
        /// <returns>
        /// Returns the <see cref="T:NAME.Registry.Domain.ServiceSession" />s.
        /// </returns>
        public IEnumerable<ServiceSessionDTO> GetAllInRegisteredService(string registeredServiceId, bool includeInvalidated = false)
        {
            Guard.NotNull(registeredServiceId, nameof(registeredServiceId));

            return this.mapper.Map<IEnumerable<ServiceSession>, IEnumerable<ServiceSessionDTO>>(this.sessionRepository.GetAllInRegisteredService(registeredServiceId, includeInvalidated));
        }

        /// <summary>
        /// Gets all the <see cref="T:NAME.Registry.DTOs.ManifestSnapshotDTO" /> of a specific <see cref="T:NAME.Registry.Domain.ServiceSession" />.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>
        /// Returns the <see cref="T:NAME.Registry.DTOs.ManifestSnapshotDTO" />s.
        /// </returns>
        /// <exception cref="NAME.Registry.Exceptions.EntityNotFoundException">Happens when the entity does not exist.</exception>
        public IEnumerable<ManifestSnapshotDTO> GetAllSnapshots(Guid sessionId)
        {
            var session = this.sessionRepository.GetById(sessionId);

            if (session == null)
                throw new EntityNotFoundException();

            return this.mapper.Map<IEnumerable<ManifestSnapshot>, IEnumerable<ManifestSnapshotDTO>>(session.ManifestSnapshots);
        }

        private ServiceSessionDTO MapToDTO(ServiceSession domainObject)
        {
            var mappedSession = this.mapper.Map<ServiceSession, ServiceSessionDTO>(domainObject);
            var lastSnapshot = domainObject.ManifestSnapshots.OrderBy(m => m.DateAndTime).LastOrDefault();
            if (lastSnapshot != null)
                mappedSession.LastManifestSnapshot = this.mapper.Map<ManifestSnapshot, ManifestSnapshotDTO>(lastSnapshot);
            return mappedSession;
        }

    }
}
