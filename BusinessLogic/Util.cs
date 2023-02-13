
using System.Security.Cryptography;
using System.Text;

namespace BusinessLogic
{
   
    public class Util
    {

        public Random random = new Random();
        public string url1 = "http://demotay.com/admin";

        public string encry(string str)
        {
            //You should  not hard code the encryption key here
            string EncryptionKey = "encryptionkey";
            string eStr = passwordEncrypt(str, EncryptionKey);
            //MessageBox.Show(eStr);
            return eStr;
            //Mes*sageBox.Show(dStr);
        }
        public string decr(string eStr)
        {
            //You should  not hard code the encryption key here
            string EncryptionKey = "encryptionkey";
            //MessageBox.Show(eStr);
            string dStr = passwordDecrypt(eStr, EncryptionKey);
            return dStr;
            //Mes*sageBox.Show(dStr);
        }
        //Encrypting a string
        public static string passwordEncrypt(string inText, string key)
        {
            byte[] bytesBuff = Encoding.Unicode.GetBytes(inText);
            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(key, new byte[]
                { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Close();
                    }
                    inText = Convert.ToBase64String(mStream.ToArray());
                }
            }
            return inText;
        }
        //Decrypting a string
        public static string passwordDecrypt(string cryptTxt, string key)
        {
            cryptTxt = cryptTxt.Replace(" ", "+");
            byte[] bytesBuff = Convert.FromBase64String(cryptTxt);
            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Close();
                    }
                    cryptTxt = Encoding.Unicode.GetString(mStream.ToArray());
                }
            }
            return cryptTxt;
        }
        public string GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            string rrd = _rdm.Next(_min, _max).ToString();
            return rrd;
        }
        public string GenerateRandomNo2()
        {
            int _min = 0;
            int _max = 10;
            Random _rdm = new Random();
            string rrd = _rdm.Next(_min, _max).ToString();
            return rrd;
        }

        public DateTime EasternTime
        {
            get
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            }
        }
        public string ComputeSha256Hash(string rawData)
        {
            using (SHA256 Sha256Hash = SHA256.Create())
            {
                byte[] bytes = Sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}