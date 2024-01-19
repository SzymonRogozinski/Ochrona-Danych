using Newtonsoft.Json;
using Shared.DBObjects.AccountData;
using Shared.DBObjects.AccountStatus;
using Shared.DBObjects.TransferData;
using SharedClass.DBObjects.Logs;
using System.Security.Cryptography;
using System.Text;

namespace BankAPI.Services
{
    public static class Cryptographer
    {
        private static byte[] key = Encoding.ASCII.GetBytes("qp)XSwz+_4`udi%'");
        private static byte[] iv = Encoding.ASCII.GetBytes("pp_<Qb&'p/ D78~L");

        private static byte[] logKey = Encoding.ASCII.GetBytes(".p`Q+]pM8mi0^]dB");
        private static byte[] logIv = Encoding.ASCII.GetBytes("T8@:qTu hR7SUUel");

        private static Aes AES = Aes.Create();

        public static CryptedAccountData Encrypt(AccountData data)
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

        public static AccountData Decrypt(CryptedAccountData data)
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

        public static CryptedTransfer EncryptTransfer(Transfer transfer)
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

        public static Transfer DecryptTransfer(CryptedTransfer data)
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

        public static CryptedAccountStatus EncryptStatus(AccountStatus status)
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

        public static AccountStatus DecryptStatus(CryptedAccountStatus status)
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

        public static CryptedLogData EncryptLog(LogData data)
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

        public static LogData DecryptLog(CryptedLogData data)
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
