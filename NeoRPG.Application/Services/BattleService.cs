using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NeoRPG.Contract.DTO;
using NeoRPG.Contract.Enums;
using NeoRPG.Domain.Repositories;
using NeoRPG.Domain.Services;
using NeoRPG.Entities.Models;
using System.Text;

namespace NeoRPG.Application.Services;
public class BattleService(ICharacterRepository characterRepository,
    IValidator<BattleRequestDTO> validator,
    ILogger<BattleService> logger) : IBattleService
{
    private readonly ICharacterRepository _characterRepository = characterRepository;
    private readonly IValidator<BattleRequestDTO> _validator = validator;
    private readonly ILogger<BattleService> _logger = logger;

    private readonly int _maxRounds = 2;

    public async Task<BattleResultDTO> ExecuteBattle(BattleRequestDTO battleRequestDTO, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await ValidateRequest(battleRequestDTO);

            if (!validationResult.IsValid)
                throw new ValidationException(string.Join(",", validationResult.Errors));

            _logger.LogInformation("Battle execution started.");

            var players = await _characterRepository.GetCharacterByIdList([battleRequestDTO.FirstPlayer, battleRequestDTO.SecondPlayer], cancellationToken);

            if (players.Count() <= 1)
            {
                throw new ValidationException("Not enough players to start the battle.");
            }

            var battleResult = new BattleResultDTO();

            var battleLog = new StringBuilder();

            var playerOrderList = TryDetermineTurnOrder(players);

            var first = playerOrderList.First();

            var second = playerOrderList.Last();

            battleLog.AppendLine(GetBattleLog(first, second, BattleLogEvent.BattleStart));

            for (var i = 0; i < _maxRounds; i++)
            {
                if (i > 0)
                    playerOrderList = TryDetermineTurnOrder(players);

                first = playerOrderList.First();

                second = playerOrderList.Last();

                var battleIsOver = ExecuteAttackRound(first, second, battleLog);

                if (battleIsOver)
                {
                    battleResult = CreateBattleResult(battleLog, first, second);

                    break;
                }

                battleIsOver = ExecuteAttackRound(second, first, battleLog);

                if (battleIsOver)
                {
                    battleResult = CreateBattleResult(battleLog, second, first);

                    break;
                }
            }

            if (battleResult.Winner is null || battleResult.Loser is null)
            {
                battleLog.AppendLine(GetBattleLog(first, second, BattleLogEvent.Draw));
                battleResult.BattleLog = battleLog.ToString();
            }

            return battleResult;
        }
        catch (ValidationException validationEx)
        {
            _logger.LogWarning("An error occurred during the validation process. Message: {validationExMessage}", validationEx.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during battle execution.");
            throw;
        }

    }

    private static BattleResultDTO CreateBattleResult(StringBuilder battleLog, (int Speed, Character Character) winner, (int Speed, Character Character) loser)
    {
        return new BattleResultDTO
        {
            Winner = winner.Character.ToDTO(),
            Loser = loser.Character.ToDTO(),
            BattleLog = battleLog.ToString()
        };
    }

    private bool ExecuteAttackRound((int Speed, Character Character) first, (int Speed, Character Character) second, StringBuilder logBuilder)
    {
        logBuilder.AppendLine(GetBattleLog(first, second, BattleLogEvent.TurnOrder));

        var damage = ExecuteAttack(first.Character, second.Character);

        logBuilder.AppendLine(GetBattleLog(first, second, BattleLogEvent.Attack, damage));

        if (second.Character.IsDead)
        {
            logBuilder.AppendLine(GetBattleLog(first, second, BattleLogEvent.Victory));
            return true;
        }

        return false;
    }

    private static List<(int Speed, Character Character)> TryDetermineTurnOrder(IEnumerable<Character> players)
    {
        List<(int Speed, Character Character)>? result;

        while (true)
        {
            result = DetermineTurnOrder(players);

            var first = result.First();
            var second = result.Last();

            if (first.Speed != second.Speed && first.Speed != 0 && second.Speed != 0)
                break;
        }

        return result;
    }

    public static List<(int Speed, Character Character)> DetermineTurnOrder(IEnumerable<Character> players)
    {
        List<(int Speed, Character Character)> playerOrderList = [];

        foreach (var player in players)
        {
            Random random = new();

            var speedModifier = player.Job.BaseStats.Intelligence * player.Job.SpeedkModifier.IntelligenceModifier +
                                player.Job.BaseStats.Dexterity * player.Job.SpeedkModifier.DexterityModifier +
                                player.Job.BaseStats.Strength * player.Job.SpeedkModifier.StrengthModifier;

            var speed = random.Next(0, (int)speedModifier);

            playerOrderList.Add((speed, player));
        }

        return [.. playerOrderList.OrderByDescending(x => x.Speed)];
    }

    public static int ExecuteAttack(Character attacker, Character defender)
    {
        var baseStats = attacker.Job.BaseStats;
        var modifiers = attacker.Job.AttackModifier;

        var attackModifier = baseStats.Strength * modifiers.StrengthModifier +
                             baseStats.Dexterity * modifiers.DexterityModifier +
                             baseStats.Intelligence * modifiers.IntelligenceModifier;

        var attackPower = (int)Math.Round(attackModifier, MidpointRounding.AwayFromZero);

        Random random = new();

        var damage = random.Next(0, attackPower + 1);

        defender.TakeDamage(damage);

        return damage;
    }

    private string GetBattleLog((int Speed, Character Character) first, (int Speed, Character Character) second, BattleLogEvent logEvent, int damage = 0)
    {
        var battleLog = "";

        switch (logEvent)
        {
            case BattleLogEvent.BattleStart:
                {
                    battleLog = $"Battle between {first.Character.Name} ({first.Character.Job.Type}) - {first.Character.CurrentHp} HP and {second.Character.Name} ({second.Character.Job.Type}) - {second.Character.CurrentHp} HP begins!";
                    break;
                }
            case BattleLogEvent.TurnOrder:
                {
                    battleLog = $"{first.Character.Name} {first.Speed} speed was faster than {second.Character.Name} {second.Speed} speed and will begin this round.";
                    break;
                }
            case BattleLogEvent.Attack:
                {
                    battleLog = $"{first.Character.Name} attacks {second.Character.Name} for {damage}, {second.Character.Name} has {second.Character.CurrentHp} HP remaining.";
                    break;
                }
            case BattleLogEvent.Victory:
                {
                    battleLog = $"{first.Character.Name} wins the battle! {first.Character.Name} still has {first.Character.CurrentHp} HP remaining!";
                    break;
                }
            case BattleLogEvent.Draw:
                {
                    battleLog = "The battle ended in a draw!";
                    break;
                }
        }

        _logger.LogInformation("{log}", battleLog);

        return battleLog;
    }

    private async Task<ValidationResult> ValidateRequest(BattleRequestDTO battleRequestDTO)
    {
        _logger.LogInformation("Initializing request validation");

        return await _validator.ValidateAsync(battleRequestDTO);
    }
}
