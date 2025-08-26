using Infrastructure;
using Application;
using Domain.Static;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
    
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Client", policy =>
    {
        policy.WithOrigins(MagicStrings.ProductionOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    }); 
});

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

app.UseCors("Client");

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.Run();
