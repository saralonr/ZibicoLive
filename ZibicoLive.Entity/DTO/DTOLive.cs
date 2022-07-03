using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZibicoLive.Entity.Models;

namespace ZibicoLive.Entity.DTO
{
    public class DTOLive
    {
        public WebSitePOCO WebSite{ get; set; }
        public ConnectionUserPOCO ConnectionUser { get; set; }
        public List<StepPOCO> Steps { get; set; }
        public List<StepOptionPOCO> Options { get; set; }
        public List<StepOptionValuePOCO> Values { get; set; }
    }
}
