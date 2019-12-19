using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSData.Syst;
using GotWell.LanguageResources;

namespace GotWell.HRMS.HRMSBusiness.Syst
{
    public class stsearchBll : BaseBll
    {
        stsearchDal localDal = null;
        List<StSearchResult> lstResult = null;

        public stsearchBll()
        {
            localDal = new stsearchDal();
            baseDal = localDal;
        }

        public List<StSearchResult> Search(string keywords,string mode)
        {
            lstResult = new List<StSearchResult>();
            //just support AND now
            string[] arrKeywords = keywords.Split(' ');

            //Get Staff Information
            SearchPerson(arrKeywords,mode);

            //Education
            SearchEducation(arrKeywords,mode);

            //Experience
            SearchExperience(arrKeywords, mode);

            //Event
            SearchEvent(arrKeywords, mode);

            //Address
            SearchAddress(arrKeywords, mode);

            //Contact
            SearchContact(arrKeywords, mode);

            //Contract
            SearchContract(arrKeywords, mode);

            //Detain
            SearchDetain(arrKeywords, mode);

            //Skill
            SearchSkill(arrKeywords, mode);

            return lstResult;
        }

        private void SearchPerson(string[] arrKeywords,string mode)
        {
            List<vw_employment> lstEmp = localDal.SearchPerson(arrKeywords,mode);

            for (int i = 0; i < lstEmp.Count; i++)
            {
                lstResult.Add(
                    new StSearchResult() {
                        emno=lstEmp[i].emno,
                        url="/psperson.mvc/index?menuId=M2010&helpId=psperson&emno=" + lstEmp[i].emno,
                        menu="M2010",
                        ttds=HRMSRes.ResourceManager.GetString("Public_Menu_psperson"),
                        ttle= HRMSRes.ResourceManager.GetString("Public_Label_staff") + ": " + lstEmp[i].sfid + "(" + lstEmp[i].ntnm + ")",
                        text = ">>> " 
                            + HRMSRes.ResourceManager.GetString("Personal_Label_sfid") +": " + lstEmp[i].sfid + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_flnm") + ": " + lstEmp[i].ntnm + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_ennm") + ": " + lstEmp[i].ennm + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_emst") + ": " + lstEmp[i].emst + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_brdt") + ": " + (lstEmp[i].brdt.HasValue?lstEmp[i].brdt.Value.ToString("yyyy-MM-dd"):"N/A") + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_idno") + ": " + lstEmp[i].idno + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_grnm") + ": " + lstEmp[i].grnm + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_grnm") + ": " + lstEmp[i].grnm
                    });
            }
        }

        private void SearchEducation(string[] arrKeywords, string mode)
        {
            List<tpseductn> lstEducation = localDal.SearchEducation(arrKeywords, mode);

            for (int i = 0; i < lstEducation.Count; i++)
            {
                lstResult.Add(
                    new StSearchResult()
                    {
                        emno = lstEducation[i].emno,
                        url = "/pseductn.mvc/index?menuId=M2040&helpId=pseductn&emno=" + lstEducation[i].emno,
                        menu = "M2040",
                        ttds = HRMSRes.ResourceManager.GetString("Public_Menu_pseductn"),
                        ttle = HRMSRes.ResourceManager.GetString("Public_Menu_pseductn") + " -> " + HRMSRes.ResourceManager.GetString("Public_Label_staff") + ": " + lstEducation[i].tpsperson.sfid + "(" + lstEducation[i].tpsperson.ntnm + ") ",
                        text = ">>> "
                            + lstEducation[i].frdt.ToString("yyyy-MM-dd") + " - "
                            + lstEducation[i].todt.ToString("yyyy-MM-dd") + "    "
                            + lstEducation[i].scho + "  "
                            + lstEducation[i].subj + "  "
                            + lstEducation[i].stwa + "  "
                            + lstEducation[i].remk
                    });

                #region
                //text = ">>> "
                //    + HRMSRes.ResourceManager.GetString("Personal_Label_frdt") + ": " + lstEducation[i].frdt.ToString("yyyy-MM-dd") + " - "
                //    + HRMSRes.ResourceManager.GetString("Personal_Label_todt") + ": " + lstEducation[i].todt.ToString("yyyy-MM-dd") + "  "
                //    + HRMSRes.ResourceManager.GetString("Personal_Label_scho") + ": " + lstEducation[i].scho + "  "
                //    + HRMSRes.ResourceManager.GetString("Personal_Label_subj") + ": " + lstEducation[i].subj + "  "
                //    + HRMSRes.ResourceManager.GetString("Personal_Label_stwa") + ": " + lstEducation[i].stwa + "  "
                //    + HRMSRes.ResourceManager.GetString("Public_Label_remk") + ": " + lstEducation[i].remk + ","
                //    + HRMSRes.ResourceManager.GetString("Personal_Label_grnm") + ": " + lstEducation[i].grnm + ","
                //    + HRMSRes.ResourceManager.GetString("Personal_Label_grnm") + ": " + lstEducation[i].grnm
                #endregion
            }
        }

        private void SearchExperience(string[] arrKeywords, string mode)
        {
            List<tpsexprnc> lstExperience = localDal.SearchExperience(arrKeywords, mode);

            for (int i = 0; i < lstExperience.Count; i++)
            {
                lstResult.Add(
                    new StSearchResult()
                    {
                        emno = lstExperience[i].emno,
                        url = "/psexprnc.mvc/index?menuId=M2150&helpId=psexprnc&emno=" + lstExperience[i].emno,
                        menu = "M2150",
                        ttds = HRMSRes.ResourceManager.GetString("Public_Menu_psexprnc"),
                        ttle = HRMSRes.ResourceManager.GetString("Public_Menu_psexprnc") + " -> " + HRMSRes.ResourceManager.GetString("Public_Label_staff") + ": " + lstExperience[i].tpsperson.sfid + "(" + lstExperience[i].tpsperson.ntnm + ") ",
                        text = ">>> "
                            + lstExperience[i].frdt.ToString("yyyy-MM-dd") + " - "
                            + lstExperience[i].todt.ToString("yyyy-MM-dd") + "    "
                            + lstExperience[i].cmnm + "  "
                            + lstExperience[i].fpos + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_jbds") + ": " + lstExperience[i].jbds + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_stsa") + ": " + lstExperience[i].stsa + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_saly") + ": " + lstExperience[i].saly + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_wkyr") + ": " + lstExperience[i].wkyr + "  "
                            + lstExperience[i].remk
                    });
            }
        }

        private void SearchEvent(string[] arrKeywords, string mode)
        {
            List<tpsevent> lstEvent = localDal.SearchEvent(arrKeywords, mode);

            for (int i = 0; i < lstEvent.Count; i++)
            {
                lstResult.Add(
                    new StSearchResult()
                    {
                        emno = lstEvent[i].emno,
                        url = "/psevent.mvc/index?menuId=M2080&helpId=psevent&emno=" + lstEvent[i].emno,
                        menu = "M2080",
                        ttds = HRMSRes.ResourceManager.GetString("Public_Menu_psevent"),
                        ttle = HRMSRes.ResourceManager.GetString("Public_Menu_psevent") + " -> " + HRMSRes.ResourceManager.GetString("Public_Label_staff") + ": " + lstEvent[i].tpsperson.sfid + "(" + lstEvent[i].tpsperson.ntnm + ") ",
                        text = ">>> "
                            + (lstEvent[i].evdt.HasValue ? lstEvent[i].evdt.Value.ToString("yyyy-MM-dd") : "N/A") + "  "
                            + lstEvent[i].tbsevttyp.evnm + "  "
                            + lstEvent[i].desc + "  "
                            + lstEvent[i].resn + "  "
                            + lstEvent[i].remk
                    });
            }
        }

        private void SearchAddress(string[] arrKeywords, string mode)
        {
            List<tpsaddre> lstAddress = localDal.SearchAddress(arrKeywords, mode);

            for (int i = 0; i < lstAddress.Count; i++)
            {
                lstResult.Add(
                    new StSearchResult()
                    {
                        emno = lstAddress[i].emno,
                        url = "/psaddres.mvc/index?menuId=M2020&helpId=psaddres&emno=" + lstAddress[i].emno,
                        menu = "M2020",
                        ttds = HRMSRes.ResourceManager.GetString("Public_Menu_psaddres"),
                        ttle = HRMSRes.ResourceManager.GetString("Public_Menu_psaddres") + " -> " + HRMSRes.ResourceManager.GetString("Public_Label_staff") + ": " + lstAddress[i].tpsperson.sfid + "(" + lstAddress[i].tpsperson.ntnm + ") ",
                        text = ">>> "
                            + lstAddress[i].tbsaddtyp.atnm + "  "
                            + lstAddress[i].addr + "  "
                            + lstAddress[i].tbsarea.arnm + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_isdf") + ": " +lstAddress[i].isdf + "  "
                            + lstAddress[i].remk
                    });
            }
        }

        private void SearchContact(string[] arrKeywords, string mode)
        {
            List<tpscontct> lstContact = localDal.SearchContact(arrKeywords, mode);

            for (int i = 0; i < lstContact.Count; i++)
            {
                lstResult.Add(
                    new StSearchResult()
                    {
                        emno = lstContact[i].emno,
                        url = "/pscontct.mvc/index?menuId=M2030&helpId=pscontct&emno=" + lstContact[i].emno,
                        menu = "M2030",
                        ttds = HRMSRes.ResourceManager.GetString("Public_Menu_pscontct"),
                        ttle = HRMSRes.ResourceManager.GetString("Public_Menu_pscontct") + " -> " + HRMSRes.ResourceManager.GetString("Public_Label_staff") + ": " + lstContact[i].tpsperson.sfid + "(" + lstContact[i].tpsperson.ntnm + ") ",
                        text = ">>> "
                            + lstContact[i].tbsctatyp.ctnm + "  "
                            + lstContact[i].cota1 + "  "
                            + lstContact[i].cota2 + "  "
                            + HRMSRes.ResourceManager.GetString("Master_Label_isdf") + ": " + lstContact[i].isdf + "  "
                            + lstContact[i].remk
                    });
            }
        }

        private void SearchContract(string[] arrKeywords, string mode)
        {
            List<tpscontrt> lstContract = localDal.SearchContract(arrKeywords, mode);

            for (int i = 0; i < lstContract.Count; i++)
            {
                lstResult.Add(
                    new StSearchResult()
                    {
                        emno = lstContract[i].emno,
                        url = "/pscontrt.mvc/index?menuId=M2220&helpId=pscontrt&emno=" + lstContract[i].emno,
                        menu = "M2220",
                        ttds = HRMSRes.ResourceManager.GetString("Public_Menu_pscontrt"),
                        ttle = HRMSRes.ResourceManager.GetString("Public_Menu_pscontrt") + " -> " + HRMSRes.ResourceManager.GetString("Public_Label_staff") + ": " + lstContract[i].tpsperson.sfid + "(" + lstContract[i].tpsperson.ntnm + ") ",
                        text = ">>> "
                            + lstContract[i].tbsctrtyp.ctnm + "  "
                            + lstContract[i].crnm + "  "
                            + lstContract[i].crde + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_efdt") + ": " +(lstContract[i].efdt.HasValue?lstContract[i].efdt.Value.ToString("yyyy-MM-dd"):"N/A") + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_exdt") + ": " +(lstContract[i].exdt.HasValue?lstContract[i].exdt.Value.ToString("yyyy-MM-dd"):"N/A") + "  "
                            + HRMSRes.ResourceManager.GetString("Master_Label_isdf") + ": " + (lstContract[i].isdt.HasValue ? lstContract[i].isdt.Value.ToString("yyyy-MM-dd") : "N/A") + "  "
                            + lstContract[i].remk
                    });
            }
        }


        private void SearchDetain(string[] arrKeywords, string mode)
        {
            List<tpsdetain> lstDetain = localDal.SearchDetain(arrKeywords, mode);

            for (int i = 0; i < lstDetain.Count; i++)
            {
                lstResult.Add(
                    new StSearchResult()
                    {
                        emno = lstDetain[i].emno,
                        url = "/psdetain.mvc/index?menuId=M2120&helpId=psdetain&emno=" + lstDetain[i].emno,
                        menu = "M2120",
                        ttds = HRMSRes.ResourceManager.GetString("Public_Menu_psdetain"),
                        ttle = HRMSRes.ResourceManager.GetString("Public_Menu_psdetain") + " -> " + HRMSRes.ResourceManager.GetString("Public_Label_staff") + ": " + lstDetain[i].tpsperson.sfid + "(" + lstDetain[i].tpsperson.ntnm + ") ",
                        text = ">>> "
                            + lstDetain[i].stnm + "  "
                            + lstDetain[i].dtre + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_isdt") + ": " + (lstDetain[i].isdt.ToString("yyyy-MM-dd")) + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_rtfl") + ": " + lstDetain[i].rtfl + "  "
                            + lstDetain[i].rtre + "  "
                            + HRMSRes.ResourceManager.GetString("Personal_Label_rtdt") + ": " + (lstDetain[i].rtdt.HasValue ? lstDetain[i].rtdt.Value.ToString("yyyy-MM-dd") : "N/A") + "  "
                            + lstDetain[i].remk
                    });
            }
        }

        private void SearchSkill(string[] arrKeywords, string mode)
        {
            List<tpsskill> lstSkill = localDal.SearchSkill(arrKeywords, mode);

            for (int i = 0; i < lstSkill.Count; i++)
            {
                lstResult.Add(
                    new StSearchResult()
                    {
                        emno = lstSkill[i].emno,
                        url = "/psskill.mvc/index?menuId=M2230&helpId=psskill&emno=" + lstSkill[i].emno,
                        menu = "M2230",
                        ttds = HRMSRes.ResourceManager.GetString("Public_Menu_psskill"),
                        ttle = HRMSRes.ResourceManager.GetString("Public_Menu_psskill") + " -> " + HRMSRes.ResourceManager.GetString("Public_Label_staff") + ": " + lstSkill[i].tpsperson.sfid + "(" + lstSkill[i].tpsperson.ntnm + ") ",
                        text = ">>> "
                            + lstSkill[i].tbsskltyp.stnm + "  "
                            + lstSkill[i].sknm + "  "
                            + lstSkill[i].skle + "  "
                            + lstSkill[i].remk
                    });
            }
        }

    }
}
