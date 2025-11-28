using FluentValidation;
using NeoRPG.Application.Services;
using NeoRPG.Application.Validators;
using NeoRPG.Data.Repositories;
using NeoRPG.Domain.Repositories;
using NeoRPG.Domain.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IBattleService, BattleService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCharacterRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<BattleRequestValidator>();
builder.Services.AddSingleton<ICharacterRepository, CharacterRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
