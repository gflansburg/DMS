using System;
using System.Collections.Generic;
using DotNetNuke.Common.Utilities;
using System.Linq;
using Gafware.Modules.DMS.Data;

namespace Gafware.Modules.DMS.Components
{
    public class DocumentController
    {
        #region "Public Methods"
        // Get a single Glossary item by primary key

        public static Category GetCategory(int categoryId)
        {
            return CBO.FillObject<Category>(DataProvider.Instance().GetCategory(categoryId));
        }

        public static List<Category> GetAllCategories(int portalId, int tabModuleId)
        {
            return CBO.FillCollection<Category>(DataProvider.Instance().GetAllCategories(portalId, tabModuleId));
        }

        public static void DeleteCategory(int categoryId)
        {
            DataProvider.Instance().DeleteCategory(categoryId);
        }

        public static int SaveCategory(Category objCategory)
        {
            objCategory.CategoryId = DataProvider.Instance().SaveCategory(objCategory);
            return objCategory.CategoryId;
        }

        public static Status GetStatus(int statusId)
        {
            return CBO.FillObject<Status>(DataProvider.Instance().GetStatus(statusId));
        }

        public static List<Status> GetAllStatuses()
        {
            return CBO.FillCollection<Status>(DataProvider.Instance().GetAllStatuses());
        }

        public static void DeleteStatus(int statusId)
        {
            DataProvider.Instance().DeleteStatus(statusId);
        }

        public static int SaveStatus(Status objStatus)
        {
            objStatus.StatusId = DataProvider.Instance().SaveStatus(objStatus);
            return objStatus.StatusId;
        }

        public static Tag GetTag(int tagId)
        {
            return CBO.FillObject<Tag>(DataProvider.Instance().GetTag(tagId));
        }

        public static Tag GetTagByTagName(string tagName, int portalId, int tabModuleId)
        {
            return CBO.FillObject<Tag>(DataProvider.Instance().GetTagByTagName(tagName, portalId, tabModuleId));
        }

        public static List<Tag> GetAllTags(int portalId, int tabModuleId)
        {
            return CBO.FillCollection<Tag>(DataProvider.Instance().GetAllTags(portalId, tabModuleId));
        }

        public static void DeleteTag(int tagId)
        {
            DataProvider.Instance().DeleteTag(tagId);
        }

        public static int SaveTag(Tag objTag)
        {
            objTag.TagId = DataProvider.Instance().SaveTag(objTag);
            return objTag.TagId;
        }

        public static Document GetDocument(int documentId)
        {
            return CBO.FillObject<Document>(DataProvider.Instance().GetDocument(documentId));
        }

        public static DocumentView GetDocumentForView(int documentId)
        {
            return CBO.FillObject<DocumentView>(DataProvider.Instance().GetDocument(documentId));
        }

        public static Document GetDocumentByName(string name, int portalId, int tabModuleId)
        {
            return CBO.FillObject<Document>(DataProvider.Instance().GetDocumentByName(name, portalId, tabModuleId));
        }

        public static List<Document> GetAllDocuments(int portalId, int tabModuleId)
        {
            return CBO.FillCollection<Document>(DataProvider.Instance().GetAllDocuments(portalId, tabModuleId));
        }

        public static List<DocumentView> GetAllDocumentsForDropDown(int portalId, int tabModuleId)
        {
            return CBO.FillCollection<DocumentView>(DataProvider.Instance().GetAllDocuments(portalId, tabModuleId));
        }

        public static List<DocumentView> GetAllDocumentsForView(int portalId, int tabModuleId)
        {
            return CBO.FillCollection<DocumentView>(DataProvider.Instance().GetAllDocuments(portalId, tabModuleId));
        }

        public static List<DocumentView> GetAllDocumentsForTag(int tagId, bool portalWideRepository)
        {
            return CBO.FillCollection<DocumentView>(DataProvider.Instance().GetAllDocumentsForTag(tagId, portalWideRepository));
        }

        public static List<Document> GetDocumentsForTag(int tagId, int portalId, int tabModuleId, int userId)
        {
            return CBO.FillCollection<Document>(DataProvider.Instance().GetDocumentsForTag(tagId, portalId, tabModuleId, userId));
        }

