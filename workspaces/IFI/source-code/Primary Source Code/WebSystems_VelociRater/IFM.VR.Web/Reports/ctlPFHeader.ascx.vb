Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Public Class ctlPFHeader
    Inherits System.Web.UI.UserControl

    Private _Quote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
    Public Property billingData As Diamond.Common.Objects.Billing.Data

    Public Property Quote As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            Return _Quote
        End Get
        Set(value As QuickQuote.CommonObjects.QuickQuoteObject)
            _Quote = value
            LoadQuoteInfo()
        End Set
    End Property
    Private _QQHelper As QuickQuote.CommonMethods.QuickQuoteHelperClass
    Public ReadOnly Property QQHelper As QuickQuote.CommonMethods.QuickQuoteHelperClass
        Get
            If _QQHelper Is Nothing Then
                _QQHelper = New QuickQuote.CommonMethods.QuickQuoteHelperClass
            End If
            Return _QQHelper
        End Get
    End Property
    Private _SummaryHelper As SummaryHelperClass
    Public ReadOnly Property SummaryHelper As SummaryHelperClass
        Get
            If _SummaryHelper Is Nothing Then
                _SummaryHelper = New SummaryHelperClass
            End If
            Return _SummaryHelper
        End Get
    End Property

    'Added 7/26/2019 for Auto Endorsements Task 32783 MLW
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub LoadQuoteInfo()

        If _Quote IsNot Nothing Then
            With _Quote
                'note: client info may look different for other LOBs; this
                Dim Name As String = .Policyholder.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />")
                Select Case Me.Quote.LobType
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal, 
                         QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal, 
                         QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                        'Only show policyholder2 on personal lines
                        If .Policyholder2?.Name?.DisplayNameForWeb IsNot Nothing AndAlso .Policyholder2.Name.DisplayNameForWeb.IsNullEmptyorWhitespace() = False Then
                            Name &= "<br />" & .Policyholder2.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />")
                        End If
                End Select
                Me.lblClientInfo.Text = QQHelper.appendText(Name, .Policyholder.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                'Updated 7/17/2019 for Home Endorsements Task 38921 MLW
                If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
                    Me.lblClientInfo.Text = QQHelper.appendText(Me.lblClientInfo.Text, "<br />")
                Else
                    Me.lblClientInfo.Text = QQHelper.appendText(Me.lblClientInfo.Text, .Policyholder.PrimaryPhone, "<br />")
                End If

                Me.lblAgencyInfo.Text = QQHelper.appendText(SummaryHelper.GetAgencyName(.Agency.Name).Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), .Agency.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                Me.lblAgencyInfo.Text = QQHelper.appendText(Me.lblAgencyInfo.Text, .Agency.PrimaryPhone, "<br />")


                If String.IsNullOrWhiteSpace(.AgencyProducerCode) = False Then
                    Me.lblProducerCode.Text = .AgencyProducerCode
                Else
                    Me.pnlProducer.Visible = False
                End If

                Me.lblPolNum.Text = .PolicyNumber
                Me.lblTranEffDt.Text = .TransactionEffectiveDate



                'Updated 7/26/2019 for task 32783 MLW
                If IsBillingUpdate() Then
                    divEndorsementSpace.Visible = True
                    trPFLine1Space.Visible = True
                    trPFLine2Space.Visible = True
                    trPFLine3Space.Visible = True
                    tdTotalAnnualPremiumPrior.Visible = False
                    trTotalAnnualPremiumAfter.Visible = False
                    tdBillMethod.Visible = True
                    trBillToAndPayPlan.Visible = True
                    trNextPaymentAmountAndDue.Visible = True
                    tdImageRemarks.RowSpan = 1

                    'Me.lblBillMethod.Text = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, Quote.BillMethodId, Quote.LobType)
                    'updated 9/27/2021 to not include lobType; not needed... would only be needed if we had the same id w/ different text for different lobs
                    Me.lblBillMethod.Text = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, Quote.BillMethodId)
                    Me.lblImgRemarks.Text = .TransactionRemark
                    'Me.lblCurrentBillTo.Text = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, .BillToId, Quote.LobType)
                    'updated 9/27/2021 to not include lobType; not needed (and could cause issues whenever staticData specifies lobs for the option but it doesn't include the one passed in)... would only be needed if we had the same id w/ different text for different lobs
                    Me.lblCurrentBillTo.Text = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillToId, .BillToId)
                    'Me.lblPayPlan.Text = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillingPayPlanId, .CurrentPayplanId, Quote.LobType)
                    'updated 9/27/2021 to not include lobType; not needed (and could cause issues whenever staticData specifies lobs for the option but it doesn't include the one passed in)... would only be needed if we had the same id w/ different text for different lobs
                    Me.lblPayPlan.Text = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillingPayPlanId, .CurrentPayplanId)
                    Me.lblPayPlan.Text = (Me.lblPayPlan.Text).Replace("2", "").Trim()
                    If Me.lblPayPlan.Text = "RENEWAL CREDIT CARD MONTHLY" OrElse Me.lblPayPlan.Text = "Renewal Credit Card Monthly" Then
                        Me.lblPayPlan.Text = "Recurring Credit Card"
                    End If
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

                    End If
                    If (billingData?.NextActivity?.Amount IsNot Nothing) Then
                        Me.lblNextPaymentAmount.Text = (billingData.NextActivity.Amount).ToString("C")
                    End If

                    Dim nextDueDate As Date = billingData.NextActivity.NextDueDate
                    Me.lblNextPaymentDue.Text = IIf(String.IsNullOrWhiteSpace(billingData.NextActivity.NextDueDate), Date.MinValue, nextDueDate.ToShortDateString)

                Else
                    'Updated 01/15/2020 for task/bug 42504 KLJ
                    'Updated 03/05/2020 for task/buy 44887 KLJ
                    Dim priorPrem As String = ""

                    divEndorsementSpace.Visible = False
                    trPFLine1Space.Visible = False
                    trPFLine2Space.Visible = False
                    trPFLine3Space.Visible = False
                    tdTotalAnnualPremiumPrior.Visible = True
                    trTotalAnnualPremiumAfter.Visible = True
                    tdBillMethod.Visible = False
                    trBillToAndPayPlan.Visible = False
                    trNextPaymentAmountAndDue.Visible = False
                    tdImageRemarks.RowSpan = 2

                    If QQHelper.IsNumericString(.TotalQuotedPremium) = True AndAlso QQHelper.IsNumericString(.ChangeInFullTermPremium) = True Then
                        priorPrem = QQHelper.QuotedPremiumFormat(QQHelper.getDiff(.TotalQuotedPremium, .ChangeInFullTermPremium))
                    End If
                    If QQHelper.IsNumericString(priorPrem) = False Then
                        priorPrem = .TotalQuotedPremium
                    End If
                    Me.lblPriorAnnualPrem.Text = priorPrem
                    Me.lblImgRemarks.Text = .TransactionRemark
                    Me.lblAnnualPrem.Text = .TotalQuotedPremium

                    'Updated 01/15/2020 for task/bug 42504 KLJ
                    tdImageRemarks.RowSpan = 1
                    Dim premiumChangeDifference = Convert.ToDecimal(.TotalQuotedPremium.TrimStart("$")) - Convert.ToDecimal(priorPrem.TrimStart("$"))
                    Me.lblPremiumChange.Text = String.Format("{0:$#,##0.00;($#,##0.00)}", premiumChangeDifference)

                End If
                Select Case .QuoteTransactionType
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                        Me.lblTranEffDtImageOrChange.Text = "Change"
                        Me.lblImgRemarksImageOrChange.Text = "Policy Changes"
                        'Updated 7/26/2019 for task 32783 MLW
                        If Not IsBillingUpdate() Then
                            Me.lblPriorAnnualPremImageOrChange.Text = "Change"
                            Me.lblAnnualPremImageOrChange.Text = "Change"
                        End If
                    Case Else
                        'labels should already be set w/ default text
                End Select
            End With
        End If
    End Sub

End Class