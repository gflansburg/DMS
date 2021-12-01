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

using System.Data;
using System;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using Gafware.Modules.DMS.Components;

namespace Gafware.Modules.DMS.Data
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// An abstract class for the data access layer
    /// 
    /// The abstract data provider provides the methods that a control data provider (sqldataprovider)
    /// must implement. You'll find two commented out examples in the Abstract methods region below.
    /// </summary>
    /// -----------------------------------------------------------------------------
    public abstract class DataProvider
    {

        #region Shared/Static Methods

        private static DataProvider provider;

        // return the provider
        public static DataProvider Instance()
        {
            if (provider == null)
            {
                const string assembly = "Gafware.Modules.DMS.Data.SqlDataprovider,Gafware.DMS";
                Type objectType = Type.GetType(assembly, true, true);

                provider = (DataProvider)Activator.CreateInstance(objectType);
                DataCache.SetCache(objectType.FullName, provider);
            }

            return provider;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not returning class state information")]
        public static IDbConnection GetConnection()
        {
            const string providerType = "data";
            ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(providerType);

            Provider objProvider = ((Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider]);
            string _connectionString;
            if (!String.IsNullOrEmpty(objProvider.Attributes["connectionStringName"]) && !String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[objProvider.Attributes["connectionStringName"]]))
            {
                _connectionString = System.Configuration.ConfigurationManager.AppSettings[objProvider.Attributes["connectionStringName"]];
            }
            else
            {
                _connectionString = objProvider.Attributes["connectionString"];
            }

            IDbConnection newConnection = new System.Data.SqlClient.SqlConnection();
            newConnection.ConnectionString = _connectionString.ToString();
            newConnection.Open();
            return newConnection;
        }

        #endregion

        #region Abstract methods

        public abstract IDataReader GetAllCategories(int portalId, int tabModuleId);

        public abstract IDataReader GetCategory(int categoryId);

        public abstract void DeleteCategory(int categoryId);

        public abstract int SaveCategory(Category objCategory);

        public abstract IDataReader GetAllStatuses();

        public abstract IDataReader GetStatus(int statusId);

        public abstract void DeleteStatus(int statusId);

        public abstract int SaveStatus(Status objStatus);

        public abstract IDataReader GetAllTags(int portalId, int tabModuleId);

        public abstract IDataReader GetTag(int tagId);

        public abstract IDataReader GetTagByTagName(string tagName, int portalId, int tabModuleId);

        public abstract void DeleteTag(int tagId);

        public abstract int SaveTag(Tag objTag);

        public abstract IDataReader GetAllDocuments(int portalId, int tabModuleId);

        public abstract IDataReader GetAllDocumentsForDropDown(int portalId, int tabModuleId);

        public abstract IDataReader GetAllPublicDocuments(int portalId, int tabModuleId);

        public abstract IDataReader GetAllDocumentsForTag(int tagId, bool portalWideRepository);
        
        public abstract IDataReader GetDocumentsForTag(int tagId, int portalId, int tabModuleId, int userId);

        public abstract IDataReader GetDocument(int documentId);

        public abstract IDataReader GetDocumentByName(string name, int portalId, int tabModuleId);

        public abstract void DeleteDocument(int documentId);

        public abstract int SaveDocument(Document objDocument);

        public abstract IDataReader GetAllTagsForDocument(int documentId);

        public abstract IDataReader GetDocumentTag(int documentTagId);

        public abstract void DeleteDocumentTag(int documentTagId);

        public abstract int SaveDocumentTag(DocumentTag objDocumentTag);

        public abstract IDataReader GetAllCategoriesForDocument(int documentId);

        public abstract IDataReader GetDocumentCategory(int documentCategoryId);

        public abstract void DeleteDocumentCategory(int documentCategoryId);

        public abstract int SaveDocumentCategory(DocumentCategory objDocumentCategory);

        public abstract IDataReader GetAllPackets(int portalId, int tabModuleId);

        public abstract IDataReader GetAllPacketsForUser(int userId, int portalId, int tabModuleId);

        public abstract IDataReader GetAllPacketsContainingDocument(int documentId);

        public abstract IDataReader GetPacket(int packetId);

        public abstract IDataReader GetPacketByName(string name, int portalId, int tabModuleId);

        public abstract void DeletePacket(int packetId);

        public abstract int SavePacket(Packet objPacket);

        public abstract IDataReader GetAllDocumentsForPacket(int packetId);

        public abstract IDataReader GetPacketDoc(int packetDocId);

        public abstract void DeletePacketDoc(int packetDocId);

        public abstract int SavePacketDoc(PacketDocument objPacketDoc);

        public abstract IDataReader GetAllTagsForPacket(int packetId);

        public abstract IDataReader GetPacketTag(int packetTagId);

        public abstract void DeletePacketTag(int packetTagId);

        public abstract int SavePacketTag(PacketTag objPacketTag);

        public abstract IDataReader GetAllFilesForDocument(int documentId);

        public abstract IDataReader GetFile(int fileId);
        
        public abstract IDataReader GetFileByName(int portalId, int tabModuleId, string fileName);

        public abstract void DeleteFile(int fileId);

        public abstract int SaveFile(DMSFile objFile);

        public abstract IDataReader Search(int categoryId, string keywords, bool bPrivate, int portalId, int tabModuleId, int userId);

        public abstract IDataReader FindSearchTags(string term, int portalId, int tabModuleId);

        public abstract IDataReader GetDocumentList(int categoryId, string keywords, int userId, int portalId, int tabModuleId);

        public abstract void SaveFileContents(int fileVersionId, byte[] contents);

        public abstract void SaveFileContents(int fileVersionId, System.IO.Stream stream);

        public abstract byte[] GetFileContents(int fileVersionId);

        public abstract void SaveThumbnail(int fileVersionId, bool isLandscape, byte[] thumbnail);

        public abstract byte[] GetThumbnail(int fileVersionId);

        public abstract void ChanngeDocumentOwnership(int currentOwnerId, int newOwnerId, int portalId);

        public abstract void ChanngePacketOwnership(int currentOwnerId, int newOwnerId, int portalId);

        public abstract IDataReader GetUsers(int roleId, int portalId);

        public abstract IDataReader GetFileNotificationRecipients(int roleId, int portalId);

        public abstract string GetConnectionString();

        public abstract IDataReader GetAllFileTypes(int portalId, int tabModuleId);

        public abstract IDataReader GetFileType(int fileTypeId);

        public abstract IDataReader GetFileTypeByExt(string fileTypeExt, int portalId, int tabModuleId);

        public abstract void DeleteFileType(int fileTypeId);

        public abstract int SaveFileType(FileType objFileType);

        public abstract void MovePacket(int documentId, int tagId, int newSortOrder);

        public abstract IDataReader GetFileVersions(int fileId);

        public abstract IDataReader GetFileVersion(int fileVersionId);

        public abstract void DeleteFileVersion(int fileVersionId);

        public abstract int SaveFileVersion(FileVersion objFileVersion);

        public abstract IDataReader GetRepository(int portalId, int tabModuleId);

        public abstract IDataReader GetAllRepositories(int portalId);

        public abstract int SaveRepository(Repository objRepository);

        public abstract IDataReader GetPortalSettings(int portalId);

        public abstract int SavePortalSettings(DMSPortalSettings objPortal);

        public abstract void AddDefaultFileExtensions(int portalId, int tabModuleId);

        #endregion

    }

}