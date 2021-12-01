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
    [XmlType("Tag")]
    [XmlRoot("ContentItem")]
    public class Tag : ContentItem
    {
        /// <summary>
        /// Id of tag
        /// </summary>
        public int TagId { get; set; }
        /// <summary>
        /// Tag name
        /// </summary>
        public string TagName { get; set; }
        /// <summary>
        /// Weight (order)
        /// </summary>
        public int Weight { get; set; }
        /// <summary>
        /// Portal id
        /// </summary>
        public int PortalId { get; set; }
        /// <summary>
        /// Tab Module id
        /// </summary>
        public int TabModuleId { get; set; }
        /// <summary>
        /// Document count
        /// </summary>
        public int DocumentCount { get; set; }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            TagId = Null.SetNullInteger(dr["TagId"]);
            TagName = Null.SetNullString(dr["TagName"]);
            Weight = Null.SetNullInteger(dr["Weight"]);
            DocumentCount = Null.SetNullInteger(dr["DocumentCount"]);
            PortalId = Null.SetNullInteger(dr["PortalID"]);
            TabModuleId = Null.SetNullInteger(dr["TabModuleID"]);
        }

        public override int KeyID
        {
            get
            {
                return TagId;
            }
            set
            {
                TagId = value;
            }
        }
    }
}