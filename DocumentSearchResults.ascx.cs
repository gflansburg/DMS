using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Abstractions;
using DotNetNuke.Services.Localization;
using Gafware.Modules.DMS.Components;
using Gafware.Modules.DMS.Data;

namespace Gafware.Modules.DMS
{
    public partial class DocumentSearchResults : System.Web.UI.UserControl
    {
        private INavigationManager _navigationManager;
        private string _localResourceFile;

        private List<Category> _poralCategories = null;
        protected List<Category> PortalCategories
        {
            get
            {
                if (_poralCategories == null)
                {
                    _poralCategories = Components.DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId);
                }
                return _poralCategories;
            }
        }

        public INavigationManager NavigationManager
        {
            get
            {
                return _navigationManager;
            }
            set
            {
                _navigationManager = value;
            }
        }

        public bool ShowAdmin
        {
            get
            {
                return (ViewState["ShowAdmin"] != null ? (bool)ViewState["ShowAdmin"] : false);
            }
            set
            {
                ViewState["ShowAdmin"] = value;
            }
        }

        public bool IsAdmin
        {
            get
            {
                return (ViewState["IsAdmin"] != null ? (bool)ViewState["IsAdmin"] : false);
            }
            set
            {
                ViewState["IsAdmin"] = value;
            }
        }

        public string Title
        {
            get
            {
                return (ViewState["Title"] != null ? ViewState["Title"].ToString() : String.Empty);
            }
            set
            {
                ViewState["Title"] = value;
            }
        }

        public string CategoryName
        {
            get
            {
                return (ViewState["CategoryName"] != null ? ViewState["CategoryName"].ToString() : "Category");
            }
            set
            {
                ViewState["CategoryName"] = value;
            }
        }

        public string Theme
        {
            get
            {
                return (ViewState["Theme"] != null ? ViewState["Theme"].ToString() : "990000");
            }
            set
            {
                ViewState["Theme"] = value;
            }
        }

        public string ThumbnailType
        {
            get
            {
                return (ViewState["ThumbnailType"] != null ? ViewState["ThumbnailType"].ToString() : "classic");
            }
            set
            {
                ViewState["ThumbnailType"] = value;
            }
        }

        public int ThumbnailLandscapeHeight
        {
            get
            {
                return (int)((97 * ThumbnailSize) / 128);
            }
        }

        public int ThumbnailSize
        {
            get
            {
                return (ViewState["ThumbnailSize"] != null ? (int)ViewState["ThumbnailSize"] : 128);
            }
            set
            {
                ViewState["ThumbnailSize"] = value;
            }
        }

        public int PageSize
        {
            get
            {
                return (ViewState["PageSize"] != null ? (int)ViewState["PageSize"] : 20);
            }
            set
            {
                ViewState["PageSize"] = value;
            }
        }

        public bool UseLocalFile
        {
            get
            {
                return (ViewState["UseLocalFile"] != null ? (bool)ViewState["UseLocalFile"] : false);
            }
            set
            {
                ViewState["UseLocalFile"] = value;
            }
        }

        public string ControlPath
        {
            get
            {
                return (ViewState["ControlPath"] != null ? ViewState["ControlPath"].ToString() : String.Empty);
            }
            set
            {
                if (!(ViewState["ControlPath"] ?? String.Empty).Equals(value ?? String.Empty))
                {
                    ViewState["ControlPath"] = value;
                }
            }
        }

        public int PortalId
        {
            get
            {
                return (ViewState["PortalId"] != null ? (int)ViewState["PortalId"] : 0);
            }
            set
            {
                ViewState["PortalId"] = value;
            }
        }

        public int TabModuleId
        {
            get
            {
                return (ViewState["TabPortalId"] != null ? (int)ViewState["TabPortalId"] : 0);
            }
            set
            {
                ViewState["TabPortalId"] = value;
            }
        }

        public int UserId
        {
            get
            {
                return (ViewState["UserId"] != null ? (int)ViewState["UserId"] : 0);
            }
            set
            {
                ViewState["UserId"] = value;
            }
        }

        public bool PortalWideRepository
        {
            get
            {
                return (ViewState["PortalWideRepository"] != null ? (bool)ViewState["PortalWideRepository"] : true);
            }
            set
            {
                ViewState["PortalWideRepository"] = value;
            }
        }

        public int ModuleId
        {
            get
            {
                return (ViewState["ModuleId"] != null ? (int)ViewState["ModuleId"] : 0);
            }
            set
            {
                ViewState["ModuleId"] = value;
            }
        }

