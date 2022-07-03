using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZibicoLive.Entity.DTO
{
    public class DTOStepFlowSummary
    {
        public Guid GroupKey { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Username { get; set; }
        public long StepID { get; set; }
        public string Question { get; set; }
        public long OptionID { get; set; }
        public string Option { get; set; }
        public long NextStepID { get; set; }
        public string NextStep { get; set; }

    }
}
