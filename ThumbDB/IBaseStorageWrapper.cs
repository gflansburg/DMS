using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

namespace Gafware.Modules.DMS.ThumbDBLib
{
    /// <summary>
    /// The class <c>IBaseStorageWrapper</c> is the base wrapper for the Interop.IStorage interface
    /// </summary>
    public class IBaseStorageWrapper
    {
        /// <summary>
        /// Internal member storing the actual storage interface
        /// </summary>
        protected Interop.IStorage storage;
        /// <summary>
        /// Internal member storing the baseUrl of the file(s)
        /// </summary>
        private static string baseUrl;
        /// <summary>
        /// Internal member storing the file object collection (if files where enumerated)
        /// </summary>
        public FileObjectCollection foCollection;

        /// <summary>
        /// Gets the internal Interop.IStorage member
        /// </summary>
        public Interop.IStorage Storage
        {
            get { return storage; }
        }

        /// <summary>
        /// Gets the base url of files
        /// </summary>
        public static string BaseUrl
        {
            get
            {
                return baseUrl;
            }

            set
            {
                baseUrl = String.Concat(value, "::/");
            }
        }

        /// <summary>
        /// Constructor of the class
        /// </summary>
        public IBaseStorageWrapper()
        {
            foCollection = new FileObjectCollection();
        }

        /// <summary>
        /// Enumerates an Interop.IStorage object and creates the internal file object collection
        /// </summary>
        /// <param name="stgEnum">Interop.IStorage to enumerate</param>
        public virtual void EnumIStorageObject(Interop.IStorage stgEnum)
        {
            EnumIStorageObject(stgEnum, "");
        }

        /// <summary>
        /// Enumerates an Interop.IStorage object and creates the internal file object collection
        /// </summary>
        /// <param name="stgEnum">Interop.IStorage to enumerate</param>
        /// <param name="BasePath">Sets the base url for the storage files</param>
        protected void EnumIStorageObject(Interop.IStorage stgEnum, string BasePath)
        {
            Interop.IEnumSTATSTG iEnumSTATSTG;

            System.Runtime.InteropServices.ComTypes.STATSTG sTATSTG;

            int i;

            stgEnum.EnumElements(0, IntPtr.Zero, 0, out iEnumSTATSTG);
            iEnumSTATSTG.Reset();
            while (iEnumSTATSTG.Next(1, out sTATSTG, out i) == (int)Interop.S_OK)
            {
                if(i==0)
                    break;

                FileObject newFileObj = new FileObject();
                newFileObj.FileType = sTATSTG.type;
                switch (sTATSTG.type)
                {
                case 1:
                    Interop.IStorage iStorage = stgEnum.OpenStorage(sTATSTG.pwcsName, IntPtr.Zero, 16, IntPtr.Zero, 0);
                    if (iStorage != null)
                    {
                        string str = String.Concat(BasePath, sTATSTG.pwcsName.ToString());
                        newFileObj.FileStorage = iStorage;
                        newFileObj.FilePath = BasePath;
                        newFileObj.FileName = sTATSTG.pwcsName.ToString();
                        foCollection.Add(newFileObj);
                        EnumIStorageObject(iStorage, str);
                    }
                    break;

                case 2:
                        System.Runtime.InteropServices.ComTypes.IStream uCOMIStream = stgEnum.OpenStream(sTATSTG.pwcsName, IntPtr.Zero, 16, 0);
                    newFileObj.FilePath = BasePath;
                    newFileObj.FileName = sTATSTG.pwcsName.ToString();
                    newFileObj.FileStream = uCOMIStream;
                    foCollection.Add(newFileObj);
                    break;

                case 4:
                    Debug.WriteLine("Ignoring IProperty type ...");
                    break;

                case 3:
                    Debug.WriteLine("Ignoring ILockBytes type ...");
                    break;

                default:
                    Debug.WriteLine("Unknown object type ...");
                    break;
                }
            }
        }
    }

}
