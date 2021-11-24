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
    [XmlType("Status")]
    [XmlRoot("ContentItem")]
    public class Status : ContentItem
    {
        /// <summary>
        /// Id of status
        /// </summary>
        public int StatusId { get; set; }
        /// <summary>
        /// Status name
        /// </summary>
        public string StatusName { get; set; }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            StatusId = Null.SetNullInteger(dr["StatusID"]);
            StatusName = Null.SetNullString(dr["StatusName"]);
        }

        public override int KeyID
        {
            get
            {
                return StatusId;
            }
            set
            {
                StatusId = value;
            }
        }
    }
}