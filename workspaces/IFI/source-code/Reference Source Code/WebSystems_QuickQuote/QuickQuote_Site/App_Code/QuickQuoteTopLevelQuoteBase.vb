Imports Microsoft.VisualBasic
Imports System.Web
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store top-level base information for a quote; includes properties that were previously on QuickQuote only
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteTopLevelQuoteBase 'added 7/27/2018
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        'removed 8/18/2018
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

        'Private _EFT_BankRoutingNumber As String
        'Private _EFT_BankAccountNumber As String
        'Private _EFT_BankAccountTypeId As String
        'Private _EFT_DeductionDay As String

        'Private _OnlyUsePropertyToSetFieldWithSameName As Boolean

        'removed 8/18/2018
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

        'removed 8/18/2018
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

        'Private _Comments As List(Of QuickQuoteComment)

        'Private _Messages As List(Of QuickQuoteMessage)

        'Private _QuoteLevel As QuickQuoteHelperClass.QuoteLevel 'added 7/28/2018

        Private _DatabaseInfo As QuickQuoteTopLevelQuoteBase_DatabaseInfo 'added 8/18/2018
        Private _CommonInfo As QuickQuoteTopLevelQuoteBase_CommonInfo 'added 8/18/2018


        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property XmlType As QuickQuoteObject.QuickQuoteXmlType
            Get
                Return CommonInfo.XmlType
            End Get
            Set(value As QuickQuoteObject.QuickQuoteXmlType)
                CommonInfo.XmlType = value
            End Set
        End Property
        '<Script.Serialization.ScriptIgnore>
        '<System.Xml.Serialization.XmlIgnore()>
        'Public Property CompanyId As String 'removed 11/26/2022 (now on VersionAndLobInfo)
        '    Get
        '        Return CommonInfo.CompanyId
        '    End Get
        '    Set(value As String)
        '        CommonInfo.CompanyId = value
        '    End Set
        'End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Success As Boolean
            Get
                Return CommonInfo.Success
            End Get
            Set(value As Boolean)
                CommonInfo.Success = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AgencyCode As String
            Get
                Return CommonInfo.AgencyCode
            End Get
            Set(value As String)
                CommonInfo.AgencyCode = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AgencyId As String
            Get
                Return CommonInfo.AgencyId
            End Get
            Set(value As String)
                CommonInfo.AgencyId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AgencyProducerId As String
            Get
                Return CommonInfo.AgencyProducerId
            End Get
            Set(value As String)
                CommonInfo.AgencyProducerId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AgencyProducerCode As String
            Get
                Return CommonInfo.AgencyProducerCode
            End Get
            Set(value As String)
                CommonInfo.AgencyProducerCode = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AgencyProducerName As QuickQuoteName
            Get
                Return CommonInfo.AgencyProducerName
            End Get
            Set(value As QuickQuoteName)
                CommonInfo.AgencyProducerName = value
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
                Return CommonInfo.QuoteNumber
            End Get
            Set(value As String)
                CommonInfo.QuoteNumber = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Protected Friend ReadOnly Property QuoteNumber_Actual As String
            Get
                Return CommonInfo.QuoteNumber
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
                Return CommonInfo.PolicyNumber
            End Get
            Set(value As String)
                CommonInfo.PolicyNumber = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Protected Friend ReadOnly Property PolicyNumber_Actual As String
            Get
                Return CommonInfo.PolicyNumber
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property QuoteDescription As String
            Get
                Return CommonInfo.QuoteDescription
            End Get
            Set(value As String)
                CommonInfo.QuoteDescription = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property EffectiveDate As String
            Get
                Return CommonInfo.EffectiveDate
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
                CommonInfo.EffectiveDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ExpirationDate As String
            Get
                Return CommonInfo.ExpirationDate
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
                CommonInfo.ExpirationDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ValidationItems As Generic.List(Of QuickQuoteValidationItem)
            Get
                Return CommonInfo.ValidationItems
            End Get
            Set(value As Generic.List(Of QuickQuoteValidationItem))
                CommonInfo.ValidationItems = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Client As QuickQuoteClient
            Get
                Return CommonInfo.Client
            End Get
            Set(value As QuickQuoteClient)
                CommonInfo.Client = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property IsNew As Boolean
            Get
                Return CommonInfo.IsNew
            End Get
            Set(value As Boolean)
                CommonInfo.IsNew = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property BillToId As String
            Get
                Return CommonInfo.BillToId
            End Get
            Set(value As String)
                '_BillToId = value
                ''sets both when set by developer; only sets 1 when read from xml
                'If _OnlyUsePropertyToSetFieldWithSameName = False Then
                '    _CurrentBilltoId = value
                'Else 'don't set other value; set flag back to False after
                '    _OnlyUsePropertyToSetFieldWithSameName = False
                'End If
                CommonInfo.BillToId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CurrentBilltoId As String
            Get
                Return CommonInfo.CurrentBilltoId
            End Get
            Set(value As String)
                '_CurrentBilltoId = value
                ''sets both when set by developer; only sets 1 when read from xml
                'If _OnlyUsePropertyToSetFieldWithSameName = False Then
                '    _BillToId = value
                'Else 'don't set other value; set flag back to False after
                '    _OnlyUsePropertyToSetFieldWithSameName = False
                'End If
                CommonInfo.CurrentBilltoId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CurrentPayplanId As String 'triggered off this instead of BillingPayPlanId
            Get
                Return CommonInfo.CurrentPayplanId
            End Get
            Set(value As String)
                '_CurrentPayplanId = value
                ''sets both when set by developer; only sets 1 when read from xml
                'If _OnlyUsePropertyToSetFieldWithSameName = False Then
                '    _BillingPayPlanId = value
                'Else 'don't set other value; set flag back to False after
                '    _OnlyUsePropertyToSetFieldWithSameName = False
                'End If
                CommonInfo.CurrentPayplanId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyTermId As String
            Get
                Return CommonInfo.PolicyTermId
            End Get
            Set(value As String)
                CommonInfo.PolicyTermId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ReceivedDate As String
            Get
                Return CommonInfo.ReceivedDate
            End Get
            Set(value As String)
                CommonInfo.ReceivedDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TransactionEffectiveDate As String
            Get
                Return CommonInfo.TransactionEffectiveDate
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
                CommonInfo.TransactionEffectiveDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TransactionExpirationDate As String
            Get
                Return CommonInfo.TransactionExpirationDate
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
                CommonInfo.TransactionExpirationDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TransactionTypeId As String
            Get
                Return CommonInfo.TransactionTypeId
            End Get
            Set(value As String)
                CommonInfo.TransactionTypeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TransactionUsersId As String
            Get
                Return CommonInfo.TransactionUsersId
            End Get
            Set(value As String)
                CommonInfo.TransactionUsersId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property WorkflowQueueId As String
            Get
                Return CommonInfo.WorkflowQueueId
            End Get
            Set(value As String)
                CommonInfo.WorkflowQueueId = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Policyholder As QuickQuotePolicyholder
            Get
                Return CommonInfo.Policyholder
            End Get
            Set(value As QuickQuotePolicyholder)
                CommonInfo.Policyholder = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Policyholder2 As QuickQuotePolicyholder
            Get
                Return CommonInfo.Policyholder2
            End Get
            Set(value As QuickQuotePolicyholder)
                CommonInfo.Policyholder2 = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property BillMethodId As String
            Get
                Return CommonInfo.BillMethodId
            End Get
            Set(value As String)
                CommonInfo.BillMethodId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property BillingPayPlanId As String 'triggered off CurrentPayPlanId instead of this one
            Get
                Return CommonInfo.BillingPayPlanId
            End Get
            Set(value As String)
                '_BillingPayPlanId = value
                ''sets both when set by developer; only sets 1 when read from xml
                'If _OnlyUsePropertyToSetFieldWithSameName = False Then
                '    _CurrentPayplanId = value
                'Else 'don't set other value; set flag back to False after
                '    _OnlyUsePropertyToSetFieldWithSameName = False
                'End If
                CommonInfo.BillingPayPlanId = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyOriginTypeId As String
            Get
                Return CommonInfo.PolicyOriginTypeId
            End Get
            Set(value As String)
                CommonInfo.PolicyOriginTypeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property HasInitiatedFinalize As Boolean
            Get
                Return CommonInfo.HasInitiatedFinalize
            End Get
            Set(value As Boolean)
                CommonInfo.HasInitiatedFinalize = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyId As String
            Get
                Return CommonInfo.PolicyId
            End Get
            Set(value As String)
                CommonInfo.PolicyId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property RenewalVersion As String
            Get
                Return CommonInfo.RenewalVersion
            End Get
            Set(value As String)
                CommonInfo.RenewalVersion = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyImageNum As String
            Get
                Return CommonInfo.PolicyImageNum
            End Get
            Set(value As String)
                CommonInfo.PolicyImageNum = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyBridgingURL As String
            Get
                Return CommonInfo.PolicyBridgingURL
            End Get
            Set(value As String)
                CommonInfo.PolicyBridgingURL = value
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
                Return CommonInfo.PaymentOptions
            End Get
            Set(value As Generic.List(Of QuickQuotePaymentOption))
                CommonInfo.PaymentOptions = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ExperienceModificationFactor As String
            Get
                Return CommonInfo.ExperienceModificationFactor
            End Get
            Set(value As String)
                CommonInfo.ExperienceModificationFactor = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ExperienceModificationBureauTypeId As String
            Get
                Return CommonInfo.ExperienceModificationBureauTypeId
            End Get
            Set(value As String)
                CommonInfo.ExperienceModificationBureauTypeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ExperienceModificationRiskIdentifier As String
            Get
                Return CommonInfo.ExperienceModificationRiskIdentifier
            End Get
            Set(value As String)
                CommonInfo.ExperienceModificationRiskIdentifier = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ExperienceModifications As List(Of QuickQuoteExperienceModification)
            Get
                Return CommonInfo.ExperienceModifications
            End Get
            Set(value As List(Of QuickQuoteExperienceModification))
                CommonInfo.ExperienceModifications = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CanUseExperienceModificationNumForExperienceModificationReconciliation As Boolean
            Get
                Return CommonInfo.CanUseExperienceModificationNumForExperienceModificationReconciliation
            End Get
            Set(value As Boolean)
                CommonInfo.CanUseExperienceModificationNumForExperienceModificationReconciliation = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property HasConvertedExperienceModifications As Boolean
            Get
                Return CommonInfo.HasConvertedExperienceModifications
            End Get
            Set(value As Boolean)
                CommonInfo.HasConvertedExperienceModifications = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Protected Friend Property DiamondExperienceModificationIndexesToUpdate As List(Of Integer)
            Get
                Return CommonInfo.DiamondExperienceModificationIndexesToUpdate
            End Get
            Set(value As List(Of Integer))
                CommonInfo.DiamondExperienceModificationIndexesToUpdate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property GuaranteedRatePeriodEffectiveDate As String
            Get
                Return CommonInfo.GuaranteedRatePeriodEffectiveDate
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
                CommonInfo.GuaranteedRatePeriodEffectiveDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property GuaranteedRatePeriodExpirationDate As String
            Get
                Return CommonInfo.GuaranteedRatePeriodExpirationDate
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
                CommonInfo.GuaranteedRatePeriodExpirationDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ModificationProductionDate As String
            Get
                Return CommonInfo.ModificationProductionDate
            End Get
            Set(value As String)
                CommonInfo.ModificationProductionDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property RatingEffectiveDate As String
            Get
                Return CommonInfo.RatingEffectiveDate
            End Get
            Set(value As String)
                CommonInfo.RatingEffectiveDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AdditionalPolicyholders As Generic.List(Of QuickQuoteAdditionalPolicyholder)
            Get
                Return CommonInfo.AdditionalPolicyholders
            End Get
            Set(value As Generic.List(Of QuickQuoteAdditionalPolicyholder))
                CommonInfo.AdditionalPolicyholders = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property QuoteTypeId As String
            Get
                Return CommonInfo.QuoteTypeId
            End Get
            Set(value As String)
                CommonInfo.QuoteTypeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property EFT_BankRoutingNumber As String
            Get
                Return CommonInfo.EFT_BankRoutingNumber
            End Get
            Set(value As String)
                CommonInfo.EFT_BankRoutingNumber = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property EFT_BankAccountNumber As String
            Get
                Return CommonInfo.EFT_BankAccountNumber
            End Get
            Set(value As String)
                CommonInfo.EFT_BankAccountNumber = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property EFT_BankAccountTypeId As String '1=Checking; 2=Savings
            Get
                Return CommonInfo.EFT_BankAccountTypeId
            End Get
            Set(value As String)
                CommonInfo.EFT_BankAccountTypeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property EFT_DeductionDay As String
            Get
                Return CommonInfo.EFT_DeductionDay
            End Get
            Set(value As String)
                CommonInfo.EFT_DeductionDay = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property OnlyUsePropertyToSetFieldWithSameName As Boolean
            Get
                Return CommonInfo.OnlyUsePropertyToSetFieldWithSameName
            End Get
            Set(value As Boolean)
                CommonInfo.OnlyUsePropertyToSetFieldWithSameName = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuoteId As String
            Get
                Return DatabaseInfo.Database_QuoteId
            End Get
            Set(value As String)
                DatabaseInfo.Database_QuoteId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuoteXmlId As String
            Get
                Return DatabaseInfo.Database_QuoteXmlId
            End Get
            Set(value As String)
                DatabaseInfo.Database_QuoteXmlId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuoteNumber As String
            Get
                Return DatabaseInfo.Database_QuoteNumber
            End Get
            Set(value As String)
                DatabaseInfo.Database_QuoteNumber = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_LobId As String
            Get
                Return DatabaseInfo.Database_LobId
            End Get
            Set(value As String)
                DatabaseInfo.Database_LobId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_CurrentQuoteXmlId As String
            Get
                Return DatabaseInfo.Database_CurrentQuoteXmlId
            End Get
            Set(value As String)
                DatabaseInfo.Database_CurrentQuoteXmlId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_XmlQuoteId As String
            Get
                Return DatabaseInfo.Database_XmlQuoteId
            End Get
            Set(value As String)
                DatabaseInfo.Database_XmlQuoteId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_LastAvailableQuoteNumber As String
            Get
                Return DatabaseInfo.Database_LastAvailableQuoteNumber
            End Get
            Set(value As String)
                DatabaseInfo.Database_LastAvailableQuoteNumber = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuoteStatusId As String
            Get
                Return DatabaseInfo.Database_QuoteStatusId
            End Get
            Set(value As String)
                DatabaseInfo.Database_QuoteStatusId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_XmlStatusId As String
            Get
                Return DatabaseInfo.Database_XmlStatusId
            End Get
            Set(value As String)
                DatabaseInfo.Database_XmlStatusId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_IsPolicy As Boolean
            Get
                Return DatabaseInfo.Database_IsPolicy
            End Get
            Set(value As Boolean)
                DatabaseInfo.Database_IsPolicy = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_DiamondPolicyNumber As String
            Get
                Return DatabaseInfo.Database_DiamondPolicyNumber
            End Get
            Set(value As String)
                DatabaseInfo.Database_DiamondPolicyNumber = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_OriginatedInVR As Boolean
            Get
                Return DatabaseInfo.Database_OriginatedInVR
            End Get
            Set(value As Boolean)
                DatabaseInfo.Database_OriginatedInVR = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_EffectiveDate As String
            Get
                Return DatabaseInfo.Database_EffectiveDate
            End Get
            Set(value As String)
                DatabaseInfo.Database_EffectiveDate = value
            End Set
        End Property
        'added 9/28/2018
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_ActualLobId As String
            Get
                Return DatabaseInfo.Database_ActualLobId
            End Get
            Set(value As String)
                DatabaseInfo.Database_ActualLobId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_GoverningStateId As String
            Get
                Return DatabaseInfo.Database_GoverningStateId
            End Get
            Set(value As String)
                DatabaseInfo.Database_GoverningStateId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_StateIds As String
            Get
                Return DatabaseInfo.Database_StateIds
            End Get
            Set(value As String)
                DatabaseInfo.Database_StateIds = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuoteActualLobId As String
            Get
                Return DatabaseInfo.Database_QuoteActualLobId
            End Get
            Set(value As String)
                DatabaseInfo.Database_QuoteActualLobId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuoteGoverningStateId As String
            Get
                Return DatabaseInfo.Database_QuoteGoverningStateId
            End Get
            Set(value As String)
                DatabaseInfo.Database_QuoteGoverningStateId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuoteStateIds As String
            Get
                Return DatabaseInfo.Database_QuoteStateIds
            End Get
            Set(value As String)
                DatabaseInfo.Database_QuoteStateIds = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_AppActualLobId As String
            Get
                Return DatabaseInfo.Database_AppActualLobId
            End Get
            Set(value As String)
                DatabaseInfo.Database_AppActualLobId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_AppGoverningStateId As String
            Get
                Return DatabaseInfo.Database_AppGoverningStateId
            End Get
            Set(value As String)
                DatabaseInfo.Database_AppGoverningStateId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_AppStateIds As String
            Get
                Return DatabaseInfo.Database_AppStateIds
            End Get
            Set(value As String)
                DatabaseInfo.Database_AppStateIds = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Agency As QuickQuoteAgency
            Get
                Return CommonInfo.Agency
            End Get
            Set(value As QuickQuoteAgency)
                CommonInfo.Agency = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyImageId As String
            Get
                Return CommonInfo.PolicyImageId
            End Get
            Set(value As String)
                CommonInfo.PolicyImageId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property BillingAddressee As QuickQuoteBillingAddressee
            Get
                Return CommonInfo.BillingAddressee
            End Get
            Set(value As QuickQuoteBillingAddressee)
                CommonInfo.BillingAddressee = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property FirstWrittenDate As String 'will only be used if it's there... else will keep Diamond default
            Get
                Return CommonInfo.FirstWrittenDate
            End Get
            Set(value As String)
                CommonInfo.FirstWrittenDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property QuoteTransactionType As QuickQuoteObject.QuickQuoteTransactionType
            Get
                Return CommonInfo.QuoteTransactionType
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property OriginalEffectiveDate As String
            Get
                Return CommonInfo.OriginalEffectiveDate
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property OriginalExpirationDate As String
            Get
                Return CommonInfo.OriginalExpirationDate
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property OriginalTransactionEffectiveDate As String
            Get
                Return CommonInfo.OriginalTransactionEffectiveDate
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property OriginalTransactionExpirationDate As String
            Get
                Return CommonInfo.OriginalTransactionExpirationDate
            End Get
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TransactionRemark As String
            Get
                Return CommonInfo.TransactionRemark
            End Get
            Set(value As String)
                CommonInfo.TransactionRemark = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TransactionReasonId As String
            Get
                Return CommonInfo.TransactionReasonId
            End Get
            Set(value As String)
                CommonInfo.TransactionReasonId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Comments As List(Of QuickQuoteComment)
            Get
                Return CommonInfo.Comments
            End Get
            Set(value As List(Of QuickQuoteComment))
                CommonInfo.Comments = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Messages As List(Of QuickQuoteMessage) 'TODO: Dan - Parent?
            Get
                Return CommonInfo.Messages
            End Get
            Set(value As List(Of QuickQuoteMessage))
                CommonInfo.Messages = value
            End Set
        End Property

        'added 3/19/2019
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_DiamondImageInfoId As Integer
            Get
                Return DatabaseInfo.Database_DiamondImageInfoId
            End Get
            Set(value As Integer)
                DatabaseInfo.Database_DiamondImageInfoId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_DiamondImageXmlId As Integer
            Get
                Return DatabaseInfo.Database_DiamondImageXmlId
            End Get
            Set(value As Integer)
                DatabaseInfo.Database_DiamondImageXmlId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_DiamondImageInfoType As QuickQuoteDiamondImageInfo.ImageInfoType
            Get
                Return DatabaseInfo.Database_DiamondImageInfoType
            End Get
            Set(value As QuickQuoteDiamondImageInfo.ImageInfoType)
                DatabaseInfo.Database_DiamondImageInfoType = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_IsBillingUpdate As Boolean
            Get
                Return DatabaseInfo.Database_IsBillingUpdate
            End Get
            Set(value As Boolean)
                DatabaseInfo.Database_IsBillingUpdate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_EndorsementOrigin As QuickQuoteEndorsementForPolicyIdAndTransactionDateInput.EndorsementOriginTypes
            Get
                Return DatabaseInfo.Database_EndorsementOrigin
            End Get
            Set(value As QuickQuoteEndorsementForPolicyIdAndTransactionDateInput.EndorsementOriginTypes)
                DatabaseInfo.Database_EndorsementOrigin = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_DevDictionaryID As Integer
            Get
                Return DatabaseInfo.Database_DevDictionaryID
            End Get
            Set(value As Integer)
                DatabaseInfo.Database_DevDictionaryID = value
            End Set
        End Property

        'added 1/4/2020 (Interoperability)
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_LastRulesOverrideRecordModifiedDate As String
            Get
                Return DatabaseInfo.Database_LastRulesOverrideRecordModifiedDate
            End Get
            Set(value As String)
                DatabaseInfo.Database_LastRulesOverrideRecordModifiedDate = value
            End Set
        End Property

        '<Script.Serialization.ScriptIgnore>
        '<System.Xml.Serialization.XmlIgnore()>
        'Public ReadOnly Property QuoteLevel As QuickQuoteHelperClass.QuoteLevel 'added 7/28/2018; 12/30/2018 - moved to StateAndLobParts object so it would not get Copied between topLevel and stateLevel quotes
        '    Get
        '        Return CommonInfo.QuoteLevel
        '    End Get
        'End Property

        'added 5/22/2019
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AddedDate As String
            Get
                Return CommonInfo.AddedDate
            End Get
            Set(value As String)
                CommonInfo.AddedDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property LastModifiedDate As String
            Get
                Return CommonInfo.LastModifiedDate
            End Get
            Set(value As String)
                CommonInfo.LastModifiedDate = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PCAdded_Date As String
            Get
                Return CommonInfo.PCAdded_Date
            End Get
            Set(value As String)
                CommonInfo.PCAdded_Date = value
            End Set
        End Property

        'added 6/15/2019
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyCurrentStatusId As String
            Get
                Return CommonInfo.PolicyCurrentStatusId
            End Get
            Set(value As String)
                CommonInfo.PolicyCurrentStatusId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyStatusCodeId As String
            Get
                Return CommonInfo.PolicyStatusCodeId
            End Get
            Set(value As String)
                CommonInfo.PolicyStatusCodeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property CancelDate As String
            Get
                Return CommonInfo.CancelDate
            End Get
            Set(value As String)
                CommonInfo.CancelDate = value
            End Set
        End Property

        'added 5/13/2021
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuickQuote_Inserted As String
            Get
                Return DatabaseInfo.Database_QuickQuote_Inserted
            End Get
            Set(value As String)
                DatabaseInfo.Database_QuickQuote_Inserted = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuickQuote_Updated As String
            Get
                Return DatabaseInfo.Database_QuickQuote_Updated
            End Get
            Set(value As String)
                DatabaseInfo.Database_QuickQuote_Updated = value
            End Set
        End Property

        'added 11/27/2022
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_CompanyId As String
            Get
                Return DatabaseInfo.Database_CompanyId
            End Get
            Set(value As String)
                DatabaseInfo.Database_CompanyId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_QuoteCompanyId As String
            Get
                Return DatabaseInfo.Database_QuoteCompanyId
            End Get
            Set(value As String)
                DatabaseInfo.Database_QuoteCompanyId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_AppCompanyId As String
            Get
                Return DatabaseInfo.Database_AppCompanyId
            End Get
            Set(value As String)
                DatabaseInfo.Database_AppCompanyId = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Database_DiaCompanyId As String 'added 7/27/2023
            Get
                Return DatabaseInfo.Database_DiaCompanyId
            End Get
            Set(value As String)
                DatabaseInfo.Database_DiaCompanyId = value
            End Set
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property Database_DiaCompany As QuickQuoteHelperClass.QuickQuoteCompany 'added 7/27/2023
            Get
                Return DatabaseInfo.Database_DiaCompany
            End Get
        End Property

        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Protected Friend Property InternalDevDictionary As QuickQuoteDevDictionaryList
            Get
                Return CommonInfo.InternalDevDictionary
            End Get
            Set(value As QuickQuoteDevDictionaryList)
                CommonInfo.InternalDevDictionary = value
            End Set
        End Property

        Public Property DatabaseInfo As QuickQuoteTopLevelQuoteBase_DatabaseInfo 'added 8/18/2018
            Get
                If _DatabaseInfo Is Nothing Then
                    _DatabaseInfo = New QuickQuoteTopLevelQuoteBase_DatabaseInfo
                End If
                SetObjectsParent(_DatabaseInfo)
                Return _DatabaseInfo
            End Get
            Set(value As QuickQuoteTopLevelQuoteBase_DatabaseInfo)
                _DatabaseInfo = value
                SetObjectsParent(_DatabaseInfo)
            End Set
        End Property
        Public Property CommonInfo As QuickQuoteTopLevelQuoteBase_CommonInfo 'added 8/18/2018
            Get
                If _CommonInfo Is Nothing Then
                    _CommonInfo = New QuickQuoteTopLevelQuoteBase_CommonInfo
                End If
                SetObjectsParent(_CommonInfo)
                Return _CommonInfo
            End Get
            Set(value As QuickQuoteTopLevelQuoteBase_CommonInfo)
                _CommonInfo = value
                SetObjectsParent(_CommonInfo)
            End Set
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
            _DatabaseInfo = New QuickQuoteTopLevelQuoteBase_DatabaseInfo 'added 8/18/2018
            _CommonInfo = New QuickQuoteTopLevelQuoteBase_CommonInfo 'added 8/18/2018

            'removed 8/18/2018
            '_XmlType = QuickQuoteObject.QuickQuoteXmlType.None
            '_CompanyId = "1" '12/11/2013 note: not worth using xml right now; company_code is blank for both company_id 0 and 1; 1 has am_best_number 2251

            '_Success = False

            '_AgencyCode = ""
            '_AgencyId = ""
            'If System.Web.HttpContext.Current.Session("DiamondAgencyCode") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondAgencyCode").ToString <> "" Then
            '    _AgencyCode = System.Web.HttpContext.Current.Session("DiamondAgencyCode").ToString
            'ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
            '    _AgencyCode = ConfigurationManager.AppSettings("QuickQuoteTestAgencyCode").ToString
            'End If
            'If System.Web.HttpContext.Current.Session("DiamondAgencyId") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondAgencyId").ToString <> "" AndAlso IsNumeric(System.Web.HttpContext.Current.Session("DiamondAgencyId").ToString) = True Then
            '    _AgencyId = System.Web.HttpContext.Current.Session("DiamondAgencyId").ToString
            'ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
            '    _AgencyId = ConfigurationManager.AppSettings("QuickQuoteTestAgencyId").ToString
            'End If
            ''8/13/2014 note: could update to use new shared functions in QuickQuoteHelperClass

            ''added for staff
            'If _AgencyCode = "" OrElse _AgencyId = "" Then
            '    qqHelper.SetUserAgencyVariables()
            '    If System.Web.HttpContext.Current.Session("DiamondAgencyCode") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondAgencyCode").ToString <> "" Then
            '        _AgencyCode = System.Web.HttpContext.Current.Session("DiamondAgencyCode").ToString
            '    End If
            '    If System.Web.HttpContext.Current.Session("DiamondAgencyId") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondAgencyId").ToString <> "" AndAlso IsNumeric(System.Web.HttpContext.Current.Session("DiamondAgencyId").ToString) = True Then
            '        _AgencyId = System.Web.HttpContext.Current.Session("DiamondAgencyId").ToString
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

            '_EFT_BankRoutingNumber = ""
            '_EFT_BankAccountNumber = ""
            '_EFT_BankAccountTypeId = ""
            '_EFT_DeductionDay = ""

            '_OnlyUsePropertyToSetFieldWithSameName = False

            'Reset_Database_Values() 'removed 8/18/2018

            'removed 8/18/2018
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

            '_Comments = Nothing

            '_Messages = Nothing

            '_QuoteLevel = QuickQuoteHelperClass.QuoteLevel.None

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
            'updated 8/18/2018
            CommonInfo.Reset_Database_Values()

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

            'updated 8/18/2018
            DatabaseInfo.Reset_Database_Values()
        End Sub
        Protected Friend Sub Set_QuoteTransactionType(ByVal qqTranType As QuickQuoteObject.QuickQuoteTransactionType, Optional ByVal setTransactionTypeIdForNewBusinessQuoteAndEndorsement As Boolean = False)
            '_QuoteTransactionType = qqTranType
            ''VersionAndLobInfo.Set_QuoteTransactionType(_QuoteTransactionType) 'this is still being done from same method on QuickQuoteObject
            'If setTransactionTypeIdForNewBusinessQuoteAndEndorsement = True Then
            '    'note: ReadOnlyImage should not reset anything
            '    If _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote Then
            '        _TransactionTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.TransactionTypeId, "New Business") '2 for Diamond
            '    ElseIf _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
            '        _TransactionTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.TransactionTypeId, "Endorsement") '3 for Diamond
            '    End If
            'End If
            CommonInfo.Set_QuoteTransactionType(qqTranType, setTransactionTypeIdForNewBusinessQuoteAndEndorsement:=setTransactionTypeIdForNewBusinessQuoteAndEndorsement)
        End Sub
        Protected Friend Sub Set_OriginalEffectiveDate(ByVal origEffDate As String)
            '_OriginalEffectiveDate = origEffDate
            CommonInfo.Set_OriginalEffectiveDate(origEffDate)
        End Sub
        Protected Friend Sub Set_OriginalExpirationDate(ByVal origExpDate As String)
            '_OriginalExpirationDate = origExpDate
            CommonInfo.Set_OriginalExpirationDate(origExpDate)
        End Sub
        Protected Friend Sub Set_OriginalTransactionEffectiveDate(ByVal origTEffDate As String)
            '_OriginalTransactionEffectiveDate = origTEffDate
            CommonInfo.Set_OriginalTransactionEffectiveDate(origTEffDate)
        End Sub
        Protected Friend Sub Set_OriginalTransactionExpirationDate(ByVal origTExpDate As String)
            '_OriginalTransactionExpirationDate = origTExpDate
            CommonInfo.Set_OriginalTransactionExpirationDate(origTExpDate)
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
            CommonInfo.Default_PolicyOriginTypeId()
        End Sub
        Protected Friend Sub Set_QuoteStatus(ByVal status As QuickQuoteXML.QuickQuoteStatusType)
            '_Database_QuoteStatusId = CInt(status)
            'updated 8/18/2018
            DatabaseInfo.Set_QuoteStatus(status)
        End Sub

        Protected Friend Sub Set_CurrentlyParsingPaymentOptions(ByVal currentlyParsing As Boolean)
            '_CurrentlyParsingPaymentOptions = currentlyParsing
            CommonInfo.Set_CurrentlyParsingPaymentOptions(currentlyParsing)
        End Sub

        Protected Friend Function Get_PaymentOptions_Variable() As List(Of QuickQuotePaymentOption)
            'Return _PaymentOptions
            Return CommonInfo.Get_PaymentOptions_Variable()
        End Function
        Protected Friend Sub NumberPaymentOptionsIfPresent()
            'If _PaymentOptions IsNot Nothing AndAlso _PaymentOptions.Count > 0 Then
            '    qqHelper.NumberPaymentOptions(_PaymentOptions)
            'End If
            CommonInfo.NumberPaymentOptionsIfPresent()
        End Sub

        'Protected Friend Sub Set_QuoteLevel(ByVal level As QuickQuoteHelperClass.QuoteLevel) 'added 7/28/2018; 12/30/2018 - moved to StateAndLobParts object so it would not get Copied between topLevel and stateLevel quotes
        '    '_QuoteLevel = level
        '    CommonInfo.Set_QuoteLevel(level)
        'End Sub


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

                    'removed 8/18/2018
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
                    'qqHelper.DisposeString(_EFT_BankRoutingNumber)
                    'qqHelper.DisposeString(_EFT_BankAccountNumber)
                    'qqHelper.DisposeString(_EFT_BankAccountTypeId)
                    'qqHelper.DisposeString(_EFT_DeductionDay)
                    '_OnlyUsePropertyToSetFieldWithSameName = Nothing
                    'removed 8/18/2018
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
                    'removed 8/18/2018
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

                    '_QuoteLevel = Nothing 'added 7/28/2018

                    If _DatabaseInfo IsNot Nothing Then 'added 8/18/2018
                        _DatabaseInfo.Dispose()
                        _DatabaseInfo = Nothing
                    End If
                    If _CommonInfo IsNot Nothing Then 'added 8/18/2018
                        _CommonInfo.Dispose()
                        _CommonInfo = Nothing
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


