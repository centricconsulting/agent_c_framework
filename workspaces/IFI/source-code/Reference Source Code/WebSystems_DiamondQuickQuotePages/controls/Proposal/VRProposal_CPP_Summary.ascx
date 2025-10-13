<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_CPP_Summary.ascx.vb" Inherits="controls_Proposal_VRProposal_CPP_Summary" %>

<%@ Register src="VRProposal_CGL_Summary.ascx" tagname="VRProposal_CGL_Summary" tagprefix="VR" %>
<%@ Register src="VRProposal_CPR_Summary.ascx" tagname="VRProposal_CPR_Summary" tagprefix="VR" %>
<%@ Register src="VRProposal_CIM_Summary.ascx" tagname="VRProposal_CIM_Summary" tagprefix="VR" %>
<%@ Register src="VRProposal_CRM_Summary.ascx" tagname="VRProposal_CRM_Summary" tagprefix="VR" %>
<%--added GAR control 4/22/2017--%>
<%@ Register src="VRProposal_GAR_Summary.ascx" tagname="VRProposal_GAR_Summary" tagprefix="VR" %>

<br />
<table width="100%" class="quickQuoteSectionTable">
    <tr>
        <td colspan="2" class="tableFieldHeaderLarger">
            Commercial Policy Package - <asp:Label runat="server" ID="lblQuoteNumber"></asp:Label>
        </td>
    </tr>
    <tr id="trCommercialProperty" runat="server">
        <td colspan="2" align="center">
            <VR:VRProposal_CPR_Summary runat="server" ID="CPR_Summary" />
            <%--<table width="100%" class="quickQuoteSectionTable">
                <tr>
                    <td align="left" colspan="2" class="tableFieldHeader">
                        Commercial Property
                    </td>
                </tr>
                <tr runat="server" id="BuildingRow">
                    <td align="left" class="tableFieldValue">
                        Building Coverage
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblBuildingPremium"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="PersPropRow">
                    <td align="left" class="tableFieldValue">
                        Personal Property Coverage
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblPersPropPremium"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="PersPropOfOthersRow">
                    <td align="left" class="tableFieldValue">
                        Personal Property Of Others
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblPersPropOfOthersPremium"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="BusIncRow">
                    <td align="left" class="tableFieldValue">
                        Business Income Coverage
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblBusIncPremium"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="PropInTheOpenRow">
                    <td align="left" class="tableFieldValue">
                        Property in the Open
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblPropInTheOpenPremium"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="EnhEnd_CprRow">
                    <td align="left" class="tableFieldValue">
                        <asp:Label ID="lblCPREnhancementEndorsementText" runat="server" Text="Enhancement Endorsement" ></asp:Label>
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblEnhEndPremium_Cpr"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="EbRow">
                    <td align="left" class="tableFieldValue">
                        Equipment Breakdown
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblEbPremium"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="EqRow">
                    <td align="left" class="tableFieldValue">
                        Earthquake
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblEqPremium"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="trCPRAmountToMeetMinimumPremiumRow">
                    <td align="left" class="tableFieldValue">
                        Amount to Equal Minimum Premium - (<asp:Label runat="server" ID="lblCPRAmountToMeetMinimumAmountText"></asp:Label>)
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblCPRAmountToEqualMinimumPremium"></asp:Label>
                    </td>
                </tr>
                <tr style="border-top: 1px solid black;">
                    <td align="left" class="tableFieldValue">
                        Property Total Premium
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblCprPremium"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;</td>
                </tr>
            </table>--%>
        </td>
    </tr>
    <tr runat="server" id="trPageBreak_between_CPR_and_CGL" visible="false">
        <td colspan="2">
            <asp:PlaceHolder runat="server" ID="phPageBreak_between_CPR_and_CGL"></asp:PlaceHolder>
        </td>
    </tr>
    <tr id="trGeneralLiability" runat="server">
        <td colspan="2" align="center">
            <VR:VRProposal_CGL_Summary runat="server" ID="CGL_Summary" />
            <%--<table width="100%" class="quickQuoteSectionTable">
                <tr>
                    <td align="left" colspan="2" class="tableFieldHeader">
                        Commercial General Liability
                    </td>
                </tr>
                <tr runat="server" id="EnhEnd_CglRow">
                    <td align="left" class="tableFieldValue">
                        <asp:Label ID="lblCGLEnhancementEndorsementText" runat="server" Text="Enhancement Endorsement" ></asp:Label>
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblEnhEndPrem_Cgl"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="PremOpsRow">
                    <td align="left" class="tableFieldValue">
                        Premises/Operations
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblPremOps"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="ProdCompOpsRow">
                    <td align="left" class="tableFieldValue">
                        Products/Completed Operations
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblProdCompOps"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="OptCovsRow">
                    <td align="left" class="tableFieldValue">
                        Optional Coverages
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblOptCovs"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="MinPrem_PremRow">
                    <td align="left" class="tableFieldValue">
                        Amount to Equal Minimum Premium (Premises) - (<asp:Label runat="server" ID="lblMinPrem_Prem"></asp:Label>)
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblAmtForMinPrem_Prem"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="MinPrem_ProdRow">
                    <td align="left" class="tableFieldValue">
                        Amount to Equal Minimum Premium (Products) - (<asp:Label runat="server" ID="lblMinPrem_Prod"></asp:Label>)
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblAmtForMinPrem_Prod"></asp:Label>
                    </td>
                </tr>
                <tr style="border-top: 1px solid black;">
                    <td align="left" class="tableFieldValue">
                        General Liability Total Premium
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblCglPremium"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;</td>
                </tr>
            </table>--%>
        </td>
    </tr>
    <tr runat="server" id="trPageBreak_between_CGL_and_GAR" visible="false">
        <td colspan="2">
            <asp:PlaceHolder runat="server" ID="phPageBreak_between_CGL_and_GAR"></asp:PlaceHolder>
        </td>
    </tr>
    <tr id="trCommercialGarage" runat="server">
        <td colspan="2" align="center">
            <VR:VRProposal_GAR_Summary runat="server" ID="GAR_Summary" />            
        </td>
    </tr>
    <%--<tr runat="server" id="trPageBreak_between_CGL_and_CIM" visible="false">
        <td colspan="2">
            <asp:PlaceHolder runat="server" ID="phPageBreak_between_CGL_and_CIM"></asp:PlaceHolder>
        </td>
    </tr>--%>
    <tr runat="server" id="trPageBreak_between_GAR_and_CIM" visible="false">
        <td colspan="2">
            <asp:PlaceHolder runat="server" ID="phPageBreak_between_GAR_and_CIM"></asp:PlaceHolder>
        </td>
    </tr>
    <tr id="trInlandMarine" runat="server">
        <td colspan="2" align="center">
            <VR:VRProposal_CIM_Summary runat="server" ID="CIM_Summary" />
            <%--<table width="100%" class="quickQuoteSectionTable">
                <tr>
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
                        INCLUDED
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
                <tr runat="server" id="trCIMAmountToMeetMinimumRow">
                    <td align="left" class="tableFieldValue">
                        <asp:Label ID="lblCIMAmountToMeetMinimumText" runat="server" Text="Amount to Equal Minimum Premium"></asp:Label>
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblCIMAmountToMeetMinimumAmount"></asp:Label>
                    </td>
                </tr>
                <tr style="border-top: 1px solid black;">
                    <td align="left" class="tableFieldValue">
                        Inland Marine Total Premium
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblCIMTotalPremium"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;</td>
                </tr>
            </table>--%>
        </td>
    </tr>
    <tr runat="server" id="trPageBreak_between_CIM_and_CRM" visible="false">
        <td colspan="2">
            <asp:PlaceHolder runat="server" ID="phPageBreak_between_CIM_and_CRM"></asp:PlaceHolder>
        </td>
    </tr>
    <tr id="trCommercialCrime" runat="server">
        <td colspan="2" align="center">
            <VR:VRProposal_CRM_Summary runat="server" ID="CRM_Summary" />
            <%--<table width="100%" class="quickQuoteSectionTable">
                <tr>
                    <td align="left" colspan="2" class="tableFieldHeader">
                        Commercial Crime
                    </td>
                </tr>
                <tr runat="server" id="trCRMEmployeeTheft">
                    <td align="left" class="tableFieldValue">
                        Employee Theft
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblCRMEmployeeTheft"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="trCRMInsidePremises">
                    <td align="left" class="tableFieldValue">
                        Inside the Premises - Money and Securities
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblCRMInsidePremises"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="trCRMOutsideThePremises">
                    <td align="left" class="tableFieldValue">
                        Outside the Premises
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblCRMOutsideThePremises"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="trCRMAmtToEqualMinumum">
                    <td align="left" class="tableFieldValue">
                        <asp:Label ID="lblCRMAmtToEqualMinPremiumText" runat="server" Text="Amount to Equal Minimum Premium"></asp:Label>
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblCRMAmtToEqualMinPremiumAmount"></asp:Label>
                    </td>
                </tr>
                <tr style="border-top: 1px solid black;">
                    <td align="left" class="tableFieldValue">
                        Commercial Crime Total Premium
                    </td>
                    <td align="right" class="tableFieldValue">
                        <asp:Label runat="server" ID="lblCRMTotalPremium"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;</td>
                </tr>
            </table>--%>
        </td>
    </tr>
    <%--<tr runat="server" id="trPageBreak_between_CRM_and_GAR" visible="false">
        <td colspan="2">
            <asp:PlaceHolder runat="server" ID="phPageBreak_between_CRM_and_GAR"></asp:PlaceHolder>
        </td>
    </tr>
    <tr id="trCommercialGarage" runat="server">
        <td colspan="2" align="center">
            <VR:VRProposal_GAR_Summary runat="server" ID="GAR_Summary" />            
        </td>
    </tr>--%>
    <tr style="border-top: 1px solid black;">
        <td class="tableFieldHeader">
            Total Package Premium Due
        </td>
        <td align="right" class="tableFieldValue">
            <asp:Label runat="server" ID="lblTotalPremium"></asp:Label>
        </td>
    </tr>
</table>
<br />