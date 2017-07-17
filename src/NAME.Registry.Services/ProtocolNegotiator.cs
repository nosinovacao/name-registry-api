using Microsoft.Extensions.Logging;
using NAME.Registry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAME.Registry.Services
{
    /// <summary>
    /// Provides mechanisms to negotiate the protocol between the registrar and client.
    /// </summary>
    /// <seealso cref="NAME.Registry.Interfaces.IProtocolNegotiator" />
    public class ProtocolNegotiator : IProtocolNegotiator
    {
        private uint[] registrarSupportedProtocols;
        private ILogger<IProtocolNegotiator> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolNegotiator" /> class.
        /// </summary>
        /// <param name="registrarSupportedProtocols">The protocols supported by the registrar.</param>
        /// <param name="logger">The logger.</param>
        public ProtocolNegotiator(uint[] registrarSupportedProtocols, ILogger<IProtocolNegotiator> logger)
        {
            Guard.NotNull(registrarSupportedProtocols, nameof(registrarSupportedProtocols));
            Guard.NotNull(logger, nameof(logger));

            this.registrarSupportedProtocols = registrarSupportedProtocols;
            this.logger = logger;
        }

        /// <summary>
        /// Tries to choose a protocol to be used between the registrar and a specific client.
        /// </summary>
        /// <param name="serviceSupportedProtocols">The protocols supported by the client service.</param>
        /// <param name="chosenProtocol">The chosen protocol.</param>
        /// <returns>
        /// Returns <c>true</c> if a protocol is chosen successfuly. <c>false</c> if not.
        /// </returns>
        public bool TryChooseProtocol(uint[] serviceSupportedProtocols, out uint chosenProtocol)
        {
            chosenProtocol = 0;
            if (serviceSupportedProtocols == null || serviceSupportedProtocols.Length == 0)
                return false;

            var commonProtocols = serviceSupportedProtocols.Intersect(this.registrarSupportedProtocols);
            if (!commonProtocols.Any())
                return false;
            chosenProtocol = commonProtocols.Max();
            return true;
        }
    }
}
