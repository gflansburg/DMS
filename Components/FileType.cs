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
    [XmlType("FileType")]
    [XmlRoot("ContentItem")]
    public class FileType : ContentItem
    {
        /// <summary>
        /// Id of file type
        /// </summary>
        public int FileTypeId { get; set; }
        /// <summary>
        /// File type name
        /// </summary>
        public string FileTypeName { get; set; }
        /// <summary>
        /// File type shor tname
        /// </summary>
        public string FileTypeShortName { get; set; }
        /// <summary>
        /// File type extentions
        /// </summary>
        public string FileTypeExt { get; set; }
        /// <summary>
        /// Id of portal
        /// </summary>
        public int PortalId { get; set; }
        /// <summary>
        /// Id of Tab Module
        /// </summary>
        public int TabModuleId { get; set; }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            FileTypeId = Null.SetNullInteger(dr["FileTypeID"]);
            FileTypeName = Null.SetNullString(dr["FileTypeName"]);
            FileTypeShortName = Null.SetNullString(dr["FileTypeShortName"]);
            FileTypeExt = Null.SetNullString(dr["FileTypeExt"]);
            PortalId = Null.SetNullInteger(dr["PortalId"]);
            TabModuleId = Null.SetNullInteger(dr["TabModuleID"]);
        }

        public override int KeyID
        {
            get
            {
                return FileTypeId;
            }
            set
            {
                FileTypeId = value;
            }
        }
    }
}