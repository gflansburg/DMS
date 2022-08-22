/*
' Copyright (c) 2021 Gafware
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Reflection;
using System.Web.UI.WebControls;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Tabs;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Xml.Serialization;
using Gafware.Modules.DMS.Components;
using System.Web;
using DotNetNuke.Common;
using System.Collections;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using System.Drawing;
using System.Drawing.Imaging;
using DotNetNuke.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using DotNetNuke.Abstractions.Portals;
using Ghostscript.NET.Rasterizer;
using System.IO;
using Gafware.Modules.DMS.ThumbDBLib;
using OfficeOpenXml;

public static class ImageExtensions
{
    public static byte[] ToByteArray(this System.Drawing.Image image, ImageFormat format)
    {
        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
        {
            image.Save(ms, format);
            return ms.ToArray();
        }
    }
}

namespace Gafware.Modules.DMS
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from DMSModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class DocumentList : DMSModuleBase, IActionable
    {
        private readonly INavigationManager _navigationManager;

        public DocumentList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        public ViewMode ViewMode
        {
            get
            {
                return (ViewState["ViewMode"] != null ? (ViewMode)ViewState["ViewMode"] : ViewMode.Details);
            }
            set
            {
                if (ViewMode != value)
                {
                    DotNetNuke.Entities.Users.UserInfo user = DotNetNuke.Entities.Users.UserController.Instance.GetUserById(PortalId, UserId);
                    ViewState["ViewMode"] = value;
                    pnlDetails3.Visible = (value == DMS.ViewMode.Details);
                    pnlEdit.Visible = (value == DMS.ViewMode.Edit && DocumentID > 0);
                    pnlDetails2.Visible = (value == DMS.ViewMode.Details || DocumentID > 0);
                    pnlOwnerDetails.Visible = (value == DMS.ViewMode.Details || (value == DMS.ViewMode.Edit && !(user.IsSuperUser || user.IsInRole("Administrator"))));
                    pnlOwnerEdit.Visible = (value == DMS.ViewMode.Edit && (user.IsSuperUser || user.IsInRole("Administrator")));
                    pnlIsPublicDetails.Visible = (value == DMS.ViewMode.Details);
                    pnlIsPublicEdit.Visible = (value == DMS.ViewMode.Edit);
                    pnlActivationDateDetails.Visible = (value == DMS.ViewMode.Details);
                    pnlActivationDateEdit.Visible = (value == DMS.ViewMode.Edit);
                    pnlExpirationDateDetails.Visible = (value == DMS.ViewMode.Details);
                    pnlExpirationDateEdit.Visible = (value == DMS.ViewMode.Edit);
                    pnlSearchableDetails.Visible = (value == DMS.ViewMode.Details);
                    pnlSearchableEdit.Visible = (value == DMS.ViewMode.Edit);
                    pnlUseCategorySecurityRolesDetails.Visible = (value == DMS.ViewMode.Details);
                    pnlUseCategorySecurityRolesEdit.Visible = (value == DMS.ViewMode.Edit);
                    pnlSecurityRoleDetails.Visible = (value == DMS.ViewMode.Details);
                    pnlSecurityRoleEdit.Visible = (value == DMS.ViewMode.Edit);
                    pnlDocumentDetailsDetails.Visible = (value == DMS.ViewMode.Details);
                    pnlDocumentDetailsEdit.Visible = (value == DMS.ViewMode.Edit);
                    pnlAdminCommentsDetails.Visible = (value == DMS.ViewMode.Details);
                    pnlAdminCommentsEdit.Visible = (value == DMS.ViewMode.Edit);
                    pnlDocumentNameEdit.Visible = (value == DMS.ViewMode.Edit);
                    rptCategory.DataSource = Components.DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId);
                    rptCategory.DataBind();
                    rptCategory.Visible = Components.DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId).Count > 1;
                    BindDocData();
                }
            }
        }

        public int DocumentID
        {
            get
            {
                return (ViewState["DocumentID"] != null ? (int)ViewState["DocumentID"] : 0);
            }
            set
            {
                ViewState["DocumentID"] = value;
                BindDocData();
            }
        }

        protected string SortColumn
        {
            get
            {
                return (ViewState["SortColumn"] != null ? ViewState["SortColumn"].ToString() : "DocumentName");
            }
            set
            {
                ViewState["SortColumn"] = value;
            }
        }

        protected SortDirection SortDirection
        {
            get
            {
                return (ViewState["SortDirection"] != null ? (SortDirection)ViewState["SortDirection"] : SortDirection.Ascending);
            }
            set
            {
                ViewState["SortDirection"] = value;
            }
        }

        /*protected void Page_Init(object sender, EventArgs e)
        {
            if (DotNetNuke.Framework.AJAX.IsInstalled())
            {
                DotNetNuke.Framework.AJAX.RegisterScriptManager();
                TelerikAjaxUtility.InstallRadAjaxManager();
            }
        }*/

        public string GenerateScript()
        {
            System.Web.UI.HtmlControls.HtmlGenericControl script = new System.Web.UI.HtmlControls.HtmlGenericControl();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("function canRemoveTag(sender, eventArgs) {");
            sb.AppendLine("  if(confirm('" + LocalizeString("DeleteTagConfirm") + "')) {");
            sb.AppendLine("    var autoCompleteBox = $find('" + tbTags.ClientID + "');");
            sb.AppendLine("    var count = autoCompleteBox.get_entries().get_count();");
            sb.AppendLine("    if(count < 2) {");
            sb.AppendLine("      $.alert({ title: 'Tag Error', content: '" + LocalizeString("OneTagError") + "' });");
            //sb.AppendLine("      alert('" + LocalizeString("OneTagError") + "');");
            sb.AppendLine("      eventArgs.set_cancel(true);");
            sb.AppendLine("    } else {");
            sb.AppendLine("      var hid = document.getElementById('" + hidTagToRemove.ClientID + "');");
            sb.AppendLine("      hid.value = eventArgs.get_entry().get_text();");
            sb.AppendLine("      " + Page.ClientScript.GetPostBackEventReference(lnkRemoveTag, String.Empty) + ";");
            sb.AppendLine("    }");
            sb.AppendLine("  } else {");
            sb.AppendLine("    eventArgs.set_cancel(true);");
            sb.AppendLine("  }");
            sb.AppendLine("}");
            sb.AppendLine("function newUrlKeyPress(event) {");
            sb.AppendLine(" var key = event.keyCode;");
            sb.AppendLine(" if (key == 13) {");
            sb.AppendLine("  " + Page.ClientScript.GetPostBackEventReference(btnSaveLink, "") + ";");
            sb.AppendLine("   return false;");
            sb.AppendLine(" }");
            sb.AppendLine(" return true;");
            sb.AppendLine("}");
            sb.Append("var validFiles = [");
            List<Components.FileType> fileTypes = Components.DocumentController.GetAllFileTypes(PortalId, PortalWideRepository ? 0 : TabModuleId);
            bool first = true;
            foreach (Components.FileType fileType in fileTypes)
            {
                string[] extensions = fileType.FileTypeExt.Split(',');
                foreach (string ext in extensions)
                {
                    if (!first)
                    {
                        sb.Append(",");
                    }
                    sb.Append("'" + ext + "'");
                }
                first = false;
            }
            sb.AppendLine("];");
            sb.AppendLine("function OnUpload() {");
            sb.AppendLine("  var obj = document.getElementById('" + upDocument.ClientID + "');");
            sb.AppendLine("  var source = obj.value;");
            sb.AppendLine("  var ext = source.substring(source.lastIndexOf('.') + 1, source.length).toLowerCase();");
            sb.AppendLine("  for (var i = 0; i < validFiles.length; i++) {");
            sb.AppendLine("    if (validFiles[i] == ext)");
            sb.AppendLine("      break;");
            sb.AppendLine("  }");
            sb.AppendLine("  if (i >= validFiles.length) {");
            sb.Append("    var msg = '" + LocalizeString("InvalidFileType") + ":<br/><br/>");
            foreach (Components.FileType fileType in fileTypes)
            {
                sb.Append(fileType.FileTypeName.Replace("'", "\\'") + " (" + fileType.FileTypeExt + ")<br/>");
            }
            sb.AppendLine("';");
            sb.AppendLine("    $('#" + pnlUploadMsg.ClientID + "').html(msg); $('#" + pnlUploadMsg.ClientID + "').css('visibility', 'visible').css('opacity', '1'); setTimeout(function () { $('#" + pnlUploadMsg.ClientID + "').fadeOut(\"slow\", function () { $('#" + pnlUploadMsg.ClientID + "').show().css('visibility', 'hidden'); }); }, 5000);");
            sb.AppendLine("    return false;");
            sb.AppendLine("  }");
            sb.AppendLine("  $('#newFileDialog').dialog('close');");
            sb.AppendLine("  showBlockingScreen(\"Uploading\");");
            sb.AppendLine("  return true;");
            sb.AppendLine("}");
            sb.AppendLine("function MM_swapImgRestore() { //v3.0");
            sb.AppendLine("    var i, x, a = document.MM_sr; for (i = 0; a && i < a.length && (x = a[i]) && x.oSrc; i++) x.src = x.oSrc;");
            sb.AppendLine("}");
            sb.AppendLine("function MM_preloadImages() { //v3.0");
            sb.AppendLine("    var d = document; if (d.images)");
            sb.AppendLine("    {");
            sb.AppendLine("        if (!d.MM_p) d.MM_p = new Array();");
            sb.AppendLine("        var i, j = d.MM_p.length, a = MM_preloadImages.arguments; for (i = 0; i < a.length; i++)");
            sb.AppendLine("            if (a[i].indexOf(\"#\") != 0) { d.MM_p[j] = new Image; d.MM_p[j++].src = a[i]; }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            sb.AppendLine("function MM_findObj(n, d) { //v4.01");
            sb.AppendLine("    var p, i, x; if (!d) d = document; if ((p = n.indexOf(\"?\")) > 0 && parent.frames.length)");
            sb.AppendLine("    {");
            sb.AppendLine("        d = parent.frames[n.substring(p + 1)].document; n = n.substring(0, p);");
            sb.AppendLine("    }");
            sb.AppendLine("    if (!(x = d[n]) && d.all) x = d.all[n]; for (i = 0; !x && i < d.forms.length; i++) x = d.forms[i][n];");
            sb.AppendLine("    for (i = 0; !x && d.layers && i < d.layers.length; i++) x = MM_findObj(n, d.layers[i].document);");
            sb.AppendLine("    if (!x && d.getElementById) x = d.getElementById(n); return x;");
            sb.AppendLine("}");
            sb.AppendLine("function MM_swapImage() { //v3.0");
            sb.AppendLine("    var i, j = 0, x, a = MM_swapImage.arguments; document.MM_sr = new Array; for (i = 0; i < (a.length - 2); i += 3)");
            sb.AppendLine("        if ((x = MM_findObj(a[i])) != null) { document.MM_sr[j++] = x; if (!x.oSrc) x.oSrc = x.src; x.src = a[i + 2]; }");
            sb.AppendLine("}");
            sb.AppendLine("MM_preloadImages('" + ResolveUrl("~" + ControlPath + "Images/Icons/DeleteIcon2.gif") + "','" + ResolveUrl("~/" + ControlPath + "Images/Icons/HistoryIcon2.gif") + "');");
            sb.AppendLine("jQuery(document).ready(function() {");
            sb.AppendLine("  var prm = Sys.WebForms.PageRequestManager.getInstance();");
            sb.AppendLine("  prm.add_endRequest(MyDocControlEndRequest);");
            sb.AppendLine("  initDocumentControlJavascript();");
            sb.AppendLine("});");
            sb.AppendLine("function MyDocControlEndRequest(sender, args) {");
            sb.AppendLine("  initDocumentControlJavascript();");
            sb.AppendLine("  hideBlockingScreen();");

            sb.AppendLine("   $('.hidMessage').each(function(i, item) {");
            sb.AppendLine("    var text = $(this).val()");
            sb.AppendLine("    if(text != '') {");
            sb.AppendLine("      var fileId = $(this).attr('data-uid');");
            sb.AppendLine("      var msg2 = $(\".statusMessage[data-uid='\" + fileId + \"']\");");
            sb.AppendLine("      msg2.text(text); msg2.show(); setTimeout(function () { msg2.fadeOut(\"slow\", function () { msg2.hide(); }); }, 2000);");
            sb.AppendLine("    }");
            sb.AppendLine("   });");

            sb.AppendLine("}");
            sb.AppendLine("function tbTags_EntryAdded(sender, eventArgs) {");
            sb.AppendLine("  $telerik.$(eventArgs.get_entry().get_token()).find('a').attr(\"title\", \"" + LocalizeString("DeleteTag") + "\");");
            sb.AppendLine("}");
            sb.AppendLine("function tbTags_EntryRemoving(sender, eventArgs) {");
            sb.AppendLine("  eventArgs.set_cancel(true);");
            sb.AppendLine("  var tag = eventArgs.get_entry().get_text();");
            sb.AppendLine("  $.confirm({");
            sb.AppendLine("    title: '" + LocalizeString("DeleteTag") + "',");
            sb.AppendLine("    content: '" + LocalizeString("ConfirmDeleteTag") + "',");
            sb.AppendLine("    buttons: {");
            sb.AppendLine("      yes: function() {");
            sb.AppendLine("         $('#" + hidTagToRemove.ClientID + "').val(tag);");
            sb.AppendLine("         " + Page.ClientScript.GetPostBackEventReference(lnkRemoveTag, String.Empty) + ";");
            sb.AppendLine("      },");
            sb.AppendLine("      no: function() {");
            sb.AppendLine("      }");
            sb.AppendLine("    }");
            sb.AppendLine("  });");
            sb.AppendLine("  return false;");
            sb.AppendLine("}");
            sb.AppendLine("function confirmDeleteAll(control) {");
            sb.AppendLine("  $.confirm({");
            sb.AppendLine("    title: '" + LocalizeString("DeleteAll") + "',");
            sb.AppendLine("    content: '" + LocalizeString("ConfirmDeleteAll") + "',");
            sb.AppendLine("    buttons: {");
            sb.AppendLine("      yes: function() {");
            sb.AppendLine("        eval($(control).attr('href').replace('javascript:', ''));");
            sb.AppendLine("      },");
            sb.AppendLine("      no: function() {");
            sb.AppendLine("      }");
            sb.AppendLine("    }");
            sb.AppendLine("  });");
            sb.AppendLine("}");
            sb.AppendLine("function confirmDeleteFile(control, fileType) {");
            sb.AppendLine("  $.confirm({");
            sb.AppendLine("    title: (fileType === 'url' ? \"" + LocalizeString("DeleteLink") + "\" : \"" + LocalizeString("DeleteFile") + "\"),");
            sb.AppendLine("    content: \"" + LocalizeString("ConfirmDeleteFile") + " \" + (fileType === 'url' ? \"" + LocalizeString("DeleteLink") + "\" : \"" + LocalizeString("DeleteFile") + "\") + \"?\",");
            sb.AppendLine("    buttons: {");
            sb.AppendLine("      yes: function() {");
            sb.AppendLine("        eval($(control).attr('href').replace('javascript:', ''));");
            sb.AppendLine("      },");
            sb.AppendLine("      no: function() {");
            sb.AppendLine("      }");
            sb.AppendLine("    }");
            sb.AppendLine("  });");
            sb.AppendLine("}");
            sb.AppendLine("function confirmDelete(control, docName) {");
            sb.AppendLine("  $.confirm({");
            sb.AppendLine("    title: \"" + LocalizeString("Delete") + " '\" + docName + \"'\",");
            sb.AppendLine("    content: (\"" + LocalizeString("ConfirmDelete") + "\").replaceAll('{0}', docName),");
            sb.AppendLine("    buttons: {");
            sb.AppendLine("      yes: function() {");
            sb.AppendLine("        eval($(control).attr('href').replace('javascript:', ''));");
            sb.AppendLine("      },");
            sb.AppendLine("      no: function() {");
            sb.AppendLine("      }");
            sb.AppendLine("    }");
            sb.AppendLine("  });");
            sb.AppendLine("}");
            sb.AppendLine("function editVersion(a) {");
            sb.AppendLine("  var ary = a.text.split('.');");
            sb.AppendLine("  $('#" + tbMajor.ClientID + "').val(ary[0]);");
            sb.AppendLine("  $('#" + tbMinor.ClientID + "').val(ary[1]);");
            sb.AppendLine("  $('#" + tbBuild.ClientID + "').val(ary[2]);");
            sb.AppendLine("  $('#" + hidFileVersionId.ClientID + "').val(a.dataset.uid);");
            sb.AppendLine("  $('#versionDialog').dialog('open');");
            sb.AppendLine("  $('#version-content').scrollTop(0);");
            sb.AppendLine("  setTimeout(function() { document.getElementById('" + tbMajor.ClientID + "').focus(); }, 1000);");
            sb.AppendLine("}");
            sb.AppendLine("var isBlockingScreen = true;");
            sb.AppendLine("function showBlockingScreen(message) {");
            sb.AppendLine("  if (!isBlockingScreen) {");
            sb.AppendLine("    isBlockingScreen = true;");
            sb.AppendLine("    if (message) {");
            sb.AppendLine("      $.blockUI({ message: '<h3 class=\"uiMessage\"><img src=\"" + ControlPath + "/Images/loading.gif\" /> ' + message + '...</h2>' });");
            sb.AppendLine("    } else {");
            sb.AppendLine("      $('.se-pre-con').show();");
            sb.AppendLine("    }");
            sb.AppendLine("  }");
            sb.AppendLine("}");
            sb.AppendLine("function hideBlockingScreen() {");
            sb.AppendLine("  //if (isBlockingScreen) {");
            sb.AppendLine("    // Animate loader off screen");
            sb.AppendLine("    isBlockingScreen = false;");
            sb.AppendLine("    $('.se-pre-con').fadeOut('fast');");
            sb.AppendLine("    $.unblockUI();");
            sb.AppendLine("  //}");
            sb.AppendLine("}");
            sb.AppendLine("$(window).ready(function() {");
            sb.AppendLine("  hideBlockingScreen();");
            sb.AppendLine("});");
            sb.AppendLine("$(window).load(function() {");
            sb.AppendLine("  //hideBlockingScreen();");
            sb.AppendLine("});");
            sb.AppendLine("$(window).unload(function() {");
            sb.AppendLine("  //showBlockingScreen();");
            sb.AppendLine("});");
            sb.AppendLine("var ignore_onbeforeunload = false;");
            sb.AppendLine("window.addEventListener('beforeunload', function(event) {");
            sb.AppendLine("  if (!ignore_onbeforeunload) {");
            sb.AppendLine("    showBlockingScreen();");
            sb.AppendLine("  }");
            sb.AppendLine("  ignore_onbeforeunload = false;");
            sb.AppendLine("});");
            sb.AppendLine("function initDocumentControlJavascript() {");
            sb.AppendLine("  $('a[href^=mailto]').on('click', function() {");
            sb.AppendLine("    ignore_onbeforeunload = true;");
            sb.AppendLine("  });");
            sb.AppendLine("  $(\"#" + btnCancelLink.ClientID + "\").click(function(e) {");
            sb.AppendLine("    e.preventDefault();");
            sb.AppendLine("    $('#newHyperlinkDialog').dialog('close');");
            sb.AppendLine("  });");
            sb.AppendLine("  $(\"#" + btnNewLink.ClientID + "\").click(function(e) {");
            sb.AppendLine("    e.preventDefault();");
            sb.AppendLine("    $('#" + tbURL.ClientID + "').val('');");
            sb.AppendLine("    $('#newHyperlinkDialog .FormInstructions').hide();");
            sb.AppendLine("    $('#newHyperlinkDialog').dialog('open');");
            sb.AppendLine("    $('#newHyperlink-content').scrollTop(0);");
            sb.AppendLine("    setTimeout(function() { document.getElementById('" + tbURL.ClientID + "').focus(); }, 1000);");
            sb.AppendLine("  });");
            sb.AppendLine("  $('#historyDialog').dialog({");
            sb.AppendLine("    autoOpen: false,");
            sb.AppendLine("    bgiframe: true,");
            sb.AppendLine("    modal: true,");
            sb.AppendLine("    width: 600,");
            sb.AppendLine("    height: 700,");
            sb.AppendLine("    resizable: false,");
            sb.AppendLine("    appendTo: '.dms',");
            sb.AppendLine("    dialogClass: 'dialog',");
            sb.AppendLine("    title: 'History',");
            sb.AppendLine("    closeOnEsacpe: true,");
            sb.AppendLine("    Cancel: function () {");
            sb.AppendLine("      $(this).dialog('close');");
            sb.AppendLine("    }");
            sb.AppendLine("  });");
            sb.AppendLine("  var form = $('#newHyperlinkDialog').dialog({");
            sb.AppendLine("     autoOpen: false,");
            sb.AppendLine("     bgiframe: true,");
            sb.AppendLine("     modal: true,");
            sb.AppendLine("     width: 600,");
            sb.AppendLine("     height: 200,");
            sb.AppendLine("     appendTo: 'form',");
            sb.AppendLine("     dialogClass: 'dialog',");
            sb.AppendLine("     resizable: false,");
            sb.AppendLine("     title: 'Add New Hyperlink',");
            sb.AppendLine("     closeOnEsacpe: true,");
            sb.AppendLine("     Cancel: function () {");
            sb.AppendLine("       $(this).dialog('close');");
            sb.AppendLine("     }");
            sb.AppendLine("   });");
            sb.AppendLine("  form.on(\"keypress\", \"input[type = text]\", function(event) {");
            sb.AppendLine("    if (event.keyCode == 13) {");
            sb.AppendLine("      event.preventDefault();");
            sb.AppendLine("      event.stopPropagation();");
            //sb.AppendLine("      WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(\"" + btnSaveLink.UniqueID + "\", \"\", true, \"" + btnSaveLink.ValidationGroup + "\", \"\", false, false));");
            //sb.AppendLine("      " + Page.ClientScript.GetPostBackEventReference(btnSaveLink, String.Empty) + ";");
            sb.AppendLine("      $('#" + btnSaveLink.ClientID + "').click();");
            sb.AppendLine("    }");
            sb.AppendLine("  });");
            sb.AppendLine("  $(\"#" + btnCancelFile.ClientID + "\").click(function(e) {");
            sb.AppendLine("    e.preventDefault();");
            sb.AppendLine("    $('#newFileDialog').dialog('close');");
            sb.AppendLine("  });");
            sb.AppendLine("  $(\"#" + btnNewFile.ClientID + "\").click(function(e) {");
            sb.AppendLine("    e.preventDefault();");
            sb.AppendLine("    $('#" + upDocument.ClientID + "').val('');");
            sb.AppendLine("    $('#" + btnSaveFile.ClientID + "').prop('disabled', true);");
            sb.AppendLine("    $('#newFileDialog .FormInstructions').hide();");
            sb.AppendLine("    $('#newFileDialog').dialog('open');");
            sb.AppendLine("    $('#newFile-content').scrollTop(0);");
            sb.AppendLine("  });");
            sb.AppendLine("  $('#" + tbLinkURL.ClientID + "').on('focus', function (e) {");
            sb.AppendLine("    $(this).select();");
            sb.AppendLine("  });");
            sb.AppendLine("  form = $('#newFileDialog').dialog({");
            sb.AppendLine("     autoOpen: false,");
            sb.AppendLine("     bgiframe: true,");
            sb.AppendLine("     modal: true,");
            sb.AppendLine("     width: 600,");
            sb.AppendLine("     height: 250,");
            sb.AppendLine("     appendTo: 'form',");
            sb.AppendLine("     dialogClass: 'dialog',");
            sb.AppendLine("     resizable: false,");
            sb.AppendLine("     title: '" + LocalizeString("AddNewFile") + "',");
            sb.AppendLine("     closeOnEsacpe: true,");
            sb.AppendLine("     Cancel: function () {");
            sb.AppendLine("       $(this).dialog('close');");
            sb.AppendLine("     }");
            sb.AppendLine("   });");
            sb.AppendLine("  form.on(\"keypress\", \"input[type = text]\", function(event) {");
            sb.AppendLine("    if (event.keyCode == 13) {");
            sb.AppendLine("      event.preventDefault();");
            sb.AppendLine("      event.stopPropagation();");
            //sb.AppendLine("      WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(\"" + btnSaveFile.UniqueID + "\", \"\", true, \"" + btnSaveFile.ValidationGroup + "\", \"\", false, false));");
            //sb.AppendLine("      " + Page.ClientScript.GetPostBackEventReference(btnSaveFile, String.Empty) + ";");
            sb.AppendLine("      $('#" + btnSaveFile.ClientID + "').click();");
            sb.AppendLine("    }");
            sb.AppendLine("  });");
            sb.AppendLine("  $('#versionDialog').dialog({");
            sb.AppendLine("     autoOpen: false,");
            sb.AppendLine("     bgiframe: true,");
            sb.AppendLine("     modal: true,");
            sb.AppendLine("     width: 400,");
            sb.AppendLine("     height: 200,");
            sb.AppendLine("     dialogClass: 'dialog',");
            sb.AppendLine("     resizable: false,");
            sb.AppendLine("     title: '" + LocalizeString("Version") + "',");
            sb.AppendLine("     closeOnEsacpe: true,");
            sb.AppendLine("     Cancel: function () {");
            sb.AppendLine("       $(this).dialog('close');");
            sb.AppendLine("     }");
            sb.AppendLine("   });");
            sb.AppendLine("   $('.dms .inputfile').each(function() {");
            sb.AppendLine("    var $input = $(this);");
            sb.AppendLine("    var $label = $input.next('label');");
            sb.AppendLine("    var labelVal = $label.html();");
            sb.AppendLine("    $input.on('change', function(e) {");
            sb.AppendLine("      var fileName = '';");
            sb.AppendLine("      if (this.files && this.files.length > 1)");
            sb.AppendLine("        fileName = (this.attr('data-multiple-caption') || '' ).replace('{count}', this.files.length);");
            sb.AppendLine("      else if (e.target.value)");
            sb.AppendLine("        fileName = e.target.value.split('\\\\').pop();");
            sb.AppendLine("      if (fileName) {");
            sb.AppendLine("        $label.find('span').html(fileName);");
            sb.AppendLine("        $('#" + btnSaveFile.ClientID + "').prop('disabled', false);");
            sb.AppendLine("      } else {");
            sb.AppendLine("        $label.html(labelVal);");
            sb.AppendLine("        $('#" + btnSaveFile.ClientID + "').prop('disabled', true);");
            sb.AppendLine("      }");
            sb.AppendLine("    });");
            sb.AppendLine("    $input.on('focus', function(){ $input.addClass('has-focus'); }).on('blur', function(){ $input.removeClass('has-focus'); });");
            sb.AppendLine("  });");
            sb.AppendLine("  var history = Number($('#" + history.ClientID + "').val());");
            sb.AppendLine("  if(history)");
            sb.AppendLine("    showHistory();");
            sb.AppendLine("}");
            sb.AppendLine("function showHistory() {");
            sb.AppendLine("  $('#historyDialog').dialog('open');");
            sb.AppendLine("}");
            sb.AppendLine("function toggleNewGroup(cb) {");
            sb.AppendLine("  if(cb.checked) {");
            sb.AppendLine("    $('#" + pnlNewOwner.ClientID + "').hide();");
            sb.AppendLine("    $('#" + pnlNewGroup.ClientID + "').show();");
            sb.AppendLine("  } else {");
            sb.AppendLine("    $('#" + pnlNewOwner.ClientID + "').show();");
            sb.AppendLine("    $('#" + pnlNewGroup.ClientID + "').hide();");
            sb.AppendLine("  }");
            sb.AppendLine("}");
            sb.AppendLine("function newOwnerRequired(source, arguments) {");
            sb.AppendLine("  arguments.IsValid = (arguments.Value > 0 || $('#" + cbIsGroupOwner2.ClientID + "').is(':checked') === true);");
            sb.AppendLine("}");
            sb.AppendLine("function newGroupRequired(source, arguments) {");
            sb.AppendLine("  arguments.IsValid = (arguments.Value > 0 || $('#" + cbIsGroupOwner2.ClientID + "').is(':checked') === false);");
            sb.AppendLine("}");
            sb.AppendLine("function toggleCurrentGroup(cb) {");
            sb.AppendLine("  if(cb.checked) {");
            sb.AppendLine("    $('#" + pnlCurrentOwner.ClientID + "').hide();");
            sb.AppendLine("    $('#" + pnlCurrentGroup.ClientID + "').show();");
            sb.AppendLine("  } else {");
            sb.AppendLine("    $('#" + pnlCurrentOwner.ClientID + "').show();");
            sb.AppendLine("    $('#" + pnlCurrentGroup.ClientID + "').hide();");
            sb.AppendLine("  }");
            sb.AppendLine("}");
            sb.AppendLine("function currentOwnerRequired(source, arguments) {");
            sb.AppendLine("  arguments.IsValid = (arguments.Value > 0 || $('#" + cbIsGroupOwner3.ClientID + "').is(':checked') === true);");
            sb.AppendLine("}");
            sb.AppendLine("function currentGroupRequired(source, arguments) {");
            sb.AppendLine("  arguments.IsValid = (arguments.Value > 0 || $('#" + cbIsGroupOwner3.ClientID + "').is(':checked') === false);");
            sb.AppendLine("}");
            string url = GetNaviateUrl("GetDocuments");
            sb.AppendLine("function toggleStatus(cb, fileId) {");
            sb.AppendLine("  $.ajax({");
            sb.AppendLine("    url: \"" + ControlPath + "DMSController.asmx/ToggleStatus\",");
            sb.AppendLine("    type: \"POST\",");
            sb.AppendLine("    dataType: \"json\",");
            sb.AppendLine("    data: { fileId: fileId, bActive: cb.checked, path: '" + ControlPath + "', mid: " + ModuleId.ToString() + ", url: '" + url + "' },");
            sb.AppendLine("    success: function (result) {");
            sb.AppendLine("      if (result.Status === 1) {");
            sb.AppendLine("        var msg = $(\".statusMessage[data-uid='\" + fileId + \"']\");");
            sb.AppendLine("        msg.text('" + LocalizeString("Saved") + "'); msg.show(); setTimeout(function () { msg.fadeOut(\"slow\", function () { msg.hide(); }); }, 2000);");
            sb.AppendLine("        $(\".document[data-uid='\" + fileId + \"']\").prop('href', result.DocumentUrl);");
            sb.AppendLine("        $.each(result.Inactive, function (i, item) {");
            sb.AppendLine("          $(\".toggleButton[data-uid='\" + item.FileId + \"'] label input[type='checkbox']\").prop(\"checked\", false);");
            sb.AppendLine("          $(\".document[data-uid='\" + item.FileId + \"']\").prop('href', item.DocumentUrl);");
            sb.AppendLine("          var msg2 = $(\".statusMessage[data-uid='\" + item.FileId + \"']\");");
            sb.AppendLine("          msg2.text('" + LocalizeString("Saved") + "'); msg2.show(); setTimeout(function () { msg2.fadeOut(\"slow\", function () { msg2.hide(); }); }, 2000);");
            sb.AppendLine("        });");
            sb.AppendLine("      } else {");
            sb.AppendLine("        cb.checked = !cb.checked;");
            sb.AppendLine("        var msg = $(\".statusMessage[data-uid='\" + fileId + \"']\");");
            sb.AppendLine("        msg.text(result.Error); msg.show(); setTimeout(function () { msg.fadeOut(\"slow\", function () { msg.hide(); }); }, 2000);");
            sb.AppendLine("      }");
            sb.AppendLine("    },");
            sb.AppendLine("    error: function(jqXHR, exception) {");
            sb.AppendLine("      var error = '';");
            sb.AppendLine("      if (jqXHR.status === 0) {");
            sb.AppendLine("        error = 'Not connect. Verify Network.';");
            sb.AppendLine("      } else if (jqXHR.status == 404) {");
            sb.AppendLine("        error = 'Requested page not found. [404]';");
            sb.AppendLine("      } else if (jqXHR.status == 500) {");
            sb.AppendLine("        error = 'Internal Server Error [500].';");
            sb.AppendLine("      } else if (exception === 'parsererror') {");
            sb.AppendLine("        error = 'Requested JSON parse failed.';");
            sb.AppendLine("      } else if (exception === 'timeout') {");
            sb.AppendLine("        error = 'Time out error.';");
            sb.AppendLine("      } else if (exception === 'abort') {");
            sb.AppendLine("        error = 'Ajax request aborted.';");
            sb.AppendLine("      } else {");
            sb.AppendLine("        error = 'Uncaught Error. ' + jqXHR.responseText;");
            sb.AppendLine("      }");
            sb.AppendLine("      __doPostBack(cb.id, fileId);");
            //sb.AppendLine("      cb.checked = !cb.checked;");
            //sb.AppendLine("      var msg = $(\".statusMessage[data-uid='\" + fileId + \"']\");");
            //sb.AppendLine("      msg.text(error); msg.show(); setTimeout(function () { msg.fadeOut(\"slow\", function () { msg.hide(); }); }, 2000);");
            sb.AppendLine("    }");
            sb.AppendLine("  });");
            //sb.AppendLine("      __doPostBack(cb.id, fileId);");
            sb.AppendLine("}");
            sb.AppendLine("function saveVersion() {");
            sb.AppendLine("  var major = $('#" + tbMajor.ClientID + "').val();");
            sb.AppendLine("  var minor = $('#" + tbMinor.ClientID + "').val();");
            sb.AppendLine("  var build = $('#" + tbBuild.ClientID + "').val();");
            sb.AppendLine("  var fileVersionId = $('#" + hidFileVersionId.ClientID + "').val();");
            sb.AppendLine("  $.ajax({");
            sb.AppendLine("    url: \"" + ControlPath + "DMSController.asmx/SaveVersion\",");
            sb.AppendLine("    type: \"POST\",");
            sb.AppendLine("    dataType: \"json\",");
            sb.AppendLine("    data: { fileVersionId: fileVersionId, major: major, minor: minor, build: build, path: '" + ControlPath + "', mid: " + ModuleId.ToString() + ", url: '" + url + "' },");
            sb.AppendLine("    success: function (result) {");
            sb.AppendLine("      if (result.Status === 1) {");
            sb.AppendLine("        $('#versionDialog').dialog('close');");
            sb.AppendLine("        if($('#historyDialog').dialog('isOpen')) {");
            sb.AppendLine("          $(\"#historyDialog .version[data-uid='\" + fileVersionId + \"']\").text(result.Version);");
            sb.AppendLine("          $(\"#" + gvHistory.ClientID + " td[data-uid='\" + fileVersionId + \"']\").attr('data-version', result.NewVersion);");
            sb.AppendLine("          if (result.NewFile) {");
            //sb.AppendLine("            $('#historyDialog').dialog('close');");
            //sb.AppendLine("            $(\"#" + gvFiles.ClientID + " .version[data-uid='\" + result.OrigFileVersionId + \"']\").text(result.Version);");
            //sb.AppendLine("            $(\"#" + gvFiles.ClientID + " .filesize[data-uid='\" + result.FileId + \"']\").text(result.Filesize);");
            //sb.AppendLine("            $(\"#" + gvFiles.ClientID + " .version[data-uid='\" + result.OrigfileVersionId + \"']\").attr('data-uid', result.NewFileVersionId);");
            //sb.AppendLine("            $(\"#" + gvFiles.ClientID + " .document[data-uid='\" + result.FileId + \"']\").prop('href', result.DocumentUrl);");
            sb.AppendLine("            $(\"#" + gvFiles.ClientID + " .history[data-uid='\" + result.FileId + \"']\").click();");
            sb.AppendLine("          } else {");
            sb.AppendLine("            $('#" + gvHistory.ClientID + "').find('td').filter(function() {");
            sb.AppendLine("              return $(this).index() === 1;");
            sb.AppendLine("            }).sortElements(function(a, b) {");
            sb.AppendLine("              return Number($([a]).attr('data-version')) > Number($([b]).attr('data-version')) ? -1 : 1;");
            sb.AppendLine("            }, function() {");
            sb.AppendLine("              // parentNode is the element we want to move");
            sb.AppendLine("              return this.parentNode;");
            sb.AppendLine("            });");
            sb.AppendLine("            $(\"#" + gvHistory.ClientID + " tr\").css(\"background-color\", function(index) {");
            sb.AppendLine("                return index === 0 ? '#666666' : index % 2 === 0 ? 'White' : 'Silver';");
            sb.AppendLine("            });");
            sb.AppendLine("          }");
            sb.AppendLine("        } else {");
            sb.AppendLine("          if (result.NewFile) {");
            //sb.AppendLine("            $(\"#" + gvFiles.ClientID + " .filesize[data-uid='\" + result.FileId + \"']\").text(result.Filesize);");
            //sb.AppendLine("            $(\"#" + gvFiles.ClientID + " .version[data-uid='\" + fileVersionId + \"']\").attr('data-uid', result.NewFileVersionId);");
            //sb.AppendLine("            $(\"#" + gvFiles.ClientID + " .document[data-uid='\" + result.FileId + \"']\").prop('href', result.DocumentUrl);");
            sb.AppendLine("            " + Page.ClientScript.GetPostBackEventReference(lnkReload, String.Empty) + ";");
            sb.AppendLine("          } else {");
            sb.AppendLine("            $(\"#" + gvFiles.ClientID + " .version[data-uid='\" + fileVersionId + \"']\").text(result.Version);");
            sb.AppendLine("          }");
            sb.AppendLine("        }");
            sb.AppendLine("      } else {");
            sb.AppendLine("        $('#" + pnlVersionMsg.ClientID + "').text('That version number already exists!'); $('#" + pnlVersionMsg.ClientID + "').show(); setTimeout(function () { $('#" + pnlVersionMsg.ClientID + "').fadeOut(\"slow\", function () { $('#" + pnlVersionMsg.ClientID + "').hide(); }); }, 2000);");
            sb.AppendLine("      }");
            sb.AppendLine("    },");
            sb.AppendLine("    error: function(jqXHR, exception) {");
            sb.AppendLine("      var msg = '';");
            sb.AppendLine("      if (jqXHR.status === 0) {");
            sb.AppendLine("        msg = 'Not connect.<br/> Verify Network.';");
            sb.AppendLine("      } else if (jqXHR.status == 404) {");
            sb.AppendLine("        msg = 'Requested page not found. [404]';");
            sb.AppendLine("      } else if (jqXHR.status == 500) {");
            sb.AppendLine("        msg = 'Internal Server Error [500].';");
            sb.AppendLine("      } else if (exception === 'parsererror') {");
            sb.AppendLine("        msg = 'Requested JSON parse failed.';");
            sb.AppendLine("      } else if (exception === 'timeout') {");
            sb.AppendLine("        msg = 'Time out error.';");
            sb.AppendLine("      } else if (exception === 'abort') {");
            sb.AppendLine("        msg = 'Ajax request aborted.';");
            sb.AppendLine("      } else {");
            sb.AppendLine("        msg = 'Uncaught Error.<br/>' + jqXHR.responseText;");
            sb.AppendLine("      }");
            sb.AppendLine("      $('#" + pnlVersionMsg.ClientID + "').html(msg); $('#" + pnlVersionMsg.ClientID + "').show(); setTimeout(function () { $('#" + pnlVersionMsg.ClientID + "').fadeOut(\"slow\", function () { $('#" + pnlVersionMsg.ClientID + "').hide(); }); }, 2000);");
            sb.AppendLine("    }");
            sb.AppendLine("  });");
            sb.AppendLine("}");
            return sb.ToString();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl scriptConfirm = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("JQueryConfirmScriptJS");
            if (scriptConfirm == null)
            {
                scriptConfirm = new System.Web.UI.HtmlControls.HtmlGenericControl("script")
                {
                    ID = "JQueryConfirmScriptJS"
                };
                scriptConfirm.Attributes.Add("language", "javascript");
                scriptConfirm.Attributes.Add("type", "text/javascript");
                scriptConfirm.Attributes.Add("src", ControlPath + "Scripts/jquery-confirm.js");
                this.Page.Header.Controls.Add(scriptConfirm);
            }
            System.Web.UI.HtmlControls.HtmlGenericControl cssConfirm = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("JQueryConfirmScriptCSS");
            if (cssConfirm == null)
            {
                cssConfirm = new System.Web.UI.HtmlControls.HtmlGenericControl("link")
                {
                    ID = "JQueryConfirmScriptCSS"
                };
                cssConfirm.Attributes.Add("type", "text/css");
                cssConfirm.Attributes.Add("rel", "stylesheet");
                cssConfirm.Attributes.Add("href", ControlPath + "Scripts/jquery-confirm.css");
                this.Page.Header.Controls.Add(cssConfirm);
            }
            System.Web.UI.HtmlControls.HtmlGenericControl script = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("ComponentScriptSortElements");
            if (script == null)
            {
                script = new System.Web.UI.HtmlControls.HtmlGenericControl("script")
                {
                    ID = "ComponentScriptSortElements"
                };
                script.Attributes.Add("language", "javascript");
                script.Attributes.Add("type", "text/javascript");
                script.Attributes.Add("src", ControlPath + "Scripts/sortElements.js");
                this.Page.Header.Controls.Add(script);
            }
            System.Web.UI.HtmlControls.HtmlGenericControl script2 = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("ComponentScriptBlockUI");
            if (script2 == null)
            {
                script2 = new System.Web.UI.HtmlControls.HtmlGenericControl("script")
                {
                    ID = "ComponentScriptBlockUI"
                };
                script2.Attributes.Add("language", "javascript");
                script2.Attributes.Add("type", "text/javascript");
                script2.Attributes.Add("src", ControlPath + "Scripts/jquery.blockUI.js");
                this.Page.Header.Controls.Add(script2);
            }
            System.Web.UI.HtmlControls.HtmlGenericControl literal = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("ComponentScriptDocumentControl");
            if (literal == null)
            {
                literal = new System.Web.UI.HtmlControls.HtmlGenericControl("script")
                {
                    ID = "ComponentScriptDocumentControl"
                };
                literal.Attributes.Add("language", "javascript");
                literal.Attributes.Add("type", "text/javascript");
                literal.InnerHtml = GenerateScript();
                this.Page.Header.Controls.Add(literal);
            }
            System.Web.UI.HtmlControls.HtmlGenericControl css = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("ComponentCSSDocumentControl");
            if (css == null)
            {
                css = new System.Web.UI.HtmlControls.HtmlGenericControl("style")
                {
                    ID = "ComponentCSSDocumentControl"
                };
                css.Attributes.Add("type", "text/css");
                css.InnerHtml = Generic.ToggleButtonCssString(LocalizeString("No"), LocalizeString("Yes"), new Unit("100px"), System.Drawing.ColorTranslator.FromHtml("#" + Theme)) + Generic.ToggleButtonCssString("Inactive", "Active", new Unit("120px"), ".itemStatus", System.Drawing.ColorTranslator.FromHtml("#" + Theme));
                this.Page.Header.Controls.Add(css);
            }
            System.Web.UI.HtmlControls.HtmlGenericControl css2 = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("ComponentStyleAutoComplete");
            if (css2 == null)
            {
                css2 = new System.Web.UI.HtmlControls.HtmlGenericControl("link")
                {
                    ID = "ComponentStyleAutoComplete"
                };
                css2.Attributes.Add("type", "text/css");
                css2.Attributes.Add("rel", "stylesheet");
                css2.Attributes.Add("href", ControlPath + "Scripts/jquery.auto-complete.css");
                this.Page.Header.Controls.Add(css2);
            }
            System.Web.UI.HtmlControls.HtmlGenericControl script3 = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("ComponentScriptAutoComplete");
            if (script3 == null)
            {
                script3 = new System.Web.UI.HtmlControls.HtmlGenericControl("script")
                {
                    ID = "ComponentScriptAutoComplete"
                };
                script3.Attributes.Add("language", "javascript");
                script3.Attributes.Add("type", "text/javascript");
                script3.Attributes.Add("src", ControlPath + "Scripts/jquery.auto-complete.js");
                this.Page.Header.Controls.Add(script3);
            }
            System.Web.UI.HtmlControls.HtmlGenericControl script4 = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("ComponentScriptBlockUI");
            if (script4 == null)
            {
                script4 = new System.Web.UI.HtmlControls.HtmlGenericControl("script")
                {
                    ID = "ComponentScriptBlockUI"
                };
                script4.Attributes.Add("language", "javascript");
                script4.Attributes.Add("type", "text/javascript");
                script4.Attributes.Add("src", ControlPath + "Scripts/jquery.blockUI.js");
                this.Page.Header.Controls.Add(script4);
            }
            System.Web.UI.HtmlControls.HtmlGenericControl literal2 = (System.Web.UI.HtmlControls.HtmlGenericControl)Page.Header.FindControl("ComponentScriptDMS");
            if (literal2 == null)
            {
                literal2 = new System.Web.UI.HtmlControls.HtmlGenericControl("script")
                {
                    ID = "ComponentScriptDMS"
                };
                literal.Attributes.Add("language", "javascript");
                literal.Attributes.Add("type", "text/javascript");
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine("var isBlockingScreen = true;");
                sb.AppendLine("function showBlockingScreen(message) {");
                sb.AppendLine("  if (!isBlockingScreen) {");
                sb.AppendLine("    isBlockingScreen = true;");
                sb.AppendLine("    if (message) {");
                sb.AppendLine("      $.blockUI({ message: '<h3 class=\"uiMessage\"><img src=\"" + ControlPath + "/Images/loading.gif\" /> ' + message + '...</h2>' });");
                sb.AppendLine("    } else {");
                sb.AppendLine("      $('.se-pre-con').show();");
                sb.AppendLine("    }");
                sb.AppendLine("  }");
                sb.AppendLine("}");
                sb.AppendLine("function hideBlockingScreen() {");
                sb.AppendLine("  //if (isBlockingScreen) {");
                sb.AppendLine("    // Animate loader off screen");
                sb.AppendLine("    isBlockingScreen = false;");
                sb.AppendLine("    $('.se-pre-con').fadeOut('fast');");
                sb.AppendLine("    $.unblockUI();");
                sb.AppendLine("  //}");
                sb.AppendLine("}");
                sb.AppendLine("$(window).ready(function() {");
                sb.AppendLine("  hideBlockingScreen();");
                sb.AppendLine("});");
                sb.AppendLine("$(window).load(function() {");
                sb.AppendLine("  //hideBlockingScreen();");
                sb.AppendLine("});");
                sb.AppendLine("$(window).unload(function() {");
                sb.AppendLine("  //showBlockingScreen();");
                sb.AppendLine("});");
                sb.AppendLine("var ignore_onbeforeunload = false;");
                sb.AppendLine("window.addEventListener('beforeunload', function(event) {");
                sb.AppendLine("  if (!ignore_onbeforeunload) {");
                sb.AppendLine("    showBlockingScreen();");
                sb.AppendLine("  }");
                sb.AppendLine("  ignore_onbeforeunload = false;");
                sb.AppendLine("});");
                sb.AppendLine("jQuery(document).ready(function() {");
                sb.AppendLine("  var prm = Sys.WebForms.PageRequestManager.getInstance();");
                sb.AppendLine("  prm.add_endRequest(MyEndRequest);");
                sb.AppendLine("  initDocumentListJavascript();");
                sb.AppendLine("  $(\".ui-autocomplete\").wrap('<div class=\"dms\" />');");
                sb.AppendLine("});");
                sb.AppendLine("function MyEndRequest(sender, args) {");
                sb.AppendLine("  initDocumentListJavascript();");
                sb.AppendLine("  hideBlockingScreen();");
                sb.AppendLine("  if ($('#" + hidFileDeleteStatus.ClientID + "').val() === 'Started') {");
                sb.AppendLine("    $('#" + hidFileDeleteStatus.ClientID + "').val('Running');");
                sb.AppendLine("    $('#" + hidFilesDeleted.ClientID + "').val('0')");
                sb.AppendLine("    $('#progress').text('0%');");
                sb.AppendLine("    $('#" + progressBar.ClientID + "').width($('#progress').text());");
                sb.AppendLine("    doPolling($('#" + hidProcessName.ClientID + "').val());");
                sb.AppendLine("  } else if ($('#" + hidFileDeleteStatus.ClientID + "').val() === 'Finished') {");
                sb.AppendLine("    $.alert({ title: '" + LocalizeString("DeleteAll") + "', content: $('#" + hidFilesDeleted.ClientID + "').val() + ' Document(s) Deleted.' });");
                //sb.AppendLine("    $(\"<div title='Delete All'><div style='padding: 10px; text-align: center;'>\" + $('#" + hidFilesDeleted.ClientID + "').val() + \" Document(s) Deleted.</div></div>\").dialog({buttons: [{text:'OK', click: function() { $(this).dialog('close');}}]});");
                sb.AppendLine("    $('#" + hidFileDeleteStatus.ClientID + "').val('Idle');");
                sb.AppendLine("  }");
                sb.AppendLine("}");
                sb.AppendLine("function doPolling(processName) {");
                sb.AppendLine("  $.ajax({");
                sb.AppendLine("    url: \"" + ControlPath + "DMSController.asmx/GetDeleteAllProgress\",");
                sb.AppendLine("    type: \"POST\",");
                sb.AppendLine("    dataType: \"json\",");
                sb.AppendLine("    data: { processName: processName },");
                sb.AppendLine("    success: function (result) {");
                sb.AppendLine("      $('#" + hidFilesDeleted.ClientID + "').val(result.FilesProcessed)");
                sb.AppendLine("      $('#progress').text(result.Progress + '%');");
                sb.AppendLine("      $('#" + progressBar.ClientID + "').width($('#progress').text());");
                sb.AppendLine("      if (parseInt(result.Progress, 10) < 100) {");
                sb.AppendLine("        setTimeout(function() { doPolling(processName); }, 250);");
                sb.AppendLine("      } else {");
                sb.AppendLine("        var window = $find('" + deleteAllWindow.ClientID + "');");
                sb.AppendLine("        window.close();");
                sb.AppendLine("         " + Page.ClientScript.GetPostBackEventReference(lnkFinish, String.Empty) + ";");
                sb.AppendLine("      }");
                sb.AppendLine("    }");
                sb.AppendLine("  });");
                sb.AppendLine("}");
                sb.AppendLine("function initDocumentListJavascript() {");
                sb.AppendLine("  $('a[href^=mailto]').on('click', function() {");
                sb.AppendLine("    ignore_onbeforeunload = true;");
                sb.AppendLine("  });");
                sb.AppendLine("  $('#" + tbKeywords.ClientID + "').autoComplete({");
                sb.AppendLine("    source: function(term, response) { $.getJSON('" + ControlPath + "SearchTerms.ashx', { q: term, pid: " + PortalId.ToString() + ", mid: " + TabModuleId.ToString() + ", cid: " + (ddCategory.Items.Count == 1 ? ddCategory.SelectedValue : "$('#" + ddCategory.ClientID + "').val()") + ", p: true, uid: " + UserId.ToString() + ", a: true, u: " + ddOwner.SelectedValue + " }, function(data) { response(data); }); },");
                sb.AppendLine("    cache: false,");
                sb.AppendLine("    minChars: 1,");
                sb.AppendLine("    onSelect: function(event, term, item) {");
                sb.AppendLine("      $('#" + tbKeywords.ClientID + "').val(term);");
                //sb.AppendLine("      $('#" + btnSearch.ClientID + "').click();");
                sb.AppendLine("        " + Page.ClientScript.GetPostBackEventReference(btnSearch, String.Empty) + ";");
                sb.AppendLine("    }");
                sb.AppendLine("  }).keyup(function(e) {");
                sb.AppendLine("    if(e.which === 13) {");
                sb.AppendLine("      e.stopImmediatePropagation();");
                sb.AppendLine("      e.preventDefault();");
                sb.AppendLine("      $('#" + tbKeywords.ClientID + "').autoComplete('close');");
                //sb.AppendLine("      $('#" + btnSearch.ClientID + "').click();");
                sb.AppendLine("        " + Page.ClientScript.GetPostBackEventReference(btnSearch, String.Empty) + ";");
                sb.AppendLine("    }");
                sb.AppendLine("  });");
                sb.AppendLine("  $(\"#" + btnCancelChange.ClientID + "\").click(function(e) {");
                sb.AppendLine("    e.preventDefault();");
                sb.AppendLine("    $('#changeOwnershipDialog').dialog('close');");
                sb.AppendLine("  });");

                sb.AppendLine("  $(\"#" + exportCommandButton.ClientID + "\").click(function(e) {");
                sb.AppendLine("    e.preventDefault();");
                sb.AppendLine("    window.open('" + ControlPath + "ExportToExcel.ashx', '_blank');");
                sb.AppendLine("    hideBlockingScreen();");
                sb.AppendLine("  });");

                sb.AppendLine("  $(\"#" + changeOwnershipCommandButton.ClientID + "\").click(function(e) {");
                sb.AppendLine("    e.preventDefault();");
                sb.AppendLine("    $('#" + ddNewOwner.ClientID + "').prop('selectedIndex', 0);");
                sb.AppendLine("    $('#" + ddNewGroup.ClientID + "').prop('selectedIndex', 0);");
                sb.AppendLine("    $('#" + cbIsGroupOwner2.ClientID + "').prop('checked', false);");
                sb.AppendLine("    $('#" + pnlNewOwner.ClientID + "').show();");
                sb.AppendLine("    $('#" + pnlNewGroup.ClientID + "').hide();");
                sb.AppendLine("    $('#" + ddCurrentOwner.ClientID + "').prop('selectedIndex', 0);");
                sb.AppendLine("    $('#" + ddCurrentGroup.ClientID + "').prop('selectedIndex', 0);");
                sb.AppendLine("    $('#" + cbIsGroupOwner3.ClientID + "').prop('checked', false);");
                sb.AppendLine("    $('#" + pnlCurrentOwner.ClientID + "').show();");
                sb.AppendLine("    $('#" + pnlCurrentGroup.ClientID + "').hide();");
                sb.AppendLine("    $('#changeOwnershipDialog .FormInstructions').hide();");
                sb.AppendLine("    $('#changeOwnershipDialog').dialog('open');");
                sb.AppendLine("    $('#changeOwnership-content').scrollTop(0);");
                sb.AppendLine("  });");
                sb.AppendLine("  var form = $('#changeOwnershipDialog').dialog({");
                sb.AppendLine("    autoOpen: false,");
                sb.AppendLine("    bgiframe: true,");
                sb.AppendLine("    modal: true,");
                sb.AppendLine("    width: 485,");
                sb.AppendLine("    height: 400,");
                sb.AppendLine("    appendTo: 'form',");
                sb.AppendLine("    dialogClass: 'dialog',");
                sb.AppendLine("     resizable: false,");
                sb.AppendLine("    title: '" + LocalizeString("ChangeOwnership") + "',");
                sb.AppendLine("    closeOnEsacpe: true,");
                sb.AppendLine("    Cancel: function () {");
                sb.AppendLine("      $(this).dialog('close');");
                sb.AppendLine("    }");
                sb.AppendLine("  });");
                sb.AppendLine("}");
                literal2.InnerHtml = sb.ToString();
                this.Page.Header.Controls.Add(literal2);
            }
        }

        public class CategoryTemplate : System.Web.UI.ITemplate
        {
            System.Web.UI.WebControls.ListItemType templateType;
            string categoryName = String.Empty;

            public CategoryTemplate(string name, System.Web.UI.WebControls.ListItemType type)
            {
                categoryName = name;
                templateType = type;
            }

            public void InstantiateIn(System.Web.UI.Control container)
            {
                Label lblCategoryName = new Label();
                lblCategoryName.ID = "lbl" + Generic.RemoveSpecialCharacters(categoryName).Replace(" ", "_");
                lblCategoryName.Text = categoryName;
                container.Controls.Add(lblCategoryName);
            }
        }

        public void Item_DataBinding(object sender, System.EventArgs e)
        {
            PlaceHolder ph = (PlaceHolder)sender;
            GridViewRow ri = (GridViewRow)ph.NamingContainer;
            List<Category> categories = DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId);
            foreach (Category category in categories)
            {
                Label lbl = ((Label)ph.FindControl("lbl" + Generic.RemoveSpecialCharacters(category.CategoryName).Replace(" ", "_")));
                if (lbl != null)
                {
                    if (DataBinder.Eval(ri.DataItem, category.CategoryName).GetType() == typeof(string))
                    {
                        lbl.Text = (string)DataBinder.Eval(ri.DataItem, category.CategoryName);
                    }
                    else
                    {
                        lbl.Text = "No";
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if(!IsDMSUser())
                {
                    base.Response.Redirect(_navigationManager.NavigateURL(), true);
                }
                deleteAllWindow.IconUrl = ControlPath + "Images/icons/DeleteIcon1_16px.gif";
                deleteAllWindow.VisibleOnPageLoad = false;
                hidFileDeleteStatus.Value = "Idle";
                lblInstructions.Visible = ShowInstructions;
                lblInstructions.Text = Instructions;
                history.Value = "0";
                hidFileVersionId.Value = "0";
                letterFilter.ForeColor = gvPackets.HeaderStyle.BackColor = gvHistory.HeaderStyle.BackColor = gvFiles.HeaderStyle.BackColor = gv.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#" + Theme);
                tbURL.Attributes.Add("onkeypress", "return newUrlKeyPress(event)");
                tbDocumentDetails.Attributes.Add("onfocus", "this.rows=8;");
                tbDocumentDetails.Attributes.Add("onblur", "this.rows=1;");
                tbAdminComments.Attributes.Add("onfocus", "this.rows=8;");
                tbAdminComments.Attributes.Add("onblur", "this.rows=1;");
                changeOwnershipCommandButton.Visible = delAllCommandButton.Visible = IsAdmin();
                progressBar.Style["background-color"] = "#" + Theme;
                List<Category> categories = DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId);
                if (!IsPostBack)
                {
                    btnAddTag.Text = LocalizeString(btnAddTag.ID);
                    btnBack.Text = LocalizeString(btnBack.ID);
                    btnBack2.Text = LocalizeString(btnBack.ID);
                    btnCancel.Text = LocalizeString(btnCancel.ID);
                    btnCancel2.Text = LocalizeString(btnCancel.ID);
                    btnDelete.Text = LocalizeString(btnDelete.ID);
                    btnEdit.Text = LocalizeString(btnEdit.ID);
                    btnReloadSecurityRoles.Text = LocalizeString(btnReloadSecurityRoles.ID);
                    btnSave.Text = LocalizeString(btnSave.ID);
                    btnSave2.Text = LocalizeString(btnSave.ID);
                    btnSaveLink.Text = LocalizeString(btnSaveLink.ID);
                    btnSaveChange.Text = LocalizeString(btnSaveChange.ID);
                    btnSaveFile.Text = LocalizeString(btnSaveFile.ID);
                    btnSearch.Text = LocalizeString(btnSearch.ID);
                    lnkReload.Text = LocalizeString(lnkReload.ID);
                    btnCancelChange.Text = LocalizeString("Cancel");
                    gvPackets.EmptyDataText = LocalizeString("NoPackets");
                    gvFiles.EmptyDataText = LocalizeString("NoFiles");
                    gvHistory.EmptyDataText = LocalizeString("NoHistory");
                    gv.EmptyDataText = LocalizeString("NoDocuments");
                    gv.PageSize = PageSize;
                    deleteAllWindow.Title = deleteAllWindow.ToolTip = LocalizeString("DeletingAll");
                    searchBox.Style["background"] = string.Format("url({0}Images/results-background-{1}.jpg) no-repeat", ControlPath, Theme);
                    lblCategory.Text = CategoryName;
                    //pnlDetails.Style.Add("display", "none");
                    //pnlGrid.Style.Remove("display");
                    BindDropDowns();
                    if (Request.QueryString["uid"] != null && Generic.IsNumber(Request.QueryString["uid"]) && IsAdmin())
                    {
                        ddOwner.SelectedValue = Request.QueryString["uid"];
                    }
                    foreach (Category category in categories)
                    {
                        TemplateField field = new TemplateField();
                        field.HeaderText = category.CategoryName + " <img src='" + ControlPath + "Images/sortneutral.png' border='0' alt='Sort by " + category.CategoryName + "' />";
                        field.HeaderStyle.Wrap = false;
                        field.SortExpression = category.CategoryName;
                        field.ItemStyle.Width = new Unit("100px");
                        field.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                        field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                        field.ItemTemplate = new CategoryTemplate(category.CategoryName, ListItemType.Item);
                        gv.Columns.Add(field);
                    }
                    pnlAdmin.Visible = IsDMSUser();
                    pnlOwner.Visible = IsAdmin();
                    changeOwnershipCommandButton.Visible = IsAdmin();
                    if(Request.QueryString["id"] != null && Generic.IsNumber(Request.QueryString["id"]))
                    {
                        if(Session["gv"] != null)
                        {
                            Session.Remove("gv");
                        }
                        ShowDetails(Convert.ToInt32(Request.QueryString["id"]));
                    }
                    else
                    {
                        BindData();
                    }
                }
                else
                {
                    foreach (Category category in categories)
                    {
                        foreach (DataControlField field in gv.Columns)
                        {
                            if (field.GetType() == typeof(TemplateField))
                            {
                                TemplateField templateField = field as TemplateField;
                                if (templateField.SortExpression == category.CategoryName)
                                {
                                    templateField.ItemTemplate = new CategoryTemplate(category.CategoryName, ListItemType.Item);
                                }
                            }
                        }
                    }
                    Generic.ApplyPaging(gv);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindDropDowns()
        {
            ddlSecurityRole.DataSource = DotNetNuke.Security.Roles.RoleController.Instance.GetRoles(PortalId);
            ddlSecurityRole.DataBind();
            ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Users", "-1"));
            ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Superusers", "-2"));
            ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Unauthenticated Users", "-3"));
            ddCategory.DataSource = DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId);
            ddCategory.DataBind();
            ddCategory.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All", "0"));
            ddCategory.SelectedIndex = 0;
            ddOwner.DataSource = Components.UserController.GetUsers(UserRole, PortalId);
            ddOwner.DataBind();
            ddOwner.Items.Insert(0, new ListItem(LocalizeString("Owner2"), "0"));
            if (IsAdmin())
            {
                ddOwner.SelectedIndex = 0;
            }
            else
            {
                ddOwner.SelectedIndex = ddOwner.Items.IndexOf(ddOwner.Items.FindByValue(UserId.ToString()));
            }

            ddCurrentOwner.DataSource = Components.UserController.GetUsers(UserRole, PortalId);
            ddCurrentOwner.DataBind();
            ddCurrentOwner.Items.Insert(0, new ListItem(LocalizeString("CurrentOwner"), "0"));
            ddCurrentOwner.SelectedIndex = 0;

            ddCurrentGroup.DataSource = DotNetNuke.Security.Roles.RoleController.Instance.GetRoles(PortalId);
            ddCurrentGroup.DataBind();
            ddCurrentGroup.Items.Insert(0, new ListItem(LocalizeString("CurrentGroup"), "0"));
            ddCurrentGroup.SelectedIndex = 0;

            ddNewOwner.DataSource = Components.UserController.GetUsers(UserRole, PortalId);
            ddNewOwner.DataBind();
            ddNewOwner.Items.Insert(0, new ListItem(LocalizeString("NewOwner"), "0"));
            ddNewOwner.SelectedIndex = 0;

            ddNewGroup.DataSource = DotNetNuke.Security.Roles.RoleController.Instance.GetRoles(PortalId);
            ddNewGroup.DataBind();
            ddNewGroup.Items.Insert(0, new ListItem(LocalizeString("NewGroup"), "0"));
            ddNewGroup.SelectedIndex = 0;

            ddOwner2.DataSource = Components.UserController.GetUsers(UserRole, PortalId);
            ddOwner2.DataBind();
            ddOwner2.Items.Insert(0, new ListItem(LocalizeString("SelectOwner"), "0"));
            ddOwner2.SelectedIndex = 0;

            ddGroup.DataSource = DotNetNuke.Security.Roles.RoleController.Instance.GetRoles(PortalId);
            ddGroup.DataBind();
            ddGroup.Items.Insert(0, new ListItem(LocalizeString("SelectGroup"), "0"));
            ddGroup.SelectedIndex = 0;

            ddTags.DataSource = Components.DocumentController.GetAllTags(PortalId, PortalWideRepository ? 0 : TabModuleId);
            ddTags.DataBind();
            ddTags.Items.Insert(0, new ListItem(LocalizeString("SelectExistingTag"), ""));
            ddTags.SelectedIndex = 0;
        }

        protected void ddCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateDataTable(true);
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                return new ModuleActionCollection()
                {
                    {
                        GetNextActionID(), Localization.GetString("ActivityReport", LocalResourceFile), "", "", ControlPath + "Images/report.png",
                        EditUrl("ActivityReport"), false, SecurityAccessLevel.Admin, true, false
                    },
                };
            }
        }

        protected void ViewDocumentsCommandButtonClicked(object sender, EventArgs e)
        {
            HttpResponse response = base.Response;
            string[] strArrays = new string[1];
            int moduleId = base.ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            response.Redirect(GetDocumentsLink(strArrays));
        }

        public bool IsDMSUser()
        {
            if ((new ModuleSecurity((new ModuleController()).GetTabModule(this.TabModuleId))).HasEditPermissions)
            {
                return true;
            }
            if (UserInfo != null)
            {
                DotNetNuke.Security.Roles.RoleInfo userRole = DotNetNuke.Security.Roles.RoleController.Instance.GetRoleById(PortalId, UserRole);
                if (userRole != null)
                {
                    return UserInfo.IsInRole(userRole.RoleName);
                }
            }
            return false;
        }

        public bool IsAdmin()
        {
            if ((new ModuleSecurity((new ModuleController()).GetTabModule(this.TabModuleId))).HasEditPermissions)
            {
                return true;
            }
            return false;
        }

        protected void gv_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gv.PageIndex = e.NewPageIndex;
            CreateDataTable(false);
        }

        protected void gv_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (e.SortExpression != SortColumn)
            {
                SortColumn = e.SortExpression;
                SortDirection = System.Web.UI.WebControls.SortDirection.Ascending;
            }
            else
            {
                if (SortDirection == SortDirection.Ascending)
                {
                    SortDirection = SortDirection.Descending;
                }
                else
                {
                    SortDirection = SortDirection.Ascending;
                }
            }
            System.Data.DataView dataView = (System.Data.DataView)Session["gv"];
            dataView.Sort = SortColumn + " " + (SortDirection == SortDirection.Ascending ? "ASC" : "DESC");
            gv.PageIndex = 0;
            gv.DataSource = dataView;
            gv.DataBind();
            Session["gv"] = dataView;
        }

        protected void letterFilter_Click(object sender, LetterFilter.LetterFilterEventArgs e)
        {
            AddFilter((System.Data.DataView)Session["gv"]);
        }

        protected void lnkDocumentName_Command(object sender, CommandEventArgs e)
        {
            ShowDetails(Convert.ToInt32(e.CommandArgument.ToString()));
        }

        private void ShowDetails(int documentId)
        {
            DocumentID = documentId;
            ViewMode = ViewMode.Details;
            //pnlDetails.Style.Remove("display");
            //pnlGrid.Style.Add("display", "none");
            pnlDetails.Visible = true;
            pnlGrid.Visible = false;
        }

        protected void gv_DataBound(object sender, EventArgs e)
        {
            Generic.ApplyPaging(gv);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            letterFilter.Filter = "All";
            CreateDataTable(true);
        }

        protected void ddOwner_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateDataTable(true, true);
        }

        private void BindData()
        {
            CreateDataTable(true);
        }

        public System.Data.DataView CreateDataTable(bool bReload, bool useRaw = false)
        {
            System.Data.DataView dataView = (System.Data.DataView)Session["gv"];
            if (dataView == null || bReload)
            {
                List<Gafware.Modules.DMS.Components.DocumentView> docs = null;
                if (Session["gvRaw"] != null && useRaw)
                {
                    docs = (List<Components.DocumentView>)Session["gvRaw"];
                }
                else
                {
                    docs = DocumentController.GetDocumentList(Convert.ToInt32(ddCategory.SelectedValue), tbKeywords.Text.Trim(), UserId, PortalId, PortalWideRepository ? 0 : TabModuleId);
                    Session["gvRaw"] = docs;
                }
                if (Convert.ToInt32(ddOwner.SelectedValue) > 0)
                {
                    //DotNetNuke.Entities.Users.UserInfo user = DotNetNuke.Entities.Users.UserController.GetUserById(PortalId, Convert.ToInt32(ddOwner.SelectedValue));
                    //docs = docs.FindAll(p => (!p.IsGroupOwner && p.CreatedByUserID == Convert.ToInt32(ddOwner.SelectedValue)) || (p.IsGroupOwner && user != null && user.IsInRole(Components.UserController.GetRoleById(PortalId, p.CreatedByUserID).RoleName)));
                    int userId = Convert.ToInt32(ddOwner.SelectedValue);
                    docs = docs.FindAll(p => (!p.IsGroupOwner && p.CreatedByUserID == Convert.ToInt32(ddOwner.SelectedValue)) || (p.IsGroupOwner && DocumentController.UserIsInRole(userId, p.CreatedByUserID)));
                }
                System.Data.DataTable dtResult = Generic.DocumentListToDataTable(docs, PortalId, PortalWideRepository ? 0 : TabModuleId);
                dtResult.Columns.Remove("CreatedByUserId");
                dtResult.Columns.Remove("IsGroupOwner");
                dtResult.Columns.Remove("KeyID");
                dtResult.Columns.Remove("ContentItemId");
                dtResult.Columns.Remove("Content");
                dtResult.Columns.Remove("ContentKey");
                dtResult.Columns.Remove("ContentTypeId");
                dtResult.Columns.Remove("Indexed");
                dtResult.Columns.Remove("ModuleID");
                dtResult.Columns.Remove("TabID");
                dtResult.Columns.Remove("ContentTitle");
                dtResult.Columns.Remove("StateID");
                dtResult.Columns.Remove("LastModifiedByUserID");
                dataView = new System.Data.DataView(dtResult);
                gv.PageIndex = 0;
            }
            dataView.Sort = SortColumn + " " + (SortDirection == SortDirection.Ascending ? "ASC" : "DESC");
            AddFilter(dataView);
            return dataView;
        }

        private void AddFilter(System.Data.DataView dataView)
        {
            if (!letterFilter.Filter.Equals("All"))
            {
                if (letterFilter.Filter.Equals("#"))
                {
                    dataView.RowFilter = "DocumentName LIKE '1%' OR DocumentName LIKE '2%' OR DocumentName LIKE '3%' OR DocumentName LIKE '4%' OR DocumentName LIKE '5%' OR DocumentName LIKE '6%' OR DocumentName LIKE '7%' OR DocumentName LIKE '8%' OR DocumentName LIKE '9%' OR DocumentName LIKE '0%'";
                }
                else
                {
                    dataView.RowFilter = "DocumentName LIKE '" + letterFilter.Filter + "%'";
                }
                gv.EmptyDataText = LocalizeString("NoDocumentsFor") + " '" + letterFilter.Filter + "'.</strong>";
            }
            else
            {
                dataView.RowFilter = String.Empty;
                gv.EmptyDataText = LocalizeString("NoDocuments");
            }
            gv.DataSource = dataView;
            gv.DataBind();
            Session["gv"] = dataView;
        }

        protected void newDocumentCommandButton_Click(object sender, EventArgs e)
        {
            DocumentID = 0;
            ViewMode = ViewMode.Edit;
            //pnlDetails.Style.Remove("display");
            //pnlGrid.Style.Add("display", "none");
            pnlDetails.Visible = true;
            pnlGrid.Visible = false;
        }

        protected void backCommandButton_Click(object sender, EventArgs e)
        {
            base.Response.Redirect(_navigationManager.NavigateURL(), true);
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                System.Data.DataRowView row = (System.Data.DataRowView)e.Row.DataItem;
                List<Components.DocumentCategory> docCategories = Components.DocumentController.GetAllCategoriesForDocument((int)row["DocumentId"]);
                
                List<Category> categories = DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId);
                foreach (Category category in categories)
                {
                    Label lbl = ((Label)e.Row.FindControl("lbl" + Generic.RemoveSpecialCharacters(category.CategoryName).Replace(" ", "_")));
                    if (lbl != null)
                    {
                        lbl.Text = (docCategories.FirstOrDefault(docCat => category.CategoryId == docCat.CategoryId) != null ? "Yes" : "No");
                        //if (row[Generic.RemoveSpecialCharacters(category.CategoryName).Replace(" ", "_")].GetType() == typeof(string))
                        //{
                        //    lbl.Text = (string)row[Generic.RemoveSpecialCharacters(category.CategoryName).Replace(" ", "_")];
                        //}
                        //else
                        //{
                        //    lbl.Text = "No";
                        //}
                    }
                }
                LinkButton lnk = (LinkButton)e.Row.FindControl("lnkDocumentName");
                if(lnk != null)
                {
                    lnk.ForeColor = System.Drawing.ColorTranslator.FromHtml("#" + Theme);
                }
            }
        }

        protected void btnSaveChange_Click(object sender, EventArgs e)
        {
            DocumentController.ChangeOwnership(cbIsGroupOwner3.Checked ? Convert.ToInt32(ddCurrentGroup.SelectedValue) : Convert.ToInt32(ddCurrentOwner.SelectedValue), cbIsGroupOwner3.Checked, cbIsGroupOwner2.Checked ? Convert.ToInt32(ddNewGroup.SelectedValue) : Convert.ToInt32(ddNewOwner.SelectedValue), cbIsGroupOwner2.Checked, PortalId);
            BindData();
        }

        private void BindDocData()
        {
            DotNetNuke.Entities.Users.UserInfo user = DotNetNuke.Entities.Users.UserController.Instance.GetUserById(PortalId, UserId);
            Components.Document doc = Components.DocumentController.GetDocument(DocumentID);
            DotNetNuke.Security.Roles.RoleInfo groupOwner = (doc != null && doc.DocumentId > 0 && doc.IsGroupOwner ? Components.UserController.GetRoleById(PortalId, doc.CreatedByUserID) : null);
            if (DocumentID == 0 || (doc != null && doc.DocumentId > 0 && ((!doc.IsGroupOwner && doc.CreatedByUserID == UserId) || (doc.IsGroupOwner && user.IsInRole(groupOwner.RoleName)) || user.IsInRole("Administrator") || user.IsSuperUser)))
            {
                if (doc == null)
                {
                    doc = new Components.Document();
                }
                lblActivationDate.Text = doc.ActivationDate.HasValue ? doc.ActivationDate.Value.ToString("MM/dd/yyyy") : "&nbsp;";
                lblAdminComments.Text = !String.IsNullOrEmpty(doc.AdminComments) ? doc.AdminComments : "&nbsp;";
                lblDateCreated.Text = doc.CreatedOnDate.ToString("MM/dd/yyyy");
                lblDateLastModified.Text = doc.LastModifiedOnDate.ToString("MM/dd/yyyy");
                lblDetails.Text = !String.IsNullOrEmpty(doc.DocumentDetails) ? doc.DocumentDetails : "&nbsp;";
                lblDocumentID.Text = doc.DocumentId.ToString();
                lblDocumentName.Text = !String.IsNullOrEmpty(doc.DocumentName) ? doc.DocumentName : (DocumentID == 0 && ViewMode == ViewMode.Edit ? LocalizeString("NewDocument") : "&nbsp;");
                btnDelete.OnClientClick = "confirmDelete(this, \"" + JSEncode(lblDocumentName.Text) + "\");  return false;";
                lblExpirationDate.Text = doc.ExpirationDate.HasValue ? doc.ExpirationDate.Value.ToString("MM/dd/yyyy") : "&nbsp;";
                //lblIPAddress.Text = !String.IsNullOrEmpty(doc.IPAddress) ? doc.IPAddress : "&nbsp;";
                lblIsSearchable.Text = LocalizeString(doc.IsSearchable ? "Yes" : "No");
                lblUseCategorySecurityRoles.Text = LocalizeString(doc.UseCategorySecurityRoles ? "Yes" : "No");
                lblSecurityRole.Text = (doc.SecurityRole != null ? doc.SecurityRole.RoleName : LocalizeString("Unknown"));
                lblIsPublic.Text = LocalizeString(doc.IsPublic ? "Yes" : "No");
                if (doc.IsGroupOwner)
                {
                    lblOwner.Text = doc.Group != null && !String.IsNullOrEmpty(doc.Group.RoleName) ? doc.Group.RoleName : "&nbsp;";
                }
                else
                {
                    lblOwner.Text = doc.CreatedByUser != null && !String.IsNullOrEmpty(doc.CreatedByUser.LastName) ? doc.CreatedByUser.FirstName + " " + doc.CreatedByUser.LastName : "&nbsp;";
                }
                if (doc.IsGroupOwner)
                {
                    pnlOwnerEdit2.Visible = false;
                    pnlGroupEdit.Visible = true;
                    ddGroup.SelectedIndex = ddGroup.Items.IndexOf(ddGroup.Items.FindByValue(doc.CreatedByUserID.ToString()));
                    ddOwner2.SelectedIndex = 0;
                }
                else
                {
                    pnlOwnerEdit2.Visible = true;
                    pnlGroupEdit.Visible = false;
                    ddOwner2.SelectedIndex = ddOwner2.Items.IndexOf(ddOwner2.Items.FindByValue(doc.CreatedByUserID.ToString()));
                    ddGroup.SelectedIndex = 0;
                }
                dtActivation.SelectedDate = doc.ActivationDate;
                dtExpiration.SelectedDate = doc.ExpirationDate;
                if (doc.DocumentId > 0)
                {
                    cbIsGroupOwner.Checked = doc.IsGroupOwner;
                    cbIsPublic.Checked = doc.IsPublic;
                    cbIsSearchable.Checked = doc.IsSearchable;
                    cbUseCategorySecurityRoles.Checked = doc.UseCategorySecurityRoles;
                    ddlSecurityRole.SelectedIndex = ddlSecurityRole.Items.IndexOf(ddlSecurityRole.Items.FindByValue(doc.SecurityRoleId.ToString()));
                }
                else
                {
                    cbIsGroupOwner.Checked = false;
                    cbIsPublic.Checked = true;
                    cbIsSearchable.Checked = true;
                    cbUseCategorySecurityRoles.Checked = true;
                    ddlSecurityRole.SelectedIndex = ddlSecurityRole.Items.IndexOf(ddlSecurityRole.Items.FindByValue("-1"));
                }
                pnlSecurityRole.Visible = !cbUseCategorySecurityRoles.Checked;
                tbAdminComments.Text = doc.AdminComments;
                tbDocumentDetails.Text = doc.DocumentDetails;
                tbDocumentName.Text = doc.DocumentName;
                tbTags.Entries.Clear();
                foreach (Components.DocumentTag tag in doc.Tags)
                {
                    tbTags.Entries.Add(new Telerik.Web.UI.AutoCompleteBoxEntry(tag.Tag.TagName));
                }
                gvFiles.DataSource = doc.Files;
                gvFiles.DataBind();
                gvPackets.DataSource = Components.PacketController.GetAllPacketsContainingDocument(DocumentID);
                gvPackets.DataBind();
                TagsFilesEnabled = (DocumentID != 0);
                if (DocumentID == 0)
                {
                    pnlOwnerEdit2.Visible = true;
                    pnlGroupEdit.Visible = false;
                    ddOwner2.SelectedIndex = ddOwner2.Items.IndexOf(ddOwner2.Items.FindByValue(UserId.ToString()));
                    lblOwner.Text = user.DisplayName;
                }
                pnlNotFound.Visible = false;
                pnlFound.Visible = true;
            }
            else
            {
                ViewState["DocumentID"] = 0;
                pnlNotFound.Visible = true;
                pnlFound.Visible = false;
            }

            rptCategory.DataSource = Components.DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId);
            rptCategory.DataBind();
            rptCategory.Visible = Components.DocumentController.GetAllCategories(PortalId, PortalWideRepository ? 0 : TabModuleId).Count > 1;
            string[] strArrays = new string[2];
            int moduleId = ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            strArrays[1] = string.Concat("q=", Generic.StringToHex(Generic.UrlEncode(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(String.Format("docids={0}", DocumentID)))));
            tbLinkURL.Text = GetDocumentsLink(strArrays);
            pnlLink.Visible = (DocumentID > 0);
            pnlTags.Visible = (DocumentID != 0);
            pnlBack.Visible = (DocumentID > 0);
            pnlBack2.Visible = (DocumentID == 0);
            pnlControl.Visible = (DocumentID > 0);
            pnlControl2.Visible = (DocumentID == 0 && !pnlNotFound.Visible);
            pnlFiles.Visible = (DocumentID != 0 /*&& doc.Tags.Count > 0*/);
            pnlPackets.Visible = (DocumentID != 0 && (user.IsSuperUser || user.IsInRole("Administrator")));
        }

        private string GetNaviateUrl(string filename)
        {
            string url= _navigationManager.NavigateURL(filename);
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && ForceHttps)
            {
                url = string.Format("https://{0}", url.Substring(7));
            }
            return url;
        }

        private string GetDocumentsLink(string[] strArrays)
        {
            string url = _navigationManager.NavigateURL("GetDocuments", strArrays);
            if(url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && ForceHttps)
            {
                url = string.Format("https://{0}", url.Substring(7));
            }
            return url;
        }

        private string GetPacketsLink(string[] strArrays)
        {
            string url = _navigationManager.NavigateURL("PacketsList", strArrays);
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && ForceHttps)
            {
                url = string.Format("https://{0}", url.Substring(7));
            }
            return url;
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            ViewMode = DMS.ViewMode.Edit;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            valExists.IsValid = true;
            Components.Document exists = Components.DocumentController.GetDocumentByName(tbDocumentName.Text, PortalId, PortalWideRepository ? 0 : TabModuleId);
            if (exists == null || exists.DocumentId == 0 || exists.DocumentId == DocumentID)
            {
                Components.Document doc = Components.DocumentController.GetDocument(DocumentID);
                if(doc == null)
                {
                    doc = new Document();
                    doc.Categories = new List<DocumentCategory>();
                    doc.Files = new List<Components.DMSFile>();
                    doc.Tags = new List<DocumentTag>();
                    doc.CategoriesRaw = new List<Category>();
                    doc.PortalId = PortalId;
                    doc.TabModuleId = TabModuleId;
                }
                doc.ActivationDate = dtActivation.SelectedDate;
                doc.AdminComments = tbAdminComments.Text;
                doc.IsGroupOwner = cbIsGroupOwner.Checked;
                doc.CreatedByUserID = Convert.ToInt32(cbIsGroupOwner.Checked ? ddGroup.SelectedValue : ddOwner2.SelectedValue);
                doc.DocumentDetails = tbDocumentDetails.Text;
                doc.ExpirationDate = dtExpiration.SelectedDate;
                doc.IsSearchable = cbIsSearchable.Checked;
                doc.UseCategorySecurityRoles = cbUseCategorySecurityRoles.Checked;
                doc.SecurityRoleId = Convert.ToInt32(ddlSecurityRole.SelectedValue);
                doc.IsPublic = cbIsPublic.Checked;
                doc.DocumentName = tbDocumentName.Text;
                doc.IPAddress = Generic.GetIPAddress();
                Components.DocumentController.SaveDocument(doc);
                DotNetNuke.Entities.Portals.PortalInfo portal = DotNetNuke.Entities.Portals.PortalController.Instance.GetPortal(PortalId);
                Repository repository = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                Thumbnail thumb = new Thumbnail(portal, repository, ControlPath);
                foreach (Components.DMSFile file in doc.Files)
                {
                    if (cbReplacePDFTitle2.Checked && System.IO.Path.GetExtension(file.Filename).Equals(".pdf", StringComparison.OrdinalIgnoreCase) && file.StatusId == 1)
                    {
                        FileVersion fileVersion = file.FileVersion;
                        fileVersion.LoadContents();
                        fileVersion.FileVersionId = 0;
                        fileVersion.Version++;
                        fileVersion.IPAddress = Generic.GetIPAddress();
                        fileVersion.CreatedByUserID = UserId;
                        fileVersion.IsGroupOwner = false;
                        fileVersion.CreatedOnDate = DateTime.Now;
                        Components.DocumentController.SaveFileVersion(fileVersion);
                        file.FileVersionId = fileVersion.FileVersionId;
                        fileVersion.SaveContents();
                        file.Filesize = fileVersion.Filesize;
                        Components.DocumentController.SaveFile(file);
                        try
                        {
                            Generic.ReplacePDFTitle(file, doc.DocumentName);
                        }
                        catch(Exception)
                        {
                        }
                    }
                    try
                    {
                        thumb.CreateThumbnail(Request, file);
                    }
                    catch(Exception)
                    {
                    }
                }
                doc.Files = Components.DocumentController.GetAllFilesForDocument(DocumentID);
                gvFiles.DataSource = doc.Files;
                gvFiles.DataBind();
                foreach (RepeaterItem item in rptCategory.Items)
                {
                    HiddenField hidCategoryId = (HiddenField)item.FindControl("hidCategoryId");
                    if (hidCategoryId != null)
                    {
                        CheckBox cbCategory = (CheckBox)item.FindControl("cbCategory");
                        if (cbCategory != null)
                        {
                            int categoryId = Convert.ToInt32(hidCategoryId.Value);
                            if (cbCategory.Checked)
                            {
                                if (!doc.Categories.Exists(c => c.CategoryId == categoryId))
                                {
                                    Components.DocumentCategory category = new Components.DocumentCategory();
                                    category.CategoryId = categoryId;
                                    category.DocumentId = doc.DocumentId;
                                    Components.DocumentController.SaveDocumentCategory(category);
                                    doc.Categories.Add(category);
                                }
                            }
                            else
                            {
                                if (doc.Categories.Exists(c => c.CategoryId == categoryId))
                                {
                                    Components.DocumentCategory category = doc.Categories.Find(c => c.CategoryId == categoryId);
                                    Components.DocumentController.DeleteDocumentCategory(category.DocumentCategoryId);
                                    doc.Categories.Remove(category);
                                }
                            }
                        }
                    }
                }
                cbReplacePDFTitle2.Checked = false;
                DocumentID = doc.DocumentId;
                TagsFilesEnabled = (DocumentID != 0);
                ViewMode = ViewMode.Details;
                BindDocData();
                CreateDataTable(true);
            }
            else
            {
                valExists.IsValid = false;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            TagsFilesEnabled = (DocumentID != 0);
            if (DocumentID == 0)
            {
                //pnlDetails.Style.Add("display", "none");
                //pnlGrid.Style.Remove("display");
                pnlDetails.Visible = false;
                pnlGrid.Visible = true;
                DocumentID = 0;
                CreateDataTable(false);
            }
            else
            {
                ViewMode = ViewMode.Details;
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Components.Document doc = Components.DocumentController.GetDocument(DocumentID);
            if (doc != null)
            {
                DotNetNuke.Entities.Portals.PortalSettings portal = DotNetNuke.Entities.Portals.PortalSettings.Current;
                string uploadDirectory = String.Format("{0}Files\\{1}\\{2}", portal.HomeDirectoryMapPath, TabModuleId, Generic.CreateSafeFolderName(doc.DocumentName));
                if (System.IO.Directory.Exists(uploadDirectory))
                {
                    System.IO.Directory.Delete(uploadDirectory, true);
                }
                Components.DocumentController.DeleteDocument(DocumentID);
            }
            DocumentID = 0;
            //pnlDetails.Style.Add("display", "none");
            //pnlGrid.Style.Remove("display");
            pnlDetails.Visible = false;
            pnlGrid.Visible = true;
            DocumentID = 0;
            CreateDataTable(true);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            //pnlDetails.Style.Add("display", "none");
            //pnlGrid.Style.Remove("display");
            pnlDetails.Visible = false;
            pnlGrid.Visible = true;
            DocumentID = 0;
            CreateDataTable(false);
        }

        private bool HasTag(string tag)
        {
            Telerik.Web.UI.AutoCompleteBoxEntry[] tags = new Telerik.Web.UI.AutoCompleteBoxEntry[tbTags.Entries.Count];
            tbTags.Entries.CopyTo(tags, 0);
            List<Telerik.Web.UI.AutoCompleteBoxEntry> entries = tags.ToList();
            return (entries.Find(p => p.Text.Equals(tag, StringComparison.OrdinalIgnoreCase)) != null);
        }

        protected void btnAddTag_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(tbTag.Text))
            {
                if (!HasTag(tbTag.Text))
                {
                    tbTags.Entries.Add(new Telerik.Web.UI.AutoCompleteBoxEntry(tbTag.Text));
                    AddTag(tbTag.Text);
                }
            }
            else if (ddTags.SelectedIndex > 0)
            {
                if (!HasTag(ddTags.SelectedItem.Text))
                {
                    tbTags.Entries.Add(new Telerik.Web.UI.AutoCompleteBoxEntry(ddTags.SelectedItem.Text));
                    AddTag(Convert.ToInt32(ddTags.SelectedValue), false);
                }
            }
            ddTags.SelectedIndex = 0;
            tbTag.Text = String.Empty;
        }

        private void AddTag(string tagName)
        {
            List<Components.DocumentTag> tags = Components.DocumentController.GetAllTagsForDocument(DocumentID);
            Components.DocumentTag docTag = tags.Find(p => p.Tag.TagName.Equals(tagName, StringComparison.OrdinalIgnoreCase));
            if (docTag == null)
            {
                docTag = new Components.DocumentTag();
                docTag.DocumentId = DocumentID;
                Tag tag = Components.DocumentController.GetTagByTagName(tagName, PortalId, PortalWideRepository ? 0 : TabModuleId);
                if (tag == null || tag.TagId == 0)
                {
                    tag = new Components.Tag();
                    tag.TagName = tagName;
                    tag.PortalId = PortalId;
                    tag.TabModuleId = TabModuleId;
                    Components.DocumentController.SaveTag(tag);
                    docTag.TagId = tag.TagId;
                    ddTags.DataSource = Components.DocumentController.GetAllTags(PortalId, PortalWideRepository ? 0 : TabModuleId);
                    ddTags.DataBind();
                    ddTags.Items.Insert(0, new ListItem(LocalizeString("SelectExistingTag"), ""));
                    ddTags.SelectedIndex = 0;
                }
                else
                {
                    docTag.TagId = tag.TagId;
                }
                Components.DocumentController.SaveDocumentTag(docTag);
                tags.Add(docTag);
                tbTags.Entries.Clear();
                foreach (Components.DocumentTag tag2 in tags)
                {
                    tbTags.Entries.Add(new Telerik.Web.UI.AutoCompleteBoxEntry(tag2.Tag.TagName));
                }
                pnlFiles.Visible = (DocumentID != 0 /*&& tags.Count > 0*/);
            }
        }

        private void AddTag(int tagID, bool setFocus)
        {
            List<Components.DocumentTag> tags = Components.DocumentController.GetAllTagsForDocument(DocumentID);
            Components.DocumentTag docTag = tags.Find(p => p.Tag.TagId == tagID);
            if (docTag == null)
            {
                docTag = new Components.DocumentTag();
                docTag.DocumentId = DocumentID;
                docTag.TagId = tagID;
                Components.DocumentController.SaveDocumentTag(docTag);
                tags.Add(docTag);
                tbTags.Entries.Clear();
                foreach (Components.DocumentTag tag in tags)
                {
                    tbTags.Entries.Add(new Telerik.Web.UI.AutoCompleteBoxEntry(tag.Tag.TagName));
                }
                //TelerikAjaxUtility.GetCurrentRadAjaxManager().ResponseScripts.Add("$(\"#" + pnlSaveMessage.ClientID + "\").show(); " + (setFocus ? "setCaretAtEnd(document.getElementById('" + tbTags.ClientID + "')); " : String.Empty ) + "setTimeout(function() { $(\"#" + pnlSaveMessage.ClientID + "\").fadeOut(\"slow\", function () { $(\"#" + pnlSaveMessage.ClientID + "\").hide(); }); }, 2000);");
                pnlFiles.Visible = (DocumentID != 0 /*&& tags.Count > 0*/);
            }
        }

        private void RemoveTag(string tagName)
        {
            List<Components.DocumentTag> tags = Components.DocumentController.GetAllTagsForDocument(DocumentID);
            Components.DocumentTag tag = tags.Find(p => p.Tag.TagName.Equals(tagName, StringComparison.OrdinalIgnoreCase));
            if (tag != null)
            {
                tags.Remove(tag);
                Components.DocumentController.DeleteDocumentTag(tag.DocumentTagId);
                tbTags.Entries.Clear();
                foreach (Components.DocumentTag tag2 in tags)
                {
                    tbTags.Entries.Add(new Telerik.Web.UI.AutoCompleteBoxEntry(tag2.Tag.TagName));
                }
                // TelerikAjaxUtility.GetCurrentRadAjaxManager().ResponseScripts.Add("$(\"#" + pnlSaveMessage.ClientID + "\").show(); setTimeout(function() { $(\"#" + pnlSaveMessage.ClientID + "\").fadeOut(\"slow\", function () { $(\"#" + pnlSaveMessage.ClientID + "\").hide(); }); }, 2000);");
                pnlFiles.Visible = (DocumentID != 0 /*&& tags.Count > 0*/);
            }
        }

        protected void tbTags_EntryAdded(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            AddTag(e.Entry.Text);
        }

        protected void tbTags_EntryRemoved(object sender, Telerik.Web.UI.AutoCompleteEntryEventArgs e)
        {
            RemoveTag(e.Entry.Text);
        }

        protected void gvFiles_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int fileID = Convert.ToInt32(gvFiles.DataKeys[e.RowIndex].Value.ToString());
            Components.DocumentController.DeleteFile(fileID);
            Components.DMSFile file = Components.DocumentController.GetFile(fileID);
            if (file != null)
            {
                DotNetNuke.Entities.Portals.PortalSettings portal = DotNetNuke.Entities.Portals.PortalSettings.Current;
                if (System.IO.File.Exists(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename)))
                {
                    System.IO.File.Delete(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename));
                }
            }
            List<Components.DMSFile> files = Components.DocumentController.GetAllFilesForDocument(DocumentID);
            gvFiles.DataSource = files;
            gvFiles.DataBind();
            e.Cancel = true;
        }

        protected void gvFiles_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Components.DMSFile file = (Components.DMSFile)e.Row.DataItem;
                LinkButton btn = (LinkButton)e.Row.FindControl("btnDelete");
                if (btn != null)
                {
                    btn.Attributes.Add("onMouseOver", "MM_swapImage('" + btn.ClientID + "','','" + ResolveUrl("~" + ControlPath + "Images/Icons/DeleteIcon2.gif") + "',1)");
                }
                LinkButton historyButton = (LinkButton)e.Row.FindControl("historyButton");
                if (historyButton != null)
                {
                    historyButton.Attributes.Add("onMouseOver", "MM_swapImage('" + historyButton.ClientID + "','','" + ResolveUrl(ControlPath + "Images/Icons/HistoryIcon2.gif") + "',1)");
                    historyButton.Attributes.Add("data-uid", file.FileId.ToString());
                    historyButton.Enabled = (file.History.Count > 1);
                }
            }
        }

        protected void btnSaveLink_Click(object sender, EventArgs e)
        {
            Components.FileVersion fileVersion;
            Components.Document doc = Components.DocumentController.GetDocument(DocumentID);
            Components.DMSFile file = doc.Files.Find(p => p.FileType.Equals("url", StringComparison.OrdinalIgnoreCase));
            if (file == null)
            {
                file = new Components.DMSFile();
                file.DocumentId = DocumentID;
                file.FileType = "url";
                file.MimeType = "text/url";
                file.StatusId = 2;
                Components.DocumentController.SaveFile(file);
                fileVersion = new Components.FileVersion();
                fileVersion.FileId = file.FileId;
                fileVersion.Version = 1000000;
                doc.Files.Add(file);
            }
            else
            {
                fileVersion = file.FileVersion;
                fileVersion.FileVersionId = 0;
                fileVersion.Version++;
            }
            fileVersion.IPAddress = Generic.GetIPAddress();
            fileVersion.CreatedByUserID = UserId;
            fileVersion.IsGroupOwner = false;
            fileVersion.CreatedOnDate = DateTime.Now;
            fileVersion.Filesize = 0;
            fileVersion.WebPageUrl = tbURL.Text.Split(',')[0];
            Components.DocumentController.SaveFileVersion(fileVersion);
            file.FileVersionId = fileVersion.FileVersionId;
            file.Filesize = fileVersion.Filesize;
            Components.DocumentController.SaveFile(file);
            ToggleStatus(file, true);
            doc.LastModifiedOnDate = DateTime.Now;
            Components.DocumentController.SaveDocument(doc);
            doc.Files = Components.DocumentController.GetAllFilesForDocument(DocumentID);
            gvFiles.DataSource = doc.Files;
            gvFiles.DataBind();
        }

        private void ToggleStatus(Components.DMSFile file, bool bActive)
        {
            DotNetNuke.Entities.Portals.PortalSettings portal = DotNetNuke.Entities.Portals.PortalSettings.Current;
            if (bActive)
            {
                List<Components.DMSFile> files = Components.DocumentController.GetAllFilesForDocument(file.DocumentId).FindAll(p => p.FileType.Equals(file.FileType, StringComparison.OrdinalIgnoreCase) && p.StatusId == 1);
                foreach (Components.DMSFile f in files)
                {
                    f.StatusId = 2;
                    Components.DocumentController.SaveFile(f);
                    if (!f.FileType.Equals("url", StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, f.UploadDirectory.Replace("/", "\\"), f.Filename)))
                    {
                        System.IO.File.Delete(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, f.UploadDirectory.Replace("/", "\\"), f.Filename));
                    }
                }
            }
            file.StatusId = (bActive ? 1 : 2);
            Components.DocumentController.SaveFile(file);
            if (!file.FileType.Equals("url", StringComparison.OrdinalIgnoreCase))
            {
                if (bActive && SaveLocalFile)
                {
                    Components.Document doc = Components.DocumentController.GetDocument(file.DocumentId);
                    if (doc != null && (!doc.ActivationDate.HasValue || DateTime.Now >= doc.ActivationDate.Value) && (!doc.ExpirationDate.HasValue || DateTime.Now <= (doc.ExpirationDate.Value + new TimeSpan(23, 59, 59))))
                    {
                        file.FileVersion.LoadContents();
                        if (file.FileVersion.Contents != null && file.FileVersion.Contents.Length > 0)
                        {
                            file.CreateFolder();
                            if (System.IO.File.Exists(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename)))
                            {
                                System.IO.File.Delete(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename));
                            }
                            System.IO.FileStream fs = new System.IO.FileStream(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename), System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
                            fs.Write(file.FileVersion.Contents, 0, file.FileVersion.Contents.Length);
                            fs.Close();
                            try
                            {
                                System.IO.File.SetLastWriteTime(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename), file.FileVersion.CreatedOnDate);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        file.FileVersion.Contents = null;
                    }
                }
                else if (System.IO.File.Exists(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename)))
                {
                    System.IO.File.Delete(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename));
                }
            }
        }

        protected void btnSaveFile_Click(object sender, EventArgs e)
        {
            Components.Document doc = Components.DocumentController.GetDocument(DocumentID);
            Components.DMSFile file = doc.Files.Find(p => p.Filename.Equals(System.IO.Path.GetFileName(upDocument.PostedFile.FileName).Replace(" ", "_"), StringComparison.OrdinalIgnoreCase));
            if (file == null)
            {
                file = new Components.DMSFile();
                file.DocumentId = DocumentID;
                file.FileType = Components.DMSFile.GetFileType(System.IO.Path.GetExtension(upDocument.PostedFile.FileName), PortalId, PortalWideRepository ? 0 : TabModuleId);
                file.StatusId = 2;
                file.Filename = System.IO.Path.GetFileName(upDocument.PostedFile.FileName).Replace(" ", "_");
                file.UploadDirectory = string.Format("Files/{0}/{1}", TabModuleId, Generic.CreateSafeFolderName(doc.DocumentName));
                file.MimeType = upDocument.PostedFile.ContentType;
                Components.DocumentController.SaveFile(file);
                file.FileVersion = new Components.FileVersion();
                file.FileVersion.FileId = file.FileId;
                file.FileVersion.Version = 1000000;
                doc.Files.Add(file);
            }
            else
            {
                file.FileVersion = file.FileVersion;
                file.FileVersion.FileVersionId = 0;
                file.FileVersion.Version++;
            }
            file.FileVersion.IPAddress = Generic.GetIPAddress();
            file.FileVersion.CreatedByUserID = UserId;
            file.FileVersion.IsGroupOwner = false;
            file.FileVersion.CreatedOnDate = DateTime.Now;
            file.FileVersion.Filesize = upDocument.PostedFile.ContentLength;
            Components.DocumentController.SaveFileVersion(file.FileVersion);
            file.FileVersion.SaveContents(upDocument.PostedFile.InputStream);
            file.FileVersionId = file.FileVersion.FileVersionId;
            file.Filesize = file.FileVersion.Filesize;
            Components.DocumentController.SaveFile(file);
            /*if (file.StatusId == 1)
            {
                doc.CreateDocumentFolder();
                if(System.IO.File.Exists(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename)))
                {
                    System.IO.File.Delete(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename));
                }
                upDocument.PostedFile.SaveAs(string.Format("{0}{1}\\{2}", portal.HomeDirectoryMapPath, file.UploadDirectory.Replace("/", "\\"), file.Filename));
            }*/
            ToggleStatus(file, true);
            doc.LastModifiedOnDate = DateTime.Now;
            Components.DocumentController.SaveDocument(doc);
            if (cbReplacePDFTitle.Checked && System.IO.Path.GetExtension(file.Filename).Equals(".pdf", StringComparison.OrdinalIgnoreCase) && file.StatusId == 1)
            {
                try
                {
                    Generic.ReplacePDFTitle(file, doc.DocumentName);
                }
                catch(Exception)
                {
                }
            }
            cbReplacePDFTitle.Checked = false;
            try
            {
                DotNetNuke.Entities.Portals.PortalInfo portal = DotNetNuke.Entities.Portals.PortalController.Instance.GetPortal(PortalId);
                Repository repository = Components.DocumentController.GetRepository(PortalId, PortalWideRepository ? 0 : TabModuleId);
                Thumbnail thumb = new Thumbnail(portal, repository, ControlPath);
                thumb.CreateThumbnail(Request, file);
            }
            catch(Exception)
            {
            }
            doc.Files = Components.DocumentController.GetAllFilesForDocument(DocumentID);
            gvFiles.DataSource = doc.Files;
            gvFiles.DataBind();
            List<DotNetNuke.Entities.Users.UserInfo> subscribers = Components.UserController.GetFileNotificationRecipients(FileNotificationsRole, PortalId);
            NotificationType notificationType = NotificationsController.Instance.GetNotificationType("HtmlNotification");
            PortalSettings portalSettings = DotNetNuke.Entities.Portals.PortalSettings.Current;
            UserInfo adminUser = DotNetNuke.Entities.Users.UserController.GetUserById(portalSettings.PortalId, portalSettings.AdministratorId);
            Components.FileType fileType = Components.DocumentController.GetFileTypeByExt(file.FileType, PortalId, PortalWideRepository ? 0 : TabModuleId);
            string fileTypeShort = fileType != null ? fileType.FileTypeShortName : String.Empty;
            Notification notification = new Notification
            {
                NotificationTypeID = notificationType.NotificationTypeId,
                Subject = NewFileSubject,
                Body = TokenMsg(NewFileMsg, file.Filename, file.FileType, fileTypeShort, file.FileVersion.IsGroupOwner ? file.FileVersion.Group.RoleName : file.FileVersion.CreatedByUser.DisplayName, file.FileVersion.CreatedOnDate.ToString("d"), doc.DocumentName).ToString(),
                IncludeDismissAction = true,
                SenderUserID = adminUser.UserID
            };
            if (subscribers.Count > 0)
            {
                NotificationsController.Instance.SendNotification(notification, portalSettings.PortalId, null, subscribers);
            }
        }

        public System.Text.StringBuilder TokenMsg(string replymsg, string fileName, string fileType, string fileTypeName, string uploader, string uploadDate, string document)
        {
            var stringb = new System.Text.StringBuilder();
            stringb.Append(replymsg);
            stringb.Replace("[FILENAME]", fileName);
            stringb.Replace("[FILETYPE]", fileType);
            stringb.Replace("[FILETYPENAME]", fileTypeName);
            stringb.Replace("[UPLOADER]", uploader);
            stringb.Replace("[UPLOADDATE]", uploadDate);
            stringb.Replace("[DOCUMENT]", document);
            var converttext = Server.HtmlDecode(stringb.ToString());
            stringb.Clear();
            stringb.Append(converttext);
            return stringb;
        }

        private bool TagsFilesEnabled
        {
            set
            {
                btnAddTag.Enabled = value;
                if (value)
                {
                    if (btnNewLink.Attributes["disabled"] != null)
                    {
                        btnNewLink.Attributes.Remove("disabled");
                    }
                    if (btnNewFile.Attributes["disabled"] != null)
                    {
                        btnNewFile.Attributes.Remove("disabled");
                    }
                }
                else
                {
                    if (btnNewLink.Attributes["disabled"] == null)
                    {
                        btnNewLink.Attributes.Add("disabled", "disabled");
                    }
                    if (btnNewFile.Attributes["disabled"] == null)
                    {
                        btnNewFile.Attributes.Add("disabled", "disabled");
                    }
                }
                gvFiles.Enabled = value;
                ddTags.Enabled = value;
                tbTag.Enabled = value;
                tbTags.Enabled = value;

            }
        }

        public string GetLinkUrl(bool showDescription, string documentList, int fileId, string headerText)
        {
            string[] strArrays = new string[2];
            int moduleId = ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            strArrays[1] = string.Concat("q=", Generic.StringToHex(Generic.UrlEncode(Gafware.Modules.DMS.Cryptography.CryptographyUtil.Encrypt(String.Format("descriptions={0}&docids={1}&fileid={2}&headertext={3}", showDescription.ToString(), documentList, fileId, HttpUtility.UrlEncode(headerText))))));
            return GetDocumentsLink(strArrays);
        }

        protected string GetUrl(object item)
        {
            if (item.GetType() == typeof(Components.DMSFile))
            {
                Components.DMSFile file = item as Components.DMSFile;
                if (file != null)
                {
                    if (file.FileType.Equals("url", StringComparison.OrdinalIgnoreCase))
                    {
                        return "<a class=\"document\" data-uid=\"" + file.FileId.ToString() + "\"href='" + file.WebPageUrl + "' target='_blank' style=\"color: #" + Theme + ";\"> " + file.WebPageUrl + "</a>";
                    }
                    else
                    {
                        Components.Document doc = Components.DocumentController.GetDocument(file.DocumentId);
                        if (doc != null && (!doc.ActivationDate.HasValue || DateTime.Now >= doc.ActivationDate.Value) && (!doc.ExpirationDate.HasValue || DateTime.Now <= (doc.ExpirationDate.Value + new TimeSpan(23, 59, 59))))
                        {
                            if (file.StatusId == 1)
                            {
                                return "<a class=\"document\" data-uid=\"" + file.FileId.ToString() + "\" href='" + GetLinkUrl(false, file.DocumentId.ToString(), file.FileId, String.Empty) + "' target='_blank' style=\"color: #" + Theme + ";\">" + file.Filename + "</a>";
                            }
                            else
                            {
                                return "<a class=\"document\" data-uid=\"" + file.FileId.ToString() + "\"href='" + ResolveUrl("~" + ControlPath + "GetFile.ashx") + "?id=" + file.FileId.ToString() + "' target='_blank' style=\"color: #" + Theme + ";\">" + file.Filename + "</a>";
                            }
                        }
                        else
                        {
                            return "<a class=\"document\" data-uid=\"" + file.FileId.ToString() + "\"href='" + ResolveUrl("~" + ControlPath + "GetFile.ashx") + "?id=" + file.FileId.ToString() + "' target='_blank' style=\"color: #" + Theme + ";\">" + file.Filename + "</a>";
                        }
                    }
                }
            }
            return String.Empty;
        }

        protected string GetVersion(object item)
        {
            if (item.GetType() == typeof(Components.DMSFile))
            {
                Components.DMSFile file = item as Components.DMSFile;
                int build = file.Version % 1000;
                int minor = ((int)(file.Version / 1000)) % 1000;
                int major = (int)(((int)(file.Version / 1000)) / 1000);
                return string.Concat("<a class=\"version\" data-uid=\"", file.FileVersionId, "\" onclick=\"editVersion(this);\" style=\"color: #" + Theme + ";\">", string.Format("{0}.{1}.{2}", major, minor, build), "</a>");
            }
            else if (item.GetType() == typeof(Components.FileVersion))
            {
                Components.FileVersion file = item as Components.FileVersion;
                int build = file.Version % 1000;
                int minor = ((int)(file.Version / 1000)) % 1000;
                int major = (int)(((int)(file.Version / 1000)) / 1000);
                return string.Concat("<a class=\"version\" data-uid=\"", file.FileVersionId, "\" onclick=\"editVersion(this);\" style=\"color: #" + Theme + ";\">", string.Format("{0}.{1}.{2}", major, minor, build), "</a>");
            }
            return String.Empty;
        }

        protected bool IsUrl(object item)
        {
            if (item.GetType() == typeof(Components.DMSFile))
            {
                Components.DMSFile file = item as Components.DMSFile;
                return file.FileType.Equals("url", StringComparison.OrdinalIgnoreCase);
            }
            else if (item.GetType() == typeof(Components.FileVersion))
            {
                Components.FileVersion fileVersion = item as Components.FileVersion;
                Components.DMSFile file = Components.DocumentController.GetFile(fileVersion.FileId);
                if (file != null && file.FileType.Equals("url", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        protected string GetFilesize(object item)
        {
            if (item.GetType() == typeof(Components.DMSFile))
            {
                Components.DMSFile file = item as Components.DMSFile;
                if (!file.FileType.Equals("url", StringComparison.OrdinalIgnoreCase))
                {
                    string[] sizes = { "bytes", "kb", "mb", "gb", "tb" };
                    double len = file.Filesize;
                    int order = 0;
                    while (len >= 1024 && order < sizes.Length - 1)
                    {
                        order++;
                        len = len / 1024;
                    }
                    return string.Concat("<span class=\"filesize\" data-uid=\"", file.FileId, "\">", String.Format("{0:0.#} {1}", len, sizes[order]), "</span>");
                }
            }
            else if (item.GetType() == typeof(Components.FileVersion))
            {
                Components.FileVersion fileVersion = item as Components.FileVersion;
                Components.DMSFile file = Components.DocumentController.GetFile(fileVersion.FileId);
                if (file != null && !file.FileType.Equals("url", StringComparison.OrdinalIgnoreCase))
                {
                    string[] sizes = { "bytes", "kb", "mb", "gb", "tb" };
                    double len = fileVersion.Filesize;
                    int order = 0;
                    while (len >= 1024 && order < sizes.Length - 1)
                    {
                        order++;
                        len = len / 1024;
                    }
                    return string.Concat("<span class=\"filesize\" data-uid=\"", fileVersion.FileVersionId, "\">", String.Format("{0:0.#} {1}", len, sizes[order]), "</span>");
                }
            }
            return String.Empty;
        }

        protected void lnkRemoveTag_Click(object sender, EventArgs e)
        {
            RemoveTag(hidTagToRemove.Value);
        }

        protected string GetComments(string comments)
        {
            if (!String.IsNullOrEmpty(comments))
            {
                float width = Generic.GetWidthOfString(comments, gvPackets.RowStyle.Font);
#if DEBUG
                if (width > 400)
#else
                if (width > 510)
#endif
                {
                    string temp = comments;
#if DEBUG
                    while ((width = Generic.GetWidthOfString(temp + "...", gvPackets.RowStyle.Font)) > 400)
#else
                    while ((width = Generic.GetWidthOfString(temp + "...", gvPackets.RowStyle.Font)) > 510)
#endif
                    {
                        temp = temp.Substring(0, temp.Length - 1);
                    }
                    return temp + "...";
                }
                return comments;
            }
            return String.Empty;
        }

        protected void lnkPacketView_Command(object sender, CommandEventArgs e)
        {
            HttpResponse response = base.Response;
            string[] strArrays = new string[2];
            strArrays[0] = string.Concat("mid=", ModuleId.ToString());
            strArrays[1] = string.Concat("id=", e.CommandArgument);
            response.Redirect(GetPacketsLink(strArrays));
        }

        protected void rptCategory_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Components.Category item = (Components.Category)e.Item.DataItem;
                if (item != null)
                {
                    Components.DocumentCategory category = Components.DocumentController.GetAllCategoriesForDocument(DocumentID).Find(c => c.CategoryId == item.CategoryId);
                    Label lbl = (Label)e.Item.FindControl("lblCategory");
                    if (lbl != null)
                    {
                        lbl.Text = (category != null ? "Yes" : "No");
                    }
                    CheckBox cbCategory = (CheckBox)e.Item.FindControl("cbCategory");
                    if (cbCategory != null)
                    {
                        cbCategory.Checked = (category != null ? true : (DocumentID == 0 ? true : false));
                    }
                    Label lblCategoryRole = (Label)e.Item.FindControl("lblCategoryRole");
                    if (lblCategoryRole != null)
                    {
                        lblCategoryRole.Text = string.Format("Security Role: {0}", item.Role.RoleName);
                    }
                }
            }
        }

        protected void gvHistory_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int fileVersionID = Convert.ToInt32(gvHistory.DataKeys[e.RowIndex].Value.ToString());
            Components.FileVersion fileVersion = Components.DocumentController.GetFileVersion(fileVersionID);
            if (fileVersion != null)
            {
                Components.DMSFile file = Components.DocumentController.GetFile(fileVersion.FileId);
                if (file != null)
                {
                    Components.DocumentController.DeleteFileVersion(fileVersionID);
                    List<Components.FileVersion> files = Components.DocumentController.GetFileVersions(fileVersion.FileId).FindAll(p => p.FileVersionId != file.FileVersionId);
                    gvHistory.DataSource = files;
                    gvHistory.DataBind();
                }
            }
            history.Value = "1";
            e.Cancel = true;
        }

        protected void gvHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton btn = (LinkButton)e.Row.FindControl("btnDelete");
                if (btn != null)
                {
                    btn.Attributes.Add("onMouseOver", "MM_swapImage('" + btn.ClientID + "','','" + ResolveUrl("~" + ControlPath + "Images/Icons/DeleteIcon2.gif") + "',1)");
                }
                Components.FileVersion fileVersion = (Components.FileVersion)e.Row.DataItem;
                e.Row.Cells[1].Attributes.Add("data-version", fileVersion.Version.ToString());
                e.Row.Cells[1].Attributes.Add("data-uid", fileVersion.FileVersionId.ToString());
            }
        }

        protected void historyButton_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName.Equals("History", StringComparison.OrdinalIgnoreCase))
            {
                int fileId = Convert.ToInt32(e.CommandArgument);
                Components.DMSFile file = Components.DocumentController.GetFile(fileId);
                if (file != null)
                {
                    if (file.FileType.Equals("url", StringComparison.OrdinalIgnoreCase))
                    {
                        gvHistory.Columns[gvHistory.Columns.Count - 1].HeaderText = "Url";
                        gvHistory.Columns[gvHistory.Columns.Count - 1].ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                        gvHistory.Columns[gvHistory.Columns.Count - 1].HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    }
                    else
                    {
                        gvHistory.Columns[gvHistory.Columns.Count - 1].HeaderText = "Download";
                        gvHistory.Columns[gvHistory.Columns.Count - 1].ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                        gvHistory.Columns[gvHistory.Columns.Count - 1].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    }
                    List<Components.FileVersion> fileVersions = Components.DocumentController.GetFileVersions(fileId).FindAll(p => p.FileVersionId != file.FileVersionId);
                    gvHistory.DataSource = fileVersions;
                    gvHistory.DataBind();
                }
                List<Components.DMSFile> files = Components.DocumentController.GetAllFilesForDocument(DocumentID);
                gvFiles.DataSource = files;
                gvFiles.DataBind();
                history.Value = "1";
            }
        }

        protected void lnkReload_Click(object sender, EventArgs e)
        {
            List<Components.DMSFile> files = Components.DocumentController.GetAllFilesForDocument(DocumentID);
            gvFiles.DataSource = files;
            gvFiles.DataBind();
        }

        /*public Bitmap RenderPage(PDFLibNet.PDFWrapper doc, int page)
        {
            doc.CurrentPage = page + 1;
            doc.CurrentX = 0;
            doc.CurrentY = 0;
            doc.RenderPage(IntPtr.Zero);
            //create an image to draw the page into
            var buffer = new Bitmap(doc.PageWidth, doc.PageHeight);
            doc.ClientBounds = new Rectangle(0, 0, doc.PageWidth, doc.PageHeight);
            using (var g = Graphics.FromImage(buffer))
            {
                var hdc = g.GetHdc();
                try
                {
                    doc.DrawPageHDC(hdc);
                }
                finally
                {
                    g.ReleaseHdc();
                }
            }
            return buffer;
        }

        public byte[] ConvertPDFtoJPG(byte[] pdf)
        {
            byte[] data = new byte[0];
            if (pdf != null && pdf.Length > 0)
            {
                using (System.IO.MemoryStream pdfStream = new System.IO.MemoryStream(pdf))
                {
                    PDFLibNet.PDFWrapper _pdfDoc = new PDFLibNet.PDFWrapper();
                    _pdfDoc.LoadPDF(pdfStream);
                    if (_pdfDoc.PageCount > 0)
                    {
                        Bitmap img = RenderPage(_pdfDoc, 0);
                        data = img.ToByteArray(ImageFormat.Jpeg);
                    }
                    _pdfDoc.Dispose();
                }
            }
            return data;
        }*/

        protected void cbUseCategorySecurityRoles_CheckedChanged(object sender, EventArgs e)
        {
            pnlSecurityRole.Visible = !cbUseCategorySecurityRoles.Checked;
        }

        protected void btnReloadSecurityRoles_Click(object sender, EventArgs e)
        {
            int oldIndex = ddlSecurityRole.SelectedIndex;
            ddlSecurityRole.SelectedIndex = -1;
            ddlSecurityRole.DataSource = DotNetNuke.Security.Roles.RoleController.Instance.GetRoles(PortalId);
            ddlSecurityRole.DataBind();
            ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Users", "-1"));
            ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Superusers", "-2"));
            ddlSecurityRole.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Unauthenticated Users", "-3"));
            if (oldIndex < ddlSecurityRole.Items.Count)
            {
                ddlSecurityRole.SelectedIndex = oldIndex;
            }
        }

        protected void btnSaveVersion_Click(object sender, EventArgs e)
        {
            pnlVersionMsg.Visible = false;
            int fileVersionId = Convert.ToInt32(hidFileVersionId.Value);
            int nVersion = Convert.ToInt32(string.Format("{0}{1}{2}", tbMajor.Text.PadLeft(3, '0'), tbMinor.Text.PadLeft(3, '0'), tbBuild.Text.PadLeft(3, '0')));
            Components.FileVersion version = Components.DocumentController.GetFileVersion(fileVersionId);
            if (version != null)
            {
                Components.DMSFile file = Components.DocumentController.GetFile(version.FileId);
                if (file != null)
                {
                    int origFileVersionId = file.FileVersionId;
                    List<Components.FileVersion> versions = Components.DocumentController.GetFileVersions(version.FileId);
                    bool exists = versions.Exists(p => p.Version == nVersion && p.FileVersionId != fileVersionId);
                    if (!exists)
                    {
                        version.Version = nVersion;
                        Components.DocumentController.SaveFileVersion(version);
                        file = Components.DocumentController.GetFile(version.FileId);
                        if (file != null && file.FileVersionId != origFileVersionId)
                        {
                            ToggleStatus(file, file.StatusId == 1);
                        }
                        Components.Document doc = Components.DocumentController.GetDocument(DocumentID);
                        doc.LastModifiedOnDate = DateTime.Now;
                        Components.DocumentController.SaveDocument(doc);
                        List<Components.DMSFile> files = Components.DocumentController.GetAllFilesForDocument(DocumentID);
                        gvFiles.DataSource = files;
                        gvFiles.DataBind();
                    }
                    else
                    {
                        pnlVersionMsg.Visible = true;
                    }
                }
            }
        }

        protected void gvPackets_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            LinkButton lnk = (LinkButton)e.Row.FindControl("lnkPacketView");
            if (lnk != null)
            {
                lnk.ForeColor = System.Drawing.ColorTranslator.FromHtml("#" + Theme);
            }
        }

        protected void delAllCommandButton_Click(object sender, EventArgs e)
        {
            if(Session["gv"] != null)
            {
                System.Data.DataView dataView = (System.Data.DataView)Session["gv"];
                hidProcessName.Value = DMSController.DeleteAll(dataView, PortalId, TabModuleId, PortalWideRepository);
                deleteAllWindow.VisibleOnPageLoad = true;
                hidFileDeleteStatus.Value = "Started";
            }
        }

        protected void lnkFinish_Click(object sender, EventArgs e)
        {
            deleteAllWindow.VisibleOnPageLoad = false;
            hidFileDeleteStatus.Value = "Finished";
            CreateDataTable(true);
        }

        protected void btnBack2_Click(object sender, EventArgs e)
        {
            btnBack_Click(sender, e);
        }

        protected string JSEncode(string text)
        {
            return Generic.JSEncode(text);
        }

        protected void cbActive_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Request["__EVENTARGUMENT"]) && Generic.IsNumber(Request["__EVENTARGUMENT"]))
                {
                    int fileId = Convert.ToInt32(Request["__EVENTARGUMENT"]);
                    Components.DMSFile file = Components.DocumentController.GetFile(fileId);
                    if (file != null && HttpContext.Current.User.Identity.IsAuthenticated)
                    {
                        Components.Document document = Components.DocumentController.GetDocument(file.DocumentId);
                        DotNetNuke.Entities.Users.UserInfo user = DotNetNuke.Entities.Users.UserController.GetUserByName(PortalId, HttpContext.Current.User.Identity.Name);
                        List<DMSController.Inactive> inactive = new List<DMSController.Inactive>();
                        DotNetNuke.Security.Roles.RoleInfo groupOwner = (document != null && document.DocumentId > 0 && document.IsGroupOwner ? Components.UserController.GetRoleById(PortalId, document.CreatedByUserID) : null);
                        if (document != null && (((!document.IsGroupOwner && document.CreatedByUserID == user.UserID) || (document.IsGroupOwner && user.IsInRole(groupOwner.RoleName))) || user.IsSuperUser || user.IsInRole("Administrator")))
                        {
                            DMSController controller = new DMSController();
                            string url = GetNaviateUrl("GetDocuments");
                            CheckBox cb = sender as CheckBox;
                            inactive = controller.ToggleStatus(file, cb.Checked, ControlPath, ModuleId, url);

                        }
                        document.LastModifiedOnDate = DateTime.Now;
                        Components.DocumentController.SaveDocument(document);
                        List<DMSFile> files = Components.DocumentController.GetAllFilesForDocument(file.DocumentId);
                        foreach(DMSFile f in files)
                        {
                            if(f.FileId == fileId)
                            {
                                f.Message = LocalizeString("Saved");
                                break;
                            }
                        }
                        foreach(DMSController.Inactive i in inactive)
                        {
                            foreach(DMSFile f in files)
                            {
                                if(f.FileId == i.FileId)
                                {
                                    f.Message = LocalizeString("Saved");
                                    break;
                                }
                            }
                        }
                        gvFiles.DataSource = files;
                        gvFiles.DataBind();
                    }
                    else
                    {
                    }
                }
                else
                {
                }
            }
            catch(Exception)
            {
            }
        }

        protected void cbIsGroupOwner_CheckedChanged(object sender, EventArgs e)
        {
            pnlOwnerEdit2.Visible = !cbIsGroupOwner.Checked;
            pnlGroupEdit.Visible = cbIsGroupOwner.Checked;
            if(!cbIsGroupOwner.Checked && ddOwner2.SelectedIndex == 0)
            {
                ddOwner2.SelectedIndex = ddOwner2.Items.IndexOf(ddOwner2.Items.FindByValue(UserId.ToString()));
            }
        }

        protected void activityReportCommandButton_Click(object sender, EventArgs e)
        {
            HttpResponse response = base.Response;
            string[] strArrays = new string[ddOwner.SelectedIndex > 0 ? 2 : 1];
            int moduleId = base.ModuleId;
            strArrays[0] = string.Concat("mid=", moduleId.ToString());
            if(ddOwner.SelectedIndex > 0)
            {
                strArrays[1] = string.Concat("uid=", ddOwner.SelectedValue);
            }
            response.Redirect(_navigationManager.NavigateURL("ActivityReport", strArrays));
        }
    }
}