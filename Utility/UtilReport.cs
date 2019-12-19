using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using GotWell.Common;

namespace GotWell.Utility
{
    public class ColumnPattern
    {
        public string IncludeCondition { get; set; }
        public string ExcludeCondition { get; set; }
        public string ColumnText { get; set; }
        public string DataField { get; set; }

        public ColumnPattern() 
        {
            IncludeCondition = string.Empty;
            ExcludeCondition = string.Empty;
            ColumnText = string.Empty;
            DataField = string.Empty;
        }
    }

    public class RowPattern
    {
        public string RowGroup { get; set; }
        public string IncludeCondition { get; set; }
        public string ExcludeCondition { get; set; }

        public RowPattern()
        {
            RowGroup = string.Empty;
            IncludeCondition = string.Empty;
            ExcludeCondition = string.Empty;
        }
    }

    public class ReportColumn
    {
        public string ColumnText { get; set; }
        public string ColumnValue { get; set; }
        public string ParentValue { get; set; }

        public ReportColumn()
        {
            ColumnText = string.Empty;
            ColumnValue = string.Empty;
            ParentValue = string.Empty;
        }
    }

    public class ReportFixedRow
    {
        public string RowText { get; set; }
        public string RowValue { get; set; }
        public string ParentValue { get; set; }
        public int ChildCount { get; set; }

        public ReportFixedRow()
        {
            RowText = string.Empty;
            RowValue = string.Empty;
            ParentValue = string.Empty;
            ChildCount = 0;
        }
    }

    public class FixedReportColumn
    {
        public string ParentValue { get; set; }
        public string ValueDataField { get; set; }
        public string ParentDataField { get; set; }
        public FixedReportColumn()
        {
            ParentValue = string.Empty;
            ValueDataField = string.Empty;
            ParentDataField = string.Empty;
        }
    }

    public class HtmlReport
    {
        public HtmlCaption Capation { get; set; }
        public int TotalRowSpan { get; set; }
        public List<HtmlColumn> Columns { get; set; }
        public List<HtmlRow> Rows { get; set; }
        public int TotalRows { get; set; }
        public int ColumnSpan()
        {
            int totalColumnSpan = 0;
            for (int i = 0; i < Columns.Count; i++)
            {
                HtmlColumn column = Columns[i];
                totalColumnSpan += column.GetTotalColSpan();
            }
            return totalColumnSpan;
        }

        public HtmlReport()
        {
            Capation = new HtmlCaption();
            TotalRowSpan = 0;
            Columns = new List<HtmlColumn>();
            Rows = new List<HtmlRow>();
            TotalRows = 0;
        }
    }

    public class HtmlHeader
    {
        public string HeaderText { get; set; }
        public string HeaderValue { get; set; }
        public HtmlStyle Style { get; set; }

        public HtmlHeader()
        {
            HeaderText = string.Empty;
            HeaderValue = string.Empty;
            Style = new HtmlStyle();
        }
    }

    public class HtmlCaption
    {
        public string Title { get; set; }
        public HtmlStyle Style { get; set; }

        public HtmlCaption()
        {
            Title = string.Empty;
            Style = new HtmlStyle();
        }
    }

    public class HtmlRow
    {
        public int RowSpan { get; set; }
        public HtmlHeader Header { get; set; }
        public List<HtmlRow> ChildRows { get; set; }

        public HtmlRow()
        {
            RowSpan = 1;
            Header = new HtmlHeader();
            ChildRows = new List<HtmlRow>();
        }
    }

    public class HtmlColumn
    {
        public string DataType { get; set; }
        public int RowSpan { get; set; }
        public HtmlHeader Header { get; set; }
        public List<HtmlColumn> ChildColumns { get; set; }
        public bool IsLeaf { get; set; }

        public HtmlColumn()
        {
            DataType = "string";
            RowSpan = 1;
            Header = new HtmlHeader();
            ChildColumns = new List<HtmlColumn>();
            IsLeaf = false;
        }
        /// <summary>
        /// Gets the total col span.
        /// </summary>
        /// <returns></returns>
        /// <Remarks>
        public int GetTotalColSpan()
        {
            int totalColSpan = 0;
            TotalColSpan(this, ref totalColSpan);
            return totalColSpan;
        }

        public int GetTotalRowSpan()
        {
            int totalRowSpan = 0;
            TotalRowSpan(this, ref totalRowSpan);
            return totalRowSpan;
        }


        /// <summary>
        /// Totals the col span.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="totalColSpan">The total col span.</param>
        /// <Remarks>
        private void TotalColSpan(HtmlColumn column, ref int totalColSpan)
        {
            if (column.IsLeaf)
            {
                totalColSpan++;
                return;
            }
            for (int i = 0; i < column.ChildColumns.Count; i++)
            {
                TotalColSpan(column.ChildColumns[i], ref totalColSpan);
            }
        }

        private void TotalRowSpan(HtmlColumn column, ref int totalRowSpan)
        {
            if (column.ChildColumns.Count > 0)
                totalRowSpan++;
            else
                return;


            for (int i = 0; i < column.ChildColumns.Count; i++)
            {
                TotalRowSpan(column.ChildColumns[i], ref totalRowSpan);
                i = column.ChildColumns.Count;
            }
        }

    }

    public class HtmlStyle
    {
        public string Align { get; set; }
        public string FontSize { get; set; }
        public string Color { get; set; }

        public HtmlStyle()
        {
            Align = "center";
            FontSize = "12";
            Color = "black";
        }
    }


    public class HtmlReportGenerator
    {
        private string reportXmlFile = string.Empty;
        private DataSet dataSource;
        public HtmlReport report = null;

