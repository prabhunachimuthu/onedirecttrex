using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Helper
{
    public sealed class ConfigVars
    {
        public string AdminUserName = string.Empty;
        public string AdminPassword = string.Empty;
        public string apikey = string.Empty;
        public string secretkey = string.Empty;
        public TimeSlot slots = null;

        private ConfigVars()
        {
            AdminUserName = Environment.GetEnvironmentVariable("ADMIN_USERNAME");
            AdminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
            apikey = Environment.GetEnvironmentVariable("VSEE_APIKEY");
            secretkey = Environment.GetEnvironmentVariable("VSEE_SECRETKEY");

            //For local testing
            AdminUserName = "admin";
            AdminPassword = "a";
            apikey = "fghnoxeqrdecu9xyzw0lydeu86izx7824hzp4jlc3awfhvyigk1vhtiqimqknkuv";
            secretkey = "gtsf6ygej8bqngnjvffqrrowxh6yk0yoqsvrmxavbtxeh3p2izjouikp6y6olgl3";
        }
        public static ConfigVars NewInstance
        {
            get
            {
                return ConfigvarInstance.Instance;
            }
        }
        private class ConfigvarInstance
        {
            static ConfigvarInstance()
            {

            }
            internal static readonly ConfigVars Instance = new ConfigVars();
        }
    }
}
