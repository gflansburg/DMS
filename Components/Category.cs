using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content;
using Gafware.Modules.DMS.Data;
using System.Xml.Serialization;
using DotNetNuke.Security.Roles;

namespace Gafware.Modules.DMS.Components
{
    [Serializable]
    [XmlType("Category")]
    [XmlRoot("ContentItem")]
    public class Category : ContentItem, IEquatable<Category>, IComparable<Category>
    {
        /// <summary>
        /// Id of Category
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// Category name
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// Portal Id
        /// </summary>
        public int PortalId { get; set; }
        /// <summary>
        /// Id of required role
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// Name of required role
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// Required role
        /// </summary>
        public RoleInfo Role { get; set; }
        /// <summary>
        /// Tab Module Id
        /// </summary>
        public int TabModuleId { get; set; }


        public Category()
        {
            RoleId = -1;
        }

        public override void Fill(IDataReader dr)
        {
            //base.Fill(dr);

            CategoryId = Null.SetNullInteger(dr["CategoryID"]);
            CategoryName = Null.SetNullString(dr["Category"]);
            PortalId = Null.SetNullInteger(dr["PortalId"]);
            RoleId = Null.SetNullInteger(dr["RoleId"]);
            Role = UserController.GetRoleById(PortalId, RoleId);
            RoleName = Role.RoleName;
            TabModuleId = Null.SetNullInteger(dr["TabModuleId"]);
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
                return CategoryId;
            }
            set
            {
                CategoryId = value;
            }
        }
    }
}