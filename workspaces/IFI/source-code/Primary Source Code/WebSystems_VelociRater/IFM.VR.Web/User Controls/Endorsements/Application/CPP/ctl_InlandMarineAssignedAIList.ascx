<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_InlandMarineAssignedAIList.ascx.vb" Inherits="IFM.VR.Web.ctl_InlandMarineAssignedAIList" %>
<%@ Register Src="~/User Controls/Endorsements/Application/CPP/ctl_InlandMarineAssignedAI.ascx" TagPrefix="uc1" TagName="ctl_InlandMarineAssignedAI" %>


<div runat="server" id="divIMAssignedAdditionalInterests" class="standardSubSection">
    <h3>
        <asp:Label ID="lblHeader" runat="server" Text="Assigned Additional Interests"></asp:Label>
    </h3>
    <div>
        <div runat="server" id="divIMAssignedAdditionalInterestItems">
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <uc1:ctl_InlandMarineAssignedAI runat="server" ID="ctl_InlandMarineAssignedAI" />
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</div>
<asp:HiddenField ID="hiddenIMAdditionalInterest" runat="server" />
<asp:HiddenField ID="hiddenIMAdditionalInterestItems" runat="server" />