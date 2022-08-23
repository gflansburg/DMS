<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Gafware.Modules.DMS.View" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/desktopmodules/Gafware/DMS/DocumentSearchResults.ascx" TagPrefix="uc1" TagName="DocumentSearchResults" %>
<style type="text/css">
    .se-pre-con {
	    position: fixed;
	    left: 0px;
	    top: 0px;
	    width: 100%;
	    height: 100%;
	    z-index: 9999;
	    background-color: #000;
        opacity: 0.6;
    }
</style>
<asp:Literal ID="litCSS" runat="server"></asp:Literal>
<div class="se-pre-con"></div>
<h2 id="menuText" runat="server" style="margin-bottom: 6px; padding-bottom: 0px;"><%=LocalizeString("BasicSettings")%></h2>
<div class="dms">
    <asp:Panel ID="pnlMain" runat="server">
        <asp:HiddenField ID="hidTab" runat="server" />
        <div style="float:right;margin-bottom:10px;" id="pnlAdmin" runat="server" visible="false">
            <asp:linkbutton runat="server" id="viewDocumentsCommandButton" causesvalidation="False" OnClick="ViewDocumentsCommandButtonClicked" CssClass="secondaryButton docButton"><asp:label runat="server" resourcekey="viewDocumentsCommandButton" /></asp:linkbutton>
            <asp:linkbutton runat="server" id="viewPacketsCommandButton" causesvalidation="False" CssClass="secondaryButton packetButton" OnClick="viewPacketsCommandButton_Click"><asp:label runat="server" resourcekey="viewPacketsCommandButton" /></asp:linkbutton>
            <asp:linkbutton runat="server" id="viewTagsCommandButton" causesvalidation="False" CssClass="secondaryButton tagButton" OnClick="viewTagsCommandButton_Click"><asp:label runat="server" resourcekey="viewTagsCommandButton" /></asp:linkbutton>
            <asp:linkbutton runat="server" id="linkCreatorCommandButton" causesvalidation="False" CssClass="secondaryButton linkCreateButton" OnClick="linkCreatorCommandButton_Click"><asp:label runat="server" resourcekey="linkCreatorCommandButton" /></asp:linkbutton>
            <asp:linkbutton runat="server" id="uploadReportCommandButton" causesvalidation="False" CssClass="secondaryButton reportButton" OnClick="uploadReportCommandButton_Click"><asp:label runat="server" resourcekey="uploadReportCommandButton" /></asp:linkbutton>
            <asp:linkbutton runat="server" id="bulkImportCommandButton" causesvalidation="False" CssClass="secondaryButton importButton" OnClick="bulkImportCommandButton_Click"><asp:label runat="server" resourcekey="bulkImportCommandButton" /></asp:linkbutton>
            <asp:linkbutton runat="server" id="settingsCommandButton" causesvalidation="False" CssClass="secondaryButton settingsButton" OnClick="settingsCommandButton_Click"><asp:label runat="server" resourcekey="settingsCommandButton" /></asp:linkbutton>
        </div>
        <div style="clear: both"></div>
        <asp:Panel ID="pnlSearch" runat="server">
            <div class="searchBox" id="searchBox" runat="server">
                <div style="padding: 5px; width: 100%; text-align: left;">
                    <div style="width: 100%;">
                        <span class="SearchText"><strong><%= LocalizeString("SearchTerms") %></strong> </span>
                        <br style="clear: none" />
                        <asp:TextBox ID="tbKeywords" runat="server" style="min-width: 250px; width: calc(100% - 110px);" autofocus placeholder="Search Terms ..."></asp:TextBox>
                        <asp:LinkButton Width="100px" CssClass="dnnPrimaryAction" ID="btnSearch" runat="server" Text="Go!" OnClick="btnSearch_Click" ValidationGroup="Search" style="margin-left: 5px;" />
                    </div>
                    <asp:Panel ID="pnlInstructions" runat="server" style="text-align: center">
                        <br />
                        <asp:Label ID="lblInstructions" runat="server" Text='To view all documents, click "Go!" without typing a keyword.' CssClass="SearchText"></asp:Label>
                    </asp:Panel>
                </div>
                <div style="display: inline; padding: 5px;">
                    <div style="display: inline-block" id="pnlCategory" runat="server">
                        <span class="SearchText"><strong><asp:Label ID="lblCategory" runat="server" Text="Category"></asp:Label>:</strong></span><br style="clear: none"/> 
                        <asp:DropDownList ID="ddCategory" runat="server" DataTextField="CategoryName" DataValueField="CategoryId" OnSelectedIndexChanged="ddCategory_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    </div>
                </div>
                <div style="float: right; display: inline; margin-top: 20px;">
                    <div style="display: inline; width: 100%; min-width: 275px;">
                        <div style="height: 30px; vertical-align: middle; display: inline-block; padding-top: 4px;">
                            <span class="SearchText"><strong><%= LocalizeString("ShowDescriptions") %></strong></span>&nbsp;
                        </div>
                        <div style="display: inline-block">
                            <div class="toggleButton" id="cbShowDescriptionToggleButton" runat="server" style="width: 110px;">
                                <label><asp:CheckBox ID="cbShowDescription" AutoPostBack="true" Checked="true" runat="server" OnCheckedChanged="cbShowDescription_CheckedChanged" /><span></span></label>
                            </div>
                        </div>
                    </div>
                </div>
                <br style="clear: both;" />
            </div>
            <asp:Panel ID="pnlDefault" runat="server">
	            <span style="margin: 0px 0px 0px 15px;"><span class="SearchText"><strong><%= LocalizeString("DocumentSearchTips") %></strong></span></span>
	            <ul style="margin-top: 5px;">
	                <li id="pnlCategoryTip" runat="server"><%= LocalizeString("ChooseOne") %> <asp:Label ID="lblCategoryName" runat="server" Text="category"></asp:Label>. </li>
	                <%= LocalizeString("SearchTips") %>
                </ul>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
    <asp:Panel ID="pnlResults" runat="server" Visible="false">
        <asp:Panel ID="pnlSearchResults" runat="server">
            <div style="text-align: center"><br /><asp:Literal ID="litSearch" runat="server"></asp:Literal></div><br />
            <p id="addBookmarkContainer"></p>
        </asp:Panel>
        <uc1:DocumentSearchResults runat="server" id="documentSearchResults" Search="true" />
    </asp:Panel>
</div>
<script language="javascript">
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
        try {
            //google.search.cse.element.go();
            addBookmarkObj.URL = location.href;
            addBookmarkObj.addTextLink('addBookmarkContainer');
            initViewJavascript();
        }
        catch (err) { }
    });
    Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(function (sender, args) {
        try {
            if (window.location.hash) {
                var hash = window.location.hash.substring(1); //Puts hash in variable, and removes the # character
                adocument.getElementById('<%= hidTab.ClientID %>').value = hash;
            }
        }
        catch (err) { }
    });
    $(document).ready(function () {
        if (window.location.hash) {
            var hash = window.location.hash.substring(1); //Puts hash in variable, and removes the # character
            document.getElementById('<%= hidTab.ClientID %>').value = hash;
        }
    });
</script>