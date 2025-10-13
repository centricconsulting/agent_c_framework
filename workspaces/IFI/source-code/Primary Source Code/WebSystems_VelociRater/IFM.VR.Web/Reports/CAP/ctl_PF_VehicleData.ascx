<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_PF_VehicleData.ascx.vb" Inherits="IFM.VR.Web.ctl_PF_VehicleData" %>

<script type="text/javascript">

    // Initial Page load.
    $(function () {
        // Alternat color backgrounds for table items.
        window.opener.focus();
    });

</script>
<div id="SCEContainer">
    <style>
        .container {
            overflow: hidden;
        }

        .column {
            float: left;
            padding-bottom: 100%;
            margin-bottom: -100%;
            width: 45%;
            padding-left: 2em;
        }

        img {
            margin-bottom: 20px;
            width: 407px;
            height: 75px;
        }

        .imgBox {
            text-align: center;
        }

        .SCEItem {
            margin-bottom: 1em;
            margin-left: 10%;
            font-size: 12pt;
        }

        .PolicyNumHeader {
            font-size: 12pt;
            font-weight: 700;
        }

        .PolicyHolderHeader {
            font-size: 11pt;
        }

        .KeepPrintGroup {
            page-break-inside: avoid;
        }

        .quickQuoteGrid {
            font-family: Calibri;
            font-size: 10pt;
            border: 1px solid #000;
            'margin-left: 1in; 'margin-right: 1in; width: 100%
        }

        .GridHeader {
            color: white;
            background-color: #797777;
        }

        .GridItem {
            border: 1px solid #000;
        }
    </style>
    <div class="imgBox">
        <img src="../images/Vehicle List Logo.jpg" /></div>
    <div id="PageHeader" style="text-align: center; margin-bottom: 2em;">
        <span class="PolicyNumHeader">List of vehicles for
            <asp:Label ID="TitleQuoteNum" runat="server" Text="QBOP000000"></asp:Label></span><br />
        <span class="PolicyHolderHeader">Policyholder:
            <asp:Label ID="PolicyHolderName" runat="server" Text="Chad Corp"></asp:Label>&nbsp;&nbsp;&nbsp;<span id="DBANameElement" runat="server" visible="false">DBA:
                <asp:Label ID="DBAName" runat="server" Text="CCorp"></asp:Label></span></span>
    </div>
    <asp:DataGrid ID="dgrdVehicles" runat="server" HorizontalAlign="Center" CellPadding="4" AutoGenerateColumns="false" GridLines="Both" CssClass="quickQuoteGrid">
        <ItemStyle CssClass="GridItem"></ItemStyle>
        <HeaderStyle CssClass="GridHeader" HorizontalAlign="Center"></HeaderStyle>
        <Columns>
            <asp:BoundColumn DataField="VehicleNum" SortExpression="VehicleNum" HeaderText="#"></asp:BoundColumn>
            <asp:BoundColumn DataField="Year" SortExpression="Year" HeaderText="Year"></asp:BoundColumn>
            <asp:BoundColumn DataField="Make" SortExpression="Make" HeaderText="Make"></asp:BoundColumn>
            <asp:BoundColumn DataField="Model" SortExpression="Model" HeaderText="Model"></asp:BoundColumn>
            <asp:BoundColumn DataField="Vin" SortExpression="Vin" HeaderText="Vin #"></asp:BoundColumn>
            <asp:BoundColumn DataField="CostNew" SortExpression="CostNew" HeaderText="Cost New"></asp:BoundColumn>
            <asp:BoundColumn DataField="Class" SortExpression="Class" HeaderText="Class"></asp:BoundColumn>
            <asp:BoundColumn DataField="LiabPrem" SortExpression="LiabPrem" HeaderText="LiabPrem"></asp:BoundColumn>
            <asp:BoundColumn DataField="MedPrem" SortExpression="MedPrem" HeaderText="MedPrem"></asp:BoundColumn>
            <asp:BoundColumn DataField="CompDed" SortExpression="CompDed" HeaderText="CompDed"></asp:BoundColumn>
            <asp:BoundColumn DataField="CompPrem" SortExpression="CompPrem" HeaderText="CompPrem"></asp:BoundColumn>
            <asp:BoundColumn DataField="CollDed" SortExpression="CollDed" HeaderText="CollDed"></asp:BoundColumn>
            <asp:BoundColumn DataField="CollPrem" SortExpression="CollPrem" HeaderText="CollPrem"></asp:BoundColumn>
            <asp:BoundColumn DataField="Rntl" SortExpression="Rntl" HeaderText="Rntl"></asp:BoundColumn>
            <asp:BoundColumn DataField="Tow" SortExpression="Tow" HeaderText="Tow"></asp:BoundColumn>
            <asp:BoundColumn DataField="TotlPrem" SortExpression="TotlPrem" HeaderText="TotlPrem"></asp:BoundColumn>
            <asp:BoundColumn DataField="Terr#" SortExpression="Terr#" HeaderText="Terr#"></asp:BoundColumn>
        </Columns>
    </asp:DataGrid>
</div>




