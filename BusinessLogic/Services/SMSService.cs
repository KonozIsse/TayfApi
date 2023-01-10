using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface ISMSService
    {
        Task<HttpStatusCode> SendSMS(string phoneNo, int? Vervified_code, string Country_code);
    }
    public class SMSService : ISMSService
    {
        static readonly HttpClient httpClient = new HttpClient();
        public async Task<HttpStatusCode> SendSMS(string phoneNo, int? Vervified_code, string Country_code)
        {
            //https://connectsms.vodafone.com.qa/SMSConnect/SendServlet?application=http_gw1157&password=bdeyc5h3&content=Hello&destination=97455824236&source=97433&mask=ETayf

            var req = new HttpRequestMessage(HttpMethod.Get,
            "https://connectsms.vodafone.com.qa/SMSConnect/SendServlet?application=http_gw1157&password=bdeyc5h3" + "&content=your code is " + Vervified_code + "&destination="
            + Country_code + phoneNo + "&source=97433&mask=ETayf");
            await httpClient.SendAsync(req);
            return HttpStatusCode.OK;
        }
    }
}
