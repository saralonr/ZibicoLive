using System;
using System.Collections.Generic;
using System.Text;

namespace ZibicoLive.Entity
{
    public class DBCon
    {
        private static string _context;

        public static string context
        {
            get
            {
                if (_context == null) _context = "DBConnection";
                return _context;
            }
        }

    }
}
