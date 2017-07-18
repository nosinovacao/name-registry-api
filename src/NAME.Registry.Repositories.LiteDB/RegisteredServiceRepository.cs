using LiteDB;
using NAME.Registry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NAME.Registry.Domain;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;

namespace NAME.Registry.Repositories.LiteDB
{
    /// <summary>
    /// Provides a mechanism to access the service repository in the a <see cref="LiteDB"/> database.
    /// </summary>
    /// <seealso cref="NAME.Registry.Interfaces.IRegisteredServiceRepository" />
    public class RegisteredServiceRepository : IRegisteredServiceRepository
    {
        private LiteCollection<RegisteredService> serviceCollection;
        private ILogger<RegisteredServiceRepository> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredServiceRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceCollection">The service collection.</param>
        public RegisteredServiceRepository(ILogger<RegisteredServiceRepository> logger, LiteCollection<RegisteredService> serviceCollection)
        {
            this.logger = logger;
            this.serviceCollection = serviceCollection;
            this.serviceCollection.EnsureIndex(s => s.Id, true);
        }

        /// <summary>
        /// Gets all the services.
        /// </summary>
        /// <param name="hostnameFilter">The hostname filter.</param>
        /// <param name="appNameFilter">The application name filter.</param>
        /// <param name="appVersionFilter">The application version filter.</param>
        /// <returns>
        /// Returns all the services.
        /// </returns>
        public IEnumerable<RegisteredService> GetAll(string hostnameFilter = null, string appNameFilter = null, string appVersionFilter = null)
        {
            List<Query> queries = new List<Query>();
            if (!string.IsNullOrEmpty(hostnameFilter))
                queries.Add(Query.Where(nameof(RegisteredService.Hostname), v => v.AsString.ToLower().Contains(hostnameFilter.ToLower())));
            if (!string.IsNullOrEmpty(appNameFilter))
                queries.Add(Query.Where(nameof(RegisteredService.AppName), v => v.AsString.ToLower().Contains(appNameFilter.ToLower())));
            if (!string.IsNullOrEmpty(appVersionFilter))
                queries.Add(Query.Contains(nameof(RegisteredService.AppVersion), appVersionFilter));
            
            if (queries.Count == 0)
                return this.serviceCollection.FindAll();

            Query finalQuery;
            finalQuery = queries[0];
            for (int i = 1; i < queries.Count; i++)
            {
                finalQuery = Query.And(finalQuery, queries[1]);
            }

            return this.serviceCollection.Find(finalQuery);
        }

        /// <summary>
        /// Gets the Service by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// Returns the service, or null if it does not exist.
        /// </returns>
        public RegisteredService GetById(string id)
        {
            return this.serviceCollection.FindById(id);
        }

        /// <summary>
        /// Inserts the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        public void Insert(RegisteredService service)
        {
            this.serviceCollection.Insert(service);
        }

        /// <summary>
        /// Removes the specified Service.
        /// </summary>
        /// <param name="id">The Service.</param>
        public void Remove(string id)
        {
            this.serviceCollection.Delete(id);
        }

        /// <summary>
        /// Checks if the specified service exists by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// Returns true if the service exists.
        /// </returns>
        public bool Exists(string id)
        {
            return this.serviceCollection.Exists(s => s.Id == id);
        }

        /// <summary>
        /// Updates the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>
        /// Returns true if a service is updated.
        /// </returns>
        public bool Update(RegisteredService service)
        {
            return this.serviceCollection.Update(service);
        }
    }
}
