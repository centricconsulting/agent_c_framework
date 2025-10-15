<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_App_Location.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_App_Location" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/BOP/ctl_BOP_App_BuildingList.ascx" TagPrefix="uc1" TagName="ctl_BOP_App_BuildingList" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Location #0 - "></asp:Label>
        <span style="float: right;">
        <asp:LinkButton ID="lnkSave" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
    </span>
</h3>
<div>
    <style>
        .LOCLabelColumn {
            width:32%;
            text-align:left;
        }
        .LOCDataColumn {
            /*width:10%;*/
            text-align:left;
        }
        .LOCUITextBox {
            width:75%;
        }
    </style>
    <%--6/8/2017 - moved table to ctl_BOP_App_Building--%>
    <%--<table>
        <tr>
            <td class="LOCLabelColumn">
                *Square Feet
            </td>
            <td class="LOCDataColumn">
                <asp:TextBox ID="txtSquareFeet" runat="server" CssClass="LOCUITextBox" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
            </td>
            <td class="LOCLabelColumn">
                *Year Built
            </td>
            <td class="LOCDataColumn">
                <asp:TextBox ID="txtYearBuilt" runat="server" CssClass="LOCUITextBox" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="LOCLabelColumn">
                *Year Roof Updated
            </td>
            <td class="LOCDataColumn">
                <asp:TextBox ID="txtYearRoofUpdated" runat="server" CssClass="LOCUITextBox" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
            </td>
            <td class="LOCLabelColumn">
                *Year Wiring Updated
            </td>
            <td class="LOCDataColumn">
                <asp:TextBox ID="txtYearWiringUpdated" runat="server" CssClass="LOCUITextBox" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="LOCLabelColumn">
                *Year Plumbing Updated
            </td>
            <td class="LOCDataColumn">
                <asp:TextBox ID="txtYearPlumbingUpdated" runat="server" CssClass="LOCUITextBox" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
            </td>
            <td class="LOCLabelColumn">
                *Year Heat Updated
            </td>
            <td class="LOCDataColumn">
                <asp:TextBox ID="txtYearHeatUpdated" runat="server" CssClass="LOCUITextBox" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
            </td>
        </tr>
    </table>--%>
    <uc1:ctl_BOP_App_BuildingList runat="server" ID="ctl_BOP_App_BuildingList" />
</div>
