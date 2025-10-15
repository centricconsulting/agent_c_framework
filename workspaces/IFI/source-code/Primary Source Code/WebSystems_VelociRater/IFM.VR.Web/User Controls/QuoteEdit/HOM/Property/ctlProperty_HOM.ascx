<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlProperty_HOM.ascx.vb" Inherits="IFM.VR.Web.ctlProperty_HOM" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlAccidentHistoryList.ascx" TagPrefix="uc1" TagName="ctlAccidentHistoryList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlProperty_Address.ascx" TagPrefix="uc1" TagName="ctlProperty_Address" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlResidence.ascx" TagPrefix="uc1" TagName="ctlResidence" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlMobileHome.ascx" TagPrefix="uc1" TagName="ctlMobileHome" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlProtectionClass_HOM.ascx" TagPrefix="uc1" TagName="ctlProtectionClass_HOM" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlPropertyAdditionalQuestions.ascx" TagPrefix="uc1" TagName="ctlPropertyAdditionalQuestions" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_AdditionalInterest_MiniSerach.ascx" TagPrefix="uc1" TagName="ctl_AdditionalInterest_MiniSerach" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlVehicleAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctlVehicleAdditionalInterestList" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Billing_Info_PPA.ascx" TagPrefix="uc1" TagName="ctl_Billing_Info_PPA" %>


<style>
    #tblAdditionalQuestions tr {
        height: 50px;
    }

    #tblAdditionalQuestions td {
        padding-left: 15px;
    }

    #tblAdditionalQuestions tr:nth-child(odd) {
        background-color: lightgray;
    }
</style>

<div id="PolicyInfoDiv" runat="server">
    <h3>
        <asp:Label ID="lblMainHeader" runat="server" Text="Property"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearGeneralInfo" runat="server" ToolTip="Clear Property Page" CssClass="RemovePanelLink">Clear Page</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveGeneralInfo" runat="server" ToolTip="Save Page" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div>

        <uc1:ctlProperty_Address runat="server" MyLocationIndex="0" ID="ctlProperty_Address" />
        <uc1:ctlResidence runat="server" MyLocationIndex="0" ID="ctlResidence" />
        <uc1:ctlMobileHome runat="server" MyLocationIndex="0" ID="ctlMobileHome" />
        <uc1:ctlProtectionClass_HOM runat="server" MyLocationIndex="0" ID="ctlProtectionClass_HOM" />

        <uc1:ctlAccidentHistoryList runat="server" ID="ctlAccidentHistoryList" DriverIndex="0" />
        <uc1:ctl_AdditionalInterest_MiniSerach runat="server" ID="ctl_AdditionalInterest_MiniSerach" Visible="false" />
        <uc1:ctlVehicleAdditionalInterestList runat="server" ID="ctlVehicleAdditionalInterestList" Visible="false" />
        <uc1:ctl_Billing_Info_PPA runat="server" ID="ctl_Billing_Info_PPA" Visible="false" />
        <uc1:ctlPropertyAdditionalQuestions runat="server" MyLocationIndex="0" ID="ctlPropertyAdditionalQuestions" />

        <asp:HiddenField ID="HiddenField1" runat="server" />

        
        <div id="divActionButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;">
            <asp:Button TabIndex="33" ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();"  CssClass="StandardSaveButton" ToolTip="Saves any policyholders entered." Text="Save Property" />
            <asp:Button TabIndex="34" ID="btnSaveGotoNextSection" OnClientClick="DisableFormOnSaveRemoves();"  CssClass="StandardSaveButton" runat="server" Text="Coverages Page" />
            <asp:Button TabIndex="35" ID="btnRateHome" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Rate Quote" Text="Rate this Quote"/><br />
        </div>
        <div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
            <asp:Button TabIndex="33" ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>
            <asp:Button TabIndex="34" ID="btnViewGotoNextSection" OnClientClick="DisableFormOnSaveRemoves();"  CssClass="StandardSaveButton" runat="server" Text="Coverages Page" /><br />
        </div>
     
        <br />

        <div align="center">
            <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Bold="true" Text="&nbsp;"></asp:Label>
        </div>
    </div>
</div>