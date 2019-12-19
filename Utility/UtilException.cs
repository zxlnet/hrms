using System;
using System.Runtime.Serialization;
using GotWell.Common;

namespace GotWell.Utility
{
    public sealed class UtilException : Exception
    {
        #region Definition

        private int _code;
        private Exception_ExceptionSeverity _severity;

        #endregion Definition

        #region Properity
        /// <summary>
        /// Error Code
        /// </summary>
        public int Code
        {
            get { return _code; }
            set { _code = value; }
        }

        /// <summary>
        /// Exception Serverity
        /// </summary>
        public Exception_ExceptionSeverity Serverity
        {
            get { return _severity; }
            set { _severity = value; }
        }

        public string TraceFileName { get; set; }

        #endregion Properity

        #region Construct
        /// <summary>
        /// construct
        /// </summary>
        public UtilException()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public UtilException(string message)
            : base(message)
        {
            _severity = Exception_ExceptionSeverity.Medium;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="severity"></param>
        public UtilException(string message, Exception_ExceptionSeverity severity)
            : base(message)
        {
            _severity = severity;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        public UtilException(string message, int code)
            : base(message)
        {
            _code = code;
            _severity = Exception_ExceptionSeverity.Medium;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <param name="severity"></param>
        public UtilException(string message, int code, Exception_ExceptionSeverity severity)
            : base(message)
        {
            _code = code;
            _severity = severity;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected UtilException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _severity = Exception_ExceptionSeverity.Medium;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public UtilException(string message,  Exception inner)
            : base(message, inner)
        {
            _code = -1;
            _severity = Exception_ExceptionSeverity.Medium;
        }


        public UtilException(string message, Exception inner,string traceFileName)
            : base(message, inner)
        {
            _code = -1;
            _severity = Exception_ExceptionSeverity.Medium;
            TraceFileName = traceFileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <param name="inner"></param>
        public UtilException(string message,int code, Exception inner)
            : base(message, inner)
        {
            _code = code;
            _severity = Exception_ExceptionSeverity.Medium;
        }

        /// <summary>
        /// With Severity
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <param name="inner"></param>
        /// <param name="severity"></param>
        public UtilException(string message, int code, Exception inner, Exception_ExceptionSeverity severity)
            : base(message, inner)
        {
            _code = code;
            _severity = severity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <param name="inner"></param>
        public UtilException(string message, Exception_ErrorMessage code, Exception inner)
            : base(message, inner)
        {

            _code =(int)code ;
            _severity = Exception_ExceptionSeverity.Medium;
        }

        public UtilException(string message, Exception_ErrorMessage code, Exception inner, string traceFileName)
            : base(message, inner)
        {

            _code = (int)code;
            _severity = Exception_ExceptionSeverity.Medium;
            TraceFileName = traceFileName;
        }

        public UtilException(string message, int code, Exception inner, string traceFileName)
            : base(message, inner)
        {

            _code = code;
            _severity = Exception_ExceptionSeverity.Medium;
            TraceFileName = traceFileName;
        }

        /// <summary>
        /// With Severity
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <param name="inner"></param>
        /// <param name="severity"></param>
        public UtilException(string message, Exception_ErrorMessage code, Exception inner, Exception_ExceptionSeverity severity)
            : base(message, inner)
        {

            _code = (int)code;
            _severity = severity;
        }

        #endregion Construct
    }
}