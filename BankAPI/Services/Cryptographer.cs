using Newtonsoft.Json;
using Shared.DBObjects.AccountData;
using Shared.DBObjects.AccountStatus;
using Shared.DBObjects.TransferData;
using SharedClass.DBObjects.Logs;
using System.Security.Cryptography;
using System.Text;

namespace BankAPI.Services
{
	public class Cryptographer
	{
		private readonly byte[] key;
		private readonly byte[] iv;

		private readonly byte[] logKey;
		private readonly byte[] logIv;

		private readonly Aes AES;

		private readonly IConfiguration _configuration;

		public Cryptographer(IConfiguration configuration)
		{
			this._configuration = configuration;
			this.AES = Aes.Create();

			key = Encoding.ASCII.GetBytes(_configuration.GetSection("Keys:DataKey").Value);
			iv = Encoding.ASCII.GetBytes(_configuration.GetSection("Keys:DataIv").Value);
			logKey = Encoding.ASCII.GetBytes(_configuration.GetSection("Keys:LogKey").Value);
			logIv = Encoding.ASCII.GetBytes(_configuration.GetSection("Keys:LogIv").Value);
		}

		public CryptedAccountData Encrypt(AccountData data)
		{
			PrivateData privateData = new PrivateData(data);
			var plainText = JsonConvert.SerializeObject(privateData);
			byte[] result;

			ICryptoTransform encryptor = AES.CreateEncryptor(key, iv);

			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
				{
					using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
					{
						streamWriter.Write(plainText);
					}

					result = memoryStream.ToArray();
				}
			}
			return new CryptedAccountData()
			{
				UserName = data.UserName,
				AccountNumber = data.AccountNumber,
				CryptedInfo = Convert.ToBase64String(result),
			};
		}

		public AccountData Decrypt(CryptedAccountData data)
		{
			string res;
			byte[] bytes;

			byte[] buffer = Convert.FromBase64String(data.CryptedInfo);

			using (Aes aes = Aes.Create())
			{
				aes.Key = key;
				aes.IV = iv;
				ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
						{
							res = streamReader.ReadToEnd();
						}
					}
				}
			}
			PrivateData pd = JsonConvert.DeserializeObject<PrivateData>(res);
			return new AccountData(data.UserName, data.AccountNumber, pd);
		}

		public CryptedTransfer EncryptTransfer(Transfer transfer)
		{
			PrivateTransferData privateData = new PrivateTransferData()
			{
				Title = transfer.Title,
				Price = transfer.Price,
				TimeStamp = transfer.TimeStamp,
			};
			var plainText = JsonConvert.SerializeObject(privateData);
			byte[] result;

			ICryptoTransform encryptor = AES.CreateEncryptor(key, iv);

			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
				{
					using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
					{
						streamWriter.Write(plainText);
					}

					result = memoryStream.ToArray();
				}
			}
			return new CryptedTransfer()
			{
				Address = transfer.Address,
				Sender = transfer.Sender,
				CryptedInfo = Convert.ToBase64String(result),
			};
		}

		public Transfer DecryptTransfer(CryptedTransfer data)
		{
			string res;
			byte[] bytes;

			byte[] buffer = Convert.FromBase64String(data.CryptedInfo);

			using (Aes aes = Aes.Create())
			{
				aes.Key = key;
				aes.IV = iv;
				ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
						{
							res = streamReader.ReadToEnd();
						}
					}
				}
			}
			var pd = JsonConvert.DeserializeObject<PrivateTransferData>(res);
			return new Transfer()
			{
				Sender = data.Sender,
				Address = data.Address,
				Title = pd.Title,
				Price = pd.Price,
				TimeStamp = pd.TimeStamp,
			};
		}

		public CryptedAccountStatus EncryptStatus(AccountStatus status)
		{
			PrivateAccountStatus priv = new PrivateAccountStatus()
			{
				TimeStamp = status.TimeStamp,
				HowManyTrials = status.HowManyTrials,
				Status = status.Status,
			};

			var plainText = JsonConvert.SerializeObject(priv);
			byte[] result;

			ICryptoTransform encryptor = AES.CreateEncryptor(key, iv);

			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
				{
					using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
					{
						streamWriter.Write(plainText);
					}

					result = memoryStream.ToArray();
				}
			}

			return new CryptedAccountStatus()
			{
				UserId = status.UserId,
				CryptedInfo = Convert.ToBase64String(result),
			};
		}

		public AccountStatus DecryptStatus(CryptedAccountStatus status)
		{
			string res;
			byte[] bytes;

			byte[] buffer = Convert.FromBase64String(status.CryptedInfo);

			using (Aes aes = Aes.Create())
			{
				aes.Key = key;
				aes.IV = iv;
				ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
						{
							res = streamReader.ReadToEnd();
						}
					}
				}
			}
			var pd = JsonConvert.DeserializeObject<PrivateAccountStatus>(res);

			return new AccountStatus()
			{
				UserId = status.UserId,
				TimeStamp = pd.TimeStamp,
				HowManyTrials = pd.HowManyTrials,
				Status = pd.Status,
			};
		}

		public CryptedLogData EncryptLog(LogData data)
		{
			var plainText = JsonConvert.SerializeObject(data);
			byte[] result;

			ICryptoTransform encryptor = AES.CreateEncryptor(logKey, logIv);

			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
				{
					using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
					{
						streamWriter.Write(plainText);
					}

					result = memoryStream.ToArray();
				}
			}

			return new CryptedLogData()
			{
				CryptedInfo = Convert.ToBase64String(result),
			};
		}

		public LogData DecryptLog(CryptedLogData data)
		{
			string res;
			byte[] bytes;

			byte[] buffer = Convert.FromBase64String(data.CryptedInfo);

			using (Aes aes = Aes.Create())
			{
				aes.Key = logKey;
				aes.IV = logIv;
				ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
						{
							res = streamReader.ReadToEnd();
						}
					}
				}
			}
			var pd = JsonConvert.DeserializeObject<LogData>(res);

			return pd;
		}
	}
}
