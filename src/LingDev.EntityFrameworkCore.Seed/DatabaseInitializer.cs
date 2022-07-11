using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LingDev.EntityFrameworkCore.Seed
{
    internal class DatabaseInitializer<TDbContext>
        where TDbContext : DbContext
    {
        private readonly ILogger<DatabaseInitializer<TDbContext>> _logger;
        private readonly TDbContext _context;
        private readonly SeedOptions _options;

        public DatabaseInitializer(
            ILogger<DatabaseInitializer<TDbContext>> logger,
            TDbContext context,
            SeedOptions options,
            IEnumerable<Type>)
        {
            _logger = logger;
            _context = context;
            _options = options;
        }

        public async Task RunAsnyc()
        {
            if (_options.ApplyMigration)
            {
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Applied migrations.");
            }
        }

        private Task ApplySeedAsync()
        {

        }
    }
}
