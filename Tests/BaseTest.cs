using Application;
using Application.CQ.Songs.Command.CreateSong;
using Application.Projections;
using Application.Repositories.Shared;
using Application.Services;
using DotNet.Testcontainers.Containers;
using FluentValidation;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

public abstract class BaseTest : IAsyncLifetime
{
    protected ServiceProvider _serviceProvider = null!;
    protected IServiceScope _scope = null!;
    protected IMediator _mediator = null!;
    protected IUnitOfWork _uow = null!;
    private IContainer _postgresContainer = null!;
    protected IProjectionProvider _projectionProvider = null!;

    public async Task InitializeAsync()
    {
        var postgres = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("user")
            .WithPassword("pass")
            .Build();
        
        await postgres.StartAsync();
        _postgresContainer = postgres;

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(postgres.GetConnectionString() + ";Include Error Detail=true")
            );

        builder.Services.AddRepositories();
        builder.Services.AddValidatorsFromAssemblyContaining<IApplicationMarker>();
        builder.Services.AddScoped<ISearchService, SearchService>();
        builder.Services.AddScoped<ILoggerService, LoggerService>();
        builder.Services.AddMediatR(config => 
            config.RegisterServicesFromAssembly(typeof(CreateSongCommand).Assembly));

        _serviceProvider = builder.Services.BuildServiceProvider();

        _scope = _serviceProvider.CreateScope();
        _mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
        _uow = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        _projectionProvider = _scope.ServiceProvider.GetRequiredService<IProjectionProvider>();

        var db = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        _scope.Dispose();
        _serviceProvider.Dispose();
    }

    public string RandomString() => Guid.NewGuid().ToString();
}