namespace NAME.Registry.DTOs
{
    /// <summary>
    /// Represents a Registered Service.
    /// </summary>
    public class RegisteredServiceDTO
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
        /// Gets or sets the current session.
        /// </summary>
        /// <value>
        /// The current session.
        /// </value>
        public ServiceSessionDTO CurrentSession { get; set; }
    }
}
