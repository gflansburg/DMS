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
    [XmlType("ReportDocument")]
    public class DocumentActivity
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
        /// TabModule Id
        /// </summary>
        public int TabModuleId { get; set; }
        /// <summary>
        /// Date of activity
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// IP Addresss of requestor
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// Id of file
        /// </summary>
        public int FileId { get; set; }
        /// <summary>
        /// Filename
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// File type
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// Search terms
        /// </summary>
        public string SearchTerms { get; set; }
    }
}