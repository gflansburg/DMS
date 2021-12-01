/*
' Copyright (c) 2021  Gafware
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Reflection;
using System.Web.UI.WebControls;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Tabs;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Xml.Serialization;
using Gafware.Modules.DMS.Components;
using System.Web;
using DotNetNuke.Common;
using System.Collections;
using DotNetNuke.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Gafware.Modules.DMS
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from DMSModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class PacketList : DMSModuleBase, IActionable
    {
        private readonly INavigationManager _navigationManager;

        public PacketList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        public int PacketID
        {
            get
            {
                return (ViewState["PacketID"] != null ? (int)ViewState["PacketID"] : 0);
            }
            set
            {
                ViewState["PacketID"] = value;
                BindPacketData();
            }
        }

        public enum PayloadType
        {
            Document,
            Tag
        }

        protected string SortColumn
        {
            get
            {
                return (ViewState["SortColumn"] != null ? ViewState["SortColumn"].ToString() : "Name");
            }
            set
            {
                ViewState["SortColumn"] = value;
            }
        }

        protected SortDirection SortDirection
        {
            get
            {
                return (ViewState["SortDirection"] != null ? (SortDirection)ViewState["SortDirection"] : SortDirection.Ascending);
            }
            set
            {
                ViewState["SortDirection"] = value;
            }
        }

        [Serializable]
        public class PacketPayload : IComparable<PacketPayload>
        {
            public PayloadType PayloadType { get; set; }
            public Components.PacketDocument PacketDocument { get; set; }
            public Components.PacketTag PacketTag { get; set; }
            public int SortOrder { get; set; }

            public PacketPayload()
            {
            }

            public int CompareTo(PacketPayload other)
            {
                return SortOrder.CompareTo(other.SortOrder);
            }

            public PacketPayload(Components.Document doc, int packetId, int sortOrder)
            {
                this.PacketDocument = new Components.PacketDocument();
                this.PacketDocument.Document = doc;
                this.PacketDocument.DocumentId = doc.DocumentId;
                this.PacketDocument.PacketId = packetId;
                this.PayloadType = PayloadType.Document;
                this.SortOrder = sortOrder;
            }

            public PacketPayload(Components.Tag tag, int packetId, int sortOrder)
            {
                this.PacketTag = new Components.PacketTag();
                this.PacketTag.Tag = tag;
                this.PacketTag.TagId = tag.TagId;
                this.PacketTag.PacketId = packetId;
                this.PayloadType = PayloadType.Tag;
                this.SortOrder = sortOrder;
            }
        }

        private List<PacketPayload> SelectedDocuments
        {
            get
            {
                if (ViewState["SelectedDocuments"] == null)
                {
                    ViewState["SelectedDocuments"] = new List<PacketPayload>();
                }
                return (List<PacketPayload>)ViewState["SelectedDocuments"];
            }
            set
            {
                ViewState["SelectedDocuments"] = value;
            }
        }

        private void SaveSelectedDocuments()
        {
            foreach (GridViewRow row in gvPackets.Rows)
            {
                HiddenField subItemID = (HiddenField)row.FindControl("subItemID");
                HiddenField payloadType = (HiddenField)row.FindControl("payloadType");
                PayloadType type = (PayloadType)Enum.Parse(typeof(PayloadType), payloadType.Value);
                if (type == PayloadType.Document)
                {
                    DropDownList ddFileType = (DropDownList)row.FindControl("ddFileType");
                    if (ddFileType != null)
                    {
                        int documentID = Convert.ToInt32(subItemID.Value);
                        PacketPayload doc = SelectedDocuments.Find(p => p.PayloadType == PayloadType.Document && p.PacketDocument.DocumentId == documentID);
                        if (doc != null)
                        {
                            doc.PacketDocument.FileId = Convert.ToInt32(ddFileType.SelectedValue);
                        }
                    }
                }
            }
        }

        public string JavaScript
        {
            get
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine("function initTableDnD() {");
                sb.AppendLine("  $(\"#" + gvPackets.ClientID + "\").tableDnD({");
                sb.AppendLine("    onDrop: onDropItem");
                sb.AppendLine("  });");
                sb.AppendLine("  $('#" + tbLinkURL.ClientID + "').on('focus', function (e) {");
                sb.AppendLine("    $(this).select();");
                sb.AppendLine("  });");
                sb.AppendLine("}");
                sb.AppendLine("function onDropItem(table, row) {");
                sb.AppendLine("  var rows = jQuery(\"tr\", table);");
                sb.AppendLine("  rows.each(function () {");
                sb.AppendLine("    var row2 = jQuery(this);");
                sb.AppendLine("    if ((row2[0].rowIndex % 2) == 0) {");
                sb.AppendLine("      row2.css(\"background-color\", \"White\");");
                sb.AppendLine("    } else {");
                sb.AppendLine("      row2.css(\"background-color\", \"#F7F7F7\");");
                sb.AppendLine("    }");
                sb.AppendLine("  });");
                sb.AppendLine("  $.ajax({");
                sb.AppendLine("    type: \"POST\",");
                sb.AppendLine("    dataType: \"json\",");
                sb.AppendLine("    url: \"" + ControlPath + "DMSController.asmx/ReorderPackets\",");
                sb.AppendLine("    data: { id: $(row)[0].id, rowIndex: $(row)[0].rowIndex },");
                sb.AppendLine("    async: true,");
                sb.AppendLine("    success: function (msg) {");
                sb.AppendLine("      if(msg === 'Success') {");
                sb.AppendLine("        $(row).highlightFade({ color: 'rgb(255, 241, 168)', end: ($(row)[0].rowIndex % 2) == 0 ? 'White' : '#F7F7F7', speed: 1000, final: ($(row)[0].rowIndex % 2) == 0 ? 'White' : '#F7F7F7' });");
                sb.AppendLine("      } else {");
                sb.AppendLine("        alert(msg);");
                sb.AppendLine("      }");
                sb.AppendLine("    }");
                sb.AppendLine("  });");
                sb.AppendLine("}");
                sb.AppendLine("function previewDocument() {");
                sb.AppendLine("  var url = $('textarea#" + tbLinkURL.ClientID + "');");
                sb.AppendLine("  var win = window.open(url.val(), \"preview\", \"width=800,height=600, resize=yes,menubar=yes,status=yes, location=yes,toolbar=yes,scrollbars=yes\", true);");
                sb.AppendLine("  win.focus();");
                sb.AppendLine("}");
                sb.AppendLine("function nameKeyPress(event) {");
                sb.AppendLine("  var fileCount = $('input#" + hidFileCount.ClientID + "');");
                sb.AppendLine("  if (parseInt(fileCount.val(), 10) > 1) {");
                sb.AppendLine("    setTimeout(\"" + Page.ClientScript.GetPostBackClientHyperlink(lnkCustomHeaderPostback, String.Empty) + ";\", 10);");
                sb.AppendLine("  }");
                sb.AppendLine("  return true;");
                sb.AppendLine("}");
                sb.AppendLine("function initNameKeyDown() {");
                sb.AppendLine("  var custom = $('input#" + tbName.ClientID + "');");
                sb.AppendLine("  custom.bind('keydown', function (event) { nameKeyPress(event); });");
                sb.AppendLine("}");
                sb.AppendLine("function MM_swapImgRestore() { //v3.0");
                sb.AppendLine("    var i, x, a = document.MM_sr; for (i = 0; a && i < a.length && (x = a[i]) && x.oSrc; i++) x.src = x.oSrc;");
                sb.AppendLine("}");
                sb.AppendLine("function MM_preloadImages() { //v3.0");
                sb.AppendLine("    var d = document; if (d.images)");
                sb.AppendLine("    {");
                sb.AppendLine("        if (!d.MM_p) d.MM_p = new Array();");
                sb.AppendLine("        var i, j = d.MM_p.length, a = MM_preloadImages.arguments; for (i = 0; i < a.length; i++)");
                sb.AppendLine("            if (a[i].indexOf(\"#\") != 0) { d.MM_p[j] = new Image; d.MM_p[j++].src = a[i]; }");
                sb.AppendLine("    }");
                sb.AppendLine("}");
                sb.AppendLine("function MM_findObj(n, d) { //v4.01");
                sb.AppendLine("    var p, i, x; if (!d) d = document; if ((p = n.indexOf(\"?\")) > 0 && parent.frames.length)");
                sb.AppendLine("    {");
                sb.AppendLine("        d = parent.frames[n.substring(p + 1)].document; n = n.substring(0, p);");
                sb.AppendLine("    }");
                sb.AppendLine("    if (!(x = d[n]) && d.all) x = d.all[n]; for (i = 0; !x && i < d.forms.length; i++) x = d.forms[i][n];");
                sb.AppendLine("    for (i = 0; !x && d.layers && i < d.layers.length; i++) x = MM_findObj(n, d.layers[i].document);");
                sb.AppendLine("    if (!x && d.getElementById) x = d.getElementById(n); return x;");
                sb.AppendLine("}");
                sb.AppendLine("function MM_swapImage() { //v3.0");
                sb.AppendLine("    var i, j = 0, x, a = MM_swapImage.arguments; document.MM_sr = new Array; for (i = 0; i < (a.length - 2); i += 3)");
                sb.AppendLine("        if ((x = MM_findObj(a[i])) != null) { document.MM_sr[j++] = x; if (!x.oSrc) x.oSrc = x.src; x.src = a[i + 2]; }");
                sb.AppendLine("}");
                sb.AppendLine("MM_preloadImages('" + ResolveUrl("~/desktopmodules/Gafware/DMS/images/Icons/DeleteIcon2_16px.gif") + "');");
                sb.AppendLine("jQuery(document).ready(function () {");
                sb.AppendLine("  initNameKeyDown();");
                sb.AppendLine("  initTableDnD();");
                sb.AppendLine("  Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {");
                sb.AppendLine("    initNameKeyDown();");
                sb.AppendLine("    initTableDnD();");
                sb.AppendLine("  });");
                sb.AppendLine("});");
                return sb.ToString();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            tbDescription.Attributes.Add("onfocus", "this.rows=8;");
            tbDescription.Attributes.Add("onblur", "this.rows=1;");
            tbAdminComments.Attributes.Add("onfocus", "this.rows=8;");
            tbAdminComments.Attributes.Add("onblur", "this.rows=1;");
            if (IsPostBack)
            {
                SaveSelectedDocuments();
            }
            btnEditName.OnClientClick = (tbName.Enabled ? "if(confirm('Changing the packet name will break exising links to this packet.  Are you sure you wish to rename the packet?')) { document.getElementById('" + hidCancelRename.ClientID + "').value = 'false'; } else { document.getElementById('" + hidCancelRename.ClientID + "').value = 'true'; } return true;" : String.Empty);
            System.Web.UI.HtmlControls.HtmlGenericControl script2 = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("ComponentScriptBlockUI");
            if (script2 == null)
            {
                script2 = new System.Web.UI.HtmlControls.HtmlGenericControl("script")
                {
                    ID = "ComponentScriptBlockUI"
                };
                script2.Attributes.Add("language", "javascript");
                script2.Attributes.Add("type", "text/javascript");
                script2.Attributes.Add("src", ControlPath + "Scripts/jquery.blockUI.js");
                this.Page.Header.Controls.Add(script2);
            }
            System.Web.UI.HtmlControls.HtmlGenericControl script = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("TableDnDScript");
            if (script == null)
            {
                script = new System.Web.UI.HtmlControls.HtmlGenericControl("script")
                {
                    ID = "TableDnDScript"
                };
                script.Attributes.Add("language", "javascript");
                script.Attributes.Add("type", "text/javascript");
                script.Attributes.Add("src", ControlPath + "Scripts/jquery.tablednd_0_5.js");
                this.Page.Header.Controls.Add(script);
            }
            System.Web.UI.HtmlControls.HtmlGenericControl script3 = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("HighlightFadeScript");
            if (script3 == null)
            {
                script3 = new System.Web.UI.HtmlControls.HtmlGenericControl("script")
                {
                    ID = "HighlightFadeScript"
                };
                script3.Attributes.Add("language", "javascript");
                script3.Attributes.Add("type", "text/javascript");
                script3.Attributes.Add("src", ControlPath + "Scripts/jquery.highlightFade.js");
                this.Page.Header.Controls.Add(script3);
            }
            System.Web.UI.HtmlControls.HtmlGenericControl literal = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("ComponentScriptDMS");
            if (literal == null)
            {
                literal = new System.Web.UI.HtmlControls.HtmlGenericControl("script")
                {
                    ID = "ComponentScriptDMS"
                };
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine("var isBlockingScreen = true;");
                sb.AppendLine("function showBlockingScreen(message) {");
                sb.AppendLine("  if (!isBlockingScreen) {");
                sb.AppendLine("    isBlockingScreen = true;");
                sb.AppendLine("    if (message) {");
                sb.AppendLine("      $.blockUI({ message: '<h3 class=\"uiMessage\"><img src=\"" + ControlPath + "/Images/loading.gif\" /> ' + message + '...</h2>' });");
                sb.AppendLine("    } else {");
                sb.AppendLine("      $('.se-pre-con').show();");
                sb.AppendLine("    }");
                sb.AppendLine("  }");
                sb.AppendLine("}");
                sb.AppendLine("function hideBlockingScreen() {");
                sb.AppendLine("  //if (isBlockingScreen) {");
                sb.AppendLine("    // Animate loader off screen");
                sb.AppendLine("    isBlockingScreen = false;");
                sb.AppendLine("    $('.se-pre-con').fadeOut('fast');");
                sb.AppendLine("    $.unblockUI();");
                sb.AppendLine("  //}");
                sb.AppendLine("}");
                sb.AppendLine("$(window).ready(function() {");
                sb.AppendLine("  hideBlockingScreen();");
                sb.AppendLine("});");
                sb.AppendLine("$(window).load(function() {");
                sb.AppendLine("  //hideBlockingScreen();");
                sb.AppendLine("});");
                sb.AppendLine("$(window).unload(function() {");
                sb.AppendLine("  //showBlockingScreen();");
                sb.AppendLine("});");
                sb.AppendLine("var ignore_onbeforeunload = false;");
                sb.AppendLine("window.addEventListener('beforeunload', function(event) {");
                sb.AppendLine("  if (!ignore_onbeforeunload) {");
                sb.AppendLine("    showBlockingScreen();");
                sb.AppendLine("  }");
                sb.AppendLine("  ignore_onbeforeunload = false;");
                sb.AppendLine("});");
                sb.AppendLine("jQuery(document).ready(function() {");
                sb.AppendLine("  var prm = Sys.WebForms.PageRequestManager.getInstance();");
                sb.AppendLine("  prm.add_endRequest(MyEndRequest);");
                sb.AppendLine("  initPacketListJavascript();");
                sb.AppendLine("});");
                sb.AppendLine("function MyEndRequest(sender, args) {");
                sb.AppendLine("  initPacketListJavascript();");
                sb.AppendLine("  hideBlockingScreen();");
                sb.AppendLine("}");
                sb.AppendLine("function initPacketListJavascript() {");
                sb.AppendLine("  $('#" + tbLinkURL.ClientID + "').on('focus', function (e) {");
                sb.AppendLine("    $(this).select();");
                sb.AppendLine("  });");
                sb.AppendLine("  $('a[href^=mailto]').on('click', function() {");
                sb.AppendLine("    ignore_onbeforeunload = true;");
                sb.AppendLine("  });");
                sb.AppendLine("  window.setTimeout(\"window.scrollTo(0, 0)\", 100);");
                sb.AppendLine("  $(\"#" + btnCancelChange.ClientID + "\").click(function(e) {");
                sb.AppendLine("    e.preventDefault();");
                sb.AppendLine("    $('#changeOwnershipDialog').dialog('close');");
                sb.AppendLine("  });");
                sb.AppendLine("  $(\"#" + changeOwnershipCommandButton.ClientID + "\").click(function(e) {");
                sb.AppendLine("    e.preventDefault();");
                sb.AppendLine("    $('#" + ddCurrentOwner.ClientID + "').prop('selectedIndex', 0);");
                sb.AppendLine("    $('#" + ddNewOwner.ClientID + "').prop('selectedIndex', 0);");
                sb.AppendLine("    $('#changeOwnershipDialog .FormInstructions').hide();");
                sb.AppendLine("    $('#changeOwnershipDialog').dialog('open');");
                sb.AppendLine("    $('#changeOwnership-content').scrollTop(0);");
                sb.AppendLine("  });");
                sb.AppendLine("  var form = $('#changeOwnershipDialog').dialog({");
                sb.AppendLine("    autoOpen: false,");
                sb.AppendLine("    bgiframe: true,");
                sb.AppendLine("    modal: true,");
                sb.AppendLine("    width: 600,");
                sb.AppendLine("    height: 300,");
                sb.AppendLine("    appendTo: 'form',");
                sb.AppendLine("    dialogClass: 'dialog',");
                sb.AppendLine("    title: 'Change Packet Ownership',");
                sb.AppendLine("    closeOnEsacpe: true,");
                sb.AppendLine("    Cancel: function () {");
                sb.AppendLine("      $(this).dialog('close');");
                sb.AppendLine("    }");
                sb.AppendLine("  });");
                sb.AppendLine("  $('#previewDialog').dialog({");
                sb.AppendLine("    autoOpen: false,");
                sb.AppendLine("    bgiframe: true,");
                sb.AppendLine("    modal: true,");
                sb.AppendLine("    width: 600,");
                sb.AppendLine("    height: 700,");
                sb.AppendLine("    resizable: false,");
                sb.AppendLine("    appendTo: '.dms',");
                sb.AppendLine("    dialogClass: 'dialog',");
                sb.AppendLine("    title: 'Preview Documents',");
                sb.AppendLine("    closeOnEsacpe: true,");
                sb.AppendLine("    Cancel: function () {");
                sb.AppendLine("      $(this).dialog('close');");
                sb.AppendLine("    }");
                sb.AppendLine("  });");
                sb.AppendLine("  var preview = Number($('#" + preview.ClientID + "').val());");
                sb.AppendLine("  if(preview)");
                sb.AppendLine("    $('#previewDialog').dialog('open');");
                sb.AppendLine("}");
                sb.Append(JavaScript);
                literal.InnerHtml = sb.ToString();
                this.Page.Header.Controls.Add(literal);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsAdmin())
                {
                    base.Response.Redirect(_navigationManager.NavigateURL(), true);
                }
                letterFilter.ForeColor = gvPackets.HeaderStyle.BackColor = gv.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#" + Theme);
                documentSearchResults.NavigationManager = _navigationManager;
                documentSearchResults.CategoryName = CategoryName;
                documentSearchResults.Theme = Theme;
                documentSearchResults.PageSize = PageSize;
                documentSearchResults.ThumbnailSize = ThumbnailSize;
                documentSearchResults.ThumbnailType = ThumbnailType;
                documentSearchResults.UseLocalFile = SaveLocalFile;
                documentSearchResults.PortalId = PortalId;
                documentSearchResults.PortalWideRepository = PortalWideRepository;
                documentSearchResults.UserId = UserId;
                documentSearchResults.TabModuleId = TabModuleId;
                documentSearchResults.ModuleId = ModuleId;
                documentSearchResults.IsAdmin = IsAdmin();
                documentSearchResults.ControlPath = ControlPath;
                documentSearchResults.IsLink = false;
                documentSearchResults.Search = true;
                preview.Value = "0";
                if (!IsPostBack)
                {
                    gv.PageSize = PageSize;
                    litCSS.Text = "<style type=\"text/css\">" + Generic.ToggleButtonCssString("No", "Yes", new Unit("100px"), System.Drawing.ColorTranslator.FromHtml("#" + Theme)) + "</style>";
                    BindDropDowns();
                    pnlDetails.Style.Add("display", "none");
                    pnlGrid.Style.Remove("display");
                    pnlOwner.Visible = pnlAdmin.Visible = IsAdmin();
                    changeOwnershipCommandButton.Visible = IsAdmin();
                    BindData();
                    if (!String.IsNullOrEmpty(Request.QueryString["id"]) && Generic.IsNumber(Request.QueryString["id"]))
                    {
                        if (Session["gv"] != null)
                        {
                            Session.Remove("gv");
                        }
                        EditPacket(Convert.ToInt32(Request.QueryString["id"]));
                    }
                    else
                    {
                        CreateDataTable(true);
                    }
                }
                else
                {
                    Generic.ApplyPaging(gv);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                return new ModuleActionCollection();
            }
        }

        private void BindData()
        {
            ddOwner.DataSource = Components.UserController.GetUsers(UserRole, PortalId);
            ddOwner.DataBind();
            ddOwner.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All", "0"));
            if (IsAdmin())
            {
                ddOwner.SelectedIndex = 0;
            }
            else
            {
                ddOwner.SelectedIndex = ddOwner.Items.IndexOf(ddOwner.Items.FindByValue(UserId.ToString()));
            }
            ddCurrentOwner.DataSource = Components.UserController.GetUsers(UserRole, PortalId);
            ddCurrentOwner.DataBind();
            ddCurrentOwner.Items.Insert(0, new ListItem("-- Current Owner --", "0"));
            ddCurrentOwner.SelectedIndex = 0;

            ddNewOwner.DataSource = Components.UserController.GetUsers(UserRole, PortalId);
            ddNewOwner.DataBind();
            ddNewOwner.Items.Insert(0, new ListItem("-- New Owner --", "0"));
            ddNewOwner.SelectedIndex = 0;
        }

        public System.Data.DataView CreateDataTable(bool bReload)
        {
            System.Data.DataView dataView = (System.Data.DataView)Session["gv"];
            if (dataView == null || bReload)
            {
                List<Packet> packets = PacketController.GetAllPacketsForUser(UserId, PortalId, PortalWideRepository ? 0 : TabModuleId);
                if (Convert.ToInt32(ddOwner.SelectedValue) > 0)
                {
                    packets = packets.FindAll(p => p.CreatedByUserID == Convert.ToInt32(ddOwner.SelectedValue));
                }
                System.Data.DataTable dtResult = Generic.ListToDataTable(packets);
                dataView = new System.Data.DataView(dtResult);
                gv.PageIndex = 0;
            }
            dataView.Sort = SortColumn + " " + (SortDirection == SortDirection.Ascending ? "ASC" : "DESC");
            AddFilter(dataView);
            return dataView;
        }

        private void AddFilter(System.Data.DataView dataView)
        {
            if (!letterFilter.Filter.Equals("All"))
            {
                if (letterFilter.Filter.Equals("#"))
                {
                    dataView.RowFilter = "Name LIKE '1%' OR Name LIKE '2%' OR Name LIKE '3%' OR Name LIKE '4%' OR Name LIKE '5%' OR Name LIKE '6%' OR Name LIKE '7%' OR Name LIKE '8%' OR Name LIKE '9%' OR Name LIKE '0%'";
                }
                else
                {
                    dataView.RowFilter = "Name LIKE '" + letterFilter.Filter + "%'";
                }
                gv.EmptyDataText = "<br /><strong>No packets found for '" + letterFilter.Filter + "'.</strong>";
            }
            else
            {
                dataView.RowFilter = String.Empty;
                gv.EmptyDataText = "<br /><strong>No packets found.</strong>";
            }
            gv.DataSource = dataView;
            gv.DataBind();
            Session["gv"] = dataView;
        }

        protected void gv_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv.PageIndex = e.NewPageIndex;
            CreateDataTable(false);
        }

        protected void gv_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (e.SortExpression != SortColumn)
            {
                SortColumn = e.SortExpression;
                SortDirection = System.Web.UI.WebControls.SortDirection.Ascending;
            }
            else
            {
                if (SortDirection == SortDirection.Ascending)
                {
                    SortDirection = SortDirection.Descending;
                }
                else
                {
                    SortDirection = SortDirection.Ascending;
                }
            }
            System.Data.DataView dataView = (System.Data.DataView)Session["gv"];
            dataView.Sort = SortColumn + " " + (SortDirection == SortDirection.Ascending ? "ASC" : "DESC");
            gv.PageIndex = 0;
            gv.DataSource = dataView;
            gv.DataBind();
            Session["gv"] = dataView;
        }

        protected void letterFilter_Click(object sender, LetterFilter.LetterFilterEventArgs e)
        {
            AddFilter((System.Data.DataView)Session["gv"]);
        }

        private void EditPacket(int packetId)
        {
            PacketID = packetId;
            pnlDetails.Style.Remove("display");
            pnlGrid.Style.Add("display", "none");
        }

        protected void gv_DataBound(object sender, EventArgs e)
        {
            Generic.ApplyPaging(gv);
        }

        protected void gv_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int packetID = Convert.ToInt32(gv.DataKeys[e.RowIndex].Value.ToString());
            PacketController.DeletePacket(packetID);
            CreateDataTable(true);
            e.Cancel = true;
        }

        protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
        {
            int packetId = Convert.ToInt32(gv.DataKeys[e.NewEditIndex].Value.ToString());
            EditPacket(packetId);
            e.Cancel = true;
        }

        protected string GetUrl(object item)
        {
            if (item.GetType() == typeof(System.Data.DataRowView))
            {
                System.Data.DataRowView row = item as System.Data.DataRowView;
                Packet packet = PacketController.GetPacket((int)row["PacketID"]);
                return String.Format("{0}/type/documents/p/{1}", _navigationManager.NavigateURL(), Generic.UrlEncode(packet.Name));
            }
            return String.Empty;
        }

        protected string GetComments(string comments)
        {
            if (!String.IsNullOrEmpty(comments))
            {
                float width = Generic.GetWidthOfString(comments, gv.RowStyle.Font);
#if DEBUG
                if (width > 400)
#else
                if (width > 510)
#endif
                {
                    string temp = comments;
#if DEBUG
                    while ((width = Generic.GetWidthOfString(temp + "...", gv.RowStyle.Font)) > 400)
#else
                    while ((width = Generic.GetWidthOfString(temp + "...", gv.RowStyle.Font)) > 510)
#endif
                    {
                        temp = temp.Substring(0, temp.Length - 1);
                    }
                    return temp + "...";
                }
                return comments;
            }
            return String.Empty;
        }

        public bool IsDMSUser()
        {
            if ((new ModuleSecurity((new ModuleController()).GetTabModule(this.TabModuleId))).HasEditPermissions)
            {
                return true;
            }
            if (UserInfo != null)
            {
                DotNetNuke.Security.Roles.RoleInfo userRole = DotNetNuke.Security.Roles.RoleController.Instance.GetRoleById(PortalId, UserRole);
                if (userRole != null)
                {
                    return UserInfo.IsInRole(userRole.RoleName);
                }
            }
            return false;
        }

        public bool IsAdmin()
        {
            if ((new ModuleSecurity((new ModuleController()).GetTabModule(this.TabModuleId))).HasEditPermissions)
            {
                return true;
            }
            return false;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            base.Response.Redirect(_navigationManager.NavigateURL(), true);
        }

        protected void backCommandButton_Click(object sender, EventArgs e)
        {
            base.Response.Redirect(_navigationManager.NavigateURL(), true);
        }

        protected void newPacketCommandButton_Click(object sender, EventArgs e)
        {
            PacketID = 0;
            pnlDetails.Style.Remove("display");
            pnlGrid.Style.Add("display", "none");
        }

        protected void ddOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateDataTable(true);
        }

        protected void btnSaveChange_Click(object sender, EventArgs e)
        {
            PacketController.ChangeOwnership(Convert.ToInt32(ddCurrentOwner.SelectedValue), Convert.ToInt32(ddNewOwner.SelectedValue), PortalId);
        }

        protected void lnkPreview_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName.Equals("Preview", StringComparison.OrdinalIgnoreCase))
            {
                int packetId = Convert.ToInt32(e.CommandArgument);
                Packet packet = PacketController.GetPacket(packetId);
                if (packet != null)
                {
                    documentSearchResults.QueryString = string.Concat("p=", Generic.UrlEncode(packet.Name));
                    preview.Value = "1";
                }
            }
        }

        private void BindPacketData()
        {
            Components.Packet packet = Components.PacketController.GetPacket(PacketID);
            if (PacketID == 0 || packet != null)
            {
                if (packet == null)
                {
                    packet = new Components.Packet();
                    packet.ShowDescription = true;
                    packet.PortalId = PortalId;
                    packet.CreatedByUserID = UserId;
                    packet.AdminComments = String.Empty;
                    packet.CustomHeader = String.Empty;
                    packet.Description = String.Empty;
                    packet.Documents = new List<Components.PacketDocument>();
                    packet.Name = Generic.GetRandomKeyNoDuplication(PortalId);
                    packet.Tags = new List<Components.PacketTag>();
                }
                tbAdminComments.Text = packet.AdminComments.Trim();
                tbCustomHeader.Text = packet.CustomHeader.Trim();
                tbDescription.Text = packet.Description.Trim();
                tbName.Text = packet.Name.Trim();
                if (String.IsNullOrWhiteSpace(packet.Name))
                {
                    tbName.Text = Generic.GetRandomKeyNoDuplication(PortalId);
                }
                tbName.Enabled = (packet.PacketId == 0);
                btnEditName.Visible = (packet.PacketId > 0);
                //rblShowDescription.SelectedIndex = (packet.ShowDescription ? 0 : 1);
                cbShowDescription.Checked = packet.ShowDescription;
                cbShowPacketDescription.Checked = packet.ShowPacketDescription;
                SelectedDocuments.Clear();
                foreach (Components.PacketDocument doc in packet.Documents)
                {
                    PacketPayload payload = new PacketPayload();
                    payload.PayloadType = PayloadType.Document;
                    payload.PacketDocument = doc;
                    payload.SortOrder = doc.SortOrder;
                    SelectedDocuments.Add(payload);
                }
                foreach (Components.PacketTag tag in packet.Tags)
                {
                    PacketPayload payload = new PacketPayload();
                    payload.PayloadType = PayloadType.Tag;
                    payload.PacketTag = tag;
                    payload.SortOrder = tag.SortOrder;
                    SelectedDocuments.Add(payload);
                }
                SelectedDocuments = SelectedDocuments.OrderBy(o => o.SortOrder).ToList();
                gvPackets.DataSource = SelectedDocuments;
                gvPackets.DataBind();
                SetLinkUrl();
                pnlFound.Visible = true;
                pnlNotFound.Visible = false;
                btnSave.Visible = true;
                btnCancel.Text = "Cancel";
                tbName.BackColor = (tbName.Enabled ? System.Drawing.Color.White : System.Drawing.Color.Silver);
                tbName.Width = new Unit(packet.PacketId > 0 ? "686px" : "780px");
                btnEditName.Text = (tbName.Enabled ? "Save Name" : "Edit Name");
                ddDocuments.SelectedIndex = 0;
                ddTags.SelectedIndex = 0;
            }
            else
            {
                pnlNotFound.Visible = true;
                pnlFound.Visible = false;
                btnSave.Visible = false;
                btnCancel.Text = "Back";
            }
        }

        public class SearchResult
        {
            public int DocumentID { get; set; }
            public string DocumentName { get; set; }
        }

        private void BindDropDowns()
        {
            List<Components.DocumentView> docs = Components.DocumentController.GetAllDocumentsForDropDown(PortalId, PortalWideRepository ? 0 : TabModuleId);
            List<SearchResult> results = (from doc in docs select new SearchResult { DocumentID = doc.DocumentId, DocumentName = doc.DocumentName }).ToList();
            ddDocuments.DataSource = results;
            ddDocuments.DataBind();
            ddDocuments.Items.Insert(0, new ListItem("-- Select A Document --", "0"));
            ddDocuments.SelectedIndex = 0;
            List<Components.Tag> tags = Components.DocumentController.GetAllTags(PortalId, PortalWideRepository ? 0 : TabModuleId);
            ddTags.DataSource = tags;
            ddTags.DataBind();
            ddTags.Items.Insert(0, new ListItem("-- Select A Tag --", "0"));
            ddTags.SelectedIndex = 0;
        }

        public int GetDocCount()
        {
            return SelectedDocuments.Count;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Components.Document doc = Components.DocumentController.GetDocument(Convert.ToInt32(ddDocuments.SelectedValue));
            //Components.Document doc = Components.DocumentController.GetDocument(Convert.ToInt32(hidDocumentId.Value));
            if (doc.DocumentId > 0 && SelectedDocuments.Find(p => p.PayloadType == PayloadType.Document && p.PacketDocument.DocumentId == doc.DocumentId) == null)
            {
                SelectedDocuments.Add(new PacketPayload(doc, PacketID, SelectedDocuments.Count + 1));
                gvPackets.DataSource = SelectedDocuments;
                gvPackets.DataBind();
                SetLinkUrl();
            }
            ddDocuments.SelectedIndex = 0;
            //hidDocumentId.Value = "0";
            //tbDocument.Text = string.Empty;
        }

        protected void gvPackets_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridViewRow row = gvPackets.Rows[e.RowIndex];
            HiddenField subItemID = (HiddenField)row.FindControl("subItemID");
            HiddenField payloadType = (HiddenField)row.FindControl("payloadType");
            PayloadType type = (PayloadType)Enum.Parse(typeof(PayloadType), payloadType.Value);
            if (type == PayloadType.Document)
            {

                int documentID = Convert.ToInt32(subItemID.Value);
                SelectedDocuments.Remove(SelectedDocuments.Find(p => p.PayloadType == PayloadType.Document && p.PacketDocument.DocumentId == documentID));
            }
            else
            {
                int tagID = Convert.ToInt32(subItemID.Value);
                SelectedDocuments.Remove(SelectedDocuments.Find(p => p.PayloadType == PayloadType.Tag && p.PacketTag.TagId == tagID));
            }
            gvPackets.DataSource = SelectedDocuments;
            gvPackets.DataBind();
            SetLinkUrl();
        }

        private Components.DMSFile GetFirstActiveFile
        {
            get
            {
                foreach (GridViewRow row in gvPackets.Rows)
                {
                    HiddenField subItemID = (HiddenField)row.FindControl("subItemID");
                    HiddenField payloadType = (HiddenField)row.FindControl("payloadType");
                    PayloadType type = (PayloadType)Enum.Parse(typeof(PayloadType), payloadType.Value);
                    if (type == PayloadType.Document)
                    {
                        Components.Document doc = Components.DocumentController.GetDocument(Convert.ToInt32(subItemID.Value));
                        if ((!doc.ActivationDate.HasValue || DateTime.Now >= doc.ActivationDate) && (!doc.ExpirationDate.HasValue || DateTime.Now <= doc.ExpirationDate) && doc.Files.FindAll(p => p.Status.StatusId == 1).Count > 0)
                        {
                            DropDownList ddFileType = (DropDownList)row.FindControl("ddFileType");
                            if (ddFileType != null)
                            {
                                if (ddFileType.SelectedIndex == 0)
                                {
                                    return doc.Files.Find(p => p.Status.StatusId == 1);
                                }
                                else
                                {
                                    return doc.Files.Find(p => p.FileId == Convert.ToInt32(ddFileType.SelectedValue));
                                }
                            }
                        }
                    }
                }
                return null;
            }
        }

        protected bool FilesOnSingleRow
        {
            get
            {
                int fileCount = GetFileCount;
                if (fileCount > 0)
                {
                    List<PacketPayload> items = SelectedDocuments.FindAll(p => p.PayloadType == PayloadType.Document && ((!p.PacketDocument.Document.ActivationDate.HasValue || DateTime.Now > p.PacketDocument.Document.ActivationDate.Value) && (!p.PacketDocument.Document.ExpirationDate.HasValue || DateTime.Now <= p.PacketDocument.Document.ExpirationDate.Value)));
                    foreach (PacketPayload payload in items)
                    {
                        int rowCount = payload.PacketDocument.Document.Files.FindAll(p => p.Status.StatusId == 1).Count;
                        if (rowCount == fileCount)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private void SetLinkUrl()
        {
            SaveSelectedDocuments();
            //btnPreview.Enabled = (PacketID > 0);
            pnlLink.Visible = (SelectedDocuments.Count > 0);
            string[] strArrays = new string[3];
            int moduleId = ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            strArrays[1] = string.Concat("type=", "documents");
            strArrays[2] = string.Concat("p=", Generic.UrlEncode(tbName.Text));
            tbLinkURL.Text = _navigationManager.NavigateURL("GetDocuments", strArrays);
            hidBaseUrl.Value = GetUrl();
            hidFileCount.Value = GetActualFileCount.ToString();
        }

        protected string GetUrl()
        {
            string[] strArrays = new string[3];
            int moduleId = ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            strArrays[1] = string.Concat("type=", "documents");
            strArrays[2] = string.Concat("p=", String.Empty);
            return _navigationManager.NavigateURL("GetDocuments", strArrays);
        }

        protected int GetFileCount
        {
            get
            {
                int count = 0;
                List<PacketPayload> items = SelectedDocuments.FindAll(p => p.PayloadType == PayloadType.Document && ((!p.PacketDocument.Document.ActivationDate.HasValue || DateTime.Now > p.PacketDocument.Document.ActivationDate.Value) && (!p.PacketDocument.Document.ExpirationDate.HasValue || DateTime.Now <= p.PacketDocument.Document.ExpirationDate.Value)));
                foreach (PacketPayload payload in items)
                {
                    count += payload.PacketDocument.Document.Files.FindAll(p => p.Status.StatusId == 1).Count;
                }
                return count;
            }
        }

        protected int GetActualFileCount
        {
            get
            {
                int count = 0;
                List<PacketPayload> items = SelectedDocuments.FindAll(p => p.PayloadType == PayloadType.Document && ((!p.PacketDocument.Document.ActivationDate.HasValue || DateTime.Now > p.PacketDocument.Document.ActivationDate.Value) && (!p.PacketDocument.Document.ExpirationDate.HasValue || DateTime.Now <= p.PacketDocument.Document.ExpirationDate.Value)));
                foreach (PacketPayload payload in items)
                {
                    count += payload.PacketDocument.Document.Files.FindAll(p => p.Status.StatusId == 1 && (payload.PacketDocument.FileId == 0 || p.FileId == payload.PacketDocument.FileId)).Count;
                }
                return count;
            }
        }

        protected void gvPackets_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                PacketPayload payload = (PacketPayload)e.Row.DataItem;
                ImageButton deleteButton = (ImageButton)e.Row.FindControl("deleteButton");
                if (deleteButton != null)
                {
                    deleteButton.Attributes.Add("onMouseOver", "MM_swapImage('" + deleteButton.ClientID + "','','" + ResolveUrl(ControlPath + "Images/Icons/DeleteIcon2_16px.gif") + "',1)");
                }
                DropDownList ddFileType = (DropDownList)e.Row.FindControl("ddFileType");
                Label lblType = (Label)e.Row.FindControl("lblType");
                if (payload.PayloadType == PayloadType.Document)
                {
                    List<Components.DMSFile> files = payload.PacketDocument.Document.Files.FindAll(p => p.Status.StatusId == 1);
                    ddFileType.DataSource = files;
                    ddFileType.DataBind();
                    ddFileType.Items.Insert(0, new ListItem("All", "0"));
                    ddFileType.SelectedIndex = ddFileType.Items.IndexOf(ddFileType.Items.FindByValue(payload.PacketDocument.FileId.ToString()));
                    ddFileType.Visible = (files.Count > 1 && SelectedDocuments.Count == 1);
                    lblType.Visible = (ddFileType.Visible ? false : true);
                }
                lblType.Text = payload.PayloadType.ToString();
                e.Row.Attributes["id"] = (payload.PayloadType == PayloadType.Document ? payload.PacketDocument.PacketDocId : payload.PacketTag.PacketTagId * -1).ToString();
            }
        }

        protected void rblShowDescription_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetLinkUrl();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            BindPacketData();
        }

        protected void lnkCustomHeaderPostback_Click(object sender, EventArgs e)
        {
            SetLinkUrl();
            //RadAjaxManager.ResponseScripts.Add("setCaretAtEnd(document.getElementById('" + tbName.ClientID + "'))");
        }

        protected void ddFileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetLinkUrl();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlDetails.Style.Add("display", "none");
            pnlGrid.Style.Remove("display");
            PacketID = 0;
            CreateDataTable(false);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveSelectedDocuments();
            Components.Packet packet = Components.PacketController.GetPacket(PacketID);
            if (packet == null)
            {
                packet = new Components.Packet();
                packet.ShowDescription = true;
                packet.PortalId = PortalId;
                packet.TabModuleId = TabModuleId;
                packet.CreatedByUserID = UserId;
                packet.Documents = new List<Components.PacketDocument>();
                packet.Tags = new List<Components.PacketTag>();
            }
            packet.AdminComments = tbAdminComments.Text.Trim();
            packet.CustomHeader = tbCustomHeader.Text.Trim();
            packet.Description = tbDescription.Text.Trim();
            packet.Name = tbName.Text.Trim();
            //packet.ShowDescription = (rblShowDescription.SelectedIndex == 0);
            packet.ShowDescription = cbShowDescription.Checked;
            packet.ShowPacketDescription = cbShowPacketDescription.Checked;
            packet.Documents = new List<Components.PacketDocument>();
            packet.Tags = new List<Components.PacketTag>();
            int index = 0;
            foreach (PacketPayload payload in SelectedDocuments)
            {
                if (payload.PayloadType == PayloadType.Document)
                {
                    payload.PacketDocument.SortOrder = index;
                    packet.Documents.Add(payload.PacketDocument);
                }
                else if (payload.PayloadType == PayloadType.Tag)
                {
                    payload.PacketTag.SortOrder = index;
                    packet.Tags.Add(payload.PacketTag);
                }
                index++;
            }
            if (packet.CreatedByUserID == 0)
            {
                packet.CreatedByUserID = UserId;
            }
            Components.PacketController.SavePacket(packet);
            pnlDetails.Style.Add("display", "none");
            pnlGrid.Style.Remove("display");
            PacketID = 0;
            int page = gv.PageIndex;
            CreateDataTable(false);
            gv.PageIndex = page;
            CreateDataTable(true);
        }

        protected void btnEditName_Click(object sender, EventArgs e)
        {
            if (tbName.Enabled)
            {
                Components.Packet packet = Components.PacketController.GetPacket(PacketID);
                if (!Generic.ToBoolean(hidCancelRename.Value))
                {
                    if (!packet.Name.Equals(tbName.Text.Trim()))
                    {
                        Components.PacketController.SavePacket(packet);
                        CreateDataTable(true);
                    }
                }
                else
                {
                    tbName.Text = packet.Name.Trim();
                    SetLinkUrl();
                }
            }
            btnEditName.Text = (!tbName.Enabled ? "Save Name" : "Edit Name");
            tbName.Enabled = !tbName.Enabled;
            tbName.BackColor = (tbName.Enabled ? System.Drawing.Color.White : System.Drawing.Color.Silver);
        }

        protected void btnSubmit2_Click(object sender, EventArgs e)
        {
            Components.Tag tag = Components.DocumentController.GetTag(Convert.ToInt32(ddTags.SelectedValue));
            if (tag.TagId > 0 && SelectedDocuments.Find(p => p.PayloadType == PayloadType.Tag && p.PacketTag.TagId == tag.TagId) == null)
            {
                SelectedDocuments.Add(new PacketPayload(tag, PacketID, SelectedDocuments.Count + 1));
                gvPackets.DataSource = SelectedDocuments;
                gvPackets.DataBind();
                SetLinkUrl();
            }
            ddDocuments.SelectedIndex = 0;
            //hidDocumentId.Value = "0";
            //tbDocument.Text = string.Empty;
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            LinkButton lnk = (LinkButton)e.Row.FindControl("editButton");
            if (lnk != null)
            {
                lnk.ForeColor = System.Drawing.ColorTranslator.FromHtml("#" + Theme);
            }
            lnk = (LinkButton)e.Row.FindControl("deleteButton");
            if (lnk != null)
            {
                lnk.ForeColor = System.Drawing.ColorTranslator.FromHtml("#" + Theme);
            }
            lnk = (LinkButton)e.Row.FindControl("lnkPreview");
            if (lnk != null)
            {
                lnk.ForeColor = System.Drawing.ColorTranslator.FromHtml("#" + Theme);
            }
        }
    }
}