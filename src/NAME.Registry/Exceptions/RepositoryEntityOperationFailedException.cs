using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAME.Registry.Exceptions
{
    /// <summary>
    /// Provides an exception for when a repository operation fails.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class RepositoryEntityOperationFailedException : Exception
    {
        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        public object Entity { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryEntityOperationFailedException"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public RepositoryEntityOperationFailedException(object entity)
        {
            this.Entity = entity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryEntityOperationFailedException"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="message">The message.</param>
        public RepositoryEntityOperationFailedException(object entity, string message)
            : base(message)
        {
            this.Entity = entity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryEntityOperationFailedException"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public RepositoryEntityOperationFailedException(object entity, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Entity = entity;
        }
    }
}
