Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Helpers
Imports IFM.VR.Common

<Serializable()>
Public Class ControlValidationHelper
    Dim _ControlWasRendered As Boolean = False
    Public Property ControlWasRendered As Boolean
        Get
            Return _ControlWasRendered
        End Get
        Set(value As Boolean)
            If _ControlWasRendered = False Then ' once set to true there is no going back Matt A 3-22-16
                _ControlWasRendered = value
            End If
        End Set
    End Property

    Dim _ItemsWereCopiedToOtherValidationhelper As Boolean = False
    ''' <summary>
    ''' This will be set to true if there were error/warning items copied to another validationHelper. Note: If a copy was processed but there were no items copied over it will remain false.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ItemsWereCopiedToOtherValidationhelper As Boolean
        Get
            Return _ItemsWereCopiedToOtherValidationhelper
        End Get
    End Property

    Public Property GroupName As String
    Public Property ValidationErrors As New List(Of WebValidationItem)

    Public Sub Clear()
        Me.ValidationErrors.Clear()
    End Sub

    Public Function HasWarnings() As Boolean
        Return (From v In ValidationErrors Where v.IsWarning Select v).Any()
    End Function

    Public Function HasErrros() As Boolean
        Return (From v In ValidationErrors Where v.IsWarning = False Select v).Any()
    End Function

    Public Function HasWarningsOrErrrors() As Boolean
        Return HasWarnings() OrElse HasErrros()
    End Function

    Public Function GetWarnings() As List(Of WebValidationItem)
        Dim warnings = From v In ValidationErrors Where v.IsWarning Select v
        Return If(warnings IsNot Nothing, warnings.ToList(), New List(Of WebValidationItem))
    End Function

    Public Function GetErrors() As List(Of WebValidationItem)
        Dim errs = From v In ValidationErrors Where v.IsWarning = False Select v
        Return If(errs IsNot Nothing, errs.ToList(), New List(Of WebValidationItem))
    End Function

    Public Function GetLastError() As WebValidationItem
        Try
            Return (From v In GetErrors() Select v).LastOrDefault()
        Catch ex As Exception
#If DEBUG Then
            Debugger.Break()
#End If
        End Try
        Return Nothing
    End Function

    Public Function GetLastWarning() As WebValidationItem
        Try
            Return (From v In GetWarnings() Select v).LastOrDefault()
        Catch ex As Exception
#If DEBUG Then
            Debugger.Break()
