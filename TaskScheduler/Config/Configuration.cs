using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler.Config
{
    public sealed class Configuration
    {        
        public Accounting[] Account { get; set; }
    }

    public sealed class Accounting
    {
        public string AuthorisationToken { get; set; }
        public string InvestorId { get; set; }
        public string PortfolioId { get; set; }
        public string ActorId { get; set; }
    }

}
