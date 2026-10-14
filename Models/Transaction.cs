using System.Transactions;

namespace BankingAPI.Models
{
	public class Transaction
	{
		public int Id { get; set; }
		public string TransactionUniqueReference { get; set; }
		public decimal TransactionAmount { get; set; }
		public TranStatus TransactionStatus { get; set; }
		public bool IsSuccessful => TransactionStatus == TranStatus.Success;
		public string TransactionSourceAccount { get; set; }
		public string TransactionDestinationAccount { get; set; }
		public string TransactionParticulars { get; set; }
		public TranType TransactionType { get; set; }
		public DateTime TransactionDate { get; set; }

		public Transaction()
		{
			TransactionUniqueReference = Guid.NewGuid().ToString().Replace("-", "").Substring(1, 27);
		}

	}
	public enum TranStatus
	{
		Failed,
		Success,
		Error,
		Pending
	}

	public enum TranType
	{
		Deposit,
		Withdrawal,
		Transfer,
		Payment
	}
}

//22mins 30secs