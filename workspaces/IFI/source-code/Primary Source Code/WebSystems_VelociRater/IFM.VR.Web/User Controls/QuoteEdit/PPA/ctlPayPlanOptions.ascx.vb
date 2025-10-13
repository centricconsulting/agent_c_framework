Imports QuickQuote.CommonObjects
Imports IFM.VR.Common.Helpers.PPA
Imports QuickQuote.CommonMethods
Public Class ctlPayPlanOptions
    Inherits VRControlBase

    Public Event QuoteRateRequested()

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        'Originally fix pay plan option change by using UseRatedQuoteImage = True, but now it works only when set to False. Do not know what caused this to change. 
        'Keeping this commented code in place in case we need to revert back. 10/03/2019 MLW
        ''Updated 09/27/2019 for bug 40515 MLW
        'If (Me.IsQuoteEndorsement OrElse Me.IsQuoteReadOnly) Then
        '    Me.UseRatedQuoteImage = False
        'Else
        '    Me.UseRatedQuoteImage = True
        'End If
    End Sub

    'Added 10/03/2019 for bug 40515 MLW
    Dim _RatedQuote As QuickQuoteObject = Nothing
    Protected ReadOnly Property RatedQuote As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            If _RatedQuote Is Nothing Then
                Dim errCreateQSO As String = ""
                If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then
                    _RatedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyQuickQuoteObjectForPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, ratedView:=True, errorMessage:=errCreateQSO)
                ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                    _RatedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteForPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, ratedView:=True, errorMessage:=errCreateQSO)
                Else
                    _RatedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.None, Nothing)
                End If
            End If
            Return _RatedQuote
        End Get
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        divPPONotAvailable.Visible = False
        If Quote IsNot Nothing Then
            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso IsQuoteEndorsement() AndAlso IsBillingUpdate() = False Then
                divPPOSection.Visible = False
                divPPONotAvailable.Visible = True
            End If

            'Added 09/25/2019 for bug 40515 MLW
            Dim quotePayPlanId As Int32 = Quote.BillingPayPlanId
            If (Me.IsQuoteEndorsement OrElse Me.IsQuoteReadOnly) Then
                quotePayPlanId = Quote.CurrentPayplanId
            End If

            'Updated 09/25/2019 for bug 40515 MLW
            If String.IsNullOrWhiteSpace(quotePayPlanId) = False AndAlso IsNumeric(quotePayPlanId) Then
                Dim payplanId As Integer = 0
                Dim payPlanOptions As List(Of IFM.VR.Common.Helpers.PPA.PayPlanOptions)
                Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
                Integer.TryParse(quotePayPlanId, payplanId)

                If Quote.BillMethodId = "1" Then
                    rbMonthly.Style.Add("display", "none")
                    rbMonthlyEFT.Style.Add("display", "none")
                    lblPayPlanMonthlyEFTAmount.Style.Add("display", "none")
                    lblPayPlanMonthlyAmount.Style.Add("display", "none")
                End If

                'Updated 10/03/2019 for bug 40515 MLW
                If (Me.IsQuoteEndorsement OrElse Me.IsQuoteReadOnly) Then
                    payPlanOptions = IFM.VR.Common.Helpers.PPA.PPA_Payplans.GetPaymentOptionsBasedOnBillMethod(RatedQuote)
                Else
                    payPlanOptions = IFM.VR.Common.Helpers.PPA.PPA_Payplans.GetPaymentOptionsBasedOnBillMethod(Quote)
                End If
                hfCurrentPayPlanID.Value = quotePayPlanId

'added 9/29/2021... just in case the call is updated to return different ids at some point; still has fall-back logic in case
                Dim annualIds As List(Of Integer) = QQHelper.BillingPayPlanIdsForPayPlanType(QuickQuote.CommonMethods.QuickQuoteHelperClass.PayPlanType.Annual)
                If annualIds Is Nothing Then
                    annualIds = New List(Of Integer)
                End If
                If annualIds.Count = 0 Then
                    annualIds.Add(12)
                    annualIds.Add(20)
