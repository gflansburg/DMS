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

        private int FilesImported { get; set; }

        public BulkImport()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            FilesImported = 0;
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
                sb.AppendLine("function MyEndRequest(sender, args) {");
                sb.AppendLine("  hideBlockingScreen();");
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
                sb.AppendLine("  if (eval($('#" + hidFileImportComplete.ClientID + "').val()) === true) {");
                sb.AppendLine("    alert($('#" + hidFilesImported.ClientID + "').val() + ' Document(s) Imported.');");
                sb.AppendLine("  }");
                sb.AppendLine("});");
                literal.InnerHtml = sb.ToString();
                this.Page.Header.Controls.Add(literal);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    litCSS.Text = "<style type=\"text/css\">" + Generic.ToggleButtonCssString("No", "Yes", new Unit("100px"), System.Drawing.ColorTranslator.FromHtml("#" + Theme)) + "</style>";
                    ddlSecurityRole.DataSource = DotNetNuke.Security.Roles.RoleController.Instance.GetRoles(PortalId);
                    ddlSecurityRole.DataBind();
                    ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Users", "-1"));
                    ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Superusers", "-2"));
                    ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Unauthenticated Users", "-3"));
                    rptCategory.DataSource = Components.DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId);
                    rptCategory.DataBind();
                    ddlSecurityRole.SelectedIndex = ddlSecurityRole.Items.IndexOf(ddlSecurityRole.Items.FindByValue("-1"));
                    ddOwner.DataSource = Components.UserController.GetUsers(UserRole, PortalId);
                    ddOwner.DataBind();
                    ddOwner.Items.Insert(0, new ListItem("-- Select Owner --", "0"));
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
            FilesImported = 0;
            hidFilesImported.Value = "0";
            hidFileImportComplete.Value = "false";
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
                    ImportFiles(tbFilePath.Text, searchPatterns.ToString().Split('|'), 0);
                    btnReset_Click(sender, e);
                    hidFileImportComplete.Value = "true";
                    hidFilesImported.Value = FilesImported.ToString();
                }
                catch (Exception ex)
                {
                    valFilePath.ErrorMessage = "<br />" + ex.Message + (FilesImported > 0 ? string.Format(" However, {0} document" + (FilesImported > 1 ? "s were" : " was") + " imported.", FilesImported) : string.Empty);
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
            cbPrependSubFolderName.Checked = false;
            ddlSecurityRole.DataSource = DotNetNuke.Security.Roles.RoleController.Instance.GetRoles(PortalId);
            ddlSecurityRole.DataBind();
            ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Users", "-1"));
            ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Superusers", "-2"));
            ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Unauthenticated Users", "-3"));
            rptCategory.DataSource = Components.DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId);
            rptCategory.DataBind();
            pnlSecurityRole.Visible = !cbUseCategorySecurityRoles.Checked;
            ddlSecurityRole.SelectedIndex = ddlSecurityRole.Items.IndexOf(ddlSecurityRole.Items.FindByValue("-1"));
            ddOwner.DataSource = Components.UserController.GetUsers(UserRole, PortalId);
            ddOwner.DataBind();
            ddOwner.Items.Insert(0, new ListItem("-- Select Owner --", "0"));
            ddOwner.SelectedIndex = ddOwner.Items.IndexOf(ddOwner.Items.FindByValue(UserId.ToString()));
            pnlSeperator.Visible = pnlLevel.Visible = false;
        }

        private void ImportFiles(string filePath, string[] searchPatterns, int level, string parent = "", string tagName = "")
        {
            string[] folders = System.IO.Directory.GetDirectories(filePath);
            string[] files =  searchPatterns.SelectMany(filter => System.IO.Directory.GetFiles(filePath, filter)).ToArray();
            if (string.IsNullOrEmpty(tagName) && level >= Convert.ToInt32(lstLevel.SelectedValue) && cbSubFolderIsTag.Checked)
            {
                tagName = System.IO.Path.GetFileName(filePath).Replace("_", " ");
            }
            foreach (string folder in folders)
            {
                ImportFiles(folder, searchPatterns, level + 1, (level >= Convert.ToInt32(lstLevel.SelectedValue) && (cbSubFolderIsTag.Checked || cbPrependSubFolderName.Checked) ? (!string.IsNullOrEmpty(parent) ? parent + lstSeperator.SelectedValue : string.Empty) + System.IO.Path.GetFileName(filePath) : string.Empty), tagName);
            }
            Document doc = new Document();
            if (cbSubFolderIsDocumentName.Checked && files.Length > 0)
            {
                string documentName = (level >= Convert.ToInt32(lstLevel.SelectedValue) && cbPrependSubFolderName.Checked ? (!String.IsNullOrEmpty(parent) ? parent.Replace("_", " ") + lstSeperator.SelectedValue : string.Empty) : string.Empty) + System.IO.Path.GetFileName(filePath).Replace("_", " ");
                doc = Components.DocumentController.GetDocumentByName(documentName, PortalId, PortalWideRepository ? 0 : TabModuleId);
                if (doc == null)
                {
                    doc.Categories = new List<DocumentCategory>();
                    doc.Files = new List<Components.DMSFile>();
                    doc.Tags = new List<DocumentTag>();
                    doc.CategoriesRaw = new List<Category>();
                    doc.PortalId = PortalId;
                    doc.TabModuleId = TabModuleId;
                    doc.ActivationDate = dtActivation.SelectedDate;
                    doc.AdminComments = string.Empty;
                    doc.CreatedByUserID = Convert.ToInt32(ddOwner.SelectedValue);
                    doc.DocumentDetails = documentName;
                    doc.ExpirationDate = dtExpiration.SelectedDate;
                    doc.IsSearchable = (cbIsSearchable.Checked ? "Yes" : "No");
                    doc.UseCategorySecurityRoles = cbUseCategorySecurityRoles.Checked;
                    doc.SecurityRoleId = Convert.ToInt32(ddlSecurityRole.SelectedValue);
                    //doc.ManagerToolkit = "No";
                    doc.DocumentName = documentName;
                    doc.IPAddress = Request.ServerVariables["REMOTE_ADDR"];
                    Components.DocumentController.SaveDocument(doc);
                }
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
                                if (!doc.Categories.Exists(c => c.CategoryId == categoryId))
                                {
                                    Components.DocumentCategory category = new Components.DocumentCategory();
                                    category.CategoryId = categoryId;
                                    category.DocumentId = doc.DocumentId;
                                    Components.DocumentController.SaveDocumentCategory(category);
                                    doc.Categories.Add(category);
                                }
                            }
                        }
                    }
                }
                if(cbSubFolderIsTag.Checked)
                {
                    AddTag(doc, tagName);
                }
            }
            foreach (string file in files)
            {
                if (!cbSubFolderIsDocumentName.Checked)
                {
                    DMSFile tempFile = Components.DocumentController.GetFileByName(PortalId, PortalWideRepository ? 0 : TabModuleId, (!String.IsNullOrEmpty(parent) && level >= Convert.ToInt32(lstLevel.SelectedValue) && cbPrependSubFolderName.Checked ? parent.Replace("_", " ") + lstSeperator.SelectedValue : string.Empty) + System.IO.Path.GetFileNameWithoutExtension(file).Replace("_", " "));
                    if (tempFile == null || tempFile.MimeType.Equals(MimeMapping.GetMimeMapping(file), StringComparison.OrdinalIgnoreCase))
                    {
                        doc = new Document();
                        doc.Categories = new List<DocumentCategory>();
                        doc.Files = new List<Components.DMSFile>();
                        doc.Tags = new List<DocumentTag>();
                        doc.CategoriesRaw = new List<Category>();
                        doc.PortalId = PortalId;
                        doc.TabModuleId = TabModuleId;
                        doc.ActivationDate = dtActivation.SelectedDate;
                        doc.AdminComments = string.Empty;
                        doc.CreatedByUserID = Convert.ToInt32(ddOwner.SelectedValue);
                        doc.DocumentDetails = (level >= Convert.ToInt32(lstLevel.SelectedValue) && cbPrependSubFolderName.Checked ? (!String.IsNullOrEmpty(parent) ? parent.Replace("_", " ") + lstSeperator.SelectedValue : string.Empty) + System.IO.Path.GetFileName(filePath).Replace("_", " ") + lstSeperator.SelectedValue : string.Empty) + System.IO.Path.GetFileNameWithoutExtension(file).Replace("_", " ");
                        doc.ExpirationDate = dtExpiration.SelectedDate;
                        doc.IsSearchable = (cbIsSearchable.Checked ? "Yes" : "No");
                        doc.UseCategorySecurityRoles = cbUseCategorySecurityRoles.Checked;
                        doc.SecurityRoleId = Convert.ToInt32(ddlSecurityRole.SelectedValue);
                        //doc.ManagerToolkit = "No";
                        doc.DocumentName = (level >= Convert.ToInt32(lstLevel.SelectedValue) && cbPrependSubFolderName.Checked ? (!String.IsNullOrEmpty(parent) ? parent.Replace("_", " ") + lstSeperator.SelectedValue : string.Empty) + System.IO.Path.GetFileName(filePath).Replace("_", " ") + lstSeperator.SelectedValue : string.Empty) + System.IO.Path.GetFileNameWithoutExtension(file).Replace("_", " ");
                        doc.IPAddress = Request.ServerVariables["REMOTE_ADDR"];
                        Components.DocumentController.SaveDocument(doc);
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
                                        if (!doc.Categories.Exists(c => c.CategoryId == categoryId))
                                        {
                                            Components.DocumentCategory category = new Components.DocumentCategory();
                                            category.CategoryId = categoryId;
                                            category.DocumentId = doc.DocumentId;
                                            Components.DocumentController.SaveDocumentCategory(category);
                                            doc.Categories.Add(category);
                                        }
                                    }
                                }
                            }
                        }
                        if (cbSubFolderIsTag.Checked)
                        {
                            AddTag(doc, tagName);
                        }
                    }
                    else
                    {
                        doc = Components.DocumentController.GetDocument(tempFile.DocumentId);
                    }
                }
                using (System.IO.FileStream stream = new System.IO.FileStream(file, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                {
                    if (stream.Length > 0)
                    {
                        Components.DMSFile dmsFile = doc.Files.Find(p => p.Filename.Equals(System.IO.Path.GetFileName(file).Replace(" ", "_"), StringComparison.OrdinalIgnoreCase));
                        if (dmsFile == null)
                        {
                            dmsFile = new Components.DMSFile();
                            dmsFile.DocumentId = doc.DocumentId;
                            dmsFile.FileType = Components.DMSFile.GetFileType(System.IO.Path.GetExtension(file), PortalId, PortalWideRepository ? 0 : TabModuleId);
                            dmsFile.StatusId = 1;
                            dmsFile.Filename = System.IO.Path.GetFileName(file).Replace(" ", "_");
                            dmsFile.UploadDirectory = string.Format("Files/{0}", Generic.CreateSafeFolderName(doc.DocumentName));
                            dmsFile.MimeType = MimeMapping.GetMimeMapping(file);
                            Components.DocumentController.SaveFile(dmsFile);
                            dmsFile.FileVersion = new Components.FileVersion();
                            dmsFile.FileVersion.FileId = dmsFile.FileId;
                            dmsFile.FileVersion.Version = 1000000;
                            dmsFile.CreatedOnDate = DateTime.Now;
                            doc.Files.Add(dmsFile);
                        }
                        else
                        {
                            dmsFile.FileVersion = dmsFile.FileVersion;
                            dmsFile.FileVersion.FileVersionId = 0;
                            dmsFile.FileVersion.Version++;
                        }
                        dmsFile.FileVersion.IPAddress = Request.ServerVariables["REMOTE_ADDR"];
                        dmsFile.FileVersion.CreatedByUserID = UserId;
                        dmsFile.FileVersion.CreatedOnDate = DateTime.Now;
                        dmsFile.FileVersion.Filesize = (int)stream.Length;
                        Components.DocumentController.SaveFileVersion(dmsFile.FileVersion);
                        dmsFile.FileVersionId = dmsFile.FileVersion.FileVersionId;
                        Components.DocumentController.SaveFile(dmsFile);
                        dmsFile.FileVersion.SaveContents(stream);
                        dmsFile.FileVersion.Contents = null;
                        Generic.CreateThumbnail(Request, ControlPath, dmsFile);
                        dmsFile.FileVersion.Thumbnail = null;
                        FilesImported++;
                    }
                }
            }
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

        private void AddTag(Document doc, string tagName)
        {
            Components.DocumentTag docTag = new Components.DocumentTag();
            docTag.DocumentId = doc.DocumentId;
            docTag.Tag = Components.DocumentController.GetTagByTagName(tagName, PortalId, PortalWideRepository ? 0 : TabModuleId);
            if (docTag.Tag == null || docTag.Tag.TagId == 0)
            {
                docTag.Tag = new Components.Tag();
                docTag.Tag.TagName = tagName;
                docTag.Tag.IsPrivate = "No";
                docTag.Tag.PortalId = PortalId;
                docTag.Tag.TabModuleId = TabModuleId;
                Components.DocumentController.SaveTag(docTag.Tag);
                docTag.TagId = docTag.Tag.TagId;
            }
            else
            {
                docTag.TagId = docTag.Tag.TagId;
            }
            Components.DocumentController.SaveDocumentTag(docTag);
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
    }
}