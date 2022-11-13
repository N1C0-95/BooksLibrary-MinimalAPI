using BooksLibrary.Endpoints;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using MinimalApi.Auth;
using MinimalApi.Data;
using MinimalApi.Models;
using MinimalApi.Services;

var builder = WebApplication.CreateBuilder(args);

//service registration start here

builder.Services.AddAuthentication(ApiKeySchemeConstant.SchemeName)
    .AddScheme<ApiKeyAuthSchemeOptions, ApiKeyAuthHandler>(ApiKeySchemeConstant.SchemeName, _ => { });
builder.Services.AddAuthorization();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory>(_ => new SqlLiteConnectionFactory(builder.Configuration.GetValue<string>("Database:ConnectionString")));
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

//endpoint builder
builder.Services.AddLibraryEndpoint();

//service registration stop here
var app = builder.Build();

// middleware registration starts here
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

//use endpoint
app.UseLibraryEndpoints();


//DB Init here
var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync();




app.Run();
