using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Payroll;
using GotWell.Model.HRMS;
using System.Collections;
using Newtonsoft.Json;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSCore;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class prbaalcrController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("prbaalcr");
        }

        public void list()
        {
            getList<prbaalcrBll, object>();
        }

        public void New()
        {
            getNew<tprbaalcr>();
        }

        public void Edit()
        {
            getEdit<tprbaalcr>();
        }

        public void Delete()
        {
            getDelete<tprbaalcr>();
        }

        public void exportExcel()
        {
            getExportExcel<prbaalcrBll, object>();
        }

        public void getSalaryItems()
        {
            try
            {
                string record = this.Request["record"];

                BaseBll bll = new BaseBll();
                List<tprsalitm> list = bll.GetSelectedRecords<tprsalitm>(new List<ColumnInfo>() { 
                        new ColumnInfo() { ColumnName = "bkal", ColumnValue = "Y" } 
                });
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

    }
}
