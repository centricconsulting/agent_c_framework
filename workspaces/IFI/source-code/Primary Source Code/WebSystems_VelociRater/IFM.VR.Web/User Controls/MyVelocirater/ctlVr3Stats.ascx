<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlVr3Stats.ascx.vb" Inherits="IFM.VR.Web.ctlVr3Stats" %>

<script src="<%=ResolveClientUrl("~/js/VR3Stats.js")%>"></script>
<script src="<%=ResolveClientUrl("~/js/3rdParty/Chart.min.js")%>"></script>

<div style="margin-top: 40px;" class="mainStatSection">

    <div class="standardStatSection" style="width: 400px; text-align: center;">
        <table>
            <tr>
                <td>Start Date
                <asp:TextBox ID="txtstartDate" Text="1/1/2013" runat="server"></asp:TextBox>
                </td>
                <td>End Date
                <asp:TextBox ID="txtEndDate" Text="" runat="server"></asp:TextBox>
                </td>
            </tr>
        </table>
        <input id="Button1" class="StandardButton" type="button" value="Populate Stats" onclick="VRStats.GetVr3Stats('<%=divStats.ClientId%>    ',$('#<%= txtstartDate.ClientID%>    ').val(), $('#<%=txtEndDate.ClientID%>    ').val(), master_AgencyId, '<%=Me.SupportedLob_CSV%>    ');" />
        </br><i>Finds quotes with a last modified date between the dates above.</i>
    </div>

    <div id="divStats" runat="server">
    </div>
</div>