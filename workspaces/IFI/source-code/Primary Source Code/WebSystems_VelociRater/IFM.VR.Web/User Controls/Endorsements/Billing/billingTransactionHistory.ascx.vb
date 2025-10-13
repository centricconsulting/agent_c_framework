Public Class billingTransactionHistory
    Inherits VRControlBase

    Public Property HistoryList() As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Billing.AccountHistory)
        Get
            If ViewState.Item("HistoryDt") Is Nothing Then
                Return New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Billing.AccountHistory)
            Else
                Return ViewState.Item("HistoryDt")
            End If
        End Get
        Set(ByVal value As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Billing.AccountHistory))
            If ViewState.Item("HistoryDt") Is Nothing Then
                ViewState.Add("HistoryDt", value)
            Else
                ViewState.Item("HistoryDt") = value
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
        Me.DataGrid_HistoryInfo.DataSource = HistoryList
        Me.DataGrid_HistoryInfo.DataBind()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If IsBillingUpdate() Then
            Me.VRScript.CreateAccordion(BillTransactionHistory.ClientID, hdnAccordGenInfo, "1")
        Else
            Me.VRScript.CreateAccordion(BillTransactionHistory.ClientID, hdnAccordGenInfo, "0")
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
    End Function

    Public Sub DataGrid_HistoryInfo_Command(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles DataGrid_HistoryInfo.ItemCommand

        Select Case e.CommandName
            Case "Page"
                If ViewState.Item("HistoryDt") IsNot Nothing Then
                    DataGrid_HistoryInfo.CurrentPageIndex = e.CommandArgument - 1
                    DataGrid_HistoryInfo.DataSource = HistoryList
                    DataGrid_HistoryInfo.DataBind()
                End If

        End Select

    End Sub
End Class