using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NeoRPG.Contract.DTO;
using NeoRPG.Contract.Enums;
using NeoRPG.Domain.Repositories;
using NeoRPG.Domain.Services;
using NeoRPG.Entities.Models;

namespace NeoRPG.Application.Services;

public class CharacterService(ICharacterRepository repository, 
    IValidator<CreateCharacterDTO> validator,
    ILogger<CharacterService> logger) : ICharacterService
{
    private readonly ICharacterRepository _repository = repository;
    private readonly IValidator<CreateCharacterDTO> _validator = validator;
    private readonly ILogger<CharacterService> _logger = logger;

    public async Task<IEnumerable<CharacterDTO>> GetCharacters(int page, int pageSize, CancellationToken cancellationToken)
    {
        var result = await _repository.GetCharacters(page, pageSize, cancellationToken);

        return result.Select(x => x.ToDTO());
    }

    public async Task<CharacterDetailDTO> GetCharacterById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _repository.GetCharacterById(id, cancellationToken)
            ?? throw new Exception("Character not found");

        return result.ToDetailDTO();
    }

    public async Task CreateCharacters(CreateCharacterDTO characterRequest, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await ValidateCreationRequest(characterRequest);

            if (!validationResult.IsValid)
                throw new ValidationException(string.Join(",", validationResult.Errors));

            var job = await GetJobDetails(characterRequest.Job, cancellationToken)
                ?? throw new Exception("Job not found");

            var character = new Character(characterRequest.Name, job);

            await _repository.CreateCharacters(character, cancellationToken);
        }
        catch (ValidationException validationEx)
        {
            _logger.LogWarning("An error occurred during the validation process. Message: {validationExMessage}", validationEx.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during character creation.");
            throw;
        }
    }

    public async Task<ValidationResult> ValidateCreationRequest(CreateCharacterDTO paymentRequest)
    {
        _logger.LogInformation("Initializing request validation");

        return await _validator.ValidateAsync(paymentRequest);
    }

    public async Task<Job?> GetJobDetails(JobType jobType, CancellationToken cancellation)
    {
        //In production environment, adding jobs to cache to improve performance

        return (await _repository.GetAllJobs(cancellation)).FirstOrDefault(x => x.Type == jobType);
    }
}