        public HtmlReportGenerator(string _reportXmlFile, DataSet _dataSource)
        {
            this.reportXmlFile = _reportXmlFile;
            this.dataSource = _dataSource;
        }

        /// <summary>
        /// Generates the report.
        /// </summary>
        /// <returns></returns>
        /// <Remarks>
        public string GenerateReport()
        {
            if (report == null)
            {
                report = ParseXml();
            }
            StringBuilder builder = new StringBuilder();
            builder.Append("<html><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><body>")
                   .Append(@"<table width='100%' border='1'>")
                   .Append("<tr><td colspan='").Append(report.ColumnSpan()).Append("' ")
                   .Append("align='").Append(report.Capation.Style.Align).Append("' ")
                   .Append("style='font-size:").Append(report.Capation.Style.FontSize).Append("px;")
                   .Append("color:").Append(report.Capation.Style.Color).Append(";'>")
                   .Append(report.Capation.Title).Append("</td></tr>");

            builder.Append(CreateHeader());
            builder.Append(CreateBody());
            builder.Append("</table></body></html>");
            return builder.ToString();
        }

        /// <summary>
        /// Generates the report HTML.
        /// </summary>
        /// <returns></returns>
        /// <Remarks>
        public string GenerateReportHtml(string tabId)
        {
            if (report == null)
            {
                report = ParseXml();
            } 

            StringBuilder builder = new StringBuilder();
            builder.Append(@"<table id='").Append(tabId).Append("_table' ")
                .Append("width='100%' border='1' class='rep-border'>")
                .Append("<caption style='text-align:").Append(report.Capation.Style.Align).Append(";")
                .Append("color:").Append(report.Capation.Style.Color).Append(";")
                .Append("font-size:").Append(report.Capation.Style.FontSize).Append("px;'>")
                .Append(report.Capation.Title).Append("</caption>")
                .Append(CreateHeader()).Append(CreateBody()).Append("</table>");
            return builder.ToString().EscapeHtml();
        }

