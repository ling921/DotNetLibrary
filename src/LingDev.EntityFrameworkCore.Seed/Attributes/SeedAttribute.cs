namespace LingDev.EntityFrameworkCore.Seed.Attributes;

/// <summary>
/// Indicates the class allowed anonymous auditing of specific database operations.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class SeedAttribute : Attribute
{
    /// <summary>
    /// The seed data file name.
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public SeedAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="fileName">The seed data file name.</param>
    public SeedAttribute(string? fileName)
    {
        FileName = fileName;
    }
}
