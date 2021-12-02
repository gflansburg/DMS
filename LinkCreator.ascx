<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LinkCreator.ascx.cs" Inherits="Gafware.Modules.DMS.LinkCreator" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>
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
    .dms .dnnFormItem .toggleButton {
        top: 0;
        left: -8px !important;
    }
</style>
<asp:Literal ID="litCSS" runat="server"></asp:Literal>
<div class="se-pre-con"></div>
<div class="dms" style="padding: 10px;">
    <h3><%=LocalizeString("BasicSettings")%></h3>
    <br />
    <asp:LinkButton ID="btnBack" runat="server" Text="Back" CssClass="dnnSecondaryAction" OnClick="btnBack_Click" CausesValidation="false" />
    <br /><br />
    <p><%=LocalizeString("BasicInfo")%></p>
    <br />
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="lblCustomHeader" runat="server" ControlName="tbCustomHeader" Suffix=":" /> 
            <asp:TextBox ID="tbCustomHeader" runat="server" Width="100%" autofocus ValidationGroup="LinkCreator"></asp:TextBox>
        </div>
        <asp:Panel ID="pnlIncludePrivate" runat="server" Visible="false">
            <div class="dnnFormItem">
                <dnn:Label ID="lblIncludePrivate" runat="server" ControlName="cbIncludePrivate" Suffix=":" /> 
                <div class="toggleButton" id="cbIncludePrivateToggleButton" runat="server">
                    <label for="<%= cbIncludePrivate.ClientID %>"><asp:CheckBox ID="cbIncludePrivate" AutoPostBack="true" Checked="false" runat="server" OnCheckedChanged="cbIncludePrivate_CheckedChanged" /><span></span></label>
                </div>
            </div>
        </asp:Panel>
        <div class="dnnFormItem">
            <dnn:Label ID="lblShowDescription" runat="server" ControlName="cbShowDescription" Suffix=":" /> 
            <div class="toggleButton" id="cbShowDescriptionToggleButton" runat="server">
                <label for="<%= cbShowDescription.ClientID %>"><asp:CheckBox ID="cbShowDescription" AutoPostBack="true" Checked="false" runat="server" OnCheckedChanged="cbShowDescription_CheckedChanged" /><span></span></label>
            </div>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblDocuments" runat="server" ControlName="ddDocuments" Suffix=":" /> 
            <asp:DropDownList ID="ddDocuments" runat="server" Width="100%" DataTextField="DocumentName" DataValueField="DocumentID" ValidationGroup="LinkCreator"></asp:DropDownList>
            <asp:LinkButton ID="btnAddDocument" runat="server" OnClick="btnAddDocument_Click" Text="Add Document" ValidationGroup="LinkCreator" CssClass="dnnPrimaryAction" style="line-height: 20px;" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="ddDocuments" InitialValue="0" Display="Dynamic" ErrorMessage=" Document Required" Font-Bold="true" ForeColor="Red" ValidationGroup="LinkCreator"></asp:RequiredFieldValidator>
        </div>
    </fieldset>
    <asp:HiddenField ID="hidFileCount" runat="server" Value="0" />
    <span style="font-weight: bold"><%=LocalizeString("DocumentsSelected")%></span><br />
    <div style="background-color: #EEEEEE; border: 1px solid #999; text-align: left; margin: 5px 1px 5px 0px;">
        <asp:GridView ID="gv" runat="server" BorderWidth="1px" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False"
            RowStyle-BackColor="#eeeeee" RowStyle-Height="18" Width="100%" GridLines="None" ShowHeader="false" CssClass="filesList"
            Font-Names="Arial" Font-Size="Small" CellPadding="3" CellSpacing="3" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None"
            ForeColor="Black" DataKeyNames="PacketDocId,DocumentId" OnRowDeleting="gv_RowDeleting" OnRowDataBound="gv_RowDataBound">
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
        <p><strong><%= LocalizeString("LinkURL") %></strong> <span><%= LocalizeString("BasicHelp") %></span></p>
        <asp:TextBox ID="tbLinkURL" TextMode="MultiLine" ReadOnly="true" Width="100%" Rows="3" runat="server"></asp:TextBox>
        <br style="clear: both" />
        <hr />
        <div style="float:right;margin-bottom:10px;">
            <asp:LinkButton ID="btnPreview" runat="server" Text="Preview Documents" CssClass="dnnSecondaryAction" OnClick="btnPreview_Click" />
            <asp:LinkButton ID="btnReset" runat="server" Text="Reset Page" CssClass="dnnSecondaryAction" OnClick="btnReset_Click" />
        </div>
    </asp:Panel>
</div>
<div id="previewDialog" class="nocontent dms">
    <div id="preview-content" class="dialog-content" style="overflow: auto; height: 100%; width: 100%;">
        <uc1:DocumentSearchResults runat="server" id="documentSearchResults" Search="true" />
    </div>
</div>
