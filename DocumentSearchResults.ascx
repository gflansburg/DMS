<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentSearchResults.ascx.cs" Inherits="Gafware.Modules.DMS.DocumentSearchResults" %>
<asp:Literal ID="litMetaRedirect" runat="server"></asp:Literal>
<asp:Panel ID="pnlSearchNotFound" runat="server" Visible="false">
    <div class="SearchResultHeader" id="SearchResultHeader" runat="server"><span class="text_medium" style="float: left;"><strong style="color:#FFFFFF;"><asp:Label ID="lblHeader" runat="server" Text="Document Search Results" /></strong></span><span style="float: right;" id="pnlAdmin" runat="server"><asp:ImageButton ID="btnAdmin" ImageUrl="Images/settings.png" OnClick="btnAdmin_Click" runat="server" /></span><br style="clear: both;" /></div>
    <br />
    <div style="margin-left: 5px;"><%= LocalizeString("NoDocumentsFound") %></div>
</asp:Panel>
<asp:Panel ID="pnlDocumentNotFound" runat="server" Visible="false">
    <h3><%= LocalizeString("NotAvailable") %></h3><br />
    <%= LocalizeString("NotAvailableHelp") %>
</asp:Panel>
<asp:Panel ID="pnlError" runat="server" Visible="false">
    <h3><%= LocalizeString("NotProcessed") %></h3><br />
</asp:Panel>
<asp:Panel ID="pnlDocumentFound" runat="server" Visible="false">
    <h3><%= LocalizeString("Thanks") %></h3>
	<%= LocalizeString("ThanksInfo") %><br/>
    <asp:HyperLink ID="lnkFileLocation" runat="server" Target="_self"></asp:HyperLink>
</asp:Panel>
<asp:Panel ID="pnlDocumentsFound" runat="server" Visible="false">
    <div class="SearchResultHeader" id="SearchResultHeader2" runat="server"><span class="text_medium" style="float: left;"><strong style="color:#FFFFFF;"><asp:Label ID="lblHeader2" runat="server" Text="Document Search Results" /></strong></span><span style="float: right;" id="pnlAdmin2" runat="server"><asp:ImageButton ID="btnAdmin2" ImageUrl="Images/settings.png" OnClick="btnAdmin_Click" runat="server" /></span><br style="clear: both;" /></div>
    <asp:Panel ID="pnlDescription" runat="server" CssClass="PacketDescription" Visible="false"><asp:Label ID="lblDescription" runat="server"></asp:Label></asp:Panel>
    <asp:GridView ID="rptDocuments" runat="server" AutoGenerateColumns="False" CellPadding="3" CellSpacing="3" 
        EmptyDataText="No matching document found." OnRowDataBound="rptDocuments_RowDataBound" OnDataBound="rptDocuments_DataBound"
        ForeColor="Black" GridLines="None" DataKeyNames="DocumentID" BackColor="White" BorderColor="#DEDFDE" PageSize="20"
        BorderStyle="None" BorderWidth="1px" AllowPaging="True" AllowSorting="False" Width="100%" ShowFooter="True"
        OnPageIndexChanging="rptDocuments_PageIndexChanging" ShowHeader="False" PagerSettings-PageButtonCount="5">
		<Columns>
            <asp:TemplateField ItemStyle-HorizontalAlign="Left">
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
                <FooterTemplate>
                    <div style="float: right;"><%= GetFooter() %> <%= LocalizeString("Results") %></div>
                </FooterTemplate>
            </asp:TemplateField>
		</Columns>
		<RowStyle BackColor="#FFFFFF" VerticalAlign="Top" />
		<PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
        <PagerTemplate>
            <table class="pager">
                <tr>
                    <td>
                        <asp:PlaceHolder ID="ph" runat="server"></asp:PlaceHolder>
                    </td>
                </tr>
            </table>
        </PagerTemplate>		            
	</asp:GridView>
</asp:Panel>
