<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlInlandMarineList.ascx.vb" Inherits="IFM.VR.Web.ctlInlandMarineList" %>
<%--<%@ Register Src="~/User Controls/QuoteEdit/HOM/ctlInlandMarineItem.ascx" TagPrefix="uc1" TagName="ctlIM" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlValidationSummary.ascx" TagPrefix="uc1" TagName="ctlValidationSummary" %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#<%=Me.divIMItems.ClientID%>").accordion(
            {   heightStyle: "content",
                active: <%=Me.hiddenActiveIM.Value%>,
                collapsible: true,
                activate: function (event, ui)
                {
                    //$("#<%=Me.hiddenActiveIM.ClientID%>").val($("#<%=Me.divInlandMarineList.ClientID%>").accordion('option','active'));
                    $("#<%=Me.hiddenActiveIM.ClientID%>").val($("#<%=Me.divIMItems.ClientID%>").accordion('option','active'));
                }
        });
    });
</script>

<div id="divInlandMarineList" runat="server">
    <div>
        <div id="divIMItems" runat="server">
            <asp:Repeater ID="rptInlandMarines" runat="server">
                <ItemTemplate>
                    <uc1:ctlIM runat="server" ID="ctlInlandMarineItem" />
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <asp:Label ID="lblMsg" runat="server" Text="&nbsp;" ForeColor="Red" Font-Bold="true"></asp:Label>
        <div style="width: 100%; text-align: center;">
            <div style="float: left;">
                <asp:Button ID="btnAddInlandMarine" runat="server" CssClass="StandardSaveButton" Text="Add Inland Marine" Width="150px" />
            </div>
            <asp:Button ID="btnSubmit" runat="server" CssClass="StandardSaveButton" Text="Save Inland Marines" Width="150px" />
            <asp:Button ID="btnSaveAndGotoResidence" runat="server" CssClass="StandardSaveButton" Text="Residence Page" Width="150px" />
            <asp:HiddenField ID="hiddenActiveIM" Value="0" runat="server" />
        </div>
    </div>
</div>--%>