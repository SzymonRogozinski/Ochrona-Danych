using BankAPI.Context;
using Microsoft.EntityFrameworkCore;
using Shared.DBObjects.AccountStatus;

namespace BankAPI.Services
{
	public interface IStatusService
	{
		Task<string> GetStatus(string username);
		Task<bool> SetStatus(string username, string status);
		Task<bool> ReduceTrials(string username);
		Task<bool> RestartTrials(string username);
	}

	public class StatusService : IStatusService
	{
		private readonly DataContext _context;
		private readonly Cryptographer _cryptographer;
		public StatusService(DataContext context, Cryptographer cryptographer)
		{
			_context = context;
			_cryptographer = cryptographer;
		}

		public async Task<string> GetStatus(string username)
		{
			try
			{
				await StatusCheck(username);
				var acc = await _context.Accounts.FirstAsync(user => user.UserName == username);

				var res = await _context.AccStatus.FirstAsync(stat => stat.UserId == acc.Id);
				var uncryptedRes = _cryptographer.DecryptStatus(res);
				return uncryptedRes.Status;

			}
			catch (Exception e)
			{
				return null;
			}
		}

		public async Task<bool> SetStatus(string username, string status)
		{
			try
			{
				await StatusCheck(username);
				var acc = await _context.Accounts.FirstAsync(user => user.UserName == username);

				var res = await _context.AccStatus.FirstAsync(stat => stat.UserId == acc.Id);
				var uncryptedRes = _cryptographer.DecryptStatus(res);

				_context.ChangeTracker.Clear();

				uncryptedRes.Status = status;
				var copyId = res.Id;

				var newCryptedStatus = _cryptographer.EncryptStatus(uncryptedRes);
				newCryptedStatus.Id = copyId;

				var updatedStatus = new CryptedAccountStatus() { Id = copyId };
				_context.AccStatus.Attach(updatedStatus);

				updatedStatus.CryptedInfo = newCryptedStatus.CryptedInfo;
				updatedStatus.UserId = newCryptedStatus.UserId;

				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		public async Task<bool> ReduceTrials(string username)
		{
			try
			{
				await StatusCheck(username);
				var acc = await _context.Accounts.FirstAsync(user => user.UserName == username);

				var res = await _context.AccStatus.FirstAsync(stat => stat.UserId == acc.Id);
				var uncryptedRes = _cryptographer.DecryptStatus(res);

				_context.ChangeTracker.Clear();

				if (uncryptedRes.HowManyTrials <= 0)
				{
					return false;
				}
				uncryptedRes.HowManyTrials--;
				var copyId = res.Id;

				var newCryptedStatus = _cryptographer.EncryptStatus(uncryptedRes);
				newCryptedStatus.Id = copyId;

				var updatedStatus = new CryptedAccountStatus() { Id = copyId };
				_context.AccStatus.Attach(updatedStatus);

				updatedStatus.CryptedInfo = newCryptedStatus.CryptedInfo;
				updatedStatus.UserId = newCryptedStatus.UserId;

				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		public async Task<bool> RestartTrials(string username)
		{
			try
			{
				await StatusCheck(username);
				var acc = await _context.Accounts.FirstAsync(user => user.UserName == username);

				var res = await _context.AccStatus.FirstAsync(stat => stat.UserId == acc.Id);
				var uncryptedRes = _cryptographer.DecryptStatus(res);

				_context.ChangeTracker.Clear();

				if (uncryptedRes.HowManyTrials <= 0)
				{
					return false;
				}
				uncryptedRes.HowManyTrials = AccountStatus.MaxTrials;
				var copyId = res.Id;

				var newCryptedStatus = _cryptographer.EncryptStatus(uncryptedRes);
				newCryptedStatus.Id = copyId;

				var updatedStatus = new CryptedAccountStatus() { Id = copyId };
				_context.AccStatus.Attach(updatedStatus);

				updatedStatus.CryptedInfo = newCryptedStatus.CryptedInfo;
				updatedStatus.UserId = newCryptedStatus.UserId;

				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		private async Task StatusCheck(string username)
		{
			var acc = await _context.Accounts.FirstAsync(user => user.UserName == username);

			var res = await _context.AccStatus.FirstAsync(stat => stat.UserId == acc.Id);
			var uncryptedRes = _cryptographer.DecryptStatus(res);


			string stat;
			int trials = uncryptedRes.HowManyTrials;
			if (uncryptedRes.Status == Statuses.PASSWORDS_TRIALS_OUT)
			{
				var time = uncryptedRes.TimeStamp;
				if (time.Add(AccountStatus.BlockTime) > DateTime.Now)
				{
					return;
				}
				stat = Statuses.OK;
			}
			else if (uncryptedRes.Status == Statuses.OK)
			{
				if (trials > 0)
				{
					return;
				}
				trials = AccountStatus.MaxTrials;
				stat = Statuses.PASSWORDS_TRIALS_OUT;
			}
			else
			{
				return;
			}

			_context.ChangeTracker.Clear();

			uncryptedRes.Status = stat;
			uncryptedRes.HowManyTrials = trials;
			var copyId = res.Id;

			var newCryptedStatus = _cryptographer.EncryptStatus(uncryptedRes);
			newCryptedStatus.Id = copyId;

			var updatedStatus = new CryptedAccountStatus() { Id = copyId };
			_context.AccStatus.Attach(updatedStatus);

			updatedStatus.CryptedInfo = newCryptedStatus.CryptedInfo;
			updatedStatus.UserId = newCryptedStatus.UserId;

			await _context.SaveChangesAsync();
		}
	}
}
