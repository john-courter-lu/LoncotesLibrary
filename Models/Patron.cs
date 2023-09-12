using System.ComponentModel.DataAnnotations;

namespace LoncotesLibrary.Models;

public class Patron
{
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public List<Material> Materials { get; set; }
}

//boolean is not nullable by default in C#, like int, so no [Required] needed, even if it's NN