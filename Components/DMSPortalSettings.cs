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
    [XmlType("DMSPortalSettings")]
    [XmlRoot("ContentItem")]
    public class DMSPortalSettings : ContentItem
    {
        /// <summary>
        /// Portal id
        /// </summary>
        public int PortalId { get; set; }
        /// <summary>
        /// Portal wide repository
        /// </summary>
        public bool PortalWideRepository { get; set; }
        /// <summary>
        /// Enable DNN Search
        /// </summary>
        public bool EnableDNNSearch { get; set; }
        
        public DMSPortalSettings()
        {
            PortalWideRepository = true;
        }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            PortalId = Null.SetNullInteger(dr["PortalID"]);
            PortalWideRepository = Null.SetNullBoolean(dr["PortalWideRepository"]);
            EnableDNNSearch = Null.SetNullBoolean(dr["EnableDNNSearch"]);
        }

        public override int KeyID
        {
            get
            {
                return PortalId;
            }
            set
            {
                PortalId = value;
            }
        }
    }
}