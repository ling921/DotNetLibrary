using LingDev.EntityFrameworkCore.Audit.Models;

namespace LingDev.EntityFrameworkCore.Audit.Attributes
{
    /// <summary>
    /// Indicates the class allowed anonymous auditing of specific database operations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AnonymousAuditingAttribute : Attribute
    {
        /// <summary>
        /// The database operations allowed anonymous auditing.
        /// </summary>
        public DbOperation Operation { get; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="operation">The database operations allowed anonymous auditing.</param>
        public AnonymousAuditingAttribute(DbOperation operation)
        {
            Operation = operation;
        }
    }
}
