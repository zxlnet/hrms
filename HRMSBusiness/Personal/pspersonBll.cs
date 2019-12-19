using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Personal;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using GotWell.Common;

namespace GotWell.HRMS.HRMSBusiness.Personal
{
    public class pspersonBll : BaseBll
    {
        pspersonDal localDal = null;
        public pspersonBll()
        {
            localDal = new pspersonDal();
            baseDal = localDal;
        }

        public int GetMaxemno()
        {
            string m = localDal.GetMaxemno();

            return Convert.ToInt32(m);
        }

        public void GetAutoStaffId(ref string emno,ref string sfid)
        {
            localDal.GetAutoStaffId(ref emno, ref sfid);

            formaterEMNO femno = new formaterEMNO();
            emno = femno.Parse(emno, femno.GetEmnoFormater());

            formaterSFID fsfid = new formaterSFID();
            sfid = fsfid.Parse(sfid, fsfid.GetEmnoFormater());
        }
    }
}
