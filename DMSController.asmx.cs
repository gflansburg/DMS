using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.ServiceModel.Web;
using System.Data;

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

        private List<Inactive> ToggleStatus(Components.DMSFile file, bool bActive, string path, int mid, string url)
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
            if (!file.FileType.Equals("url", StringComparison.OrdinalIgnoreCase))
            {
                if (bActive)
                {
                    Components.Document doc = Components.DocumentController.GetDocument(file.DocumentId);
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
            return string.Concat(controlPath, "/mid/", mid, "/q/", Generic.StringToHex(HttpUtility.UrlEncode(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(String.Format("descriptions={0}&docids={1}&fileid={2}&headertext={3}", showDescription.ToString(), documentList, fileId, HttpUtility.UrlEncode(headerText))))));
        }

        [WebMethod]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ReorderPackets(int id, int rowIndex)
        {
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            try
            {
                Components.PacketController.MovePacket(id > 0 ? id : 0, id < 0 ? id * -1 : 0, rowIndex);
                Context.Response.Write(oSerializer.Serialize("Success"));
            }
            catch (Exception ex)
            {
                Context.Response.Write(oSerializer.Serialize("Failed: " + ex.Message));
            }
        }

        [WebMethod]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ToggleStatus(int fileId, bool bActive, string path, int mid, string url)
        {
            Response response = new Response();
            response.Status = Status.Failed;
            response.Inactive = new List<Inactive>();
            Components.DMSFile file = Components.DocumentController.GetFile(fileId);
            if (file != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                Components.Document document = Components.DocumentController.GetDocument(file.DocumentId);
                var name = HttpContext.Current.User.Identity.Name;
                if (document != null && (document.CreatedByUserID == User.UserID || User.IsSuperUser || User.IsInRole("Administrator")))
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
            string url = string.Concat(path, "/q/", Generic.StringToHex(HttpUtility.UrlEncode(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(String.Format("descriptions={0}&docids={1}&fileid={2}&headertext={3}", showDescription, documentList, fileId, HttpUtility.UrlEncode(headerText))))));
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
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.AppendLine("<datum>");
                    sb.AppendLine(String.Format("  <Time>{0}</Time>", DateTime.Now));
                    sb.AppendLine(String.Format("  <IP>{0}</IP>", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]));
                    sb.AppendLine(String.Format("  <Doc_ID>{0}</Doc_ID>", id));
                    sb.AppendLine(String.Format("  <File_ID>{0}</File_ID>", fileId));
                    sb.AppendLine(String.Format("  <File_Type>{0}</File_Type>", type));
                    sb.AppendLine(String.Format("  <Search_Terms>{0}</Search_Terms>", terms));
                    sb.AppendLine("</datum>");
                    string strNewFileName = GetNewLogFilename(HttpContext.Current.Request.MapPath("~/Portals/_default/Logs"), DateTime.Now, "Gafware_DMS_" + pid + "_");

                    WriteToDocLog(HttpContext.Current.Request.MapPath("~/Portals/_default/Logs"), strNewFileName, sb.ToString());
                }
            }
            catch (Exception)
            {
            }
        }

        private string GetNewLogFilename(string strDirectory, DateTime date, string strFilePrefix)
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

        private void WriteToDocLog(string strDirectory, string strLogName, string strNewText)
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

        private static Dictionary<string, Stats> ImportStats = new Dictionary<string, Stats>();

        public static string ImportFiles(string controlPath, string filePath, bool subFolderIsDocumentName, bool subFolderIsTag, bool prependSubFolderName, string seperator, int firstLevel, DateTime? activationDate, DateTime? expirationDate, int ownerId, bool searchable, bool useCategorySecurityRoles, int securityRoleId, int[] categories, int portalId, int tabModuleId, bool portalWideRepository)
        {
            BulkImportThread worker = new BulkImportThread(HttpContext.Current.Request);
            worker.Finished += new EventHandler(BulkImportThread_Finished);
            worker.ImportFiles(controlPath, filePath, subFolderIsDocumentName, subFolderIsTag, prependSubFolderName, seperator, firstLevel, activationDate, expirationDate, ownerId, searchable, useCategorySecurityRoles, securityRoleId, categories, portalId, tabModuleId, portalWideRepository);
            ImportStats.Add(worker.ProcessName, new Stats());
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
                if (ImportStats != null && ImportStats.ContainsKey(worker.ProcessName ?? String.Empty))
                {
                    Stats stats = ImportStats[worker.ProcessName];
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
            if (ImportStats != null && ImportStats.ContainsKey(processName ?? String.Empty))
            {
                if (ProcessProgresses != null && ProcessProgresses.ContainsKey(processName ?? String.Empty))
                {
                    BulkImportThread worker = (BulkImportThread)ProcessProgresses[processName];
                    stats = ImportStats[processName];
                    stats.CheckTime = DateTime.Now;
                    stats.Progress = worker.Progress;
                    stats.FilesProcessed = worker.FilesImported;
                }
                else if (ImportStats != null && ImportStats.ContainsKey(processName ?? String.Empty))
                {
                    stats = ImportStats[processName];
                }
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
                if (ImportStats != null && ImportStats.ContainsKey(processName ?? String.Empty))
                {
                    Stats stats = ImportStats[processName];
                    stats.CheckTime = DateTime.Now;
                    stats.FilesProcessed = filesImported;
                }
            }
            else if (ImportStats != null && ImportStats.ContainsKey(processName ?? String.Empty))
            {
                Stats stats = ImportStats[processName];
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
            ImportStats.Add(worker.ProcessName, new Stats());
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
                if (ImportStats != null && ImportStats.ContainsKey(worker.ProcessName ?? String.Empty))
                {
                    Stats stats = ImportStats[worker.ProcessName];
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
            if (ImportStats != null && ImportStats.ContainsKey(processName ?? String.Empty))
            {
                if (ProcessProgresses != null && ProcessProgresses.ContainsKey(processName ?? String.Empty))
                {
                    DeleteAllThread worker = (DeleteAllThread)ProcessProgresses[processName];
                    stats = ImportStats[processName];
                    stats.CheckTime = DateTime.Now;
                    stats.Progress = worker.Progress;
                    stats.FilesProcessed = worker.FilesDeleted;
                }
                else if (ImportStats != null && ImportStats.ContainsKey(processName ?? String.Empty))
                {
                    stats = ImportStats[processName];
                }
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
                if (ImportStats != null && ImportStats.ContainsKey(processName ?? String.Empty))
                {
                    Stats stats = ImportStats[processName];
                    stats.CheckTime = DateTime.Now;
                    stats.FilesProcessed = filesDeleted;
                }
            }
            else if (ImportStats != null && ImportStats.ContainsKey(processName ?? String.Empty))
            {
                Stats stats = ImportStats[processName];
                filesDeleted = stats.FilesProcessed;
            }
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            Context.Response.Write(oSerializer.Serialize(filesDeleted));
        }
    }
}
