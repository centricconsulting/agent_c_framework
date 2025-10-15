<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomePersonalLossList.ascx.vb" Inherits="IFM.VR.Web.ctlHomePersonalLossList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/ctlHomePersonalLossItem.ascx" TagPrefix="uc1" TagName="ctlLH" %>
<%@ Register Src="~/User Controls/ctlValidationSummary.ascx" TagPrefix="uc1" TagName="ctlValidationSummary" %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#<%=Me.divLossHistoryList.ClientID%>").accordion(
            {   heightStyle: "content",
                active: <%=Me.hiddenActiveIM.Value%>,
                collapsible: true,
                activate: function (event, ui)
                {
                    $("#<%=Me.hiddenActiveIM.ClientID%>").val($("#<%=Me.divLossHistoryList.ClientID%>").accordion('option','active'));
                }
           });

    });
</script>

<div id="divLossHistoryList" runat="server">
    <asp:Repeater ID="rptLossHistories" runat="server">
        <ItemTemplate>
            <uc1:ctlLH runat="server" ID="ctlLossHistoryItem" />
        </ItemTemplate>
    </asp:Repeater>
</div>
<asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red" Font-Bold="true"></asp:Label>
<div style="width: 100%; text-align: center;">
    <div style="float: left;">
        <asp:Button ID="btnAddLossHistory" runat="server" CssClass="StandardSaveButton" Text="Add Loss History" Width="150px" />
    </div>
    <asp:Button ID="btnSubmit" runat="server" CssClass="StandardSaveButton" Text="Save Loss Histories" Width="150px" />
    <asp:Button ID="btnSaveAndGotoResidence" runat="server" CssClass="StandardSaveButton" Text="Residence Page" Width="150px" />
    <asp:HiddenField ID="hiddenActiveIM" Value="0" runat="server" />
</div>