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
    [XmlType("Document")]
    [XmlRoot("ContentItem")]
    public class Document : ContentItem
    {
        /// <summary>
        /// Id of document
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Document name
        /// </summary>
        public string DocumentName { get; set; }
        /// <summary>
        /// Short description
        /// </summary>
        public string ShortDescription { get; set; }
        /// <summary>
        /// Document details
        /// </summary>
        public string DocumentDetails { get; set; }
        /// <summary>
        /// Admin comments
        /// </summary>
        public string AdminComments { get; set; }
        /// <summary>
        /// IP Address of uploader
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// Manager toolkit
        /// </summary>
        public string ManagerToolkit { get; set; }
        /// <summary>
        /// Activation date
        /// </summary>
        public DateTime? ActivationDate { get; set; }
        /// <summary>
        /// ExpirationDate
        /// </summary>
        public DateTime? ExpirationDate { get; set; }
        /// <summary>
        /// Is searchable
        /// </summary>
        public string IsSearchable { get; set; }
        /// <summary>
        /// Use Category Security Roles
        /// </summary>
        public bool UseCategorySecurityRoles { get; set; }
        /// <summary>
        /// Security Role ID
        /// </summary>
        public int SecurityRoleId { get; set; }
        /// <summary>
        /// Portal Id
        /// </summary>
        public int PortalId { get; set; }
        /// <summary>
        /// Tab Module Id
        /// </summary>
        public int TabModuleId { get; set; }
        /// <summary>
        /// Categories
        /// </summary>
        public List<DocumentCategory> Categories { get; set; }
        /// <summary>
        /// Tags
        /// </summary>
        public List<DocumentTag> Tags { get; set; }
        /// <summary>
        /// Files
        /// </summary>
        public new List<DMSFile> Files { get; set; }
        /// <summary>
        /// Created on date
        /// </summary>
        public new DateTime CreatedOnDate { get; set; }
        /// <summary>
        /// Last modify date
        /// </summary>
        public new DateTime LastModifiedOnDate { get; set; }
        /// <summary>
        /// User id of creator
        /// </summary>
        public new int CreatedByUserID { get; set; }
        /// <summary>
        /// Creator
        /// </summary>
        public new UserInfo CreatedByUser
        {
            get
            {
                if (CreatedByUserID > 0)
                {
                    return DotNetNuke.Entities.Users.UserController.GetUserById(PortalId, CreatedByUserID);
                }
                return null;
            }
        }
        /// <summary>
        /// CategoriesRaw
        /// </summary>
        public List<Category> CategoriesRaw { get; set; }

        public DotNetNuke.Security.Roles.RoleInfo SecurityRole
        {
            get
            {
                return UserController.GetRoleById(DotNetNuke.Entities.Portals.PortalController.Instance.GetCurrentSettings().PortalId, SecurityRoleId);
            }
        }

        public Document()
        {
            SecurityRoleId = -1;
            Tags = new List<Components.DocumentTag>();
            Files = new List<Components.DMSFile>();
        }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            DocumentId = Null.SetNullInteger(dr["DocumentID"]);
            CreatedByUserID = Null.SetNullInteger(dr["CreatorID"]);
            DocumentName = Null.SetNullString(dr["DocumentName"]);
            ShortDescription = Null.SetNullString(dr["ShortDescription"]);
            DocumentDetails = Null.SetNullString(dr["DocumentDetails"]);
            AdminComments = Null.SetNullString(dr["AdminComments"]);
            ManagerToolkit = Null.SetNullString(dr["ManagerToolkit"]);
            ActivationDate = (dr["ActivationDate"] == DBNull.Value ? (DateTime?)null : Null.SetNullDateTime(dr["ActivationDate"]));
            ExpirationDate = (dr["ExpirationDate"] == DBNull.Value ? (DateTime?)null : Null.SetNullDateTime(dr["ExpirationDate"]));
            IPAddress = Null.SetNullString(dr["IPAddress"]);
            IsSearchable = Null.SetNullString(dr["IsSearchable"]);
            SecurityRoleId = Null.SetNullInteger(dr["SecurityRoleID"]);
            UseCategorySecurityRoles = Null.SetNullBoolean(dr["UseCategoryRoles"]);
            PortalId = Null.SetNullInteger(dr["PortalID"]);
            TabModuleId = Null.SetNullInteger(dr["TabModuleID"]);
            CreatedOnDate = Null.SetNullDateTime(dr["DateCreated"]);
            LastModifiedOnDate = Null.SetNullDateTime(dr["DateLastModified"]);
            Tags = DocumentController.GetAllTagsForDocument(DocumentId);
            Files = DocumentController.GetAllFilesForDocument(DocumentId);
            Categories = DocumentController.GetAllCategoriesForDocument(DocumentId);
            CategoriesRaw = new List<Category>();
            foreach(DocumentCategory category in Categories)
            {
                CategoriesRaw.Add(category.Category);
            }
        }

        public override int KeyID
        {
            get
            {
                return DocumentId;
            }
            set
            {
                DocumentId = value;
            }
        }

        public void CreateDocumentFolder()
        {
            DotNetNuke.Entities.Portals.PortalInfo portal = DotNetNuke.Entities.Portals.PortalController.Instance.GetPortal(PortalId);
            if (!System.IO.Directory.Exists(string.Format("{0}Files", portal.HomeDirectoryMapPath)))
            {
                System.IO.Directory.CreateDirectory(string.Format("{0}Files", portal.HomeDirectoryMapPath));
            }
            if (!String.IsNullOrEmpty(DocumentName) && !System.IO.Directory.Exists(string.Format("{0}Files\\{1}", portal.HomeDirectoryMapPath, Generic.CreateSafeFolderName(DocumentName))))
            {
                System.IO.Directory.CreateDirectory(string.Format("{0}Files\\{1}", portal.HomeDirectoryMapPath, Generic.CreateSafeFolderName(DocumentName)));
            }
        }
    }
}