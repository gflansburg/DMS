using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Gafware.Modules.DMS.Components;
using Ghostscript.NET.Processor;
using Ghostscript.NET.Rasterizer;
using Microsoft.ApplicationBlocks.Data;

namespace Gafware.Modules.DMS
{
    public class Generic
    {
        private static string _connectionString = String.Empty;
        private static string _documentPath = String.Empty;
        private static string _physicalPath = String.Empty;

        public static bool UserHasAccess(Components.Document doc)
        {
            if (!doc.UseCategorySecurityRoles)
            {
                DotNetNuke.Security.Roles.RoleInfo securityRole = Components.UserController.GetRoleById(doc.PortalId, doc.SecurityRoleId);
                if (securityRole != null)
                {
                    if (DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo().IsInRole(securityRole.RoleName))
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (Components.DocumentCategory category in doc.Categories)
                {
                    DotNetNuke.Security.Roles.RoleInfo categoryRole = Components.UserController.GetRoleById(doc.PortalId, category.Category.RoleId);
                    if (categoryRole != null)
                    {
                        if (DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo().IsInRole(categoryRole.RoleName))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static string StringToHex(string str)
        {
            byte[] ba = System.Text.Encoding.ASCII.GetBytes(str);
            var hexString = BitConverter.ToString(ba);
            hexString = hexString.Replace("-", "");
            return hexString;
        }

        public static DotNetNuke.Entities.Users.UserInfo FindUserInfoByUserId(int portalId, int userId)
        {
            DotNetNuke.Entities.Users.UserInfo userInfo = DotNetNuke.Entities.Users.UserController.GetUserById(portalId, userId);
            return userInfo;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static bool IsBoolean(object o)
        {
            try
            {
                ToBoolean(o);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool ToBoolean(object o)
        {
            if (o != null)
            {
                if (o.GetType() == typeof(string))
                {
                    string s = o.ToString();
                    if (!String.IsNullOrEmpty(s))
                    {
                        if (s.ToLower().Equals("no") || s.ToLower().Equals("false") || s.ToLower().Equals("0") || s.ToLower().Equals("off"))
                        {
                            return false;
                        }
                        else if (s.ToLower().Equals("yes") || s.ToLower().Equals("true") || s.ToLower().Equals("1") || s.ToLower().Equals("on"))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (o == null)
                {
                    return false;
                }
                return Convert.ToBoolean(o);
            }
            return false;
        }

        static public bool IsNumber(string str)
        {
            bool b = true;
            try
            {
                Convert.ToInt32(str);
            }
            catch
            {
                b = false;
            }
            return b;
        }

        public static int GetAscVal(string val, int high)
        {
            int getVal = 0;
            foreach (char c in val)
            {
                getVal += c;
            }
            getVal = getVal % high;
            return getVal;
        }

        public static string GenerateRandString(int len, int seed)
        {
            int max = GenerateRandNum(seed, 35);
            int min = 0;
            string strRandom = String.Empty;
            Random rand = new Random(seed);

            for (int i = 1; i <= len; i++)
            {
                max = ((max + len) * 29 + seed) % 35;
                int num = Convert.ToInt32(((max - min + 1) * rand.NextDouble() + min));
                if (num < 10)
                {
                    strRandom = strRandom + Convert.ToChar(num + 48);
                }
                else
                {
                    strRandom = strRandom + Convert.ToChar((num - 10) + 65);
                }
                seed = num;
            }
            return Convert.ToString(strRandom);
        }

        private static int GenerateRandNum(int seed, int high)
        {
            DateTime currentTime = DateTime.Now;
            int randNum = Convert.ToInt32((((currentTime.Second) * (currentTime.Hour) * (currentTime.Minute)) + (currentTime.Day) + seed) % high);
            return randNum;
        }

        public static DateTime GetDefaultDate()
        {
            return new DateTime(1900, 1, 1);
        }

        /// <summary>
        /// Returns a site relative HTTP path from a partial path starting out with a ~.
        /// Same syntax that ASP.Net internally supports but this method can be used
        /// outside of the Page framework.
        /// 
        /// Works like Control.ResolveUrl including support for ~ syntax
        /// but returns an absolute URL.
        /// </summary>
        /// <param name="originalUrl">Any Url including those starting with ~</param>
        /// <returns>relative url</returns>
        public static string ResolveUrl(string originalUrl)
        {
            if (originalUrl == null)
            {
                return null;
            }
            // *** Absolute path - just return
            if (originalUrl.IndexOf("://") != -1)
            {
                return originalUrl;
            }
            // *** Fix up image path for ~ root app dir directory
            if (originalUrl.StartsWith("~"))
            {
                string newUrl = "";
                if (HttpContext.Current != null)
                {
                    newUrl = HttpContext.Current.Request.ApplicationPath + originalUrl.Substring(1).Replace("//", "/");
                }
                else
                {
                    // *** Not context: assume current directory is the base directory
                    throw new ArgumentException("Invalid URL: Relative URL not allowed.");
                }
                // *** Just to be sure fix up any double slashes
                return newUrl;
            }
            return originalUrl;
        }

        public static string CreateSafeFolderName(string folder)
        {
            string newFolder = String.Empty;
            for (int i = 0; i < folder.Length; i++)
            {
                char c = folder[i];
                if (c == 32)
                {
                    // space
                    newFolder += '_';
                }
                else if ((c >= 48 && c <= 57) || (c >= 65 && c <= 90) || (c >= 97 && c <= 122) || c == 45 || c == 95)
                {
                    // safe
                    newFolder += c;
                }
                else
                {
                    // unsafe
                }
            }
            return newFolder;
        }

        static public String JSEncode(String str)
        {
            String retStr = str;

            retStr = retStr.Replace("\\'", "&##39;");
            retStr = retStr.Replace("\\\"", "\"");
            retStr = retStr.Replace("\\\\", "\\");
            retStr = retStr.Replace("&##39;", "\'");
            retStr = retStr.Replace("&quot;", "\"");
            retStr = retStr.Replace("\\r", "\r");
            retStr = retStr.Replace("\\n", "\n");
            retStr = retStr.Replace("\\t", "\t");

            retStr = retStr.Replace("\\", "\\\\");
            retStr = retStr.Replace("\t", "\\t");
            retStr = retStr.Replace("\r\n", "\\n");
            retStr = retStr.Replace("\'", "\\'");
            retStr = retStr.Replace("\"", "\\x22");
            retStr = retStr.Replace("\r", "\\r");
            retStr = retStr.Replace("\n", "\\n");
            return retStr;
        }

        static public string StripHTML(string source)
        {
            try
            {

                string result;

                // Remove HTML Development formatting
                result = source.Replace("\r", " ");        // Replace line breaks with space because browsers inserts space
                result = result.Replace("\n", " ");        // Replace line breaks with space because browsers inserts space
                result = result.Replace("\t", string.Empty);    // Remove step-formatting
                result = System.Text.RegularExpressions.Regex.Replace(result, @"( )+", " ");    // Remove repeating speces becuase browsers ignore them

                // Remove the header (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*head([^>])*>", "<head>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, @"(<( )*(/)( )*head( )*>)", "</head>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, "(<head>).*(</head>)", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all scripts (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*script([^>])*>", "<script>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, @"(<( )*(/)( )*script( )*>)", "</script>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result, @"(<script>)([^(<script>\.</script>)])*(</script>)",string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, @"(<script>).*(</script>)", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all styles (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*style([^>])*>", "<style>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, @"(<( )*(/)( )*style( )*>)", "</style>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, "(<style>).*(</style>)", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert tabs in spaces of <td> tags
                result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*td([^>])*>", "\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line breaks in places of <BR> and <LI> tags
                result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*br( )*>", "\r\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*li( )*>", "\r\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = result.Replace("<br />", "\r\n");
                result = result.Replace("<BR />", "\r\n");
                result = result.Replace("<br/>", "\r\n");
                result = result.Replace("<BR/>", "\r\n");

                // insert line paragraphs (double line breaks) in place if <P>, <DIV> and <TR> tags
                result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*div([^>])*>", "\r\n\r\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*tr([^>])*>", "\r\n\r\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, @"<( )*p([^>])*>", "\r\n\r\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // Remove remaining tags like <a>, links, images, comments etc - anything thats enclosed inside < >
                result = System.Text.RegularExpressions.Regex.Replace(result, @"<[^>]*>", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // replace special characters:
                result = System.Text.RegularExpressions.Regex.Replace(result, @"&nbsp;", " ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result, @"&bull;", " * ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, @"&lsaquo;", "<", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, @"&rsaquo;", ">", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, @"&trade;", "(tm)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, @"&frasl;", "/", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, @"&lt;", "<", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, @"&gt;", ">", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, @"&quot;", "\"", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, @"&copy;", "(c)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, @"&reg;", "(r)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove all others. More can be added, see http://hotwired.lycos.com/webmonkey/reference/special_characters/
                result = System.Text.RegularExpressions.Regex.Replace(result, @"&(.{2,6});", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // for testng
                //System.Text.RegularExpressions.Regex.Replace(result, this.txtRegex.Text,string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // make line breaking consistent
                result = result.Replace("\n", "\r");

                // Remove extra line breaks and tabs: replace over 2 breaks with 2 and over 4 tabs with 4. 
                // Prepare first to remove any whitespaces inbetween the escaped characters and remove redundant tabs inbetween linebreaks
                result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)( )+(\r)", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, "(\t)( )+(\t)", "\t\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, "(\t)( )+(\r)", "\t\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)( )+(\t)", "\r\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)(\t)+(\r)", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase);    // Remove redundant tabs
                result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)(\t)+", "\r\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase);        // Remove multible tabs followind a linebreak with just one tab
                string breaks = "\r\r\r";        // Initial replacement target string for linebreaks
                string tabs = "\t\t\t\t\t";        // Initial replacement target string for tabs
                for (int index = 0; index < result.Length; index++)
                {
                    result = result.Replace(breaks, "\r\r");
                    result = result.Replace(tabs, "\t\t\t\t");
                    breaks = breaks + "\r";
                    tabs = tabs + "\t";
                }

                // Thats it.
                return result;

            }
            catch
            {
            }
            return String.Empty;
        }

        public static void ApplyPaging(System.Web.UI.WebControls.GridView gv)
        {
            System.Web.UI.WebControls.GridViewRow row = gv.BottomPagerRow;
            if (row != null)
            {
                System.Web.UI.WebControls.PlaceHolder ph;
                System.Web.UI.WebControls.LinkButton lnkPaging;
                System.Web.UI.WebControls.LinkButton lnkFirstPage;
                System.Web.UI.WebControls.LinkButton lnkPrevPage;
                System.Web.UI.WebControls.LinkButton lnkNextPage;
                System.Web.UI.WebControls.LinkButton lnkLastPage;
                System.Web.UI.WebControls.Label lblPaging;
                System.Web.UI.WebControls.Label lblFirstPage;
                System.Web.UI.WebControls.Label lblPrevPage;
                System.Web.UI.WebControls.Label lblNextPage;
                System.Web.UI.WebControls.Label lblLastPage;
                lblFirstPage = new System.Web.UI.WebControls.Label();
                lblFirstPage.Text = HttpUtility.HtmlEncode("<<"); ;
                lnkFirstPage = new System.Web.UI.WebControls.LinkButton();
                lnkFirstPage.Controls.Add(lblFirstPage);
                lnkFirstPage.Style.Add("min-width", "20px");
                lnkFirstPage.CommandName = "Page";
                lnkFirstPage.CommandArgument = "first";
                lnkFirstPage.CssClass = "LinkPaging";
                lnkFirstPage.Enabled = gv.PageIndex > 0;
                lblPrevPage = new System.Web.UI.WebControls.Label();
                lblPrevPage.Text = HttpUtility.HtmlEncode("<"); ;
                lnkPrevPage = new System.Web.UI.WebControls.LinkButton();
                lnkPrevPage.Controls.Add(lblPrevPage);
                lnkPrevPage.Style.Add("min-width", "20px");
                lnkPrevPage.CommandName = "Page";
                lnkPrevPage.CommandArgument = "prev";
                lnkPrevPage.CssClass = "LinkPaging";
                lnkPrevPage.Enabled = gv.PageIndex > 0;
                ph = (System.Web.UI.WebControls.PlaceHolder)row.FindControl("ph");
                ph.Controls.Add(lnkFirstPage);
                ph.Controls.Add(lnkPrevPage);
                if (gv.PageIndex == 0)
                {
                    lnkFirstPage.Enabled = false;
                    lnkPrevPage.Enabled = false;
                }
                int startIndex = 1;
                int endIndex = gv.PageCount;
                if (gv.PageCount > gv.PagerSettings.PageButtonCount)
                {
                    startIndex = Math.Max(1, (gv.PageIndex + 1) - (gv.PagerSettings.PageButtonCount / 2));
                    endIndex = Math.Min(startIndex + gv.PagerSettings.PageButtonCount - 1, gv.PageCount);
                }
                for (int i = startIndex; i <= endIndex; i++)
                {
                    lblPaging = new System.Web.UI.WebControls.Label();
                    lblPaging.Text = i.ToString();
                    lnkPaging = new System.Web.UI.WebControls.LinkButton();
                    lnkPaging.Style.Add("min-width", "20px");
                    lnkPaging.CssClass = "LinkPaging" + (i == gv.PageIndex + 1 ? "Selected" : String.Empty);
                    lnkPaging.Controls.Add(lblPaging);
                    lnkPaging.CommandName = "Page";
                    lnkPaging.CommandArgument = i.ToString();
                    if (i == gv.PageIndex + 1)
                    {
                        lnkPaging.BackColor = System.Drawing.Color.Pink;
                    }
                    ph = (System.Web.UI.WebControls.PlaceHolder)row.FindControl("ph");
                    ph.Controls.Add(lnkPaging);
                }
                lblNextPage = new System.Web.UI.WebControls.Label();
                lblNextPage.Text = HttpUtility.HtmlEncode(">"); ;
                lnkNextPage = new System.Web.UI.WebControls.LinkButton();
                lnkNextPage.Controls.Add(lblNextPage);
                lnkNextPage.Style.Add("min-width", "20px");
                lnkNextPage.CommandName = "Page";
                lnkNextPage.CommandArgument = "next";
                lnkNextPage.CssClass = "LinkPaging";
                lnkNextPage.Enabled = gv.PageIndex < gv.PageCount - 1;
                lblLastPage = new System.Web.UI.WebControls.Label();
                lblLastPage.Text = HttpUtility.HtmlEncode(">>"); ;
                lnkLastPage = new System.Web.UI.WebControls.LinkButton();
                lnkLastPage.Controls.Add(lblLastPage);
                lnkLastPage.Style.Add("min-width", "20px");
                lnkLastPage.CommandName = "Page";
                lnkLastPage.CommandArgument = "last";
                lnkLastPage.CssClass = "LinkPaging";
                lnkLastPage.Enabled = gv.PageIndex < gv.PageCount - 1;
                ph = (System.Web.UI.WebControls.PlaceHolder)row.FindControl("ph");
                ph.Controls.Add(lnkNextPage);
                ph = (System.Web.UI.WebControls.PlaceHolder)row.FindControl("ph");
                ph.Controls.Add(lnkLastPage);
                if (gv.PageIndex == gv.PageCount - 1)
                {
                    lnkNextPage.Enabled = false;
                    lnkLastPage.Enabled = false;
                }
            }
        }

        public static string GetRandomKeyNoDuplication(int portalId, int tabModuleId)
        {
            bool matchFound = true;
            string rkey = GenerateRandString(13, GetAscVal("Gafware_DMS_Packet", 101));

            while (matchFound)
            {
                Components.Packet packet = Components.PacketController.GetPacketByName(rkey, portalId, tabModuleId);
                if (packet == null)
                {
                    matchFound = false;
                    break;
                }
                rkey = GenerateRandString(13, GetAscVal(rkey, 101));
            }
            return rkey;
        }

        static public string UrlEncode(string text)
        {
            string outText = "";
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if ((c >= 48 && c <= 57) || (c >= 65 && c <= 90) || (c >= 97 && c <= 122) || c == 45 || c == 46 || c == 61)
                {
                    // safe
                    outText += c;
                }
                else
                {
                    // unsafe
                    outText += "%" + Convert.ToInt32(c).ToString("X");
                }
            }
            return outText;
        }

        public class FontSizeConverter
        {
            public static int ToPixels(System.Web.UI.WebControls.FontUnit unitSize)
            {
                int result = 16;

                if (unitSize.Type != System.Web.UI.WebControls.FontSize.NotSet && unitSize.Type != System.Web.UI.WebControls.FontSize.AsUnit)
                {
                    // size in x-small, etc
                    switch (unitSize.Type)
                    {
                        case System.Web.UI.WebControls.FontSize.XXSmall:
                            result = 9;
                            break;
                        case System.Web.UI.WebControls.FontSize.XSmall:
                            result = 10;
                            break;
                        case System.Web.UI.WebControls.FontSize.Smaller:
                        case System.Web.UI.WebControls.FontSize.Small:
                            result = 13;
                            break;
                        case System.Web.UI.WebControls.FontSize.Medium:
                            result = 16;
                            break;
                        case System.Web.UI.WebControls.FontSize.Large:
                        case System.Web.UI.WebControls.FontSize.Larger:
                            result = 18;
                            break;
                        case System.Web.UI.WebControls.FontSize.XLarge:
                            result = 24;
                            break;
                        case System.Web.UI.WebControls.FontSize.XXLarge:
                            result = 32;
                            break;
                        default:
                            result = 16;
                            break;
                    }
                }
                else if (unitSize.Type == System.Web.UI.WebControls.FontSize.AsUnit)
                {
                    switch (unitSize.Unit.Type)
                    {
                        case System.Web.UI.WebControls.UnitType.Pixel:
                            result = (int)Math.Round(unitSize.Unit.Value);
                            break;
                        case System.Web.UI.WebControls.UnitType.Point:
                            result = (int)Math.Round(unitSize.Unit.Value * 1.33);
                            break;
                        case System.Web.UI.WebControls.UnitType.Em:
                            result = (int)Math.Round(unitSize.Unit.Value * 16);
                            break;
                        case System.Web.UI.WebControls.UnitType.Percentage:
                            result = (int)Math.Round(unitSize.Unit.Value * 16 / 100);
                            break;
                        default:
                            // other types are not supported. just return the medium
                            result = 16;
                            break;
                    }
                }

                return result;
            }
        }

        public static System.Drawing.FontStyle GetFontStyle(System.Web.UI.WebControls.FontInfo fontInfo)
        {
            System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;
            if (fontInfo.Bold)
            {
                style |= System.Drawing.FontStyle.Bold;
            }
            if (fontInfo.Italic)
            {
                style |= System.Drawing.FontStyle.Italic;
            }
            if (fontInfo.Strikeout)
            {
                style |= System.Drawing.FontStyle.Strikeout;
            }
            if (fontInfo.Underline)
            {
                style |= System.Drawing.FontStyle.Underline;
            }
            return style;
        }

        public static float GetWidthOfString(string str)
        {
            return GetWidthOfString(str, new System.Drawing.Font("Arial", 12));
        }

        public static float GetWidthOfString(string str, System.Web.UI.WebControls.FontInfo fontInfo)
        {
            return GetWidthOfString(str, new System.Drawing.Font(fontInfo.Name, FontSizeConverter.ToPixels(fontInfo.Size), GetFontStyle(fontInfo)));
        }

        public static float GetWidthOfString(string str, System.Drawing.Font font)
        {
            System.Drawing.Bitmap objBitmap = new System.Drawing.Bitmap(500, 200);
            System.Drawing.Graphics objGraphics = System.Drawing.Graphics.FromImage(objBitmap);
            System.Drawing.SizeF stringSize = objGraphics.MeasureString(str, font);
            objBitmap.Dispose();
            objGraphics.Dispose();
            return stringSize.Width;
        }

        private static void AddSubColumns(string prefix, System.ComponentModel.PropertyDescriptor property, System.Data.DataTable table)
        {
            System.ComponentModel.PropertyDescriptorCollection props = System.ComponentModel.TypeDescriptor.GetProperties(property.PropertyType);
            for (int i = 0; i < props.Count; i++)
            {
                System.ComponentModel.PropertyDescriptor prop = props[i];
                if (prop.PropertyType.IsPrimitive || prop.PropertyType.Equals(typeof(String)) || (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType).Equals(typeof(DateTime)))
                {
                    table.Columns.Add(prefix + "_" + prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
                else if (prop.PropertyType.IsClass)
                {
                    table.Columns.Add(prefix + "_" + prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    AddSubColumns(prefix + "_" + prop.Name, prop, table);
                }
            }
        }

        private static void SetSubValue(string prefix, System.ComponentModel.PropertyDescriptor property, System.Data.DataRow row, object item)
        {
            System.ComponentModel.PropertyDescriptorCollection props = System.ComponentModel.TypeDescriptor.GetProperties(property.PropertyType);
            for (int i = 0; i < props.Count; i++)
            {
                System.ComponentModel.PropertyDescriptor prop = props[i];
                if (prop.PropertyType.IsPrimitive || prop.PropertyType.Equals(typeof(String)) || (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType).Equals(typeof(DateTime)))
                {
                    row[prefix + "_" + prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                else if (prop.PropertyType.IsClass)
                {
                    row[prefix + "_" + prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    SetSubValue(prefix + "_" + prop.Name, prop, row, prop.GetValue(item));
                }
            }
        }

        public static System.Data.DataTable ListToDataTable<T>(System.Collections.Generic.IList<T> list)
        {
            System.ComponentModel.PropertyDescriptorCollection props = System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            System.Data.DataTable table = new System.Data.DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                System.ComponentModel.PropertyDescriptor prop = props[i];
                if (prop.PropertyType.IsPrimitive || prop.PropertyType.BaseType.Equals(typeof(System.Enum)) || prop.PropertyType.Equals(typeof(decimal)) || prop.PropertyType.Equals(typeof(String)) || (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType).Equals(typeof(DateTime)))
                {
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
                /*else if (prop.PropertyType.IsClass)
                {
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    AddSubColumns(prop.Name, prop, table);
                }*/
            }
            foreach (T item in list)
            {
                System.Data.DataRow row = table.NewRow();
                for (int i = 0; i < props.Count; i++)
                {
                    System.ComponentModel.PropertyDescriptor prop = props[i];
                    if (prop.PropertyType.IsPrimitive || prop.PropertyType.BaseType.Equals(typeof(System.Enum)) || prop.PropertyType.Equals(typeof(decimal)) || prop.PropertyType.Equals(typeof(String)) || (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType).Equals(typeof(DateTime)))
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }
                    /*else if (prop.PropertyType.IsClass)
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                        SetSubValue(prop.Name, prop, row, prop.GetValue(item));
                    }*/
                }
                table.Rows.Add(row);
            }
            return table;
        }

        public static string RemoveSpecialCharacters(string input)
        {
            Regex r = new Regex("(?:[^a-zA-Z0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return r.Replace(input, String.Empty);
        }

        public static System.Data.DataTable DocumentListToDataTable<T>(System.Collections.Generic.IList<T> list, int portalId, int tabModuleId)
        {
            System.ComponentModel.PropertyDescriptorCollection props = System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            System.Data.DataTable table = new System.Data.DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                System.ComponentModel.PropertyDescriptor prop = props[i];
                if (prop.PropertyType.IsPrimitive || prop.PropertyType.BaseType.Equals(typeof(System.Enum)) || prop.PropertyType.Equals(typeof(decimal)) || prop.PropertyType.Equals(typeof(String)) || (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType).Equals(typeof(DateTime)))
                {
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
                /*else if (prop.PropertyType.IsClass)
                {
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    AddSubColumns(prop.Name, prop, table);
                }*/
            }
            List<Components.Category> categories = Components.DocumentController.GetAllCategories(portalId, tabModuleId);
            foreach (Components.Category category in categories)
            {
                table.Columns.Add(RemoveSpecialCharacters(category.CategoryName).Replace(" ", "_"), typeof(string));
            }
            foreach (T item in list)
            {
                System.Data.DataRow row = table.NewRow();
                for (int i = 0; i < props.Count; i++)
                {
                    System.ComponentModel.PropertyDescriptor prop = props[i];
                    if (prop.PropertyType.IsPrimitive || prop.PropertyType.BaseType.Equals(typeof(System.Enum)) || prop.PropertyType.Equals(typeof(decimal)) || prop.PropertyType.Equals(typeof(String)) || (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType).Equals(typeof(DateTime)))
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }
                    /*else if (prop.PropertyType.IsClass)
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                        SetSubValue(prop.Name, prop, row, prop.GetValue(item));
                    }*/
                    if (prop.Name == "CategoriesRaw")
                    {
                        List<Components.Category> categoriesRaw = (List<Components.Category>)prop.GetValue(item);
                        foreach (Components.Category category in categoriesRaw)
                        {
                            row[Generic.RemoveSpecialCharacters(category.CategoryName).Replace(" ", "_")] = "Yes";
                        }
                    }
                }
                table.Rows.Add(row);
            }
            return table;
        }

        public static long GetFileSize(string filename)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(filename);
            return fi.Length;
        }

        public static string HTMLTrim(string str)
        {
            string s = str;
            s = s.Trim();
            while (s.StartsWith("\r\n"))
            {
                s = s.Substring(2);
            }
            while (s.EndsWith("\r\n"))
            {
                s = s.Substring(0, s.Length - 2);
            }
            while (s.ToLower().StartsWith("&nbsp;"))
            {
                s = s.Substring(6);
            }
            while (s.ToLower().EndsWith("&nbsp;"))
            {
                s = s.Substring(0, s.Length - 6);
            }
            while (s.StartsWith("\r\n"))
            {
                s = s.Substring(2);
            }
            while (s.EndsWith("\r\n"))
            {
                s = s.Substring(0, s.Length - 2);
            }
            return s;
        }

        public static string ToggleButtonCssString()
        {
            return ToggleButtonCssString("No", "Yes", new Unit("100px"), String.Empty, System.Drawing.Color.Blue);
        }

        public static string ToggleButtonCssString(System.Drawing.Color backColor)
        {
            return ToggleButtonCssString("No", "Yes", new Unit("100px"), String.Empty, backColor);
        }

        public static string ToggleButtonCssString(string offText, string onText, Unit width, System.Drawing.Color backColor)
        {
            return ToggleButtonCssString(offText, onText, width, String.Empty, backColor);
        }

        public static string ToggleButtonCssString(string offText, string onText, Unit width, string scope, System.Drawing.Color backColor)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span::after,");
            sb.AppendLine(".toggleButton input[type=\"radio\"] + span::after");
            sb.AppendLine("{");
            sb.AppendLine("    content: \"" + offText + "\";");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"]:checked + span::after,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"]:checked + span::after");
            sb.AppendLine("{");
            sb.AppendLine("    content: \"" + onText + "\";");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton");
            sb.AppendLine("{");
            sb.AppendLine("    -moz-user-select: none;");
            sb.AppendLine("    -webkit-user-select: none;");
            sb.AppendLine("    user-select: none;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton");
            sb.AppendLine("{");
            //sb.AppendLine("    margin: 4px 0;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton label,");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"] + span,");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span::before,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"] + span::before,");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span::after,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"] + span::after");
            sb.AppendLine("{");
            sb.AppendLine("    display: inline-block;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton label");
            //sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span,");
            //sb.AppendLine(scope + " .toggleButton input[type=\"radio\"] + span");
            sb.AppendLine("{");
            sb.AppendLine("    vertical-align: middle;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"] + span,");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span::before,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"] + span::before,");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span::after,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"] + span::after");
            sb.AppendLine("{");
            sb.AppendLine("    -moz-border-radius: 4px;");
            sb.AppendLine("    -webkit-border-radius: 4px;");
            sb.AppendLine("    border-radius: 4px;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span::before,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"] + span::before,");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span::after,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"] + span::after,");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"]:checked + span::before,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"]:checked + span::before,");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"]:checked + span::after,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"]:checked + span::after");
            sb.AppendLine("{");
            sb.AppendLine("    top: 0;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span::after,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"] + span::after,");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"]:checked + span::before,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"]:checked + span::before");
            sb.AppendLine("{");
            sb.AppendLine("    right: 0;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span::before,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"] + span::before,");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"]:checked + span::after,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"]:checked + span::after");
            sb.AppendLine("{");
            sb.AppendLine("    left: 0;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"],");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"],");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span::before,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"] + span::before,");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span::after,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"] + span::after");
            sb.AppendLine("{");
            sb.AppendLine("    position: absolute;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton label");
            sb.AppendLine("{");
            sb.AppendLine("    margin: 2px 0 2px 4px;");
            sb.AppendLine("    cursor: pointer;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"],");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"]");
            sb.AppendLine("{");
            sb.AppendLine("    filter: alpha(opacity=0);");
            sb.AppendLine("    -moz-opacity: 0;");
            sb.AppendLine("    -webkit-opacity: 0;");
            sb.AppendLine("    opacity: 0;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"] + span");
            sb.AppendLine("{");
            sb.AppendLine("    width: " + width.ToString() + ";");
            sb.AppendLine("    height: 30px;");
            sb.AppendLine("    font: bold 14px/30px Arial, Sans-serif;");
            sb.AppendLine("    color: #8c8c8c;");
            sb.AppendLine("    text-transform: uppercase;");
            sb.AppendLine("    border: solid 1px #bcbbbb;");
            sb.AppendLine("    filter: progid:DXImageTransform.Microsoft.gradient(startColorstr=\"#c8c8c8\", endColorstr=\"#f3f3f3\");");
            sb.AppendLine("    background: -moz-linear-gradient(top, #c8c8c8, #f3f3f3);");
            sb.AppendLine("    background: -webkit-linear-gradient(top, #c8c8c8, #f3f3f3);");
            sb.AppendLine("    background: -o-linear-gradient(top, #c8c8c8, #f3f3f3);");
            sb.AppendLine("    background: -ms-linear-gradient(top, #c8c8c8, #f3f3f3);");
            sb.AppendLine("    background: linear-gradient(top, #c8c8c8, #f3f3f3);");
            sb.AppendLine("    position: relative;");
            sb.AppendLine("    text-indent: -9999px;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span::before,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"] + span::before");
            sb.AppendLine("{");
            sb.AppendLine("    content: \"\";");
            sb.AppendLine("    width: 40%;");
            sb.AppendLine("    height: 29px;");
            sb.AppendLine("    border-top: solid 1px #fff;");
            sb.AppendLine("    border-right: solid 1px #bebebe;");
            sb.AppendLine("    filter: progid:DXImageTransform.Microsoft.gradient(startColorstr=\"#cfcfcf\", endColorstr=\"#f9f9f9\");");
            sb.AppendLine("    background: -moz-linear-gradient(top, #cfcfcf, #f9f9f9);");
            sb.AppendLine("    background: -webkit-linear-gradient(top, #cfcfcf, #f9f9f9);");
            sb.AppendLine("    background: -o-linear-gradient(top, #cfcfcf, #f9f9f9);");
            sb.AppendLine("    background: -ms-linear-gradient(top, #cfcfcf, #f9f9f9);");
            sb.AppendLine("    background: linear-gradient(top, #cfcfcf, #f9f9f9);");
            sb.AppendLine("    -moz-box-shadow: 1px 0 1px #bebebe;");
            sb.AppendLine("    -webkit-box-shadow: 1px 0 1px #bebebe;");
            sb.AppendLine("    box-shadow: 1px 0 1px #bebebe;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"] + span::after,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"] + span::after");
            sb.AppendLine("{");
            sb.AppendLine("    text-indent: 0;");
            sb.AppendLine("    width: 62%;");
            sb.AppendLine("    height: 32px;");
            sb.AppendLine("    text-align: center;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"]:checked + span,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"]:checked + span");
            sb.AppendLine("{");
            /*sb.AppendLine("    filter: progid:DXImageTransform.Microsoft.gradient(startColorstr=\"#1b45bd\", endColorstr=\"#5d96ea\");");
            sb.AppendLine("    background: -moz-linear-gradient(top, #1b45bd, #5d96ea);");
            sb.AppendLine("    background: -webkit-linear-gradient(top, #1b45bd, #5d96ea);");
            sb.AppendLine("    background: -o-linear-gradient(top, #1b45bd, #5d96ea);");
            sb.AppendLine("    background: -ms-linear-gradient(top, #1b45bd, #5d96ea);");
            sb.AppendLine("    background: linear-gradient(top, #1b45bd, #5d96ea);");*/
            sb.AppendLine("    filter: progid:DXImageTransform.Microsoft.gradient(startColorstr=\"" + System.Drawing.ColorTranslator.ToHtml(backColor) + "\", endColorstr=\"" + System.Drawing.ColorTranslator.ToHtml(backColor) + "\");");
            sb.AppendLine("    background: -moz-linear-gradient(top, " + System.Drawing.ColorTranslator.ToHtml(backColor) + ", " + System.Drawing.ColorTranslator.ToHtml(backColor) + ");");
            sb.AppendLine("    background: -webkit-linear-gradient(top, " + System.Drawing.ColorTranslator.ToHtml(backColor) + ", " + System.Drawing.ColorTranslator.ToHtml(backColor) + ");");
            sb.AppendLine("    background: -o-linear-gradient(top, " + System.Drawing.ColorTranslator.ToHtml(backColor) + ", " + System.Drawing.ColorTranslator.ToHtml(backColor) + ");");
            sb.AppendLine("    background: -ms-linear-gradient(top, " + System.Drawing.ColorTranslator.ToHtml(backColor) + ", " + System.Drawing.ColorTranslator.ToHtml(backColor) + ");");
            sb.AppendLine("    background: linear-gradient(top, " + System.Drawing.ColorTranslator.ToHtml(backColor) + ", " + System.Drawing.ColorTranslator.ToHtml(backColor) + ");");
            //sb.AppendLine("    background: " + System.Drawing.ColorTranslator.ToHtml(backColor) + ";");
            sb.AppendLine("    color: #fff;");
            sb.AppendLine("    text-shadow: -1px -1px #0d2046;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"]:checked + span::before,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"]:checked + span::before");
            sb.AppendLine("{");
            sb.AppendLine("    left: auto;");
            sb.AppendLine("    -moz-box-shadow: -2px 0 1px #3a5e91;");
            sb.AppendLine("    -webkit-box-shadow: -2px 0 1px #3a5e91;");
            sb.AppendLine("    box-shadow: -2px 0 1px #3a5e91;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton input[type=\"checkbox\"]:checked + span::after,");
            sb.AppendLine(scope + " .toggleButton input[type=\"radio\"]:checked + span::after");
            sb.AppendLine("{");
            sb.AppendLine("    border-top: solid 1px #0f2a4f;");
            sb.AppendLine("    border-bottom: solid 1px #0f2a4f;");
            sb.AppendLine("    border-left: solid 1px #2c5496;");
            sb.AppendLine("    height: 30px;");
            sb.AppendLine("    top: -1px;");
            sb.AppendLine("    left: -1px;");
            sb.AppendLine("    -moz-border-radius: 4px 0 0 4px;");
            sb.AppendLine("    -webkit-border-radius: 4px 0 0 4px;");
            sb.AppendLine("    border-radius: 4px 0 0 4px;");
            sb.AppendLine("}");
            sb.AppendLine(scope + " .toggleButton_browser-support li.ie6,");
            sb.AppendLine(scope + " .toggleButton_browser-support li.ie7,");
            sb.AppendLine(scope + " .toggleButton_browser-support li.ie8");
            sb.AppendLine("{");
            sb.AppendLine("    display: none;");
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static void ReplacePDFTitle(DMSFile file, string title)
        {
            if (System.IO.Path.GetExtension(file.Filename).Equals(".pdf", StringComparison.OrdinalIgnoreCase) && file.StatusId == 1)
            {
                try
                {
                    file.FileVersion.LoadContents();
                    if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                    {
                        string tempPdf = String.Format("{0}{1}.pdf", System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(file.Filename));
                        string keywords = string.Join(", ", (from tag in Components.DocumentController.GetAllTagsForDocument(file.DocumentId) select tag.Tag.TagName).ToList());
                        using (Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument(file.FileVersion.Contents))
                        {
                            doc.XmpMetaData.SetTitle(title);
                            doc.XmpMetaData.SetKeywords(keywords);
                            doc.SaveToFile(tempPdf);
                            if (System.IO.File.Exists(tempPdf))
                            {
                                file.FileVersion.Contents = System.IO.File.ReadAllBytes(tempPdf);
                                file.FileVersion.SaveContents();
                                try
                                {
                                    System.IO.File.Delete(tempPdf);
                                }
                                catch(Exception)
                                {
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

        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height, bool fast = false)
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

        public static byte[] CreateThumbnail(HttpRequest request, string controlPath, System.Drawing.Bitmap img, ref bool isLandscape, ThumbnailMode thumbnailMode = ThumbnailMode.Auto, ThumbnailResize thumbnailResize = ThumbnailResize.Stretch)
        {
            byte[] byteImage = null;
            string templatePortraitFile = request.MapPath(string.Format("{0}/Images/pdftemplate_portrait.png", controlPath));
            string templateLandscapeFile = request.MapPath(string.Format("{0}/Images/pdftemplate_landscape.png", controlPath));
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

        public static byte[] CreateThumbnail(HttpRequest request, string controlPath, string inputFile, ref bool isLandscape)
        {
            try
            {
                using (GhostscriptRasterizer rasterizer = new GhostscriptRasterizer())
                {
                    rasterizer.CustomSwitches.Add("-dUseCropBox");
                    rasterizer.CustomSwitches.Add("-c");
                    rasterizer.CustomSwitches.Add("[/CropBox [24 72 559 794] /PAGES pdfmark");
                    rasterizer.CustomSwitches.Add("-f");
                    rasterizer.Open(inputFile);
                    var pageNumber = 1;
                    using (Bitmap img = new Bitmap(rasterizer.GetPage(10, pageNumber)))
                    {
                        if (!IsImageBlank(img))
                        {
                            return CreateThumbnail(request, controlPath, img, ref isLandscape);
                        }
                    }
                    rasterizer.Close();
                    rasterizer.CustomSwitches.Clear();
                    rasterizer.Open(inputFile);
                    using (Bitmap img = new Bitmap(rasterizer.GetPage(10, pageNumber)))
                    {
                        using (Bitmap img2 = new Bitmap((img.Width > img.Height ? (img.Height * 83) / 109 : img.Width), img.Height, PixelFormat.Format24bppRgb))
                        {
                            using (Graphics g = Graphics.FromImage(img2))
                            {
                                g.DrawImage(img, 0, 0);
                            }
                            return CreateThumbnail(request, controlPath, img2, ref isLandscape);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PixelData
        {
            public byte B;
            public byte G;
            public byte R;
            public byte A;
        }

        public static bool IsImageBlank(Bitmap bitmap)
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

        public static byte[] FiletoByteArray(string filePath)
        {
            return System.IO.File.ReadAllBytes(filePath);
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        public static void CreateThumbnail(HttpRequest request, string controlPath, Components.DMSFile file)
        {
            file.FileVersion.LoadThumbnail();
            if (file.FileVersion.Thumbnail == null || file.FileVersion.Thumbnail.Length == 0)
            {
                bool isLandscape = false;
                if (file != null && file.FileType.Equals("pdf", StringComparison.OrdinalIgnoreCase) && file.Status.StatusId == 1)
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
                        file.FileVersion.Thumbnail = CreateThumbnail(request, controlPath, tempPdf, ref isLandscape);
                        if (file.FileVersion.Thumbnail != null)
                        {
                            file.FileVersion.SaveThumbnail(isLandscape);
                        }
                        if (System.IO.File.Exists(tempPdf))
                        {
                            System.IO.File.Delete(tempPdf);
                        }
                    }
                }
                else if (file != null && (file.FileType.Equals("png", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("jpg", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("gif", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("bmp", StringComparison.OrdinalIgnoreCase)) && file.Status.StatusId == 1)
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
                                file.FileVersion.Thumbnail = CreateThumbnail(request, controlPath, bmp, ref isLandscape);
                                if (file.FileVersion.Thumbnail != null)
                                {
                                    file.FileVersion.SaveThumbnail(isLandscape);
                                }
                            }
                        }
                    }
                }
                else if (file != null && (file.FileType.Equals("doc", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("docx", StringComparison.OrdinalIgnoreCase)) && file.Status.StatusId == 1)
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
                            }
                        }
                        catch (Exception)
                        {
                        }
                        if (System.IO.File.Exists(tempPdf))
                        {
                            file.FileVersion.Thumbnail = CreateThumbnail(request, controlPath, tempPdf, ref isLandscape);
                            if (file.FileVersion.Thumbnail != null)
                            {
                                file.FileVersion.SaveThumbnail(isLandscape);
                            }
                            System.IO.File.Delete(tempPdf);
                        }
                    }
                }
                else if (file != null && (file.FileType.Equals("xls", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("xlsx", StringComparison.OrdinalIgnoreCase)) && file.Status.StatusId == 1)
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
                                    //workbook.LoadFromFile(tempExcel);
                                    workbook.LoadFromStream(xlsStream);
                                    workbook.ConverterSetting.SheetFitToPage = true;
                                    workbook.SaveToFile(tempPdf, Spire.Xls.FileFormat.PDF);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                        if (System.IO.File.Exists(tempPdf))
                        {
                            file.FileVersion.Thumbnail = CreateThumbnail(request, controlPath, tempPdf, ref isLandscape);
                            if (file.FileVersion.Thumbnail != null)
                            {
                                file.FileVersion.SaveThumbnail(isLandscape);
                            }
                            System.IO.File.Delete(tempPdf);
                        }
                    }
                }
                /*else if (file != null && (file.FileType.Equals("ppt", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("pptx", StringComparison.OrdinalIgnoreCase)) && file.Status.StatusId == 1)
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
                            }
                        }
                        catch(Exception)
                        {
                        }
                        if (System.IO.File.Exists(tempPdf))
                        {
                            file.FileVersion.Thumbnail = CreateThumbnail(tempPdf);
                            if (file.FileVersion.Thumbnail != null)
                            {
                                file.FileVersion.SaveThumbnail();
                            }
                            System.IO.File.Delete(tempPdf);
                        }
                    }
                }*/
                else if (file != null && file.FileType.Equals("mp3", StringComparison.OrdinalIgnoreCase) && file.Status.StatusId == 1)
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
                                                        file.FileVersion.Thumbnail = CreateThumbnail(request, controlPath, bmp, ref isLandscape, ThumbnailMode.Auto, ThumbnailResize.MaintainAspectRatio);
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
                else if (file != null && (file.FileType.Equals("mp4", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("mov", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("wmv", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("avi", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("wma", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("ppt", StringComparison.OrdinalIgnoreCase) || file.FileType.Equals("pptx", StringComparison.OrdinalIgnoreCase)) && file.Status.StatusId == 1)
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
                            file.FileVersion.Thumbnail = CreateThumbnail(request, controlPath, bmp, ref isLandscape, ThumbnailMode.Auto, ThumbnailResize.MaintainAspectRatio);
                            if (file.FileVersion.Thumbnail != null)
                            {
                                file.FileVersion.SaveThumbnail(isLandscape);
                            }
                            if (System.IO.File.Exists(tempFile))
                            {
                                System.IO.File.Delete(tempFile);
                            }
                        }
                    }
                }
            }
            file.FileVersion.Contents = null;
            file.FileVersion.Thumbnail = null;
        }

        public static void CreateThumbnail(HttpRequest request, string controlPath, string fileName, int fileVersionId)
        {
            if (System.IO.File.Exists(fileName))
            {
                bool isLandscape = false;
                if (System.IO.Path.GetExtension(fileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] thumbnail = CreateThumbnail(request, controlPath, fileName, ref isLandscape);
                    if (thumbnail != null && thumbnail.Length > 0)
                    {
                        DocumentController.SaveThumbnail(fileVersionId, isLandscape, thumbnail);
                    }
                }
                else if (System.IO.Path.GetExtension(fileName).Equals(".png", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".jpg", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".gif", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                {
                    using (Bitmap bmp = new Bitmap(fileName))
                    {
                        byte[] thumbnail = CreateThumbnail(request, controlPath, bmp, ref isLandscape);
                        if (thumbnail != null && thumbnail.Length > 0)
                        {
                            DocumentController.SaveThumbnail(fileVersionId, isLandscape, thumbnail);
                        }
                    }
                }
                else if (System.IO.Path.GetExtension(fileName).Equals(".doc", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".docx", StringComparison.OrdinalIgnoreCase))
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
                        byte[] thumbnail = CreateThumbnail(request, controlPath, tempPdf, ref isLandscape);
                        if (thumbnail != null && thumbnail.Length > 0)
                        {
                            DocumentController.SaveThumbnail(fileVersionId, isLandscape, thumbnail);
                        }
                        System.IO.File.Delete(tempPdf);
                    }
                }
                else if (System.IO.Path.GetExtension(fileName).Equals(".xls", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    string tempPdf = String.Format("{0}{1}_temp.pdf", System.IO.Path.GetTempPath(), System.IO.Path.GetFileNameWithoutExtension(fileName));
                    try
                    {
                        using (Spire.Xls.Workbook workbook = new Spire.Xls.Workbook())
                        {
                            workbook.LoadFromFile(fileName);
                            workbook.ConverterSetting.SheetFitToPage = true;
                            workbook.SaveToFile(tempPdf, Spire.Xls.FileFormat.PDF);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    if (System.IO.File.Exists(tempPdf))
                    {
                        byte[] thumbnail = CreateThumbnail(request, controlPath, tempPdf, ref isLandscape);
                        if (thumbnail != null && thumbnail.Length > 0)
                        {
                            DocumentController.SaveThumbnail(fileVersionId, isLandscape, thumbnail);
                        }
                        System.IO.File.Delete(tempPdf);
                    }
                }
                else if (System.IO.Path.GetExtension(fileName).Equals(".mp3", StringComparison.OrdinalIgnoreCase))
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
                                                byte[] thumbnail = CreateThumbnail(request, controlPath, bmp, ref isLandscape, ThumbnailMode.Auto, ThumbnailResize.MaintainAspectRatio);
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
                else if (System.IO.Path.GetExtension(fileName).Equals(".mp4", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".mov", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".wmv", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".avi", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".wma", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".ppt", StringComparison.OrdinalIgnoreCase) || System.IO.Path.GetExtension(fileName).Equals(".pptx", StringComparison.OrdinalIgnoreCase))
                {
                    using (Microsoft.WindowsAPICodePack.Shell.ShellFile shellFile = Microsoft.WindowsAPICodePack.Shell.ShellFile.FromFilePath(fileName))
                    {
                        Bitmap bmp = new Bitmap(shellFile.Thumbnail.LargeBitmap);
                        bmp.MakeTransparent();
                        byte[] thumbnail = CreateThumbnail(request, controlPath, bmp, ref isLandscape, ThumbnailMode.Auto, ThumbnailResize.MaintainAspectRatio);
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