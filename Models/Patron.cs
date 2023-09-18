using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

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
    public List<Checkout> Checkouts { get; set; }

    //EF Core can directly handle joint table, which is not the case in Book 2

    // Balance
    // Add another calculated property to the Patron class called Balance that totals up the unpaid fines that a patron owes. Add a Paid property of type bool to the Checkout class that indicates whether a fee has been paid or not. 
    public decimal? Balance
    {
        get
        {
            decimal? totalBalance = 0M;

            if (Checkouts == null) // for endpoints with no Checkouts included;
            {
                return null;
            }

            foreach (Checkout checkout in Checkouts)
            {
                if (!checkout.Paid)
                {
                    totalBalance += checkout.LateFee;
                }
            }
            return totalBalance;
        }
    }
}

//boolean is not nullable by default in C#, like int, so no [Required] needed, even if it's NN