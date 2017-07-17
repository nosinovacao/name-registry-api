using NAME.Registry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Registry.Interfaces
{
    /// <summary>
    /// Represents a repository to access the services.
    /// </summary>
    public interface IRegisteredServiceRepository
    {
        /// <summary>
        /// Inserts the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        void Insert(RegisteredService service);

        /// <summary>
        /// Gets the Service by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Returns the service, or null if it does not exist.</returns>
        RegisteredService GetById(string id);

        /// <summary>
        /// Removes the specified Service.
        /// </summary>
        /// <param name="id">The Service.</param>
        void Remove(string id);

        /// <summary>
        /// Gets all the services.
        /// </summary>
        /// <param name="hostnameFilter">The hostname filter.</param>
        /// <param name="appNameFilter">The application name filter.</param>
        /// <param name="appVersionFilter">The application version filter.</param>
        /// <returns>
        /// Returns all the services.
        /// </returns>
        IEnumerable<RegisteredService> GetAll(string hostnameFilter = null, string appNameFilter = null, string appVersionFilter = null);

        /// <summary>
        /// Checks if the specified service exists by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Returns true if the service exists.</returns>
        bool Exists(string id);

        /// <summary>
        /// Updates the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>Returns true if a service is updated.</returns>
        bool Update(RegisteredService service);
    }
}