        /// <summary>
        /// Creates the header.
        /// </summary>
        /// <returns></returns>
        /// <Remarks>
        public StringBuilder CreateHeader()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<thead class='x-grid3-header'>");
            for (int i = 0; i < this.report.TotalRows; i++)
            {
                builder.Append("<tr class='x-grid3-hd-row'>");
                for (int j = 0; j < report.Columns.Count; j++)
                {
                    HtmlColumn column = report.Columns[j];
                    int rowSpan = report.TotalRows - column.GetTotalRowSpan();
                    int colSpan = column.GetTotalColSpan();
                    if (i == 0)
                    {
                        builder.Append("<td rowspan='").Append(rowSpan).Append("' ")
                               .Append("colspan='").Append(colSpan).Append("' ")
                               .Append("align='").Append(column.Header.Style.Align).Append("' ")
                               .Append("style='font-size:").Append(column.Header.Style.FontSize).Append("px;")
                               .Append("color:").Append(column.Header.Style.Color).Append(";'>")
                               .Append(column.Header.HeaderText).Append("</td>");
                    }
                    else
                    {
                        if (i >= rowSpan)
                        {
                            CreateColWhenDeep(column, i - rowSpan + 1, builder);
                        }
                    }

                }
                builder.Append("</tr>");
            }
            builder.Append("</thead>");
            return builder;
        }
        /// <summary>
        /// Creates the body.
        /// </summary>
        /// <returns></returns>
        /// <Remarks>
        public StringBuilder CreateBody()
        {
            StringBuilder builder = new StringBuilder();
            System.Web.UI.HtmlControls.HtmlTable table = new System.Web.UI.HtmlControls.HtmlTable();
            DataTable dt = this.dataSource.Tables[0];
            bool flag = false;

            if (this.dataSource != null)
            {
                //处理有分区的行
                for (int i = 0; i < report.Rows.Count; i++)
                {
                    System.Web.UI.HtmlControls.HtmlTableRow tr1 = null;
                    System.Web.UI.HtmlControls.HtmlTableRow tr = new System.Web.UI.HtmlControls.HtmlTableRow();
                    System.Web.UI.HtmlControls.HtmlTableCell td = new System.Web.UI.HtmlControls.HtmlTableCell();
                    td.InnerText = report.Rows[i].Header.HeaderText;
                    td.RowSpan = report.Rows[i].RowSpan;
                    tr.Cells.Add(td);

                    for (int n = 0; n < dt.Rows.Count; n++)
                    {
                        DataRow row = dt.Rows[n];
                        tr1 = new System.Web.UI.HtmlControls.HtmlTableRow();
                        if (row[0].ToString() == report.Rows[i].Header.HeaderValue)
                        {
                            for (int j = 1; j < dt.Columns.Count; j++)
                            {
                                DataColumn column = dt.Columns[j];
                                System.Web.UI.HtmlControls.HtmlTableCell td1 = new System.Web.UI.HtmlControls.HtmlTableCell();
                                td1.InnerText = row[column].ToString();

                                if (j == 1)
                                    tr.Cells.Add(td1);
                                else
                                {
                                    tr1.Cells.Add(td1);
                                }

                                flag = true;
                            }
                        }

                        if (flag)
                        {
                            if (n == 0)
                                table.Rows.Add(tr);
                            else
                                table.Rows.Add(tr1);

                            dt.Rows.RemoveAt(i);
                        }
                    }

                }

                //处理余下的行
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    System.Web.UI.HtmlControls.HtmlTableRow tr = new System.Web.UI.HtmlControls.HtmlTableRow();
                    DataRow row = dt.Rows[i];

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        DataColumn column = dt.Columns[j];
                        System.Web.UI.HtmlControls.HtmlTableCell td = new System.Web.UI.HtmlControls.HtmlTableCell();
                        td.RowSpan = 1;
                        td.ColSpan = 1;
                        td.InnerText = row[column].ToString();
                        tr.Cells.Add(td);
                    }

                    table.Rows.Add(tr);
                }
            }

            builder.Append("<tbody>");
            
            for (int i = 0; i < table.Rows.Count; i++)
            {
                builder.Append("<tr>");
                for (int j = 0; j < table.Rows[i].Cells.Count; j++)
                {
                    builder.Append("<td style='font:11' rowspan=" + table.Rows[i].Cells[j].RowSpan + ">");
                    builder.Append(table.Rows[i].Cells[j].InnerText);
                    builder.Append("</td>");
                }
                builder.Append("</tr>");

            }
            builder.Append("</tbody>");
            return builder;

            //StringBuilder builder = new StringBuilder();
            //System.Web.UI.HtmlControls.HtmlTable table = new System.Web.UI.HtmlControls.HtmlTable();

            //if (this.dataSource != null)
            //{
            //    DataTable dt = this.dataSource.Tables[0];
            //    for (int i = 0; i < dt.Rows.Count; i++)
            //    {
            //        DataRow row = dt.Rows[i];
            //        builder.Append("<tr>");
            //        for (int j = 0; j < dt.Columns.Count; j++)
            //        {
            //            DataColumn column = dt.Columns[j];
            //            builder.Append("<td>").Append(row[column].ToString()).Append("</td>");
            //        }
            //        builder.Append("</tr>");
            //    }
            //}
            //return builder;
        }

        /// <summary>
        /// Creates the col when deep.
        /// </summary>
        /// <param name="_column">The _column.</param>
        /// <param name="deep">The deep.</param>
        /// <param name="builder">The builder.</param>
        /// <Remarks>
        private void CreateColWhenDeep(HtmlColumn _column, int deep, StringBuilder builder)
        {
            if (deep == 1)
            {
                for (int i = 0; i < _column.ChildColumns.Count; i++)
                {
                    HtmlColumn column = _column.ChildColumns[i];
                    int rowSpan = column.RowSpan;
                    int colSpan = column.GetTotalColSpan();
                    builder.Append("<td rowspan='").Append(rowSpan).Append("' ")
                           .Append("colspan='").Append(colSpan).Append("' ")
                           .Append("align='").Append(column.Header.Style.Align).Append("' ")
                           .Append("style='font-size:").Append(column.Header.Style.FontSize).Append("px;")
                           .Append("color:").Append(column.Header.Style.Color).Append(";'>")
                           .Append(column.Header.HeaderText).Append("</td>");
                }
                return;
            }

            for (int i = 0; i < _column.ChildColumns.Count; i++)
            {
                CreateColWhenDeep(_column.ChildColumns[i], deep - 1, builder);
            }
        }

        /// <summary>
        /// Parses the XML.
        /// </summary>
        /// <returns></returns>
        /// <Remarks>
        public HtmlReport ParseXml()
        {
            report = new HtmlReport();
            if (this.reportXmlFile == null || this.reportXmlFile.Equals(string.Empty))
            {
                throw new Exception("The xml file for reporting is not found!");
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(reportXmlFile);            
            XmlNode rootNode = xmlDoc.DocumentElement;
            report.Capation = CreateCaption(rootNode);
            report.TotalRows = GetTotalRows(rootNode); 
            report.Columns = GetColumns(rootNode);
            return report;
        }

        /// <summary>
        /// Creates the caption.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <returns></returns>
        /// <Remarks>
        private HtmlCaption CreateCaption(XmlNode parentNode)
        {
            HtmlCaption caption = new HtmlCaption();
            XmlNode captionNode = parentNode.SelectSingleNode("Caption");
            if (captionNode != null)
            {
                XmlNode captionTextNode = captionNode.SelectSingleNode("Text");
                if (captionTextNode != null)
                {
                    caption.Title = captionTextNode.InnerText;;
                }
                
                caption.Style = GetStyle(captionNode);
            }
            return caption;
        }

        /// <summary>
        /// Gets the total rows.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <returns></returns>
        /// <Remarks>
        private int GetTotalRows(XmlNode parentNode)
        {
            short totalRow = 3;
            XmlNode totalRowNode = parentNode.SelectSingleNode("TotalRow");
            if (totalRowNode != null)
            {
                short result = 0;
                if (Int16.TryParse(totalRowNode.InnerText, out result))
                {
                    totalRow = result;
                }
            }
            return totalRow;
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <returns></returns>
        /// <Remarks>
        private List<HtmlColumn> GetColumns(XmlNode parentNode)
        {
            List<HtmlColumn> columns = new List<HtmlColumn>();
            XmlNode columnsNode = parentNode.SelectSingleNode("Columns");
            if (columnsNode != null)
            {
                XmlNodeList columnNodes = columnsNode.SelectNodes("Column");
                if (columnNodes != null)
                {
                    for (int i = 0; i < columnNodes.Count; i++)
                    {
                        XmlNode columnNode = columnNodes[i];
                        HtmlColumn htmlColumn = new HtmlColumn();
                        htmlColumn.Header = this.GetHeader(columnNode);
                        XmlNode dataTypeNode = columnNode.SelectSingleNode("DataType");
                        if (dataTypeNode != null)
                        {
                            htmlColumn.DataType = dataTypeNode.InnerText;
                        }
                        XmlNode rowSpanNode = columnNode.SelectSingleNode("RowSpan");
                        int rowSpan = 1;
                        if (rowSpanNode != null)
                        {
                            short result = 0;
                            if (Int16.TryParse(rowSpanNode.InnerText, out result))
                            {
                                rowSpan = result;
                            }
                        }
                        htmlColumn.RowSpan = rowSpan;
                        List<HtmlColumn> list = new List<HtmlColumn>();
                        htmlColumn.ChildColumns = list;
                        GetChildColumns(columnNode, ref list);
                        htmlColumn.IsLeaf = list.Count == 0;
                        columns.Add(htmlColumn);
                    }
                }
            }
            return columns;
        }

        /// <summary>
        /// Gets the child columns.
        /// </summary>
        /// <param name="ParentColumnNode">The parent column node.</param>
        /// <returns></returns>
        /// <Remarks>
        private void GetChildColumns(XmlNode ParentColumnNode, ref List<HtmlColumn> columns)
        {
            XmlNodeList columnNodes = ParentColumnNode.SelectNodes("ChildColumns/Column");
            if (columnNodes != null)
            {
                for (int i = 0; i < columnNodes.Count; i++)
                {
                    XmlNode columnNode = columnNodes[i];
                    HtmlColumn htmlColumn = new HtmlColumn();
                    htmlColumn.Header = this.GetHeader(columnNode);
                    XmlNode dataTypeNode = columnNode.SelectSingleNode("DataType");
                    if (dataTypeNode != null)
                    {
                        htmlColumn.DataType = dataTypeNode.InnerText;
                    }
                    
                    XmlNode rowSpanNode = columnNode.SelectSingleNode("RowSpan");
                    int rowSpan = 1;
                    if (rowSpanNode != null)
                    {
                        short result = 0;
                        if (Int16.TryParse(rowSpanNode.InnerText, out result))
                        {
                            rowSpan = result;
                        }
                    }
                    htmlColumn.RowSpan = rowSpan;
                    columns.Add(htmlColumn);
                    List<HtmlColumn> list = new List<HtmlColumn>();
                    htmlColumn.ChildColumns = list;
                    GetChildColumns(columnNode, ref list);
                    htmlColumn.IsLeaf = list.Count == 0;
                }
            }
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <returns></returns>
        /// <Remarks>
        private HtmlHeader GetHeader(XmlNode parentNode)
        {
            HtmlHeader header = new HtmlHeader();
            XmlNode headerNode = parentNode.SelectSingleNode("Header");
            if (headerNode != null)
            {
                XmlNode headerTextNode = headerNode.SelectSingleNode("HeaderText");
                if (headerTextNode != null)
                {
                    header.HeaderText = headerTextNode.InnerText;
                }

                XmlNode headerValueNode = headerNode.SelectSingleNode("HeaderValue");
                if (headerValueNode != null)
                {
                    header.HeaderValue = headerValueNode.InnerText;
                }

                header.Style = this.GetStyle(headerNode);
            }
            return header;
        }

        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <returns></returns>
        /// <Remarks>
        private HtmlStyle GetStyle(XmlNode parentNode)
        {
            HtmlStyle style = new HtmlStyle();
            XmlNode styleNode = parentNode.SelectSingleNode("Style");
            if (styleNode != null)
            {
                XmlNode alignNode = styleNode.SelectSingleNode("Align");
                if (alignNode != null)
                {
                    style.Align = alignNode.InnerText;
                }
                
                XmlNode fontSizeNode = styleNode.SelectSingleNode("FontSize");
                if (fontSizeNode != null)
                {
                    style.FontSize = fontSizeNode.InnerText;

                }

                XmlNode colorNode = styleNode.SelectSingleNode("Color");
                if (colorNode != null)
                {
                    style.Color = colorNode.InnerText;
                }
            }
            return style;
        }
    }

    public class ReportDataSetConvertor
    {
        public List<string> RowGroupFieldName { get; set; }
        public List<string> RowGroup { get; set; }
        public string ColumnGroupFieldName { get; set; }
        public List<FixedReportColumn> FixedColumns { get; set; }
        public List<ColumnPattern> FixedColumnsPatterns { get; set; }
        public List<ReportColumn> DynamicColumns { get; set; }
        public List<ColumnPattern> DynamicColumnsPatterns { get; set; }
        public DataSet ReportDataDS { get; set; }
        public string NullReplacer { get; set; }
        public List<RowPattern> RowPatterns { get; set; }
        public string ParentColumnFieldName { get; set; }
        public List<string> ColumnOrder { get; set; }
        public List<string> RowOrder { get; set; }
        public List<ReportFixedRow> FixedRows { get; set; }

        private DataTable dtResult;
        public ReportDataSetConvertor()
        {
            RowGroupFieldName = new List<string>();
            RowGroup = new List<string>();
            ColumnGroupFieldName = string.Empty;
            FixedColumns = new List<FixedReportColumn>();
            FixedColumnsPatterns = new List<ColumnPattern>();
            DynamicColumns = new List<ReportColumn>();
            DynamicColumnsPatterns = new List<ColumnPattern>();
            ReportDataDS = null;
            NullReplacer = "-";
            RowPatterns = new List<RowPattern>();
            ParentColumnFieldName = "";
            ColumnOrder = new List<string>();
            RowOrder = new List<string>();
            FixedRows = new List<ReportFixedRow>(); 
        }

        public DataSet Build()
        {
            dtResult = new DataTable();
            //Sort Column
            SortColumnGroup();
            //Sort Row
            SortRowGroup();

            BuildCustomDataSet(dtResult);
            InsertData2DataSet(dtResult, ReportDataDS);

            DataSet ds = new DataSet();
            ds.Tables.Add(dtResult);

            return ds;
        }

        private void SortColumnGroup()
        {
            List<ReportColumn> leftColumns = new List<ReportColumn>();
            List<ReportColumn> rightColumns = new List<ReportColumn>();
            //处理排序的列
            for (int i = 0; i < ColumnOrder.Count; i++)
            {
                for (int j = 0; j < DynamicColumns.Count; j++)
                {
                    if (DynamicColumns[j].ColumnText.Trim().Equals(ColumnOrder[i].Trim()))
                    {
                        leftColumns.Add(DynamicColumns[j]);
                        DynamicColumns.RemoveAt(j);
                        break;
                    }
                }
            }
            //处理排在最后的列
            for (int i = 0; i < ColumnOrder.Count; i++)
            {
                for (int j = 0; j < DynamicColumns.Count; j++)
                {
                    if (DynamicColumns[j].ColumnText.Trim().Equals("->" + ColumnOrder[i].Trim()))
                    {
                        rightColumns.Add(DynamicColumns[j]);
                        DynamicColumns.RemoveAt(j);
                        break;
                    }
                }
            }


            //处理余下的列
            for (int i = 0; i < DynamicColumns.Count; i++)
            {
                leftColumns.Add(DynamicColumns[i]);
            }

            //处理排在尾部的列
            for (int i = 0; i < rightColumns.Count; i++)
            {
                leftColumns.Add(rightColumns[i]);
            }

            DynamicColumns = leftColumns;
        }

        private void SortRowGroup()
        {
            List<string> left = new List<string>();
            List<string> right = new List<string>();

            //处理前部
            for (int i = 0; i < RowOrder.Count; i++)
            {
                for (int j = 0; j < RowGroup.Count; j++)
                {
                    if (RowOrder[i] == RowGroup[j])
                    {
                        left.Add(RowGroup[j]);
                        RowGroup.RemoveAt(j);
                        break;
                    }
                }
            }

            //处理后部
            for (int i = 0; i < RowOrder.Count; i++)
            {
                for (int j = 0; j < RowGroup.Count; j++)
                {
                    if (RowOrder[i] == ("->" + RowGroup[j]))
                    {
                        right.Add(RowGroup[j]);
                        RowGroup.RemoveAt(j);
                        break;
                    }
                }
            }

            //处理其余
            for (int j = 0; j < RowGroup.Count; j++)
            {
                left.Add(RowGroup[j]);
            }

            //添加尾部
            for (int j = 0; j < right.Count; j++)
            {
                left.Add(right[j]);
            }

            RowGroup.Clear();

            for (int i = 0; i < left.Count; i++)
            {
                RowGroup.Add(left[i]);
            }

            ////处理前部
            //for (int i = 0; i < RowOrder.Count; i++)
            //{
            //    for (int j = 0; j < ReportDataDS.Tables[0].Rows.Count; j++)
            //    {
            //        string sValue = string.Empty;
            //        for (int n = 0; n < RowGroupFieldName.Count; n++)
            //        {
            //            sValue = (n == 0 ? "" : ",") + ReportDataDS.Tables[0].Rows[j][RowGroupFieldName[n]].ToString();
            //        }

            //        if (sValue.Trim() == RowOrder[i].Trim())
            //        {
            //            DataRow dr = leftdt.NewRow();
            //            dr.ItemArray = ReportDataDS.Tables[0].Rows[j].ItemArray;
            //            leftdt.Rows.Add(dr);
            //            ReportDataDS.Tables[0].Rows.RemoveAt(j);
            //            j = 0;
            //        }
            //    }
            //}

            ////处理后部
            //for (int i = 0; i < RowOrder.Count; i++)
            //{
            //    for (int j = 0; j < ReportDataDS.Tables[0].Rows.Count; j++)
            //    {
            //        string sValue = string.Empty;
            //        for (int n = 0; n < RowGroupFieldName.Count; n++)
            //        {
            //            sValue = (n == 0 ? "" : ",") + ReportDataDS.Tables[0].Rows[j][RowGroupFieldName[n]].ToString();
            //        }

            //        if (sValue.Trim() == ("->" + RowOrder[i].Trim()))
            //        {
            //            DataRow dr = rightdt.NewRow();
            //            dr.ItemArray = ReportDataDS.Tables[0].Rows[j].ItemArray;
            //            rightdt.Rows.Add(dr);
            //            ReportDataDS.Tables[0].Rows.RemoveAt(j);
            //            j = 0;
            //        }
            //    }
            //}

            ////处理其余
            //for (int j = 0; j < ReportDataDS.Tables[0].Rows.Count; j++)
            //{
            //    DataRow dr = leftdt.NewRow();
            //    dr.ItemArray = ReportDataDS.Tables[0].Rows[j].ItemArray;
            //    leftdt.Rows.Add(dr);
            //}
            
            ////添加尾部
            //for (int j = 0; j < rightdt.Rows.Count; j++)
            //{
            //    DataRow dr = leftdt.NewRow();
            //    dr.ItemArray = rightdt.Rows[j].ItemArray;
            //    leftdt.Rows.Add(dr);
            //}

            //ReportDataDS.Tables.RemoveAt(0);
            //ReportDataDS.Tables.Add(leftdt);
        }

        private Exception_ErrorMessage BuildCustomDataSet(DataTable _dt)
        {
            try
            {
                //Build DataColumn for Fixed Columns
                BuildCustomFixedDataColumns(_dt);

                //Build Data Column for Dynamic columns
                BuildCustomDynamicDataColumns(_dt);

                return Exception_ErrorMessage.NoError;
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

        private Exception_ErrorMessage BuildCustomFixedDataColumns(DataTable _dt)
        {
            //BuilD first colunm
            //if (FixedRows.Count > 0)
            //{
            //    DataColumn column = new DataColumn("RowCategoryXXX", typeof(System.String));
            //    _dt.Columns.Add(column);
            //}

            //Row group as the first column
            for (int i = 0; i < RowGroupFieldName.Count; i++)
            {
                DataColumn column = new DataColumn(RowGroupFieldName[i], typeof(System.String));
                _dt.Columns.Add(column);
            }

            for (int i = 0; i < FixedColumns.Count; i++)
            {
                string fieldName = FixedColumns[i].ParentValue.Trim() == string.Empty ? FixedColumns[i].ValueDataField : FixedColumns[i].ParentValue.Trim() + "_" + FixedColumns[i].ValueDataField;
                DataColumn column = new DataColumn(fieldName, typeof(System.String));
                _dt.Columns.Add(column);
            }

            return Exception_ErrorMessage.NoError;
        }

        private Exception_ErrorMessage BuildCustomDynamicDataColumns(DataTable _dt)
        {
            for (int i = 0; i < DynamicColumns.Count; i++)
            {
                //遍历模式
                for (int j = 0; j < DynamicColumnsPatterns.Count; j++)
                {
                    ColumnPattern pat = DynamicColumnsPatterns[j];

                    if (pat.IncludeCondition.Equals("*"))
                    {
                        //针对所有情况
                        string[] arrCondition = pat.ExcludeCondition.Split(',');
                        if (!arrCondition.Contains(DynamicColumns[i].ColumnValue))
                        {
                            //不包含在排除列表中
                            DataColumn column = new DataColumn((DynamicColumns[i].ParentValue.Equals(string.Empty)?"":(DynamicColumns[i].ParentValue + "_")) + DynamicColumns[i].ColumnValue.Replace(" ", "") + "_" + pat.DataField, typeof(System.String));
                            _dt.Columns.Add(column);
                        }
                    }
                    else
                    {
                        //针对特定列，不考虑排它情况
                        string[] arrCondition = pat.IncludeCondition.Split(',');
                        if (arrCondition.Contains(DynamicColumns[i].ColumnValue))
                        {
                            DataColumn column = new DataColumn((DynamicColumns[i].ParentValue.Equals(string.Empty) ? "" : (DynamicColumns[i].ParentValue + "_")) + DynamicColumns[i].ColumnValue.Replace(" ", "") + "_" + pat.DataField, typeof(System.String));
                            _dt.Columns.Add(column);
                        }
                    }
                }
            }

            return Exception_ErrorMessage.NoError;
        }

        private Exception_ErrorMessage InsertData2DataSet(DataTable _dt, DataSet _ds)
        {
            try
            {
                //Insert fixed columns's data
                InsertFixedColumnsData(_dt, _ds);
                //Insert Dynamic columns's data
                InsertDynamicColumnsData(_dt, _ds);

                return Exception_ErrorMessage.NoError;
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

        private Exception_ErrorMessage InsertFixedColumnsData(DataTable _dt, DataSet _ds)
        {
            return Exception_ErrorMessage.NoError;
        }

        private RowPattern GetRowPattern(string _rowGroup)
        {
            foreach(RowPattern pat in RowPatterns)
            {
               if (pat.RowGroup ==_rowGroup)
                   return pat;
            }

            return null;
        }

        private Exception_ErrorMessage InsertDynamicColumnsData(DataTable _dt, DataSet _ds)
        {
            string dsFilterWhere = string.Empty;
            //遍历所有的Row Group
            for (int n = 0; n < RowGroup.Count; n++)
            {
                //初始化，设置默认值
                dsFilterWhere = string.Empty;
                List<string> listValue = new List<string>();
                for (int i = 0; i < _dt.Columns.Count; i++)
                {
                    //init
                    listValue.Add(NullReplacer);
                }

                //过滤出Group的数据
                string[] arrRowGroupValue = RowGroup[n].Split(',');
                for (int i = 0; i < RowGroupFieldName.Count; i++)
                {

                    dsFilterWhere += (i == 0 ? "" : " and ") + RowGroupFieldName[i] + "='" + arrRowGroupValue[i] + "'";
                }

                DataRow[] rows = _ds.Tables[0].Select(dsFilterWhere);//_ds.Tables[0].Select(RowGroupFieldName + "='" + RowGroup[n] + "'");
                //遍历过滤出来的数据，构造成一条横向数据
                for (int i = 0; i < rows.Length; i++)
                {
                    //填写category数据
                    listValue[0] = rows[i][0].ToString();
                    //填写rowgroup数据
                    for (int x = 0; x < RowGroupFieldName.Count; x++)
                    {
                        int idxRowGroup = _dt.Columns.IndexOf(RowGroupFieldName[x]);
                        listValue[idxRowGroup] = rows[i][RowGroupFieldName[x]].ToString();
                    }

                    //填写Fixed Column数据
                    for (int x = 0; x < FixedColumns.Count; x++)
                    {
                        if (FixedColumns[x].ParentValue.Trim() != string.Empty)
                        {
                            string fieldName = rows[i][FixedColumns[x].ParentDataField].ToString() + "_" + FixedColumns[x].ValueDataField;
                            int idxFixedColumn = _dt.Columns.IndexOf(fieldName);
                            if (idxFixedColumn >= 0)
                            {
                                listValue[idxFixedColumn] = FormatValue(rows[i][FixedColumns[x].ValueDataField]);
                            }
                        }
                        else
                        {
                            int idxFixedColumn = _dt.Columns.IndexOf(FixedColumns[x].ValueDataField);
                            if (idxFixedColumn >= 0)
                            {
                                listValue[idxFixedColumn] = FormatValue(rows[i][FixedColumns[x].ValueDataField]);
                            }
                        }

                        //if (FixedColumns[x].ParentValue.Trim() != string.Empty)
                        //{
                        //    string fieldName = FixedColumns[i].ParentValue.Trim() + "_" + FixedColumns[i].ColumnValue;
                        //    int idxFixedColumn = _dt.Columns.IndexOf(fieldName);
                        //    listValue[idxFixedColumn] = rows[i][FixedColumns[x].ColumnText].ToString();
                        //}
                        //else
                        //{
                        //    int idxFixedColumn = _dt.Columns.IndexOf(FixedColumns[x].ColumnValue);
                        //    listValue[idxFixedColumn] = rows[i][FixedColumns[x].ColumnText].ToString();
                        //}
                    }

                    //匹配Column Pattern
                    for (int j = 0; j < DynamicColumnsPatterns.Count; j++)
                    {
                        ColumnPattern pat = DynamicColumnsPatterns[j];

                        if (pat.IncludeCondition.Equals("*"))
                        {
                            //该模式针对所有列都有效
                            string[] arrCondition = pat.ExcludeCondition.Split(',');
                            if (!arrCondition.Contains(rows[i][ColumnGroupFieldName].ToString()))
                            {
                                string fieldName = string.Empty;

                                if (!ParentColumnFieldName.Trim().Equals(string.Empty))
                                {
                                    fieldName = rows[i][ParentColumnFieldName].ToString().Replace(" ", "") + "_" + rows[i][ColumnGroupFieldName].ToString().Replace(" ", "") + "_" + pat.DataField;
                                }
                                else
                                {
                                    fieldName = rows[i][ColumnGroupFieldName].ToString().Replace(" ", "") + "_" + pat.DataField;
                                }

                                int idx = _dt.Columns.IndexOf(fieldName);
                                if (idx >= 0)
                                {
                                    //found
                                    listValue[idx] = FormatValue(rows[i][pat.DataField]);
                                }
                            }
                        }
                        else
                        {
                            //该模式针对部分列都有效，不判断排它列表
                            string[] arrCondition = pat.IncludeCondition.Split(',');
                            if (arrCondition.Contains(rows[i][ColumnGroupFieldName].ToString()))
                            {
                                string fieldName = string.Empty;

                                if (!ParentColumnFieldName.Trim().Equals(string.Empty))
                                {
                                    fieldName = rows[i][ParentColumnFieldName].ToString().Replace(" ", "") + "_" + rows[i][ColumnGroupFieldName].ToString().Replace(" ", "") + "_" + pat.DataField;
                                }
                                else
                                {
                                    fieldName = rows[i][ColumnGroupFieldName].ToString().Replace(" ", "") + "_" + pat.DataField;
                                }


                                int idx = _dt.Columns.IndexOf(fieldName);
                                if (idx >= 0)
                                {
                                    //found
                                    listValue[idx] = FormatValue(rows[i][pat.DataField]);
                                }
                            }
                        }
                    }
                }
                _dt.Rows.Add(listValue.ToArray<string>());
            }

            return Exception_ErrorMessage.NoError;
        }

        #region BuildCustomHtmlReport
        public void BuildCustomColumns(ref HtmlReportGenerator _generator,ref ReportDataSetConvertor _convert)
        {
            try
            {
                HtmlReport reportStruct = _generator.ParseXml();
                //get data to build htmlreport
                for (int i = 0; i < _convert.DynamicColumns.Count; i++)
                {
                    HtmlColumn parentColumn = new HtmlColumn();
                    parentColumn.Header.HeaderText = _convert.DynamicColumns[i].ColumnText;
                    parentColumn.Header.HeaderValue = _convert.DynamicColumns[i].ColumnValue;
                    parentColumn.ChildColumns = new List<HtmlColumn>();

                    for (int j = 0; j < _convert.DynamicColumnsPatterns.Count; j++)
                    {
                        ColumnPattern pat = _convert.DynamicColumnsPatterns[j];

                        if (pat.IncludeCondition == "*")
                        {
                            //该模式针对所有列都有效
                            string[] arrCondition = pat.ExcludeCondition.Split(',');
                            if (!arrCondition.Contains(_convert.DynamicColumns[i].ColumnText))
                            {
                                HtmlColumn childColumn = new HtmlColumn();
                                childColumn.Header.HeaderText = pat.ColumnText;
                                childColumn.Header.HeaderValue = pat.ColumnText;
                                childColumn.IsLeaf = true;
                                childColumn.RowSpan = 1;
                                parentColumn.ChildColumns.Add(childColumn);
                            }
                        }
                        else
                        {
                            //该模式针对部分列都有效，不判断排它列表
                            string[] arrCondition = pat.IncludeCondition.Split(',');
                            if (arrCondition.Contains(_convert.DynamicColumns[i].ColumnText))
                            {
                                HtmlColumn childColumn = new HtmlColumn();
                                childColumn.Header.HeaderText = pat.ColumnText;
                                childColumn.Header.HeaderValue = pat.ColumnText;
                                childColumn.IsLeaf = true;
                                childColumn.RowSpan = 1;
                                parentColumn.ChildColumns.Add(childColumn);
                            }
                        }
                    }

                    if (parentColumn.ChildColumns.Count > 0)
                        parentColumn.IsLeaf = false;
                    else
                    {
                        parentColumn.IsLeaf = true;
                    }

                    //判断是否有上层节点
                    HtmlColumn pcolumn = FindColumn(reportStruct.Columns, _convert.DynamicColumns[i].ParentValue);

                    if (pcolumn == null)
                    {
                        reportStruct.Columns.Add(parentColumn);
                    }
                    else
                    {
                        pcolumn.ChildColumns.Add(parentColumn);
                        pcolumn.IsLeaf = false;
                    }
                }

                //Sort
                SortReportStruct(ref reportStruct, ref _convert);

                //Add Fixed rows
                AddFixedRows(ref reportStruct,ref  _convert);

                _generator.report = reportStruct;

                //DataSet ds = localDal.GetCustomColumns(_year, _period, Costing_Reporting_SearchType.FE.ToString());
                //HtmlReport reportStruct = _generator.ParseXml();

                //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //{
                //    HtmlColumn column = new HtmlColumn();
                //    column.Header.HeaderText = ds.Tables[0].Rows[i]["wcid"].ToString();
                //    column.ChildColumns = new List<HtmlColumn>();

                //    HtmlColumn childColumn = new HtmlColumn();
                //    childColumn.Header.HeaderText = "Std";
                //    childColumn.IsLeaf = true;
                //    column.ChildColumns.Add(childColumn);

                //    childColumn = new HtmlColumn();
                //    childColumn.Header.HeaderText = "Var";
                //    childColumn.IsLeaf = true;
                //    column.ChildColumns.Add(childColumn);

                //    reportStruct.Columns.Add(column);
                //}

                //return reportStruct;
                ////get data to build htmlreport
                //DataSet ds = localDal.GetCustomColumns(_year, _period,Costing_Reporting_SearchType.FE.ToString());
                //HtmlReport reportStruct = _generator.ParseXml();

                //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //{
                //    HtmlColumn column = new HtmlColumn();
                //    column.Header.HeaderText = ds.Tables[0].Rows[i]["wcid"].ToString();
                //    column.ChildColumns = new List<HtmlColumn>();

                //    HtmlColumn childColumn = new HtmlColumn();
                //    childColumn.Header.HeaderText = "Std";
                //    childColumn.IsLeaf = true;
                //    column.ChildColumns.Add(childColumn);

                //    childColumn = new HtmlColumn();
                //    childColumn.Header.HeaderText = "Var";
                //    childColumn.IsLeaf = true;
                //    column.ChildColumns.Add(childColumn);

                //    reportStruct.Columns.Add(column);
                //}

                //return reportStruct;
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
        #endregion

        private void AddFixedRows(ref HtmlReport _reportStruct, ref ReportDataSetConvertor _convert)
        {
            for (int i = 0; i < _convert.FixedRows.Count; i++)
            {
                HtmlRow row = new HtmlRow();
                row.Header.HeaderText = _convert.FixedRows[i].RowText;
                row.Header.HeaderValue = _convert.FixedRows[i].RowValue;
                row.RowSpan = _convert.FixedRows[i].ChildCount;

                _reportStruct.Rows.Add(row);
            }
        }

        private HtmlColumn FindColumn(List<HtmlColumn> _parentColumns, string _headerValue)
        {
            if (_headerValue.Trim() == string.Empty)
                return null;

            foreach (HtmlColumn column in _parentColumns)
            {
                if (column.Header.HeaderValue == _headerValue)
                {
                    return column;
                }
                else
                {
                    FindColumn(column.ChildColumns, _headerValue);
                }
            }

            return null;
        }

        private void SortReportStruct(ref HtmlReport _reportStruct, ref ReportDataSetConvertor _convert)
        {
            HtmlReport leftReport = new HtmlReport();
            HtmlReport rightReport = new HtmlReport();

            //处理排序的列
            for (int i = 0; i < _convert.ColumnOrder.Count; i++)
            {
                for (int j = 0; j < _reportStruct.Columns.Count; j++)
                {
                    if (_reportStruct.Columns[j].Header.HeaderValue.Trim().Equals(_convert.ColumnOrder[i].Trim()))
                    {
                        leftReport.Columns.Add(_reportStruct.Columns[j]);
                        _reportStruct.Columns.RemoveAt(j);
                        break;
                    }
                }
            }

            //处理排在最后的列
            for (int i = 0; i < _convert.ColumnOrder.Count; i++)
            {
                for (int j = 0; j < _reportStruct.Columns.Count; j++)
                {
                    if (_reportStruct.Columns[j].Header.HeaderValue.Trim().Equals("->" + _convert.ColumnOrder[i].Trim()))
                    {
                        rightReport.Columns.Add(_reportStruct.Columns[j]);
                        _reportStruct.Columns.RemoveAt(j);
                        break;
                    }
                }
            }


            //处理余下的列
            for (int i = 0; i < _reportStruct.Columns.Count; i++)
            {
                leftReport.Columns.Add(_reportStruct.Columns[i]);
            }


            //处理排在尾部的列
            for (int i = 0; i < rightReport.Columns.Count; i++)
            {
                leftReport.Columns.Add(rightReport.Columns[i]);
            }

            _reportStruct.Columns.Clear();

            for (int i = 0; i < leftReport.Columns.Count; i++)
            {
                _reportStruct.Columns.Add(leftReport.Columns[i]);
            }

        }

        private string FormatValue(object obj)
        {
            string ret = string.Empty;

            switch (obj.GetType().Name)
            {
                case "Decimal":
                    ret=((Decimal)obj).ToString("N");
                    break;
                default:
                    ret = "";
                    break;
            }
            return ret;
        }
    }
}
