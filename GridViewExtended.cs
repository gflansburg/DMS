using System.Linq;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Gafware.Modules.DMS
{
    //modified from http://stackoverflow.com/questions/3437581/show-gridview-footer-on-empty-grid
    public class GridViewExtended : GridView
    {

        private GridViewRow _footerRow;
        [DefaultValue(false), Category("Appearance"), Description("Include the footer when the table is empty")]
        public bool ShowFooterWhenEmpty { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public override GridViewRow FooterRow
        {
            get
            {
                if ((this._footerRow == null))
                {
                    this.EnsureChildControls();
                }
                return this._footerRow;
            }
        }

        protected override int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding)
        {
            //creates all the rows that would normally be created when instantiating the grid
            int returnVal = base.CreateChildControls(dataSource, dataBinding);
            //if no rows were created (i.e. returnVal == 0), and we need to show the footer row, then we need to create and bind the footer row.
            if (returnVal == 0 && this.ShowFooterWhenEmpty)
            {
                Table table = this.Controls.OfType<Table>().First<Table>();
                DataControlField[] dcf = new DataControlField[this.Columns.Count];
                this.Columns.CopyTo(dcf, 0);
                //creates the footer row
                this._footerRow = this.CreateRow(-1, -1, DataControlRowType.Footer, DataControlRowState.Normal, dataBinding, null, dcf, table.Rows, null);
                if (!this.ShowFooter)
                {
                    _footerRow.Visible = false;
                }
            }
            return returnVal;
        }

        private GridViewRow CreateRow(int rowIndex, int dataSourceIndex, DataControlRowType rowType, DataControlRowState rowState, bool dataBind, object dataItem, DataControlField[] fields, TableRowCollection rows, PagedDataSource pagedDataSource)
        {
            GridViewRow row = this.CreateRow(rowIndex, dataSourceIndex, rowType, rowState);
            GridViewRowEventArgs e = new GridViewRowEventArgs(row);
            if ((rowType != DataControlRowType.Pager))
            {
                this.InitializeRow(row, fields);
            }
            else
            {
                this.InitializePager(row, fields.Length, pagedDataSource);
            }
            //if the row has data, sets the data item
            if (dataBind)
            {
                row.DataItem = dataItem;
            }
            //Raises the RowCreated event
            this.OnRowCreated(e);
            //adds the row to the gridview's row collection
            rows.Add(row);
            //explicitly binds the data item to the row, including the footer row and raises the RowDataBound event.
            if (dataBind)
            {
                row.DataBind();
                this.OnRowDataBound(e);
                row.DataItem = null;
            }
            return row;
        }

    }
}