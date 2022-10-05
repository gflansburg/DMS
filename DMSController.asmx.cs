using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.ServiceModel.Web;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Gafware.Modules.DMS
{
    /// <summary>
    /// Summary description for SendEmail
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class DMSController : System.Web.Services.WebService
    {
        public delegate void BulkImportFinishEventHandler(int filesImported);
        public static event BulkImportFinishEventHandler OnBulkImportFinish;

        public delegate void DeleteAllFinishEventHandler(int filesDeleted);
        public static event DeleteAllFinishEventHandler OnDeleteAllFinish;

        public enum Status
        {
            Failed,
            Success
        }

        public class Inactive
        {
            public int FileId { get; set; }
            public string DocumentUrl { get; set; }
        }

        public class Response
        {
            public List<Inactive> Inactive { get; set; }
            public Status Status { get; set; }
            public string Error { get; set; }
            public string DocumentUrl { get; set; }
        }

        public class VersionResponse
        {
            public string Version { get; set; }
            public int NewFileVersionId { get; set; }
            public bool NewFile { get; set;  }
            public Status Status { get; set; }
            public string DocumentUrl { get; set; }
            public int FileId { get; set; }
            public int OrigFileVersionId { get; set; }
            public string Filesize { get; set; }
            public int NewVersion { get; set; }
            public int OrigVersion { get; set; }
        }

        public new DotNetNuke.Entities.Users.UserInfo User
        {
            get
            {
                return DotNetNuke.Entities.Users.UserController.GetUserByName(PortalId, HttpContext.Current.User.Identity.Name);
            }
        }

        public int UserId
        {
            get
            {
                return DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo().UserID;
            }
        }

        public int PortalId
        {
            get
            {
                return DotNetNuke.Entities.Portals.PortalController.Instance.GetCurrentSettings().PortalId;
            }
        }

        public List<Inactive> ToggleStatus(Components.DMSFile file, bool bActive, string path, int mid, string url)
        {
            List<Inactive> inactive = new List<Inactive>();
            DotNetNuke.Entities.Portals.PortalSettings portal = DotNetNuke.Entities.Portals.PortalSettings.Current;
            if (bActive)
            {
                List<Components.DMSFile> files = Components.DocumentController.GetAllFilesForDocument(file.DocumentId).FindAll(p => p.FileType.Equals(file.FileType, StringComparison.OrdinalIgnoreCase) && p.StatusId == 1);
                foreach (Components.DMSFile f in files)
                {
                    f.StatusId = 2;
                    Components.DocumentController.SaveFile(f);
                    if (!f.FileType.Equals("url", StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, f.UploadDirectory.Replace("/", "\\"), f.Filename)))
                    {
                        System.IO.File.Delete(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, f.UploadDirectory.Replace("/", "\\"), f.Filename));
                    }
                    Inactive i = new Inactive();
                    i.FileId = f.FileId;
                    if (f.FileType.Equals("url", StringComparison.OrdinalIgnoreCase))
                    {
                        i.DocumentUrl = f.WebPageUrl;
                    }
                    else
                    {
                        if (f.StatusId == 1)
                        {
                            i.DocumentUrl = GetLinkUrl(url, mid, false, f.DocumentId.ToString(), f.FileId, String.Empty);
                        }
                        else
                        {
                            i.DocumentUrl = path + "GetFile.ashx" + "?id=" + f.FileId.ToString();
                        }
                    }
                    inactive.Add(i);
                }
            }
            file.StatusId = (bActive ? 1 : 2);
            Components.DocumentController.SaveFile(file);
            Components.Document doc = Components.DocumentController.GetDocument(file.DocumentId);
            if (doc != null)
            {
                doc.LastModifiedOnDate = DateTime.Now;
                Components.DocumentController.SaveDocument(doc);
            }
            if (!file.FileType.Equals("url", StringComparison.OrdinalIgnoreCase))
            {
                if (bActive)
                {
                    try
                    {
                        DotNetNuke.Entities.Portals.PortalInfo portalInfo = DotNetNuke.Entities.Portals.PortalController.Instance.GetPortal(PortalId);
                        Components.DMSPortalSettings portalSettings = Components.DocumentController.GetPortalSettings(PortalId);
                        Components.Repository repository = Components.DocumentController.GetRepository(PortalId, portalSettings.PortalWideRepository ? 0 : doc.TabModuleId);
                        Thumbnail thumb = new Thumbnail(portalInfo, repository, path);
                        thumb.CreateThumbnail(HttpContext.Current.Request, file);
                    }
                    catch (Exception)
                    {
                    }
                    if (doc != null && (!doc.ActivationDate.HasValue || DateTime.Now >= doc.ActivationDate.Value) && (!doc.ExpirationDate.HasValue || DateTime.Now <= (doc.ExpirationDate.Value + new TimeSpan(23, 59, 59))))
                    {
                        file.FileVersion.LoadContents();
                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                        {
                            file.CreateFolder();
                            if (System.IO.File.Exists(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename)))
                            {
                                System.IO.File.Delete(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename));
                            }
                            System.IO.FileStream fs = new System.IO.FileStream(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename), System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
                            fs.Write(file.FileVersion.Contents, 0, file.FileVersion.Contents.Length);
                            fs.Close();
                            try
                            {
                                System.IO.File.SetLastWriteTime(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename), file.FileVersion.CreatedOnDate);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                else if (System.IO.File.Exists(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename)))
                {
                    System.IO.File.Delete(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename));
                }
            }
            return inactive;
        }

        public string GetLinkUrl(string controlPath, int mid, bool showDescription, string documentList, int fileId, string headerText)
        {
            return string.Concat(controlPath, "/mid/", mid, "/q/", Generic.StringToHex(Generic.UrlEncode(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(String.Format("descriptions={0}&docids={1}&fileid={2}&headertext={3}", showDescription.ToString(), documentList, fileId, HttpUtility.UrlEncode(headerText))))));
        }

        public class DMSResult
        {
            public string Error { get; set; }
            public string Result { get; set; }
        }

        [WebMethod]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ReorderPackets(string list, int id, int rowIndex)
        {
            DMSResult result = new DMSResult();
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            try
            {
                List<PacketList.PacketPayload> selectedDocuments = JsonConvert.DeserializeObject<List<PacketList.PacketPayload>>(list);
                PacketList.PacketPayload packet = selectedDocuments.Find(d => (id > 0 && d.PayloadType == PacketList.PayloadType.Document && d.PacketDocument.DocumentId == id) || (id < 0 && d.PayloadType == PacketList.PayloadType.Tag && d.PacketTag.TagId == Math.Abs(id)));
                int oldSortOrder = packet.SortOrder;
                packet.SortOrder = rowIndex;
                if(rowIndex < oldSortOrder)
                {
                    foreach(PacketList.PacketPayload payload in selectedDocuments)
                    {
                        if(payload.PayloadType == PacketList.PayloadType.Document)
                        {
                            if (payload.PacketDocument.DocumentId != id && payload.SortOrder >= rowIndex && payload.SortOrder < oldSortOrder)
                            {
                                payload.SortOrder++;
                            }
                        }
                        if(payload.PayloadType == PacketList.PayloadType.Tag)
                        {
                            if (payload.PacketTag.TagId != Math.Abs(id) && payload.SortOrder >= rowIndex && payload.SortOrder < oldSortOrder)
                            {
                                payload.SortOrder++;
                            }
                        }
                    }
                }
                else
                {
                    foreach (PacketList.PacketPayload payload in selectedDocuments)
                    {
                        if (payload.PayloadType == PacketList.PayloadType.Document)
                        {
                            if (payload.PacketDocument.DocumentId != id && payload.SortOrder <= rowIndex && payload.SortOrder > oldSortOrder)
                            {
                                payload.SortOrder--;
                            }
                        }
                        if (payload.PayloadType == PacketList.PayloadType.Tag)
                        {
                            if (payload.PacketTag.TagId != Math.Abs(id) && payload.SortOrder <= rowIndex && payload.SortOrder > oldSortOrder)
                            {
                                payload.SortOrder--;
                            }
                        }
                    }
                }
                //Components.PacketController.MovePacket(id > 0 ? id : 0, id < 0 ? id * -1 : 0, rowIndex);
                //result.Result = "Success";
                result.Result = JsonConvert.SerializeObject(selectedDocuments.OrderBy(d => d.SortOrder));
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }
            Context.Response.Write(oSerializer.Serialize(result));
        }

        [WebMethod]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ToggleStatus(int fileId, bool bActive, string path, int mid, string url)
        {
            Response response = new Response();
            response.Status = Status.Failed;
            response.Inactive = new List<Inactive>();
            try
            {
                Components.DMSFile file = Components.DocumentController.GetFile(fileId);
                if (file != null && HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    Components.Document document = Components.DocumentController.GetDocument(file.DocumentId);
                    var name = HttpContext.Current.User.Identity.Name;
                    DotNetNuke.Security.Roles.RoleInfo groupOwner = (document.IsGroupOwner ? Components.UserController.GetRoleById(PortalId, document.CreatedByUserID) : null);
                    if (document != null && ((!document.IsGroupOwner && document.CreatedByUserID == User.UserID) || (document.IsGroupOwner && User.IsInRole(groupOwner.RoleName)) || User.IsSuperUser || User.IsInRole("Administrator")))
                    {
                        response.Inactive = ToggleStatus(file, bActive, path, mid, url);
                        response.Status = Status.Success;
                        if (file.FileType.Equals("url", StringComparison.OrdinalIgnoreCase))
                        {
                            response.DocumentUrl = file.WebPageUrl;
                        }
                        else
                        {
                            if (file.StatusId == 1)
                            {
                                response.DocumentUrl = GetLinkUrl(url, mid, false, file.DocumentId.ToString(), file.FileId, String.Empty);
                            }
                            else
                            {
                                response.DocumentUrl = path + "GetFile.ashx" + "?id=" + file.FileId.ToString();
                            }
                        }
                    }
                    else
                    {
                        response.Error = "Unauthorized";
                    }
                }
                else
                {
                    response.Error = "Not Logged In";
                }
            }
            catch(Exception ex)
            {
                response.Error = ex.Message;
            }
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            Context.Response.Write(oSerializer.Serialize(response));
        }

        [WebMethod]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SaveVersion(int fileVersionId, int major, int minor, int build, string path, int mid, string url)
        {
            VersionResponse response = new VersionResponse();
            response.Status = Status.Failed;
            response.OrigFileVersionId = fileVersionId;
            response.NewFileVersionId = fileVersionId;
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            int nVersion = Convert.ToInt32(string.Format("{0}{1}{2}", major.ToString().PadLeft(3, '0'), minor.ToString().PadLeft(3, '0'), build.ToString().PadLeft(3, '0')));
            Components.FileVersion version = Components.DocumentController.GetFileVersion(fileVersionId);
            if (version != null)
            {
                response.OrigVersion = version.Version;
                response.NewVersion = nVersion;
                Components.DMSFile file = Components.DocumentController.GetFile(version.FileId);
                if (file != null)
                {
                    response.OrigFileVersionId = file.FileVersionId;
                    List<Components.FileVersion> versions = Components.DocumentController.GetFileVersions(version.FileId);
                    bool exists = versions.Exists(p => p.Version == nVersion && p.FileVersionId != fileVersionId);
                    if (!exists)
                    {
                        version.Version = nVersion;
                        response.Status = Status.Success;
                        response.Version = GetVersion(nVersion);
                        Components.DocumentController.SaveFileVersion(version);
                        file = Components.DocumentController.GetFile(version.FileId);
                        if (file != null)
                        {
                            Components.Document doc = Components.DocumentController.GetDocument(file.DocumentId);
                            if (doc != null)
                            {
                                doc.LastModifiedOnDate = DateTime.Now;
                                Components.DocumentController.SaveDocument(doc);
                            }
                        }
                        if (file != null && file.FileVersionId != response.OrigFileVersionId)
                        {
                            response.NewFile = true;
                            ToggleStatus(file, file.StatusId == 1, path, mid, url);
                            response.Version = GetVersion(file.Version);
                            response.NewFileVersionId = file.FileVersionId;
                            response.FileId = file.FileId;
                            response.Filesize = GetFilesize(file.Filesize);
                            if (file.FileType.Equals("url", StringComparison.OrdinalIgnoreCase))
                            {
                                response.DocumentUrl = file.WebPageUrl;
                            }
                            else
                            {
                                if (file.StatusId == 1)
                                {
                                    response.DocumentUrl = GetLinkUrl(url, mid, false, file.DocumentId.ToString(), file.FileId, String.Empty);
                                }
                                else
                                {
                                    response.DocumentUrl = path + "GetFile.ashx" + "?id=" + file.FileId.ToString();
                                }
                            }
                        }
                    }
                }
            }
            Context.Response.Write(oSerializer.Serialize(response));
        }

        private string GetFilesize(int size)
        {
            string[] sizes = { "bytes", "kb", "mb", "gb", "tb" };
            double len = size;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return String.Format("{0:0.#} {1}", len, sizes[order]);
        }

        private string GetVersion(int version)
        {
            int build = version % 1000;
            int minor = ((int)(version / 1000)) % 1000;
            int major = (int)(((int)(version / 1000)) / 1000);
            return string.Format("{0}.{1}.{2}", major, minor, build);
        }

        [WebMethod]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetLinkUrl(bool showDescription, string documentList, int fileId, string headerText, string path)
        {
            string url = string.Concat(path, "/q/", Generic.StringToHex(Generic.UrlEncode(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(String.Format("descriptions={0}&docids={1}&fileid={2}&headertext={3}", showDescription, documentList, fileId, HttpUtility.UrlEncode(headerText))))));
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            Context.Response.Write(oSerializer.Serialize(url));
        }

        [WebMethod]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetDocumentsForTag(int tagId)
        {
            Components.Tag tag = Components.DocumentController.GetTag(tagId);
            if (tag != null)
            {
                Components.DMSPortalSettings settings = Components.DocumentController.GetPortalSettings(tag.PortalId);
                if (settings == null)
                {
                    settings = new Components.DMSPortalSettings();
                }
                List<Components.DocumentView> docs = Components.DocumentController.GetAllDocumentsForTag(tagId, settings.PortalWideRepository);
                DataTable dt = Generic.DocumentListToDataTable(docs, tag.PortalId, settings.PortalWideRepository ? 0 : tag.TabModuleId);
                string json = ConvertDataTabletoString(dt);
                Context.Response.Write(json);
            }
            else
            {
                Context.Response.Write("[]");
            }
        }

        [WebMethod]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void RecordDocumentRequest(string q, int pid)
        {
            try
            {
                string query = HttpUtility.UrlDecode(q);
                query = Gafware.Modules.DMS.Cryptography.CryptographyUtil.Decrypt(query);
                System.Collections.Specialized.NameValueCollection queryString = HttpUtility.ParseQueryString(query);
                string id = queryString["id"] ?? String.Empty;
                string fileId = queryString["fileid"] ?? String.Empty;
                string type = queryString["type"] ?? String.Empty;
                string terms = queryString["terms"] ?? String.Empty;
                if (!String.IsNullOrEmpty(id) && Generic.IsNumber(id))
                {
                    RecordDocumentRequest(pid, Convert.ToInt32(id), !String.IsNullOrEmpty(fileId) && Generic.IsNumber(fileId) ? Convert.ToInt32(fileId) : 0, type, terms);
                }
            }
            catch (Exception)
            {
            }
        }

        public static void RecordDocumentRequest(int portalId, int documentId, int fileId, string fileType, string searchTerms)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("  <datum>");
            sb.AppendLine(String.Format("    <Time>{0}</Time>", DateTime.Now));
            sb.AppendLine(String.Format("    <IP>{0}</IP>", Generic.GetIPAddress()));
            sb.AppendLine(String.Format("    <User_Agent>{0}</User_Agent>", HttpContext.Current.Request.UserAgent));
            sb.AppendLine(String.Format("    <Browser_Type>{0}</Browser_Type>", HttpContext.Current.Request.Browser.Type));
            sb.AppendLine(String.Format("    <Browser_Name>{0}</Browser_Name>", GetBrowserName(HttpContext.Current.Request)));
            sb.AppendLine(String.Format("    <Browser_Version>{0}</Browser_Version>", GetBrowserVersion(HttpContext.Current.Request)));
            sb.AppendLine(String.Format("    <Platform>{0}</Platform>", GetUserPlatform(HttpContext.Current.Request)));
            sb.AppendLine(String.Format("    <Is_Mobile>{0}</Is_Mobile>", HttpContext.Current.Request.Browser.IsMobileDevice));
            sb.AppendLine(String.Format("    <Is_Crawler>{0}</Is_Crawler>", HttpContext.Current.Request.Browser.Crawler));
            sb.AppendLine(String.Format("    <Portal_ID>{0}</Portal_ID>", portalId));
            sb.AppendLine(String.Format("    <Doc_ID>{0}</Doc_ID>", documentId));
            sb.AppendLine(String.Format("    <File_ID>{0}</File_ID>", fileId));
            sb.AppendLine(String.Format("    <File_Type>{0}</File_Type>", fileType));
            sb.AppendLine(String.Format("    <Search_Terms>{0}</Search_Terms>", HttpUtility.HtmlEncode(searchTerms)));
            sb.AppendLine("  </datum>");
            string strNewFileName = GetNewLogFilename(HttpContext.Current.Request.MapPath("~/Portals/_default/Logs"), DateTime.Now, "OUHR_DMS_" + portalId + "_");
            WriteToDocLog(HttpContext.Current.Request.MapPath("~/Portals/_default/Logs"), strNewFileName, sb.ToString());
        }

        private static string GetBrowserName(HttpRequest request)
        {
            var ua = request.UserAgent;
            string browser = request.Browser.Browser;
            Regex regExEdge1 = new Regex(@"Edge/(?'version'(?'major'\d+)(?'minor'\.\d+))");
            Regex regExEdge2 = new Regex(@"Edg/(?'version'(?'major'\d+)(?'minor'\.\d+))");
            Regex regExOpera = new Regex(@"OPR/(?'version'(?'major'\d+)(?'minor'\.\d+))");
            Regex regExFxiOS = new Regex(@"fxios\/([\w\.-]+)", RegexOptions.IgnoreCase); // Firefox for iOS 
            Regex regExCriOS = new Regex(@"crios\/([\w\.-]+)", RegexOptions.IgnoreCase); // Chrome for iOS 
            Regex regExEdgiOS = new Regex(@"edgios\/([\w\.-]+)", RegexOptions.IgnoreCase); // Edge for iOS 
            Regex regExEdgA = new Regex(@"edga\/([\w\.-]+)", RegexOptions.IgnoreCase); // Edge for Android
            Regex regExUC = new Regex(@"ucbrowser\/([\w\.-]+)", RegexOptions.IgnoreCase); // UCBrowser
            Regex regExBrave = new Regex(@"brave\/([\w\.-]+)", RegexOptions.IgnoreCase); // Brave
            Regex regExBC = new Regex(@"brave chrome\/([\w\.-]+)", RegexOptions.IgnoreCase); // Brave
            Regex regExSamsung = new Regex(@"samsungbrowser\/([\w\.-]+)", RegexOptions.IgnoreCase); // Samsung Browser

            if (regExEdge1.IsMatch(ua) || regExEdge2.IsMatch(ua))
            {
                browser = "Edge";
            }
            else if (regExOpera.IsMatch(ua))
            {
                browser = "Opera";
            }
            else if (regExFxiOS.IsMatch(ua))
            {
                browser = "Firefox";
            }
            else if (regExCriOS.IsMatch(ua))
            {
                browser = "Chrome";
            }
            else if (regExEdgiOS.IsMatch(ua) || regExEdgA.IsMatch(ua))
            {
                browser = "Edge";
            }
            else if (regExUC.IsMatch(ua))
            {
                browser = "UC Browser";
            }
            else if (regExBC.IsMatch(ua) || regExBrave.IsMatch(ua) || ua.Contains("Brave", StringComparison.OrdinalIgnoreCase))
            {
                browser = "Brave";
            }
            else if (regExSamsung.IsMatch(ua))
            {
                browser = "Samsung";
            }
            if (ua.Contains("Googlebot", StringComparison.OrdinalIgnoreCase) && request.Browser.Crawler)
            {
                browser = "Googlebot";
            }
            return browser + (ua.Contains("Mobile") ? " Mobile" : string.Empty);
        }

        private static string GetUserPlatform(HttpRequest request)
        {
            var ua = request.UserAgent;

            if (ua.Contains("Android"))
            {
                return string.Format("Android {0}", GetMobileVersion(ua, "Android"));
            }

            if (ua.Contains("iPad"))
            {
                return string.Format("iPad OS {0}", GetMobileVersion(ua, "OS"));
            }

            if (ua.Contains("iPhone"))
            {
                return string.Format("iPhone OS {0}", GetMobileVersion(ua, "OS"));
            }

            if (ua.Contains("Windows Phone"))
            {
                return string.Format("Windows Phone {0}", GetMobileVersion(ua, "Windows Phone"));
            }

            if (ua.Contains("Linux") && ua.Contains("KFAPWI"))
            {
                return "Kindle Fire";
            }

            if (ua.Contains("RIM Tablet") || ((ua.Contains("BB") || ua.Contains("BlackBerry")) && ua.Contains("Mobile")))
            {
                return "Black Berry";
            }

            if (ua.Contains("Mac OS"))
            {
                return "Mac OS";
            }

            if (ua.Contains("Xbox One"))
            {
                return "Xbox One";
            }

            if (ua.Contains("Xbox"))
            {
                return "Xbox";
            }

            if (ua.Contains("Windows NT 5.1") || ua.Contains("Windows NT 5.2"))
            {
                return "Windows XP";
            }

            if (ua.Contains("Windows NT 5"))
            {
                return "Windows 2000";
            }

            if (ua.Contains("Windows NT 4"))
            {
                return "Windows NT";
            }

            if (ua.Contains("Win 9x 4.90"))
            {
                return "Windows Me";
            }

            if (ua.Contains("Windows 98"))
            {
                return "Windows 98";
            }

            if (ua.Contains("Windows 95"))
            {
                return "Windows 95";
            }

            if (ua.Contains("Windows NT 6.0"))
            {
                return "Windows Vista";
            }

            if (ua.Contains("Windows NT 6.1"))
            {
                return "Windows 7";
            }

            if (ua.Contains("Windows NT 6.2"))
            {
                return "Windows 8";
            }

            if (ua.Contains("Windows NT 6.3"))
            {
                return "Windows 8.1";
            }

            if (ua.Contains("Windows NT 10"))
            {
                return "Windows 10";
            }

            if (ua.Contains("Windows NT 11"))
            {
                return "Windows 11";
            }

            //fallback to basic platform:
            return request.Browser.Platform + (ua.Contains("Mobile") ? " Mobile" : string.Empty);
        }

        private static string GetBrowserVersion(HttpRequest request)
        {
            var ua = request.UserAgent;
            Regex regExEdge1 = new Regex(@"Edge/(?'version'(?'major'\d+)(?'minor'\.\d+))");
            Regex regExEdge2 = new Regex(@"Edg/(?'version'(?'major'\d+)(?'minor'\.\d+))");
            Regex regExOpera = new Regex(@"OPR/(?'version'(?'major'\d+)(?'minor'\.\d+))");
            Regex regExFxiOS = new Regex(@"fxios\/([\w\.-]+)", RegexOptions.IgnoreCase); // Firefox for iOS 
            Regex regExCriOS = new Regex(@"crios\/([\w\.-]+)", RegexOptions.IgnoreCase); // Chrome for iOS 
            Regex regExEdgiOS = new Regex(@"edgios\/([\w\.-]+)", RegexOptions.IgnoreCase); // Edge for iOS 
            Regex regExEdgA = new Regex(@"edga\/([\w\.-]+)", RegexOptions.IgnoreCase); // Edge for Android
            Regex regExUC = new Regex(@"ucbrowser\/([\w\.-]+)", RegexOptions.IgnoreCase); // UCBrowser
            Regex regExBrave = new Regex(@"brave\/([\w\.-]+)", RegexOptions.IgnoreCase); // Brave
            Regex regExBC = new Regex(@"brave chrome\/([\w\.-]+)", RegexOptions.IgnoreCase); // Brave
            Regex regExSamsung = new Regex(@"samsungbrowser\/([\w\.-]+)", RegexOptions.IgnoreCase); // Samsung Browser
            Regex regExSafariiOS = new Regex(@"safari\/([\w\.-]+)", RegexOptions.IgnoreCase); // Safari for iOS 
            if (regExEdge1.IsMatch(ua))
            {
                string[] match = regExEdge1.Split(ua);
                if (match.Length > 1)
                {
                    return match[1];
                }
            }
            else if (regExEdge2.IsMatch(ua))
            {
                string[] match = regExEdge2.Split(ua);
                if (match.Length > 1)
                {
                    return match[1];
                }
            }
            else if (regExOpera.IsMatch(ua))
            {
                string[] match = regExOpera.Split(ua);
                if (match.Length > 1)
                {
                    return match[1];
                }
            }
            else if (regExFxiOS.IsMatch(ua))
            {
                Match match = regExFxiOS.Match(ua);
                string[] nameAndVersion = match.Value.Split('/');
                if(nameAndVersion.Length > 1)
                {
                    return nameAndVersion[1];
                }
            }
            else if (regExCriOS.IsMatch(ua))
            {
                Match match = regExCriOS.Match(ua);
                string[] nameAndVersion = match.Value.Split('/');
                if (nameAndVersion.Length > 1)
                {
                    return nameAndVersion[1];
                }
            }
            else if (regExEdgiOS.IsMatch(ua))
            {
                Match match = regExEdgiOS.Match(ua);
                string[] nameAndVersion = match.Value.Split('/');
                if (nameAndVersion.Length > 1)
                {
                    return nameAndVersion[1];
                }
            }
            else if (regExEdgA.IsMatch(ua))
            {
                Match match = regExEdgA.Match(ua);
                string[] nameAndVersion = match.Value.Split('/');
                if (nameAndVersion.Length > 1)
                {
                    return nameAndVersion[1];
                }
            }
            else if (regExUC.IsMatch(ua))
            {
                Match match = regExUC.Match(ua);
                string[] nameAndVersion = match.Value.Split('/');
                if (nameAndVersion.Length > 1)
                {
                    return nameAndVersion[1];
                }
            }
            else if (regExBC.IsMatch(ua))
            {
                Match match = regExBC.Match(ua);
                string[] nameAndVersion = match.Value.Split('/');
                if (nameAndVersion.Length > 1)
                {
                    return nameAndVersion[1];
                }
            }
            else if (regExBrave.IsMatch(ua))
            {
                Match match = regExBrave.Match(ua);
                string[] nameAndVersion = match.Value.Split('/');
                if (nameAndVersion.Length > 1)
                {
                    return nameAndVersion[1];
                }
            }
            else if (regExSamsung.IsMatch(ua))
            {
                Match match = regExSamsung.Match(ua);
                string[] nameAndVersion = match.Value.Split('/');
                if (nameAndVersion.Length > 1)
                {
                    return nameAndVersion[1];
                }
            }
            else if (regExSafariiOS.IsMatch(ua) && request.Browser.Browser.Equals("Safari", StringComparison.OrdinalIgnoreCase) && (ua.Contains("iPad") || ua.Contains("iPhone")))
            {
                Match match = regExSafariiOS.Match(ua);
                string[] nameAndVersion = match.Value.Split('/');
                if (nameAndVersion.Length > 1)
                {
                    return nameAndVersion[1];
                }
            }
            //fallback to basic version:
            return request.Browser.Version;
        }

        private static string GetMobileVersion(string userAgent, string device)
        {
            var temp = userAgent.Substring(userAgent.IndexOf(device) + device.Length).TrimStart();
            var version = string.Empty;

            foreach (var character in temp)
            {
                var validCharacter = false;
                int test = 0;

                if (Int32.TryParse(character.ToString(), out test))
                {
                    version += character;
                    validCharacter = true;
                }

                if (character == '.' || character == '_')
                {
                    version += '.';
                    validCharacter = true;
                }

                if (validCharacter == false)
                    break;
            }

            return version;
        }
        
        private static string GetNewLogFilename(string strDirectory, DateTime date, string strFilePrefix)
        {
            int fileCount = 1;
            bool validFilename = false;
            string strFileName = String.Empty;

            while (!validFilename)
            {
                strFileName = strFilePrefix + date.ToString("MM_dd_yyyy") + "_(" + fileCount.ToString() + ").xml";
                bool exists = System.IO.File.Exists(strDirectory + "\\" + strFileName);
                if (exists)
                {
                    long fileSize = Generic.GetFileSize(strDirectory + "\\" + strFileName);
                    if (fileSize < 500000)
                    {
                        validFilename = true;
                    }
                    else
                    {
                        fileCount++;
                    }
                }
                else
                {
                    validFilename = true;
                }
            }
            return strFileName;
        }

        private static void WriteToDocLog(string strDirectory, string strLogName, string strNewText)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.FileStream fs = new System.IO.FileStream(strDirectory + "\\" + strLogName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
            string xml = strNewText;
            if (fs.Length > 0)
            {
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, (int)fs.Length);
                xml = System.Text.UTF8Encoding.ASCII.GetString(data);
                xml = xml.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", String.Empty);
                xml = xml.Replace("<DocRequests>", String.Empty);
                xml = xml.Replace("</DocRequests>", String.Empty);
                xml = Generic.HTMLTrim(xml);
                xml = strNewText + xml;
            }
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<DocRequests>");
            sb.AppendLine(xml);
            sb.AppendLine("</DocRequests>");
            fs.Seek(0, System.IO.SeekOrigin.Begin);
            byte[] text = System.Text.UTF8Encoding.ASCII.GetBytes(sb.ToString());
            fs.Write(text, 0, text.Length);
            fs.Close();
        }

        private string ConvertDataTabletoString(DataTable dt)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }

        private static Dictionary<string, object> ProcessProgresses = new Dictionary<string, object>();

        private class Stats
        {
            public DateTime CheckTime { get; set; }
            public int Progress { get; set; }
            public int FilesProcessed { get; set; }
        }

        private static Dictionary<string, Stats> JobStats = new Dictionary<string, Stats>();

        public static string ImportFiles(string controlPath, string filePath, bool subFolderIsDocumentName, bool subFolderIsTag, bool prependSubFolderName, string seperator, int firstLevel, DateTime? activationDate, DateTime? expirationDate, int ownerId, bool searchable, bool useCategorySecurityRoles, int securityRoleId, int[] categories, bool replacePDFTitle, bool isPublic, int portalId, int tabModuleId, bool portalWideRepository)
        {
            BulkImportThread worker = new BulkImportThread(HttpContext.Current.Request);
            worker.Finished += new EventHandler(BulkImportThread_Finished);
            worker.ImportFiles(controlPath, filePath, subFolderIsDocumentName, subFolderIsTag, prependSubFolderName, seperator, firstLevel, activationDate, expirationDate, ownerId, searchable, useCategorySecurityRoles, securityRoleId, categories, replacePDFTitle, isPublic, portalId, tabModuleId, portalWideRepository);
            JobStats.Add(worker.ProcessName, new Stats());
            ProcessProgresses.Add(worker.ProcessName, worker);
            return worker.ProcessName;
        }

        private static void BulkImportThread_Finished(object sender, EventArgs e)
        {
            BulkImportThread worker = (BulkImportThread)sender;
            if (ProcessProgresses.ContainsKey(worker.ProcessName))
            {
                OnBulkImportFinish?.Invoke(worker.FilesImported);
                ProcessProgresses.Remove(worker.ProcessName);
                if (JobStats != null && JobStats.ContainsKey(worker.ProcessName ?? String.Empty))
                {
                    Stats stats = JobStats[worker.ProcessName];
                    stats.CheckTime = DateTime.Now;
                    stats.FilesProcessed = worker.FilesImported;
                    stats.Progress = 100;
                }
            }
        }

        [WebMethod]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetImportFilesProgress(string processName)
        {
            Stats stats = null;
            if (ProcessProgresses != null && ProcessProgresses.ContainsKey(processName ?? String.Empty))
            {
                BulkImportThread worker = (BulkImportThread)ProcessProgresses[processName];
                if (JobStats != null && JobStats.ContainsKey(processName ?? String.Empty))
                {
                    stats = JobStats[processName];
                    stats.CheckTime = DateTime.Now;
                    stats.Progress = worker.Progress;
                    stats.FilesProcessed = worker.FilesImported;
                }
                else
                {
                    stats = new Stats()
                    {
                        Progress = worker.Progress,
                        FilesProcessed = worker.FilesImported,
                        CheckTime = DateTime.Now
                    };
                    JobStats.Add(processName, stats);
                }
            }
            else if (JobStats != null && JobStats.ContainsKey(processName ?? String.Empty))
            {
                stats = JobStats[processName];
                if ((DateTime.Now - stats.CheckTime).TotalMinutes > 5)
                {
                    stats.Progress = 100;
                }
            }
            else
            {
                stats = new Stats()
                {
                    Progress = 100,
                    FilesProcessed = 0,
                    CheckTime = DateTime.Now
                };
            }
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            Context.Response.Write(oSerializer.Serialize(stats));
        }

        [WebMethod]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetFilesImportedCount(string processName)
        {
            int filesImported = 0;
            if (ProcessProgresses != null && ProcessProgresses.ContainsKey(processName ?? String.Empty))
            {
                BulkImportThread worker = (BulkImportThread)ProcessProgresses[processName];
                filesImported = worker.FilesImported;
                if (JobStats != null && JobStats.ContainsKey(processName ?? String.Empty))
                {
                    Stats stats = JobStats[processName];
                    stats.CheckTime = DateTime.Now;
                    stats.FilesProcessed = filesImported;
                }
            }
            else if (JobStats != null && JobStats.ContainsKey(processName ?? String.Empty))
            {
                Stats stats = JobStats[processName];
                filesImported = stats.FilesProcessed;
            }
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            Context.Response.Write(oSerializer.Serialize(filesImported));
        }

        public static string DeleteAll(System.Data.DataView dataView, int portalId, int tabModuleId, bool portalWideRepository)
        {
            DeleteAllThread worker = new DeleteAllThread(HttpContext.Current.Request);
            worker.Finished += new EventHandler(DeleteAllThread_Finished);
            worker.DeleteAll(dataView, portalId, tabModuleId, portalWideRepository);
            JobStats.Add(worker.ProcessName, new Stats());
            ProcessProgresses.Add(worker.ProcessName, worker);
            return worker.ProcessName;
        }

        private static void DeleteAllThread_Finished(object sender, EventArgs e)
        {
            DeleteAllThread worker = (DeleteAllThread)sender;
            if (ProcessProgresses.ContainsKey(worker.ProcessName))
            {
                OnDeleteAllFinish?.Invoke(worker.FilesDeleted);
                ProcessProgresses.Remove(worker.ProcessName);
                if (JobStats != null && JobStats.ContainsKey(worker.ProcessName ?? String.Empty))
                {
                    Stats stats = JobStats[worker.ProcessName];
                    stats.CheckTime = DateTime.Now;
                    stats.FilesProcessed = worker.FilesDeleted;
                    stats.Progress = 100;
                }
            }
        }

        [WebMethod]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetDeleteAllProgress(string processName)
        {
            Stats stats = null;
            if (ProcessProgresses != null && ProcessProgresses.ContainsKey(processName ?? String.Empty))
            {
                DeleteAllThread worker = (DeleteAllThread)ProcessProgresses[processName];
                if (JobStats != null && JobStats.ContainsKey(processName ?? String.Empty))
                {
                    stats = JobStats[processName];
                    stats.CheckTime = DateTime.Now;
                    stats.Progress = worker.Progress;
                    stats.FilesProcessed = worker.FilesDeleted;
                }
                else
                {
                    stats = new Stats()
                    {
                        Progress = worker.Progress,
                        FilesProcessed = worker.FilesDeleted,
                        CheckTime = DateTime.Now
                    };
                    JobStats.Add(processName, stats);
                }
            }
            else if (JobStats != null && JobStats.ContainsKey(processName ?? String.Empty))
            {
                stats = JobStats[processName];
                if((DateTime.Now - stats.CheckTime).TotalMinutes > 5)
                {
                    stats.Progress = 100;
                }
            }
            else
            {
                stats = new Stats()
                {
                    Progress = 100,
                    FilesProcessed = 0,
                    CheckTime = DateTime.Now
                };
            }
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            Context.Response.Write(oSerializer.Serialize(stats));
        }

        [WebMethod]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetDeleteAllCount(string processName)
        {
            int filesDeleted = 0;
            if (ProcessProgresses != null && ProcessProgresses.ContainsKey(processName ?? String.Empty))
            {
                DeleteAllThread worker = (DeleteAllThread)ProcessProgresses[processName];
                filesDeleted = worker.FilesDeleted;
                if (JobStats != null && JobStats.ContainsKey(processName ?? String.Empty))
                {
                    Stats stats = JobStats[processName];
                    stats.CheckTime = DateTime.Now;
                    stats.FilesProcessed = filesDeleted;
                }
            }
            else if (JobStats != null && JobStats.ContainsKey(processName ?? String.Empty))
            {
                Stats stats = JobStats[processName];
                filesDeleted = stats.FilesProcessed;
            }
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            Context.Response.Write(oSerializer.Serialize(filesDeleted));
        }
    }
}
