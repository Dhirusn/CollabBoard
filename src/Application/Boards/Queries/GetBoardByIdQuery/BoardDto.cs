namespace CollabBoard.Application.Boards.Queries.GetBoardByIdQuery;
public class BoardDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset Created { get; set; }
    // Add other properties as needed
}
