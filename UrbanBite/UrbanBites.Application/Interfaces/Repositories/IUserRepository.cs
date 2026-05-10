namespace UrbanBites.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<string?> GetUserEmailAsync(Guid userId);
        Task<string?> GetUserNameAsync(Guid userId);
    }
}
