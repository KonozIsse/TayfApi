
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class objPassword
    {
        public string msg { get; set; }
        public bool valid { get; set; }
    }
    public class Util
    {
        private readonly LocService _locService;

        public Random random = new Random();
        public string url1 = "http://demotay.com/admin";
        public Util(LocService locService)
        {
            _locService = locService;
        }
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        static readonly HttpClient httpClient = new HttpClient();

        public async Task SendSMS(string phoneNo, int Vervified_code)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, "https://connectsms.vodafone.com.qa/SMSConnect/SendServlet?application=http_gw1157&password=bdeyc5h3" 
                + "&content=your code is " + Vervified_code + "&destination=" +  phoneNo + "&source=97433&mask=ETayf");

            await httpClient.SendAsync(req);
            
        }
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

        //Password Policy
        public objPassword CheckPasswordAsPolicy(string password)
        {
            string msg = "";
            if (String.IsNullOrEmpty(password) || password.Length < 12)
            {
                msg = _locService.GetLocalizedStringValue("passwordLength") + ","; 
            }
            //At least one upper case English letter, 
            string upperPattern = @"(?=.*?[A-Z])";
            //At least one lower case English letter, 
            string lowerPattern = @"(?=.*?[a-z])";
            //At least one digit, 
            string digitPattern = @"(?=.*?[0-9])";
            //At least one special character,    
            string specialPattern = @"(?=.*?[#?!@$%^&*-])";
            int validCount = 0;

            if (!Regex.IsMatch(password, upperPattern))
            {
                msg += _locService.GetLocalizedStringValue("PasswordUpperCase") +","; 
            }
            else
            {
                validCount += 1;
            }

            if (!Regex.IsMatch(password, lowerPattern))
            {
                msg += _locService.GetLocalizedStringValue("PasswordLowerCase") +",";
            }
            else
            {
                validCount += 1;
            }

            if (!Regex.IsMatch(password, digitPattern))
            {
                msg += _locService.GetLocalizedStringValue("PasswordDigit") +",";
            }
            else
            {
                validCount += 1;
            }

            if (!Regex.IsMatch(password, specialPattern))
            {
                msg += _locService.GetLocalizedStringValue("PasswordSpecial");
            }
            else
            {
                validCount += 1;
            }

            if(validCount >= 3)
            {
                return new objPassword { msg = msg, valid = true };
            }
            else
            {
                return new objPassword { msg = msg, valid = false };
            }
        }

    }
}