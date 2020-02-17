namespace App.Domain.Core.Models
{
    /// <summary>
    /// Base class for all entities
    /// </summary>
    public class EntityBase
    {
        /// <summary>
        /// Unique entity identifier
        /// </summary>
        public virtual int Id { get; set; }
    }
}