using ExpressMapper;
using ExpressMapper.Extensions;
using NAME.Registry.Domain;
using NAME.Registry.DTOs;
using NAME.Registry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NAME.Registry.API.Utils
{
    /// <summary>
    /// Provides a mechanism to map objects using <see cref="ExpressMapper"/>.
    /// </summary>
    /// <seealso cref="NAME.Registry.Interfaces.IMapper" />
    public class ExpressMapperMapper : IMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressMapperMapper"/> class.
        /// </summary>
        public ExpressMapperMapper()
        {
            Mapper.RegisterCustom<string, Guid>(input => Guid.Parse(input));
            Mapper.RegisterCustom<Guid, string>(input => input.ToString());
            Mapper.Register<RegisteredService, RegisteredServiceDTO>();
            //Mapper.Register<ServiceSession, ServiceSessionDTO>();
            Mapper.Register<ManifestSnapshot, ManifestSnapshotDTO>();
            Mapper.RegisterCustom<ServiceSession, ServiceSessionDTO>((session) =>
            {
                return new ServiceSessionDTO()
                {
                    Bootstrapped = session.Bootstrapped,
                    Id = session.Id.ToString(),
                    Invalidated = session.Invalidated,
                    LastManifestSnapshot = Mapper.Map<ManifestSnapshot, ManifestSnapshotDTO>(
                        session.ManifestSnapshots.OrderBy(s => s.DateAndTime).LastOrDefault()),
                    ManifestSnapshotCount = session.ManifestSnapshots.Count,
                    LastPing = session.LastPing,
                    RegisteredServiceId = session.RegisteredServiceId
                };
            });
            Mapper.Compile();
            Mapper.PrecompileCollection<IEnumerable<RegisteredService>, IEnumerable<RegisteredServiceDTO>>();
            Mapper.PrecompileCollection<IEnumerable<ServiceSession>, IEnumerable<ServiceSessionDTO>>();
            Mapper.PrecompileCollection<IEnumerable<ManifestSnapshot>, IEnumerable<ManifestSnapshotDTO>>();
        }

        /// <summary>
        /// Maps the specified object into another type.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TDestination">The type of the destination object.</typeparam>
        /// <param name="source">The source object.</param>
        /// <returns>
        /// Returns the mapped object in the specified type.
        /// </returns>
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return source.Map<TSource, TDestination>();
        }

        /// <summary>
        /// Maps the specified object values to another object.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TDestination">The type of the destination object.</typeparam>
        /// <param name="destination">The destination.</param>
        /// <param name="source">The source object.</param>
        /// <returns>
        /// Returns the object with the new values.
        /// </returns>
        public TDestination Map<TSource, TDestination>(TDestination destination, TSource source)
        {
            return source.Map(destination);
        }
    }
}
