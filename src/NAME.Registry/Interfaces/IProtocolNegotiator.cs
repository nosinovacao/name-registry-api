using System;
using System.Collections.Generic;
using System.Text;

namespace NAME.Registry.Interfaces
{
    /// <summary>
    /// Provides mechanisms to negotiate the protocol between the registrar and the service.
    /// </summary>
    public interface IProtocolNegotiator
    {
        /// <summary>
        /// Tries to choose a protocol to be used between the registrar and a specific client.
        /// </summary>
        /// <param name="serviceSupportedProtocols">The protocols supported by the client service.</param>
        /// <param name="chosenProtocol">The chosen protocol.</param>
        /// <returns>Returns <c>true</c> if a protocol is chosen successfuly. <c>false</c> if not.</returns>
        bool TryChooseProtocol(uint[] serviceSupportedProtocols, out uint chosenProtocol);
    }
}
