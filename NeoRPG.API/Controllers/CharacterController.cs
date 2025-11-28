using Microsoft.AspNetCore.Mvc;
using NeoRPG.Contract.DTO;
using NeoRPG.Domain.Services;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace NeoRPG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharactersController(ICharacterService characterService) : ControllerBase
{
    private readonly ICharacterService _characterService = characterService;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CharacterDTO>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCharacters([FromQuery] int page = 1, [FromQuery] int pageSize = 100, CancellationToken cancellationToken = default)
    {
        var result = await _characterService.GetCharacters(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:Guid}")]
    [ProducesResponseType(typeof(CharacterDetailDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetCharacterById([FromRoute][Required] Guid id, CancellationToken cancellationToken)
    {
        var result = await _characterService.GetCharacterById(id, cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateCharacters([FromBody][Required] CreateCharacterDTO characterRequest, CancellationToken cancellationToken)
    {
        await _characterService.CreateCharacters(characterRequest, cancellationToken);

        return Created();
    }
}
