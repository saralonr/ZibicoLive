using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZibicoLive.Entity.DTO
{
    public class DTOStepFlow
    {
        public Guid GroupKey { get; set; }
        public string SessionKey { get; set; }
        public string Username { get; set; }
        public int Status { get; set; }
    }
}
