﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Personal;

namespace GotWell.HRMS.HRMSBusiness.Personal
{
    public class pseventBll : BaseBll
    {
        pseventDal localDal = null;
        public pseventBll()
        {
            localDal = new pseventDal();
            baseDal = localDal;
        }

    }
}
