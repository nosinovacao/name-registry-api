using System;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;

namespace NAME.Registry.Services.Tests
{
    public class ProtocolNegotiatorTests
    {
        [Fact]
        [Trait("TestCategory", "Unit")]
        public void ChooseProtocolValidValue()
        {
            var registrarProtocols = new uint[] { 1u, 2u, 40u, 3u };
            var serviceProtocols = new uint[] { 1u, 50u, 2u, 10u, 3u };
            var loggerMock = new Mock<ILogger<ProtocolNegotiator>>();
            var negotiator = new ProtocolNegotiator(registrarProtocols, loggerMock.Object);

            Assert.True(negotiator.TryChooseProtocol(serviceProtocols, out uint chosenProtocol));
            Assert.Equal(3u, chosenProtocol);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void ChooseProtocolNoValidValue()
        {
            var registrarProtocols = new uint[] { 1u, 2u, 40u, 3u };
            var serviceProtocols = new uint[] { 50u, 10u };
            var loggerMock = new Mock<ILogger<ProtocolNegotiator>>();
            var negotiator = new ProtocolNegotiator(registrarProtocols, loggerMock.Object);

            Assert.False(negotiator.TryChooseProtocol(serviceProtocols, out uint chosenProtocol));
        }
    }
}
