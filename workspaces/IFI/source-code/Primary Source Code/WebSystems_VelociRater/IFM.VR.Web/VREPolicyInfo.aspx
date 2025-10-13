<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VREPolicyInfo.aspx.vb" Inherits="IFM.VR.Web.VREPolicyInfoPage" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/Endorsements/ctlPolicyInfo.ascx" TagPrefix="uc1" TagName="ctlPolicyInfo" %>


<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    
    <table style="width: 100%;">
        <tr>
            <td style="width: 250px; vertical-align: top;">
                <div id="divAboveTree"></div>
                <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
            </td>
            <td style="vertical-align: top;">
                <div id="divAppEditControls">
                    <uc1:ctlPolicyInfo runat="server" id="ctlPolicyInfo" />
                </div>
            </td>
        </tr>
    </table>
    <div>
    </div>
</asp:Content>

