<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_App_Building.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_App_Building" %>

<h3>
    <asp:Label ID="lblAccordHeader" runat="server" Text="Building - "></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkSave" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
    </span>
</h3>

<div>
    <style>
        /*re-named from BLDLabelColumn 6/8/2017... so we could use that for the stuff that came over from ctl_BOP_App_Location*/
        .BLDAILabelColumn {
            width:30%;
            text-align:right;
        }
        .BldDDL {
            width:50%;
        }
        /*added 6/8/2017; originally came from ctl_BOP_App_Location and then re-named*/
        .BLDLabelColumn {
            width:32%;
            text-align:left;
        }
        .BLDDataColumn {
            /*width:10%;*/
            text-align:left;
        }
        .BLDUITextBox {
            width:75%;
        }
    </style>
    <%--6/8/2017 - added table from ctl_BOP_App_Location--%>
    <table>
        <tr id="trNewCo" runat="server" style="width:100%;">
            <td colspan="4">
                <table style="width:100%;">
                    <tr>
                        <td>*Square Feet</td>
                        <td><asp:TextBox ID="txtSquareFeetNewCo" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57' Width="40px"></asp:TextBox></td>
                        <td>*Year Built</td>
                        <td><asp:TextBox ID="txtYearBuiltNewCo" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57' Width="40px"></asp:TextBox></td>
                        <td>*Number of Stories</td>
                        <td><asp:TextBox ID="txtNumOfStoriesNewCo" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57' Width="40px"></asp:TextBox></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr id="trOldCo" runat="server">
            <td class="BLDLabelColumn">
                *Square Feet
            </td>
            <td class="BLDDataColumn">
                <asp:TextBox ID="txtSquareFeet" runat="server" CssClass="BLDUITextBox" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
            </td>
            <td class="BLDLabelColumn">
                *Year Built
            </td>
            <td class="BLDDataColumn">
                <asp:TextBox ID="txtYearBuilt" runat="server" CssClass="BLDUITextBox" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="BLDLabelColumn">
                *Year Roof Updated
            </td>
            <td class="BLDDataColumn">
                <asp:TextBox ID="txtYearRoofUpdated" runat="server" CssClass="BLDUITextBox" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
            </td>
            <td class="BLDLabelColumn">
                *Year Wiring Updated
            </td>
            <td class="BLDDataColumn">
                <asp:TextBox ID="txtYearWiringUpdated" runat="server" CssClass="BLDUITextBox" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="BLDLabelColumn">
                *Year Plumbing Updated
            </td>
            <td class="BLDDataColumn">
                <asp:TextBox ID="txtYearPlumbingUpdated" runat="server" CssClass="BLDUITextBox" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
            </td>
            <td class="BLDLabelColumn">
                *Year Heat Updated
            </td>
            <td class="BLDDataColumn">
                <asp:TextBox ID="txtYearHeatUpdated" runat="server" CssClass="BLDUITextBox" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
            </td>
        </tr>
    </table>
    <br />
    <table style="width:100%;">
        <tr>
            <td>
                <asp:Label ID="lblBuildingLimit" runat="server" Text="Building Limit: $xx,xxx,xxx" Font-Bold="true"></asp:Label>
                <br />
                <table style="width:100%">
                    <tr>
                        <td class="BLDAILabelColumn">
                            &nbsp;&nbsp;
                            Loss Payee Name
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlBuildingLimitLossPayeeName" runat="server" CssClass="BldDDL"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="BLDAILabelColumn">
                            &nbsp;&nbsp;
                            Loss Payee Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlBuildingLimitLossPayeeType" runat="server" CssClass="BldDDL">
                                <asp:ListItem Value="65">Loss Payable</asp:ListItem>
                                <asp:ListItem Value="66">Lenders Loss Payable</asp:ListItem>
                                <asp:ListItem Value="67">Contract of Sale</asp:ListItem>
                                <asp:ListItem Value="42">First Mortgagee</asp:ListItem>
                                <asp:ListItem Value="78">Loss Payable - Building Owner</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="BLDAILabelColumn">
                            &nbsp;&nbsp;
                            ATIMA
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlBuildingLimitATMA" runat="server" CssClass="BldDDL">
                                <asp:ListItem Value="0">None</asp:ListItem>
                                <asp:ListItem Value="1">ATIMA</asp:ListItem>
                                <asp:ListItem Value="2">ISAOA</asp:ListItem>
                                <asp:ListItem Value="3">ATIMA/ISAOA</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr><td colspan="2">&nbsp;</td></tr>
        <tr>
            <td>
                <asp:Label ID="lblPersonalPropertyLimit" runat="server" Text="Personal Property Limit: $xx,xxx,xxx" Font-Bold="true"></asp:Label>
                <br />
                <table style="width:100%">
                    <tr>
                        <td class="BLDAILabelColumn">
                            &nbsp;&nbsp;
                            Loss Payee Name
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlPersonalPropertyLimitLossPayeeName" runat="server" CssClass="BldDDL"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="BLDAILabelColumn">
                            &nbsp;&nbsp;
                            Loss Payee Type
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlPersonalPropertyLimitLossPayeeType" runat="server" CssClass="BldDDL">
                                <asp:ListItem Value="65">Loss Payable</asp:ListItem>
                                <asp:ListItem Value="66">Lenders Loss Payable</asp:ListItem>
                                <asp:ListItem Value="67">Contract of Sale</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="BLDAILabelColumn">
                            &nbsp;&nbsp;
                            ATIMA
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlPersonalPropertyLimitATMA" runat="server" CssClass="BldDDL">
                                <asp:ListItem Value="0">None</asp:ListItem>
                                <asp:ListItem Value="1">ATIMA</asp:ListItem>
                                <asp:ListItem Value="2">ISAOA</asp:ListItem>
                                <asp:ListItem Value="3">ATIMA/ISAOA</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>

        </tr>
    </table>
</div>