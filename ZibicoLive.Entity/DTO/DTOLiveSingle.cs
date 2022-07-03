using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZibicoLive.Entity.Models;

namespace ZibicoLive.Entity.DTO
{
    public class DTOLiveSingle
    {
        public WebSitePOCO WebSite { get; set; }
        public ConnectionUserPOCO ConnectionUser { get; set; }
        public StepPOCO Step { get; set; }
        public List<StepOptionPOCO> Options { get; set; }
    }
}
