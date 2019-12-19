using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using GotWell.Common;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using GotWell.LanguageResources;

namespace GotWell.HRMS.HRMSCore.LogControl
{
    public class stactlogBll : BaseBll
    {
        //stactlogDal dal = null;

        //public stactlogBll()
        //{
        //    dal = new stactlogDal();
        //    baseDal = dal;
        //}

        public string WriteLog(string action, object newobj,object oldobj)
        {
            string rfid = string.Empty;
            string lgtx = string.Empty;

            object obj = oldobj == null ? newobj : oldobj;
            PropertyInfo prop = obj.GetType().GetProperty("rfid");
            if (prop != null)
            {
                rfid = prop.GetValue(obj, null) == null ? string.Empty : prop.GetValue(obj, null).ToString();
            }

            if (rfid != string.Empty)
            {
                //Log
                tstactlog actlog = new tstactlog();
                actlog.actn = action;
                actlog.lgid = Function.GetGUID();
                actlog.lgtm = DateTime.Now;

                if (action == "Update")
                    actlog.lgtx = BuildOperationDescUpdate(newobj, oldobj);
                else if (action == "Add")
                    actlog.lgtx = BuildOperationDesc(newobj,action);
                else if (action == "Delete")
                    actlog.lgtx = BuildOperationDesc(newobj, action);
                        

                actlog.lgur = Function.GetCurrentUser();
                actlog.rfid = rfid;
                actlog.tbnm = obj.GetType().Name;
                baseDal.DoInsert<tstactlog>(actlog);

                lgtx = actlog.lgtx;
            }

            return lgtx;
        }

        private string BuildOperationDesc(object obj,string action)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            string operationDesc = string.Empty;

            operationDesc = "&lt;Log&gt;";  //<Log>
            operationDesc += "<br>&lt;Action&gt;<span style=\"font-weight: bold; color: rgb(255, 0, 0);\">" + action + "</span>&lt;/Action&gt;";  //<Action></Action>
            
            string rfid = string.Empty;
            PropertyInfo prop = obj.GetType().GetProperty("rfid");
            if (prop != null)
                rfid = prop.GetValue(obj, null).ToString();

            operationDesc += "<br>&lt;ReferenceId&gt;<span style=\"font-weight: bold; color: rgb(255, 0, 0);\">" + rfid + "</span>&lt;/ReferenceId&gt;";

            operationDesc += "<br>&lt;Data&gt;";
            for (int i = 0; i < properties.Length; i++)
            {
                if (((properties[i].PropertyType.IsValueType) || (properties[i].PropertyType.FullName == "System.String"))
                    && (properties[i].Name != "lmtm" && properties[i].Name != "lmur" && properties[i].Name != "rfid"))
                {
                    object o = properties[i].GetValue(obj, null);
                    string fieldName = GetFieldRes(obj.GetType().Name, properties[i].Name);

                    operationDesc += "<br>&lt;" + fieldName + "&gt;<span style=\"font-weight: bold; color: rgb(255, 0, 0);\">" + o + "</span>&lt;/" + fieldName + "&gt;";
                }
            }
            operationDesc += "<br>&lt;/Data&gt;";  //</Data>
            operationDesc += "<br>&lt;/Log&gt;";   //</Log>
            return operationDesc;
        }

        private string BuildOperationDescUpdate(object objNew, object objOld)
        {
            PropertyInfo[] propertiesNew = objNew.GetType().GetProperties();
            PropertyInfo[] propertiesOld = objOld.GetType().GetProperties();
            string operationDesc = string.Empty;

            operationDesc = "&lt;Log&gt;";  //<Log>
            operationDesc += "<br>&lt;Action&gt;<span style=\"font-weight: bold; color: rgb(255, 0, 0);\">" + "Update" + "</span>&lt;/Action&gt;";  //<Action></Action>

            string rfid = string.Empty;
            PropertyInfo prop = objOld.GetType().GetProperty("rfid");
            if (prop != null)
                rfid = prop.GetValue(objOld, null).ToString();

            operationDesc += "<br>&lt;ReferenceId&gt;<span style=\"font-weight: bold; color: rgb(255, 0, 0);\">" + rfid + "</span>&lt;/ReferenceId&gt;";

            operationDesc += "<br>&lt;Data&gt;";
            
            for (int i = 0; i < propertiesNew.Length; i++)
            {
                if (((propertiesNew[i].PropertyType.IsValueType) || (propertiesNew[i].PropertyType.FullName == "System.String"))
                    && (propertiesNew[i].Name != "lmtm" && propertiesNew[i].Name != "lmur" && propertiesNew[i].Name != "rfid"))
                {
                    object oNew = propertiesNew[i].GetValue(objNew, null);
                    object oOld = propertiesOld[i].GetValue(objOld, null);

                    oNew = oNew == null ? string.Empty : oNew;
                    oOld = oOld == null ? string.Empty : oOld;

                    if (!oNew.Equals(oOld))
                    {
                        string fieldName = GetFieldRes(objOld.GetType().Name, propertiesNew[i].Name);
                        operationDesc += "<br>&lt;" + fieldName + "&gt;&lt;New&gt;<span style=\"font-weight: bold; color: rgb(255, 0, 0);\">" + oNew + "</span>&lt;/New&gt;&lt;Old&gt;<span style=\"font-weight: bold; color:#0000ff;\">" + oOld + "</span>&lt;/Old&gt;" + "&lt;/" + fieldName + "&gt;";
                    }
                }
            }

            operationDesc += "<br>&lt;/Data&gt;";  //</Data>
            operationDesc += "<br>&lt;/Log&gt;";   //</Log>

            return operationDesc;
        }

        private string GetFieldRes(string tableName, string fieldName)
        {
            string res = string.Empty;
            try
            {
                List<tsttabdef> lstDef = null;

                if (Parameter.TABLE_DEFINITION == null)
                {
                    lstDef = GetSelectedRecords<tsttabdef>(new List<ColumnInfo>() { });

                    Parameter.TABLE_DEFINITION = lstDef;
                }
                else
                {
                    lstDef = Parameter.TABLE_DEFINITION as List<tsttabdef>;
                }

                var q = (from p in lstDef
                         where p.tbnm == tableName
                         && p.conm == fieldName
                         && p.rsid.Trim() != string.Empty
                         select p).ToList();

                for (int i = 0; i < q.Count; i++)
                {
                    res = HRMSRes.ResourceManager.GetString(q[i].rsid);
                    if (res.Trim() == string.Empty)
                        res = q[i].conm;
                }

            }
            catch (Exception ex)
            {
            }

            return res;
        }
    }
}
