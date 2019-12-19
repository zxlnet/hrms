using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Model.Common;
using Newtonsoft.Json;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSBusiness.Common
{
    public class ValueInfoBll : BaseBll
    {
        #region getClub
        public List<ValueInfo> getClub()
        {
            try
            {
                List<tbsclub> clubList = GetSelectedRecords<tbsclub>(new List<ColumnInfo>() { });
                var clnmList = (from p in clubList
                                select new ValueInfo { ValueField = p.clcd, DisplayField = p.clnm }).ToList();
                return clnmList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region getStaffType
        public List<ValueInfo> getStaffType()
        {
            try
            {
                List<tbsstftyp> list = GetSelectedRecords<tbsstftyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.stcd, DisplayField = p.stnm }).ToList();
                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region getDivision
        public List<ValueInfo> getDivision()
        {
            try
            {
                List<tbsdivion> list = GetSelectedRecords<tbsdivion>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.dvcd, DisplayField = p.dvnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region getBusiness
        public List<ValueInfo> getBusiness()
        {
            try
            {
                List<tbsbusne> list = GetSelectedRecords<tbsbusne>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.bscd, DisplayField = p.bsnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region getDepartment
        public List<ValueInfo> getDepartment()
        {
            try
            {
                List<tbsdepmnt> list = GetSelectedRecords<tbsdepmnt>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.dpcd, DisplayField = p.dpnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region getGrade
        public List<ValueInfo> getGrade()
        {
            try
            {
                List<tbsgrade> list = GetSelectedRecords<tbsgrade>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.grcd, DisplayField = p.grnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region getGradeLevel
        public List<ValueInfo> getGradeLevel()
        {
            try
            {
                List<tbsgrdlev> list = GetSelectedRecords<tbsgrdlev>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.glcd, DisplayField = p.glnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region getPosition
        public List<ValueInfo> getPosition()
        {
            try
            {
                List<tbspstion> list = GetSelectedRecords<tbspstion>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.pscd, DisplayField = p.psnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public List<ValueInfo> getPositionLevel()
        {
            try
            {
                List<tbsposlev> list = GetSelectedRecords<tbsposlev>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.plcd, DisplayField = p.plnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getJobClass()
        {
            try
            {
                List<tbsjobcl> list = GetSelectedRecords<tbsjobcl>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.jccd, DisplayField = p.jcnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getJobType()
        {
            try
            {
                List<tbsjobtyp> list = GetSelectedRecords<tbsjobtyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.jtcd, DisplayField = p.jtnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getWorkGroup()
        {
            try
            {
                List<tbswrkgrp> list = GetSelectedRecords<tbswrkgrp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.wgcd, DisplayField = p.wgnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getWorkSite()
        {
            try
            {
                List<tbswrkste> list = GetSelectedRecords<tbswrkste>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.wscd, DisplayField = p.wsnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getCostCenter()
        {
            try
            {
                List<tbscosctr> list = GetSelectedRecords<tbscosctr>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.cccd, DisplayField = p.ccnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getSC()
        {
            try
            {
                List<tbssc> list = GetSelectedRecords<tbssc>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.sccd, DisplayField = p.scnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getCalendar()
        {
            try
            {
                List<tatcaldar> list = GetSelectedRecords<tatcaldar>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.clcd, DisplayField = p.cdnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getPayType()
        {
            try
            {
                List<tbspaytyp> list = GetSelectedRecords<tbspaytyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ptcd, DisplayField = p.ptnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getRoster()
        {
            try
            {
                List<tatroster> list = GetSelectedRecords<tatroster>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.rscd.ToString(), DisplayField = p.rsnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getctcd()
        {
            try
            {
                List<tbschgtyp> list = GetSelectedRecords<tbschgtyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ctcd, DisplayField = p.ctnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getTermReason()
        {
            try
            {
                List<tbsterrsn> list = GetSelectedRecords<tbsterrsn>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.trcd, DisplayField = p.trnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getTermType()
        {
            try
            {
                List<tbstertyp> list = GetSelectedRecords<tbstertyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ttcd, DisplayField = p.ttnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getContractType()
        {
            try
            {
                List<tbsctrtyp> list = GetSelectedRecords<tbsctrtyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ctcd, DisplayField = p.ctnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getOverTimeType()
        {
            try
            {
                List<tottype> list = GetSelectedRecords<tottype>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.otcd, DisplayField = p.otnm }).ToList();
                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getHolidayType()
        {
            try
            {
                List<tathldtyp> list = GetSelectedRecords<tathldtyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.htcd, DisplayField = p.htnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getShift()
        {
            try
            {
                List<tatshift> list = GetSelectedRecords<tatshift>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.sfcd, DisplayField = p.sfnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getLeaveType()
        {
            try
            {
                List<tlvleatyp> list = GetSelectedRecords<tlvleatyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ltcd, DisplayField = p.ltnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getLeaveReason()
        {
            try
            {
                List<tlvlearsn> list = GetSelectedRecords<tlvlearsn>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.lrcd, DisplayField = p.lrnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<ValueInfo> getValidPeriod()
        {
            try
            {
                List<tstperiod> list = GetSelectedRecords<tstperiod>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.perd, DisplayField = p.perd }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getArea()
        {
            try
            {
                List<tbsarea> list = GetSelectedRecords<tbsarea>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.arcd, DisplayField = p.arnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getAddressType()
        {
            try
            {
                List<tbsaddtyp> list = GetSelectedRecords<tbsaddtyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.atcd, DisplayField = p.atnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getBank()
        {
            try
            {
                List<tbsbank> list = GetSelectedRecords<tbsbank>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.bkcd, DisplayField = p.bknm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getRelationShip()
        {
            try
            {
                List<tbsreltyp> list = GetSelectedRecords<tbsreltyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.rtcd, DisplayField = p.rtnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getCountry()
        {
            try
            {
                List<tbscontry> list = GetSelectedRecords<tbscontry>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.cocd, DisplayField = p.conm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getCertifType()
        {
            try
            {
                List<tbscertyp> list = GetSelectedRecords<tbscertyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ctcd, DisplayField = p.ctnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getChangeType()
        {
            try
            {
                List<tbschgtyp> list = GetSelectedRecords<tbschgtyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ctcd, DisplayField = p.ctnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getChangeTypeForEmployment()
        {
            try
            {
                List<tbschgtyp> list = GetSelectedRecords<tbschgtyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               where (p.ctcd != "sys$0") && (p.ctcd != "sys$2") && (p.ctcd != "sys$3")
                               select new ValueInfo { ValueField = p.ctcd, DisplayField = p.ctnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getCurrency()
        {
            try
            {
                List<tbscurncy> list = GetSelectedRecords<tbscurncy>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.crcd, DisplayField = p.crnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getContactType()
        {
            try
            {
                List<tbsctatyp> list = GetSelectedRecords<tbsctatyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ctcd, DisplayField = p.ctnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getEducationLevel()
        {
            try
            {
                List<tbsedulev> list = GetSelectedRecords<tbsedulev>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.elcd, DisplayField = p.elnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getEventType()
        {
            try
            {
                List<tbsevttyp> list = GetSelectedRecords<tbsevttyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.evcd, DisplayField = p.evnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getBusinessNature()
        {
            try
            {
                List<tbsbusntr> list = GetSelectedRecords<tbsbusntr>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.bncd, DisplayField = p.bnnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getInsuranceSchema()
        {
            try
            {
                List<tbsinsscm> list = GetSelectedRecords<tbsinsscm>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.iscd, DisplayField = p.isnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getInsuranceCompany()
        {
            try
            {
                List<tbsinscmp> list = GetSelectedRecords<tbsinscmp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.iccd, DisplayField = p.icnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getInsuranceType()
        {
            try
            {
                List<tbsinstyp> list = GetSelectedRecords<tbsinstyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.itcd, DisplayField = p.itnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getQualType()
        {
            try
            {
                List<tbsqlfctn> list = GetSelectedRecords<tbsqlfctn>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.qfcd, DisplayField = p.qfnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getTrainingType()
        {
            try
            {
                List<tbstratyp> list = GetSelectedRecords<tbstratyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.ttcd, DisplayField = p.ttnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getWelfareType()
        {
            try
            {
                List<tbswlftyp> list = GetSelectedRecords<tbswlftyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.wtcd, DisplayField = p.wtnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getPolity()
        {
            try
            {
                List<tbspolity> list = GetSelectedRecords<tbspolity>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.plcd, DisplayField = p.plnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getNation()
        {
            try
            {
                List<tbsnation> list = GetSelectedRecords<tbsnation>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.nacd, DisplayField = p.nanm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getReligion()
        {
            try
            {
                List<tbsrligon> list = GetSelectedRecords<tbsrligon>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.rgcd, DisplayField = p.rgnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getSkillType()
        {
            try
            {
                List<tbsskltyp> list = GetSelectedRecords<tbsskltyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.stcd, DisplayField = p.stnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getRating()
        {
            try
            {
                List<tbsrating> list = GetSelectedRecords<tbsrating>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.racd, DisplayField = p.ranm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getSalaryType()
        {
            try
            {
                List<tprsaltyp> list = GetSelectedRecords<tprsaltyp>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.stcd, DisplayField = p.stnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getVariables()
        {
            try
            {
                List<tprvarble> list = GetSelectedRecords<tprvarble>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.vacd, DisplayField = p.vanm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getSalaryItems()
        {
            try
            {
                List<tprsalitm> list = GetSelectedRecords<tprsalitm>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.itcd, DisplayField = p.itnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getPayDay()
        {
            try
            {
                List<tprpayday> list = GetSelectedRecords<tprpayday>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.pdcd, DisplayField = p.padt.ToString() }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getFormula()
        {
            try
            {
                List<tprfmular> list = GetSelectedRecords<tprfmular>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.frcd, DisplayField = p.frnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getPubRuleSet()
        {
            try
            {
                List<tprpubrst> list = GetSelectedRecords<tprpubrst>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.rscd, DisplayField = p.rsnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getCondition()
        {
            try
            {
                List<tstcondtn> list = GetSelectedRecords<tstcondtn>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.cdno, DisplayField = p.cdnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getRun()
        {
            try
            {
                List<tprrun> list = GetSelectedRecords<tprrun>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.rnno, DisplayField = p.rnno }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValueInfo> getHeadCoungCfg()
        {
            try
            {
                List<tpshctcfg> list = GetSelectedRecords<tpshctcfg>(new List<ColumnInfo>() { });
                var finList = (from p in list
                               select new ValueInfo { ValueField = p.hccd, DisplayField = p.hcnm }).ToList();

                return finList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //public List<ValueInfo> GetValueInfoFromString(string val)
        //{
        //    string[] arr = val.Split(new string[] { "rows:" });

        //    string v = arr[1].Substring(0, arr[1].Length - 1);

        //    return JavaScriptConvert.DeserializeObject<List<ValueInfo>>(v);
        //}
    }
}
