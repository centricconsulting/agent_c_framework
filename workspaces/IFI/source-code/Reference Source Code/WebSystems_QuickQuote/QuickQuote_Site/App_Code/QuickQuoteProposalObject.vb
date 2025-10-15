Imports System.Configuration
Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store quote proposal information
    ''' </summary>
    ''' <remarks>holds multiple QuickQuoteObjects (<see cref="QuickQuoteObject"/>)</remarks>
    <Serializable()> _
    Public Class QuickQuoteProposalObject 'added 4/30/2013 pm; 8/4/2014 note: will not be inheriting QuickQuoteBaseObject
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass
        Dim QQxml As New QuickQuoteXML

        Private _QuoteIds As Generic.List(Of String)
        Private _ValidQuoteIds As Generic.List(Of String)
        Private _QuickQuoteObjects As Generic.List(Of QuickQuoteObject)
        Private _ErrorMessages As Generic.List(Of String)
        Private _CombinedPremium As String
        'added 5/1/2013
        'Private _ClientId As String 'will hold last clientId if there are different ones
        'Private _ClientName As QuickQuoteName 'will hold last client name if there are more than 1
        Private _Client As QuickQuoteClient 'decided to use entire client instead of just client id and name
        Private _HasMultipleClients As Boolean
        Private _Agency As QuickQuoteAgency
        Private _HasMultipleAgencies As Boolean
        Private _AnnualPaymentOption As QuickQuotePaymentOption
        Private _SemiAnnualPaymentOption As QuickQuotePaymentOption
        Private _QuarterlyPaymentOption As QuickQuotePaymentOption
        Private _MonthlyPaymentOption As QuickQuotePaymentOption
        Private _EftMonthlyPaymentOption As QuickQuotePaymentOption
        Private _CreditCardMonthlyPaymentOption As QuickQuotePaymentOption
        Private _RenewalCreditCardMonthlyPaymentOption As QuickQuotePaymentOption
        Private _RenewalEftMonthlyPaymentOption As QuickQuotePaymentOption
        Private _AnnualMtgPaymentOption As QuickQuotePaymentOption
        'added logo stuff 5/8/2013
        Private _LogoSavePath As String
        Private _LogoWebPath As String

        'added 3/22/2017
        Private _PolicyRecords As List(Of QuickQuotePolicyLookupInfo)
        Private _ValidPolicyRecords As List(Of QuickQuotePolicyLookupInfo)
        Private _DiamondProposal As QuickQuoteDiamondProposal 'added 3/30/2017

        'added 9/4/2017
        Private _CombinedPaymentOptions As List(Of QuickQuotePaymentOption)

        Public Property QuoteIds As Generic.List(Of String)
            Get
                Return _QuoteIds
            End Get
            Set(value As Generic.List(Of String))
                _QuoteIds = value
            End Set
        End Property
        Public Property ValidQuoteIds As Generic.List(Of String)
            Get
                Return _ValidQuoteIds
            End Get
            Set(value As Generic.List(Of String))
                _ValidQuoteIds = value
            End Set
        End Property
        Public Property QuickQuoteObjects As Generic.List(Of QuickQuoteObject)
            Get
                Return _QuickQuoteObjects
            End Get
            Set(value As Generic.List(Of QuickQuoteObject))
                _QuickQuoteObjects = value
            End Set
        End Property
        Public Property ErrorMessages As Generic.List(Of String)
            Get
                Return _ErrorMessages
            End Get
            Set(value As Generic.List(Of String))
                _ErrorMessages = value
            End Set
        End Property
        Public Property CombinedPremium As String
            Get
                'Return _CombinedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_CombinedPremium)
            End Get
            Set(value As String)
                _CombinedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CombinedPremium)
            End Set
        End Property
        'Public Property ClientId As String
        '    Get
        '        Return _ClientId
        '    End Get
        '    Set(value As String)
        '        _ClientId = value
        '    End Set
        'End Property
        'Public Property ClientName As QuickQuoteName
        '    Get
        '        Return _ClientName
        '    End Get
        '    Set(value As QuickQuoteName)
        '        _ClientName = value
        '    End Set
        'End Property
        Public Property Client As QuickQuoteClient
            Get
                Return _Client
            End Get
            Set(value As QuickQuoteClient)
                _Client = value
            End Set
        End Property
        Public Property HasMultipleClients As Boolean
            Get
                Return _HasMultipleClients
            End Get
            Set(value As Boolean)
                _HasMultipleClients = value
            End Set
        End Property
        Public Property Agency As QuickQuoteAgency
            Get
                Return _Agency
            End Get
            Set(value As QuickQuoteAgency)
                _Agency = value
            End Set
        End Property
        Public Property HasMultipleAgencies As Boolean
            Get
                Return _HasMultipleAgencies
            End Get
            Set(value As Boolean)
                _HasMultipleAgencies = value
            End Set
        End Property
        Public Property AnnualPaymentOption As QuickQuotePaymentOption
            Get
                Return _AnnualPaymentOption
            End Get
            Set(value As QuickQuotePaymentOption)
                _AnnualPaymentOption = value
            End Set
        End Property
        Public Property SemiAnnualPaymentOption As QuickQuotePaymentOption
            Get
                Return _SemiAnnualPaymentOption
            End Get
            Set(value As QuickQuotePaymentOption)
                _SemiAnnualPaymentOption = value
            End Set
        End Property
        Public Property QuarterlyPaymentOption As QuickQuotePaymentOption
            Get
                Return _QuarterlyPaymentOption
            End Get
            Set(value As QuickQuotePaymentOption)
                _QuarterlyPaymentOption = value
            End Set
        End Property
        Public Property MonthlyPaymentOption As QuickQuotePaymentOption
            Get
                Return _MonthlyPaymentOption
            End Get
            Set(value As QuickQuotePaymentOption)
                _MonthlyPaymentOption = value
            End Set
        End Property
        Public Property EftMonthlyPaymentOption As QuickQuotePaymentOption
            Get
                Return _EftMonthlyPaymentOption
            End Get
            Set(value As QuickQuotePaymentOption)
                _EftMonthlyPaymentOption = value
            End Set
        End Property
        Public Property CreditCardMonthlyPaymentOption As QuickQuotePaymentOption
            Get
                Return _CreditCardMonthlyPaymentOption
            End Get
            Set(value As QuickQuotePaymentOption)
                _CreditCardMonthlyPaymentOption = value
            End Set
        End Property
        Public Property RenewalCreditCardMonthlyPaymentOption As QuickQuotePaymentOption
            Get
                Return _RenewalCreditCardMonthlyPaymentOption
            End Get
            Set(value As QuickQuotePaymentOption)
                _RenewalCreditCardMonthlyPaymentOption = value
            End Set
        End Property
        Public Property RenewalEftMonthlyPaymentOption As QuickQuotePaymentOption
            Get
                Return _RenewalEftMonthlyPaymentOption
            End Get
            Set(value As QuickQuotePaymentOption)
                _RenewalEftMonthlyPaymentOption = value
            End Set
        End Property
        Public Property AnnualMtgPaymentOption As QuickQuotePaymentOption
            Get
                Return _AnnualMtgPaymentOption
            End Get
            Set(value As QuickQuotePaymentOption)
                _AnnualMtgPaymentOption = value
            End Set
        End Property
        Public Property LogoSavePath As String
            Get
                Return _LogoSavePath
            End Get
            Set(value As String)
                _LogoSavePath = value
            End Set
        End Property
        Public Property LogoWebPath As String
            Get
                Return _LogoWebPath
            End Get
            Set(value As String)
                _LogoWebPath = value
            End Set
        End Property

        'added 3/22/2017
        Public Property PolicyRecords As List(Of QuickQuotePolicyLookupInfo)
            Get
                Return _PolicyRecords
            End Get
            Set(value As List(Of QuickQuotePolicyLookupInfo))
                _PolicyRecords = value
            End Set
        End Property
        Public Property ValidPolicyRecords As List(Of QuickQuotePolicyLookupInfo)
            Get
                Return _ValidPolicyRecords
            End Get
            Set(value As List(Of QuickQuotePolicyLookupInfo))
                _ValidPolicyRecords = value
            End Set
        End Property
        Public Property DiamondProposal As QuickQuoteDiamondProposal 'added 3/30/2017
            Get
                Return _DiamondProposal
            End Get
            Set(value As QuickQuoteDiamondProposal)
                _DiamondProposal = value
            End Set
        End Property

        'added 9/4/2017
        Public ReadOnly Property CombinedPaymentOptions As List(Of QuickQuotePaymentOption)
            Get
                Return _CombinedPaymentOptions
            End Get
        End Property

        Public Sub New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _QuoteIds = New Generic.List(Of String)
            _PolicyRecords = New List(Of QuickQuotePolicyLookupInfo) 'added 3/22/2017
            _DiamondProposal = Nothing 'added 3/30/2017
            ResetResultValues()
        End Sub
        Private Sub ResetResultValues()
            _ValidQuoteIds = New Generic.List(Of String)
            _QuickQuoteObjects = New Generic.List(Of QuickQuoteObject)
            _ErrorMessages = New Generic.List(Of String)
            _CombinedPremium = ""
            '_ClientId = ""
            '_ClientName = New QuickQuoteName
            _Client = New QuickQuoteClient '12/24/2014 note: may neet to set NameAddressSourceId on Name and Name2 like is done whenever the QuickQuoteObject is instantiated
            _HasMultipleClients = False
            _Agency = New QuickQuoteAgency
            _HasMultipleAgencies = False
            _AnnualPaymentOption = New QuickQuotePaymentOption
            _SemiAnnualPaymentOption = New QuickQuotePaymentOption
            _QuarterlyPaymentOption = New QuickQuotePaymentOption
            _MonthlyPaymentOption = New QuickQuotePaymentOption
            _EftMonthlyPaymentOption = New QuickQuotePaymentOption
            _CreditCardMonthlyPaymentOption = New QuickQuotePaymentOption
            _RenewalCreditCardMonthlyPaymentOption = New QuickQuotePaymentOption
            _RenewalEftMonthlyPaymentOption = New QuickQuotePaymentOption
            _AnnualMtgPaymentOption = New QuickQuotePaymentOption
            _LogoSavePath = ""
            _LogoWebPath = ""

            'added 3/22/2017
            _ValidPolicyRecords = New List(Of QuickQuotePolicyLookupInfo)

            'added 9/4/2017
            _CombinedPaymentOptions = Nothing
        End Sub
        Public Sub LoadQuickQuoteObjects()
            ResetResultValues()
            If _QuoteIds IsNot Nothing AndAlso _QuoteIds.Count > 0 Then
                For Each qId As String In _QuoteIds
                    Dim errorMsg As String = ""
                    'Dim quickQuote As QuickQuoteObject = Nothing
                    '3/22/2017 - re-named variable to something more unique (since quickQuote is same name as Namespace)
                    Dim qqo As QuickQuoteObject = Nothing
                    Dim rateType As QuickQuoteXML.QuickQuoteSaveType = Nothing
                    Dim okayToInclude As Boolean = False
                    If qId <> "" AndAlso IsNumeric(qId) = True Then
                        QQxml.GetRatedQuote(qId, qqo, rateType, errorMsg)
                        If qqo IsNot Nothing Then
                            If rateType = QuickQuoteXML.QuickQuoteSaveType.AppGap Then
                                'rated app gap
                            Else
                                'rated quote
                            End If
                            If qqo.QuoteNumber <> "" Then
                                'has quote #
                            End If
                            If qqo.LobType <> QuickQuoteObject.QuickQuoteLobType.None Then
                                'has valid LOB
                            End If
                            If qqo.Success = True Then
                                'successful quote
                                'If qqo.ValidationItems IsNot Nothing AndAlso qqo.ValidationItems.Count > 0 Then
                                '    For Each vi As QuickQuoteValidationItem In qqo.ValidationItems
                                '        'just messages
                                '    Next
                                'End If
                                okayToInclude = True
                            Else
                                'If qqo.ValidationItems IsNot Nothing AndAlso qqo.ValidationItems.Count > 0 Then
                                '    'not successful; has messages
                                '    For Each vi As QuickQuoteValidationItem In qqo.ValidationItems
                                '        'validation errors/messages
                                '    Next
                                'Else
                                '    'The quote failed, but no validation items were found
                                'End If
                                errorMsg = "The quote tied to quoteId " & qId & " was not successfully rated."
                            End If
                        Else
                            If errorMsg <> "" Then
                                'has errorMsg
                                errorMsg = "Error for quoteId " & qId & ":  " & errorMsg
                            Else
                                errorMsg = "There was a problem locating quoteId " & qId & " in the database."
                            End If
                        End If
                    Else
                        errorMsg = "Invalid format for quoteId " & qId & "."
                    End If
                    If okayToInclude = True Then
                        _ValidQuoteIds.Add(qId)
                        _QuickQuoteObjects.Add(qqo)
                        'updated 5/6/2013 to use different property for WCP premium; payment options already reflects correct premium
                        If qqo.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation Then
                            'Updated 6/24/2019 for Bug 32394 MLW
                            'CombinedPremium = qqHelper.getSum(_CombinedPremium, qqo.Dec_WC_TotalPremiumDue)
                            Dim subQuotes As List(Of QuickQuoteObject) = Nothing
                            subQuotes = qqHelper.MultiStateQuickQuoteObjects(qqo)
                            CombinedPremium = qqHelper.getSum(_CombinedPremium, qqHelper.GetSumForPropertyValues(subQuotes, Function() qqo.Dec_WC_TotalPremiumDue, maintainFormattingOrDefaultValue:=True))
                        Else
                            CombinedPremium = qqHelper.getSum(_CombinedPremium, qqo.TotalQuotedPremium)
                        End If
                        'added 5/1/2013
                        'If _ClientId = "" Then
                        '    _ClientId = qqo.Client.ClientId
                        '    _ClientName = qqo.Client.Name
                        'Else
                        '    'already set once
                        '    If _ClientId <> qqo.Client.ClientId Then
                        '        _HasMultipleClients = True
                        '        _ClientId = qqo.Client.ClientId
                        '        _ClientName = qqo.Client.Name
                        '    End If
                        'End If
                        If _Client.ClientId = "" Then
                            _Client = qqo.Client
                            MovePolicyHolderDataToClient(_Client, qqo)
                        Else
                            If _Client.ClientId <> qqo.Client.ClientId Then
                                _HasMultipleClients = True
                                _Client = qqo.Client
                                MovePolicyHolderDataToClient(_Client, qqo)
                            End If
                        End If
                        If _Agency.AgencyId = "" Then
                            _Agency = qqo.Agency
                            SetLogoPaths()
                        Else
                            If _Agency.AgencyId <> qqo.Agency.AgencyId Then
                                _HasMultipleAgencies = True
                                _Agency = qqo.Agency
                                SetLogoPaths()
                            End If
                        End If
                        SetPaymentOptions(qqo.PaymentOptions)
                    Else
                        If errorMsg = "" Then
                            errorMsg = "The quote tied to quoteId " & qId & " was not eligible for inclusion."
                        End If
                    End If
                    If errorMsg <> "" Then
                        _ErrorMessages.Add(errorMsg)
                    End If
                Next
                'added 5/1/2013
                If _HasMultipleClients = True Then
                    _ErrorMessages.Add("Multiple clients have been included.")
                End If
                If _HasMultipleAgencies = True Then
                    _ErrorMessages.Add("Multiple agencies have been included.")
                End If
            ElseIf _PolicyRecords IsNot Nothing AndAlso _PolicyRecords.Count > 0 Then 'added 3/22/2017; should probably go back and clean up some duplicate logic so some is re-usable
                Dim policyRecordCounter As Integer = 0
                For Each pr As QuickQuotePolicyLookupInfo In _PolicyRecords
                    policyRecordCounter += 1
                    Dim errorMsg As String = ""
                    Dim qqo As QuickQuoteObject = Nothing
                    Dim okayToInclude As Boolean = False
                    If pr.HasAnyDistinguishableInfo = True Then
                        qqo = QQxml.ReadOnlyQuickQuoteObjectForPolicyLookupInfo(pr, errorMsg)
                        If qqo IsNot Nothing Then
                            If qqo.QuoteNumber <> "" Then
                                'has quote #
                            End If
                            If qqo.LobType <> QuickQuoteObject.QuickQuoteLobType.None Then
                                'has valid LOB
                            End If
                            If qqHelper.IsPositiveDecimalString(qqo.TotalQuotedPremium) = True Then
                                'successful quote
                                okayToInclude = True
                            Else
                                errorMsg = "The quote tied to policy record " & policyRecordCounter.ToString & " was not successfully rated."
                            End If
                        Else
                            If errorMsg <> "" Then
                                'has errorMsg
                                errorMsg = "Error for policy record " & policyRecordCounter.ToString & ":  " & errorMsg
                            Else
                                errorMsg = "There was a problem locating policy record " & policyRecordCounter.ToString & " in Diamond."
                            End If
                        End If
                    Else
                        errorMsg = "Invalid lookup info for policy record " & policyRecordCounter.ToString & "."
                    End If
                    If okayToInclude = True Then
                        _ValidPolicyRecords.Add(qqHelper.CloneObject(pr))
                        _QuickQuoteObjects.Add(qqo)
                        'updated 5/6/2013 to use different property for WCP premium; payment options already reflects correct premium
                        If qqo.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation Then
                            'Updated 6/24/2019 for Bug 32394 MLW
                            'CombinedPremium = qqHelper.getSum(_CombinedPremium, qqo.Dec_WC_TotalPremiumDue)
                            Dim subQuotes As List(Of QuickQuoteObject) = Nothing
                            subQuotes = qqHelper.MultiStateQuickQuoteObjects(qqo)
                            CombinedPremium = qqHelper.getSum(_CombinedPremium, qqHelper.GetSumForPropertyValues(subQuotes, Function() qqo.Dec_WC_TotalPremiumDue, maintainFormattingOrDefaultValue:=True))

                        Else
                            CombinedPremium = qqHelper.getSum(_CombinedPremium, qqo.TotalQuotedPremium)
                        End If
                        'added 5/1/2013
                        'If _ClientId = "" Then
                        '    _ClientId = qqo.Client.ClientId
                        '    _ClientName = qqo.Client.Name
                        'Else
                        '    'already set once
                        '    If _ClientId <> qqo.Client.ClientId Then
                        '        _HasMultipleClients = True
                        '        _ClientId = qqo.Client.ClientId
                        '        _ClientName = qqo.Client.Name
                        '    End If
                        'End If
                        If _Client.ClientId = "" Then
                            _Client = qqo.Client
                            MovePolicyHolderDataToClient(_Client, qqo)
                        Else
                            If _Client.ClientId <> qqo.Client.ClientId Then
                                _HasMultipleClients = True
                                _Client = qqo.Client
                                MovePolicyHolderDataToClient(_Client, qqo)
                            End If
                        End If
                        If _Agency.AgencyId = "" Then
                            _Agency = qqo.Agency
                            SetLogoPaths()
                        Else
                            If _Agency.AgencyId <> qqo.Agency.AgencyId Then
                                _HasMultipleAgencies = True
                                _Agency = qqo.Agency
                                SetLogoPaths()
                            End If
                        End If
                        SetPaymentOptions(qqo.PaymentOptions)
                    Else
                        If errorMsg = "" Then
                            errorMsg = "The quote tied to policy record " & policyRecordCounter.ToString & " was not eligible for inclusion."
                        End If
                    End If
                    If errorMsg <> "" Then
                        _ErrorMessages.Add(errorMsg)
                    End If
                Next
                'added 5/1/2013
                If _HasMultipleClients = True Then
                    _ErrorMessages.Add("Multiple clients have been included.")
                End If
                If _HasMultipleAgencies = True Then
                    _ErrorMessages.Add("Multiple agencies have been included.")
                End If
            ElseIf _DiamondProposal IsNot Nothing AndAlso _DiamondProposal.Images IsNot Nothing AndAlso _DiamondProposal.Images.Count > 0 Then 'added 3/30/2017; should probably go back and clean up some duplicate logic so some is re-usable... now moreso than before
                Dim policyRecordCounter As Integer = 0
                'For Each pr As QuickQuotePolicyLookupInfo In _PolicyRecords
                For Each img As QuickQuoteDiamondProposalImage In _DiamondProposal.Images
                    policyRecordCounter += 1
                    Dim errorMsg As String = ""
                    Dim qqo As QuickQuoteObject = Nothing
                    Dim okayToInclude As Boolean = False
                    Dim lookupErrorMsg As String = ""
                    Dim pr As QuickQuotePolicyLookupInfo = Nothing
                    If img IsNot Nothing Then
                        pr = QuickQuoteHelperClass.PolicyLookupInfoForPolicyInfo(policyId:=img.PolicyId, policyImageNum:=img.PolicyImageNum, errorMessage:=lookupErrorMsg)
                    End If
                    If pr IsNot Nothing AndAlso pr.HasAnyDistinguishableInfo = True Then
                        qqo = QQxml.ReadOnlyQuickQuoteObjectForPolicyLookupInfo(pr, errorMsg)
                        If qqo IsNot Nothing Then
                            If qqo.QuoteNumber <> "" Then
                                'has quote #
                            End If
                            If qqo.LobType <> QuickQuoteObject.QuickQuoteLobType.None Then
                                'has valid LOB
                            End If
                            If qqHelper.IsPositiveDecimalString(qqo.TotalQuotedPremium) = True Then
                                'successful quote
                                okayToInclude = True
                            Else
                                errorMsg = "The quote tied to policy record " & policyRecordCounter.ToString & " was not successfully rated."
                            End If
                        Else
                            If errorMsg <> "" Then
                                'has errorMsg
                                errorMsg = "Error for policy record " & policyRecordCounter.ToString & ":  " & errorMsg
                            Else
                                errorMsg = "There was a problem locating policy record " & policyRecordCounter.ToString & " in Diamond."
                            End If
                        End If
                    Else
                        errorMsg = "Invalid lookup info for policy record " & policyRecordCounter.ToString & "."
                    End If
                    If okayToInclude = True Then
                        If qqo IsNot Nothing AndAlso img IsNot Nothing AndAlso img.Comments IsNot Nothing AndAlso img.Comments.Count > 0 Then
                            For Each c As QuickQuoteDiamondProposalComment In img.Comments
                                If String.IsNullOrWhiteSpace(c.CommentText) = False Then
                                    If qqo.Comments Is Nothing Then
                                        qqo.Comments = New List(Of QuickQuoteComment)
                                    End If
                                    Dim qqC As New QuickQuoteComment
                                    With qqC
                                        .CommentText = c.CommentText
                                        .LobId = c.LobId
                                    End With
                                    qqo.Comments.Add(qqC)
                                End If
                            Next
                        End If

                        _ValidPolicyRecords.Add(qqHelper.CloneObject(pr))
                        _QuickQuoteObjects.Add(qqo)
                        'updated 5/6/2013 to use different property for WCP premium; payment options already reflects correct premium
                        If qqo.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation Then
                            'Updated 6/24/2019 for Bug 32394 MLW
                            'CombinedPremium = qqHelper.getSum(_CombinedPremium, qqo.Dec_WC_TotalPremiumDue)
                            Dim subQuotes As List(Of QuickQuoteObject) = Nothing
                            subQuotes = qqHelper.MultiStateQuickQuoteObjects(qqo)
                            CombinedPremium = qqHelper.getSum(_CombinedPremium, qqHelper.GetSumForPropertyValues(subQuotes, Function() qqo.Dec_WC_TotalPremiumDue, maintainFormattingOrDefaultValue:=True))

                        Else
                            CombinedPremium = qqHelper.getSum(_CombinedPremium, qqo.TotalQuotedPremium)
                        End If
                        'added 5/1/2013
                        'If _ClientId = "" Then
                        '    _ClientId = qqo.Client.ClientId
                        '    _ClientName = qqo.Client.Name
                        'Else
                        '    'already set once
                        '    If _ClientId <> qqo.Client.ClientId Then
                        '        _HasMultipleClients = True
                        '        _ClientId = qqo.Client.ClientId
                        '        _ClientName = qqo.Client.Name
                        '    End If
                        'End If
                        If _Client.ClientId = "" Then
                            _Client = qqo.Client
                            MovePolicyHolderDataToClient(_Client, qqo)
                        Else
                            If _Client.ClientId <> qqo.Client.ClientId Then
                                _HasMultipleClients = True
                                _Client = qqo.Client
                                MovePolicyHolderDataToClient(_Client, qqo)
                            End If
                        End If
                        If _Agency.AgencyId = "" Then
                            _Agency = qqo.Agency
                            SetLogoPaths()
                        Else
                            If _Agency.AgencyId <> qqo.Agency.AgencyId Then
                                _HasMultipleAgencies = True
                                _Agency = qqo.Agency
                                SetLogoPaths()
                            End If
                        End If
                        SetPaymentOptions(qqo.PaymentOptions)
                    Else
                        If errorMsg = "" Then
                            errorMsg = "The quote tied to policy record " & policyRecordCounter.ToString & " was not eligible for inclusion."
                        End If
                    End If
                    If errorMsg <> "" Then
                        _ErrorMessages.Add(errorMsg)
                    End If
                Next
                'added 5/1/2013
                If _HasMultipleClients = True Then
                    _ErrorMessages.Add("Multiple clients have been included.")
                End If
                If _HasMultipleAgencies = True Then
                    _ErrorMessages.Add("Multiple agencies have been included.")
                End If
            End If
        End Sub

        ''' <summary>
        ''' Added 9/16/2020  BUG 52121
        ''' BOP was showing the wrong address, should have been policyholder but it was client.  This should fix it.  MGB
        ''' </summary>
        ''' <param name="ClientObject"></param>
        ''' <param name="qqo"></param>
        Private Sub MovePolicyHolderDataToClient(ByRef ClientObject As QuickQuote.CommonObjects.QuickQuoteClient, ByVal qqo As QuickQuoteObject)
            If qqo.Policyholder IsNot Nothing Then
                With qqo.Policyholder
                    ClientObject.Address = .Address
                    ClientObject.Name = .Name
                    ClientObject.Emails = .Emails
                    ClientObject.Phones = .Phones
                End With
            End If

            Exit Sub
        End Sub

        Private Sub SetPaymentOptions(ByVal pmtOptions As Generic.List(Of QuickQuotePaymentOption))
            If pmtOptions IsNot Nothing AndAlso pmtOptions.Count > 0 Then
                '9/4/2017 added logic to combine entire list via helper method
                qqHelper.CombinePaymentOptions(_CombinedPaymentOptions, pmtOptions)

                For Each po As QuickQuotePaymentOption In pmtOptions
                    Select Case UCase(po.Description)
                        Case "ANNUAL 2" '9/4/2017 note: still valid
                            SetPaymentOption(_AnnualPaymentOption, po)
                        Case "SEMI ANNUAL 2" '9/4/2017 note: still valid
                            SetPaymentOption(_SemiAnnualPaymentOption, po)
                        Case "QUARTERLY 2" '9/4/2017 note: still valid
                            SetPaymentOption(_QuarterlyPaymentOption, po)
                        Case "MONTHLY 2" '9/4/2017 note: still valid
                            SetPaymentOption(_MonthlyPaymentOption, po)
                        Case "EFT MONTHLY 2"
                            SetPaymentOption(_EftMonthlyPaymentOption, po)
                        Case "CREDIT CARD MONTHLY 2"
                            SetPaymentOption(_CreditCardMonthlyPaymentOption, po)
                        Case "RENEWAL CREDIT CARD MONTHLY 2" '9/4/2017 note: still valid
                            SetPaymentOption(_RenewalCreditCardMonthlyPaymentOption, po)
                        Case "RENEWAL EFT MONTHLY 2" '9/4/2017 note: still valid
                            SetPaymentOption(_RenewalEftMonthlyPaymentOption, po)
                        Case "ANNUAL MTG" '9/4/2017 note: still valid
                            SetPaymentOption(_AnnualMtgPaymentOption, po)
                    End Select
                Next
            End If
        End Sub
        Private Sub SetPaymentOption(ByRef existingPmtOption As QuickQuotePaymentOption, ByVal newPmtOption As QuickQuotePaymentOption)
            If existingPmtOption IsNot Nothing AndAlso newPmtOption IsNot Nothing Then
                If existingPmtOption.Description = "" Then
                    existingPmtOption = newPmtOption
                Else
                    With existingPmtOption
                        .DepositAmount = qqHelper.getSum(.DepositAmount, newPmtOption.DepositAmount)
                        .InstallmentAmount = qqHelper.getSum(.InstallmentAmount, newPmtOption.InstallmentAmount)
                        .InstallmentFee = qqHelper.getSum(.InstallmentFee, newPmtOption.InstallmentFee)
                        'added 9/4/2017
                        .TotalInstallmentAmount = qqHelper.getSum(.DepositAmount, newPmtOption.TotalInstallmentAmount)
                        .TotalPremiumFeeServiceChargeAmount = qqHelper.getSum(.TotalPremiumFeeServiceChargeAmount, newPmtOption.TotalPremiumFeeServiceChargeAmount) 'note: appears to be estimate based on DownPaymentAmount and TotalInstallmentAmount, but doesn't match SUM of actual installments
                    End With
                End If
            End If
        End Sub
        Private Sub SetLogoPaths() 'added 5/8/2013
            _LogoSavePath = ""
            _LogoWebPath = ""

            If _Agency IsNot Nothing AndAlso _Agency.Code <> "" AndAlso Len(_Agency.Code) > 3 AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_LogoWebPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_LogoWebPath").ToString <> "" AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_LogoSavePath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_LogoSavePath").ToString <> "" AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_LogoExtension") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_LogoExtension").ToString <> "" Then
                Dim logoFileName As String = "Logo_" & Right(_Agency.Code, 4) & ConfigurationManager.AppSettings("QuickQuote_Proposal_LogoExtension").ToString
                _LogoSavePath = ConfigurationManager.AppSettings("QuickQuote_Proposal_LogoSavePath").ToString & logoFileName
                _LogoWebPath = ConfigurationManager.AppSettings("QuickQuote_Proposal_LogoWebPath").ToString & logoFileName
            End If
        End Sub
        Public Function LogoExists() As Boolean
            Dim _logoExists As Boolean = False

            If _LogoSavePath <> "" AndAlso System.IO.File.Exists(_LogoSavePath) = True Then
                _logoExists = True
            End If

            Return _logoExists
        End Function


