using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZibicoLive.Entity.DTO
{
    public class DTOChatLogin
    {
        public string Username { get; set; }
        public string WebSiteUniqueId { get; set; }
        public string IpAddress { get; set; }
        public string SessionKey { get; set; }
        public string ConnectionId { get; set; }
    }
}
