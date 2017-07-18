using NAME.Registry.Domain;
using NAME.Registry.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Registry.Interfaces
{
    /// <summary>
    /// Provides mechanisms to manage <see cref="RegisteredService" />s.
    /// </summary>
    public interface IRegisteredServiceManager
    {
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
        /// <returns>Returns the <see cref="RegisteredService"/> and the corresponding <see cref="ServiceSession"/>.</returns>
        RegisteredServiceDTO RegisterService(string hostname, string nameEndpoint, uint? namePort, string appName, string appVersion, string nameVersion);

        /// <summary>
        /// Gets all the <see cref="RegisteredService" />s.
        /// </summary>
        /// <param name="hostnameFilter">The hostname filter.</param>
        /// <param name="appNameFilter">The application name filter.</param>
        /// <param name="appVersionFilter">The application version filter.</param>
        /// <param name="minimumLastPingFilter">The minimum last ping filter.</param>
        /// <returns>
        /// Returns the <see cref="RegisteredService" />s.
        /// </returns>
        IEnumerable<RegisteredServiceDTO> GetAll(string hostnameFilter = null, string appNameFilter = null, string appVersionFilter = null, DateTime? minimumLastPingFilter = null);

        /// <summary>
        /// Gets all the <see cref="RegisteredService"/>s and corresponding <see cref="ServiceSession"/>.
        /// </summary>
        /// <returns>Returns the <see cref="RegisteredService"/>s and <see cref="ServiceSession"/>.</returns>
        IEnumerable<RegisteredServiceWithSessionsDTO> GetAllWithSessions();

        /// <summary>
        /// Gets the <see cref="RegisteredService"/> by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Returns the <see cref="RegisteredService"/>.</returns>
        RegisteredServiceDTO GetByID(string id);
    }
}
