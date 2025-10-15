Imports IFM.PrimativeExtensions

Public Class billingAccountSummary
    Inherits VRControlBase

    Public Property isBillingSummary As Boolean
    Public Property PayPlanResult As String
    Public Property ActivityDate As DateTime
    Public Property ActivityType As String
    Public Property Amount As String
    Public Property NextDueDate As DateTime
    Public Property BillMethod As String
    Public Property BillTo As String
    Public Property BalanceDueDate As DateTime
    Public Property BalanceDueAmount As String
    Public Property FuturePremium As String

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
        Throw New NotImplementedException()
    End Sub

    Public Overrides Sub Populate()
        If isBillingSummary Then
            lblAccountSum.Text = "Billing Information Change Summary"
            lnkPrint.Visible = True
            lblTranEffDate.Visible = True
            lblTranEffDate.Text = "Effective Date: " & Quote.TransactionEffectiveDate
        Else
            lblAccountSum.Text = "Account Summary"
            lnkPrint.Visible = False
            lblTranEffDate.Visible = False
        End If

        txtPayPlanResult.Text = PayPlanResult
        txtActivityDate.Text = ActivityDate.ToShortDateString
        txtActivityType.Text = ActivityType
        txtAmount.Text = IIf(Amount.TryToGetDouble > 0, " for " + Amount.TryToFormatAsCurreny, "")
        txtNextDueDate.Text = NextDueDate.ToShortDateString
        txtBillMethod.Text = BillMethod
        txtBillTo.Text = BillTo
        txtBalanceDueDate.Text = BalanceDueDate.ToShortDateString
        txtBalanceDueAmount.Text = BalanceDueAmount.TryToFormatAsCurreny
        txtPayInFull.Text = (FuturePremium.TryToGetDouble + BalanceDueAmount.TryToGetDouble).TryToFormatAsCurreny
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(BillAccountSum.ClientID, hdnAccordGenInfo, "0")
        Me.VRScript.StopEventPropagation(Me.lnkPrint.ClientID, False)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

    Protected Sub lnkPrint_Click(sender As Object, e As EventArgs) Handles lnkPrint.Click
        Dim quoteOrPolicyInfo As String = ""
        If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then
            quoteOrPolicyInfo = "ReadOnlyPolicyIdAndImageNum=" & Me.ReadOnlyPolicyId.ToString & "|" & Me.ReadOnlyPolicyImageNum.ToString
        ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
            quoteOrPolicyInfo = "EndorsementPolicyIdAndImageNum=" & Me.EndorsementPolicyId.ToString & "|" & Me.EndorsementPolicyImageNum.ToString
        ElseIf String.IsNullOrWhiteSpace(Me.QuoteId) = False Then
            quoteOrPolicyInfo = "quoteid=" & Me.QuoteId
        End If
        If String.IsNullOrWhiteSpace(quoteOrPolicyInfo) = False Then
            Dim sumType As String = ""
            If Session("SumType") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Session("SumType").ToString) = False AndAlso UCase(Session("SumType").ToString) = "APP" Then
                sumType = "App"
            End If
            Response.Redirect(String.Format("~/Reports/PPA/PFQuoteSummary.aspx?{0}&summarytype={1}", quoteOrPolicyInfo, sumType))
        End If
    End Sub
End Class