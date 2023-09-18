using System.ComponentModel.DataAnnotations;

namespace LoncotesLibrary.Models;

public class Checkout
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public Material Material { get; set; }
    public int PatronId { get; set; }
    public Patron Patron { get; set; }
    public DateTime CheckoutDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    
    private static decimal _lateFeePerDay = .50M;
    public decimal? LateFee
    {
        get

        {
            //do logic to return fee...
            DateTime dueDate = CheckoutDate.AddDays(Material.MaterialType.CheckoutDays);
            if (ReturnDate != null && dueDate < ReturnDate)
            {
                int daysLate = ((DateTime)ReturnDate - dueDate).Days;
                decimal fee = daysLate * _lateFeePerDay;
                return fee;
            }

            //otherwise return null
            return null;
        }
    }

    //Add another calculated property to the Patron class called Balance that totals up the unpaid fines that a patron owes. Add a Paid property of type bool to the Checkout class that indicates whether a fee has been paid or not. 
    public bool Paid => LateFee == null ? true : false;
}

// DateTime is not nullable by default in C#, so no [Reqiured] needed even if it's NN.
// So are int, boolean/bool