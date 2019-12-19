using System;
using System.Collections.Generic;
using System.Text;

namespace GotWell.Common
{

    #region Public Enum
    /************************Public Section*************************/
    public enum Public_DateTimeType
    {
        Date,
        DateTime,
        Time
    }

    public enum Public_Priority
    {
        High,
        Medium,
        Low,
    }

    public enum Public_Status
    {
        Active,
        Inactive,
        Valid,
        Invalid
    }

    public enum Public_Flag
    {
        Y,
        N,
        X
    }

    public enum Public_DefaultValue
    {
        sysdate,
        sysuser
    }

    #endregion

    #region Daemon Enums
    /************Daemon Service********************/
    public enum Damon_TaskExecutionResult
    {
        Successfully,
        Failed,
        Unknown
    }

    public enum Daemon_ScheduleStatus
    {
        Active,
        Inactive
    }

    public enum Daemon_ScheduleRunMode
    {
        RecoveryMode,
        NormalMode
    }

    public enum Daemon_AlarmingMsgLevel
    {
        Success,
        Fail,
        Both
    }

    public enum Daemon_TaskProcStatus
    {
        Processing,
        Idle
    }
    #endregion

    #region Alarm Enums
    /********************Alarm Section**************************/
    public enum Alarm_AlarmStatus
    {
        Handled,
        Unhandled,
        Expired,
        Abandoned,
        Abnormal
    }

    public enum Costing_CalculationStep_LockStatus
    {
        Y,
        N,
        Unknown
    }

    public enum Alarm_AlarmCatagory
    {
        PMORDER,
        PMJOB,
        EQP
    }


    public enum Alarm_AlarmType
    {
        Email,
        PhoneCall,
        SMS,
        Board

    }

    public enum Alarm_BoardStatus
    {
        All,
        Read,
        UnRead
    }

    public enum Alarm_DeliveryStatus
    {
        Sent,
        Unsent,
        Abnormal
    }

    public enum Alarm_MessageType
    {
        PublicMessage,
        LeaveApplication,
        OverTime,
        InfoUpdated,
        InfoAdded,
        InfoDeleted,
        Training
    }
    #endregion

    #region Security Enums
    /********************Security Section***********************/
    public enum Security_UserStatus
    {
        Active,
        Inactive,
        Deleted
    }

    public enum Security_Permission_Type
    {
        Allowed,
        Denied
    }

    //行级权限控制
    public enum Security_RSC_Action
    {
        Select,
        Delete,
        Update,
        Create
    }

    #endregion

    #region Exception,Error
    /********************Exception Section************************/
    public enum Exception_ExceptionSeverity
    {
        High,
        Medium,
        Low
    }

    public enum Exception_ErrorMessage
    {
        NoError = 1000,
        Pass = 1001,
        IsDuplicate = -1001,
        IsNull = -1002,
        IsUsed = -1003,
        ParameterError = -1004,
        Fail = -1005,
        IsSystemUser = -1006,
        CheckPreconditionFail = -1007,
        CheckP209BusinessFail1 = -1008,
        PeriodIsClosedImportDenied = -1009,
        PeriodIsUnused = -1010,
        ImportDataFail = -1011,
        CheckP205BusinessFail = -1012,
        CheckP210BusinessFail = -1013,
        OpenPeriodFoundOpenDenied = -1014,
        CheckP207ProcessFail = -1015,
        CheckP207MaterialFail = -1016,
        CheckP207PeriodFail = -1017,
        CheckP207BusenessFail = -1018,
        CheckP208ProcessFail = -1019,
        CheckP208MaterialFail = -1020,
        CheckP208PeriodFail = -1021,
        CheckP208BusinessFail = -1022,
        CheckP302BusinessFail1 = -1023,
        CheckP302BusinessFail2 = -1024,
        NoDataToCarryForward = -1025,
        StepCompletedCarryForwardDenied = -1026,
        CheckP301BusinessFail = -1027,
        CheckP301BusinessFail1 = -1028,
        CheckP304BusinessFail1 = -1029,
        CheckP304BusinessFail2 = -1030,
        ResetIsUnnecessary = -1031,
        CheckP306BusinessFail1 = -1032,
        CheckP306BusinessFail2 = -1033,
        CheckP504BusinessFail=-1034,
        CheckP506MessageError = -1035,
        P505SAPFeedBackError = -1036,
        P506SAPFeedBackError = -1037,
        StepIsCompleted=-1038,
        StepIsLocked=-1039,
        PeriodIsClosed = -1040,
        CheckP506ResetFail=-1050,
        CheckP303FailByWeight = -1051,
        CheckP303FailByAmount = -1052,
        CheckP210BusinessFail1=-1053,
        NoReportData = -1054
    } 
    #endregion

    #region Log Enums
    /******************Log Section*****************************/
    public enum Log_LoggingLevel
    {
        Admin,          //Keep all logs
        Normal,
        None
    }
    #endregion

    public enum Transaction_OperationCategory
    {
        Add,
        Update,
        Delete 
    }

    #region HRMS
    /// <summary>
    /// Enum for Costing Calculation Flow
    /// </summary>
    public enum HRMS_CalculationStep_Status
    {
        Completed,
        Processing,
        Uncompleted,
        Waiting,
        Unknown
    }

    public enum HRMS_Period_Status
    {
        Open,
        Closed,
        Unused
    }

    public enum HRMS_Import_Status
    {
        Completed,
        Processing,
        Waiting,
        Unknown,
        Error
    }

    public enum HRMS_Attendance_State
    {
        Normal,
        EarlyLate,
        Abnormal
    }

    public enum HRMS_Limit_Scope
    {
        Week,
        Year,
        Month
    }

    public enum HRMS_Limit_DEF_For
    {
        Leave,
        Overtime
    }

    public enum HRMS_Limit_By
    {
        E,
        Y,
        Others
    }

    public enum HRMS_Limit_Type
    {
        LeaveHours,
        LeaveCarryforwardHours,
        OvertimeHours,
        OvertimeTTLVHours
    }

    public enum HRMS_User_Status
    {
        Active,
        Inactive,
        Locked
    }

    public enum HRMS_SalaryItem_ValueType
    {
        Value,
        Formula,
        Sum,
        Customization
    }
    #endregion
}
