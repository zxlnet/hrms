using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Leave;
using GotWell.Utility;
using GotWell.Model.HRMS;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSBusiness.Leave
{
    public class lvlealmtBll : BaseBll
    {
        lvlealmtDal dal = null;

        public lvlealmtBll()
        {
            dal = new lvlealmtDal();
            baseDal = dal;
        }

        public void ApplyTo(List<vw_employment> _emps, tlvlealmt obj)
        {
            try
            {
                int maxlmno = GetMaxNo("tlvlealmt", "lmno").Value;
                for (int i = 0; i < _emps.Count; i++)
                {
                    if (_emps[i].emno != obj.lmva)
                    {
                        List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "lmby", ColumnValue = _emps[i].emno }, 
                                                                                new ColumnInfo(){ColumnName="lmva",ColumnValue=obj.lmva},
                                                                                new ColumnInfo(){ColumnName="lmsc",ColumnValue=obj.lmsp},
                                                                                new ColumnInfo(){ColumnName="ltcd",ColumnValue=obj.ltcd}
                         };

                        tlvlealmt oldobj = GetSelectedObject<tlvlealmt>(parameters);

                        if (oldobj == null)
                        {
                            //新增
                            maxlmno++;

                            tlvlealmt newobj = new tlvlealmt();
                            newobj.lmtm = obj.lmtm;
                            newobj.lmur = obj.lmur;
                            newobj.lmby = _emps[i].emno;
                            newobj.lmno = maxlmno;
                            newobj.lmsp = obj.lmsp;
                            newobj.lmtx = _emps[i].sfid + " - " + _emps[i].ntnm;
                            newobj.lmva = obj.lmva;
                            newobj.mxlh = obj.mxlh;
                            newobj.mxch = obj.mxch;
                            newobj.ltcd = obj.ltcd;

                            DoInsert<tlvlealmt>(newobj);
                        }
                        else
                        {
                            //更新
                            oldobj.lmtm = obj.lmtm;
                            oldobj.lmur = obj.lmur;
                            oldobj.mxlh = obj.mxlh;
                            oldobj.mxch = obj.mxch;
                            DoUpdate<tlvlealmt>(oldobj);
                        }
                    }
                }
            }
            catch (UtilException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
        }
    }
}
