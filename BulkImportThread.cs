using Gafware.Modules.DMS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ghostscript.NET;
using Ghostscript.NET.Processor;
using DotNetNuke.Entities.Portals;

namespace Gafware.Modules.DMS
{
    public class BulkImportThread
    {
        private System.Threading.Thread _worker = null;
        private string _processName = Guid.NewGuid().ToString();
        public event EventHandler Finished;

        public HttpRequest Request { get; private set; }
        public int PortalId { get; private set; }
        public int TabModuleId { get; private set; }
        public bool PortalWideRepository { get; private set; }
        public string ControlPath { get; private set; }
        public string FilePath { get; private set; }
        public bool SubFolderIsDocumentName { get; private set; }
        public bool SubFolderIsTag { get; private set; }
        public bool PrependSubFolderName { get; private set; }
        public string Seperator { get; private set; }
        public int FirstLevel { get; private set; }
        public DateTime? ActivationDate { get; private set; }
        public DateTime? ExpirationDate { get; private set; }
        public int OwnerId { get; private set; }
        public bool IsPublic { get; private set; }
        public bool Searchable { get; private set; }
        public bool UseCategorySecurityRoles { get; private set; }
        public int SecurityRoleId { get; private set; }
        public int[] Categories { get; private set; }
        public bool ReplacePDFTitle { get; private set; }
        public string[] SearchPatterns { get; private set; }
        public int Progress { get; private set; }
        public int FilesImported { get; private set; }
        public int FileCount { get; private set; }

        public BulkImportThread(HttpRequest request)
        {
            Request = request;
        }

        public string ProcessName
        {
            get
            {
                return _processName;
            }
        }

        public void ImportFiles(string controlPath, string filePath, bool subFolderIsDocumentName, bool subFolderIsTag, bool prependSubFolderName, string seperator, int firstLevel, DateTime? activationDate, DateTime? expirationDate, int ownerId, bool searchable, bool useCategorySecurityRoles, int securityRoleId, int[] categories, bool replacePDFTitle, bool isPublic, int portalId, int tabModuleId, bool portalWideRepository)
        {
            PortalId = portalId;
            TabModuleId = tabModuleId;
            PortalWideRepository = portalWideRepository;
            ControlPath = controlPath;
            FilePath = filePath;
            SubFolderIsDocumentName = subFolderIsDocumentName;
            SubFolderIsTag = subFolderIsTag;
            PrependSubFolderName = prependSubFolderName;
            Seperator = seperator;
            FirstLevel = firstLevel;
            ActivationDate = activationDate;
            ExpirationDate = expirationDate;
            OwnerId = ownerId;
            Searchable = searchable;
            UseCategorySecurityRoles = useCategorySecurityRoles;
            SecurityRoleId = securityRoleId;
            Categories = categories;
            ReplacePDFTitle = replacePDFTitle;
            IsPublic = isPublic;
            _worker = new System.Threading.Thread(new System.Threading.ThreadStart(DoBulkImport));
            _worker.Start();
        }

        private void DoBulkImport()
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
            SearchPatterns = searchPatterns.ToString().Split('|');
            FileCount = GetFileCount(FilePath);
            ImportFiles(FilePath);
            Finished?.Invoke(this, EventArgs.Empty);
        }

        private int GetFileCount(string filePath)
        {
            int fileCount = 0;
            string[] folders = System.IO.Directory.GetDirectories(filePath);
            string[] files = SearchPatterns.SelectMany(filter => System.IO.Directory.GetFiles(filePath, filter)).ToArray();
            foreach(string folder in folders)
            {
                fileCount += GetFileCount(folder);
            }
            fileCount += files.Length;
            return fileCount;
        }

