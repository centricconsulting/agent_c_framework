Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuote_Copy_Parameters 'added 7/26/2018

        Public Enum QuoteCopyType
            None = 0
            StateToState = 1
            MultiStateBase = 2
            BetweenQuoteAndMultiStateLevels = 3 'added 7/28/2018
            FullQuoteReplacement = 4 'added 8/15/2018
            RatedQuoteToSourceQuote = 5 'added 8/16/2018
            GoverningStateSwitch = 6 'added 8/16/2018
        End Enum

        'note: Properties won't even be checked if Parent = True
        Public Property CopyAllTopLevelQuoteInfo As Boolean = False 'no parent
        Public Property CopyTopLevelBaseInfo As Boolean = True 'Parent: CopyAllTopLevelQuoteInfo
        Public Property CopyTopLevelBaseDatabaseInfo As Boolean = True 'added 8/18/2018; Parent: CopyAllTopLevelQuoteInfo/CopyTopLevelBaseInfo
        Public Property CopyTopLevelBaseCommonInfo As Boolean = True 'added 8/18/2018; Parent: CopyAllTopLevelQuoteInfo/CopyTopLevelBaseInfo
        Public Property CopyTopLevelPremiums As Boolean = False 'Parent: CopyAllTopLevelQuoteInfo
        Public Property CopyTopLevelStateAndLobParts As Boolean = False 'Parent: CopyAllTopLevelQuoteInfo
        Public Property CopyAllVersionAndLobInfo As Boolean = False 'no parent
        Public Property CopyAllLobPolicyLevelInfo As Boolean = False 'Parent: CopyAllVersionAndLobInfo; updated 8/18/2018 from True to False
        Public Property CopyLobPolicyLevelGoverningStateInfo As Boolean = False 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
        Public Property CopyLobPolicyLevelAllStatesInfo As Boolean = True 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
        Public Property CopyLobPolicyLevelIndividualStateInfo As Boolean = False 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
        Public Property CopyAllLobRiskLevelInfo As Boolean = False 'Parent: CopyAllVersionAndLobInfo
        Public Property CopyApplicants As Boolean = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
        Public Property CopyDrivers As Boolean = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
        Public Property CopyLocations As Boolean = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
        Public Property CopyVehicles As Boolean = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
        Public Property CopyOperators As Boolean = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo

        Private _CopyType As QuoteCopyType = QuoteCopyType.StateToState
        Public Property CopyType As QuoteCopyType
            Get
                Return _CopyType
            End Get
            Set(value As QuoteCopyType)
                _CopyType = value
                SetCopyTypeDefaults()
            End Set
        End Property

        Private Sub SetCopyTypeDefaults()
            Select Case _CopyType
                Case QuoteCopyType.StateToState 'should match defaults
                    _CopyAllTopLevelQuoteInfo = False 'no parent
                    _CopyTopLevelBaseInfo = True 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyTopLevelBaseDatabaseInfo = True 'added 8/18/2018; Parent: CopyAllTopLevelQuoteInfo/CopyTopLevelBaseInfo
                    _CopyTopLevelBaseCommonInfo = True 'added 8/18/2018; Parent: CopyAllTopLevelQuoteInfo/CopyTopLevelBaseInfo
                    _CopyTopLevelPremiums = False 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyTopLevelStateAndLobParts = False 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyAllVersionAndLobInfo = False 'no parent
                    _CopyAllLobPolicyLevelInfo = False 'Parent: CopyAllVersionAndLobInfo; updated 8/18/2018 from True to False
                    _CopyLobPolicyLevelGoverningStateInfo = False 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyLobPolicyLevelAllStatesInfo = True 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyLobPolicyLevelIndividualStateInfo = False 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyAllLobRiskLevelInfo = False 'Parent: CopyAllVersionAndLobInfo
                    _CopyApplicants = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo; updated 8/18/2018 from True to False
                    _CopyDrivers = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo; updated 8/18/2018 from True to False
                    _CopyLocations = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyVehicles = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyOperators = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo; updated 8/18/2018 from True to False
                Case QuoteCopyType.MultiStateBase
                    _CopyAllTopLevelQuoteInfo = False 'no parent
                    _CopyTopLevelBaseInfo = True 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyTopLevelBaseDatabaseInfo = True 'added 8/18/2018; Parent: CopyAllTopLevelQuoteInfo/CopyTopLevelBaseInfo
                    _CopyTopLevelBaseCommonInfo = True 'added 8/18/2018; Parent: CopyAllTopLevelQuoteInfo/CopyTopLevelBaseInfo
                    _CopyTopLevelPremiums = False 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyTopLevelStateAndLobParts = False 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyAllVersionAndLobInfo = False 'no parent
                    _CopyAllLobPolicyLevelInfo = False 'Parent: CopyAllVersionAndLobInfo
                    _CopyLobPolicyLevelGoverningStateInfo = False 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyLobPolicyLevelAllStatesInfo = False 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyLobPolicyLevelIndividualStateInfo = False 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyAllLobRiskLevelInfo = False 'Parent: CopyAllVersionAndLobInfo
                    _CopyApplicants = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyDrivers = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyLocations = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyVehicles = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyOperators = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                Case QuoteCopyType.BetweenQuoteAndMultiStateLevels 'added 7/28/2018
                    _CopyAllTopLevelQuoteInfo = False 'no parent
                    _CopyTopLevelBaseInfo = True 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyTopLevelBaseDatabaseInfo = True 'added 8/18/2018; Parent: CopyAllTopLevelQuoteInfo/CopyTopLevelBaseInfo
                    _CopyTopLevelBaseCommonInfo = True 'added 8/18/2018; Parent: CopyAllTopLevelQuoteInfo/CopyTopLevelBaseInfo
                    _CopyTopLevelPremiums = False 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyTopLevelStateAndLobParts = False 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyAllVersionAndLobInfo = True 'no parent
                    _CopyAllLobPolicyLevelInfo = True 'Parent: CopyAllVersionAndLobInfo
                    _CopyLobPolicyLevelGoverningStateInfo = True 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyLobPolicyLevelAllStatesInfo = True 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyLobPolicyLevelIndividualStateInfo = True 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyAllLobRiskLevelInfo = True 'Parent: CopyAllVersionAndLobInfo
                    _CopyApplicants = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyDrivers = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyLocations = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyVehicles = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyOperators = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                Case QuoteCopyType.FullQuoteReplacement 'added 8/15/2018
                    _CopyAllTopLevelQuoteInfo = True 'no parent
                    _CopyTopLevelBaseInfo = True 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyTopLevelBaseDatabaseInfo = True 'added 8/18/2018; Parent: CopyAllTopLevelQuoteInfo/CopyTopLevelBaseInfo
                    _CopyTopLevelBaseCommonInfo = True 'added 8/18/2018; Parent: CopyAllTopLevelQuoteInfo/CopyTopLevelBaseInfo
                    _CopyTopLevelPremiums = True 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyTopLevelStateAndLobParts = True 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyAllVersionAndLobInfo = True 'no parent
                    _CopyAllLobPolicyLevelInfo = True 'Parent: CopyAllVersionAndLobInfo
                    _CopyLobPolicyLevelGoverningStateInfo = True 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyLobPolicyLevelAllStatesInfo = True 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyLobPolicyLevelIndividualStateInfo = True 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyAllLobRiskLevelInfo = True 'Parent: CopyAllVersionAndLobInfo
                    _CopyApplicants = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyDrivers = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyLocations = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyVehicles = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyOperators = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                Case QuoteCopyType.RatedQuoteToSourceQuote 'added 8/18/2018
                    _CopyAllTopLevelQuoteInfo = True 'no parent
                    _CopyTopLevelBaseInfo = False 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyTopLevelBaseDatabaseInfo = False 'added 8/18/2018; Parent: CopyAllTopLevelQuoteInfo/CopyTopLevelBaseInfo
                    _CopyTopLevelBaseCommonInfo = True 'added 8/18/2018; Parent: CopyAllTopLevelQuoteInfo/CopyTopLevelBaseInfo
                    _CopyTopLevelPremiums = True 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyTopLevelStateAndLobParts = True 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyAllVersionAndLobInfo = True 'no parent
                    _CopyAllLobPolicyLevelInfo = True 'Parent: CopyAllVersionAndLobInfo
                    _CopyLobPolicyLevelGoverningStateInfo = True 'Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyLobPolicyLevelAllStatesInfo = True 'Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyLobPolicyLevelIndividualStateInfo = True 'Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyAllLobRiskLevelInfo = True 'Parent: CopyAllVersionAndLobInfo
                    _CopyApplicants = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyDrivers = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyLocations = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyVehicles = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyOperators = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                Case QuoteCopyType.GoverningStateSwitch 'added 8/18/2018
                    _CopyAllTopLevelQuoteInfo = False 'no parent
                    _CopyTopLevelBaseInfo = False 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyTopLevelBaseDatabaseInfo = False 'added 8/18/2018; Parent: CopyAllTopLevelQuoteInfo/CopyTopLevelBaseInfo
                    _CopyTopLevelBaseCommonInfo = False 'added 8/18/2018; Parent: CopyAllTopLevelQuoteInfo/CopyTopLevelBaseInfo
                    _CopyTopLevelPremiums = False 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyTopLevelStateAndLobParts = False 'Parent: CopyAllTopLevelQuoteInfo
                    _CopyAllVersionAndLobInfo = False 'no parent
                    _CopyAllLobPolicyLevelInfo = False 'Parent: CopyAllVersionAndLobInfo
                    _CopyLobPolicyLevelGoverningStateInfo = True 'Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyLobPolicyLevelAllStatesInfo = False 'Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyLobPolicyLevelIndividualStateInfo = False 'Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
                    _CopyAllLobRiskLevelInfo = False 'Parent: CopyAllVersionAndLobInfo
                    _CopyApplicants = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyDrivers = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyLocations = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyVehicles = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                    _CopyOperators = True 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
                Case Else 'None
                    ResetProperties()
            End Select
        End Sub
        Public Sub ResetProperties()
            _CopyAllTopLevelQuoteInfo = False 'no parent
            _CopyTopLevelBaseInfo = False 'Parent: CopyAllTopLevelQuoteInfo
            _CopyTopLevelBaseDatabaseInfo = False 'added 8/18/2018; Parent: CopyAllTopLevelQuoteInfo/CopyTopLevelBaseInfo
            _CopyTopLevelBaseCommonInfo = False 'added 8/18/2018; Parent: CopyAllTopLevelQuoteInfo/CopyTopLevelBaseInfo
            _CopyTopLevelPremiums = False 'Parent: CopyAllTopLevelQuoteInfo
            _CopyTopLevelStateAndLobParts = False 'Parent: CopyAllTopLevelQuoteInfo
            _CopyAllVersionAndLobInfo = False 'no parent
            _CopyAllLobPolicyLevelInfo = False 'Parent: CopyAllVersionAndLobInfo
            _CopyLobPolicyLevelGoverningStateInfo = False 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
            _CopyLobPolicyLevelAllStatesInfo = False 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
            _CopyLobPolicyLevelIndividualStateInfo = False 'added 8/18/2018; Parent: CopyAllVersionAndLobInfo/CopyAllLobPolicyLevelInfo
            _CopyAllLobRiskLevelInfo = False 'Parent: CopyAllVersionAndLobInfo
            _CopyApplicants = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
            _CopyDrivers = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
            _CopyLocations = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
            _CopyVehicles = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
            _CopyOperators = False 'Parents: CopyAllVersionAndLobInfo/CopyAllLobRiskLevelInfo
        End Sub
    End Class
End Namespace

