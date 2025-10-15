Imports IFM.PrimativeExtensions

Public Class ctl_Farm_Remarks
    Inherits VRControlBase

    Const vrTitle As String = "VR-REMARK"

    Private Property Noteid As Int32
        Get
            Return ViewState.GetInt32("vs_noteid", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_noteid") = value
        End Set
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Nothing, "0")
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        'Me.txtRemarks.Text = Me.Quote.
        Dim xml As New QuickQuote.CommonMethods.QuickQuoteXML
        Dim notes As List(Of Diamond.Common.Objects.Notes.NoteKeyStruct) = Nothing
        Dim err As String = Nothing
        xml.LoadPolicyNotes(Me.Quote, notes, err)
        If notes IsNot Nothing Then
            For Each n As Diamond.Common.Objects.Notes.NoteKeyStruct In notes
                With n
                    If .Note IsNot Nothing Then
                        With .Note
                            If .Title = vrTitle Then
                                Me.Noteid = .NoteId
                                Me.txtRemarks.Text = .Note
                            End If
                            'Dim noteid As Int32 = .NoteId
                            'Dim remark As String = .Note
                            'Dim title As String = .Title
                            'Dim userId As Int32 = .UserId

                        End With
                    End If
                End With
            Next
        End If

        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.divMain.ClientID
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()

        Dim xml As New QuickQuote.CommonMethods.QuickQuoteXML
        Dim err As String = Nothing
        Dim userId As Int32 = Me.QQHelper.IntegerForString(QuickQuote.CommonMethods.QuickQuoteHelperClass.DiamondUserId)
        'Dim noteid As Int32 = -1

        If String.IsNullOrWhiteSpace(Me.txtRemarks.Text) Then
            'delete
            If Me.Noteid >= 0 Then
                'delete
                xml.DeleteNote(Me.Noteid, err, True)
                Me.Noteid = -1
            End If
        Else
            'create or update
            If Me.Noteid >= 0 Then
                'updating
                xml.SaveNote(Me.Quote, Me.Noteid, userId, Me.txtRemarks.Text.Trim(), vrTitle, err)
            Else
                'creating new
                xml.AddPolicyNote(Me.Quote, userId, Me.txtRemarks.Text.Trim(), vrTitle, err)
                Me.Populate() ' finds the new note and sets the Me.Noteid
            End If
        End If

        If String.IsNullOrWhiteSpace(err) = False Then
            Me.ValidationHelper.AddWarning(err)
        End If

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent()
    End Sub
End Class