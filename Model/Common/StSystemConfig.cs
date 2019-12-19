using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.Model.Common
{
    public class StSystemConfig
    {
        public string PsASID { get; set; }
        public string IsOtEntTime { get; set; }
        public string AtWDPW { get; set; }
        public string LvCLB { get; set; }
        public string LvCCF { get; set; }
        public string LvAOLL { get; set; }
        public string LvINCV { get; set; }
        public string OtCOTB { get; set; }
        public string OtAOOTL { get; set; }
        public string PsROEIFR { get; set; }
        public string ScSBAD { get; set; }
        public string PrSWHPD { get; set; }
        public string PrFLLIPC { get; set; }
        public string PrFOLIPC { get; set; }
        public string PsSFIDF { get; set; }
        public string PsEMNOF { get; set; }
        public string PsASTESPI { get; set; }
        public string PsClubAutoConfirm { get; set; }
        public string AtDS { get; set; }
        public string PrDFPD { get; set; }
        public string PsCKID { get; set; }
        public string PrDFCUR { get; set; }
        public string PsOJST { get; set; }

        public StSystemConfig()
        {
            PsASID = "N";
            IsOtEntTime = "N";
            AtWDPW = "5";
            LvCLB = "N";
            LvCCF = "N";
            LvAOLL = "N";
            LvINCV = "N";
            OtCOTB = "N";
            OtAOOTL = "N";
            PsROEIFR = "N";
            ScSBAD = "N";
            PrFLLIPC = "N";
            PrFOLIPC = "N";
            PsASTESPI = "N";
            PsClubAutoConfirm = "N";
            AtDS = "2";
            PsEMNOF = "Integer(6)";
            PsSFIDF = "Integer(6)";
            PrDFPD = "0";
            PsCKID = "N";
            PrDFCUR = string.Empty;
            PsOJST = string.Empty;
        }
    }
}
