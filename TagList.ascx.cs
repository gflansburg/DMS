/*
' Copyright (c) 2021 Gafware
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
using System.Data;
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
    public partial class TagList : DMSModuleBase, IActionable
    {
        private readonly INavigationManager _navigationManager;

        public TagList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        public int TagID
        {
            get
            {
                return (ViewState["TagID"] != null ? (int)ViewState["TagID"] : 0);
            }
            set
            {
                ViewState["TagID"] = value;
            }
        }

        protected string DocumentsSortColumn
        {
            get
            {
                return (ViewState["DocumentsSortColumn"] != null ? ViewState["DocumentsSortColumn"].ToString() : "DocumentName");
            }
            set
            {
                ViewState["DocumentsSortColumn"] = value;
            }
        }

        protected SortDirection DocumentsSortDirection
        {
            get
            {
                return (ViewState["DocumentsSortDirection"] != null ? (SortDirection)ViewState["DocumentsSortDirection"] : SortDirection.Ascending);
            }
            set
            {
                ViewState["DocumentsSortDirection"] = value;
            }
        }

        protected string SortColumn
        {
            get
            {
                return (ViewState["SortColumn"] != null ? ViewState["SortColumn"].ToString() : "TagName");
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

        protected void Page_PreRender(object sender, EventArgs e)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl scriptConfirm = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("JQueryConfirmScriptJS");
            if (scriptConfirm == null)
            {
                scriptConfirm = new System.Web.UI.HtmlControls.HtmlGenericControl("script")
                {
                    ID = "JQueryConfirmScriptJS"
                };
                scriptConfirm.Attributes.Add("language", "javascript");
                scriptConfirm.Attributes.Add("type", "text/javascript");
                scriptConfirm.Attributes.Add("src", ControlPath + "Scripts/jquery-confirm.js");
                this.Page.Header.Controls.Add(scriptConfirm);
            }
            System.Web.UI.HtmlControls.HtmlGenericControl cssConfirm = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("JQueryConfirmScriptCSS");
            if (cssConfirm == null)
            {
                cssConfirm = new System.Web.UI.HtmlControls.HtmlGenericControl("link")
                {
                    ID = "JQueryConfirmScriptCSS"
                };
                cssConfirm.Attributes.Add("type", "text/css");
                cssConfirm.Attributes.Add("rel", "stylesheet");
                cssConfirm.Attributes.Add("href", ControlPath + "Scripts/jquery-confirm.css");
                this.Page.Header.Controls.Add(cssConfirm);
            }
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
            System.Web.UI.HtmlControls.HtmlGenericControl literal = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("ComponentScriptDMS");
            if (literal == null)
            {
                literal = new System.Web.UI.HtmlControls.HtmlGenericControl("script")
                {
                    ID = "ComponentScriptDMS"
                };
                literal.Attributes.Add("language", "javascript");
                literal.Attributes.Add("type", "text/javascript");
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
                sb.AppendLine("  initTagListJavascript();");
                sb.AppendLine("});");
                sb.AppendLine("var dateFormatOptions = { day: 'numeric', month: 'numeric', year: 'numeric' };");
                sb.AppendLine("function confirmDelete(control, tagName, documentCount) {");
                sb.AppendLine("  $.confirm({");
                sb.AppendLine("    title: \"" + LocalizeString("Delete") + " '\" + tagName + \"'\",");
                sb.AppendLine("    content: (\"" + LocalizeString("ConfirmDelete") + "\").replaceAll('{0}', tagName),");
                sb.AppendLine("    buttons: {");
                sb.AppendLine("      yes: function() {");
                sb.AppendLine("        if(documentCount === 0) {");
                sb.AppendLine("          eval($(control).attr('href').replace('javascript:', ''));");
                sb.AppendLine("        } else {");
                sb.AppendLine("          $.confirm({");
                sb.AppendLine("            title: \"" + LocalizeString("Delete") + " '\" + tagName + \"'\",");
                sb.AppendLine("            content: (\"" + LocalizeString("ConfirmReallyDelete") + "\").replaceAll('{0}', tagName),");
                sb.AppendLine("            buttons: {");
                sb.AppendLine("              yes: function() {");
                sb.AppendLine("                eval($(control).attr('href').replace('javascript:', ''));");
                sb.AppendLine("              },");
                sb.AppendLine("              no: function() {");
                sb.AppendLine("              }");
                sb.AppendLine("            }");
                sb.AppendLine("          });");
                sb.AppendLine("        }");
                sb.AppendLine("      },");
                sb.AppendLine("      no: function() {");
                sb.AppendLine("      }");
                sb.AppendLine("    }");
                sb.AppendLine("  });");
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
                sb.AppendLine("MM_preloadImages('" + ResolveUrl("~" + ControlPath + "Images/Icons/DeleteIcon2_16px.gif") + "','" + ResolveUrl("~" + ControlPath + "Images/Icons/EditIcon2_16px.gif") + "');");
                sb.AppendLine("function MyEndRequest(sender, args) {");
                sb.AppendLine("  initTagListJavascript();");
                sb.AppendLine("  hideBlockingScreen();");
                sb.AppendLine("}");
                sb.AppendLine("Number.prototype.padLeft = function(size) {");
                sb.AppendLine("  var s = String(this);");
                sb.AppendLine("  while (s.length < (size || 2)) { s = '0' + s; }");
                sb.AppendLine("  return s;");
                sb.AppendLine("}");
                sb.AppendLine("function initTagListJavascript() {");
                sb.AppendLine("  $('a[href^=mailto]').on('click', function() {");
                sb.AppendLine("    ignore_onbeforeunload = true;");
                sb.AppendLine("  });");
                sb.AppendLine("  $('.documentView').click(function(e) {");
                sb.AppendLine("    showBlockingScreen();");
                sb.AppendLine("    e.preventDefault();");
                sb.AppendLine("    var id = $(this).attr('data-id');");
                sb.AppendLine("    $.ajax({");
                sb.AppendLine("      url: \"" + ControlPath + "DMSController.asmx/GetDocumentsForTag\",");
                sb.AppendLine("      type: \"POST\",");
                sb.AppendLine("      dataType: \"json\",");
                sb.AppendLine("      data: { tagId: id },");
                sb.AppendLine("      success: function(data) {");
                sb.AppendLine("        hideBlockingScreen();");
                sb.AppendLine("        $('#" + gvDocuments.ClientID + "').empty();");
                sb.AppendLine("        if (data.length > 0) {");
                sb.Append("          $('#" + gvDocuments.ClientID + "').append(\"<tr align='left' valign='top' style='color: White; background-color:#" + Theme + ";font-size:10pt;font-weight:normal;text-decoration:none;'><th align='center' scope='col' style='white-space:nowrap;text-align:center;'><span>ID</span></th><th scope='col' style='white-space:nowrap;'><span>Document Name</span></th>");
                List<Category> categories = DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId);
                foreach (Category category in categories)
                {
                    sb.Append("<th align='center' scope='col' style='white-space:nowrap;text-align:center;'><span>" + category.CategoryName + "</span></th>");
                }
                sb.AppendLine("<th align='center' scope='col' style='white-space:nowrap;text-align:center;'><span>Last Modified</span></th></tr>\");");
                sb.AppendLine("          for (var i = 0; i < data.length; i++) {");
                string[] strArrays = new string[1];
                int moduleId = base.ModuleId;
                strArrays[0] = string.Concat("mid=", moduleId.ToString());
                sb.Append("            $('#" + gvDocuments.ClientID + "').append(\"<tr valign='top' style='background-color:\" + ((i % 2 == 0) ? \"#F7F7F7\" : \"#FFFFFF\") + \";'><td align='center'>\" + data[i].DocumentId + \"</td><td><a href='" + _navigationManager.NavigateURL("DocumentList", strArrays) + "/id/\" + data[i].DocumentId + \"' style='color: #" + Theme + ";'>\" + data[i].DocumentName + \"</a></td>");
                foreach (Category category in categories)
                {
                    sb.Append("<td align='center' style='width:100px;'><span>\" + (!data[i]." + Generic.RemoveSpecialCharacters(category.CategoryName).Replace(" ", "_") + " ? \"No\" : data[i]." + Generic.RemoveSpecialCharacters(category.CategoryName).Replace(" ", "_") + ") + \"</span></td>");
                }
                sb.AppendLine("<td align='center' style='width:100px;'>\" + (new Date(parseInt(data[i].LastModifiedOnDate.substr(6)))).toLocaleString('en-US', dateFormatOptions) + \"</td></tr>\");");
                sb.AppendLine("          }");
                sb.AppendLine("        }");
                sb.AppendLine("        $('#documentListDialog').dialog('open');");
                sb.AppendLine("        $('#documentList-content').scrollTop(0);");
                sb.AppendLine("      },");
                sb.AppendLine("      error: function(jqXHR, exception) {");
                sb.AppendLine("        var msg = '';");
                sb.AppendLine("        if (jqXHR.status === 0) {");
                sb.AppendLine("          msg = 'Not connect.\\n Verify Network.';");
                sb.AppendLine("        }");
                sb.AppendLine("        else if (jqXHR.status == 404) {");
                sb.AppendLine("          msg = 'Requested page not found. [404]';");
                sb.AppendLine("        }");
                sb.AppendLine("        else if (jqXHR.status == 500) {");
                sb.AppendLine("          msg = 'Internal Server Error [500].';");
                sb.AppendLine("        }");
                sb.AppendLine("        else if (exception === 'parsererror') {");
                sb.AppendLine("          msg = 'Requested JSON parse failed.';");
                sb.AppendLine("        }");
                sb.AppendLine("        else if (exception === 'timeout') {");
                sb.AppendLine("          msg = 'Time out error.';");
                sb.AppendLine("        }");
                sb.AppendLine("        else if (exception === 'abort') {");
                sb.AppendLine("          msg = 'Ajax request aborted.';");
                sb.AppendLine("        }");
                sb.AppendLine("        else {");
                sb.AppendLine("          msg = 'Uncaught Error.\\n' + jqXHR.responseText;");
                sb.AppendLine("        }");
                sb.AppendLine("        hideBlockingScreen();");
                sb.AppendLine("        $(\"<div title='Document List'><div style='padding: 10px; text-align: center;'>\" + msg + \"</div></div>\").dialog({buttons: [{text:'OK', click: function() { $(this).dialog('close');}}]});");
                sb.AppendLine("      }");
                sb.AppendLine("    });");
                sb.AppendLine("  });");
                sb.AppendLine("  var form = $('#documentListDialog').dialog({");
                sb.AppendLine("    autoOpen: false,");
                sb.AppendLine("    bgiframe: true,");
                sb.AppendLine("    modal: true,");
                sb.AppendLine("    width: 750,");
                sb.AppendLine("    height: 600,");
                sb.AppendLine("    appendTo: 'form',");
                sb.AppendLine("    dialogClass: 'dialog',");
                sb.AppendLine("    title: 'Documents',");
                sb.AppendLine("    closeOnEsacpe: true,");
                sb.AppendLine("    Cancel: function () {");
                sb.AppendLine("      $(this).dialog('close');");
                sb.AppendLine("    }");
                sb.AppendLine("  });");
                sb.AppendLine("}");
                literal.InnerHtml = sb.ToString();
                this.Page.Header.Controls.Add(literal);
            }
        }

        public class CategoryTemplate : System.Web.UI.ITemplate
        {
            System.Web.UI.WebControls.ListItemType templateType;
            string categoryName = String.Empty;

            public CategoryTemplate(string name, System.Web.UI.WebControls.ListItemType type)
            {
                categoryName = name;
                templateType = type;
            }

            public void InstantiateIn(System.Web.UI.Control container)
            {
                Label lblCategoryName = new Label();
                lblCategoryName.ID = "lbl" + categoryName;
                lblCategoryName.Text = categoryName;
                container.Controls.Add(lblCategoryName);
            }
        }

        protected void gvDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                System.Data.DataRowView row = (System.Data.DataRowView)e.Row.DataItem;
                List<Category> categories = DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId);
                foreach (Category category in categories)
                {
                    Label lbl = ((Label)e.Row.FindControl("lbl" + Generic.RemoveSpecialCharacters(category.CategoryName).Replace(" ", "_")));
                    if (lbl != null)
                    {
                        if (row[Generic.RemoveSpecialCharacters(category.CategoryName).Replace(" ", "_")].GetType() == typeof(string))
                        {
                            lbl.Text = (string)row[Generic.RemoveSpecialCharacters(category.CategoryName).Replace(" ", "_")];
                        }
                        else
                        {
                            lbl.Text = "No";
                        }
                    }
                }
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
                filter.ForeColor = gvDocuments.HeaderStyle.BackColor = gv.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#" + Theme);
                List<Category> categories = DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId);
                if (!IsPostBack)
                {
                    btnSave.Text = LocalizeString(btnSave.ID);
                    btnCancel.Text = LocalizeString(btnCancel.ID);
                    gv.PageSize = PageSize;
                    foreach (Category category in categories)
                    {
                        TemplateField field = new TemplateField();
                        field.HeaderText = category.CategoryName + " <img src='" + ControlPath + "Images/sortneutral.png' border='0' alt='Sort by " + category.CategoryName + "' />";
                        field.HeaderStyle.Wrap = false;
                        field.SortExpression = category.CategoryName;
                        field.ItemStyle.Width = new Unit("100px");
                        field.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                        field.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                        field.ItemTemplate = new CategoryTemplate(category.CategoryName, ListItemType.Item);
                        gvDocuments.Columns.Add(field);
                    }
                    btnAddNewTag.Visible = IsDMSUser();
                    CreateDataTable(true);
                    BindDocuments(0);
                }
                else
                {
                    foreach (Category category in categories)
                    {
                        foreach (DataControlField field in gvDocuments.Columns)
                        {
                            if (field.GetType() == typeof(TemplateField))
                            {
                                TemplateField templateField = field as TemplateField;
                                if (templateField.SortExpression == category.CategoryName)
                                {
                                    templateField.ItemTemplate = new CategoryTemplate(category.CategoryName, ListItemType.Item);
                                }
                            }
                        }
                    }
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

        private void BindDocuments(int tagId)
        {
            List<DocumentView> docs = DocumentController.GetAllDocumentsForTag(tagId, PortalWideRepository);
            System.Data.DataTable dtResult = Generic.DocumentListToDataTable(docs, PortalId, PortalWideRepository ? 0 : TabModuleId);
            DataView dataView = new System.Data.DataView(dtResult);
            gvDocuments.PageIndex = 0;
            dataView.Sort = DocumentsSortColumn + " " + (DocumentsSortDirection == SortDirection.Ascending ? "ASC" : "DESC");
            gvDocuments.DataSource = dataView;
            gvDocuments.DataBind();
        }

        protected void gvDocuments_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (e.SortExpression != DocumentsSortColumn)
            {
                DocumentsSortColumn = e.SortExpression;
                DocumentsSortDirection = System.Web.UI.WebControls.SortDirection.Ascending;
            }
            else
            {
                if (DocumentsSortDirection == SortDirection.Ascending)
                {
                    DocumentsSortDirection = SortDirection.Descending;
                }
                else
                {
                    DocumentsSortDirection = SortDirection.Ascending;
                }
            }
            BindDocuments(TagID);
        }

        protected void lnkDocumentName_Command(object sender, CommandEventArgs e)
        {
            HttpResponse response = base.Response;
            string[] strArrays = new string[2];
            int moduleId = base.ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            strArrays[1] = string.Concat("id=", e.CommandArgument);
            response.Redirect(_navigationManager.NavigateURL("DocumentList", strArrays));
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            base.Response.Redirect(_navigationManager.NavigateURL(), true);
        }

        public DataView CreateDataTable(bool bReload)
        {
            DataView dataView = (DataView)Session["gv"];
            if (dataView != null && !bReload)
            {
                gv.DataSource = dataView;
                gv.DataBind();
                return dataView;
            }
            else
            {
                List<Components.Tag> tags = DocumentController.GetAllTags(PortalId, PortalWideRepository ? 0 : TabModuleId);
                DataTable dtResult = Generic.ListToDataTable(tags);
                dataView = new DataView(dtResult);
                dataView.Sort = SortColumn + " " + (SortDirection == SortDirection.Ascending ? "ASC" : "DESC");
                AddFilter(dataView);
                return dataView;
            }
        }

        private void AddFilter(DataView dataView)
        {
            if (!filter.Filter.Equals("All"))
            {
                if (filter.Filter.Equals("#"))
                {
                    dataView.RowFilter = "TagName LIKE '1%' OR TagName LIKE '2%' OR TagName LIKE '3%' OR TagName LIKE '4%' OR TagName LIKE '5%' OR TagName LIKE '6%' OR TagName LIKE '7%' OR TagName LIKE '8%' OR TagName LIKE '9%' OR TagName LIKE '0%'";
                }
                else
                {
                    dataView.RowFilter = "TagName LIKE '" + filter.Filter + "%'";
                }
                gv.EmptyDataText = "<br /><strong>No tags found for '" + filter.Filter + "'.</strong>";
            }
            else
            {
                dataView.RowFilter = String.Empty;
                gv.EmptyDataText = "<br /><strong>No tags found.</strong>";
            }
            gv.PageIndex = 0;
            gv.DataSource = dataView;
            gv.DataBind();
            Session["gv"] = dataView;
        }

        protected void gv_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dtResult = new DataTable();
            DataView dataView = new DataView(dtResult);
            SortColumn = e.SortExpression;
            if (SortDirection == SortDirection.Ascending)
            {
                SortDirection = SortDirection.Descending;
            }
            else
            {
                SortDirection = SortDirection.Ascending;
            }
            dataView = (DataView)Session["gv"];
            dataView.Sort = SortColumn + " " + (SortDirection == SortDirection.Ascending ? "ASC" : "DESC");
            gv.PageIndex = 0;
            gv.DataSource = dataView;
            gv.DataBind();
            Session["gv"] = dataView;
            gv.Visible = true;
        }

        protected void gv_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            DataTable dtResult = new DataTable();
            DataView dataView = new DataView(dtResult);
            dataView = (DataView)Session["gv"];
            gv.PageIndex = e.NewPageIndex;
            gv.DataSource = dataView;
            gv.DataBind();
            gv.Visible = true;
        }

        protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
        {
            int tagID = Convert.ToInt32(gv.DataKeys[e.NewEditIndex].Value.ToString());
            TagID = tagID;
            BindData();
            pnlDetails.Visible = true;
            pnlGrid.Visible = false;
            e.Cancel = true;
        }

        protected void gv_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int tagID = Convert.ToInt32(gv.DataKeys[e.RowIndex].Value.ToString());
            DocumentController.DeleteTag(tagID);
            CreateDataTable(true);
            e.Cancel = true;
        }

        private void AddNewTag()
        {
            TagID = 0;
            BindData();
            pnlDetails.Visible = true;
            pnlGrid.Visible = false;
        }

        protected void btnAddNewTag_Click(object sender, EventArgs e)
        {
            AddNewTag();
        }

        protected void filter_OnClick(object sender, LetterFilter.LetterFilterEventArgs e)
        {
            AddFilter((DataView)Session["gv"]);
        }

        protected void gv_DataBound(object sender, EventArgs e)
        {
            Generic.ApplyPaging(gv);
        }

        private void BindData()
        {
            Components.Tag tag = Components.DocumentController.GetTag(TagID);
            if (tag != null)
            {
                tbTagName.Text = tag.TagName;
                //rblIsPrivate.SelectedIndex = (tag.IsPrivate ? 0 : 1);
                //tbWeight.Value = tag.Weight;
            }
            else
            {
                tbTagName.Text = String.Empty;
                //rblIsPrivate.SelectedIndex = 0;
                //tbWeight.Value = 0;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlDetails.Visible = false;
            pnlGrid.Visible = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Components.Tag tempTag = Components.DocumentController.GetTagByTagName(tbTagName.Text, PortalId, PortalWideRepository ? 0 : TabModuleId);
            if (tempTag == null || tempTag.TagId == 0 || tempTag.TagId == TagID)
            {
                Components.Tag tag = Components.DocumentController.GetTag(TagID);
                if(tag == null)
                {
                    tag = new Tag();
                    tag.PortalId = PortalId;
                    tag.TabModuleId = TabModuleId;
                }
                tag.TagName = tbTagName.Text;
                //tag.IsPrivate = (rblIsPrivate.SelectedIndex == 0);
                //tag.Weight = (int)tbWeight.Value;
                Components.DocumentController.SaveTag(tag);
                pnlDetails.Visible = false;
                pnlGrid.Visible = true;
                CreateDataTable(true);
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "A tag with that name already exists!", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
            }
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
        }

        protected string JSEncode(string text)
        {
            return Generic.JSEncode(text);
        }
    }
}