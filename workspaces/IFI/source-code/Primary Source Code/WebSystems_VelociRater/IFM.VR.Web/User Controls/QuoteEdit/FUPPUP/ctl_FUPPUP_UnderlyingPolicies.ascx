<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_FUPPUP_UnderlyingPolicies.ascx.vb" Inherits="IFM.VR.Web.ctl_FUPPUP_UnderlyingPolicies" %>
<%@ Register Src="~/User Controls/QuoteEdit/FUPPUP/ctl_FUPPUP_UnderlyingPolicy_Item.ascx" TagPrefix="uc1" TagName="ctl_FUPPUP_UnderlyingPolicy_Item" %>
<%@ Register Src="~/User Controls/QuoteEdit/FUPPUP/ctl_FUPPUP_MiscPolicyInfo.ascx" TagPrefix="uc1" TagName="ctl_FUPPUP_MiscPolicyInfo" %>




<div runat="server" id="divUmbrellaUnderlyingPolicies">
        <h3>
            Underlying Policies
             <span style="float: right;">
                <asp:LinkButton ID="lnkClearGeneralInfo" runat="server" OnClientClick="var confirmed = confirm('Clear ALL Limits?'); if(confirmed)DisableFormOnSaveRemoves(); return confirmed;" ToolTip="Reset Policy Coverage Limits to Default Values" CssClass="RemovePanelLink">Clear</asp:LinkButton>
                <asp:LinkButton ID="lnkSaveGeneralInfo" runat="server" ToolTip="Save Policy Coverage" CssClass="RemovePanelLink">Save</asp:LinkButton>
             </span>
        </h3>
        <div>
            <asp:Label ID="LeadText" runat="server">Please enter all policies or quotes which the umbrella will cover.  The underlying policies must be with Indiana Farmers.</asp:Label>
            <table>
                <tr>
                    <%--hom--%>
                    <td><uc1:ctl_FUPPUP_UnderlyingPolicy_Item runat="server" id="ctl_FUPPUP_UnderlyingPolicy_Item_hom" /></td>
                </tr>
                <tr>
                    <%--ppa--%>
                    <td><uc1:ctl_FUPPUP_UnderlyingPolicy_Item runat="server" id="ctl_FUPPUP_UnderlyingPolicy_Item_ppa" /></td>
                </tr>
                <tr>
                    <%--far--%>
                    <td><uc1:ctl_FUPPUP_UnderlyingPolicy_Item runat="server" id="ctl_FUPPUP_UnderlyingPolicy_Item_far" /></td>
                </tr>
                <tr>
                    <%--dfr--%>
                    <td><uc1:ctl_FUPPUP_UnderlyingPolicy_Item runat="server" id="ctl_FUPPUP_UnderlyingPolicy_Item_dfr" /></td>
                </tr>
                <tr>
                    <%--wcp--%>
                    <td><uc1:ctl_FUPPUP_UnderlyingPolicy_Item runat="server" id="ctl_FUPPUP_UnderlyingPolicy_Item_wcp" /></td>
                </tr>
                <tr>
                    <%--cap--%>
                    <td><uc1:ctl_FUPPUP_UnderlyingPolicy_Item runat="server" id="ctl_FUPPUP_UnderlyingPolicy_Item_cap" /></td>
                </tr>
                
                <tr>
                    <%--Buttons--%>
                    <td></td>
                </tr>
            </table>

            <uc1:ctl_FUPPUP_MiscPolicyInfo runat="server" id="ctl_FUPPUP_MiscPolicyInfo" />
        </div>
</div>

    <asp:HiddenField ID="hdnAccordGenInfo" runat="server" />
