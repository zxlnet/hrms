using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.Utility
{
    public class ExcelWriter
    {
        System.IO.FileStream _writer;

        public ExcelWriter(string strPath)
        {

            _writer = new System.IO.FileStream(strPath, System.IO.FileMode.OpenOrCreate);
        }
        /// <summary>
        /// 写入short数组
        /// </summary>
        /// <param name="values"></param>
        private void _writeFile(short[] values)
        {
            foreach (short v in values)
            {
                byte[] b = System.BitConverter.GetBytes(v);
                _writer.Write(b, 0, b.Length);
            }
        }
        /// <summary>
        /// 写文件头
        /// </summary>
        public void BeginWrite()
        {
            _writeFile(new short[] { 0x809, 8, 0, 0x10, 0, 0 });
        }
        /// <summary>
        /// 写文件尾
        /// </summary>
        public void EndWrite()
        {
            _writeFile(new short[] { 0xa, 0 });
            _writer.Close();
        }
        /// <summary>
        /// 写一个数字到单元格x,y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        public void WriteNumber(short x, short y, double value)
        {
            _writeFile(new short[] { 0x203, 14, x, y, 0 });
            byte[] b = System.BitConverter.GetBytes(value);
            _writer.Write(b, 0, b.Length);
        }
        /// <summary>
        /// 写一个字符到单元格x,y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        public void WriteString(short x, short y, string value)
        {
            byte[] b = System.Text.Encoding.Default.GetBytes(value);
            _writeFile(new short[] { 0x204, (short)(b.Length + 8), x, y, 0, (short)b.Length });
            _writer.Write(b, 0, b.Length);
        }
    }
}
