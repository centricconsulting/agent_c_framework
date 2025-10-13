Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Helpers

''' <summary>
''' GENERALLY DO NOT INHERIT FROM THIS. Use VrControlBase instead.
''' </summary>
Public MustInherit Class VrControlBaseEssentials
    Inherits System.Web.UI.UserControl

    Dim _qqHelper As QuickQuoteHelperClass = Nothing
    Protected _myAccordianIndex As Integer = 0
    Dim _senderName As String = ""
    ''' <summary>
    ''' This provides access to helper logic such as static data.
    ''' </summary>
    ''' <returns></returns>
    Protected ReadOnly Property QQHelper As QuickQuoteHelperClass
        Get
            If _qqHelper Is Nothing Then
                _qqHelper = New QuickQuoteHelperClass
            End If
            Return _qqHelper
        End Get
    End Property

    ''' <summary>
    ''' This provides access to several common js scripts as well as the ability to inject js script via code-behind.
    ''' </summary>
    ''' <returns></returns>
    Protected ReadOnly Property VRScript As ctlPageStartupScript
        Get
            Return DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        End Get
    End Property

    ''' <summary>
    ''' Can return (quote, quote rated, app, and app rated) images. The call will automatically get quote on quote side and app on app side.
    ''' If you want rated image you need to set the property below ' UseRatedQuoteImage'
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            _QuoteWasRequested = True 'added 10/3/2019 (12/11/2019 in Sprint 10)
            _FailedToLoadQuoteOnLastRequest = False 'added 10/3/2019 (12/11/2019 in Sprint 10)
            _ErrorMessageFromLastQuoteRequest = "" 'added 10/3/2019 (12/11/2019 in Sprint 10)
            Dim qqo As QuickQuoteObject = Nothing 'added 10/3/2019 (12/11/2019 in Sprint 10)
            Dim loadErrorMsg As String = "" 'added 12/18/2019

            If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/14/2019; original logic in ELSE
                Dim readOnlyLoadError As String = ""
                If UseRatedQuoteImage Then
                    If TypeOf Me.Page.Master Is VelociRater Then
                        'Return DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()
                        'updated 12/18/2019
                        qqo = DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache(loadErrorMsg:=readOnlyLoadError)
                    Else
#If DEBUG Then
                        Debugger.Break() ' should not be happening
#End If
                        'Return VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyQuickQuoteObjectForPolicyIdAndImageNum(ReadOnlyPolicyId, ReadOnlyPolicyImageNum, saveTypeView:=If(IsOnAppPage = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote), ratedView:=True, errorMessage:=readOnlyLoadError)
                        'updated 12/18/2019
                        qqo = VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyQuickQuoteObjectForPolicyIdAndImageNum(ReadOnlyPolicyId, ReadOnlyPolicyImageNum, saveTypeView:=If(IsOnAppPage = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote), ratedView:=True, errorMessage:=readOnlyLoadError)
                    End If
                Else
                    'Return VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyImageFromAnywhere(ReadOnlyPolicyId, ReadOnlyPolicyImageNum, saveTypeView:=If(IsOnAppPage = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote), errorMessage:=readOnlyLoadError)
                    'updated 12/18/2019
                    qqo = VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyImageFromAnywhere(ReadOnlyPolicyId, ReadOnlyPolicyImageNum, saveTypeView:=If(IsOnAppPage = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote), errorMessage:=readOnlyLoadError)
                End If
                loadErrorMsg = readOnlyLoadError 'added 12/18/2019
            ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                Dim endorsementLoadError As String = ""
                If UseRatedQuoteImage Then
                    If TypeOf Me.Page.Master Is VelociRater Then
                        'Return DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()
                        'updated 12/18/2019
                        qqo = DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache(loadErrorMsg:=endorsementLoadError)
                    Else
#If DEBUG Then
                        Debugger.Break() ' should not be happening
#End If
                        'Return VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteForPolicyIdAndImageNum(EndorsementPolicyId, EndorsementPolicyImageNum, saveTypeView:=If(IsOnAppPage = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote), ratedView:=True, errorMessage:=endorsementLoadError)
                        'updated 12/18/2019
                        qqo = VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteForPolicyIdAndImageNum(EndorsementPolicyId, EndorsementPolicyImageNum, saveTypeView:=If(IsOnAppPage = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote), ratedView:=True, errorMessage:=endorsementLoadError)
                    End If
                Else
                    'Return VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteFromAnywhere(EndorsementPolicyId, EndorsementPolicyImageNum, saveTypeView:=If(IsOnAppPage = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote), errorMessage:=endorsementLoadError)
                    'updated 12/18/2019
                    qqo = VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteFromAnywhere(EndorsementPolicyId, EndorsementPolicyImageNum, saveTypeView:=If(IsOnAppPage = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote), errorMessage:=endorsementLoadError)
                End If
                loadErrorMsg = endorsementLoadError 'added 12/18/2019
            Else
                Dim errCreateQSO As String = ""
                If IsOnAppPage Then
                    If UseRatedQuoteImage Then
                        If TypeOf Me.Page.Master Is VelociRater Then
                            'Return DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache() '6-3-14 - Matt
                            'updated 10/3/2019 (12/11/2019 in Sprint 10)
                            qqo = DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache(loadErrorMsg:=errCreateQSO)
                        Else
