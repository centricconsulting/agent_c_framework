<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Cov_CanineExclusionList.ascx.vb" Inherits="IFM.VR.Web.Cov_CanineExclusionList" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/CoverageControls/Cov_CanineExclusionItem.ascx" TagPrefix="uc1" TagName="Cov_CanineExclusionItem" %>



<div id="divCanineOption" runat="server">
    <asp:CheckBox ID="chkCanine" runat="server" />&nbsp;
    <asp:Label ID="lblCanine" runat="server" Text="Exclusion - Canine (FAR 4003 01 22)"></asp:Label>
    <div id="dvCanineInfo" runat="server" class="div" style="display: none">
        <asp:Repeater ID="ceRepeater" runat="server">
            <ItemTemplate>
                <uc1:Cov_CanineExclusionItem runat="server" id="Cov_CanineExclusionItem" />
            </ItemTemplate>
        </asp:Repeater>
    
        <div>
            <div style="float:right">
                <asp:LinkButton ID="lnkAddCanine" runat="server">Add Additional Canine</asp:LinkButton>
            </div>
        </div>
        <div id="divSpecialText" runat="server" style="color:blue;text-align:center;padding:1em;margin-top:1em">
            <asp:Label ID="lblSpecialText" runat="server" Text=""></asp:Label>
        </div>
    </div>





</div>