End If
                Dim semiAnnualIds As List(Of Integer) = QQHelper.BillingPayPlanIdsForPayPlanType(QuickQuote.CommonMethods.QuickQuoteHelperClass.PayPlanType.SemiAnnual)
                If semiAnnualIds Is Nothing Then
                    semiAnnualIds = New List(Of Integer)
                End If
                If semiAnnualIds.Count = 0 Then
                    semiAnnualIds.Add(13)
                    semiAnnualIds.Add(21)
                End If
                Dim quarterlyIds As List(Of Integer) = QQHelper.BillingPayPlanIdsForPayPlanType(QuickQuote.CommonMethods.QuickQuoteHelperClass.PayPlanType.Quarterly)
                If quarterlyIds Is Nothing Then
                    quarterlyIds = New List(Of Integer)
                End If
                If quarterlyIds.Count = 0 Then
                    quarterlyIds.Add(14)
                    quarterlyIds.Add(22)
                End If
                Dim monthlyIds As List(Of Integer) = QQHelper.BillingPayPlanIdsForPayPlanType(QuickQuote.CommonMethods.QuickQuoteHelperClass.PayPlanType.Monthly)
                If monthlyIds Is Nothing Then
                    monthlyIds = New List(Of Integer)
                End If
                If monthlyIds.Count = 0 Then
                    monthlyIds.Add(15)
                End If
                Dim eftIds As List(Of Integer) = QQHelper.BillingPayPlanIdsForPayPlanType(QuickQuote.CommonMethods.QuickQuoteHelperClass.PayPlanType.EftMonthly)
                If eftIds Is Nothing Then
                    eftIds = New List(Of Integer)
                End If
                If eftIds.Count = 0 Then
                    eftIds.Add(19)
                End If
                Dim rccIds As List(Of Integer) = New List(Of Integer)
                If RccOptionHelper.IsRccOptionAvailable(Quote) = True Then
                    rccIds = QQHelper.BillingPayPlanIdsForPayPlanType(QuickQuote.CommonMethods.QuickQuoteHelperClass.PayPlanType.RccMonthly)
                    If rccIds Is Nothing Then
                        rccIds = New List(Of Integer)
                    End If
                    If rccIds.Count = 0 Then
                        rccIds.Add(18)
                    End If
                End If

                For Each planOption As IFM.VR.Common.Helpers.PPA.PayPlanOptions In payPlanOptions
                    Dim total As String = planOption.DownPayment
                    qqh.ConvertToQuotedPremiumFormat(total)

                    '    Select Case planOption.PayPlanId
                    '        Case 12, 20
                    '            rbAnnual.Checked = payplanId.Equals(planOption.PayPlanId)
                    '            rbAnnual.Attributes.Add("data-PayPlanID", planOption.PayPlanId)
                    '            lblPayPlanAnnualAmount.Text = total
                    '        Case 13, 21
                    '            rbSemiAnnual.Checked = payplanId.Equals(planOption.PayPlanId)
                    '            rbSemiAnnual.Attributes.Add("data-PayPlanID", planOption.PayPlanId)
                    '            lblPayPlanSemiAnnualAmount.Text = total
                    '        Case 14, 22
                    '            rbQuarterly.Checked = payplanId.Equals(planOption.PayPlanId)
                    '            rbQuarterly.Attributes.Add("data-PayPlanID", planOption.PayPlanId)
                    '            lblPayPlanQuarterlyAmount.Text = total
                    '        Case 15
                    '            rbMonthly.Checked = payplanId.Equals(planOption.PayPlanId)
                    '            rbMonthly.Attributes.Add("data-PayPlanID", planOption.PayPlanId)
                    '            lblPayPlanMonthlyAmount.Text = total
                    ''Case 18
                    '        Case 19
                    '            rbMonthlyEFT.Checked = payplanId.Equals(planOption.PayPlanId)
                    '            rbMonthlyEFT.Attributes.Add("data-PayPlanID", planOption.PayPlanId)
                    '            lblPayPlanMonthlyEFTAmount.Text = total
                    '    End Select
                    'updated 9/29/2021... just in case the call is updated to return different ids at some point; still has fall-back logic in case
                    If annualIds.Contains(planOption.PayPlanId) = True Then
                        rbAnnual.Checked = If(annualIds.Contains(payplanId) = True, True, False)
                        rbAnnual.Attributes.Add("data-PayPlanID", planOption.PayPlanId)
                        lblPayPlanAnnualAmount.Text = total
                    ElseIf semiAnnualIds.Contains(planOption.PayPlanId) = True Then
                        rbSemiAnnual.Checked = If(semiAnnualIds.Contains(payplanId) = True, True, False)
                        rbSemiAnnual.Attributes.Add("data-PayPlanID", planOption.PayPlanId)
                        lblPayPlanSemiAnnualAmount.Text = total
                    ElseIf quarterlyIds.Contains(planOption.PayPlanId) = True Then
                        rbQuarterly.Checked = If(quarterlyIds.Contains(payplanId) = True, True, False)
                        rbQuarterly.Attributes.Add("data-PayPlanID", planOption.PayPlanId)
                        lblPayPlanQuarterlyAmount.Text = total
                    ElseIf monthlyIds.Contains(planOption.PayPlanId) = True Then
                        rbMonthly.Checked = If(monthlyIds.Contains(payplanId) = True, True, False)
                        rbMonthly.Attributes.Add("data-PayPlanID", planOption.PayPlanId)
                        lblPayPlanMonthlyAmount.Text = total
                    ElseIf eftIds.Contains(planOption.PayPlanId) = True Then
                        rbMonthlyEFT.Checked = If(eftIds.Contains(payplanId) = True, True, False)
                        rbMonthlyEFT.Attributes.Add("data-PayPlanID", planOption.PayPlanId)
                        lblPayPlanMonthlyEFTAmount.Text = total
                    ElseIf rccIds.Contains(planOption.PayPlanId) = True AndAlso RccOptionHelper.IsRccOptionAvailable(Quote) Then
                        rbMonthlyRCC.Checked = If(rccIds.Contains(payplanId) = True, True, False)
                        rbMonthlyRCC.Attributes.Add("data-PayPlanID", planOption.PayPlanId)
                        lblPayPlayMonthlyRccAmount.Text = total
                    End If
                Next

                'Updated 10/02/2019 for bug 40515 MLW
                'hfQuoteId.Value = Quote.Database_QuoteId
                Dim quoteOrPolicyInfo As String = ""
                If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then
                    quoteOrPolicyInfo = "ReadOnlyPolicyIdAndImageNum=" & Me.ReadOnlyPolicyId.ToString & "%7c" & Me.ReadOnlyPolicyImageNum.ToString
                ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                    quoteOrPolicyInfo = "EndorsementPolicyIdAndImageNum=" & Me.EndorsementPolicyId.ToString & "%7c" & Me.EndorsementPolicyImageNum.ToString
                ElseIf String.IsNullOrWhiteSpace(Me.QuoteId) = False Then
                    quoteOrPolicyInfo = "quoteid=" & Me.QuoteId
                End If
                Dim sumType As String = ""
                If String.IsNullOrWhiteSpace(quoteOrPolicyInfo) = False Then
                    If Session("SumType") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Session("SumType").ToString) = False AndAlso UCase(Session("SumType").ToString) = "APP" Then
                        sumType = "App"
                    End If
                End If
                hfQuoteId.Value = quoteOrPolicyInfo
                hfSumType.Value = sumType
            Else
                Throw New System.Exception("BillingPayPlanId of '" & quotePayPlanId & "' is not valid")
            End If

            If IFM.VR.Common.Helpers.PPA.RccOptionHelper.IsRccOptionAvailable(Quote) Then
                RccOptions.Visible = True
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        ''If Session("SumType") = "" Then
        If IsOnAppPage = False Then
            'Updated 09/30/2019 for bug 40515 MLW
            If (Me.IsQuoteEndorsement OrElse Me.IsQuoteReadOnly) Then
                Dim payplanid As String = GetSelectedPayPlanId()
                If String.IsNullOrWhiteSpace(payplanid) AndAlso String.IsNullOrWhiteSpace(Quote.CurrentPayplanId) = False AndAlso IsNumeric(Quote.CurrentPayplanId) Then
                    payplanid = Quote.CurrentPayplanId
                End If
                Me.Quote.OnlyUsePropertyToSetFieldWithSameName = True
                Quote.CurrentPayplanId = payplanid
            Else
                Dim payplanid As String = GetSelectedPayPlanId()
                If String.IsNullOrWhiteSpace(payplanid) AndAlso String.IsNullOrWhiteSpace(Quote.BillingPayPlanId) = False AndAlso IsNumeric(Quote.BillingPayPlanId) Then
                    payplanid = Quote.BillingPayPlanId
                End If
                Quote.BillingPayPlanId = payplanid
            End If
        End If
        Return True
    End Function

    Private Sub doRate()
        'Added 09/25/2019 for bug 40515 MLW
        Dim quotePayPlanId = Quote.BillingPayPlanId
        If (Me.IsQuoteEndorsement OrElse Me.IsQuoteReadOnly) Then
            quotePayPlanId = Quote.CurrentPayplanId
        End If

        Dim payplanid As String = GetSelectedPayPlanId()
        'Updated 09/25/2019 for bug 40515 MLW
        If quotePayPlanId <> payplanid Then
            Save()
            RaiseEvent QuoteRateRequested()
        End If
    End Sub

    Private Sub btnRate_Click() Handles btnRate.Click
        doRate()
    End Sub

    Private Sub btnRateAndDisplayPrintFriendly_Click() Handles btnRateAndDisplayPrintFriendly.Click
        hfRequestPrintFriendly.Value = True
        doRate()
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.divPayPlanOptions.ClientID, HiddenField1, "false")
    End Sub

    Private Function GetSelectedPayPlanId() As String
        Dim payplanid As String = ""

        '9/29/2021 note: would need to be updated if we ever start loading payplans based on effDate (would rely on VR_Default_PayPlanIds and VR_ConvertPayPlanIdsIfNeeded config keys until then)
        If Quote.BillMethodId = "1" Then
            Select Case True
                Case rbAnnual.Checked
                    payplanid = 20
                Case rbSemiAnnual.Checked
                    payplanid = 21
                Case rbQuarterly.Checked
                    payplanid = 22
            End Select
        Else
            Select Case True
                Case rbAnnual.Checked
                    payplanid = 12
                Case rbSemiAnnual.Checked
                    payplanid = 13
                Case rbQuarterly.Checked
                    payplanid = 14
                Case rbMonthly.Checked
                    payplanid = 15
                Case rbMonthlyEFT.Checked
                    payplanid = 19
                Case rbMonthlyRCC.Checked
                    payplanid = 18
            End Select
        End If

        Return payplanid
    End Function
End Class