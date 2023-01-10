using Contracts;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic.Services
{
    public class PaymentService
    {
        protected readonly IConfiguration _configuration;
        public PaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GetPayment(decimal total, int CustomerID, string csp)
        {
            int marchentId = Convert.ToInt32(_configuration.GetSection("marchentId").Value);
            string UserId = CustomerID.ToString();
          //  ExternalDonationServices.ExternalDonationServicesClient _object = new ExternalDonationServices.ExternalDonationServicesClient();
            string _ExceptionString = "test";

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            string token = "";
            //string token = _object.AddGeneralDonationToBasket(_UserName: "tayfmerchant",
            //    _Password: "tmerchant123",
            //    _ExceptionString: ref _ExceptionString,
            //    _AccountsDonationChannelId: 702,
            //    _AccountstypeId: null,
            //_CountryId: null,
            //_IsLive: true,
            //_DonorId: null,//Int32.Parse(UserId),
            //_IsRecurrent: null,
            //_MerchantId: marchentId,
            //_MotivatorId: 240,
            //_totalAmount: Convert.ToDecimal(total));

            return token;
        }

        public string GetEncryCSP(string enc_token, string csp)
        {
            int marchentId = Convert.ToInt32(_configuration.GetSection("marchentId").Value);
            var token = enc_token;
            //ExternalDonationServices.ExternalDonationServicesClient _object = new ExternalDonationServices.ExternalDonationServicesClient();
            string _ExceptionString = "test";

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            string CSPtoken = ""; //_object.EncryptCSP(
            // _UserName: "tayfmerchant",
            // _Password: "tmerchant123",
            // _ExceptionString: ref _ExceptionString,
            // _AccountsDonationChannelId: 702,
            //_CSP: csp,
            //_Token: token, // token from AddGeneralDonationToBasket service
            //_IsLive: true,
            //_MerchantId: marchentId
            //);

            return CSPtoken;
        }


    }
}
