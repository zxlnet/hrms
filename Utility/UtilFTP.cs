using System;
using GotWell.Common;
using EnterpriseDT.Net.Ftp;

namespace GotWell.Utility
{
    public class UtilFTP
    {
        #region Definition

        FTPConnection ftp = null;

        public enum UtilFTPTransferType
        {
            ASCII = 1,
            BINARY = 2
        }

        #endregion Definition

        #region construct

        public UtilFTP()
        {
            ftp = new FTPConnection();
        }

        #endregion construct


        #region connect
        public void Connect(string host, int port, string user, string password, string serverDirectory)
        {
            try
            {
                if (host.Trim() == "")
                {
                    UtilException ex = new UtilException("Host is empty.", Exception_ExceptionSeverity.High);
                    throw ex;
                }

                ftp.ServerAddress = host.Trim();
                ftp.ServerPort = port;
                ftp.UserName = user.Trim();
                ftp.Password = password;
                ftp.ServerDirectory = serverDirectory;

                ftp.Connect();
            }
            catch (Exception ex)
            {
                if (ftp.IsConnected)
                {
                    ftp.Close();
                }

                throw ex;
            }
            finally
            {
            }
        }
        #endregion connect

        #region Set Transfer Type
        public void SetTransferType(UtilFTPTransferType type)
        {
            if (type == UtilFTPTransferType.ASCII)
            {
                ftp.TransferType = FTPTransferType.ASCII;
            }

            if (type == UtilFTPTransferType.BINARY)
            {
                ftp.TransferType = FTPTransferType.BINARY;
            }
        }
        #endregion Set Transfer Type

        #region IsConnected
        public bool IsConnected()
        {
            return ftp.IsConnected;
        }
        #endregion IsConnected


        #region Download File
        public void DownloadFile(string localPath, string remoteFile)
        {
            ftp.DownloadFile(localPath, remoteFile);
        }

        #endregion Download File

        #region Close FTP
        public void Close()
        {
            if (ftp.IsConnected)
                ftp.Close();
        }

        #endregion CloseFTP
    }
}
