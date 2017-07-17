using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NAME.Core.DTOs;
using NAME.DTOs;
using NAME.Registry.Domain;
using NAME.Registry.DTOs;
using NAME.Registry.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace NAME.Registry.API.Controllers
{
    /// <summary>
    /// Provides endpoints for the Service resource.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/v1/services")]
    public class ServicesController : Controller
    {
        private ILogger<ServicesController> logger;
        private IRegisteredServiceManager serviceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceManager">The service manager.</param>
        public ServicesController(ILogger<ServicesController> logger, IRegisteredServiceManager serviceManager)
        {
            this.logger = logger;
            this.serviceManager = serviceManager;
        }

        /// <summary>
        /// Gets all the <see cref="RegisteredServiceDTO" />s.
        /// </summary>
        /// <param name="hostname">The hostname.</param>
        /// <param name="appName">The name.</param>
        /// <param name="appVersion">The version.</param>
        /// <param name="lastPingLowerThreshold">The last ping lower threshold.</param>
        /// <returns>
        /// Returns the action result.
        /// </returns>
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(IEnumerable<RegisteredServiceDTO>))]
        public IActionResult GetAll([FromQuery]string hostname = null, [FromQuery]string appName = null, [FromQuery]string appVersion = null, [FromQuery]DateTime? lastPingLowerThreshold = null)
        {
            return this.Ok(this.serviceManager.GetAll(hostname, appName, appVersion, lastPingLowerThreshold));
        }

        /// <summary>
        /// Gets the specified service identifier.
        /// </summary>
        /// <param name="serviceId">The service identifier.</param>
        /// <returns>Returns the action result.</returns>
        [HttpGet]
        [Route("{serviceId}")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(RegisteredServiceDTO))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public IActionResult Get([FromRoute]string serviceId)
        {
            var result = this.serviceManager.GetByID(serviceId);

            if (result == null)
                return this.NotFound();

            return this.Ok(result);
        }

    }
}
