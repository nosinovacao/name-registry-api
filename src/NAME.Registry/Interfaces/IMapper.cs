namespace NAME.Registry.Interfaces
{
    /// <summary>
    /// Represents a mechanism to map objects to other types.
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// Maps the specified object into another type.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TDestination">The type of the destination object.</typeparam>
        /// <param name="source">The source object.</param>
        /// <returns>Returns the mapped object in the specified type.</returns>
        TDestination Map<TSource, TDestination>(TSource source);

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
        TDestination Map<TSource, TDestination>(TDestination destination, TSource source);
    }
}
