using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content;
using Gafware.Modules.DMS.Data;
using System.Xml.Serialization;
using DotNetNuke.Entities.Users;

namespace Gafware.Modules.DMS.Components
{
    [Serializable]
    [XmlType("File")]
    [XmlRoot("ContentItem")]
    public class DMSFile : ContentItem
    {
        /// <summary>
        /// Id of File
        /// </summary>
        public int FileId { get; set; }
        /// <summary>
        /// Id of of latest version of content
        /// </summary>
        public int FileVersionId { get; set; }
        /// <summary>
        /// Version number of latest file
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// Id of status
        /// </summary>
        public int StatusId { get; set; }
        /// <summary>
        /// Id of document
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// User id of creator
        /// </summary>
        public new int CreatedByUserID { get; set; }
        /// <summary>
        /// Upload directory
        /// </summary>
        public string UploadDirectory { get; set; }
        /// <summary>
        /// File type
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// Filename
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// Webpage URL
        /// </summary>
        public string WebPageUrl { get; set; }
        /// <summary>
        /// Created on date
        /// </summary>
        public new DateTime CreatedOnDate { get; set; }
        /// <summary>
        /// IP Address of uploader
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// Mimetype
        /// </summary>
        public string MimeType { get; set; }
        /// <summary>
        /// Filesize
        /// </summary>
        public int Filesize { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public Status Status
        {
            get
            {
                return DocumentController.GetStatus(StatusId);
            }
        }
        private FileVersion _fileVersion = null;
        /// <summary>
        /// FileVersion
        /// </summary>
        public FileVersion FileVersion 
        { 
            get
            {
                if(_fileVersion == null)
                {
                    _fileVersion = DocumentController.GetFileVersion(FileVersionId);
                }
                return _fileVersion;
            }
            set
            {
                _fileVersion = value;
            }
        }
        /// <summary>
        /// Uploader
        /// </summary>
        public new UserInfo CreatedByUser
        {
            get
            {
                return DotNetNuke.Entities.Users.UserController.GetUserById(DotNetNuke.Entities.Portals.PortalController.Instance.GetCurrentSettings().PortalId, CreatedByUserID);
            }
        }
        /// <summary>
        /// History
        /// </summary>
        public List<FileVersion> History
        {
            get
            {
                return DocumentController.GetFileVersions(FileId);
            }
        }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            FileId = Null.SetNullInteger(dr["FileID"]);
            FileVersionId = Null.SetNullInteger(dr["FileVersionID"]);
            Version = Null.SetNullInteger(dr["Version"]);
            StatusId = Null.SetNullInteger(dr["StatusID"]);
            DocumentId = Null.SetNullInteger(dr["DocumentID"]);
            CreatedByUserID = Null.SetNullInteger(dr["UploaderID"]);
            UploadDirectory = Null.SetNullString(dr["UploadDirectory"]);
            FileType = Null.SetNullString(dr["FileType"]);
            Filename = Null.SetNullString(dr["FileName"]);
            WebPageUrl = Null.SetNullString(dr["WebpageURL"]);
            CreatedOnDate = Null.SetNullDateTime(dr["DateUploaded"]);
            IPAddress = Null.SetNullString(dr["IPAddress"]);
            MimeType = Null.SetNullString(dr["MimeType"]);
            Filesize = Null.SetNullInteger(dr["Filesize"]);
        }

        public override int KeyID
        {
            get
            {
                return FileId;
            }
            set
            {
                FileId = value;
            }
        }

        public void CreateFolder()
        {
            DotNetNuke.Entities.Portals.PortalSettings portal = DotNetNuke.Entities.Portals.PortalSettings.Current;
            if (!System.IO.Directory.Exists(string.Format("{0}Files", portal.HomeDirectoryMapPath)))
            {
                System.IO.Directory.CreateDirectory(string.Format("{0}Files", portal.HomeDirectoryMapPath));
            }
            if (!String.IsNullOrEmpty(UploadDirectory) && !System.IO.Directory.Exists(string.Format("{0}\\{1}", portal.HomeDirectoryMapPath, UploadDirectory)))
            {
                System.IO.Directory.CreateDirectory(string.Format("{0}\\{1}", portal.HomeDirectoryMapPath, UploadDirectory));
            }
        }

        public static string GetFileType(string extension, int portalId, int tabModuleId)
        {
            string ext = extension;
            if(extension.StartsWith("."))
            {
                ext = extension.Substring(1);
            }
            List<FileType> fileTypes = DocumentController.GetAllFileTypes(portalId, tabModuleId);
            FileType fileType = fileTypes.FirstOrDefault(p => p.FileTypeExt.Split(',').Contains(ext));
            if (fileType != null)
            {
                string[] types = fileType.FileTypeExt.Split(',');
                if (types.Length > 0)
                {
                    return types[0];
                }
            }
            return ext;
        }
    }
}