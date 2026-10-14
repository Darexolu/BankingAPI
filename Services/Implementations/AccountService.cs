using BankingAPI.Data;
using BankingAPI.Models;
using BankingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Linq;

namespace BankingAPI.Services.Implementations
{
	public class AccountService : IAccountService
	{

		private BankingDbContext _dbContext;

		public AccountService(BankingDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public Account Authenticate(string accountNumber, string pin)
		{
			var account = _dbContext.Accounts.SingleOrDefault(x => x.AccountNumberGenerated == accountNumber);
			if (account == null) {return null; }

			if(!VerifyPinHash(pin, account.PinHash, account.PinSalt)) { return null; }

			return account;
		}

		private static bool VerifyPinHash(string pin, byte[] pinHash, byte[] pinSalt)
		{
			if (pin == null) throw new ArgumentNullException("pin");
			if (string.IsNullOrWhiteSpace(pin)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "pin");
			if (pinHash.Length != 64) throw new ArgumentException("Invalid length of pin hash (64 bytes expected).", "pinHash");
			if (pinSalt.Length != 128) throw new ArgumentException("Invalid length of pin salt (128 bytes expected).", "pinSalt");
			using (var hmac = new System.Security.Cryptography.HMACSHA512(pinSalt))
			{
				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pin));
				for (int i = 0; i < computedHash.Length; i++)
				{
					if (computedHash[i] != pinHash[i]) return false;
				}
			}
			return true;
		}
		public Account Create(Account account, string pin, string ConfirmPin)
		{
			if(_dbContext.Accounts.Any(x => x.Email == account.Email)) throw new ApplicationException("An account already exists with this email");

			if(!pin.Equals(ConfirmPin)) throw new ArgumentException("Pins do not match", "Pin");

			byte[] pinHash, pinSalt;
			//hashing/encrypting pin before saving to database
			CreatePinHash(pin, out pinHash, out pinSalt);
			account.PinHash = pinHash;
			account.PinSalt = pinSalt;

			_dbContext.Accounts.Add(account);
			_dbContext.SaveChanges();

			return account;

		}

		private static void CreatePinHash(string pin, out byte[] pinMash, out byte[] pinSalt)
		{
			if (pin == null) throw new ArgumentNullException("pin");
			if (string.IsNullOrWhiteSpace(pin)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "pin");
			using (var hmac = new System.Security.Cryptography.HMACSHA512())
			{
				pinSalt = hmac.Key;
				pinMash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pin));
			}
		}

		public void Delete(int id)
		{
			var account = _dbContext.Accounts.Find(id);
			if (account != null)
			{
				_dbContext.Accounts.Remove(account);
				_dbContext.SaveChanges();
			}
		}

		public IEnumerable<Account> GetAllAccounts()
		{
			return _dbContext.Accounts.ToList();
		}

		public Account GetByAccountNumber(string accountNumber)
		{
			var account = _dbContext.Accounts.Where(x => x.AccountNumberGenerated == accountNumber).FirstOrDefault();

			if (account == null) { return null; }		
			return account;
		}

		public Account GetById(int id)
		{
			var account = _dbContext.Accounts.Where(x => x.Id == id).FirstOrDefault();	
			if (account == null) { return null; }
			return account;
		}

		public void Update(Account account, string pin = null)
		{
			var accountToUpdate = _dbContext.Accounts.Where(x => x.Email == account.Email).SingleOrDefault();
			if (accountToUpdate == null) { throw new ApplicationException("Account does not exist"); }

			if(!string.IsNullOrWhiteSpace(account.Email))
			{
				if (_dbContext.Accounts.Any(x => x.Email == account.Email && x.Id != accountToUpdate.Id))
				{
					throw new ApplicationException("This Email " + account.Email + " already exists");
				}
				accountToUpdate.Email = account.Email;
			}

			if (!string.IsNullOrWhiteSpace(account.PhoneNumber))
			{
				if (_dbContext.Accounts.Any(x => x.PhoneNumber == account.PhoneNumber && x.Id != accountToUpdate.Id))
				{
					throw new ApplicationException("This PhoneNumber " + account.PhoneNumber + " already exists");
				}
				accountToUpdate.PhoneNumber = account.PhoneNumber;
			}

			if (!string.IsNullOrWhiteSpace(pin))
			{
				byte[] pinHash, pinSalt;
				CreatePinHash(pin, out pinHash, out pinSalt);
				accountToUpdate.PinHash = pinHash;
				accountToUpdate.PinSalt = pinSalt;

			}
			accountToUpdate.DateLastUpdated = DateTime.UtcNow;

			_dbContext.Accounts.Update(accountToUpdate);
			_dbContext.SaveChanges();
		}
	}
}
