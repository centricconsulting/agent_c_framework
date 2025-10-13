<%@ Page Language="VB" AutoEventWireup="false" CodeFile="QuickQuoteTestApp.aspx.vb" Inherits="QuickQuoteTestApp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>QuickQuote Test App</title>
    <script type="text/javascript" src="js/CommonFunctions.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        QuoteId or PolicyId/ImageNum: <asp:TextBox runat="server" ID="txtQuoteIdOrPolicyIdAndImageNum"></asp:TextBox>
        <br />
        <asp:Button runat="server" ID="btnLoad" Text="Load" />
        <br />
        Results: <asp:Label runat="server" ID="lblResults"></asp:Label>
        <br /><br />
        <div runat="server" id="UpdateSection" visible="false">
            <div runat="server" id="RvWatercraftOperatorAssignSection" visible="false">
                <asp:CheckBox runat="server" ID="cbAssignRvWatercraftOperator" Text="Assign RvWatercraft Operator" />
            </div>
            <div runat="server" id="RvWatercraftOperatorUnAssignSection" visible="false">
                <asp:CheckBox runat="server" ID="cbUnAssignRvWatercraftOperator" Text="Un-Assign RvWatercraft Operator" />
            </div>
            <div runat="server" id="DiamondSaveOptionsSection" visible="false">
                <br />
                <asp:CheckBox runat="server" ID="cbSaveRateWithSeparateMethods" Text="Save/Rate with Separate Methods" />
            </div>

            <br /><br />
            <asp:Button runat="server" ID="btnSaveRate" Text="Save/Rate" />
            <br /><br />
            Save/Rate Results: <asp:Label runat="server" ID="lblSaveRateResults"></asp:Label>
        </div>
        <%--<br /><br />
        <div runat="server" id="SaveRateSection" visible="false">
            <asp:Button runat="server" ID="btnSaveRate" Text="Save/Rate" />
            <br /><br />
            Save/Rate Results: <asp:Label runat="server" ID="lblSaveRateResults"></asp:Label>
        </div>--%>
        
    </div>
    </form>
</body>
</html>
