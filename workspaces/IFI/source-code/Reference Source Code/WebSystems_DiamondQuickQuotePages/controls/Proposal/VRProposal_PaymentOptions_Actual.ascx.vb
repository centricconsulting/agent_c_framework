Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Partial Class controls_Proposal_VRProposal_PaymentOptions_Actual 'added 9/16/2017
    Inherits System.Web.UI.UserControl

    Dim qqHelper As New QuickQuoteHelperClass

    Private _QuoteObject As QuickQuoteObject
    Public Property QuoteObject As QuickQuoteObject
        Get
            Return _QuoteObject
        End Get
        Set(value As QuickQuoteObject)
            _QuoteObject = value
            LoadPaymentOptions()
        End Set
    End Property
    Private _ProposalObject As QuickQuoteProposalObject
    Public Property ProposalObject As QuickQuoteProposalObject
        Get
            Return _ProposalObject
        End Get
        Set(value As QuickQuoteProposalObject)
            _ProposalObject = value
            LoadPaymentOptions()
        End Set
    End Property
    Private _PaymentOptions As List(Of QuickQuotePaymentOption)
    Public Property PaymentOptions As List(Of QuickQuotePaymentOption)
        Get
            Return _PaymentOptions
        End Get
        Set(value As List(Of QuickQuotePaymentOption))
            _PaymentOptions = value
            LoadPaymentOptions()
        End Set
    End Property
    Public ReadOnly Property HasVisiblePaymentOptions As Boolean
        Get
            Return Me.pnlPaymentOptions.Visible
        End Get
    End Property

    Private Sub controls_Proposal_VRProposal_PaymentOptions_Actual_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Me.IsPostBack = False Then

        End If
    End Sub

    Private Sub LoadPaymentOptions()
        Me.pnlPaymentOptions.Visible = False
        Dim paymentOptionsToUse As List(Of QuickQuotePaymentOption) = Nothing

        If _QuoteObject IsNot Nothing AndAlso _QuoteObject.PaymentOptions IsNot Nothing AndAlso _QuoteObject.PaymentOptions.Count > 0 Then
            paymentOptionsToUse = _QuoteObject.PaymentOptions
        ElseIf _ProposalObject IsNot Nothing AndAlso _ProposalObject.CombinedPaymentOptions IsNot Nothing AndAlso _ProposalObject.CombinedPaymentOptions.Count > 0 Then
            paymentOptionsToUse = _ProposalObject.CombinedPaymentOptions
        ElseIf _PaymentOptions IsNot Nothing AndAlso _PaymentOptions.Count > 0 Then
            paymentOptionsToUse = _PaymentOptions
        End If

        'added 10/2/2017
        'Dim listOfTxtToIgnore As List(Of String) = Nothing
        ''QuickQuoteHelperClass.AddStringToList("Account Bill", listOfTxtToIgnore)
        ''QuickQuoteHelperClass.AddStringToList("MTG", listOfTxtToIgnore)
        ''Dim strTxtToIgnore As String = QuickQuoteHelperClass.configAppSettingValueAsString("QuickQuote_Proposal_ActualPaymentOptions_DescriptionTextToIgnore")
        ''If String.IsNullOrWhiteSpace(strTxtToIgnore) = False Then
        ''    listOfTxtToIgnore = QuickQuoteHelperClass.ListOfStringFromString(strTxtToIgnore, delimiter:="|", returnPairForEachDelimiter:=False)
        ''End If
        'Dim phc As New ProposalHelperClass
        'listOfTxtToIgnore = phc.ActualPaymentOptions_DescriptionTextToIgnore()
        'Dim filteredPaymentOptions As List(Of QuickQuotePaymentOption) = QuickQuoteHelperClass.QuickQuotePaymentOptionsWithoutMatchingTextInDescription(paymentOptionsToUse, listOfTxtToIgnore)
        ''qqHelper.SortPaymentOptions(filteredPaymentOptions, sortBy:=QuickQuotePaymentOption.SortBy.NumberOfInstallmentsDescendingThenFriendlyDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        ''qqHelper.SortPaymentOptions(filteredPaymentOptions, sortBy:=QuickQuotePaymentOption.SortBy.FriendlyDescriptionAscending, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        'Dim sortBy As QuickQuotePaymentOption.SortBy = phc.ActualPaymentOptions_SortBy()
        'If sortBy <> Nothing AndAlso sortBy <> QuickQuotePaymentOption.SortBy.None Then
        '    qqHelper.SortPaymentOptions(filteredPaymentOptions, sortBy:=sortBy, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
        'End If
        'now using helper methods for everything; so it can easily be used by other projects (i.e. VR Personal Printer Friendly Summaries)
        Dim phc As New ProposalHelperClass
        Dim filteredPaymentOptions As List(Of QuickQuotePaymentOption) = phc.ActualPaymentOptions_FilteredAndSorted(paymentOptionsToUse)

        'If paymentOptionsToUse IsNot Nothing AndAlso paymentOptionsToUse.Count > 0 Then
        'updated 10/2/2017 to use filtered list
        If filteredPaymentOptions IsNot Nothing AndAlso filteredPaymentOptions.Count > 0 Then
            Dim dtPaymentOptions As New Data.DataTable
            dtPaymentOptions.Columns.Add("Description", System.Type.GetType("System.String"))
            dtPaymentOptions.Columns.Add("DownPayment", System.Type.GetType("System.String"))
            dtPaymentOptions.Columns.Add("NumInstalls", System.Type.GetType("System.String"))
            dtPaymentOptions.Columns.Add("InstallAmt", System.Type.GetType("System.String"))
            dtPaymentOptions.Columns.Add("InstallChg", System.Type.GetType("System.String"))
            dtPaymentOptions.Columns.Add("TotalInstallAmt", System.Type.GetType("System.String"))
            dtPaymentOptions.Columns.Add("DueEvery", System.Type.GetType("System.String"))

            'For Each po As QuickQuotePaymentOption In paymentOptionsToUse
            'updated 10/2/2017 to use filtered list
            For Each po As QuickQuotePaymentOption In filteredPaymentOptions
                If po IsNot Nothing Then
                    Dim drPaymentOption As Data.DataRow = dtPaymentOptions.NewRow
                    drPaymentOption.Item("Description") = po.FriendlyDescription 'maybe use po.FriendlyDescription
                    drPaymentOption.Item("DownPayment") = po.DepositAmount
                    drPaymentOption.Item("NumInstalls") = po.NumInstalls
                    drPaymentOption.Item("InstallAmt") = po.InstallmentAmount
                    drPaymentOption.Item("InstallChg") = po.InstallmentFee
                    drPaymentOption.Item("TotalInstallAmt") = po.TotalInstallmentAmount
                    drPaymentOption.Item("DueEvery") = po.InstallmentInterval
                    dtPaymentOptions.Rows.Add(drPaymentOption)
                End If
            Next

            If dtPaymentOptions IsNot Nothing AndAlso dtPaymentOptions.Rows IsNot Nothing AndAlso dtPaymentOptions.Rows.Count > 0 Then
                Me.rptPaymentOptions.DataSource = dtPaymentOptions
                Me.rptPaymentOptions.DataBind()

                Me.pnlPaymentOptions.Visible = True
            End If
        End If
    End Sub
End Class
