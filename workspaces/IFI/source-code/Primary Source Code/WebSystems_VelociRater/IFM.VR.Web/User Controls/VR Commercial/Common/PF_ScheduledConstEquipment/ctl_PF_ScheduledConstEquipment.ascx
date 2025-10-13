<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_PF_ScheduledConstEquipment.ascx.vb" Inherits="IFM.VR.Web.ctl_PF_ScheduledConstEquipment" %>
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

    </style>
    <div class="imgBox"><img src="../images/IFM logo Cool Gray 11.jpg" /></div>
    <div id="PageHeader" style="text-align:center; margin-bottom: 2em;">
        <span class="PolicyNumHeader">List of Scheduled Contractors Equipment for <asp:Label ID="TitleQuoteNum" runat="server" Text="QBOP000000"></asp:Label></span><br />
        <span class="PolicyHolderHeader">Policyholder: <asp:Label ID="PolicyHolderName" runat="server" Text="Chad Corp"></asp:Label>&nbsp;&nbsp;&nbsp;<span id="DBANameElement" runat="server" visible="false">DBA: <asp:Label ID="DBAName" runat="server" Text="CCorp"></asp:Label></span></span>
    </div>
    <asp:Repeater ID="rptSchedConEquip" runat="server">
        <ItemTemplate>
            <asp:panel id="ItemGroup" runat="server" CssClass="ItemGroup SCEItem" >
                <div class="SCEItem KeepPrintGroup">
                    <div>Item #<asp:Label ID="ItemNum" runat="server" Text="<%# Container.ItemIndex + 1 %>"></asp:Label>: <br /></div>
                    <div class="container">
                        <div class="column">Description: <asp:Label ID="lblDescriptionText" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Description")%>'></asp:Label></div>
                        <div class="column">Limit: <asp:Label ID="lblLimitText" runat="server" Text='<%# FormatAsCurrency(DataBinder.Eval(Container.DataItem, "Limit"))%>'></asp:Label></div>
                    </div>
                </div>
            </asp:panel>
        </ItemTemplate>
    </asp:Repeater>
</div>



