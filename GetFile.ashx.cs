using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gafware.Modules.DMS
{
    /// <summary>
    /// Summary description for GetFile
    /// </summary>
    public class GetFile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if(!String.IsNullOrEmpty(context.Request.QueryString["id"]))
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
                        if (Generic.UserHasAccess(doc))
                        {
                            file.FileVersion.LoadContents();
                            context.Response.ContentType = file.MimeType;
                            context.Response.AppendHeader("pragma", "no-cache");
                            context.Response.AppendHeader("expires", "0");
                            context.Response.AppendHeader("cache-control", "no-cache, no-store, must-revalidate, max-age=0");
                            context.Response.AppendHeader("content-control", "no-cache, no-store, must-revalidate, max-age=0");
                            context.Response.AppendHeader("content-disposition", "inline;filename=\"" + string.Format("{0}.{1}", Generic.CreateSafeFolderName(doc.DocumentName), file.FileType) + "\";creation-date=\"" + DateTimeParser.ToRFC822(file.CreatedOnDate) + "\"");
                            if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                            {
                                context.Response.BinaryWrite(file.FileVersion.Contents);
                            }
                        }
                        else
                        {
                            if (context.Request.IsAuthenticated)
                            {
                                context.Response.ContentType = "text/html";
                                context.Response.AppendHeader("pragma", "no-cache");
                                context.Response.AppendHeader("expires", "0");
                                context.Response.AppendHeader("cache-control", "no-cache, no-store, must-revalidate, max-age=0");
                                context.Response.AppendHeader("content-control", "no-cache, no-store, must-revalidate, max-age=0");
                                context.Response.AppendHeader("content-disposition", "inline;filename=\"" + string.Format("{0}.{1}", Generic.CreateSafeFolderName(doc.DocumentName), file.FileType) + "\";creation-date=\"" + DateTimeParser.ToRFC822(file.CreatedOnDate) + "\"");
                                context.Response.Write("<html><head><title>Unauthorized</title></head><body><h1>Unauthorized</h1></body></html>");
                            }
                            else
                            {
                                context.Response.Redirect(string.Format("/login?returnurl={0}", Generic.UrlEncode(string.Format("/DesktopModules/Gafware/DMS/GetFile.ashx?id={0}", context.Request.QueryString["id"]))), true);
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
                            if (Generic.UserHasAccess(doc))
                            {
                                fileVersion.LoadContents();
                                context.Response.ContentType = file.MimeType;
                                context.Response.AppendHeader("pragma", "no-cache");
                                context.Response.AppendHeader("expires", "0");
                                context.Response.AppendHeader("cache-control", "no-cache, no-store, must-revalidate, max-age=0");
                                context.Response.AppendHeader("content-control", "no-cache, no-store, must-revalidate, max-age=0");
                                context.Response.AppendHeader("content-disposition", "inline;filename=\"" + string.Format("{0}.{1}", Generic.CreateSafeFolderName(doc.DocumentName), file.FileType) + "\";creation-date=\"" + DateTimeParser.ToRFC822(file.CreatedOnDate) + "\"");
                                if (fileVersion.Contents != null && fileVersion.Contents.Length > 0)
                                {
                                    context.Response.BinaryWrite(fileVersion.Contents);
                                }
                            }
                            else
                            {
                                if (context.Request.IsAuthenticated)
                                {
                                    context.Response.ContentType = "text/html";
                                    context.Response.AppendHeader("pragma", "no-cache");
                                    context.Response.AppendHeader("expires", "0");
                                    context.Response.AppendHeader("cache-control", "no-cache, no-store, must-revalidate, max-age=0");
                                    context.Response.AppendHeader("content-control", "no-cache, no-store, must-revalidate, max-age=0");
                                    context.Response.AppendHeader("content-disposition", "inline;filename=\"" + string.Format("{0}.{1}", Generic.CreateSafeFolderName(doc.DocumentName), file.FileType) + "\";creation-date=\"" + DateTimeParser.ToRFC822(file.CreatedOnDate) + "\"");
                                    context.Response.Write("<html><head><title>Unauthorized</title></head><body><h1>Unauthorized</h1></body></html>");
                                }
                                else
                                {
                                    context.Response.Redirect(string.Format("/login?returnurl={0}", Generic.UrlEncode(string.Format("/DesktopModules/Gafware/DMS/GetFile.ashx?vid={0}", context.Request.QueryString["vid"]))), true);
                                }
                            }
                        }
                    }
                }
            }
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