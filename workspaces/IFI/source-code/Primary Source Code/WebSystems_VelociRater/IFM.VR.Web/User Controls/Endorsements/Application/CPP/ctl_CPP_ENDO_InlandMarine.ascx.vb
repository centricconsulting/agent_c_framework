Imports IFM.VR.Web.Helpers

Public Class ctl_CPP_ENDO_InlandMarine
    Inherits VRControlBase


    Public Event CallTreeUpdate()
    Public Event TriggerTransactionCount(ByRef count As Integer)
    Public Event TriggerUpdateRemarks()

    Private Property _CppDictItems As DevDictionaryHelper.AllCommercialDictionary
    Public ReadOnly Property CppDictItems As DevDictionaryHelper.AllCommercialDictionary
        Get
            If Quote IsNot Nothing Then
                If _CppDictItems Is Nothing Then
                    _CppDictItems = New DevDictionaryHelper.AllCommercialDictionary(Quote)
                End If
            End If
            Return _CppDictItems
        End Get
    End Property

    Public ReadOnly Property TransactionLimitReached As Boolean
        Get
            If Me.ParentVrControl IsNot Nothing AndAlso TypeOf Me.ParentVrControl Is ctl_WorkflowManager_CPP_Endo Then
                Dim Parent = CType(ParentVrControl, ctl_WorkflowManager_CPP_Endo)
                Return Parent.TransactionLimitReached
            End If

            Return False
        End Get
    End Property

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        'cov_BuildersRisk.IM_Parent = Me
        Me.PopulateChildControls()
        'Me.CheckHasPackage()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID, False)
        'Me.VRScript.StopEventPropagation(Me.lnkClear.ClientID, False)
        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        _script.AddScriptLine("$(""#InlandMarineDiv"").accordion({collapsible: false, heightStyle: ""content""});")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub EffectiveDateChanged(qqTranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType, newEffectiveDate As String, oldEffectiveDate As String)
        'cov_Transportation.HandleEffectiveDateChange(qqTranType, newEffectiveDate, oldEffectiveDate)
        Exit Sub
    End Sub

    Public Overrides Function Save() As Boolean
        'Look at PackagePart, look for coverages that have a premium,
        'Create Part.  Remove Part if none exist.
        'Move to Contract Equipment or create a rule.
        ' #ToDo
        'For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
        '    sq.CPP_Has_InlandMarine_PackagePart = False
        'Next
        'Me.CheckHasPackage()



        Me.SaveChildControls()
        'Dim endRemarksHelper = New EndorsementsRemarksHelper(CppDictItems)
        'Dim updatedRemarks As String = endRemarksHelper.UpdateRemarks(EndorsementsRemarksHelper.RemarksType.AllCppRemarks)
        'Quote.TransactionRemark = updatedRemarks
        Populate()

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        'cov_BuildersRisk.IM_Parent = Me
        Me.ValidateChildControls(valArgs)
    End Sub

    'Private Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
    '    For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
    '        sq.CPP_Has_InlandMarine_PackagePart = False
    '    Next
    '    Me.ClearChildControls()
    '    PopulateChildControls()
    '    Save_FireSaveEvent()
    '    Populate()
    'End Sub

    Public Sub Handle_DeleteIM_Request()
        ' Handle the Inland Marine delete request from the treeview
        'Me.lnkClear_Click(Me, New EventArgs())
    End Sub

    Public Sub CheckHasPackage()
        'Look at PackagePart, look for coverages that have a premium,
        'Create Part.  Remove Part if none exist.
        'Move to Contract Equipment or create a rule.
        ' #ToDo

        'If ChildVrControls.Any() = False Then
        '    FindChildVrControls()
        'End If
        'For Each i In Me.ChildVrControls
        '    If i.hasSetting() Then
        '        ' TODO: Does this need to save to all the subquotes?  Chad?
        '        GoverningStateQuote.CPP_Has_InlandMarine_PackagePart = True

        '        Return
        '    End If
        'Next
    End Sub

    Public Function CheckHasOwnersOrTransportaion() As Boolean
        'If ChildVrControls.Any() = False Then
        '    FindChildVrControls()
        'End If
        'For Each i In Me.ChildVrControls
        '    If i.hasSetting() AndAlso (i Is cov_OwnersCargo OrElse i Is cov_Transportation) Then
        '        Return True
        '    End If
        'Next
        'Return False
    End Function

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click, btnRate.Click, lnkSave.Click
        Me.Save_FireSaveEvent()
        Populate()
        If sender.Equals(btnRate) Then
            If Me.ValidationSummmary.HasErrors = False Then
                Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
            End If
        End If
    End Sub

    Private Sub btnContinue_Click(sender As Object, e As EventArgs) Handles btnToCrime.Click
        Me.Save_FireSaveEvent()
        If Me.ValidationSummmary.HasErrors = False Then
            Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.Crime, "")
        End If
    End Sub

    Public Sub ClearTransportation()
        'cov_Transportation.ClearControl()
    End Sub
    Public Sub ReloadTransportation()
        'cov_Transportation.Populate()
    End Sub

    Public Sub TransactionCountRequested(Optional ByRef count As Integer = 0)
        RaiseEvent TriggerTransactionCount(count)
    End Sub

    Public Sub UpdateRemarks()
        RaiseEvent TriggerUpdateRemarks()
    End Sub

    Private Sub PageChanged() Handles ctl_Endo_VehicleAdditionalInterestList.AIChange, ctl_CPP_ENDO_App_InlandMarine.AIChange 'added 9/22/2017
        'Populate()
        UpdateRemarks()

        TransactionCountRequested()
        If TransactionLimitReached Then
        Else
        End If
        RaiseEvent CallTreeUpdate()
        PopulateChildControls()
    End Sub

End Class