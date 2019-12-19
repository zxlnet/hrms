using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Attendance;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSBusiness.Master;
using GotWell.Utility;
using GotWell.Common;
using System.Transactions;

namespace GotWell.HRMS.HRMSBusiness.Attendance
{
    public class atcaldarBll : BaseBll
    {
        atcaldarDal dal = null;

         public atcaldarBll()
         {
             dal = new atcaldarDal();
             baseDal = dal;
         }

        public List<object> GetCalendarDetails(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            return dal.GetCalendarDetails(_parameter, paging, start, num, ref totalRecordCount);
        }

        public List<object> GetCalendarDetails(List<ColumnInfo> _parameter)
        {
            List<tatclddtl> result = new List<tatclddtl>();
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            string otcd = string.Empty;
            string htcd = string.Empty;

            var q = from p in _parameter
                    where p.ColumnName == "stdt"
                    select p.ColumnValue;
            startDate = Convert.ToDateTime(q.Single());

            q = from p in _parameter
                    where p.ColumnName == "endt"
                    select p.ColumnValue;

            endDate = Convert.ToDateTime(q.Single());

            q = from p in _parameter
                where p.ColumnName == "otcd"
                select p.ColumnValue;
            otcd = q.Single();

            q = from p in _parameter
                where p.ColumnName == "htcd"
                select p.ColumnValue;
            htcd = q.Single();

            //tbssyscfgBll syscfgBll = new tbssyscfgBll();
            int weekday = Convert.ToUInt16(((StSystemConfig)Parameter.CURRENT_SYSTEM_CONFIG).AtWDPW);//syscfgBll.GetSysParameter_Weekwkdas();
            bool isHoliday = false;

            while(startDate<=endDate)
            {
                if (weekday == 5)
                {
                    if ((startDate.DayOfWeek == DayOfWeek.Saturday) || (startDate.DayOfWeek == DayOfWeek.Sunday))
                    {
                        isHoliday = true;
                    }
                    else
                    {
                        isHoliday = false;
                    }
                }
                else if (weekday == 6)
                {
                    if ((startDate.DayOfWeek == DayOfWeek.Sunday))
                    {
                        isHoliday = true;
                    }
                    else
                    {
                        isHoliday = false;
                    }
                }

                if (isHoliday)
                {
                    tatclddtl dtl = new tatclddtl (){cddt=startDate,
                                                           htcd = htcd,
                                                           otcd = otcd,
                                                           remk ="auto generation",
                                                           lmtm = DateTime.Now,
                                                           lmur ="System"};
                    result.Add(dtl);
                }

                startDate = startDate.AddDays(1);
            }

            string htnm = string.Empty;
            string otnm = string.Empty;
            BaseBll baseBll = new BaseBll();

            tathldtyp ht = baseBll.GetSelectedObject<tathldtyp>(new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "htcd", ColumnValue = htcd } });
            htnm = ht.htnm;
            tottype ottype = baseBll.GetSelectedObject<tottype>(new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "otcd", ColumnValue = otcd } });
            otnm = ottype.otnm;

            var q1 = from p in result
                    select new
                    {
                        p.cddt,
                        p.htcd,
                        p.otcd,
                        p.remk,
                        p.lmtm,
                        p.lmur,
                        htnm = htnm,
                        otnm = otnm
                    };

            return q1.Cast<object>().ToList() ;
        }

        public void DeleteCalendar(List<ColumnInfo> _parameters)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //delete first
                    tatcaldar obj = GetSelectedObject<tatcaldar>(_parameters);

                    List<tatclddtl> oldList = GetSelectedRecords<tatclddtl>(_parameters);

                    for (int i = 0; i < oldList.Count; i++)
                    {
                        dal.DoDelete<tatclddtl>(oldList[i]);
                    }

                    //delete
                    dal.DoDelete<tatcaldar>(obj);

                    scope.Complete();
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

        public void InsertCalendar(tatcaldar obj, List<tatclddtl> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //insert
                    DoInsert<tatcaldar>(obj);

                    for (int i = 0; i < list.Count; i++)
                    {
                        DoInsert<tatclddtl>(list[i]);
                    }
                    scope.Complete();
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

        public void UpdateCalendar(tatcaldar obj, List<tatclddtl> list,List<string> deleteddobj)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //delete first
                    for (int i = 0; i < deleteddobj.Count; i++)
                    {
                        List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "clcd", ColumnValue = obj.clcd },
                                                                           new ColumnInfo() { ColumnName = "cddt", ColumnValue = deleteddobj[i],ColumnType="datetime" } };

                        DoMultiDelete<tatclddtl>(parameters);
                        //List<tatclddtl> oldList = GetSelectedRecords<tatclddtl>(parameters);

                        //for (int j = 0; j < oldList.Count; j++)
                        //{
                        //    dal.DoDelete<tatclddtl>(oldList[j]);
                        //}
                    }

                    for (int i = 0; i < list.Count; i++)
                    {
                        List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "clcd", ColumnValue = obj.clcd },
                                                                           new ColumnInfo() { ColumnName = "cddt", ColumnValue = UtilDatetime.FormatDate1(list[i].cddt),ColumnType="datetime" } };

                        DoMultiDelete<tatclddtl>(parameters);
                    }

                    List<ColumnInfo> cldParameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "clcd", ColumnValue = obj.clcd } };
                    //update
                    DoUpdate<tatcaldar>(obj, cldParameters);

                    for (int i = 0; i < list.Count; i++)
                    {
                        dal.DoInsert<tatclddtl>(list[i]);
                    }

                    scope.Complete();
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
