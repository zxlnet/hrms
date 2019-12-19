using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using GotWell.Model.Common;
using GotWell.Common;
using GotWell.Utility;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSData.Common
{
    public class MenuConfigDal : BaseDal
    {
        public MenuConfigDal()
        {

        }
        #region  成员方法

        public List<tstmnucfg> GetMenuByLevelNo(List<ColumnInfo> _parameters)
        {
            try
            {
                var menus = from p in gDB.tstmnucfgs
                            where p.leno == Convert.ToInt16(GetColumnValue("leno", _parameters))
                            && p.apnm == GetColumnValue("apnm", _parameters)//"CentralServiceWeb"
                            orderby p.srno
                            select p;

                return menus.ToList <tstmnucfg>();

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

        public List<tstmnucfg> GetSubMenu(List<ColumnInfo> _parameters)
        {
            try
            {
                var subMenus = from p in gDB.tstmnucfgs
                               where p.pami == GetColumnValue("pami", _parameters)
                               && p.apnm == GetColumnValue("apnm", _parameters)
                               orderby p.srno
                               select p;

                //return GetSelectedRecords<tstmnucfg>(_parameters);

                return subMenus.ToList<tstmnucfg>();
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

        public List<tstmnucfg> GetMUF(string _apnm)
        {
            try
            {
                var q = from p in gDB.tstmnucfgs
                        join t in gDB.tstmnumufs on new { p.apnm, p.muid } equals new { t.apnm, t.muid }
                        where t.urid == Function.GetCurrentUser()
                        && t.apnm == _apnm
                        select p;

                return q.ToList();
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

        #endregion
    }
}

