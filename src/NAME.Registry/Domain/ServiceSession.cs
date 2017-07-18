using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Registry.Domain
{
    /// <summary>
    /// Represents a session for a service.
    /// </summary>
    public class ServiceSession
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the registered service.
        /// </summary>
        /// <value>
        /// The registered service.
        /// </value>
        public string RegisteredServiceId { get; set; }

        /// <summary>
        /// Gets or sets the manifest snapshots.
        /// </summary>
        /// <value>
        /// The manifest snapshots.
        /// </value>
        public List<ManifestSnapshot> ManifestSnapshots { get; set; }

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

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceSession"/> class.
        /// </summary>
        public ServiceSession()
        {
            this.ManifestSnapshots = new List<ManifestSnapshot>();
        }
    }
}
