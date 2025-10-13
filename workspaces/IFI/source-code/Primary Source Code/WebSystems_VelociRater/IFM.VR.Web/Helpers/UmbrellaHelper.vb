Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.PolicyLoader.QuickQuote
Imports IFM.PolicyLoader
Imports QuickQuote.CommonObjects.Umbrella
Imports IFM.PrimativeExtensions
Imports IFM.VR.Web.ENUMHelper

Namespace Helpers

    Public Class UmbrellaCalcFunctions
        Public Property CalcList As List(Of Calc)
        Public Property InputString As String
        Public Property Pattern As String
        Public Property IncreasedLimitFactor As String = "0"
        Private Property regEx As Regex


        Public Const CalcRegEx = "(?<stub>[(a-zA-Z)+\d]+):\s(?<name>[(a-zA-Z0-9\$\,\s\/)]+)\s(?<premium>[\d]+)\s"
        Public Const IncLimitFactorRegEx = "(.+)Increased Limit Factor\s(?<IncLimitFactor>[\d.]+)\s"

        ' *************************************************************
        ' Test Code:
        Dim TestCalcText As String = "Personal Liability Premium
								PERSLIAB2: Primary Res 40 +
								PERSLIAB2: Incidental Farming 5 +
								PERSLIAB2: Farmers Personal Liability 10 +
								PERSLIAB2: Primary Res 40 +
								PERSLIAB2: Incidental Farming 5 +
								PERSLIAB2: Farmers Personal Liability 10 +
								PERSLIAB2: Primary Res 40 +
								PERSLIAB2: Incidental Farming 5 +
								PERSLIAB2: Farmers Personal Liability 10 +
								PERSLIAB2: Primary Res 40 +
								PERSLIAB2: Incidental Farming 5 +
								PERSLIAB2: Farmers Personal Liability 10 +
								PERSLIAB2: Primary Res 40 +
								PERSLIAB2: Incidental Farming 5 +
								PERSLIAB2: Farmers Personal Liability 10 +
								PERSLIAB2: Primary Res 40 +
								PERSLIAB2: Primary Res 40 +
								PERSLIAB2: Primary Res 40 +
								PERSLIAB2: Primary Res 40 +
								PERSLIAB2: Primary Res 40 +
								PERSLIAB2: Secondary Res 10 +
								PERSLIAB2: Farmers Personal Liability 10 +
								PERSLIAB2: Secondary Res 10 +
								PERSLIAB2: Farmers Personal Liability 10 +
								PERSLIAB2: Secondary Res 10 +
								PERSLIAB2: Farmers Personal Liability 10 +
								PERSLIAB2: Secondary Res 10 +
								PERSLIAB2: Farmers Personal Liability 10 +
								PERSLIAB2: Secondary Res 10 +
								PERSLIAB2: Farmers Personal Liability 10 +
								PERSLIAB2: Secondary Res 10 +
								PERSLIAB2: Farmers Personal Liability 10 +
								PERSLIAB2: Secondary Res 10 +
								PERSLIAB2: Farmers Personal Liability 10 +
								PERSLIAB2: Secondary Res 10 +
								PERSLIAB2: Farmers Personal Liability 10 +
								PERSLIAB2: Secondary Res 10 +
								PERSLIAB2: Farmers Personal Liability 10 +
								PERSLIAB2: Secondary Res 10 +
								PERSLIAB2: Farmers Personal Liability 10 +
								PERSLIAB2: Secondary Res 10 +
								PERSLIAB2: Farmers Personal Liability 10 = 695
								Auto Liability Premium
								AUTOLIAB1: Initial Auto 50 +
								AUTOLIAB1: Youthful 16 To 20 Premium 50 +
								AUTOLIAB1: Youthful 21 To 24 Premium 0 +
								AUTOLIAB1: Additional Auto 45 +
								AUTOLIAB1: Additional Auto 45 +
								AUTOLIAB1: Additional Auto 45 +
								AUTOLIAB1: Additional Auto 45 +
								AUTOLIAB1: Additional Auto 45 +
								AUTOLIAB1: Additional Auto 45 +
								AUTOLIAB1: Additional Auto 45 = 415
								Watercraft Liability Premium
								WCLiab4: JetSki 15 +
								WCLiab4: JetSki 15 +
								WCLiab4: JetSki 15 +
								WCLiab4: JetSki 15 +
								WCLiab4: JetSki 15 +
								WCLiab4: JetSki 15 +
								WCLiab4: JetSki 15 +
								WCLiab4: JetSki 15 +
								WCLiab4: JetSki 15 +
								WCLiab4: JetSki 15 +
								WCLiab4: JetSki 15 +
								WCLiab4: JetSki 15 +
								WCLiab4: PowerBoat 50 = 230
								Miscellaneous Liability Premium
								ProfLiab4: Teacher Including Corporal Punishment 35 +
								ProfLiab4: Teacher Including Corporal Punishment 35 +
								ProfLiab4: Teacher Excluding Corporal Punishment 10 +
								InvPLiab4: Incidental Business Exposures Offices 10 +
								InvPLiab4: Incidental Business Exposures Offices 10 +
								InvPLiab4: Incidental Business Exposures Offices 10 +
								InvPLiab4: Incidental Business Exposures Offices 10 +
								InvPLiab4: Incidental Business Exposures Offices 10 +
								InvPLiab4: Incidental Business Exposures Rental Dwelling 20 +
								InvPLiab4: Incidental Business Exposures Rental Dwelling 20 +
								InvPLiab4: Incidental Business Exposures Rental Dwelling 20 +
								InvPLiab4: Incidental Business Exposures Rental Dwelling 20 +
								InvPLiab4: Incidental Business Exposures Rental Dwelling 20 +
								MiscLiab4: Swimming Pool 40 +
								MiscLiab4: Swimming Pool 40 +
								MiscLiab4: Trampoline 10 +
								MiscLiab4: Trampoline 10 +
								MiscLiab4: Trampoline 10 = 340
								Recreational Vehicle Liability Premium*
								RvHomeLiab4: Initial RV 25 +
								RvHomeLiab4: Additional RV 20 +
								RvHomeLiab4: Additional RV 20 +
								RvHomeLiab4: Additional RV 20 +
								RvHomeLiab4: Additional RV 20 +
								RvHomeLiab4: Additional RV 20 +
								RvHomeLiab4: Additional RV 20 = 145
								Farm Liability Premium = 0
								Total Liability Premium 1825 * Increased Limit Factor 4.2 * Employee Discount 1 = Premium 7665 (Round)
								* If a motorcycle is attached to the auto policy, then initial RV should be calculated with the auto premium.
								Manual Premium for Umbrella Limit Over 5 Million = 0"

        Sub New(inputString As String)
            If String.IsNullOrWhiteSpace(inputString) = False Then
                Me.InputString = inputString
                Me.Pattern = CalcRegEx
                ProcessCalc()
            End If
        End Sub

        Sub ProcessCalc()
            'Determine Increased Limit Factor
            GetLimitFactor()
            'Process Calc Info
            Me.CalcList = New List(Of Calc)
            If String.IsNullOrWhiteSpace(InputString) = False Then
                Me.regEx = New Regex(Pattern)
                Dim collection = regEx.Matches(Me.InputString)
                If collection.Count > 0 Then
                    Dim lineNumber As Integer = 0
                    For Each m As Match In collection
                        lineNumber += 1
                        Dim premium = ProcessPremiumWithIncreasedRateFactor(m.Groups("premium").Value)
                        Me.CalcList.Add(New Calc(lineNumber, m.Groups("stub").Value, m.Groups("name").Value, premium))
                    Next
                End If
            End If
        End Sub

        Function ProcessPremiumWithIncreasedRateFactor(basePremium As String) As String
            If String.IsNullOrWhiteSpace(IncreasedLimitFactor) = False AndAlso String.IsNullOrWhiteSpace(basePremium) = False Then
                Return (basePremium.TryToGetDouble * IncreasedLimitFactor.TryToGetDouble).TryToFormatAsCurreny(True)
            End If
            Return basePremium

        End Function

        Function GetStubGroup(stub As String) As List(Of Calc)
            If String.IsNullOrWhiteSpace(stub) = False AndAlso CalcList.IsLoaded Then
                Return Me.CalcList?.Where(Function(x) x.Stub.ToUpper = stub.ToUpper).ToList()
            End If
            Return New List(Of Calc)
        End Function

        Sub GetLimitFactor()
            If String.IsNullOrWhiteSpace(InputString) = False Then
                Dim ILFRegEx = New Regex(IncLimitFactorRegEx)
                Dim match = ILFRegEx.Match(Me.InputString)
                IncreasedLimitFactor = match.Groups("IncLimitFactor").Value
            End If
        End Sub

    End Class

    Public Class Calc
        Property Stub As String
        Property Name As String
        Property Premium As String
        Property Index As Integer
        Sub New(index As Integer, stub As String, name As String, premium As String)
            Me.Index = index
            Me.Stub = stub
            Me.Name = name
            Me.Premium = premium
        End Sub

    End Class

    Public Class UmbrellaUnderlyingValidation
        Property Quote As QuickQuoteObject
        Property GoverningStateQuote As QuickQuoteObject
        Property LoaderValidationErrors() As Dictionary(Of String, String)
        Property DevDictionaryHelper As DevDictionaryHelper.DevDictionaryHelper
        Property ValidationHelper As ControlValidationHelper

        Public ReadOnly UmbrellaDictionaryName As String = "UmbrellaUnderlyingDetails"
        Public Const LIABILITY_OPTION_ID_NO_LIABILITY_COVERAGE = "6"
        Public Const LIABILITY_OPTION_ID_NOT_SPECIFIED = "0"


        Public Sub ValidateAndAddUnderlyingPolicies(ByRef _GoverningStateQuote As QuickQuoteObject, ByRef _LoaderValidationErrors As Dictionary(Of String, String), ByRef _DevDictionaryHelper As DevDictionaryHelper.DevDictionaryHelper)
            Me.GoverningStateQuote = _GoverningStateQuote
            Me.LoaderValidationErrors = _LoaderValidationErrors
            Me.DevDictionaryHelper = _DevDictionaryHelper
            TryAddPolicyToUnderlyingPolicies()
        End Sub

        Public Sub ValidateForSaveRateUnderlyingPolicies(ByRef quote As QuickQuoteObject, ByRef _ValidationHelper As ControlValidationHelper)
            Me.Quote = quote



            Dim qqhelper = New QuickQuoteHelperClass()
            Me.GoverningStateQuote = qqhelper.GoverningStateQuote(quote)

            Me.ValidationHelper = _ValidationHelper

            ValidateUnderlyingPoliciesAtSaveRate()
        End Sub

        Function PolicyValidator(qqObj As QuickQuoteObject) As (IsValid As Boolean, Message As List(Of String))
            Dim retval = (IsValid:=True, Message:=New List(Of String))

            If qqObj Is Nothing Then
                retval.IsValid = False
                retval.Message.Add("No matching active Policy or Quote number found for {0}. Please reenter.")
            Else
                Dim qqhelper = New QuickQuoteHelperClass()
                Dim currentPolicyStatus As QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus = qqObj.PolicyCurrentStatus()
                ' Messages that start with "*" will be treated as warnings.

                'verify that policy has liablity coverage 
                Dim farmLiability_N_A_or_ProperlySet As Boolean = True

                If qqObj.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                    Dim liabilityOptionId As String = If(qqObj.MultiStateQuotes?.FirstOrDefault()?.LiabilityOptionId?.Trim("0"), qqObj.LiabilityOptionId.Trim("0"))

                    farmLiability_N_A_or_ProperlySet = (liabilityOptionId <> LIABILITY_OPTION_ID_NO_LIABILITY_COVERAGE AndAlso
                                                        liabilityOptionId <> LIABILITY_OPTION_ID_NOT_SPECIFIED)
                End If

                If Not farmLiability_N_A_or_ProperlySet Then
                    retval.Message.Add("Underlying policy does not have liability coverage, so it cannot be added to the umbrella policy.")
                    retval.IsValid = False
                Else
                    Select Case currentPolicyStatus

                        Case QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.Cancelled, QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.NonRenewed
                            retval.Message.Add("Policy status must be In-Force or Pending")
                            retval.IsValid = False
                        Case QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.InForce, QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.Future
                            Dim policyLookupInfo As New QuickQuotePolicyLookupInfo
                            With policyLookupInfo
                                .PolicyId = qqhelper.IntegerForString(qqObj.PolicyId)
                                .PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByImage
                            End With
                            Dim hasPending As Boolean = False
                            Dim hasFutureRenewalWithSameEffDate As Boolean = False
                            Dim policyResults As List(Of QuickQuotePolicyLookupInfo) = QuickQuoteHelperClass.PolicyResultsForLookupInfo(policyLookupInfo)
                            If policyResults IsNot Nothing AndAlso policyResults.Count > 0 Then
                                For Each pr As QuickQuotePolicyLookupInfo In policyResults
                                    If pr IsNot Nothing Then
                                        Select Case pr.PolicyStatusCode()
                                            Case QuickQuotePolicyLookupInfo.DiamondPolicyStatusCode.Pending
                                                hasPending = True
                                            Case QuickQuotePolicyLookupInfo.DiamondPolicyStatusCode.Future
                                                If pr.TransTypeId = 4 Then 'Renewal; see Diamond's TransType table
                                                    If qqhelper.IsValidDateString(GoverningStateQuote.EffectiveDate, mustBeGreaterThanDefaultDate:=True) = True AndAlso qqhelper.IsValidDateString(pr.EffectiveDate, mustBeGreaterThanDefaultDate:=True) = True AndAlso CDate(GoverningStateQuote.EffectiveDate) = CDate(pr.EffectiveDate) Then
                                                        hasFutureRenewalWithSameEffDate = True
                                                    End If
                                                End If
                                        End Select
                                        If hasPending = True AndAlso hasFutureRenewalWithSameEffDate = True Then
                                            Exit For
                                        End If
                                    End If
                                Next
                                Dim messageToReturn As String = ""
                                If hasPending = True OrElse hasFutureRenewalWithSameEffDate = True Then
                                    If hasPending = True Then
                                        messageToReturn = qqhelper.appendText(messageToReturn, "Policy/quote {0} has a pending image. " & If(currentPolicyStatus = QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.Future, "Issued", "In-Force") & " image is being used.  Quoted premium could change.", splitter:=" ")
                                    End If
                                    If hasFutureRenewalWithSameEffDate = True Then
                                        messageToReturn = qqhelper.appendText(messageToReturn, "Effective date is the same as the future renewal effective date for policy/quote {0}.  Final rate could change.", splitter:=" ")
                                    End If
                                    messageToReturn = "*" & messageToReturn
                                End If
                                If String.IsNullOrWhiteSpace(messageToReturn) = False Then
                                    retval.Message.Add(messageToReturn)
                                End If
                            End If
                        Case Else
                            If qqhelper.IsPositiveDecimalString(qqObj.TotalQuotedPremium) = False Then
                                retval.Message.Add("Your underlying quote(s) must be rated in order to rate the Umbrella")
                                retval.IsValid = False
                                ' Check for Policies that have updates that have not been rated. And provide a message.
                                'ElseIf currentPolicyStatus = QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.Pending _
                                '    AndAlso qqhelper.IntegerForString(qqObj.TransactionTypeId) = 2 _ '2 is New Business
                                '    AndAlso qqhelper.IntegerForString(qqObj.PolicyOriginTypeId) > 0 Then '2 is Velocirater (0 is diamond, 1 is comprater)
                                '    retval.Message.Add("*New changes to quotes that have not been re-rated will not be included in the Umbrella Policy.")
                                '    'recent changes to underlying policy may not be reflected until re-rate
                            End If


                    End Select
                End If
                'verify that policy has liablity coverage 
                'If qqObj.LiabilityOptionId = LIABILITY_OPTION_ID_NO_LIABILITY_COVERAGE Then
                '    retval.Message.Add("Underlying policy does not have liability coverage, so it cannot be added to the umbrella policy.")
                '    retval.IsValid = False
                'End If
            End If

            Return retval
        End Function

        Function TryAddPolicyToUnderlyingPolicies() As Boolean
            Dim QQxml As New QuickQuoteXML()
            Dim policyLoader As New QuickQuoteUnderlyingPolicyLoaderService(QQxml, New UnderlyingPolicyPostOpHandlerProvider())
            Dim policyList = DevDictionaryHelper.GetUmbrellaDictionaryByLobType()

            LoaderValidationErrors.Clear()

            For Each quoteNum In policyList?.Keys
                Dim request = New LoadPolicyRequest(Of QuickQuoteObject)(quoteNum, GoverningStateQuote)
                Dim result = policyLoader.LoadPolicy(request, AddressOf PolicyValidator)


                '***Future changes to track existing Policies and not create new records.
                'If DoesUmbPolicyExistByPolicyNumber(quoteNum) Then
                '    request = New UpdatePolicyRequest(quoteNum)
                '    result = policyLoader.UpdatePolicy(request, AddressOf PolicyValidator)
                'Else
                '    request = New LoadPolicyRequest(quoteNum)
                '    result = policyLoader.LoadPolicy(request, AddressOf PolicyValidator)

                'End If

                'Dim valHelper As ControlValidationHelper = Nothing
                'If Me.Parent IsNot Nothing AndAlso Me.Parent.Parent IsNot Nothing AndAlso TypeOf Me.Parent.Parent Is ctl_FUPPUP_UnderlyingPolicies Then
                '    valHelper = DirectCast(Me.Parent.Parent, ctl_FUPPUP_UnderlyingPolicies).ValidationHelper
                'Else
                '    valHelper = Me.ValidationHelper
                'End If

                If result.Success Then
                    DevDictionaryHelper.AddToMasterValueDictionary(quoteNum, CType(result.Previous?.Data, QuickQuoteObject)?.Policyholder.Name.DisplayNameForWeb)
                    AddResultsToUnderlyingPolicies(result)


                Else
                    If result?.Messages.IsLoaded = False Then
                        result.AddMessage("Quote or Policy {0} did not load correctly.")
                    End If
                End If
                LoaderValidationErrors.Add(quoteNum, result.Messages.JoinAnd(vbCrLf).Replace("{0}", quoteNum))
            Next
            Return True
        End Function

        Function ValidateUnderlyingPoliciesAtSaveRate() As Boolean
            Dim QQxml As New QuickQuoteXML()
            Dim policyLoader As New QuickQuoteUnderlyingPolicyLoaderService(QQxml, New UnderlyingPolicyPostOpHandlerProvider())
            Dim DevDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, UmbrellaDictionaryName)
            Dim policyList = DevDictionaryHelper.GetMasterValueAsDictionary

            Dim OldUPList = GoverningStateQuote.UnderlyingPolicies

            GoverningStateQuote.UnderlyingPolicies = New List(Of QuickQuoteUnderlyingPolicy)

            For Each quoteNum In policyList?.Keys
                Dim request = New LoadPolicyRequest(Of QuickQuoteObject)(quoteNum, GoverningStateQuote)
                Dim result = policyLoader.LoadPolicy(request, AddressOf PolicyValidator)

                If result.Success Then
                    DevDictionaryHelper.AddToMasterValueDictionary(quoteNum, CType(result.Previous?.Data, QuickQuoteObject)?.Policyholder.Name.DisplayNameForWeb)
                    AddResultsToUnderlyingPolicies(result)

                Else

                    ValidationHelper.AddError("Policy load for " + quoteNum + " was unsuccessful when rating, please check the Coverages Page.")
                End If
            Next

            'check to see if we can remove this code because of changes elsewhere
            Dim _MainFarmPolicyOld As QuickQuoteUnderlyingPolicy = Nothing
            Dim _MainFarmPolicyNew As QuickQuoteUnderlyingPolicy = Nothing
            If OldUPList IsNot Nothing AndAlso OldUPList.Count > 0 Then
                _MainFarmPolicyOld = OldUPList.FirstOrDefault(Function(x As QuickQuoteUnderlyingPolicy) IFM.VR.Common.Helpers.LOBHelper.GetLobFromPrefix_QuoteOrPolicy(x.PrimaryPolicyNumber) = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                _MainFarmPolicyNew = GoverningStateQuote.UnderlyingPolicies.FirstOrDefault(Function(x As QuickQuoteUnderlyingPolicy) IFM.VR.Common.Helpers.LOBHelper.GetLobFromPrefix_QuoteOrPolicy(x.PrimaryPolicyNumber) = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                If _MainFarmPolicyOld?.LobId <> Nothing AndAlso _MainFarmPolicyNew?.LobId <> Nothing Then
                    Dim policyInfoNew_misc = _MainFarmPolicyNew.PolicyInfos.FirstOrDefault(Function(pi) pi.TypeId.TryToGetInt32 = PolicyTypeId.MiscellaneousLiability)
                    Dim policyInfoOld_misc = _MainFarmPolicyOld.PolicyInfos.FirstOrDefault(Function(pi) pi.TypeId.TryToGetInt32 = PolicyTypeId.MiscellaneousLiability)

                    If (policyInfoNew_misc Is Nothing OrElse
                       policyInfoNew_misc.MiscellaneousLiabilities?.Any() = False) AndAlso
                       policyInfoOld_misc IsNot Nothing Then
                        _MainFarmPolicyNew.PolicyInfos.Add(policyInfoOld_misc)
                    Else
                        'Family Farm Corp
                        Dim miscFFCorp = policyInfoOld_misc?.MiscellaneousLiabilities?.Where(Function(ml) ml.TypeId = "3")
                        If miscFFCorp?.Any() Then
                            policyInfoNew_misc.MiscellaneousLiabilities.AddRange(miscFFCorp)
                        End If

                        'swimming pool - UI added
                        If policyInfoNew_misc?.MiscellaneousLiabilities?.Any(Function(ml) ml.TypeId = "1") = False AndAlso
                           policyInfoOld_misc?.MiscellaneousLiabilities?.Any(Function(ml) ml.TypeId = "1") = True Then
                            policyInfoNew_misc.MiscellaneousLiabilities.Add(policyInfoOld_misc.MiscellaneousLiabilities.FirstOrDefault(Function(ml) ml.TypeId = "1"))
                        End If
                    End If
                    'For Each PolicyTypeInfo As PolicyInfo In _MainFarmPolicyOld.PolicyInfos
                    '    If PolicyTypeInfo.TypeId.TryToGetInt32 = PolicyTypeId.MiscellaneousLiability AndAlso PolicyTypeInfo.MiscellaneousLiabilities IsNot Nothing Then
                    '        For Each misc As MiscellaneousLiability In PolicyTypeInfo.MiscellaneousLiabilities
                    '            '  If misc.TypeId = "1" OrElse misc.TypeId = "3" Then
                    '            _MainFarmPolicyNew.PolicyInfos.Add(PolicyTypeInfo)
                    '            '    Exit For
                    '            ' End If

                    '        Next
                    '    End If
                    'Next
                End If
            End If


            Return True
        End Function

        Public Sub AddResultsToUnderlyingPolicies(result As LoadPolicyResult(Of QuickQuoteUnderlyingPolicy))
            GoverningStateQuote.UnderlyingPolicies.Add(result.Data)
            Dim NextUP = result.Next
            While NextUP IsNot Nothing
                GoverningStateQuote.UnderlyingPolicies.Add(NextUP.Data)
                NextUP = NextUP.Next
            End While
        End Sub
    End Class
End Namespace