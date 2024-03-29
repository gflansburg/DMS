﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content;
using Gafware.Modules.DMS.Data;
using System.Xml.Serialization;
using DotNetNuke.Entities.Users;

namespace Gafware.Modules.DMS.Components
{
    [Serializable]
    [XmlType("Packet")]
    [XmlRoot("ContentItem")]
    public class Packet : ContentItem
    {
        /// <summary>
        /// Id of packet
        /// </summary>
        public int PacketId { get; set; }
        /// <summary>
        /// Packet name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Show documents descriptions
        /// </summary>
        public bool ShowDescription { get; set; }
        /// <summary>
        /// Show description
        /// </summary>
        public bool ShowPacketDescription { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Admin comments
        /// </summary>
        public string AdminComments { get; set; }
        /// <summary>
        /// Custom header
        /// </summary>
        public string CustomHeader { get; set; }
        /// <summary>
        /// Portal Id
        /// </summary>
        public int PortalId { get; set; }
        /// <summary>
        /// Tab Module Id
        /// </summary>
        public int TabModuleId { get; set; }
        /// <summary>
        /// User id of creator
        /// </summary>
        public new int CreatedByUserID { get; set; }
        /// <summary>
        /// Is owner a group
        /// </summary>
        public bool IsGroupOwner { get; set; }
        /// <summary>
        /// Tags
        /// </summary>
        public List<PacketTag> Tags { get; set; }
        private List<PacketDocument> _documents = null;
        /// <summary>
        /// Documents
        /// </summary>
        public List<PacketDocument> Documents 
        { 
            get
            {
                if(_documents == null)
                {
                    Documents = PacketController.GetAllDocumentsForPacket(PacketId, 0);
                }
                return _documents;
            }
            set
            {
                _documents = value;
            }
        }
        /// <summary>
        /// Creator
        /// </summary>
        public new UserInfo CreatedByUser
        {
            get
            {
                if (CreatedByUserID > 0 && !IsGroupOwner)
                {
                    return DotNetNuke.Entities.Users.UserController.GetUserById(PortalId, CreatedByUserID);
                }
                return null;
            }
        }
        /// <summary>
        /// Group owner
        /// </summary>
        public DotNetNuke.Security.Roles.RoleInfo Group
        {
            get
            {
                if (CreatedByUserID > 0 && IsGroupOwner)
                {
                    return DotNetNuke.Security.Roles.RoleController.Instance.GetRoleById(PortalId, CreatedByUserID);
                }
                return null;
            }
        }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            PacketId = Null.SetNullInteger(dr["PacketID"]);
            CreatedByUserID = Null.SetNullInteger(dr["UserID"]);
            IsGroupOwner = Null.SetNullBoolean(dr["IsGroupOwner"]);
            Name = Null.SetNullString(dr["Name"]);
            ShowDescription = Null.SetNullBoolean(dr["ShowDescription"]);
            ShowPacketDescription = Null.SetNullBoolean(dr["ShowPacketDescription"]);
            Description = Null.SetNullString(dr["Description"]);
            AdminComments = Null.SetNullString(dr["AdminComments"]);
            CustomHeader = Null.SetNullString(dr["CustomHeader"]);
            PortalId = Null.SetNullInteger(dr["PortalID"]);
            TabModuleId = Null.SetNullInteger(dr["TabModuleID"]);
            Tags = PacketController.GetAllTagsForPacket(PacketId);
        }

        public override int KeyID
        {
            get
            {
                return PacketId;
            }
            set
            {
                PacketId = value;
            }
        }
    }
}