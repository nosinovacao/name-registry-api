using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NAME.Registry
{
    /// <summary>
    /// Provides mechanisms to generate.
    /// </summary>
    public static class Generator
    {
        /// <summary>
        /// Generates a service id with the values provided.
        /// </summary>
        /// <param name="hostname">The hostname.</param>
        /// <param name="nameEndpoint">The name endpoint.</param>
        /// <param name="namePort">The name port.</param>
        /// <param name="appName">Name of the application.</param>
        /// <param name="appVersion">The application version.</param>
        /// <param name="nameVersion">The name version.</param>
        /// <returns>Returns the generated service Id.</returns>
        public static string RegisteredServiceId(
            string hostname,
            string nameEndpoint,
            uint? namePort,
            string appName,
            string appVersion,
            string nameVersion)
        {

            using (MemoryStream memStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(memStream, Encoding.UTF8, true))
                {
                    writer.Write(hostname);
                    writer.Write(nameEndpoint);
                    if (namePort != null)
                        writer.Write(namePort.Value);
                    writer.Write(appName);
                    writer.Write(appVersion);
                    writer.Write(nameVersion);
                    writer.Flush();
                }

                using (var hasher = MD5.Create())
                {
                    byte[] hash = hasher.ComputeHash(memStream.ToArray());

                    StringBuilder result = new StringBuilder(hash.Length * 2);

                    for (int i = 0; i < hash.Length; i++)
                        result.Append(hash[i].ToString("X2"));

                    return result.ToString();
                }
            }
        }
    }
}
