namespace CollabBoard.Domain.Entities;
public class Page : BaseEntity<Guid>
{
    public int OrderIndex { get; set; }
    public Guid BoardId { get; set; }
    public Board Board { get; set; } = null!;
    public ICollection<ContentBlock> Blocks { get; private set; } = [];
}