#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _QuoteIds IsNot Nothing Then
                        If _QuoteIds.Count > 0 Then
                            For Each q As String In _QuoteIds
                                If q IsNot Nothing Then
                                    q = Nothing
                                End If
                            Next
                            _QuoteIds.Clear()
                        End If
                        _QuoteIds = Nothing
                    End If
                    If _ValidQuoteIds IsNot Nothing Then
                        If _ValidQuoteIds.Count > 0 Then
                            For Each q As String In _ValidQuoteIds
                                If q IsNot Nothing Then
                                    q = Nothing
                                End If
                            Next
                            _ValidQuoteIds.Clear()
                        End If
                        _ValidQuoteIds = Nothing
                    End If
                    If _QuickQuoteObjects IsNot Nothing Then
                        If _QuickQuoteObjects.Count > 0 Then
                            For Each q As QuickQuoteObject In _QuickQuoteObjects
                                If q IsNot Nothing Then
                                    q.Dispose()
                                    q = Nothing
                                End If
                            Next
                            _QuickQuoteObjects.Clear()
                        End If
                        _QuickQuoteObjects = Nothing
                    End If
                    If _ErrorMessages IsNot Nothing Then
                        If _ErrorMessages.Count > 0 Then
                            For Each e As String In _ErrorMessages
                                If e IsNot Nothing Then
                                    e = Nothing
                                End If
                            Next
                            _ErrorMessages.Clear()
                        End If
                        _ErrorMessages = Nothing
                    End If
                    If _CombinedPremium IsNot Nothing Then
                        _CombinedPremium = Nothing
                    End If
                    'If _ClientId IsNot Nothing Then
                    '    _ClientId = Nothing
                    'End If
                    'If _ClientName IsNot Nothing Then
                    '    _ClientName.Dispose()
                    '    _ClientName = Nothing
                    'End If
                    If _Client IsNot Nothing Then
                        _Client.Dispose()
                        _Client = Nothing
                    End If
                    If _HasMultipleClients <> Nothing Then
                        _HasMultipleClients = Nothing
                    End If
                    If _Agency IsNot Nothing Then
                        _Agency.Dispose()
                        _Agency = Nothing
                    End If
                    If _HasMultipleAgencies <> Nothing Then
                        _HasMultipleAgencies = Nothing
                    End If
                    If _AnnualPaymentOption IsNot Nothing Then
                        _AnnualPaymentOption.Dispose()
                        _AnnualPaymentOption = Nothing
                    End If
                    If _SemiAnnualPaymentOption IsNot Nothing Then
                        _SemiAnnualPaymentOption.Dispose()
                        _SemiAnnualPaymentOption = Nothing
                    End If
                    If _QuarterlyPaymentOption IsNot Nothing Then
                        _QuarterlyPaymentOption.Dispose()
                        _QuarterlyPaymentOption = Nothing
                    End If
                    If _MonthlyPaymentOption IsNot Nothing Then
                        _MonthlyPaymentOption.Dispose()
                        _MonthlyPaymentOption = Nothing
                    End If
                    If _EftMonthlyPaymentOption IsNot Nothing Then
                        _EftMonthlyPaymentOption.Dispose()
                        _EftMonthlyPaymentOption = Nothing
                    End If
                    If _CreditCardMonthlyPaymentOption IsNot Nothing Then
                        _CreditCardMonthlyPaymentOption.Dispose()
                        _CreditCardMonthlyPaymentOption = Nothing
                    End If
                    If _RenewalCreditCardMonthlyPaymentOption IsNot Nothing Then
                        _RenewalCreditCardMonthlyPaymentOption.Dispose()
                        _RenewalCreditCardMonthlyPaymentOption = Nothing
                    End If
                    If _RenewalEftMonthlyPaymentOption IsNot Nothing Then
                        _RenewalEftMonthlyPaymentOption.Dispose()
                        _RenewalEftMonthlyPaymentOption = Nothing
                    End If
                    If _AnnualMtgPaymentOption IsNot Nothing Then
                        _AnnualMtgPaymentOption.Dispose()
                        _AnnualMtgPaymentOption = Nothing
                    End If
                    If _LogoSavePath IsNot Nothing Then
                        _LogoSavePath = Nothing
                    End If
                    If _LogoWebPath IsNot Nothing Then
                        _LogoWebPath = Nothing
                    End If

                    'added 3/22/2017
                    If _PolicyRecords IsNot Nothing Then
                        If _PolicyRecords.Count > 0 Then
                            For Each pr As QuickQuotePolicyLookupInfo In _PolicyRecords
                                pr = Nothing
                            Next
                            _PolicyRecords.Clear()
                        End If
                        _PolicyRecords = Nothing
                    End If
                    If _ValidPolicyRecords IsNot Nothing Then
                        If _ValidPolicyRecords.Count > 0 Then
                            For Each pr As QuickQuotePolicyLookupInfo In _ValidPolicyRecords
                                pr = Nothing
                            Next
                            _ValidPolicyRecords.Clear()
                        End If
                        _ValidPolicyRecords = Nothing
                    End If
                    If _DiamondProposal IsNot Nothing Then 'added 3/30/2017
                        _DiamondProposal.Dispose()
                        _DiamondProposal = Nothing
                    End If

                    'added 9/4/2017
                    If _CombinedPaymentOptions IsNot Nothing Then
                        If _CombinedPaymentOptions.Count > 0 Then
                            For Each po As QuickQuotePaymentOption In _CombinedPaymentOptions
                                If po IsNot Nothing Then
                                    po.Dispose()
                                    po = Nothing
                                End If
                            Next
                            _CombinedPaymentOptions.Clear()
                        End If
                        _CombinedPaymentOptions = Nothing
                    End If
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
