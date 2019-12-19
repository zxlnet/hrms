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
    public class rcaplinfController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("rcaplinf");
        }

        public void list()
        {
            getList<rcaplinfBll, object>();
        }

        public void New()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());
                List<trcaplfml> lstFamily = JavaScriptConvert.DeserializeObject<List<trcaplfml>>(ht["family"].ToString());
                List<trcapledu> lstEducation = JavaScriptConvert.DeserializeObject<List<trcapledu>>(ht["education"].ToString());
                List<trcaplexp> lstExperience = JavaScriptConvert.DeserializeObject<List<trcaplexp>>(ht["experience"].ToString());
                List<trcapllan> lstLanguage = JavaScriptConvert.DeserializeObject<List<trcapllan>>(ht["language"].ToString());
                List<trcaplref> lstReference = JavaScriptConvert.DeserializeObject<List<trcaplref>>(ht["reference"].ToString());

                trcaplinf obj = JavaScriptConvert.DeserializeObject<trcaplinf>(ht["params"].ToString());

                rcaplinfBll bll = new rcaplinfBll();

                bll.CreateApplicant(obj, lstFamily, lstEducation, lstExperience, lstLanguage, lstReference);

                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void Edit()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());
                List<trcaplfml> lstFamily = JavaScriptConvert.DeserializeObject<List<trcaplfml>>(ht["family"].ToString());
                List<trcapledu> lstEducation = JavaScriptConvert.DeserializeObject<List<trcapledu>>(ht["education"].ToString());
                List<trcaplexp> lstExperience = JavaScriptConvert.DeserializeObject<List<trcaplexp>>(ht["experience"].ToString());
                List<trcapllan> lstLanguage = JavaScriptConvert.DeserializeObject<List<trcapllan>>(ht["language"].ToString());
                List<trcaplref> lstReference = JavaScriptConvert.DeserializeObject<List<trcaplref>>(ht["reference"].ToString());

                trcaplinf obj = JavaScriptConvert.DeserializeObject<trcaplinf>(ht["params"].ToString());

                rcaplinfBll bll = new rcaplinfBll();

                bll.UpdateApplicant(obj, lstFamily, lstEducation, lstExperience, lstLanguage, lstReference);

                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void Delete()
        {
            getDelete<rcaplinfBll, trcaplinf>();
        }

        public void exportExcel()
        {
            getExportExcel<rcaplinfBll, object>();
        }

        public void getApplicantFamily()
        {
            getList<BaseBll, trcaplfml>();
        }

        public void getApplicantEducation()
        {
            getList<BaseBll, trcapledu>();
        }

        public void getApplicantExperience()
        {
            getList<BaseBll, trcaplexp>();
        }

        public void getApplicantLanguage()
        {
            getList<BaseBll, trcapllan>();
        }

        public void getApplicantReference()
        {
            getList<BaseBll, trcaplref>();
        }

        public void exportExcel_family()
        {
            getExportExcel<BaseBll, trcaplfml>();
        }

        public void exportExcel_education()
        {
            getExportExcel<BaseBll, trcapledu>();
        }

        public void exportExcel_experience()
        {
            getExportExcel<BaseBll, trcaplexp>();
        }

        public void exportExcel_language()
        {
            getExportExcel<BaseBll, trcapllan>();
        }

        public void exportExcel_reference()
        {
            getExportExcel<BaseBll, trcaplref>();
        }

    }
}
