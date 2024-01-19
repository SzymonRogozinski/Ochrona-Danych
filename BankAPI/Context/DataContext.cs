using BankAPI.Services;
using Microsoft.EntityFrameworkCore;
using Shared.DBObjects.AccountData;
using Shared.DBObjects.AccountStatus;
using Shared.DBObjects.TransferData;
using SharedClass.DBObjects.Logs;
using System.Text;

namespace BankAPI.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<CryptedTransfer> Transfers { get; set; }
        public DbSet<CryptedAccountData> Accounts { get; set; }
        public DbSet<CryptedAccountStatus> AccStatus { get; set; }
        public DbSet<CryptedLogData> Logs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CryptedTransfer>().HasKey(t => t.Id);
            modelBuilder.Entity<CryptedAccountData>().HasKey(t => t.Id);
            modelBuilder.Entity<CryptedAccountStatus>().HasKey(t => t.Id);
            modelBuilder.Entity<CryptedLogData>().HasKey(t => t.Id);


            modelBuilder.Entity<CryptedAccountData>().HasData(AddAccounts());
            modelBuilder.Entity<CryptedTransfer>().HasData(AddTransfers());
            modelBuilder.Entity<CryptedAccountStatus>().HasData(AddStatuses());
            modelBuilder.Entity<CryptedLogData>().HasData(AddLog());
        }

        private List<CryptedAccountData> AddAccounts()
        {
            var salt = Encoding.ASCII.GetBytes("QWERTYXD");
            var hmac = new System.Security.Cryptography.HMACSHA512(salt);
            var result = new List<CryptedAccountData>();
            //First account - Basia
            List<Password> ps = new List<Password>();
            ps.Add(new Password()
            {
                PasswordValue = hmac.ComputeHash(Encoding.ASCII.GetBytes("Ga*z**t*u*KBSa")),
                PasswordTempalte = "  * ** * *    ",
            });
            ps.Add(new Password()
            {
                PasswordValue = hmac.ComputeHash(Encoding.ASCII.GetBytes("*aFz*O*FuPK*S*")),
                PasswordTempalte = "*   * *    * *",
            });
            ps.Add(new Password()
            {
                PasswordValue = hmac.ComputeHash(Encoding.ASCII.GetBytes("*aFzH*tF**K*Sa")),
                PasswordTempalte = "*    *  ** *  ",
            });
            var ac = new AccountData()
            {
                UserName = "BasiaK6",
                AccountNumber = "143601139776",
                FirstName = "Barbara",
                LastName = "Kwarc",
                Balance = 500.00,
                Email = "basia.kwarc@wp.pl",
                Role = Roles.CUSTOMER,
                Pesel = "13908069463",
                CardNumber = "0951945186993010",
                CardExp = "04/30",
                CardCSC = 136,
                Passwords = ps.ToArray(),
                PasswordSalt = salt,
            };
            var cac = Cryptographer.Encrypt(ac);
            cac.Id = 1;
            result.Add(cac);

            //Second account - Krzysztof
            ps = new List<Password>();
            ps.Add(new Password()
            {
                PasswordValue = hmac.ComputeHash(Encoding.ASCII.GetBytes("*dngM**n*1G**A")),
                PasswordTempalte = "*    ** *  ** ",
            });
            ps.Add(new Password()
            {
                PasswordValue = hmac.ComputeHash(Encoding.ASCII.GetBytes("D*ng*LA*t1*4g*")),
                PasswordTempalte = " *  *  *  *  *",
            });
            ps.Add(new Password()
            {
                PasswordValue = hmac.ComputeHash(Encoding.ASCII.GetBytes("*dn*ML*nt*G4g*")),
                PasswordTempalte = "*  *  *  *   *",
            });
            ac = new AccountData()
            {
                UserName = "Konon111",
                AccountNumber = "298456755675",
                FirstName = "Krzysztof",
                LastName = "Kononowicz",
                Balance = 2.45,
                Email = "konon.tv@onet.pl",
                Role = Roles.CUSTOMER,
                Pesel = "78574588629",
                CardNumber = "8769000850987815",
                CardExp = "09/25",
                CardCSC = 701,
                Passwords = ps.ToArray(),
                PasswordSalt = salt,
            };
            cac = Cryptographer.Encrypt(ac);
            cac.Id = 2;
            result.Add(cac);

            //Third account - Admin
            ps = new List<Password>();
            ps.Add(new Password()
            {
                PasswordValue = hmac.ComputeHash(Encoding.ASCII.GetBytes("eGG*Q9O**jOx**Bn")),
                PasswordTempalte = "   *   **   **  ",
            });
            ps.Add(new Password()
            {
                PasswordValue = hmac.ComputeHash(Encoding.ASCII.GetBytes("*GG8*9OQ*j*xS*Bn")),
                PasswordTempalte = "*   *   * *  *  ",
            });
            ps.Add(new Password()
            {
                PasswordValue = hmac.ComputeHash(Encoding.ASCII.GetBytes("***8Q9OQR*OxS*Bn")),
                PasswordTempalte = "***      *   *  ",
            });
            ac = new AccountData()
            {
                UserName = "Administrator",
                AccountNumber = "609044535706",
                FirstName = "Admin",
                LastName = "Banku",
                Balance = 0.00,
                Email = "admin@admin.pl",
                Role = Roles.ADMIN,
                Pesel = "35372080528",
                CardNumber = "8189326147389612",
                CardExp = "11/30",
                CardCSC = 111,
                Passwords = ps.ToArray(),
                PasswordSalt = salt,
            };
            cac = Cryptographer.Encrypt(ac);
            cac.Id = 3;
            result.Add(cac);

            return result;
        }

        private List<CryptedTransfer> AddTransfers()
        {
            var result = new List<CryptedTransfer>();

            //Od Basia do Krzysia
            var tran = new Transfer()
            {
                Sender = 1,
                Address = 2,
                Title = "Za wycieczkę do Bonbasu",
                Price = 729.99,
                TimeStamp = new DateTime(2023, 10, 2, 16, 30, 55)
            };
            var cryptoTran = Cryptographer.EncryptTransfer(tran);
            cryptoTran.Id = 1;
            result.Add(cryptoTran);

            //Odsetki
            tran = new Transfer()
            {
                Sender = 2,
                Address = 3,
                Title = "Spłata kredytu za grudzień",
                Price = 300,
                TimeStamp = new DateTime(2023, 12, 22, 18, 12, 30)
            };
            cryptoTran = Cryptographer.EncryptTransfer(tran);
            cryptoTran.Id = 2;
            result.Add(cryptoTran);

            return result;
        }

        private List<CryptedAccountStatus> AddStatuses()
        {
            var res = new List<CryptedAccountStatus>();

            for (int i = 1; i < 4; i++)
            {
                var stat = new AccountStatus()
                {
                    UserId = i,
                    TimeStamp = DateTime.Now,
                    HowManyTrials = AccountStatus.MaxTrials,
                    Status = Statuses.OK
                };

                var cryptedStat = Cryptographer.EncryptStatus(stat);
                cryptedStat.Id = i;
                res.Add(cryptedStat);
            }
            return res;
        }

        private List<CryptedLogData> AddLog()
        {
            var log = new LogData()
            {
                Who = "System",
                When = DateTime.Now,
                IsError = false,
                Message = "Init logs"
            };
            var cryptedLog = Cryptographer.EncryptLog(log);
            cryptedLog.Id = 1;

            var logList = new List<CryptedLogData>
            {
                cryptedLog
            };

            return logList;
        }
    }
}
