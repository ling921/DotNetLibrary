namespace LingDev.EntityFrameworkCore.Seed
{
    /// <summary>
    /// Options for configuring data seeds.
    /// </summary>
    public class SeedOptions
    {
        /// <summary>
        /// The path to the json files of the seed data, default to "seeds".
        /// </summary>
        public string Path { get; set; } = "seeds";

        /// <summary>
        /// Whether to apply the migration, default to <see langword="true"/>.
        /// <para>
        /// 
        /// </para>
        /// </summary>
        public bool ApplyMigration { get; set; } = true;

        public bool ApplySeed { get; set; } = true;
    }
}
