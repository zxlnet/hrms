using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Recruitment;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSBusiness.Recruitment
{
    public class rcapplicBll : BaseBll
    {
        rcapplicDal localDal = null;
        public rcapplicBll()
        {
            localDal = new rcapplicDal();
            baseDal = localDal;
        }

        public List<object> GetActiveApplication(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            return localDal.GetActiveApplication(_parameter,paging,start,num,ref totalRecordCount);
        }
    }
}
