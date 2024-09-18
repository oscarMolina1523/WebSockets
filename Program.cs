var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

//este es para usar el web soket de manera segura 
app.UseWebSockets();

app.MapControllers();

app.Run();
