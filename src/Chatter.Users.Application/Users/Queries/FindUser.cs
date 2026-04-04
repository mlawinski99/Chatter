using Core.CQRS;
using Core.Infrastructure;
using Core.ResultPattern;
using Chatter.Users.DataAccess.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Chatter.Users.Application.Users.Queries;

public class FindUser : IQueryHandler<FindUser.FindUserQuery, Result<FindUser.FindUserDto?>>
{
    public record FindUserDto(Guid Id, string Username, string Email);
    public record FindUserQuery(string Search) : IQuery<Result<FindUserDto?>>;

    private readonly UsersDbContext _dbContext;
    private readonly IEncryptor _encryptor;

    public FindUser(UsersDbContext dbContext, IEncryptor encryptor)
    {
        _dbContext = dbContext;
        _encryptor = encryptor;
    }

    public async Task<Result<FindUserDto?>> Handle(FindUserQuery query, CancellationToken cancellationToken)
    {
        var encryptedSearch = _encryptor.Encrypt(query.Search);

        var user = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.UserName == encryptedSearch || u.Email == encryptedSearch)
            .FirstOrDefaultAsync(cancellationToken);

        return Result<FindUserDto?>.Success(user is not null
            ? new FindUserDto(user.Id, user.UserName, user.Email)
            : null);
    }
}