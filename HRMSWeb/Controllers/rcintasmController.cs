using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Recruitment;
using GotWell.Model.HRMS;
using System.Collections;
using Newtonsoft.Json;
using GotWell.Model.Common;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.HRMS.HRMSBusiness.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class rcintasmController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("rcintasm");
        }

        public void list()
        {
            getList<rcintasmBll, trcintasm>();
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
            getDelete<rcintasmBll, trcintasm>();
        }

        public void exportExcel()
        {
            getExportExcel<rcintasmBll, trcintasm>();
        }

        public void exportExcelDtl()
        {
            getExportExcel<rcintadtBll, object>();
        }

        public void getNew()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());

                trcintasm obj = JavaScriptConvert.DeserializeObject<trcintasm>(ht["params"].ToString());
                List<trcintadt> listDtl = JavaScriptConvert.DeserializeObject<List<trcintadt>>(ht["dtlparams"].ToString());

                rcintasmBll bll = new rcintasmBll();

                bll.CreateRecruitmentAssessment(obj, listDtl);

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

                trcintasm obj = JavaScriptConvert.DeserializeObject<trcintasm>(ht["params"].ToString());
                List<trcintadt> listDtl = JavaScriptConvert.DeserializeObject<List<trcintadt>>(ht["dtlparams"].ToString());
                List<string> listDeleted = JavaScriptConvert.DeserializeObject<List<string>>(ht["dtldeletedline"].ToString());

                rcintasmBll bll = new rcintasmBll();

                bll.UpdateRecruitmentAssessment(obj, listDtl,listDeleted);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void getRecruitmentAssessmentDtl()
        {
            getList<rcintadtBll, object>();
        }
    }
}
