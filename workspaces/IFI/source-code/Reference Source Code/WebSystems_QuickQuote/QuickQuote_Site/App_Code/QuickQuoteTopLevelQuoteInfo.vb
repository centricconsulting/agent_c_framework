Imports Microsoft.VisualBasic
Imports System.Web
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store top-level information for a quote; includes properties that were previously on QuickQuote only
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteTopLevelQuoteInfo 'added 7/26/2018
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        'Private _XmlType As QuickQuoteObject.QuickQuoteXmlType

        'Private _CompanyId As String

        'Private _Success As Boolean

        'Private _AgencyCode As String
        'Private _AgencyId As String

        'Private _AgencyProducerId As String
        'Private _AgencyProducerCode As String
        'Private _AgencyProducerName As QuickQuoteName

        'Private _QuoteNumber As String
        'Private _PolicyNumber As String
        'Private _QuoteDescription As String
        'Private _EffectiveDate As String
        'Private _ExpirationDate As String
        'Private _TotalQuotedPremium As String

        'Private _ValidationItems As Generic.List(Of QuickQuoteValidationItem)

        'Private _Client As QuickQuoteClient

        'Private _IsNew As Boolean
        'Private _BillToId As String
        'Private _CurrentBilltoId As String
        'Private _CurrentPayplanId As String
        'Private _PolicyTermId As String
        'Private _ReceivedDate As String
        'Private _TransactionEffectiveDate As String
        'Private _TransactionExpirationDate As String
        'Private _TransactionTypeId As String
        'Private _TransactionUsersId As String

        'Private _WorkflowQueueId As String

        'Private _Policyholder As QuickQuotePolicyholder
        'Private _Policyholder2 As QuickQuotePolicyholder

        'Private _BillMethodId As String
        'Private _BillingPayPlanId As String

        'Private _PolicyOriginTypeId As String

        'Private _HasInitiatedFinalize As Boolean

        'Private _PolicyId As String
        'Private _PolicyImageNum As String

        'Private _PolicyBridgingURL As String

        'Private _PaymentOptions As Generic.List(Of QuickQuotePaymentOption)
        'Private _CurrentlyParsingPaymentOptions As Boolean

        'Private _ExperienceModificationFactor As String
        'Private _ExperienceModificationBureauTypeId As String '-1 = , 0 = N/A, 1 = NCCI
        'Private _ExperienceModificationRiskIdentifier As String
        'Private _ExperienceModifications As List(Of QuickQuoteExperienceModification)
        'Private _CanUseExperienceModificationNumForExperienceModificationReconciliation As Boolean
        'Private _HasConvertedExperienceModifications As Boolean
        'Private _DiamondExperienceModificationIndexesToUpdate As List(Of Integer)

        'Private _GuaranteedRatePeriodEffectiveDate As String
        'Private _GuaranteedRatePeriodExpirationDate As String
        'Private _ModificationProductionDate As String
        'Private _RatingEffectiveDate As String

        'Private _AdditionalPolicyholders As Generic.List(Of QuickQuoteAdditionalPolicyholder)

        'Private _QuoteTypeId As String '0 = N/A; 1 = Quick Quote; 2 = New Application; 3 = Finalize Quote

        'Private _PackageParts As Generic.List(Of QuickQuotePackagePart)

        'Private _EFT_BankRoutingNumber As String
        'Private _EFT_BankAccountNumber As String
        'Private _EFT_BankAccountTypeId As String
        'Private _EFT_DeductionDay As String

        'Private _OnlyUsePropertyToSetFieldWithSameName As Boolean

        'Private _Database_QuoteId As String
        'Private _Database_QuoteXmlId As String
        'Private _Database_QuoteNumber As String
        'Private _Database_LobId As String
        'Private _Database_CurrentQuoteXmlId As String
        'Private _Database_XmlQuoteId As String
        'Private _Database_LastAvailableQuoteNumber As String
        'Private _Database_QuoteStatusId As String
        'Private _Database_XmlStatusId As String
        'Private _Database_IsPolicy As Boolean
        'Private _Database_DiamondPolicyNumber As String
        'Private _Database_OriginatedInVR As Boolean
        'Private _Database_EffectiveDate As String

        'Private _Agency As QuickQuoteAgency

        'Private _PolicyImageId As String

        'Private _BillingAddressee As QuickQuoteBillingAddressee

        'Private _FirstWrittenDate As String

        'Private _QuoteTransactionType As QuickQuoteObject.QuickQuoteTransactionType
        'Private _OriginalEffectiveDate As String
        'Private _OriginalExpirationDate As String
        'Private _OriginalTransactionEffectiveDate As String
        'Private _OriginalTransactionExpirationDate As String

        'Private _TransactionRemark As String
        'Private _TransactionReasonId As String

        'Private _AnnualPremium As String 'PolicyImage.premium_annual
        'Private _ChangeInFullTermPremium As String 'PolicyImage.premium_chg_fullterm
        'Private _ChangeInWrittenPremium As String 'PolicyImage.premium_chg_written
        'Private _DifferenceChangeInFullTermPremium As String 'PolicyImage.premium_diff_chg_fullterm
        'Private _DifferenceChangeInWrittenPremium As String 'PolicyImage.premium_diff_chg_written
        'Private _FullTermPremium As String 'PolicyImage.premium_fullterm
        'Private _FullTermPremiumOffsetForPreviousImage As String 'PolicyImage.ftp_offset_for_prev_image
        'Private _FullTermPremiumOnsetForCurrent As String 'PolicyImage.ftp_onset_for_current
        'Private _OffsetPremiumForPreviousImage As String 'PolicyImage.offset_for_prev_image
        'Private _OnsetPremiumForCurrentImage As String 'PolicyImage.onset_for_current
        'Private _PreviousWrittenPremium As String 'PolicyImage.premium_previous_written
        'Private _WrittenPremium As String 'PolicyImage.premium_written
        'Private _PriorTermAnnual As String 'PolicyImage.prior_term_annual_premium
        'Private _PriorTermFullterm As String 'PolicyImage.prior_term_fullterm

        'Private _Comments As List(Of QuickQuoteComment)

        'Private _Messages As List(Of QuickQuoteMessage)

        'Private _MultiStateQuotes As List(Of QuickQuoteObject)

        'added 7/27/2018
        Private _QuoteBase As QuickQuoteTopLevelQuoteBase
        Private _QuotePremiums As QuickQuoteTopLevelQuotePremiums
        Private _StateAndLobParts As QuickQuoteTopLevelStateAndLobParts


        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property XmlType As QuickQuoteObject.QuickQuoteXmlType
            Get
                Return QuoteBase.XmlType
            End Get
            Set(value As QuickQuoteObject.QuickQuoteXmlType)
                QuoteBase.XmlType = value
            End Set
        End Property
        '<Script.Serialization.ScriptIgnore>
        '<System.Xml.Serialization.XmlIgnore()>
        'Public Property CompanyId As String 'removed 11/26/2022 (now on VersionAndLobInfo)
        '    Get
        '        Return QuoteBase.CompanyId
        '    End Get
        '    Set(value As String)
        '        QuoteBase.CompanyId = value
        '    End Set
        'End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Success As Boolean
            Get
                Return QuoteBase.Success
            End Get
            Set(value As Boolean)
                QuoteBase.Success = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AgencyCode As String
            Get
                Return QuoteBase.AgencyCode
            End Get
            Set(value As String)
                QuoteBase.AgencyCode = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AgencyId As String
            Get
                Return QuoteBase.AgencyId
            End Get
            Set(value As String)
                QuoteBase.AgencyId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AgencyProducerId As String
            Get
                Return QuoteBase.AgencyProducerId
            End Get
            Set(value As String)
                QuoteBase.AgencyProducerId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AgencyProducerCode As String
            Get
                Return QuoteBase.AgencyProducerCode
            End Get
            Set(value As String)
                QuoteBase.AgencyProducerCode = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AgencyProducerName As QuickQuoteName
            Get
                Return QuoteBase.AgencyProducerName
            End Get
            Set(value As QuickQuoteName)
                QuoteBase.AgencyProducerName = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property QuoteNumber As String
            Get
                'Dim quoteNumToReturn As String = _QuoteNumber
                'If String.IsNullOrWhiteSpace(quoteNumToReturn) = True AndAlso String.IsNullOrWhiteSpace(_PolicyNumber) = False AndAlso UCase(Left(_PolicyNumber, 1)) = "Q" Then
                '    quoteNumToReturn = _PolicyNumber
                'End If
                'Return quoteNumToReturn
                Return QuoteBase.QuoteNumber
            End Get
            Set(value As String)
                QuoteBase.QuoteNumber = value
            End Set
        End Property
        Protected Friend ReadOnly Property QuoteNumber_Actual As String
            Get
                Return QuoteBase.QuoteNumber_Actual
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyNumber As String
            Get
                'Dim polNumToReturn As String = _PolicyNumber
                'If String.IsNullOrWhiteSpace(polNumToReturn) = True AndAlso String.IsNullOrWhiteSpace(_QuoteNumber) = False Then
                '    polNumToReturn = _QuoteNumber
                'End If
                'Return polNumToReturn
                Return QuoteBase.PolicyNumber
            End Get
            Set(value As String)
                QuoteBase.PolicyNumber = value
            End Set
        End Property
        Protected Friend ReadOnly Property PolicyNumber_Actual As String
            Get
                Return QuoteBase.PolicyNumber_Actual
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property QuoteDescription As String
            Get
                Return QuoteBase.QuoteDescription
            End Get
            Set(value As String)
                QuoteBase.QuoteDescription = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property EffectiveDate As String
            Get
                Return QuoteBase.EffectiveDate
            End Get
            Set(value As String)
                'Dim oldEffectiveDate As String = _EffectiveDate
                '_EffectiveDate = value
                ''VersionAndLobInfo.Set_QuoteEffectiveDate(_EffectiveDate) 'still being called from QuickQuoteObject Property
                'qqHelper.ConvertToShortDate(_EffectiveDate)

                'If _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                '    Exit Property
                'End If

                'If IsDate(_EffectiveDate) = True Then
                '    ExpirationDate = DateAdd(DateInterval.Year, 1, CDate(_EffectiveDate)).ToString
                '    _TransactionEffectiveDate = _EffectiveDate
                '    '_TransactionExpirationDate = _ExpirationDate 'should automatically happen when above logic sets ExpirationDate property
                '    _GuaranteedRatePeriodEffectiveDate = _EffectiveDate
                '    '_GuaranteedRatePeriodExpirationDate = _ExpirationDate 'should automatically happen when above logic sets ExpirationDate property

                '    If _FirstWrittenDate = "" OrElse IsDate(_FirstWrittenDate) = False Then
                '        'blank or invalid
                '        _FirstWrittenDate = _EffectiveDate
                '    Else
                '        'valid date
                '        If CDate(_FirstWrittenDate) > CDate(_EffectiveDate) Then
                '            'FWD cannot be after effDate
                '            _FirstWrittenDate = _EffectiveDate
                '        Else
                '            'FWD is either the same or older than new effDate
                '            If oldEffectiveDate = "" OrElse IsDate(oldEffectiveDate) = False Then
                '                'blank or invalid previous effDate

                '            Else
                '                'valid previous effDate
                '                If CDate(_FirstWrittenDate) = CDate(oldEffectiveDate) Then
                '                    _FirstWrittenDate = _EffectiveDate
                '                End If
                '            End If
                '        End If
                '    End If
                'End If
                QuoteBase.EffectiveDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ExpirationDate As String
            Get
                Return QuoteBase.ExpirationDate
            End Get
            Set(value As String)
                '_ExpirationDate = value
                'qqHelper.ConvertToShortDate(_ExpirationDate)

                'If _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                '    Exit Property
                'End If

                'If IsDate(_ExpirationDate) = True Then
                '    _GuaranteedRatePeriodExpirationDate = _ExpirationDate
                '    _TransactionExpirationDate = _ExpirationDate
                'End If
                QuoteBase.ExpirationDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TotalQuotedPremium As String
            Get
                Return QuotePremiums.TotalQuotedPremium
            End Get
            Set(value As String)
                QuotePremiums.TotalQuotedPremium = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ValidationItems As Generic.List(Of QuickQuoteValidationItem)
            Get
                Return QuoteBase.ValidationItems
            End Get
            Set(value As Generic.List(Of QuickQuoteValidationItem))
                QuoteBase.ValidationItems = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Client As QuickQuoteClient
            Get
                Return QuoteBase.Client
            End Get
            Set(value As QuickQuoteClient)
                QuoteBase.Client = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property IsNew As Boolean
            Get
                Return QuoteBase.IsNew
            End Get
            Set(value As Boolean)
                QuoteBase.IsNew = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property BillToId As String
            Get
                Return QuoteBase.BillToId
            End Get
            Set(value As String)
                '_BillToId = value
                ''sets both when set by developer; only sets 1 when read from xml
                'If _OnlyUsePropertyToSetFieldWithSameName = False Then
                '    _CurrentBilltoId = value
                'Else 'don't set other value; set flag back to False after
                '    _OnlyUsePropertyToSetFieldWithSameName = False
                'End If
                QuoteBase.BillToId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CurrentBilltoId As String
            Get
                Return QuoteBase.CurrentBilltoId
            End Get
            Set(value As String)
                '_CurrentBilltoId = value
                ''sets both when set by developer; only sets 1 when read from xml
                'If _OnlyUsePropertyToSetFieldWithSameName = False Then
                '    _BillToId = value
                'Else 'don't set other value; set flag back to False after
                '    _OnlyUsePropertyToSetFieldWithSameName = False
                'End If
                QuoteBase.CurrentBilltoId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CurrentPayplanId As String 'triggered off this instead of BillingPayPlanId
            Get
                Return QuoteBase.CurrentPayplanId
            End Get
            Set(value As String)
                '_CurrentPayplanId = value
                ''sets both when set by developer; only sets 1 when read from xml
                'If _OnlyUsePropertyToSetFieldWithSameName = False Then
                '    _BillingPayPlanId = value
                'Else 'don't set other value; set flag back to False after
                '    _OnlyUsePropertyToSetFieldWithSameName = False
                'End If
                QuoteBase.CurrentPayplanId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyTermId As String
            Get
                Return QuoteBase.PolicyTermId
            End Get
            Set(value As String)
                QuoteBase.PolicyTermId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ReceivedDate As String
            Get
                Return QuoteBase.ReceivedDate
            End Get
            Set(value As String)
                '_ReceivedDate = value
                'qqHelper.ConvertToShortDate(_ReceivedDate)
                QuoteBase.ReceivedDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TransactionEffectiveDate As String
            Get
                Return QuoteBase.TransactionEffectiveDate
            End Get
            Set(value As String)
                'If _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                '    _TransactionEffectiveDate = value
                '    qqHelper.ConvertToShortDate(_TransactionEffectiveDate)
                '    Exit Property
                'End If

                'If _TransactionEffectiveDate = "" OrElse IsDate(_TransactionEffectiveDate) = False OrElse _EffectiveDate = "" OrElse IsDate(_EffectiveDate) = False Then
                '    _TransactionEffectiveDate = value
                '    qqHelper.ConvertToShortDate(_TransactionEffectiveDate)
                '    If IsDate(_TransactionEffectiveDate) = True Then
                '        _TransactionExpirationDate = DateAdd(DateInterval.Year, 1, CDate(_TransactionEffectiveDate)).ToString
                '        qqHelper.ConvertToShortDate(_TransactionExpirationDate)
                '    End If
                'End If
                QuoteBase.TransactionEffectiveDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TransactionExpirationDate As String
            Get
                Return QuoteBase.TransactionExpirationDate
            End Get
            Set(value As String)
                'If _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                '    _TransactionExpirationDate = value
                '    qqHelper.ConvertToShortDate(_TransactionExpirationDate)
                '    Exit Property
                'End If

                'If _TransactionExpirationDate = "" OrElse IsDate(_TransactionExpirationDate) = False OrElse _ExpirationDate = "" OrElse IsDate(_ExpirationDate) = False Then
                '    _TransactionExpirationDate = value
                '    qqHelper.ConvertToShortDate(_TransactionExpirationDate)
                'End If
                QuoteBase.TransactionExpirationDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TransactionTypeId As String
            Get
                Return QuoteBase.TransactionTypeId
            End Get
            Set(value As String)
                QuoteBase.TransactionTypeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TransactionUsersId As String
            Get
                Return QuoteBase.TransactionUsersId
            End Get
            Set(value As String)
                QuoteBase.TransactionUsersId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property WorkflowQueueId As String
            Get
                Return QuoteBase.WorkflowQueueId
            End Get
            Set(value As String)
                QuoteBase.WorkflowQueueId = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Policyholder As QuickQuotePolicyholder
            Get
                Return QuoteBase.Policyholder
            End Get
            Set(value As QuickQuotePolicyholder)
                QuoteBase.Policyholder = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Policyholder2 As QuickQuotePolicyholder
            Get
                Return QuoteBase.Policyholder2
            End Get
            Set(value As QuickQuotePolicyholder)
                QuoteBase.Policyholder2 = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property BillMethodId As String
            Get
                Return QuoteBase.BillMethodId
            End Get
            Set(value As String)
                QuoteBase.BillMethodId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property BillingPayPlanId As String 'triggered off CurrentPayPlanId instead of this one
            Get
                Return QuoteBase.BillingPayPlanId
            End Get
            Set(value As String)
                '_BillingPayPlanId = value
                ''sets both when set by developer; only sets 1 when read from xml
                'If _OnlyUsePropertyToSetFieldWithSameName = False Then
                '    _CurrentPayplanId = value
                'Else 'don't set other value; set flag back to False after
                '    _OnlyUsePropertyToSetFieldWithSameName = False
                'End If
                QuoteBase.BillingPayPlanId = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyOriginTypeId As String
            Get
                Return QuoteBase.PolicyOriginTypeId
            End Get
            Set(value As String)
                QuoteBase.PolicyOriginTypeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property HasInitiatedFinalize As Boolean
            Get
                Return QuoteBase.HasInitiatedFinalize
            End Get
            Set(value As Boolean)
                QuoteBase.HasInitiatedFinalize = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyId As String
            Get
                Return QuoteBase.PolicyId
            End Get
            Set(value As String)
                QuoteBase.PolicyId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyImageNum As String
            Get
                Return QuoteBase.PolicyImageNum
            End Get
            Set(value As String)
                QuoteBase.PolicyImageNum = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property RenewalVersion As String
            Get
                Return QuoteBase.RenewalVersion
            End Get
            Set(value As String)
                QuoteBase.RenewalVersion = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyBridgingURL As String
            Get
                Return QuoteBase.PolicyBridgingURL
            End Get
            Set(value As String)
                QuoteBase.PolicyBridgingURL = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PaymentOptions As Generic.List(Of QuickQuotePaymentOption)
            Get
                'If _CurrentlyParsingPaymentOptions = False AndAlso (_PaymentOptions Is Nothing OrElse _PaymentOptions.Count = 0) AndAlso qqHelper.IsPositiveIntegerString(_PolicyId) = True AndAlso qqHelper.IsPositiveIntegerString(_PolicyImageNum) = True Then
                '    Dim img As Diamond.Common.Objects.Policy.Image = QuickQuoteHelperClass.GetPolicyImage(_PolicyId, _PolicyImageNum)
                '    If img IsNot Nothing Then
                '        Dim strPayplanPreviewsXml As String = ""
                '        Dim qqXml As New QuickQuoteXML
                '        qqXml.DiamondService_LoadPaymentOptions(img, strPayplanPreviewsXml)
                '        If strPayplanPreviewsXml <> "" Then
                '            qqXml.ParseArrayOfPayPlanPreviewXmlString(strPayplanPreviewsXml, _PaymentOptions)

                '            qqHelper.NumberPaymentOptions(_PaymentOptions)
                '        End If
                '    End If
                'End If
                'Return _PaymentOptions
                Return QuoteBase.PaymentOptions
            End Get
            Set(value As Generic.List(Of QuickQuotePaymentOption))
                QuoteBase.PaymentOptions = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ExperienceModificationFactor As String
            Get
                Return QuoteBase.ExperienceModificationFactor
            End Get
            Set(value As String)
                QuoteBase.ExperienceModificationFactor = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ExperienceModificationBureauTypeId As String
            Get
                Return QuoteBase.ExperienceModificationBureauTypeId
            End Get
            Set(value As String)
                QuoteBase.ExperienceModificationBureauTypeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ExperienceModificationRiskIdentifier As String
            Get
                Return QuoteBase.ExperienceModificationRiskIdentifier
            End Get
            Set(value As String)
                QuoteBase.ExperienceModificationRiskIdentifier = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ExperienceModifications As List(Of QuickQuoteExperienceModification)
            Get
                Return QuoteBase.ExperienceModifications
            End Get
            Set(value As List(Of QuickQuoteExperienceModification))
                QuoteBase.ExperienceModifications = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CanUseExperienceModificationNumForExperienceModificationReconciliation As Boolean
            Get
                Return QuoteBase.CanUseExperienceModificationNumForExperienceModificationReconciliation
            End Get
            Set(value As Boolean)
                QuoteBase.CanUseExperienceModificationNumForExperienceModificationReconciliation = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property HasConvertedExperienceModifications As Boolean
            Get
                Return QuoteBase.HasConvertedExperienceModifications
            End Get
            Set(value As Boolean)
                QuoteBase.HasConvertedExperienceModifications = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Protected Friend Property DiamondExperienceModificationIndexesToUpdate As List(Of Integer)
            Get
                Return QuoteBase.DiamondExperienceModificationIndexesToUpdate
            End Get
            Set(value As List(Of Integer))
                QuoteBase.DiamondExperienceModificationIndexesToUpdate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property GuaranteedRatePeriodEffectiveDate As String
            Get
                Return QuoteBase.GuaranteedRatePeriodEffectiveDate
            End Get
            Set(value As String)
                'If _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                '    _GuaranteedRatePeriodEffectiveDate = value
                '    qqHelper.ConvertToShortDate(_GuaranteedRatePeriodEffectiveDate)
                '    Exit Property
                'End If

                'If _GuaranteedRatePeriodEffectiveDate = "" OrElse IsDate(_GuaranteedRatePeriodEffectiveDate) = False OrElse _EffectiveDate = "" OrElse IsDate(_EffectiveDate) = False Then
                '    _GuaranteedRatePeriodEffectiveDate = value
                '    qqHelper.ConvertToShortDate(_GuaranteedRatePeriodEffectiveDate)
                '    If IsDate(_GuaranteedRatePeriodEffectiveDate) = True Then
                '        _GuaranteedRatePeriodExpirationDate = DateAdd(DateInterval.Year, 1, CDate(_GuaranteedRatePeriodEffectiveDate)).ToString
                '        qqHelper.ConvertToShortDate(_GuaranteedRatePeriodExpirationDate)
                '    End If
                'End If
                QuoteBase.GuaranteedRatePeriodEffectiveDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property GuaranteedRatePeriodExpirationDate As String
            Get
                Return QuoteBase.GuaranteedRatePeriodExpirationDate
            End Get
            Set(value As String)
                'If _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                '    _GuaranteedRatePeriodExpirationDate = value
                '    qqHelper.ConvertToShortDate(_GuaranteedRatePeriodExpirationDate)
                '    Exit Property
                'End If

                'If _GuaranteedRatePeriodExpirationDate = "" OrElse IsDate(_GuaranteedRatePeriodExpirationDate) = False OrElse _ExpirationDate = "" OrElse IsDate(_ExpirationDate) = False Then
                '    _GuaranteedRatePeriodExpirationDate = value
                '    qqHelper.ConvertToShortDate(_GuaranteedRatePeriodExpirationDate)
                'End If
                QuoteBase.GuaranteedRatePeriodExpirationDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ModificationProductionDate As String
            Get
                Return QuoteBase.ModificationProductionDate
            End Get
            Set(value As String)
                '_ModificationProductionDate = value
                'qqHelper.ConvertToShortDate(_ModificationProductionDate)
                QuoteBase.ModificationProductionDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property RatingEffectiveDate As String
            Get
                Return QuoteBase.RatingEffectiveDate
            End Get
            Set(value As String)
                '_RatingEffectiveDate = value
                'qqHelper.ConvertToShortDate(_RatingEffectiveDate)
                QuoteBase.RatingEffectiveDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AdditionalPolicyholders As Generic.List(Of QuickQuoteAdditionalPolicyholder)
            Get
                Return QuoteBase.AdditionalPolicyholders
            End Get
            Set(value As Generic.List(Of QuickQuoteAdditionalPolicyholder))
                QuoteBase.AdditionalPolicyholders = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property QuoteTypeId As String
            Get
                Return QuoteBase.QuoteTypeId
            End Get
            Set(value As String)
                QuoteBase.QuoteTypeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PackageParts As Generic.List(Of QuickQuotePackagePart)
            Get
                Return StateAndLobParts.PackageParts
            End Get
            Set(value As Generic.List(Of QuickQuotePackagePart))
                StateAndLobParts.PackageParts = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property EFT_BankRoutingNumber As String
            Get
                Return QuoteBase.EFT_BankRoutingNumber
            End Get
            Set(value As String)
                QuoteBase.EFT_BankRoutingNumber = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property EFT_BankAccountNumber As String
            Get
                Return QuoteBase.EFT_BankAccountNumber
            End Get
            Set(value As String)
                QuoteBase.EFT_BankAccountNumber = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property EFT_BankAccountTypeId As String '1=Checking; 2=Savings
            Get
                Return QuoteBase.EFT_BankAccountTypeId
            End Get
            Set(value As String)
                QuoteBase.EFT_BankAccountTypeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property EFT_DeductionDay As String
            Get
                Return QuoteBase.EFT_DeductionDay
            End Get
            Set(value As String)
                QuoteBase.EFT_DeductionDay = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property OnlyUsePropertyToSetFieldWithSameName As Boolean
            Get
                Return QuoteBase.OnlyUsePropertyToSetFieldWithSameName
            End Get
            Set(value As Boolean)
                QuoteBase.OnlyUsePropertyToSetFieldWithSameName = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuoteId As String
            Get
                Return QuoteBase.Database_QuoteId
            End Get
            Set(value As String)
                QuoteBase.Database_QuoteId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuoteXmlId As String
            Get
                Return QuoteBase.Database_QuoteXmlId
            End Get
            Set(value As String)
                QuoteBase.Database_QuoteXmlId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuoteNumber As String
            Get
                Return QuoteBase.Database_QuoteNumber
            End Get
            Set(value As String)
                QuoteBase.Database_QuoteNumber = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_LobId As String
            Get
                Return QuoteBase.Database_LobId
            End Get
            Set(value As String)
                QuoteBase.Database_LobId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_CurrentQuoteXmlId As String
            Get
                Return QuoteBase.Database_CurrentQuoteXmlId
            End Get
            Set(value As String)
                QuoteBase.Database_CurrentQuoteXmlId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_XmlQuoteId As String
            Get
                Return QuoteBase.Database_XmlQuoteId
            End Get
            Set(value As String)
                QuoteBase.Database_XmlQuoteId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_LastAvailableQuoteNumber As String
            Get
                Return QuoteBase.Database_LastAvailableQuoteNumber
            End Get
            Set(value As String)
                QuoteBase.Database_LastAvailableQuoteNumber = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuoteStatusId As String
            Get
                Return QuoteBase.Database_QuoteStatusId
            End Get
            Set(value As String)
                QuoteBase.Database_QuoteStatusId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_XmlStatusId As String
            Get
                Return QuoteBase.Database_XmlStatusId
            End Get
            Set(value As String)
                QuoteBase.Database_XmlStatusId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_IsPolicy As Boolean
            Get
                Return QuoteBase.Database_IsPolicy
            End Get
            Set(value As Boolean)
                QuoteBase.Database_IsPolicy = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_DiamondPolicyNumber As String
            Get
                Return QuoteBase.Database_DiamondPolicyNumber
            End Get
            Set(value As String)
                QuoteBase.Database_DiamondPolicyNumber = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_OriginatedInVR As Boolean
            Get
                Return QuoteBase.Database_OriginatedInVR
            End Get
            Set(value As Boolean)
                QuoteBase.Database_OriginatedInVR = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_EffectiveDate As String
            Get
                Return QuoteBase.Database_EffectiveDate
            End Get
            Set(value As String)
                QuoteBase.Database_EffectiveDate = value
            End Set
        End Property
        'added 9/28/2018
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_ActualLobId As String
            Get
                Return QuoteBase.Database_ActualLobId
            End Get
            Set(value As String)
                QuoteBase.Database_ActualLobId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_GoverningStateId As String
            Get
                Return QuoteBase.Database_GoverningStateId
            End Get
            Set(value As String)
                QuoteBase.Database_GoverningStateId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_StateIds As String
            Get
                Return QuoteBase.Database_StateIds
            End Get
            Set(value As String)
                QuoteBase.Database_StateIds = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuoteActualLobId As String
            Get
                Return QuoteBase.Database_QuoteActualLobId
            End Get
            Set(value As String)
                QuoteBase.Database_QuoteActualLobId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuoteGoverningStateId As String
            Get
                Return QuoteBase.Database_QuoteGoverningStateId
            End Get
            Set(value As String)
                QuoteBase.Database_QuoteGoverningStateId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuoteStateIds As String
            Get
                Return QuoteBase.Database_QuoteStateIds
            End Get
            Set(value As String)
                QuoteBase.Database_QuoteStateIds = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_AppActualLobId As String
            Get
                Return QuoteBase.Database_AppActualLobId
            End Get
            Set(value As String)
                QuoteBase.Database_AppActualLobId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_AppGoverningStateId As String
            Get
                Return QuoteBase.Database_AppGoverningStateId
            End Get
            Set(value As String)
                QuoteBase.Database_AppGoverningStateId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_AppStateIds As String
            Get
                Return QuoteBase.Database_AppStateIds
            End Get
            Set(value As String)
                QuoteBase.Database_AppStateIds = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Agency As QuickQuoteAgency
            Get
                Return QuoteBase.Agency
            End Get
            Set(value As QuickQuoteAgency)
                QuoteBase.Agency = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyImageId As String
            Get
                Return QuoteBase.PolicyImageId
            End Get
            Set(value As String)
                QuoteBase.PolicyImageId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property BillingAddressee As QuickQuoteBillingAddressee
            Get
                Return QuoteBase.BillingAddressee
            End Get
            Set(value As QuickQuoteBillingAddressee)
                QuoteBase.BillingAddressee = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property FirstWrittenDate As String 'will only be used if it's there... else will keep Diamond default
            Get
                Return QuoteBase.FirstWrittenDate
            End Get
            Set(value As String)
                '_FirstWrittenDate = value
                'qqHelper.ConvertToShortDate(_FirstWrittenDate)
                QuoteBase.FirstWrittenDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property QuoteTransactionType As QuickQuoteObject.QuickQuoteTransactionType
            Get
                Return QuoteBase.QuoteTransactionType
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property OriginalEffectiveDate As String
            Get
                Return QuoteBase.OriginalEffectiveDate
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property OriginalExpirationDate As String
            Get
                Return QuoteBase.OriginalExpirationDate
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property OriginalTransactionEffectiveDate As String
            Get
                Return QuoteBase.OriginalTransactionEffectiveDate
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property OriginalTransactionExpirationDate As String
            Get
                Return QuoteBase.OriginalTransactionExpirationDate
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TransactionRemark As String
            Get
                Return QuoteBase.TransactionRemark
            End Get
            Set(value As String)
                QuoteBase.TransactionRemark = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TransactionReasonId As String
            Get
                Return QuoteBase.TransactionReasonId
            End Get
            Set(value As String)
                QuoteBase.TransactionReasonId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property AnnualPremium As String 'PolicyImage.premium_annual
            Get
                Return QuotePremiums.AnnualPremium
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property ChangeInFullTermPremium As String 'PolicyImage.premium_chg_fullterm
            Get
                Return QuotePremiums.ChangeInFullTermPremium
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property ChangeInWrittenPremium As String 'PolicyImage.premium_chg_written
            Get
                Return QuotePremiums.ChangeInWrittenPremium
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property DifferenceChangeInFullTermPremium As String 'PolicyImage.premium_diff_chg_fullterm
            Get
                Return QuotePremiums.DifferenceChangeInFullTermPremium
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property DifferenceChangeInWrittenPremium As String 'PolicyImage.premium_diff_chg_written
            Get
                Return QuotePremiums.DifferenceChangeInWrittenPremium
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property FullTermPremium As String 'PolicyImage.premium_fullterm
            Get
                Return QuotePremiums.FullTermPremium
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property FullTermPremiumOffsetForPreviousImage As String 'PolicyImage.ftp_offset_for_prev_image
            Get
                Return QuotePremiums.FullTermPremiumOffsetForPreviousImage
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property FullTermPremiumOnsetForCurrent As String 'PolicyImage.ftp_onset_for_current
            Get
                Return QuotePremiums.FullTermPremiumOnsetForCurrent
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property OffsetPremiumForPreviousImage As String 'PolicyImage.offset_for_prev_image
            Get
                Return QuotePremiums.OffsetPremiumForPreviousImage
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property OnsetPremiumForCurrentImage As String 'PolicyImage.onset_for_current
            Get
                Return QuotePremiums.OnsetPremiumForCurrentImage
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property PreviousWrittenPremium As String 'PolicyImage.premium_previous_written
            Get
                Return QuotePremiums.PreviousWrittenPremium
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property WrittenPremium As String 'PolicyImage.premium_written
            Get
                Return QuotePremiums.WrittenPremium
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property PriorTermAnnual As String 'PolicyImage.prior_term_annual_premium
            Get
                Return QuotePremiums.PriorTermAnnual
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property PriorTermFullterm As String 'PolicyImage.prior_term_fullterm
            Get
                Return QuotePremiums.PriorTermFullterm
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Comments As List(Of QuickQuoteComment)
            Get
                Return QuoteBase.Comments
            End Get
            Set(value As List(Of QuickQuoteComment))
                QuoteBase.Comments = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Messages As List(Of QuickQuoteMessage) 'TODO: Dan - Parent?
            Get
                Return QuoteBase.Messages
            End Get
            Set(value As List(Of QuickQuoteMessage))
                QuoteBase.Messages = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property MultiStateQuotes As List(Of QuickQuoteObject)
            Get
                Return StateAndLobParts.MultiStateQuotes
            End Get
            Set(value As List(Of QuickQuoteObject))
                StateAndLobParts.MultiStateQuotes = value
            End Set
        End Property
        'added 7/27/2018
        Public Property QuoteBase As QuickQuoteTopLevelQuoteBase
            Get
                If _QuoteBase Is Nothing Then
                    _QuoteBase = New QuickQuoteTopLevelQuoteBase
                End If
                SetObjectsParent(_QuoteBase)
                Return _QuoteBase
            End Get
            Set(value As QuickQuoteTopLevelQuoteBase)
                _QuoteBase = value
                SetObjectsParent(_QuoteBase)
            End Set
        End Property
        Public Property QuotePremiums As QuickQuoteTopLevelQuotePremiums
            Get
                If _QuotePremiums Is Nothing Then
                    _QuotePremiums = New QuickQuoteTopLevelQuotePremiums
                End If
                SetObjectsParent(_QuotePremiums)
                Return _QuotePremiums
            End Get
            Set(value As QuickQuoteTopLevelQuotePremiums)
                _QuotePremiums = value
                SetObjectsParent(_QuotePremiums)
            End Set
        End Property
        Public Property StateAndLobParts As QuickQuoteTopLevelStateAndLobParts
            Get
                If _StateAndLobParts Is Nothing Then
                    _StateAndLobParts = New QuickQuoteTopLevelStateAndLobParts
                End If
                SetObjectsParent(_StateAndLobParts)
                Return _StateAndLobParts
            End Get
            Set(value As QuickQuoteTopLevelStateAndLobParts)
                _StateAndLobParts = value
                SetObjectsParent(_StateAndLobParts)
            End Set
        End Property

        'added 7/28/2018
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property OriginallyHadMultipleQuoteStates As Boolean
            Get
                Return StateAndLobParts.OriginallyHadMultipleQuoteStates
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property OriginalQuoteStates As List(Of QuickQuoteHelperClass.QuickQuoteState)
            Get
                Return StateAndLobParts.OriginalQuoteStates
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property OriginalGoverningState As QuickQuoteHelperClass.QuickQuoteState
            Get
                Return StateAndLobParts.OriginalGoverningState
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property TotalQuotedPremiumType As QuickQuoteHelperClass.PremiumType
            Get
                Return QuotePremiums.TotalQuotedPremiumType
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property QuoteLevel As QuickQuoteHelperClass.QuoteLevel 'added 7/28/2018; 12/30/2018 - moved from TopLevelBaseCommonInfo to StateAndLobParts object so it would not get Copied between topLevel and stateLevel quotes
            Get
                Return StateAndLobParts.QuoteLevel
            End Get
        End Property
        'added 7/31/2018
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property OriginallyInMultiStatePackageFormat As Boolean
            Get
                Return StateAndLobParts.OriginallyInMultiStatePackageFormat
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property OriginalPackageParts As List(Of QuickQuotePackagePart)
            Get
                Return StateAndLobParts.OriginalPackageParts
            End Get
        End Property

        'added 8/1/2018
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property NeedsMultiStateFormat As Boolean
            Get
                Return StateAndLobParts.NeedsMultiStateFormat
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property LobIdToUse As String
            Get
                Return StateAndLobParts.LobIdToUse
            End Get
        End Property

        'added 9/17/2018
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property MasterPackageLocations As List(Of QuickQuoteLocation)
            Get
                Return StateAndLobParts.MasterPackageLocations
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property MasterPackageVehicles As List(Of QuickQuoteVehicle)
            Get
                Return StateAndLobParts.MasterPackageVehicles
            End Get
        End Property
        '<Script.Serialization.ScriptIgnore>
        '<System.Xml.Serialization.XmlIgnore()>
        'Public ReadOnly Property MasterPackageAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        '    Get
        '        Return StateAndLobParts.MasterPackageAdditionalInterests
        '    End Get
        'End Property
        'added 10/18/2018
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property CGLPackageLocations As List(Of QuickQuoteLocation)
            Get
                Return StateAndLobParts.CGLPackageLocations
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property CPRPackageLocations As List(Of QuickQuoteLocation)
            Get
                Return StateAndLobParts.CPRPackageLocations
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property CIMPackageLocations As List(Of QuickQuoteLocation)
            Get
                Return StateAndLobParts.CIMPackageLocations
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property CRMPackageLocations As List(Of QuickQuoteLocation)
            Get
                Return StateAndLobParts.CRMPackageLocations
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property GARPackageLocations As List(Of QuickQuoteLocation)
            Get
                Return StateAndLobParts.GARPackageLocations
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property CGLPackageVehicles As List(Of QuickQuoteVehicle)
            Get
                Return StateAndLobParts.CGLPackageVehicles
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property CPRPackageVehicles As List(Of QuickQuoteVehicle)
            Get
                Return StateAndLobParts.CPRPackageVehicles
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property CIMPackageVehicles As List(Of QuickQuoteVehicle)
            Get
                Return StateAndLobParts.CIMPackageVehicles
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property CRMPackageVehicles As List(Of QuickQuoteVehicle)
            Get
                Return StateAndLobParts.CRMPackageVehicles
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property GARPackageVehicles As List(Of QuickQuoteVehicle)
            Get
                Return StateAndLobParts.GARPackageVehicles
            End Get
        End Property
        'added 10/19/2018
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property MasterPackageModifiers As List(Of QuickQuoteModifier)
            Get
                Return StateAndLobParts.MasterPackageModifiers
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property CGLPackageModifiers As List(Of QuickQuoteModifier)
            Get
                Return StateAndLobParts.CGLPackageModifiers
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property CPRPackageModifiers As List(Of QuickQuoteModifier)
            Get
                Return StateAndLobParts.CPRPackageModifiers
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property CIMPackageModifiers As List(Of QuickQuoteModifier)
            Get
                Return StateAndLobParts.CIMPackageModifiers
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property CRMPackageModifiers As List(Of QuickQuoteModifier)
            Get
                Return StateAndLobParts.CRMPackageModifiers
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property GARPackageModifiers As List(Of QuickQuoteModifier)
            Get
                Return StateAndLobParts.GARPackageModifiers
            End Get
        End Property

        'added 10/17/2018
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CanUseLocationNumForMasterPartLocationReconciliation As Boolean
            Get
                Return StateAndLobParts.CanUseLocationNumForMasterPartLocationReconciliation
            End Get
            Set(value As Boolean)
                StateAndLobParts.CanUseLocationNumForMasterPartLocationReconciliation = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CanUseLocationNumForCGLPartLocationReconciliation As Boolean
            Get
                Return StateAndLobParts.CanUseLocationNumForCGLPartLocationReconciliation
            End Get
            Set(value As Boolean)
                StateAndLobParts.CanUseLocationNumForCGLPartLocationReconciliation = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CanUseLocationNumForCPRPartLocationReconciliation As Boolean
            Get
                Return StateAndLobParts.CanUseLocationNumForCPRPartLocationReconciliation
            End Get
            Set(value As Boolean)
                StateAndLobParts.CanUseLocationNumForCPRPartLocationReconciliation = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CanUseLocationNumForCIMPartLocationReconciliation As Boolean
            Get
                Return StateAndLobParts.CanUseLocationNumForCIMPartLocationReconciliation
            End Get
            Set(value As Boolean)
                StateAndLobParts.CanUseLocationNumForCIMPartLocationReconciliation = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CanUseLocationNumForCRMPartLocationReconciliation As Boolean
            Get
                Return StateAndLobParts.CanUseLocationNumForCRMPartLocationReconciliation
            End Get
            Set(value As Boolean)
                StateAndLobParts.CanUseLocationNumForCRMPartLocationReconciliation = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CanUseLocationNumForGARPartLocationReconciliation As Boolean
            Get
                Return StateAndLobParts.CanUseLocationNumForGARPartLocationReconciliation
            End Get
            Set(value As Boolean)
                StateAndLobParts.CanUseLocationNumForGARPartLocationReconciliation = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CanUseVehicleNumForMasterPartVehicleReconciliation As Boolean
            Get
                Return StateAndLobParts.CanUseVehicleNumForMasterPartVehicleReconciliation
            End Get
            Set(value As Boolean)
                StateAndLobParts.CanUseVehicleNumForMasterPartVehicleReconciliation = value
            End Set
        End Property
        'added 10/18/2018
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CanUseVehicleNumForCGLPartVehicleReconciliation As Boolean
            Get
                Return StateAndLobParts.CanUseVehicleNumForCGLPartVehicleReconciliation
            End Get
            Set(value As Boolean)
                StateAndLobParts.CanUseVehicleNumForCGLPartVehicleReconciliation = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CanUseVehicleNumForCPRPartVehicleReconciliation As Boolean
            Get
                Return StateAndLobParts.CanUseVehicleNumForCPRPartVehicleReconciliation
            End Get
            Set(value As Boolean)
                StateAndLobParts.CanUseVehicleNumForCPRPartVehicleReconciliation = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CanUseVehicleNumForCIMPartVehicleReconciliation As Boolean
            Get
                Return StateAndLobParts.CanUseVehicleNumForCIMPartVehicleReconciliation
            End Get
            Set(value As Boolean)
                StateAndLobParts.CanUseVehicleNumForCIMPartVehicleReconciliation = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CanUseVehicleNumForCRMPartVehicleReconciliation As Boolean
            Get
                Return StateAndLobParts.CanUseVehicleNumForCRMPartVehicleReconciliation
            End Get
            Set(value As Boolean)
                StateAndLobParts.CanUseVehicleNumForCRMPartVehicleReconciliation = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CanUseVehicleNumForGARPartVehicleReconciliation As Boolean
            Get
                Return StateAndLobParts.CanUseVehicleNumForGARPartVehicleReconciliation
            End Get
            Set(value As Boolean)
                StateAndLobParts.CanUseVehicleNumForGARPartVehicleReconciliation = value
            End Set
        End Property

        'added 3/19/2019
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_DiamondImageInfoId As Integer
            Get
                Return QuoteBase.Database_DiamondImageInfoId
            End Get
            Set(value As Integer)
                QuoteBase.Database_DiamondImageInfoId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_DiamondImageXmlId As Integer
            Get
                Return QuoteBase.Database_DiamondImageXmlId
            End Get
            Set(value As Integer)
                QuoteBase.Database_DiamondImageXmlId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_DiamondImageInfoType As QuickQuoteDiamondImageInfo.ImageInfoType
            Get
                Return QuoteBase.Database_DiamondImageInfoType
            End Get
            Set(value As QuickQuoteDiamondImageInfo.ImageInfoType)
                QuoteBase.Database_DiamondImageInfoType = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_IsBillingUpdate As Boolean
            Get
                Return QuoteBase.Database_IsBillingUpdate
            End Get
            Set(value As Boolean)
                QuoteBase.Database_IsBillingUpdate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_EndorsementOrigin As QuickQuoteEndorsementForPolicyIdAndTransactionDateInput.EndorsementOriginTypes
            Get
                Return QuoteBase.Database_EndorsementOrigin
            End Get
            Set(value As QuickQuoteEndorsementForPolicyIdAndTransactionDateInput.EndorsementOriginTypes)
                QuoteBase.Database_EndorsementOrigin = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_DevDictionaryID As Integer
            Get
                Return QuoteBase.Database_DevDictionaryID
            End Get
            Set(value As Integer)
                QuoteBase.Database_DevDictionaryID = value
            End Set
        End Property

        'added 1/4/2020 (Interoperability)
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_LastRulesOverrideRecordModifiedDate As String
            Get
                Return QuoteBase.Database_LastRulesOverrideRecordModifiedDate
            End Get
            Set(value As String)
                QuoteBase.Database_LastRulesOverrideRecordModifiedDate = value
            End Set
        End Property

        'added 5/13/2021
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuickQuote_Inserted As String
            Get
                Return QuoteBase.Database_QuickQuote_Inserted
            End Get
            Set(value As String)
                QuoteBase.Database_QuickQuote_Inserted = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuickQuote_Updated As String
            Get
                Return QuoteBase.Database_QuickQuote_Updated
            End Get
            Set(value As String)
                QuoteBase.Database_QuickQuote_Updated = value
            End Set
        End Property

        'added 11/27/2022
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_CompanyId As String
            Get
                Return QuoteBase.Database_CompanyId
            End Get
            Set(value As String)
                QuoteBase.Database_CompanyId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuoteCompanyId As String
            Get
                Return QuoteBase.Database_QuoteCompanyId
            End Get
            Set(value As String)
                QuoteBase.Database_QuoteCompanyId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_AppCompanyId As String
            Get
                Return QuoteBase.Database_AppCompanyId
            End Get
            Set(value As String)
                QuoteBase.Database_AppCompanyId = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_DiaCompanyId As String 'added 7/27/2023
            Get
                Return QuoteBase.Database_DiaCompanyId
            End Get
            Set(value As String)
                QuoteBase.Database_DiaCompanyId = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property Database_DiaCompany As QuickQuoteHelperClass.QuickQuoteCompany 'added 7/27/2023
            Get
                Return QuoteBase.Database_DiaCompany
            End Get
        End Property

        'added 5/22/2019
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AddedDate As String
            Get
                Return QuoteBase.AddedDate
            End Get
            Set(value As String)
                QuoteBase.AddedDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property LastModifiedDate As String
            Get
                Return QuoteBase.LastModifiedDate
            End Get
            Set(value As String)
                QuoteBase.LastModifiedDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PCAdded_Date As String
            Get
                Return QuoteBase.PCAdded_Date
            End Get
            Set(value As String)
                QuoteBase.PCAdded_Date = value
            End Set
        End Property

        'added 6/15/2019
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyCurrentStatusId As String
            Get
                Return QuoteBase.PolicyCurrentStatusId
            End Get
            Set(value As String)
                QuoteBase.PolicyCurrentStatusId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyStatusCodeId As String
            Get
                Return QuoteBase.PolicyStatusCodeId
            End Get
            Set(value As String)
                QuoteBase.PolicyStatusCodeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CancelDate As String
            Get
                Return QuoteBase.CancelDate
            End Get
            Set(value As String)
                QuoteBase.CancelDate = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Protected Friend Property InternalDevDictionary As QuickQuoteDevDictionaryList
            Get
                Return QuoteBase.InternalDevDictionary
            End Get
            Set(value As QuickQuoteDevDictionaryList)
                QuoteBase.InternalDevDictionary = value
            End Set
        End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Public Sub New(Parent As QuickQuoteObject) 'added 6/27/2018; could probably just use generic type so one constructor could be used for multiple types
            MyBase.New()
            SetDefaults()
            Me.SetParent = Parent
        End Sub
        Private Sub SetDefaults()
            'added 7/27/2018
            _QuoteBase = New QuickQuoteTopLevelQuoteBase
            _QuotePremiums = New QuickQuoteTopLevelQuotePremiums
            _StateAndLobParts = New QuickQuoteTopLevelStateAndLobParts

            '_XmlType = QuickQuoteObject.QuickQuoteXmlType.None
            '_CompanyId = "1" '12/11/2013 note: not worth using xml right now; company_code is blank for both company_id 0 and 1; 1 has am_best_number 2251

            '_Success = False

            '_AgencyCode = ""
            '_AgencyId = ""
            'If System.Web.HttpContext.Current?.Session("DiamondAgencyCode") IsNot Nothing AndAlso System.Web.HttpContext.Current?.Session("DiamondAgencyCode").ToString <> "" Then
            '    _AgencyCode = System.Web.HttpContext.Current?.Session("DiamondAgencyCode").ToString
            'ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
            '    _AgencyCode = ConfigurationManager.AppSettings("QuickQuoteTestAgencyCode").ToString
            'End If
            'If System.Web.HttpContext.Current?.Session("DiamondAgencyId") IsNot Nothing AndAlso System.Web.HttpContext.Current?.Session("DiamondAgencyId").ToString <> "" AndAlso IsNumeric(System.Web.HttpContext.Current?.Session("DiamondAgencyId").ToString) = True Then
            '    _AgencyId = System.Web.HttpContext.Current?.Session("DiamondAgencyId").ToString
            'ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
            '    _AgencyId = ConfigurationManager.AppSettings("QuickQuoteTestAgencyId").ToString
            'End If
            ''8/13/2014 note: could update to use new shared functions in QuickQuoteHelperClass

            ''added for staff
            'If _AgencyCode = "" OrElse _AgencyId = "" Then
            '    qqHelper.SetUserAgencyVariables()
            '    If System.Web.HttpContext.Current?.Session("DiamondAgencyCode") IsNot Nothing AndAlso System.Web.HttpContext.Current?.Session("DiamondAgencyCode").ToString <> "" Then
            '        _AgencyCode = System.Web.HttpContext.Current?.Session("DiamondAgencyCode").ToString
            '    End If
            '    If System.Web.HttpContext.Current?.Session("DiamondAgencyId") IsNot Nothing AndAlso System.Web.HttpContext.Current?.Session("DiamondAgencyId").ToString <> "" AndAlso IsNumeric(System.Web.HttpContext.Current?.Session("DiamondAgencyId").ToString) = True Then
            '        _AgencyId = System.Web.HttpContext.Current?.Session("DiamondAgencyId").ToString
            '    End If
            'End If


            '_AgencyProducerId = ""
            '_AgencyProducerCode = ""
            '_AgencyProducerName = New QuickQuoteName
            '_AgencyProducerName.NameAddressSourceId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.NameAddressSourceId, "Agency Producer")

            '_QuoteNumber = "" 'note: now being reset from quickQuote.Reset_Database_Values() too
            '_PolicyNumber = "" 'note: now being reset from quickQuote.Reset_Database_Values() too
            '_QuoteDescription = ""

            '_EffectiveDate = ""
            ''VersionAndLobInfo.Set_QuoteEffectiveDate(_EffectiveDate) 'still being set from QuickQuoteObject's SetDefaults method
            '_ExpirationDate = ""
            '_TotalQuotedPremium = ""

            '_ValidationItems = Nothing

            '_Client = New QuickQuoteClient
            '_Client.Name.NameAddressSourceId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.NameAddressSourceId, "Client 1")
            '_Client.Name2.NameAddressSourceId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.NameAddressSourceId, "Client 2")

            '_IsNew = False
            '_BillToId = ""
            '_CurrentBilltoId = _BillToId
            '_CurrentPayplanId = ""
            '_PolicyTermId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, "12 Month")
            '_ReceivedDate = Date.Today.ToShortDateString '*defaulting to current date
            '_TransactionEffectiveDate = Date.Today.ToShortDateString '*defaulting to current date; note 12/11/2013: may need to change to always use EffectiveDate
            '_TransactionExpirationDate = DateAdd(DateInterval.Year, 1, CDate(_TransactionEffectiveDate)).ToShortDateString
            '_TransactionTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.TransactionTypeId, "New Business")
            '_TransactionUsersId = ""
            'QuickQuoteHelperClass.SetDiamondUserId(_TransactionUsersId)
            '_WorkflowQueueId = ""

            '_Policyholder = New QuickQuotePolicyholder
            '_Policyholder.Name.NameAddressSourceId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.NameAddressSourceId, "Policy Holder 1")
            '_Policyholder2 = New QuickQuotePolicyholder
            '_Policyholder2.Name.NameAddressSourceId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.NameAddressSourceId, "Policy Holder 2")

            '_BillMethodId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.BillMethodId, "Direct Bill")
            '_BillingPayPlanId = _CurrentPayplanId

            'Default_PolicyOriginTypeId()

            '_HasInitiatedFinalize = False

            '_PolicyBridgingURL = ""

            '_PaymentOptions = Nothing
            '_CurrentlyParsingPaymentOptions = False

            '_ExperienceModificationFactor = ""
            '_ExperienceModificationBureauTypeId = ""
            '_ExperienceModificationRiskIdentifier = ""
            '_ExperienceModifications = Nothing
            '_CanUseExperienceModificationNumForExperienceModificationReconciliation = False
            '_HasConvertedExperienceModifications = False
            '_DiamondExperienceModificationIndexesToUpdate = Nothing

            '_GuaranteedRatePeriodEffectiveDate = Date.Today.ToShortDateString
            '_GuaranteedRatePeriodExpirationDate = DateAdd(DateInterval.Year, 1, CDate(_GuaranteedRatePeriodEffectiveDate)).ToShortDateString
            '_ModificationProductionDate = ""
            '_RatingEffectiveDate = ""

            '_AdditionalPolicyholders = Nothing

            '_QuoteTypeId = ""

            '_PackageParts = Nothing

            '_EFT_BankRoutingNumber = ""
            '_EFT_BankAccountNumber = ""
            '_EFT_BankAccountTypeId = ""
            '_EFT_DeductionDay = ""

            '_OnlyUsePropertyToSetFieldWithSameName = False

            'Reset_Database_Values()

            '_Agency = New QuickQuoteAgency

            '_BillingAddressee = New QuickQuoteBillingAddressee

            '_FirstWrittenDate = ""

            '_QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote
            ''VersionAndLobInfo.Set_QuoteTransactionType(_QuoteTransactionType) 'will still be called from QuickQuoteObject's SetDefaults
            '_OriginalEffectiveDate = ""
            '_OriginalExpirationDate = ""
            '_OriginalTransactionEffectiveDate = ""
            '_OriginalTransactionExpirationDate = ""

            '_TransactionRemark = ""
            '_TransactionReasonId = ""

            '_AnnualPremium = "" 'PolicyImage.premium_annual
            '_ChangeInFullTermPremium = "" 'PolicyImage.premium_chg_fullterm
            '_ChangeInWrittenPremium = "" 'PolicyImage.premium_chg_written
            '_DifferenceChangeInFullTermPremium = "" 'PolicyImage.premium_diff_chg_fullterm
            '_DifferenceChangeInWrittenPremium = "" 'PolicyImage.premium_diff_chg_written
            '_FullTermPremium = "" 'PolicyImage.premium_fullterm
            '_FullTermPremiumOffsetForPreviousImage = "" 'PolicyImage.ftp_offset_for_prev_image
            '_FullTermPremiumOnsetForCurrent = "" 'PolicyImage.ftp_onset_for_current
            '_OffsetPremiumForPreviousImage = "" 'PolicyImage.offset_for_prev_image
            '_OnsetPremiumForCurrentImage = "" 'PolicyImage.onset_for_current
            '_PreviousWrittenPremium = "" 'PolicyImage.premium_previous_written
            '_WrittenPremium = "" 'PolicyImage.premium_written
            '_PriorTermAnnual = "" 'PolicyImage.prior_term_annual_premium
            '_PriorTermFullterm = "" 'PolicyImage.prior_term_fullterm

            '_Comments = Nothing

            '_Messages = Nothing

            '_MultiStateQuotes = Nothing

        End Sub

        Public Sub Reset_Database_Values() 'for Copy Quote functionality so database values (specifically quoteId) aren't tied to new quote... since new validation will now prevent save or rate from completing... also don't want to inadvertently use previous policyId, imageNum, or policyImageId from copied quote when using Diamond services
            '_QuoteNumber = "" 'note: already being reset from SetDefaults() too
            '_PolicyNumber = "" 'note: already being reset from SetDefaults() too

            '_PolicyId = ""
            '_PolicyImageNum = ""
            '_PolicyImageId = ""
            ''If _VersionAndLobInfo IsNot Nothing AndAlso _VersionAndLobInfo.PolicyLevelInfoExtended IsNot Nothing Then 'still being called from QuickQuoteObject's Reset_Database_Values method
            ''    _VersionAndLobInfo.PolicyLevelInfoExtended.PolicyId = ""
            ''    _VersionAndLobInfo.PolicyLevelInfoExtended.PolicyImageNum = ""
            ''End If

            '_Database_QuoteId = ""
            '_Database_QuoteXmlId = ""
            '_Database_QuoteNumber = ""
            '_Database_LobId = ""
            '_Database_CurrentQuoteXmlId = ""
            '_Database_XmlQuoteId = ""
            '_Database_LastAvailableQuoteNumber = ""
            '_Database_QuoteStatusId = ""
            '_Database_XmlStatusId = ""
            '_Database_IsPolicy = False
            '_Database_DiamondPolicyNumber = ""
            '_Database_OriginatedInVR = False

            '_Database_EffectiveDate = ""

            If _QuoteBase IsNot Nothing Then
                _QuoteBase.Reset_Database_Values()
            End If
        End Sub
        Protected Friend Sub Set_QuoteTransactionType(ByVal qqTranType As QuickQuoteObject.QuickQuoteTransactionType, Optional ByVal setTransactionTypeIdForNewBusinessQuoteAndEndorsement As Boolean = False)
            '_QuoteTransactionType = qqTranType
            ''VersionAndLobInfo.Set_QuoteTransactionType(_QuoteTransactionType) 'this is still being done from same method on QuickQuoteObject
            'If setTransactionTypeIdForNewBusinessQuoteAndEndorsement = True Then
            '    'note: ReadOnlyImage should not reset anything
            '    If _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote Then
            '        _TransactionTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.TransactionTypeId, "New Business") '2 for Diamond
            '    ElseIf _QuoteTransactionType = quickquoteobject.QuickQuoteTransactionType.EndorsementQuote Then
            '        _TransactionTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.TransactionTypeId, "Endorsement") '3 for Diamond
            '    End If
            'End If

            QuoteBase.Set_QuoteTransactionType(qqTranType, setTransactionTypeIdForNewBusinessQuoteAndEndorsement:=setTransactionTypeIdForNewBusinessQuoteAndEndorsement)
        End Sub
        Protected Friend Sub Set_OriginalEffectiveDate(ByVal origEffDate As String)
            '_OriginalEffectiveDate = origEffDate

            QuoteBase.Set_OriginalEffectiveDate(origEffDate)
        End Sub
        Protected Friend Sub Set_OriginalExpirationDate(ByVal origExpDate As String)
            '_OriginalExpirationDate = origExpDate

            QuoteBase.Set_OriginalExpirationDate(origExpDate)
        End Sub
        Protected Friend Sub Set_OriginalTransactionEffectiveDate(ByVal origTEffDate As String)
            '_OriginalTransactionEffectiveDate = origTEffDate

            QuoteBase.Set_OriginalTransactionEffectiveDate(origTEffDate)
        End Sub
        Protected Friend Sub Set_OriginalTransactionExpirationDate(ByVal origTExpDate As String)
            '_OriginalTransactionExpirationDate = origTExpDate

            QuoteBase.Set_OriginalTransactionExpirationDate(origTExpDate)
        End Sub
        'added 10/28/2016 so it can be called from multiple spots... needed w/ additional of new values in Diamond... 0=Diamond, 1=Comparative Rating (used to be Web), 2=VelociRater (new), 3=Consumer Quoting (new)
        Protected Friend Sub Default_PolicyOriginTypeId()
            '_PolicyOriginTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyOriginTypeId, "VelociRater")
            'If qqHelper.IsNumericString(_PolicyOriginTypeId) = False Then 'added just in case static data file isn't updated in environment
            '    _PolicyOriginTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyOriginTypeId, "Comparative Rating")
            '    If qqHelper.IsNumericString(_PolicyOriginTypeId) = False Then 'added just in case static data file hasn't been updated w/ change from Web to Comparative Rating
            '        _PolicyOriginTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyOriginTypeId, "Web")
            '    End If
            'End If

            QuoteBase.Default_PolicyOriginTypeId()
        End Sub
        Protected Friend Sub Set_AnnualPremium(ByVal prem As String) 'PolicyImage.premium_annual
            '_AnnualPremium = prem

            QuotePremiums.Set_AnnualPremium(prem)
        End Sub
        Protected Friend Sub Set_ChangeInFullTermPremium(ByVal prem As String) 'PolicyImage.premium_chg_fullterm
            '_ChangeInFullTermPremium = prem

            QuotePremiums.Set_ChangeInFullTermPremium(prem)
        End Sub
        Protected Friend Sub Set_ChangeInWrittenPremium(ByVal prem As String) 'PolicyImage.premium_chg_written
            '_ChangeInWrittenPremium = prem

            QuotePremiums.Set_ChangeInWrittenPremium(prem)
        End Sub
        Protected Friend Sub Set_DifferenceChangeInFullTermPremium(ByVal prem As String) 'PolicyImage.premium_diff_chg_fullterm
            '_DifferenceChangeInFullTermPremium = prem

            QuotePremiums.Set_DifferenceChangeInFullTermPremium(prem)
        End Sub
        Protected Friend Sub Set_DifferenceChangeInWrittenPremium(ByVal prem As String) 'PolicyImage.premium_diff_chg_written
            '_DifferenceChangeInWrittenPremium = prem

            QuotePremiums.Set_DifferenceChangeInWrittenPremium(prem)
        End Sub
        Protected Friend Sub Set_FullTermPremium(ByVal prem As String) 'PolicyImage.premium_fullterm
            '_FullTermPremium = prem

            QuotePremiums.Set_FullTermPremium(prem)
        End Sub
        Protected Friend Sub Set_FullTermPremiumOffsetForPreviousImage(ByVal prem As String) 'PolicyImage.ftp_offset_for_prev_image
            '_FullTermPremiumOffsetForPreviousImage = prem

            QuotePremiums.Set_FullTermPremiumOffsetForPreviousImage(prem)
        End Sub
        Protected Friend Sub Set_FullTermPremiumOnsetForCurrent(ByVal prem As String) 'PolicyImage.ftp_onset_for_current
            '_FullTermPremiumOnsetForCurrent = prem

            QuotePremiums.Set_FullTermPremiumOnsetForCurrent(prem)
        End Sub
        Protected Friend Sub Set_OffsetPremiumForPreviousImage(ByVal prem As String) 'PolicyImage.offset_for_prev_image
            '_OffsetPremiumForPreviousImage = prem

            QuotePremiums.Set_OffsetPremiumForPreviousImage(prem)
        End Sub
        Protected Friend Sub Set_OnsetPremiumForCurrentImage(ByVal prem As String) 'PolicyImage.onset_for_current
            '_OnsetPremiumForCurrentImage = prem

            QuotePremiums.Set_OnsetPremiumForCurrentImage(prem)
        End Sub
        Protected Friend Sub Set_PreviousWrittenPremium(ByVal prem As String) 'PolicyImage.premium_previous_written
            '_PreviousWrittenPremium = prem

            QuotePremiums.Set_PreviousWrittenPremium(prem)
        End Sub
        Protected Friend Sub Set_WrittenPremium(ByVal prem As String) 'PolicyImage.premium_written
            '_WrittenPremium = prem

            QuotePremiums.Set_WrittenPremium(prem)
        End Sub
        Protected Friend Sub Set_PriorTermAnnual(ByVal prem As String) 'PolicyImage.prior_term_annual_premium
            '_PriorTermAnnual = prem

            QuotePremiums.Set_PriorTermAnnual(prem)
        End Sub
        Protected Friend Sub Set_PriorTermFullterm(ByVal prem As String) 'PolicyImage.prior_term_fullterm
            '_PriorTermFullterm = prem

            QuotePremiums.Set_PriorTermFullterm(prem)
        End Sub
        Protected Friend Sub Set_QuoteStatus(ByVal status As QuickQuoteXML.QuickQuoteStatusType)
            '_Database_QuoteStatusId = CInt(status)

            QuoteBase.Set_QuoteStatus(status)
        End Sub

        Protected Friend Sub Set_CurrentlyParsingPaymentOptions(ByVal currentlyParsing As Boolean)
            '_CurrentlyParsingPaymentOptions = currentlyParsing

            QuoteBase.Set_CurrentlyParsingPaymentOptions(currentlyParsing)
        End Sub

        Protected Friend Function Get_PaymentOptions_Variable() As List(Of QuickQuotePaymentOption)
            'Return _PaymentOptions

            Return QuoteBase.Get_PaymentOptions_Variable()
        End Function
        Protected Friend Sub NumberPaymentOptionsIfPresent()
            'If _PaymentOptions IsNot Nothing AndAlso _PaymentOptions.Count > 0 Then
            '    qqHelper.NumberPaymentOptions(_PaymentOptions)
            'End If

            QuoteBase.NumberPaymentOptionsIfPresent()
        End Sub

        'added 7/28/2018
        Protected Friend Sub Set_OriginallyHadMultipleQuoteStates(ByVal hadMultipleQuoteStates As Boolean)
            StateAndLobParts.Set_OriginallyHadMultipleQuoteStates(hadMultipleQuoteStates)
        End Sub
        Protected Friend Sub Set_OriginalQuoteStates(ByVal states As List(Of QuickQuoteHelperClass.QuickQuoteState))
            StateAndLobParts.Set_OriginalQuoteStates(states)
        End Sub
        Protected Friend Sub Set_OriginalGoverningState(ByVal governingState As QuickQuoteHelperClass.QuickQuoteState)
            StateAndLobParts.Set_OriginalGoverningState(governingState)
        End Sub
        Protected Friend Sub Set_TotalQuotedPremiumType(ByVal premType As QuickQuoteHelperClass.PremiumType)
            QuotePremiums.Set_TotalQuotedPremiumType(premType)
        End Sub
        Protected Friend Sub Set_QuoteLevel(ByVal level As QuickQuoteHelperClass.QuoteLevel) '12/30/2018 - moved from TopLevelBaseCommonInfo to StateAndLobParts object so it would not get Copied between topLevel and stateLevel quotes
            StateAndLobParts.Set_QuoteLevel(level)
        End Sub
        'added 7/31/2018
        Protected Friend Sub Set_OriginallyInMultiStatePackageFormat(ByVal inMultiStatePackageFormat As Boolean)
            StateAndLobParts.Set_OriginallyInMultiStatePackageFormat(inMultiStatePackageFormat)
        End Sub
        Protected Friend Sub ArchiveAndClearPackageParts()
            StateAndLobParts.ArchiveAndClearPackageParts()
        End Sub

        'added 8/1/2018
        Protected Friend Sub Set_NeedsMultiStateFormat(ByVal needsMultiState As Boolean)
            StateAndLobParts.Set_NeedsMultiStateFormat(needsMultiState)
        End Sub
        Protected Friend Sub Set_LobIdToUse(ByVal lobId As String)
            StateAndLobParts.Set_LobIdToUse(lobId)
        End Sub

        'added 9/17/2018
        Protected Friend Sub Set_MasterPackageLocations(ByVal locs As List(Of QuickQuoteLocation))
            StateAndLobParts.Set_MasterPackageLocations(locs)
        End Sub
        Protected Friend Sub Set_MasterPackageVehicles(ByVal vehs As List(Of QuickQuoteVehicle))
            StateAndLobParts.Set_MasterPackageVehicles(vehs)
        End Sub
        'Protected Friend Sub Set_MasterPackageAdditionalInterests(ByVal ais As List(Of QuickQuoteAdditionalInterest))
        '    StateAndLobParts.Set_MasterPackageAdditionalInterests(ais)
        'End Sub
        'added 10/18/2018
        Protected Friend Sub Set_CGLPackageLocations(ByVal locs As List(Of QuickQuoteLocation))
            StateAndLobParts.Set_CGLPackageLocations(locs)
        End Sub
        Protected Friend Sub Set_CPRPackageLocations(ByVal locs As List(Of QuickQuoteLocation))
            StateAndLobParts.Set_CPRPackageLocations(locs)
        End Sub
        Protected Friend Sub Set_CIMPackageLocations(ByVal locs As List(Of QuickQuoteLocation))
            StateAndLobParts.Set_CIMPackageLocations(locs)
        End Sub
        Protected Friend Sub Set_CRMPackageLocations(ByVal locs As List(Of QuickQuoteLocation))
            StateAndLobParts.Set_CRMPackageLocations(locs)
        End Sub
        Protected Friend Sub Set_GARPackageLocations(ByVal locs As List(Of QuickQuoteLocation))
            StateAndLobParts.Set_GARPackageLocations(locs)
        End Sub
        Protected Friend Sub Set_CGLPackageVehicles(ByVal vehs As List(Of QuickQuoteVehicle))
            StateAndLobParts.Set_CGLPackageVehicles(vehs)
        End Sub
        Protected Friend Sub Set_CPRPackageVehicles(ByVal vehs As List(Of QuickQuoteVehicle))
            StateAndLobParts.Set_CPRPackageVehicles(vehs)
        End Sub
        Protected Friend Sub Set_CIMPackageVehicles(ByVal vehs As List(Of QuickQuoteVehicle))
            StateAndLobParts.Set_CIMPackageVehicles(vehs)
        End Sub
        Protected Friend Sub Set_CRMPackageVehicles(ByVal vehs As List(Of QuickQuoteVehicle))
            StateAndLobParts.Set_CRMPackageVehicles(vehs)
        End Sub
        Protected Friend Sub Set_GARPackageVehicles(ByVal vehs As List(Of QuickQuoteVehicle))
            StateAndLobParts.Set_GARPackageVehicles(vehs)
        End Sub
        'added 10/19/2018
        Protected Friend Sub Set_MasterPackageModifiers(ByVal mods As List(Of QuickQuoteModifier))
            StateAndLobParts.Set_MasterPackageModifiers(mods)
        End Sub
        Protected Friend Sub Set_CGLPackageModifiers(ByVal mods As List(Of QuickQuoteModifier))
            StateAndLobParts.Set_CGLPackageModifiers(mods)
        End Sub
        Protected Friend Sub Set_CPRPackageModifiers(ByVal mods As List(Of QuickQuoteModifier))
            StateAndLobParts.Set_CPRPackageModifiers(mods)
        End Sub
        Protected Friend Sub Set_CIMPackageModifiers(ByVal mods As List(Of QuickQuoteModifier))
            StateAndLobParts.Set_CIMPackageModifiers(mods)
        End Sub
        Protected Friend Sub Set_CRMPackageModifiers(ByVal mods As List(Of QuickQuoteModifier))
            StateAndLobParts.Set_CRMPackageModifiers(mods)
        End Sub
        Protected Friend Sub Set_GARPackageModifiers(ByVal mods As List(Of QuickQuoteModifier))
            StateAndLobParts.Set_GARPackageModifiers(mods)
        End Sub

        Public Overrides Function ToString() As String
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.Database_QuoteId <> "" Then
                    str = qqHelper.appendText(str, "QuoteId: " & Me.Database_QuoteId, vbCrLf)
                End If
                If Me.Database_QuoteXmlId <> "" Then
                    str = qqHelper.appendText(str, "QuoteXmlId: " & Me.Database_QuoteXmlId, vbCrLf)
                End If
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
                If Me.TotalQuotedPremium <> "" Then 'added 6/30/2015
                    str = qqHelper.appendText(str, "TotalQuotedPremium: " & Me.TotalQuotedPremium, vbCrLf)
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

                    '_XmlType = Nothing
                    'qqHelper.DisposeString(_CompanyId)
                    '_Success = Nothing
                    'qqHelper.DisposeString(_AgencyCode)
                    'qqHelper.DisposeString(_AgencyId)
                    'qqHelper.DisposeString(_AgencyProducerId)
                    'qqHelper.DisposeString(_AgencyProducerCode)
                    'If _AgencyProducerName IsNot Nothing Then
                    '    _AgencyProducerName.Dispose()
                    '    _AgencyProducerName = Nothing
                    'End If
                    'qqHelper.DisposeString(_QuoteNumber)
                    'qqHelper.DisposeString(_PolicyNumber)
                    'qqHelper.DisposeString(_QuoteDescription)
                    'qqHelper.DisposeString(_EffectiveDate)
                    'qqHelper.DisposeString(_ExpirationDate)
                    'qqHelper.DisposeString(_TotalQuotedPremium)
                    'If _ValidationItems IsNot Nothing Then
                    '    If _ValidationItems.Count > 0 Then
                    '        For Each val As QuickQuoteValidationItem In _ValidationItems
                    '            val.Dispose()
                    '            val = Nothing
                    '        Next
                    '        _ValidationItems.Clear()
                    '    End If
                    '    _ValidationItems = Nothing
                    'End If
                    'If _Client IsNot Nothing Then
                    '    _Client.Dispose()
                    '    _Client = Nothing
                    'End If
                    '_IsNew = Nothing
                    'qqHelper.DisposeString(_BillToId)
                    'qqHelper.DisposeString(_CurrentBilltoId)
                    'qqHelper.DisposeString(_CurrentPayplanId)
                    'qqHelper.DisposeString(_PolicyTermId)
                    'qqHelper.DisposeString(_ReceivedDate)
                    'qqHelper.DisposeString(_TransactionEffectiveDate)
                    'qqHelper.DisposeString(_TransactionExpirationDate)
                    'qqHelper.DisposeString(_TransactionTypeId)
                    'qqHelper.DisposeString(_TransactionUsersId)
                    'qqHelper.DisposeString(_WorkflowQueueId)
                    'If _Policyholder IsNot Nothing Then
                    '    _Policyholder.Dispose()
                    '    _Policyholder = Nothing
                    'End If
                    'If _Policyholder2 IsNot Nothing Then
                    '    _Policyholder2.Dispose()
                    '    _Policyholder2 = Nothing
                    'End If
                    'qqHelper.DisposeString(_BillMethodId)
                    'qqHelper.DisposeString(_BillingPayPlanId)
                    'qqHelper.DisposeString(_PolicyOriginTypeId)
                    '_HasInitiatedFinalize = Nothing
                    'qqHelper.DisposeString(_PolicyId)
                    'qqHelper.DisposeString(_PolicyImageNum)
                    'qqHelper.DisposeString(_PolicyBridgingURL)
                    'If _PaymentOptions IsNot Nothing Then
                    '    If _PaymentOptions.Count > 0 Then
                    '        For Each po As QuickQuotePaymentOption In _PaymentOptions
                    '            po.Dispose()
                    '            po = Nothing
                    '        Next
                    '        _PaymentOptions.Clear()
                    '    End If
                    '    _PaymentOptions = Nothing
                    'End If
                    '_CurrentlyParsingPaymentOptions = Nothing
                    'qqHelper.DisposeString(_ExperienceModificationFactor)
                    'qqHelper.DisposeString(_ExperienceModificationBureauTypeId)
                    'qqHelper.DisposeString(_ExperienceModificationRiskIdentifier)
                    'If _ExperienceModifications IsNot Nothing Then 'added 9/18/2017
                    '    If _ExperienceModifications.Count > 0 Then
                    '        For Each expMod As QuickQuoteExperienceModification In _ExperienceModifications
                    '            If expMod IsNot Nothing Then
                    '                expMod.Dispose()
                    '                expMod = Nothing
                    '            End If
                    '        Next
                    '        _ExperienceModifications.Clear()
                    '    End If
                    '    _ExperienceModifications = Nothing
                    'End If
                    '_CanUseExperienceModificationNumForExperienceModificationReconciliation = Nothing
                    '_HasConvertedExperienceModifications = Nothing
                    'qqHelper.DisposeIntegers(_DiamondExperienceModificationIndexesToUpdate)
                    'qqHelper.DisposeString(_GuaranteedRatePeriodEffectiveDate)
                    'qqHelper.DisposeString(_GuaranteedRatePeriodExpirationDate)
                    'qqHelper.DisposeString(_ModificationProductionDate)
                    'qqHelper.DisposeString(_RatingEffectiveDate)
                    'If _AdditionalPolicyholders IsNot Nothing Then
                    '    If _AdditionalPolicyholders.Count > 0 Then
                    '        For Each ph As QuickQuoteAdditionalPolicyholder In _AdditionalPolicyholders
                    '            ph.Dispose()
                    '            ph = Nothing
                    '        Next
                    '        _AdditionalPolicyholders.Clear()
                    '    End If
                    '    _AdditionalPolicyholders = Nothing
                    'End If
                    'qqHelper.DisposeString(_QuoteTypeId)
                    'If _PackageParts IsNot Nothing Then
                    '    If _PackageParts.Count > 0 Then
                    '        For Each pp As QuickQuotePackagePart In _PackageParts
                    '            pp.Dispose()
                    '            pp = Nothing
                    '        Next
                    '        _PackageParts.Clear()
                    '    End If
                    '    _PackageParts = Nothing
                    'End If
                    'qqHelper.DisposeString(_EFT_BankRoutingNumber)
                    'qqHelper.DisposeString(_EFT_BankAccountNumber)
                    'qqHelper.DisposeString(_EFT_BankAccountTypeId)
                    'qqHelper.DisposeString(_EFT_DeductionDay)
                    '_OnlyUsePropertyToSetFieldWithSameName = Nothing
                    'qqHelper.DisposeString(_Database_QuoteId)
                    'qqHelper.DisposeString(_Database_QuoteXmlId)
                    'qqHelper.DisposeString(_Database_QuoteNumber)
                    'qqHelper.DisposeString(_Database_LobId)
                    'qqHelper.DisposeString(_Database_CurrentQuoteXmlId)
                    'qqHelper.DisposeString(_Database_XmlQuoteId)
                    'qqHelper.DisposeString(_Database_LastAvailableQuoteNumber)
                    'qqHelper.DisposeString(_Database_QuoteStatusId)
                    'qqHelper.DisposeString(_Database_XmlStatusId)
                    '_Database_IsPolicy = Nothing
                    'qqHelper.DisposeString(_Database_DiamondPolicyNumber)
                    '_Database_OriginatedInVR = Nothing
                    'qqHelper.DisposeString(_Database_EffectiveDate)
                    'If _Agency IsNot Nothing Then
                    '    _Agency.Dispose()
                    '    _Agency = Nothing
                    'End If
                    'qqHelper.DisposeString(_PolicyImageId)
                    'If _BillingAddressee IsNot Nothing Then
                    '    _BillingAddressee.Dispose()
                    '    _BillingAddressee = Nothing
                    'End If
                    'qqHelper.DisposeString(_FirstWrittenDate)
                    '_QuoteTransactionType = Nothing
                    'qqHelper.DisposeString(_OriginalEffectiveDate)
                    'qqHelper.DisposeString(_OriginalExpirationDate)
                    'qqHelper.DisposeString(_OriginalTransactionEffectiveDate)
                    'qqHelper.DisposeString(_OriginalTransactionExpirationDate)
                    'qqHelper.DisposeString(_TransactionRemark)
                    'qqHelper.DisposeString(_TransactionReasonId)
                    'qqHelper.DisposeString(_AnnualPremium) 'PolicyImage.premium_annual
                    'qqHelper.DisposeString(_ChangeInFullTermPremium) 'PolicyImage.premium_chg_fullterm
                    'qqHelper.DisposeString(_ChangeInWrittenPremium) 'PolicyImage.premium_chg_written
                    'qqHelper.DisposeString(_DifferenceChangeInFullTermPremium) 'PolicyImage.premium_diff_chg_fullterm
                    'qqHelper.DisposeString(_DifferenceChangeInWrittenPremium) 'PolicyImage.premium_diff_chg_written
                    'qqHelper.DisposeString(_FullTermPremium) 'PolicyImage.premium_fullterm
                    'qqHelper.DisposeString(_FullTermPremiumOffsetForPreviousImage) 'PolicyImage.ftp_offset_for_prev_image
                    'qqHelper.DisposeString(_FullTermPremiumOnsetForCurrent) 'PolicyImage.ftp_onset_for_current
                    'qqHelper.DisposeString(_OffsetPremiumForPreviousImage) 'PolicyImage.offset_for_prev_image
                    'qqHelper.DisposeString(_OnsetPremiumForCurrentImage) 'PolicyImage.onset_for_current
                    'qqHelper.DisposeString(_PreviousWrittenPremium) 'PolicyImage.premium_previous_written
                    'qqHelper.DisposeString(_WrittenPremium) 'PolicyImage.premium_written
                    'qqHelper.DisposeString(_PriorTermAnnual) 'PolicyImage.prior_term_annual_premium
                    'qqHelper.DisposeString(_PriorTermFullterm) 'PolicyImage.prior_term_fullterm
                    'If _Comments IsNot Nothing Then
                    '    If _Comments.Count > 0 Then
                    '        For Each c As QuickQuoteComment In _Comments
                    '            If c IsNot Nothing Then
                    '                c.Dispose()
                    '                c = Nothing
                    '            End If
                    '        Next
                    '        _Comments.Clear()
                    '    End If
                    '    _Comments = Nothing
                    'End If
                    'qqHelper.DisposeMessages(_Messages)
                    'qqHelper.DisposeQuickQuoteObjects(_MultiStateQuotes)

                    'added 7/27/2018
                    If _QuoteBase IsNot Nothing Then
                        _QuoteBase.Dispose()
                        _QuoteBase = Nothing
                    End If
                    If _QuotePremiums IsNot Nothing Then
                        _QuotePremiums.Dispose()
                        _QuotePremiums = Nothing
                    End If
                    If _StateAndLobParts IsNot Nothing Then
                        _StateAndLobParts.Dispose()
                        _StateAndLobParts = Nothing
                    End If

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