#If DEBUG Then
                            Debugger.Break() ' should not be happening
#End If
                            'Return Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.None, QuickQuoteXML.QuickQuoteSaveType.AppGap)
                            'updated 10/3/2019 (12/11/2019 in Sprint 10)
                            qqo = Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.None, QuickQuoteXML.QuickQuoteSaveType.AppGap)
                        End If
                    Else
                        'Return VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.None, QuoteId, errCreateQSO, True, QuickQuoteXML.QuickQuoteSaveType.AppGap)
                        'updated 10/3/2019 (12/11/2019 in Sprint 10)
                        qqo = VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.None, QuoteId, errCreateQSO, True, QuickQuoteXML.QuickQuoteSaveType.AppGap)
                    End If
                Else
                    If UseRatedQuoteImage Then
                        If TypeOf Me.Page.Master Is VelociRater Then
                            'Return DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache() '6-3-14 - Matt
                            'updated 10/3/2019 (12/11/2019 in Sprint 10)
                            qqo = DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache(loadErrorMsg:=errCreateQSO)
                        Else
#If DEBUG Then
                            Debugger.Break() ' should not be happening
#End If
                            'Return Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.None)
                            'updated 10/3/2019 (12/11/2019 in Sprint 10)
                            qqo = Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.None)
                        End If

                    Else
                        'Return VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.None, QuoteId, errCreateQSO, True)
                        'updated 10/3/2019 (12/11/2019 in Sprint 10)
                        qqo = VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.None, QuoteId, errCreateQSO, True)
                    End If
                End If
                loadErrorMsg = errCreateQSO 'added 12/18/2019
            End If

            'added 10/3/2019 (12/11/2019 in Sprint 10)
            If qqo Is Nothing Then
                _FailedToLoadQuoteOnLastRequest = True
                _ErrorMessageFromLastQuoteRequest = loadErrorMsg
                If TypeOf Me Is ctlVr3Stats = False AndAlso UseRatedQuoteImage = False Then 'added 10/4/2019; note: logic from VRControlBase.ascx.vb.VRControlBase_Init
                    Response.Redirect("Vr3InvalidQuote.aspx", True)
                    Response.End()
                End If
            End If
            Return qqo

        End Get
    End Property
    'added 10/3/2019 (12/11/2019 in Sprint 10)
    Private _QuoteWasRequested As Boolean = False
    Public ReadOnly Property QuoteWasRequested As Boolean
        Get
            Return _QuoteWasRequested
        End Get
    End Property
    Private _FailedToLoadQuoteOnLastRequest As Boolean = False
    Public ReadOnly Property FailedToLoadQuoteOnLastRequest As Boolean
        Get
            Return _FailedToLoadQuoteOnLastRequest
        End Get
    End Property
    Private _ErrorMessageFromLastQuoteRequest As String = ""
    Public ReadOnly Property ErrorMessageFromLastQuoteRequest As String
        Get
            Return _ErrorMessageFromLastQuoteRequest
        End Get
    End Property

    ''' <summary>
    ''' Set this via markup on any controls that use the rated image before using property 'Quote'
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UseRatedQuoteImage As Boolean ' defaults to false
        Get
            Return If(ViewState("vs_useratedimage") Is Nothing, False, CBool(ViewState("vs_useratedimage")))
        End Get
        Set(value As Boolean)
            ViewState("vs_useratedimage") = value
        End Set
    End Property

    ''' <summary>
    ''' Provides the agencyid as selected by the dropdown on that master page.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property AgencyId As Int32
        Get
            'probably better to just know that this is failing - would only happen if you change the masterpage
            Return DirectCast(Me.Page.Master, VelociRater).AgencyID
        End Get
    End Property

    Public ReadOnly Property QuoteId As String
        Get
            If Request.QueryString("quoteid") IsNot Nothing Then
                Return Request.QueryString("quoteid")
            End If
            If Page.RouteData.Values("quoteid") IsNot Nothing Then
                Return Page.RouteData.Values("quoteid").ToString()
            End If


#If DEBUG Then
            Debug.WriteLine("quoteID is null or empty")
            'Debugger.Break()