        public static List<Document> GetAllPublicDocuments(int portalId, int tabModuleId)
        {
            return CBO.FillCollection<Document>(DataProvider.Instance().GetAllPublicDocuments(portalId, tabModuleId));
        }

        public static List<DocumentView> GetDocumentList(int categoryId, string keywords, int userId, int portalId, int tabModuleId)
        {
            return CBO.FillCollection<DocumentView>(DataProvider.Instance().GetDocumentList(categoryId, keywords, userId, portalId, tabModuleId));
        }

        public static void DeleteDocument(int documentId)
        {
            DataProvider.Instance().DeleteDocument(documentId);
        }

        public static int SaveDocument(Document objDocument)
        {
            objDocument.DocumentId = DataProvider.Instance().SaveDocument(objDocument);
            foreach (DocumentCategory category in objDocument.Categories)
            {
                category.DocumentId = objDocument.DocumentId;
                category.DocumentCategoryId = SaveDocumentCategory(category);
            }
            foreach (DocumentTag tag in objDocument.Tags)
            {
                tag.DocumentId = objDocument.DocumentId;
                tag.DocumentTagId = SaveDocumentTag(tag);
            }
            foreach (DMSFile file in objDocument.Files)
            {
                file.DocumentId = objDocument.DocumentId;
                file.FileId = SaveFile(file);
            }
            return objDocument.DocumentId;
        }

        public static DocumentTag GetDocumentTag(int documentTagId)
        {
            return CBO.FillObject<DocumentTag>(DataProvider.Instance().GetDocumentTag(documentTagId));
        }

        public static List<DocumentTag> GetAllTagsForDocument(int documentId)
        {
            return CBO.FillCollection<DocumentTag>(DataProvider.Instance().GetAllTagsForDocument(documentId));
        }

        public static void DeleteDocumentTag(int documentTagId)
        {
            DataProvider.Instance().DeleteDocumentTag(documentTagId);
        }

        public static int SaveDocumentTag(DocumentTag objDocumentTag)
        {
            objDocumentTag.DocumentTagId = DataProvider.Instance().SaveDocumentTag(objDocumentTag);
            return objDocumentTag.DocumentTagId;
        }

        public static DocumentCategory GetDocumentCategory(int documentCategoryId)
        {
            return CBO.FillObject<DocumentCategory>(DataProvider.Instance().GetDocumentCategory(documentCategoryId));
        }

        public static List<DocumentCategory> GetAllCategoriesForDocument(int documentId)
        {
            return CBO.FillCollection<DocumentCategory>(DataProvider.Instance().GetAllCategoriesForDocument(documentId));
        }

        public static void DeleteDocumentCategory(int documentCategoryId)
        {
            DataProvider.Instance().DeleteDocumentCategory(documentCategoryId);
        }

        public static int SaveDocumentCategory(DocumentCategory objDocumentCategory)
        {
            objDocumentCategory.DocumentCategoryId = DataProvider.Instance().SaveDocumentCategory(objDocumentCategory);
            return objDocumentCategory.DocumentCategoryId;
        }

        public static DMSFile GetFile(int fileId)
        {
            return CBO.FillObject<DMSFile>(DataProvider.Instance().GetFile(fileId));
        }

        public static DMSFile GetFileByName(int portalId, int tabModuleId, string fileName)
        {
            return CBO.FillObject<DMSFile>(DataProvider.Instance().GetFileByName(portalId, tabModuleId, fileName));
        }

        public static List<DMSFile> GetAllFilesForDocument(int documentId)
        {
            return CBO.FillCollection<DMSFile>(DataProvider.Instance().GetAllFilesForDocument(documentId));
        }

        public static void DeleteFile(int fileId)
        {
            DataProvider.Instance().DeleteFile(fileId);
        }

        public static int SaveFile(DMSFile objFile)
        {
            objFile.FileId = DataProvider.Instance().SaveFile(objFile);
            return objFile.FileId;
        }

        public static List<Document> Search(int categoryId, string keywords, bool bPrivate, int portalId, int tabModuleId, int userId)
        {
            return CBO.FillCollection<Document>(DataProvider.Instance().Search(categoryId, keywords, bPrivate, portalId, tabModuleId, userId));
        }

