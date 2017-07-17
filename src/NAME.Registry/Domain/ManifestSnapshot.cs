using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Registry.Domain
{
    /// <summary>
    /// Represents a manifest at a certain time.
    /// </summary>
    public class ManifestSnapshot
    {
        /// <summary>
        /// Gets or sets the manifest.
        /// </summary>
        /// <value>
        /// The manifest.
        /// </value>
        public string Manifest { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time.
        /// </summary>
        /// <value>
        /// The date and time.
        /// </value>
        public DateTime DateAndTime { get; set; }
    }
}
