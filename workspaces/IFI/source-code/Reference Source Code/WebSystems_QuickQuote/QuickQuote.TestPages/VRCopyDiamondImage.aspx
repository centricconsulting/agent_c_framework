<%@ Page Language="VB" AutoEventWireup="false" CodeFile="VRCopyDiamondImage.aspx.vb" Inherits="VRCopyDiamondImage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>VR Copy Diamond Image</title>
    <script type="text/javascript" src="js/CommonFunctions.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <p class="normaltext" align="center"><b>Copy Diamond Image Into New VR Quote</b></p>        
        <br />
        <p class="normaltext" align="center">
            *Policy Id: <asp:TextBox runat="server" ID="txtPolicyId"></asp:TextBox>
            <br />
            *Policy Image Number: <asp:TextBox runat="server" ID="txtPolicyImageNum"></asp:TextBox>
            <br />
            Agency Code: <asp:TextBox runat="server" ID="txtAgencyCode"></asp:TextBox>
            <br />
            <asp:Button runat="server" ID="btnCopyImage" Text="Create New VR Quote" />
        </p>
        <br />
        <p class="normaltext" align="center" runat="server" id="AgentsLinkSection" visible="false"><a runat="server" id="AgentsLink">Return to Agents Only Site</a></p>
        <br />
    </div>
    </form>
</body>
</html>
