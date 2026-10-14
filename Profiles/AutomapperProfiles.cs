using AutoMapper;
using BankingAPI.Models;

namespace BankingAPI.Profiles
{
	public class AutomapperProfiles : Profile
	{
		public AutomapperProfiles() {

			CreateMap<RegisterNewAccountModel, Account>();
			CreateMap<UpdateAccountModel, Account>();
			CreateMap<Account, GetAccountModel>();

		}

	}
}
