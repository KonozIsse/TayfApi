using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModel
{
    public class SettingVM
    {
        public string contact_us_email { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string phone_no { get; set; }
        public string website_logo { get; set; }
        public string app_name { get; set; }
        public string facebook_url { get; set; }
        public string google_url { get; set; }
        public string twitter_url { get; set; }
        public string linked_in { get; set; }
        public string order_email { get; set; }
        public string cp_logo { get; set; }
        public string open_time { get; set; }
        public string close_time { get; set; }
        public string whatsApp { get; set; }
        public string instagram_url { get; set; }
        public string skype { get; set; }
        public string youtube_link { get; set; }
        public string press_link { get; set; }
        public string google_map_api { get; set; }
        public string hide_price { get; set; }
        public string android_app_link { get; set; }
        public string ios_app_link { get; set; }
    }
    public class SettingStoreVM
    {
        public string contact_us_email { get; set; }
        public string order_email { get; set; }
        public string google_map_api { get; set; }
        public string hide_price { get; set; }
    }

}
