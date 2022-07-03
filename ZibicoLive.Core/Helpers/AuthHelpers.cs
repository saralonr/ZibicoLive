using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ZibicoLive.Core.Helpers
{
    public class AuthHelpers
    {
        public static readonly string authKey = ConfigurationManager.AppSettings["authKey"];
        public static readonly string authPass = ConfigurationManager.AppSettings["authPass"];

        public static bool Control(string authorizeParameter)
        {
            bool result = false;
            try
            {
                if (string.IsNullOrWhiteSpace(authorizeParameter)) return result;

                var strKey = authorizeParameter.Split(':')[0];
                var strPass = authorizeParameter.Split(':')[1];

                if (strKey == AuthHelpers.authKey && strPass == AuthHelpers.authPass) result = true;
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }

    }
}