        public bool IsPreview
        {
            get
            {
                return (ViewState["IsPreview"] != null ? (bool)ViewState["IsPreview"] : false);
            }
        }

        public bool IsLink
        {
            get
            {
                return (ViewState["IsLink"] != null ? (bool)ViewState["IsLink"] : true);
            }
            set
            {
                ViewState["IsLink"] = value;
            }
        }

        public bool Search
        {
            get
            {
                return (ViewState["Search"] != null ? (bool)ViewState["Search"] : false);
            }
            set
            {
                ViewState["Search"] = value;
            }
        }

        public List<PacketDocument> SelectedDocuments
        {
            get
            {
                if (ViewState["SelectedDocuments"] == null)
                {
                    ViewState["SelectedDocuments"] = new List<PacketDocument>();
                }
                return (List<PacketDocument>)ViewState["SelectedDocuments"];
            }
            set
            {
                ViewState["SelectedDocuments"] = value;
                BindData();
            }
        }

        public string QueryString
        {
            get
            {
                return (ViewState["QueryString"] != null ? ViewState["QueryString"].ToString() : String.Empty);
            }
            set
            {
                if (!(ViewState["QueryString"] ?? String.Empty).Equals(value ?? String.Empty))
                {
                    ViewState["QueryString"] = value;
                }
                ParseQueryString();
            }
        }

        public string Header
        {
            get
            {
                return lblHeader.Text;
            }
            set
            {
                lblHeader.Text = (String.IsNullOrEmpty(value) ? LocalizeString("ResultsHeader") : value);
            }
        }

        public bool ShowDescription
        {
            get
            {
                return (ViewState["Description"] != null ? (bool)ViewState["Description"] : true);
            }
            set
            {
                ViewState["Description"] = value;
            }
        }

