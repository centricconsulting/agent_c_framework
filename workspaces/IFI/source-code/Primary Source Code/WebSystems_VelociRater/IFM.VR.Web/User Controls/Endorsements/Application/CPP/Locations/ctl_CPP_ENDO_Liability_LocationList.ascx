<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPP_ENDO_Liability_LocationList.ascx.vb" Inherits="IFM.VR.Web.ctl_CPP_ENDO_Liability_LocationList" %>

<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>
<%@ Register Src="~/User Controls/Endorsements/Application/CPP/Locations/ctl_CPP_ENDO_Liabilty_Location.ascx" TagPrefix="uc1" TagName="ctl_CPP_ENDO_Liabilty_Location" %>
<%@ Register Src="~/User Controls/Endorsements/Application/CPP/ClassCode/ctl_CGL_Endo_ClassCodeList.ascx" TagPrefix="uc1" TagName="ctl_CGL_Endo_ClassCodeList" %>




<div id="divLocationList" runat="server">
    <asp:Label ID="lblCPPCGLPropertyMessage" runat="server" ForeColor="Red" Width="100%" style="text-align:center;" Text="All locations must be added on the Property section.  Locations cannot be edited here.  Please add your GL Class Codes below." ></asp:Label>
    <asp:Label ID="lblUwContactMessage" runat="server" CssClass="informationalText" Width="100%" style="text-align:center;" Text="If uncertain of which class code to add, please contact your underwriter." />
    
    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <uc1:ctl_CPP_ENDO_Liabilty_Location runat="server" ID="ctl_CPP_ENDO_Liabilty_Location" />
        </ItemTemplate>
    </asp:Repeater>
</div>
<uc1:ctl_CGL_Endo_ClassCodeList runat="server" id="ctl_CGL_Endo_ClassCodeList" />

<div id="divActionButtons" runat="server" class="standardSubSection center" style="width:100%;text-align:center;">

    <asp:Label ID="ClassCodeDeleteMessage" runat="server" Text="All existing GL Class Codes are at a policy level, please contact your underwriter to complete this change." visible="False" CssClass="informationalTextRed mB10 inlineBlk" />
    <asp:Label ID="ClassCodeAddMessage" runat="server" Text="There is no GL Class Code assigned to the property location added. Please add a GL Class Code for the newly added property location." visible="False" CssClass="informationalTextRed mB10 inlineBlk"/>
    <asp:Button ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Save GL Class Codes" Text="Save GL Class Codes" />
    <asp:Button ID="btnRate" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Rate This Quote" />
    <uc1:ctl_RouteToUw runat="server" ID="ctl_RouteToUw" />
    <br />
</div>

<asp:HiddenField ID="hdnAccord" runat="server" />
