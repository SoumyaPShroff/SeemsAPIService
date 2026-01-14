namespace SeemsAPIService.Application.Mapper
{
    public interface IEntityMapper<TDto, TEntity, TExtra>
    {
        TEntity MapForAdd(TDto dto, TExtra extra);
        void MapForEdit(TDto dto, TEntity existing, TExtra extra);
    }
}
