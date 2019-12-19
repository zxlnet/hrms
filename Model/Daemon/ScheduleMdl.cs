using System;
using System.Collections.Generic;
using System.Text;

namespace GotWell.Model.Daemon
{
    /// <summary>
    /// 
    /// </summary>
    public class ScheduleMdl
    {
        private string schedule_id;
        private string schedule_name;
        private string schedule_status;
        private string cron_string;
        private string log_file;
        private string result;

        public string task_id;
        public string task_name;
        public string class_name;
        public int interval;
        public string task_status;
        public DateTime next_run_time_id;
        public DateTime last_run_time_id;
        public string last_result;
        public string comments;



        /// <summary>
        /// Gets or sets the last_ result.
        /// </summary>
        /// <value>The last_ result.</value>
        public string Last_Result
        {
            get
            {
                return last_result;
            }
            set
            {
                last_result = value;
            }
        }

        /// <summary>
        /// Gets or sets the last_ result.
        /// </summary>
        /// <value>The last_ result.</value>
        public string Comments
        {
            get
            {
                return comments;
            }
            set
            {
                comments = value;
            }
        }

        public DateTime Last_Run_Time_Id
        {
            get
            {
                return last_run_time_id;
            }
            set
            {
                last_run_time_id = value;
            }
        }

        public DateTime Next_Run_Time_Id
        {
            get
            {
                return next_run_time_id;
            }
            set
            {
                next_run_time_id = value;
            }
        }

        public string Task_status
        {
            get
            {
                return task_status;
            }
            set
            {
                task_status = value;
            }
        }


        public int Interval
        {
            get
            {
                return interval;
            }
            set
            {
                interval = value;
            }
        }


        public string Class_name
        {
            get
            {
                return class_name;
            }
            set
            {
                class_name = value;
            }
        }


        public string Task_name
        {
            get
            {
                return task_name;
            }
            set
            {
                task_name = value;
            }
        }




        public string Task_id
        {
            get
            {
                return task_id;
            }
            set
            {
                task_id = value;
            }
        }


        public string Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
            }
        }

        public string Log_file
        {
            get
            {
                return log_file;
            }
            set
            {
                log_file = value;
            }
        }

        public string Cron_string
        {
            get
            {
                return cron_string;
            }
            set
            {
                cron_string = value;
            }
        }

        public string Schedule_status
        {
            get
            {
                return schedule_status;
            }
            set
            {
                schedule_status = value;
            }
        }
        public string Schedule_name
        {
            get
            {
                return schedule_name;
            }
            set
            {
                schedule_name = value;
            }
        }
        public string Schedule_id
        {
            get
            {
                return schedule_id;
            }
            set
            {
                schedule_id = value;
            }
        }
    }
}
