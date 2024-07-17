using Core.Commands;
using Core.EventStoreDb.Repository;

namespace MoneyWave.UserAccounts.OpeningUserAccount;

public record OpenUserAccount(Guid AccountId, Guid UserId, Currency Currency);

internal class HandleOpenUserAccount(IEventStoreDbRepository<UserAccount> userAccountRepository)
    : ICommandHandler<OpenUserAccount>
{
    public Task Handle(OpenUserAccount command, CancellationToken ct)
    {
        return userAccountRepository.Add(command.AccountId,
            UserAccount.Open(command.UserId, command.AccountId, command.Currency),
            ct);
    }
}
