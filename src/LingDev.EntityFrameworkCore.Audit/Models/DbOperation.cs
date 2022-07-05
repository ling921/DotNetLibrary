namespace LingDev.EntityFrameworkCore.Audit.Models
{
    /// <summary>
    /// Enum that specifies database operation.
    /// </summary>
    [Flags]
    public enum DbOperation : byte
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,

        /// <summary>
        /// Read.
        /// </summary>
        Read = 1 << 0,

        /// <summary>
        /// Create.
        /// </summary>
        Create = 1 << 1,

        /// <summary>
        /// Update.
        /// </summary>
        Update = 1 << 2,

        /// <summary>
        /// Delete.
        /// </summary>
        Delete = 1 << 3,

        /// <summary>
        /// Create, Update and Delete.
        /// </summary>
        CUD = Create | Update | Delete,

        /// <summary>
        /// Create, Read, Update and Delete.
        /// </summary>
        CRUD = CUD | Read,
    }
}