        public static List<Tag> FindSearchTags(string term, int portalId, int tabModuleId)
        {
            return CBO.FillCollection<Tag>(DataProvider.Instance().FindSearchTags(term, portalId, tabModuleId));
        }

        public static void SaveFileContents(int fileVersionId, byte[] contents)
        {
            DataProvider.Instance().SaveFileContents(fileVersionId, contents);
        }

        public static void SaveFileContents(int fileVersionId, System.IO.Stream stream)
        {
            DataProvider.Instance().SaveFileContents(fileVersionId, stream);
        }

        public static byte[] GetFileContents(int fileVersionId)
        {
            return DataProvider.Instance().GetFileContents(fileVersionId);
        }

        public static void SaveThumbnail(int fileId, bool isLandscape, byte[] thumbnail)
        {
            DataProvider.Instance().SaveThumbnail(fileId, isLandscape, thumbnail);
        }

        public static byte[] GetThumbnail(int fileId)
        {
            return DataProvider.Instance().GetThumbnail(fileId);
        }

        public static void ChangeOwnership(int currentOwnerId, int newOwnerId, int portalId)
        {
            DataProvider.Instance().ChanngeDocumentOwnership(currentOwnerId, newOwnerId, portalId);
        }

        public static string ConnectionString
        {
            get
            {
                return DataProvider.Instance().GetConnectionString();
            }
        }

        public static FileType GetFileType(int fileTypeId)
        {
            return CBO.FillObject<FileType>(DataProvider.Instance().GetFileType(fileTypeId));
        }

        public static FileType GetFileTypeByExt(string fileTypeExt, int portalId, int tabModuleId)
        {
            return CBO.FillObject<FileType>(DataProvider.Instance().GetFileTypeByExt(fileTypeExt, portalId, tabModuleId));
        }

        public static List<FileType> GetAllFileTypes(int portalId, int tabModuleId)
        {
            return CBO.FillCollection<FileType>(DataProvider.Instance().GetAllFileTypes(portalId, tabModuleId));
        }

        public static void DeleteFileType(int fileTypeId)
        {
            DataProvider.Instance().DeleteFileType(fileTypeId);
        }

        public static int SaveFileType(FileType objFileType)
        {
            objFileType.FileTypeId = DataProvider.Instance().SaveFileType(objFileType);
            return objFileType.FileTypeId;
        }

        public static List<FileVersion> GetFileVersions(int fileId)
        {
            return CBO.FillCollection<FileVersion>(DataProvider.Instance().GetFileVersions(fileId));
        }

        public static FileVersion GetFileVersion(int fileVersionId)
        {
            return CBO.FillObject<FileVersion>(DataProvider.Instance().GetFileVersion(fileVersionId));
        }

        public static void DeleteFileVersion(int fileVersionId)
        {
            DataProvider.Instance().DeleteFileVersion(fileVersionId);
        }

        public static int SaveFileVersion(FileVersion objFileVersion)
        {
            objFileVersion.FileVersionId = DataProvider.Instance().SaveFileVersion(objFileVersion);
            return objFileVersion.FileVersionId;
        }

        public static Repository GetRepository(int portalId, int tabModuleId)
        {
            return CBO.FillObject<Repository>(DataProvider.Instance().GetRepository(portalId, tabModuleId));
        }

        public static List<Repository> GetAllRepositories(int portalId)
        {
            return CBO.FillCollection<Repository>(DataProvider.Instance().GetAllRepositories(portalId));
        }

        public static int SaveRepository(Repository objRepository)
        {
            DataProvider.Instance().SaveRepository(objRepository);
            return objRepository.RepositoryId;
        }

        public static DMSPortalSettings GetPortalSettings(int portalId)
        {
            return CBO.FillObject<DMSPortalSettings>(DataProvider.Instance().GetPortalSettings(portalId));
        }

        public static int SavePortalSettings(DMSPortalSettings objPortalSettings)
        {
            DataProvider.Instance().SavePortalSettings(objPortalSettings);
            return objPortalSettings.PortalId;
        }

        public static void AddDefaultFileExtensions(int portalId, int tabModuleId)
        {
            DataProvider.Instance().AddDefaultFileExtensions(portalId, tabModuleId);
        }
        #endregion
    }
}