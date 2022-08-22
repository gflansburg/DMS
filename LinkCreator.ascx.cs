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
    public partial class LinkCreator : DMSModuleBase, IActionable
    {
        private readonly INavigationManager _navigationManager;

        public LinkCreator()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
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
                sb.AppendLine("function previewDocument() {");
                //sb.AppendLine("  var url = $('#" + tbLinkURL.ClientID + "');");
                //sb.AppendLine("  var win = window.open(url.val().replace(\"GetDocuments\", \"Preview\"), 'preview', 'width=800,height=600, resize=yes,menubar=yes,status=yes, location=yes,toolbar=yes,scrollbars=yes', true);");
                //sb.AppendLine("  win.focus();");

                //sb.AppendLine("  $(\"#preview-content\").load(url.val().replace(\"GetDocuments\", \"Preview\"), function() {");
                //sb.AppendLine("    var opt = { autoOpen: false,");
                //sb.AppendLine("    bgiframe: true,");
                //sb.AppendLine("    modal: true,");
                //sb.AppendLine("    width: 800,");
                //sb.AppendLine("    height: 500,");
                //sb.AppendLine("    appendTo: 'form',");
                //sb.AppendLine("    dialogClass: 'dialog',");
                //sb.AppendLine("    title: 'Preview Documents',");
                //sb.AppendLine("    closeOnEsacpe: true };");
                //sb.AppendLine("    $('#previewDialog').dialog(opt).dialog('open');");
                //sb.AppendLine("  });");
                sb.AppendLine("    $('#previewDialog').dialog('open');");
                sb.AppendLine("  return false;");
                sb.AppendLine("}");
                sb.AppendLine("function customHeaderKeyPress(event) {");
                sb.AppendLine("  var fileCount = Number($('#" + hidFileCount.ClientID + "').val());");
                sb.AppendLine("  if (fileCount > 0) {");
                sb.AppendLine("    setTimeout(new function() {");
                sb.AppendLine("      var docIds = $('#" + hidDocList.ClientID + "').val();");
                sb.AppendLine("      var fileId = $('#" + hidFileID.ClientID + "').val();");
                sb.AppendLine("      var cb = $('#" + cbShowDescription.ClientID + "').is(':checked');");
                sb.AppendLine("      var header = $('#" + tbCustomHeader.ClientID + "').val();");
                // sb.AppendLine("      $.alert({ content: 'File Count: ' + fileCount + '\\nDoc List: ' + docIds + '\\nFile ID: ' + fileId + '\\nShow Description: ' + cb + '\\nHeader Text: ' + header });");
                // sb.AppendLine("      alert('File Count: ' + fileCount + '\\nDoc List: ' + docIds + '\\nFile ID: ' + fileId + '\\nShow Description: ' + cb + '\\nHeader Text: ' + header);");
                sb.AppendLine("      $.ajax({");
                sb.AppendLine("        url: \"" + ControlPath + "DMSController.asmx/GetLinkUrl\",");
                sb.AppendLine("        type: \"POST\",");
                sb.AppendLine("        dataType: \"json\",");
                string[] strArrays = new string[1];
                int moduleId = base.ModuleId;
                strArrays[0] = string.Concat("mid=", base.ModuleId.ToString());
                string url = GetDocumentsLink(strArrays);
                sb.AppendLine("        data: { showDescription: cb, documentList: docIds, fileId: fileId, headerText: header, path: '" + url + "' },");
                sb.AppendLine("        success: function (result) {");
                sb.AppendLine("          $('#" + tbLinkURL.ClientID + "').val(result);");
                sb.AppendLine("        },");
                sb.AppendLine("        error: function(jqXHR, exception) {");
                sb.AppendLine("          var msg = '';");
                sb.AppendLine("          if (jqXHR.status === 0) {");
                sb.AppendLine("            msg = 'Not connect.\\n Verify Network.';");
                sb.AppendLine("          } else if (jqXHR.status == 404) {");
                sb.AppendLine("            msg = 'Requested page not found. [404]';");
                sb.AppendLine("          } else if (jqXHR.status == 500) {");
                sb.AppendLine("            msg = 'Internal Server Error [500].';");
                sb.AppendLine("          } else if (exception === 'parsererror') {");
                sb.AppendLine("            msg = 'Requested JSON parse failed.';");
                sb.AppendLine("          } else if (exception === 'timeout') {");
                sb.AppendLine("            msg = 'Time out error.';");
                sb.AppendLine("          } else if (exception === 'abort') {");
                sb.AppendLine("            msg = 'Ajax request aborted.';");
                sb.AppendLine("          } else {");
                sb.AppendLine("            msg = 'Uncaught Error.\\n' + jqXHR.responseText;");
                sb.AppendLine("          }");
                sb.AppendLine("          $.alert({ title: 'Error', content: msg });");
                //sb.AppendLine("          alert(msg);");
                sb.AppendLine("        }");
                sb.AppendLine("      });");
                sb.AppendLine("    }, 10);");
                sb.AppendLine("  }");
                sb.AppendLine("  return true;");
                sb.AppendLine("}");
                sb.AppendLine("function initCustomHeaderKeyDown() {");
                sb.AppendLine("  $('input#" + tbCustomHeader.ClientID + "').bind('keyup', function(event) { customHeaderKeyPress(event); } );");
                sb.AppendLine("}");
                sb.AppendLine("function MyEndRequest(sender, args) {");
                sb.AppendLine("  initCustomHeaderKeyDown();");
                sb.AppendLine("  initDialogJavascript();");
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
                sb.AppendLine("MM_preloadImages('" + ResolveUrl("~" + ControlPath + "Images/Icons/DeleteIcon2.gif") + "');");
                sb.AppendLine("$(document).ready(function() {");
                sb.AppendLine("  var prm = Sys.WebForms.PageRequestManager.getInstance();");
                sb.AppendLine("  prm.add_endRequest(MyEndRequest);");
                sb.AppendLine("  initCustomHeaderKeyDown();");
                sb.AppendLine("  initDialogJavascript();");
                sb.AppendLine("});");
                sb.AppendLine("function initDialogJavascript() {");
                sb.AppendLine("  $('a[href^=mailto]').on('click', function() {");
                sb.AppendLine("    ignore_onbeforeunload = true;");
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
                sb.AppendLine("  $('#" + tbLinkURL.ClientID + "').on('focus', function (e) {");
                sb.AppendLine("    $(this).select();");
                sb.AppendLine("  });");
                sb.AppendLine("  var preview = Number($('#" + preview.ClientID + "').val());");
                sb.AppendLine("  if(preview)");
                sb.AppendLine("    previewDocument();");
                sb.AppendLine("}");
                literal.InnerHtml = sb.ToString();
                this.Page.Header.Controls.Add(literal);
            }
        }

        private string GetDocumentsLink(string[] strArrays)
        {
            string url = _navigationManager.NavigateURL("GetDocuments", strArrays);
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && ForceHttps)
            {
                url = string.Format("https://{0}", url.Substring(7));
            }
            return url;
        }

        public class SearchResult
        {
            public int DocumentID { get; set; }
            public string DocumentName { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsDMSUser())
                {
                    base.Response.Redirect(_navigationManager.NavigateURL(), true);
                }
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
                //btnReset.OnClientClick = "$('#" + tbCustomHeader.ClientID + "').val(''); return true;";
                if (!IsPostBack)
                {
                    btnAddDocument.Text = LocalizeString(btnAddDocument.ID);
                    btnBack.Text = LocalizeString(btnBack.ID);
                    btnPreview.Text = LocalizeString(btnPreview.ID);
                    btnReset.Text = LocalizeString(btnReset.ID);
                    gv.EmptyDataText = LocalizeString("NoDocuments");
                    pnlIncludePrivate.Visible = IsAdmin();
                    gv.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#" + Theme);
                    litCSS.Text = "<style type=\"text/css\">" + Generic.ToggleButtonCssString(LocalizeString("No"), LocalizeString("Yes"), new Unit("100px"), System.Drawing.ColorTranslator.FromHtml("#" + Theme)) + "</style>";
                    List<Components.DocumentView> docs = Components.DocumentController.GetAllDocumentsForDropDown(PortalId, PortalWideRepository ? 0 : TabModuleId);
                    List<SearchResult> results = (from doc in docs select new SearchResult { DocumentID = doc.DocumentId, DocumentName = doc.DocumentName }).ToList();
                    ddDocuments.DataSource = results;
                    ddDocuments.DataBind();
                    ddDocuments.Items.Insert(0, new ListItem("-- Select A Document --", "0"));
                    ddDocuments.SelectedIndex = 0;
                    SelectedDocuments = new List<PacketDocument>();
                    gv.DataSource = SelectedDocuments;
                    gv.DataBind();
                }
                else
                {
                    SaveSelectedDocuments();
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

        private List<PacketDocument> SelectedDocuments
        {
            get
            {
                if (ViewState["SelectedDocuments"] == null)
                {
                    ViewState["SelectedDocuments"] = new List<Document>();
                }
                return (List<PacketDocument>)ViewState["SelectedDocuments"];
            }
            set
            {
                ViewState["SelectedDocuments"] = value;
            }
        }

        private DMSFile GetFirstActiveFile
        {
            get
            {
                foreach (GridViewRow row in gv.Rows)
                {
                    Document doc = DocumentController.GetDocument((int)gv.DataKeys[row.DataItemIndex].Values[1]);
                    if ((!doc.ActivationDate.HasValue || DateTime.Now >= doc.ActivationDate) && (!doc.ExpirationDate.HasValue || DateTime.Now <= doc.ExpirationDate) && doc.Files.FindAll(p => p.StatusId == 1).Count > 0)
                    {
                        DropDownList ddFileType = (DropDownList)row.FindControl("ddFileType");
                        if (ddFileType != null)
                        {
                            if (ddFileType.SelectedIndex == 0)
                            {
                                return doc.Files.Find(p => p.StatusId == 1);
                            }
                            else
                            {
                                return doc.Files.Find(p => p.FileId == Convert.ToInt32(ddFileType.SelectedValue));
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
                    List<PacketDocument> docs = SelectedDocuments.FindAll(p => (!p.Document.ActivationDate.HasValue || DateTime.Now > p.Document.ActivationDate.Value) && (!p.Document.ExpirationDate.HasValue || DateTime.Now <= p.Document.ExpirationDate.Value));
                    foreach (PacketDocument doc in docs)
                    {
                        int rowCount = doc.Document.Files.FindAll(p => p.StatusId == 1).Count;
                        if (rowCount == fileCount)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public string GetLinkUrl(bool showDescription, string documentList, int fileId, string headerText)
        {
            string[] strArrays = new string[2];
            int moduleId = base.ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            strArrays[1] = string.Concat("q=", Generic.StringToHex(Generic.UrlEncode(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(String.Format("descriptions={0}&docids={1}&fileid={2}&headertext={3}", showDescription.ToString(), documentList, fileId, HttpUtility.UrlEncode(headerText))))));
            return GetDocumentsLink(strArrays);
        }

        public string GetPreviewUrl(bool showDescription, string documentList, int fileId, string headerText)
        {
            string[] strArrays = new string[2];
            int moduleId = base.ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            strArrays[1] = string.Concat("q=", Generic.StringToHex(Generic.UrlEncode(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(String.Format("descriptions={0}&docids={1}&fileid={2}&headertext={3}&preview=true", showDescription.ToString(), documentList, fileId, HttpUtility.UrlEncode(headerText))))));
            return GetDocumentsLink(strArrays);
        }

        private void SetLinkUrl()
        {
            SaveSelectedDocuments();
            pnlLink.Visible = (SelectedDocuments.Count > 0);
            gv.Columns[1].Visible = (GetFileCount > 1 && FilesOnSingleRow);
            string strDocList = String.Empty;
            int fileId = 0;
            foreach (PacketDocument doc in SelectedDocuments)
            {
                if (!String.IsNullOrEmpty(strDocList))
                {
                    strDocList += ",";
                }
                if (doc.FileId > 0)
                {
                    fileId = doc.FileId;
                }
                strDocList += doc.DocumentId.ToString();
            }
            if (GetActualFileCount != 1)
            {
                fileId = 0;
            }
            tbLinkURL.Text = GetLinkUrl(cbShowDescription.Checked, strDocList, fileId, tbCustomHeader.Text);
            hidDocList.Value = strDocList;
            hidFileID.Value = fileId.ToString();
            hidBaseUrl.Value = GetUrl();
            previewUrl.Value = GetPreviewUrl(cbShowDescription.Checked, strDocList, fileId, tbCustomHeader.Text);
            hidFileCount.Value = GetActualFileCount.ToString();
        }

        protected string GetUrl()
        {
            string strDocList = String.Empty;
            int fileId = 0;
            foreach (PacketDocument doc in SelectedDocuments)
            {
                if (!String.IsNullOrEmpty(strDocList))
                {
                    strDocList += ",";
                }
                if (doc.FileId > 0)
                {
                    fileId = doc.FileId;
                }
                strDocList += doc.DocumentId.ToString();
            }
            if (GetActualFileCount != 1)
            {
                fileId = 0;
            }
            string[] strArrays = new string[2];
            int moduleId = base.ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            strArrays[1] = string.Concat("q=", Generic.StringToHex(Generic.UrlEncode(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(String.Format("descriptions={0}&docids={1}&fileid={2}&searchprivate={3}&headertext=", cbShowDescription.Checked ? "Yes" : "No", strDocList, fileId, cbIncludePrivate.Checked ? "Yes" : "No")))));
            return GetDocumentsLink(strArrays);
        }

        protected int GetFileCount
        {
            get
            {
                int count = 0;
                List<PacketDocument> docs = SelectedDocuments.FindAll(p => (!p.Document.ActivationDate.HasValue || DateTime.Now > p.Document.ActivationDate.Value) && (!p.Document.ExpirationDate.HasValue || DateTime.Now <= p.Document.ExpirationDate.Value));
                foreach (PacketDocument doc in docs)
                {
                    count += doc.Document.Files.FindAll(p => p.StatusId == 1).Count;
                }
                return count;
            }
        }

        protected int GetActualFileCount
        {
            get
            {
                int count = 0;
                List<PacketDocument> docs = SelectedDocuments.FindAll(p => (!p.Document.ActivationDate.HasValue || DateTime.Now > p.Document.ActivationDate.Value) && (!p.Document.ExpirationDate.HasValue || DateTime.Now <= p.Document.ExpirationDate.Value));
                foreach (PacketDocument doc in docs)
                {
                    count += doc.Document.Files.FindAll(p => p.StatusId == 1 && (doc.FileId == 0 || p.FileId == doc.FileId)).Count;
                }
                return count;
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            base.Response.Redirect(_navigationManager.NavigateURL(), true);
        }

        protected void btnAddDocument_Click(object sender, EventArgs e)
        {
            Document doc = DocumentController.GetDocument(Convert.ToInt32(ddDocuments.SelectedValue));
            if (doc.DocumentId > 0 && SelectedDocuments.Find(p => p.DocumentId == doc.DocumentId) == null)
            {
                SelectedDocuments.Add(new PacketDocument(doc, 0));
                gv.DataSource = SelectedDocuments;
                gv.DataBind();
                SetLinkUrl();
            }
            ddDocuments.SelectedIndex = 0;
        }

        protected void gv_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int documentID = (int)gv.DataKeys[e.RowIndex].Values[1];
            SelectedDocuments.Remove(SelectedDocuments.Find(p => p.DocumentId == documentID));
            gv.DataSource = SelectedDocuments;
            gv.DataBind();
            SetLinkUrl();
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                PacketDocument doc = (PacketDocument)e.Row.DataItem;
                ImageButton deleteButton = (ImageButton)e.Row.FindControl("deleteButton");
                if (deleteButton != null)
                {
                    deleteButton.Attributes.Add("onMouseOver", "MM_swapImage('" + deleteButton.ClientID + "','','" + ResolveUrl("~" + ControlPath + "Images/Icons/DeleteIcon2.gif") + "',1)");
                }
                DropDownList ddFileType = (DropDownList)e.Row.FindControl("ddFileType");
                if (ddFileType != null)
                {
                    List<DMSFile> files = doc.Document.Files.FindAll(p => p.StatusId == 1);
                    ddFileType.DataSource = files;
                    ddFileType.DataBind();
                    ddFileType.Items.Insert(0, new ListItem("All", "0"));
                    ddFileType.SelectedIndex = 0;
                    gv.Columns[1].Visible = (files.Count > 1 && gv.Rows.Count == 1);
                }
            }
        }

        protected void cbShowDescription_CheckChanged(object sender, EventArgs e)
        {
            SetLinkUrl();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            tbCustomHeader.Text = String.Empty;
            cbShowDescription.Checked = false;
            ddDocuments.SelectedIndex = 0;
            //hidDocumentId.Value = "0";
            //tbDocument.Text = String.Empty;
            SelectedDocuments = new List<PacketDocument>();
            gv.DataSource = SelectedDocuments;
            gv.DataBind();
            SetLinkUrl();
        }

        protected void lnkCustomHeaderPostback_Click(object sender, EventArgs e)
        {
            SetLinkUrl();
        }

        protected void ddFileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetLinkUrl();
        }

        private void SaveSelectedDocuments()
        {
            foreach (GridViewRow row in gv.Rows)
            {
                DropDownList ddFileType = (DropDownList)row.FindControl("ddFileType");
                if (ddFileType != null)
                {
                    int documentID = (int)gv.DataKeys[row.DataItemIndex].Values[1];
                    PacketDocument doc = SelectedDocuments.Find(p => p.DocumentId == documentID);
                    if (doc != null)
                    {
                        doc.FileId = Convert.ToInt32(ddFileType.SelectedValue);
                    }
                }
            }
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            string strDocList = String.Empty;
            int fileId = 0;
            foreach (PacketDocument doc in SelectedDocuments)
            {
                if (!String.IsNullOrEmpty(strDocList))
                {
                    strDocList += ",";
                }
                if (doc.FileId > 0)
                {
                    fileId = doc.FileId;
                }
                strDocList += doc.DocumentId.ToString();
            }
            if (GetActualFileCount != 1)
            {
                fileId = 0;
            }
            documentSearchResults.QueryString = string.Concat("q=", Generic.UrlEncode(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(String.Format("descriptions={0}&docids={1}&fileid={2}&headertext={3}&preview=true", cbShowDescription.Checked.ToString(), strDocList, fileId, HttpUtility.UrlEncode(tbCustomHeader.Text)))));
            preview.Value = "1";
        }

        protected void cbShowDescription_CheckedChanged(object sender, EventArgs e)
        {
            SetLinkUrl();
        }

        protected void cbIncludePrivate_CheckedChanged(object sender, EventArgs e)
        {
            SetLinkUrl();
        }
    }
}