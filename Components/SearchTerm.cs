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
    [XmlType("SearchTerm")]
    [XmlRoot("ContentItem")]
    public class SearchTerm : ContentItem
    {
        /// <summary>
        /// Id of search term
        /// </summary>
        public int SearchTermId { get; set; }
        /// <summary>
        /// Term
        /// </summary>
        public string Term { get; set; }
        /// <summary>
        /// Frequency
        /// </summary>
        public int Frequency { get; set; }
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
        /// Last modify date
        /// </summary>
        public new DateTime LastModifiedOnDate { get; set; }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            SearchTermId = Null.SetNullInteger(dr["SearchTermID"]);
            Term = Null.SetNullString(dr["Term"]);
            Frequency = Null.SetNullInteger(dr["Frequency"]);
            PortalId = Null.SetNullInteger(dr["PortalID"]);
            TabModuleId = Null.SetNullInteger(dr["TabModuleID"]);
            CreatedOnDate = Null.SetNullDateTime(dr["DateAdded"]);
            LastModifiedOnDate = Null.SetNullDateTime(dr["LastDate"]);
        }

        public override int KeyID
        {
            get
            {
                return SearchTermId;
            }
            set
            {
                SearchTermId = value;
            }
        }
    }
}