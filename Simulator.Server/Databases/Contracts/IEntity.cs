using Simulator.Shared.Intefaces;

namespace Simulator.Server.Databases.Contracts
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }

        DateTime? DeletedOnUtc { get; set; }
    }
    public interface ICreator<T> where T : class, IEntity
    {
        static abstract T Create(IDto dto);
    }
    public interface IMapper
    {

        void MapFromDto(IDto dto);

        T MapToDto<T>() where T : IDto, new();
    }

    public interface IQueryHandler<T> where T : class, IEntity
    {
        static abstract Expression<Func<T, bool>> GetFilterBy(IDto dto);
        static abstract Func<IQueryable<T>, IIncludableQueryable<T, object>> GetIncludesBy(IDto dto);
        static abstract Expression<Func<T, object>> GetOrderBy(IDto dto);
    }

    public interface IEntity : ISoftDeletable
    {
        Guid Id { get; }
        DateTime CreatedOn { get; set; }
        string? CreatedBy { get; set; }
        bool IsTenanted { get; }
        int Order { get; set; }
    }
    public interface ITennant
    {
        string TenantId { get; set; }

    }
    public interface IValidationRule<T> where T : class, IEntity
    {
        // Define los criterios para validar unicidad
        static abstract Expression<Func<T, bool>> GetIdCriteria(IDto dto);
        static abstract Expression<Func<T, bool>> GetExistCriteria(IDto dto, string validationKey);
    }
}