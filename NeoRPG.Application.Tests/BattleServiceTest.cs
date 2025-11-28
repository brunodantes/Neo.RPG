using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using NeoRPG.Application.Services;
using NeoRPG.Contract.DTO;
using NeoRPG.Contract.Enums;
using NeoRPG.Data.Repositories;
using NeoRPG.Entities.Models;

namespace NeoRPG.Application.Tests;

public class BattleServiceTest
{
    private readonly CharacterRepository _characterRepository;
    private readonly Mock<IValidator<BattleRequestDTO>> _validatorMock;
    private readonly Mock<ILogger<BattleService>> _loggerMock;
    private readonly BattleService _sut; // System Under Test

    public BattleServiceTest()
    {
        _characterRepository = new CharacterRepository();
        _validatorMock = new Mock<IValidator<BattleRequestDTO>>();
        _loggerMock = new Mock<ILogger<BattleService>>();

        _sut = new BattleService(_characterRepository, _validatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task DetermineTurnOrder_ShouldReturnPlayersWithValidSpeeds()
    {
        //Arrange
        var players = await GetPlayers();

        // Act
        var result = BattleService.DetermineTurnOrder(players);

        // Assert
        Assert.Equal(players.Count, result.Count);

        foreach (var p in players)
            Assert.Contains(result, x => x.Character.Id == p.Id);

        // Assert
        foreach (var item in result)
        {
            var character = item.Character;

            var expectedSpeedModifier =
                character.Job.BaseStats.Intelligence * character.Job.SpeedkModifier.IntelligenceModifier +
                character.Job.BaseStats.Dexterity * character.Job.SpeedkModifier.DexterityModifier +
                character.Job.BaseStats.Strength * character.Job.SpeedkModifier.StrengthModifier;

            Assert.InRange(item.Speed, 0, (int)expectedSpeedModifier - 1);
        }

        // Assert
        var sorted = result.OrderByDescending(x => x.Speed).ToList();

        Assert.Equal(sorted, result);
    }


    [Fact]
    public async Task ExecuteBattle_ShouldThrowValidationException_WhenNotEnoughPlayers()
    {
        // Arrange
        var request = new BattleRequestDTO
        {
            FirstPlayer = Guid.NewGuid()
        };

        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        // Act - Assert
        await Assert.ThrowsAsync<ValidationException>(() => _sut.ExecuteBattle(request, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteBattle_ShouldReturnWinnerAndLoser_WhenBattleCompletes()
    {
        // Arrange

        var jobs = await _characterRepository.GetAllJobs(default);


        await _characterRepository.CreateCharacters(new Character("Jhon", jobs.First(x => x.Type == JobType.Warrior)), default);
        await _characterRepository.CreateCharacters(new Character("Merlin", jobs.First(x => x.Type == JobType.Mage)), default);

        
        var firstPlayer = (await _characterRepository.GetCharacters(1, 10, default)).First(x => x.Name == "Jhon");
        var secondPlayer = (await _characterRepository.GetCharacters(1, 10, default)).First(x => x.Name == "Merlin");

        var request = new BattleRequestDTO
        {
            FirstPlayer = firstPlayer.Id,
            SecondPlayer = secondPlayer.Id
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await _sut.ExecuteBattle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Winner);
        Assert.NotNull(result.Loser);
        Assert.False(string.IsNullOrWhiteSpace(result.BattleLog));
        Assert.NotEqual(result.Winner.Id, result.Loser.Id);
    }

    public async Task<List<Character>> GetPlayers()
    {
        var result = new List<Character>();

        var jobs = await _characterRepository.GetAllJobs(default);

        var warrior = jobs.First(x => x.Type == JobType.Warrior);

        result.Add(new Character("Jhon", warrior));

        var mage = jobs.First(x => x.Type == JobType.Mage);

        result.Add(new Character("Merlin", mage));

        return result;
    }
}