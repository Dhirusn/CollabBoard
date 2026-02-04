public interface ICollabClient
{
    Task UserPresenceChanged(UserPresenceDto dto);
    Task UserCursorMoved(CursorDto dto);
    Task UserDisconnected(string connectionId);
    Task SyncYjsUpdate(byte[] update);
    Task SyncAwareness(byte[] update);   // ← add this if you need it
}
