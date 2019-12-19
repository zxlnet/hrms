using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.Model.HRMS;
using Newtonsoft.Json;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSBusiness.Master;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.LanguageResources;
using System.Collections;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class DropDownController : Controller
    {
        #region executeAction
        public ActionResult executeAction()
        {

            string message = string.Empty;
            try
            {
                string record = this.Request["record"];
                string action = JavaScriptConvert.DeserializeObject<string>(record);


                if (!action.Equals(string.Empty))
                    return this.RedirectToAction(action);

            }
            catch (Exception ex)
            {
            }

            return null;
        }
        #endregion

        #region getClub
        public void getClub()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsclub> clubList = bll.GetSelectedRecords<tbsclub>(new List<ColumnInfo>() { });
                var clnmList = (from p in clubList
                                select new ValueInfo { ValueField = p.clcd, DisplayField = p.clnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(clnmList);
                json = "{results:" + clnmList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ListValidPersonal
        public void ListValidPersonal()
        {
            psemplymBll bll = new psemplymBll();

            List<ColumnInfo> parameters = new List<ColumnInfo>() { };

            List<vw_employment> list = bll.GetValidEmployment();

            var q = from p in list
                    select new { sfnm = (p.sfid + " - " + p.ntnm), emno = p.emno };

            string json = JavaScriptConvert.SerializeObject(q);
            Response.Write("{results:1" + ",rows:" + json + "}");
        }
        #endregion

        #region getInteger
        public void getInteger()
        {
            List<ValueInfo> parameters = new List<ValueInfo>() { };

            for (int i = 1; i <= 100; i++)
            {
                ValueInfo col = new ValueInfo() { DisplayField = i.ToString(), ValueField = i.ToString() };
                parameters.Add(col);
            }

            string json = JavaScriptConvert.SerializeObject(parameters);
            Response.Write("{results:1" + ",rows:" + json + "}");
        }
        #endregion

        #region getPerson
        public void getPerson()
        {
            List<ColumnInfo> parameters = new List<ColumnInfo>() { };

            //List<tpsperson> list = bll.GetSelectedRecords<tpsperson>(parameters);
            //var q = from p in list
            //        select new { DisplayField = (p.sfid + " - " + p.ntnm), ValueField = p.emno };

            psemplymBll bll = new psemplymBll();

            List<vw_employment> dataList = bll.GetValidEmployment();
            var q = from p in dataList
                    select new { DisplayField = (p.sfid + " - " + p.ntnm), ValueField = p.emno };

            string json = JavaScriptConvert.SerializeObject(q);
            Response.Write("{results:1" + ",rows:" + json + "}");
        }
        #endregion

        #region ListAllPersonal
        public void ListAllPersonal()
        {
            pspersonBll bll = new pspersonBll();

            List<ColumnInfo> parameters = new List<ColumnInfo>() { };

            List<tpsperson> list = bll.GetSelectedRecords<tpsperson>(parameters);

            var q = from p in list
                    select new { sfnm = (p.sfid + " - " + p.ntnm), emno = p.emno };

            string json = JavaScriptConvert.SerializeObject(q);
            Response.Write("{results:1" + ",rows:" + json + "}");
        }
        #endregion

        #region getYesNoFlag
        public void getYesNoFlag()
        {
            try
            {
                List<ValueInfo> valueInfo = new List<ValueInfo>() { new ValueInfo {DisplayField="Yes",ValueField="Y" }, 
                                                                    new ValueInfo {DisplayField="No",ValueField="N" } };
                string json = JavaScriptConvert.SerializeObject(valueInfo);
                json = "{results:" + valueInfo.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region getChangeField
        public void getChangeField()
        {
            try
            {
                //BaseBll bll = new BaseBll();
                //List<tstmastdf> fieldList = bll.GetSelectedRecords<tstmastdf>(new List<ColumnInfo>() { new ColumnInfo { ColumnName = "table_name", ColumnValue = "tpsemplym" } });

                //var fieldNameList = (from p in fieldList
                //                     select new ValueInfo
                //                     {
                //                         ValueField = p.conm,
                //                         DisplayField = HRMSRes.ResourceManager.GetString(p.rsid) == null ? p.conm : HRMSRes.ResourceManager.GetString(p.rsid)
                //                     }).ToList();

                //string json = JavaScriptConvert.SerializeObject(fieldNameList);
                //json = "{results:" + fieldNameList.Count + ",rows:" + json + "}";

                //Response.Output.Write(json);

                BaseBll bll = new BaseBll();
                List<tstdefcfg> list = bll.GetSelectedRecords<tstdefcfg>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               where (p.dffr == "Empchg")
                               select new ValueInfo
                               {
                                   ValueField = p.dfnm,
                                   DisplayField = HRMSRes.ResourceManager.GetString(p.dfrs),
                                   MiscField1 = p.dasc,
                                   MiscField2=p.finm
                               }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region getStaffType
        public void getStaffType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsstftyp> list = bll.GetSelectedRecords<tbsstftyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.stcd, DisplayField = p.stnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region getEmst
        public void getEmst()
        {
            try
            {
                //BaseBll bll = new BaseBll();
                //List<tbse> list = bll.GetSelectedRecords<tbsstftyp>(new List<ColumnInfo>() { });
                //var finList = (from p in list
                //               select new ValueInfo { ValueField = p.stcd, DisplayField = p.stafftypename }).ToList();

                //string json = JavaScriptConvert.SerializeObject(finList);
                string json = "{results:0,rows:[]}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region getDivision
        public void getDivision()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsdivion> list = bll.GetSelectedRecords<tbsdivion>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.dvcd, DisplayField = p.dvnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region getBusiness
        public void getBusiness()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsbusne> list = bll.GetSelectedRecords<tbsbusne>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.bscd, DisplayField = p.bsnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region getDepartment
        public void getDepartment()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsdepmnt> list = bll.GetSelectedRecords<tbsdepmnt>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.dpcd, DisplayField = p.dpnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region getGrade
        public void getGrade()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsgrade> list = bll.GetSelectedRecords<tbsgrade>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.grcd, DisplayField = p.grnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region getGradeLevel
        public void getGradeLevel()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsgrdlev> list = bll.GetSelectedRecords<tbsgrdlev>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.glcd, DisplayField = p.glnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region getPosition
        public void getPosition()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbspstion> list = bll.GetSelectedRecords<tbspstion>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.pscd, DisplayField = p.psnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public void getPositionLevel()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsposlev> list = bll.GetSelectedRecords<tbsposlev>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.plcd, DisplayField = p.plnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getJobClass()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsjobcl> list = bll.GetSelectedRecords<tbsjobcl>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.jccd, DisplayField = p.jcnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getJobType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsjobtyp> list = bll.GetSelectedRecords<tbsjobtyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.jtcd, DisplayField = p.jtnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getWorkGroup()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbswrkgrp> list = bll.GetSelectedRecords<tbswrkgrp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.wgcd, DisplayField = p.wgnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getWorkSite()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbswrkste> list = bll.GetSelectedRecords<tbswrkste>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.wscd, DisplayField = p.wsnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getCostCenter()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbscosctr> list = bll.GetSelectedRecords<tbscosctr>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.cccd, DisplayField = p.ccnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getSC()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbssc> list = bll.GetSelectedRecords<tbssc>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.sccd, DisplayField = p.scnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getCalendar()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tatcaldar> list = bll.GetSelectedRecords<tatcaldar>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.clcd, DisplayField = p.cdnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getPayType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbspaytyp> list = bll.GetSelectedRecords<tbspaytyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ptcd, DisplayField = p.ptnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getRoster()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tatroster> list = bll.GetSelectedRecords<tatroster>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.rscd.ToString(), DisplayField = p.rsnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getctcd()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbschgtyp> list = bll.GetSelectedRecords<tbschgtyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ctcd, DisplayField = p.ctnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getTermReason()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsterrsn> list = bll.GetSelectedRecords<tbsterrsn>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.trcd, DisplayField = p.trnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getTermType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbstertyp> list = bll.GetSelectedRecords<tbstertyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ttcd, DisplayField = p.ttnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getContractType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsctrtyp> list = bll.GetSelectedRecords<tbsctrtyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ctcd, DisplayField = p.ctnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getOverTimeType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tottype> list = bll.GetSelectedRecords<tottype>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.otcd, DisplayField = p.otnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getHolidayType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tathldtyp> list = bll.GetSelectedRecords<tathldtyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.htcd, DisplayField = p.htnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getShift()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tatshift> list = bll.GetSelectedRecords<tatshift>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.sfcd, DisplayField = p.sfnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getLeaveType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tlvleatyp> list = bll.GetSelectedRecords<tlvleatyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ltcd, DisplayField = p.ltnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getLeaveReason()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tlvlearsn> list = bll.GetSelectedRecords<tlvlearsn>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.lrcd, DisplayField = p.lrnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getLvDef()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tstdefcfg> list = bll.GetSelectedRecords<tstdefcfg>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               where p.dfnm != "E" && p.dfnm != "Y"
                               && ((p.dffr == "*") || (p.dffr == "Leave"))
                               select new ValueInfo
                               {
                                   ValueField = p.dfnm,
                                   DisplayField = HRMSRes.ResourceManager.GetString(p.dfrs),
                                   MiscField1 = p.dasc
                               }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getOTDef()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tstdefcfg> list = bll.GetSelectedRecords<tstdefcfg>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               where p.dfnm != "E" && p.dfnm != "Y"
                               && ((p.dffr == "*") || (p.dffr == "Overtime"))
                               select new ValueInfo
                               {
                                   ValueField = p.dfnm,
                                   DisplayField = HRMSRes.ResourceManager.GetString(p.dfrs),
                                   MiscField1 = p.dasc
                               }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void getLvdffrLimit()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tstdefcfg> list = bll.GetSelectedRecords<tstdefcfg>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               where (p.dffr == "*" || p.dffr == "Leave")
                               select new ValueInfo
                               {
                                   ValueField = p.dfnm,
                                   DisplayField = HRMSRes.ResourceManager.GetString(p.dfrs),
                                   MiscField1 = p.dasc
                               }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getOTdffrLimit()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tstdefcfg> list = bll.GetSelectedRecords<tstdefcfg>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               where (p.dffr == "*" || p.dffr == "Overtime")
                               select new ValueInfo
                               {
                                   ValueField = p.dfnm,
                                   DisplayField = HRMSRes.ResourceManager.GetString(p.dfrs),
                                   MiscField1 = p.dasc
                               }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getValidPeriod()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tstperiod> list = bll.GetSelectedRecords<tstperiod>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.perd, DisplayField = p.perd }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getArea()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsarea> list = bll.GetSelectedRecords<tbsarea>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.arcd, DisplayField = p.arnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getAddressType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsaddtyp> list = bll.GetSelectedRecords<tbsaddtyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.atcd, DisplayField = p.atnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getBank()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsbank> list = bll.GetSelectedRecords<tbsbank>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.bkcd, DisplayField = p.bknm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getRelationShip()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsreltyp> list = bll.GetSelectedRecords<tbsreltyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.rtcd, DisplayField = p.rtnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getCountry()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbscontry> list = bll.GetSelectedRecords<tbscontry>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.cocd, DisplayField = p.conm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getCertifType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbscertyp> list = bll.GetSelectedRecords<tbscertyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ctcd, DisplayField = p.ctnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getChangeType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbschgtyp> list = bll.GetSelectedRecords<tbschgtyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ctcd, DisplayField = p.ctnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getChangeTypeForEmployment()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbschgtyp> list = bll.GetSelectedRecords<tbschgtyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               where (p.ctcd != "sys$0") && (p.ctcd != "sys$2") && (p.ctcd != "sys$3")
                               select new ValueInfo { ValueField = p.ctcd, DisplayField = p.ctnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getCurrency()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbscurncy> list = bll.GetSelectedRecords<tbscurncy>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.crcd, DisplayField = p.crnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getContactType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsctatyp> list = bll.GetSelectedRecords<tbsctatyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ctcd, DisplayField = p.ctnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getEducationLevel()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsedulev> list = bll.GetSelectedRecords<tbsedulev>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.elcd, DisplayField = p.elnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getEventType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsevttyp> list = bll.GetSelectedRecords<tbsevttyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.evcd, DisplayField = p.evnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getBusinessNature()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsbusntr> list = bll.GetSelectedRecords<tbsbusntr>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.bncd, DisplayField = p.bnnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getInsuranceSchema()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsinsscm> list = bll.GetSelectedRecords<tbsinsscm>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.iscd, DisplayField = p.isnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getInsuranceCompany()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsinscmp> list = bll.GetSelectedRecords<tbsinscmp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.iccd, DisplayField = p.icnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getInsuranceType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsinstyp> list = bll.GetSelectedRecords<tbsinstyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.itcd, DisplayField = p.itnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getQualType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsqlfctn> list = bll.GetSelectedRecords<tbsqlfctn>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.qfcd, DisplayField = p.qfnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getTrainingType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbstratyp> list = bll.GetSelectedRecords<tbstratyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ttcd, DisplayField = p.ttnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getWelfareType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbswlftyp> list = bll.GetSelectedRecords<tbswlftyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.wtcd, DisplayField = p.wtnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getPolity()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbspolity> list = bll.GetSelectedRecords<tbspolity>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.plcd, DisplayField = p.plnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getNation()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsnation> list = bll.GetSelectedRecords<tbsnation>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.nacd, DisplayField = p.nanm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getReligion()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsrligon> list = bll.GetSelectedRecords<tbsrligon>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.rgcd, DisplayField = p.rgnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getSkillType()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsskltyp> list = bll.GetSelectedRecords<tbsskltyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.stcd, DisplayField = p.stnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getRating()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tbsrating> list = bll.GetSelectedRecords<tbsrating>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.racd, DisplayField = p.ranm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getRatingDtl()
        {
            try
            {
                string record = this.Request["record"];

                if (record == null) return;

                BaseBll bll = new BaseBll();
                List<tbsratdtl> list = bll.GetSelectedRecords<tbsratdtl>(new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "racd", ColumnValue = record } });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.rtcd, DisplayField = p.rtnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getSalaryItemFlag()
        {
            try
            {
                string record = this.Request["record"];

                BaseBll bll = new BaseBll();
                List<tprsaitfg> list = bll.GetSelectedRecords<tprsaitfg>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.sifg, DisplayField = p.dscr }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getSalaryType()
        {
            try
            {
                string record = this.Request["record"];

                BaseBll bll = new BaseBll();
                List<tprsaltyp> list = bll.GetSelectedRecords<tprsaltyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.stcd, DisplayField = p.stnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getVariables()
        {
            try
            {
                string record = this.Request["record"];

                BaseBll bll = new BaseBll();
                List<tprvarble> list = bll.GetSelectedRecords<tprvarble>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.vacd, DisplayField = p.vanm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getSalaryItems()
        {
            try
            {
                string record = this.Request["record"];

                BaseBll bll = new BaseBll();
                List<tprsalitm> list = bll.GetSelectedRecords<tprsalitm>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.itcd, DisplayField = p.itnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getPayDay()
        {
            try
            {
                string record = this.Request["record"];

                BaseBll bll = new BaseBll();
                List<tprpayday> list = bll.GetSelectedRecords<tprpayday>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.pdcd, DisplayField = p.padt.ToString() }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getFormula()
        {
            try
            {
                string record = this.Request["record"];

                BaseBll bll = new BaseBll();
                List<tprfmular> list = bll.GetSelectedRecords<tprfmular>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.frcd, DisplayField = p.frnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getPubRuleSet()
        {
            try
            {
                string record = this.Request["record"];

                BaseBll bll = new BaseBll();
                List<tprpubrst> list = bll.GetSelectedRecords<tprpubrst>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.rscd, DisplayField = p.rsnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getCondition()
        {
            try
            {
                string record = this.Request["record"];

                BaseBll bll = new BaseBll();
                List<tstcondtn> list = bll.GetSelectedRecords<tstcondtn>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.cdno, DisplayField = p.cdnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getAccount()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                BaseBll bll = new BaseBll();
                List<tpsaccont> list = bll.GetSelectedRecords<tpsaccont>(new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = ht["emno"].ToString() } });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.acno, DisplayField = p.acno }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getRun()
        {
            try
            {
                string record = this.Request["record"];

                BaseBll bll = new BaseBll();
                List<tprrun> list = bll.GetSelectedRecords<tprrun>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.rnno, DisplayField = p.rnno }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getHeadCoungCfg()
        {
            try
            {
                string record = this.Request["record"];

                BaseBll bll = new BaseBll();
                List<tpshctcfg> list = bll.GetSelectedRecords<tpshctcfg>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.hccd, DisplayField = p.hcnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getHeadCountDef()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tstdefcfg> list = bll.GetSelectedRecords<tstdefcfg>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               where (p.dffr == "*" || p.dffr == "HeadCount")
                               select new ValueInfo
                               {
                                   ValueField = p.dfnm,
                                   DisplayField = HRMSRes.ResourceManager.GetString(p.dfrs),
                                   MiscField1 = p.dasc
                               }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getCompanyTraining()
        {
            try
            {
                string record = this.Request["record"];

                BaseBll bll = new BaseBll();
                List<ttrtraing> list = bll.GetSelectedRecords<ttrtraing>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.trcd, DisplayField = p.trnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getSex()
        {
            try
            {
                string record = this.Request["record"];

                List<ValueInfo> finList = new List<ValueInfo>() { 
                    new ValueInfo() { ValueField = "M", DisplayField = HRMSRes.ResourceManager.GetString("Public_Label_Male") },
                    new ValueInfo() { ValueField = "M", DisplayField = HRMSRes.ResourceManager.GetString("Public_Label_Female") }
                };

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getRecruitmentType()
        {
            try
            {
                string record = this.Request["record"];

                BaseBll bll = new BaseBll();
                List<trcrcttyp> list = bll.GetSelectedRecords<trcrcttyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.rtcd, DisplayField = p.rtnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getHeadcountControl()
        {
            try
            {
                string record = this.Request["record"];

                List<ValueInfo> finList = new List<ValueInfo>() { 
                    new ValueInfo() { ValueField = "I", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_ihcnt") },
                    new ValueInfo() { ValueField = "O", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_ohcnt") },
                    new ValueInfo() { ValueField = "U", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_unknown") }
                };

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getWhereHiring()
        {
            try
            {
                string record = this.Request["record"];

                List<ValueInfo> finList = new List<ValueInfo>() { 
                    new ValueInfo() { ValueField = "I", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_internal") },
                    new ValueInfo() { ValueField = "E", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_external") },
                    new ValueInfo() { ValueField = "A", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_any") }
                };

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getRecruitmentAppStatus()
        {
            try
            {
                string record = this.Request["record"];

                List<ValueInfo> finList = new List<ValueInfo>() { 
                    new ValueInfo() { ValueField = "Created", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_created") },
                    new ValueInfo() { ValueField = "Approved", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_approved") },
                    new ValueInfo() { ValueField = "Rejected", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_rejected") },
                    new ValueInfo() { ValueField = "Cancelled", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_cancelled") },
                    new ValueInfo() { ValueField = "Processing", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_processing") },
                    new ValueInfo() { ValueField = "Completed", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_completed") },
                    new ValueInfo() { ValueField = "Expired", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_expired") }
                };

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getRecruitmentApplication()
        {
            try
            {
                string record = this.Request["record"];

                BaseBll bll = new BaseBll();
                List<trcapplic> list = bll.GetSelectedRecords<trcapplic>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               where (p.stus == "Created" || p.stus =="Approved" || p.stus =="Processing")
                               select new ValueInfo { ValueField = p.apno, DisplayField = p.apno,MiscField1=p.jbcd,MiscField2=p.jbnm }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getRecruitmentJobType()
        {
            try
            {
                string record = this.Request["record"];

                List<ValueInfo> finList = new List<ValueInfo>() { 
                    new ValueInfo() { ValueField = "L", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_long") },
                    new ValueInfo() { ValueField = "S", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_short") },
                    new ValueInfo() { ValueField = "A", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_any") }
                };

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getRecruitmentAssessment()
        {
            try
            {
                string record = this.Request["record"];

                BaseBll bll = new BaseBll();
                List<trcintasm> list = bll.GetSelectedRecords<trcintasm>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.iacd, DisplayField = p.iade }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getRecruitmentScheduleType()
        {
            try
            {
                string record = this.Request["record"];

                List<ValueInfo> finList = new List<ValueInfo>() { 
                    new ValueInfo() { ValueField = "Scheduled", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_Scheduled") },
                    new ValueInfo() { ValueField = "Unscheduled", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_Unscheduled") }
                };

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getInterviewResult()
        {
            try
            {
                string record = this.Request["record"];

                List<ValueInfo> finList = new List<ValueInfo>() { 
                    new ValueInfo() { ValueField = "Considerable", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_Considerable") },
                    new ValueInfo() { ValueField = "Probational", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_Probational") },
                    new ValueInfo() { ValueField = "Re-interview", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_Reinterview") },
                    new ValueInfo() { ValueField = "Rejected", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_rejc") },
                    new ValueInfo() { ValueField = "Cancelled", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_canc") },
                    new ValueInfo() { ValueField = "Absent", DisplayField = HRMSRes.ResourceManager.GetString("Recruitment_Label_Absent") }
                };
                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Excel Report
        public void getDefForExcelReport()
        {
            string record = this.Request["record"];
            switch (record)
            {
                case "Personal":
                    getDefForPersonnelExcelReport();
                    break;
                case "Payroll":
                    getDefForPayrollExcelReport();
                    break;
                case "PayrollBankAlloc":
                    getDefForPayrollBankAllocExcelReport();
                    break;
                default:
                    getDefForPersonnelExcelReport();
                    break;
            }
        }
        public void getDefForPersonnelExcelReport()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tstdefcfg> list = bll.GetSelectedRecords<tstdefcfg>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               where (p.dffr == "ExcelReportPersonnel")
                               select new ValueInfo
                               {
                                   ValueField = p.dfnm,
                                   DisplayField = HRMSRes.ResourceManager.GetString(p.dfrs),
                                   MiscField1 = p.dasc,
                                   MiscField2 = p.finm,
                                   MiscField3 = p.fity
                               }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getDefForPayrollExcelReport()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tstdefcfg> list = bll.GetSelectedRecords<tstdefcfg>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               where (p.dffr == "ExcelReportPersonnel" || p.dffr == "ExcelReportPayroll")
                               select new ValueInfo
                               {
                                   ValueField = p.dfnm,
                                   DisplayField = HRMSRes.ResourceManager.GetString(p.dfrs),
                                   MiscField1 = p.dasc,
                                   MiscField2 = p.finm,
                                   MiscField3 = p.fity
                               }).ToList();

                List<tprsalitm> lstSalItm = bll.GetSelectedRecords<tprsalitm>(new List<ColumnInfo>() { });
                var payrollList = (from p in lstSalItm
                                   select new ValueInfo
                                   {
                                       ValueField = p.itcd,
                                       DisplayField = p.itnm,
                                       MiscField1 = "salitm",
                                       MiscField2 = p.itcd,
                                       MiscField3 = "float"
                                   }).ToList();

                for (int i = 0; i < payrollList.Count; i++)
                {
                    finList.Add(payrollList[i]);
                }

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getDefForPayrollBankAllocExcelReport()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<tstdefcfg> list = bll.GetSelectedRecords<tstdefcfg>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               where (p.dffr == "ExcelReportPersonnel" || p.dffr == "ExcelReportBankAllocation")
                               select new ValueInfo
                               {
                                   ValueField = p.dfnm,
                                   DisplayField = HRMSRes.ResourceManager.GetString(p.dfrs),
                                   MiscField1 = p.dasc,
                                   MiscField2 = p.finm,
                                   MiscField3 = p.fity
                               }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void getExcelReportDef()
        {
            try
            {
                BaseBll bll = new BaseBll();
                List<trpexrpdf> list = bll.GetSelectedRecords<trpexrpdf>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo
                               {
                                   ValueField = p.rpcd,
                                   DisplayField = p.rpnm,
                               }).ToList();

                string json = JavaScriptConvert.SerializeObject(finList);
                json = "{results:" + finList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}

