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
        /// IsPublic
        /// </summary>
        public bool IsPublic { get; set; }
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
        public bool IsSearchable { get; set; }
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
                if (CreatedByUserID > 0 && !IsGroupOwner)
                {
                    return DotNetNuke.Entities.Users.UserController.GetUserById(PortalId, CreatedByUserID);
                }
                return null;
            }
        }
        /// <summary>
        /// Is owner a group id
        /// </summary>
        public bool IsGroupOwner { get; set; }
        /// <summary>
        /// Group owner
        /// </summary>
        public DotNetNuke.Security.Roles.RoleInfo Group
        {
            get
            {
                if(CreatedByUserID > 0 && IsGroupOwner)
                {
                    return DotNetNuke.Security.Roles.RoleController.Instance.GetRoleById(PortalId, CreatedByUserID);
                }
                return null;
            }
        }
        private List<Category> _categoriesRaw = null;
        /// <summary>
        /// CategoriesRaw
        /// </summary>
        public List<Category> CategoriesRaw 
        { 
            get
            {
                if(_categoriesRaw == null)
                {
                    _categoriesRaw = new List<Category>();
                    foreach (DocumentCategory category in Categories)
                    {
                        _categoriesRaw.Add(category.Category);
                    }
                }
                return _categoriesRaw;
            }
            set
            {
                _categoriesRaw = value;
            }
        }

        public DotNetNuke.Security.Roles.RoleInfo SecurityRole
        {
            get
            {
                return UserController.GetRoleById(DotNetNuke.Entities.Portals.PortalController.Instance.GetCurrentSettings().PortalId, SecurityRoleId);
            }
        }

        private List<DMSFile> _files = null;
        /// <summary>
        /// Files
        /// </summary>
        public new List<DMSFile> Files
        {
            get
            {
                if (_files == null)
                {
                    _files = DocumentController.GetAllFilesForDocument(DocumentId);
                }
                return _files;
            }
            set
            {
                _files = value;
            }
        }

        private List<DocumentCategory> _categories = null;
        /// <summary>
        /// Categories
        /// </summary>
        public List<DocumentCategory> Categories
        {
            get
            {
                if (_categories == null)
                {
                    _categories = DocumentController.GetAllCategoriesForDocument(DocumentId);
                }
                return _categories;
            }
            set
            {
                _categories = value;
            }
        }

        private List<DocumentTag> _tags = null;
        /// <summary>
        /// Tags
        /// </summary>
        public List<DocumentTag> Tags 
        { 
            get
            {
                if(_tags == null)
                {
                    _tags = DocumentController.GetAllTagsForDocument(DocumentId);
                }
                return _tags;
            }
            set
            {
                _tags = value;
            }
        }

        public Document()
        {
            SecurityRoleId = -1;
            IsPublic = true;
            IsSearchable = true;
        }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            DocumentId = Null.SetNullInteger(dr["DocumentID"]);
            CreatedByUserID = Null.SetNullInteger(dr["CreatorID"]);
            IsGroupOwner = Null.SetNullBoolean(dr["IsGroupOwner"]);
            DocumentName = Null.SetNullString(dr["DocumentName"]);
            DocumentDetails = Null.SetNullString(dr["DocumentDetails"]);
            AdminComments = Null.SetNullString(dr["AdminComments"]);
            IsPublic = Null.SetNullBoolean(dr["IsPublic"]);
            ActivationDate = (dr["ActivationDate"] == DBNull.Value ? (DateTime?)null : Null.SetNullDateTime(dr["ActivationDate"]));
            ExpirationDate = (dr["ExpirationDate"] == DBNull.Value ? (DateTime?)null : Null.SetNullDateTime(dr["ExpirationDate"]));
            IPAddress = Null.SetNullString(dr["IPAddress"]);
            IsSearchable = Null.SetNullBoolean(dr["IsSearchable"]);
            SecurityRoleId = Null.SetNullInteger(dr["SecurityRoleID"]);
            UseCategorySecurityRoles = Null.SetNullBoolean(dr["UseCategoryRoles"]);
            PortalId = Null.SetNullInteger(dr["PortalID"]);
            TabModuleId = Null.SetNullInteger(dr["TabModuleID"]);
            CreatedOnDate = Null.SetNullDateTime(dr["DateCreated"]);
            LastModifiedOnDate = Null.SetNullDateTime(dr["DateLastModified"]);
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