using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using GotWell.Common;

namespace GotWell.HRMS.HRMSCore.MessageControl
{
    public class stalsubsBll : BaseBll
    {
        private static List<tstalsub> lstAlarmSubs = null;
        private static List<vw_empemail1> lstUserRoleAD = null;
        private static List<vw_empemail2> lstUserRoleNonAD = null;
        private static List<tscusrrol> lstUserRole = null;
        private static List<tscuser> lstUser = null;

        public stalsubsBll()
        {
            if (lstAlarmSubs == null)
            {
                lstAlarmSubs = GetSelectedRecords<tstalsub>(new List<ColumnInfo>() { });
                lstUserRoleAD = GetSelectedRecords<vw_empemail1>(new List<ColumnInfo>() { });
                lstUserRoleNonAD = GetSelectedRecords<vw_empemail2>(new List<ColumnInfo>() { });
                lstUserRole = GetSelectedRecords<tscusrrol>(new List<ColumnInfo>() { });
                lstUser = GetSelectedRecords<tscuser>(new List<ColumnInfo>() { });
            }
        }

        public string GetRecipicientsEmail(string _emno)
        {
            List<string> lstRole = new List<string>();
            vw_employment emp = GetSelectedObject<vw_employment>(new List<ColumnInfo>() { new ColumnInfo() { ColumnName="emno",ColumnValue=_emno } });

            if (emp != null)
            {
                for (int i = 0; i < lstAlarmSubs.Count; i++)
                {
                    PropertyInfo prop = emp.GetType().GetProperty(lstAlarmSubs[i].attr);
                    if (prop != null)
                    {
                        if (prop.GetValue(emp, null).ToString() == lstAlarmSubs[i].atva.Trim())
                        {
                            lstRole.Add("[" + lstAlarmSubs[i].alpf +"]");
                        }
                    }
                }
            }

            string strRole = lstRole.ToArray().ToString();
            
            StSystemConfig sysCfg = (StSystemConfig)Parameter.CURRENT_SYSTEM_CONFIG;

            string strReci = string.Empty;

            if (sysCfg.ScSBAD == "Y")
            {
                var q = (from p in lstUserRoleAD
                         where strRole.Contains("[" + p.alpf + "]") == true
                         select p).ToList();

                for (int i = 0; i < q.Count; i++)
                {
                    string email = q[i].cota1.Trim() == string.Empty ? q[i].cota2.Trim() : q[i].cota1.Trim();

                    if (email != string.Empty)
                        strReci += (strReci.Trim() == string.Empty) ? "" : "," + email;
                }
            }
            else
            {
                var q = (from p in lstUserRoleNonAD
                         where strRole.Contains("[" + p.alpf + "]") == true
                         select p).ToList();

                for (int i = 0; i < q.Count; i++)
                {
                    string email = q[i].cota1.Trim() == string.Empty ? q[i].cota2.Trim() : q[i].cota1.Trim();

                    if (email != string.Empty)
                        strReci += (strReci.Trim() == string.Empty) ? "" : "," + email;
                }
            }

            return strReci;
        }

        public string GetRecipicientsBoard(string _emno)
        {
            //List<string> lstRole = new List<string>();
            vw_employment emp = GetSelectedObject<vw_employment>(new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = _emno } });
            string strRole = string.Empty;

            if (emp != null)
            {
                for (int i = 0; i < lstAlarmSubs.Count; i++)
                {
                    if (lstAlarmSubs[i].attr == "*")
                    {
                        //订阅全部员工的消息
                        strRole += (strRole == string.Empty ? "" : ",") + "[" + lstAlarmSubs[i].alpf + "]";
                    }
                    else
                    {
                        PropertyInfo prop = emp.GetType().GetProperty(lstAlarmSubs[i].attr);
                        if (prop != null)
                        {
                            object v = prop.GetValue(emp, null);
                            if (v != null)
                            {
                                if (v.ToString() == lstAlarmSubs[i].atva.Trim())
                                {
                                    strRole += (strRole == string.Empty ? "" : ",") + "[" + lstAlarmSubs[i].alpf + "]";
                                }
                            }
                        }
                    }
                }
            }

            StSystemConfig sysCfg = (StSystemConfig)Parameter.CURRENT_SYSTEM_CONFIG;

            string strReci = string.Empty;

            if (sysCfg.ScSBAD == "Y")
            {
                var q = (from p in lstUserRole
                         where strRole.Contains("[" + p.alpf + "]") == true
                         select p).ToList();

                for (int i = 0; i < q.Count; i++)
                {
                    if (q[i].sfid != null && q[i].sfid.Trim() != string.Empty)

                        strReci += (strReci.Trim() == string.Empty ? "" : ",") + q[i].sfid;
                }
            }
            else
            {
                var q = (from p in lstUser
                         where strRole.Contains("[" + p.alpf + "]") == true
                         select p).ToList();

                for (int i = 0; i < q.Count; i++)
                {
                    if (q[i].sfid != null && q[i].sfid.Trim() != string.Empty)

                        strReci += (strReci.Trim() == string.Empty ? "" : ",") + q[i].sfid;
                }
            }

            return strReci;
        }
 
    }
}
