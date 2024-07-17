using Core.Commands;
using Core.Ids;
using Core.Queries;
using Microsoft.AspNetCore.Mvc;
using MoneyWave.UserAccounts;
using MoneyWave.UserAccounts.OpeningUserAccount;

namespace MoneyWave.Api.Controllers;

[Route("api/accounts")]
public class UserAccountController(ICommandBus commandBus, IQueryBus queryBus) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> OpenAccount([FromBody] OpenUserAccountRequest request, [FromServices] IIdGenerator idGenerator)
    {
        var accountId = idGenerator.New();
        var command = new OpenUserAccount(request.UserId, accountId, request.Currency);

        await commandBus.Send(command);

        return NoContent();
    }
}
