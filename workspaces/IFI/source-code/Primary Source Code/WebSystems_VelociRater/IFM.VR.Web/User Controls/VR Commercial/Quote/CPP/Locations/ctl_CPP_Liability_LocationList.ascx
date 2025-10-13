<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPP_Liability_LocationList.ascx.vb" Inherits="IFM.VR.Web.ctl_CPP_Liability_LocationList" %>

<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/Locations/ctl_CPP_Liabilty_Location.ascx" TagPrefix="uc1" TagName="ctl_CGL_Location" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CGL/ClassCode/ctl_CGL_ClassCodeList.ascx" TagPrefix="uc1" TagName="ctl_CGL_ClassCodes" %>

<div id="divLocationList" runat="server">
    <asp:Label ID="lblCPPCGLPropertyMessage" runat="server" ForeColor="Red" Width="100%" style="text-align:center;" Text="All locations must be added on the Property section.  Locations cannot be edited here.  Please add your GL Class Codes below." ></asp:Label>
    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <uc1:ctl_CGL_Location runat="server" id="ctl_CPPCGL_Location" />
        </ItemTemplate>
    </asp:Repeater>
</div>
<uc1:ctl_CGL_ClassCodes ID="ctl_CPPCGL_ClassCodes" runat="server" />

<div id="divActionButtons" runat="server" class="standardSubSection center" style="width:100%;text-align:center;">
    <asp:Button ID="btnContinueIM" runat="server" Text="Continue to Inland Marine" CssClass="StandardSaveButton" />
    <asp:Button ID="btnContinueCRM" runat="server" Text="Continue to Crime" CssClass="StandardSaveButton" />
    <asp:Button ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Save GL Class Codes" Text="Save GL Class Codes" />
    <asp:Button ID="btnRate" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Rate This Quote" />
    <input type="button" id="btnEmailForUWAssistance" runat="server" onclick="InitEmailToUW();" title="Email for UW Assistance" value="Email for UW Assistance" class="StandardSaveButton" />
    <br />
</div>
<div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
    <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>
    <asp:Button ID="btnViewInlandMarine" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Inland Marine" />
</div>

<asp:HiddenField ID="hdnAccord" runat="server" />
