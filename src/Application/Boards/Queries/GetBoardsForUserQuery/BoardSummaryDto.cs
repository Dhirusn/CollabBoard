using CollabBoard.Domain.Entities;

namespace CollabBoard.Application.Boards.Queries.GetBoardsForUserQuery;
public class BoardSummaryDto
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Board, BoardSummaryDto>();
        }
    }
}
