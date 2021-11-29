/*
' Copyright (c) 2021 Gafware
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Data;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework.Providers;
using Gafware.Modules.DMS.Components;
using Microsoft.ApplicationBlocks.Data;

namespace Gafware.Modules.DMS.Data
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// SQL Server implementation of the abstract DataProvider class
    /// 
    /// This concreted data provider class provides the implementation of the abstract methods 
    /// from data dataprovider.cs
    /// 
    /// In most cases you will only modify the Public methods region below.
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class SqlDataProvider : DataProvider
    {

        #region Private Members

        private const string ProviderType = "data";
        private const string ModuleQualifier = "Gafware_DMS_";

        private readonly ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);
        private readonly string _connectionString;
        private readonly string _providerPath;
        private readonly string _objectQualifier;
        private readonly string _databaseOwner;

        #endregion

        #region Constructors

        public SqlDataProvider()
        {

            // Read the configuration specific information for this provider
            Provider objProvider = (Provider)(_providerConfiguration.Providers[_providerConfiguration.DefaultProvider]);

            // Read the attributes for this provider

            //Get Connection string from web.config
            _connectionString = Config.GetConnectionString();

            if (string.IsNullOrEmpty(_connectionString))
            {
                // Use connection string specified in provider
                _connectionString = objProvider.Attributes["connectionString"];
            }

            _providerPath = objProvider.Attributes["providerPath"];

            _objectQualifier = objProvider.Attributes["objectQualifier"];
            if (!string.IsNullOrEmpty(_objectQualifier) && _objectQualifier.EndsWith("_", StringComparison.Ordinal) == false)
            {
                _objectQualifier += "_";
            }

            _databaseOwner = objProvider.Attributes["databaseOwner"];
            if (!string.IsNullOrEmpty(_databaseOwner) && _databaseOwner.EndsWith(".", StringComparison.Ordinal) == false)
            {
                _databaseOwner += ".";
            }

        }

        #endregion

        #region Properties

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        public string ProviderPath
        {
            get
            {
                return _providerPath;
            }
        }

        public string ObjectQualifier
        {
            get
            {
                return _objectQualifier;
            }
        }

        public string DatabaseOwner
        {
            get
            {
                return _databaseOwner;
            }
        }

        // used to prefect your database objects (stored procedures, tables, views, etc)
        private string NamePrefix
        {
            get { return DatabaseOwner + ObjectQualifier + ModuleQualifier; }
        }

        #endregion

        #region Private Methods

        private static object GetNull(object field)
        {
            return Null.GetNull(field, DBNull.Value);
        }

        #endregion

        #region Public Methods

        public override IDataReader GetAllCategories(int portalId, int tabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetAllCategories", portalId, tabModuleId);
        }

        public override IDataReader GetCategory(int categoryId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetCategory", categoryId);
        }

        public override void DeleteCategory(int categoryId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "DeleteCategory", categoryId);
        }

        public override int SaveCategory(Category objCategory)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, NamePrefix + "SaveCategory", objCategory.CategoryId, objCategory.CategoryName, objCategory.RoleId, objCategory.PortalId, objCategory.TabModuleId));
        }

        public override IDataReader GetAllStatuses()
        {
            return SqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, NamePrefix + "GetAllStatuses");
        }

        public override IDataReader GetStatus(int statusId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetStatus", statusId);
        }

        public override void DeleteStatus(int statusId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "DeleteStatus", statusId);
        }

        public override int SaveStatus(Status objStatus)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, NamePrefix + "SaveStatus", objStatus.StatusId, objStatus.StatusName));
        }

        public override IDataReader GetAllTags(int portalId, int tabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetAllTags", portalId, tabModuleId);
        }

        public override IDataReader GetTag(int tagId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetTag", tagId);
        }

        public override IDataReader GetTagByTagName(string tagName, int portalId, int tabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetTagByTagName", tagName, portalId, tabModuleId);
        }

        public override void DeleteTag(int tagId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "DeleteTag", tagId);
        }

        public override int SaveTag(Tag objTag)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, NamePrefix + "SaveTag", objTag.TagId, objTag.TagName, objTag.IsPrivate, objTag.Weight, objTag.PortalId, objTag.TabModuleId));
        }

        public override IDataReader GetAllDocuments(int portalId, int tabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetAllDocuments", portalId, tabModuleId);
        }

        public override IDataReader GetAllDocumentsForDropDown(int portalId, int tabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetAllDocuments", portalId, tabModuleId);
        }

        public override IDataReader GetDocument(int documentId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetDocument", documentId);
        }

        public override IDataReader GetDocumentByName(string name, int portalId, int tabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetDocumentByName", name, portalId, tabModuleId);
        }

        public override void DeleteDocument(int documentId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "DeleteDocument", documentId);
        }

        public override int SaveDocument(Document objDocument)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, NamePrefix + "SaveDocument", objDocument.DocumentId, objDocument.CreatedByUserID, objDocument.DocumentName, objDocument.ShortDescription, objDocument.DocumentDetails, objDocument.AdminComments, objDocument.ManagerToolkit, objDocument.ActivationDate, objDocument.ExpirationDate, objDocument.IPAddress, objDocument.IsSearchable, objDocument.UseCategorySecurityRoles, objDocument.SecurityRoleId, objDocument.PortalId, objDocument.TabModuleId));
        }

        public override IDataReader GetAllTagsForDocument(int documentId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetAllTagsForDocument", documentId);
        }

        public override IDataReader GetDocumentTag(int documentTagId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetDocumentTag", documentTagId);
        }

        public override void DeleteDocumentTag(int documentTagId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "DeleteDocumentTag", documentTagId);
        }

        public override int SaveDocumentTag(DocumentTag objDocumentTag)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, NamePrefix + "SaveDocumentTag", objDocumentTag.DocumentTagId, objDocumentTag.DocumentId, objDocumentTag.TagId));
        }

        public override IDataReader GetAllCategoriesForDocument(int documentId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetAllCategoriesForDocument", documentId);
        }

        public override IDataReader GetDocumentCategory(int documentCategoryId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetDocumentCategory", documentCategoryId);
        }

        public override void DeleteDocumentCategory(int documentCategoryId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "DeleteDocumentCategory", documentCategoryId);
        }

        public override int SaveDocumentCategory(DocumentCategory objDocumentCategory)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, NamePrefix + "SaveDocumentCategory", objDocumentCategory.DocumentCategoryId, objDocumentCategory.DocumentId, objDocumentCategory.CategoryId));
        }

        public override IDataReader GetAllPackets(int portalId, int tabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetAllPackets", portalId, tabModuleId);
        }

        public override IDataReader GetAllPacketsContainingDocument(int documentId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetAllPacketsContainingDocument", documentId);
        }

        public override IDataReader GetPacket(int packetId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetPacket", packetId);
        }

        public override void DeletePacket(int packetId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "DeletePacket", packetId);
        }

        public override int SavePacket(Packet objPacket)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, NamePrefix + "SavePacket", objPacket.PacketId, objPacket.CreatedByUserID, objPacket.Name, objPacket.ShowDescription, objPacket.ShowPacketDescription, objPacket.Description, objPacket.AdminComments, objPacket.CustomHeader, objPacket.PortalId, objPacket.TabModuleId));
        }

        public override IDataReader GetAllDocumentsForPacket(int packetId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetAllDocumentsForPacket", packetId);
        }

        public override IDataReader GetPacketDoc(int packetDocId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetPacketDocument", packetDocId);
        }

        public override void DeletePacketDoc(int packetDocId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "DeletePacketDocument", packetDocId);
        }

        public override int SavePacketDoc(PacketDocument objPacketDoc)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, NamePrefix + "SavePacketDocument", objPacketDoc.PacketDocId, objPacketDoc.PacketId, objPacketDoc.DocumentId, objPacketDoc.FileId, objPacketDoc.SortOrder));
        }

        public override IDataReader GetAllTagsForPacket(int packetId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetAllTagsForPacket", packetId);
        }

        public override IDataReader GetPacketTag(int packetTagId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetPacketTag", packetTagId);
        }

        public override void DeletePacketTag(int packetTagId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "DeletePacketTag", packetTagId);
        }

        public override int SavePacketTag(PacketTag objPacketTag)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, NamePrefix + "SavePacketTag", objPacketTag.PacketTagId, objPacketTag.PacketId, objPacketTag.TagId, objPacketTag.SortOrder));
        }

        public override IDataReader GetAllFilesForDocument(int documentId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetAllFilesForDocument", documentId);
        }

        public override IDataReader GetFile(int fileId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetFile", fileId);
        }

        public override IDataReader GetFileByName(int portalId, int tabModuleId, string fileName)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetFileByName", portalId, tabModuleId, fileName);
        }

        public override void DeleteFile(int fileId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "DeleteFile", fileId);
        }

        public override int SaveFile(DMSFile objFile)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, NamePrefix + "SaveFile", objFile.FileId, objFile.StatusId, objFile.DocumentId, objFile.UploadDirectory, objFile.FileType, objFile.Filename, objFile.MimeType));
        }

        public override IDataReader GetPacketByName(string name, int portalId, int tabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetPacketByName", name, portalId, tabModuleId);
        }

        public override IDataReader Search(int categoryId, string keywords, bool bPrivate, int portalId, int tabModuleId, int userId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "SearchDocuments", categoryId, keywords, bPrivate, portalId, tabModuleId, userId);
        }

        public override IDataReader FindSearchTags(string term, int portalId, int tabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "FindSearchTags", term, portalId, tabModuleId);
        }

        public override IDataReader GetDocumentList(int categoryId, string keywords, int userId, int portalId, int tabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetDocumentList", categoryId, keywords, userId, portalId, tabModuleId);
        }

        public override void SaveFileContents(int fileVersionId, byte[] contents)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "SaveFileContents", fileVersionId, contents == null ? DBNull.Value : (object)contents, false);
        }

        const int SQL_BINARY_BUFFER_SIZE = 1024 * 1024; // optimal chunk size

        public override void SaveFileContents(int fileVersionId, System.IO.Stream stream)
        {
            byte[] buffer = new byte[SQL_BINARY_BUFFER_SIZE];
            int bytesRead;
            long fileSize = stream.Length;
            long totalBytes = 0;
            bool append = false;
            while ((bytesRead = stream.Read(buffer, 0, SQL_BINARY_BUFFER_SIZE)) > 0)
            {
                totalBytes += bytesRead;
                if (bytesRead == SQL_BINARY_BUFFER_SIZE) // pass the filled buffer
                {
                    SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "SaveFileContents", fileVersionId, buffer, append);
                }
                else // didn't fill an entire buffer, reached end of stream
                { 
                    byte[] smallBuffer = new byte[bytesRead];
                    Buffer.BlockCopy(buffer, 0, smallBuffer, 0, bytesRead);
                    SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "SaveFileContents", fileVersionId, smallBuffer, append);
                    break;
                }
                append = true; // subsequent calls should append data
            }
        }

        public override byte[] GetFileContents(int fileVersionId)
        {
            System.Data.DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, NamePrefix + "GetFileContents", fileVersionId);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                System.Data.DataRow row = ds.Tables[0].Rows[0];
                if (row["Contents"] != DBNull.Value)
                {
                    return (byte[])row["Contents"];
                }
            }
            return null;
        }

        public override void SaveThumbnail(int fileVersionId, bool isLandscape, byte[] thumbnail)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "SaveThumbnail", fileVersionId, isLandscape, thumbnail == null ? DBNull.Value : (object)thumbnail);
        }

        public override byte[] GetThumbnail(int fileVersionId)
        {
            System.Data.DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, NamePrefix + "GetThumbnail", fileVersionId);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                System.Data.DataRow row = ds.Tables[0].Rows[0];
                if (row["Thumbnail"] != DBNull.Value)
                {
                    return (byte[])row["Thumbnail"];
                }
            }
            return null;
        }

        public override void ChanngeDocumentOwnership(int currentOwnerId, int newOwnerId, int portalId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "ChangeDocumentOwnership", currentOwnerId, newOwnerId, portalId);
        }

        public override void ChanngePacketOwnership(int currentOwnerId, int newOwnerId, int portalId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "ChangePacketOwnership", currentOwnerId, newOwnerId, portalId);
        }

        public override string GetConnectionString()
        {
            return ConnectionString;
        }

        public override IDataReader GetUsers(int roleId, int portalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetUsers", roleId, portalId);
        }

        public override IDataReader GetFileNotificationRecipients(int roleId, int portalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetFileNotificationRecipients", roleId, portalId);
        }
        
        public override IDataReader GetAllFileTypes(int portalId, int tabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetAllFileTypes", portalId, tabModuleId);
        }

        public override IDataReader GetFileType(int fileTypeId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetFileType", fileTypeId);
        }

        public override void DeleteFileType(int fileTypeId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "DeleteFileType", fileTypeId);
        }

        public override int SaveFileType(FileType objFileType)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, NamePrefix + "SaveFileType", objFileType.FileTypeId, objFileType.FileTypeName, objFileType.FileTypeShortName, objFileType.FileTypeExt, objFileType.PortalId, objFileType.TabModuleId));
        }

        public override IDataReader GetAllPublicDocuments(int portalId, int tabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetAllPublicDocuments", portalId, tabModuleId);
        }

        public override IDataReader GetAllDocumentsForTag(int tagId, bool portalWideRepository)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetAllDocumentsForTag", tagId, portalWideRepository);
        }

        public override IDataReader GetAllPacketsForUser(int userId, int portalId, int tabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetAllPacketsForUser", userId, portalId, tabModuleId);
        }

        public override void MovePacket(int documentId, int tagId, int newSortOrder)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "MovePacket", documentId, tagId, newSortOrder);
        }

        public override IDataReader GetFileVersions(int fileId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetFileVersions", fileId);
        }

        public override IDataReader GetFileVersion(int fileVersionId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetFileVersion", fileVersionId);
        }

        public override void DeleteFileVersion(int fileVersionId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "DeleteFileVersion", fileVersionId);
        }

        public override int SaveFileVersion(FileVersion objFileVersion)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, NamePrefix + "SaveFileVersion", objFileVersion.FileVersionId, objFileVersion.FileId, objFileVersion.Version, objFileVersion.CreatedOnDate, objFileVersion.CreatedByUserID, objFileVersion.WebPageUrl, objFileVersion.IPAddress, objFileVersion.Filesize));
        }

        public override IDataReader GetFileTypeByExt(string fileTypeExt, int portalId, int tabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetFileTypeByExt", fileTypeExt, portalId, tabModuleId);
        }

        public override IDataReader GetRepository(int portalId, int tabModuleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetRepository", portalId, tabModuleId);
        }

        public override int SaveRepository(Components.Repository objRepository)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, NamePrefix + "SaveRepository", objRepository.RepositoryId, objRepository.PortalId, objRepository.TabModuleId, objRepository.UserRoleId, objRepository.FileNotificationsRoleId, objRepository.NewFileSubject, objRepository.NewFileMsg, objRepository.CategoryName, objRepository.SaveLocalFile, objRepository.ShowTips, objRepository.ShowInstructions, objRepository.Instructions, objRepository.Theme, objRepository.ThumbnailType, objRepository.ThumbnailSize));
        }

        public override IDataReader GetPortalSettings(int portalId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, NamePrefix + "GetPortalSettings", portalId);
        }

        public override int SavePortalSettings(Components.DMSPortalSettings objPortalSettings)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, NamePrefix + "SavePortalSettings", objPortalSettings.PortalId, objPortalSettings.PortalWideRepository));
        }

        public override void AddDefaultFileExtensions(int portalId, int tabModuleId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, NamePrefix + "AddDefaultFileExtensions", portalId, tabModuleId);
        }
        #endregion

    }

}