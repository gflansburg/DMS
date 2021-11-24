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
    [XmlType("DocumentTag")]
    [XmlRoot("ContentItem")]
    public class DocumentTag : ContentItem
    {
        /// <summary>
        /// Id of document tag
        /// </summary>
        public int DocumentTagId { get; set; }
        /// <summary>
        /// Id of document
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Id of tag
        /// </summary>
        public int TagId { get; set; }
        /// <summary>
        /// Tag
        /// </summary>
        public Tag Tag { get; set; }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            DocumentTagId = Null.SetNullInteger(dr["DocumentTagID"]);
            DocumentId = Null.SetNullInteger(dr["DocumentID"]);
            TagId = Null.SetNullInteger(dr["TagID"]);
            Tag = DocumentController.GetTag(TagId);
        }

        public override int KeyID
        {
            get
            {
                return DocumentTagId;
            }
            set
            {
                DocumentTagId = value;
            }
        }
    }
}