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
using DotNetNuke.Services.Scheduling;
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
    public partial class View : DMSModuleBase, IActionable
    {
        private readonly INavigationManager _navigationManager;
        private bool _filterAdded = false;

        public View()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        public bool Private
        {
            get
            {
                return (ViewState["Private"] != null ? (bool)ViewState["Private"] : false);
            }
            set
            {
                ViewState["Private"] = value;
            }
        }

        /*protected void Page_Init(object sender, EventArgs e)
        {
            if (DotNetNuke.Framework.AJAX.IsInstalled())
            {
                DotNetNuke.Framework.AJAX.RegisterScriptManager();
                TelerikAjaxUtility.InstallRadAjaxManager();
            }
        } */
        
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
            System.Web.UI.HtmlControls.HtmlGenericControl script3 = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("ComponentScriptAutoComplete");
            if (script3 == null)
            {
                script3 = new System.Web.UI.HtmlControls.HtmlGenericControl("script")
                {
                    ID = "ComponentScriptAutoComplete"
                };
                script3.Attributes.Add("language", "javascript");
                script3.Attributes.Add("type", "text/javascript");
                script3.Attributes.Add("src", ControlPath + "Scripts/jquery.auto-complete.js");
                this.Page.Header.Controls.Add(script3);
            }
            System.Web.UI.HtmlControls.HtmlGenericControl script = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("ComponentScriptBookmark");
            if (script == null)
            {
                script = new System.Web.UI.HtmlControls.HtmlGenericControl("script")
                {
                    ID = "ComponentScriptBookmark"
                };
                script.Attributes.Add("language", "javascript");
                script.Attributes.Add("type", "text/javascript");
                script.Attributes.Add("src", ControlPath + "Scripts/add_bookmark.jquery.js");
                this.Page.Header.Controls.Add(script);
            }
            System.Web.UI.HtmlControls.HtmlGenericControl css = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("ComponentStyleAutoComplete");
            if (css == null)
            {
                css = new System.Web.UI.HtmlControls.HtmlGenericControl("link")
                {
                    ID = "ComponentStyleAutoComplete"
                };
                css.Attributes.Add("type", "text/css");
                css.Attributes.Add("rel", "stylesheet");
                css.Attributes.Add("href", ControlPath + "Scripts/jquery.auto-complete.css");
                this.Page.Header.Controls.Add(css);
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
                sb.AppendLine("});");
                sb.AppendLine("function ViewEndRequest(sender, args) {");
                sb.AppendLine("  initViewJavascript();");
                sb.AppendLine("  hideBlockingScreen();");
                sb.AppendLine("}");
                sb.AppendLine("function initViewJavascript() {");
                sb.AppendLine("  $('a[href^=mailto]').on('click', function() {");
                sb.AppendLine("    ignore_onbeforeunload = true;");
                sb.AppendLine("  });");
                sb.AppendLine("  $('#" + tbKeywords.ClientID + "').autoComplete({");
                sb.AppendLine("    source: function(term, response) { $.getJSON('" + ControlPath + "SearchTerms.ashx', { q: term, pid: " + PortalId.ToString() + ", mid: " + TabModuleId.ToString() + ", cid: " + (ddCategory.Items.Count == 1 ? ddCategory.SelectedValue : "$('#" + ddCategory.ClientID + "').val()") + ", cid: " + (ddCategory.Items.Count == 1 ? ddCategory.SelectedValue : "$('#" + ddCategory.ClientID + "').val()") + ", p: " + Private.ToString().ToLower() + ", uid: " + UserId.ToString() + " }, function(data) { response(data); }); },");
                sb.AppendLine("    cache: false,");
                sb.AppendLine("    minChars: 1,");
                sb.AppendLine("    onSelect: function(event, term, item) {");
                sb.AppendLine("      $('#" + tbKeywords.ClientID + "').val(term);");
                //sb.AppendLine("      $('#" + btnSearch.ClientID + "').click();");
                sb.AppendLine("        " + Page.ClientScript.GetPostBackEventReference(btnSearch, String.Empty) + ";");
                sb.AppendLine("    }");
                sb.AppendLine("  }).keyup(function(e) {");
                sb.AppendLine("    if(e.which === 13) {");
                sb.AppendLine("      e.stopImmediatePropagation();");
                sb.AppendLine("      e.preventDefault();");
                sb.AppendLine("      $('#" + tbKeywords.ClientID + "').autoComplete('close');");
                //sb.AppendLine("      $('#" + btnSearch.ClientID + "').click();");
                sb.AppendLine("        " + Page.ClientScript.GetPostBackEventReference(btnSearch, String.Empty) + ";");
                sb.AppendLine("    }");
                sb.AppendLine("  });");
                sb.AppendLine("  $(\".ui-autocomplete\").wrap('<div class=\"dms\" />');");
                sb.AppendLine("  addBookmarkObj.URL = location.href;");
                sb.AppendLine("  addBookmarkObj.addTextLink('addBookmarkContainer');");
                sb.AppendLine("}");
                literal.InnerHtml = sb.ToString();
                this.Page.Header.Controls.Add(literal);
            }
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private void FixThumnails()
        {
            List<Document> docs = DocumentController.GetAllDocuments(PortalId, TabModuleId);
            foreach(Document doc in docs)
            {
                doc.Files = DocumentController.GetAllFilesForDocument(doc.DocumentId);
                foreach (DMSFile file in doc.Files)
                {
                    if (file.FileType.Equals("pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            file.FileVersion.Thumbnail = null;
                            file.FileVersion.SaveThumbnail(false);
                            Generic.CreateThumbnail(this.Request, ControlPath, file);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private void FixTitles()
        {
            List<DocumentView> docs = DocumentController.GetAllDocumentsForTag(17, PortalWideRepository);
            Response.Write(docs.Count() + " documents found.<br />");
            foreach (DocumentView doc in docs)
            {
                List<DMSFile> files = DocumentController.GetAllFilesForDocument(doc.DocumentId);
                foreach (DMSFile file in files)
                {
                    Response.Write(file.Filename + "<br />");
                    if (file.FileType.Equals("pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            Generic.ReplacePDFTitle(file, doc.DocumentName);
                            Response.Write(doc.DocumentName + "<br />");
                        }
                        catch(Exception ex)
                        {
                            Response.Write(ex.Message + "<br />");
                        }
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
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
                if (!IsPostBack)
                {
                    //if (IsAdmin())
                    //{
                        //FixThumnails();
                        //FixTitles();
                    //}
                    searchBox.Style["background"] = string.Format("url({0}Images/results-background-{1}.jpg) no-repeat", ControlPath, Theme);
                    pnlDefault.Visible = ShowTips;
                    lblCategoryName.Text = CategoryName.ToLower();
                    pnlInstructions.Visible = ShowInstructions;
                    lblInstructions.Text = Instructions;
                    ScheduleJob();
                    BindDropDowns();
                    lblCategory.Text = CategoryName;
                    pnlAdmin.Visible = IsDMSUser();
                    settingsCommandButton.Visible = bulkImportCommandButton.Visible = viewPacketsCommandButton.Visible = viewTagsCommandButton.Visible = uploadReportCommandButton.Visible = IsAdmin();
                    Session["search"] = null;
                    bool bSearchVisible = true;
                    bool bMenuOn = (!String.IsNullOrEmpty(Request.QueryString["menu"]) && Generic.IsBoolean(Request.QueryString["menu"]) ? Generic.ToBoolean(Request.QueryString["menu"]) : false);
                    bool bDescriptions = (!String.IsNullOrEmpty(Request.QueryString["descriptions"]) && Generic.IsBoolean(Request.QueryString["descriptions"]) ? Generic.ToBoolean(Request.QueryString["descriptions"]) : true);
                    bool bPrivate = (!String.IsNullOrEmpty(Request.QueryString["searchprivate"]) && Generic.IsBoolean(Request.QueryString["searchprivate"]) ? Generic.ToBoolean(Request.QueryString["searchprivate"]) : false);
                    string strHeaderText = (Request.QueryString["headerText"] ?? String.Empty).Trim();
                    string strKeywords = (Request.QueryString["keywords"] ?? String.Empty).Trim();
                    string strCategory = (Request.QueryString["category"] ?? String.Empty).Trim();
                    string strDocIDs = (Request.QueryString["docids"] ?? String.Empty).Trim();
                    string packetID = (Request.QueryString["p"] ?? String.Empty).Trim();
                    string strQuery = (Request.QueryString["q"] ?? String.Empty).Trim();
                    string strType = (Request.QueryString["type"] ?? String.Empty).Trim();
                    if(strQuery.Equals("[All]"))
                    {
                        strQuery = string.Empty;
                    }
                    if (String.IsNullOrEmpty(strQuery) && !String.IsNullOrEmpty(Request.QueryString["Search"]))
                    {
                        strQuery = Request.QueryString["Search"].Trim();
                    }
                    //strQuery = Generic.StripHTML(strQuery);
                    bool bSearch = (strType.Equals("documents", StringComparison.OrdinalIgnoreCase));
                    string[] aryDocIds = (!String.IsNullOrEmpty(strDocIDs) ? strDocIDs.Split(',') : new string[] {} );
                    if (aryDocIds.Length == 0)
                    {
                        bMenuOn = false;
                    }
                    if (!String.IsNullOrEmpty(packetID) || (!String.IsNullOrEmpty(strQuery) && String.IsNullOrEmpty(strType)))
                    {
                        bSearchVisible = false;
                    }
                    tbKeywords.Text = strKeywords;
                    //rblShowDescription.SelectedIndex = (bDescriptions ? 0 : 1);
                    cbShowDescription.Checked = bDescriptions;
                    documentSearchResults.Header = strHeaderText;
                    Private = bPrivate;
                    if(!string.IsNullOrEmpty(strCategory))
                    {
                        ddCategory.SelectedValue = strCategory;
                    }
                    menuText.Visible = bMenuOn;
                    pnlSearch.Visible = pnlSearchResults.Visible = bSearchVisible;
                    List<Document> docs = new List<Document>();
                    if (aryDocIds.Length > 0)
                    {
                        foreach (string docId in aryDocIds)
                        {
                            if (!String.IsNullOrEmpty(docId) && Generic.IsNumber(docId))
                            {
                                Document doc = DocumentController.GetDocument(Convert.ToInt32(docId));
                                if (doc.DocumentId > 0)
                                {
                                    docs.Add(doc);
                                }
                            }
                        }
                    }
                    if(!String.IsNullOrEmpty(packetID) || !String.IsNullOrEmpty(strQuery))
                    //if(!String.IsNullOrEmpty(Request.QueryString.ToString()))
                    {
                        if (!String.IsNullOrEmpty(packetID))
                        {
                            documentSearchResults.QueryString = String.Format("p={0}", packetID);
                            pnlSearch.Visible = pnlSearchResults.Visible = false;
                        }
                        else if(String.IsNullOrEmpty(strType))
                        {
                            if (!String.IsNullOrEmpty(strQuery))
                            {
                                try
                                {
                                    Gafware.Modules.DMS.Cryptography.CryptographyUtil.Decrypt(strQuery);
                                    documentSearchResults.QueryString = String.Format("q={0}", System.Web.HttpUtility.UrlEncode(strQuery));
                                    pnlSearch.Visible = pnlSearchResults.Visible = false;
                                }
                                catch
                                {
                                    try
                                    {
                                        byte[] aQuery = StringToByteArray(strQuery);
                                        string strNewQuery = System.Web.HttpUtility.UrlDecode(System.Text.Encoding.ASCII.GetString(aQuery));
                                        Gafware.Modules.DMS.Cryptography.CryptographyUtil.Decrypt(strNewQuery);
                                        documentSearchResults.QueryString = String.Format("q={0}", System.Web.HttpUtility.UrlEncode(strNewQuery));
                                        pnlSearch.Visible = pnlSearchResults.Visible = false;
                                    }
                                    catch
                                    {
                                        tbKeywords.Text = strQuery;
                                        Search(true, 0, strQuery);
                                        pnlSearch.Visible = pnlSearchResults.Visible = true;
                                    }
                                }
                            }
                            else
                            {
                                Search(true);
                            }
                        }
                        else if(strType.Equals("documents", StringComparison.OrdinalIgnoreCase))
                        {
                            tbKeywords.Text = strQuery;
                            Search(true);
                            pnlSearch.Visible = pnlSearchResults.Visible = true;
                        }
                        foreach (PacketDocument doc in documentSearchResults.SelectedDocuments)
                        {
                            if (Generic.UserHasAccess(doc.Document))
                            {
                                docs.Add(doc.Document);
                            }
                        }
                    }
                    else if (String.IsNullOrEmpty(strQuery) && String.IsNullOrEmpty(strType) && Request.QueryString["q"] != null)
                    {
                        Search(true);
                    }
                    if (docs.Count > 0)
                    {
                        if (!_filterAdded)
                        {
                            AddFilter(docs, strQuery);
                        }
                        SetSearchText();
                        documentSearchResults.IsLink = !bSearch;
                        pnlSearchResults.Visible = (docs.Count > 1);
                    }
                    else if (!String.IsNullOrEmpty(tbKeywords.Text))
                    {
                        Search(true);
                    }
                    litCSS.Text = "<style type=\"text/css\">" + Generic.ToggleButtonCssString(System.Drawing.ColorTranslator.FromHtml("#" + Theme)) + "</style>";
                }
                else
                {
                    documentSearchResults.IsLink = false;
                }
                pnlMain.Visible = !documentSearchResults.IsPreview;
                if(documentSearchResults.IsPreview)
                {
                    pnlSearchResults.Visible = false;
                    pnlResults.Visible = true;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindDropDowns()
        {
            List<Category> categories = DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId);
            List<Category> filteredCategories = new List<Category>();
            foreach(Category category in categories)
            {
                DotNetNuke.Security.Roles.RoleInfo categoryRole = UserController.GetRoleById(PortalId, category.RoleId);
                if (categoryRole != null)
                {
                    if (UserInfo.IsInRole(categoryRole.RoleName))
                    {
                        filteredCategories.Add(category);
                    }
                }
            }
            ddCategory.DataSource = filteredCategories;
            ddCategory.DataBind();
            ddCategory.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All", "0"));
            ddCategory.SelectedIndex = 0;
            pnlCategory.Visible = pnlCategoryTip.Visible = filteredCategories.Count > 1;
        }

        public void Search(bool bReload)
        {
            Search(bReload, Convert.ToInt32(ddCategory.SelectedValue), tbKeywords.Text.Trim());
        }

        public void Search(bool bReload, int categoryID, string searchTerms)
        {
            List<Document> docs = (List<Document>)Session["search"];
            if (docs == null || bReload)
            {
                SetSearchText();
                docs = DocumentController.Search(categoryID, searchTerms, Private, PortalId, PortalWideRepository ? 0 : TabModuleId, UserId);
            }
            AddFilter(docs, searchTerms);
        }

        private void SetSearchText()
        {
            if(ddCategory.SelectedIndex > 0)
            {
                if (String.IsNullOrWhiteSpace(tbKeywords.Text.Trim()))
                {
                    litSearch.Text = String.Format("Searching {0}.", ddCategory.SelectedItem.Text);
                }
                else
                {
                    litSearch.Text = String.Format("Searching {0} for \"<strong>{1}</strong>\".", ddCategory.SelectedItem.Text, System.Web.HttpUtility.HtmlEncode(tbKeywords.Text.Trim()));
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(tbKeywords.Text.Trim()))
                {
                    litSearch.Text = "Searching all locations";
                }
                else
                {
                    litSearch.Text = String.Format("Searching all locations for \"<strong>{0}</strong>\".", System.Web.HttpUtility.HtmlEncode(tbKeywords.Text.Trim()));
                }
            }
            litSearch.Text += " Documents will open in a new window.";
        }

        private void AddFilter(List<Document> docs, string searchTerms)
        {
            List<Document> filter = new List<Document>(docs);
            documentSearchResults.SelectedDocuments = new List<PacketDocument>();
            foreach (Document doc in filter)
            {
                /*foreach (DocumentCategory docCat in doc.Categories)
                {
                    if (UserInfo.IsInRole(docCat.Category.RoleName))
                    {
                        documentSearchResults.SelectedDocuments.Add(new PacketDocument(doc, 0));
                        break;
                    }
                }*/
                documentSearchResults.SelectedDocuments.Add(new PacketDocument(doc, 0));
            }
            documentSearchResults.Keywords = searchTerms;
            documentSearchResults.BindData();
            Session["search"] = docs;
            pnlDefault.Visible = false;
            pnlResults.Visible = true;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            /*if (Request.QueryString["type"] == null && String.IsNullOrEmpty(tbKeywords.Text))
            {
                PropertyInfo isreadonly = typeof(System.Collections.Specialized.NameValueCollection).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
                // make collection editable
                isreadonly.SetValue(this.Request.QueryString, false, null);
                // remove
                this.Request.QueryString.Remove("q");
                Search(true);
            }
            else*/
            {
                string url = TabController.CurrentPage.FullUrl + "?q=" + System.Web.HttpUtility.UrlEncode(string.IsNullOrEmpty(tbKeywords.Text.Trim()) && PortalSettings.ActiveTab.TabName.Equals("Home", StringComparison.OrdinalIgnoreCase) ? "[All]" : tbKeywords.Text.Trim()) + (!String.IsNullOrEmpty(hidTab.Value) ? "#" + hidTab.Value : String.Empty);
                Response.Redirect(url, true); // + (Request.QueryString["type"] != null ? "&type=" + Request.QueryString["type"] : String.Empty));
            }
        }

        protected void ddCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            Search(true);
        }

        //protected void rblShowDescription_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    documentSearchResults.ShowDescription = (rblShowDescription.SelectedIndex == 0);
        //    documentSearchResults.BindData();
        //}

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection
                {
                    {
                        GetNextActionID(), Localization.GetString("DocumentList", LocalResourceFile), "", "", ControlPath + "Images/dms.png",
                        EditUrl("DocumentList"), false, SecurityAccessLevel.Edit, true, false
                    },
                    {
                        GetNextActionID(), Localization.GetString("PacketList", LocalResourceFile), "", "", ControlPath + "Images/dms.png",
                        EditUrl("PacketList"), false, SecurityAccessLevel.Admin, true, false
                    },
                    {
                        GetNextActionID(), Localization.GetString("TagList", LocalResourceFile), "", "", ControlPath + "Images/dms.png",
                        EditUrl("TagList"), false, SecurityAccessLevel.Admin, true, false
                    },
                    {
                        GetNextActionID(), Localization.GetString("LinkCreator", LocalResourceFile), "", "", ControlPath + "Images/dms.png",
                        EditUrl("LinkCreator"), false, SecurityAccessLevel.Edit, true, false
                    },
                    {
                        GetNextActionID(), Localization.GetString("UploadReport", LocalResourceFile), "", "", ControlPath + "Images/dms.png",
                        EditUrl("UploadReport"), false, SecurityAccessLevel.Admin, true, false
                    },
                    {
                        GetNextActionID(), Localization.GetString("BulkImport", LocalResourceFile), "", "", ControlPath + "Images/dms.png",
                        EditUrl("BulkImport"), false, SecurityAccessLevel.Admin, true, false
                    },
                    {
                        GetNextActionID(), Localization.GetString("Settings", LocalResourceFile), "", "", ControlPath + "Images/dms.png",
                        EditUrl("EditSettings"), false, SecurityAccessLevel.Admin, true, false
                    }
                };
                return actions;
            }
        }

        protected void ViewDocumentsCommandButtonClicked(object sender, EventArgs e)
        {
            HttpResponse response = base.Response;
            string[] strArrays = new string[1];
            int moduleId = base.ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            response.Redirect(_navigationManager.NavigateURL("DocumentList", strArrays));
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

        protected void viewPacketsCommandButton_Click(object sender, EventArgs e)
        {
            HttpResponse response = base.Response;
            string[] strArrays = new string[1];
            int moduleId = base.ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            response.Redirect(_navigationManager.NavigateURL("PacketList", strArrays));
        }

        protected void viewTagsCommandButton_Click(object sender, EventArgs e)
        {
            HttpResponse response = base.Response;
            string[] strArrays = new string[1];
            int moduleId = base.ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            response.Redirect(_navigationManager.NavigateURL("TagList", strArrays));
        }

        protected void linkCreatorCommandButton_Click(object sender, EventArgs e)
        {
            HttpResponse response = base.Response;
            string[] strArrays = new string[1];
            int moduleId = base.ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            response.Redirect(_navigationManager.NavigateURL("LinkCreator", strArrays));
        }

        protected void uploadReportCommandButton_Click(object sender, EventArgs e)
        {
            HttpResponse response = base.Response;
            string[] strArrays = new string[1];
            int moduleId = base.ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            response.Redirect(_navigationManager.NavigateURL("UploadReport", strArrays));
        }

        protected void ScheduleJob()
        {
            ScheduleItem item = SchedulingProvider.Instance().GetSchedule(DMSJob.AssemblyName, String.Empty);
            if (item == null)
            {
                item = new ScheduleItem();
                item.CatchUpEnabled = false;
                item.Enabled = true;
                item.ScheduleStartDate = DateTime.Parse(DateTime.Now.AddDays(1).ToString("MM/dd/yyyy"));
                item.NextStart = item.ScheduleStartDate;
                item.RetainHistoryNum = 60;
                item.RetryTimeLapse = 1;
                item.RetryTimeLapseMeasurement = "h";
                item.TimeLapse = 1;
                item.TimeLapseMeasurement = "d";
                item.ScheduleSource = ScheduleSource.NOT_SET;
                item.TypeFullName = DMSJob.AssemblyName;
                item.FriendlyName = "DMS: Activate/Deactivate Documents";
                SchedulingProvider.Instance().AddSchedule(item);
            }
        }

        protected void cbShowDescription_CheckedChanged(object sender, EventArgs e)
        {
            documentSearchResults.ShowDescription = cbShowDescription.Checked;
            documentSearchResults.BindData();
        }

        protected void settingsCommandButton_Click(object sender, EventArgs e)
        {
            HttpResponse response = base.Response;
            string[] strArrays = new string[1];
            int moduleId = base.ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            response.Redirect(_navigationManager.NavigateURL("EditSettings", strArrays));
        }

        protected void bulkImportCommandButton_Click(object sender, EventArgs e)
        {
            HttpResponse response = base.Response;
            string[] strArrays = new string[1];
            int moduleId = base.ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            response.Redirect(_navigationManager.NavigateURL("BulkImport", strArrays));
        }
    }
}