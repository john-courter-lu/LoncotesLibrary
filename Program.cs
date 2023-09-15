using LoncotesLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<LoncotesLibraryDbContext>(builder.Configuration["LoncotesLibraryDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// endpoints 
//1  Get all Materials
//2  Get Materials by Genre and/or MaterialType
//all the circulating materials. Include the Genre and MaterialType. Exclude materials that have a OutOfCirculationSince value.
app.MapGet("/api/materials", (LoncotesLibraryDbContext db, int? materialTypeId, int? genreId) =>
{
    List<Material> circulatingMaterials = db.Materials
      .Where(m => m.OutOfCirculationSince == null)
      .Include(m => m.Genre)
      .Include(m => m.MaterialType)
      .ToList();

    List<Material> matchedMaterials = circulatingMaterials;

    if (materialTypeId != null && genreId != null)
    {
        matchedMaterials = circulatingMaterials
             .FindAll(m => m.MaterialTypeId == materialTypeId && m.GenreId == genreId);

    }
    else if (materialTypeId != null && genreId == null)
    {
        matchedMaterials = circulatingMaterials
             .FindAll(m => m.MaterialTypeId == materialTypeId);

    }
    else if (materialTypeId == null && genreId != null)
    {
        matchedMaterials = circulatingMaterials
     .FindAll(m => m.GenreId == genreId);

    }

    if (matchedMaterials.Count == 0)
    {
        return Results.NotFound();
    }

    return Results.Ok(matchedMaterials);


    /* (LoncotesLibraryDbContext db) : dependency injection, where the framework sees a dependency that the handler requires, and passes in an instance of it as an arg so that the handler can use it. */
});

//3 Get Meterial by Id
app.MapGet("/api/materials/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    return db.Materials
    .Include(m => m.Genre)
    .Include(m => m.MaterialType)
    .Include(m => m.Checkouts)
        .ThenInclude(c => c.Patron)
    .SingleOrDefault(m => m.Id == id);
});

// inside ThenInclude, c or m or co, it doesn't matter.

//4 Add a Material
app.MapPost("/api/materials", (LoncotesLibraryDbContext db, Material material) =>
{
    db.Materials.Add(material);
    db.SaveChanges();
    return Results.Created($"/api/materials/{material.Id}", material);
});



app.Run();