#End If
            Return ""
        End Get
    End Property

    Public ReadOnly Property IsOnAppPage As Boolean
        Get
            If Me.Page.GetType() IsNot Nothing AndAlso UCase(Me.Page.GetType().ToString).Contains("APP") = True Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property IsCommercialPolicy As Boolean
        Get
            Select Case Quote?.LobType
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialCrime, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialInlandMarine, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                    ' Commercial Lines
                    Return True
                Case QuickQuoteObject.QuickQuoteLobType.Farm
                    ' Farm Line
                    If SubQuoteFirst?.LiabilityOptionId IsNot Nothing AndAlso (SubQuoteFirst?.LiabilityOptionId = "1" OrElse SubQuoteFirst.LiabilityOptionId = "2") Then
                        Return SubQuoteFirst?.LiabilityOptionId = "2" '2 = Commercial
                    Else
                        Return Quote?.Policyholder?.Name?.TypeId = "2"
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal
                    Return Quote?.Policyholder?.Name?.TypeId = "2"
                Case Else
                    ' Personal Lines
                    Return False
            End Select
            Return False
        End Get
    End Property

    ''' <summary>
    ''' This provides access to the ValidationSummary logic that lives on the master page. The validationsummary will display errors/warnings that are in the ValidationHelper object.
    ''' </summary>
    ''' <returns></returns>
    Protected ReadOnly Property ValidationSummmary As ctlValidationSummary
        Get
            Return DirectCast(Me.Page.Master, VelociRater).ValidationSummary()
        End Get
    End Property

    Private validator As New ControlValidationHelper
    ''' <summary>
    ''' A control to manage any validation errors/warnings produced by this control.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ValidationHelper As ControlValidationHelper
        Get
            Return validator
        End Get

    End Property

    Private ReadOnly Property RatedQuote As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            Return DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()
        End Get
    End Property

    Public ReadOnly Property HasRatedQuoteAvailable As Boolean
        Get
            Dim isRated As Boolean = False
            Dim ratedQuote = DirectCast(Me.Page.Master, VelociRater).GetRatedQuotefromCache()
            If ratedQuote IsNot Nothing AndAlso ratedQuote.Success Then
                isRated = True
            End If
            Return isRated
        End Get
    End Property

    'added 2/14/2019
    Private _EndorsementPolicyId As Integer = 0
    Public ReadOnly Property EndorsementPolicyId As Integer
        Get
            If _EndorsementPolicyId <= 0 Then
                LoadEndorsementPolicyIdAndImageNum()
            End If
            Return _EndorsementPolicyId
        End Get
    End Property
    Private _EndorsementPolicyImageNum As Integer = 0
    Public ReadOnly Property EndorsementPolicyImageNum As Integer
        Get
            If _EndorsementPolicyImageNum <= 0 Then
                LoadEndorsementPolicyIdAndImageNum()
            End If
            Return _EndorsementPolicyImageNum
        End Get
    End Property
    Public ReadOnly Property EndorsementPolicyIdAndImageNum As String
        Get
            Dim strEndorsementPolicyIdAndImageNum As String = ""
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("EndorsementPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("EndorsementPolicyIdAndImageNum").ToString) = False Then
                strEndorsementPolicyIdAndImageNum = Request.QueryString("EndorsementPolicyIdAndImageNum").ToString
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("EndorsementPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("EndorsementPolicyIdAndImageNum").ToString) = False Then
                strEndorsementPolicyIdAndImageNum = Page.RouteData.Values("EndorsementPolicyIdAndImageNum").ToString
            End If
            Return strEndorsementPolicyIdAndImageNum
        End Get
    End Property
    Private Sub LoadEndorsementPolicyIdAndImageNum(Optional ByVal reset As Boolean = False)
        If reset = True Then
            _EndorsementPolicyId = 0
            _EndorsementPolicyImageNum = 0
        End If
        Dim strEndorsementPolicyIdAndImageNum As String = EndorsementPolicyIdAndImageNum
        If String.IsNullOrWhiteSpace(strEndorsementPolicyIdAndImageNum) = False Then
            Dim intList As List(Of Integer) = QuickQuoteHelperClass.ListOfIntegerFromString(strEndorsementPolicyIdAndImageNum, delimiter:="|", positiveOnly:=True)
            If intList IsNot Nothing AndAlso intList.Count > 0 Then
                _EndorsementPolicyId = intList(0)
                If intList.Count > 1 Then
                    _EndorsementPolicyImageNum = intList(1)
                End If
            End If
        End If
    End Sub
    Private _ReadOnlyPolicyId As Integer = 0
    Public ReadOnly Property ReadOnlyPolicyId As Integer
        Get
            If _ReadOnlyPolicyId <= 0 Then
                LoadReadOnlyPolicyIdAndImageNum()
            End If
            Return _ReadOnlyPolicyId
        End Get
    End Property
    Private _ReadOnlyPolicyImageNum As Integer = 0
    Public ReadOnly Property ReadOnlyPolicyImageNum As Integer
        Get
            If _ReadOnlyPolicyImageNum <= 0 Then
                LoadReadOnlyPolicyIdAndImageNum()
            End If
            Return _ReadOnlyPolicyImageNum
        End Get
    End Property
    Public ReadOnly Property ReadOnlyPolicyIdAndImageNum As String
        Get
            Dim strReadOnlyPolicyIdAndImageNum As String = ""
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("ReadOnlyPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("ReadOnlyPolicyIdAndImageNum").ToString) = False Then
                strReadOnlyPolicyIdAndImageNum = Request.QueryString("ReadOnlyPolicyIdAndImageNum").ToString
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum").ToString) = False Then
                strReadOnlyPolicyIdAndImageNum = Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum").ToString
            End If
            Return strReadOnlyPolicyIdAndImageNum
        End Get
    End Property
    Private Sub LoadReadOnlyPolicyIdAndImageNum(Optional ByVal reset As Boolean = False)
        If reset = True Then
            _ReadOnlyPolicyId = 0
            _ReadOnlyPolicyImageNum = 0
        End If
        Dim strReadOnlyPolicyIdAndImageNum As String = ReadOnlyPolicyIdAndImageNum
        If String.IsNullOrWhiteSpace(strReadOnlyPolicyIdAndImageNum) = False Then
            Dim intList As List(Of Integer) = QuickQuoteHelperClass.ListOfIntegerFromString(strReadOnlyPolicyIdAndImageNum, delimiter:="|", positiveOnly:=True)
            If intList IsNot Nothing AndAlso intList.Count > 0 Then
                _ReadOnlyPolicyId = intList(0)
                If intList.Count > 1 Then
                    _ReadOnlyPolicyImageNum = intList(1)
                End If
            End If
        End If
    End Sub
    'added 2/21/2019
    Private _QuoteTransactionType As QuickQuoteObject.QuickQuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.None
    Public ReadOnly Property QuoteTransactionType As QuickQuoteObject.QuickQuoteTransactionType
        Get
            If System.Enum.IsDefined(GetType(QuickQuoteObject.QuickQuoteTransactionType), _QuoteTransactionType) = False OrElse _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.None Then
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then
                    _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                Else
                    _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote
                End If
            End If
            Return _QuoteTransactionType
        End Get
    End Property

    Public ReadOnly Property QuoteIdOrPolicyIdPipeImageNumber As String
        Get
            If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then
                Return ReadOnlyPolicyIdAndImageNum
            ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                Return EndorsementPolicyIdAndImageNum
            Else
                Return QuoteId
            End If
        End Get
    End Property

    '''////////////////////////////////////////////////////////////////////////////////////////////
    ''' <summary>
    '''     Gets the quote identifier for new business or (policy identifier underscore image number)
    '''     if related to endorsements. This is JavaScript safe. JavaScript will not accept a pipe.
    ''' </summary>
    '''
    ''' <value> The quote identifier or (policy identifier underscore image number). </value>
    '''////////////////////////////////////////////////////////////////////////////////////////////
    Public ReadOnly Property QuoteIdOrPolicyIdPipeImageNumberJavaScriptSafe As String
        Get
            If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then
                Return ReadOnlyPolicyIdAndImageNum.Replace("|", "_")
            ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                Return EndorsementPolicyIdAndImageNum.Replace("|", "_")
            Else
                Return QuoteId
            End If
        End Get
    End Property

    'added 5/19/2023
    'Public Property QuoteSummaryActionsValidationHelper As ControlValidationHelper
    '    Get
    '        Dim cvh As ControlValidationHelper = Nothing
    '        Dim vs As StateBag = Me.PageViewState
    '        If vs IsNot Nothing Then
    '            If vs("QuoteSummaryActionsValidationHelper") IsNot Nothing Then
    '                cvh = CType(vs("QuoteSummaryActionsValidationHelper"), ControlValidationHelper)
    '            End If
    '        End If
    '        Return cvh
    '    End Get
    '    Set(value As ControlValidationHelper)
    '        Dim vs As StateBag = Me.PageViewState
    '        If vs IsNot Nothing Then
    '            vs("QuoteSummaryActionsValidationHelper") = value
    '        End If
    '    End Set
    'End Property
    'updated 5/22/2023 to use BasePage property
    Public Property QuoteSummaryActionsValidationHelper As ControlValidationHelper
        Get
            Dim cvh As ControlValidationHelper = Nothing
            If TypeOf Me.Page Is BasePage Then
                cvh = CType(Me.Page, BasePage).QuoteSummaryActionsValidationHelper
            End If
            Return cvh
        End Get
        Set(value As ControlValidationHelper)
            If TypeOf Me.Page Is BasePage Then
                CType(Me.Page, BasePage).QuoteSummaryActionsValidationHelper = value
            End If
        End Set
    End Property
    Public Property RiskGradeSearchIsVisible As Boolean
        Get
            Dim isVisible As Boolean = False
            If TypeOf Me.Page Is BasePage Then
                isVisible = CType(Me.Page, BasePage).RiskGradeSearchIsVisible
            End If
            Return isVisible
        End Get
        Set(value As Boolean)
            If TypeOf Me.Page Is BasePage Then
                CType(Me.Page, BasePage).RiskGradeSearchIsVisible = value
            End If
        End Set
    End Property
    Public Property InsuredListProcessedCommercialDataPrefillFirmographicsResultsOnLastSave As Boolean
        Get
            Dim justProcessed As Boolean = False
            If TypeOf Me.Page Is BasePage Then
                justProcessed = CType(Me.Page, BasePage).InsuredListProcessedCommercialDataPrefillFirmographicsResultsOnLastSave
            End If
            Return justProcessed
        End Get
        Set(value As Boolean)
            If TypeOf Me.Page Is BasePage Then
                CType(Me.Page, BasePage).InsuredListProcessedCommercialDataPrefillFirmographicsResultsOnLastSave = value
            End If
        End Set
    End Property
    Public Property HasAttemptedCommercialDataPrefillFirmographicsPreload As Boolean
        Get
            Dim hasTried As Boolean = False
            If TypeOf Me.Page Is BasePage Then
                hasTried = CType(Me.Page, BasePage).HasAttemptedCommercialDataPrefillFirmographicsPreload
            End If
            Return hasTried
        End Get
        Set(value As Boolean)
            If TypeOf Me.Page Is BasePage Then
                CType(Me.Page, BasePage).HasAttemptedCommercialDataPrefillFirmographicsPreload = value
            End If
        End Set
    End Property
    Public Property HasAttemptedCommercialDataPrefillPropertyPreload As Boolean
        Get
            Dim hasTried As Boolean = False
            If TypeOf Me.Page Is BasePage Then
                hasTried = CType(Me.Page, BasePage).HasAttemptedCommercialDataPrefillPropertyPreload
            End If
            Return hasTried
        End Get
        Set(value As Boolean)
            If TypeOf Me.Page Is BasePage Then
                CType(Me.Page, BasePage).HasAttemptedCommercialDataPrefillPropertyPreload = value
            End If
        End Set
    End Property
    Public ReadOnly Property PageViewState As StateBag
        Get
            Dim vs As StateBag = Nothing
            If TypeOf Me.Page Is BasePage Then
                vs = CType(Me.Page, BasePage).PageViewState
            End If
            Return vs
        End Get
    End Property
    Public ReadOnly Property IsSummaryWorkflow As Boolean
        Get
            'Dim isIt As Boolean = False

            'Dim wkflowTxt As String = "Workflow"
            'If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString(wkflowTxt) IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString(wkflowTxt).ToString) = False AndAlso UCase(Request.QueryString(wkflowTxt).ToString) = "SUMMARY" Then
            '    isIt = True
            'End If

            'Return isIt
            Return WebHelper_Personal.IsSummaryWorkflow(Request)
        End Get
    End Property


#Region "MultiState"
    'note: updated SubQuotes from IEnumerable(Of QuickQuote.CommonObjects.QuickQuoteObject) to just use List
    Protected ReadOnly Property SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject)
        Get
            Return IFM.VR.Common.Helpers.MultiState.General.SubQuotes(Me.Quote)

            If Me.Quote IsNot Nothing Then
                If HttpContext.Current.Items("vrControlBase_MultiStateQuickQuoteObjects") Is Nothing Then
                    HttpContext.Current.Items("vrControlBase_MultiStateQuickQuoteObjects") = IFM.VR.Common.Helpers.MultiState.General.SubQuotes(Me.Quote)
                End If
            End If
            Return DirectCast(HttpContext.Current.Items("vrControlBase_MultiStateQuickQuoteObjects"), List(Of QuickQuote.CommonObjects.QuickQuoteObject))
        End Get
    End Property

    Protected ReadOnly Property SubQuoteFirst As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            Return SubQuotes.GetItemAtIndex(0)
        End Get
    End Property

    Protected ReadOnly Property ILSubQuote As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            If QuickQuoteHelperClass.QuoteHasState(Quote, QuickQuoteHelperClass.QuickQuoteState.Illinois) Then
                Return SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
            Else
                Return Nothing
            End If
        End Get
    End Property

    Protected Sub SubQuoteListRefresh()
        HttpContext.Current.Items("vrControlBase_MultiStateQuickQuoteObjects") = Nothing
        ' since this list depends on the subquotes list you should assume you need to pull both fresh next time
        HttpContext.Current.Items("vrControlBase_GoverningState") = Nothing
    End Sub

    Protected Function SubQuotesHasAnyState(ParamArray stateTypes() As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) As Boolean
        Return IFM.VR.Common.Helpers.MultiState.General.SubQuotesHasAnyState(Me.SubQuotes, stateTypes)
    End Function

    Protected Function SubQuotesContainsState(stateAbbreviation As String) As Boolean
        Return IFM.VR.Common.Helpers.MultiState.General.SubQuotesContainsState(Me.SubQuotes, stateAbbreviation)
    End Function

    Protected Function SubQuoteForState(stateType As QuickQuoteHelperClass.QuickQuoteState) As QuickQuote.CommonObjects.QuickQuoteObject
        Return IFM.VR.Common.Helpers.MultiState.General.SubQuoteForState(Me.SubQuotes, stateType)
    End Function

    Protected Function SubQuoteForState(stateAbbreviation As String) As QuickQuote.CommonObjects.QuickQuoteObject
        Return IFM.VR.Common.Helpers.MultiState.General.SubQuoteForState(Me.SubQuotes, stateAbbreviation)
    End Function

    Protected Function SubQuoteForVehicle(ByVal veh As QuickQuoteVehicle) As QuickQuoteObject 'would be used whenever vehicle controls need access to a quote to pull policy level information that could vary state
        Return IFM.VR.Common.Helpers.MultiState.General.SubQuoteForVehicle(Me.SubQuotes, veh)
    End Function

    Protected Function SubQuoteForLocation(ByVal loc As QuickQuoteLocation) As QuickQuoteObject 'would be used whenever location controls need access to a quote to pull policy level information that could vary state
        Return IFM.VR.Common.Helpers.MultiState.General.SubQuoteForLocation(Me.SubQuotes, loc)
    End Function

    Protected Function GoverningStateQuote() As QuickQuoteObject 'added 8/15/2018
        If Me.Quote IsNot Nothing Then
            'If HttpContext.Current.Items("vrControlBase_GoverningState") Is Nothing Then
            '    If System.Enum.IsDefined(GetType(QuickQuoteHelperClass.QuickQuoteState), Me.Quote.QuickQuoteState) = True AndAlso Me.Quote.QuickQuoteState <> QuickQuoteHelperClass.QuickQuoteState.None Then
            '        HttpContext.Current.Items("vrControlBase_GoverningState") = SubQuoteForState(Me.Quote.QuickQuoteState)
            '    Else
            '        HttpContext.Current.Items("vrControlBase_GoverningState") = SubQuoteFirst
            '    End If
            'End If
            'Dim gS = HttpContext.Current.Items("vrControlBase_GoverningState")
            'updated 10/9/2019
            Dim govStateQuoteName As String = "vrControlBase_GoverningState"
            If UseRatedQuoteImage = True Then
                govStateQuoteName &= "_Rated"
            End If
            If HttpContext.Current.Items(govStateQuoteName) Is Nothing Then
                If System.Enum.IsDefined(GetType(QuickQuoteHelperClass.QuickQuoteState), Me.Quote.QuickQuoteState) = True AndAlso Me.Quote.QuickQuoteState <> QuickQuoteHelperClass.QuickQuoteState.None Then
                    HttpContext.Current.Items(govStateQuoteName) = SubQuoteForState(Me.Quote.QuickQuoteState)
                Else
                    HttpContext.Current.Items(govStateQuoteName) = SubQuoteFirst
                End If
            End If
            Dim gS = HttpContext.Current.Items(govStateQuoteName)
#If DEBUG Then
            If gS Is Nothing Then
                Debugger.Break()
            End If
#End If
            Return DirectCast(gS, QuickQuoteObject)
        End If
        Return Nothing
    End Function
    '10/5/2018 - changed from Function to Property; also updated to not use HttpContext for now; reverted back 10/6/2018
    'Protected ReadOnly Property GoverningStateQuote As QuickQuoteObject
    '    Get
    '        If Me.Quote IsNot Nothing Then
    '            If System.Enum.IsDefined(GetType(QuickQuoteHelperClass.QuickQuoteState), Me.Quote.QuickQuoteState) = True AndAlso Me.Quote.QuickQuoteState <> QuickQuoteHelperClass.QuickQuoteState.None Then
    '                Return SubQuoteForState(Me.Quote.QuickQuoteState)
    '            Else
    '                Return SubQuoteFirst
    '            End If

    '            If HttpContext.Current.Items("vrControlBase_GoverningState") Is Nothing Then
    '                If System.Enum.IsDefined(GetType(QuickQuoteHelperClass.QuickQuoteState), Me.Quote.QuickQuoteState) = True AndAlso Me.Quote.QuickQuoteState <> QuickQuoteHelperClass.QuickQuoteState.None Then
    '                    HttpContext.Current.Items("vrControlBase_GoverningState") = SubQuoteForState(Me.Quote.QuickQuoteState)
    '                Else
    '                    HttpContext.Current.Items("vrControlBase_GoverningState") = SubQuoteFirst
    '                End If
    '            End If
    '            Return DirectCast(HttpContext.Current.Items("vrControlBase_GoverningState"), QuickQuoteObject)
    '        End If
    '        Return Nothing
    '    End Get
    'End Property
#End Region

#Region "Helper Methods"
    Public Sub LockTree()
        Me.VRScript.AddScriptLine("ifm.vr.ui.LockTree_Freeze(); ")
    End Sub

    Public Sub UnlockTree()
        Me.VRScript.AddScriptLine("ifm.vr.ui.UnLockTree(); ")
    End Sub

    ''' <summary>
    ''' There are almost no place that you should call this via code behind.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LockForm()
        ' I use when you make it to the app page but the quote isn't rated. Should never happen in real world.
        Me.VRScript.AddScriptLine("DisableMainFormOnSaveRemoves();")
    End Sub

    'Added 1/11/2022 for Bug 67521 MLW - Esig Feature Flag
    Protected Function hasEsigOption() As Boolean
        Dim esigLOBList = System.Configuration.ConfigurationManager.AppSettings("Esignature_LobsToAllow").CSVtoList
        If Quote IsNot Nothing AndAlso esigLOBList.Contains(Quote.LobType.ToString) Then
            Return True
        End If
        Return False
    End Function

    Protected Function IsQuoteEndorsement() As Boolean
        If Quote IsNot Nothing AndAlso Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
            Return True
        End If
        Return False
    End Function

    Protected Function IsQuoteReadOnly() As Boolean
        If Quote IsNot Nothing AndAlso Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
            Return True
        End If
        Return False
    End Function
    Protected Function IsEndorsementRelated() As Boolean
        If Quote IsNot Nothing AndAlso Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
            Return True
        End If
        Return False
    End Function

    Protected Function IsCommercialQuote() As Boolean
        If Quote IsNot Nothing Then
            Return CommercialQuoteHelper.IsCommercialLob(Quote.LobType)
        Else
            Return False
        End If
    End Function

    Protected Function IsNewCo() As Boolean
        If Quote IsNot Nothing Then
            Return IFM.VR.Common.Helpers.NewCompanyIdHelper.IsNewCompany(Quote)
            'If Quote.Company = QuickQuoteHelperClass.QuickQuoteCompany.IndianaFarmersIndemnity Then
            '    Return True
            'Else
            '    Return False
            'End If
        Else
            Return False
        End If
    End Function

    Protected Function ValidYear(ByVal testYear As String) As Boolean
        If Not IsNumeric(testYear) Then Return False
        Dim yr As Integer = CInt(testYear)
        If yr > DateTime.Now.Year Then Return False
        If yr < 1900 Then Return False
        Return True
    End Function

    'Added 8/10/2022 for task 76302 MLW
    Protected Function IsPreexistingLocationOnEndorsement(MyLocationIndex As Integer) As Boolean
        Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
        If endorsementsPreexistHelper.IsPreexistingLocation(MyLocationIndex) Then
            Return True
        Else
            Return False
        End If
    End Function

    'Added 12/03/2020 for CAP Endorsements Task 52973 MLW
    Protected Function IsNewDriverOnEndorsement(MyDriver As QuickQuoteDriver) As Boolean
        If Quote IsNot Nothing AndAlso Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
            Return QQHelper.IsQuickQuoteDriverNewToImage(MyDriver, Me.Quote)
        End If
        Return False
    End Function

    'Added 12/08/2020 for CAP Endorsements Task 52974 MLW
    Protected Function IsNewVehicleOnEndorsement(MyVehicle As QuickQuoteVehicle) As Boolean
        If Quote IsNot Nothing AndAlso Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
            Return QQHelper.IsQuickQuoteVehicleNewToImage(MyVehicle, Me.Quote)
        End If
        Return False
    End Function

    'Added 12/24/2020 for CAP Endorsements Task 52974 MLW
    Protected Function IsNewAdditionalInterestOnEndorsement(MyAdditionalInterest As QuickQuoteAdditionalInterest) As Boolean
        If Quote IsNot Nothing AndAlso Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
            Return QQHelper.IsQuickQuoteAdditionalInterestNewToImage(MyAdditionalInterest, Me.Quote)
        End If
        Return False
    End Function

    ''03/07/2023 for CPP Endorsements Task 73928
    'Protected Function IsNewLocationOnEndorsement(location As QuickQuoteLocation) As Boolean
    '    If Quote IsNot Nothing AndAlso Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
    '        Return QQHelper.IsQuickQuoteLocationNewToImage(location, Me.Quote)
    '    End If
    '    Return False
    'End Function

    ''03/07/2023 for CPP Endorsements Task 73928
    'Protected Function IsNewbuildingOnEndorsement(building As QuickQuoteBuilding) As Boolean
    '    If Quote IsNot Nothing AndAlso Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
    '        Return QQHelper.IsQuickQuoteBuildingNewToImage(building, Me.Quote)
    '    End If
    '    Return False
    'End Function

    'Added 12/22/2020 for CAP Endorsements Task 52971 MLW
    Protected Function TypeOfEndorsement() As String
        If Quote IsNot Nothing AndAlso Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
            Return QQDevDictionary_GetItem("Type_Of_Endorsement_Selected")
        End If
        Return Nothing
    End Function

    'Added 6/2/2021 for CAP Endorsements Task 52974 MLW
    Protected Function GetCAPOriginalVehicleAIAssignmentList() As String
        'Gets the DevDictionary List that has the AI to vehicle assignments when the endorsement was created. 
        'List Is vehicleNum And AI listId. Ex: 1==167037&&2==43686
        If Quote IsNot Nothing AndAlso Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
            Return QQDevDictionary_GetItem("CAPEndorsements_Original_Vehicle_AIs")
        End If
        Return Nothing
    End Function

    Private Function GetPageNameForDevDictionary(isPageSpecific As Boolean) As String
        Dim pageName As String = ""

        If isPageSpecific = True Then
            pageName = _senderName
        Else
            pageName = "global"
        End If
        Return pageName
    End Function

    Public Sub QQDevDictionary_SetItem(key As String, value As String, Optional isPageSpecific As Boolean = False)
        Me.Quote.SetDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key, value)
    End Sub

    Public Sub QQDevDictionary_SetItem(key As String, value As String, stateAbbreviation As String, Optional isPageSpecific As Boolean = False)
        Me.Quote.SetDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key, value, stateAbbreviation)
    End Sub

    Public Sub QQDevDictionary_SetItem(key As String, value As String, state As QuickQuoteHelperClass.QuickQuoteState, Optional isPageSpecific As Boolean = False)
        Me.Quote.SetDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key, value, state)
    End Sub

    Public Sub QQDevDictionary_SetItem(key As String, value As String, listControlIndex As Integer, Optional isPageSpecific As Boolean = False)
        Me.Quote.SetDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key, value, listControlIndex)
    End Sub

    Public Sub QQDevDictionary_SetItem(key As String, value As String, listControlIndex As Integer, state As QuickQuoteHelperClass.QuickQuoteState, Optional isPageSpecific As Boolean = False)
        Me.Quote.SetDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key, value, listControlIndex, state)
    End Sub

    Public Sub QQDevDictionary_SetItem(key As String, value As String, listControlIndex As Integer, stateAbbreviation As String, Optional isPageSpecific As Boolean = False)
        Me.Quote.SetDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key, value, listControlIndex, stateAbbreviation)
    End Sub

    Public Function QQDevDictionary_GetItem(key As String, Optional isPageSpecific As Boolean = False) As String
        Return Me.Quote.GetDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key)
    End Function

    Public Function QQDevDictionary_GetItem(key As String, listControlIndex As Integer, Optional isPageSpecific As Boolean = False) As String
        Return Me.Quote.GetDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key, listControlIndex)
    End Function

    Public Function QQDevDictionary_GetItem(key As String, stateAbbreviation As String, Optional isPageSpecific As Boolean = False) As String
        Return Me.Quote.GetDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key, stateAbbreviation)
    End Function

    Public Function QQDevDictionary_GetItem(key As String, state As QuickQuoteHelperClass.QuickQuoteState, Optional isPageSpecific As Boolean = False) As String
        Return Me.Quote.GetDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key, state)
    End Function

    Public Function QQDevDictionary_GetItem(key As String, listControlIndex As Integer, stateAbbreviation As String, Optional isPageSpecific As Boolean = False) As String
        Return Me.Quote.GetDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key, listControlIndex, stateAbbreviation)
    End Function

    Public Function QQDevDictionary_GetItem(key As String, listControlIndex As Integer, state As QuickQuoteHelperClass.QuickQuoteState, Optional isPageSpecific As Boolean = False) As String
        Return Me.Quote.GetDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key, listControlIndex, state)
    End Function

    Public Sub QQDevDictionary_RemoveItem(key As String, listControlIndex As Integer, Optional isPageSpecific As Boolean = False)
        Me.Quote.RemoveDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key, listControlIndex)
    End Sub

    Public Sub QQDevDictionary_RemoveItem(key As String, stateAbbreviation As String, Optional isPageSpecific As Boolean = False)
        Me.Quote.RemoveDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key, stateAbbreviation)
    End Sub

    Public Sub QQDevDictionary_RemoveItem(key As String, state As QuickQuoteHelperClass.QuickQuoteState, Optional isPageSpecific As Boolean = False)
        Me.Quote.RemoveDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key, state)
    End Sub

    Public Sub QQDevDictionary_RemoveItem(key As String, listControlIndex As Integer, stateAbbreviation As String, Optional isPageSpecific As Boolean = False)
        Me.Quote.RemoveDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key, listControlIndex, stateAbbreviation)
    End Sub

    Public Sub QQDevDictionary_RemoveItem(key As String, listControlIndex As Integer, state As QuickQuoteHelperClass.QuickQuoteState, Optional isPageSpecific As Boolean = False)
        Me.Quote.RemoveDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key, listControlIndex, state)
    End Sub

    Public Sub QQDevDictionary_RemoveItem(key As String, Optional isPageSpecific As Boolean = False)
        Me.Quote.RemoveDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key)
    End Sub

    Protected Friend Sub SetSender(sender As Object)
        If sender IsNot Nothing Then
            _senderName = sender.GetType.Name
            _senderName = _senderName.Replace("user_controls_", "")
            If _myAccordianIndex > 0 Then
                _senderName = _senderName & "_" & _myAccordianIndex
            End If
        End If
    End Sub



#End Region
    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub
    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        Me.ValidationHelper.ControlWasRendered = True
    End Sub

#Region "API Calls"
    Public Sub CallCapeComPreLoad()
        Dim ih As New IntegrationHelper
        ih.CallCapeComPreLoad(Me.Quote)
    End Sub
    Public Sub CallBetterViewComPreLoad()
        Dim ih As New IntegrationHelper
        ih.CallBetterViewComPreLoad(Me.Quote)
    End Sub
    Public Sub CallProtectionClassPreLoad()
        Dim ih As New IntegrationHelper
        ih.CallProtectionClassPreload(Me.Quote)

    End Sub

#End Region



End Class