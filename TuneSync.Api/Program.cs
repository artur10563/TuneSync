using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using TuneSync.Api.Endpoints;
using TuneSync.Application.Repositories;
using TuneSync.Application.Services;
using TuneSync.Infrastructure.Repositories;
using TuneSync.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




builder.Services.AddScoped<IYoutubeService, YoutubeService>();
builder.Services.AddScoped<ISongRepository, SongRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.RegisterSongsEndpoints();

app.Run();
