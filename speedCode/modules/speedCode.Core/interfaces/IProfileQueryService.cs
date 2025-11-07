namespace speedCode.Core.interfaces;

public interface IProfileQueryService
{
    Task<ProfileDto?> GetByIdAsync(Guid id);
    Task<List<ProfileDto>> GetByIdsAsync(List<Guid> ids);
}
