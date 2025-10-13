Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    Public Interface IQuickQuoteStateAndOrLobInfo
        Sub CopyInfo_Base(ByVal fromInfo As IQuickQuoteStateAndOrLobInfo, Optional ByVal onlyUpdateClassSpecificInfoIfCurrentIsInvalid As Boolean = False, Optional ByVal doNotUpdateBaseInfo As Boolean = False)

        Property LobType As QuickQuoteObject.QuickQuoteLobType
        Property StateType As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState
        Sub Reset_Base()
        Sub Dispose_Base()
        Function HasLobType() As Boolean
        Function HasStateType() As Boolean
        Function HasLobAndStateType(Optional ByRef hasLob As Boolean = False, Optional ByRef hasState As Boolean = False) As Boolean
        Function HasLobOrStateType(Optional ByRef hasLob As Boolean = False, Optional ByRef hasState As Boolean = False) As Boolean
        Function HasState_Prop() As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType
        Function HasLob_Prop() As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType
        Function HasClassSpecificInfo_Base() As Boolean

    End Interface
    <Serializable()>
    Public MustInherit Class QuickQuoteStateAndOrLobInfo
        Implements IQuickQuoteStateAndOrLobInfo

        Private _HasLob As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Maybe
        Private _HasState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Maybe

        Private _LobType As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None
        Public Property LobType As QuickQuoteObject.QuickQuoteLobType Implements IQuickQuoteStateAndOrLobInfo.LobType
            Get
                'Throw New NotImplementedException()
                Return _LobType
            End Get
            Set(value As QuickQuoteObject.QuickQuoteLobType)
                'Throw New NotImplementedException()
                _LobType = value
                _HasLob = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Maybe
            End Set
        End Property

        Private _StateType As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState = CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None
        Public Property StateType As QuickQuoteHelperClass.QuickQuoteState Implements IQuickQuoteStateAndOrLobInfo.StateType
            Get
                'Throw New NotImplementedException()
                Return _StateType
            End Get
            Set(value As QuickQuoteHelperClass.QuickQuoteState)
                'Throw New NotImplementedException()
                _StateType = value
                _HasState = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Maybe
            End Set
        End Property

        Protected MustOverride Sub CopyInfo(ByVal fromInfo As QuickQuoteStateAndOrLobInfo, Optional onlyUpdateClassSpecificInfoIfCurrentIsInvalid As Boolean = False)
        Public Sub CopyInfo_Base(fromInfo As IQuickQuoteStateAndOrLobInfo, Optional onlyUpdateClassSpecificInfoIfCurrentIsInvalid As Boolean = False, Optional ByVal doNotUpdateBaseInfo As Boolean = False) Implements IQuickQuoteStateAndOrLobInfo.CopyInfo_Base
            'Throw New NotImplementedException()
            If fromInfo IsNot Nothing Then
                If doNotUpdateBaseInfo = False Then
                    _LobType = fromInfo.LobType
                    _StateType = fromInfo.StateType
                    _HasLob = fromInfo.HasLob_Prop
                    _HasState = fromInfo.HasState_Prop
                End If
                CopyInfo(fromInfo, onlyUpdateClassSpecificInfoIfCurrentIsInvalid:=onlyUpdateClassSpecificInfoIfCurrentIsInvalid)
            Else
                Dispose_Base()
            End If
        End Sub

        Protected MustOverride Sub Reset()
        Public Sub Reset_Base() Implements IQuickQuoteStateAndOrLobInfo.Reset_Base
            'Throw New NotImplementedException()
            _LobType = QuickQuoteObject.QuickQuoteLobType.None
            _StateType = CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None
            _HasLob = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Maybe
            _HasState = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Maybe
            Reset()
        End Sub

        Protected MustOverride Sub Dispose()
        Public Sub Dispose_Base() Implements IQuickQuoteStateAndOrLobInfo.Dispose_Base
            'Throw New NotImplementedException()
            _LobType = Nothing
            _StateType = Nothing
            _HasLob = Nothing
            _HasState = Nothing
            Dispose()
        End Sub

        Public Function HasLobType() As Boolean Implements IQuickQuoteStateAndOrLobInfo.HasLobType
            'Throw New NotImplementedException()
            Dim hasIt As Boolean = False

            If _HasLob = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then
                hasIt = True
            ElseIf _HasLob = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.No Then
                hasIt = False
            Else
                If System.Enum.IsDefined(GetType(QuickQuoteObject.QuickQuoteLobType), _LobType) = True AndAlso _LobType <> QuickQuoteObject.QuickQuoteLobType.None Then
                    _HasLob = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes
                    hasIt = True
                Else
                    _HasLob = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.No
                    hasIt = False
                End If
            End If

            Return hasIt
        End Function

        Public Function HasStateType() As Boolean Implements IQuickQuoteStateAndOrLobInfo.HasStateType
            'Throw New NotImplementedException()
            Dim hasIt As Boolean = False

            If _HasState = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then
                hasIt = True
            ElseIf _HasState = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.No Then
                hasIt = False
            Else
                If System.Enum.IsDefined(GetType(CommonMethods.QuickQuoteHelperClass.QuickQuoteState), _StateType) = True AndAlso _StateType <> CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                    _HasState = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes
                    hasIt = True
                Else
                    _HasState = CommonMethods.QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.No
                    hasIt = False
                End If
            End If

            Return hasIt
        End Function

        Public Function HasLobAndStateType(ByRef Optional hasLob As Boolean = False, ByRef Optional hasState As Boolean = False) As Boolean Implements IQuickQuoteStateAndOrLobInfo.HasLobAndStateType
            'Throw New NotImplementedException()
            Dim hasBoth As Boolean = False

            hasLob = HasLobType()
            hasState = HasStateType()

            If hasLob = True AndAlso hasState = True Then
                hasBoth = True
            End If

            Return hasBoth
        End Function

        Public Function HasLobOrStateType(ByRef Optional hasLob As Boolean = False, ByRef Optional hasState As Boolean = False) As Boolean Implements IQuickQuoteStateAndOrLobInfo.HasLobOrStateType
            'Throw New NotImplementedException()
            Dim hasEither As Boolean = False

            hasLob = HasLobType()
            hasState = HasStateType()

            If hasLob = True OrElse hasState = True Then
                hasEither = True
            End If

            Return hasEither
        End Function

        Protected Friend Function HasState_Prop() As QuickQuoteHelperClass.QuickQuoteYesNoMaybeType Implements IQuickQuoteStateAndOrLobInfo.HasState_Prop
            'Throw New NotImplementedException()
            Return _HasState
        End Function

        Protected Friend Function HasLob_Prop() As QuickQuoteHelperClass.QuickQuoteYesNoMaybeType Implements IQuickQuoteStateAndOrLobInfo.HasLob_Prop
            'Throw New NotImplementedException()
            Return _HasLob
        End Function

        Protected MustOverride Function HasClassSpecificInfo() As Boolean
        Public Function HasClassSpecificInfo_Base() As Boolean Implements IQuickQuoteStateAndOrLobInfo.HasClassSpecificInfo_Base
            'Throw New NotImplementedException()
            Return HasClassSpecificInfo()
        End Function

    End Class

    <Serializable()>
    Public Class QuickQuotePayplanIdForStateAndOrLob
        Inherits QuickQuoteStateAndOrLobInfo

        Public Property PayplanId As Integer = 0

        Protected Overrides Sub CopyInfo(fromInfo As QuickQuoteStateAndOrLobInfo, Optional onlyUpdateClassSpecificInfoIfCurrentIsInvalid As Boolean = False)
            'Throw New NotImplementedException()
            If fromInfo IsNot Nothing AndAlso TypeOf fromInfo Is QuickQuotePayplanIdForStateAndOrLob Then
                'If onlyUpdateClassSpecificInfoIfValid = False OrElse CType(fromInfo, QuickQuotePayplanIdForStateAndOrLob).PayplanId > 0 Then
                '    _PayplanId = CType(fromInfo, QuickQuotePayplanIdForStateAndOrLob).PayplanId
                'End If
                If onlyUpdateClassSpecificInfoIfCurrentIsInvalid = False OrElse _PayplanId <= 0 Then
                    _PayplanId = CType(fromInfo, QuickQuotePayplanIdForStateAndOrLob).PayplanId
                End If
            Else
                'Dispose()
                Dispose_Base()
            End If
        End Sub

        Protected Overrides Sub Reset()
            'Throw New NotImplementedException()
            _PayplanId = 0
        End Sub

        Protected Overrides Sub Dispose()
            'Throw New NotImplementedException()
            _PayplanId = Nothing
        End Sub

        Protected Overrides Function HasClassSpecificInfo() As Boolean
            'Throw New NotImplementedException()
            If _PayplanId > 0 Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class
    <Serializable()>
    Public Class QuickQuotePayplanTermDatesForStateAndOrLob
        Inherits QuickQuoteStateAndOrLobInfo

        Public Property StartDate As String = ""
        Public Property EndDate As String = ""
        Public Property RenewalStartDate As String = ""
        Public Property RenewalEndDate As String = ""

        Protected Overrides Sub CopyInfo(fromInfo As QuickQuoteStateAndOrLobInfo, Optional onlyUpdateClassSpecificInfoIfCurrentIsInvalid As Boolean = False)
            'Throw New NotImplementedException()
            If fromInfo IsNot Nothing AndAlso TypeOf fromInfo Is QuickQuotePayplanTermDatesForStateAndOrLob Then
                Dim qqHelper As New QuickQuoteHelperClass
                'If onlyUpdateClassSpecificInfoIfValid = False OrElse qqHelper.IsValidDateString(CType(fromInfo, QuickQuotePayplanTermDatesForStateAndOrLob).StartDate, mustBeGreaterThanDefaultDate:=True) = True Then
                '    _StartDate = CType(fromInfo, QuickQuotePayplanTermDatesForStateAndOrLob).StartDate
                'End If
                'If onlyUpdateClassSpecificInfoIfValid = False OrElse qqHelper.IsValidDateString(CType(fromInfo, QuickQuotePayplanTermDatesForStateAndOrLob).EndDate, mustBeGreaterThanDefaultDate:=True) = True Then
                '    _EndDate = CType(fromInfo, QuickQuotePayplanTermDatesForStateAndOrLob).EndDate
                'End If
                'If onlyUpdateClassSpecificInfoIfValid = False OrElse qqHelper.IsValidDateString(CType(fromInfo, QuickQuotePayplanTermDatesForStateAndOrLob).RenewalStartDate, mustBeGreaterThanDefaultDate:=True) = True Then
                '    _RenewalStartDate = CType(fromInfo, QuickQuotePayplanTermDatesForStateAndOrLob).RenewalStartDate
                'End If
                'If onlyUpdateClassSpecificInfoIfValid = False OrElse qqHelper.IsValidDateString(CType(fromInfo, QuickQuotePayplanTermDatesForStateAndOrLob).RenewalEndDate, mustBeGreaterThanDefaultDate:=True) = True Then
                '    _RenewalEndDate = CType(fromInfo, QuickQuotePayplanTermDatesForStateAndOrLob).RenewalEndDate
                'End If
                If onlyUpdateClassSpecificInfoIfCurrentIsInvalid = False OrElse qqHelper.IsValidDateString(_StartDate, mustBeGreaterThanDefaultDate:=True) = False Then
                    _StartDate = CType(fromInfo, QuickQuotePayplanTermDatesForStateAndOrLob).StartDate
                End If
                If onlyUpdateClassSpecificInfoIfCurrentIsInvalid = False OrElse qqHelper.IsValidDateString(_EndDate, mustBeGreaterThanDefaultDate:=True) = False Then
                    _EndDate = CType(fromInfo, QuickQuotePayplanTermDatesForStateAndOrLob).EndDate
                End If
                If onlyUpdateClassSpecificInfoIfCurrentIsInvalid = False OrElse qqHelper.IsValidDateString(_RenewalStartDate, mustBeGreaterThanDefaultDate:=True) = False Then
                    _RenewalStartDate = CType(fromInfo, QuickQuotePayplanTermDatesForStateAndOrLob).RenewalStartDate
                End If
                If onlyUpdateClassSpecificInfoIfCurrentIsInvalid = False OrElse qqHelper.IsValidDateString(_RenewalEndDate, mustBeGreaterThanDefaultDate:=True) = False Then
                    _RenewalEndDate = CType(fromInfo, QuickQuotePayplanTermDatesForStateAndOrLob).RenewalEndDate
                End If
            Else
                'Dispose()
                Dispose_Base()
            End If
        End Sub

        Protected Overrides Sub Reset()
            'Throw New NotImplementedException()
            _StartDate = ""
            _EndDate = ""
            _RenewalStartDate = ""
            _RenewalEndDate = ""
        End Sub

        Protected Overrides Sub Dispose()
            'Throw New NotImplementedException()
            _StartDate = Nothing
            _EndDate = Nothing
            _RenewalStartDate = Nothing
            _RenewalEndDate = Nothing
        End Sub

        Protected Overrides Function HasClassSpecificInfo() As Boolean
            'Throw New NotImplementedException()
            Dim qqHelper As New QuickQuoteHelperClass
            If qqHelper.IsValidDateString(_StartDate, mustBeGreaterThanDefaultDate:=True) = True OrElse qqHelper.IsValidDateString(_EndDate, mustBeGreaterThanDefaultDate:=True) = True OrElse qqHelper.IsValidDateString(_RenewalStartDate, mustBeGreaterThanDefaultDate:=True) = True OrElse qqHelper.IsValidDateString(_RenewalEndDate, mustBeGreaterThanDefaultDate:=True) = True Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class

    'added 5/1/2022
    <Serializable()>
    Public Class QuickQuotePayplanStateAndOrLobElementStuff
        Public Property EquivalentInfo As QuickQuotePayplanIdForStateAndOrLob = Nothing
        Public Property EquivalentInfosForStateLob As List(Of QuickQuotePayplanIdForStateAndOrLob) = Nothing
        Public Property PrevPayPlanIdInfo As QuickQuotePayplanIdForStateAndOrLob = Nothing
        Public Property PrevPayPlanIdInfosForStateLob As List(Of QuickQuotePayplanIdForStateAndOrLob) = Nothing
        Public Property NextPayPlanIdInfo As QuickQuotePayplanIdForStateAndOrLob = Nothing
        Public Property NextPayPlanIdInfosForStateLob As List(Of QuickQuotePayplanIdForStateAndOrLob) = Nothing
        Public Sub Reset()
            _EquivalentInfo = Nothing
            _EquivalentInfosForStateLob = Nothing
            _PrevPayPlanIdInfo = Nothing
            _PrevPayPlanIdInfosForStateLob = Nothing
            _NextPayPlanIdInfo = Nothing
            _NextPayPlanIdInfosForStateLob = Nothing
        End Sub

        Public Sub Dispose()
            _EquivalentInfo = Nothing
            _EquivalentInfosForStateLob = Nothing
            _PrevPayPlanIdInfo = Nothing
            _PrevPayPlanIdInfosForStateLob = Nothing
            _NextPayPlanIdInfo = Nothing
            _NextPayPlanIdInfosForStateLob = Nothing
        End Sub
    End Class
    <Serializable()>
    Public Class QuickQuotePayplanStateAndOrLobAttributeStuff
        Public Property TermDateInfo As QuickQuotePayplanTermDatesForStateAndOrLob = Nothing
        Public Property TermDateInfosForStateLob As List(Of QuickQuotePayplanTermDatesForStateAndOrLob) = Nothing
        Public Property IsStateAndOrLobSpecific As Boolean = False
        Public Property SpecificStateLobCombos As List(Of QuickQuotePayplanIdForStateAndOrLob) = Nothing 'added 5/2/2022
        Public Sub Reset()
            _TermDateInfo = Nothing
            _TermDateInfosForStateLob = Nothing
            _IsStateAndOrLobSpecific = False
            _SpecificStateLobCombos = Nothing
        End Sub

        Public Sub Dispose()
            _TermDateInfo = Nothing
            _TermDateInfosForStateLob = Nothing
            _IsStateAndOrLobSpecific = Nothing
            _SpecificStateLobCombos = Nothing
        End Sub
    End Class
    <Serializable()>
    Public Class QuickQuotePayplanStateAndOrLobElementAndAttributeStuff
        Public Property ElementStuff As QuickQuotePayplanStateAndOrLobElementStuff
        Public Property AttributeStuff As QuickQuotePayplanStateAndOrLobAttributeStuff
        Public Sub Reset()
            _ElementStuff = Nothing
            _AttributeStuff = Nothing
        End Sub

        Public Sub Dispose()
            _ElementStuff = Nothing
            _AttributeStuff = Nothing
        End Sub
    End Class
End Namespace
