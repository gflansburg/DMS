using System;
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
        /// Portal Id
        /// </summary>
        public int PortalId { get; set; }
        /// <summary>
        /// Date of last modification
        /// </summary>
        public new DateTime LastModifiedOnDate { get; set; }
        /// <summary>
        /// CategoriesRaw
        /// </summary>
        //public List<Category> CategoriesRaw { get; set; }
        /// <summary>
        /// Creator ID
        /// </summary>
        new public int CreatedByUserID { get; set; }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            DocumentId = Null.SetNullInteger(dr["DocumentID"]);
            CreatedByUserID = Null.SetNullInteger(dr["CreatorID"]);
            DocumentName = Null.SetNullString(dr["DocumentName"]);
            PortalId = Null.SetNullInteger(dr["PortalID"]);
            LastModifiedOnDate = Null.SetNullDateTime(dr["DateLastModified"]);
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