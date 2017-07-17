using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NAME.Registry.DTOs;
using NAME.Registry.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using NAME.Registry.API.Utils;
using NAME.DummyRegistryService.Util;

namespace NAME.Registry.API.Controllers
{
    /// <summary>
    /// Provides endpoints for the Service resource.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/v1/sessions")]
    public class SessionsController : Controller
    {
        private ILogger<SessionsController> logger;
        private IServiceSessionManager sessionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="sessionManager">The service manager.</param>
        public SessionsController(ILogger<SessionsController> logger, IServiceSessionManager sessionManager)
        {
            this.logger = logger;
            this.sessionManager = sessionManager;
        }

        /// <summary>
        /// Gets all the <see cref="ServiceSessionDTO" />s of a specific service.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <returns>
        /// Returns the action result.
        /// </returns>
        [HttpGet]
        [Route("/api/v1/services/{serviceId}/sessions")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(IEnumerable<ServiceSessionDTO>))]
        public IActionResult GetAllForRegisteredService(string serviceId)
        {
            return this.Ok(this.sessionManager.GetAllInRegisteredService(serviceId, true));
        }


        /// <summary>
        /// Gets the specified <see cref="ServiceSessionDTO" />.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>
        /// Returns the action result.
        /// </returns>
        [HttpGet]
        [Route("{sessionId}")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(ServiceSessionDTO))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public IActionResult GetById(string sessionId)
        {
            if (!Guid.TryParse(sessionId, out Guid sessionGuid))
            {
                this.logger.LogWarning($"Received a request for {nameof(this.GetById)} with an invalid {nameof(sessionId)}.");
                return this.NotFound();
            }
            var session = this.sessionManager.GetById(sessionGuid);

            if (session == null)
                return this.NotFound();

            return this.Ok(session);
        }


        /// <summary>
        /// Gets all the <see cref="ManifestSnapshotDTO" />s of a specific session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>
        /// Returns the action result.
        /// </returns>
        [HttpGet]
        [Route("{sessionId}/snapshots")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(IEnumerable<ManifestSnapshotDTO>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public IActionResult GetAllManifestSnapshotsForSession([FromRoute]string sessionId)
        {
            if (!Guid.TryParse(sessionId, out Guid sessionGuid))
            {
                this.logger.LogWarning($"Received a request for {nameof(this.GetById)} with an invalid {nameof(sessionId)}.");
                return this.NotFound();
            }

            return this.GuardNotFoundException(
                () => this.sessionManager.GetAllSnapshots(sessionGuid));
        }

    }
}
