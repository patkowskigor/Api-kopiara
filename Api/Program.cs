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
            maxRetryCount: 5, // Maksymalna liczba ponowie�
            maxRetryDelay: TimeSpan.FromSeconds(30), // Maksymalne op�nienie mi�dzy ponowieniami
            errorNumbersToAdd: null); // Mo�esz doda� specyficzne numery b��d�w SQL, kt�re maj� by� obj�te retry policy
    });
});

builder.Services.AddControllers();
// Dowiedz si� wi�cej o konfigurowaniu Swagger/OpenAPI na https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

//builder.Services.AddAutoMapper(typeof(MapperConfig));

var app = builder.Build();

// Konfiguracja potoku ��da� HTTP.
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
