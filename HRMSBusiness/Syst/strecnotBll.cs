using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSCore;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using System.Transactions;

namespace GotWell.HRMS.HRMSBusiness.Syst
{
    public class strecnotBll : BaseBll
    {
        public strecnotBll()
        {
            baseDal = new BaseDal();
        }

        public string GetNote(string rfid,string emno)
        {
            List<ColumnInfo> paras = new List<ColumnInfo>() { 
                  new ColumnInfo() { ColumnName = "ntid", ColumnValue = rfid },
                  new ColumnInfo() { ColumnName = "emno", ColumnValue = emno }
            };
            tstrecnot obj = GetSelectedObject<tstrecnot>(paras);

            if (obj == null) return string.Empty;

            return obj.note;
        }

        public void UpdateNote(string rfid, string emno, string note)
        {
            List<ColumnInfo> paras = new List<ColumnInfo>() { 
                    new ColumnInfo() { ColumnName = "ntid", ColumnValue = rfid },
                    new ColumnInfo() { ColumnName = "emno", ColumnValue = emno }
            };
            tstrecnot obj = GetSelectedObject<tstrecnot>(paras);

            using (TransactionScope scope = new TransactionScope())
            {
                if (obj == null)
                {
                    obj = new tstrecnot();
                    obj.ntid = rfid;
                    obj.note = note;
                    DoInsert<tstrecnot>(obj);
                }
                else
                {
                    obj.note = note;
                    DoUpdate<tstrecnot>(obj);
                }

                scope.Complete();
            }
        }
    }
}
