Imports System.Configuration
Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store top-level base common information for a quote; includes properties that were previously on QuickQuote only
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteTopLevelQuoteBase_CommonInfo 'added 8/18/2018
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _XmlType As QuickQuoteObject.QuickQuoteXmlType

        'Private _CompanyId As String 'removed 11/26/2022 (now on VersionAndLobInfo)

        Private _Success As Boolean

        Private _AgencyCode As String
        Private _AgencyId As String

        Private _AgencyProducerId As String
        Private _AgencyProducerCode As String
        Private _AgencyProducerName As QuickQuoteName

        Private _QuoteNumber As String
        Private _PolicyNumber As String
        Private _QuoteDescription As String
        Private _EffectiveDate As String
        Private _ExpirationDate As String

        Private _ValidationItems As Generic.List(Of QuickQuoteValidationItem)

        Private _Client As QuickQuoteClient

        Private _IsNew As Boolean
        Private _BillToId As String
        Private _CurrentBilltoId As String
        Private _CurrentPayplanId As String
        Private _PolicyTermId As String
        Private _ReceivedDate As String
        Private _TransactionEffectiveDate As String
        Private _TransactionExpirationDate As String
        Private _TransactionTypeId As String
        Private _TransactionUsersId As String

        Private _WorkflowQueueId As String

        Private _Policyholder As QuickQuotePolicyholder
        Private _Policyholder2 As QuickQuotePolicyholder

        Private _BillMethodId As String
        Private _BillingPayPlanId As String

        Private _PolicyOriginTypeId As String

        Private _HasInitiatedFinalize As Boolean

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _RenewalVersion As String

        Private _PolicyBridgingURL As String

        Private _PaymentOptions As Generic.List(Of QuickQuotePaymentOption)
        Private _CurrentlyParsingPaymentOptions As Boolean

        Private _ExperienceModificationFactor As String
        Private _ExperienceModificationBureauTypeId As String '-1 = , 0 = N/A, 1 = NCCI
        Private _ExperienceModificationRiskIdentifier As String
        Private _ExperienceModifications As List(Of QuickQuoteExperienceModification)
        Private _CanUseExperienceModificationNumForExperienceModificationReconciliation As Boolean
        Private _HasConvertedExperienceModifications As Boolean
        Private _DiamondExperienceModificationIndexesToUpdate As List(Of Integer)

        Private _GuaranteedRatePeriodEffectiveDate As String
        Private _GuaranteedRatePeriodExpirationDate As String
        Private _ModificationProductionDate As String
        Private _RatingEffectiveDate As String

        Private _AdditionalPolicyholders As Generic.List(Of QuickQuoteAdditionalPolicyholder)

        Private _QuoteTypeId As String '0 = N/A; 1 = Quick Quote; 2 = New Application; 3 = Finalize Quote

        Private _EFT_BankRoutingNumber As String
        Private _EFT_BankAccountNumber As String
        Private _EFT_BankAccountTypeId As String
        Private _EFT_DeductionDay As String

        Private _OnlyUsePropertyToSetFieldWithSameName As Boolean

        Private _Agency As QuickQuoteAgency

        Private _PolicyImageId As String

        Private _BillingAddressee As QuickQuoteBillingAddressee

        Private _FirstWrittenDate As String

        Private _QuoteTransactionType As QuickQuoteObject.QuickQuoteTransactionType
        Private _OriginalEffectiveDate As String
        Private _OriginalExpirationDate As String
        Private _OriginalTransactionEffectiveDate As String
        Private _OriginalTransactionExpirationDate As String

        Private _TransactionRemark As String
        Private _TransactionReasonId As String

        Private _Comments As List(Of QuickQuoteComment)

        Private _Messages As List(Of QuickQuoteMessage)

        'Private _QuoteLevel As QuickQuoteHelperClass.QuoteLevel '12/30/2018 - moved to StateAndLobParts object so it would not get Copied between topLevel and stateLevel quotes

        'added 5/22/2019
        Private _AddedDate As String
        Private _LastModifiedDate As String
        Private _PCAdded_Date As String

        'added 6/15/2019
        Private _PolicyCurrentStatusId As String
        Private _PolicyStatusCodeId As String
        Private _CancelDate As String

        Private _DevDictionary As New QuickQuoteDevDictionaryList

        'added 9/28/2021
        Private _InitialBillingPayPlanIdAtRetrieval As String
        Private _InitialBillingPayPlanStaticDataOptionAtRetrieval As QuickQuoteStaticDataOption
        Private _ConvertedBillingPayPlanIdAtRetrieval As String
        Private _ConvertedBillingPayPlanStaticDataOptionAtRetrieval As QuickQuoteStaticDataOption
        Private _InitialCurrentPayPlanIdAtRetrieval As String
        Private _InitialCurrentPayPlanStaticDataOptionAtRetrieval As QuickQuoteStaticDataOption
        Private _ConvertedCurrentPayPlanIdAtRetrieval As String
        Private _ConvertedCurrentPayPlanStaticDataOptionAtRetrieval As QuickQuoteStaticDataOption
        Private _UseBillingPayPlanIdConvertedAtSave As Boolean
        Private _BillingPayPlanIdConvertedAtSave As String
        Private _UseCurrentPayPlanIdConvertedAtSave As Boolean
        Private _CurrentPayPlanIdConvertedAtSave As String

        Public Property XmlType As QuickQuoteObject.QuickQuoteXmlType
            Get
                Return _XmlType
            End Get
            Set(value As QuickQuoteObject.QuickQuoteXmlType)
                _XmlType = value
            End Set
        End Property
        'Public Property CompanyId As String 'removed 11/26/2022 (now on VersionAndLobInfo)
        '    Get
        '        Return _CompanyId
        '    End Get
        '    Set(value As String)
        '        _CompanyId = value
        '    End Set
        'End Property
        Public Property Success As Boolean
            Get
                Return _Success
            End Get
            Set(value As Boolean)
                _Success = value
            End Set
        End Property
        Public Property AgencyCode As String
            Get
                Return _AgencyCode
            End Get
            Set(value As String)
                _AgencyCode = value
            End Set
        End Property
        Public Property AgencyId As String
            Get
                Return _AgencyId
            End Get
            Set(value As String)
                _AgencyId = value
            End Set
        End Property
        Public Property AgencyProducerId As String
            Get
                Return _AgencyProducerId
            End Get
            Set(value As String)
                _AgencyProducerId = value
            End Set
        End Property
        Public Property AgencyProducerCode As String
            Get
                Return _AgencyProducerCode
            End Get
            Set(value As String)
                _AgencyProducerCode = value
            End Set
        End Property
        Public Property AgencyProducerName As QuickQuoteName
            Get
                Return _AgencyProducerName
            End Get
            Set(value As QuickQuoteName)
                _AgencyProducerName = value
            End Set
        End Property
        Public Property QuoteNumber As String
            Get
                Dim quoteNumToReturn As String = _QuoteNumber
                If String.IsNullOrWhiteSpace(quoteNumToReturn) = True AndAlso String.IsNullOrWhiteSpace(_PolicyNumber) = False AndAlso UCase(Left(_PolicyNumber, 1)) = "Q" Then
                    quoteNumToReturn = _PolicyNumber
                End If
                Return quoteNumToReturn
            End Get
            Set(value As String)
                _QuoteNumber = value
            End Set
        End Property
        Protected Friend ReadOnly Property QuoteNumber_Actual As String
            Get
                Return _QuoteNumber
            End Get
        End Property
        Public Property PolicyNumber As String
            Get
                Dim polNumToReturn As String = _PolicyNumber
                If String.IsNullOrWhiteSpace(polNumToReturn) = True AndAlso String.IsNullOrWhiteSpace(_QuoteNumber) = False Then
                    polNumToReturn = _QuoteNumber
                End If
                Return polNumToReturn
            End Get
            Set(value As String)
                _PolicyNumber = value
            End Set
        End Property
        Protected Friend ReadOnly Property PolicyNumber_Actual As String
            Get
                Return _PolicyNumber
            End Get
        End Property
        Public Property QuoteDescription As String
            Get
                Return _QuoteDescription
            End Get
            Set(value As String)
                _QuoteDescription = value
            End Set
        End Property
        Public Property EffectiveDate As String
            Get
                Return _EffectiveDate
            End Get
            Set(value As String)
                Dim oldEffectiveDate As String = _EffectiveDate
                _EffectiveDate = value
                'VersionAndLobInfo.Set_QuoteEffectiveDate(_EffectiveDate) 'still being called from QuickQuoteObject Property
                qqHelper.ConvertToShortDate(_EffectiveDate)

                If _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    Exit Property
                End If

                If IsDate(_EffectiveDate) = True Then
                    ExpirationDate = DateAdd(DateInterval.Year, 1, CDate(_EffectiveDate)).ToString
                    _TransactionEffectiveDate = _EffectiveDate
                    '_TransactionExpirationDate = _ExpirationDate 'should automatically happen when above logic sets ExpirationDate property
                    _GuaranteedRatePeriodEffectiveDate = _EffectiveDate
                    '_GuaranteedRatePeriodExpirationDate = _ExpirationDate 'should automatically happen when above logic sets ExpirationDate property

                    If _FirstWrittenDate = "" OrElse IsDate(_FirstWrittenDate) = False Then
                        'blank or invalid
                        _FirstWrittenDate = _EffectiveDate
                    Else
                        'valid date
                        If CDate(_FirstWrittenDate) > CDate(_EffectiveDate) Then
                            'FWD cannot be after effDate
                            _FirstWrittenDate = _EffectiveDate
                        Else
                            'FWD is either the same or older than new effDate
                            If oldEffectiveDate = "" OrElse IsDate(oldEffectiveDate) = False Then
                                'blank or invalid previous effDate

                            Else
                                'valid previous effDate
                                If CDate(_FirstWrittenDate) = CDate(oldEffectiveDate) Then
                                    _FirstWrittenDate = _EffectiveDate
                                End If
                            End If
                        End If
                    End If
                End If
            End Set
        End Property
        Public Property ExpirationDate As String
            Get
                Return _ExpirationDate
            End Get
            Set(value As String)
                _ExpirationDate = value
                qqHelper.ConvertToShortDate(_ExpirationDate)

                If _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    Exit Property
                End If

                If IsDate(_ExpirationDate) = True Then
                    _GuaranteedRatePeriodExpirationDate = _ExpirationDate
                    _TransactionExpirationDate = _ExpirationDate
                End If
            End Set
        End Property
        Public Property ValidationItems As Generic.List(Of QuickQuoteValidationItem)
            Get
                Return _ValidationItems
            End Get
            Set(value As Generic.List(Of QuickQuoteValidationItem))
                _ValidationItems = value
            End Set
        End Property
        Public Property Client As QuickQuoteClient
            Get
                Return _Client
            End Get
            Set(value As QuickQuoteClient)
                _Client = value
            End Set
        End Property

        Public Property IsNew As Boolean
            Get
                Return _IsNew
            End Get
            Set(value As Boolean)
                _IsNew = value
            End Set
        End Property
        Public Property BillToId As String
            Get
                Return _BillToId
            End Get
            Set(value As String)
                _BillToId = value
                'sets both when set by developer; only sets 1 when read from xml
                If _OnlyUsePropertyToSetFieldWithSameName = False Then
                    _CurrentBilltoId = value
                Else 'don't set other value; set flag back to False after
                    _OnlyUsePropertyToSetFieldWithSameName = False
                End If
            End Set
        End Property
        Public Property CurrentBilltoId As String
            Get
                Return _CurrentBilltoId
            End Get
            Set(value As String)
                _CurrentBilltoId = value
                'sets both when set by developer; only sets 1 when read from xml
                If _OnlyUsePropertyToSetFieldWithSameName = False Then
                    _BillToId = value
                Else 'don't set other value; set flag back to False after
                    _OnlyUsePropertyToSetFieldWithSameName = False
                End If
            End Set
        End Property
        Public Property CurrentPayplanId As String 'triggered off this instead of BillingPayPlanId
            Get
                Return _CurrentPayplanId
            End Get
            Set(value As String)
                _CurrentPayplanId = value
                'sets both when set by developer; only sets 1 when read from xml
                If _OnlyUsePropertyToSetFieldWithSameName = False Then
                    _BillingPayPlanId = value
                Else 'don't set other value; set flag back to False after
                    _OnlyUsePropertyToSetFieldWithSameName = False
                End If
            End Set
        End Property
        Public Property PolicyTermId As String
            Get
                Return _PolicyTermId
            End Get
            Set(value As String)
                _PolicyTermId = value
            End Set
        End Property
        Public Property ReceivedDate As String
            Get
                Return _ReceivedDate
            End Get
            Set(value As String)
                _ReceivedDate = value
                qqHelper.ConvertToShortDate(_ReceivedDate)
            End Set
        End Property
        Public Property TransactionEffectiveDate As String
            Get
                Return _TransactionEffectiveDate
            End Get
            Set(value As String)
                If _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    _TransactionEffectiveDate = value
                    qqHelper.ConvertToShortDate(_TransactionEffectiveDate)
                    Exit Property
                End If

                If _TransactionEffectiveDate = "" OrElse IsDate(_TransactionEffectiveDate) = False OrElse _EffectiveDate = "" OrElse IsDate(_EffectiveDate) = False Then
                    _TransactionEffectiveDate = value
                    qqHelper.ConvertToShortDate(_TransactionEffectiveDate)
                    If IsDate(_TransactionEffectiveDate) = True Then
                        _TransactionExpirationDate = DateAdd(DateInterval.Year, 1, CDate(_TransactionEffectiveDate)).ToString
                        qqHelper.ConvertToShortDate(_TransactionExpirationDate)
                    End If
                End If
            End Set
        End Property
        Public Property TransactionExpirationDate As String
            Get
                Return _TransactionExpirationDate
            End Get
            Set(value As String)
                If _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    _TransactionExpirationDate = value
                    qqHelper.ConvertToShortDate(_TransactionExpirationDate)
                    Exit Property
                End If

                If _TransactionExpirationDate = "" OrElse IsDate(_TransactionExpirationDate) = False OrElse _ExpirationDate = "" OrElse IsDate(_ExpirationDate) = False Then
                    _TransactionExpirationDate = value
                    qqHelper.ConvertToShortDate(_TransactionExpirationDate)
                End If
            End Set
        End Property
        Public Property TransactionTypeId As String
            Get
                Return _TransactionTypeId
            End Get
            Set(value As String)
                _TransactionTypeId = value
            End Set
        End Property
        Public Property TransactionUsersId As String
            Get
                Return _TransactionUsersId
            End Get
            Set(value As String)
                _TransactionUsersId = value
            End Set
        End Property
        Public Property WorkflowQueueId As String
            Get
                Return _WorkflowQueueId
            End Get
            Set(value As String)
                _WorkflowQueueId = value
            End Set
        End Property

        Public Property Policyholder As QuickQuotePolicyholder
            Get
                Return _Policyholder
            End Get
            Set(value As QuickQuotePolicyholder)
                _Policyholder = value
            End Set
        End Property
        Public Property Policyholder2 As QuickQuotePolicyholder
            Get
                Return _Policyholder2
            End Get
            Set(value As QuickQuotePolicyholder)
                _Policyholder2 = value
            End Set
        End Property

        Public Property BillMethodId As String
            Get
                Return _BillMethodId
            End Get
            Set(value As String)
                _BillMethodId = value
            End Set
        End Property
        Public Property BillingPayPlanId As String 'triggered off CurrentPayPlanId instead of this one
            Get
                Return _BillingPayPlanId
            End Get
            Set(value As String)
                _BillingPayPlanId = value
                'sets both when set by developer; only sets 1 when read from xml
                If _OnlyUsePropertyToSetFieldWithSameName = False Then
                    _CurrentPayplanId = value
                Else 'don't set other value; set flag back to False after
                    _OnlyUsePropertyToSetFieldWithSameName = False
                End If
            End Set
        End Property

        Public Property PolicyOriginTypeId As String
            Get
                Return _PolicyOriginTypeId
            End Get
            Set(value As String)
                _PolicyOriginTypeId = value
            End Set
        End Property
        Public Property HasInitiatedFinalize As Boolean
            Get
                Return _HasInitiatedFinalize
            End Get
            Set(value As Boolean)
                _HasInitiatedFinalize = value
            End Set
        End Property

        Public Property PolicyId As String
            Get
                Return _PolicyId
            End Get
            Set(value As String)
                _PolicyId = value
            End Set
        End Property
        Public Property PolicyImageNum As String
            Get
                Return _PolicyImageNum
            End Get
            Set(value As String)
                _PolicyImageNum = value
            End Set
        End Property
        Public Property RenewalVersion As String
            Get
                Return _RenewalVersion
            End Get
            Set(value As String)
                _RenewalVersion = value
            End Set
        End Property

        Public Property PolicyBridgingURL As String
            Get
                Return _PolicyBridgingURL
            End Get
            Set(value As String)
                _PolicyBridgingURL = value
            End Set
        End Property

        Public Property PaymentOptions As Generic.List(Of QuickQuotePaymentOption)
            Get
                If _CurrentlyParsingPaymentOptions = False AndAlso (_PaymentOptions Is Nothing OrElse _PaymentOptions.Count = 0) AndAlso qqHelper.IsPositiveIntegerString(_PolicyId) = True AndAlso qqHelper.IsPositiveIntegerString(_PolicyImageNum) = True Then
                    Dim img As Diamond.Common.Objects.Policy.Image = QuickQuoteHelperClass.GetPolicyImage(_PolicyId, _PolicyImageNum)
                    If img IsNot Nothing Then
                        Dim strPayplanPreviewsXml As String = ""
                        Dim qqXml As New QuickQuoteXML
                        qqXml.DiamondService_LoadPaymentOptions(img, strPayplanPreviewsXml)
                        If strPayplanPreviewsXml <> "" Then
                            qqXml.ParseArrayOfPayPlanPreviewXmlString(strPayplanPreviewsXml, _PaymentOptions)

                            qqHelper.NumberPaymentOptions(_PaymentOptions)
                        End If
                    End If
                End If
                Return _PaymentOptions
            End Get
            Set(value As Generic.List(Of QuickQuotePaymentOption))
                _PaymentOptions = value
            End Set
        End Property
        Public Property ExperienceModificationFactor As String
            Get
                Return _ExperienceModificationFactor
            End Get
            Set(value As String)
                _ExperienceModificationFactor = value
            End Set
        End Property
        Public Property ExperienceModificationBureauTypeId As String
            Get
                Return _ExperienceModificationBureauTypeId
            End Get
            Set(value As String)
                _ExperienceModificationBureauTypeId = value
            End Set
        End Property
        Public Property ExperienceModificationRiskIdentifier As String
            Get
                Return _ExperienceModificationRiskIdentifier
            End Get
            Set(value As String)
                _ExperienceModificationRiskIdentifier = value
            End Set
        End Property
        Public Property ExperienceModifications As List(Of QuickQuoteExperienceModification)
            Get
                Return _ExperienceModifications
            End Get
            Set(value As List(Of QuickQuoteExperienceModification))
                _ExperienceModifications = value
            End Set
        End Property
        Public Property CanUseExperienceModificationNumForExperienceModificationReconciliation As Boolean
            Get
                Return _CanUseExperienceModificationNumForExperienceModificationReconciliation
            End Get
            Set(value As Boolean)
                _CanUseExperienceModificationNumForExperienceModificationReconciliation = value
            End Set
        End Property
        Public Property HasConvertedExperienceModifications As Boolean
            Get
                Return _HasConvertedExperienceModifications
            End Get
            Set(value As Boolean)
                _HasConvertedExperienceModifications = value
            End Set
        End Property
        Protected Friend Property DiamondExperienceModificationIndexesToUpdate As List(Of Integer)
            Get
                Return _DiamondExperienceModificationIndexesToUpdate
            End Get
            Set(value As List(Of Integer))
                _DiamondExperienceModificationIndexesToUpdate = value
            End Set
        End Property
        Public Property GuaranteedRatePeriodEffectiveDate As String
            Get
                Return _GuaranteedRatePeriodEffectiveDate
            End Get
            Set(value As String)
                If _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    _GuaranteedRatePeriodEffectiveDate = value
                    qqHelper.ConvertToShortDate(_GuaranteedRatePeriodEffectiveDate)
                    Exit Property
                End If

                If _GuaranteedRatePeriodEffectiveDate = "" OrElse IsDate(_GuaranteedRatePeriodEffectiveDate) = False OrElse _EffectiveDate = "" OrElse IsDate(_EffectiveDate) = False Then
                    _GuaranteedRatePeriodEffectiveDate = value
                    qqHelper.ConvertToShortDate(_GuaranteedRatePeriodEffectiveDate)
                    If IsDate(_GuaranteedRatePeriodEffectiveDate) = True Then
                        _GuaranteedRatePeriodExpirationDate = DateAdd(DateInterval.Year, 1, CDate(_GuaranteedRatePeriodEffectiveDate)).ToString
                        qqHelper.ConvertToShortDate(_GuaranteedRatePeriodExpirationDate)
                    End If
                End If
            End Set
        End Property
        Public Property GuaranteedRatePeriodExpirationDate As String
            Get
                Return _GuaranteedRatePeriodExpirationDate
            End Get
            Set(value As String)
                If _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    _GuaranteedRatePeriodExpirationDate = value
                    qqHelper.ConvertToShortDate(_GuaranteedRatePeriodExpirationDate)
                    Exit Property
                End If

                If _GuaranteedRatePeriodExpirationDate = "" OrElse IsDate(_GuaranteedRatePeriodExpirationDate) = False OrElse _ExpirationDate = "" OrElse IsDate(_ExpirationDate) = False Then
                    _GuaranteedRatePeriodExpirationDate = value
                    qqHelper.ConvertToShortDate(_GuaranteedRatePeriodExpirationDate)
                End If
            End Set
        End Property
        Public Property ModificationProductionDate As String
            Get
                Return _ModificationProductionDate
            End Get
            Set(value As String)
                _ModificationProductionDate = value
                qqHelper.ConvertToShortDate(_ModificationProductionDate)
            End Set
        End Property
        Public Property RatingEffectiveDate As String
            Get
                Return _RatingEffectiveDate
            End Get
            Set(value As String)
                _RatingEffectiveDate = value
                qqHelper.ConvertToShortDate(_RatingEffectiveDate)
            End Set
        End Property
        Public Property AdditionalPolicyholders As Generic.List(Of QuickQuoteAdditionalPolicyholder)
            Get
                Return _AdditionalPolicyholders
            End Get
            Set(value As Generic.List(Of QuickQuoteAdditionalPolicyholder))
                _AdditionalPolicyholders = value
            End Set
        End Property
        Public Property QuoteTypeId As String
            Get
                Return _QuoteTypeId
            End Get
            Set(value As String)
                _QuoteTypeId = value
            End Set
        End Property
        Public Property EFT_BankRoutingNumber As String
            Get
                Return _EFT_BankRoutingNumber
            End Get
            Set(value As String)
                _EFT_BankRoutingNumber = value
            End Set
        End Property
        Public Property EFT_BankAccountNumber As String
            Get
                Return _EFT_BankAccountNumber
            End Get
            Set(value As String)
                _EFT_BankAccountNumber = value
            End Set
        End Property
        Public Property EFT_BankAccountTypeId As String '1=Checking; 2=Savings
            Get
                Return _EFT_BankAccountTypeId
            End Get
            Set(value As String)
                _EFT_BankAccountTypeId = value
            End Set
        End Property
        Public Property EFT_DeductionDay As String
            Get
                Return _EFT_DeductionDay
            End Get
            Set(value As String)
                _EFT_DeductionDay = value
            End Set
        End Property

        Public Property OnlyUsePropertyToSetFieldWithSameName As Boolean
            Get
                Return _OnlyUsePropertyToSetFieldWithSameName
            End Get
            Set(value As Boolean)
                _OnlyUsePropertyToSetFieldWithSameName = value
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

        Public Property PolicyImageId As String
            Get
                Return _PolicyImageId
            End Get
            Set(value As String)
                _PolicyImageId = value
            End Set
        End Property
        Public Property BillingAddressee As QuickQuoteBillingAddressee
            Get
                Return _BillingAddressee
            End Get
            Set(value As QuickQuoteBillingAddressee)
                _BillingAddressee = value
            End Set
        End Property
        Public Property FirstWrittenDate As String 'will only be used if it's there... else will keep Diamond default
            Get
                Return _FirstWrittenDate
            End Get
            Set(value As String)
                _FirstWrittenDate = value
                qqHelper.ConvertToShortDate(_FirstWrittenDate)
            End Set
        End Property
        Public ReadOnly Property QuoteTransactionType As QuickQuoteObject.QuickQuoteTransactionType
            Get
                Return _QuoteTransactionType
            End Get
        End Property
        Public ReadOnly Property OriginalEffectiveDate As String
            Get
                Return _OriginalEffectiveDate
            End Get
        End Property
        Public ReadOnly Property OriginalExpirationDate As String
            Get
                Return _OriginalExpirationDate
            End Get
        End Property
        Public ReadOnly Property OriginalTransactionEffectiveDate As String
            Get
                Return _OriginalTransactionEffectiveDate
            End Get
        End Property
        Public ReadOnly Property OriginalTransactionExpirationDate As String
            Get
                Return _OriginalTransactionExpirationDate
            End Get
        End Property
        Public Property TransactionRemark As String
            Get
                Return _TransactionRemark
            End Get
            Set(value As String)
                _TransactionRemark = value
            End Set
        End Property
        Public Property TransactionReasonId As String
            Get
                Return _TransactionReasonId
            End Get
            Set(value As String)
                _TransactionReasonId = value
            End Set
        End Property
        Public Property Comments As List(Of QuickQuoteComment)
            Get
                Return _Comments
            End Get
            Set(value As List(Of QuickQuoteComment))
                _Comments = value
            End Set
        End Property
        Public Property Messages As List(Of QuickQuoteMessage) 'TODO: Dan - Parent?
            Get
                Return _Messages
            End Get
            Set(value As List(Of QuickQuoteMessage))
                _Messages = value
            End Set
        End Property

        'Public ReadOnly Property QuoteLevel As QuickQuoteHelperClass.QuoteLevel '12/30/2018 - moved to StateAndLobParts object so it would not get Copied between topLevel and stateLevel quotes
        '    Get
        '        Return _QuoteLevel
        '    End Get
        'End Property

        'added 5/22/2019
        Public Property AddedDate As String
            Get
                Return _AddedDate
            End Get
            Set(value As String)
                _AddedDate = value
            End Set
        End Property
        Public Property LastModifiedDate As String
            Get
                Return _LastModifiedDate
            End Get
            Set(value As String)
                _LastModifiedDate = value
            End Set
        End Property
        Public Property PCAdded_Date As String
            Get
                Return _PCAdded_Date
            End Get
            Set(value As String)
                _PCAdded_Date = value
            End Set
        End Property

        'added 6/15/2019
        Public Property PolicyCurrentStatusId As String
            Get
                Return _PolicyCurrentStatusId
            End Get
            Set(value As String)
                _PolicyCurrentStatusId = value
            End Set
        End Property
        Public Property PolicyStatusCodeId As String
            Get
                Return _PolicyStatusCodeId
            End Get
            Set(value As String)
                _PolicyStatusCodeId = value
            End Set
        End Property
        Public Property CancelDate As String
            Get
                Return _CancelDate
            End Get
            Set(value As String)
                _CancelDate = value
            End Set
        End Property

        Protected Friend Property InternalDevDictionary As QuickQuoteDevDictionaryList
            Get
                Return _DevDictionary
            End Get
            Set(value As QuickQuoteDevDictionaryList)
                _DevDictionary = value
            End Set
        End Property

        'added 9/28/2021
        Public ReadOnly Property InitialBillingPayPlanIdAtRetrieval As String
            Get
                Return _InitialBillingPayPlanIdAtRetrieval
            End Get
        End Property
        Public ReadOnly Property InitialBillingPayPlanStaticDataOptionAtRetrieval As QuickQuoteStaticDataOption
            Get
                Return _InitialBillingPayPlanStaticDataOptionAtRetrieval
            End Get
        End Property
        Public ReadOnly Property ConvertedBillingPayPlanIdAtRetrieval As String
            Get
                Return _ConvertedBillingPayPlanIdAtRetrieval
            End Get
        End Property
        Public ReadOnly Property ConvertedBillingPayPlanStaticDataOptionAtRetrieval As QuickQuoteStaticDataOption
            Get
                Return _ConvertedBillingPayPlanStaticDataOptionAtRetrieval
            End Get
        End Property
        Public ReadOnly Property InitialCurrentPayPlanIdAtRetrieval As String
            Get
                Return _InitialCurrentPayPlanIdAtRetrieval
            End Get
        End Property
        Public ReadOnly Property InitialCurrentPayPlanStaticDataOptionAtRetrieval As QuickQuoteStaticDataOption
            Get
                Return _InitialCurrentPayPlanStaticDataOptionAtRetrieval
            End Get
        End Property
        Public ReadOnly Property ConvertedCurrentPayPlanIdAtRetrieval As String
            Get
                Return _ConvertedCurrentPayPlanIdAtRetrieval
            End Get
        End Property
        Public ReadOnly Property ConvertedCurrentPayPlanStaticDataOptionAtRetrieval As QuickQuoteStaticDataOption
            Get
                Return _ConvertedCurrentPayPlanStaticDataOptionAtRetrieval
            End Get
        End Property
        Public ReadOnly Property UseBillingPayPlanIdConvertedAtSave As Boolean
            Get
                Return _UseBillingPayPlanIdConvertedAtSave
            End Get
        End Property
        Public ReadOnly Property BillingPayPlanIdConvertedAtSave As String
            Get
                Return _BillingPayPlanIdConvertedAtSave
            End Get
        End Property
        Public ReadOnly Property UseCurrentPayPlanIdConvertedAtSave As Boolean
            Get
                Return _UseCurrentPayPlanIdConvertedAtSave
            End Get
        End Property
        Public ReadOnly Property CurrentPayPlanIdConvertedAtSave As String
            Get
                Return _CurrentPayPlanIdConvertedAtSave
            End Get
        End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Public Sub New(Parent As Object) 'generic, but Parent will likely be TopLevelQuoteInfo
            MyBase.New()
            SetDefaults()
            Me.SetParent = Parent
        End Sub
        Private Sub SetDefaults()
            _XmlType = QuickQuoteObject.QuickQuoteXmlType.None
            '_CompanyId = "1" '12/11/2013 note: not worth using xml right now; company_code is blank for both company_id 0 and 1; 1 has am_best_number 2251; removed 11/26/2022 (now on VersionAndLobInfo)

            _Success = False

            _AgencyCode = ""
            _AgencyId = ""
            If System.Web.HttpContext.Current?.Session("DiamondAgencyCode") IsNot Nothing AndAlso System.Web.HttpContext.Current?.Session("DiamondAgencyCode").ToString <> "" Then
                _AgencyCode = System.Web.HttpContext.Current?.Session("DiamondAgencyCode").ToString
            ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
                _AgencyCode = ConfigurationManager.AppSettings("QuickQuoteTestAgencyCode").ToString
            End If
            If System.Web.HttpContext.Current?.Session("DiamondAgencyId") IsNot Nothing AndAlso System.Web.HttpContext.Current?.Session("DiamondAgencyId").ToString <> "" AndAlso IsNumeric(System.Web.HttpContext.Current?.Session("DiamondAgencyId").ToString) = True Then
                _AgencyId = System.Web.HttpContext.Current?.Session("DiamondAgencyId").ToString
            ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
                _AgencyId = ConfigurationManager.AppSettings("QuickQuoteTestAgencyId").ToString
            End If
            '8/13/2014 note: could update to use new shared functions in QuickQuoteHelperClass

            'added for staff
            If _AgencyCode = "" OrElse _AgencyId = "" Then
                qqHelper.SetUserAgencyVariables()
                If System.Web.HttpContext.Current?.Session("DiamondAgencyCode") IsNot Nothing AndAlso System.Web.HttpContext.Current?.Session("DiamondAgencyCode").ToString <> "" Then
                    _AgencyCode = System.Web.HttpContext.Current?.Session("DiamondAgencyCode").ToString
                End If
                If System.Web.HttpContext.Current?.Session("DiamondAgencyId") IsNot Nothing AndAlso System.Web.HttpContext.Current?.Session("DiamondAgencyId").ToString <> "" AndAlso IsNumeric(System.Web.HttpContext.Current?.Session("DiamondAgencyId").ToString) = True Then
                    _AgencyId = System.Web.HttpContext.Current?.Session("DiamondAgencyId").ToString
                End If
            End If


            _AgencyProducerId = ""
            _AgencyProducerCode = ""
            _AgencyProducerName = New QuickQuoteName
            _AgencyProducerName.NameAddressSourceId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.NameAddressSourceId, "Agency Producer")

            ' 07/18/2022 CAH - Don suggested upate during RenewalVersion addition to QQ.
            Reset_Database_Values()
            '_QuoteNumber = "" 'note: now being reset from quickQuote.Reset_Database_Values() too
            '_PolicyNumber = "" 'note: now being reset from quickQuote.Reset_Database_Values() too


            _QuoteDescription = ""

            _EffectiveDate = ""
            'VersionAndLobInfo.Set_QuoteEffectiveDate(_EffectiveDate) 'still being set from QuickQuoteObject's SetDefaults method
            _ExpirationDate = ""

            _ValidationItems = Nothing

            _Client = New QuickQuoteClient
            _Client.Name.NameAddressSourceId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.NameAddressSourceId, "Client 1")
            _Client.Name2.NameAddressSourceId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.NameAddressSourceId, "Client 2")

            _IsNew = False
            _BillToId = ""
            _CurrentBilltoId = _BillToId
            _CurrentPayplanId = ""
            _PolicyTermId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, "12 Month")
            _ReceivedDate = Date.Today.ToShortDateString '*defaulting to current date
            _TransactionEffectiveDate = Date.Today.ToShortDateString '*defaulting to current date; note 12/11/2013: may need to change to always use EffectiveDate
            _TransactionExpirationDate = DateAdd(DateInterval.Year, 1, CDate(_TransactionEffectiveDate)).ToShortDateString
            _TransactionTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.TransactionTypeId, "New Business")
            _TransactionUsersId = ""
            QuickQuoteHelperClass.SetDiamondUserId(_TransactionUsersId)
            _WorkflowQueueId = ""

            _Policyholder = New QuickQuotePolicyholder
            _Policyholder.Name.NameAddressSourceId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.NameAddressSourceId, "Policy Holder 1")
            _Policyholder2 = New QuickQuotePolicyholder
            _Policyholder2.Name.NameAddressSourceId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.NameAddressSourceId, "Policy Holder 2")

            _BillMethodId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, "Direct Bill")
            _BillingPayPlanId = _CurrentPayplanId

            Default_PolicyOriginTypeId()

            _HasInitiatedFinalize = False

            _PolicyBridgingURL = ""

            _PaymentOptions = Nothing
            _CurrentlyParsingPaymentOptions = False

            _ExperienceModificationFactor = ""
            _ExperienceModificationBureauTypeId = ""
            _ExperienceModificationRiskIdentifier = ""
            _ExperienceModifications = Nothing
            _CanUseExperienceModificationNumForExperienceModificationReconciliation = False
            _HasConvertedExperienceModifications = False
            _DiamondExperienceModificationIndexesToUpdate = Nothing

            _GuaranteedRatePeriodEffectiveDate = Date.Today.ToShortDateString
            _GuaranteedRatePeriodExpirationDate = DateAdd(DateInterval.Year, 1, CDate(_GuaranteedRatePeriodEffectiveDate)).ToShortDateString
            _ModificationProductionDate = ""
            _RatingEffectiveDate = ""

            _AdditionalPolicyholders = Nothing

            _QuoteTypeId = ""

            _EFT_BankRoutingNumber = ""
            _EFT_BankAccountNumber = ""
            _EFT_BankAccountTypeId = ""
            _EFT_DeductionDay = ""

            _OnlyUsePropertyToSetFieldWithSameName = False

            'Reset_Database_Values() 'removed 8/18/2018

            _Agency = New QuickQuoteAgency

            _BillingAddressee = New QuickQuoteBillingAddressee

            _FirstWrittenDate = ""

            _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote
            'VersionAndLobInfo.Set_QuoteTransactionType(_QuoteTransactionType) 'will still be called from QuickQuoteObject's SetDefaults
            _OriginalEffectiveDate = ""
            _OriginalExpirationDate = ""
            _OriginalTransactionEffectiveDate = ""
            _OriginalTransactionExpirationDate = ""

            _TransactionRemark = ""
            _TransactionReasonId = ""

            _Comments = Nothing

            _Messages = Nothing

            '_QuoteLevel = QuickQuoteHelperClass.QuoteLevel.None '12/30/2018 - moved to StateAndLobParts object so it would not get Copied between topLevel and stateLevel quotes

            'added 5/22/2019
            _AddedDate = ""
            _LastModifiedDate = ""
            _PCAdded_Date = ""

            'added 6/15/2019
            _PolicyCurrentStatusId = ""
            _PolicyStatusCodeId = ""
            _CancelDate = ""

            'added 9/28/2021
            _InitialBillingPayPlanIdAtRetrieval = ""
            _InitialBillingPayPlanStaticDataOptionAtRetrieval = Nothing
            _ConvertedBillingPayPlanIdAtRetrieval = ""
            _ConvertedBillingPayPlanStaticDataOptionAtRetrieval = Nothing
            _InitialCurrentPayPlanIdAtRetrieval = ""
            _InitialCurrentPayPlanStaticDataOptionAtRetrieval = Nothing
            _ConvertedCurrentPayPlanIdAtRetrieval = ""
            _ConvertedCurrentPayPlanStaticDataOptionAtRetrieval = Nothing
            _UseBillingPayPlanIdConvertedAtSave = False
            _BillingPayPlanIdConvertedAtSave = ""
            _UseCurrentPayPlanIdConvertedAtSave = False
            _CurrentPayPlanIdConvertedAtSave = ""

        End Sub

        Public Sub Reset_Database_Values() 'for Copy Quote functionality so database values (specifically quoteId) aren't tied to new quote... since new validation will now prevent save or rate from completing... also don't want to inadvertently use previous policyId, imageNum, or policyImageId from copied quote when using Diamond services
            _QuoteNumber = "" 'note: already being reset from SetDefaults() too
            _PolicyNumber = "" 'note: already being reset from SetDefaults() too

            _PolicyId = ""
            _PolicyImageNum = ""
            _PolicyImageId = ""
            _RenewalVersion = ""
            'If _VersionAndLobInfo IsNot Nothing AndAlso _VersionAndLobInfo.PolicyLevelInfoExtended IsNot Nothing Then 'still being called from QuickQuoteObject's Reset_Database_Values method
            '    _VersionAndLobInfo.PolicyLevelInfoExtended.PolicyId = ""
            '    _VersionAndLobInfo.PolicyLevelInfoExtended.PolicyImageNum = ""
            'End If
        End Sub
        Protected Friend Sub Set_QuoteTransactionType(ByVal qqTranType As QuickQuoteObject.QuickQuoteTransactionType, Optional ByVal setTransactionTypeIdForNewBusinessQuoteAndEndorsement As Boolean = False)
            _QuoteTransactionType = qqTranType
            'VersionAndLobInfo.Set_QuoteTransactionType(_QuoteTransactionType) 'this is still being done from same method on QuickQuoteObject
            If setTransactionTypeIdForNewBusinessQuoteAndEndorsement = True Then
                'note: ReadOnlyImage should not reset anything
                If _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote Then
                    _TransactionTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.TransactionTypeId, "New Business") '2 for Diamond
                ElseIf _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                    _TransactionTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.TransactionTypeId, "Endorsement") '3 for Diamond
                End If
            End If
        End Sub
        Protected Friend Sub Set_OriginalEffectiveDate(ByVal origEffDate As String)
            _OriginalEffectiveDate = origEffDate
        End Sub
        Protected Friend Sub Set_OriginalExpirationDate(ByVal origExpDate As String)
            _OriginalExpirationDate = origExpDate
        End Sub
        Protected Friend Sub Set_OriginalTransactionEffectiveDate(ByVal origTEffDate As String)
            _OriginalTransactionEffectiveDate = origTEffDate
        End Sub
        Protected Friend Sub Set_OriginalTransactionExpirationDate(ByVal origTExpDate As String)
            _OriginalTransactionExpirationDate = origTExpDate
        End Sub
        Protected Friend Sub Default_PolicyOriginTypeId()
            _PolicyOriginTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyOriginTypeId, "VelociRater")
            If qqHelper.IsNumericString(_PolicyOriginTypeId) = False Then 'added just in case static data file isn't updated in environment
                _PolicyOriginTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyOriginTypeId, "Comparative Rating")
                If qqHelper.IsNumericString(_PolicyOriginTypeId) = False Then 'added just in case static data file hasn't been updated w/ change from Web to Comparative Rating
                    _PolicyOriginTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyOriginTypeId, "Web")
                End If
            End If
        End Sub

        Protected Friend Sub Set_CurrentlyParsingPaymentOptions(ByVal currentlyParsing As Boolean)
            _CurrentlyParsingPaymentOptions = currentlyParsing
        End Sub

        Protected Friend Function Get_PaymentOptions_Variable() As List(Of QuickQuotePaymentOption)
            Return _PaymentOptions
        End Function
        Protected Friend Sub NumberPaymentOptionsIfPresent()
            If _PaymentOptions IsNot Nothing AndAlso _PaymentOptions.Count > 0 Then
                qqHelper.NumberPaymentOptions(_PaymentOptions)
            End If
        End Sub

        'Protected Friend Sub Set_QuoteLevel(ByVal level As QuickQuoteHelperClass.QuoteLevel) 'added 7/28/2018; 12/30/2018 - moved to StateAndLobParts object so it would not get Copied between topLevel and stateLevel quotes
        '    _QuoteLevel = level
        'End Sub

        'added 9/28/2021
        Protected Friend Sub Set_InitialBillingPayPlanIdAtRetrieval(ByVal id As String)
            _InitialBillingPayPlanIdAtRetrieval = id
        End Sub
        Protected Friend Sub Set_InitialBillingPayPlanStaticDataOptionAtRetrieval(ByVal sdo As QuickQuoteStaticDataOption)
            _InitialBillingPayPlanStaticDataOptionAtRetrieval = sdo
        End Sub
        Protected Friend Sub Set_ConvertedBillingPayPlanIdAtRetrieval(ByVal id As String)
            _ConvertedBillingPayPlanIdAtRetrieval = id
        End Sub
        Protected Friend Sub Set_ConvertedBillingPayPlanStaticDataOptionAtRetrieval(ByVal sdo As QuickQuoteStaticDataOption)
            _ConvertedBillingPayPlanStaticDataOptionAtRetrieval = sdo
        End Sub
        Protected Friend Sub Set_InitialCurrentPayPlanIdAtRetrieval(ByVal id As String)
            _InitialCurrentPayPlanIdAtRetrieval = id
        End Sub
        Protected Friend Sub Set_InitialCurrentPayPlanStaticDataOptionAtRetrieval(ByVal sdo As QuickQuoteStaticDataOption)
            _InitialCurrentPayPlanStaticDataOptionAtRetrieval = sdo
        End Sub
        Protected Friend Sub Set_ConvertedCurrentPayPlanIdAtRetrieval(ByVal id As String)
            id = _ConvertedCurrentPayPlanIdAtRetrieval
        End Sub
        Protected Friend Sub Set_ConvertedCurrentPayPlanStaticDataOptionAtRetrieval(ByVal sdo As QuickQuoteStaticDataOption)
            sdo = _ConvertedCurrentPayPlanStaticDataOptionAtRetrieval
        End Sub
        Protected Friend Sub Set_UseBillingPayPlanIdConvertedAtSave(ByVal useIt As Boolean)
            _UseBillingPayPlanIdConvertedAtSave = useIt
        End Sub
        Protected Friend Sub Set_BillingPayPlanIdConvertedAtSave(ByVal id As String)
            _BillingPayPlanIdConvertedAtSave = id
        End Sub
        Protected Friend Sub Set_UseCurrentPayPlanIdConvertedAtSave(ByVal useIt As Boolean)
            _UseCurrentPayPlanIdConvertedAtSave = useIt
        End Sub
        Protected Friend Sub Set_CurrentPayPlanIdConvertedAtSave(id As String)
            _CurrentPayPlanIdConvertedAtSave = id
        End Sub


        Public Overrides Function ToString() As String
            Dim str As String = ""
            If Me IsNot Nothing Then

                'If Me.QuoteNumber <> "" Then
                '    str = qqHelper.appendText(str, "QuoteNumber: " & Me.QuoteNumber, vbCrLf)
                'End If
                'updated 10/30/2016
                Dim qNumToUse As String = Me.QuoteNumber
                Dim pNumToUse As String = Me.PolicyNumber
                If qNumToUse <> "" OrElse pNumToUse <> "" Then
                    'write quoteNumber and/or policyNumber
                    If qNumToUse <> "" AndAlso pNumToUse <> "" Then
                        If qNumToUse = pNumToUse Then
                            If UCase(Left(pNumToUse, 1)) = "Q" Then
                                str = qqHelper.appendText(str, "QuoteNumber: " & qNumToUse, vbCrLf)
                            Else
                                str = qqHelper.appendText(str, "PolicyNumber: " & pNumToUse, vbCrLf)
                            End If
                        Else
                            str = qqHelper.appendText(str, "PolicyNumber: " & pNumToUse, vbCrLf)
                            str = qqHelper.appendText(str, "QuoteNumber: " & qNumToUse, vbCrLf)
                        End If
                    ElseIf qNumToUse <> "" Then
                        str = qqHelper.appendText(str, "QuoteNumber: " & qNumToUse, vbCrLf)
                    ElseIf pNumToUse <> "" Then 'could just use ELSE since it shouldn't get here unless pNumToUse is something
                        str = qqHelper.appendText(str, "PolicyNumber: " & pNumToUse, vbCrLf)
                    End If
                End If
                If Me.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.None Then 'added 10/28/2016
                    str = qqHelper.appendText(str, "QuoteTransactionType: " & System.Enum.GetName(GetType(QuickQuoteObject.QuickQuoteTransactionType), Me.QuoteTransactionType), vbCrLf)
                End If

                If Me.QuoteDescription <> "" Then
                    str = qqHelper.appendText(str, "QuoteDescription: " & Me.QuoteDescription, vbCrLf)
                End If
            Else
                str = "Nothing"
            End If
            Return str
        End Function



