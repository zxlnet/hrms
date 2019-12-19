using System;
using System.Collections.Generic;
using System.Text;

namespace GotWell.Model.Daemon
{
    public class RptTaskMdl
    {
        public RptTaskMdl()
		{}


		#region Model
		private string _task_id;
		private string _task_name;
		private string _class_name;
		private Int32 _interval;
		private string _task_status;
		private DateTime _next_run_time_id;
		private DateTime _last_run_time_id;
		private string _last_run_result;
		/// <summary>
		/// 
		/// </summary>
		public string task_id
		{
			set{ _task_id=value;}
			get{return _task_id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string task_name
		{
			set{ _task_name=value;}
			get{return _task_name;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string class_name
		{
			set{ _class_name=value;}
			get{return _class_name;}
		}
		/// <summary>
		/// 
		/// </summary>
		public Int32 interval
		{
			set{ _interval=value;}
			get{return _interval;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string task_status
		{
			set{ _task_status=value;}
			get{return _task_status;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime next_run_time_id
		{
			set{ _next_run_time_id=value;}
			get{return _next_run_time_id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime last_run_time_id
		{
			set{ _last_run_time_id=value;}
			get{return _last_run_time_id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string last_run_result
		{
			set{ _last_run_result=value;}
			get{return _last_run_result;}
		}
		#endregion Model
    }
}
