using System;
using System.Collections.Generic;
using System.Linq;
using Gafware.Modules.DMS.Components;
using DotNetNuke.Services.Scheduling;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Common.Utilities;

namespace Gafware.Modules.DMS
{
    public class DMSJob : SchedulerClient 
    {
        public const string AssemblyName = "Gafware.Modules.DMS.DMSJob,Gafware.DMS";
        
        public DMSJob (ScheduleHistoryItem oItem) : base()
        {
            this.ScheduleHistoryItem = oItem;
        }

         public override void DoWork()
        {
            try
            {
                //Perform required items for logging
                this.Progressing();

                //Your code goes here
                PortalController portalController = new PortalController();
                PortalAliasController paController = new PortalAliasController();
                List<PortalInfo> portals = portalController.GetPortals().Cast<PortalInfo>().ToList();
                this.ScheduleHistoryItem.AddLogNote("Checking " + portals.Count.ToString() + " Portals<br />\r\n");
                foreach (PortalInfo portal in portals)
                {
                    string portalAlias = String.Empty;
#pragma warning disable CS0618 // Type or member is obsolete
                    foreach (var pa in paController.GetPortalAliasesByPortalId(portal.PortalID))
#pragma warning restore CS0618 // Type or member is obsolete
                    {
                        if (pa.IsPrimary)
                        {
#pragma warning disable CS0618 // Type or member is obsolete
                            portalAlias = pa.HTTPAlias;
#pragma warning restore CS0618 // Type or member is obsolete
                            break;
                        }
                    }
                    if (!String.IsNullOrEmpty(portalAlias))
                    {
                        this.ScheduleHistoryItem.AddLogNote("<br />\r\nChecking Documents For : " + portal.PortalName + "<br />\r\n");
#pragma warning disable CS0618 // Type or member is obsolete
                        Components.Repository portalSettings = Components.DocumentController.GetRepository(portal.PortalID, 0);
                        List<Document> documents = DocumentController.GetAllDocuments(portal.PortalID, 0);
#pragma warning restore CS0618 // Type or member is obsolete
                        foreach (Document doc in documents)
                        {
                            if (doc != null)
                            {
                                bool done = false;
                                foreach (DMSFile file in doc.Files)
                                {
                                    if (!file.FileType.Equals("url", StringComparison.OrdinalIgnoreCase) && file.StatusId == 1)
                                    {
                                        Generic.CreateThumbnail(portal, "/DesktopModules/Gafware/DMS", file);
                                        if ((!doc.ActivationDate.HasValue || DateTime.Now >= doc.ActivationDate.Value) && (!doc.ExpirationDate.HasValue || DateTime.Now <= (doc.ExpirationDate.Value + new TimeSpan(23, 59, 59))))
                                        {
                                            if (!System.IO.File.Exists(MapPath("~/" + file.UploadDirectory + "/" + file.Filename, portal)))
                                            {
                                                if (!done)
                                                {
                                                    this.ScheduleHistoryItem.AddLogNote("<br />\r\nActivating Files For: " + doc.DocumentName + "<br />\r\n");
                                                    done = true;
                                                }
                                                CreateFolder(file, portal);
                                                file.FileVersion.LoadContents();
                                                string fileName = MapPath("~/" + file.UploadDirectory + "/" + file.Filename, portal);
                                                this.ScheduleHistoryItem.AddLogNote("<br />\r\nWriting File: " + fileName + "<br />\r\n");
                                                System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
                                                fs.Write(file.FileVersion.Contents, 0, file.FileVersion.Contents.Length);
                                                fs.Close();
                                                try
                                                {
                                                    System.IO.File.SetLastWriteTime(MapPath("~/" + file.UploadDirectory + "/" + file.Filename, portal), file.FileVersion.CreatedOnDate);
                                                }
                                                catch (Exception)
                                                {
                                                }
                                            }
                                        }
                                        else if ((doc.ActivationDate.HasValue && DateTime.Now < doc.ActivationDate.Value) || (doc.ExpirationDate.HasValue && DateTime.Now > (doc.ExpirationDate.Value + new TimeSpan(23, 59, 59))) || file.StatusId == 2)
                                        {
                                            if (System.IO.File.Exists(MapPath("~/" + file.UploadDirectory + "/" + file.Filename, portal)))
                                            {
                                                if (!done)
                                                {
                                                    this.ScheduleHistoryItem.AddLogNote("<br />\r\nDeleting Files For: " + doc.DocumentName + "<br />\r\n");
                                                    done = true;
                                                }
                                                string fileName = MapPath("~/" + file.UploadDirectory + "/" + file.Filename, portal);
                                                this.ScheduleHistoryItem.AddLogNote("<br />\r\nDeleting File: " + fileName + "<br />\r\n");
                                                System.IO.File.Delete(fileName);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                this.ScheduleHistoryItem.AddLogNote("<br />\r\nFinished<br />\r\n");
                this.ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception ex)
            {
                this.ScheduleHistoryItem.Succeeded = false;
                this.ScheduleHistoryItem.AddLogNote("Exception= " + ex.ToString());
                this.Errored(ref ex);
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
            }
        }

        private void CreateFolder(DMSFile file, PortalInfo portal)
        {
            if (!System.IO.Directory.Exists(MapPath("~/Files", portal)))
            {
                System.IO.Directory.CreateDirectory(MapPath("~/Files", portal));
            }
            if (!String.IsNullOrEmpty(file.UploadDirectory) && !System.IO.Directory.Exists(MapPath("~/" + file.UploadDirectory, portal)))
            {
                System.IO.Directory.CreateDirectory(MapPath("~/" + file.UploadDirectory, portal));
            }
        }

        private string MapPath(string virtualPath, PortalInfo portal)
        {
            return virtualPath.Replace("/", "\\").Replace("~", portal.HomeDirectoryMapPath).Replace("\\\\", "\\");
        }
    }
}
