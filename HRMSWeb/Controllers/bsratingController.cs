using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSBusiness.Master;
using System.Collections;
using GotWell.Model.Common;
using Newtonsoft.Json;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSWeb.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class bsratingController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("bsrating");
        }

        public void list()
        {
            getList<bsratingBll, tbsrating>();
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
            getDelete<bsratingBll, tbsrating>();
        }

        public void exportExcel()
        {
            getExportExcel<bsratingBll, tbsrating>();
        }

        public void exportExcelDtl()
        {
            getExportExcel<bsratingBll, tbsratdtl>();
        }

        public void getNew()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());

                tbsrating obj = JavaScriptConvert.DeserializeObject<tbsrating>(ht["params"].ToString());
                List<tbsratdtl> listDtl = JavaScriptConvert.DeserializeObject<List<tbsratdtl>>(ht["dtlparams"].ToString());

                bsratingBll bll = new bsratingBll();

                bll.CreateRating(obj, listDtl);

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

                tbsrating obj = JavaScriptConvert.DeserializeObject<tbsrating>(ht["params"].ToString());
                List<tbsratdtl> listDtl = JavaScriptConvert.DeserializeObject<List<tbsratdtl>>(ht["dtlparams"].ToString());
                List<string> listDeleted = JavaScriptConvert.DeserializeObject<List<string>>(ht["dtldeletedline"].ToString());

                bsratingBll bll = new bsratingBll();

                bll.UpdateRating(obj, listDtl, listDeleted);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void getRatingDtl()
        {
            getList<bsratingBll, tbsratdtl>();
        }
    }
}
