Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods 'added 4/17/2015

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store QuickQuote database information for a quote
    ''' </summary>
    ''' <remarks>used when making VR database calls</remarks>
    <Serializable()> _
    Public Class QuickQuoteDatabaseRecord 'added 4/16/2015
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass 'added 4/17/2015

        Private _quoteId As String 'added 4/17/2015
        Private _lobId As String
        Private _agencyId As String
        Private _agencyCode As String
        Private _initialUserId As String
        Private _userId As String
        Private _quoteXmlId As String
        Private _clientId As String
        Private _clientDisplayName As String
        Private _quoteNumber As String
        Private _premium As String
        Private _CPP_CGL_premium As String 'added 11/30/2012
        Private _CPP_CPR_premium As String 'added 11/30/2012
        'updated 1/26/2015 for CIM and CRM; db not updated yet
        Private _CPP_CIM_premium As String
        Private _CPP_CRM_premium As String
        Private _quoteStatusId As String
        Private _quoteStatus As String
        Private _quoteUpdated As String
        Private _quoteInserted As String

        'updated 12/5/2012 for Risk Grade 3 Error
        Private _errorRiskGradeLookupId As String
        Private _replacementRiskGradeLookupId As String
        Private _CPP_CGL_errorRiskGradeLookupId As String
        Private _CPP_CGL_replacementRiskGradeLookupId As String
        Private _CPP_CPR_errorRiskGradeLookupId As String
        Private _CPP_CPR_replacementRiskGradeLookupId As String
        'updated 1/26/2015 for CIM and CRM; db not updated yet
        Private _CPP_CIM_errorRiskGradeLookupId As String
        Private _CPP_CIM_replacementRiskGradeLookupId As String
        Private _CPP_CRM_errorRiskGradeLookupId As String
        Private _CPP_CRM_replacementRiskGradeLookupId As String

        Private _description As String
        Private _initialClientId As String
        Private _initialClientDisplayName As String
        Private _initialQuoteNumber As String
        Private _quoteUserId As String
        Private _quoteXml As Byte()
        Private _ratedQuoteXml As Byte()
        Private _ratedQuoteSuccess As Boolean
        Private _ratedQuotePremium As String
        Private _ratedQuote_CPP_CGL_Premium As String 'added 11/30/2012
        Private _ratedQuote_CPP_CPR_Premium As String 'added 11/30/2012
        'updated 4/17/2015 for CIM and CRM
        Private _ratedQuote_CPP_CIM_Premium As String
        Private _ratedQuote_CPP_CRM_Premium As String
        Private _ratedClientId As String
        Private _ratedClientDisplayName As String
        Private _ratedQuoteNumber As String
        Private _appGapClientId As String
        Private _appGapClientDisplayName As String
        Private _appGapQuoteNumber As String
        Private _appGapUserId As String
        Private _appGapXml As Byte()
        Private _ratedAppGapXml As Byte()
        Private _ratedAppGapSuccess As Boolean
        Private _ratedAppGapPremium As String
        Private _ratedAppGap_CPP_CGL_Premium As String 'added 11/30/2012
        Private _ratedAppGap_CPP_CPR_Premium As String 'added 11/30/2012
        'updated 4/17/2015 for CIM and CRM
        Private _ratedAppGap_CPP_CIM_Premium As String
        Private _ratedAppGap_CPP_CRM_Premium As String
        Private _ratedAppGapClientId As String
        Private _ratedAppGapClientDisplayName As String
        Private _ratedAppGapQuoteNumber As String
        Private _xmlStatusId As String
        Private _xmlUpdated As String
        Private _xmlInserted As String
        Private _xmlStatus As String

        'added 2/7/2013
        Private _currentQuoteXmlId As String
        Private _xmlQuoteId As String

        'added 9/23/2014
        Private _diaPolicyNumber As String
        Private _diaPolicyNumberDescription As String
        Private _isPolicy As Boolean
        Private _originatedInVR As Boolean 'added 1/8/2016

        'added 11/29/2017
        Private _policyId As String
        Private _ratedQuotePolicyId As String
        Private _ratedAppGapPolicyId As String

        ''added 03/23/2018
        Private _effectiveDate As String
        Private _quoteEffectiveDate As String
        Private _appGapEffectiveDate As String

        'added 9/28/2018
        Private _actualLobId As String
        Private _governingStateId As String
        Private _stateIds As String
        Private _quoteActualLobId As String
        Private _quoteGoverningStateId As String
        Private _quoteStateIds As String
        Private _appActualLobId As String
        Private _appGoverningStateId As String
        Private _appStateIds As String

        'added 01/02/2020 - DJG
        Private _eSigInfo As New QuickQuoteEsignatureInformation

        'added 1/4/2020 (Interoperability)
        Private _LastRulesOverrideRecordModifiedDate As String

        'added 4/21/2021 (Umbrella)
        Private _programTypeId As String

        'added 11/27/2022
        Private _companyId As String
        Private _quoteCompanyId As String
        Private _appCompanyId As String

        'added 7/27/2023
        Private _diaCompanyId As String

        Public Property quoteId As String 'added 4/17/2015
            Get
                Return _quoteId
            End Get
            Set(value As String)
                _quoteId = value
            End Set
        End Property
        Public Property lobId As String
            Get
                Return _lobId
            End Get
            Set(value As String)
                _lobId = value
            End Set
        End Property
        Public Property agencyId As String
            Get
                Return _agencyId
            End Get
            Set(value As String)
                _agencyId = value
            End Set
        End Property
        Public Property agencyCode As String
            Get
                Return _agencyCode
            End Get
            Set(value As String)
                _agencyCode = value
            End Set
        End Property
        Public Property initialUserId As String
            Get
                Return _initialUserId
            End Get
            Set(value As String)
                _initialUserId = value
            End Set
        End Property
        Public Property userId As String
            Get
                Return _userId
            End Get
            Set(value As String)
                _userId = value
            End Set
        End Property
        Public Property quoteXmlId As String
            Get
                Return _quoteXmlId
            End Get
            Set(value As String)
                _quoteXmlId = value
            End Set
        End Property
        Public Property clientId As String
            Get
                Return _clientId
            End Get
            Set(value As String)
                _clientId = value
            End Set
        End Property
        Public Property clientDisplayName As String
            Get
                Return _clientDisplayName
            End Get
            Set(value As String)
                _clientDisplayName = value
            End Set
        End Property
        Public Property quoteNumber As String
            Get
                Return _quoteNumber
            End Get
            Set(value As String)
                _quoteNumber = value
            End Set
        End Property
        Public Property premium As String
            Get
                Return _premium
            End Get
            Set(value As String)
                _premium = value
            End Set
        End Property
        Public Property CPP_CGL_premium As String 'added 11/30/2012
            Get
                Return _CPP_CGL_premium
            End Get
            Set(value As String)
                _CPP_CGL_premium = value
            End Set
        End Property
        Public Property CPP_CPR_premium As String 'added 11/30/2012
            Get
                Return _CPP_CPR_premium
            End Get
            Set(value As String)
                _CPP_CPR_premium = value
            End Set
        End Property
        'updated 1/26/2015 for CIM and CRM; db not updated yet
        Public Property CPP_CIM_premium As String
            Get
                Return _CPP_CIM_premium
            End Get
            Set(value As String)
                _CPP_CIM_premium = value
            End Set
        End Property
        Public Property CPP_CRM_premium As String
            Get
                Return _CPP_CRM_premium
            End Get
            Set(value As String)
                _CPP_CRM_premium = value
            End Set
        End Property
        Public Property quoteStatusId As String
            Get
                Return _quoteStatusId
            End Get
            Set(value As String)
                _quoteStatusId = value
            End Set
        End Property
        Public Property quoteStatus As String
            Get
                Return _quoteStatus
            End Get
            Set(value As String)
                _quoteStatus = value
            End Set
        End Property
        Public Property quoteUpdated As String
            Get
                Return _quoteUpdated
            End Get
            Set(value As String)
                _quoteUpdated = value
            End Set
        End Property
        Public Property quoteInserted As String
            Get
                Return _quoteInserted
            End Get
            Set(value As String)
                _quoteInserted = value
            End Set
        End Property

        'updated 12/5/2012 for Risk Grade 3 Error
        Public Property errorRiskGradeLookupId As String
            Get
                Return _errorRiskGradeLookupId
            End Get
            Set(value As String)
                _errorRiskGradeLookupId = value
            End Set
        End Property
        Public Property replacementRiskGradeLookupId As String
            Get
                Return _replacementRiskGradeLookupId
            End Get
            Set(value As String)
                _replacementRiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CGL_errorRiskGradeLookupId As String
            Get
                Return _CPP_CGL_errorRiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CGL_errorRiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CGL_replacementRiskGradeLookupId As String
            Get
                Return _CPP_CGL_replacementRiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CGL_replacementRiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CPR_errorRiskGradeLookupId As String
            Get
                Return _CPP_CPR_errorRiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CPR_errorRiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CPR_replacementRiskGradeLookupId As String
            Get
                Return _CPP_CPR_replacementRiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CPR_replacementRiskGradeLookupId = value
            End Set
        End Property
        'updated 1/26/2015 for CIM and CRM; db not updated yet
        Public Property CPP_CIM_errorRiskGradeLookupId As String
            Get
                Return _CPP_CIM_errorRiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CIM_errorRiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CIM_replacementRiskGradeLookupId As String
            Get
                Return _CPP_CIM_replacementRiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CIM_replacementRiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CRM_errorRiskGradeLookupId As String
            Get
                Return _CPP_CRM_errorRiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CRM_errorRiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CRM_replacementRiskGradeLookupId As String
            Get
                Return _CPP_CRM_replacementRiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CRM_replacementRiskGradeLookupId = value
            End Set
        End Property

        Public Property description As String
            Get
                Return _description
            End Get
            Set(value As String)
                _description = value
            End Set
        End Property
        Public Property initialClientId As String
            Get
                Return _initialClientId
            End Get
            Set(value As String)
                _initialClientId = value
            End Set
        End Property
        Public Property initialClientDisplayName As String
            Get
                Return _initialClientDisplayName
            End Get
            Set(value As String)
                _initialClientDisplayName = value
            End Set
        End Property
        Public Property initialQuoteNumber As String
            Get
                Return _initialQuoteNumber
            End Get
            Set(value As String)
                _initialQuoteNumber = value
            End Set
        End Property
        Public Property quoteUserId As String
            Get
                Return _quoteUserId
            End Get
            Set(value As String)
                _quoteUserId = value
            End Set
        End Property
        Public Property quoteXml As Byte()
            Get
                Return _quoteXml
            End Get
            Set(value As Byte())
                _quoteXml = value
            End Set
        End Property
        Public Property ratedQuoteXml As Byte()
            Get
                Return _ratedQuoteXml
            End Get
            Set(value As Byte())
                _ratedQuoteXml = value
            End Set
        End Property
        Public Property ratedQuoteSuccess As Boolean
            Get
                Return _ratedQuoteSuccess
            End Get
            Set(value As Boolean)
                _ratedQuoteSuccess = value
            End Set
        End Property
        Public Property ratedQuotePremium As String
            Get
                Return _ratedQuotePremium
            End Get
            Set(value As String)
                _ratedQuotePremium = value
            End Set
        End Property
        Public Property ratedQuote_CPP_CGL_Premium As String 'added 11/30/2012
            Get
                Return _ratedQuote_CPP_CGL_Premium
            End Get
            Set(value As String)
                _ratedQuote_CPP_CGL_Premium = value
            End Set
        End Property
        Public Property ratedQuote_CPP_CPR_Premium As String 'added 11/30/2012
            Get
                Return _ratedQuote_CPP_CPR_Premium
            End Get
            Set(value As String)
                _ratedQuote_CPP_CPR_Premium = value
            End Set
        End Property
        'updated 4/17/2015 for CIM and CRM
        Public Property ratedQuote_CPP_CIM_Premium As String
            Get
                Return _ratedQuote_CPP_CIM_Premium
            End Get
            Set(value As String)
                _ratedQuote_CPP_CIM_Premium = value
            End Set
        End Property
        Public Property ratedQuote_CPP_CRM_Premium As String
            Get
                Return _ratedQuote_CPP_CRM_Premium
            End Get
            Set(value As String)
                _ratedQuote_CPP_CRM_Premium = value
            End Set
        End Property
        Public Property ratedClientId As String
            Get
                Return _ratedClientId
            End Get
            Set(value As String)
                _ratedClientId = value
            End Set
        End Property
        Public Property ratedClientDisplayName As String
            Get
                Return _ratedClientDisplayName
            End Get
            Set(value As String)
                _ratedClientDisplayName = value
            End Set
        End Property
        Public Property ratedQuoteNumber As String
            Get
                Return _ratedQuoteNumber
            End Get
            Set(value As String)
                _ratedQuoteNumber = value
            End Set
        End Property
        Public Property appGapClientId As String
            Get
                Return _appGapClientId
            End Get
            Set(value As String)
                _appGapClientId = value
            End Set
        End Property
        Public Property appGapClientDisplayName As String
            Get
                Return _appGapClientDisplayName
            End Get
            Set(value As String)
                _appGapClientDisplayName = value
            End Set
        End Property
        Public Property appGapQuoteNumber As String
            Get
                Return _appGapQuoteNumber
            End Get
            Set(value As String)
                _appGapQuoteNumber = value
            End Set
        End Property
        Public Property appGapUserId As String
            Get
                Return _appGapUserId
            End Get
            Set(value As String)
                _appGapUserId = value
            End Set
        End Property
        Public Property appGapXml As Byte()
            Get
                Return _appGapXml
            End Get
            Set(value As Byte())
                _appGapXml = value
            End Set
        End Property
        Public Property ratedAppGapXml As Byte()
            Get
                Return _ratedAppGapXml
            End Get
            Set(value As Byte())
                _ratedAppGapXml = value
            End Set
        End Property
        Public Property ratedAppGapSuccess As Boolean
            Get
                Return _ratedAppGapSuccess
            End Get
            Set(value As Boolean)
                _ratedAppGapSuccess = value
            End Set
        End Property
        Public Property ratedAppGapPremium As String
            Get
                Return _ratedAppGapPremium
            End Get
            Set(value As String)
                _ratedAppGapPremium = value
            End Set
        End Property
        Public Property ratedAppGap_CPP_CGL_Premium As String 'added 11/30/2012
            Get
                Return _ratedAppGap_CPP_CGL_Premium
            End Get
            Set(value As String)
                _ratedAppGap_CPP_CGL_Premium = value
            End Set
        End Property
        Public Property ratedAppGap_CPP_CPR_Premium As String 'added 11/30/2012
            Get
                Return _ratedAppGap_CPP_CPR_Premium
            End Get
            Set(value As String)
                _ratedAppGap_CPP_CPR_Premium = value
            End Set
        End Property
        'updated 4/17/2015 for CIM and CRM
        Public Property ratedAppGap_CPP_CIM_Premium As String
            Get
                Return _ratedAppGap_CPP_CIM_Premium
            End Get
            Set(value As String)
                _ratedAppGap_CPP_CIM_Premium = value
            End Set
        End Property
        Public Property ratedAppGap_CPP_CRM_Premium As String
            Get
                Return _ratedAppGap_CPP_CRM_Premium
            End Get
            Set(value As String)
                _ratedAppGap_CPP_CRM_Premium = value
            End Set
        End Property
        Public Property ratedAppGapClientId As String
            Get
                Return _ratedAppGapClientId
            End Get
            Set(value As String)
                _ratedAppGapClientId = value
            End Set
        End Property
        Public Property ratedAppGapClientDisplayName As String
            Get
                Return _ratedAppGapClientDisplayName
            End Get
            Set(value As String)
                _ratedAppGapClientDisplayName = value
            End Set
        End Property
        Public Property ratedAppGapQuoteNumber As String
            Get
                Return _ratedAppGapQuoteNumber
            End Get
            Set(value As String)
                _ratedAppGapQuoteNumber = value
            End Set
        End Property
        Public Property xmlStatusId As String
            Get
                Return _xmlStatusId
            End Get
            Set(value As String)
                _xmlStatusId = value
            End Set
        End Property
        Public Property xmlUpdated As String
            Get
                Return _xmlUpdated
            End Get
            Set(value As String)
                _xmlUpdated = value
            End Set
        End Property
        Public Property xmlInserted As String
            Get
                Return _xmlInserted
            End Get
            Set(value As String)
                _xmlInserted = value
            End Set
        End Property
        Public Property xmlStatus As String
            Get
                Return _xmlStatus
            End Get
            Set(value As String)
                _xmlStatus = value
            End Set
        End Property

        'added 2/7/2013
        Public Property currentQuoteXmlId As String
            Get
                Return _currentQuoteXmlId
            End Get
            Set(value As String)
                _currentQuoteXmlId = value
            End Set
        End Property
        Public Property xmlQuoteId As String
            Get
                Return _xmlQuoteId
            End Get
            Set(value As String)
                _xmlQuoteId = value
            End Set
        End Property

        'added 9/23/2014
        Public Property diaPolicyNumber As String
            Get
                Return _diaPolicyNumber
            End Get
            Set(value As String)
                _diaPolicyNumber = value
            End Set
        End Property
        Public Property diaPolicyNumberDescription As String
            Get
                Return _diaPolicyNumberDescription
            End Get
            Set(value As String)
                _diaPolicyNumberDescription = value
            End Set
        End Property
        Public Property isPolicy As Boolean
            Get
                Return _isPolicy
            End Get
            Set(value As Boolean)
                _isPolicy = value
            End Set
        End Property
        Public Property originatedInVR As Boolean 'added 1/8/2016
            Get
                Return _originatedInVR
            End Get
            Set(value As Boolean)
                _originatedInVR = value
            End Set
        End Property

        'added 11/29/2017
        Public Property policyId As String
            Get
                Return _policyId
            End Get
            Set(value As String)
                _policyId = value
            End Set
        End Property
        Public Property ratedQuotePolicyId As String
            Get
                Return _ratedQuotePolicyId
            End Get
            Set(value As String)
                _ratedQuotePolicyId = value
            End Set
        End Property
        Public Property ratedAppGapPolicyId As String
            Get
                Return _ratedAppGapPolicyId
            End Get
            Set(value As String)
                _ratedAppGapPolicyId = value
            End Set
        End Property

        'added 03/23/2018
        Public Property effectiveDate As String
            Get
                Return _effectiveDate
            End Get
            Set(value As String)
                _effectiveDate = value
            End Set
        End Property
        Public Property quoteEffectiveDate As String
            Get
                Return _quoteEffectiveDate
            End Get
            Set(value As String)
                _quoteEffectiveDate = value
            End Set
        End Property
        Public Property appGapEffectiveDate As String
            Get
                Return _appGapEffectiveDate
            End Get
            Set(value As String)
                _appGapEffectiveDate = value
            End Set
        End Property

        'added 9/28/2018
        Public Property actualLobId As String
            Get
                Return _actualLobId
            End Get
            Set(value As String)
                _actualLobId = value
            End Set
        End Property
        Public Property governingStateId As String
            Get
                Return _governingStateId
            End Get
            Set(value As String)
                _governingStateId = value
            End Set
        End Property
        Public Property stateIds As String
            Get
                Return _stateIds
            End Get
            Set(value As String)
                _stateIds = value
            End Set
        End Property
        Public Property quoteActualLobId As String
            Get
                Return _quoteActualLobId
            End Get
            Set(value As String)
                _quoteActualLobId = value
            End Set
        End Property
        Public Property quoteGoverningStateId As String
            Get
                Return _quoteGoverningStateId
            End Get
            Set(value As String)
                _quoteGoverningStateId = value
            End Set
        End Property
        Public Property quoteStateIds As String
            Get
                Return _quoteStateIds
            End Get
            Set(value As String)
                _quoteStateIds = value
            End Set
        End Property
        Public Property appActualLobId As String
            Get
                Return _appActualLobId
            End Get
            Set(value As String)
                _appActualLobId = value
            End Set
        End Property
        Public Property appGoverningStateId As String
            Get
                Return _appGoverningStateId
            End Get
            Set(value As String)
                _appGoverningStateId = value
            End Set
        End Property
        Public Property appStateIds As String
            Get
                Return _appStateIds
            End Get
            Set(value As String)
                _appStateIds = value
            End Set
        End Property
        'added 01/02/2020 - DJG
        Protected Friend Property eSigInfo As QuickQuoteEsignatureInformation
            Get
                Return _eSigInfo
            End Get
            Set(value As QuickQuoteEsignatureInformation)
                _eSigInfo = value
            End Set
        End Property

        'added 1/4/2020 (Interoperability)
        Public Property LastRulesOverrideRecordModifiedDate As String
            Get
                Return _LastRulesOverrideRecordModifiedDate
            End Get
            Set(value As String)
                _LastRulesOverrideRecordModifiedDate = value
            End Set
        End Property

        'added 4/21/2021 (Umbrella)
        Public Property programTypeId As String
            Get
                Return _programTypeId
            End Get
            Set(value As String)
                _programTypeId = value
            End Set
        End Property

        'added 11/27/2022
        Public Property companyId As String
            Get
                Return _companyId
            End Get
            Set(value As String)
                _companyId = value
            End Set
        End Property
        Public Property quoteCompanyId As String
            Get
                Return _quoteCompanyId
            End Get
            Set(value As String)
                _quoteCompanyId = value
            End Set
        End Property
        Public Property appCompanyId As String
            Get
                Return _appCompanyId
            End Get
            Set(value As String)
                _appCompanyId = value
            End Set
        End Property

        'added 7/27/2023
        Public Property diaCompanyId As String
            Get
                Return _diaCompanyId
            End Get
            Set(value As String)
                _diaCompanyId = value
            End Set
        End Property

        Public Sub New(ByVal qId As String)
            SetDefaults()
            _quoteId = qId
        End Sub
        Private Sub SetDefaults()
            _quoteId = "" 'added 4/17/2015
            _lobId = ""
            _agencyId = ""
            _agencyCode = ""
            _initialUserId = ""
            _userId = ""
            _quoteXmlId = ""
            _clientId = ""
            _clientDisplayName = ""
            _quoteNumber = ""
            _premium = ""
            _CPP_CGL_premium = "" 'added 11/30/2012
            _CPP_CPR_premium = "" 'added 11/30/2012
            'updated 1/26/2015 for CIM and CRM; db not updated yet
            _CPP_CIM_premium = ""
            _CPP_CRM_premium = ""
            _quoteStatusId = ""
            _quoteStatus = ""
            _quoteUpdated = ""
            _quoteInserted = ""

            'updated 12/5/2012 for Risk Grade 3 Error
            _errorRiskGradeLookupId = ""
            _replacementRiskGradeLookupId = ""
            _CPP_CGL_errorRiskGradeLookupId = ""
            _CPP_CGL_replacementRiskGradeLookupId = ""
            _CPP_CPR_errorRiskGradeLookupId = ""
            _CPP_CPR_replacementRiskGradeLookupId = ""
            'updated 1/26/2015 for CIM and CRM; db not updated yet
            _CPP_CIM_errorRiskGradeLookupId = ""
            _CPP_CIM_replacementRiskGradeLookupId = ""
            _CPP_CRM_errorRiskGradeLookupId = ""
            _CPP_CRM_replacementRiskGradeLookupId = ""

            _description = ""
            _initialClientId = ""
            _initialClientDisplayName = ""
            _initialQuoteNumber = ""
            _quoteUserId = ""
            _quoteXml = Nothing
            _ratedQuoteXml = Nothing
            _ratedQuoteSuccess = False
            _ratedQuotePremium = ""
            _ratedQuote_CPP_CGL_Premium = "" 'added 11/30/2012
            _ratedQuote_CPP_CPR_Premium = "" 'added 11/30/2012
            'updated 4/17/2015 for CIM and CRM
            _ratedQuote_CPP_CIM_Premium = ""
            _ratedQuote_CPP_CRM_Premium = ""
            _ratedClientId = ""
            _ratedClientDisplayName = ""
            _ratedQuoteNumber = ""
            _appGapClientId = ""
            _appGapClientDisplayName = ""
            _appGapQuoteNumber = ""
            _appGapUserId = ""
            _appGapXml = Nothing
            _ratedAppGapXml = Nothing
            _ratedAppGapSuccess = False
            _ratedAppGapPremium = ""
            _ratedAppGap_CPP_CGL_Premium = "" 'added 11/30/2012
            _ratedAppGap_CPP_CPR_Premium = "" 'added 11/30/2012
            'updated 4/17/2015 for CIM and CRM
            _ratedAppGap_CPP_CIM_Premium = ""
            _ratedAppGap_CPP_CRM_Premium = ""
            _ratedAppGapClientId = ""
            _ratedAppGapClientDisplayName = ""
            _ratedAppGapQuoteNumber = ""
            _xmlStatusId = ""
            _xmlUpdated = ""
            _xmlInserted = ""
            _xmlStatus = ""

            'added 2/7/2013
            _currentQuoteXmlId = ""
            _xmlQuoteId = ""

            'added 9/23/2014
            _diaPolicyNumber = ""
            _diaPolicyNumberDescription = ""
            _isPolicy = False
            _originatedInVR = False 'added 1/8/2016

            'added 11/29/2017
            _policyId = ""
            _ratedQuotePolicyId = ""
            _ratedAppGapPolicyId = ""

            ''added 03/23/2018
            _effectiveDate = ""
            _quoteEffectiveDate = ""
            _appGapEffectiveDate = ""

            'added 9/28/2018
            _actualLobId = ""
            _governingStateId = ""
            _stateIds = ""
            _quoteActualLobId = ""
            _quoteGoverningStateId = ""
            _quoteStateIds = ""
            _appActualLobId = ""
            _appGoverningStateId = ""
            _appStateIds = ""

            'added 1/4/2020 (Interoperability)
            _LastRulesOverrideRecordModifiedDate = ""

            'added 4/21/2021 (Umbrella)
            _programTypeId = ""

            'added 11/27/2022
            _companyId = ""
            _quoteCompanyId = ""
            _appCompanyId = ""

            'added 7/27/2023
            _diaCompanyId = ""
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    qqHelper.DisposeString(_quoteId) 'added 4/17/2015
                    qqHelper.DisposeString(_lobId)
                    qqHelper.DisposeString(_agencyId)
                    qqHelper.DisposeString(_agencyCode)
                    qqHelper.DisposeString(_initialUserId)
                    qqHelper.DisposeString(_userId)
                    qqHelper.DisposeString(_quoteXmlId)
                    qqHelper.DisposeString(_clientId)
                    qqHelper.DisposeString(_clientDisplayName)
                    qqHelper.DisposeString(_quoteNumber)
                    qqHelper.DisposeString(_premium)
                    qqHelper.DisposeString(_CPP_CGL_premium) 'added 11/30/2012
                    qqHelper.DisposeString(_CPP_CPR_premium) 'added 11/30/2012
                    'updated 1/26/2015 for CIM and CRM; db not updated yet
                    qqHelper.DisposeString(_CPP_CIM_premium)
                    qqHelper.DisposeString(_CPP_CRM_premium)
                    qqHelper.DisposeString(_quoteStatusId)
                    qqHelper.DisposeString(_quoteStatus)
                    qqHelper.DisposeString(_quoteUpdated)
                    qqHelper.DisposeString(_quoteInserted)

                    'updated 12/5/2012 for Risk Grade 3 Error
                    qqHelper.DisposeString(_errorRiskGradeLookupId)
                    qqHelper.DisposeString(_replacementRiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CGL_errorRiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CGL_replacementRiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CPR_errorRiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CPR_replacementRiskGradeLookupId)
                    'updated 1/26/2015 for CIM and CRM; db not updated yet
                    qqHelper.DisposeString(_CPP_CIM_errorRiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CIM_replacementRiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CRM_errorRiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CRM_replacementRiskGradeLookupId)

                    qqHelper.DisposeString(_description)
                    qqHelper.DisposeString(_initialClientId)
                    qqHelper.DisposeString(_initialClientDisplayName)
                    qqHelper.DisposeString(_initialQuoteNumber)
                    qqHelper.DisposeString(_quoteUserId)
                    If _quoteXml IsNot Nothing Then
                        _quoteXml = Nothing
                    End If
                    If _quoteXml IsNot Nothing Then
                        _ratedQuoteXml = Nothing
                    End If
                    _ratedQuoteSuccess = Nothing
                    qqHelper.DisposeString(_ratedQuotePremium)
                    qqHelper.DisposeString(_ratedQuote_CPP_CGL_Premium) 'added 11/30/2012
                    qqHelper.DisposeString(_ratedQuote_CPP_CPR_Premium) 'added 11/30/2012
                    'updated 4/17/2015 for CIM and CRM
                    qqHelper.DisposeString(_ratedQuote_CPP_CIM_Premium)
                    qqHelper.DisposeString(_ratedQuote_CPP_CRM_Premium)
                    qqHelper.DisposeString(_ratedClientId)
                    qqHelper.DisposeString(_ratedClientDisplayName)
                    qqHelper.DisposeString(_ratedQuoteNumber)
                    qqHelper.DisposeString(_appGapClientId)
                    qqHelper.DisposeString(_appGapClientDisplayName)
                    qqHelper.DisposeString(_appGapQuoteNumber)
                    qqHelper.DisposeString(_appGapUserId)
                    If _quoteXml IsNot Nothing Then
                        _appGapXml = Nothing
                    End If
                    If _quoteXml IsNot Nothing Then
                        _ratedAppGapXml = Nothing
                    End If
                    _ratedAppGapSuccess = Nothing
                    qqHelper.DisposeString(_ratedAppGapPremium)
                    qqHelper.DisposeString(_ratedAppGap_CPP_CGL_Premium) 'added 11/30/2012
                    qqHelper.DisposeString(_ratedAppGap_CPP_CPR_Premium) 'added 11/30/2012
                    'updated 4/17/2015 for CIM and CRM
                    qqHelper.DisposeString(_ratedAppGap_CPP_CIM_Premium)
                    qqHelper.DisposeString(_ratedAppGap_CPP_CRM_Premium)
                    qqHelper.DisposeString(_ratedAppGapClientId)
                    qqHelper.DisposeString(_ratedAppGapClientDisplayName)
                    qqHelper.DisposeString(_ratedAppGapQuoteNumber)
                    qqHelper.DisposeString(_xmlStatusId)
                    qqHelper.DisposeString(_xmlUpdated)
                    qqHelper.DisposeString(_xmlInserted)
                    qqHelper.DisposeString(_xmlStatus)

                    'added 2/7/2013
                    qqHelper.DisposeString(_currentQuoteXmlId)
                    qqHelper.DisposeString(_xmlQuoteId)

                    'added 9/23/2014
                    qqHelper.DisposeString(_diaPolicyNumber)
                    qqHelper.DisposeString(_diaPolicyNumberDescription)
                    _isPolicy = Nothing
                    _originatedInVR = Nothing 'added 1/8/2016

                    'added 11/29/2017
                    qqHelper.DisposeString(_policyId)
                    qqHelper.DisposeString(_ratedQuotePolicyId)
                    qqHelper.DisposeString(_ratedAppGapPolicyId)

                    'added 9/28/2018
                    qqHelper.DisposeString(_effectiveDate)
                    qqHelper.DisposeString(_quoteEffectiveDate)
                    qqHelper.DisposeString(_appGapEffectiveDate)
                    qqHelper.DisposeString(_actualLobId)
                    qqHelper.DisposeString(_governingStateId)
                    qqHelper.DisposeString(_stateIds)
                    qqHelper.DisposeString(_quoteActualLobId)
                    qqHelper.DisposeString(_quoteGoverningStateId)
                    qqHelper.DisposeString(_quoteStateIds)
                    qqHelper.DisposeString(_appActualLobId)
                    qqHelper.DisposeString(_appGoverningStateId)
                    qqHelper.DisposeString(_appStateIds)

                    'added 1/4/2020 (Interoperability)
                    qqHelper.DisposeString(_LastRulesOverrideRecordModifiedDate)

                    'added 4/21/2021 (Umbrella)
                    qqHelper.DisposeString(_programTypeId)

                    'added 11/27/2022
                    qqHelper.DisposeString(_companyId)
                    qqHelper.DisposeString(_quoteCompanyId)
                    qqHelper.DisposeString(_appCompanyId)

                    'added 7/27/2023
                    qqHelper.DisposeString(_diaCompanyId)
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
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
