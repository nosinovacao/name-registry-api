using Microsoft.Extensions.Logging;
using Moq;
using NAME.Registry.Domain;
using NAME.Registry.DTOs;
using NAME.Registry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace NAME.Registry.Services.Tests
{
    public class RegisteredServicesManagerTests
    {
        [Fact]
        [Trait("TestCategory", "Unit")]
        public void RegisterServiceNewService()
        {
            var serviceRepoMock = new Mock<IRegisteredServiceRepository>(MockBehavior.Strict);
            var sessionManagerMock = new Mock<IServiceSessionManager>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<RegisteredServicesManager>>();
            var mapperMock = new Mock<IMapper>();
            var serviceManager = new RegisteredServicesManager(loggerMock.Object, serviceRepoMock.Object, sessionManagerMock.Object, mapperMock.Object);

            var appName = "nPVR.DS.Rest.API";
            var appVersion = "1.8.0";
            var hostname = "ct-cloud-be1";
            var NAMEEndpoint = "/nPVR.DS.Rest.API/manifest";
            var NAMEPort = 80u;
            var NAMEVersion = "1.0.0";

            string generatedServiceId = Generator.RegisteredServiceId(hostname, NAMEEndpoint, NAMEPort, appName, appVersion, NAMEVersion);
            Guid serviceSessionGuid = Guid.NewGuid();

            serviceRepoMock
                .Setup(repo => repo.GetById(generatedServiceId))
                .Returns<RegisteredService>(null)
                .Verifiable();

            sessionManagerMock.Setup(manager => manager.CreateSession(It.IsAny<string>()))
                .Returns<string>(registeredServiceId =>
                {
                    return new ServiceSessionDTO()
                    {
                        Id = serviceSessionGuid.ToString(),
                        Bootstrapped = DateTime.UtcNow,
                        RegisteredServiceId = registeredServiceId
                    };
                })
                .Verifiable();

            serviceRepoMock.Setup(repo => repo.Insert(It.Is<RegisteredService>(s =>
                s.Id == generatedServiceId
                && s.AppName == appName
                && s.AppVersion == appVersion
                && s.Hostname == hostname
                && s.NAMEEndpoint == NAMEEndpoint
                && s.NAMEPort == NAMEPort
                && s.NAMEVersion == NAMEVersion
                && s.CurrentSessionId == serviceSessionGuid))).Verifiable();

            mapperMock.Setup(m => m.Map<RegisteredService, RegisteredServiceDTO>(It.IsAny<RegisteredService>()))
                .Returns<RegisteredService>((s) =>
                {
                    return new RegisteredServiceDTO()
                    {
                        Id = s.Id
                    };
                });

            var result = serviceManager.RegisterService(hostname, NAMEEndpoint, NAMEPort, appName, appVersion, NAMEVersion);

            Assert.Equal(generatedServiceId, result.Id);
            Assert.Equal(serviceSessionGuid.ToString(), result.CurrentSession.Id);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void RegisterServiceNewServiceNullNAMEPort()
        {
            var serviceRepoMock = new Mock<IRegisteredServiceRepository>(MockBehavior.Strict);
            var sessionManagerMock = new Mock<IServiceSessionManager>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<RegisteredServicesManager>>();
            var mapperMock = new Mock<IMapper>();
            var serviceManager = new RegisteredServicesManager(loggerMock.Object, serviceRepoMock.Object, sessionManagerMock.Object, mapperMock.Object);

            var appName = "nPVR.DS.Rest.API";
            var appVersion = "1.8.0";
            var hostname = "ct-cloud-be1";
            var NAMEEndpoint = "/nPVR.DS.Rest.API/manifest";
            uint? NAMEPort = null;
            var NAMEVersion = "1.0.0";

            string generatedServiceId = Generator.RegisteredServiceId(hostname, NAMEEndpoint, NAMEPort, appName, appVersion, NAMEVersion);
            Guid serviceSessionGuid = Guid.NewGuid();

            serviceRepoMock
                .Setup(repo => repo.GetById(generatedServiceId))
                .Returns<RegisteredService>(null)
                .Verifiable();

            sessionManagerMock.Setup(manager => manager.CreateSession(It.IsAny<string>()))
                .Returns<string>(registeredServiceId =>
                {
                    return new ServiceSessionDTO()
                    {
                        Id = serviceSessionGuid.ToString(),
                        Bootstrapped = DateTime.UtcNow,
                        RegisteredServiceId = registeredServiceId
                    };
                })
                .Verifiable();

            serviceRepoMock.Setup(repo => repo.Insert(It.Is<RegisteredService>(s =>
                s.Id == generatedServiceId
                && s.AppName == appName
                && s.AppVersion == appVersion
                && s.Hostname == hostname
                && s.NAMEEndpoint == NAMEEndpoint
                && s.NAMEPort == NAMEPort
                && s.NAMEVersion == NAMEVersion
                && s.CurrentSessionId == serviceSessionGuid))).Verifiable();

            mapperMock.Setup(m => m.Map<RegisteredService, RegisteredServiceDTO>(It.IsAny<RegisteredService>()))
                .Returns<RegisteredService>((s) =>
                {
                    return new RegisteredServiceDTO()
                    {
                        Id = s.Id,
                    };
                });

            var result = serviceManager.RegisterService(hostname, NAMEEndpoint, NAMEPort, appName, appVersion, NAMEVersion);

            Assert.Equal(generatedServiceId, result.Id);
            Assert.Equal(serviceSessionGuid.ToString(), result.CurrentSession.Id);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void RegisterServiceExistingService()
        {
            var serviceRepoMock = new Mock<IRegisteredServiceRepository>(MockBehavior.Strict);
            var sessionManagerMock = new Mock<IServiceSessionManager>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<RegisteredServicesManager>>();
            var mapperMock = new Mock<IMapper>();
            var serviceManager = new RegisteredServicesManager(loggerMock.Object, serviceRepoMock.Object, sessionManagerMock.Object, mapperMock.Object);

            var appName = "nPVR.DS.Rest.API";
            var appVersion = "1.8.0";
            var hostname = "ct-cloud-be1";
            var NAMEEndpoint = "/nPVR.DS.Rest.API/manifest";
            var NAMEPort = 80u;
            var NAMEVersion = "1.0.0";

            string generatedServiceId = Generator.RegisteredServiceId(hostname, NAMEEndpoint, NAMEPort, appName, appVersion, NAMEVersion);
            Guid serviceSessionGuid = Guid.NewGuid();

            serviceRepoMock
                .Setup(repo => repo.GetById(generatedServiceId))
                .Returns(new RegisteredService()
                {
                    Id = generatedServiceId,
                    AppName = appName,
                    AppVersion = appVersion,
                    Hostname = hostname,
                    NAMEEndpoint = NAMEEndpoint,
                    NAMEPort = NAMEPort,
                    NAMEVersion = NAMEVersion,
                })
                .Verifiable();

            sessionManagerMock.Setup(manager => manager.CreateSession(It.IsAny<string>()))
                .Returns<string>(registeredServiceId =>
                {
                    return new ServiceSessionDTO()
                    {
                        Id = serviceSessionGuid.ToString(),
                        Bootstrapped = DateTime.UtcNow,
                        RegisteredServiceId = registeredServiceId
                    };
                })
                .Verifiable();

            serviceRepoMock.Setup(repo => repo.Update(It.Is<RegisteredService>(s =>
                s.Id == generatedServiceId
                && s.AppName == appName
                && s.AppVersion == appVersion
                && s.Hostname == hostname
                && s.NAMEEndpoint == NAMEEndpoint
                && s.NAMEPort == NAMEPort
                && s.NAMEVersion == NAMEVersion
                && s.CurrentSessionId == serviceSessionGuid
                ))).Returns(true).Verifiable();

            mapperMock.Setup(m => m.Map<RegisteredService, RegisteredServiceDTO>(It.IsAny<RegisteredService>()))
                .Returns<RegisteredService>((s) =>
                {
                    return new RegisteredServiceDTO()
                    {
                        Id = s.Id
                    };
                });

            var result = serviceManager.RegisterService(hostname, NAMEEndpoint, NAMEPort, appName, appVersion, NAMEVersion);

            serviceRepoMock.Verify();
            sessionManagerMock.Verify();

            Assert.Equal(generatedServiceId, result.Id);
            Assert.Equal(serviceSessionGuid.ToString(), result.CurrentSession.Id);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void RegisterServiceExistingServiceNullNAMEPort()
        {
            var serviceRepoMock = new Mock<IRegisteredServiceRepository>(MockBehavior.Strict);
            var sessionManagerMock = new Mock<IServiceSessionManager>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<RegisteredServicesManager>>();
            var mapperMock = new Mock<IMapper>();
            var serviceManager = new RegisteredServicesManager(loggerMock.Object, serviceRepoMock.Object, sessionManagerMock.Object, mapperMock.Object);

            var appName = "nPVR.DS.Rest.API";
            var appVersion = "1.8.0";
            var hostname = "ct-cloud-be1";
            var NAMEEndpoint = "/nPVR.DS.Rest.API/manifest";
            uint? NAMEPort = null;
            var NAMEVersion = "1.0.0";

            string generatedServiceId = Generator.RegisteredServiceId(hostname, NAMEEndpoint, NAMEPort, appName, appVersion, NAMEVersion);
            Guid serviceSessionGuid = Guid.NewGuid();

            serviceRepoMock
                .Setup(repo => repo.GetById(generatedServiceId))
                .Returns(new RegisteredService()
                {
                    Id = generatedServiceId,
                    AppName = appName,
                    AppVersion = appVersion,
                    Hostname = hostname,
                    NAMEEndpoint = NAMEEndpoint,
                    NAMEPort = NAMEPort,
                    NAMEVersion = NAMEVersion,
                })
                .Verifiable();

            sessionManagerMock.Setup(manager => manager.CreateSession(It.IsAny<string>()))
                .Returns<string>(registeredServiceId =>
                {
                    return new ServiceSessionDTO()
                    {
                        Id = serviceSessionGuid.ToString(),
                        Bootstrapped = DateTime.UtcNow,
                        RegisteredServiceId = registeredServiceId
                    };
                })
                .Verifiable();

            serviceRepoMock.Setup(repo => repo.Update(It.Is<RegisteredService>(s =>
                s.Id == generatedServiceId
                && s.AppName == appName
                && s.AppVersion == appVersion
                && s.Hostname == hostname
                && s.NAMEEndpoint == NAMEEndpoint
                && s.NAMEPort == NAMEPort
                && s.NAMEVersion == NAMEVersion
                && s.CurrentSessionId == serviceSessionGuid
                ))).Returns(true).Verifiable();

            mapperMock.Setup(m => m.Map<RegisteredService, RegisteredServiceDTO>(It.IsAny<RegisteredService>()))
                .Returns<RegisteredService>((s) =>
                {
                    return new RegisteredServiceDTO()
                    {
                        Id = s.Id
                    };
                });

            var result = serviceManager.RegisterService(hostname, NAMEEndpoint, NAMEPort, appName, appVersion, NAMEVersion);

            serviceRepoMock.Verify();
            sessionManagerMock.Verify();

            Assert.Equal(generatedServiceId, result.Id);
            Assert.Equal(serviceSessionGuid.ToString(), result.CurrentSession.Id);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void GetAllWithoutLastPingFilter()
        {
            var serviceRepoMock = new Mock<IRegisteredServiceRepository>(MockBehavior.Strict);
            var sessionManagerMock = new Mock<IServiceSessionManager>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<RegisteredServicesManager>>();
            var mapperMock = new Mock<IMapper>();
            var serviceManager = new RegisteredServicesManager(loggerMock.Object, serviceRepoMock.Object, sessionManagerMock.Object, mapperMock.Object);

            var appName = "nPVR.DS.Rest.API";
            var appVersion = "1.8.0";
            var hostname = "ct-cloud-be1";
            var NAMEEndpoint = "/nPVR.DS.Rest.API/manifest";
            uint? NAMEPort = null;
            var NAMEVersion = "1.0.0";
            var currentSessionId = Guid.NewGuid();

            string generatedServiceId = Generator.RegisteredServiceId(hostname, NAMEEndpoint, NAMEPort, appName, appVersion, NAMEVersion);

            string hostnameFilter = "ct-cloud";
            string appNameFilter = "nPVR";
            string appVersionFilter = "1.";

            serviceRepoMock
                .Setup(repo => repo.GetAll(hostnameFilter, appNameFilter, appVersionFilter))
                .Returns(new List<RegisteredService>{
                    new RegisteredService()
                    {
                        Id = generatedServiceId,
                        AppName = appName,
                        AppVersion = appVersion,
                        Hostname = hostname,
                        NAMEEndpoint = NAMEEndpoint,
                        NAMEPort = NAMEPort,
                        NAMEVersion = NAMEVersion,
                        CurrentSessionId=currentSessionId
                    }
                })
                .Verifiable();

            var returnedSessionDto = new ServiceSessionDTO()
            {
                Id = currentSessionId.ToString(),
                Bootstrapped = DateTime.UtcNow,
                RegisteredServiceId = generatedServiceId.ToString(),
                LastPing = DateTime.UtcNow
            };

            sessionManagerMock.Setup(manager => manager.GetById(currentSessionId))
                .Returns(returnedSessionDto)
                .Verifiable();

            mapperMock.Setup(m => m.Map<RegisteredService, RegisteredServiceDTO>(It.IsAny<RegisteredService>()))
                .Returns<RegisteredService>((s) =>
                {
                    return new RegisteredServiceDTO()
                    {
                        Id = s.Id,
                        CurrentSession = returnedSessionDto
                    };
                });

            var result = serviceManager.GetAll(hostnameFilter, appNameFilter, appVersionFilter, null);

            serviceRepoMock.Verify();
            sessionManagerMock.Verify();

            Assert.Equal(1, result.Count());
            Assert.Equal(generatedServiceId, result.First().Id);
            Assert.Equal(currentSessionId.ToString(), result.First().CurrentSession.Id);
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void GetAllWithLastPingFilter()
        {
            var serviceRepoMock = new Mock<IRegisteredServiceRepository>(MockBehavior.Strict);
            var sessionManagerMock = new Mock<IServiceSessionManager>(MockBehavior.Strict);
            var loggerMock = new Mock<ILogger<RegisteredServicesManager>>();
            var mapperMock = new Mock<IMapper>();
            var serviceManager = new RegisteredServicesManager(loggerMock.Object, serviceRepoMock.Object, sessionManagerMock.Object, mapperMock.Object);

            var appName = "nPVR.DS.Rest.API";
            var appVersion = "1.8.0";
            var hostname = "ct-cloud-be1";
            var NAMEEndpoint = "/nPVR.DS.Rest.API/manifest";
            uint? NAMEPort = null;
            var NAMEVersion = "1.0.0";
            var currentSessionId = Guid.NewGuid();

            string generatedServiceId = Generator.RegisteredServiceId(hostname, NAMEEndpoint, NAMEPort, appName, appVersion, NAMEVersion);

            string hostnameFilter = "ct-cloud";
            string appNameFilter = "nPVR";
            string appVersionFilter = "1.";
            DateTime lastPingFilter = DateTime.UtcNow;

            serviceRepoMock
                .Setup(repo => repo.GetAll(hostnameFilter, appNameFilter, appVersionFilter))
                .Returns(new List<RegisteredService>{
                    new RegisteredService()
                    {
                        Id = generatedServiceId,
                        AppName = appName,
                        AppVersion = appVersion,
                        Hostname = hostname,
                        NAMEEndpoint = NAMEEndpoint,
                        NAMEPort = NAMEPort,
                        NAMEVersion = NAMEVersion,
                        CurrentSessionId=currentSessionId
                    }
                })
                .Verifiable();

            var returnedSessionDto = new ServiceSessionDTO()
            {
                Id = currentSessionId.ToString(),
                Bootstrapped = DateTime.UtcNow,
                RegisteredServiceId = generatedServiceId.ToString(),
                LastPing = lastPingFilter.Subtract(TimeSpan.FromMinutes(1))
            };

            sessionManagerMock.Setup(manager => manager.GetById(currentSessionId))
                .Returns(returnedSessionDto)
                .Verifiable();

            mapperMock.Setup(m => m.Map<RegisteredService, RegisteredServiceDTO>(It.IsAny<RegisteredService>()))
                .Returns<RegisteredService>((s) =>
                {
                    return new RegisteredServiceDTO()
                    {
                        Id = s.Id,
                        CurrentSession = returnedSessionDto
                    };
                });

            var result = serviceManager.GetAll(hostnameFilter, appNameFilter, appVersionFilter, lastPingFilter);

            serviceRepoMock.Verify();
            sessionManagerMock.Verify();

            Assert.Equal(0, result.Count());
        }
    }
}
