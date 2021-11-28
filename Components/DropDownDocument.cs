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
    [XmlType("DropDownDocument")]
    [XmlRoot("ContentItem")]
    public class DropDownDocument : ContentItem
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
        /// Activation date
        /// </summary>
        public DateTime? ActivationDate { get; set; }
        /// <summary>
        /// ExpirationDate
        /// </summary>
        public DateTime? ExpirationDate { get; set; }
        /// <summary>
        /// Id of portal
        /// </summary>
        public int PortalId { get; set; }
        /// <summary>
        /// Last modify date
        /// </summary>
        public new DateTime LastModifiedOnDate { get; set; }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            DocumentId = Null.SetNullInteger(dr["DocumentID"]);
            DocumentName = Null.SetNullString(dr["DocumentName"]);
            PortalId = Null.SetNullInteger(dr["PortalID"]);
            ActivationDate = (dr["ActivationDate"] == DBNull.Value ? (DateTime?)null : Null.SetNullDateTime(dr["ActivationDate"]));
            ExpirationDate = (dr["ExpirationDate"] == DBNull.Value ? (DateTime?)null : Null.SetNullDateTime(dr["ExpirationDate"]));
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