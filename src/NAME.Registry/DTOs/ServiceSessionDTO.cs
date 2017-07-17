using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.Registry.DTOs
{
    /// <summary>
    /// Represents a ServiceSession.
    /// </summary>
    public class ServiceSessionDTO
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the registered service.
        /// </summary>
        /// <value>
        /// The registered service.
        /// </value>
        public string RegisteredServiceId { get; set; }

        /// <summary>
        /// Gets or sets the last manifest snapshot.
        /// </summary>
        /// <value>
        /// The last manifest snapshot.
        /// </value>
        public ManifestSnapshotDTO LastManifestSnapshot { get; set; }

        /// <summary>
        /// Gets or sets the manifest snapshot count.
        /// </summary>
        /// <value>
        /// The manifest snapshot count.
        /// </value>
        public int ManifestSnapshotCount { get; set; }

        /// <summary>
        /// Gets or sets the last ping.
        /// </summary>
        /// <value>
        /// The last ping.
        /// </value>
        public DateTime? LastPing { get; set; }

        /// <summary>
        /// Gets or sets the bootstrapped date and time.
        /// </summary>
        /// <value>
        /// The bootstrapped date and time.
        /// </value>
        public DateTime Bootstrapped { get; set; }

        /// <summary>
        /// Gets or sets the invalidated date and time.
        /// </summary>
        /// <value>
        /// The invalidated date and time.
        /// </value>
        public DateTime? Invalidated { get; set; }

    }
}
