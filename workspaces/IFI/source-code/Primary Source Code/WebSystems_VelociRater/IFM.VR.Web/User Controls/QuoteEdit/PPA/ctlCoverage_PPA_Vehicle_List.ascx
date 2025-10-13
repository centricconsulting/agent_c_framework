<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlCoverage_PPA_Vehicle_List.ascx.vb" Inherits="IFM.VR.Web.ctlCoverage_PPA_Vehicle_List" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlCoverage_PPA_VehicleSpecific.ascx" TagPrefix="uc1" TagName="ctlCoverage_PPA_VehicleSpecific" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Billing_Info_PPA.ascx" TagPrefix="uc1" TagName="ctl_Billing_Info_PPA" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>


<div id="VehicleCoverageListControlDivTopMost" runat="server">
    <asp:Panel ID="pnlVehicleCoverageList" runat="server" DefaultButton="btnRate">
        <div id="dvCoverageVehicleList" runat="server">
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <uc1:ctlCoverage_PPA_VehicleSpecific runat="server" ID="ctlCoverage_PPA_VehicleSpecific" />
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <uc1:ctl_Billing_Info_PPA runat="server" ID="ctl_Billing_Info_PPA" />
        <br />
        <div id="divActionButtons" runat="server" style="text-align: center">
            <asp:Button ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" Text="Save Coverages" />
            <asp:Button ID="btnRate" runat="server" OnClientClick="DisableFormOnSaveRemoves();$(this).prop('value','Rating...');" CssClass="StandardSaveButton" Text="Rate this Quote" />
            <uc1:ctl_RouteToUw runat="server" id="ctl_RouteToUw" Visible="false" />
        </div>
         <div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
            <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>
            <asp:Button ID="btnViewGotoBilling" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Billing Information Page" />
        </div>
    </asp:Panel>
</div>

<asp:HiddenField ID="hidden_VehicleCoverageListActive" Value="0" runat="server" />
<asp:HiddenField ID="hiddenVehicleCoveragePlanList" runat="server" />
