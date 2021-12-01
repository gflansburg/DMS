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
    [XmlType("Repository")]
    [XmlRoot("ContentItem")]
    public class Repository : ContentItem
    {
        /// <summary>
        /// Id of the repository
        /// </summary>
        public int RepositoryId { get; set; }
        /// <summary>
        /// Name of repository
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id of user role
        /// </summary>
        public int UserRoleId { get; set; }
        /// <summary>
        /// Id of file notifications role
        /// </summary>
        public int FileNotificationsRoleId { get; set; }
        /// <summary>
        /// New file subject
        /// </summary>
        public string NewFileSubject { get; set; }
        /// <summary>
        /// New file message
        /// </summary>
        public string NewFileMsg { get; set; }
        /// <summary>
        /// Portal id
        /// </summary>
        public int PortalId { get; set; }
        /// <summary>
        /// Tab Module id
        /// </summary>
        public int TabModuleId { get; set; }
        /// <summary>
        /// Category Name
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// Save local file
        /// </summary>
        public bool SaveLocalFile { get; set; }
        /// <summary>
        /// Show tips
        /// </summary>
        public bool ShowTips { get; set; }
        /// <summary>
        /// Show instructions
        /// </summary>
        public bool ShowInstructions { get; set; }
        /// <summary>
        /// Instructions
        /// </summary>
        public string Instructions { get; set; }
        /// <summary>
        /// Theme
        /// </summary>
        public string Theme { get; set; }
        /// <summary>
        /// Tumbnail Type
        /// </summary>
        public string ThumbnailType { get; set; }
        /// <summary>
        /// Page Size
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Tumbnail Size
        /// </summary>
        public int ThumbnailSize { get; set; }

        public Repository()
        {
            UserRoleId = -2;
            FileNotificationsRoleId = -2;
            ShowTips = true;
            ShowInstructions = true;
            PageSize = 20;
        }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            RepositoryId = Null.SetNullInteger(dr["RepositoryId"]);
            Name = Null.SetNullString(dr["Name"]);
            UserRoleId = Null.SetNullInteger(dr["UserRoleID"]);
            FileNotificationsRoleId = Null.SetNullInteger(dr["FileNotificationsRoleID"]);
            NewFileSubject = Null.SetNullString(dr["NewFileSubject"]);
            NewFileMsg = Null.SetNullString(dr["NewFileBody"]);
            PortalId = Null.SetNullInteger(dr["PortalID"]);
            TabModuleId = Null.SetNullInteger(dr["TabModuleID"]);
            CategoryName = Null.SetNullString(dr["CategoryName"]);
            SaveLocalFile = Null.SetNullBoolean(dr["SaveLocalFile"]);
            ShowTips = Null.SetNullBoolean(dr["ShowTips"]);
            ShowInstructions = Null.SetNullBoolean(dr["ShowInstructions"]);
            Instructions = Null.SetNullString(dr["Instructions"]);
            Theme = Null.SetNullString(dr["Theme"]);
            ThumbnailType = Null.SetNullString(dr["ThumbnailType"]);
            ThumbnailSize = Null.SetNullInteger(dr["ThumbnailSize"]);
            PageSize = Null.SetNullInteger(dr["PageSize"]);
        }

        public override int KeyID
        {
            get
            {
                return RepositoryId;
            }
            set
            {
                RepositoryId = value;
            }
        }
    }
}