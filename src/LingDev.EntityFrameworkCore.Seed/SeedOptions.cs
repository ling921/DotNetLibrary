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
        /// </summary>
        public bool ApplyMigration { get; set; } = true;

        /// <summary>
        /// Whether to apply the seed data, default to <see langword="true"/>.
        /// </summary>
        public bool ApplySeed { get; set; } = true;

        internal Type[] ModelTypes { get; set; } = Array.Empty<Type>();
    }
}
