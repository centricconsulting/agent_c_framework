Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
#If DEBUG Then
Imports System.Diagnostics
#End If

'Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
'Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
'Imports QuickQuote.CommonObjects.QuickQuoteObject
'Imports IFM.VR.Web.Helpers.WebHelper_Personal
'Imports IFM.Common.InputValidation.InputHelpers

Public MustInherit Class VRControlBase
    Inherits VrControlBaseEssentials

    Public Event SaveRequested(args As VrControlBaseSaveEventArgs)
    Public Event PopulateRequested()

    Public Enum BroadCastEventType
        ThirdPartyReportOrdered = 1
        PrefillAddedDriversOrVehicles = 2
        PreFillRequested = 3
        RateRequested = 4
        DoHOMCreditRequest = 5
    End Enum
    Public Event BroadcastGenericEvent(type As BroadCastEventType)
    Public Event BroadcastWorkflowChangeRequestEvent(type As IFM.VR.Common.Workflow.Workflow.WorkflowSection, subworkflowParm As String)

#Region "Properties"
    
    ''' <summary>
    ''' This is the client id of the div that is used as the accordion header of this control.
    ''' </summary>
    ''' <returns></returns>
    Public Property MainAccordionDivId As String
        Get
            If ViewState("vs_MainAcordId_") IsNot Nothing Then
                Return ViewState("vs_MainAcordId_")?.ToString
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_MainAcordId_") = value
        End Set
    End Property

    ''' <summary>
    ''' This is the client id of the div that is used as the accordion header of this list in this control.
    ''' </summary>
    ''' <returns></returns>
    Public Property ListAccordionDivId As String
        Get
            If ViewState("vs_ListAcordId_") IsNot Nothing Then
                Return ViewState("vs_ListAcordId_")?.ToString
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_ListAcordId_") = value
        End Set
    End Property

    ''' <summary>
    ''' Gets a list of accordions and their set panel indexes.
    ''' If this is no working properly check that all the children of vr controls that contain an accordion has the MyAccordionIndex set usual to something like vehicleindex/locationindex/violationindex.
    ''' You set the MyAccordionIndex of the children of a 'list control'. This logic walks up the vr control stack when it finds an accordion in a parent control it uses that MyAccordionIndex along with the info about the accordion at the parent to create a way to open all accordions.
    ''' It is also possible the accordion(list) control doesn't have the ListAccordionDivId property set.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MyAccordionList As List(Of VRAccordionTogglePair)
        Get
            Dim accordList As New List(Of VRAccordionTogglePair)
            Me.SetAccordionTogglePairStack(accordList)
            Return accordList
        End Get
    End Property

    Private ReadOnly Property Internal_MyAccords As List(Of VRAccordionTogglePair)
        Get

            Dim accordLevels As New List(Of VRAccordionTogglePair)

            ' !!!!!!!   do ListAccordionDivId first then MainAccordionDivId  !!!!!!
            If String.IsNullOrWhiteSpace(ListAccordionDivId) = False Then
                accordLevels.Add(New VRAccordionTogglePair(ListAccordionDivId, "0")) ' this value needs to get replaced by the calling child control so override SetAccordionTogglePairStack() on the child control see ctl_FarBuiling.vb for example
            End If

            If String.IsNullOrWhiteSpace(MainAccordionDivId) = False Then
                accordLevels.Add(New VRAccordionTogglePair(MainAccordionDivId, "0"))
            End If
            Return accordLevels
        End Get
    End Property

    Private Sub SetAccordionTogglePairStack(list As List(Of VRAccordionTogglePair))
        list.AddRange(Internal_MyAccords)
        If Me.ParentVrControl.Equals(Me) = False Then
            If String.IsNullOrWhiteSpace(Me.ParentVrControl.ListAccordionDivId) = False Then
                Dim nextIndex As Int32 = list.Count ' hold onto what will be the next index
                Me.ParentVrControl.SetAccordionTogglePairStack(list)
                If list.Count > nextIndex Then
                    Dim lastItem = list(nextIndex)
                    If lastItem IsNot Nothing Then
                        lastItem.AccordIndex = MyAccordionIndex.ToString()
                    End If
                End If
            Else
                Me.ParentVrControl.SetAccordionTogglePairStack(list)
            End If

        End If
    End Sub

    Protected Overridable ReadOnly Property MyAccordionIndex As Int32
        Get
            Return 0
        End Get
    End Property

    '''////////////////////////////////////////////////////////////////////////////////////////////
    ''' <summary>   Gets the ViewState of the Page. </summary>
    '''
    ''' <value> The ViewState. </value>
    '''
    ''' ### <example>
    '''     ParentVrControl.VrViewState("showContractorSection") : Gets the ViewState Variable
    '''     "showContractorSection" from the current control's Parent's Viewstate
    ''' </example>
    '''////////////////////////////////////////////////////////////////////////////////////////////
    Public ReadOnly Property VrViewState
        Get
            Return Me.ViewState
        End Get
    End Property


    Public _ParentVrControl As VRControlBase
    ''' <summary>
    ''' This is the Vr Control that holds this control.
    ''' </summary>
    Public Property ParentVrControl As VRControlBase
        Get
            If _ParentVrControl Is Nothing Then ' don't want any nulls so if no vrparent then you are the vrparent
