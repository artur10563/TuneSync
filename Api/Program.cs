using Api.Endpoints;
using Api.Extensions;
using Application.BackgroundJobs;
using Hangfire;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.DIFromContainer(builder.Configuration);

builder.Services.AddCors(options =>
{
    var clients = builder.Configuration.GetSection("CORS:Clients").Get<string[]>() ?? [];
    options.AddPolicy("AllowAngularClient", corsBuilder =>
    {
        corsBuilder.WithOrigins(clients)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddSwaggerWithJwtAuth();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard();
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAngularClient");

app.UseAuthentication();
app.UseAuthorization();


using (var scope = app.Services.CreateScope())
{
    var jobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    jobManager.AddOrUpdate<FileCleanupJob>(
        FileCleanupJob.Id,
        job => job.ExecuteAsync(),
        Cron.Weekly(DayOfWeek.Friday)
    );
}

app.RegisterAlbumEndpoints()
    .RegisterSongsEndpoints()
    .RegisterUserEndpoints()
    .RegisterPlaylistEndpoints()
    .RegisterYoutubeEndpoints()
    .RegisterJobEndpoints()
    .RegisterArtistEndpoints()
    .RegisterFavoriteEndpoints()
    .RegisterAdminEndpoints();

app.Run();