        public string Keywords
        {
            get
            {
                return (ViewState["Keywords"] != null ? ViewState["Keywords"].ToString() : String.Empty);
            }
            set
            {
                ViewState["Keywords"] = value;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("function recordDocumentRequest(strQueryString) {");
            sb.AppendLine("  $.ajax({");
            sb.AppendLine("    type: \"POST\",");
            sb.AppendLine("    dataType: \"json\",");
            sb.AppendLine("    url: \"" + ControlPath + "DMSController.asmx/RecordDocumentRequest\",");
            sb.AppendLine("    data: { q: strQueryString, pid: " + PortalId.ToString() + " },");
            sb.AppendLine("    async: true,");
            sb.AppendLine("    success: function () {");
            sb.AppendLine("    }");
            sb.AppendLine("  });");
            sb.AppendLine("}");
            Page.ClientScript.RegisterStartupScript(GetType(), "RecordDocumentRequest", sb.ToString(), true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlAdmin.Visible = ShowAdmin && IsAdmin;
                rptDocuments.PageSize = (PageSize == 0 ? 10 : PageSize);
                if(PageSize == 0)
                {
                    rptDocuments.AllowPaging = false;
                }
                lnkFileLocation.ForeColor = System.Drawing.ColorTranslator.FromHtml("#" + Theme);
                SearchResultHeader.Style["background"] = SearchResultHeader2.Style["background"] = String.Format("url({0}Images/category-header-{1}.png) no-repeat;", ControlPath, Theme);
                Label1.Style["color"] = lblHeader.Style["color"] = "white !important";
                BindData();
            }
            else
            {
                Generic.ApplyPaging(rptDocuments);
            }
        }

        private void ParseQueryString()
        {
            string strQueryString = (String.IsNullOrEmpty(QueryString) ? Request.QueryString.ToString() : QueryString);
            System.Collections.Specialized.NameValueCollection aryQueryString = HttpUtility.ParseQueryString(strQueryString);
            if (!String.IsNullOrEmpty(aryQueryString["q"]))
            {
                try
                {
                    string query = Gafware.Modules.DMS.Cryptography.CryptographyUtil.Decrypt(aryQueryString["q"]);
                    System.Collections.Specialized.NameValueCollection queryString = HttpUtility.ParseQueryString(query);
                    if (!String.IsNullOrEmpty(queryString["descriptions"]) && Generic.IsBoolean(queryString["descriptions"]))
                    {
                        ShowDescription = Generic.ToBoolean(queryString["descriptions"]);
                    }
                    if (!String.IsNullOrEmpty(queryString["headertext"]))
                    {
                        Header = queryString["headertext"];
                    }
                    if (!String.IsNullOrEmpty(queryString["preview"]))
                    {
                        ViewState["IsPreview"] = Generic.ToBoolean(queryString["preview"]);
                    }
                    int fileId = 0;
                    if (!String.IsNullOrEmpty(queryString["fileid"]) && Generic.IsNumber(queryString["fileid"]))
                    {
                        fileId = Convert.ToInt32(queryString["fileid"]);
                    }
                    if (!String.IsNullOrEmpty(queryString["docids"]))
                    {
                        string[] docids = queryString["docids"].Split(',');
                        List<PacketDocument> selectedDocuments = new List<PacketDocument>();
                        foreach (string docid in docids)
                        {
                            Document doc = DocumentController.GetDocument(Convert.ToInt32(docid));
                            if (Generic.UserHasAccess(doc))
                            {
                                if (doc.Files.FindAll(p => p.StatusId == 1).Count > 0)
                                {
                                    PacketDocument packetDoc = new PacketDocument(doc, 0);
                                    if (fileId > 0)
                                    {
                                        if (packetDoc.Document.Files.Find(p => p.FileId == fileId) != null)
                                        {
                                            packetDoc.FileId = fileId;
                                        }
                                    }
                                    selectedDocuments.Add(packetDoc);
                                }
                            }
                        }
                        SelectedDocuments = selectedDocuments;
                    }
                    else
                    {
                        pnlError.Visible = true;
                    }
                }
                catch
                {
                    pnlError.Visible = true;
                }
            }
            else if (!String.IsNullOrEmpty(aryQueryString["p"]))
            {
                try
                {
                    Packet packet = PacketController.GetPacketByName(aryQueryString["p"], PortalId, PortalWideRepository ? 0 : TabModuleId);
                    if (packet != null)
                    {
                        pnlDescription.Visible = !String.IsNullOrEmpty(packet.Description) && packet.ShowPacketDescription;
                        lblDescription.Text = packet.Description;
                        ShowDescription = packet.ShowDescription;
                        Header = packet.CustomHeader;
                        SelectedDocuments = PacketController.GetAllDocumentsForPacket(packet.PacketId, UserId);
                        foreach (PacketTag tag in packet.Tags)
                        {
                            List<Document> docs = DocumentController.GetDocumentsForTag(tag.TagId, PortalId, PortalWideRepository ? 0 : TabModuleId, UserId);
                            foreach (Document doc in docs)
                            {
                                if (SelectedDocuments.Find(p => p.Document.DocumentId == doc.DocumentId) == null)
                                {
                                    PacketDocument packetDoc = new PacketDocument(doc, packet.PacketId);
                                    SelectedDocuments.Add(packetDoc);
                                }
                            }
                        }
                        BindData();
                    }
                    else
                    {
                        pnlSearchNotFound.Visible = true;
                        Title = "Document Packet Not Found";
                    }
                }
                catch
                {
                    pnlError.Visible = true;
                }
            }
            else
            {
                string documentID = (aryQueryString["id"] ?? String.Empty).Trim();
                string strType = (aryQueryString["type"] ?? String.Empty).Trim();
                string strTarget = (aryQueryString["target"] ?? String.Empty).Trim();
                string strSearchTerms = (aryQueryString["terms"] ?? String.Empty).Trim();
                if (!String.IsNullOrEmpty(documentID) && Generic.IsNumber(documentID))
                {
                    lnkFileLocation.Target = (!String.IsNullOrEmpty(strTarget) ? strTarget : "_blank");
                    Keywords = strSearchTerms;
                    SelectedDocuments = new List<PacketDocument>();
                    Document doc = DocumentController.GetDocument(Convert.ToInt32(documentID));
                    if (doc.DocumentId > 0)
                    {
                        if (Generic.UserHasAccess(doc))
                        {
                            PacketDocument packet = new PacketDocument(doc, 0);
                            if (!String.IsNullOrEmpty(strType))
                            {
                                DMSFile file = doc.Files.Find(p => p.StatusId == 1 && p.FileType.Equals(strType, StringComparison.OrdinalIgnoreCase));
                                if (file != null)
                                {
                                    packet.FileId = file.FileId;
                                }
                            }
                            SelectedDocuments.Add(packet);
                        }
                    }
                    BindData();
                }
                else
                {
                    pnlError.Visible = true;
                }
            }
        }

        public void BindData()
        {
            pnlError.Visible = false;
            pnlDocumentFound.Visible = false;
            pnlDocumentsFound.Visible = false;
            pnlDocumentNotFound.Visible = false;
            pnlSearchNotFound.Visible = false;
            DotNetNuke.Entities.Portals.PortalSettings portal = DotNetNuke.Entities.Portals.PortalSettings.Current;
            if (GetFileCount == 1 && IsLink)
            {
                DMSFile file = GetFirstActiveFile(SelectedDocuments.FindAll(p => (!p.Document.ActivationDate.HasValue || DateTime.Now >= p.Document.ActivationDate.Value) && (!p.Document.ExpirationDate.HasValue || DateTime.Now <= (p.Document.ExpirationDate.Value + new TimeSpan(23, 59, 59)))));
                if(file.FileType.Equals("url", StringComparison.OrdinalIgnoreCase))
                {
                    pnlDocumentFound.Visible = true;
                    lnkFileLocation.NavigateUrl = lnkFileLocation.Text = file.WebPageUrl;
                    System.Web.UI.HtmlControls.HtmlGenericControl literal = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("MetaDocumentControl");
                    if (literal == null)
                    {
                        literal = new System.Web.UI.HtmlControls.HtmlGenericControl("meta")
                        {
                            ID = "MetaDocumentControl"
                        };
                        literal.Attributes.Add("http-equiv", "refresh");
                        literal.Attributes.Add("content", string.Format("2; URL={0}", file.WebPageUrl));
                        this.Page.Header.Controls.Add(literal);
                    }
                    else
                    {
                        literal.Attributes["content"] = string.Format("2; URL={0}", file.WebPageUrl);
                    }
                    Title = file.WebPageUrl;
                }
                else if(System.IO.File.Exists(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename)) && UseLocalFile)
                {
                    pnlDocumentFound.Visible = true;
                    lnkFileLocation.NavigateUrl = lnkFileLocation.Text = string.Format("{0}{1}/{2}", portal.HomeDirectory, file.UploadDirectory.Replace("\\", "/"), file.Filename);
                    System.Web.UI.HtmlControls.HtmlGenericControl literal = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("MetaDocumentControl");
                    if (literal == null)
                    {
                        literal = new System.Web.UI.HtmlControls.HtmlGenericControl("meta")
                        {
                            ID = "MetaDocumentControl"
                        };
                        literal.Attributes.Add("http-equiv", "refresh");
                        literal.Attributes.Add("content", String.Format("2; URL={0}", lnkFileLocation.NavigateUrl));
                        this.Page.Header.Controls.Add(literal);
                    }
                    else
                    {
                        literal.Attributes["content"] = String.Format("2; URL={0}", lnkFileLocation.NavigateUrl);
                    }
                    Title = file.Filename;
                }
                else if(!UseLocalFile)
                {
                    pnlDocumentFound.Visible = true;
                    lnkFileLocation.NavigateUrl = lnkFileLocation.Text = string.Format("{0}?id={1}", ResolveUrl("~/DesktopModules/Gafware/DMS/GetFile.ashx"), Generic.StringToHex(HttpUtility.UrlEncode(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(String.Format("{0}", file.FileId)))));
                    System.Web.UI.HtmlControls.HtmlGenericControl literal = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("MetaDocumentControl");
                    if (literal == null)
                    {
                        literal = new System.Web.UI.HtmlControls.HtmlGenericControl("meta")
                        {
                            ID = "MetaDocumentControl"
                        };
                        literal.Attributes.Add("http-equiv", "refresh");
                        literal.Attributes.Add("content", string.Format("2; URL={0}", lnkFileLocation.NavigateUrl));
                        this.Page.Header.Controls.Add(literal);
                    }
                    else
                    {
                        literal.Attributes["content"] = string.Format("2; URL={0}", lnkFileLocation.NavigateUrl);
                    }
                    Title = file.Filename;
                }
                else
                {
                    if (Search)
                    {
                        pnlSearchNotFound.Visible = true;
                        Title = "No Search Results Found";
                    }
                    else
                    {
                        pnlDocumentNotFound.Visible = true;
                        Title = "Document Not Found";
                    }
                }
            }
            else if (GetFileCount > 1 || (GetFileCount == 1 && !IsLink))
            {
                rptDocuments.DataSource = SelectedDocuments.FindAll(p => (!p.Document.ActivationDate.HasValue || DateTime.Now >= p.Document.ActivationDate.Value) && (!p.Document.ExpirationDate.HasValue || DateTime.Now <= (p.Document.ExpirationDate.Value + new TimeSpan(23, 59, 59))) && GetActiveFilesForDocument(p).Count > 0).OrderBy(o => o.SortOrder).ToList();
                rptDocuments.DataBind();
                pnlDocumentsFound.Visible = true;
                Title = "Documents";
            }
            else
            {
                if (Search)
                {
                    pnlSearchNotFound.Visible = true;
                    Title = "No Search Results Found";
                }
                else
                {
                    pnlDocumentNotFound.Visible = true;
                    Title = "Document Not Found";
                }
            }
        }

