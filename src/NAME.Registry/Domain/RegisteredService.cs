using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NAME.Registry.Domain
{
    /// <summary>
    /// Represents a service.
    /// </summary>
    public class RegisteredService
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the name endpoint.
        /// </summary>
        /// <value>
        /// The name endpoint.
        /// </value>
        public string NAMEEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the NAME port.
        /// </summary>
        /// <value>
        /// The NAME port.
        /// </value>
        public uint? NAMEPort { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        public string AppName { get; set; }

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        public string AppVersion { get; set; }

        /// <summary>
        /// Gets or sets the NAME version.
        /// </summary>
        /// <value>
        /// The name version.
        /// </value>
        public string NAMEVersion { get; set; }

        /// <summary>
        /// Gets or sets the current session.
        /// </summary>
        /// <value>
        /// The current session.
        /// </value>
        public Guid CurrentSessionId { get; set; }

        /// <summary>
        /// Gets or sets the created time.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the updated time.
        /// </summary>
        /// <value>
        /// The updated time.
        /// </value>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredService"/> class.
        /// </summary>
        public RegisteredService()
        {

        }
    }
}
