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
    [XmlType("SearchResult")]
    [XmlRoot("ContentItem")]
    public class SearchResult : ContentItem
    {
        /// <summary>
        /// Id of search term
        /// </summary>
        public int SearchId { get; set; }
        /// <summary>
        /// Search terms
        /// </summary>
        public string SearchTerms { get; set; }
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
        /// Result count
        /// </summary>
        public int ResultCount { get; set; }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            SearchId = Null.SetNullInteger(dr["SearchId"]);
            SearchTerms = Null.SetNullString(dr["SearchTerms"]);
            PortalId = Null.SetNullInteger(dr["PortalID"]);
            TabModuleId = Null.SetNullInteger(dr["TabModuleID"]);
            CreatedOnDate = Null.SetNullDateTime(dr["SearchDate"]);
            ResultCount = Null.SetNullInteger(dr["ResultCount"]);
        }

        public override int KeyID
        {
            get
            {
                return SearchId;
            }
            set
            {
                SearchId = value;
            }
        }
    }
}