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
    [XmlType("DocumentCategory")]
    [XmlRoot("ContentItem")]
    public class DocumentCategory : ContentItem, IEquatable<Category>, IComparable<Category>
    {
        /// <summary>
        /// Id of category document
        /// </summary>
        public int DocumentCategoryId { get; set; }
        /// <summary>
        /// Id of document
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Id of category
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// Category
        /// </summary>
        public Category Category { get; set; }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            DocumentCategoryId = Null.SetNullInteger(dr["DocumentCategoryID"]);
            DocumentId = Null.SetNullInteger(dr["DocumentID"]);
            CategoryId = Null.SetNullInteger(dr["CategoryID"]);
            Category = DocumentController.GetCategory(CategoryId);
        }

        public bool Equals(Category other)
        {
            if (this.CategoryId == other.CategoryId)
            {
                return true;
            }
            return false;
        }

        public int CompareTo(Category other)
        {
            if (this.CategoryId < other.CategoryId)
            {
                return -1;
            }
            if (this.CategoryId > other.CategoryId)
            {
                return 1;
            }
            return 0;
        }

        public override int KeyID
        {
            get
            {
                return DocumentCategoryId;
            }
            set
            {
                DocumentCategoryId = value;
            }
        }
    }
}