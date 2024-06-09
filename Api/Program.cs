using Api.Configurations;
using Api.Model.Entities.Note;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Dodaj serwisy do kontenera.
var connectionString = builder.Configuration.GetConnectionString("kopiarka");
builder.Services.AddDbContext<NoteContext>(options =>
{
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5, // Maksymalna liczba ponowieñ
            maxRetryDelay: TimeSpan.FromSeconds(30), // Maksymalne opóŸnienie miêdzy ponowieniami
            errorNumbersToAdd: null); // Mo¿esz dodaæ specyficzne numery b³êdów SQL, które maj¹ byæ objête retry policy
    });
});

builder.Services.AddControllers();
// Dowiedz siê wiêcej o konfigurowaniu Swagger/OpenAPI na https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

//builder.Services.AddAutoMapper(typeof(MapperConfig));

var app = builder.Build();

// Konfiguracja potoku ¿¹dañ HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
