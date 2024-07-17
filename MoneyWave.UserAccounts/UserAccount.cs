using Core.Aggregates;
using MoneyWave.Api.UserAccounts;
using MoneyWave.UserAccounts.OpeningUserAccount;

namespace MoneyWave.UserAccounts;

public class UserAccount : Aggregate
{
    public Guid UserId { get; private set; }
    public AccountStatus Status { get; private set; }
    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }

    public static UserAccount Open(Guid userId, Guid accountId, Currency currency) => new(userId, accountId, currency);

    private UserAccount(Guid userId, Guid accountId, Currency currency)
    {
        var @event = UserAccountOpened.Create(userId, accountId, currency);
        Enqueue(@event);
        Apply(@event);
    }

    public override void Apply(object @event)
    {
        switch (@event)
        {
            case UserAccountOpened cartOpened:
                Apply(cartOpened);
                return;
        }
    }

    private void Apply(UserAccountOpened @event)
    {
        Version = 0;

        Id = @event.AccountId;
        UserId = @event.UserId;
        Status = AccountStatus.Active;
        Amount = 0;
        Currency = @event.Currency;
    }
}
