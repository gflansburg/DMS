using System;
using System.Collections.Generic;
using DotNetNuke.Common.Utilities;
using System.Linq;
using Gafware.Modules.DMS.Data;

namespace Gafware.Modules.DMS.Components
{
    public class UserController
    {
        #region "Public Methods"

        public static DotNetNuke.Security.Roles.RoleInfo GetRoleById(int portalId, int roleId)
        {
            if(roleId < 0)
            {
                DotNetNuke.Security.Roles.RoleInfo role = new DotNetNuke.Security.Roles.RoleInfo();
                role.RoleID = roleId;
                switch (roleId)
                {
                    case -1:
                        role.RoleName = "All Users";
                        break;
                    case -2:
                        role.RoleName = "Superusers";
                        break;
                    case -3:
                        role.RoleName = "Unauthenticated Users";
                        break;
                }
                return role;
            }
            return DotNetNuke.Security.Roles.RoleController.Instance.GetRoleById(portalId, roleId);
        }

        public static List<DotNetNuke.Entities.Users.UserInfo> GetUsers(int roleId, int portalId)
        {
            return CBO.FillCollection<DotNetNuke.Entities.Users.UserInfo>(DataProvider.Instance().GetUsers(roleId, portalId));
        }

        public static List<DotNetNuke.Entities.Users.UserInfo> GetFileNotificationRecipients(int roleId, int portalId)
        {
            return CBO.FillCollection<DotNetNuke.Entities.Users.UserInfo>(DataProvider.Instance().GetFileNotificationRecipients(roleId, portalId));
        }

        #endregion
    }
}