using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NAME.Core;
using NAME.Core.DTOs;
using NAME.DTOs;
using NAME.DummyRegistryService.Util;
using NAME.Registry.API.Utils;
using NAME.Registry.Domain;
using NAME.Registry.Exceptions;
using NAME.Registry.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Net;

namespace NAME.Registry.API.Controllers
{
    /// <summary>
    /// Provides endpoints for the Service resource.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/v1/registrar")]
    public class RegistrarController : Controller
    {
        private ILogger<RegistrarController> logger;
        private IRegisteredServiceManager serviceManager;
        private IProtocolNegotiator protocolNegotiator;
        private IServiceSessionManager sessionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrarController" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceManager">The service repository.</param>
        /// <param name="protocolNegotiator">The protocol negotiator.</param>
        /// <param name="sessionManager">The session manager.</param>
        public RegistrarController(ILogger<RegistrarController> logger, IRegisteredServiceManager serviceManager, IProtocolNegotiator protocolNegotiator, IServiceSessionManager sessionManager)
        {
            this.logger = logger;
            this.serviceManager = serviceManager;
            this.protocolNegotiator = protocolNegotiator;
            this.sessionManager = sessionManager;
        }

        /// <summary>
        /// Bootstraps the service for the requesting application.
        /// </summary>
        /// <param name="serviceDto">The service.</param>
        /// <returns>Returns the action result.</returns>
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(BootstrapResultDto))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Happens when the request payload is invalid.")]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Description = "Happens when the service did not send an acceptable protocol.")]
        public IActionResult Bootstrap([FromBody]BootstrapDTO serviceDto)
        {
            if (!this.ModelState.IsValid)
            {
                this.logger.LogWarning($"Received a request for {nameof(this.Bootstrap)} with an invalid DTO. {this.ModelState.ErrorCount} errors.");
                return this.BadRequest(this.ModelState);
            }
            if (!this.protocolNegotiator.TryChooseProtocol(serviceDto.SupportedProtocols, out uint chosenProtocol))
                return new StatusCodeResult((int)HttpStatusCode.Conflict);

            var session = this.serviceManager.RegisterService(serviceDto.Hostname, serviceDto.NAMEEndpoint, serviceDto.NAMEPort, serviceDto.AppName, serviceDto.AppVersion, serviceDto.NAMEVersion);
            return this.Ok(new BootstrapResultDto
            {
                Protocol = chosenProtocol,
                SessionId = session.CurrentSession.Id
            });
        }

        /// <summary>
        /// Register a new manifest snapshot for the specified sessionId.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="manifestDTO">The manifest dto.</param>
        /// <returns>Returns the action result.</returns>
        [HttpPost]
        [Route("{sessionId}/manifest")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Happens when the session could not be found.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Happens when the request payload is invalid.")]
        public IActionResult ManifestSnapshot(string sessionId, [FromBody]SendManifestDTO manifestDTO)
        {
            if (!Guid.TryParse(sessionId, out Guid sessionGuid))
            {
                this.logger.LogWarning($"Received a request for {nameof(this.ManifestSnapshot)} with an invalid {nameof(sessionId)}.");
                return this.NotFound();
            }

            if (!this.ModelState.IsValid)
            {
                this.logger.LogWarning($"Received a request for {nameof(this.ManifestSnapshot)} with an invalid DTO. {this.ModelState.ErrorCount} errors.");
                return this.BadRequest(this.ModelState);
            }

            return this.GuardNotFoundException(() =>
            {
                this.sessionManager.AddManifestSnapshot(sessionGuid, manifestDTO.Manifest);
            });
        }

        /// <summary>
        /// Pings the specified session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>Returns the action result.</returns>
        [HttpHead]
        [Route("{sessionId}")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Happens when the session could not be found.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Happens when the request payload is invalid.")]
        public IActionResult Ping(string sessionId)
        {
            if (!Guid.TryParse(sessionId, out Guid sessionGuid))
            {
                this.logger.LogWarning($"Received a request for {nameof(this.ManifestSnapshot)} with an invalid {nameof(sessionId)}.");
                return this.NotFound();
            }

            return this.GuardNotFoundException(() =>
            {
                this.sessionManager.PingSession(sessionGuid);
            });
        }
    }
}
