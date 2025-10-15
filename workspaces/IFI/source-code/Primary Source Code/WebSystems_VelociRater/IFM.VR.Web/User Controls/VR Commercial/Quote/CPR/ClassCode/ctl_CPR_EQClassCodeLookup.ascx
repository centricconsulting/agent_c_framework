<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_EQClassCodeLookup.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_EQClassCodeLookup" %>

<div runat="server" id="divMain" style="border:none;" >    
    <script type="text/javascript">
        function ValidateEQCC() {
            if ($('#trYardRateRow').style.display != 'none') {

            }
            return true;
        }
    </script>
     <table style="margin-left:auto;margin-right:auto;">
        <tr>
            <td>
                <asp:Label ID="CPRCCLabel2" runat="server" Text="Filter By" Width="200px"></asp:Label>
            </td>
            <td>
                <asp:Label ID="CPRCCLabel3" runat="server" Text="Filter Value" Width="200px" ></asp:Label><br />
            </td>
        </tr>
        <tr>
            <td>
                <asp:DropDownList ID="ddlFilterBy" runat="server" 
                    TabIndex="47" Width="200px">
                    <asp:ListItem Value="2">Contains</asp:ListItem>
                    <%--<asp:ListItem Value="3">Rate Grade</asp:ListItem>--%>
                    <asp:ListItem Value="1">Starts With</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:TextBox MaxLength="20" ID="txtFilterValue" 
                    runat="server" Width="200px" TabIndex="48"></asp:TextBox>
            </td>
            <td> 
                <asp:Button ID="btnSearch" runat="server" Text="Find" />     
            </td>
        </tr>
    </table>

    <div id="divResults" runat="server"></div>
    
    <br />

    <div align="center">
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="return confirm('Cancel?');" />
        <asp:Button ID="btnClose" runat="server" Text="Close" style="display:none;" />
    </div>

    <asp:HiddenField ID="hdnClassCode" runat="server" />
    <asp:HiddenField ID="hdnDescription" runat="server" />
    <asp:HiddenField ID="hdnDIAClass_Id" runat="server" />
    <asp:HiddenField ID="hdnRateGrade" runat="server" />
</div>