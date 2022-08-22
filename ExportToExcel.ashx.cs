using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace Gafware.Modules.DMS
{
    /// <summary>
    /// Summary description for ExportToExcel
    /// </summary>
    public class ExportToExcel : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            context.Response.AppendHeader("pragma", "no-cache");
            context.Response.AppendHeader("expires", "0");
            context.Response.AppendHeader("cache-control", "no-cache, no-store, must-revalidate, max-age=0");
            context.Response.AppendHeader("content-control", "no-cache, no-store, must-revalidate, max-age=0");
            context.Response.AppendHeader("content-disposition", "inline;filename=\"DocumentList.xlsx\";creation-date=\"" + DateTimeParser.ToRFC822(DateTime.Now) + "\"");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Worksheets.Add("DocumentList");
                if (context.Session["gv"] != null)
                {
                    string[] ignore = new string[] { "PortalId", "TabModuleId" };
                    System.Data.DataView view = (System.Data.DataView)context.Session["gv"];
                    Components.DMSPortalSettings portalSettings = null;
                    foreach (System.Data.DataRowView row in view)
                    {
                        int portalId = (int)row["PortalId"];
                        int tabModuleId = (int)row["TabModuleId"];
                        if (portalSettings == null)
                        {
                            portalSettings = Components.DocumentController.GetPortalSettings(portalId);
                        }
                        List<Components.DocumentCategory> docCategories = Components.DocumentController.GetAllCategoriesForDocument((int)row["DocumentId"]);
                        List<Components.Category> categories = Components.DocumentController.GetAllCategories(portalId, portalSettings.PortalWideRepository ? 0 : tabModuleId);
                        foreach (Components.Category category in categories)
                        {
                            row[Generic.RemoveSpecialCharacters(category.CategoryName).Replace(" ", "_")] = (docCategories.FirstOrDefault(docCat => category.CategoryId == docCat.CategoryId) != null ? true : false);
                        }
                    }
                    Excel.ExportToExcel(view, excel, "DocumentList", ignore);
                }
                excel.SaveAs(context.Response.OutputStream);
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