#If DEBUG Then
                Debug.WriteLine("My Parent is null. This is unusual.-" + Me.ClientID)
#End If
                Return Me
            End If
            Return _ParentVrControl
        End Get
        Set(value As VRControlBase)
            _ParentVrControl = value
        End Set
    End Property

    ''' <summary>
    ''' This is a list of all direct child Vr controls inside this control.
    ''' </summary>
    Public ChildVrControls As New List(Of VRControlBase)


    'Protected ReadOnly Property DiamondVersion As String
    '    Get
    '        Return System.Configuration.ConfigurationManager.AppSettings("diamondVersion")
    '    End Get
    'End Property


    ''' <summary>
    ''' When true this control will call all child control saves then persist changes to database.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PerformsDirectSave As Boolean

    ''' <summary>
    ''' Provides the type of validation that will be performed(such as quote,app,finalize) when validations are performed. It is based on whether the control is on an app page or not.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property DefaultValidationType As IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType '1/1/15 Matt A
        Get
            Return If(Me.IsOnAppPage, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate, VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate)
            'If Me.IsQuoteEndorsement Then
            '    Return Validation.ObjectValidation.ValidationItem.ValidationType.endorsement
            'Else
            '    Return If(Me.IsOnAppPage, IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType.appRate, VR.Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate)
            'End If
        End Get
    End Property

    ''' <summary>
    ''' When true this control will not get the normal save, populate, validate calls because it will be excluded from the child vr list.
    ''' *******  Almost never used. **********
    ''' It will however still know who its parent is.
    ''' </summary>
    ''' <returns></returns>
    Public Property HideFromParent As Boolean
        Get
            If ViewState("vs_I_Hide") IsNot Nothing Then
                Return CBool(ViewState("vs_I_Hide"))
            End If
            Return False
        End Get
        Set(value As Boolean)
            ViewState("vs_I_Hide") = value
        End Set
    End Property

    Private _myId As String = Guid.NewGuid().ToString() ' Matt A 5-18-2017
    Public ReadOnly Property MyId As String
        Get
            Return _myId
        End Get
    End Property

#End Region
    Public MustOverride Sub LoadStaticData()
    Public MustOverride Sub Populate()
    Public MustOverride Function Save() As Boolean
    Public Overridable Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
    End Sub
    Public Overridable Sub ValidateControl(valArgs As VRValidationArgs)
        'Me.ValidationHelper.Clear() ' Not needed Matt A - 10-14-14
    End Sub

    ''' <summary>
    ''' This script will be added when ever the control is loaded.
    ''' </summary>
    ''' <remarks></remarks>
    Public MustOverride Sub AddScriptAlways()

    ''' <summary>
    ''' This script will only be added when the control renders on the screen. Fired by PreRender control event.
    ''' </summary>
    ''' <remarks></remarks>
    Public MustOverride Sub AddScriptWhenRendered()

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.ChildVrControls.Clear()
        AttachControlEvents(Me)
        AddScriptAlways()
        Me.ValidationSummmary.RegisterValidationHelper(Me.ValidationHelper) 'Matt A - 10-21-14
        _myAccordianIndex = MyAccordionIndex
        SetSender(sender)
    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        AddScriptWhenRendered()
    End Sub

    ''' <summary>
    ''' Goes through the whole control and attaches the base events.
    ''' </summary>
    ''' <param name="control"></param>
    ''' <remarks></remarks>
    Private Sub AttachControlEvents(control As Control, Optional OnlyFindChildren As Boolean = False)
        'this does this so the dev doesn't have to remember to do so - it stinks that it is done every time but it isn't too expensive
        For Each c As Control In control.Controls
            If TypeOf c Is VRControlBase Then
                Dim con As VRControlBase = CType(c, VRControlBase)
                If con.HideFromParent = False Then ' some controls want to be excluded from normal bubbling calls like save, populate, ect
                    Me.ChildVrControls.Add(CType(c, VRControlBase))
                End If
                con.ParentVrControl = Me
                If OnlyFindChildren = False Then
                    AddHandler DirectCast(c, VRControlBase).PopulateRequested, AddressOf Populate_FirePopulateEvent
                    AddHandler DirectCast(c, VRControlBase).SaveRequested, AddressOf Save_FireSaveEvent
                    AddHandler DirectCast(c, VRControlBase).BroadcastGenericEvent, AddressOf Fire_GenericBoardcastEvent
                    AddHandler DirectCast(c, VRControlBase).BroadcastWorkflowChangeRequestEvent, AddressOf Fire_BroadcastWorkflowChangeRequestEvent
                End If
            Else
                ' could put in logic to eliminate some control types here ?? turns out it is no faster with this but it does reduce the stack size due to the lack of additional recursive calls
                If TypeOf c Is TextBox = False AndAlso TypeOf c Is DropDownList = False AndAlso TypeOf c Is HyperLink = False AndAlso TypeOf c Is Button = False AndAlso TypeOf c Is Label = False AndAlso TypeOf c Is CheckBox = False AndAlso TypeOf c Is RadioButton = False AndAlso TypeOf c Is LiteralControl = False Then
                    AttachControlEvents(c, OnlyFindChildren)
                End If
            End If
        Next
    End Sub

    ''' <summary>
    ''' Finds any direct child Vr controls that this control contains. Ran at load automatically but can(should) be ran again after dynamic controls are added.
    ''' </summary>
    Public Sub FindChildVrControls()
        Me.ChildVrControls.Clear()
        Me.AttachControlEvents(Me, True)
    End Sub

    ''' <summary>
    ''' Bubbles up a populate request until it hits the workflowmanager control. At that point it calls Populate() which should cascade down to all controls under the workflow manager control.
    ''' </summary>
    Protected Sub Populate_FirePopulateEvent()
        If PerformsDirectSave Then
            Me.Populate()
        End If
        RaiseEvent PopulateRequested()
    End Sub

    ''' <summary>
    ''' Call this to perform a save. Generally you do not call the controls Save() method directly. You call this, it bubbles until it gets
    ''' to a 'Workflow' control. That workflow control will call Save() that will bubble back up and do the actual save.
    ''' </summary>
    ''' <param name="args"></param>
    ''' <remarks></remarks>
    Protected Sub Save_FireSaveEvent(args As VrControlBaseSaveEventArgs) 'args As VrControlBaseSaveEventArgs)

        'If TypeOf Me Is ctl_Master_HOM_APP Then 'this can help in debugging
        '    Dim a As Int32 = 0
        'End If

        If PerformsDirectSave Then ' normally the event is just bubbled up but you might need this control to act as a stand alone and do its own saves
            ' this usually only happens on the master controls
            Me.Save()
            If args Is Nothing OrElse (args IsNot Nothing AndAlso args.SaveToObjectsButNotToDatabase = False) Then
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/15/2019; original logic in ELSE
                    'no Save needed
                ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    Dim endorsementSaveError As String = ""
                    Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=endorsementSaveError, saveTypeView:=If(Me.IsOnAppPage = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote))
                    'Added 10/28/2021 for BOP Endorsements Task 65882 MLW
                    If successfulEndorsementSave = True AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                        Quote.CopyProfessionalLiabilityCoveragesFromPolicyToBuildings_UseBuildingClassificationList()
                    End If
                    IFM.VR.Common.Helpers.QuickQuoteObjectHelper.CheckQuoteForKillorStopEvent(Me.Quote, Me.Page, Response, Me.Session)
                Else
                    If Me.IsOnAppPage Then
                        VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, Nothing, QuickQuoteXML.QuickQuoteSaveType.AppGap)
                    Else
                        VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, Nothing)
                    End If
                    IFM.VR.Common.Helpers.QuickQuoteObjectHelper.CheckQuoteForKillorStopEvent(Me.Quote, Me.Page, Response, Me.Session)
                End If

            End If


            AfterSaveOccurrs(args)

            If Not TypeOf Me Is VRMasterControlBase Then
                If args IsNot Nothing AndAlso args.InvokeValidations AndAlso Me.ChildVrControls.Any() Then
                    ' always save then validate - you need to do this because some controls will look at other areas so they need the newest data not the pre-save data
                    For Each v As VRControlBase In Me.ChildVrControls
                        v.ValidateControl(args.ValidationArgs)
                    Next
                End If
            Else
                Dim mCon As VRMasterControlBase = CType(Me, VRMasterControlBase)
                If args IsNot Nothing AndAlso args.InvokeValidations AndAlso mCon.ControlsToValidate_Custom.Any() Then
                    ' always save then validate - you need to do this because some controls will look at other areas so they need the newest data not the pre-save data
                    For Each v As VRControlBase In mCon.ControlsToValidate_Custom
                        v.ValidateControl(args.ValidationArgs)
                    Next
                End If
            End If

        End If

        RaiseEvent SaveRequested(args)

    End Sub

    ''' <summary>
    ''' An overload to simplify raising the save event. This will cause validations to fire by default and uses the 'DefaultValidationType' property.
    ''' </summary>
    Protected Sub Save_FireSaveEvent(Optional invokeValidations As Boolean = True)
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, invokeValidations, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
    End Sub



    ''' <summary>
    ''' An overload to simplify raising the save event. Offers the ability to modify whether validations should be invoke and what the validation type is as well.
    ''' </summary>
    Protected Sub Save_FireSaveEvent(invokeValidations As Boolean, valType As Validation.ObjectValidation.ValidationItem.ValidationType)
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, invokeValidations, New IFM.VR.Web.VRValidationArgs(valType)))
    End Sub

    Protected Sub Fire_GenericBoardcastEvent(type As BroadCastEventType)
        RaiseEvent BroadcastGenericEvent(type)
        If type = BroadCastEventType.RateRequested Then
            Populate_FirePopulateEvent()
        End If
    End Sub

    ''' <summary>
    ''' Fires an event up until it finds a workflow manager control. At that point it calls [SetCurrentWorkFlow(type, "")]
    ''' </summary>
    ''' <param name="type"></param>
    Protected Sub Fire_BroadcastWorkflowChangeRequestEvent(type As IFM.VR.Common.Workflow.Workflow.WorkflowSection, subworkflowParm As String)
        If TypeOf Me Is VRMasterControlBase Then
            'handle
            DirectCast(Me, VRMasterControlBase).SetCurrentWorkFlow(type, subworkflowParm)
        End If

        'continue passing it up
        RaiseEvent BroadcastWorkflowChangeRequestEvent(type, subworkflowParm)

    End Sub

    ''' <summary>
    ''' Usually only used on a 'Workflow' control to do something after a save is completed.
    ''' </summary>
    ''' <remarks></remarks>
    Public Overridable Sub AfterSaveOccurrs(args As VrControlBaseSaveEventArgs)
        'do nothing override if you need too
    End Sub

    ''' <summary>
    ''' Usually only used on a 'Workflow' control to do something right before rate.
    ''' </summary>
    ''' <remarks></remarks>
    Public Overridable Sub PreRateEvents()
        'do nothing override if you need too
    End Sub

    ''' <summary>
    ''' Calls the EffectiveDateChanged sub of all direct child Vr controls. Those child controls should bubble that populate all the way down the control stack.
    ''' </summary>
    Public Sub EffectiveDateChangedNotifyChildControls(NewEffectiveDate As String, OldEffectiveDate As String)
        If ChildVrControls.Any() = False Then
            FindChildVrControls()
        End If
        For Each i In Me.ChildVrControls
            i.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
        Next
    End Sub

    ''' <summary>
    ''' Calls the populate sub of all direct child Vr controls. Those child controls should bubble that populate all the way down the control stack.
    ''' </summary>
    Public Sub PopulateChildControls()
        If ChildVrControls.Any() = False Then
            FindChildVrControls()
        End If
        For Each i In Me.ChildVrControls
            i.Populate()
        Next
    End Sub

    ''' <summary>
    ''' Reloads ChildControls and then calls the populate sub of all direct child Vr controls. Those child controls should bubble that populate all the way down the control stack.
    ''' </summary>
    Public Sub FindAndPopulateChildControls()
        FindChildVrControls()
        For Each i In Me.ChildVrControls
            i.Populate()
        Next
    End Sub

    ''' <summary>
    ''' Calls the validate sub of all direct child Vr controls. Those child controls should bubble that validation call all the way down the control stack.
    ''' </summary>
    ''' <param name="valArgs"></param>
    Public Sub ValidateChildControls(valArgs As VRValidationArgs)
        If ChildVrControls.Any() = False Then
            FindChildVrControls()
        End If
        For Each i In Me.ChildVrControls
            i.ValidateControl(valArgs)
        Next
    End Sub

    ''' <summary>
    ''' Calls the save sub of all direct child Vr controls. Those child controls should bubble that save call all the way down the control stack.
    ''' </summary>
    Public Sub SaveChildControls()
        If ChildVrControls.Any() = False Then
            FindChildVrControls()
        End If
        For Each i In Me.ChildVrControls
            i.Save()
        Next
    End Sub

    Protected Sub ClearChildControls()
        If ChildVrControls.Any() = False Then
            FindChildVrControls()
        End If
        For Each i In Me.ChildVrControls
            i.ClearControl()
        Next
    End Sub

    Public Overridable Sub ClearControl()

        ' test this before uncommenting but should work
        'Dim act As Action(Of Control) = Sub(c As Control)
        '                                    For Each cnt As Control In c.Controls
        '                                        If TypeOf cnt Is TextBox Then
        '                                            DirectCast(cnt, TextBox).Text = ""
        '                                        End If
        '                                        If TypeOf cnt Is DropDownList Then
        '                                            DirectCast(cnt, DropDownList).SetFromValue("")
        '                                        End If
        '                                        If TypeOf cnt Is CheckBox Then
        '                                            DirectCast(cnt, CheckBox).Checked = False
        '                                        End If
        '                                        If TypeOf cnt Is RadioButton Then
        '                                            DirectCast(cnt, RadioButton).Checked = False
        '                                        End If
        '                                        If cnt.Controls.Count > 0 Then
        '                                            act(cnt)
        '                                        End If
        '                                    Next
        '                                End Sub
        'act(Me)



        Me.ClearChildControls()
    End Sub

    ''' <summary>
    ''' Used by parent to check if children report they have a setting set true or false
    ''' </summary>
    Public Overridable Function hasSetting() As Boolean
        ''' Returns bool
    End Function


    ''' <summary>
    ''' Finds the all VrControls of specified type in whole UI stack.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="vrControl"></param>
    ''' <returns></returns>
    Protected Function FindVrControlsOfType(Of T As VRControlBase)() As List(Of T)
        Dim topmostControl = Me.FindFirstVRParentOfType(Of VRMasterControlBase)
        Dim vrControls = topmostControl.GatherChildrenOfType(Of T)(False)

#If DEBUG Then
        If Not vrControls.Any() Then
            Debug.WriteLine("Did not find a any with proper type. - " + Me.ClientID)
            Debugger.Break()
        End If
#End If

        Return vrControls
    End Function




    ''' <summary>
    ''' Finds the first control in the parent stack of the provided Type.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="vrControl"></param>
    ''' <returns></returns>
    Protected Function FindFirstVRParentOfType(Of T As VRControlBase)() As T
        Dim vrControl As VRControlBase = Me
        While vrControl.ParentVrControl.Equals(vrControl) = False
            If TypeOf vrControl.ParentVrControl Is T Then
                Return CType(DirectCast(vrControl.ParentVrControl, Object), T)
            End If
            vrControl = vrControl.ParentVrControl
        End While

