Public Class billingStatementInfo
    Inherits VRControlBase

    Public Property isBillingSummary As Boolean

    Public Property StatementList() As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Billing.Statement)
        Get
            If ViewState.Item("StatementDt") Is Nothing Then
                Return New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Billing.Statement)
            Else
                Return ViewState.Item("StatementDt")
            End If
        End Get
        Set(ByVal value As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Billing.Statement))
            If ViewState.Item("StatementDt") Is Nothing Then
                ViewState.Add("StatementDt", value)
            Else
                ViewState.Item("StatementDt") = value
            End If
        End Set
    End Property

    Public ReadOnly Property IsBillingUpdate() As Boolean
        Get
            Dim result As Boolean = False
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("isBillingUpdate") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("isBillingUpdate").ToString) = False Then
                result = CBool(Request.QueryString("isBillingUpdate").ToString)
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("isBillingUpdate") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("isBillingUpdate").ToString) = False Then
                result = CBool(Page.RouteData.Values("isBillingUpdate").ToString)
            Else
                If Me.Quote IsNot Nothing AndAlso Me.Quote.IsBillingEndorsement = True Then
                    result = True
                End If
            End If
            Return result
        End Get
    End Property

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        'Me.rptDocPrint.DataSource = StatementList
        'Me.rptDocPrint.DataBind()
        If isBillingSummary Then
            lblAccountSum.Text = "Change Billing Detail"
        Else
            lblAccountSum.Text = "Statement Information"
        End If

        Me.DataGrid_StatementInfo.DataSource = StatementList
        Me.DataGrid_StatementInfo.DataBind()

        If isBillingSummary Then
            Me.DataGrid_StatementInfo.Columns(2).Visible = False
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If IsBillingUpdate() Then
            Me.VRScript.CreateAccordion(BillStatementInfo.ClientID, hdnAccordGenInfo, "1")
        Else
            Me.VRScript.CreateAccordion(BillStatementInfo.ClientID, hdnAccordGenInfo, "0")
        End If
        ' Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        '_script.AddScriptLine("$("".DocPrintSection"").accordion({collapsible: false, heightStyle: ""content"", icons: false});")
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
    End Function

    Public Sub DataGrid_StatementInfo_Command(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles DataGrid_StatementInfo.ItemCommand
        Select Case e.CommandName
            Case "Page"
                If ViewState.Item("StatementDt") IsNot Nothing Then
                    Me.DataGrid_StatementInfo.CurrentPageIndex = e.CommandArgument - 1
                    DataGrid_StatementInfo.DataSource = StatementList
                    DataGrid_StatementInfo.DataBind()
                End If
        End Select
    End Sub

    Private Sub DataGrid_StatementInfo_ItemDataBound(sender As Object, e As DataGridItemEventArgs) Handles DataGrid_StatementInfo.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim itemRow As String = e.Item.Cells(5).Text
            If IsDate(itemRow) = True AndAlso CDate(itemRow) = CDate("1/1/1800") Then
                e.Item.Cells(5).Text = ""
            End If
        End If
    End Sub
End Class