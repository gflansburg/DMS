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
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using Gafware.Modules.DMS.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Gafware.Modules.DMS
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Settings class manages Module Settings
    /// 
    /// Typically your settings control would be used to manage settings for your module.
    /// There are two types of settings, ModuleSettings, and TabModuleSettings.
    /// 
    /// ModuleSettings apply to all "copies" of a module on a site, no matter which page the module is on. 
    /// 
    /// TabModuleSettings apply only to the current module on the current page, if you copy that module to
    /// another page the settings are not transferred.
    /// 
    /// If you happen to save both TabModuleSettings and ModuleSettings, TabModuleSettings overrides ModuleSettings.
    /// 
    /// Below we have some examples of how to access these settings but you will need to uncomment to use.
    /// 
    /// Because the control inherits from DMSSettingsBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Settings : DMSModuleSettingsBase
    {
        private readonly INavigationManager _navigationManager;

        public Settings()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
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
                sb.AppendLine("jQuery(document).ready(function() {");
                sb.AppendLine("  var prm = Sys.WebForms.PageRequestManager.getInstance();");
                sb.AppendLine("  prm.add_endRequest(SettingsEndRequest);");
                sb.AppendLine("  prm.add_beginRequest(SettingsBeginRequest);");
                sb.AppendLine("});");
                sb.AppendLine("function SettingsBeginRequest(sender, args) {");
                sb.AppendLine("  try { CKEDITOR.instances." + txtReplyEmail.ClientID + "_txtReplyEmail.updateElement(); } catch {}");
                sb.AppendLine("}");
                sb.AppendLine("function SettingsEndRequest(sender, args) {");
                sb.AppendLine("  if (CKEDITOR.instances['" + txtReplyEmail.ClientID + "_txtReplyEmail']) {");
                sb.AppendLine("    CKEDITOR.remove(CKEDITOR.instances['" + txtReplyEmail.ClientID + "_txtReplyEmail']);");
                sb.AppendLine("  }");
                sb.AppendLine("  LoadCKEditorInstance_" + txtReplyEmail.ClientID + "_txtReplyEmail(sender, args);");
                sb.AppendLine("}");
                literal.InnerHtml = sb.ToString();
                this.Page.Header.Controls.Add(literal);
            }
        }

        #region Base Method Implementations
        public void Page_Load()
        {
            if(!IsPostBack)
            {
                lblSaveLocalFile.HelpText = string.Format(LocalizeString("SaveLocalFileHelp"), PortalId, TabModuleId);
            }
            if (Request.QueryString["ctl"].ToLower() == "editsettings" && !IsAdmin())
            {
                base.Response.Redirect(_navigationManager.NavigateURL(), true);
            }
            if (!IsPostBack && Request.QueryString["ctl"].ToLower() == "editsettings")
            {
                LoadSettings();
                pnlUpdateSettings.Visible = true;
            }
        }

        public bool IsAdmin()
        {
            if ((new ModuleSecurity((new ModuleController()).GetTabModule(this.TabModuleId))).HasEditPermissions)
            {
                return true;
            }
            return false;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LoadSettings loads the settings from the Database and displays them
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void LoadSettings()
        {
            try
            {
                if (Page.IsPostBack == false)
                {
                    litCSS.Text = "<style type=\"text/css\">" + Generic.ToggleButtonCssString(LocalizeString("No"), LocalizeString("Yes")) + "</style>";
                    //Check for existing settings and use those on this page
                    ddlRole.DataSource = DotNetNuke.Security.Roles.RoleController.Instance.GetRoles(PortalId);
                    ddlRole.DataBind();
                    ddlRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Users", "-1"));
                    ddlRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Superusers", "-2"));
                    ddlRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Unauthenticated Users", "-3"));
                    ddlRole.SelectedIndex = ddlRole.Items.IndexOf(ddlRole.Items.FindByValue(UserRole.ToString()));

                    ddFileNotifications.DataSource = DotNetNuke.Security.Roles.RoleController.Instance.GetRoles(PortalId);
                    ddFileNotifications.DataBind();
                    ddFileNotifications.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Users", "-1"));
                    ddFileNotifications.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Superusers", "-2"));
                    ddFileNotifications.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Unauthenticated Users", "-3"));
                    ddFileNotifications.SelectedIndex = ddFileNotifications.Items.IndexOf(ddlRole.Items.FindByValue(FileNotificationsRole.ToString()));

                    ddlPageSize.SelectedIndex = ddlPageSize.Items.IndexOf(ddlPageSize.Items.FindByValue(PageSize.ToString()));

                    chkForceHttps.Checked = ForceHttps;
                    tbFileNotificationSubject.Text = NewFileSubject;
                    txtReplyEmail.Text = NewFileMsg;
                    tbName.Text = RepositoryName;
                    pnlName.Visible = !PortalWideRepository;

                    chkUseThumbnails.Checked = UseThumbnails;
                    pnlCreateThumbnails.Visible = UseThumbnails;
                    chkCreatePDF.Checked = CreatePDF;
                    chkCreateWord.Checked = CreateWord;
                    chkCreateExcel.Checked = CreateExcel;
                    chkCreatePowerPoint.Checked = CreatePowerPoint;
                    chkCreateImage.Checked = CreateImage;
                    chkCreateAudio.Checked = CreateAudio;
                    chkCreateVideo.Checked = CreateVideo;

                    tbCategory.Text = CategoryName;
                    if(DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId).Count == 0)
                    {
                        Category defaultCategory = new Category()
                        {
                            CategoryName = "Category 1",
                            PortalId = PortalId,
                            TabModuleId = TabModuleId
                        };
                        DocumentController.SaveCategory(defaultCategory);
                    }
                    gv_Categories.DataSource = DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId);
                    gv_Categories.DataBind();

                    if (DocumentController.GetAllFileTypes(PortalId, PortalWideRepository ? 0 : TabModuleId).Count == 0)
                    {
                        DocumentController.AddDefaultFileExtensions(PortalId, PortalWideRepository ? 0 : TabModuleId);
                    }
                    gv_docTypes.DataSource = DocumentController.GetAllFileTypes(PortalId, PortalWideRepository ? 0 : TabModuleId);
                    gv_docTypes.DataBind();

                    chkSaveLocalFile.Checked = SaveLocalFile;
                    chkShowTips.Checked = ShowTips;
                    chkShowInstructions.Checked = ShowInstructions;
                    chkPortalWideRepository.Checked = PortalWideRepository;
                    chkEnableDNNSearch.Checked = EnableDNNSearch;
                    tbInstructions.Text = Instructions;
                    pnlInstructions.Visible = chkShowInstructions.Checked;
                    ddlTheme.SelectedIndex = ddlTheme.Items.IndexOf(ddlTheme.Items.FindByValue(Theme));
                    ddlThumbnailType.SelectedIndex = ddlThumbnailType.Items.IndexOf(ddlThumbnailType.Items.FindByValue(ThumbnailType));
                    ddlThumbnailSize.SelectedIndex = ddlThumbnailSize.Items.IndexOf(ddlThumbnailSize.Items.FindByValue(ThumbnailSize.ToString()));
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpdateSettings saves the modified settings to the Database
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void UpdateSettings()
        {
            try
            {
                //module settings
                Components.Repository repository = Components.DocumentController.GetRepository(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
                if(repository == null)
                {
                    repository = new Repository();
                    repository.PortalId = PortalId;
                    repository.TabModuleId = chkPortalWideRepository.Checked ? 0 : TabModuleId;
                }
                repository.FileNotificationsRoleId = Convert.ToInt32(ddFileNotifications.SelectedValue);
                repository.UserRoleId = Convert.ToInt32(ddlRole.SelectedValue);
                repository.NewFileMsg = txtReplyEmail.Text;
                repository.NewFileSubject = tbFileNotificationSubject.Text;
                repository.CategoryName = tbCategory.Text;
                repository.SaveLocalFile = chkSaveLocalFile.Checked;
                repository.ShowTips = chkShowTips.Checked;
                repository.ShowInstructions = chkShowInstructions.Checked;
                repository.Instructions = tbInstructions.Text;
                repository.Theme = ddlTheme.SelectedValue;
                repository.ThumbnailType = ddlThumbnailType.SelectedValue;
                repository.ThumbnailSize = Convert.ToInt32(ddlThumbnailSize.SelectedValue);
                repository.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
                repository.ForceHttps = chkForceHttps.Checked;
                repository.UseThumbnails = chkUseThumbnails.Checked;
                repository.CreatePDF = chkCreatePDF.Checked;
                repository.CreateWord = chkCreateWord.Checked;
                repository.CreateExcel = chkCreateExcel.Checked;
                repository.CreatePowerPoint = chkCreatePowerPoint.Checked;
                repository.CreateImage = chkCreateImage.Checked;
                repository.CreateAudio = chkCreateAudio.Checked;
                repository.CreateVideo = chkCreateVideo.Checked;
                repository.Name = (chkPortalWideRepository.Checked ? "All Portal Repositories" : tbName.Text);
                try
                {
                    Components.DocumentController.SaveRepository(repository);
                }
                catch(Exception)
                {
                }
                DMSPortalSettings settings = Components.DocumentController.GetPortalSettings(PortalId);
                if(settings == null)
                {
                    settings.PortalId = PortalId;
                }
                settings.PortalWideRepository = chkPortalWideRepository.Checked;
                settings.EnableDNNSearch = chkEnableDNNSearch.Checked;
                Components.DocumentController.SavePortalSettings(settings);
                DotNetNuke.Entities.Portals.PortalSettings portalSettings = DotNetNuke.Entities.Portals.PortalSettings.Current;
                if (repository.SaveLocalFile)
                {
                    List<Components.DocumentView> documents = Components.DocumentController.GetAllDocumentsForDropDown(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
                    foreach (Components.DocumentView doc in documents)
                    {
                        if (doc != null && (!doc.ActivationDate.HasValue || DateTime.Now >= doc.ActivationDate.Value) && (!doc.ExpirationDate.HasValue || DateTime.Now <= (doc.ExpirationDate.Value + new TimeSpan(23, 59, 59))))
                        {
                            List<Components.DMSFile> files = Components.DocumentController.GetAllFilesForDocument(doc.DocumentId);
                            foreach (Components.DMSFile file in files)
                            {
                                if (file.Status.StatusId == 1)
                                {
                                    if (!System.IO.File.Exists(string.Format("{0}{1}\\{2}", portalSettings.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename)))
                                    {
                                        file.FileVersion.LoadContents();
                                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                                        {
                                            file.CreateFolder();
                                            if (System.IO.File.Exists(string.Format("{0}{1}\\{2}", portalSettings.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename)))
                                            {
                                                System.IO.File.Delete(string.Format("{0}{1}\\{2}", portalSettings.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename));
                                            }
                                            System.IO.FileStream fs = new System.IO.FileStream(string.Format("{0}{1}\\{2}", portalSettings.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename), System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
                                            fs.Write(file.FileVersion.Contents, 0, file.FileVersion.Contents.Length);
                                            fs.Close();
                                            try
                                            {
                                                System.IO.File.SetLastWriteTime(string.Format("{0}{1}\\{2}", portalSettings.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename), file.FileVersion.CreatedOnDate);
                                            }
                                            catch (Exception)
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    string filesPath = string.Format("{0}Files", portalSettings.HomeDirectoryMapPath);
                    if(System.IO.Directory.Exists(filesPath))
                    {
                        System.IO.Directory.Delete(filesPath, true);
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void btnReload_Click(object sender, EventArgs e)
        {
            int oldIndex = ddlRole.SelectedIndex;
            ddlRole.SelectedIndex = -1;
            ddlRole.DataSource = DotNetNuke.Security.Roles.RoleController.Instance.GetRoles(PortalId);
            ddlRole.DataBind();
            ddlRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Users", "-1"));
            ddlRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Superusers", "-2"));
            ddlRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Unauthenticated Users", "-3"));
            if (oldIndex < ddlRole.Items.Count)
            {
                ddlRole.SelectedIndex = oldIndex;
            }
        }
        #endregion

        protected void gv_Categories_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gv_Categories.EditIndex = e.NewEditIndex;
            gv_Categories.DataSource = DocumentController.GetAllCategories(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
            gv_Categories.DataBind();
        }

        protected void gv_Categories_RowUpdating(object sender, System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            int index = gv_Categories.EditIndex;
            GridViewRow row = gv_Categories.Rows[index];
            TextBox categoryName = row.FindControl("tbCategoryName") as TextBox;
            DropDownList ddlRequiredRole = row.FindControl("ddlRequiredRole") as DropDownList;
            List<Category> categories = DocumentController.GetAllCategories(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
            Category category = categories.Find(p => p.CategoryId == (int)gv_Categories.DataKeys[index].Value);
            if (category != null)
            {
                category.PortalId = PortalId;
                category.TabModuleId = TabModuleId;
                category.CategoryName = categoryName.Text;
                category.RoleId = Convert.ToInt32(ddlRequiredRole.SelectedValue);
                DocumentController.SaveCategory(category);
            }
            gv_Categories.EditIndex = -1;
            gv_Categories.DataSource = DocumentController.GetAllCategories(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
            gv_Categories.DataBind();
        }

        protected void gv_Categories_RowCancelingEdit(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gv_Categories.EditIndex = -1;
            gv_Categories.DataSource = DocumentController.GetAllCategories(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
            gv_Categories.DataBind();
        }

        protected void gv_Categories_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if(e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Text = tbCategory.Text;
            }
            else if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Footer)
            {
                TextBox categoryName = e.Row.FindControl(e.Row.RowType == DataControlRowType.DataRow ? "tbCategoryName" : "tbCategoryName2") as TextBox;
                if (categoryName != null)
                {
                    ImageButton saveButton = e.Row.FindControl(e.Row.RowType == DataControlRowType.DataRow ? "saveButton" : "saveInsertButton") as ImageButton;
                    string js = "if ((event.which && event.which == 13) || "
                                + "(event.keyCode && event.keyCode == 13)) "
                                //+ "{" + Page.ClientScript.GetPostBackEventReference(e.Row.RowType == DataControlRowType.DataRow ? saveButton : newButton, String.Empty) + ";return false;} "
                                + "{ $('#" + saveButton.ClientID + "').click(); return false; } "
                                + "else return true;";
                    categoryName.Attributes.Add("onkeydown", js);
                }
                DropDownList lstRole = e.Row.FindControl(e.Row.RowType == DataControlRowType.DataRow ? "ddlRequiredRole" : "ddlRequiredRole2") as DropDownList;
                if (lstRole != null)
                {
                    Category category = e.Row.DataItem as Category;
                    lstRole.DataSource = DotNetNuke.Security.Roles.RoleController.Instance.GetRoles(PortalId);
                    lstRole.DataBind();
                    lstRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Users", "-1"));
                    lstRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Superusers", "-2"));
                    lstRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Unauthenticated Users", "-3"));
                    if (category != null)
                    {
                        lstRole.SelectedValue = category.RoleId.ToString();
                    }
                    else
                    {
                        lstRole.SelectedValue = "-1";
                    }
                }
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Visible = false;
                }
            }
        }

        protected void gv_Categories_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int categoryId = (int)gv_Categories.DataKeys[e.RowIndex].Value;
            DocumentController.DeleteCategory(categoryId);
            gv_Categories.DataSource = DocumentController.GetAllCategories(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
            gv_Categories.DataBind();
        }

        protected void gv_Categories_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.Equals("New"))
                {
                    GridViewRow row = null;
                    if (e.CommandSource.GetType() == typeof(LinkButton))
                    {
                        LinkButton btnNew = e.CommandSource as LinkButton;
                        row = btnNew.NamingContainer as GridViewRow;
                    }
                    else if (e.CommandSource.GetType() == typeof(ImageButton))
                    {
                        ImageButton btnNew = e.CommandSource as ImageButton;
                        row = btnNew.NamingContainer as GridViewRow;
                    }
                    if (row == null)
                    {
                        return;
                    }
                    for (int i = 1; i < row.Cells.Count; i++)
                    {
                        row.Cells[i].Visible = true;
                    }
                    ImageButton newButton = row.FindControl("newButton") as ImageButton;
                    ImageButton saveButton = row.FindControl("saveInsertButton") as ImageButton;
                    ImageButton cancelButton = row.FindControl("cancelInsertButton") as ImageButton;
                    newButton.Visible = false;
                    saveButton.Visible = cancelButton.Visible = true;
                }
                else if (e.CommandName.Equals("Cancel"))
                {
                    GridViewRow row = null;
                    if (e.CommandSource.GetType() == typeof(LinkButton))
                    {
                        LinkButton btnNew = e.CommandSource as LinkButton;
                        row = btnNew.NamingContainer as GridViewRow;
                    }
                    else if (e.CommandSource.GetType() == typeof(ImageButton))
                    {
                        ImageButton btnNew = e.CommandSource as ImageButton;
                        row = btnNew.NamingContainer as GridViewRow;
                    }
                    if (row == null)
                    {
                        return;
                    }
                    for (int i = 1; i < row.Cells.Count; i++)
                    {
                        row.Cells[i].Visible = false;
                    }
                    ImageButton newButton = row.FindControl("newButton") as ImageButton;
                    ImageButton saveButton = row.FindControl("saveInsertButton") as ImageButton;
                    ImageButton cancelButton = row.FindControl("cancelInsertButton") as ImageButton;
                    newButton.Visible = true;
                    saveButton.Visible = cancelButton.Visible = false;
                }
                else if (e.CommandName.Equals("Insert"))
                {
                    GridViewRow row = null;
                    if (e.CommandSource.GetType() == typeof(LinkButton))
                    {
                        LinkButton btnNew = e.CommandSource as LinkButton;
                        row = btnNew.NamingContainer as GridViewRow;
                    }
                    else if (e.CommandSource.GetType() == typeof(ImageButton))
                    {
                        ImageButton btnNew = e.CommandSource as ImageButton;
                        row = btnNew.NamingContainer as GridViewRow;
                    }
                    if (row == null)
                    {
                        return;
                    }
                    for (int i = 1; i < row.Cells.Count; i++)
                    {
                        row.Cells[i].Visible = false;
                    }
                    ImageButton newButton = row.FindControl("newButton") as ImageButton;
                    ImageButton saveButton = row.FindControl("saveInsertButton") as ImageButton;
                    ImageButton cancelButton = row.FindControl("cancelInsertButton") as ImageButton;
                    newButton.Visible = true;
                    saveButton.Visible = cancelButton.Visible = false;
                    TextBox tbCategoryName = row.FindControl("tbCategoryName2") as TextBox;
                    DropDownList ddlRequiredRole = row.FindControl("ddlRequiredRole2") as DropDownList;
                    Category category = new Category();
                    category.PortalId = PortalId;
                    category.TabModuleId = TabModuleId;
                    category.CategoryName = tbCategoryName.Text;
                    category.RoleId = Convert.ToInt32(ddlRequiredRole.SelectedValue);
                    DocumentController.SaveCategory(category);
                    gv_Categories.DataSource = DocumentController.GetAllCategories(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
                    gv_Categories.DataBind();
                }
            }
            catch (Exception)
            {
            }
        }

        protected void gv_docTypes_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gv_docTypes.EditIndex = e.NewEditIndex;
            gv_docTypes.DataSource = DocumentController.GetAllFileTypes(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
            gv_docTypes.DataBind();
        }

        protected void gv_docTypes_RowUpdating(object sender, System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            int index = gv_docTypes.EditIndex;
            GridViewRow row = gv_docTypes.Rows[index];
            TextBox fileTypeName = row.FindControl("tbFileTypeName") as TextBox;
            TextBox fileTypeShortName = row.FindControl("tbFileTypeShortName") as TextBox;
            TextBox fileTypeExt = row.FindControl("tbFileTypeExt") as TextBox;
            List<FileType> fileTypes = DocumentController.GetAllFileTypes(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
            FileType fileType = fileTypes.Find(p => p.FileTypeId == (int)gv_docTypes.DataKeys[index].Value);
            if (fileType != null)
            {
                fileType.PortalId = PortalId;
                fileType.TabModuleId = chkPortalWideRepository.Checked ? 0 : TabModuleId;
                fileType.FileTypeName = fileTypeName.Text;
                fileType.FileTypeShortName = fileTypeShortName.Text;
                fileType.FileTypeExt = fileTypeExt.Text;
                DocumentController.SaveFileType(fileType);
            }
            gv_docTypes.EditIndex = -1;
            gv_docTypes.DataSource = DocumentController.GetAllFileTypes(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
            gv_docTypes.DataBind();
        }

        protected void gv_docTypes_RowCancelingEdit(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gv_docTypes.EditIndex = -1;
            gv_docTypes.DataSource = DocumentController.GetAllFileTypes(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
            gv_docTypes.DataBind();
        }

        protected void gv_docTypes_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Footer)
            {
                TextBox fileTypeExt = e.Row.FindControl(e.Row.RowType == DataControlRowType.DataRow ? "tbFileTypeExt" : "tbFileTypeExt2") as TextBox;
                if (fileTypeExt != null)
                {
                    ImageButton saveButton = e.Row.FindControl(e.Row.RowType == DataControlRowType.DataRow ? "saveFileTypeButton" : "saveInsertFileTypeButton") as ImageButton;
                    string js = "if ((event.which && event.which == 13) || "
                                + "(event.keyCode && event.keyCode == 13)) "
                                //+ "{" + Page.ClientScript.GetPostBackEventReference(e.Row.RowType == DataControlRowType.DataRow ? saveButton : newButton, String.Empty) + ";return false;} "
                                + "{ $('#" + saveButton.ClientID + "').click(); return false; } "
                                + "else return true;";
                    fileTypeExt.Attributes.Add("onkeydown", js);
                    TextBox fileTypeShortName = e.Row.FindControl(e.Row.RowType == DataControlRowType.DataRow ? "tbFileTypeShortName" : "tbFileTypeShortName2") as TextBox;
                    if (fileTypeShortName != null)
                    {
                        js = "if ((event.which && event.which == 13) || "
                                    + "(event.keyCode && event.keyCode == 13)) "
                                    + "{ $('#" + fileTypeExt.ClientID + "').focus(); var tmpStr = $('#" + fileTypeExt.ClientID + "').val(); $('#" + fileTypeExt.ClientID + "').val(''); $('#" + fileTypeExt.ClientID + "').val(tmpStr); return false; } "
                                    + "else return true;";
                        fileTypeShortName.Attributes.Add("onkeydown", js);
                    }
                    TextBox fileTypeName = e.Row.FindControl(e.Row.RowType == DataControlRowType.DataRow ? "tbFileTypeName" : "tbFileTypeName2") as TextBox;
                    if (fileTypeName != null)
                    {
                        if (fileTypeShortName != null)
                        {
                            js = "if ((event.which && event.which == 13) || "
                                    + "(event.keyCode && event.keyCode == 13)) "
                                    + "{ $('#" + fileTypeShortName.ClientID + "').focus(); var tmpStr = $('#" + fileTypeShortName.ClientID + "').val(); $('#" + fileTypeShortName.ClientID + "').val(''); $('#" + fileTypeShortName.ClientID + "').val(tmpStr); return false; } "
                                    + "else return true;";
                            fileTypeName.Attributes.Add("onkeydown", js);
                        }
                        else
                        {
                            js = "if ((event.which && event.which == 13) || "
                                        + "(event.keyCode && event.keyCode == 13)) "
                                        + "{ $('#" + fileTypeExt.ClientID + "').focus(); var tmpStr = $('#" + fileTypeExt.ClientID + "').val(); $('#" + fileTypeExt.ClientID + "').val(''); $('#" + fileTypeExt.ClientID + "').val(tmpStr); return false; } "
                                        + "else return true;";
                            fileTypeName.Attributes.Add("onkeydown", js);
                        }
                    }
                }
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Visible = false;
                }
            }
        }

        protected void gv_docTypes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int fileTypeId = (int)gv_docTypes.DataKeys[e.RowIndex].Value;
            DocumentController.DeleteFileType(fileTypeId);
            gv_docTypes.DataSource = DocumentController.GetAllFileTypes(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
            gv_docTypes.DataBind();
        }

        protected void gv_docTypes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.Equals("New"))
                {
                    GridViewRow row = null;
                    if (e.CommandSource.GetType() == typeof(LinkButton))
                    {
                        LinkButton btnNew = e.CommandSource as LinkButton;
                        row = btnNew.NamingContainer as GridViewRow;
                    }
                    else if (e.CommandSource.GetType() == typeof(ImageButton))
                    {
                        ImageButton btnNew = e.CommandSource as ImageButton;
                        row = btnNew.NamingContainer as GridViewRow;
                    }
                    if (row == null)
                    {
                        return;
                    }
                    for (int i = 1; i < row.Cells.Count; i++)
                    {
                        row.Cells[i].Visible = true;
                    }
                    ImageButton newButton = row.FindControl("newFileTypeButton") as ImageButton;
                    ImageButton saveButton = row.FindControl("saveInsertFileTypeButton") as ImageButton;
                    ImageButton cancelButton = row.FindControl("cancelInsertFileTypeButton") as ImageButton;
                    newButton.Visible = false;
                    saveButton.Visible = cancelButton.Visible = true;
                }
                else if (e.CommandName.Equals("Cancel"))
                {
                    GridViewRow row = null;
                    if (e.CommandSource.GetType() == typeof(LinkButton))
                    {
                        LinkButton btnNew = e.CommandSource as LinkButton;
                        row = btnNew.NamingContainer as GridViewRow;
                    }
                    else if (e.CommandSource.GetType() == typeof(ImageButton))
                    {
                        ImageButton btnNew = e.CommandSource as ImageButton;
                        row = btnNew.NamingContainer as GridViewRow;
                    }
                    if (row == null)
                    {
                        return;
                    }
                    for (int i = 1; i < row.Cells.Count; i++)
                    {
                        row.Cells[i].Visible = false;
                    }
                    ImageButton newButton = row.FindControl("newFileTypeButton") as ImageButton;
                    ImageButton saveButton = row.FindControl("saveInsertFileTypeButton") as ImageButton;
                    ImageButton cancelButton = row.FindControl("cancelInsertFileTypeButton") as ImageButton;
                    newButton.Visible = true;
                    saveButton.Visible = cancelButton.Visible = false;
                }
                else if (e.CommandName.Equals("Insert"))
                {
                    GridViewRow row = null;
                    if (e.CommandSource.GetType() == typeof(LinkButton))
                    {
                        LinkButton btnNew = e.CommandSource as LinkButton;
                        row = btnNew.NamingContainer as GridViewRow;
                    }
                    else if (e.CommandSource.GetType() == typeof(ImageButton))
                    {
                        ImageButton btnNew = e.CommandSource as ImageButton;
                        row = btnNew.NamingContainer as GridViewRow;
                    }
                    if (row == null)
                    {
                        return;
                    }
                    for (int i = 1; i < row.Cells.Count; i++)
                    {
                        row.Cells[i].Visible = false;
                    }
                    ImageButton newButton = row.FindControl("newFileTypeButton") as ImageButton;
                    ImageButton saveButton = row.FindControl("saveInsertFileTypeButton") as ImageButton;
                    ImageButton cancelButton = row.FindControl("cancelInsertFileTypeButton") as ImageButton;
                    newButton.Visible = true;
                    saveButton.Visible = cancelButton.Visible = false;
                    TextBox tbFileTypeName = row.FindControl("tbFileTypeName2") as TextBox;
                    TextBox tbFileTypeShortName = row.FindControl("tbFileTypeShortName2") as TextBox;
                    TextBox tbFileTypeExt = row.FindControl("tbFileTypeExt2") as TextBox;
                    FileType fileType = new FileType();
                    fileType.PortalId = PortalId;
                    fileType.TabModuleId = chkPortalWideRepository.Checked ? 0 : TabModuleId;
                    fileType.FileTypeName = tbFileTypeName.Text;
                    fileType.FileTypeShortName = tbFileTypeShortName.Text;
                    fileType.FileTypeExt = tbFileTypeExt.Text;
                    DocumentController.SaveFileType(fileType);
                    gv_docTypes.DataSource = DocumentController.GetAllFileTypes(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
                    gv_docTypes.DataBind();
                }
            }
            catch (Exception)
            {
            }
        }

        protected void btnReload2_Click(object sender, EventArgs e)
        {
            int oldIndex = ddFileNotifications.SelectedIndex;
            ddFileNotifications.SelectedIndex = -1;
            ddFileNotifications.DataSource = DotNetNuke.Security.Roles.RoleController.Instance.GetRoles(PortalId);
            ddFileNotifications.DataBind();
            ddFileNotifications.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Users", "-1"));
            ddFileNotifications.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Superusers", "-2"));
            ddFileNotifications.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Unauthenticated Users", "-3"));
            if (oldIndex < ddFileNotifications.Items.Count)
            {
                ddFileNotifications.SelectedIndex = oldIndex;
            }
        }

        protected void updateSettings_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                UpdateSettings();
                cancelSettings_Click(sender, e);
            }
        }

        protected void cancelSettings_Click(object sender, EventArgs e)
        {
            Response.Redirect(_navigationManager.NavigateURL());
        }

        protected void chkShowInstructions_CheckedChanged(object sender, EventArgs e)
        {
            pnlInstructions.Visible = chkShowInstructions.Checked;
        }

        protected void chkPortalWideRepository_CheckedChanged(object sender, EventArgs e)
        {
            if (DocumentController.GetAllCategories(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId).Count == 0)
            {
                Category defaultCategory = new Category()
                {
                    CategoryName = "Category 1",
                    PortalId = PortalId,
                    TabModuleId = TabModuleId
                };
                DocumentController.SaveCategory(defaultCategory);
            }
            gv_Categories.DataSource = DocumentController.GetAllCategories(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
            gv_Categories.DataBind();
            if(DocumentController.GetAllFileTypes(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId).Count == 0)
            {
                DocumentController.AddDefaultFileExtensions(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
            }
            gv_docTypes.DataSource = DocumentController.GetAllFileTypes(PortalId, chkPortalWideRepository.Checked ? 0 : TabModuleId);
            gv_docTypes.DataBind();
            pnlName.Visible = !chkPortalWideRepository.Checked;
        }

        protected void chkUseThumbnails_CheckedChanged(object sender, EventArgs e)
        {
            pnlCreateThumbnails.Visible = chkUseThumbnails.Checked;
        }
    }
}