using Api.Endpoints;
using Api.Extensions;
using Hangfire;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.DIFromContainer(builder.Configuration);

builder.Services.AddCors(options =>
{
    var client = builder.Configuration["CORS:Client"];
    options.AddPolicy("AllowAngularClient", builder =>
    {
        builder.WithOrigins(client)
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
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAngularClient");

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard();

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