        protected string GetCategories(object item)
        {
            if (item.GetType() == typeof(PacketDocument))
            {
                PacketDocument doc = item as PacketDocument;
                string strCategoryText = String.Empty;
                if (PortalCategories.Count > 1)
                {
                    List<DocumentCategory> categories = Components.DocumentController.GetAllCategoriesForDocument(doc.DocumentId);
                    foreach (DocumentCategory category in categories)
                    {
                        DotNetNuke.Security.Roles.RoleInfo categoryRole = UserController.GetRoleById(PortalId, category.Category.RoleId);
                        if (categoryRole != null)
                        {
                            if (DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo().IsInRole(categoryRole.RoleName))
                            {
                                strCategoryText += category.Category.CategoryName + ", ";
                            }
                        }
                    }
                    if (!String.IsNullOrEmpty(strCategoryText))
                    {
                        strCategoryText = strCategoryText.Substring(0, strCategoryText.Length - 2);
                        strCategoryText = "<strong>" + CategoryName + ": </strong>" + strCategoryText + "<br />";
                        return "<div style=\"padding: 4px 5px 0 0px;\">" + strCategoryText + "</div>";
                    }
                }
            }
            return String.Empty;
        }

        public string GetLinkUrl(string q)
        {
            string[] strArrays = new string[2];
            int moduleId = ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            strArrays[1] = string.Concat("q=", q);
            return NavigationManager.NavigateURL("GetDocuments", strArrays);
        }

