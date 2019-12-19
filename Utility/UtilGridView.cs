using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace GotWell.Utility
{
    #region GridViewGroup

    public delegate void GroupEvent(string groupName, object[] values, GridViewRow row);
    /// <summary>
    /// A class that represents a group consisting of a set of columns
    /// </summary>
    public class GridViewGroup
    {
        #region Fields

        private string[] _columns;
        private object[] _actualValues;
        private int _quantity;
        private bool _automatic;
        private bool _hideGroupColumns;
        private bool _isSuppressGroup;
        private bool _generateAllCellsOnSummaryRow;
        private GridViewSummaryList mSummaries;

        #endregion

        #region Properties

        public string[] Columns
        {
            get { return _columns; }
        }

        public object[] ActualValues
        {
            get { return _actualValues; }
        }

        public int Quantity
        {
            get { return _quantity; }
        }

        public bool Automatic
        {
            get { return _automatic; }
            set { _automatic = value; }
        }

        public bool HideGroupColumns
        {
            get { return _hideGroupColumns; }
            set { _hideGroupColumns = value; }
        }

        public bool IsSuppressGroup
        {
            get { return _isSuppressGroup; }
        }

        public bool GenerateAllCellsOnSummaryRow
        {
            get { return _generateAllCellsOnSummaryRow; }
            set { _generateAllCellsOnSummaryRow = value; }
        }

        public string Name
        {
            get { return String.Join("+", this._columns); }
        }

        public GridViewSummaryList Summaries
        {
            get { return mSummaries; }
        }

        #endregion

        #region Constructors

        public GridViewGroup(string[] cols, bool isSuppressGroup, bool auto, bool hideGroupColumns, bool generateAllCellsOnSummaryRow)
        {
            this.mSummaries = new GridViewSummaryList();
            this._actualValues = null;
            this._quantity = 0;
            this._columns = cols;
            this._isSuppressGroup = isSuppressGroup;
            this._automatic = auto;
            this._hideGroupColumns = hideGroupColumns;
            this._generateAllCellsOnSummaryRow = generateAllCellsOnSummaryRow;
        }

        public GridViewGroup(string[] cols, bool auto, bool hideGroupColumns, bool generateAllCellsOnSummaryRow)
            : this(cols, false, auto, hideGroupColumns, generateAllCellsOnSummaryRow)
        {
        }

        public GridViewGroup(string[] cols, bool auto, bool hideGroupColumns)
            : this(cols, auto, hideGroupColumns, false)
        {
        }

        #endregion

        internal void SetActualValues(object[] values)
        {
            this._actualValues = values;
        }

        public bool ContainsSummary(GridViewSummary s)
        {
            return mSummaries.Contains(s);
        }

        public void AddSummary(GridViewSummary s)
        {
            if (this.ContainsSummary(s))
            {
                throw new Exception("Summary already exists in this group.");
            }

            if (!s.Validate())
            {
                throw new Exception("Invalid summary.");
            }

            ///s._group = this;
            s.SetGroup(this);
            this.mSummaries.Add(s);
        }

        public void Reset()
        {
            this._quantity = 0;

            foreach (GridViewSummary s in mSummaries)
            {
                s.Reset();
            }
        }

        public void AddValueToSummaries(object dataitem)
        {
            this._quantity++;

            foreach (GridViewSummary s in mSummaries)
            {
                s.AddValue(DataBinder.Eval(dataitem, s.Column));
            }
        }

        public void CalculateSummaries()
        {
            foreach (GridViewSummary s in mSummaries)
            {
                s.Calculate();
            }
        }
    }
    #endregion

    #region GridViewGroupList
    /// <summary>
    /// Summary description for GridViewGroupList
    /// </summary>
    public class GridViewGroupList : List<GridViewGroup>
    {
        public GridViewGroup this[string name]
        {
            get { return this.FindGroupByName(name); }
        }

        public GridViewGroup FindGroupByName(string name)
        {
            foreach (GridViewGroup g in this)
            {
                if (g.Name.ToLower() == name.ToLower()) return g;
            }

            return null;
        }
    }
    #endregion

    #region GridViewHelper
    public delegate void FooterEvent(GridViewRow row);

    /// <summary>
    /// A class to allow you to add summaries and groups to a GridView, easily!
    /// </summary>
    public class GridViewHelper
    {

        #region Fields

        private GridView mGrid;
        private GridViewSummaryList mGeneralSummaries;
        private GridViewGroupList mGroups;
        private bool useFooter;
        private SortDirection groupSortDir;

        #endregion

        public GridViewGroupList Groups
        {
            get { return mGroups; }
        }

        public GridViewSummaryList GeneralSummaries
        {
            get { return mGeneralSummaries; }
        }


        #region Messages

        private const string USE_ADEQUATE_METHOD_TO_REGISTER_THE_SUMMARY = "Use adequate method to register a summary with custom operation.";
        private const string GROUP_NOT_FOUND = "Group {0} not found. Please register the group before the summary.";
        private const string INVALID_SUMMARY = "Invalid summary.";
        private const string SUPPRESS_GROUP_ALREADY_DEFINED = "A suppress group is already defined. You can't define suppress AND summary groups simultaneously";
        private const string ONE_GROUP_ALREADY_REGISTERED = "At least a group is already defined. A suppress group can't coexist with other groups";

        #endregion


        #region Events

        /// <summary>
        /// Event triggered when a new group starts
        /// </summary>
        public event GroupEvent GroupStart;

        /// <summary>
        /// Event triggered when a group ends
        /// </summary>
        public event GroupEvent GroupEnd;

        /// <summary>
        /// Event triggered after a row for group header be inserted
        /// </summary>
        public event GroupEvent GroupHeader;

        /// <summary>
        /// Event triggered after a row for group summary be inserted
        /// </summary>
        public event GroupEvent GroupSummary;

        /// <summary>
        /// Event triggered after the general summaries be generated
        /// </summary>
        public event FooterEvent GeneralSummary;

        /// <summary>
        /// Event triggered when the footer is databound
        /// </summary>
        public event FooterEvent FooterDataBound;

        #endregion


        #region Constructors

        public GridViewHelper(GridView grd) : this(grd, false, SortDirection.Ascending) { }

        public GridViewHelper(GridView grd, bool useFooterForGeneralSummaries) : this(grd, useFooterForGeneralSummaries, SortDirection.Ascending) { }

        public GridViewHelper(GridView grd, bool useFooterForGeneralSummaries, SortDirection groupSortDirection)
        {
            this.mGrid = grd;
            this.useFooter = useFooterForGeneralSummaries;
            this.groupSortDir = groupSortDirection;
            this.mGeneralSummaries = new GridViewSummaryList();
            this.mGroups = new GridViewGroupList();
            this.mGrid.RowDataBound += new GridViewRowEventHandler(RowDataBoundHandler);
        }

        #endregion


        #region RegisterSummary overloads

        public GridViewSummary RegisterSummary(string column, SummaryOperation operation)
        {
            return this.RegisterSummary(column, String.Empty, operation);
        }

        public GridViewSummary RegisterSummary(string column, string formatString, SummaryOperation operation)
        {
            if (operation == SummaryOperation.Custom)
            {
                throw new Exception(USE_ADEQUATE_METHOD_TO_REGISTER_THE_SUMMARY);
            }

            // TO DO: Perform column validation...
            GridViewSummary s = new GridViewSummary(column, formatString, operation, null);
            mGeneralSummaries.Add(s);

            // if general summaries are displayed in the footer, it must be set to visible
            if (useFooter) mGrid.ShowFooter = true;

            return s;
        }

        public GridViewSummary RegisterSummary(string column, SummaryOperation operation, string groupName)
        {
            return this.RegisterSummary(column, String.Empty, operation, groupName);
        }

        public GridViewSummary RegisterSummary(string column, string formatString, SummaryOperation operation, string groupName)
        {
            if (operation == SummaryOperation.Custom)
            {
                throw new Exception(USE_ADEQUATE_METHOD_TO_REGISTER_THE_SUMMARY);
            }

            GridViewGroup group = mGroups[groupName];
            if (group == null)
            {
                throw new Exception(String.Format(GROUP_NOT_FOUND, groupName));
            }

            // TO DO: Perform column validation...
            GridViewSummary s = new GridViewSummary(column, formatString, operation, group);
            group.AddSummary(s);

            return s;
        }

        public GridViewSummary RegisterSummary(string column, CustomSummaryOperation operation, SummaryResultMethod getResult)
        {
            return RegisterSummary(column, String.Empty, operation, getResult);
        }

        public GridViewSummary RegisterSummary(string column, string formatString, CustomSummaryOperation operation, SummaryResultMethod getResult)
        {
            // TO DO: Perform column validation...
            GridViewSummary s = new GridViewSummary(column, formatString, operation, getResult, null);
            mGeneralSummaries.Add(s);

            // if general summaries are displayed in the footer, it must be set to visible
            if (useFooter) mGrid.ShowFooter = true;

            return s;
        }

        public GridViewSummary RegisterSummary(string column, CustomSummaryOperation operation, SummaryResultMethod getResult, string groupName)
        {
            return RegisterSummary(column, String.Empty, operation, getResult, groupName);
        }

        public GridViewSummary RegisterSummary(string column, string formatString, CustomSummaryOperation operation, SummaryResultMethod getResult, string groupName)
        {
            GridViewGroup group = mGroups[groupName];
            if (group == null)
            {
                throw new Exception(String.Format(GROUP_NOT_FOUND, groupName));
            }

            // TO DO: Perform column validation...
            GridViewSummary s = new GridViewSummary(column, formatString, operation, getResult, group);
            group.AddSummary(s);

            return s;
        }

        public GridViewSummary RegisterSummary(GridViewSummary s)
        {
            if (!s.Validate())
            {
                throw new Exception(INVALID_SUMMARY);
            }

            if (s.Group == null)
            {
                // if general summaries are displayed in the footer, it must be set to visible
                if (useFooter) mGrid.ShowFooter = true;

                mGeneralSummaries.Add(s);
            }
            else if (!s.Group.ContainsSummary(s))
            {
                s.Group.AddSummary(s);
            }

            return s;
        }

        #endregion


        #region RegisterGroup overloads

        public GridViewGroup RegisterGroup(string column, bool auto, bool hideGroupColumns)
        {
            string[] cols = new string[1] { column };
            return RegisterGroup(cols, auto, hideGroupColumns);
        }

        public GridViewGroup RegisterGroup(string[] columns, bool auto, bool hideGroupColumns)
        {
            if (HasSuppressGroup())
            {
                throw new Exception(SUPPRESS_GROUP_ALREADY_DEFINED);
            }

            // TO DO: Perform column validation...
            GridViewGroup g = new GridViewGroup(columns, auto, hideGroupColumns);
            mGroups.Add(g);

            if (hideGroupColumns)
            {
                for (int i = 0; i < mGrid.Columns.Count; i++)
                {
                    for (int j = 0; j < columns.Length; j++)
                    {
                        if (GetDataFieldName(mGrid.Columns[i]).ToLower() == columns[j].ToLower())
                        {
                            mGrid.Columns[i].Visible = false;
                        }
                    }
                }
            }

            return g;
        }

        #endregion


        #region SetSuppressGroup overloads

        public GridViewGroup SetSuppressGroup(string column)
        {
            string[] cols = new string[1] { column };
            return SetSuppressGroup(cols);
        }

        public GridViewGroup SetSuppressGroup(string[] columns)
        {
            if (mGroups.Count > 0)
            {
                throw new Exception(ONE_GROUP_ALREADY_REGISTERED);
            }

            // TO DO: Perform column validation...
            GridViewGroup g = new GridViewGroup(columns, true, false, false, false);
            mGroups.Add(g);

            // Disable paging because pager works in datarows that
            // will be suppressed
            mGrid.AllowPaging = false;

            return g;
        }

        #endregion


        #region Private Helper functions

        private string GetSequentialGroupColumns()
        {
            string ret = String.Empty;

            foreach (GridViewGroup g in mGroups)
            {
                ret += g.Name.Replace('+', ',') + ",";
            }
            return ret.Substring(0, ret.Length - 1);
        }

        /// <summary>
        /// Compares the actual group values with the values of the current dataitem
        /// </summary>
        /// <param name="g"></param>
        /// <param name="dataitem"></param>
        /// <returns></returns>
        private bool EvaluateEquals(GridViewGroup g, object dataitem)
        {
            // The values wasn't initialized
            if (g.ActualValues == null) return false;

            for (int i = 0; i < g.Columns.Length; i++)
            {
                if (g.ActualValues[i] == null && DataBinder.Eval(dataitem, g.Columns[i]) != null) return false;
                if (g.ActualValues[i] != null && DataBinder.Eval(dataitem, g.Columns[i]) == null) return false;
                if (!g.ActualValues[i].Equals(DataBinder.Eval(dataitem, g.Columns[i]))) return false;
            }

            return true;
        }

        private bool HasSuppressGroup()
        {
            foreach (GridViewGroup g in mGroups)
            {
                if (g.IsSuppressGroup) return true;
            }
            return false;
        }

        private bool HasAutoSummary(List<GridViewSummary> list)
        {
            foreach (GridViewSummary s in list)
            {
                if (s.Automatic) return true;
            }
            return false;
        }

        private object[] GetGroupRowValues(GridViewGroup g, object dataitem)
        {
            object[] values = new object[g.Columns.Length];

            for (int i = 0; i < g.Columns.Length; i++)
            {
                values[i] = DataBinder.Eval(dataitem, g.Columns[i]);
            }

            return values;
        }

        /// <summary>
        /// Inserts a grid row. Only cells required for the summary results
        /// will be created (except if GenerateAllCellsOnSummaryRow is true).
        /// The group will be checked for columns with summary
        /// </summary>
        /// <param name="beforeRow"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        private GridViewRow InsertGridRow(GridViewRow beforeRow, GridViewGroup g)
        {
            int colspan;
            TableCell cell;
            TableCell[] tcArray;
            int visibleColumns = this.GetVisibleColumnCount();

            Table tbl = (Table)mGrid.Controls[0];
            int newRowIndex = tbl.Rows.GetRowIndex(beforeRow);
            GridViewRow newRow = new GridViewRow(newRowIndex, newRowIndex, DataControlRowType.DataRow, DataControlRowState.Normal);

            if (g != null && (g.IsSuppressGroup || g.GenerateAllCellsOnSummaryRow))
            {
                // Create all the table cells
                tcArray = new TableCell[visibleColumns];
                for (int i = 0; i < visibleColumns; i++)
                {
                    cell = new TableCell();
                    cell.ApplyStyle(mGrid.Columns[GetRealIndexFromVisibleColumnIndex(i)].ItemStyle);
                    cell.Text = "&nbsp;";
                    tcArray[i] = cell;
                }
            }
            else
            {
                // Create only the required table cells
                colspan = 0;
                List<TableCell> tcc = new List<TableCell>();
                for (int i = 0; i < mGrid.Columns.Count; i++)
                {
                    if (ColumnHasSummary(i, g))
                    {
                        if (colspan > 0)
                        {
                            cell = new TableCell();
                            cell.Text = "&nbsp;";
                            cell.ColumnSpan = colspan;
                            tcc.Add(cell);
                            colspan = 0;
                        }

                        // insert table cell and copy the style
                        cell = new TableCell();
                        cell.ApplyStyle(mGrid.Columns[i].ItemStyle);
                        tcc.Add(cell);
                    }
                    else if (mGrid.Columns[i].Visible)
                    {
                        // A visible column that will have no cell because has
                        // no summary. So we increase the colspan...
                        colspan++;
                    }
                }

                if (colspan > 0)
                {
                    cell = new TableCell();
                    cell.Text = "&nbsp;";
                    cell.ColumnSpan = colspan;
                    tcc.Add(cell);
                    colspan = 0;
                }

                tcArray = new TableCell[tcc.Count];
                tcc.CopyTo(tcArray);
            }

            newRow.Cells.AddRange(tcArray);
            tbl.Controls.AddAt(newRowIndex, newRow);

            return newRow;
        }

        #endregion


        #region Core

        private void RowDataBoundHandler(object sender, GridViewRowEventArgs e)
        {
            foreach (GridViewGroup g in mGroups)
            {
                // The last group values are caught here
                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    g.CalculateSummaries();
                    GenerateGroupSummary(g, e.Row);
                    if (GroupEnd != null)
                    {
                        GroupEnd(g.Name, g.ActualValues, e.Row);
                    }
                }
                else if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ProcessGroup(g, e);
                    if (g.IsSuppressGroup)
                    {
                        e.Row.Visible = false;
                    }
                }
                else if (e.Row.RowType == DataControlRowType.Pager)
                {
                    // Workaround to strange behavior (ColumnSpan not rendered)
                    // when AllowPaging=true
                    // Found at: http://aspadvice.com/blogs/joteke/archive/2006/02/11/15130.aspx
                    TableCell originalCell = e.Row.Cells[0];
                    TableCell newCell = new TableCell();
                    newCell.Visible = false;
                    e.Row.Cells.AddAt(0, newCell);
                    originalCell.ColumnSpan = this.GetVisibleColumnCount();
                }
            }

            // This will deal only with general summaries
            foreach (GridViewSummary s in mGeneralSummaries)
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    // Essentially this isn't required, but it prevents wrong calc
                    // in case of RowDataBound event be called twice (for each row)
                    s.Reset();
                }
                else if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    s.AddValue(DataBinder.Eval(e.Row.DataItem, s.Column));
                }
                else if (e.Row.RowType == DataControlRowType.Footer)
                {
                    s.Calculate();
                }
            }

            if (e.Row.RowType == DataControlRowType.Footer)
            {
                // Automatic generation of summary
                GenerateGeneralSummaries(e);

                // Triggers event footerdatabound
                if (FooterDataBound != null)
                {
                    FooterDataBound(e.Row);
                }
            }
        }

        private void ProcessGroup(GridViewGroup g, GridViewRowEventArgs e)
        {
            string groupHeaderText = String.Empty;

            // Check if it's still in the same group values
            if (!EvaluateEquals(g, e.Row.DataItem))
            {
                // Check if a group ends or if it is the first group values starting...
                if (g.ActualValues != null)
                {
                    g.CalculateSummaries();
                    GenerateGroupSummary(g, e.Row);

                    // Triggers event GroupEnd
                    if (GroupEnd != null)
                    {
                        GroupEnd(g.Name, g.ActualValues, e.Row);
                    }
                }

                // Another group values starts now
                g.Reset();
                g.SetActualValues(GetGroupRowValues(g, e.Row.DataItem));

                // If group is automatic inserts a group header
                if (g.Automatic)
                {
                    for (int v = 0; v < g.ActualValues.Length; v++)
                    {
                        if (g.ActualValues[v] == null) continue;
                        groupHeaderText += g.ActualValues[v].ToString();
                        if (g.ActualValues.Length - v > 1)
                        {
                            groupHeaderText += " - ";
                        }
                    }

                    GridViewRow newRow = InsertGridRow(e.Row);
                    newRow.Cells[0].Text = groupHeaderText;

                    // Triggers event GroupHeader
                    if (GroupHeader != null)
                    {
                        GroupHeader(g.Name, g.ActualValues, newRow);
                    }
                }

                // Triggers event GroupStart
                if (GroupStart != null)
                {
                    GroupStart(g.Name, g.ActualValues, e.Row);
                }
            }

            g.AddValueToSummaries(e.Row.DataItem);
        }

        private string GetFormatedString(string preferredFormat, string secondFormat, object value)
        {
            String format = preferredFormat;
            if (format.Length == 0)
            {
                format = secondFormat;
            }

            if (format.Length > 0)
                return String.Format(format, value);
            else
                return value.ToString();
        }

        private void GenerateGroupSummary(GridViewGroup g, GridViewRow row)
        {
            int colIndex;
            object colValue;

            if (!HasAutoSummary(g.Summaries) && !HasSuppressGroup()) return;

            // Inserts a new row 
            GridViewRow newRow = InsertGridRow(row, g);

            foreach (GridViewSummary s in g.Summaries)
            {
                if (s.Automatic)
                {
                    colIndex = GetVisibleColumnIndex(s.Column);
                    colIndex = ResolveCellIndex(newRow, colIndex);
                    newRow.Cells[colIndex].Text = this.GetFormatedString(s.FormatString, this.GetColumnFormat(GetColumnIndex(s.Column)), s.Value);
                }
            }

            // If it is a suppress group must set the grouped values in the cells
            // of the inserted row
            if (g.IsSuppressGroup)
            {
                for (int i = 0; i < g.Columns.Length; i++)
                {
                    colValue = g.ActualValues[i];
                    if (colValue != null)
                    {
                        colIndex = GetVisibleColumnIndex(g.Columns[i]);
                        colIndex = ResolveCellIndex(newRow, colIndex);
                        newRow.Cells[colIndex].Text = colValue.ToString();
                    }
                }
            }

            // Triggers event GroupSummary
            if (GroupSummary != null)
            {
                GroupSummary(g.Name, g.ActualValues, newRow);
            }

        }

        /// <summary>
        /// Generates the general summaries in the grid. 
        /// </summary>
        /// <param name="e">GridViewRowEventArgs</param>
        private void GenerateGeneralSummaries(GridViewRowEventArgs e)
        {
            int colIndex;
            GridViewRow row;

            if (!HasAutoSummary(this.mGeneralSummaries))
            {
                // Triggers event GeneralSummary
                if (GeneralSummary != null)
                {
                    GeneralSummary(e.Row);
                }

                return;
            }

            if (useFooter)
                row = e.Row;
            else
                row = InsertGridRow(e.Row, null);

            foreach (GridViewSummary s in mGeneralSummaries)
            {
                if (!s.Automatic) continue;

                if (useFooter)
                    colIndex = GetColumnIndex(s.Column);
                else
                    colIndex = GetVisibleColumnIndex(s.Column);

                colIndex = ResolveCellIndex(row, colIndex);
                row.Cells[colIndex].Text = this.GetFormatedString(s.FormatString, this.GetColumnFormat(GetColumnIndex(s.Column)), s.Value);
            }

            // Triggers event GeneralSummary
            if (GeneralSummary != null)
            {
                GeneralSummary(row);
            }

        }

        /// <summary>
        /// Identifies the equivalent index on a row that contains cells with colspan
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        private int ResolveCellIndex(GridViewRow row, int colIndex)
        {
            int colspansum = 0;
            int realIndex;

            for (int i = 0; i < row.Cells.Count; i++)
            {
                realIndex = i + colspansum;
                if (realIndex == colIndex) return i;

                if (row.Cells[i].ColumnSpan > 1)
                {
                    colspansum = colspansum + row.Cells[i].ColumnSpan - 1;
                }
            }

            return -1;
        }

        private bool ColumnHasSummary(int colindex, GridViewGroup g)
        {
            List<GridViewSummary> list;
            string column = this.GetDataFieldName(mGrid.Columns[colindex]);

            if (g == null)
                list = this.mGeneralSummaries;
            else
                list = g.Summaries;

            foreach (GridViewSummary s in list)
            {
                if (column.ToLower() == s.Column.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        private bool ColumnHasSummary(string column, GridViewGroup g)
        {
            List<GridViewSummary> list;

            if (g == null)
                list = this.mGeneralSummaries;
            else
                list = g.Summaries;

            foreach (GridViewSummary s in list)
            {
                if (column.ToLower() == s.Column.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        #endregion


        #region Public Helper functions

        public int GetRealIndexFromVisibleColumnIndex(int visibleIndex)
        {
            int visibles = 0;
            for (int i = 0; i < mGrid.Columns.Count; i++)
            {
                if (mGrid.Columns[i].Visible)
                {
                    if (visibleIndex == visibles) return i;
                    visibles++;
                }
            }

            // Not found....
            return -1;
        }

        public int GetVisibleColumnIndex(string columnName)
        {
            int visibles = 0;
            for (int i = 0; i < mGrid.Columns.Count; i++)
            {
                if (GetDataFieldName(mGrid.Columns[i]).ToLower() == columnName.ToLower())
                {
                    return visibles;
                }

                if (mGrid.Columns[i].Visible) visibles++;
            }

            // Not found....
            return -1;
        }

        public int GetColumnIndex(string columnName)
        {
            for (int i = 0; i < mGrid.Columns.Count; i++)
            {
                if (GetDataFieldName(mGrid.Columns[i]).ToLower() == columnName.ToLower())
                {
                    return i;
                }
            }

            // Not found....
            return -1;
        }

        public string GetDataFieldName(DataControlField field)
        {
            // TO DO: Enable search in HyperLinkField, ButtonField...

            if (field is BoundField)
            {
                return (field as BoundField).DataField;
            }
            else
            {
                // It hopes that SortExpression is set (and it's equal to column name)
                return field.SortExpression;
            }
        }

        public string GetColumnFormat(int colIndex)
        {
            // TO DO: Enable search in HyperLinkField, ButtonField...

            if (mGrid.Columns[colIndex] is BoundField)
            {
                return (mGrid.Columns[colIndex] as BoundField).DataFormatString;
            }
            else
            {
                return String.Empty;
            }
        }

        public int GetVisibleColumnCount()
        {
            int ret = 0;

            for (int i = 0; i < mGrid.Columns.Count; i++)
            {
                if (mGrid.Columns[i].Visible) ret++;
            }

            return ret;
        }

        /// <summary>
        /// This method must be called to hide columns that doesn't 
        /// have any summary operation when we are using a suppress group
        /// </summary>
        public void HideColumnsWithoutGroupSummary()
        {
            string colname;
            bool colChecked;

            foreach (DataControlField dcf in mGrid.Columns)
            {
                colChecked = false;
                colname = GetDataFieldName(dcf).ToLower();

                foreach (GridViewGroup g in mGroups)
                {
                    // Check if it's part of the group columns
                    for (int j = 0; j < g.Columns.Length; j++)
                    {
                        if (colname == g.Columns[j].ToLower())
                        {
                            colChecked = true;
                            break;
                        }
                    }

                    if (colChecked) break;

                    // Check if it's part of a group summary
                    colChecked = ColumnHasSummary(colname, g);

                    if (colChecked) break;
                }

                if (colChecked) continue;

                dcf.Visible = false;

            }
        }


        /// <summary>
        /// Legacy name...
        /// </summary>
        public void SetInvisibleColumnsWithoutGroupSummary()
        {
            this.HideColumnsWithoutGroupSummary();
        }

        /// <summary>
        ///  Inserts a grid row with one cell only
        /// </summary>
        /// <param name="beforeRow"></param>
        /// <returns></returns>
        public GridViewRow InsertGridRow(GridViewRow beforeRow)
        {
            int visibleColumns = this.GetVisibleColumnCount();

            Table tbl = (Table)mGrid.Controls[0];
            int newRowIndex = tbl.Rows.GetRowIndex(beforeRow);
            GridViewRow newRow = new GridViewRow(newRowIndex, newRowIndex, DataControlRowType.DataRow, DataControlRowState.Normal);

            newRow.Cells.Add(new TableCell());
            if (visibleColumns > 1)
            {
                newRow.Cells[0].ColumnSpan = visibleColumns;
            }

            tbl.Controls.AddAt(newRowIndex, newRow);

            return newRow;
        }

        public void ApplyGroupSort()
        {
            mGrid.Sort(this.GetSequentialGroupColumns(), groupSortDir);
        }

        #endregion
    }
    #endregion

    #region GridViewSummary
    public enum SummaryOperation { Sum, Avg, Count, Custom }
    public delegate void CustomSummaryOperation(string column, string groupName, object value);
    public delegate object SummaryResultMethod(string column, string groupName);

    /// <summary>
    /// A class that represents a summary operation defined to a column
    /// </summary>
    public class GridViewSummary
    {
        #region Fields

        private string _column;
        private SummaryOperation _operation;
        private CustomSummaryOperation _customOperation;
        private SummaryResultMethod _getSummaryMethod;
        private GridViewGroup _group;
        private object _value;
        private string _formatString;
        private int _quantity;
        private bool _automatic;
        private bool _treatNullAsZero;

        #endregion

        #region Properties

        public string Column
        {
            get { return _column; }
        }

        public SummaryOperation Operation
        {
            get { return _operation; }
        }

        public CustomSummaryOperation CustomOperation
        {
            get { return _customOperation; }
        }

        public SummaryResultMethod GetSummaryMethod
        {
            get { return _getSummaryMethod; }
        }

        public GridViewGroup Group
        {
            get { return _group; }
        }

        public object Value
        {
            get { return _value; }
        }

        public string FormatString
        {
            get { return _formatString; }
            set { _formatString = value; }
        }

        public int Quantity
        {
            get { return _quantity; }
        }

        public bool Automatic
        {
            get { return _automatic; }
            set { _automatic = value; }
        }

        public bool TreatNullAsZero
        {
            get { return _treatNullAsZero; }
            set { _treatNullAsZero = value; }
        }

        #endregion

        #region Constructors

        private GridViewSummary(string col, GridViewGroup grp)
        {
            this._column = col;
            this._group = grp;
            this._value = null;
            this._quantity = 0;
            this._automatic = true;
            this._treatNullAsZero = false;
        }

        public GridViewSummary(string col, string formatString, SummaryOperation op, GridViewGroup grp)
            : this(col, grp)
        {
            this._formatString = formatString;
            this._operation = op;
            this._customOperation = null;
            this._getSummaryMethod = null;
        }

        public GridViewSummary(string col, SummaryOperation op, GridViewGroup grp)
            : this(col, String.Empty, op, grp)
        {
        }

        public GridViewSummary(string col, string formatString, CustomSummaryOperation op, SummaryResultMethod getResult, GridViewGroup grp)
            : this(col, grp)
        {
            this._formatString = formatString;
            this._operation = SummaryOperation.Custom;
            this._customOperation = op;
            this._getSummaryMethod = getResult;
        }

        public GridViewSummary(string col, CustomSummaryOperation op, SummaryResultMethod getResult, GridViewGroup grp)
            : this(col, String.Empty, op, getResult, grp)
        {
        }

        #endregion

        internal void SetGroup(GridViewGroup g)
        {
            this._group = g;
        }

        public bool Validate()
        {
            if (this._operation == SummaryOperation.Custom)
            {
                return (this._customOperation != null && this._getSummaryMethod != null);
            }
            else
            {
                return (this._customOperation == null && this._getSummaryMethod == null);
            }
        }

        public void Reset()
        {
            this._quantity = 0;
            this._value = null;
        }

        public void AddValue(object newValue)
        {
            // Increment to (later) calc the Avg or for other calcs
            this._quantity++;

            // Built-in operations
            if (this._operation == SummaryOperation.Sum || this._operation == SummaryOperation.Avg)
            {
                if (this._value == null)
                    this._value = newValue;
                else
                    this._value = PerformSum(this._value, newValue);
            }
            else
            {
                // Custom operation
                if (this._customOperation != null)
                {
                    // Call the custom operation
                    if (this._group != null)
                        this._customOperation(this._column, this._group.Name, newValue);
                    else
                        this._customOperation(this._column, null, newValue);
                }
            }
        }

        public void Calculate()
        {
            if (this._operation == SummaryOperation.Avg)
            {
                this._value = PerformDiv(this._value, this._quantity);
            }
            if (this._operation == SummaryOperation.Count)
            {
                this._value = this._quantity;
            }
            else if (this._operation == SummaryOperation.Custom)
            {
                if (this._getSummaryMethod != null)
                {
                    this._value = this._getSummaryMethod(this._column, null);
                }
            }
            // if this.Operation == SummaryOperation.Avg
            // this.Value already contains the correct value
        }

        #region Built-in Summary Operations

        private object PerformSum(object a, object b)
        {
            object zero = 0;

            if (a == null)
            {
                if (_treatNullAsZero)
                    a = 0;
                else
                    return null;
            }

            if (b == null)
            {
                if (_treatNullAsZero)
                    b = 0;
                else
                    return null;
            }

            // Convert to proper type before add
            switch (a.GetType().FullName)
            {
                case "System.Int16": return Convert.ToInt16(a) + Convert.ToInt16(b);
                case "System.Int32": return Convert.ToInt32(a) + Convert.ToInt32(b);
                case "System.Int64": return Convert.ToInt64(a) + Convert.ToInt64(b);
                case "System.UInt16": return Convert.ToUInt16(a) + Convert.ToUInt16(b);
                case "System.UInt32": return Convert.ToUInt32(a) + Convert.ToUInt32(b);
                case "System.UInt64": return Convert.ToUInt64(a) + Convert.ToUInt64(b);
                case "System.Single": return Convert.ToSingle(a) + Convert.ToSingle(b);
                case "System.Double": return Convert.ToDouble(a) + Convert.ToDouble(b);
                case "System.Decimal": return Convert.ToDecimal(a) + Convert.ToDecimal(b);
                case "System.Byte": return Convert.ToByte(a) + Convert.ToByte(b);
                case "System.String": return a.ToString() + b.ToString();
            }

            return null;
        }

        private object PerformDiv(object a, int b)
        {
            object zero = 0;

            if (a == null)
            {
                return (_treatNullAsZero ? zero : null);
            }

            // Don't raise an exception, just return null
            if (b == 0)
            {
                return null;
            }

            // Convert to proper type before div
            switch (a.GetType().FullName)
            {
                case "System.Int16": return Convert.ToInt16(a) / b;
                case "System.Int32": return Convert.ToInt32(a) / b;
                case "System.Int64": return Convert.ToInt64(a) / b;
                case "System.UInt16": return Convert.ToUInt16(a) / b;
                case "System.UInt32": return Convert.ToUInt32(a) / b;
                case "System.Single": return Convert.ToSingle(a) / b;
                case "System.Double": return Convert.ToDouble(a) / b;
                case "System.Decimal": return Convert.ToDecimal(a) / b;
                case "System.Byte": return Convert.ToByte(a) / b;
                // Operator '/' cannot be applied to operands of type 'ulong' and 'int'
                //case "System.UInt64": return Convert.ToUInt64(a) / b;
            }

            return null;
        }

        #endregion

    }
    #endregion

    #region GridViewSummaryList
    /// <summary>
    /// Summary description for GridViewSummaryList
    /// </summary>
    public class GridViewSummaryList : List<GridViewSummary>
    {
        public GridViewSummary this[string name]
        {
            get { return this.FindSummaryByColumn(name); }
        }

        public GridViewSummary FindSummaryByColumn(string columnName)
        {
            foreach (GridViewSummary s in this)
            {
                if (s.Column.ToLower() == columnName.ToLower()) return s;
            }

            return null;
        }
    }
    #endregion

    public class MergeGridView
    {
        public void Merge(GridView grid, int cellNum)
        {
            int i = 0, rowSpanNum = 1;
            while (i < grid.Rows.Count - 1)
            {
                GridViewRow gvr = grid.Rows[i];
                for (++i; i < grid.Rows.Count; i++)
                {
                    GridViewRow gvrNext = grid.Rows[i];
                    if (gvr.Cells[cellNum].Text == gvrNext.Cells[cellNum].Text)
                    {
                        gvrNext.Cells[cellNum].Visible = false;
                        rowSpanNum++;
                    }
                    else
                    {
                        gvr.Cells[cellNum].RowSpan = rowSpanNum;
                        rowSpanNum = 1;
                        break;
                    } if (i == grid.Rows.Count - 1)
                    {
                        gvr.Cells[cellNum].RowSpan = rowSpanNum;
                    }
                }
            }
        }

        public void Merge(GridView grid, int cellNum, int cellNum2)
        {
            int i = 0, rowSpanNum = 1;
            while (i < grid.Rows.Count - 1)
            {
                GridViewRow gvr = grid.Rows[i];
                for (++i; i < grid.Rows.Count; i++)
                {
                    GridViewRow gvrNext = grid.Rows[i];
                    if (gvr.Cells[cellNum].Text + gvr.Cells[cellNum2].Text == gvrNext.Cells[cellNum].Text + gvrNext.Cells[cellNum2].Text)
                    {
                        gvrNext.Cells[cellNum].Visible = false;
                        rowSpanNum++;
                    }
                    else
                    {
                        gvr.Cells[cellNum].RowSpan = rowSpanNum;
                        rowSpanNum = 1;
                        break;
                    }

                    if (i == grid.Rows.Count - 1)
                    {
                        gvr.Cells[cellNum].RowSpan = rowSpanNum;
                    }
                }
            }
        }
    }
}
