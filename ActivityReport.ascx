<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivityReport.ascx.cs" Inherits="Gafware.Modules.DMS.ActivityReport" %>
<%@ Register Assembly="Telerik.ReportViewer.WebForms, Version=15.2.21.1125, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" Namespace="Telerik.ReportViewer.WebForms" TagPrefix="telerik" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
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
    .RadPicker {
        padding-top: 9px;
    }
</style>
<div class="se-pre-con"></div>
<div style="padding: 10px;">
    <div class="dms" style="width: 900px; padding: 5px;">
        <h3><%=LocalizeString("BasicSettings")%></h3>
        <div id="pnlBack" runat="server" style="margin-top: 9px; float: right;">
            <asp:LinkButton ID="btnBack" runat="server" Text="Back" CssClass="dnnSecondaryAction" OnClick="btnBack_Click" CausesValidation="false" />
        </div>
        <div style="float: left; text-align: left;">
            <div style="display: inline-block">
                <span style="vertical-align: middle; font-weight: bold;"><%= LocalizeString("FromDate") %></span>
                <asp:TextBox textmode="Date" id="dtFrom" runat="server" ValidationGroup="Report" />
<%--                <telerik:RadDatePicker ID="dtFrom" runat="server" DateInput-EmptyMessage="From Date" MinDate="01/01/1000" MaxDate="01/01/3000" ValidationGroup="Report" Width="213px">
                    <Calendar ID="Calendar1" runat="server">
                        <SpecialDays>
                            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                        </SpecialDays>
                    </Calendar>
                </telerik:RadDatePicker>--%>
            </div>
            <div style="display: inline-block">
                <span style="vertical-align: middle; font-weight: bold;"><%= LocalizeString("ToDate") %></span>
                <asp:TextBox textmode="Date" id="dtTo" runat="server" ValidationGroup="Report" />
<%--                <telerik:RadDatePicker ID="dtTo" runat="server" DateInput-EmptyMessage="To Date" MinDate="01/01/1000" MaxDate="01/01/3000" ValidationGroup="Report" Width="213px">
                    <Calendar ID="Calendar2" runat="server">
                        <SpecialDays>
                            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                        </SpecialDays>
                    </Calendar>
                </telerik:RadDatePicker>--%>
            </div>
            <div style="display: inline-block; top: 7px; position: relative;">
                <asp:LinkButton ID="btnGo" runat="server" Text="Go" CssClass="dnnSecondaryAction" CausesValidation="true" ValidationGroup="Report" OnClick="btnGo_Click" />
            </div>
        </div>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" CssClass="red-text" ErrorMessage="<br />From Date is required." ValidationGroup="Report" ControlToValidate="dtFrom"></asp:RequiredFieldValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" CssClass="red-text" ErrorMessage="<br />To Date is required." ValidationGroup="Report" ControlToValidate="dtTo"></asp:RequiredFieldValidator>
        <asp:CompareValidator ID="CompareValidator1" runat="server" Display="Dynamic" CssClass="red-text" ErrorMessage="<br />To Date must be greater than From Date." ValidationGroup="Report" ControlToCompare="dtFrom" ControlToValidate="dtTo" Operator="GreaterThanEqual"></asp:CompareValidator>
    </div>
    <br style="clear: both;" />
    <div style="border: 1px solid black; padding: 5px; width: 100%; max-width: 912px; height: 712px; overflow: auto;">
        <telerik:reportviewer runat="server" id="report1" width="900px" height="700px"></telerik:reportviewer>
    </div>
</div>