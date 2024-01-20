using BankAPI.Context;
using Microsoft.EntityFrameworkCore;
using Shared.DBObjects.AccountData;
using Shared.DBObjects.TransferData;
using SharedClass;
using SharedClass.ClientObjects;

namespace BankAPI.Services
{
	public interface IBankService
	{
		Task<ServiceResponse<List<TransferInfo>>> GetTransfers(string username);
		Task<ServiceResponse<bool>> MakeTransfer(TransferForm form, string username);
		Task<ServiceResponse<AccountInfo>> GetDetails(string username);
	}

	public class BankService : IBankService
	{
		private readonly DataContext _context;
		private readonly IConfiguration _config;
		private readonly ILogService _logService;
		private readonly Cryptographer _cryptographer;

		public BankService(DataContext context, IConfiguration config, ILogService logService, Cryptographer cryptographer)
		{
			this._context = context;
			this._config = config;
			this._logService = logService;
			this._cryptographer = cryptographer;
		}

		public async Task<ServiceResponse<AccountInfo>> GetDetails(string username)
		{
			try
			{
				var res = await _context.Accounts.FirstAsync(user => user.UserName == username);
				var resCut = new AccountInfo(_cryptographer.Decrypt(res));

				return new ServiceResponse<AccountInfo>()
				{
					Data = resCut,
					Success = true,
					Message = "Ok"
				};
			}
			catch (Exception ex)
			{
				await _logService.AddLog($"GetDetails:{username}", true, ex.Message);
				return new ServiceResponse<AccountInfo>()
				{
					Success = false,
				};
			}

		}

		public async Task<ServiceResponse<List<TransferInfo>>> GetTransfers(string username)
		{
			try
			{
				List<TransferInfo> result = new List<TransferInfo>();
				var clientData = await _context.Accounts.FirstAsync(u => u.UserName == username);
				//Client is sender
				var transferList = await _context.Transfers.Where(t => t.Sender == clientData.Id).ToListAsync();
				foreach (var tran in transferList)
				{
					CryptedAccountData adress = await _context.Accounts.FindAsync(tran.Address);

					var uncryptedAdd = _cryptographer.Decrypt(adress);
					var uncryptedTran = _cryptographer.DecryptTransfer(tran);
					result.Add(new TransferInfo(uncryptedTran, uncryptedAdd.AccountNumber, uncryptedAdd.FirstName, uncryptedAdd.LastName, false));

				}
				//Client is adresser
				transferList = await _context.Transfers.Where(t => t.Address == clientData.Id).ToListAsync();
				foreach (var tran in transferList)
				{
					CryptedAccountData adress = await _context.Accounts.FindAsync(tran.Sender);

					var uncryptedAdd = _cryptographer.Decrypt(adress);
					var uncryptedTran = _cryptographer.DecryptTransfer(tran);
					result.Add(new TransferInfo(uncryptedTran, uncryptedAdd.AccountNumber, uncryptedAdd.FirstName, uncryptedAdd.LastName, true));

				}

				return new ServiceResponse<List<TransferInfo>>()
				{
					Data = result,
					Success = true,
					Message = "Ok"
				};
			}
			catch (Exception ex)
			{
				await _logService.AddLog($"GetTransfers:{username}", true, ex.Message);
				return new ServiceResponse<List<TransferInfo>>()
				{
					Success = false,
				};
			}
		}

		public async Task<ServiceResponse<bool>> MakeTransfer(TransferForm form, string username)
		{
			try
			{
				//Check if data is correct
				var addres = await _context.Accounts.FirstAsync(a => a.AccountNumber == form.AdressAccountNum);

				var uncryptAdresser = _cryptographer.Decrypt(addres);
				if (uncryptAdresser.FirstName != form.AdressFirstName || uncryptAdresser.LastName != form.AdressLastName)
				{
					return new ServiceResponse<bool>()
					{
						Message = "Data is incorrect",
						Success = false,
					};
				}
				//Check if sender can pay it
				var sender = await _context.Accounts.FirstAsync(a => a.UserName == username);
				var uncryptoSender = _cryptographer.Decrypt(sender);
				if (uncryptoSender.Balance < form.Price)
				{
					return new ServiceResponse<bool>()
					{
						Message = "You don't have enough funds",
						Success = false,
					};
				}
				//Add transfer
				var tran = new Transfer()
				{
					Address = addres.Id,
					Sender = sender.Id,
					Title = form.Title,
					Price = form.Price,
					TimeStamp = DateTime.Now,
				};
				var cryptTran = _cryptographer.EncryptTransfer(tran);

				_context.ChangeTracker.Clear();

				_context.Transfers.Add(cryptTran);
				await _context.SaveChangesAsync();
				//remove sender money
				uncryptoSender.Balance -= form.Price;
				var copyId = sender.Id;
				//Copy
				var newSender = _cryptographer.Encrypt(uncryptoSender);
				newSender.Id = copyId;

				var updatedSender = new CryptedAccountData() { Id = copyId };
				_context.Accounts.Attach(updatedSender);

				updatedSender.UserName = newSender.UserName;
				updatedSender.CryptedInfo = newSender.CryptedInfo;
				updatedSender.AccountNumber = newSender.AccountNumber;

				await _context.SaveChangesAsync();
				//Add adresser money
				uncryptAdresser.Balance += form.Price;
				copyId = addres.Id;
				var newAddres = _cryptographer.Encrypt(uncryptAdresser);
				newAddres.Id = copyId;

				var updatedAdd = new CryptedAccountData() { Id = copyId };
				_context.Accounts.Attach(updatedAdd);

				updatedAdd.UserName = newAddres.UserName;
				updatedAdd.CryptedInfo = newAddres.CryptedInfo;
				updatedAdd.AccountNumber = newAddres.AccountNumber;

				await _context.SaveChangesAsync();
				return new ServiceResponse<bool>()
				{
					Success = true,
					Data = true
				};
			}
			catch (InvalidOperationException ioe)
			{
				return new ServiceResponse<bool>()
				{
					Message = "Adress doesn't exist!",
					Success = false,
				};
			}
			catch (Exception e)
			{
				await _logService.AddLog($"MakeTransfer:{username}", true, e.Message);
				return new ServiceResponse<bool>()
				{
					Message = "Ups, something go wrong!",
					Success = false
				};

			}
		}
	}
}