        protected string DocSearchFileLink(object item)
        {
            if (item.GetType() == typeof(DMSFile))
            {
                DMSFile file = item as DMSFile;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                string query = String.Format("id={0}&fileid={1}", file.DocumentId, file.FileId);
                query = Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(query);
                query = HttpUtility.UrlEncode(query);
                query = Generic.StringToHex(query);
                string strAnchor = string.Format("<a href=\"{0}\" style=\"color: #{1};\">", GetLinkUrl(query), Theme);
                sb.Append("<div style=\"display: inline-block; text-align: center;\">");
                sb.Append(strAnchor);
                //string icon = string.Format("{0}/Images/icons/{2}/{1}.png", ControlPath, file.FileType, ThumbnailType);
                string icon = String.Format("{0}?id={1}", ResolveUrl("~/DesktopModules/Gafware/DMS/GetIcon.ashx"), Generic.StringToHex(HttpUtility.UrlEncode(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(String.Format("{0}", file.FileId)))));
                /*bool portrait = true;
                bool hasThumbnail = false;
                file.FileVersion.LoadThumbnail();
                if(file.FileVersion.Thumbnail != null && file.FileVersion.Thumbnail.Length > 0)
                {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(file.FileVersion.Thumbnail))
                    {
                        using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(ms))
                        {
                            if(bmp.Width > bmp.Height)
                            {
                                portrait = false;
                            }
                            hasThumbnail = true;
                        }
                    }
                }*/
                FileType fileType = DocumentController.GetFileTypeByExt(file.FileType, PortalId, PortalWideRepository ? 0 : TabModuleId);
                if (fileType != null)
                {
                    /*if (System.IO.File.Exists(MapPath(icon)))
                    {
                        sb.Append("<img alt=\"" + (String.IsNullOrEmpty(fileType.FileTypeShortName) ? file.FileType.ToUpper() : fileType.FileTypeShortName) + " document opens in new window.\" title=\"" + (String.IsNullOrEmpty(fileType.FileTypeShortName) ? file.FileType.ToUpper() : fileType.FileTypeShortName) + " document opens in new window.\" src=\"" + icon + "\" border=\"0\" />");
                    }
                    else
                    {
                        sb.Append("<img alt=\"" + (String.IsNullOrEmpty(fileType.FileTypeShortName) ? file.FileType.ToUpper() : fileType.FileTypeShortName) + " document opens in new window.\" title=\"" + (String.IsNullOrEmpty(fileType.FileTypeShortName) ? file.FileType.ToUpper() : fileType.FileTypeShortName) + " document opens in new window.\" src=\"" + ResolveUrl("~/DesktopModules/Gafware/DMS/images/icons/_blank.png") + "\" border=\"0\" />");
                    }*/
                    //sb.Append("<img style=\"height: " + (!hasThumbnail ? ThumbnailSize.ToString() : portrait ? ThumbnailSize.ToString() : ThumbnailLandscapeHeight.ToString()) + "px;\" alt=\"" + (String.IsNullOrEmpty(fileType.FileTypeShortName) ? file.FileType.ToUpper() : fileType.FileTypeShortName) + " document opens in new window.\" title=\"" + (String.IsNullOrEmpty(fileType.FileTypeShortName) ? file.FileType.ToUpper() : fileType.FileTypeShortName) + " document opens in new window.\" src=\"" + icon + "\" border=\"0\" />");
                    sb.Append("<img style=\"height: " + (file.FileVersion.IsLandscape ? ThumbnailLandscapeHeight.ToString() : ThumbnailSize.ToString()) + "px;\" alt=\"" + (String.IsNullOrEmpty(fileType.FileTypeShortName) ? file.FileType.ToUpper() : fileType.FileTypeShortName) + " document opens in new window.\" title=\"" + (String.IsNullOrEmpty(fileType.FileTypeShortName) ? file.FileType.ToUpper() : fileType.FileTypeShortName) + " document opens in new window.\" src=\"" + icon + "\" border=\"0\" />");
                }
                else
                {
                    /*if (System.IO.File.Exists(MapPath(icon)))
                    {
                        sb.Append("<img alt=\"" + file.FileType.ToUpper() + " document opens in new window.\" title=\"" + file.FileType.ToUpper() + " document opens in new window.\" src=\"" + icon + "\" border=\"0\" />");
                    }
                    else
                    {
                        sb.Append("<img alt=\"" + file.FileType.ToUpper() + " document opens in new window.\" title=\"" + file.FileType.ToUpper() + " document opens in new window.\" src=\"" + ResolveUrl("~/DesktopModules/Gafware/DMS/images/icons/_blank.png") + "\" border=\"0\" />");
                    }*/
                    //sb.Append("<img style=\"height: " + (!hasThumbnail ? ThumbnailSize.ToString() : portrait ? ThumbnailSize.ToString() : ThumbnailLandscapeHeight.ToString()) + "px;\" alt=\"" + file.FileType.ToUpper() + " document opens in new window.\" title=\"" + file.FileType.ToUpper() + " document opens in new window.\" src=\"" + icon + "\" border=\"0\" />");
                    sb.Append("<img style=\"height: " + (file.FileVersion.IsLandscape ? ThumbnailLandscapeHeight.ToString() : ThumbnailSize.ToString()) + "px;\" alt=\"" + file.FileType.ToUpper() + " document opens in new window.\" title=\"" + file.FileType.ToUpper() + " document opens in new window.\" src=\"" + icon + "\" border=\"0\" />");
                }
                sb.Append("</a>");
                sb.Append("<br />" + strAnchor);
                if (fileType != null)
                {
                    sb.Append("<span style=\"vertical-align: bottom;\">(" + fileType.FileTypeShortName + ")</span>");
                }
                else
                {
                    sb.Append("<span style=\"vertical-align: bottom;\">(" + file.FileType.ToUpper() + ")</span>");
                }
                sb.AppendLine("</a></div>");
                return sb.ToString();
            }
            return String.Empty;
        }

