<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LinkCreator.ascx.cs" Inherits="Gafware.Modules.DMS.LinkCreator" %>
<%@ Register Src="~/desktopmodules/Gafware/DMS/DocumentSearchResults.ascx" TagPrefix="uc1" TagName="DocumentSearchResults" %>
<style type="text/css">
    .nocontent {
        display: none;
    }
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
<div class="dms" style="padding: 10px;">
    <h3><%=LocalizeString("BasicSettings")%></h3>
    <asp:LinkButton ID="btnBack" runat="server" Text="Back" CssClass="dnnSecondaryAction" OnClick="btnBack_Click" CausesValidation="false" />
    <p>Custom headers and descriptions are only shown when multiple documents are selected. If only one document is selected, then the link generated will take the user directly to the document.</p>
    <span class="FormText">
        <span class="RequiredField">*</span> Required field.
    </span>
    <br />
    <br />
    <div class="RecordDisplay">
        <label for="<%= tbCustomHeader.ClientID %>">
            <span class="FieldName">Custom Header:</span>
            <span class="FieldValue">
                <asp:TextBox ID="tbCustomHeader" runat="server" Width="790px" ValidationGroup="LinkCreator"></asp:TextBox>
            </span>
        </label>
    </div>
    <br style="clear: both" />
    <div class="RecordDisplay">
        <label for="<%= cbShowDescription.ClientID %>">
            <span class="FieldName"><span class="RequiredField">*</span> Show Description:</span>
            <span class="FieldValue">
                <%--<asp:CheckBox ID="cbShowDescription" runat="server" AutoPostBack="true" Checked="false" OnCheckedChanged="cbShowDescription_CheckedChanged" />--%>
                <div class="toggleButton" id="cbShowDescriptionToggleButton" runat="server" style="width: 120px">
                    <label><asp:CheckBox ID="cbShowDescription" AutoPostBack="true" Checked="false" runat="server" OnCheckedChanged="cbShowDescription_CheckedChanged" /><span></span></label>
                </div>
            </span>
        </label>
    </div>
    <br style="clear: both" />
    <div class="RecordDisplay">
        <label for="<%= ddDocuments.ClientID %>">
            <span class="FieldName"><span class="RequiredField">*</span> Available Documents:</span>
            <span class="FieldValue">
                <asp:DropDownList ID="ddDocuments" runat="server" Width="675px" DataTextField="DocumentName" DataValueField="DocumentID" ValidationGroup="LinkCreator"></asp:DropDownList>
                <asp:LinkButton ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Add Document" ValidationGroup="LinkCreator" CssClass="dnnPrimaryAction" style="line-height: 20px;" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="ddDocuments" InitialValue="0" Display="Dynamic" ErrorMessage=" Document Required" Font-Bold="true" ForeColor="Red" ValidationGroup="LinkCreator"></asp:RequiredFieldValidator>
            </span>
        </label>
    </div>
    <br style="clear: both" />
    <asp:HiddenField ID="hidFileCount" runat="server" Value="0" />
    <span style="font-weight: bold">Documents Selected</span><br />
    <div style="background-color: #EEEEEE; border: 1px solid #999; text-align: left; margin: 5px 1px 5px 0px;">
        <asp:GridView ID="gv" runat="server" BorderWidth="1px" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False"
            RowStyle-BackColor="#eeeeee" RowStyle-Height="18" Width="100%" GridLines="None" ShowHeader="false" CssClass="filesList"
            Font-Names="Arial" Font-Size="Small" CellPadding="3" CellSpacing="3" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None"
            ForeColor="Black" DataKeyNames="PacketDocId,DocumentId" OnRowDeleting="gv_RowDeleting" OnRowDataBound="gv_RowDataBound"
            EmptyDataText="No documents selected.">
            <RowStyle VerticalAlign="Top" Font-Names="Arial" Font-Size="Small" BackColor="#F7F7F7" />
            <FooterStyle Font-Names="Arial" Font-Size="X-Small" BackColor="White" />
            <HeaderStyle BackColor="#666666" HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" Font-Size="Small" Font-Names="Arial" ForeColor="White" Font-Bold="False" Font-Underline="false" />
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:TemplateField ItemStyle-CssClass="dont-break-out" ItemStyle-Wrap="true" ItemStyle-VerticalAlign="Middle">
                    <ItemTemplate>
                        <%# Eval("Document.DocumentName") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-HorizontalAlign="Right" ItemStyle-Width="150px">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddFileType" DataTextField="FileType" DataValueField="FileId" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddFileType_SelectedIndexChanged" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-HorizontalAlign="Right" ItemStyle-Width="24px">
                    <ItemTemplate>
                        <asp:ImageButton ID="deleteButton" runat="server" ImageUrl="~/desktopmodules/Gafware/DMS/images/icons/DeleteIcon1.gif" AlternateText="Remove Document" ToolTip="Remove Document" CommandName="Delete" Text="Delete" onMouseOut="MM_swapImgRestore()" OnClientClick="return confirm('Are you sure you wish to remove this document?')" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
        </asp:GridView>
    </div>
    <asp:LinkButton ID="lnkCustomHeaderPostback" runat="server" Style="display: none;" OnClick="lnkCustomHeaderPostback_Click"></asp:LinkButton>
    <asp:Panel ID="pnlLink" runat="server" Visible="false">
        <asp:HiddenField ID="hidBaseUrl" runat="server" Value="" />
        <asp:HiddenField ID="previewUrl" runat="server" Value="" />
        <asp:HiddenField ID="hidDocList" runat="server" Value="" />
        <asp:HiddenField ID="hidFileID" runat="server" Value="0" />
        <asp:HiddenField ID="preview" runat="server" Value="0" />
        <br />
        <p><strong>Link URL</strong> <span>Copy and paste the following link to show a listing of the documents selected above.</span></p>
        <asp:TextBox ID="tbLinkURL" TextMode="MultiLine" ReadOnly="true" Width="100%" Rows="3" runat="server"></asp:TextBox>
        <br style="clear: both" />
        <asp:LinkButton ID="btnPreview" runat="server" Text="Preview Documents" CssClass="dnnSecondaryAction" OnClick="btnPreview_Click" />
        <asp:LinkButton ID="btnReset" runat="server" Text="Reset Page" CssClass="dnnSecondaryAction" OnClick="btnReset_Click" />
    </asp:Panel>
</div>
<div id="previewDialog" class="nocontent dms">
    <div id="preview-content" class="dialog-content" style="overflow: auto; height: 100%; width: 100%;">
        <uc1:DocumentSearchResults runat="server" id="documentSearchResults" Search="true" />
    </div>
</div>