#If DEBUG Then
        Debug.WriteLine("Did not find a parent with proper type. - " + Me.ClientID)
        Debugger.Break()
#End If

        Return Nothing
    End Function

    ''' <summary>
    ''' Asks the VrParent to GatherChildrenOfType(T) which could include the calling control if it matches the search Type.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    Protected Function GatherSiblingsOfType(Of T As VRControlBase)() As List(Of T)
        If Me.ParentVrControl.Equals(Me) = False AndAlso Me.ParentVrControl IsNot Nothing Then 'I have a parent that is not myself
            Return Me.ParentVrControl.GatherChildrenOfType(Of T)
        End If
#If DEBUG Then
        Debug.WriteLine("Did not find any siblings with proper type. - " + Me.ClientID)
        Debugger.Break()
#End If

        Return Nothing
    End Function


    ''' <summary>
    ''' By default this finds any VR Control of the provided type of direct children of this control. You can widen the search to the entire child stack if you need too.
    ''' USE WITH CAUTION 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="restrictToDirectChildren"></param>
    ''' <returns></returns>
    Public Function GatherChildrenOfType(Of T As VRControlBase)(Optional restrictToDirectChildren As Boolean = True) As List(Of T)
        Dim matches As New List(Of T)
        If restrictToDirectChildren Then
            For Each vrControl In Me.ChildVrControls
                If TypeOf vrControl Is T Then
                    matches.Add(CType(DirectCast(vrControl, Object), T))
                End If
            Next
        Else
            Dim allControls As New List(Of VRControlBase)
            Dim GetChildren As Action(Of VRControlBase, List(Of VRControlBase)) = Sub(controlToScan As VRControlBase, l As List(Of VRControlBase))
                                                                                      For Each vrControl In controlToScan.ChildVrControls
                                                                                          l.Add(vrControl)
                                                                                      Next
                                                                                      For Each vrControl In controlToScan.ChildVrControls
                                                                                          GetChildren(vrControl, l)
                                                                                      Next
                                                                                  End Sub

            GetChildren(Me, allControls)

            For Each vrControl In allControls
                If TypeOf vrControl Is T Then
                    matches.Add(CType(DirectCast(vrControl, Object), T))
                End If
            Next
        End If
