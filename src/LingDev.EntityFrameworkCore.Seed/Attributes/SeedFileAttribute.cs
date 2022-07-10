namespace LingDev.EntityFrameworkCore.Seed.Attributes;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class SeedFileAttribute : Attribute
{
    /// <summary>
    /// 
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    public SeedFileAttribute(string fileName)
    {
        FileName = fileName;
    }
}
