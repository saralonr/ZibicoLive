using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ZibicoLive.Socket.Helpers
{
    public class CommonHelpers
    {
        public static string GetAppSettingKey(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? "";
        }
    }
}