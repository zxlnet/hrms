using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using GotWell.HRMS.HRMSBusiness.Attendance;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class atroshisController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("atroshis");
        }

        public void list()
        {
            getList<atroshisBll, object>();
        }

        public void New()
        {
            getNew();
        }

        public void Edit()
        {
            getEdit();
        }

        public void Delete()
        {
            getDelete<tatroshi>();
        }

        public void exportExcel()
        {
            getExportExcel<atroshisBll, object>();
        }

        public void getNew()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());

                tatroshi obj = JavaScriptConvert.DeserializeObject<tatroshi>(ht["params"].ToString());

                checkBeforeAction(obj);

                new BaseBll().DoInsert<tatroshi>(obj);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void getEdit()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());
                tatroshi obj = JavaScriptConvert.DeserializeObject<tatroshi>(ht["params"].ToString());

                checkBeforeAction(obj);

                new BaseBll().DoUpdate<tatroshi>(obj, parameters);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        protected void checkBeforeAction(tatroshi obj)
        {
            atroshisBll bll = new atroshisBll();

            if (!bll.CheckRosterHistory(obj))
                throw new UtilException("Check fail: Date range cross.", null);
        }

        public void applyto()
        {
            getApplyTo<atroshisBll, tatroshi>();
        }

    }
}
