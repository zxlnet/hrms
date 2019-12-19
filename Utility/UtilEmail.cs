using System;
using System.Collections;
using System.IO;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace GotWell.Utility
{
    public class UtilEmail
    {
        #region variable
        private string server = "";
        private int port = 25;
        private string userName = "";
        private string password = "";
        private string from = "";
        private string to = "";
        private string fromName = "";
        private string toName = "";
        private string subject = "";
        private string body = "";
        private string htmlbody = "";
        private bool ishtml = false;
        private string encoding = "8bit";
        private string languageEncoding = "GB2312";
        private int priority = 3;
        private ArrayList attachments = new ArrayList();
        #endregion

        #region Property
        ///<summary>
        /// SMTP ServerName or IP
        ///</summary>
        public string Server
        {
            get { return server; }
            set { server = value; }
        }

        ///<summary>
        ///SMTP Port[Default is 25]
        ///</summary>
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        ///<summary>
        /// Username [auth]
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        ///<summary>
        ///Password[Auth.]
        /// </summary>       
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        ///<summary>
        ///MailFrom
        /// </summary>
        public string From
        {
            get { return from; }
            set { from = value; }
        }

        ///<summary>
        ///MailTo
        ///</summary>
        public string To
        {
            get { return to; }
            set { to = value; }
        }

        ///<summary>
        ///FromName
        ///</summary>       
        public string FromName
        {
            get { return fromName; }
            set { fromName = value; }
        }

        ///<summary>
        ///ToName
        /// </summary>
        public string ToName
        {
            get { return toName; }
            set { toName = value; }
        }

        ///<summary>
        ///Mail Subject
        ///</summary>
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }
        ///<summary>
        ///Mail's Body
        /// </summary>
        public string Body
        {
            get { return body; }
            set { body = value; }
        }

        ///<summary>
        ///Body's Format
        ///</summary>
        public string HtmlBody
        {
            get { return htmlbody; }
            set { if (value != htmlbody) htmlbody = value; }
        }

        /// <summary>
        /// Is Html Format
        /// </summary>
        public bool IsHtml
        {
            get { return ishtml; }
            set { if (value != ishtml) ishtml = value; }
        }

        ///<summary>
        ///Language Encoding[default is GB2312]
        /// </summary>
        public string LanguageEncoding
        {
            get { return languageEncoding; }
            set { if (value != languageEncoding) languageEncoding = value; }
        }

        /// <summary>
        /// MailEncoding [default is 8bit]
        /// </summary>
        public string MailEncoding
        {
            get { return encoding; }
            set { if (value != encoding) encoding = value; }
        }

        ///<summary>
        ///Mail's Priority[default is 3]
        /// </summary>      
        public int Priority
        {
            get { return priority; }
            set { if (value != priority)priority = value; }
        }

        ///<summary>
        /// Mail's Attachments
        /// </summary>      
        public IList Attachments
        {
            get { return attachments; }
            // set { if (value != attachments)attachments = value; }
        }
        #endregion

        #region Method
        ///<summary>
        ///SendMail
        /// </summary>
        public void SendMail()
        {
            TcpClient tcp = null;
            try
            {
                tcp = new TcpClient(server, port);
            }
            catch (Exception)
            {
                throw new Exception("Connect to Server Error");
            }
            ReadString(tcp.GetStream());//Get Connection Info 
            // Start To Auth.
            // if status = 250 ,success 
            if (!Command(tcp.GetStream(), "EHLO Localhost", "250"))
                throw new Exception("Login failed");
            if (userName != "")
            {
                // Need Auth.
                //////if (!Command(tcp.GetStream(), "AUTH LOGIN", "334"))
                //////    throw new Exception("Auth. failed");
                //////string nameB64 = ToBase64(userName); //  
                //////if (!Command(tcp.GetStream(), nameB64, "334"))
                //////    throw new Exception("Auth. failed");
                //////string passB64 = ToBase64(password); //  
                //////if (!Command(tcp.GetStream(), passB64, "235"))
                //////    throw new Exception("Auth. failed");
            }
            WriteString(tcp.GetStream(), "mail From: " + from);
            WriteString(tcp.GetStream(), "rcpt to: " + to);
            WriteString(tcp.GetStream(), "data");

            // Send Mail Head 
            WriteString(tcp.GetStream(), "Date: " + DateTime.Now); // Time 
            WriteString(tcp.GetStream(), "From: " + fromName + "(" + from + ")"); // Accept 
            WriteString(tcp.GetStream(), "Subject: " + subject); // Title 
            WriteString(tcp.GetStream(), "To:" + toName + "(" + to + ")"); // Acceptor 

            //Mail Format 
            WriteString(tcp.GetStream(), "Content-Type: multipart/mixed;boundary=\"unique-boundary-1\"");
            WriteString(tcp.GetStream(), "Reply-To:" + from); //  
            WriteString(tcp.GetStream(), "X-Priority:" + priority); //  
            WriteString(tcp.GetStream(), "MIME-Version:1.0"); // MIME Version 

            // WriteString (tcp.GetStream(), "Message-Id: " + DateTime.Now.ToFileTime() + "@security.com"); 
            WriteString(tcp.GetStream(), "Content-Transfer-Encoding:" + encoding);
            WriteString(tcp.GetStream(), "X-Mailer:JcPersonal.Utility.MailSender");
            WriteString(tcp.GetStream(), "");
            WriteString(tcp.GetStream(), ToBase64("This is a multi-part message in MIME format."));
            WriteString(tcp.GetStream(), "");

            WriteString(tcp.GetStream(), "--unique-boundary-1");
            WriteString(tcp.GetStream(), "Content-Type: multipart/alternative;Boundary=\"unique-boundary-2\"");
            WriteString(tcp.GetStream(), "");

            if (!ishtml)
            {
                WriteString(tcp.GetStream(), "--unique-boundary-2");
                WriteString(tcp.GetStream(), "Content-Type: text/plain;charset=" + languageEncoding);
                WriteString(tcp.GetStream(), "Content-Transfer-Encoding:" + encoding);
                WriteString(tcp.GetStream(), "");
                WriteString(tcp.GetStream(), body);
                WriteString(tcp.GetStream(), "");
                WriteString(tcp.GetStream(), "--unique-boundary-2--");
                WriteString(tcp.GetStream(), "");
            }
            else
            {
                WriteString(tcp.GetStream(), "--unique-boundary-2");
                WriteString(tcp.GetStream(), "Content-Type: text/html;charset=" + languageEncoding);
                WriteString(tcp.GetStream(), "Content-Transfer-Encoding:" + encoding);
                WriteString(tcp.GetStream(), "");
                WriteString(tcp.GetStream(), htmlbody);
                WriteString(tcp.GetStream(), "");
                WriteString(tcp.GetStream(), "--unique-boundary-2--");
                WriteString(tcp.GetStream(), "");
            }

            // Send Attachments 
            for (int i = 0; i < attachments.Count; i++)
            {
                WriteString(tcp.GetStream(), "--unique-boundary-1");
                WriteString(tcp.GetStream(), "Content-Type: application/octet-stream;name=\"" + ((AttachmentInfo)attachments[i]).FileName + "\"");
                WriteString(tcp.GetStream(), "Content-Transfer-Encoding: base64");
                WriteString(tcp.GetStream(), "Content-Disposition:attachment;filename=\"" + ((AttachmentInfo)attachments[i]).FileName + "\"");
                WriteString(tcp.GetStream(), "");
                WriteString(tcp.GetStream(), ((AttachmentInfo)attachments[i]).Bytes);
                WriteString(tcp.GetStream(), "");
            }
            Command(tcp.GetStream(), ".", "250"); // End ,Input "."

            // Close Connection
            tcp.Close();
        }


        /// <summary> 
        /// Write string to Stream
        /// </summary> 
        /// (param name="netStream")TcpClient Stream(/param) 
        /// (param name="str")string(/param) 
        protected void WriteString(NetworkStream netStream, string str)
        {
            str = str + "\r\n";
            byte[] bWrite = Encoding.GetEncoding(languageEncoding).GetBytes(str.ToCharArray());
            int start = 0;
            int length = bWrite.Length;
            int page = 0;
            int size = 75;
            int count = size;
            try
            {
                if (length > 75)
                {
                    if ((length / size) * size < length)
                        page = length / size + 1;
                    else
                        page = length / size;
                    for (int i = 0; i < page; i++)
                    {
                        start = i * size;
                        if (i == page - 1)
                            count = length - (i * size);
                        netStream.Write(bWrite, start, count);
                    }
                }
                else
                    netStream.Write(bWrite, 0, bWrite.Length);
            }
            catch (Exception)
            { }

        }

        /// <summary> 
        /// Read String Form Stream 
        /// </summary> 
        /// (param name="netStream")TcpClient Stream(/param) 
        /// (returns)string(/returns) 
        protected string ReadString(NetworkStream netStream)
        {
            string sp = null;
            byte[] by = new byte[1024];
            int size = netStream.Read(by, 0, by.Length);// Read Stream 
            if (size > 0)
            {
                sp = Encoding.Default.GetString(by);// Convert To string 
            }
            return sp;
        }


        /// <summary> 
        /// Send Command 
        /// /// </summary> 
        /// (param name="netStream")TcpClient Stream(/param) 
        /// (param name="command")Command(/param) 
        /// (param name="state")status(/param) 
        /// (returns)is correct(/returns) 
        protected bool Command(NetworkStream netStream, string command, string state)
        {
            string sp = null;
            bool success = false;
            try
            {
                WriteString(netStream, command);
                sp = ReadString(netStream);
                if (sp.IndexOf(state) != -1)
                    success = true;
            }
            catch (Exception)
            {
            }
            return success;
        }

        /// <summary> 
        /// Convert String To Base64 
        /// </summary> 
        /// (param name="str")string (/param) 
        /// (returns)Base64 string(/returns) 
        protected string ToBase64(string str)
        {
            try
            {
                byte[] by = Encoding.Default.GetBytes(str.ToCharArray());
                str = Convert.ToBase64String(by);
            }
            catch (Exception)
            {
            }
            return str;
        }
        #endregion

        #region Mail Struct
        /// <summary> 
        /// Attachment Info 
        /// </summary> 
        public struct AttachmentInfo
        {
            private string fileName;
            private string bytes;

            public string FileName
            {
                get { return fileName; }
                set { fileName = Path.GetFileName(value); }
            }
            public string Bytes
            {
                get { return bytes; }
                set { if (value != bytes) bytes = value; }
            }
            /// <summary>
            /// Send Stream
            /// </summary>
            /// <param name="ifileName">File Name</param>
            /// <param name="stream">File Stream</param>
            public AttachmentInfo(string ifileName, Stream stream)
            {
                fileName = Path.GetFileName(ifileName);
                byte[] by = new byte[stream.Length];
                stream.Read(by, 0, (int)stream.Length);
                bytes = Convert.ToBase64String(by);
            }
            /// <summary>
            /// Send bytes
            /// </summary>
            /// <param name="ifileName">File Name</param>
            /// <param name="ibytes">bytes</param>
            public AttachmentInfo(string ifileName, byte[] ibytes)
            {
                fileName = Path.GetFileName(ifileName);
                bytes = Convert.ToBase64String(ibytes);
            }
            /// <summary>
            /// Send Local File
            /// </summary>
            /// <param name="path">File Path</param>
            public AttachmentInfo(string path)
            {
                fileName = Path.GetFileName(path);
                FileStream file = new FileStream(path, FileMode.Open);
                byte[] by = new byte[file.Length];
                file.Read(by, 0, (int)file.Length);
                bytes = Convert.ToBase64String(by);
                file.Close();
            }
        }
        #endregion

        #region Validation
        /// <summary>
        /// Check if a email address is valid
        /// </summary>
        /// <param name="sEmailAddress"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string sEmailAddress)
        {
            string pattern = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(sEmailAddress);
        }

        #endregion

        public bool SendMail(string _from, string _recipient, string _subject, string _body, string _username, string _password, string _mailServer)
        {
            MailMessage message = null;
            try
            {
                // Command line argument must the the SMTP host.
                SmtpClient client = new SmtpClient(_mailServer);
                client.Credentials = new System.Net.NetworkCredential(_username,_password);

                // Specify the e-mail sender.
                // Create a mailing address that includes a UTF8 character
                // in the display name.
                MailAddress from = new MailAddress(_from, _subject, System.Text.Encoding.UTF8);

                // Set destinations for the e-mail message.
                MailAddress to = new MailAddress(_recipient);

                // Specify the message content.
                message = new MailMessage(from, to);

                message.Body = _body;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                message.Subject = _subject;
                message.SubjectEncoding = System.Text.Encoding.UTF8;

                //client.SendAsync(message, userState);
                client.Send(message);

                return true;
            }

            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
            finally
            {
                if (message != null)
                {
                    message.Dispose();
                }
            }
        }
    }
}

#region How to use
//***************HOW TO USE***********************
/*
MailSender ms = new MailSender();
ms.From = "mailfrom@company.COM";
ms.To = "mailto@company.com";
ms.Subject = "Subject";
ms.Body = "body text";
ms.UserName = "XXXXXX";
ms.Password = "XXXXXX";
ms.Server = "10.133.130.62"; 
ms.Attachments.Add(new MailSender.AttachmentInfo(@"c:\a.txt"));//发送文件
System.IO.FileStream fs = new System.IO.FileStream(@"c:\tmuninst.ini", System.IO.FileMode.Open);
ms.Attachments.Add(new MailSender.AttachmentInfo("tmuninst.ini", fs));//发送文件流
Console.WriteLine("mail sending");
try
{
    ms.SendMail();
    Console.WriteLine("mail sended.");
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
*/
#endregion
