using LingDev.EntityFrameworkCore.Audit.Entities;
using LingDev.EntityFrameworkCore.Entities;

namespace LingDev.EntityFrameworkCore.Audit.Internal
{
    internal static class AuditConstants
    {
        public const string Id = nameof(IEntity<object>.Id);
        public const string CreationTime = nameof(IHasCreationTime.CreationTime);
        public const string CreatorId = nameof(IHasCreator<IEntity>.Creator) + Id;
        public const string LastModificationTime = nameof(IHasModificationTime.LastModificationTime);
        public const string LastModifierId = nameof(IHasModifier<IEntity>.LastModifier) + Id;
        public const string DeletionTime = nameof(IHasDeletionTime.DeletionTime);
        public const string DeleterId = nameof(IHasDeleter<IEntity>.Deleter) + Id;
        public const string IsDeleted = nameof(ISoftDelete.IsDeleted);

        public static readonly IReadOnlyCollection<string> PropertieNames = new[]
        {
            Id,
            CreationTime,
            CreatorId,
            LastModificationTime,
            LastModifierId,
            DeletionTime,
            DeleterId,
            IsDeleted,
        };
    }
}
