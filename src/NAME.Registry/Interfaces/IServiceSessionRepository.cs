using NAME.Registry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Registry.Interfaces
{
    /// <summary>
    /// Provides mechanisms to access the repository of <see cref="ServiceSession"/>s.
    /// </summary>
    public interface IServiceSessionRepository
    {
        /// <summary>
        /// Inserts the specified service session.
        /// </summary>
        /// <param name="session">The service session.</param>
        void Insert(ServiceSession session);

        /// <summary>
        /// Gets the service session by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Returns the service session, or null if it does not exist.</returns>
        ServiceSession GetById(Guid id);

        /// <summary>
        /// Checks if the specified service session exists by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Returns true if the service session exists.</returns>
        bool Exists(Guid id);

        /// <summary>
        /// Updates the specified service session.
        /// </summary>
        /// <param name="session">The service session.</param>
        /// <returns>Returns true if a service session is updated.</returns>
        bool Update(ServiceSession session);

        /// <summary>
        /// Updates many <see cref="ServiceSession"/>s.
        /// </summary>
        /// <param name="sessions">The sessions.</param>
        /// <returns>Returns the number of updated <see cref="ServiceSession"/>s.</returns>
        int UpdateMany(IEnumerable<ServiceSession> sessions);

        /// <summary>
        /// Gets all the <see cref="ServiceSession" />s.
        /// </summary>
        /// <param name="includeInvalidated">if set to <c>true</c> includes invalidated <see cref="ServiceSession"/>s.</param>
        /// <returns>
        /// Returns the <see cref="ServiceSession" />s.
        /// </returns>
        IEnumerable<ServiceSession> GetAll(bool includeInvalidated);

        /// <summary>
        /// Gets all the <see cref="ServiceSession" />s with the specified <see cref="ServiceSession.RegisteredServiceId"/>.
        /// </summary>
        /// <param name="registeredServiceId">The <see cref="ServiceSession.RegisteredServiceId"/>.</param>
        /// <param name="includeInvalidated">if set to <c>true</c> includes invalidated <see cref="ServiceSession"/>s.</param>
        /// <returns>
        /// Returns the <see cref="ServiceSession" />s.
        /// </returns>
        IEnumerable<ServiceSession> GetAllInRegisteredService(string registeredServiceId, bool includeInvalidated);
    }
}
