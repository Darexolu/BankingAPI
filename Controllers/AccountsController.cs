using AutoMapper;
using BankingAPI.Models;
using BankingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPI.Controllers
{
	[ApiController]
	[Route("api/v3/[controller]")]
	public class AccountsController : ControllerBase
	{
		private readonly IAccountService _accountService;
		IMapper _mapper;
		public AccountsController(IAccountService accountService, IMapper mapper)
		{
			_accountService = accountService;
			_mapper = mapper;
		}

		[HttpPost]
		[Route("register_new_account")]
		public IActionResult CreateAccount([FromBody] RegisterNewAccountModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var account = _mapper.Map<Account>(model);
			return Ok(_accountService.Create(account, model.Pin, model.ConfirmPin));
		
		}

		[HttpGet]
		[Route("get_all_accounts")]
		public IActionResult GetAllAccounts()
		{
			var accounts = _accountService.GetAllAccounts();
			var cleanedAccounts = _mapper.Map<IList<GetAccountModel>>(accounts);
			return Ok(cleanedAccounts);
		}
	}
}
