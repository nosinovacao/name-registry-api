using LiteDB;
using Microsoft.Extensions.Logging;
using NAME.Registry.Domain;
using NAME.Registry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Registry.Repositories.LiteDB
{
    /// <summary>
    /// Provides a mechanism to access the <see cref="ServiceSession"/> repository using LiteDB.
    /// </summary>
    /// <seealso cref="NAME.Registry.Interfaces.IServiceSessionRepository" />
    public class ServiceSessionRepository : IServiceSessionRepository
    {
        /// <summary>
        /// The logger
        /// </summary>
        private ILogger<ServiceSessionRepository> logger;

        /// <summary>
        /// The session collection
        /// </summary>
        private LiteCollection<ServiceSession> sessionCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceSessionRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceCollection">The service collection.</param>
        public ServiceSessionRepository(ILogger<ServiceSessionRepository> logger, LiteCollection<ServiceSession> serviceCollection)
        {
            this.logger = logger;
            this.sessionCollection = serviceCollection;
            this.sessionCollection.EnsureIndex(s => s.Id, true);
            this.sessionCollection.EnsureIndex(s => s.RegisteredServiceId, false);
            this.sessionCollection.EnsureIndex(s => s.Invalidated, false);
        }

        /// <summary>
        /// Checks if the specified service session exists by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// Returns true if the service session exists.
        /// </returns>
        public bool Exists(Guid id)
        {
            return this.sessionCollection.Exists(s => s.Id == id);
        }

        /// <summary>
        /// Gets all the <see cref="T:NAME.Registry.Domain.ServiceSession" />s.
        /// </summary>
        /// <param name="includeInvalidated">if set to <c>true</c> includes invalidated <see cref="T:NAME.Registry.Domain.ServiceSession" />s.</param>
        /// <returns>
        /// Returns the <see cref="T:NAME.Registry.Domain.ServiceSession" />s.
        /// </returns>
        public IEnumerable<ServiceSession> GetAll(bool includeInvalidated = false)
        {
            if (includeInvalidated)
                return this.sessionCollection.FindAll();
            else
                return this.sessionCollection.Find(s => s.Invalidated == null);
        }

        /// <summary>
        /// Gets all the <see cref="T:NAME.Registry.Domain.ServiceSession" />s with the specified <see cref="P:NAME.Registry.Domain.ServiceSession.RegisteredServiceId" />.
        /// </summary>
        /// <param name="registeredServiceId">The <see cref="P:NAME.Registry.Domain.ServiceSession.RegisteredServiceId" />.</param>
        /// <param name="includeInvalidated">if set to <c>true</c> includes invalidated <see cref="T:NAME.Registry.Domain.ServiceSession" />s.</param>
        /// <returns>
        /// Returns the <see cref="T:NAME.Registry.Domain.ServiceSession" />s.
        /// </returns>
        public IEnumerable<ServiceSession> GetAllInRegisteredService(string registeredServiceId, bool includeInvalidated)
        {
            if (includeInvalidated)
                return this.sessionCollection.Find(s => s.RegisteredServiceId == registeredServiceId);
            else
                return this.sessionCollection.Find(s => s.RegisteredServiceId == registeredServiceId && s.Invalidated == null);
        }
        
        /// <summary>
        /// Gets the service session by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// Returns the service session, or null if it does not exist.
        /// </returns>
        public ServiceSession GetById(Guid id)
        {
            return this.sessionCollection.FindById(id);
        }

        /// <summary>
        /// Inserts the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        public void Insert(ServiceSession service)
        {
            this.sessionCollection.Insert(service);
        }

        /// <summary>
        /// Updates the specified service session.
        /// </summary>
        /// <param name="session">The service session.</param>
        /// <returns>
        /// Returns true if a service session is updated.
        /// </returns>
        public bool Update(ServiceSession session)
        {
            return this.sessionCollection.Update(session);
        }

        /// <summary>
        /// Updates many <see cref="T:NAME.Registry.Domain.ServiceSession" />s.
        /// </summary>
        /// <param name="sessions">The sessions.</param>
        /// <returns>
        /// Returns the number of updated <see cref="T:NAME.Registry.Domain.ServiceSession" />s.
        /// </returns>
        public int UpdateMany(IEnumerable<ServiceSession> sessions)
        {
            return this.sessionCollection.Update(sessions);
        }
    }
}
