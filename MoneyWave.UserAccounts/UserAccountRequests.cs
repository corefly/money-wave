namespace MoneyWave.UserAccounts;

public record OpenUserAccountRequest(Guid UserId, Currency Currency);
