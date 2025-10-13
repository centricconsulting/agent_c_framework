Imports System.Collections.Generic
Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuotePolicyLookupInfo 'added 10/30/2016

        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass 'added 11/10/2016

        Enum LookupType
            None = 0
            ByPolicy = 1
            ByImage = 2
        End Enum
        Enum NameLookupField 'added 11/5/2016
            None = 0
            DisplayName = 1
            LastName = 2
            CommercialName1 = 3
            'added 3/31/2017
            DoingBusinessAs = 4
            CommercialName2 = 5
            SortName = 6
            CommercialName1OrCommercialName2 = 7
            CommercialName1OrDoingBusinessAs = 8
            CommercialName1OrCommercialName2OrDoingBusinessAs = 9
            LastNameOrCommercialName1 = 10
            LastNameOrDoingBusinessAs = 11
            LastNameOrCommercialName1OrCommercialName2 = 12
            LastNameOrCommercialName1OrDoingBusinessAs = 13
            LastNameOrCommercialName1OrCommercialName2OrDoingBusinessAs = 14
            DisplayNameOrSortName = 15
        End Enum
        Enum NameLookupMatchType 'added 11/5/2016
            None = 0
            ExactMatch = 1
            MatchBeginning = 2
            MatchEnd = 3
            MatchMiddle = 4
        End Enum

        Public Property PolicyNumber As String = String.Empty
        Public Property QuoteNumber As String = String.Empty
        Public Property PolicyId As Integer = 0
        Public Property PolicyImageNum As Integer = 0
        Public Property AgencyCode As String = String.Empty
        Public Property AgencyId As Integer = 0
        Public Property PolicyLookupType As LookupType = LookupType.ByPolicy
        Public Property PolicyCurrentStatusId As Integer = 0
        Public Property PolicyStatusCodeId As Integer = 0
        Public Property TransTypeId As Integer = 0 'added 11/3/2016
        Public Property AgencyCodes As List(Of String) = Nothing 'added 11/4/2016
        Public Property AgencyIds As List(Of Integer) = Nothing 'added 11/4/2016

        Public Property SqlQuery As String = Nothing
        Public Property ErrorMessage As String = Nothing

        'added 11/4/2016
        Private _EffectiveDate As String = ""
        Public ReadOnly Property EffectiveDate As String
            Get
                Return _EffectiveDate
            End Get
        End Property
        Private _ExpirationDate As String = ""
        Public ReadOnly Property ExpirationDate As String
            Get
                Return _ExpirationDate
            End Get
        End Property
        Private _TransactionEffectiveDate As String = ""
        Public ReadOnly Property TransactionEffectiveDate As String
            Get
                Return _TransactionEffectiveDate
            End Get
        End Property
        Private _TransactionExpirationDate As String = ""
        Public ReadOnly Property TransactionExpirationDate As String
            Get
                Return _TransactionExpirationDate
            End Get
        End Property
        'added 11/5/2016
        Public Property VersionId As Integer = 0
        Public Property LobId As Integer = 0
        Public Property StateId As Integer = 0 'added 11/28/2022
        Public Property CompanyId As Integer = 0 'added 11/28/2022
        Private _Policyholder1Name As String = ""
        Public ReadOnly Property Policyholder1Name As String
            Get
                Return _Policyholder1Name
            End Get
        End Property
        Private _Policyholder1SortName As String = ""
        Public ReadOnly Property Policyholder1SortName As String
            Get
                Return _Policyholder1SortName
            End Get
        End Property
        Public Property ForcePolicyholder1NameReturn As Boolean
        Public Property Policyholder1NameToFind As String = ""
        Public Property Policyholder1NameLookupField As NameLookupField = NameLookupField.DisplayName
        Public Property Policyholder1NameLookupMatchType As NameLookupMatchType = NameLookupMatchType.ExactMatch

        'added 11/10/2016
        Private _FullTermPremium As String = "" 'PolicyImage.premium_fullterm
        Private _ChangeInFullTermPremium As String = "" 'PolicyImage.premium_chg_fullterm
        Private _WrittenPremium As String = "" 'PolicyImage.premium_written
        Private _ChangeInWrittenPremium As String = "" 'PolicyImage.premium_chg_written
        Public ReadOnly Property FullTermPremium As String 'PolicyImage.premium_fullterm
            Get
                Return qqHelper.QuotedPremiumFormat(_FullTermPremium)
            End Get
        End Property
        Public ReadOnly Property ChangeInFullTermPremium As String 'PolicyImage.premium_chg_fullterm
            Get
                Return qqHelper.QuotedPremiumFormat(_ChangeInFullTermPremium)
            End Get
        End Property
        Public ReadOnly Property WrittenPremium As String 'PolicyImage.premium_written
            Get
                Return qqHelper.QuotedPremiumFormat(_WrittenPremium)
            End Get
        End Property
        Public ReadOnly Property ChangeInWrittenPremium As String 'PolicyImage.premium_chg_written
            Get
                Return qqHelper.QuotedPremiumFormat(_ChangeInWrittenPremium)
            End Get
        End Property

        Public ReadOnly Property HasAnyDistinguishableInfo As Boolean
            Get
                If String.IsNullOrWhiteSpace(_PolicyNumber) = False OrElse String.IsNullOrWhiteSpace(_QuoteNumber) = False OrElse _PolicyId > 0 Then
                    Return True
                Else
                    'If _AgencyId > 0 OrElse (_AgencyIds IsNot Nothing AndAlso _AgencyIds.Count > 0) OrElse String.IsNullOrWhiteSpace(_AgencyCode) = False OrElse (_AgencyCodes IsNot Nothing AndAlso _AgencyCodes.Count > 0) Then 'added IF 11/5/2016; could also check for _AgencyCode or _AgencyCodes, but those are just text searches and wouldn't be as efficient; went ahead and updated for AgencyCode(s) 11/5/2016
                    'updated for ClientId 2/28/2017
                    If _ClientId > 0 OrElse _AgencyId > 0 OrElse (_AgencyIds IsNot Nothing AndAlso _AgencyIds.Count > 0) OrElse String.IsNullOrWhiteSpace(_AgencyCode) = False OrElse (_AgencyCodes IsNot Nothing AndAlso _AgencyCodes.Count > 0) Then 'added IF 11/5/2016; could also check for _AgencyCode or _AgencyCodes, but those are just text searches and wouldn't be as efficient; went ahead and updated for AgencyCode(s) 11/5/2016
                        'If _TransTypeId > 0 OrElse _PolicyStatusCodeId > 0 OrElse _PolicyCurrentStatusId > 0 OrElse _VersionId > 0 OrElse _LobId > 0 OrElse (String.IsNullOrWhiteSpace(_Policyholder1NameToFind) = False AndAlso Len(_Policyholder1NameToFind) >= 3) Then 'also included VersionId and LobId 11/5/2016; then Name search 11/5/2016
                        'updated for LobIds, VersionIds, PolicyCurrentStatusIds, PolicyStatusCodeIds and TransTypeIds 3/6/2017
                        'If _TransTypeId > 0 OrElse (_TransTypeIds IsNot Nothing AndAlso _TransTypeIds.Count > 0) OrElse _PolicyStatusCodeId > 0 OrElse (_PolicyStatusCodeIds IsNot Nothing AndAlso _PolicyStatusCodeIds.Count > 0) OrElse _PolicyCurrentStatusId > 0 OrElse (_PolicyCurrentStatusIds IsNot Nothing AndAlso _PolicyCurrentStatusIds.Count > 0) OrElse _VersionId > 0 OrElse (_VersionIds IsNot Nothing AndAlso _VersionIds.Count > 0) OrElse _LobId > 0 OrElse (_LobIds IsNot Nothing AndAlso _LobIds.Count > 0) OrElse (String.IsNullOrWhiteSpace(_Policyholder1NameToFind) = False AndAlso Len(_Policyholder1NameToFind) >= 3) Then 'also included VersionId and LobId 11/5/2016; then Name search 11/5/2016
                        'updated 11/28/2022 for stateId/companyId
                        If _TransTypeId > 0 OrElse (_TransTypeIds IsNot Nothing AndAlso _TransTypeIds.Count > 0) OrElse _PolicyStatusCodeId > 0 OrElse (_PolicyStatusCodeIds IsNot Nothing AndAlso _PolicyStatusCodeIds.Count > 0) OrElse _PolicyCurrentStatusId > 0 OrElse (_PolicyCurrentStatusIds IsNot Nothing AndAlso _PolicyCurrentStatusIds.Count > 0) OrElse _VersionId > 0 OrElse (_VersionIds IsNot Nothing AndAlso _VersionIds.Count > 0) OrElse _LobId > 0 OrElse (_LobIds IsNot Nothing AndAlso _LobIds.Count > 0) OrElse _StateId > 0 OrElse (_StateIds IsNot Nothing AndAlso _StateIds.Count > 0) OrElse _CompanyId > 0 OrElse (_CompanyIds IsNot Nothing AndAlso _CompanyIds.Count > 0) OrElse (String.IsNullOrWhiteSpace(_Policyholder1NameToFind) = False AndAlso Len(_Policyholder1NameToFind) >= 3) Then 'also included VersionId and LobId 11/5/2016; then Name search 11/5/2016
                            Return True
                        End If
                    End If
                    Return False
                End If
            End Get
        End Property
        'added 11/5/2016
        Public ReadOnly Property SetToReturnPolicyholder1Name As Boolean
            Get
                If _ForcePolicyholder1NameReturn = True OrElse (String.IsNullOrWhiteSpace(_Policyholder1NameToFind) = False AndAlso Len(_Policyholder1NameToFind) >= 3) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        'added 2/28/2017
        Public Property ClientId As Integer = 0

        'added 3/6/2017
        Public Property LobIds As List(Of Integer) = Nothing
        Public Property VersionIds As List(Of Integer) = Nothing
        Public Property StateIds As List(Of Integer) = Nothing 'added 11/28/2022
        Public Property CompanyIds As List(Of Integer) = Nothing 'added 11/28/2022
        Public Property PolicyCurrentStatusIds As List(Of Integer) = Nothing
        Public Property PolicyStatusCodeIds As List(Of Integer) = Nothing
        Public Property TransTypeIds As List(Of Integer) = Nothing
        Public Property LookupDate As String = ""

        'added 3/22/2017
        Private _DateAdded As String = "" 'PolicyImage.pcadded_date
        Public ReadOnly Property DateAdded As String 'PolicyImage.pcadded_date
            Get
                Return _DateAdded
            End Get
        End Property
        Private _DateModified As String = "" 'PolicyImage.last_modified_date
        Public ReadOnly Property DateModified As String 'PolicyImage.last_modified_date
            Get
                Return _DateModified
            End Get
        End Property

        'DJG - Added 2021/04/26
        Private _EndorsementOriginTypeId As Integer
        Public ReadOnly Property EndorsementOriginTypeId As Integer
            Get
                Return _EndorsementOriginTypeId
            End Get
        End Property

        'added 11/4/2016
        Protected Friend Sub Set_EffectiveDate(ByVal effDate As String)
            _EffectiveDate = effDate
            qqHelper.ConvertToShortDate(_EffectiveDate) 'added 11/10/2016
        End Sub
        Protected Friend Sub Set_ExpirationDate(ByVal expDate As String)
            _ExpirationDate = expDate
            qqHelper.ConvertToShortDate(_ExpirationDate) 'added 11/10/2016
        End Sub
        Protected Friend Sub Set_TransactionEffectiveDate(ByVal tEffDate As String)
            _TransactionEffectiveDate = tEffDate
            qqHelper.ConvertToShortDate(_TransactionEffectiveDate) 'added 11/10/2016
        End Sub
        Protected Friend Sub Set_TransactionExpirationDate(ByVal tExpDate As String)
            _TransactionExpirationDate = tExpDate
            qqHelper.ConvertToShortDate(_TransactionExpirationDate) 'added 11/10/2016
        End Sub
        'added 11/5/2016
        Protected Friend Sub Set_Policyholder1Name(ByVal ph1Name As String)
            _Policyholder1Name = ph1Name
        End Sub
        Protected Friend Sub Set_Policyholder1SortName(ByVal ph1SortName As String)
            _Policyholder1SortName = ph1SortName
        End Sub

        'added 11/10/2016
        Protected Friend Sub Set_FullTermPremium(ByVal prem As String) 'PolicyImage.premium_fullterm
            _FullTermPremium = prem
        End Sub
        Protected Friend Sub Set_ChangeInFullTermPremium(ByVal prem As String) 'PolicyImage.premium_chg_fullterm
            _ChangeInFullTermPremium = prem
        End Sub
        Protected Friend Sub Set_WrittenPremium(ByVal prem As String) 'PolicyImage.premium_written
            _WrittenPremium = prem
        End Sub
        Protected Friend Sub Set_ChangeInWrittenPremium(ByVal prem As String) 'PolicyImage.premium_chg_written
            _ChangeInWrittenPremium = prem
        End Sub

        'added 3/22/2017
        Protected Friend Sub Set_DateAdded(ByVal dtAdded As String) 'PolicyImage.pcadded_date
            _DateAdded = dtAdded
        End Sub
        Protected Friend Sub Set_DateModified(ByVal dtModified As String) 'PolicyImage.last_modified_date
            _DateModified = dtModified
        End Sub
        'added 2021/04/26 - DJG
        Protected Friend Sub Set_EndorsementOriginTypeId(ByVal endorsementOriginTypeId As Integer)
            _EndorsementOriginTypeId = endorsementOriginTypeId
        End Sub
        'added 5/30/2019
        Public Function IsInforce() As Boolean
            Dim isIt As Boolean = False

            Dim polStatus As DiamondPolicyCurrentStatus = PolicyCurrentStatus()
            Select Case polStatus
                Case DiamondPolicyCurrentStatus.InForce
                    If UsingExpirationDateToSeeIfReallyInforce() = True Then
                        If qqHelper.IsValidDateString(_ExpirationDate, mustBeGreaterThanDefaultDate:=True) = True Then
                            If CDate(_ExpirationDate) >= Date.Today Then
                                isIt = True
                            End If
                        Else
                            isIt = True
                        End If
                    Else
                        isIt = True
                    End If
                Case DiamondPolicyCurrentStatus.Cancelled
                    If UsingCancelDateToSeeIfReallyCancelled() = True Then
                        If qqHelper.IsValidDateString(_CancelDate, mustBeGreaterThanDefaultDate:=True) = True AndAlso CDate(_CancelDate) >= Date.Today Then
                            isIt = True
                        End If
                    End If
            End Select

            Return isIt
        End Function
        Public Property statusHelper_useExpirationDateToSeeIfReallyInforce As CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Maybe
        Public Property statusHelper_useCancelDateToSeeIfReallyCancelled As CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Maybe
        Public Function UsingExpirationDateToSeeIfReallyInforce() As Boolean
            Dim usingIt As Boolean = False

            If _statusHelper_useExpirationDateToSeeIfReallyInforce = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then
                usingIt = True
            ElseIf _statusHelper_useExpirationDateToSeeIfReallyInforce = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Maybe Then
                Dim strUseExpirationDateToSeeIfReallyInforce As String = CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("PolicyNumberObject_DiamondPolicyStatus_UseExpirationDateToSeeIfReallyInforce")
                Dim setToYes As Boolean = False
                If String.IsNullOrWhiteSpace(strUseExpirationDateToSeeIfReallyInforce) = False Then
                    If UCase(strUseExpirationDateToSeeIfReallyInforce) = "YES" Then
                        setToYes = True
                    Else
                        Dim qqHelper As New CommonMethods.QuickQuoteHelperClass
                        If qqHelper.BitToBoolean(strUseExpirationDateToSeeIfReallyInforce) = True Then
                            setToYes = True
                        End If
                    End If
                End If
                If setToYes = True Then
                    _statusHelper_useExpirationDateToSeeIfReallyInforce = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes
                    usingIt = True
                Else
                    _statusHelper_useExpirationDateToSeeIfReallyInforce = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.No
                End If
            End If

            Return usingIt
        End Function
        Public Function UsingCancelDateToSeeIfReallyCancelled() As Boolean
            Dim usingIt As Boolean = False

            If _statusHelper_useCancelDateToSeeIfReallyCancelled = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then
                usingIt = True
            ElseIf _statusHelper_useCancelDateToSeeIfReallyCancelled = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Maybe Then
                Dim strUseCancelDateToSeeIfReallyCancelled As String = CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("PolicyNumberObject_DiamondPolicyStatus_UseCancelDateToSeeIfReallyCancelled")
                Dim setToYes As Boolean = False
                If String.IsNullOrWhiteSpace(strUseCancelDateToSeeIfReallyCancelled) = False Then
                    If UCase(strUseCancelDateToSeeIfReallyCancelled) = "YES" Then
                        setToYes = True
                    Else
                        Dim qqHelper As New CommonMethods.QuickQuoteHelperClass
                        If qqHelper.BitToBoolean(strUseCancelDateToSeeIfReallyCancelled) = True Then
                            setToYes = True
                        End If
                    End If
                End If
                If setToYes = True Then
                    _statusHelper_useCancelDateToSeeIfReallyCancelled = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes
                    usingIt = True
                Else
                    _statusHelper_useCancelDateToSeeIfReallyCancelled = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.No
                End If
            End If

            Return usingIt
        End Function
        Enum DiamondPolicyCurrentStatus
            None = 0
            InForce = 1
            Future = 2
            Cancelled = 3
            Pending = 4
            Denied = 5
            NonRenewed = 6
            Expired = 7
            RecordOnly = 8
            ArchivedQuote = 9
        End Enum
        Enum DiamondPolicyStatusCode
            None = 0
            InForce = 1
            Future = 2
            History = 3
            Pending = 4
            RenewalOffer = 5
            WrittenOff = 6
            InitialSubmission = 7
            PendingOOS = 8
            Reapplied = 9
            VoidDueToOOSCancel = 10
            OffsetDueToOOSCancel = 11
            Quote = 12
            ArchivedQuote = 13
            RenewalUnderwriting = 14
            PendingAudit = 15
            ProcessedAudit = 16
            RevisedAudit = 17
            ReversedAudit = 18
            RatingAudit = 19
            Expired = 20
            StartingAudit = 21
        End Enum
        Public Function PolicyCurrentStatus() As DiamondPolicyCurrentStatus
            Dim polStatus As DiamondPolicyCurrentStatus = DiamondPolicyCurrentStatus.None

            System.Enum.TryParse(Of DiamondPolicyCurrentStatus)(_PolicyCurrentStatusId, polStatus)

            Return polStatus
        End Function
        Public Function PolicyStatusCode() As DiamondPolicyStatusCode
            Dim imgStatus As DiamondPolicyStatusCode = DiamondPolicyStatusCode.None

            System.Enum.TryParse(Of DiamondPolicyStatusCode)(_PolicyStatusCodeId, imgStatus)

            Return imgStatus
        End Function
        'added 5/31/2019
        Private _CancelDate As String = ""
        Public ReadOnly Property CancelDate As String
            Get
                Return _CancelDate
            End Get
        End Property
        Protected Friend Sub Set_CancelDate(ByVal cancelDt As String)
            _CancelDate = cancelDt
            qqHelper.ConvertToShortDate(_CancelDate)
        End Sub
        Public Function IsCancelled() As Boolean
            Dim isIt As Boolean = False

            Dim polStatus As DiamondPolicyCurrentStatus = PolicyCurrentStatus()
            Select Case polStatus
                Case DiamondPolicyCurrentStatus.Cancelled
                    If UsingCancelDateToSeeIfReallyCancelled() = True Then
                        If qqHelper.IsValidDateString(_CancelDate, mustBeGreaterThanDefaultDate:=True) = True Then
                            If CDate(_CancelDate) < Date.Today Then
                                isIt = True
                            End If
                        Else
                            isIt = True
                        End If
                    Else
                        isIt = True
                    End If
            End Select

            Return isIt
        End Function
        Public Function IsExpired() As Boolean
            Dim isIt As Boolean = False

            Dim polStatus As DiamondPolicyCurrentStatus = PolicyCurrentStatus()
            Select Case polStatus
                Case DiamondPolicyCurrentStatus.Expired
                    isIt = True
                Case DiamondPolicyCurrentStatus.InForce
                    If UsingExpirationDateToSeeIfReallyInforce() = True Then
                        If qqHelper.IsValidDateString(_ExpirationDate, mustBeGreaterThanDefaultDate:=True) = True AndAlso CDate(_ExpirationDate) < Date.Today Then
                            isIt = True
                        End If
                    End If
            End Select

            Return isIt
        End Function
        Public Function IsInforceOrFuture(Optional ByRef polStatus As DiamondPolicyCurrentStatus = DiamondPolicyCurrentStatus.None) As Boolean
            Dim isIt As Boolean = False
            'polStatus = DiamondPolicyCurrentStatus.None

            'If IsInforce() = True Then
            '    isIt = True
            '    polStatus = DiamondPolicyCurrentStatus.InForce
            'Else
            '    polStatus = ActualPolicyStatus()
            '    Select Case polStatus
            '        Case DiamondPolicyCurrentStatus.Future
            '            isIt = True
            '    End Select
            'End If

            polStatus = ActualPolicyStatus()
            Select Case polStatus
                Case DiamondPolicyCurrentStatus.InForce, DiamondPolicyCurrentStatus.Future
                    isIt = True
            End Select

            Return isIt
        End Function
        Public Function ActualPolicyStatus() As DiamondPolicyCurrentStatus
            Dim polStatus As DiamondPolicyCurrentStatus = DiamondPolicyCurrentStatus.None

            If IsInforce() = True Then
                polStatus = DiamondPolicyCurrentStatus.InForce
            ElseIf IsCancelled() = True Then
                polStatus = DiamondPolicyCurrentStatus.Cancelled
            ElseIf IsExpired() = True Then
                polStatus = DiamondPolicyCurrentStatus.Expired
            Else
                polStatus = PolicyCurrentStatus()
            End If

            Return polStatus
        End Function
    End Class
End Namespace
