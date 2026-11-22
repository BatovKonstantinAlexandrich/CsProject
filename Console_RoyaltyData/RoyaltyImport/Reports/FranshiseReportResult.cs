using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_RoyaltyData.RoyaltyImport.Reports
{
   
    public class FranshiseReportResult
    {
        public int div_id { get; set; }
        public string as_fr { get; set; } = "";
        public string div_name { get; set; } = "";
        public int yy { get; set; }
        public int mm { get; set; }
        public decimal sum_works { get; set; }
        public decimal sum_parts { get; set; }
        public int cnt_run { get; set; }
        public int pret { get; set; }
        public double pret_proc { get; set; }
        public decimal sum_pur_other { get; set; }
        public decimal sum_pur_ea { get; set; }
        public decimal sum_pur_all { get; set; }
        public decimal proc_pur { get; set; }
        public decimal sum_sale { get; set; }
        public decimal sum_sale_all { get; set; }
    }
}

