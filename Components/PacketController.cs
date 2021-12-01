using System;
using System.Collections.Generic;
using DotNetNuke.Common.Utilities;
using System.Linq;
using Gafware.Modules.DMS.Data;

namespace Gafware.Modules.DMS.Components
{
    public class PacketController
    {
        #region "Public Methods"
        // Get a single Glossary item by primary key

        public static Packet GetPacket(int packetId)
        {
            return CBO.FillObject<Packet>(DataProvider.Instance().GetPacket(packetId));
        }

        public static Packet GetPacketByName(string name, int portalId, int tabModuleId)
        {
            return CBO.FillObject<Packet>(DataProvider.Instance().GetPacketByName(name, portalId, tabModuleId));
        }

        public static List<Packet> GetAllPackets(int portalId, int tabModuleId)
        {
            return CBO.FillCollection<Packet>(DataProvider.Instance().GetAllPackets(portalId, tabModuleId));
        }

        public static List<Packet> GetAllPacketsForUser(int userId, int portalId, int tabModuleId)
        {
            return CBO.FillCollection<Packet>(DataProvider.Instance().GetAllPacketsForUser(userId, portalId, tabModuleId));
        }

        public static Packet FindPacket(string packetName, int portalId)
        {
            return CBO.FillObject<Packet>(DataProvider.Instance().FindPacket(packetName, portalId));
        }

        public static List<Packet> GetAllPacketsContainingDocument(int documentId)
        {
            return CBO.FillCollection<Packet>(DataProvider.Instance().GetAllPacketsContainingDocument(documentId));
        }

        public static void DeletePacket(int packetId)
        {
            DataProvider.Instance().DeletePacket(packetId);
        }

        public static int SavePacket(Packet objPacket)
        {
            objPacket.PacketId = DataProvider.Instance().SavePacket(objPacket);
            foreach(PacketDocument doc in objPacket.Documents)
            {
                doc.PacketId = objPacket.PacketId;
                doc.PacketDocId = SavePacketDoc(doc);
            }
            foreach(PacketTag tag in objPacket.Tags)
            {
                tag.PacketId = objPacket.PacketId;
                tag.PacketTagId = SavePacketTag(tag);
            }
            return objPacket.PacketId;
        }

        public static PacketDocument GetPacketDoc(int packetDocId)
        {
            return CBO.FillObject<PacketDocument>(DataProvider.Instance().GetPacketDoc(packetDocId));
        }

        public static List<PacketDocument> GetAllDocumentsForPacket(int packetId, int userId)
        {
            return CBO.FillCollection<PacketDocument>(DataProvider.Instance().GetAllDocumentsForPacket(packetId, userId));
        }

        public static void DeletePacketDoc(int packetDocId)
        {
            DataProvider.Instance().DeletePacketDoc(packetDocId);
        }

        public static int SavePacketDoc(PacketDocument objPacketDoc)
        {
            objPacketDoc.PacketDocId = DataProvider.Instance().SavePacketDoc(objPacketDoc);
            return objPacketDoc.PacketDocId;
        }

        public static PacketTag GetPacketTag(int packetTagId)
        {
            return CBO.FillObject<PacketTag>(DataProvider.Instance().GetPacketTag(packetTagId));
        }

        public static List<PacketTag> GetAllTagsForPacket(int packetId)
        {
            return CBO.FillCollection<PacketTag>(DataProvider.Instance().GetAllTagsForPacket(packetId));
        }

        public static void DeletePacketTag(int packetTagId)
        {
            DataProvider.Instance().DeletePacketTag(packetTagId);
        }

        public static int SavePacketTag(PacketTag objPacketTag)
        {
            objPacketTag.PacketTagId = DataProvider.Instance().SavePacketTag(objPacketTag);
            return objPacketTag.PacketTagId;
        }

        public static void ChangeOwnership(int currentOwnerId, int newOwnerId, int portalId)
        {
            DataProvider.Instance().ChanngePacketOwnership(currentOwnerId, newOwnerId, portalId);
        }

        public static void MovePacket(int documentId, int tagId, int newSortOrder)
        {
            DataProvider.Instance().MovePacket(documentId, tagId, newSortOrder);
        }

        #endregion
    }
}