using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Gafware.Modules.DMS
{
    public partial class LetterFilter : System.Web.UI.UserControl
    {
        public delegate void LetterFilterEventHandler(object sender, LetterFilterEventArgs e);
        public event LetterFilterEventHandler Click;

        public class LetterFilterEventArgs : EventArgs
        {
            string Filter = String.Empty;

            public LetterFilterEventArgs(string filter)
            {
                Filter = filter;
            }
        }

        public string[] Letters
        {
            get
            {
                return (ViewState["Letters"] != null ? (string[])ViewState["Letters"] : new string[] { "#", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", 
                             "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", 
                             "U", "V", "W", "X", "Y", "Z", "All"});
            }
            set
            {
                ViewState["Letters"] = value;
            }
        }

        protected string GetSpacer(int rowNumber)
        {
            if (rowNumber < Letters.Length - 1)
            {
                return " | ";
            }
            return String.Empty;
        }

        public System.Drawing.Color ForeColor
        {
            get
            {
                return (ViewState["ForeColor"] != null ? (System.Drawing.Color)ViewState["ForeColor"] : System.Drawing.Color.Blue);
            }
            set
            {
                ViewState["ForeColor"] = value;
            }
        }

        public string Filter
        {
            get
            {
                return (ViewState["Filter"] != null ? ViewState["Filter"].ToString() : "All");
            }
            set
            {
                ViewState["Filter"] = value;
                rptLetters_Bind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rptLetters_Bind();
            }
        }

        //--------------------------------------------------------------------------
        // Letters Repeater Methods and Events
        //--------------------------------------------------------------------------

        //--------------------------------------------------------------------------
        /// <summary>
        /// Sets up Repeater control source/options and binds it.
        /// </summary>
        private void rptLetters_Bind()
        {

            // Declares a variable that will store a referance to the DataTable we are 
            //  going to bind the repeater control to.
            DataTable dt;

            //------------------------------------------------------------------------
            // Get the appropriate set of records to view/edit
            if (Session[this.ClientID + "_LettersData"] == null)
            {

                // Create a new data table
                dt = new DataTable();

                // Create the scheme of the table
                dt.Columns.Add(new DataColumn("Letter", typeof(string)));

                // Populate the data table with the letter data
                for (int i = 0; i < Letters.Length; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = Letters[i];
                    dt.Rows.Add(dr);
                }

                // Store a referance to the newly create data tabel in the session for 
                //  use on post back.
                Session[this.ClientID + "_LettersData"] = dt;
            }
            else
            {
                dt = (DataTable)Session[this.ClientID + "_LettersData"];
            }

            //------------------------------------------------------------------------
            // Bind the data's default view to the grid
            rptLetters.DataSource = dt.DefaultView;
            rptLetters.DataBind();

        } // private void dgLetters_Bind ()


        //--------------------------------------------------------------------------
        /// <summary>
        /// Called when an item in the letters repeater control is data bound to a 
        /// source.
        /// </summary>
        protected void rptLetters_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {

            // Retrieve the row of data that is to be bound to the repeater
            DataRowView data = (DataRowView)e.Item.DataItem;
            LinkButton lnkletter = (LinkButton)e.Item.FindControl("lnkletter");
            // If the letter we are binding to the current repeater control item is
            //  the same as the one currently selected, than disable it so the user
            //  knows which one was selected.
            if ((string)data[0] == Filter)
            {
                lnkletter.Enabled = false;
                lnkletter.CssClass = "linkButtonSelected";
                lnkletter.Attributes["onmouseover"] = String.Empty;
                lnkletter.Attributes["onmouseout"] = String.Empty;
            }
            else
            {
                lnkletter.ForeColor = ForeColor;
            }

        } // private void letters_ItemDataBound(sender, e)


        //--------------------------------------------------------------------------
        /// <summary>
        /// Called when a custom command in the Repeater control is executed.
        /// </summary>
        protected void rptLetters_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {

            // Handle the "Filter" new record command
            if (e.CommandName == "Filter")
            {

                // Set the row filter to the new filter
                Filter = (string)e.CommandArgument;

                // Rebind the data in the data grid.  This will also update the letter
                //  links and disable the currently selected letter.
                rptLetters_Bind();
                if (Click != null)
                {
                    Click(this, new LetterFilterEventArgs(Filter));
                }
            }

        } // private void letters_ItemCommand(source, e)
    }
}