        public void ImportFiles(string filePath, int level = 0, string parent = "", string tagName = "")
        {
            string[] folders = System.IO.Directory.GetDirectories(filePath);
            string[] files = SearchPatterns.SelectMany(filter => System.IO.Directory.GetFiles(filePath, filter)).ToArray();
            if (string.IsNullOrEmpty(tagName) && level >= FirstLevel && SubFolderIsTag)
            {
                tagName = System.IO.Path.GetFileName(filePath).Replace("_", " ");
            }
            foreach (string folder in folders)
            {
                ImportFiles(folder, level + 1, (level >= FirstLevel && (SubFolderIsTag || PrependSubFolderName) ? (!string.IsNullOrEmpty(parent) ? parent + Seperator : string.Empty) + System.IO.Path.GetFileName(filePath) : string.Empty), tagName);
            }
            Document doc = new Document();
            if (SubFolderIsDocumentName && files.Length > 0)
            {
                string documentName = (level >= FirstLevel && PrependSubFolderName ? (!String.IsNullOrEmpty(parent) ? parent.Replace("_", " ") + Seperator : string.Empty) : string.Empty) + System.IO.Path.GetFileName(filePath).Replace("_", " ");
                doc = Components.DocumentController.GetDocumentByName(documentName, PortalId, PortalWideRepository ? 0 : TabModuleId);
                if (doc == null)
                {
                    doc.Categories = new List<DocumentCategory>();
                    doc.Files = new List<Components.DMSFile>();
                    doc.Tags = new List<DocumentTag>();
                    doc.CategoriesRaw = new List<Category>();
                    doc.PortalId = PortalId;
                    doc.TabModuleId = TabModuleId;
                    doc.ActivationDate = ActivationDate;
                    doc.AdminComments = string.Empty;
                    doc.CreatedByUserID = OwnerId;
                    doc.DocumentDetails = documentName;
                    doc.ExpirationDate = ExpirationDate;
                    doc.IsSearchable = Searchable;
                    doc.UseCategorySecurityRoles = UseCategorySecurityRoles;
                    doc.SecurityRoleId = SecurityRoleId;
                    doc.IsPublic = IsPublic;
                    doc.DocumentName = documentName;
                    doc.IPAddress = Generic.GetIPAddress();
                    Components.DocumentController.SaveDocument(doc);
                }
                foreach (int categoryId in Categories)
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
                if (SubFolderIsTag)
                {
                    AddTag(doc, tagName);
                }
            }
            foreach (string file in files)
            {
                if (!SubFolderIsDocumentName)
                {
                    string documentName = (level >= FirstLevel && PrependSubFolderName ? (!String.IsNullOrEmpty(parent) ? parent.Replace("_", " ") + Seperator : string.Empty) + System.IO.Path.GetFileName(filePath).Replace("_", " ") + Seperator : string.Empty) + System.IO.Path.GetFileNameWithoutExtension(file).Replace("_", " ");
                    doc = Components.DocumentController.GetDocumentByName(documentName, PortalId, PortalWideRepository ? 0 : TabModuleId);
                    if (doc == null)
                    {
                        doc = new Document();
                        doc.Categories = new List<DocumentCategory>();
                        doc.Files = new List<Components.DMSFile>();
                        doc.Tags = new List<DocumentTag>();
                        doc.CategoriesRaw = new List<Category>();
                        doc.PortalId = PortalId;
                        doc.TabModuleId = TabModuleId;
                        doc.ActivationDate = ActivationDate;
                        doc.AdminComments = string.Empty;
                        doc.CreatedByUserID = OwnerId;
                        doc.DocumentDetails = documentName;
                        doc.ExpirationDate = ExpirationDate;
                        doc.IsSearchable = Searchable;
                        doc.UseCategorySecurityRoles = UseCategorySecurityRoles;
                        doc.SecurityRoleId = SecurityRoleId;
                        doc.IsPublic = IsPublic;
                        doc.DocumentName = documentName;
                        doc.IPAddress = Generic.GetIPAddress();
                        Components.DocumentController.SaveDocument(doc);
                        foreach (int categoryId in Categories)
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
                        if (SubFolderIsTag)
                        {
                            AddTag(doc, tagName);
                        }
                    }
                }
                if (System.IO.Path.GetExtension(file).Equals(".pdf", StringComparison.OrdinalIgnoreCase) && ReplacePDFTitle)
                {
                    string tempPdf = String.Format("{0}{1}.pdf", System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(file));
                    try
                    {
                        string keywords = string.Join(", ", (from tag in doc.Tags select tag.Tag.TagName).ToList());
                        using (Spire.Pdf.PdfDocument pdfDoc = new Spire.Pdf.PdfDocument(file))
                        {
                            pdfDoc.DocumentInformation.Title = doc.DocumentName;
                            pdfDoc.DocumentInformation.Keywords = keywords;
                            pdfDoc.SaveToFile(tempPdf);
                            if(System.IO.File.Exists(tempPdf))
                            {
                                AddFile(doc, tempPdf);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        AddFile(doc, file);
                    }
                    finally
                    {
                        if (System.IO.File.Exists(tempPdf))
                        {
                            try
                            {
                                System.IO.File.Delete(tempPdf);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                else
                {
                    AddFile(doc, file);
                }
            }
        }

        private void AddFile(Components.Document doc, string file)
        {
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
                        dmsFile.UploadDirectory = string.Format("Files/{0}/{1}", TabModuleId, Generic.CreateSafeFolderName(doc.DocumentName));
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
                    dmsFile.FileVersion.IPAddress = Generic.GetIPAddress();
                    dmsFile.FileVersion.CreatedByUserID = OwnerId;
                    dmsFile.FileVersion.CreatedOnDate = DateTime.Now;
                    dmsFile.FileVersion.Filesize = (int)stream.Length;
                    Components.DocumentController.SaveFileVersion(dmsFile.FileVersion);
                    dmsFile.FileVersionId = dmsFile.FileVersion.FileVersionId;
                    Components.DocumentController.SaveFile(dmsFile);
                    dmsFile.FileVersion.SaveContents(stream);
                    Components.Repository repository = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                    if (repository != null)
                    {
                        CreateThumbnail(repository, file, dmsFile.FileVersionId);
                    }
                    Progress = (int)(((double)FilesImported * 100.0) / (double)FileCount);
                    FilesImported++;
                }
            }
        }

        private void CreateThumbnail(Components.Repository repository, string file, int fileVersionId)
        {
            DotNetNuke.Entities.Portals.PortalInfo portal = DotNetNuke.Entities.Portals.PortalController.Instance.GetPortal(PortalId);
            Thumbnail thumb = new Thumbnail(portal, repository, ControlPath);
            thumb.CreateThumbnail(Request, file, fileVersionId);
        }

        private void AddTag(Document doc, string tagName)
        {
            Components.Tag tag = Components.DocumentController.GetTagByTagName(tagName, PortalId, PortalWideRepository ? 0 : TabModuleId);
            if (tag == null || tag.TagId == 0)
            {
                tag = new Components.Tag();
                tag.TagName = tagName;
                tag.PortalId = PortalId;
                tag.TabModuleId = TabModuleId;
                Components.DocumentController.SaveTag(tag);
            }
            if (!doc.Tags.Exists(t => t.TagId == tag.TagId))
            {
                Components.DocumentTag docTag = new Components.DocumentTag();
                docTag.DocumentId = doc.DocumentId;
                docTag.TagId = tag.TagId;
                Components.DocumentController.SaveDocumentTag(docTag);
            }
        }
    }
}