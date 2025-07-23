namespace CollabBoard.Domain.Entities;
public class Snapshot : BaseEntity<Guid>
{
    public Guid BoardId { get; set; }
    public string PagesBlob { get; set; } = string.Empty;   // gzipped JSON
    public DateTime TakenUtc { get; set; } = DateTime.UtcNow;
    public Board Board { get; set; } = null!;

}
