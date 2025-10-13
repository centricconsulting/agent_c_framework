<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlQsummary_PPA_Vehicle_List.ascx.vb" Inherits="IFM.VR.Web.ctlQsummary_PPA_Vehicle_List" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlQsummary_PPA_Vehicle.ascx" TagPrefix="uc1" TagName="ctlQsummary_PPA_Vehicle" %>

<script type="text/javascript">
    $(document).ready(function () {

        if (false)
        {
            $("#<%=me.divQSummaryVList.ClientID%>").accordion({ heightStyle: "content", active: <%=Me.hiddenvListActive.Value%>, collapsible: true, activate: function (event, ui) { $("#<%=Me.hiddenvListActive.ClientID%>").val($("#<%=me.divQSummaryVList.ClientID%>").accordion('option','active'));  } });
            $("#<%=Me.h3AccordHeader.ClientID%>").show();
        }
        else
        {
            $("#<%=Me.h3AccordHeader.ClientID%>").hide();
        }

    });
</script>

<div id="divQSummaryVList" runat="server">
    <h3 id="h3AccordHeader" runat="server">
        <asp:Label ID="lblHeader" runat="server" Text="Vehicle List"></asp:Label></h3>
    <div>
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <uc1:ctlQsummary_PPA_Vehicle runat="server" ID="ctlQsummary_PPA_Vehicle" />
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
<asp:HiddenField ID="hiddenvListActive" Value="0" runat="server" />