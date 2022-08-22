using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Ghostscript.NET.Processor;
using Ghostscript.NET.Rasterizer;
using Gafware.Modules.DMS.Components;

namespace Gafware.Modules.DMS
{
    public class Thumbnail
    {
        public Gafware.Modules.DMS.Components.Repository Repository { get; set; } 

        public DotNetNuke.Entities.Portals.PortalInfo Portal { get; set; }

        public string ControlPath { get; set; }

        public Thumbnail(DotNetNuke.Entities.Portals.PortalInfo portal, Gafware.Modules.DMS.Components.Repository repository, string controlPath)
        {
            Portal = portal;
            Repository = repository;
            ControlPath = controlPath;
        }

        private byte[] CreateThumbnail(HttpRequest request, System.Drawing.Bitmap img, ref bool isLandscape, ThumbnailMode thumbnailMode = ThumbnailMode.Auto, ThumbnailResize thumbnailResize = ThumbnailResize.Stretch)
        {
            byte[] byteImage = null;
            string templatePortraitFile = request.MapPath(string.Format("{0}/Images/pdftemplate_portrait.png", ControlPath));
            string templateLandscapeFile = request.MapPath(string.Format("{0}/Images/pdftemplate_landscape.png", ControlPath));
            // Size of generated thumbnail in pixels
            int thumbnailWidth = 97;
            int thumbnailHeight = 128;
            string templateFile = templatePortraitFile;
            // Switch between portrait and landscape
            if (img.Width <= img.Height || thumbnailMode == ThumbnailMode.Portrait)
            {
                templateFile = templatePortraitFile;
                isLandscape = false;
            }
            else if (img.Width > img.Height || thumbnailMode == ThumbnailMode.Landscape)
            {
                templateFile = templateLandscapeFile;
                // Swap width and height (little trick not using third temp variable)
                thumbnailWidth ^= thumbnailHeight;
                thumbnailHeight = thumbnailWidth ^ thumbnailHeight;
                thumbnailWidth ^= thumbnailHeight;
                isLandscape = true;
            }
            // Load the template graphic
            using (Bitmap templateBitmap = new Bitmap(templateFile))
            {
                // Create new blank bitmap
                using (Bitmap thumbnailBitmap = new Bitmap(thumbnailWidth, thumbnailHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    // To overlay the template with the image, we need to set the transparency
                    templateBitmap.MakeTransparent(Color.Magenta);
                    using (Graphics thumbnailGraphics = Graphics.FromImage(thumbnailBitmap))
                    {
                        thumbnailGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        thumbnailGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        thumbnailGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        thumbnailGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        // Draw rendered pdf image to new blank bitmap
                        if (thumbnailResize == ThumbnailResize.Stretch)
                        {
                            thumbnailGraphics.DrawImage(img, new Rectangle(2, 2, thumbnailWidth, thumbnailHeight), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
                        }
                        else
                        {
                            thumbnailGraphics.FillRectangle(Brushes.White, 0, 0, thumbnailWidth, thumbnailHeight);
                            double ratioX = (double)(thumbnailWidth - 4) / img.Width;
                            double ratioY = (double)(thumbnailHeight - 4) / img.Height;
                            double ratio = Math.Min(ratioX, ratioY);
                            int newWidth = (int)(img.Width * ratio);
                            int newHeight = (int)(img.Height * ratio);
                            thumbnailGraphics.DrawImage(img, new Rectangle(2 + (((thumbnailWidth - 4) - newWidth) / 2), 2 + (((thumbnailHeight - 4) - newHeight) / 2), newWidth, newHeight), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
                        }
                        // Draw template outline over the bitmap (pdf with show through the transparent area)
                        thumbnailGraphics.DrawImage(templateBitmap, 0, 0);
                        //to save in database
                        byteImage = ImageToByteArray(thumbnailBitmap);
                    }
                }
            }
            return byteImage;
        }

        private byte[] CreateThumbnail(HttpRequest request, string inputFile, ref bool isLandscape)
        {
            try
            {
                using (GhostscriptRasterizer rasterizer = new GhostscriptRasterizer())
                {
                    const int desiredDpi = 96; 
                    rasterizer.CustomSwitches.Add("-dUseCropBox");
                    rasterizer.CustomSwitches.Add("-c");
                    rasterizer.CustomSwitches.Add("[/CropBox [24 72 559 794] /PAGES pdfmark");
                    rasterizer.CustomSwitches.Add("-f");
                    int pageNumber = 1;
                    using (System.IO.FileStream stream = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        try
                        {
                            rasterizer.Open(stream);
                            //rasterizer.Open(inputFile);
                            int pageCount = rasterizer.PageCount;
                            if (pageNumber <= pageCount)
                            {
                                using (Image img = rasterizer.GetPage(desiredDpi, pageNumber))
                                {
                                    if (img != null)
                                    {
                                        using (Bitmap bmp = new Bitmap(img))
                                        {
                                            if (!IsImageBlank(bmp))
                                            {
                                                return CreateThumbnail(request, bmp, ref isLandscape);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            rasterizer.Close();
                            stream.Close();
                        }
                    }
                    rasterizer.CustomSwitches.Clear();
                    using (System.IO.FileStream stream = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        try
                        {
                            rasterizer.CustomSwitches.Add("-dPDFFitPage");
                            rasterizer.Open(stream);
                            //rasterizer.Open(inputFile);
                            int pageCount = rasterizer.PageCount;
                            if (pageNumber <= pageCount)
                            {
                                using (Image img = rasterizer.GetPage(desiredDpi, pageNumber))
                                {
                                    if (img != null)
                                    {
                                        using (Bitmap bmp = new Bitmap((img.Width > img.Height ? (img.Height * 83) / 109 : img.Width), img.Height, PixelFormat.Format24bppRgb))
                                        {
                                            using (Graphics g = Graphics.FromImage(bmp))
                                            {
                                                g.DrawImage(img, 0, 0);
                                            }
                                            return CreateThumbnail(request, bmp, ref isLandscape);
                                        }
                                    }
                                }
                            }
                        }
                        catch(Exception)
                        {
                        }
                        finally
                        {
                            rasterizer.Close();
                            stream.Close();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public void CreateThumbnail(HttpRequest request, Components.DMSFile file)
        {
            if (Repository.UseThumbnails && !file.FileVersion.HasThumbnail)
            {
                file.FileVersion.LoadThumbnail();
                if (file.FileVersion.Thumbnail == null || file.FileVersion.Thumbnail.Length == 0)
                {
                    bool isLandscape = false;
                    if (file != null && file.FileType.Equals("pdf", StringComparison.OrdinalIgnoreCase) && file.Status.StatusId == 1 && Repository.CreatePDF)
                    {
                        if (file.FileVersion.Contents == null || file.FileVersion.Contents.Length == 0)
                        {
                            file.FileVersion.LoadContents();
                        }
                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                        {
                            string tempPdf = String.Format("{0}{1}_temp.pdf", System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(file.Filename));
                            using (var fs = System.IO.File.Create(tempPdf))
                            {
                                fs.Write(file.FileVersion.Contents, 0, file.FileVersion.Contents.Length);
                                fs.Close();
                            }
                            file.FileVersion.Thumbnail = CreateThumbnail(request, tempPdf, ref isLandscape);
                            if (file.FileVersion.Thumbnail != null)
                            {
                                file.FileVersion.SaveThumbnail(isLandscape);
                            }
                            if (System.IO.File.Exists(tempPdf))
                            {
                                try
                                {
                                    System.IO.File.Delete(tempPdf);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    else if (file != null && (file.FileType.Equals("png", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("jpg", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("gif", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("bmp", StringComparison.OrdinalIgnoreCase)) && file.Status.StatusId == 1 && Repository.CreateImage)
                    {
                        if (file.FileVersion.Contents == null || file.FileVersion.Contents.Length == 0)
                        {
                            file.FileVersion.LoadContents();
                        }
                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(file.FileVersion.Contents))
                            {
                                using (Bitmap bmp = new Bitmap(ms))
                                {
                                    file.FileVersion.Thumbnail = CreateThumbnail(request, bmp, ref isLandscape);
                                    if (file.FileVersion.Thumbnail != null)
                                    {
                                        file.FileVersion.SaveThumbnail(isLandscape);
                                    }
                                }
                            }
                        }
                    }
                    else if (file != null && (file.FileType.Equals("doc", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("docx", StringComparison.OrdinalIgnoreCase)) && file.Status.StatusId == 1 && Repository.CreateWord)
                    {
                        if (file.FileVersion.Contents == null || file.FileVersion.Contents.Length == 0)
                        {
                            file.FileVersion.LoadContents();
                        }
                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                        {
                            string tempPdf = String.Format("{0}{1}_temp.pdf", System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(file.Filename));
                            try
                            {
                                using (MemoryStream docStream = new MemoryStream(file.FileVersion.Contents))
                                {
                                    using (Spire.Doc.Document doc = new Spire.Doc.Document(docStream))
                                    {
                                        doc.SaveToFile(tempPdf, Spire.Doc.FileFormat.PDF);
                                        doc.Close();
                                    }
                                    docStream.Close();
                                }
                            }
                            catch (Exception)
                            {
                            }
                            if (System.IO.File.Exists(tempPdf))
                            {
                                file.FileVersion.Thumbnail = CreateThumbnail(request, tempPdf, ref isLandscape);
                                if (file.FileVersion.Thumbnail != null)
                                {
                                    file.FileVersion.SaveThumbnail(isLandscape);
                                }
                                try
                                {
                                    System.IO.File.Delete(tempPdf);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    else if (file != null && (file.FileType.Equals("xls", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("xlsx", StringComparison.OrdinalIgnoreCase)) && file.Status.StatusId == 1 && Repository.CreateExcel)
                    {
                        if (file.FileVersion.Contents == null || file.FileVersion.Contents.Length == 0)
                        {
                            file.FileVersion.LoadContents();
                        }
                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                        {
                            string tempPdf = String.Format("{0}{1}_temp.pdf", System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(file.Filename));
                            try
                            {
                                using (MemoryStream xlsStream = new MemoryStream(file.FileVersion.Contents))
                                {
                                    using (Spire.Xls.Workbook workbook = new Spire.Xls.Workbook())
                                    {
                                        try
                                        {
                                            //workbook.LoadFromFile(tempExcel);
                                            workbook.LoadFromStream(xlsStream);
                                            workbook.ConverterSetting.SheetFitToPage = true;
                                            workbook.SaveToFile(tempPdf, Spire.Xls.FileFormat.PDF);
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                    xlsStream.Close();
                                }
                            }
                            catch (Exception)
                            {
                            }
                            if (System.IO.File.Exists(tempPdf))
                            {
                                file.FileVersion.Thumbnail = CreateThumbnail(request, tempPdf, ref isLandscape);
                                if (file.FileVersion.Thumbnail != null)
                                {
                                    file.FileVersion.SaveThumbnail(isLandscape);
                                }
                                try
                                {
                                    System.IO.File.Delete(tempPdf);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    else if (file != null && (file.FileType.Equals("ppt", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("pptx", StringComparison.OrdinalIgnoreCase)) && file.Status.StatusId == 1 && Repository.CreatePowerPoint)
                    {
                        if (file.FileVersion.Contents == null || file.FileVersion.Contents.Length == 0)
                        {
                            file.FileVersion.LoadContents();
                        }
                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                        {
                            string tempPdf = String.Format("{0}{1}_temp.pdf", System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(file.Filename));
                            try
                            {
                                using (MemoryStream presentationStream = new MemoryStream(file.FileVersion.Contents))
                                {
                                    using (Spire.Presentation.Presentation presentation = new Spire.Presentation.Presentation(presentationStream, Spire.Presentation.FileFormat.Auto))
                                    {
                                        presentation.SaveToFile(tempPdf, Spire.Presentation.FileFormat.PDF);
                                    }
                                    presentationStream.Close();
                                }
                            }
                            catch(Exception)
                            {
                            }
                            if (System.IO.File.Exists(tempPdf))
                            {
                                file.FileVersion.Thumbnail = CreateThumbnail(request, tempPdf, ref isLandscape);
                                if (file.FileVersion.Thumbnail != null)
                                {
                                    file.FileVersion.SaveThumbnail(isLandscape);
                                }
                                try
                                {
                                    System.IO.File.Delete(tempPdf);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    else if (file != null && file.FileType.Equals("mp3", StringComparison.OrdinalIgnoreCase) && file.Status.StatusId == 1 && Repository.CreateAudio)
                    {
                        if (file.FileVersion.Contents == null || file.FileVersion.Contents.Length == 0)
                        {
                            file.FileVersion.LoadContents();
                        }
                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                        {
                            try
                            {
                                using (MemoryStream mp3Stream = new MemoryStream(file.FileVersion.Contents))
                                {
                                    using (Id3.Mp3 mp3 = new Id3.Mp3(mp3Stream))
                                    {
                                        if (mp3 != null)
                                        {
                                            Id3.Id3Tag tag = mp3.GetTag(Id3.Id3TagFamily.Version2X);
                                            if (tag != null)
                                            {
                                                if (tag.Pictures.Count > 0)
                                                {
                                                    using (MemoryStream ms = new MemoryStream(tag.Pictures[0].PictureData))
                                                    {
                                                        using (Bitmap bmp = new Bitmap(ms))
                                                        {
                                                            file.FileVersion.Thumbnail = CreateThumbnail(request, bmp, ref isLandscape, ThumbnailMode.Auto, ThumbnailResize.MaintainAspectRatio);
                                                            if (file.FileVersion.Thumbnail != null)
                                                            {
                                                                file.FileVersion.SaveThumbnail(isLandscape);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    else if (file != null && file.Status.StatusId == 1 && (((file.FileType.Equals("mp4", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("mov", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("wmv", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("avi", StringComparison.OrdinalIgnoreCase)) && Repository.CreateVideo) || (file.FileType.Equals("wma", StringComparison.OrdinalIgnoreCase) && Repository.CreateAudio) || ((file.FileType.Equals("ppt", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("pptx", StringComparison.OrdinalIgnoreCase) && Repository.CreatePowerPoint))))
                    {
                        if (file.FileVersion.Contents == null || file.FileVersion.Contents.Length == 0)
                        {
                            file.FileVersion.LoadContents();
                        }
                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                        {
                            string tempFile = String.Format("{0}{1}_temp{2}", System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(file.Filename), System.IO.Path.GetExtension(file.Filename));
                            using (var fs = System.IO.File.Create(tempFile))
                            {
                                fs.Write(file.FileVersion.Contents, 0, file.FileVersion.Contents.Length);
                                fs.Close();
                            }
                            using (Microsoft.WindowsAPICodePack.Shell.ShellFile shellFile = Microsoft.WindowsAPICodePack.Shell.ShellFile.FromFilePath(tempFile))
                            {
                                Bitmap bmp = new Bitmap(shellFile.Thumbnail.LargeBitmap);
                                bmp.MakeTransparent();
                                file.FileVersion.Thumbnail = CreateThumbnail(request, bmp, ref isLandscape, ThumbnailMode.Auto, ThumbnailResize.MaintainAspectRatio);
                                if (file.FileVersion.Thumbnail != null)
                                {
                                    file.FileVersion.SaveThumbnail(isLandscape);
                                }
                                if (System.IO.File.Exists(tempFile))
                                {
                                    try
                                    {
                                        System.IO.File.Delete(tempFile);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
                file.FileVersion.Contents = null;
                file.FileVersion.Thumbnail = null;
                GC.Collect();
            }
        }

        public void CreateThumbnail(HttpRequest request, string fileName, int fileVersionId)
        {
            if (Repository.UseThumbnails)
            {
                if (System.IO.File.Exists(fileName))
                {
                    bool isLandscape = false;
                    if (System.IO.Path.GetExtension(fileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase) && Repository.CreatePDF)
                    {
                        byte[] thumbnail = CreateThumbnail(request, fileName, ref isLandscape);
                        if (thumbnail != null && thumbnail.Length > 0)
                        {
                            DocumentController.SaveThumbnail(fileVersionId, isLandscape, thumbnail);
                        }
                    }
                    else if ((System.IO.Path.GetExtension(fileName).Equals(".png", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".jpg", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".gif", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".bmp", StringComparison.OrdinalIgnoreCase)) && Repository.CreateImage)
                    {
                        using (Bitmap bmp = new Bitmap(fileName))
                        {
                            byte[] thumbnail = CreateThumbnail(request, bmp, ref isLandscape);
                            if (thumbnail != null && thumbnail.Length > 0)
                            {
                                DocumentController.SaveThumbnail(fileVersionId, isLandscape, thumbnail);
                            }
                        }
                    }
                    else if ((System.IO.Path.GetExtension(fileName).Equals(".doc", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".docx", StringComparison.OrdinalIgnoreCase)) && Repository.CreateWord)
                    {
                        string tempPdf = String.Format("{0}{1}_temp.pdf", System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(fileName));
                        try
                        {
                            using (Spire.Doc.Document doc = new Spire.Doc.Document(fileName))
                            {
                                doc.SaveToFile(tempPdf, Spire.Doc.FileFormat.PDF);
                                doc.Close();
                            }
                        }
                        catch (Exception)
                        {
                        }
                        if (System.IO.File.Exists(tempPdf))
                        {
                            byte[] thumbnail = CreateThumbnail(request, tempPdf, ref isLandscape);
                            if (thumbnail != null && thumbnail.Length > 0)
                            {
                                DocumentController.SaveThumbnail(fileVersionId, isLandscape, thumbnail);
                            }
                            try
                            {
                                System.IO.File.Delete(tempPdf);
                            }
                            catch
                            {
                            }
                        }
                    }
                    else if ((System.IO.Path.GetExtension(fileName).Equals(".xls", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase)) && Repository.CreateExcel)
                    {
                        string tempPdf = String.Format("{0}{1}_temp.pdf", System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(fileName));
                        try
                        {
                            using (Spire.Xls.Workbook workbook = new Spire.Xls.Workbook())
                            {
                                try
                                {
                                    workbook.LoadFromFile(fileName);
                                    workbook.ConverterSetting.SheetFitToPage = true;
                                    workbook.SaveToFile(tempPdf, Spire.Xls.FileFormat.PDF);
                                }
                                catch(Exception)
                                {
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                        if (System.IO.File.Exists(tempPdf))
                        {
                            byte[] thumbnail = CreateThumbnail(request, tempPdf, ref isLandscape);
                            if (thumbnail != null && thumbnail.Length > 0)
                            {
                                DocumentController.SaveThumbnail(fileVersionId, isLandscape, thumbnail);
                            }
                            try
                            {
                                System.IO.File.Delete(tempPdf);
                            }
                            catch
                            {
                            }
                        }
                    }
                    else if ((System.IO.Path.GetExtension(fileName).Equals(".ppt", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".pptx", StringComparison.OrdinalIgnoreCase)) && Repository.CreatePowerPoint)
                    {
                        string tempPdf = String.Format("{0}{1}_temp.pdf", System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(fileName));
                        try
                        {
                            using (Spire.Presentation.Presentation presentation = new Spire.Presentation.Presentation(fileName, Spire.Presentation.FileFormat.Auto))
                            {
                                presentation.SaveToFile(tempPdf, Spire.Presentation.FileFormat.PDF);
                            }
                        }
                        catch (Exception)
                        {
                        }
                        if (System.IO.File.Exists(tempPdf))
                        {
                            byte[] thumbnail = CreateThumbnail(request, tempPdf, ref isLandscape);
                            if (thumbnail != null && thumbnail.Length > 0)
                            {
                                DocumentController.SaveThumbnail(fileVersionId, isLandscape, thumbnail);
                            }
                            try
                            {
                                System.IO.File.Delete(tempPdf);
                            }
                            catch
                            {
                            }
                        }
                    }
                    else if (System.IO.Path.GetExtension(fileName).Equals(".mp3", StringComparison.OrdinalIgnoreCase) && Repository.CreateAudio)
                    {
                        try
                        {
                            using (Id3.Mp3 mp3 = new Id3.Mp3(fileName))
                            {
                                if (mp3 != null)
                                {
                                    Id3.Id3Tag tag = mp3.GetTag(Id3.Id3TagFamily.Version2X);
                                    if (tag != null)
                                    {
                                        if (tag.Pictures.Count > 0)
                                        {
                                            using (MemoryStream ms = new MemoryStream(tag.Pictures[0].PictureData))
                                            {
                                                using (Bitmap bmp = new Bitmap(ms))
                                                {
                                                    byte[] thumbnail = CreateThumbnail(request, bmp, ref isLandscape, ThumbnailMode.Auto, ThumbnailResize.MaintainAspectRatio);
                                                    if (thumbnail != null && thumbnail.Length > 0)
                                                    {
                                                        DocumentController.SaveThumbnail(fileVersionId, isLandscape, thumbnail);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (((System.IO.Path.GetExtension(fileName).Equals(".mp4", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".mov", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".wmv", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".avi", StringComparison.OrdinalIgnoreCase)) && Repository.CreateVideo) || (System.IO.Path.GetExtension(fileName).Equals(".wma", StringComparison.OrdinalIgnoreCase) && Repository.CreateAudio) || ((System.IO.Path.GetExtension(fileName).Equals(".ppt", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".pptx", StringComparison.OrdinalIgnoreCase)) && Repository.CreatePowerPoint))
                    {
                        using (Microsoft.WindowsAPICodePack.Shell.ShellFile shellFile = Microsoft.WindowsAPICodePack.Shell.ShellFile.FromFilePath(fileName))
                        {
                            Bitmap bmp = new Bitmap(shellFile.Thumbnail.LargeBitmap);
                            bmp.MakeTransparent();
                            byte[] thumbnail = CreateThumbnail(request, bmp, ref isLandscape, ThumbnailMode.Auto, ThumbnailResize.MaintainAspectRatio);
                            if (thumbnail != null && thumbnail.Length > 0)
                            {
                                DocumentController.SaveThumbnail(fileVersionId, isLandscape, thumbnail);
                            }
                        }
                    }
                }
            }
        }

        private byte[] CreateThumbnail(System.Drawing.Bitmap img, ref bool isLandscape, ThumbnailMode thumbnailMode = ThumbnailMode.Auto, ThumbnailResize thumbnailResize = ThumbnailResize.Stretch)
        {
            byte[] byteImage = null;
            string templatePortraitFile = MapPath(string.Format("{0}/Images/pdftemplate_portrait.png", ControlPath));
            string templateLandscapeFile = MapPath(string.Format("{0}/Images/pdftemplate_landscape.png", ControlPath));
            // Size of generated thumbnail in pixels
            int thumbnailWidth = 97;
            int thumbnailHeight = 128;
            string templateFile = templatePortraitFile;
            // Switch between portrait and landscape
            if (img.Width <= img.Height || thumbnailMode == ThumbnailMode.Portrait)
            {
                templateFile = templatePortraitFile;
                isLandscape = false;
            }
            else if (img.Width > img.Height || thumbnailMode == ThumbnailMode.Landscape)
            {
                templateFile = templateLandscapeFile;
                // Swap width and height (little trick not using third temp variable)
                thumbnailWidth ^= thumbnailHeight;
                thumbnailHeight = thumbnailWidth ^ thumbnailHeight;
                thumbnailWidth ^= thumbnailHeight;
                isLandscape = true;
            }
            // Load the template graphic
            using (Bitmap templateBitmap = new Bitmap(templateFile))
            {
                // Create new blank bitmap
                using (Bitmap thumbnailBitmap = new Bitmap(thumbnailWidth, thumbnailHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    // To overlay the template with the image, we need to set the transparency
                    templateBitmap.MakeTransparent(Color.Magenta);
                    using (Graphics thumbnailGraphics = Graphics.FromImage(thumbnailBitmap))
                    {
                        thumbnailGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        thumbnailGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        thumbnailGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        thumbnailGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        // Draw rendered pdf image to new blank bitmap
                        if (thumbnailResize == ThumbnailResize.Stretch)
                        {
                            thumbnailGraphics.DrawImage(img, new Rectangle(2, 2, thumbnailWidth, thumbnailHeight), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
                        }
                        else
                        {
                            thumbnailGraphics.FillRectangle(Brushes.White, 0, 0, thumbnailWidth, thumbnailHeight);
                            double ratioX = (double)(thumbnailWidth - 4) / img.Width;
                            double ratioY = (double)(thumbnailHeight - 4) / img.Height;
                            double ratio = Math.Min(ratioX, ratioY);
                            int newWidth = (int)(img.Width * ratio);
                            int newHeight = (int)(img.Height * ratio);
                            thumbnailGraphics.DrawImage(img, new Rectangle(2 + (((thumbnailWidth - 4) - newWidth) / 2), 2 + (((thumbnailHeight - 4) - newHeight) / 2), newWidth, newHeight), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
                        }
                        // Draw template outline over the bitmap (pdf with show through the transparent area)
                        thumbnailGraphics.DrawImage(templateBitmap, 0, 0);
                        //to save in database
                        byteImage = ImageToByteArray(thumbnailBitmap);
                    }
                }
            }
            return byteImage;
        }

        private byte[] CreateThumbnail(string inputFile, ref bool isLandscape)
        {
            try
            {
                using (GhostscriptRasterizer rasterizer = new GhostscriptRasterizer())
                {
                    const int desiredDpi = 96;
                    rasterizer.CustomSwitches.Add("-dUseCropBox");
                    rasterizer.CustomSwitches.Add("-c");
                    rasterizer.CustomSwitches.Add("[/CropBox [24 72 559 794] /PAGES pdfmark");
                    rasterizer.CustomSwitches.Add("-f");
                    int pageNumber = 1;
                    using (System.IO.FileStream stream = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        try
                        {
                            rasterizer.Open(stream);
                            //rasterizer.Open(inputFile);
                            int pageCount = rasterizer.PageCount;
                            if (pageNumber <= pageCount)
                            {
                                using (Image img = rasterizer.GetPage(desiredDpi, pageNumber))
                                {
                                    if (img != null)
                                    {
                                        using (Bitmap bmp = new Bitmap(img))
                                        {
                                            if (!IsImageBlank(bmp))
                                            {
                                                return CreateThumbnail(bmp, ref isLandscape);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            rasterizer.Close();
                            stream.Close();
                        }
                    }
                    rasterizer.CustomSwitches.Clear();
                    using (System.IO.FileStream stream = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        try
                        {
                            rasterizer.CustomSwitches.Add("-dPDFFitPage");
                            rasterizer.Open(stream);
                            //rasterizer.Open(inputFile);
                            int pageCount = rasterizer.PageCount;
                            if (pageNumber <= pageCount)
                            {
                                using (Image img = rasterizer.GetPage(desiredDpi, pageNumber))
                                {
                                    if (img != null)
                                    {
                                        using (Bitmap bmp = new Bitmap((img.Width > img.Height ? (img.Height * 83) / 109 : img.Width), img.Height, PixelFormat.Format24bppRgb))
                                        {
                                            using (Graphics g = Graphics.FromImage(bmp))
                                            {
                                                g.DrawImage(img, 0, 0);
                                            }
                                            return CreateThumbnail(bmp, ref isLandscape);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            rasterizer.Close();
                            stream.Close();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public void CreateThumbnail(Components.DMSFile file)
        {
            if (Repository.UseThumbnails && !file.FileVersion.HasThumbnail)
            {
                file.FileVersion.LoadThumbnail();
                if (file.FileVersion.Thumbnail == null || file.FileVersion.Thumbnail.Length == 0)
                {
                    bool isLandscape = false;
                    if (file != null && file.FileType.Equals("pdf", StringComparison.OrdinalIgnoreCase) && file.Status.StatusId == 1 && Repository.CreatePDF)
                    {
                        if (file.FileVersion.Contents == null || file.FileVersion.Contents.Length == 0)
                        {
                            file.FileVersion.LoadContents();
                        }
                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                        {
                            string tempPdf = String.Format("{0}{1}_temp.pdf", System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(file.Filename));
                            using (var fs = System.IO.File.Create(tempPdf))
                            {
                                fs.Write(file.FileVersion.Contents, 0, file.FileVersion.Contents.Length);
                                fs.Close();
                            }
                            file.FileVersion.Thumbnail = CreateThumbnail(tempPdf, ref isLandscape);
                            if (file.FileVersion.Thumbnail != null)
                            {
                                file.FileVersion.SaveThumbnail(isLandscape);
                            }
                            if (System.IO.File.Exists(tempPdf))
                            {
                                try
                                {
                                    System.IO.File.Delete(tempPdf);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    else if (file != null && (file.FileType.Equals("png", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("jpg", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("gif", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("bmp", StringComparison.OrdinalIgnoreCase)) && file.Status.StatusId == 1 && Repository.CreateImage)
                    {
                        if (file.FileVersion.Contents == null || file.FileVersion.Contents.Length == 0)
                        {
                            file.FileVersion.LoadContents();
                        }
                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(file.FileVersion.Contents))
                            {
                                using (Bitmap bmp = new Bitmap(ms))
                                {
                                    file.FileVersion.Thumbnail = CreateThumbnail(bmp, ref isLandscape);
                                    if (file.FileVersion.Thumbnail != null)
                                    {
                                        file.FileVersion.SaveThumbnail(isLandscape);
                                    }
                                }
                            }
                        }
                    }
                    else if (file != null && (file.FileType.Equals("doc", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("docx", StringComparison.OrdinalIgnoreCase)) && file.Status.StatusId == 1 && Repository.CreateWord)
                    {
                        if (file.FileVersion.Contents == null || file.FileVersion.Contents.Length == 0)
                        {
                            file.FileVersion.LoadContents();
                        }
                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                        {
                            string tempPdf = String.Format("{0}{1}_temp.pdf", System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(file.Filename));
                            try
                            {
                                using (MemoryStream docStream = new MemoryStream(file.FileVersion.Contents))
                                {
                                    using (Spire.Doc.Document doc = new Spire.Doc.Document(docStream))
                                    {
                                        doc.SaveToFile(tempPdf, Spire.Doc.FileFormat.PDF);
                                        doc.Close();
                                    }
                                    docStream.Close();
                                }
                            }
                            catch (Exception)
                            {
                            }
                            if (System.IO.File.Exists(tempPdf))
                            {
                                file.FileVersion.Thumbnail = CreateThumbnail(tempPdf, ref isLandscape);
                                if (file.FileVersion.Thumbnail != null)
                                {
                                    file.FileVersion.SaveThumbnail(isLandscape);
                                }
                                try
                                {
                                    System.IO.File.Delete(tempPdf);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    else if (file != null && (file.FileType.Equals("xls", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("xlsx", StringComparison.OrdinalIgnoreCase)) && file.Status.StatusId == 1 && Repository.CreateExcel)
                    {
                        if (file.FileVersion.Contents == null || file.FileVersion.Contents.Length == 0)
                        {
                            file.FileVersion.LoadContents();
                        }
                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                        {
                            string tempPdf = String.Format("{0}{1}_temp.pdf", System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(file.Filename));
                            try
                            {
                                using (MemoryStream xlsStream = new MemoryStream(file.FileVersion.Contents))
                                {
                                    using (Spire.Xls.Workbook workbook = new Spire.Xls.Workbook())
                                    {
                                        try
                                        {
                                            //workbook.LoadFromFile(tempExcel);
                                            workbook.LoadFromStream(xlsStream);
                                            workbook.ConverterSetting.SheetFitToPage = true;
                                            workbook.SaveToFile(tempPdf, Spire.Xls.FileFormat.PDF);
                                        }
                                        catch(Exception)
                                        {
                                        }
                                    }
                                    xlsStream.Close();
                                }
                            }
                            catch (Exception)
                            {
                            }
                            if (System.IO.File.Exists(tempPdf))
                            {
                                file.FileVersion.Thumbnail = CreateThumbnail(tempPdf, ref isLandscape);
                                if (file.FileVersion.Thumbnail != null)
                                {
                                    file.FileVersion.SaveThumbnail(isLandscape);
                                }
                                try
                                {
                                    System.IO.File.Delete(tempPdf);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    else if (file != null && (file.FileType.Equals("ppt", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("pptx", StringComparison.OrdinalIgnoreCase)) && file.Status.StatusId == 1 && Repository.CreatePowerPoint)
                    {
                        if (file.FileVersion.Contents == null || file.FileVersion.Contents.Length == 0)
                        {
                            file.FileVersion.LoadContents();
                        }
                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                        {
                            string tempPdf = String.Format("{0}{1}_temp.pdf", System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(file.Filename));
                            try
                            {
                                using (MemoryStream presentationStream = new MemoryStream(file.FileVersion.Contents))
                                {
                                    using (Spire.Presentation.Presentation presentation = new Spire.Presentation.Presentation(presentationStream, Spire.Presentation.FileFormat.Auto))
                                    {
                                        presentation.SaveToFile(tempPdf, Spire.Presentation.FileFormat.PDF);
                                    }
                                    presentationStream.Close();
                                }
                            }
                            catch(Exception)
                            {
                            }
                            if (System.IO.File.Exists(tempPdf))
                            {
                                file.FileVersion.Thumbnail = CreateThumbnail(tempPdf, ref isLandscape);
                                if (file.FileVersion.Thumbnail != null)
                                {
                                    file.FileVersion.SaveThumbnail(isLandscape);
                                }
                                try
                                {
                                    System.IO.File.Delete(tempPdf);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    else if (file != null && file.FileType.Equals("mp3", StringComparison.OrdinalIgnoreCase) && file.Status.StatusId == 1 && Repository.CreateAudio)
                    {
                        if (file.FileVersion.Contents == null || file.FileVersion.Contents.Length == 0)
                        {
                            file.FileVersion.LoadContents();
                        }
                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                        {
                            try
                            {
                                using (MemoryStream mp3Stream = new MemoryStream(file.FileVersion.Contents))
                                {
                                    using (Id3.Mp3 mp3 = new Id3.Mp3(mp3Stream))
                                    {
                                        if (mp3 != null)
                                        {
                                            Id3.Id3Tag tag = mp3.GetTag(Id3.Id3TagFamily.Version2X);
                                            if (tag != null)
                                            {
                                                if (tag.Pictures.Count > 0)
                                                {
                                                    using (MemoryStream ms = new MemoryStream(tag.Pictures[0].PictureData))
                                                    {
                                                        using (Bitmap bmp = new Bitmap(ms))
                                                        {
                                                            file.FileVersion.Thumbnail = CreateThumbnail(bmp, ref isLandscape, ThumbnailMode.Auto, ThumbnailResize.MaintainAspectRatio);
                                                            if (file.FileVersion.Thumbnail != null)
                                                            {
                                                                file.FileVersion.SaveThumbnail(isLandscape);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    else if (file != null && file.Status.StatusId == 1 && (((file.FileType.Equals("mp4", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("mov", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("wmv", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("avi", StringComparison.OrdinalIgnoreCase)) && Repository.CreateVideo) || (file.FileType.Equals("wma", StringComparison.OrdinalIgnoreCase) && Repository.CreateAudio) || ((file.FileType.Equals("ppt", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("pptx", StringComparison.OrdinalIgnoreCase)) && Repository.CreatePowerPoint)))
                    {
                        if (file.FileVersion.Contents == null || file.FileVersion.Contents.Length == 0)
                        {
                            file.FileVersion.LoadContents();
                        }
                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                        {
                            string tempFile = String.Format("{0}{1}_temp{2}", System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(file.Filename), System.IO.Path.GetExtension(file.Filename));
                            using (var fs = System.IO.File.Create(tempFile))
                            {
                                fs.Write(file.FileVersion.Contents, 0, file.FileVersion.Contents.Length);
                                fs.Close();
                            }
                            using (Microsoft.WindowsAPICodePack.Shell.ShellFile shellFile = Microsoft.WindowsAPICodePack.Shell.ShellFile.FromFilePath(tempFile))
                            {
                                Bitmap bmp = new Bitmap(shellFile.Thumbnail.LargeBitmap);
                                bmp.MakeTransparent();
                                file.FileVersion.Thumbnail = CreateThumbnail(bmp, ref isLandscape, ThumbnailMode.Auto, ThumbnailResize.MaintainAspectRatio);
                                if (file.FileVersion.Thumbnail != null)
                                {
                                    file.FileVersion.SaveThumbnail(isLandscape);
                                }
                                if (System.IO.File.Exists(tempFile))
                                {
                                    try
                                    {
                                        System.IO.File.Delete(tempFile);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
                file.FileVersion.Contents = null;
                file.FileVersion.Thumbnail = null;
                GC.Collect();
            }
        }

        private Bitmap ResizeImage(System.Drawing.Image image, int width, int height, bool fast = false)
        {
            Rectangle destRect = new Rectangle(0, 0, width, height);
            Bitmap destImage = null;
            if (fast)
            {
                destImage = new Bitmap(width, height, PixelFormat.Format32bppRgb);
                using (Graphics graphics = Graphics.FromImage(destImage))
                {
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
                }
            }
            else
            {
                destImage = new Bitmap(width, height, PixelFormat.Format32bppRgb);
                destImage.MakeTransparent();
                using (Graphics graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }
            }
            return destImage;
        }

        private byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PixelData
        {
            public byte B;
            public byte G;
            public byte R;
            public byte A;
        }

        private bool IsImageBlank(Bitmap bitmap)
        {
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bool bIsWhite = true;
            unsafe
            {
                PixelData* pPixel = (PixelData*)bitmapData.Scan0;
                for (int i = 0; i < bitmapData.Height && bIsWhite; i++)
                {
                    for (int j = 0; j < bitmapData.Width; j++)
                    {
                        if (pPixel->B != 255 || pPixel->G != 255 || pPixel->R != 255)
                        {
                            bIsWhite = false;
                            break;
                        }
                        pPixel++;
                    }
                    pPixel += bitmapData.Stride - (bitmapData.Width * 4);
                }
            }
            bitmap.UnlockBits(bitmapData);
            return bIsWhite;
        }

        private string MapPath(string virtualPath)
        {
            string path = (HttpContext.Current != null && HttpContext.Current.Request != null ? HttpContext.Current.Request.MapPath(virtualPath) : virtualPath.Replace("/", "\\"));
            if (path.Contains("~"))
            {
                path = path.Replace("~", Portal.HomeDirectoryMapPath.Substring(0, Portal.HomeDirectoryMapPath.IndexOf("\\Portals"))).Replace("\\\\", "\\");
            }
            else if (!path.Contains(":\\"))
            {
                path = (Portal.HomeDirectoryMapPath.Substring(0, Portal.HomeDirectoryMapPath.IndexOf("\\Portals")) + path).Replace("\\\\", "\\");
            }
            return path;
        }
    }
}