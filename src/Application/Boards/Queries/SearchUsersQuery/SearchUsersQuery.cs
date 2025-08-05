
using CollabBoard.Application.Common.Interfaces;
using CollabBoard.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CollabBoard.Application.Boards.Queries.SearchUsersQuery;
public record SearchUsersQuery(string Q) : IRequest<List<UserDto>>;

public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, List<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<SearchUsersQueryHandler> _logger;
    public SearchUsersQueryHandler(UserManager<ApplicationUser> userManager, IApplicationDbContext context, ILogger<SearchUsersQueryHandler> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<List<UserDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Searching users with query '{Query}'", request.Q);

            var query = request.Q.ToLower();

            var users = await _userManager.Users
                .Where(u => u.Email != null &&
                    (u.Email.ToLower().Contains(query) ||
                     u.UserName!.ToLower().Contains(query) ||
                     u.DisplayName.ToLower().Contains(query)))
                .Take(20)
                .ToListAsync();

            return users.Select(u => new UserDto(
                    Id: Guid.Parse(u.Id),
                    Email: u.Email!,
                    DisplayName: u.UserName!))
                .ToList();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Searching users with query error");
            throw;
        }
       
    }
}
