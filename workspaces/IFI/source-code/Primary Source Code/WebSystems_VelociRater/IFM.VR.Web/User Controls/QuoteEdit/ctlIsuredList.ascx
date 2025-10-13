<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlIsuredList.ascx.vb" Inherits="IFM.VR.Web.ctlIsuredList" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlInsured.ascx" TagPrefix="uc1" TagName="ctlInsured" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlMiniClientSearch.ascx" TagPrefix="uc1" TagName="ctlMiniClientSearch" %>
<%@ Register Src="~/User Controls/Application/ctl_Esignature.ascx" TagPrefix="uc1" TagName="ctl_Esignature" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>

<div id="InsuredsListControlDivTopMost" runat="server">
    <uc1:ctlMiniClientSearch runat="server" ID="ctlMiniClientSearch" />
    <asp:Panel ID="Panel1" runat="server" DefaultButton="btnSubmit">
        <div id="accInsuredList" runat="server">
            <uc1:ctlInsured runat="server" ID="ctlInsured" IsPolicyHolderNum1="true" />
            <uc1:ctlInsured runat="server" ID="ctlInsured1" IsPolicyHolderNum1="false" />
        </div>
        
        <uc1:ctl_Esignature runat="server" ID="ctl_Esignature" />

        <div runat="server" id="MultiStateSection" visible="false" style="margin-top:5px; padding:10px;" class="ui-accordion-content ui-helper-reset ui-widget-content ui-corner-bottom ui-accordion-content-active">
            <center>
                <asp:Label runat="server" ID="NewCoMessage" class="informationalText"></asp:Label><br />
                Governing State: <asp:Label runat="server" ID="lblGoverningState"></asp:Label>
                <br /><br />
                <div id="divMultiKYWC" runat="server">
                    <asp:Label ID="lblWCMultiQuestion" runat="server" Text="Does this risk also have locations, exposures or garaging in any of the following states?"></asp:Label>
                    <br />
                    (Select all that apply)
                    <br />
                    <asp:CheckBox ID="chkOtherState" runat="server" Text="" Width="30%" />
                    <asp:CheckBox ID="chkKentucky" runat="server" Text="Kentucky" Width="30%" />
                </div>
                <div id="divMultiDefaultNEW" runat="server" style="align-content:center;">
                    Does this risk also have locations, exposures or garaging in any of the following states?
                    <br />
                    (Select all that apply)
                    <table id="tblStates" runat="server" style="width:75%;">
                        <tr id="trStatesRow1" runat="server">
                            <td style="width:50%;text-align:center;">
                                <asp:CheckBox ID="chkState1" runat="server" Text="state1" />
                            </td>
                            <td style="width:50%;text-align:center">
                                <asp:CheckBox ID="chkState2" runat="server" Text="state2" />
                            </td>
                        </tr>
                        <tr id="trStatesRow2" runat="server" style="display:none;">
                            <td style="width:50%;text-align:center;">
                                <asp:CheckBox ID="chkState3" runat="server" Text="state3" />
                            </td>
                            <td style="width:50%;text-align:center;">
                                <asp:CheckBox ID="chkState4" runat="server" Text="state4" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="divMultiDefaultOLD" runat="server" style="align-content:center;">
                    Does this risk also have locations or garaging in <asp:label runat="server" ID="lblOtherState"></asp:label>?
                    <asp:RadioButtonList runat="server" ID="rblHasOtherState" RepeatLayout="Flow" RepeatDirection="Horizontal">
                        <asp:ListItem>Yes</asp:ListItem>
                        <asp:ListItem>No</asp:ListItem>
                    </asp:RadioButtonList>
                </div>
                <div id="divEndosementMessage" runat="server" style="margin-top:1em;" visible="false">
                    <asp:Label ID="endoMessage" runat="server" cssClass="informationalText" />
                </div>
            </center>
        </div>


        <div id="divActionButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;">
            <asp:Button ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Saves any policyholders entered." Text="Save Policyholders" />
            <asp:Button ID="btnSaveAndGotoDrivers" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Drivers Page" />
            <asp:Button ID="btnRatePolicyholder" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Rate Quote" Text="Rate this Quote"/>
            <uc1:ctl_RouteToUw runat="server" id="ctl_RouteToUw" Visible="false" />
        </div>
        <div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
            <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>
            <asp:Button ID="btnViewGotoDrivers" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Drivers Page" />
        </div>
        
    </asp:Panel>
    <asp:TextBox ID="txtClientId_Lookup" ClientIDMode="Static" runat="server"></asp:TextBox>


</div>
<asp:HiddenField ID="visibleTabIndex" runat="server" />
<asp:HiddenField ID="hdnHasOtherStateSelection" runat="server" />