#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).

                    _XmlType = Nothing
                    'qqHelper.DisposeString(_CompanyId) 'removed 11/26/2022 (now on VersionAndLobInfo)
                    _Success = Nothing
                    qqHelper.DisposeString(_AgencyCode)
                    qqHelper.DisposeString(_AgencyId)
                    qqHelper.DisposeString(_AgencyProducerId)
                    qqHelper.DisposeString(_AgencyProducerCode)
                    If _AgencyProducerName IsNot Nothing Then
                        _AgencyProducerName.Dispose()
                        _AgencyProducerName = Nothing
                    End If
                    qqHelper.DisposeString(_QuoteNumber)
                    qqHelper.DisposeString(_PolicyNumber)
                    qqHelper.DisposeString(_QuoteDescription)
                    qqHelper.DisposeString(_EffectiveDate)
                    qqHelper.DisposeString(_ExpirationDate)
                    If _ValidationItems IsNot Nothing Then
                        If _ValidationItems.Count > 0 Then
                            For Each val As QuickQuoteValidationItem In _ValidationItems
                                val.Dispose()
                                val = Nothing
                            Next
                            _ValidationItems.Clear()
                        End If
                        _ValidationItems = Nothing
                    End If
                    If _Client IsNot Nothing Then
                        _Client.Dispose()
                        _Client = Nothing
                    End If
                    _IsNew = Nothing
                    qqHelper.DisposeString(_BillToId)
                    qqHelper.DisposeString(_CurrentBilltoId)
                    qqHelper.DisposeString(_CurrentPayplanId)
                    qqHelper.DisposeString(_PolicyTermId)
                    qqHelper.DisposeString(_ReceivedDate)
                    qqHelper.DisposeString(_TransactionEffectiveDate)
                    qqHelper.DisposeString(_TransactionExpirationDate)
                    qqHelper.DisposeString(_TransactionTypeId)
                    qqHelper.DisposeString(_TransactionUsersId)
                    qqHelper.DisposeString(_WorkflowQueueId)
                    If _Policyholder IsNot Nothing Then
                        _Policyholder.Dispose()
                        _Policyholder = Nothing
                    End If
                    If _Policyholder2 IsNot Nothing Then
                        _Policyholder2.Dispose()
                        _Policyholder2 = Nothing
                    End If
                    qqHelper.DisposeString(_BillMethodId)
                    qqHelper.DisposeString(_BillingPayPlanId)
                    qqHelper.DisposeString(_PolicyOriginTypeId)
                    _HasInitiatedFinalize = Nothing
                    qqHelper.DisposeString(_PolicyId)
                    qqHelper.DisposeString(_PolicyImageNum)
                    qqHelper.DisposeString(_RenewalVersion)
                    qqHelper.DisposeString(_PolicyBridgingURL)
                    If _PaymentOptions IsNot Nothing Then
                        If _PaymentOptions.Count > 0 Then
                            For Each po As QuickQuotePaymentOption In _PaymentOptions
                                po.Dispose()
                                po = Nothing
                            Next
                            _PaymentOptions.Clear()
                        End If
                        _PaymentOptions = Nothing
                    End If
                    _CurrentlyParsingPaymentOptions = Nothing
                    qqHelper.DisposeString(_ExperienceModificationFactor)
                    qqHelper.DisposeString(_ExperienceModificationBureauTypeId)
                    qqHelper.DisposeString(_ExperienceModificationRiskIdentifier)
                    If _ExperienceModifications IsNot Nothing Then
                        If _ExperienceModifications.Count > 0 Then
                            For Each expMod As QuickQuoteExperienceModification In _ExperienceModifications
                                If expMod IsNot Nothing Then
                                    expMod.Dispose()
                                    expMod = Nothing
                                End If
                            Next
                            _ExperienceModifications.Clear()
                        End If
                        _ExperienceModifications = Nothing
                    End If
                    _CanUseExperienceModificationNumForExperienceModificationReconciliation = Nothing
                    _HasConvertedExperienceModifications = Nothing
                    qqHelper.DisposeIntegers(_DiamondExperienceModificationIndexesToUpdate)
                    qqHelper.DisposeString(_GuaranteedRatePeriodEffectiveDate)
                    qqHelper.DisposeString(_GuaranteedRatePeriodExpirationDate)
                    qqHelper.DisposeString(_ModificationProductionDate)
                    qqHelper.DisposeString(_RatingEffectiveDate)
                    If _AdditionalPolicyholders IsNot Nothing Then
                        If _AdditionalPolicyholders.Count > 0 Then
                            For Each ph As QuickQuoteAdditionalPolicyholder In _AdditionalPolicyholders
                                ph.Dispose()
                                ph = Nothing
                            Next
                            _AdditionalPolicyholders.Clear()
                        End If
                        _AdditionalPolicyholders = Nothing
                    End If
                    qqHelper.DisposeString(_QuoteTypeId)
                    qqHelper.DisposeString(_EFT_BankRoutingNumber)
                    qqHelper.DisposeString(_EFT_BankAccountNumber)
                    qqHelper.DisposeString(_EFT_BankAccountTypeId)
                    qqHelper.DisposeString(_EFT_DeductionDay)
                    _OnlyUsePropertyToSetFieldWithSameName = Nothing
                    If _Agency IsNot Nothing Then
                        _Agency.Dispose()
                        _Agency = Nothing
                    End If
                    qqHelper.DisposeString(_PolicyImageId)
                    If _BillingAddressee IsNot Nothing Then
                        _BillingAddressee.Dispose()
                        _BillingAddressee = Nothing
                    End If
                    qqHelper.DisposeString(_FirstWrittenDate)
                    _QuoteTransactionType = Nothing
                    qqHelper.DisposeString(_OriginalEffectiveDate)
                    qqHelper.DisposeString(_OriginalExpirationDate)
                    qqHelper.DisposeString(_OriginalTransactionEffectiveDate)
                    qqHelper.DisposeString(_OriginalTransactionExpirationDate)
                    qqHelper.DisposeString(_TransactionRemark)
                    qqHelper.DisposeString(_TransactionReasonId)
                    If _Comments IsNot Nothing Then
                        If _Comments.Count > 0 Then
                            For Each c As QuickQuoteComment In _Comments
                                If c IsNot Nothing Then
                                    c.Dispose()
                                    c = Nothing
                                End If
                            Next
                            _Comments.Clear()
                        End If
                        _Comments = Nothing
                    End If
                    qqHelper.DisposeMessages(_Messages)

                    '_QuoteLevel = Nothing '12/30/2018 - moved to StateAndLobParts object so it would not get Copied between topLevel and stateLevel quotes

                    'added 5/22/2019
                    qqHelper.DisposeString(_AddedDate)
                    qqHelper.DisposeString(_LastModifiedDate)
                    qqHelper.DisposeString(_PCAdded_Date)

                    'added 6/15/2019
                    qqHelper.DisposeString(_PolicyCurrentStatusId)
                    qqHelper.DisposeString(_PolicyStatusCodeId)
                    qqHelper.DisposeString(_CancelDate)

                    'added 9/28/2021
                    qqHelper.DisposeString(_InitialBillingPayPlanIdAtRetrieval)
                    If _InitialBillingPayPlanStaticDataOptionAtRetrieval IsNot Nothing Then
                        _InitialBillingPayPlanStaticDataOptionAtRetrieval = Nothing
                    End If
                    qqHelper.DisposeString(_ConvertedBillingPayPlanIdAtRetrieval)
                    If _ConvertedBillingPayPlanStaticDataOptionAtRetrieval IsNot Nothing Then
                        _ConvertedBillingPayPlanStaticDataOptionAtRetrieval = Nothing
                    End If
                    qqHelper.DisposeString(_InitialCurrentPayPlanIdAtRetrieval)
                    If _InitialCurrentPayPlanStaticDataOptionAtRetrieval IsNot Nothing Then
                        _InitialCurrentPayPlanStaticDataOptionAtRetrieval = Nothing
                    End If
                    qqHelper.DisposeString(_ConvertedCurrentPayPlanIdAtRetrieval)
                    If _ConvertedCurrentPayPlanStaticDataOptionAtRetrieval IsNot Nothing Then
                        _ConvertedCurrentPayPlanStaticDataOptionAtRetrieval = Nothing
                    End If
                    _UseBillingPayPlanIdConvertedAtSave = Nothing
                    qqHelper.DisposeString(_BillingPayPlanIdConvertedAtSave)
                    _UseCurrentPayPlanIdConvertedAtSave = Nothing
                    qqHelper.DisposeString(_CurrentPayPlanIdConvertedAtSave)

                    MyBase.Dispose()
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
        'Public Sub Dispose() Implements IDisposable.Dispose
        'updated  w/ QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
