<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentSearchResults.ascx.cs" Inherits="Gafware.Modules.DMS.DocumentSearchResults" %>
<asp:Literal ID="litMetaRedirect" runat="server"></asp:Literal>
<asp:Panel ID="pnlSearchNotFound" runat="server" Visible="false">
    <div class="SearchResultHeader" id="SearchResultHeader" runat="server"><span class="text_medium"><strong style="color:#FFFFFF;"><asp:Label ID="Label1" runat="server" Text="Document Search Results" /></strong></span></div>
    <br />
    <div style="margin-left: 5px;">No matching document found.</div>
</asp:Panel>
<asp:Panel ID="pnlDocumentNotFound" runat="server" Visible="false">
    <h3>The documents you are trying to retrieve are no longer available.</h3><br />
    If you reached this page through a bookmarked link or from another website, please use our document search to find a new version of the document you requested.
</asp:Panel>
<asp:Panel ID="pnlError" runat="server" Visible="false">
    <h3>Your document request could not be processed.</h3><br />
</asp:Panel>
<asp:Panel ID="pnlDocumentFound" runat="server" Visible="false">
    <h3>Thanks. We're opening your document.</h3>
	If your document doesn't open automatically after two seconds, click here:<br/>
    <asp:HyperLink ID="lnkFileLocation" runat="server" Target="_self"></asp:HyperLink>
</asp:Panel>
<asp:Panel ID="pnlDocumentsFound" runat="server" Visible="false">
    <div class="SearchResultHeader" id="SearchResultHeader2" runat="server"><span class="text_medium"><strong style="color:#FFFFFF;"><asp:Label ID="lblHeader" runat="server" Text="Document Search Results" /></strong></span></div>
    <asp:Panel ID="pnlDescription" runat="server" CssClass="PacketDescription" Visible="false"><asp:Label ID="lblDescription" runat="server"></asp:Label></asp:Panel>
    <asp:Repeater ID="rptDocuments" runat="server" OnItemDataBound="rptDocuments_ItemDataBound">
        <ItemTemplate>
		    <div class="SearchResultRecordAlt">
			    <span style="color: #<%# Theme %>;"><strong><%# Eval("Document.DocumentName").ToString().ToUpper() %></strong></span><br /> 
                <%# GetCategories(Container.DataItem) %>
                <div class="text_small" style="margin: 0px 0 0px 5px;">
                    <asp:Repeater ID="rptFiles" runat="server">
                        <ItemTemplate>
                            <%# DocFileLink(Container.DataItem) %>&nbsp;
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <div style="padding: 0 5px 0 0px;" id="details" runat="server" visible='<%# !String.IsNullOrEmpty(Eval("Document.DocumentDetails").ToString()) && ShowDescription %>'><strong>Description: </strong><%# Eval("Document.DocumentDetails") %></div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Panel>
