<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_Location_Coverages.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_Location_Coverages" %>

<div runat="server" id="divMain">
    <h3>
        Optional Location Coverages
         <span style="float: right;">
            <asp:LinkButton ID="lnkClear" runat="server" CssClass="RemovePanelLink" Style="margin-left: 20px" ToolTip="Clear Location Level Coverages">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSave" ToolTip="Save" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div id="divContent" runat="server">
        <table id="tblLocationCoverages" runat="server" style="width:100%;">
            <tr>
                <td>
                    <asp:CheckBox id="chkEquipmentBreakdown" runat="server" Text="Equipment Breakdown" />
                </td>
            </tr>
            <tr runat="server" id="trEB_MBR_Ineligible" visible="false"><%--added 11/13/2017; tr visibility and lbl text controlled by code-behind--%>
                <td class="informationalText">
                    <asp:Label runat="server" ID="lblEB_MBR_Ineligible" Text="Underwriting Approval required for Equipment Breakdown on this risk. Continue building quote then contact Underwriting for Equipment Breakdown quote."></asp:Label>
                </td>
            </tr>
            <tr id="trEquipmentBreakdownRow" runat="server" style="display:none;">
                <td>
                    <table id="tblEquipBrkdwn" runat="server" style="width:100%;">
                        <tr>
                            <td class="ctl_BOP_Location_Coverages_DetailLabel">
                                &nbsp;&nbsp;
                                Deductible
                            </td>
                            <td class="DetailData">
                                <asp:DropDownList ID="ddlEquipmentBreakdownDeductible" runat="server" Width="150px">
                                    <asp:ListItem selected="True" Text="250" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="500" Value="8"></asp:ListItem>
                                    <asp:ListItem Text="1,000" Value="9"></asp:ListItem>
                                    <asp:ListItem Text="2,500" Value="15"></asp:ListItem>
                                    <asp:ListItem Text="5,000" Value="16"></asp:ListItem>
                                    <asp:ListItem Text="10,000" Value="17"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox id="chkMoneySecuritiesONPremises" runat="server" Text="Money and Securities/On Premises" />
                </td>
            </tr>
            <tr id="trMoneySecuritiesONPremisesRow" runat="server" style="display:none;">
                <td>
                    <table id="tblMoneySecuritiesOnPremises" runat="server" style="width:100%;">
                        <tr>
                            <td class="ctl_BOP_Location_Coverages_DetailLabel">
                                &nbsp;&nbsp;
                                Total Limit
                            </td>
                            <td class="DetailData">
                                <asp:TextBox ID="txtMoneySecuritiesONPremisesLimit" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trMoneyAndSecuritiesOnPremisesInfoText" runat="server">
                            <td colspan="2" class="informationalText">
                                The BOP Enhancement Endorsement includes coverage for Money and Securities coverage with limits of $10,000 on-premises. Please enter the total amount of limits greater than $10,000 for on-premises coverage.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr><td>&nbsp;</td></tr>
            <tr>
                <td>
                    <asp:CheckBox id="chkMoneySecuritiesOFFPremises" runat="server" Text="Money and Securities/Off Premises" />
                </td>
            </tr>
            <tr id="trMoneySecuritiesOFFPremisesRow" runat="server" style="display:none;">
                <td>
                    <table id="tblMoneySecuritiesOFFPremises" runat="server" style="width:100%;">
                        <tr>
                            <td class="ctl_BOP_Location_Coverages_DetailLabel">
                                &nbsp;&nbsp;
                                Total Limit
                            </td>
                            <td class="DetailData">
                                <asp:TextBox ID="txtMoneySecuritiesOFFPremisesLimit" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trMoneyAndSecuritiesOffPremisesInfoText" runat="server">
                            <td colspan="2" class="informationalText">
                                The BOP Enhancement Endorsement includes coverage for Money and Securities coverage with limits of $5,000 off-premises. Please enter the total amount of limits greater than $5,000 for off-premises coverage.
                            </td>
                        </tr>
                        <tr><td>&nbsp;</td></tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox id="chkOutdoorSigns" runat="server" Text="Outdoor Signs" />
                </td>
            </tr>
            <tr id="trOutdoorSignsRow" runat="server" style="display:none;">
                <td>
                    <table id="tblOutdoorSigns" runat="server" style="width:100%;">
                        <tr>
                            <td class="ctl_BOP_Location_Coverages_DetailLabel">
                                &nbsp;&nbsp;
                                Total Limit
                            </td>
                            <td class="DetailData">
                                <asp:TextBox ID="txtOutdoorSignsLimit" runat="server" onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trOutdoorSignsInfoText" runat="server">
                            <td colspan="2" class="informationalText">
                                The BOP Enhancement Endorsement includes coverage for Outdoor Signs coverage with limits of $10,000 on-premises. Please enter the total amount of limits greater than $10,000.
                            </td>
                        </tr>
                        <tr><td>&nbsp;</td></tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                Wind Hail Deductible % 
                                <asp:DropDownList ID="ddlWindHail" runat="server" Width="150px"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="informationalText">
                                Wind/Hail deductibles are applied per building in the event of a loss.
                            </td>
                        </tr>
                        <tr><td>&nbsp;</td></tr>
                    </table>
                </td>
            </tr>
            <tr id="trRedInfoText" runat="server">
                <td class="informationalTextRed">
                    *Other optional coverages are available. Please contact your Commercial Underwriter for assistance and approval.
                </td>
            </tr>
        </table>
        <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red"></asp:Label>
    </div>
    <asp:HiddenField ID="hdnAccord" runat="server" />
</div>