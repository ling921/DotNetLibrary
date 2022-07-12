using LingDev.EntityFrameworkCore.Seed.Attributes;
using LingDev.EntityFrameworkCore.Seed.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text.Json;

namespace LingDev.EntityFrameworkCore.Seed
{
    internal interface IDatabaseInitializer
    {
        Task RunAsync();
    }

    internal class DatabaseInitializer<TDbContext> : IDatabaseInitializer
        where TDbContext : DbContext
    {
        private readonly static JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);
        private readonly ILogger _logger;
        private readonly TDbContext _context;
        private readonly SeedOptions _options;

        public DatabaseInitializer(
            ILogger<DatabaseInitializer<TDbContext>> logger,
            TDbContext context,
            IOptionsFactory<SeedOptions> factory)
        {
            _logger = logger;
            _context = context;
            _options = factory.Create(typeof(TDbContext).Name);
        }

        public async Task RunAsync()
        {
            if (_options.ApplyMigration)
            {
                await _context.Database.MigrateAsync();
                _logger.LogInformation("The database migrations applied completed!");
            }

            if (_options.ApplySeed)
            {
                if (_options.ModelTypes.Length == 0)
                {
                    _logger.LogWarning("No seed models are configured, skip applying seed data.");
                }
                else
                {
                    await ApplySeedAsync();
                    _logger.LogInformation("The database seed data applied completed!");
                }
            }
        }

        private async Task ApplySeedAsync()
        {
            var files = Directory.EnumerateFiles(_options.Path, "*.json");
            if (!files.Any())
                return;

            var entityTypes = _context.Model.GetEntityTypes().Select(t => t.ClrType);
            foreach (var modelType in _options.ModelTypes)
            {
                var entityType = modelType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISeedModel<>))
                    .Select(i => i.GetGenericArguments()[0])
                    .FirstOrDefault();
                if (entityType == null || !entityTypes.Contains(entityType))
                    continue;

                var fileName = modelType.GetCustomAttribute<SeedAttribute>()?.FileName ?? entityType.Name;
                var file = files.FirstOrDefault(fi => fileName.Equals(Path.GetFileNameWithoutExtension(fi), StringComparison.OrdinalIgnoreCase));
                if (file == null)
                    continue;
                var jsonData = File.ReadAllText(file);

                const string methodName = nameof(ApplyEntityAsync);
                var method = typeof(DatabaseInitializer<TDbContext>).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
                if (method == null)
                    throw new Exception($"Can not find static method named \"{methodName}\".");

                var result = method.MakeGenericMethod(modelType, entityType).Invoke(null, new object[] { _logger, _context, jsonData });
                if (result is Task task)
                    await task;
            }
        }

        private static async Task ApplyEntityAsync<TModel, TEntity>(ILogger logger, DbContext context, string jsonData)
            where TModel : class, ISeedModel<TEntity>
            where TEntity : class
        {
            var dbSet = context.Set<TEntity>();
            var tableName = dbSet.EntityType.GetAnnotation("Relational:TableName")?.Value?.ToString();
            if (dbSet.Any())
            {
                logger.LogInformation("There is already data in the table `{table}`, skip applying seed data.", tableName);
                return;
            }

            var models = JsonSerializer.Deserialize<IEnumerable<TModel>>(jsonData, _jsonSerializerOptions);
            if (models?.Any() != true)
            {
                logger.LogWarning("Can not get any {entity} data from json file.", typeof(TEntity).Name);
                return;
            }

            var entities = models.Select(m => m.ToEntity());

            await dbSet.AddRangeAsync(entities);
            await context.SaveChangesAsync();

            logger.LogInformation("Successfully apply the seed data of the {entity} to the database.", typeof(TEntity).Name);
        }
    }
}
