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
        /// <summary>
        /// Force HTTPS
        /// </summary>
        public bool ForceHttps { get; set; }
        /// <summary>
        /// Use Thumbnails
        /// </summary>
        public bool UseThumbnails { get; set; }
        /// <summary>
        /// Create PDF Thumbnails
        /// </summary>
        public bool CreatePDF { get; set; }
        /// <summary>
        /// Create Word Thumbnails
        /// </summary>
        public bool CreateWord { get; set; }
        /// <summary>
        /// Create Excel Thumbnails
        /// </summary>
        public bool CreateExcel { get; set; }
        /// <summary>
        /// Create PowerPoint Thumbnails
        /// </summary>
        public bool CreatePowerPoint { get; set; }
        /// <summary>
        /// Create Images Thumbnails
        /// </summary>
        public bool CreateImage { get; set; }
        /// <summary>
        /// Create Audio Thumbnails
        /// </summary>
        public bool CreateAudio { get; set; }
        /// <summary>
        /// Create Video Thumbnails
        /// </summary>
        public bool CreateVideo { get; set; }

        public Repository()
        {
            UserRoleId = -2;
            FileNotificationsRoleId = -2;
            ShowTips = true;
            ShowInstructions = true;
            PageSize = 20;
            UseThumbnails = true;
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
            ForceHttps = Null.SetNullBoolean(dr["ForceHttps"]);
            UseThumbnails = Null.SetNullBoolean(dr["UseThumbnails"]);
            CreatePDF = Null.SetNullBoolean(dr["CreatePDF"]);
            CreateWord = Null.SetNullBoolean(dr["CreateWord"]);
            CreateExcel = Null.SetNullBoolean(dr["CreateExcel"]);
            CreatePowerPoint = Null.SetNullBoolean(dr["CreatePowerPoint"]);
            CreateImage = Null.SetNullBoolean(dr["CreateImage"]);
            CreateAudio = Null.SetNullBoolean(dr["CreateAudio"]);
            CreateVideo = Null.SetNullBoolean(dr["CreateVideo"]);
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