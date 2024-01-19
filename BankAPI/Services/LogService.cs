using BankAPI.Context;
using Microsoft.EntityFrameworkCore;
using SharedClass.DBObjects.Logs;

namespace BankAPI.Services
{
    public interface ILogService
    {
        Task AddLog(string who, bool isError, string messsage);
        Task<List<LogData>> GetLogs();
    }

    public class LogService : ILogService
    {
        private readonly DataContext _context;

        public LogService(DataContext context)
        {
            this._context = context;
        }

        public async Task AddLog(string who, bool isError, string messsage)
        {
            var log = new LogData()
            {
                Who = who,
                When = DateTime.Now,
                IsError = isError,
                Message = messsage
            };

            var cryptedLog = Cryptographer.EncryptLog(log);
            await _context.Logs.AddAsync(cryptedLog);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LogData>> GetLogs()
        {
            var cryptedLogs = await _context.Logs.ToListAsync();
            var logs = new List<LogData>();
            foreach (var item in cryptedLogs)
            {
                logs.Add(Cryptographer.DecryptLog(item));
            }

            return logs;
        }
    }
}
