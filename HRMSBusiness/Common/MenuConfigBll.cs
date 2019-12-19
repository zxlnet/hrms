using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using GotWell.Common;
using GotWell.HRMS.HRMSData;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSBusiness.Common
{
    public class MenuConfigBll
    {
        MenuConfigDal dal = new MenuConfigDal();

        public List<tstmnucfg> getMUF()
        {
            return dal.GetMUF(Parameter.APPLICATION_NAME);
        }

        public List<string> getMUFForSession()
        {
            List<tstmnucfg> lstMUF = dal.GetMUF(Parameter.APPLICATION_NAME);

            var q = from p in lstMUF
                    select p.muid;

            return q.ToList();
        }


        public List<tstmnucfg> getTopMenu()
        {
            try
            {
                int level = 1;
                List<ColumnInfo> parameters = new List<ColumnInfo>(){
                        new ColumnInfo(){ColumnName="leno",ColumnValue= level.ToString(),ColumnType="int"},
                        new ColumnInfo(){ColumnName="apnm",ColumnValue= Parameter.APPLICATION_NAME}
                    };
                return dal.GetMenuByLevelNo(parameters);
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

        public List<tstmnucfg> getSubMenu(string _pami)
        {
            try
            {
                List<ColumnInfo> parameters = new List<ColumnInfo>(){
                        new ColumnInfo(){ColumnName="pami",ColumnValue=_pami.ToString()},
                        new ColumnInfo(){ColumnName="apnm",ColumnValue= Parameter.APPLICATION_NAME}
                    };
                return dal.GetSubMenu(parameters);
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

        public void UpdateMUF(string muid,string action)
        {
            try
            {
                BaseBll bll = new BaseBll();
                if (action == "add")
                {
                    tstmnumuf muf = new tstmnumuf();
                    muf.apnm = Parameter.APPLICATION_NAME;
                    muf.muid = muid.Trim();
                    muf.urid = Function.GetCurrentUser();
                    muf.adtm = DateTime.Now;

                    bll.DoInsert<tstmnumuf>(muf);
                }

                if (action == "delete")
                {
                    List<ColumnInfo> parameters = new List<ColumnInfo>(){new ColumnInfo(){ColumnName="muid",ColumnValue=muid},
                                                                    new ColumnInfo(){ColumnName="apnm",ColumnValue=Parameter.APPLICATION_NAME},
                                                                    new ColumnInfo(){ColumnName="urid",ColumnValue=Function.GetCurrentUser()}};

                    tstmnumuf obj = bll.GetSelectedObject<tstmnumuf>(parameters);
                    if (obj != null)
                    {
                        //bll.DoDelete<tstmnumuf>(parameters);
                        bll.DoDelete<tstmnumuf>(obj);
                    }
                }

            }
            catch (UtilException ex)
            {
            	throw ex;
            }
            catch(Exception ex)
            {
            	throw new UtilException(ex.Message,ex);
            }
        }
    }
}
