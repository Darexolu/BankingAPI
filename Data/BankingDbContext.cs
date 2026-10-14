using BankingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingAPI.Data
{
	public class BankingDbContext : DbContext
	{
		public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options)
		{
		}
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
	}
}
