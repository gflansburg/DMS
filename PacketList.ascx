﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PacketList.ascx.cs" Inherits="Gafware.Modules.DMS.PacketList" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>
<%@ Register Src="~/desktopmodules/Gafware/DMS/LetterFilter.ascx" TagPrefix="uc1" TagName="LetterFilter" %>
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
    .dms .nocontent {
        display: none;
    }
    .dms .dnnFormItem .toggleButton {
        top: 0;
        left: -8px !important;
    }
</style>
<div class="se-pre-con"></div>
<asp:Literal ID="litCSS" runat="server"></asp:Literal>
<asp:HiddenField ID="preview" runat="server" Value="0" />
<div class="dms" style="padding: 10px;">
    <asp:Panel ID="pnlDetails" runat="server" style="display: none">
        <h3><%=LocalizeString("EditSettings")%></h3>
        <br />
        <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
            <h3><%= LocalizeString("PacketError") %></h3><br />
            <%= LocalizeString("PacketErrorInfo") %>
        </asp:Panel>
        <asp:Panel ID="pnlFound" runat="server">
            <fieldset>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblName" runat="server" ControlName="tbName" Suffix=":" /> 
                    <asp:TextBox ID="tbName" runat="server" MaxLength="50" Width="100%" autofocus ValidationGroup="PacketEditor"></asp:TextBox>
                    <asp:LinkButton ID="btnEditName" CssClass="dnnSecondaryAction" runat="server" CausesValidation="false" Text="Edit Name" OnClick="btnEditName_Click" style="line-height: 20px;" />
                    <asp:HiddenField ID="hidCancelRename" runat="server" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="tbName" Display="Dynamic" ErrorMessage="<br />Name is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="PacketEditor"></asp:RequiredFieldValidator>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblDescription" runat="server" ControlName="tbDescription" Suffix=":" /> 
                    <asp:TextBox ID="tbDescription" runat="server" TextMode="MultiLine" Rows="1" Width="100%" ValidationGroup="PacketEditor"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="tbDescription" Display="Dynamic" ErrorMessage="<br />Description is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="PacketEditor"></asp:RequiredFieldValidator>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblShowPacketDescription" runat="server" ControlName="cbShowPacketDescription" Suffix=":" /> 
                    <div class="toggleButton" id="cbShowPacketDescriptionToggleButton" runat="server">
                        <label for='<%= cbShowPacketDescription.ClientID %>'><asp:CheckBox ID="cbShowPacketDescription" AutoPostBack="false" runat="server" Checked="true" /><span></span></label>
                    </div>
                </div>
                <asp:Panel ID="pnlAdminComments" runat="server">
                    <div class="dnnFormItem">
                        <dnn:Label ID="lblAdminComments" runat="server" ControlName="tbAdminComments" Suffix=":" /> 
                        <asp:TextBox ID="tbAdminComments" runat="server" TextMode="MultiLine" Width="100%" Rows="1" ValidationGroup="PacketEditor"></asp:TextBox>
                    </div>
                </asp:Panel>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblCustomHeader" runat="server" ControlName="tbCustomHeader" Suffix=":" /> 
                    <asp:TextBox ID="tbCustomHeader" MaxLength="100" Width="100%" runat="server" ValidationGroup="PacketEditor"></asp:TextBox>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblShowDescription" runat="server" ControlName="cbShowDescription" Suffix=":" /> 
                    <div class="toggleButton" id="cbShowDescriptionToggleButton" runat="server">
                        <label for='<%= cbShowDescription.ClientID %>'><asp:CheckBox ID="cbShowDescription" AutoPostBack="false" runat="server" Checked="true" /><span></span></label>
                    </div>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblDocuments" runat="server" ControlName="ddDocuments" Suffix=":" /> 
                    <asp:DropDownList ID="ddDocuments" runat="server" Width="100%" DataTextField="DocumentName" DataValueField="DocumentID" ValidationGroup="AddDocument"></asp:DropDownList>
                    <asp:LinkButton ID="btnAddDocument" runat="server" OnClick="btnAddDocument_Click" Text="Add Document" ValidationGroup="AddDocument" CssClass="dnnSecondaryAction" style="line-height: 20px;" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="ddDocuments" InitialValue="0" Display="Dynamic" ErrorMessage=" Document Required" Font-Bold="true" ForeColor="Red" ValidationGroup="AddDocument"></asp:RequiredFieldValidator>
                </div>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblTags" runat="server" ControlName="ddTags" Suffix=":" /> 
                    <asp:DropDownList ID="ddTags" runat="server" Width="100%" DataTextField="TagName" DataValueField="TagID" ValidationGroup="AddTag"></asp:DropDownList>
                    <asp:LinkButton ID="btnAddTag" runat="server" OnClick="btnAddTag_Click" Text="Add Tag" ValidationGroup="AddTag" CssClass="dnnSecondaryAction" style="line-height: 20px;" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="ddTags" InitialValue="0" Display="Dynamic" ErrorMessage=" Tag Required" Font-Bold="true" ForeColor="Red" ValidationGroup="AddTag"></asp:RequiredFieldValidator>
                </div>
            </fieldset>
            <span style="font-weight: bold"><%= LocalizeString("DocumentsSelected") %></span><br />
            <div style="background-color: #EEEEEE; border: 1px solid #999; text-align: left; margin: 5px 1px 5px 0px;">
                <asp:GridView ID="gvPackets" runat="server" BorderWidth="1px" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" 
                    RowStyle-BackColor="#eeeeee" RowStyle-Height="18" Width="100%" GridLines="None" ShowHeader="false"
                    Font-Names="Arial" Font-Size="Small" CellPadding="3" CellSpacing="3" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" 
                    ForeColor="Black" OnRowDeleting="gvPackets_RowDeleting" OnRowDataBound="gvPackets_RowDataBound">
                    <RowStyle VerticalAlign="Top" Font-Names="Arial" Font-Size="Small" BackColor="#F7F7F7" />
                    <FooterStyle Font-Names="Arial" Font-Size="X-Small" BackColor="White" />
                    <HeaderStyle BackColor="#666666" HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" Font-Size="Small" Font-Names="Arial" ForeColor="White" Font-Bold="False" Font-Underline="false" />
                    <AlternatingRowStyle BackColor="White" />
                    <Columns>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Left" ItemStyle-Width="0px" ItemStyle-VerticalAlign="Middle"> 
                            <ItemTemplate>
                                <%# (PayloadType)Eval("PayloadType") == PayloadType.Document ? Eval("PacketDocument.DocumentID") : Eval("PacketTag.TagId")  %>
                            </ItemTemplate> 
                        </asp:TemplateField> 
                        <asp:TemplateField ItemStyle-CssClass="dont-break-out" ItemStyle-Wrap="true" ItemStyle-VerticalAlign="Middle"> 
                            <ItemTemplate>
                                <asp:HiddenField ID="payloadType" runat="server" Value='<%# Eval("PayloadType").ToString()  %>' />
                                <asp:HiddenField ID="itemID" runat="server" Value='<%# (PayloadType)Eval("PayloadType") == PayloadType.Document ? Eval("PacketDocument.PacketDocId") : Eval("PacketTag.PacketTagId")  %>' />
                                <asp:HiddenField ID="subItemID" runat="server" Value='<%# (PayloadType)Eval("PayloadType") == PayloadType.Document ? Eval("PacketDocument.DocumentId") : Eval("PacketTag.TagId")  %>' />
                                <%# (PayloadType)Eval("PayloadType") == PayloadType.Document ? Eval("PacketDocument.Document.DocumentName") : Eval("PacketTag.Tag.TagName")  %>
                            </ItemTemplate> 
                        </asp:TemplateField> 
                        <asp:TemplateField ItemStyle-HorizontalAlign="Right" ItemStyle-Width="150px" ItemStyle-VerticalAlign="Middle"> 
                            <ItemTemplate>
                                <asp:DropDownList ID="ddFileType" DataTextField="FileType" DataValueField="FileID" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddFileType_SelectedIndexChanged" Visible="false" style="margin-bottom: 0 !important" />
                                <asp:Label ID="lblType" runat="server" Text="Tag" />
                            </ItemTemplate> 
                        </asp:TemplateField> 
                        <asp:TemplateField ItemStyle-HorizontalAlign="Right" ItemStyle-Width="24px" ItemStyle-VerticalAlign="Middle"> 
                            <ItemTemplate>
                                <asp:ImageButton ID="deleteButton" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/icons/DeleteIcon1_16px.gif" AlternateText="Remove Document" ToolTip="Remove Document" CommandName="Delete" Text="Delete" CausesValidation="false" onMouseOut="MM_swapImgRestore()" OnClientClick='<%# (PayloadType)Eval("PayloadType") == PayloadType.Document ? "return confirm(\"Are you sure you wish to remove this document?\")" : "return confirm(\"Are you sure you wish to remove this tag?\")" %>' /> 
                            </ItemTemplate> 
                        </asp:TemplateField> 
                        <asp:TemplateField ItemStyle-CssClass="dragHandle" ItemStyle-Width="20px" ItemStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Middle"> 
                            <ItemTemplate> 
                                <asp:Image ID="imgAnchor" runat="server" ImageUrl="~/DesktopModules/Gafware/DMS/Images/row-anchor.png" AlternateText="Row Anchor" ToolTip="Drag Me" Visible='<%# GetDocCount() > 1 %>' /> 
                            </ItemTemplate> 
                        </asp:TemplateField> 
                    </Columns>
                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                </asp:GridView>
            </div>
            <hr />
            <div style="float:right;margin-bottom:10px;">
                <asp:LinkButton ID="btnReset" runat="server" Text="Reset Page" CausesValidation="false" OnClick="btnReset_Click" CssClass="dnnSecondaryAction" />
                <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" CausesValidation="false" OnClick="btnCancel_Click" CssClass="dnnSecondaryAction" />
                <asp:LinkButton ID="btnSave" runat="server" Text="Save" ValidationGroup="PacketEditor" OnClick="btnSave_Click" CssClass="dnnPrimaryAction" />
            </div>
            <br style="clear: both" />
            <asp:HiddenField ID="hidFileCount" runat="server" Value="0" />
            <asp:LinkButton ID="lnkCustomHeaderPostback" runat="server" style="display: none;" CausesValidation="false" OnClick="lnkCustomHeaderPostback_Click"></asp:LinkButton>
            <asp:HiddenField ID="hidBaseUrl" runat="server" Value="" />
            <asp:Panel ID="pnlLink" runat="server" Visible="false">
                <p><strong><%= LocalizeString("LinkURL") %></strong> <span><%= LocalizeString("BasicHelp") %></span></p>
                <asp:TextBox ID="tbLinkURL" TextMode="MultiLine" ReadOnly="true" Width="100%" Rows="3" runat="server"></asp:TextBox>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
    <asp:Panel ID="pnlGrid" runat="server" style="display: none">
        <h3><%=LocalizeString("BasicSettings")%></h3>
        <div style="float: left; text-align: left; margin: 0px 0px 5px 1px;">
            <uc1:LetterFilter runat="server" OnClick="letterFilter_Click" ID="letterFilter" />
            <asp:Panel ID="pnlOwner" runat="server" style="margin-bottom: 10px;"><span style="vertical-align: middle;"><%= LocalizeString("Owner") %></span> <asp:DropDownList ID="ddOwner" DataValueField="UserId" DataTextField="DisplayName" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddOwner_SelectedIndexChanged" style="margin-bottom: 0 !important; vertical-align: middle;"></asp:DropDownList></asp:Panel>
        </div>
        <div style="float:right;margin-bottom:10px;" id="pnlAdmin" runat="server">
            <asp:LinkButton runat="server" id="backCommandButton" causesvalidation="False" CssClass="secondaryButton dmsButton" OnClick="backCommandButton_Click"><asp:label runat="server" resourcekey="backCommandButton" /></asp:LinkButton>
            <asp:LinkButton runat="server" id="newPacketCommandButton" causesvalidation="False" CssClass="secondaryButton dmsButton" OnClick="newPacketCommandButton_Click"><asp:label runat="server" resourcekey="newPacketCommandButton" /></asp:LinkButton>
            <input type="button" id="changeOwnershipCommandButton" runat="server" value="Change Ownership" class="secondaryButton dmsButton" resourcekey="changeOwnershipCommandButton" />
        </div>
        <br style="clear: both" />
        <asp:GridView ID="gv" runat="server" AutoGenerateColumns="False" CellPadding="3" CellSpacing="3" 
            EmptyDataText="<br /><strong>No packets found.</strong>" CssClass="filesList"
            ForeColor="Black" GridLines="None" DataKeyNames="PacketId" BackColor="White" BorderColor="#DEDFDE" PageSize="20" 
            BorderStyle="None" BorderWidth="1px" AllowPaging="True" AllowSorting="True" Width="100%" OnDataBound="gv_DataBound"
            OnPageIndexChanging="gv_PageIndexChanging" ShowHeader="True" OnSorting="gv_Sorting" PagerSettings-PageButtonCount="5"
            OnRowDeleting="gv_RowDeleting" OnRowEditing="gv_RowEditing" OnRowDataBound="gv_RowDataBound">
		    <Columns>
                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="100px"> 
                    <ItemTemplate>
                        <asp:LinkButton ID="editButton" runat="server" ToolTip="Edit Packet" CommandName="Edit" Text="Edit" /> 
                        &nbsp;
                        <asp:LinkButton ID="deleteButton" runat="server" ToolTip="Delete Packet" CommandName="Delete" Text="Delete" OnClientClick="if (confirm('Are you sure you want to delete this packet?')) { return true; } return false;" /> 
                    </ItemTemplate> 
                </asp:TemplateField> 
                <asp:TemplateField HeaderText="ID <img src='/desktopmodules/Gafware/DMS/Images/sortneutral.png' border='0' alt='Sort by ID' />" HeaderStyle-Wrap="false" SortExpression="PacketId" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <%# Eval("PacketId") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Packet Name <img src='/desktopmodules/Gafware/DMS/Images/sortneutral.png' border='0' alt='Sort by Packet Name' />" HeaderStyle-Wrap="false" SortExpression="Name">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkPreview" runat="server" CommandName="Preview" CommandArgument='<%# Eval("PacketId") %>' OnCommand="lnkPreview_Command"><%# Eval("Name") %></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Comments <img src='/desktopmodules/Gafware/DMS/Images/sortneutral.png' border='0' alt='Sort by Comments' />" HeaderStyle-Wrap="false" SortExpression="AdminComments" ItemStyle-Width="300px">
                    <ItemTemplate>
                        <%# GetComments(Eval("AdminComments").ToString()) %>
                    </ItemTemplate>
                </asp:TemplateField>
		    </Columns>
		    <FooterStyle BackColor="White" />
		    <RowStyle BackColor="#F7F7F7" VerticalAlign="Top" Font-Names="Arial" Font-Size="14px" />
		    <EditRowStyle VerticalAlign="Top" />
		    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" VerticalAlign="Top" />
		    <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
            <PagerTemplate>
                <table>
                    <tr>
                        <td>
                            <asp:PlaceHolder ID="ph" runat="server"></asp:PlaceHolder>
                        </td>
                    </tr>
                </table>
            </PagerTemplate>		            
		    <HeaderStyle BackColor="#666666" Font-Bold="False" ForeColor="White" HorizontalAlign="Left" Font-Size="10pt" VerticalAlign="Top" Font-Underline="false" />
		    <AlternatingRowStyle BackColor="#D0D0D0" VerticalAlign="Top" />
	    </asp:GridView>
    </asp:Panel>
    <div id="changeOwnershipDialog" class="nocontent">
        <div id="changeOwnership-content" class="dialog-content">
            <div class="dms body_padding">
                <fieldset>
                    <div class="dnnFormItem">
                        <dnn:Label ID="lblCurrentOwner" runat="server" ControlName="ddCurrentOwner" Suffix=":" /> 
                        <asp:DropDownList ID="ddCurrentOwner" runat="server" DataTextField="DisplayName" DataValueField="UserID"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddCurrentOwner" InitialValue="0" Display="Dynamic" ErrorMessage="<br />Current Owner is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="NewOwnership"></asp:RequiredFieldValidator>
                    </div>
                    <div class="dnnFormItem">
                        <dnn:Label ID="lblNewOwner" runat="server" ControlName="ddNewOwner" Suffix=":" /> 
                        <asp:DropDownList ID="ddNewOwner" runat="server" DataTextField="DisplayName" DataValueField="UserID" ValidationGroup="NewOwnership"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ddNewOwner" InitialValue="0" Display="Dynamic" ErrorMessage="<br />New Owner is required." CssClass="FormInstructions" Font-Bold="true" ForeColor="Red" ValidationGroup="NewOwnership"></asp:RequiredFieldValidator>
                    </div>
                </fieldset>
                <div style="float: right; text-align: right; margin: 5px 1px 0px 0px;">
                    <a class="dnnSecondaryAction" id="btnCancelChange" runat="server"><%= LocalizeString("Cancel") %></a>
                    <asp:LinkButton ID="btnSaveChange" runat="server" Text="Save" ValidationGroup="NewOwnership" CssClass="dnnPrimaryAction" OnClick="btnSaveChange_Click" />
                </div>
            </div>
        </div>
    </div>
</div>
<div id="previewDialog" class="nocontent dms">
    <div id="preview-content" class="dialog-content" style="overflow: auto; height: 100%; width: 100%;">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always" ChildrenAsTriggers="true" >
            <ContentTemplate>
                <uc1:DocumentSearchResults runat="server" id="documentSearchResults" Search="true" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
