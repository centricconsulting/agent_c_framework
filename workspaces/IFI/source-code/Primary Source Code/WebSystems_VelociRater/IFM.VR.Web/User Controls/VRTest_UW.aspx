<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VRTest_UW.aspx.vb" Inherits="IFM.VR.Web.VRTest_UW" %>
<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_App_Master_Edit.ascx" TagPrefix="uc1" TagName="ctl_App_Master_Edit" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/DocumentPrinting/ctlCommercial_DocPrint.ascx" TagPrefix="uc1" TagName="ctlCommercial_DocPrint" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/PF_ScheduledConstEquipment/ctl_PF_ScheduledConstEquipment.ascx" TagPrefix="uc1" TagName="ctl_PF_ScheduledConstEquipment" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPP/ctl_CPP_InlandMarine.ascx" TagPrefix="uc1" TagName="ctl_CPP_InlandMarine" %>
<%@ Register Src="~/User Controls/Endorsements/ctlPolicyInfo.ascx" TagPrefix="uc1" TagName="ctlPolicyInfo" %>


<%--<%@ Register Src="~/User Controls/VR Commercial/Application/BOP/ctlCommercialUWQuestionList.ascx" TagPrefix="uc1" TagName="ctlCommercialUWQuestionList" %>--%>
<%--<%@ Register Src="~/User Controls/VR Commercial/Common/IRPM/ctlCommercial_IRPM.ascx" TagPrefix="uc1" TagName="ctlCommercial_IRPM" %>--%>
<%--<%@ Register Src="~/User Controls/VR Commercial/Quote/CAP/ctl_CAP_QuoteSummary.ascx" TagPrefix="uc1" TagName="ctl_CAP_QuoteSummary" %>--%>





<%--<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">--%>

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
                    <%--<uc1:ctl_CPP_InlandMarine runat="server" id="ctl_CPP_InlandMarine" />--%>
                    <%--<uc1:ctlCommercial_IRPM runat="server" id="ctlCommercial_IRPM" />--%>
                    <%--<uc1:ctlCommercialUWQuestionList runat="server" id="ctlCommercialUWQuestionList" />--%>
                    <%--<uc1:ctl_CAP_QuoteSummary runat="server" id="ctl_CAP_QuoteSummary" />--%>
                    <%--<uc1:ctlCommercial_DocPrint runat="server" id="ctlCommercial_DocPrint" />--%>
                    <%--<uc1:ctl_PF_ScheduledConstEquipment runat="server" id="ctl_PF_ScheduledConstEquipment" />--%>
                </div>
            </td>
        </tr>
    </table>
    <div>
    </div>
</asp:Content>

    <%--</form>
</body>
</html>--%>