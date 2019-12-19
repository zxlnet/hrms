using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSData.Syst
{
    public class stsyscfgDal : BaseDal
    {
        public stsyscfgDal()
        {
        }

        public StSystemConfig GetSystemSetting()
        {
            try
            {
                List<tstsyscfg> lstcfg = GetSelectedRecords<tstsyscfg>(new List<ColumnInfo>() { }).ToList();
                StSystemConfig config = new StSystemConfig();

                for (int i = 0; i < lstcfg.Count; i++)
                {
                    string val = lstcfg[i].cfva==null?"":lstcfg[i].cfva.ToUpper();

                    switch (lstcfg[i].cfnm)
                    {
                        case "AtWDPW":
                            config.AtWDPW = val;
                            break;
                        case "IsOtEntTime":
                            config.IsOtEntTime = val;
                            break;
                        case "LvAOLL":
                            config.LvAOLL = val;
                            break;
                        case "LvCCF":
                            config.LvCCF = val;
                            break;
                        case "LvCLB":
                            config.LvCLB = val;
                            break;
                        case "PsASID":
                            config.PsASID = val;
                            break;
                        case "LvINCV":
                            config.LvINCV = val;
                            break;
                        case "OtCOTB":
                            config.OtCOTB = val;
                            break;
                        case "OtAOOTL":
                            config.OtAOOTL = val;
                            break;
                        case "PsROEIFR":
                            config.PsROEIFR = val;
                            break;
                        case "ScSBAD":
                            config.ScSBAD = val;
                            break;
                        case "PrSWHPD":
                            config.PrSWHPD = val;
                            break;
                        case "PrFLLIPC":
                            config.PrSWHPD = val;
                            break;
                        case "PrFOLIPC":
                            config.PrFOLIPC = val;
                            break;
                        case "PsSFIDF":
                            config.PsSFIDF = val;
                            break;
                        case "PsEMNOF":
                            config.PsEMNOF = val;
                            break;
                        case "PsASTESPI":
                            config.PsASTESPI = val;
                            break;
                        case "PsClubAutoConfirm":
                            config.PsClubAutoConfirm = val;
                            break;
                        case "AtDS":
                            config.AtDS = val;
                            break;
                        case "PrDFPD":
                            config.PrDFPD = val;
                            break;
                        case"PsCKID":
                            config.PsCKID = val;
                            break;
                        case "PrDFCUR":
                            config.PrDFCUR = val;
                            break;
                        case "PsOJST":
                            config.PsOJST = val;
                            break;
                    }

                }
                return config;
            }
            catch (UtilException ex)
            {
            	throw ex;
            }
            catch(Exception ex)
            {
            	throw new UtilException(ex.Message,ex);
            }
        }
    }
}
