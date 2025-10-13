<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_CIM_Summary.ascx.vb" Inherits="controls_Proposal_VRProposal_CIM_Summary" %>
<br runat="server" id="startBreak" />
<table width="100%" class="quickQuoteSectionTable">
    <tr runat="server" id="Monoline_header_Row" visible="true">
        <td align="left" colspan="2" class="tableFieldHeaderLarger">
            Commercial Inland Marine<span runat="server" id="quoteNumberSection"> - <asp:Label runat="server" ID="lblQuoteNumber"></asp:Label></span>
        </td>
    </tr>
    <tr runat="server" id="CPP_header_Row" visible="false">
        <td align="left" colspan="2" class="tableFieldHeader">
            Commercial Inland Marine
        </td>
    </tr>
    <tr runat="server" id="trCIMBuildersRisk">
        <td align="left" class="tableFieldValue">
            Builders Risk
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCIMBuildersRisk"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCIMComputer">
        <td align="left" class="tableFieldValue">
            Computer
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCIMComputer"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCIMContractorsEquipment">
        <td align="left" class="tableFieldValue">
            Contractors Equipment
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCIMContractorsEquipment"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCIMEquipmentLeasedRentedFromOthers">
        <td align="left" class="tableFieldValue">
            Equipment Leased/Rented From Others
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCIMEquipmentLeasedRentedFromOthers"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCIMUnscheduledTools">
        <td align="left" class="tableFieldValue">
            Unscheduled Tools
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCIMUnscheduledTools"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCIMEnhancementEndorsement">
        <td align="left" class="tableFieldValue">
            Contractors Equipment Enhancement Endorsement
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCIMEnhancementEndorsementText" Text="INCLUDED"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCIMFineArtsFloater">
        <td align="left" class="tableFieldValue">
            Fine Arts Floater
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCIMFineArtsFloater"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCIMInstallationFloater">
        <td align="left" class="tableFieldValue">
            Installation Floater
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCIMInstallationFloater"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCIMMotorTruckCargo">
        <td align="left" class="tableFieldValue">
            Motor Truck Cargo - Scheduled
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCIMMotorTruckCargo"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCIMOwnersCargo">
        <td align="left" class="tableFieldValue">
            Owners Cargo
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCIMOwnersCargo"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCIMSchedulePropertyFloater">
        <td align="left" class="tableFieldValue">
            Scheduled Property Floater
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCIMScheduledPropertyFloater"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCIMSigns">
        <td align="left" class="tableFieldValue">
            Signs
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCIMSigns"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCIMTransportation">
        <td align="left" class="tableFieldValue">
            Transportation
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCIMTransportation"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCIMGolfCart">
        <td align="left" class="tableFieldValue">
            Golf Cart
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCIMGolfCart"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCIMGolfCourse">
        <td align="left" class="tableFieldValue">
            Golf Course
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCIMGolfCourse"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="trCIMAmountToMeetMinimumRow">
        <td align="left" class="tableFieldValue">
            Amount to Equal Minimum Premium - (<asp:Label runat="server" ID="lblCIMAmountToMeetMinimumText"></asp:Label>)
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblCIMAmountToMeetMinimumAmount"></asp:Label>
        </td>
    </tr>   
    <tr runat="server" id="OptCovsRow">
        <td class="tableFieldValue">
            Additional Coverages
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblOptCovsPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="CommentsRow" visible="false">
        <td colspan="2" class="tableFieldValue">
            <asp:Label runat="server" ID="lblComments"></asp:Label>
        </td>
    </tr>
    <tr style="border-top: 1px solid black;">
        <td align="left" class="tableFieldHeader">
            <asp:Label runat="server" ID="lblPremiumText" Text="Total Premium Due"></asp:Label>
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblTotalPremium"></asp:Label>
        </td>
    </tr>
    <tr runat="server" id="SpacerRow">
        <td colspan="2">&nbsp;</td>
    </tr>
</table>
<br runat="server" id="endBreak" />