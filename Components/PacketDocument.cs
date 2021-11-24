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
    [XmlType("PacketDocument")]
    [XmlRoot("ContentItem")]
    public class PacketDocument : ContentItem
    {
        /// <summary>
        /// Id of packet doc
        /// </summary>
        [XmlElement("PacketDocumentID")]
        public int PacketDocId { get; set; }
        /// <summary>
        /// Id of packet
        /// </summary>
        [XmlElement("PacketID")]
        public int PacketId { get; set; }
        /// <summary>
        /// Id of document
        /// </summary>
        [XmlElement("DocumentID")]
        public int DocumentId { get; set; }
        /// <summary>
        /// Id of file
        /// </summary>
        [XmlElement("FileID")]
        public int FileId { get; set; }
        /// <summary>
        /// Sort order
        /// </summary>
        [XmlElement("SortOrder")]
        public int SortOrder { get; set; }
        /// <summary>
        /// Document
        /// </summary>
        [XmlElement("Document")]
        public Document Document { get; set; }

        public PacketDocument() : base()
        {
        }

        public PacketDocument(Document doc, int packetId) : base()
        {
            this.Document = doc;
            this.DocumentId = doc.DocumentId;
            this.PacketId = packetId;
        }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            PacketDocId = Null.SetNullInteger(dr["PacketDocId"]);
            PacketId = Null.SetNullInteger(dr["PacketID"]);
            DocumentId = Null.SetNullInteger(dr["DocumentID"]);
            FileId = Null.SetNullInteger(dr["FileID"]);
            SortOrder = Null.SetNullInteger(dr["SortOrder"]);
            Document = DocumentController.GetDocument(DocumentId);
        }

        public override int KeyID
        {
            get
            {
                return PacketDocId;
            }
            set
            {
                PacketDocId = value;
            }
        }
    }
}