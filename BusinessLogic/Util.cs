
using System.Security.Cryptography;
using System.Text;

namespace BusinessLogic
{
   
    public class Util
    {

        public Random random = new Random();
        public string url1 = "http://demotay.com/admin";

   
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