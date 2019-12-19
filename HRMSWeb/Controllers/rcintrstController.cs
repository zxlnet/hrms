using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Recruitment;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using Newtonsoft.Json;
using GotWell.Model.Common;
using System.Collections;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class rcintrstController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("rcintrst");
        }

        public void list()
        {
            getList<rcintrstBll, object>();
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
            getDelete<rcintrstBll,trcintrst>();
        }

        public void exportExcel()
        {
            getExportExcel<rcintrstBll, object>();
        }

        public void getRatingDtl()
        {
            getList<rcintrdtBll, object>();
        }

        public void getNew()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());

                trcintrst obj = JavaScriptConvert.DeserializeObject<trcintrst>(ht["params"].ToString());
                List<trcintrdt> listDtl = JavaScriptConvert.DeserializeObject<List<trcintrdt>>(ht["dtlparams"].ToString());

                rcintrstBll bll = new rcintrstBll();

                bll.CreateInterviewResult(obj,listDtl);

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

                trcintrst obj = JavaScriptConvert.DeserializeObject<trcintrst>(ht["params"].ToString());
                List<trcintrdt> listDtl = JavaScriptConvert.DeserializeObject<List<trcintrdt>>(ht["dtlparams"].ToString());

                rcintrstBll bll = new rcintrstBll();

                bll.UpdateInterviewResult(obj,parameters, listDtl);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void exportExcelDtl()
        {
            getExportExcel<rcintrdtBll, object>();
        }
    }
}
