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
using System.Text;

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
    public partial class BulkImport : DMSModuleBase, IActionable
    {
        private readonly INavigationManager _navigationManager;
        public BulkImport()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
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
                sb.AppendLine("function MyEndRequest(sender, args) {");
                sb.AppendLine("  hideBlockingScreen();");
                sb.AppendLine("  if ($('#" + hidFileImportStatus.ClientID + "').val() === 'Started') {");
                sb.AppendLine("    $('#" + hidFileImportStatus.ClientID + "').val('Running');");
                sb.AppendLine("    $('#" + hidFilesImported.ClientID + "').val('0')");
                sb.AppendLine("    $('#progress').text('0%');");
                sb.AppendLine("    $('#" + progressBar.ClientID + "').width($('#progress').text());");
                sb.AppendLine("    doPolling($('#" + hidProcessName.ClientID + "').val());");
                sb.AppendLine("  } else if ($('#" + hidFileImportStatus.ClientID + "').val() === 'Finished') {");
                sb.AppendLine("    $.alert({ title: 'Bulk Import', content: $('#" + hidFilesImported.ClientID + "').val() + ' Document(s) Imported.' });");
                //sb.AppendLine("    $(\"<div title='Bulk Import'><div style='padding: 10px; text-align: center;'>\" + $('#" + hidFilesImported.ClientID + "').val() + \" Document(s) Imported.</div></div>\").dialog({buttons: [{text:'OK', click: function() { $(this).dialog('close');}}]});");
                sb.AppendLine("    $('#" + hidFileImportStatus.ClientID + "').val('Idle');");
                sb.AppendLine("  }");
                sb.AppendLine("}");
                sb.AppendLine("function doPolling(processName) {");
                sb.AppendLine("  $.ajax({");
                sb.AppendLine("    url: \"" + ControlPath + "DMSController.asmx/GetImportFilesProgress\",");
                sb.AppendLine("    type: \"POST\",");
                sb.AppendLine("    dataType: \"json\",");
                sb.AppendLine("    data: { processName: processName },");
                sb.AppendLine("    success: function (result) {");
                sb.AppendLine("      $('#" + hidFilesImported.ClientID + "').val(result.FilesProcessed)");
                sb.AppendLine("      $('#progress').text(result.Progress + '%');");
                sb.AppendLine("      $('#" + progressBar.ClientID + "').width($('#progress').text());");
                sb.AppendLine("      if (parseInt(result.Progress, 10) < 100) {");
                sb.AppendLine("        setTimeout(function() { doPolling(processName); }, 250);");
                sb.AppendLine("      } else {");
                sb.AppendLine("        var window = $find('" + bulkInsertWindow.ClientID + "');");
                sb.AppendLine("        window.close();");
                sb.AppendLine("         " + Page.ClientScript.GetPostBackEventReference(lnkFinish, String.Empty) + ";");
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
                sb.AppendLine("MM_preloadImages('" + ResolveUrl("~/desktopmodules/Gafware/DMS/images/Icons/DeleteIcon2.gif") + "');");
                sb.AppendLine("$(document).ready(function() {");
                sb.AppendLine("  var prm = Sys.WebForms.PageRequestManager.getInstance();");
                sb.AppendLine("  prm.add_endRequest(MyEndRequest);");
                sb.AppendLine("});");
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
                bulkInsertWindow.IconUrl = ControlPath + "Images/import.png";
                bulkInsertWindow.VisibleOnPageLoad = false;
                hidFileImportStatus.Value = "Idle";
                progressBar.Style["background-color"] = "#" + Theme;
                if (!IsPostBack)
                {
                    btnBack.Text = LocalizeString(btnBack.ID);
                    btnImport.Text = LocalizeString(btnImport.ID);
                    btnReset.Text = LocalizeString(btnReset.ID);
                    litCSS.Text = "<style type=\"text/css\">" + Generic.ToggleButtonCssString(LocalizeString("No"), LocalizeString("Yes"), new Unit("100px"), System.Drawing.ColorTranslator.FromHtml("#" + Theme)) + "</style>";
                    ddlSecurityRole.DataSource = DotNetNuke.Security.Roles.RoleController.Instance.GetRoles(PortalId);
                    ddlSecurityRole.DataBind();
                    ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Users", "-1"));
                    ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Superusers", "-2"));
                    ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Unauthenticated Users", "-3"));
                    rptCategory.DataSource = Components.DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId);
                    rptCategory.DataBind();
                    rptCategory.Visible = Components.DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId).Count > 1;
                    ddlSecurityRole.SelectedIndex = ddlSecurityRole.Items.IndexOf(ddlSecurityRole.Items.FindByValue("-1"));
                    ddOwner.DataSource = Components.UserController.GetUsers(UserRole, PortalId);
                    ddOwner.DataBind();
                    ddOwner.SelectedIndex = ddOwner.Items.IndexOf(ddOwner.Items.FindByValue(UserId.ToString()));
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
            base.Response.Redirect(_navigationManager.NavigateURL(), true);
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            scriptManager.AsyncPostBackTimeout = 3600;
            valFilePath.ErrorMessage = "<br />Invalid Folder";
            valFilePath.IsValid = System.IO.Directory.Exists(tbFilePath.Text);
            if (Page.IsValid)
            {
                try
                {
                    List<Components.FileType> fileTypes = Components.DocumentController.GetAllFileTypes(PortalId, PortalWideRepository ? 0 : TabModuleId);
                    StringBuilder searchPatterns = new StringBuilder();
                    bool first = true;
                    foreach (Components.FileType fileType in fileTypes)
                    {
                        string[] extensions = fileType.FileTypeExt.Split(',');
                        foreach (string ext in extensions)
                        {
                            if (!first)
                            {
                                searchPatterns.Append("|");
                            }
                            searchPatterns.Append("*." + ext);
                        }
                        first = false;
                    }
                    List<int> categories = new List<int>();
                    foreach (RepeaterItem item in rptCategory.Items)
                    {
                        HiddenField hidCategoryId = (HiddenField)item.FindControl("hidCategoryId");
                        if (hidCategoryId != null)
                        {
                            CheckBox cbCategory = (CheckBox)item.FindControl("cbCategory");
                            if (cbCategory != null)
                            {
                                int categoryId = Convert.ToInt32(hidCategoryId.Value);
                                if (cbCategory.Checked)
                                {
                                    categories.Add(categoryId);
                                }
                            }
                        }
                    }
                    DateTime? activationDate = null;
                    DateTime? expirationDate = null;
                    DateTime dateTime;
                    if(DateTime.TryParse(dtActivation.Text, out dateTime))
                    {
                        activationDate = dateTime;
                    }
                    if(DateTime.TryParse(dtExpiration.Text, out dateTime))
                    {
                        expirationDate = dateTime;
                    }
                    hidProcessName.Value = DMSController.ImportFiles(ControlPath, tbFilePath.Text, cbSubFolderIsDocumentName.Checked, cbSubFolderIsTag.Checked, cbPrependSubFolderName.Checked, lstSeperator.SelectedValue, lstLevel.SelectedIndex, activationDate, expirationDate, Convert.ToInt32(ddOwner.SelectedValue), cbIsSearchable.Checked, cbUseCategorySecurityRoles.Checked, Convert.ToInt32(ddlSecurityRole.SelectedValue), categories.ToArray(), cbReplacePDFTitle.Checked, cbIsPublic.Checked, PortalId, TabModuleId, PortalWideRepository);
                    bulkInsertWindow.VisibleOnPageLoad = true;
                    hidFileImportStatus.Value = "Started";
                }
                catch (Exception ex)
                {
                    valFilePath.ErrorMessage = "<br />" + ex.Message;
                    valFilePath.IsValid = false;
                }
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            tbFilePath.Text = String.Empty;
            cbSubFolderIsDocumentName.Checked = false;
            cbSubFolderIsTag.Checked = false;
            cbUseCategorySecurityRoles.Checked = false;
            cbIsSearchable.Checked = true;
            cbReplacePDFTitle.Checked = true;
            cbPrependSubFolderName.Checked = false;
            cbIsPublic.Checked = true;
            ddlSecurityRole.DataSource = DotNetNuke.Security.Roles.RoleController.Instance.GetRoles(PortalId);
            ddlSecurityRole.DataBind();
            ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Users", "-1"));
            ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Superusers", "-2"));
            ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Unauthenticated Users", "-3"));
            rptCategory.DataSource = Components.DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId);
            rptCategory.DataBind();
            rptCategory.Visible = Components.DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId).Count > 1;
            pnlSecurityRole.Visible = !cbUseCategorySecurityRoles.Checked;
            ddlSecurityRole.SelectedIndex = ddlSecurityRole.Items.IndexOf(ddlSecurityRole.Items.FindByValue("-1"));
            ddOwner.DataSource = Components.UserController.GetUsers(UserRole, PortalId);
            ddOwner.DataBind();
            ddOwner.SelectedIndex = ddOwner.Items.IndexOf(ddOwner.Items.FindByValue(UserId.ToString()));
            pnlSeperator.Visible = pnlLevel.Visible = false;
        }

        protected void rptCategory_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                CheckBox cbCategory = (CheckBox)e.Item.FindControl("cbCategory");
                if (cbCategory != null)
                {
                    cbCategory.Checked = true;
                }
            }
        }

        protected void cbUseCategorySecurityRoles_CheckedChanged(object sender, EventArgs e)
        {
            pnlSecurityRole.Visible = !cbUseCategorySecurityRoles.Checked;
        }

        protected void btnReloadSecurityRoles_Click(object sender, EventArgs e)
        {
            int oldIndex = ddlSecurityRole.SelectedIndex;
            ddlSecurityRole.SelectedIndex = -1;
            ddlSecurityRole.DataSource = DotNetNuke.Security.Roles.RoleController.Instance.GetRoles(PortalId);
            ddlSecurityRole.DataBind();
            ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Users", "-1"));
            ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Superusers", "-2"));
            ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Unauthenticated Users", "-3"));
            if (oldIndex < ddlSecurityRole.Items.Count)
            {
                ddlSecurityRole.SelectedIndex = oldIndex;
            }
        }

        protected void cbPrependSubFolderName_CheckedChanged(object sender, EventArgs e)
        {
            pnlSeperator.Visible = cbPrependSubFolderName.Checked;
            pnlLevel.Visible = cbPrependSubFolderName.Checked || cbSubFolderIsTag.Checked;
        }

        protected void cbSubFolderIsTag_CheckedChanged(object sender, EventArgs e)
        {
            pnlLevel.Visible = cbPrependSubFolderName.Checked || cbSubFolderIsTag.Checked;
        }

        protected void lnkFinish_Click(object sender, EventArgs e)
        {
            bulkInsertWindow.VisibleOnPageLoad = false;
            btnReset_Click(sender, e);
            hidFileImportStatus.Value = "Finished";
        }
    }
}