        protected string DocFileLink(object item)
        {
            if (item.GetType() == typeof(DMSFile))
            {
                DMSFile file = item as DMSFile;
                return DocFileLink(file);
            }
            return String.Empty;
        }

        protected string DocFileLink(DMSFile file)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string strAnchor = String.Empty;
            string query = String.Format("id={0}&fileid={1}&type={2}&terms={3}", file.DocumentId, file.FileId, file.FileType, Keywords.Replace(" ", "|"));
            query = Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(query);
            query = HttpUtility.UrlEncode(query);
            query = Generic.JSEncode(query);
            if (file.FileType.Equals("url", StringComparison.OrdinalIgnoreCase))
            {
                string strURL = file.WebPageUrl;
                if (!strURL.StartsWith("http"))
                {
                    strURL = "http://" + strURL;
                }
                strAnchor = string.Format("<a href=\"{0}\" onmouseup=\"recordDocumentRequest('{1}')\" style=\"color: #{2};\" target=\"_blank\">", strURL, query, Theme);
            }
            else
            {
                strAnchor = string.Format("<a href=\"{0}\" onmouseup=\"recordDocumentRequest('{1}')\" style=\"color: #{2};\" target=\"_blank\">", String.Format("{0}?id={1}", ResolveUrl("~/DesktopModules/Gafware/DMS/GetFile.ashx"), Generic.StringToHex(HttpUtility.UrlEncode(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(String.Format("{0}", file.FileId))))), query, Theme);
            }
            sb.Append("<div style=\"display: inline-block; text-align: center;\">");
            sb.Append(strAnchor);
            string icon = String.Format("{0}?id={1}", ResolveUrl("~/DesktopModules/Gafware/DMS/GetIcon.ashx"), Generic.StringToHex(HttpUtility.UrlEncode(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(String.Format("{0}", file.FileId)))));
            FileType fileType = DocumentController.GetFileTypeByExt(file.FileType, PortalId, PortalWideRepository ? 0 : TabModuleId);
            if (fileType != null)
            {
                /*if (System.IO.File.Exists(MapPath(icon)))
                {
                    sb.Append("<img alt=\"" + (String.IsNullOrEmpty(fileType.FileTypeShortName) ? file.FileType.ToUpper() : fileType.FileTypeShortName) + " document opens in new window.\" title=\"" + (String.IsNullOrEmpty(fileType.FileTypeShortName) ? file.FileType.ToUpper() : fileType.FileTypeShortName) + " document opens in new window.\" src=\"" + icon + "\" border=\"0\" />");
                }
                else
                {
                    sb.Append("<img alt=\"" + (String.IsNullOrEmpty(fileType.FileTypeShortName) ? file.FileType.ToUpper() : fileType.FileTypeShortName) + " document opens in new window.\" title=\"" + (String.IsNullOrEmpty(fileType.FileTypeShortName) ? file.FileType.ToUpper() : fileType.FileTypeShortName) + " document opens in new window.\" src=\"" + ResolveUrl("~/DesktopModules/Gafware/DMS/images/icons/_blank.png") + "\" border=\"0\" />");
                }*/
                //sb.Append("<img style=\"height: " + (!hasThumbnail ? ThumbnailSize.ToString() : portrait ? ThumbnailSize.ToString() : ThumbnailLandscapeHeight.ToString()) + "px;\" alt=\"" + (String.IsNullOrEmpty(fileType.FileTypeShortName) ? file.FileType.ToUpper() : fileType.FileTypeShortName) + " document opens in new window.\" title=\"" + (String.IsNullOrEmpty(fileType.FileTypeShortName) ? file.FileType.ToUpper() : fileType.FileTypeShortName) + " document opens in new window.\" src=\"" + icon + "\" border=\"0\" />");
                sb.Append("<img style=\"height: " + (file.FileVersion.IsLandscape ? ThumbnailLandscapeHeight.ToString() : ThumbnailSize.ToString()) + "px;\" alt=\"" + (String.IsNullOrEmpty(fileType.FileTypeShortName) ? file.FileType.ToUpper() : fileType.FileTypeShortName) + " document opens in new window.\" title=\"" + (String.IsNullOrEmpty(fileType.FileTypeShortName) ? file.FileType.ToUpper() : fileType.FileTypeShortName) + " document opens in new window.\" src=\"" + icon + "\" border=\"0\" />");
            }
            else
            {
                /*if (System.IO.File.Exists(MapPath(icon)))
                {
                    sb.Append("<img alt=\"" + file.FileType.ToUpper() + " document opens in new window.\" title=\"" + file.FileType.ToUpper() + " document opens in new window.\" src=\"" + icon + "\" border=\"0\" />");
                }
                else
                {
                    sb.Append("<img alt=\"" + file.FileType.ToUpper() + " document opens in new window.\" title=\"" + file.FileType.ToUpper() + " document opens in new window.\" src=\"" + ResolveUrl("~/DesktopModules/Gafware/DMS/images/icons/_blank.png") + "\" border=\"0\" />");
                }*/
                //sb.Append("<img style=\"height: " + (!hasThumbnail ? ThumbnailSize.ToString() : portrait ? ThumbnailSize.ToString() : ThumbnailLandscapeHeight.ToString()) + "px;\" alt=\"" + file.FileType.ToUpper() + " document opens in new window.\" title=\"" + file.FileType.ToUpper() + " document opens in new window.\" src=\"" + icon + "\" border=\"0\" />");
                sb.Append("<img style=\"height: " + (file.FileVersion.IsLandscape ? ThumbnailLandscapeHeight.ToString() : ThumbnailSize.ToString()) + "px;\" alt=\"" + file.FileType.ToUpper() + " document opens in new window.\" title=\"" + file.FileType.ToUpper() + " document opens in new window.\" src=\"" + icon + "\" border=\"0\" />");
            }
            sb.Append("</a>");
            sb.Append("<br/>" + strAnchor);
            if (fileType != null)
            {
                sb.Append("<span style=\"vertical-align: bottom;\">(" + fileType.FileTypeShortName + ")</span>");
            }
            else
            {
                sb.Append("<span style=\"vertical-align: bottom;\">(" + file.FileType.ToUpper() + ")</span>");
            }
            sb.AppendLine("</a></div>");
            return sb.ToString();
        }

        protected void rptDocuments_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                PacketDocument doc = (PacketDocument)e.Item.DataItem;
                Repeater rpt = (Repeater)e.Item.FindControl("rptFiles");
                if (rpt != null)
                {
                    rpt.DataSource = GetActiveFilesForDocument(doc);
                    rpt.DataBind();
                }
            }
        }

        protected void rptDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                PacketDocument doc = (PacketDocument)e.Row.DataItem;
                Repeater rpt = (Repeater)e.Row.FindControl("rptFiles");
                if (rpt != null)
                {
                    rpt.DataSource = GetActiveFilesForDocument(doc);
                    rpt.DataBind();
                }
            }
        }

        List<DMSFile> GetActiveFilesForDocument(PacketDocument doc)
        {
            DotNetNuke.Entities.Portals.PortalSettings portal = DotNetNuke.Entities.Portals.PortalSettings.Current;
            return doc.Document.Files.FindAll(p => p.StatusId == 1 && (!UseLocalFile || (UseLocalFile && System.IO.File.Exists(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, p.UploadDirectory.Replace("/", "\\"), p.Filename)))));
        }

        protected int GetFileCount
        {
            get
            {
                int count = 0;
                List<PacketDocument> docs = SelectedDocuments.FindAll(p => ((p.Document.IsPublic && p.Document.IsSearchable) || p.Document.CreatedByUserID == UserId) && (!p.Document.ActivationDate.HasValue || DateTime.Now >= p.Document.ActivationDate.Value) && (!p.Document.ExpirationDate.HasValue || DateTime.Now <= (p.Document.ExpirationDate.Value + new TimeSpan(23, 59, 59))));
                foreach (PacketDocument doc in docs)
                {
                    count += doc.Document.Files.FindAll(p => p.StatusId == 1 && (doc.FileId == 0 || p.FileId == doc.FileId)).Count;
                }
                return count;
            }
        }

        private DMSFile GetFirstActiveFile(List<PacketDocument> docs)
        {
            foreach(PacketDocument doc in docs)
            {
                List<DMSFile> files = doc.Document.Files.FindAll(p => p.StatusId == 1);
                if (doc.FileId > 0)
                {
                    DMSFile file = files.Find(p => p.FileId == doc.FileId );
                    if (file != null)
                    {
                        return file;
                    }
                }
                else if(files.Count > 0)
                {
                    return files[0];
                }
            }
            return null;
        }

        protected void rptDocuments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            rptDocuments.PageIndex = e.NewPageIndex;
            BindData();
        }

        protected void rptDocuments_DataBound(object sender, EventArgs e)
        {
            Generic.ApplyPaging(rptDocuments);
        }

        protected string GetFooter()
        {
            return (PageSize == 0 ? string.Format("{0} Results", GetFileCount) : string.Format("{3} {0} {4} {1} {5} {2} ", (rptDocuments.PageIndex * rptDocuments.PageSize) + 1, Math.Min((rptDocuments.PageIndex + 1) * rptDocuments.PageSize, GetFileCount), GetFileCount, LocalizeString("Showing"), LocalizeString("To"), LocalizeString("Of")));
        }

        protected void btnAdmin_Click(object sender, ImageClickEventArgs e)
        {
            if (ShowAdmin && IsAdmin)
            {
                string[] strArrays = new string[] { string.Concat("mid=", ModuleId.ToString()) };
                Response.Redirect(_navigationManager.NavigateURL("EditSettings", strArrays));
            }
        }

        public string LocalResourceFile
        {
            get
            {
                if (string.IsNullOrEmpty(_localResourceFile))
                {
                    _localResourceFile = System.IO.Path.Combine(ControlPath, string.Concat(Localization.LocalResourceDirectory, "/", ID));
                }
                return _localResourceFile;
            }
            set
            {
                _localResourceFile = value;
            }
        }

        public string LocalizeString(string key)
        {
            return Localization.GetString(key, LocalResourceFile);
        }
    }
}