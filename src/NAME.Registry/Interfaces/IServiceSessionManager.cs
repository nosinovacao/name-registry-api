using NAME.Registry.Domain;
using NAME.Registry.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Registry.Interfaces
{
    /// <summary>
    /// Provides mechanisms to manage <see cref="ServiceSession" />s.
    /// </summary>
    public interface IServiceSessionManager
    {
        /// <summary>
        /// Pings the session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        void PingSession(Guid sessionId);

        /// <summary>
        /// Adds the manifest snapshot to a service session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="manifest">The manifest.</param>
        /// <returns>
        /// Returns the added <see cref="ManifestSnapshotDTO" />.
        /// </returns>
        ManifestSnapshotDTO AddManifestSnapshot(Guid sessionId, string manifest);

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>
        /// Returns the <see cref="ServiceSessionDTO" />.
        /// </returns>
        ServiceSessionDTO GetById(Guid sessionId);

        /// <summary>
        /// Adds the session.
        /// </summary>
        /// <param name="registeredServiceId">The registered service identifier.</param>
        /// <returns>
        /// Returns the created <see cref="ServiceSessionDTO" />
        /// </returns>
        ServiceSessionDTO CreateSession(string registeredServiceId);

        /// <summary>
        /// Gets all the <see cref="ServiceSessionDTO" />s.
        /// </summary>
        /// <param name="includeInvalidated">if set to <c>true</c> [include invalidated].</param>
        /// <returns>
        /// Returns the <see cref="ServiceSessionDTO" />s.
        /// </returns>
        IEnumerable<ServiceSessionDTO> GetAll(bool includeInvalidated = false);

        /// <summary>
        /// Gets all the <see cref="ServiceSessionDTO" />s of the specified <see cref="RegisteredServiceDTO" /> id.
        /// </summary>
        /// <param name="registeredServiceId">The <see cref="RegisteredServiceDTO" /> identifier.</param>
        /// <param name="includeInvalidated">if set to <c>true</c> [include invalidated].</param>
        /// <returns>
        /// Returns the <see cref="ServiceSessionDTO" />s.
        /// </returns>
        IEnumerable<ServiceSessionDTO> GetAllInRegisteredService(string registeredServiceId, bool includeInvalidated = false);

        /// <summary>
        /// Gets all the <see cref="ManifestSnapshotDTO"/> of a specific <see cref="ServiceSession"/>.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>Returns the <see cref="ManifestSnapshotDTO"/>s.</returns>
        IEnumerable<ManifestSnapshotDTO> GetAllSnapshots(Guid sessionId);
        
    }
}
