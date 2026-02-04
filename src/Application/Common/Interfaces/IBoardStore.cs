public interface IBoardStore
{
    Task<bool> UserCanAccessAsync(string userId, Guid boardId);
    Task<byte[]?> LoadStateAsync(Guid boardId);
    Task SaveStateAsync(Guid boardId, byte[] state);
}
