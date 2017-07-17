using Microsoft.Extensions.Logging;
using Moq;
using NAME.Registry.Domain;
using NAME.Registry.DTOs;
using NAME.Registry.Exceptions;
using NAME.Registry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace NAME.Registry.Services.Tests
{
    public class ServiceSessionManagerTests
    {
        [Fact]
        [Trait("TestCategory", "Unit")]
        public void AddManifestSnapshotValidValues()
        {
            var loggerMock = new Mock<ILogger<ServiceSessionManager>>();
            var sessionRepoMock = new Mock<IServiceSessionRepository>(MockBehavior.Strict);
            var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
            var sessionManager = new ServiceSessionManager(loggerMock.Object, sessionRepoMock.Object, mapperMock.Object);

            var manifest = "teste_manifest";

            var sessionId = Guid.NewGuid();
            var registeredServiceId = "teste_service";
            var session = new ServiceSession
            {
                Id = sessionId,
                RegisteredServiceId = registeredServiceId
            };

            sessionRepoMock
                .Setup(r => r.GetById(It.Is<Guid>(g => g == sessionId)))
                .Returns(new ServiceSession()
                {
                    Id = sessionId,
                    RegisteredServiceId = registeredServiceId
                }).Verifiable();

            sessionRepoMock
                .Setup(r => r.Update(It.Is<ServiceSession>(s =>
                    s.ManifestSnapshots[0].Manifest == manifest
                    && s.Id == sessionId
                    && s.RegisteredServiceId == registeredServiceId)
                )).Returns(true)
                .Verifiable();

            mapperMock
                .Setup(m => m.Map<ManifestSnapshot, ManifestSnapshotDTO>(It.Is<ManifestSnapshot>(s =>
                    s.Manifest == manifest
                ))).Returns(new ManifestSnapshotDTO
                {
                    Manifest = manifest
                }).Verifiable();

            var result = sessionManager.AddManifestSnapshot(sessionId, manifest);

            Assert.Equal(manifest, result.Manifest);

            sessionRepoMock.Verify();
            mapperMock.Verify();
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void AddManifestSnapshotNonExistent()
        {
            var loggerMock = new Mock<ILogger<ServiceSessionManager>>();
            var sessionRepoMock = new Mock<IServiceSessionRepository>(MockBehavior.Strict);
            var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
            var sessionManager = new ServiceSessionManager(loggerMock.Object, sessionRepoMock.Object, mapperMock.Object);
            
            var sessionId = Guid.NewGuid();
            var manifest = "test_manifest";

            sessionRepoMock
                .Setup(r => r.GetById(It.Is<Guid>(g => g == sessionId)))
                .Returns< ServiceSession>(null)
                .Verifiable();

            Assert.Throws<EntityNotFoundException>(() => sessionManager.AddManifestSnapshot(sessionId, manifest));
            
            sessionRepoMock.Verify();
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void CreateSessionValidValue()
        {
            var loggerMock = new Mock<ILogger<ServiceSessionManager>>();
            var sessionRepoMock = new Mock<IServiceSessionRepository>(MockBehavior.Strict);
            var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
            var sessionManager = new ServiceSessionManager(loggerMock.Object, sessionRepoMock.Object, mapperMock.Object);

            var serviceId = "service_id_test";

            sessionRepoMock
                .Setup(r => r.Insert(It.Is<ServiceSession>(s => s.RegisteredServiceId == serviceId)))
                .Verifiable();

            sessionRepoMock
                .Setup(r => r.GetAllInRegisteredService(It.Is<string>(s => s == serviceId), false))
                .Returns(Enumerable.Empty<ServiceSession>)
                .Verifiable();

            mapperMock
                .Setup(m => m.Map<ServiceSession, ServiceSessionDTO>(It.Is<ServiceSession>(s =>
                    s.RegisteredServiceId == serviceId)))
                .Returns(new ServiceSessionDTO { RegisteredServiceId = serviceId })
                .Verifiable();

            var result = sessionManager.CreateSession(serviceId);

            Assert.Equal(serviceId, result.RegisteredServiceId);
            sessionRepoMock.Verify();
            mapperMock.Verify();
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void GetSessionExistingId()
        {
            var loggerMock = new Mock<ILogger<ServiceSessionManager>>();
            var sessionRepoMock = new Mock<IServiceSessionRepository>(MockBehavior.Strict);
            var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
            var sessionManager = new ServiceSessionManager(loggerMock.Object, sessionRepoMock.Object, mapperMock.Object);

            var manifest = "test_manifest";
            var dateAndTime = DateTime.UtcNow;
            var manifestSnapshot = new ManifestSnapshot
            {
                Manifest = manifest,
                DateAndTime = dateAndTime
            };

            var sessionId = Guid.NewGuid();

            var serviceSession = new ServiceSession()
            {
                Id = sessionId,
                ManifestSnapshots = new List<ManifestSnapshot>
                {
                    manifestSnapshot
                }
            };

            sessionRepoMock
                .Setup(r => r.GetById(sessionId))
                .Returns(serviceSession)
                .Verifiable();

            mapperMock
                .Setup(m => m.Map<ServiceSession, ServiceSessionDTO>(It.Is<ServiceSession>(s => s == serviceSession)))
                .Returns(new ServiceSessionDTO
                {
                    Id = sessionId.ToString()
                }).Verifiable();

            mapperMock
                .Setup(m => m.Map<ManifestSnapshot, ManifestSnapshotDTO>(It.Is<ManifestSnapshot>(s => s == manifestSnapshot)))
                .Returns(new ManifestSnapshotDTO
                {
                    Manifest = manifest,
                    DateAndTime = dateAndTime
                }).Verifiable();


            var result = sessionManager.GetById(sessionId);

            Assert.Equal(serviceSession.Id.ToString(), result.Id);
            Assert.Equal(manifestSnapshot.Manifest, result.LastManifestSnapshot.Manifest);
            Assert.Equal(manifestSnapshot.DateAndTime, result.LastManifestSnapshot.DateAndTime);
            sessionRepoMock.Verify();
            mapperMock.Verify();
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void GetSessionNonExistingId()
        {
            var loggerMock = new Mock<ILogger<ServiceSessionManager>>();
            var sessionRepoMock = new Mock<IServiceSessionRepository>(MockBehavior.Strict);
            var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
            var sessionManager = new ServiceSessionManager(loggerMock.Object, sessionRepoMock.Object, mapperMock.Object);

            var sessionId = Guid.NewGuid();

            sessionRepoMock
                .Setup(r => r.GetById(sessionId))
                .Returns<ServiceSession>(null)
                .Verifiable();

            var result = sessionManager.GetById(sessionId);

            Assert.Null(result);
            sessionRepoMock.Verify();
            mapperMock.Verify();
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void PingSessionSuccess()
        {
            var loggerMock = new Mock<ILogger<ServiceSessionManager>>();
            var sessionRepoMock = new Mock<IServiceSessionRepository>(MockBehavior.Strict);
            var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
            var sessionManager = new ServiceSessionManager(loggerMock.Object, sessionRepoMock.Object, mapperMock.Object);

            var sessionId = Guid.NewGuid();
            var registeredServiceId = "teste_service";

            var serviceSession = new ServiceSession()
            {
                Id = sessionId,
                LastPing = null,
                RegisteredServiceId = registeredServiceId
            };

            sessionRepoMock
                .Setup(r => r.GetById(sessionId))
                .Returns(serviceSession)
                .Verifiable();

            sessionRepoMock
                .Setup(r => r.Update(It.Is<ServiceSession>(s =>
                    s.Id == sessionId
                    && s.RegisteredServiceId == registeredServiceId
                    && s.LastPing != null)))
                .Returns(true)
                .Verifiable();

            sessionManager.PingSession(sessionId);

            sessionRepoMock.Verify();
            mapperMock.Verify();
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void PingSessionUpdateFailure()
        {
            var loggerMock = new Mock<ILogger<ServiceSessionManager>>();
            var sessionRepoMock = new Mock<IServiceSessionRepository>(MockBehavior.Strict);
            var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
            var sessionManager = new ServiceSessionManager(loggerMock.Object, sessionRepoMock.Object, mapperMock.Object);

            var sessionId = Guid.NewGuid();
            var registeredServiceId = "teste_service";
            var serviceSession = new ServiceSession()
            {
                Id = sessionId,
                LastPing = null,
                RegisteredServiceId = registeredServiceId
            };

            sessionRepoMock
                .Setup(r => r.GetById(sessionId))
                .Returns(serviceSession)
                .Verifiable();

            sessionRepoMock
                .Setup(r => r.Update(serviceSession))
                .Returns(false)
                .Verifiable();

            var thrownException = Assert.Throws<RepositoryEntityOperationFailedException>(() =>
            {
                sessionManager.PingSession(sessionId);
            });

            Assert.IsType<ServiceSession>(thrownException.Entity);
            Assert.Equal(serviceSession, thrownException.Entity);

            sessionRepoMock.Verify();
            mapperMock.Verify();
        }

        [Fact]
        [Trait("TestCategory", "Unit")]
        public void PingSessionNonexistant()
        {
            var loggerMock = new Mock<ILogger<ServiceSessionManager>>();
            var sessionRepoMock = new Mock<IServiceSessionRepository>(MockBehavior.Strict);
            var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
            var sessionManager = new ServiceSessionManager(loggerMock.Object, sessionRepoMock.Object, mapperMock.Object);

            var sessionId = Guid.NewGuid();

            sessionRepoMock
                .Setup(r => r.GetById(sessionId))
                .Returns<ServiceSession>(null)
                .Verifiable();

            var thrownException = Assert.Throws<EntityNotFoundException>(() =>
            {
                sessionManager.PingSession(sessionId);
            });
            
            sessionRepoMock.Verify();
        }
    }
}
