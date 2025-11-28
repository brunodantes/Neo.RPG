using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using NeoRPG.Application.Services;
using NeoRPG.Application.Validators;
using NeoRPG.Contract.DTO;
using NeoRPG.Contract.Enums;
using NeoRPG.Data.Repositories;

namespace NeoRPG.Application.Tests;
public class CharacterServiceTest
{
    private readonly CharacterRepository _characterRepository;
    private readonly CreateCharacterRequestValidator _validator;
    private readonly CharacterService _sut; // System Under Test

    public CharacterServiceTest()
    {
        _validator = new CreateCharacterRequestValidator();
        _characterRepository = new CharacterRepository();
        _sut = new CharacterService(_characterRepository, _validator, new Mock<ILogger<CharacterService>>().Object);
    }

    [Fact]
    public async Task CreateCharacter_ThrowValidationException_WhenShortName()
    {
        //arrange
        var request = new CreateCharacterDTO
        {
            Name = "Ab",      // inválido
            Job = JobType.Warrior
        };

        //act
        Task act() => _sut.CreateCharacters(request, CancellationToken.None);

        // assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }


    [Fact]
    public async Task CreateCharacter_ThrowValidationException_WhenNameHasNumber()
    {
        //arrange
        var request = new CreateCharacterDTO
        {
            Name = "jhon1",      // inválido
            Job = JobType.Warrior
        };

        //act
        Task act() => _sut.CreateCharacters(request, CancellationToken.None);

        // assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }

    [Fact]
    public async Task CreateCharacter_CratedCharacterSameAsRequest()
    {
        //arrange
        var request = new CreateCharacterDTO
        {
            Name = "jhon_name",      // inválido
            Job = JobType.Warrior
        };

        //act
        await _sut.CreateCharacters(request, CancellationToken.None);

        var createdCharacter = (await _sut.GetCharacters(1, 10, CancellationToken.None)).FirstOrDefault(c => c.Name == "jhon_name");

        // assert
        Assert.NotNull(createdCharacter);
        Assert.Equal(request.Name, createdCharacter.Name);
    }
}
