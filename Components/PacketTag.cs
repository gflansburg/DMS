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
    [XmlType("PacketTag")]
    [XmlRoot("ContentItem")]
    public class PacketTag : ContentItem
    {
        /// <summary>
        /// Id of packet tag
        /// </summary>
        public int PacketTagId { get; set; }
        /// <summary>
        /// Id of packet
        /// </summary>
        public int PacketId { get; set; }
        /// <summary>
        /// Id of tag
        /// </summary>
        public int TagId { get; set; }
        /// <summary>
        /// Sort order
        /// </summary>
        public int SortOrder { get; set; }
        /// <summary>
        /// Tag
        /// </summary>
        public Tag Tag { get; set; }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            PacketTagId = Null.SetNullInteger(dr["PacketTagID"]);
            PacketId = Null.SetNullInteger(dr["PacketID"]);
            TagId = Null.SetNullInteger(dr["TagID"]);
            SortOrder = Null.SetNullInteger(dr["SortOrder"]);
            Tag = DocumentController.GetTag(TagId);
        }

        public override int KeyID
        {
            get
            {
                return PacketTagId;
            }
            set
            {
                PacketTagId = value;
            }
        }
    }
}