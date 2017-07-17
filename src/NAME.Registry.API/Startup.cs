using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using LiteDB;
using NAME.Registry.Domain;
using NAME.Registry.Interfaces;
using NAME.Registry.Repositories.LiteDB;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using FluentValidation.AspNetCore;
using NAME.AspNetCore;
using NAME.Registry.API.Utils;
using NAME.Registry.Services;
using NAME.Core;
using NAME.Registry.API.Validation;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace NAME.Registry.API
{
    /// <summary>
    /// The ASP.Net startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env">The env.</param>
        public Startup(IHostingEnvironment env)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();

            this.environment = env;
        }

        private IHostingEnvironment environment;

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfigurationRoot Configuration { get; }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services
                .AddMvc()
                .AddJsonOptions(js =>
                {
                    var resolver = js.SerializerSettings.ContractResolver;
                    if (resolver != null)
                    {
                        if (resolver is DefaultContractResolver res)
                            res.NamingStrategy = null;
                    }
                })
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<BootstrapDTOValidator>());

            // Database
            services.AddSingleton(s => new LiteDatabase(this.Configuration.GetConnectionString("RegistryLiteDB")));
            services.AddSingleton(s => s.GetRequiredService<LiteDatabase>().GetCollection<RegisteredService>(nameof(RegisteredService)));
            services.AddSingleton(s => s.GetRequiredService<LiteDatabase>().GetCollection<ServiceSession>(nameof(ServiceSession)));

            // Repos
            services.AddSingleton<IRegisteredServiceRepository, RegisteredServiceRepository>();
            services.AddSingleton<IServiceSessionRepository, ServiceSessionRepository>();

            // Services
            services.AddSingleton<IRegisteredServiceManager, RegisteredServicesManager>();
            services.AddSingleton<IServiceSessionManager, ServiceSessionManager>();
            services.AddSingleton<IProtocolNegotiator>(provider => new ProtocolNegotiator(Constants.REGISTRY_SUPPORTED_PROTOCOL_VERSIONS, provider.GetRequiredService<ILogger<IProtocolNegotiator>>()));

            // Utils
            services.AddSingleton<IMapper, ExpressMapperMapper>();

            services.AddSwaggerGen(c =>
            {
                Assembly a = typeof(Startup).GetTypeInfo().Assembly;
                c.SwaggerDoc("v1", new Info { Title = a.GetName().Name, Version = a.GetName().Version.ToString() });
                c.DescribeAllEnumsAsStrings();
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "NAME.Registry.API.xml");
                c.IncludeXmlComments(xmlPath);
            });

            services.AddCors();
        }

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var nlogConfigFile = 
            loggerFactory.AddNLog();
            var loggingConfig = new NLog.Config.XmlLoggingConfiguration(env.ContentRootPath + "/nlog.config");

            var logsDir = this.Configuration.GetSection("Logging").GetValue<string>("NLogLogsDir");
            if (!string.IsNullOrEmpty(logsDir))
                loggingConfig.Variables["LogDir"] = logsDir;

            loggerFactory.ConfigureNLog(loggingConfig);
            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseCors(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            // NAME middleware should be registered first so that the header is set before any other middleware has a change to send a response
            app.UseNAME(config =>
            {
                Assembly a = typeof(Startup).GetTypeInfo().Assembly;
                config.APIName = a.GetName().Name;
                config.APIVersion = a.GetName().Version.ToString();
                config.DependenciesFilePath = env.ContentRootPath + @"/dependencies.json";
            });

            app.Use((context, next) =>
            {
                if (context.Request.Headers.TryGetValue("X-Forwarded-Path", out StringValues values) && values.Count > 0 && !string.IsNullOrWhiteSpace(values[0]))
                {
                    context.Request.PathBase = values[0];
                }
                return next();
            });

            app.UseMvc();
            app.UseSwagger(action =>
            {
            });

            app.UseSwaggerUi(c =>
            {
                Assembly a = typeof(Startup).GetTypeInfo().Assembly;
                c.SwaggerEndpoint("/swagger/v1/swagger.json", a.GetName().Name);
                c.SwaggerEndpoint("/name/swagger/v1/swagger.json", "/name Reverse proxy");
                c.SupportedSubmitMethods(new string[] { "post", "get", "put", "delete", "patch", "head" });
            });
        }
    }
}
