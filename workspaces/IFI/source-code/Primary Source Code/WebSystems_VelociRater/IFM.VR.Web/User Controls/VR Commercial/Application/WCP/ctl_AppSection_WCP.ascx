<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_AppSection_WCP.ascx.vb" Inherits="IFM.VR.Web.ctl_AppSection_WCP" %>
<%@ Register Src="~/User Controls/Application/ctl_AppPolicyholder.ascx" TagPrefix="uc1" TagName="ctl_AppPolicyHolder" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_App_AdditionalPolicyholderList.ascx" TagPrefix="uc1" TagName="ctl_AddlPolicyholderList" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlAccidentHistoryList.ascx" TagPrefix="uc1" TagName="ctlAccidentHistoryList" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Prior_Carrier_PPA.ascx" TagPrefix="uc1" TagName="ctl_Prior_Carrier_PPA" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Billing_Info_PPA.ascx" TagPrefix="uc1" TagName="ctl_Billing_Info_PPA" %>
<%@ Register Src="~/User Controls/Application/ctl_Producer.ascx" TagPrefix="uc1" TagName="ctl_Producer" %>
<%@ Register Src="~/User Controls/Application/ctl_App_Rate.ascx" TagPrefix="uc1" TagName="ctl_App_Rate" %>
<%@ Register Src="~/User Controls/Application/ctl_Esignature.ascx" TagPrefix="uc1" TagName="ctl_Esignature" %>

<%@ Register Src="~/User Controls/VR Commercial/Application/WCP/ctl_WCP_Workplace.ascx" TagPrefix="uc1" TagName="ctl_WCP_Workplace" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/WCP/ctl_WCP_NamedIndividual.ascx" TagPrefix="uc1" TagName="ctl_WCP_NamedIndiv" %>

<div id="div_master_wcp_app" runat="server">
    <h3>
        Application
        <span style="float: right;">
            <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <div>
            <uc1:ctl_AppPolicyHolder runat="server" ID="ctl_PH" />
            <uc1:ctl_AddlPolicyholderList runat="server" ID="ctl_AddlPolicyholderList" />
        </div>

        <div id="divWorkplaces" runat="server">
            <h3>
                <asp:Label ID="lblWorkplacesAccordHeader" runat="server" Text="Workplace Addresses"></asp:Label>
                 <span style="float: right;">
                     <asp:LinkButton ID="lbAddNewWorkplace" CssClass="RemovePanelLink" ToolTip="Add a new workplace address" runat="server">Add New Workplace</asp:LinkButton>
                     <asp:LinkButton ID="lbSaveWorkplaces" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
                 </span>
            </h3>
            <div runat="server" id="divWorkplacesList">
                <asp:Repeater ID="rptWorkplaces" runat="server">
                    <ItemTemplate>
                        <uc1:ctl_WCP_Workplace id="ctl_Workplaces" runat="server" />
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <div id="divNamedIndividuals" runat="server">
            <h3>
                Named Individuals
                 <span style="float: right;">
                     <asp:LinkButton ID="lnkSaveNI" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
                 </span>
            </h3>
            <div>
                <div runat="server" id="divInclOfSoleProprietersEtc">
                    <asp:Repeater ID="rptInclOfSoleProprieters" runat="server">
                        <ItemTemplate>
                            <uc1:ctl_WCP_NamedIndiv id="ctl_NI_InclOfSoleProprietersEtc" runat="server" />
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <div runat="server" id="divWaiverOfSubro">
                    <asp:Repeater ID="rptWaiverOfSubro" runat="server">
                        <ItemTemplate>
                            <uc1:ctl_WCP_NamedIndiv id="ctl_NI_WaiverOfSubro" runat="server" />
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <div runat="server" id="divExclOfAmish">
                    <asp:Repeater ID="rptExclOfAmish" runat="server">
                        <ItemTemplate>
                            <uc1:ctl_WCP_NamedIndiv id="ctl_NI_ExclOfAmish" runat="server" />
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <div runat="server" id="divExclOfSoleOfficer">
                    <asp:Repeater ID="rptExclOfSoleOfficer" runat="server">
                        <ItemTemplate>
                            <uc1:ctl_WCP_NamedIndiv id="ctl_NI_ExclOfSoleOfficer" runat="server" />
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <div runat="server" id="divExclOfSoleProprietorsEtc_IL">
                    <asp:Repeater ID="rptExclOfSoleProprietorsEtc_IL" runat="server">
                        <ItemTemplate>
                            <uc1:ctl_WCP_NamedIndiv id="ctl_NI_ExclOfSoleProprietor_IL" runat="server" />
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <div runat="server" id="divRejectionOfCoverageEndorsement">
                    <asp:Repeater ID="rptRejectionOfCoverageEndorsement" runat="server">
                        <ItemTemplate>
                            <uc1:ctl_WCP_NamedIndiv id="ctl_NI_RejectionOfCoverageEndorsement" runat="server" />
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>

        <uc1:ctlAccidentHistoryList runat="server" ID="ctlAccidentHistoryList" />
        <uc1:ctl_Prior_Carrier_PPA runat="server" ID="ctl_Prior_CarrierPPA" />        
        <uc1:ctl_Billing_Info_PPA runat="server" ID="ctl_Billing_Info_PPA" />
        <uc1:ctl_Esignature runat="server" ID="ctl_Esignature" />
        <uc1:ctl_Producer runat="server" ID="ctlProducer" />
        <uc1:ctl_App_Rate runat="server" ID="ctlApp_Rate" />
    </div>
</div>
<asp:HiddenField ID="hdn_master_wcp_app" runat="server" />
<asp:HiddenField ID="hdnAccordWorkplace" runat="server" />
<asp:HiddenField ID="hdnAccordWorkplaceList" runat="server" />
<asp:HiddenField ID="hdnAccordNamedIndiv" runat="server" />
<asp:HiddenField ID="hdnInclSoleProprieterList" runat="server" />
<asp:HiddenField ID="hdnWaiverOfSubroList" runat="server" />
<asp:HiddenField ID="hdnExclOfAmishList" runat="server" />
<asp:HiddenField ID="hdnExclOfSoleOfficerList" runat="server" />
<asp:HiddenField ID="hdnExclOfSoleProprietorList_IL" runat="server" />
<asp:HiddenField ID="hdnRejectionOfCoverageEndorsement" runat="server" />
