Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Public Class ctlPrefill_PPA
    Inherits VRControlBase

    'This control is only used for PPA, so no multi state changes are needed 9/17/18 MLW

    Public Event JustFinishedPrefill() 'added 9/19/2019

    Public Property PrefillFetched As Boolean
        Get
            If ViewState("vs_prefillFetched") Is Nothing Then
                ViewState("vs_prefillFetched") = False
            End If
            Return CBool(ViewState("vs_prefillFetched"))
        End Get
        Set(value As Boolean)
            ViewState("vs_prefillFetched") = value
        End Set
    End Property

    Public Sub FetchPreFillData()
        ' just attaches the validation helper to validation summary in case there are any errors

        Try
            If PrefillFetched = False Then
                If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                    Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum)
                ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum)
                Else
                    VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId) ' will force a refresh of the session object on next request
                End If

                'added 5/7/2019
                IFM.VR.Common.QuoteSave.QuoteSaveHelpers.CheckQuoteForClientOverwrite(Me.Quote, quoteId:=QQHelper.IntegerForString(Me.QuoteId))

                Dim prefillAddedDriversOrVehicles = False
                Dim errMessage As String = ""
                IFM.VR.Common.Helpers.PPA.PrefillHelper.LoadPrefill(Me.Quote, prefillAddedDriversOrVehicles, errMessage)
                RaiseEvent JustFinishedPrefill() 'added 9/19/2019

                ' if no errors returned
                If errMessage.IsNullEmptyorWhitespace() Then
                    If prefillAddedDriversOrVehicles Then
                        If (Quote.Drivers IsNot Nothing AndAlso Quote.Drivers.Any()) Or (Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Any()) Then
                            ' disables the tree so the user must click save or continue to vehicle buttons - this allow validation so show before moving on 6-18-14
                            Me.VRScript.AddScriptLine("ifm.vr.ui.LockTree_Freeze(); ")
                        End If
                    End If
                    'for now just raise this event all the time so the quote gets resaved to avoid quote rating failed status
                    'If drivers.Count > 0 Or vehicles.Count > 0 Then
                    Fire_GenericBoardcastEvent(BroadCastEventType.PrefillAddedDriversOrVehicles)
                Else
                    Me.ValidationHelper.AddError(errMessage)
                End If
            End If
        Catch ex As Exception

#If DEBUG Then
            Debugger.Break()
#End If

        End Try

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()

    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
    End Sub

    Private Sub ctl_Master_Edit_BroadcastGenericEvent(type As BroadCastEventType) Handles Me.BroadcastGenericEvent
        Select Case type
            Case BroadCastEventType.PreFillRequested
                FetchPreFillData()
        End Select
    End Sub
End Class