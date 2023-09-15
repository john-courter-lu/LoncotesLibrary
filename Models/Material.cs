using System.ComponentModel.DataAnnotations;

namespace LoncotesLibrary.Models;

public class Material
{
    public int Id { get; set; }
    [Required]
    public string MaterialName { get; set; }
    public int MaterialTypeId { get; set; }
    public MaterialType MaterialType { get; set; }
    public int GenreId { get; set; }
    public Genre Genre { get; set; }
    public DateTime? OutOfCirculationSince { get; set; }
    public List<Checkout> Checkouts { get; set; }

    //EF Core can directly handle joint table, which is not the case in Book 2
}