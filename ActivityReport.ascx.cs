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
using DotNetNuke.Common;
using System.Collections;
using DotNetNuke.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

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
    public partial class ActivityReport : DMSModuleBase, IActionable
    {
        private readonly INavigationManager _navigationManager;

        public ActivityReport()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        public int ReportUserId
        {
            get
            {
                return (ViewState["ReportUserID"] != null ? (int)ViewState["ReportUserID"] : 0);
            }
            set
            {
                ViewState["ReportUserID"] = value;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
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
                sb.AppendLine("  prm.add_endRequest(ViewEndRequest);");
                sb.AppendLine("  initViewJavascript();");
                sb.AppendLine("  $(\".ui-autocomplete\").wrap('<div id=\"dms\" />');");
                sb.AppendLine("});");
                sb.AppendLine("function ViewEndRequest(sender, args) {");
                sb.AppendLine("  initViewJavascript();");
                sb.AppendLine("  hideBlockingScreen();");
                sb.AppendLine("}");
                sb.AppendLine("function initViewJavascript() {");
                sb.AppendLine("  $('a[href^=mailto]').on('click', function() {");
                sb.AppendLine("    ignore_onbeforeunload = true;");
                sb.AppendLine("  });");
                sb.AppendLine("}");
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
                if (!IsPostBack)
                {
                    if(Request.QueryString["uid"] != null && Generic.IsNumber(Request.QueryString["uid"]))
                    {
                        ReportUserId = Convert.ToInt32(Request.QueryString["uid"]);
                    }
                    dtTo.SelectedDate = DateTime.Now;
                    dtFrom.SelectedDate = DateTime.Now.AddDays(-30);
                    LoadReport();
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

        protected void btnBack_Click(object sender, EventArgs e)
        {
            HttpResponse response = base.Response;
            string[] strArrays = new string[ReportUserId != 0 ? 2 : 1];
            int moduleId = base.ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            if (ReportUserId != 0)
            {
                strArrays[1] = string.Concat("uid=", ReportUserId);
            }
            response.Redirect(_navigationManager.NavigateURL("DocumentList", strArrays));
        }

        public IList<Components.DocumentActivity> GetActivity(DateTime date)
        {
            List<Components.DocumentActivity> docs = new List<DocumentActivity>();
            DotNetNuke.Entities.Users.UserInfo user = DotNetNuke.Entities.Users.UserController.Instance.GetUserById(PortalId, ReportUserId == 0 ? UserId : ReportUserId);
            DataSet ds = new DataSet();
            string[] files = System.IO.Directory.GetFiles(MapPath("~/Portals/_default/Logs"), string.Format("Gafware_DMS_{0}_{1}_*.xml", PortalId, date.ToString("MM_dd_yyyy")));
            foreach (string filename in files)
            {
                ds.ReadXml(filename, XmlReadMode.Auto);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Components.Document doc = Components.DocumentController.GetDocument(Convert.ToInt32(row["Doc_ID"]));
                    if (doc != null)
                    {
                        DotNetNuke.Security.Roles.RoleInfo groupOwner = (doc.IsGroupOwner ? Components.UserController.GetRoleById(PortalId, doc.CreatedByUserID) : null);
                        if ((ReportUserId > 0 && ((!doc.IsGroupOwner && doc.CreatedByUserID == ReportUserId) || (doc.IsGroupOwner && user.IsInRole(groupOwner.RoleName)))) || (ReportUserId == 0 && ((!doc.IsGroupOwner && doc.CreatedByUserID == UserId) || (doc.IsGroupOwner && user.IsInRole(groupOwner.RoleName)) || IsAdmin() || user.IsSuperUser)))
                        {
                            Components.DMSFile file = Components.DocumentController.GetFile(Convert.ToInt32(row["File_ID"]));
                            if (file != null)
                            {
                                docs.Add(new DocumentActivity()
                                {
                                    DocumentId = doc.DocumentId,
                                    DocumentName = doc.DocumentName,
                                    IPAddress = row["IP"].ToString(),
                                    Time = Convert.ToDateTime(row["Time"].ToString()),
                                    SearchTerms = row["Search_Terms"].ToString(),
                                    PortalId = doc.PortalId,
                                    TabModuleId = doc.TabModuleId,
                                    FileId = file.FileId,
                                    Filename = (file.FileType.Equals("url", StringComparison.OrdinalIgnoreCase) ? file.WebPageUrl : file.Filename),
                                    FileType = file.FileType
                                });
                            }
                        }
                    }
                }
            }
            return docs;
        }

        private void LoadReport()
        {
            DateTime fromDate = dtFrom.SelectedDate.Value;
            DateTime toDate = dtTo.SelectedDate.Value;
            if (dtTo.SelectedDate.Value.Date < dtFrom.SelectedDate.Value.Date)
            {
                toDate = dtFrom.SelectedDate.Value;
                fromDate = dtTo.SelectedDate.Value;
            }
            DateTime date = fromDate;
            List<Components.DocumentActivity> docs = new List<DocumentActivity>();
            while (date.Date <= toDate.Date)
            {
                docs.AddRange(GetActivity(date));
                date = date.AddDays(1);
            }
            Telerik.Reporting.ObjectDataSource dataSource = new Telerik.Reporting.ObjectDataSource();
            dataSource.DataSource = docs.OrderByDescending(d => d.Time).ThenBy(d => d.DocumentName);
            DMSActivityReport dmsRpt = new DMSActivityReport();
            dmsRpt.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            var title = dmsRpt.Items.Find("ReportNameTextBox", true)[0] as Telerik.Reporting.TextBox;
            DotNetNuke.Entities.Users.UserInfo user = DotNetNuke.Entities.Users.UserController.Instance.GetUserById(PortalId, ReportUserId == 0 ? UserId : ReportUserId);
            if (ReportUserId == 0 && IsAdmin() || user.IsSuperUser)
            {
                title.Value = "Activity Report For All Documents";
            }
            else
            {
                title.Value = string.Format("Activity Report For Documents Owned By {0} {1}", user.FirstName, user.LastName);
            }
            var table = dmsRpt.Items.Find("table1", true)[0] as Telerik.Reporting.Table;
            table.DataSource = dataSource;
            Telerik.Reporting.InstanceReportSource report = new Telerik.Reporting.InstanceReportSource();
            report.ReportDocument = dmsRpt;
            var dateTextBox = dmsRpt.Items.Find("ReportDateTextBox", true)[0] as Telerik.Reporting.TextBox;
            dateTextBox.Value = string.Format("Date: {0:MM/dd/yyyy} to {1:MM/dd/yyyy}", fromDate, toDate);
            report1.ViewMode = Telerik.ReportViewer.WebForms.ViewMode.PrintPreview;
            report1.ReportSource = report;
            report1.RefreshReport();
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            LoadReport();
        }
    }
}