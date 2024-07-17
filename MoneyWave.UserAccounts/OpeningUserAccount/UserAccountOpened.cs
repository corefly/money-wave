namespace MoneyWave.UserAccounts.OpeningUserAccount;

public record UserAccountOpened(Guid UserId, Guid AccountId, Currency Currency)
{
    public static UserAccountOpened Create(Guid userId, Guid accountId, Currency currency)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentOutOfRangeException(nameof(userId));
        }

        if (accountId == Guid.Empty)
        {
            throw new ArgumentOutOfRangeException(nameof(accountId));
        }

        return new UserAccountOpened(userId, accountId, currency);
    }
}
