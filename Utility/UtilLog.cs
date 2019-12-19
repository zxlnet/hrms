using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using log4net;
using Newtonsoft.Json;
using GotWell.Common;

namespace GotWell.Utility
{
    public sealed class UtilLog
    {
        #region Definition

        private ILog log;
        private Log_LoggingLevel SystemLoggingLevel;
        private log4net.Appender.FileAppender fileAppender = null;

        //Maximun/Default Log file size
        public static string DefaultMaximumFileSize = "10MB";

        #endregion Definition

        #region Constructor

        public UtilLog()
            : this("")
        {

        }

        public UtilLog(string _logger)
        {
            if (_logger == null || _logger.Equals(""))
            {
                log = LogManager.GetLogger(typeof(UtilLog));
            }
            else
            {
                log = LogManager.GetLogger(_logger);
            }
        }

        public UtilLog(string _logFileName, Log_LoggingLevel _systemLoggingLevel)
        {
            fileAppender = CreateAppender(AppDomain.CurrentDomain.BaseDirectory + @"\" + _logFileName, _systemLoggingLevel.ToString());
            CreateLogger(fileAppender, _logFileName);

            log = LogManager.GetLogger(_logFileName);

            SystemLoggingLevel = _systemLoggingLevel;
        }
        #endregion

        #region InitialLog
        public static bool InitialLog(string config)
        {
            bool flag1;
            try
            {
                FileInfo info1 = new FileInfo(config);
                if (!info1.Exists)
                {
                    return false;
                }
                log4net.Config.XmlConfigurator.Configure(info1);
                flag1 = true;
            }
            catch (UtilException)
            {
                flag1 = false;
            }
            return flag1;
        }
        #endregion

        #region LogDebug
        public void LogDebug(string message)
        {
            log.Debug(message);
        }
        #endregion


        #region LogError
        public void LogError(string message)
        {
            log.Error(message);
        }
        #endregion

        #region LogFatal
        public void LogFatal(string message)
        {
            log.Fatal(message);
        }
        #endregion

        #region LogInfo
        public void LogInfo(string message)
        {
            log.Info(message);
        }
        #endregion

        #region LogWarn
        public void LogWarn(string message)
        {
            log.Warn(message);
        }
        #endregion

        #region LogInfoWithLevel
        public void LogInfoWithLevel(string message, Log_LoggingLevel messageLoggingLevel)
        {
            if (SystemLoggingLevel == Log_LoggingLevel.None)
                return;

            if (SystemLoggingLevel == Log_LoggingLevel.Admin)
            {
                if ((messageLoggingLevel == Log_LoggingLevel.Admin) || (messageLoggingLevel == Log_LoggingLevel.Normal))
                    log.Info(message);
            }

            if (SystemLoggingLevel == Log_LoggingLevel.Normal)
            {
                if (messageLoggingLevel == Log_LoggingLevel.Normal)
                    log.Info(message);
                else if (messageLoggingLevel == Log_LoggingLevel.Admin)
                    return;
            }
        }
        #endregion

        #region CreateAppender
        /// <summary>
        /// This method is used to create the appender
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="appenderName"></param>
        /// <returns></returns>
        private log4net.Appender.FileAppender CreateAppender(string fileName, string appenderName)
        {
            //log4net.Appender.FileAppender fileAppender = new log4net.Appender.FileAppender();
            log4net.Appender.RollingFileAppender fileAppender = new log4net.Appender.RollingFileAppender();

            log4net.Layout.PatternLayout patternLayOut = new log4net.Layout.PatternLayout();
            patternLayOut.Header = ""; //System.Environment.NewLine + "---Starts Here---" + System.Environment.NewLine;
            patternLayOut.Footer = ""; //
            patternLayOut.ConversionPattern = "%d %m%n";

            //patternLayOut.ConversionPattern = "";
            patternLayOut.ActivateOptions();


            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Log" ;//System.Windows.Forms.Application.StartupPath + @"\Log";
            System.IO.DirectoryInfo info = new DirectoryInfo(path);

            if (!info.Exists)
            {
                info.Create();
            }
            string[] file = fileName.Split('\\');


            fileAppender.Layout = patternLayOut;


            fileAppender.AppendToFile = true;
            fileAppender.MaximumFileSize = Parameter.LOGGING_FILE_SIZE.Trim().Equals("") ? DefaultMaximumFileSize : Parameter.LOGGING_FILE_SIZE;

            fileAppender.MaxSizeRollBackups = 10;
            fileAppender.StaticLogFileName = true;
            fileAppender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Size;

            //fileAppender.DatePattern = ".yyyyMMdd'.log'";
            //fileAppender.File = path + "\\" + file[file.Length - 1].Split('.')[0];

            fileAppender.File = path + "\\" + file[file.Length - 1];
            fileAppender.Name = appenderName;

            fileAppender.LockingModel = new log4net.Appender.FileAppender.MinimalLock();
            fileAppender.ActivateOptions();

            return fileAppender;
        }

        #endregion CreateAppender

        #region CreateLogger
        /// <summary>
        /// This method creates the loggers
        /// </summary>
        /// <param name="fileAppender"></param>
        /// <param name="loggerName"></param>
        private void CreateLogger(log4net.Appender.FileAppender fileAppender, string loggerName)
        {
            log4net.Repository.Hierarchy.Hierarchy hierarchy = (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository();
            log4net.Repository.Hierarchy.Logger logger = (log4net.Repository.Hierarchy.Logger)hierarchy.GetLogger(loggerName);
            if (logger.GetAppender(fileAppender.Name) == null)
            {
                logger.AddAppender(fileAppender);
            }

            hierarchy.Configured = true;
        }

        #endregion CreateLogger

        /// <summary>
        /// </summary>
        /// <param name="_errorText">The _error text.</param>
        /// <param name="_fileNamePrefix">The _file name prefix.</param>
        /// <returns></returns>
        /// <Remarks>
        public string LogInfoToNewFile(string _errorText,string _fileNamePrefix,string _fileDesc)
        {
            StreamWriter w = null;
            try
            {
                string pathName = AppDomain.CurrentDomain.BaseDirectory + @"\Log\";
                string fileName = _fileNamePrefix +"_"+ UtilDatetime.FormatDateTime5(DateTime.Now) + ".trc";

                FileInfo f = new FileInfo(pathName + fileName);

                w = f.CreateText();
                w.WriteLine(_fileDesc);
                w.WriteLine(_errorText);
                w.Close();

                return fileName;
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
            finally
            {
            }
        }

        public void LogHeader(string _fileName,List<string> _infoText)
        {
            StreamWriter w = null;
            try
            {
                string pathName = AppDomain.CurrentDomain.BaseDirectory + @"\Log\";
                FileInfo f = new FileInfo(pathName + _fileName);

                w = f.CreateText();

                for (int i = 0; i < _infoText.Count; i++)
                {
                    w.WriteLine(_infoText[i]);
                }
                
                w.Close();

            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
            finally
            {
            }
        }
    }
}
