<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_LossViolations_App.ascx.vb" Inherits="IFM.VR.Web.ctl_LossViolations_App" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlAccidentHistoryList.ascx" TagPrefix="uc1" TagName="ctlAccidentHistoryList" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlViolationList.ascx" TagPrefix="uc1" TagName="ctlViolationList" %>

<div runat="server" id="divAccidentsAndViolations_App" class="standardSubSection">
    <h3>
        <asp:Label ID="lblAccordHeader" runat="server" Text="Label"></asp:Label>
    </h3>
    <div>
        <div>
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:LinkButton ID="lnkMVRReport" runat="server">View MVR Report</asp:LinkButton>
                    <td>
                        <asp:LinkButton ID="lnkClueReport" runat="server">View CLUE Report</asp:LinkButton>
                </tr>
            </table>
        </div>
        <div class="standardSubSection">
            <uc1:ctlAccidentHistoryList runat="server" ID="ctlAccidentHistoryList" />
            <uc1:ctlViolationList runat="server" ID="ctlViolationList" />
        </div>
    </div>
</div>
<asp:HiddenField ID="hidden_divAccidentsAndViolations_App" runat="server" />