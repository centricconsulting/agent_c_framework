<%@ Page Language="VB" AutoEventWireup="false" CodeFile="VR_e2Value_TestPage.aspx.vb" Inherits="VR_e2Value_TestPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>e2Value Test Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label runat="server" ID="lblQuoteDetails"></asp:Label>
        <br />
        <br />
        <asp:Button runat="server" ID="btnSaveResponses" Text="Save All Responses" />
        &nbsp;&nbsp;
        <asp:Button runat="server" ID="btnInitiateE2Value" Text="Initiate e2Value" />
        <br /><br />
        <asp:CheckBox runat="server" ID="cbNewValuation" Text="Force New Valuation" />
        <asp:CheckBox runat="server" ID="cbViewUrl" Text="View Url Before Going to e2Value" Checked="true" />
        <asp:CheckBox runat="server" ID="cbUseTestReturnPage" Text="Use Test Return page" Checked="true" />
        <asp:CheckBox runat="server" ID="cbAppendReturnStructuresRpc" Text="Append return_structuresrpc=1" />
        <div runat="server" id="urlSection" visible="false">
            <asp:TextBox runat="server" ID="txtUrl" TextMode="MultiLine" Width="400px" Height="200px" ReadOnly="false"></asp:TextBox>
            <br /><br />
            <asp:Button runat="server" ID="btnGoToE2Value" Text="Go to e2Value" />
        </div>        
    </div>
    </form>
</body>
</html>
