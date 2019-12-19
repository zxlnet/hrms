using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Payroll;
using GotWell.Model.Common;
using System.Collections;
using Newtonsoft.Json;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSWeb.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class prcalculController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            //prcalculBll bll = new prcalculBll();
            //List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emp.emno", ColumnValue = "00067" } };
            //bll.Calculate(parameters, null);

            return getIndex("prcalcul");
        }

        public void list()
        {
            //getList<prcalculBll, object>();
            //prcalculBll bll = new prcalculBll();

            //List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "dpcd", ColumnValue = "ZPO" } };

            //bll.Calculate(parameters);
        }

        public void exportExcel()
        {
            getExportExcel<prcalculBll, object>();
        }

        public void calculation()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                string type = ht["type"].ToString();
                string cond = ht["cond"].ToString();
                List<ColumnInfo> personalParameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["custcond"].ToString());
                string perd = ht["perd"].ToString();
                string icnj = ht["icnj"].ToString();
                string ictm = ht["ictm"].ToString();
                string pdcd = ht["pdcd"].ToString();

                prcalculBll bll = new prcalculBll();

                bll.Calculate(type, cond, personalParameters, perd, icnj, ictm,pdcd);
                message = "{status:'success',msg:'" + "Calcualte successfully." + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse("Calcualte fail.", ex, true) + "'}";
            }
            Response.Write(message);
        }
    }
}
