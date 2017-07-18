using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAME.Registry.DTOs
{
    /// <summary>
    /// Represents a <see cref="RegisteredServiceDTO"/> with the corresponding <see cref="ServiceSessionDTO"/>s
    /// </summary>
    /// <seealso cref="NAME.Registry.DTOs.RegisteredServiceDTO" />
    public class RegisteredServiceWithSessionsDTO : RegisteredServiceDTO
    {
        /// <summary>
        /// Gets or sets the service sessions.
        /// </summary>
        /// <value>
        /// The service sessions.
        /// </value>
        public IEnumerable<ServiceSessionDTO> ServiceSessions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredServiceWithSessionsDTO"/> class.
        /// </summary>
        public RegisteredServiceWithSessionsDTO()
        {
            this.ServiceSessions = Enumerable.Empty<ServiceSessionDTO>();
        }
    }
}
