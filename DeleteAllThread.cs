using Gafware.Modules.DMS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Gafware.Modules.DMS
{
    public class DeleteAllThread
    {
        private System.Threading.Thread _worker = null;
        private string _processName = Guid.NewGuid().ToString();
        public event EventHandler Finished;

        public HttpRequest Request { get; private set; }
        public int PortalId { get; private set; }
        public int TabModuleId { get; private set; }
        public bool PortalWideRepository { get; private set; }
        public System.Data.DataView DataView { get; private set; }
        public int Progress { get; private set; }
        public int FilesDeleted { get; private set; }
        public int FileCount { get; private set; }

        public DeleteAllThread(HttpRequest request)
        {
            Request = request;
        }

        public string ProcessName
        {
            get
            {
                return _processName;
            }
        }

        public void DeleteAll(System.Data.DataView dataView, int portalId, int tabModuleId, bool portalWideRepository)
        {
            PortalId = portalId;
            TabModuleId = tabModuleId;
            PortalWideRepository = portalWideRepository;
            DataView = dataView;
            _worker = new System.Threading.Thread(new System.Threading.ThreadStart(DoDeleteAll));
            _worker.Start();
        }

        private void DoDeleteAll()
        {
            FileCount = GetFileCount();
            DeleteAll();
            Finished?.Invoke(this, EventArgs.Empty);
        }

        private int GetFileCount()
        {
            return (DataView != null ? DataView.Count : 0);
        }

        public void DeleteAll()
        {
            FilesDeleted = 0;
            DotNetNuke.Entities.Portals.PortalSettings portal = DotNetNuke.Entities.Portals.PortalSettings.Current;
            Repository repository = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
            if (repository != null)
            {
                foreach (System.Data.DataRowView row in DataView)
                {
                    int documentId = (int)row["DocumentID"];
                    if (repository.SaveLocalFile)
                    {
                        Components.Document doc = Components.DocumentController.GetDocument(documentId);
                        if (doc != null)
                        {
                            string uploadDirectory = String.Format("{0}Files\\{1}", portal.HomeDirectoryMapPath, Generic.CreateSafeFolderName(doc.DocumentName));
                            if (System.IO.Directory.Exists(uploadDirectory))
                            {
                                System.IO.Directory.Delete(uploadDirectory, true);
                            }
                        }
                    }
                    Components.DocumentController.DeleteDocument(documentId);
                    Progress = (int)(((double)FilesDeleted * 100.0) / (double)FileCount);
                    FilesDeleted++;
                }
            }
        }
    }
}