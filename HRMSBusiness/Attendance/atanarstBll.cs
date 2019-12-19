using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Attendance;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSBusiness.Attendance
{
    public class atanarstBll : BaseBll
    {
         atanarstDal dal = null;

         public atanarstBll()
         {
             dal = new atanarstDal();
             baseDal = dal;
         }

         public List<object> GetAnalyzeResult(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
         {
             return dal.GetAnalyzeResult(_parameter, paging, start, num, ref totalRecordCount);
         }
    }
}