#End If
        End Try
        Return Nothing
    End Function

    Public Sub AddWarning(ByVal msg As String)
        Dim i = New WebValidationItem(msg)
        i.IsWarning = True
        Me.ValidationErrors.Add(i)
    End Sub
    Public Sub AddWarning(ByVal msg As WebValidationItem)
        msg.IsWarning = True
        Me.ValidationErrors.Add(msg)
    End Sub
    Public Sub AddWarning(msg As String, clientId As String)
        Dim new_val As New WebValidationItem(msg, clientId)
        new_val.IsWarning = True
        Me.ValidationErrors.Add(new_val)
    End Sub
    Public Sub AddWarning(ctrl As Control, errMsg As String, openAccordIdsPanelIndex As List(Of KeyValuePair(Of String, String)))
        Me.AddWarning(errMsg, ctrl.ClientID)
        With Me.GetLastError()
            If openAccordIdsPanelIndex IsNot Nothing Then
                For Each kv As KeyValuePair(Of String, String) In openAccordIdsPanelIndex
                    .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(kv.Key, kv.Value))
                Next
            End If

            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(ctrl.ClientID))
        End With
    End Sub

    Public Sub AddError(ByVal msg As String)
        Dim new_val As New WebValidationItem(msg)
        new_val.IsWarning = False
        Me.ValidationErrors.Add(new_val)
    End Sub
    Public Sub AddError(ByVal msg As String, clientId As String)
        Dim new_val As New WebValidationItem(msg, clientId)
        new_val.IsWarning = False
        Me.ValidationErrors.Add(new_val)
    End Sub
    Public Sub AddError(ctrl As Control, errMsg As String, accordList As List(Of VRAccordionTogglePair))
        Me.AddError(errMsg, ctrl.ClientID)
        With Me.GetLastError()
            If accordList IsNot Nothing Then
                For Each a As VRAccordionTogglePair In accordList
                    .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(a.AccordDivId, a.AccordIndex))
                Next
            End If

            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(ctrl.ClientID))
        End With
    End Sub

    Public Sub AddError(ctrl As Control, errMsg As String, openAccordIdsPanelIndex As List(Of KeyValuePair(Of String, String)))
        Me.AddError(errMsg, ctrl.ClientID)
        With Me.GetLastError()
            If openAccordIdsPanelIndex IsNot Nothing Then
                For Each kv As KeyValuePair(Of String, String) In openAccordIdsPanelIndex
                    .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(kv.Key, kv.Value))
                Next
            End If

            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(ctrl.ClientID))
        End With
    End Sub

    Public Sub AddError(ctrl As Control, errMsg As String, accordId As String, accordIndex As String)
        Me.AddError(errMsg, ctrl.ClientID)
        With Me.GetLastError()
            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordId, accordIndex))
            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(ctrl.ClientID))
        End With
    End Sub
    Public Sub AddError(ByVal msg As WebValidationItem)
        Me.ValidationErrors.Add(msg)
    End Sub

    Public Sub AddError(ByVal msg As String, clientId As String, changeTitle As Boolean, titleText As String)
        Dim new_val As New WebValidationItem(msg, clientId, changeTitle, titleText)
        new_val.IsWarning = False
        Me.ValidationErrors.Add(new_val)
    End Sub

    Public Sub AddError(ByVal msg As String, changeTitle As Boolean, titleText As String)
        Dim new_val As New WebValidationItem(msg, changeTitle, titleText)
        new_val.IsWarning = False
        Me.ValidationErrors.Add(new_val)
    End Sub

    Public Sub Val_BindValidationItemToControl(ctrl As Control, valItem As IFM.VR.Validation.ObjectValidation.ValidationItem, accordID As String, accordIndex As String)
        Val_BindValidationItemToControl(ctrl.ClientID, valItem, accordID, accordIndex)
    End Sub

    Public Sub Val_BindValidationItemToControl(controlClientId As String, valItem As IFM.VR.Validation.ObjectValidation.ValidationItem, accordID As String, accordIndex As String)

        Dim accordIdAndIndex As New KeyValuePair(Of String, String)(accordID, accordIndex)
        Dim lst As New List(Of KeyValuePair(Of String, String))
        lst.Add(accordIdAndIndex)
        Val_BindValidationItemToControl(controlClientId, valItem, lst)
    End Sub

    Public Sub Val_BindValidationItemToControl(ctrl As Control, valItem As IFM.VR.Validation.ObjectValidation.ValidationItem, openAccordIdsPanelIndex As List(Of KeyValuePair(Of String, String)))
        Val_BindValidationItemToControl(ctrl.ClientID, valItem, openAccordIdsPanelIndex)
    End Sub

    Public Sub Val_BindValidationItemToControl(controlClientId As String, valItem As IFM.VR.Validation.ObjectValidation.ValidationItem, openAccordIdsPanelIndex As List(Of KeyValuePair(Of String, String)))
        If valItem.IsWarning Then
            Me.AddWarning(valItem.Message, controlClientId)
            With Me.GetLastWarning()
                If openAccordIdsPanelIndex IsNot Nothing Then
                    For Each kv As KeyValuePair(Of String, String) In openAccordIdsPanelIndex
                        .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(kv.Key, kv.Value))
                    Next
                End If
                If String.IsNullOrWhiteSpace(controlClientId) = False Then
                    .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(controlClientId))
                End If

            End With
        Else
            Me.AddError(valItem.Message, controlClientId)
            With Me.GetLastError()
                If openAccordIdsPanelIndex IsNot Nothing Then
                    For Each kv As KeyValuePair(Of String, String) In openAccordIdsPanelIndex
                        .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(kv.Key, kv.Value))
                    Next
                End If
                If String.IsNullOrWhiteSpace(controlClientId) = False Then
                    .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(controlClientId))
                End If

            End With
        End If

    End Sub

    Public Sub Val_BindValidationItemToControl(cntrl As Control, valItem As IFM.VR.Validation.ObjectValidation.ValidationItem, openAccordIdsPanelIndex As List(Of VRAccordionTogglePair))
        Dim accordLevels As New List(Of KeyValuePair(Of String, String))
        For Each pair In openAccordIdsPanelIndex
            accordLevels.Add(New KeyValuePair(Of String, String)(pair.AccordDivId, pair.AccordIndex))
        Next
        Val_BindValidationItemToControl(cntrl, valItem, accordLevels)
    End Sub

    Public Sub Val_BindValidationItemToControl(controlClientId As String, valItem As IFM.VR.Validation.ObjectValidation.ValidationItem, openAccordIdsPanelIndex As List(Of VRAccordionTogglePair))
        Dim accordLevels As New List(Of KeyValuePair(Of String, String))
        For Each pair In openAccordIdsPanelIndex
            accordLevels.Add(New KeyValuePair(Of String, String)(pair.AccordDivId, pair.AccordIndex))
        Next
        Val_BindValidationItemToControl(controlClientId, valItem, accordLevels)
    End Sub

    ''' <summary>
    ''' Copies errors and warnings then clears the validationhelper that you copied from to avoid duplicate errors/warnings.
    ''' </summary>
    ''' <param name="copyFromValidationHelper"></param>
    Public Sub InsertFromOtherValidationHelper(copyFromValidationHelper As ControlValidationHelper)
        If copyFromValidationHelper IsNot Nothing Then
            For Each er In copyFromValidationHelper.ValidationErrors
                Me.ValidationErrors.Add(er)
            Next
            copyFromValidationHelper._ItemsWereCopiedToOtherValidationhelper = copyFromValidationHelper.HasWarningsOrErrrors 'only set to true if items were actually moved
            copyFromValidationHelper.Clear() ' avoids duplicate error messages
        End If
    End Sub

End Class