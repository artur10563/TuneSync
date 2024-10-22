using Api.Endpoints;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.DIFromContainer(builder.Configuration);

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAngularClient", builder =>
	{
		builder.WithOrigins("http://localhost:4200") 
			   .AllowAnyMethod() 
			   .AllowAnyHeader();
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularClient");
//app.UseAuthorization();

app.RegisterSongsEndpoints();

app.Run();
