Imports IFM.VR.Common.Helpers.PPA
Imports QuickQuote.CommonMethods
Public Class ctlBillingInfo
    Inherits VRControlBase

    Public Event QuoteRateRequested()

    Public Property billingData As Diamond.Common.Objects.Billing.Data

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
        If Me.Visible Then
            If Me.Quote IsNot Nothing Then
                Dim callBillingPreviewLoad As Boolean = False
                Dim callNormalBillingLoad As Boolean = True
                If Me.Quote.PolicyCurrentStatus = QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.Pending Then
                    'Policy has pending status
                    callBillingPreviewLoad = True
                    callNormalBillingLoad = False 'should never need to call normal billing load
                ElseIf Me.Quote.PolicyStatusCode = QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyStatusCode.Pending AndAlso QQHelper.IsPositiveDecimalString(Me.Quote.TotalQuotedPremium) = True Then
                    'Image has pending status and has been rated
                    callBillingPreviewLoad = True
                Else
                    'Policy and image do not have pending status; just normal billing load
                End If
                If callBillingPreviewLoad = True Then
                    'call load preview
                    Dim billPreviewReq As New Diamond.Common.Services.Messages.BillingService.LoadPreview.Request
                    Dim billPreviewRes As New Diamond.Common.Services.Messages.BillingService.LoadPreview.Response

                    billPreviewReq.RequestData.PolicyId = QQHelper.IntegerForString(Me.Quote.PolicyId)
                    billPreviewReq.RequestData.PolicyImageNum = QQHelper.IntegerForString(Me.Quote.PolicyImageNum)
                    billPreviewReq.RequestData.BillingTransactionTypeId = 0

                    Try
                        Using diaServ As New Diamond.Common.Services.Proxies.BillingServiceProxy
                            billPreviewRes = diaServ.LoadPreview(billPreviewReq)
                        End Using
                    Catch ex As Exception

                    End Try

                    If billPreviewRes IsNot Nothing AndAlso billPreviewRes.ResponseData IsNot Nothing AndAlso billPreviewRes.ResponseData.BillingData IsNot Nothing Then
                        billingData = billPreviewRes.ResponseData.BillingData
                        callNormalBillingLoad = False
                    Else
                        If callNormalBillingLoad = False Then 'will be instantiate below if needed
                            billingData = New Diamond.Common.Objects.Billing.Data
                        End If
                    End If
                End If
                If callNormalBillingLoad = True Then
                    'call normal billing load
                    Dim billReq As New Diamond.Common.Services.Messages.BillingService.Load.Request
                    Dim billRes As New Diamond.Common.Services.Messages.BillingService.Load.Response

                    billReq.RequestData.PolicyId = QQHelper.IntegerForString(Me.Quote.PolicyId)

                    Try
                        Using diaServ As New Diamond.Common.Services.Proxies.BillingServiceProxy
                            billRes = diaServ.Load(billReq)
                        End Using
                    Catch ex As Exception

                    End Try

                    If billRes IsNot Nothing AndAlso billRes.ResponseData IsNot Nothing AndAlso billRes.ResponseData.BillingData IsNot Nothing Then
                        billingData = billRes.ResponseData.BillingData
                    Else
                        billingData = New Diamond.Common.Objects.Billing.Data
                    End If
                End If

                ' Fill Child controls data properties
                If billingData.Statements IsNot Nothing AndAlso billingData.Statements.Count > 0 Then
                    ' Added 5/19/2020 for bug 45235 - ZTS
                    Dim statementsReversed As New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Billing.Statement)
                    For i = billingData.Statements.Count - 1 To 0 Step -1
                        statementsReversed.Add(billingData.Statements.Item(i))
                    Next
                    'billingStatementInfo.StatementList = billingData.Statements
                    billingStatementInfo.StatementList = statementsReversed
                End If
                If billingData.Futures IsNot Nothing AndAlso billingData.Futures.Count > 0 Then
                    billingFutureInfo.FutureList = billingData.Futures
                End If
                If billingData.AccountHistory IsNot Nothing AndAlso billingData.AccountHistory.Count > 0 Then
                    billingTransactionHistory.HistoryList = billingData.AccountHistory
                End If

                ' Use to get xml dumps of items
                'If billingData IsNot Nothing Then
                '    billingData.DumpToFile("C:\Users\chhaw\Desktop\BillingData.xml")
                '    'billResBillingSummary.ResponseData.BillingSummary.DumpToFile("C:\Users\chhaw\Desktop\billResBillingSummary.xml")

                'End If

                If IsBillingUpdate() AndAlso IsQuoteEndorsement() Then
                    ctl_Billing_Info_PPA.Visible = True
                    btnPrintHistory.Visible = False
                    btnPolicyHistory.Visible = False
                    btnRate.Visible = True
                    'If Me.Quote.BillingPayPlanId = "18" Then
                    'updated 9/24/2021 to handle for all RCC payplans (text should be "Credit Card Monthly" or "Renewal Credit Card Monthly"); note: would also include "Account Bill Credit Card Monthly"
                    Dim payPlanTxt As String = ""
                    If QQHelper.IsPositiveIntegerString(Me.Quote.BillingPayPlanId) = True Then
                        payPlanTxt = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillingPayPlanId, Me.Quote.BillingPayPlanId)
                    End If
                    If String.IsNullOrWhiteSpace(payPlanTxt) = False AndAlso UCase(payPlanTxt).Contains("CREDIT CARD") = True AndAlso RccOptionHelper.IsRccOptionAvailable(Quote) = False Then
                        btnRate.Enabled = False
                    End If
                Else
                    ctl_Billing_Info_PPA.Visible = False
                    btnPrintHistory.Visible = False 'Task 40643
                    btnRate.Visible = False
                End If

                PopulateBillingChildControls()
                PopulateChildControls()
            End If
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        ctl_Billing_Info_PPA.Save()
        Return True
    End Function

    Public Sub PopulateBillingChildControls()
        If billingData.PolicyId <> 0 Then
            'Updated 09/26/2019 for bug 40515 MLW
            billingAccountSummary.PayPlanResult = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillingPayPlanId, Quote.CurrentPayplanId)
            billingAccountSummary.PayPlanResult = billingAccountSummary.PayPlanResult.Replace("2", "").Trim()
            'Added 8/20/2019 for Auto & Home Endorsements Tasks 32701/32773 & 38926 MLW
            If billingAccountSummary.PayPlanResult = "RENEWAL CREDIT CARD MONTHLY" OrElse billingAccountSummary.PayPlanResult = "Renewal Credit Card Monthly" Then
                billingAccountSummary.PayPlanResult = "Recurring Credit Card"
            End If

            billingAccountSummary.BillMethod = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, Quote.BillMethodId, Quote.LobType)
            billingAccountSummary.BillTo = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, Quote.BillToId, Quote.LobType)
            billingAccountSummary.ActivityDate = IIf(String.IsNullOrWhiteSpace(billingData.NextActivity.ActivityDate), Date.MinValue, billingData.NextActivity.ActivityDate)
            billingAccountSummary.ActivityType = billingData.NextActivity.ActivityType
            billingAccountSummary.NextDueDate = IIf(String.IsNullOrWhiteSpace(billingData.NextActivity.NextDueDate), Date.MinValue, billingData.NextActivity.NextDueDate)
            billingAccountSummary.Amount = billingData.NextActivity.Amount

            billingAccountSummary.BalanceDueAmount = billingData.Invoice.CurrentItem.CurrentOutstandingAmount
            billingAccountSummary.BalanceDueDate = billingData.Invoice.CurrentItem.DueDate
            billingAccountSummary.FuturePremium = billingData.Invoice.CurrentItem.FuturePremiumAmount

            billingFutureInfo.FuturePremium = billingData.Invoice.CurrentItem.FuturePremiumAmount
            billingFutureInfo.FutureMiscCharges = billingData.Invoice.CurrentItem.FutureMchgAmount
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ctl_Billing_Info_PPA.ValidateControl(valArgs)
    End Sub

    Private Sub btnPrintHistory_Click(sender As Object, e As EventArgs) Handles btnPrintHistory.Click
        ' Send to Policy History
        Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.printHistory, "0")
    End Sub

    'Added 10/14/2019 for task 32701 and 40643 MLW
    Private Sub btnPolicyHistory_Click(sender As Object, e As EventArgs) Handles btnPolicyHistory.Click
        ' Send to Policy History
        Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.policyHistory, "0")
    End Sub

    Private Sub btnRate_Click(sender As Object, e As EventArgs) Handles btnRate.Click
        RaiseEvent QuoteRateRequested()
    End Sub

End Class