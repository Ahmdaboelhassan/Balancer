using Infrastructure;
using Application;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services
    .AddInfrastructureLayer(builder.Configuration)
    .AddApplicationLayer(builder.Configuration);


var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors( opt =>
    opt.AllowAnyOrigin()
       .AllowAnyHeader()
       .AllowAnyMethod());

app.MapControllers();

app.Run();
