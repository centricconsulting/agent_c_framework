Imports QuickQuote.CommonMethods
Imports IFM.VR.Common.Helpers.PPA

Public Class ctlBillingChangeSummary
    Inherits VRControlBase

    'Added 7/24/2019 for Auto Endorsements Task 32773 MLW

    'Added 05/18/2020 for bug 43588 MLW
    Public Event QuoteRateRequested()

    Public Property billingData As Diamond.Common.Objects.Billing.Data

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        ctlBillingChangeSummary.isBillingSummary = True
        ctlChangeBillingDetails.isBillingSummary = True
        If Me.Quote IsNot Nothing Then
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
            Else
                billingData = New Diamond.Common.Objects.Billing.Data
            End If

            ' Fill Child controls data properties
            If billingData.Statements IsNot Nothing AndAlso billingData.Statements.Count > 0 Then
                ' Added 5/19/2020 for bug 45235 - ZTS
                Dim statementsReversed As New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Billing.Statement)
                For i = billingData.Statements.Count - 1 To 0 Step -1
                    statementsReversed.Add(billingData.Statements.Item(i))
                Next
                'ctlChangeBillingDetails.StatementList = billingData.Statements
                ctlChangeBillingDetails.StatementList = statementsReversed
            End If

            ' Use to get xml dumps of items
            'If billingData IsNot Nothing AndAlso billingData.ResponseData IsNot Nothing Then
            '        billingData.DumpToFile("C:\Users\mawil\Desktop\BillingData.xml")
            '        billResAccountInfo.ResponseData.AccountInfos.DumpToFile("C:\Users\mawil\Desktop\billResAccountInfo.xml")
            '        billResBillingSummary.ResponseData.BillingSummary.DumpToFile("C:\Users\mawil\Desktop\billResBillingSummary.xml")

            'End If

            'Added 5/18/2020 for bug 43588 MLW - do not show pay plan options when RCC pay plan type
            'If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso Me.Quote.CurrentPayplanId <> "18" Then
            'updated 9/26/2021
            Dim okayToShowPayPlans As Boolean = False
            If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                Dim currentPayPlanTxt As String = ""
                If QQHelper.IsPositiveIntegerString(Me.Quote.CurrentPayplanId) = True Then
                    currentPayPlanTxt = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.CurrentPayplanId, Me.Quote.CurrentPayplanId)
                End If
                If RccOptionHelper.IsRccOptionAvailable(Quote) = True OrElse (String.IsNullOrWhiteSpace(currentPayPlanTxt) = True OrElse UCase(currentPayPlanTxt).Contains("CREDIT CARD") = False) Then
                    okayToShowPayPlans = True
                End If
            End If
            If okayToShowPayPlans = True Then
                Me.ctlPayPlanOptions.Visible = True
                Me.ctlPayPlanOptions.Populate()
            Else
                Me.ctlPayPlanOptions.Visible = False
            End If

            PopulateBillingChildControls()
                PopulateChildControls()
            End If

    End Sub

    'Added 05/18/2020 for bug 43588 MLW
    Private Sub HandleRateRequest() Handles ctlPayPlanOptions.QuoteRateRequested
        RaiseEvent QuoteRateRequested()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

    Public Sub PopulateBillingChildControls()

        If billingData.PolicyId <> 0 Then
            'Updated 09/25/2019 for bug 40515 MLW
            ctlBillingChangeSummary.PayPlanResult = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillingPayPlanId, Quote.CurrentPayplanId)
            ctlBillingChangeSummary.PayPlanResult = ctlBillingChangeSummary.PayPlanResult.Replace("2", "").Trim()
            If ctlBillingChangeSummary.PayPlanResult = "RENEWAL CREDIT CARD MONTHLY" OrElse ctlBillingChangeSummary.PayPlanResult = "Renewal Credit Card Monthly" Then
                ctlBillingChangeSummary.PayPlanResult = "Recurring Credit Card"
            End If
            ctlBillingChangeSummary.BillMethod = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, Quote.BillMethodId, Quote.LobType)
            ctlBillingChangeSummary.BillTo = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, Quote.BillToId, Quote.LobType)
            ctlBillingChangeSummary.ActivityDate = IIf(String.IsNullOrWhiteSpace(billingData.NextActivity.ActivityDate), Date.MinValue, billingData.NextActivity.ActivityDate)
            ctlBillingChangeSummary.ActivityType = billingData.NextActivity.ActivityType
            ctlBillingChangeSummary.NextDueDate = IIf(String.IsNullOrWhiteSpace(billingData.NextActivity.NextDueDate), Date.MinValue, billingData.NextActivity.NextDueDate)
            ctlBillingChangeSummary.Amount = billingData.NextActivity.Amount

            ctlBillingChangeSummary.BalanceDueAmount = billingData.Invoice.CurrentItem.CurrentOutstandingAmount
            ctlBillingChangeSummary.BalanceDueDate = billingData.Invoice.CurrentItem.DueDate
            ctlBillingChangeSummary.FuturePremium = billingData.Invoice.CurrentItem.FuturePremiumAmount

        End If
    End Sub

End Class