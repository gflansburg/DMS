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
    [XmlType("FileVersion")]
    [XmlRoot("ContentItem")]
    public class FileVersion : ContentItem
    {
        /// <summary>
        /// Id of FileVersion
        /// </summary>
        public int FileVersionId { get; set; }
        /// <summary>
        /// Id of File
        /// </summary>
        public int FileId { get; set; }
        /// <summary>
        /// Version number of latest file
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// User id of creator
        /// </summary>
        public new int CreatedByUserID { get; set; }
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
        /// Filesize
        /// </summary>
        public int Filesize { get; set; }
        /// <summary>
        /// Is Lasscape Thumbnail
        /// </summary>
        public bool IsLandscape { get; private set; }
        /// <summary>
        /// Thumbnail
        /// </summary>
        public byte[] Thumbnail { get; set; }
        /// <summary>
        /// Contents
        /// </summary>
        public byte[] Contents { get; set; }
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

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            FileId = Null.SetNullInteger(dr["FileID"]);
            FileVersionId = Null.SetNullInteger(dr["FileVersionID"]);
            Version = Null.SetNullInteger(dr["Version"]);
            CreatedByUserID = Null.SetNullInteger(dr["UploaderID"]);
            WebPageUrl = Null.SetNullString(dr["WebpageURL"]);
            CreatedOnDate = Null.SetNullDateTime(dr["DateUploaded"]);
            IPAddress = Null.SetNullString(dr["IPAddress"]);
            Filesize = Null.SetNullInteger(dr["Filesize"]);
            IsLandscape = Null.SetNullBoolean(dr["IsLandscape"]);
        }

        public override int KeyID
        {
            get
            {
                return FileVersionId;
            }
            set
            {
                FileVersionId = value;
            }
        }

        public void SaveContents()
        {
            Filesize = Contents.Length;
            DocumentController.SaveFileVersion(this);
            DocumentController.SaveFileContents(FileVersionId, Contents);
        }

        public void SaveContents(System.IO.Stream stream)
        {
            Filesize = (int)stream.Length;
            DocumentController.SaveFileVersion(this);
            DocumentController.SaveFileContents(FileVersionId, stream);
        }

        public void SaveContents(byte[] data)
        {
            Contents = data;
            Filesize = Contents.Length;
            DocumentController.SaveFileVersion(this);
            DocumentController.SaveFileContents(FileVersionId, Contents);
        }

        public void LoadContents()
        {
            Contents = DocumentController.GetFileContents(FileVersionId);
        }

        public void SaveThumbnail(bool isLandscape)
        {
            IsLandscape = isLandscape;
            DocumentController.SaveThumbnail(FileVersionId, isLandscape, Thumbnail);
        }

        public void SaveThumbnail(System.IO.Stream stream, bool isLandscape)
        {
            Thumbnail = new byte[stream.Length];
            IsLandscape = isLandscape;
            stream.Read(Thumbnail, 0, (int)stream.Length);
            DocumentController.SaveThumbnail(FileVersionId, isLandscape, Thumbnail);
        }

        public void LoadThumbnail()
        {
            if (Thumbnail == null || Thumbnail.Length == 0)
            {
                Thumbnail = DocumentController.GetThumbnail(FileVersionId);
            }
        }
    }
}