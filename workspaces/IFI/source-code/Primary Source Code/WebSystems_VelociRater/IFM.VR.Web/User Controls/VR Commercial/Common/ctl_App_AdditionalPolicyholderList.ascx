<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_App_AdditionalPolicyholderList.ascx.vb" Inherits="IFM.VR.Web.ctl_App_AdditionalPolicyholderList" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_App_AdditionalPolicyholder.ascx" TagPrefix="UC1" TagName="ctl_App_AdditionalPolicyholder" %>

<div>
    <asp:Button ID="btnAddAdditionalPolicyholder" runat="server" Text="Add an Additional Policyholder" CssClass="StandardSaveButton" />
    <br />
    <div id="divMainList" runat="server">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <UC1:ctl_App_AdditionalPolicyholder ID="ctl_App_APH" runat="server" />
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <asp:HiddenField ID="hdnAccord" runat="server" />
</div>
