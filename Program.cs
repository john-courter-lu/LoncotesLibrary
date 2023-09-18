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
// improvement: return Results.NotFound() can be added for error handling.

//4 Add a Material
app.MapPost("/api/materials", (LoncotesLibraryDbContext db, Material material) =>
{
    db.Materials.Add(material);
    db.SaveChanges();
    return Results.Created($"/api/materials/{material.Id}", material);
});

// 5 Remove a Material From Circulation
// a soft delete, where a row is not deleted from the database, but instead has a flag that says the row is no longer active. (The endpoint to get all materials should already be filtering these items out.)
// MapDelete? MapPut? depends on whether your are sending a payload. if not, I'd do a delete. but the stakes are low
// test results: MapDelete doesn't have to involve .Remove, and you can still have access to its data via MapGet with {id}

app.MapDelete("/api/materials/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    Material matchedMaterial = db.Materials.SingleOrDefault(m => m.Id == id);

    matchedMaterial.OutOfCirculationSince = DateTime.Now;

    db.SaveChanges();

    return Results.NoContent();
});

//6 7 8 Get MaterialTypes; Get Genres; Get Patrons
app.MapGet("/api/materialtypes", (LoncotesLibraryDbContext db) =>
{
    return db.MaterialTypes.ToList();
});

app.MapGet("/api/genres", (LoncotesLibraryDbContext db) =>
{
    return db.Genres.ToList();
});

app.MapGet("/api/patrons", (LoncotesLibraryDbContext db) =>
{
    return Results.Ok(db.Patrons.ToList());
});

// 9 get a patron and include their checkouts, and further include the materials and their material types.
app.MapGet("/api/patrons/{patronId}", (LoncotesLibraryDbContext db, int patronId) =>
{
    var query = db.Patrons
        .Where(p => p.Id == patronId)
        .Include(p => p.Checkouts)
            .ThenInclude(co => co.Material)
                .ThenInclude(m => m.MaterialType)
        .ToList();

    return Results.Ok(query);
});

// 10 Update Patron; address or email changable only;
app.MapPut("/api/patrons/{id}", (LoncotesLibraryDbContext db, int id, Patron updatedPatron) =>
{
    if (updatedPatron.Id != id)
    {
        return Results.BadRequest();
    }

    Patron patronToUpdate = db.Patrons.SingleOrDefault(p => p.Id == updatedPatron.Id);
    if (patronToUpdate == null)
    {
        return Results.NotFound();
    }

    patronToUpdate.Address = updatedPatron.Address;
    patronToUpdate.Email = updatedPatron.Email;

    db.SaveChanges();

    return Results.NoContent();
});

//11 Deactivate Patron or Re-activate Patron
//if it's just a deactivation or a soft delete, MapDelete recommanded.
app.MapPut("/api/patrons/{id}/edit-active-status", (LoncotesLibraryDbContext db, int id) =>
{
    Patron matchedPatron = db.Patrons.SingleOrDefault(p => p.Id == id);

    if (matchedPatron == null)
    {
        return Results.NotFound();
    }

    matchedPatron.IsActive = !matchedPatron.IsActive;

    db.SaveChanges();

    return Results.NoContent();
});

//12 Checkout a Material
app.MapPost("/api/checkouts", (LoncotesLibraryDbContext db, Checkout newCheckout) =>
{

    // Id will be taken care of by EF Core;
    // Material, Patron will be taken care of by .Include() in other endpoints; or use SingleOrDefault
    // ReturnDate is nullable so leave it out;

    try
    {
        newCheckout.CheckoutDate = DateTime.Today;
        db.Checkouts.Add(newCheckout);
        db.SaveChanges();
        return Results.Created($"/api/checkouts/{newCheckout.Id}", newCheckout);
    }
    catch (DbUpdateException) 
    {
        return Results.BadRequest("Invalid data submitted");
    }
    
    // If the SaveChanges() method encounters any database-related issues (such as constraint violations or database connectivity problems), it can throw a DbUpdateException. 
});

/* json object to test

{
    "materialId": 2,
    "patronId": 2
}

*/

//13 Return a Material

app.Run();
