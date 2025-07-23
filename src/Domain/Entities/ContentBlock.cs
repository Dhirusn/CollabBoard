namespace CollabBoard.Domain.Entities;
public class ContentBlock : BaseEntity<Guid>
{
    public Guid PageId { get; set; }
    public BlockType Type { get; set; }          // Text, Image, Icon, Shape, Table
    public string Payload { get; set; } = string.Empty; // JSON blob (structure below)
    public double X { get; set; }                // left offset on page canvas
    public double Y { get; set; }                // top offset
    public double Width { get; set; }
    public double Height { get; set; }
    public double Rotation { get; set; }
    public string? Style { get; set; }           // JSON for fonts, colors, borders
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
    public int Version { get; set; } = 1;       // optimistic lock
    public Page Page { get; set; } = null!; // Navigation property to parent page
}
