using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.Registry.DTOs
{
    /// <summary>
    /// Represents the manifest snapshot.
    /// </summary>
    public class ManifestSnapshotDTO
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
