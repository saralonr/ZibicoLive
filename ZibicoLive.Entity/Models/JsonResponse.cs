using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZibicoLive.Entity.Models
{
    public class JsonResponse
    {
        public bool Status { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
    }
}
