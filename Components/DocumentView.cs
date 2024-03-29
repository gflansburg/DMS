﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content;
using Gafware.Modules.DMS.Data;
using System.Xml.Serialization;

namespace Gafware.Modules.DMS.Components
{
    [Serializable]
    [XmlType("DocumentView")]
    [XmlRoot("ContentItem")]
    public class DocumentView : ContentItem
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
        /// IsPublic
        /// </summary>
        public bool IsPublic { get; set; }
        /// <summary>
        /// Is searchable
        /// </summary>
        public bool IsSearchable { get; set; }
        /// <summary>
        /// Portal Id
        /// </summary>
        public int PortalId { get; set; }
        /// <summary>
        /// TabModule Id
        /// </summary>
        public int TabModuleId { get; set; }
        /// <summary>
        /// Date of last modification
        /// </summary>
        public new DateTime LastModifiedOnDate { get; set; }
        /// <summary>
        /// Activation date
        /// </summary>
        public DateTime? ActivationDate { get; set; }
        /// <summary>
        /// ExpirationDate
        /// </summary>
        public DateTime? ExpirationDate { get; set; }
        /// <summary>
        /// CategoriesRaw
        /// </summary>
        //public List<Category> CategoriesRaw { get; set; }
        /// <summary>
        /// Creator ID
        /// </summary>
        new public int CreatedByUserID { get; set; }
        /// <summary>
        /// Is owner a group
        /// </summary>
        public bool IsGroupOwner { get; set; }
        /// <summary>
        /// Created on date
        /// </summary>
        public new DateTime CreatedOnDate { get; set; }
        /// <summary>
        /// Owner
        /// </summary>
        public string Owner
        {
            get
            {
                if (CreatedByUserID > 0 && !IsGroupOwner)
                {
                    return DotNetNuke.Entities.Users.UserController.GetUserById(PortalId, CreatedByUserID).DisplayName;
                }
                if (CreatedByUserID > 0 && IsGroupOwner)
                {
                    return DotNetNuke.Security.Roles.RoleController.Instance.GetRoleById(PortalId, CreatedByUserID).RoleName;
                }
                return string.Empty;
            }
        }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            DocumentId = Null.SetNullInteger(dr["DocumentID"]);
            CreatedByUserID = Null.SetNullInteger(dr["CreatorID"]);
            IsGroupOwner = Null.SetNullBoolean(dr["IsGroupOwner"]);
            DocumentName = Null.SetNullString(dr["DocumentName"]);
            DocumentDetails = Null.SetNullString(dr["DocumentDetails"]);
            IsPublic = Null.SetNullBoolean(dr["IsPublic"]);
            IsSearchable = Null.SetNullBoolean(dr["IsSearchable"]);
            PortalId = Null.SetNullInteger(dr["PortalID"]);
            TabModuleId = Null.SetNullInteger(dr["TabModuleID"]);
            LastModifiedOnDate = Null.SetNullDateTime(dr["DateLastModified"]);
            CreatedOnDate = Null.SetNullDateTime(dr["DateCreated"]);
            ActivationDate = (dr["ActivationDate"] == DBNull.Value ? (DateTime?)null : Null.SetNullDateTime(dr["ActivationDate"]));
            ExpirationDate = (dr["ExpirationDate"] == DBNull.Value ? (DateTime?)null : Null.SetNullDateTime(dr["ExpirationDate"]));
            /*List<DocumentCategory> categories = DocumentController.GetAllCategoriesForDocument(DocumentId);
             CategoriesRaw = new List<Category>();
             foreach(DocumentCategory category in categories)
             {
                 CategoriesRaw.Add(category.Category);
             }*/
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