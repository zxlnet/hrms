using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using System.Linq.Dynamic;
using GotWell.Model.HRMS;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSData.Syst
{
    public class stsearchDal : BaseDal
    {
        private string[] arrFieldsEmp = { "sfid", "ntnm", "idno", "ennm", "dpnm", "elnm", "jcnm","napl" };

        private string GenerateWhereSql(string[] arrFields, string[] arrKeywords)
        {
            StringBuilder sbResult = new StringBuilder();
            for (int i = 0; i < arrFields.Length; i++)
            {
                if (i!=0)
                    sbResult.Append(" or ");

                for (int j = 0; j < arrKeywords.Length; j++)
                {
                    if (j!=0)
                        sbResult.Append(" and ");

                    sbResult.Append("(" + arrFields[i].Trim() + ".Contains(\"" + arrKeywords[j] + "\"))");
                }
            }

            return sbResult.ToString();
        }

        public List<vw_employment> SearchPerson(string[] arrKeywords,string mode)
        {
            try
            {
                string sSql1 = GenerateWhereSql(arrFieldsEmp, arrKeywords);

                var q = from p in gDB.vw_employments.Where(sSql1)
                        select p;

                List<vw_employment> obj = q.Cast<vw_employment>().ToList();

                return obj;
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

        public List<tpseductn> SearchEducation(string[] arrKeywords,string mode)
        {
            string[] arrFields = { "loca","scho","spec","subj" };
            try
            {
                string sSql1 = "1=1"; 
                string sSql2 = "1=1"; 

                if (mode=="Y")
                    sSql1 = GenerateWhereSql(arrFieldsEmp, arrKeywords);
                else
                    sSql2 = GenerateWhereSql(arrFields, arrKeywords); 

                var q = from p in gDB.tpseductns.Where(sSql2)
                        join o in gDB.vw_employments.Where(sSql1) on p.emno equals o.emno
                        select p;

                List<tpseductn> obj = q.Cast<tpseductn>().ToList();
                return obj;
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

        public List<tpsexprnc> SearchExperience(string[] arrKeywords, string mode)
        {
            string[] arrFields = { "fpos","cmnm","jbds" };
            try
            {
                string sSql1 = "1=1"; 
                string sSql2 = "1=1"; 

                if (mode=="Y")
                    sSql1 = GenerateWhereSql(arrFieldsEmp, arrKeywords);
                else
                    sSql2 = GenerateWhereSql(arrFields, arrKeywords); 

                var q = from p in gDB.tpsexprncs.Where(sSql2)
                        join o in gDB.vw_employments.Where(sSql1) on p.emno equals o.emno
                        select p;

                List<tpsexprnc> obj = q.Cast<tpsexprnc>().ToList();
                return obj;
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

        public List<tpsevent> SearchEvent(string[] arrKeywords, string mode)
        {
            string[] arrFields = { "desc", "resn" };
            try
            {
                string sSql1 = "1=1"; 
                string sSql2 = "1=1"; 

                if (mode=="Y")
                    sSql1 = GenerateWhereSql(arrFieldsEmp, arrKeywords);
                else
                    sSql2 = GenerateWhereSql(arrFields, arrKeywords);

                var q = from p in gDB.tpsevents.Where(sSql2)
                        join o in gDB.vw_employments.Where(sSql1) on p.emno equals o.emno
                        select p;

                List<tpsevent> obj = q.Cast<tpsevent>().ToList();
                return obj;
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

        public List<tpsaddre> SearchAddress(string[] arrKeywords, string mode)
        {
            string[] arrFields = { "addr" };
            try
            {
                string sSql1 = "1=1"; 
                string sSql2 = "1=1"; 

                if (mode=="Y")
                    sSql1 = GenerateWhereSql(arrFieldsEmp, arrKeywords);
                else
                    sSql2 = GenerateWhereSql(arrFields, arrKeywords);

                var q = from p in gDB.tpsaddres.Where(sSql2)
                        join o in gDB.vw_employments.Where(sSql1) on p.emno equals o.emno
                        select p;

                List<tpsaddre> obj = q.Cast<tpsaddre>().ToList();
                return obj;
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

        public List<tpscontct> SearchContact(string[] arrKeywords, string mode)
        {
            string[] arrFields = { "cota1","cota2" };
            try
            {
                string sSql1 = "1=1";
                string sSql2 = "1=1";

                if (mode == "Y")
                    sSql1 = GenerateWhereSql(arrFieldsEmp, arrKeywords);
                else
                    sSql2 = GenerateWhereSql(arrFields, arrKeywords);

                var q = from p in gDB.tpscontcts.Where(sSql2)
                        join o in gDB.vw_employments.Where(sSql1) on p.emno equals o.emno
                        select p;

                List<tpscontct> obj = q.Cast<tpscontct>().ToList();
                return obj;
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

        public List<tpscontrt> SearchContract(string[] arrKeywords, string mode)
        {
            string[] arrFields = { "crnm", "crde" };
            try
            {
                string sSql1 = "1=1";
                string sSql2 = "1=1";

                if (mode == "Y")
                    sSql1 = GenerateWhereSql(arrFieldsEmp, arrKeywords);
                else
                    sSql2 = GenerateWhereSql(arrFields, arrKeywords);

                var q = from p in gDB.tpscontrts.Where(sSql2)
                        join o in gDB.vw_employments.Where(sSql1) on p.emno equals o.emno
                        select p;

                List<tpscontrt> obj = q.Cast<tpscontrt>().ToList();
                return obj;
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

        public List<tpsdetain> SearchDetain(string[] arrKeywords, string mode)
        {
            string[] arrFields = { "stnm", "dtre","rtre" };
            try
            {
                string sSql1 = "1=1";
                string sSql2 = "1=1";

                if (mode == "Y")
                    sSql1 = GenerateWhereSql(arrFieldsEmp, arrKeywords);
                else
                    sSql2 = GenerateWhereSql(arrFields, arrKeywords);

                var q = from p in gDB.tpsdetains.Where(sSql2)
                        join o in gDB.vw_employments.Where(sSql1) on p.emno equals o.emno
                        select p;

                List<tpsdetain> obj = q.Cast<tpsdetain>().ToList();
                return obj;
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

        public List<tpsskill> SearchSkill(string[] arrKeywords, string mode)
        {
            string[] arrFields = { "sknm", "skle" };
            try
            {
                string sSql1 = "1=1";
                string sSql2 = "1=1";

                if (mode == "Y")
                    sSql1 = GenerateWhereSql(arrFieldsEmp, arrKeywords);
                else
                    sSql2 = GenerateWhereSql(arrFields, arrKeywords);

                var q = from p in gDB.tpsskills.Where(sSql2)
                        join o in gDB.vw_employments.Where(sSql1) on p.emno equals o.emno
                        select p;

                List<tpsskill> obj = q.Cast<tpsskill>().ToList();
                return obj;
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
