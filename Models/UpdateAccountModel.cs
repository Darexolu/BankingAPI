using System.ComponentModel.DataAnnotations;

namespace BankingAPI.Models
{
	public class UpdateAccountModel
	{
		
		public int Id { get; set; }
		public string PhoneNumber { get; set; }
		public string Email { get; set; }
		public DateTime DateLastUpdated { get; set; }
		[Required]
		[RegularExpression(@"^\d{4}$", ErrorMessage = "Pin must be a 4-digit number")]
		public string Pin { get; set; }
		[Required]
		[Compare("Pin", ErrorMessage = "Pins do not match")]
		public string ConfirmPin { get; set; }
	}
}