#If DEBUG Then
        If matches.Any() = False Then
            'Debug.WriteLine("Did not find a child with proper type. - " + Me.ClientID)
            'Debugger.Break()
        End If
#End If

        Return matches
    End Function

    Public Sub OpenAllParentAccordionsOnNextLoad(Optional scrollToId As String = "")
        For Each acc In Me.MyAccordionList
            Me.VRScript.AddScriptLine(WebHelper_Personal.SetAccordionOpenTabIndex(acc.AccordDivId, acc.AccordIndex), True)
        Next
        If Not String.IsNullOrWhiteSpace(scrollToId) Then
            Me.VRScript.AddScriptLine("$(""#" + scrollToId + """).focus();", True)

            'Me.VRScript.AddScriptLine("$(""#" + scrollToId + """).scrollTop();", True, True, False, 250)
            Me.VRScript.AddScriptLine("ifm.vr.ui.FlashFocusThenScrollToElement(""" + scrollToId + """);", True, True, False, 250)
        End If
    End Sub

    'removed 10/4/2019 (12/11/2019 from Sprint 10); Invalid Quote now being handled at VrControlBaseEssentials.ascx.Quote
    'Private Sub VRControlBase_Init(sender As Object, e As EventArgs) Handles Me.Init ' Matt A 12-20-17 Reduce exception emails when people are pressing the back button from portal
    '    'If Me.Quote Is Nothing And TypeOf Me Is ctlVr3Stats = False Then
    '    '    Response.Redirect("Vr3InvalidQuote.aspx", True)
    '    '    Response.End()
    '    'End If
    '    'updated 10/3/2019
    '    'If TypeOf Me Is ctlVr3Stats = False AndAlso Me.QuoteWasRequested = True AndAlso Me.Quote Is Nothing Then
    '    '10/4/2019 note: this will not work by itself since it executes before controls would request Me.Quote... errors can then be caught by controls requesting Me.Quote after this check happens
    '    If TypeOf Me Is ctlVr3Stats = False AndAlso Me.QuoteWasRequested = True AndAlso Me.FailedToLoadQuoteOnLastRequest = True Then
    '        'If TypeOf Me Is ctlVr3Stats = False AndAlso Me.Quote Is Nothing Then
    '        Response.Redirect("Vr3InvalidQuote.aspx", True)
    '        Response.End()
    '    End If
    'End Sub



End Class

Public Class VrControlBaseSaveEventArgs
    Public Property Sender As VRControlBase
    Public Property InvokeValidations As Boolean
    Public Property ValidationArgs As VRValidationArgs

    'only use when you know for sure that you will save to the database before the page cycle is completed
    'I have used this during item removal methods when often I am
    '   saving current state(to memory), removing an item, populating from memory, then calling save again (this time doing a database save)
    Public Property SaveToObjectsButNotToDatabase As Boolean

    Public Sub New(_Sender As VRControlBase, _InvokeValidations As Boolean, _doNotSaveToDatabase As Boolean, valArgs As VRValidationArgs)
        Me.Sender = _Sender
        Me.InvokeValidations = _InvokeValidations
        Me.SaveToObjectsButNotToDatabase = _doNotSaveToDatabase
        Me.ValidationArgs = valArgs
    End Sub

    Public Sub New(_Sender As VRControlBase, _InvokeValidations As Boolean, valArgs As VRValidationArgs)
        Me.Sender = _Sender

        Me.InvokeValidations = _InvokeValidations
        ValidationArgs = valArgs
    End Sub

End Class

Public Class VRValidationArgs
    Public Property ValidationType As IFM.VR.Validation.ObjectValidation.ValidationItem.ValidationType

    Public Sub New()
        ValidationType = Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate
    End Sub

    Public Sub New(valType As Validation.ObjectValidation.ValidationItem.ValidationType)
        ValidationType = valType
    End Sub

End Class

Public Class VRAccordionTogglePair
    Public AccordDivId As String
    Public AccordIndex As String
    Public Sub New(_AccordDivId As String, _AccordIndex As String)
        Me.AccordDivId = _AccordDivId
        Me.AccordIndex = _AccordIndex
    End Sub

    Public Overrides Function ToString() As String
        Return String.Format("{0} - {1}", AccordDivId, AccordIndex)
    End Function
End Class


