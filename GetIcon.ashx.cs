using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafware.Modules.DMS
{
    /// <summary>
    /// Summary description for GetFile
    /// </summary>
    public class GetIcon : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (!String.IsNullOrEmpty(context.Request.QueryString["id"]))
            {
                string strQuery = (context.Request.QueryString["id"] ?? String.Empty).Trim();
                int fileId = 0;
                try
                {
                    fileId = Convert.ToInt32(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Decrypt(strQuery));
                }
                catch
                {
                    try
                    {
                        byte[] aQuery = Generic.StringToByteArray(strQuery);
                        string strNewQuery = HttpUtility.UrlDecode(System.Text.Encoding.ASCII.GetString(aQuery));
                        fileId = Convert.ToInt32(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Decrypt(strNewQuery));
                    }
                    catch
                    {
                        if (Generic.IsNumber(strQuery))
                        {
                            fileId = Convert.ToInt32(strQuery);
                        }
                    }
                }
                Components.DMSFile file = Components.DocumentController.GetFile(fileId);
                if (file != null)
                {
                    Components.Document doc = Components.DocumentController.GetDocument(file.DocumentId);
                    if (doc != null)
                    {
                        Components.DMSPortalSettings settings = Components.DocumentController.GetPortalSettings(doc.PortalId);
                        if(settings == null)
                        {
                            settings = new Components.DMSPortalSettings();
                        }
                        Components.Repository repository = Components.DocumentController.GetRepository(doc.PortalId, settings.PortalWideRepository ? 0 : doc.TabModuleId);
                        if (repository != null)
                        {
                            file.FileVersion.LoadThumbnail();
                            context.Response.AppendHeader("pragma", "no-cache");
                            context.Response.AppendHeader("expires", "0");
                            context.Response.AppendHeader("cache-control", "no-cache, no-store, must-revalidate, max-age=0");
                            context.Response.AppendHeader("content-control", "no-cache, no-store, must-revalidate, max-age=0");
                            if (repository.UseThumbnails && file.FileVersion.Thumbnail != null && file.FileVersion.Thumbnail.Length > 0)
                            {
                                context.Response.ContentType = "image/png";
                                context.Response.AppendHeader("content-disposition", "inline;filename=\"" + System.IO.Path.GetFileNameWithoutExtension(file.Filename) + ".png\";creation-date=\"" + DateTimeParser.ToRFC822(file.CreatedOnDate) + "\"");
                                context.Response.BinaryWrite(file.FileVersion.Thumbnail);
                            }
                            else
                            {
                                string mimeType = string.Empty;
                                file.FileVersion.Thumbnail = GetThumbnail(context, file.FileType, repository.ThumbnailType, ref mimeType);
                                context.Response.ContentType = mimeType;
                                context.Response.AppendHeader("content-disposition", "inline;filename=\"" + file.FileType + "." + (mimeType.Equals("image/png") ? "png" : "svg") + "\";creation-date=\"" + DateTimeParser.ToRFC822(file.CreatedOnDate) + "\"");
                                context.Response.BinaryWrite(file.FileVersion.Thumbnail);
                            }
                        }
                    }
                }
            }
            else if (!String.IsNullOrEmpty(context.Request.QueryString["vid"]) && Generic.IsNumber(context.Request.QueryString["vid"]))
            {
                int fileVersionId = Convert.ToInt32(context.Request.QueryString["vid"]);
                Components.FileVersion fileVersion = Components.DocumentController.GetFileVersion(fileVersionId);
                if (fileVersion != null)
                {
                    Components.DMSFile file = Components.DocumentController.GetFile(fileVersion.FileId);
                    if (file != null)
                    {
                        Components.Document doc = Components.DocumentController.GetDocument(file.DocumentId);
                        if (doc != null)
                        {
                            Components.DMSPortalSettings settings = Components.DocumentController.GetPortalSettings(doc.PortalId);
                            if(settings == null)
                            {
                                settings = new Components.DMSPortalSettings();
                            }
                            Components.Repository repository = Components.DocumentController.GetRepository(doc.PortalId, settings.PortalWideRepository ? 0 : doc.TabModuleId);
                            if (repository != null)
                            {
                                fileVersion.LoadThumbnail();
                                context.Response.AppendHeader("pragma", "no-cache");
                                context.Response.AppendHeader("expires", "0");
                                context.Response.AppendHeader("cache-control", "no-cache, no-store, must-revalidate, max-age=0");
                                context.Response.AppendHeader("content-control", "no-cache, no-store, must-revalidate, max-age=0");
                                if (repository.UseThumbnails && fileVersion.Thumbnail != null && fileVersion.Thumbnail.Length > 0)
                                {
                                    context.Response.ContentType = "image/png";
                                    context.Response.AppendHeader("content-disposition", "inline;filename=\"" + System.IO.Path.GetFileNameWithoutExtension(file.Filename) + ".png\";creation-date=\"" + DateTimeParser.ToRFC822(file.CreatedOnDate) + "\"");
                                    context.Response.BinaryWrite(fileVersion.Thumbnail);
                                }
                                else
                                {
                                    string mimeType = string.Empty;
                                    fileVersion.Thumbnail = GetThumbnail(context, file.FileType, repository.ThumbnailType, ref mimeType);
                                    context.Response.ContentType = mimeType;
                                    context.Response.AppendHeader("content-disposition", "inline;filename=\"" + file.FileType + "." + (mimeType.Equals("image/png") ? "png" : "svg") + "\";creation-date=\"" + DateTimeParser.ToRFC822(file.CreatedOnDate) + "\"");
                                    context.Response.BinaryWrite(fileVersion.Thumbnail);
                                }
                            }
                        }
                    }
                }
            }
        }

        private byte[] GetThumbnail(HttpContext context, string fileType, string thumbnailType, ref string mimeType)
        {
            mimeType = "image/svg+xml";
            string icon = context.Request.MapPath(String.Format("/DesktopModules/Gafware/DMS/Images/icons/{0}/{1}.svg", thumbnailType, fileType.Equals("url", StringComparison.OrdinalIgnoreCase) ? "htm" : fileType));
            if (!System.IO.File.Exists(icon))
            {
                mimeType = "image/png";
                icon = context.Request.MapPath(String.Format("/DesktopModules/Gafware/DMS/Images/icons/_blank.png"));
            }
            return System.IO.File.ReadAllBytes(icon);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}