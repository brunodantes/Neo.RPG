using Microsoft.AspNetCore.Mvc;
using NeoRPG.Contract.DTO;
using NeoRPG.Domain.Services;

namespace NeoRPG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BattleController(IBattleService battleService) : ControllerBase
{
    private readonly IBattleService _battleService = battleService;

    [HttpPost]
    public async Task<IActionResult> ExecuteBattle([FromBody] BattleRequestDTO request, CancellationToken cancellationToken)
    {
        var result = await _battleService.ExecuteBattle(request, cancellationToken);

        return Ok(result);
    }
}
