Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports System.Data.SqlClient

Public Class ctl_WCP_NamedIndividual
    Inherits VRControlBase

#Region "Declarations"
    Public Event ChangeNIActiveAccordion(ByVal NIType As NIType_Enum, ByVal NIIndex As Integer)

    Public Property NamedIndividualIndex As Int32
        Get
            Return ViewState.GetInt32("vs_WCPNIIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_WCPNIIndex") = value
        End Set
    End Property

    Private ReadOnly Property MyNamedIndividual As Object
        Get
            Select Case NIType
                Case NIType_Enum.InclusionOfSoleProprietersEtc
                    ' Inclusion of Sole Proprietors - stored on Indiana Quote
                    'updated 11/27/2018
                    If GoverningStateQuote() IsNot Nothing AndAlso GoverningStateQuote.InclusionOfSoleProprietorRecords IsNot Nothing AndAlso GoverningStateQuote.InclusionOfSoleProprietorRecords.HasItemAtIndex(NamedIndividualIndex) Then
                        Return GoverningStateQuote.InclusionOfSoleProprietorRecords(NamedIndividualIndex)
                    Else
                        Return Nothing
                    End If
                    Exit Select
                Case NIType_Enum.WaiverOfSubrogation
                    ' Waiver of Subro - Governing State
                    If GoverningStateQuote() IsNot Nothing AndAlso GoverningStateQuote.WaiverOfSubrogationRecords IsNot Nothing AndAlso GoverningStateQuote.WaiverOfSubrogationRecords.HasItemAtIndex(NamedIndividualIndex) Then
                        Return GoverningStateQuote.WaiverOfSubrogationRecords(NamedIndividualIndex)
                    Else
                        Return Nothing
                    End If
                    Exit Select
                Case NIType_Enum.ExclusionOfAmishWorkers
                    ' Exclusion of Amish Workers - stored on Indiana Quote
                    'updated 11/27/2018
                    Dim INquote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuoteState.Indiana)
                    If INquote IsNot Nothing AndAlso INquote.ExclusionOfAmishWorkerRecords IsNot Nothing AndAlso INquote.ExclusionOfAmishWorkerRecords.HasItemAtIndex(NamedIndividualIndex) Then
                        Return INquote.ExclusionOfAmishWorkerRecords(NamedIndividualIndex)
                    Else
                        Return Nothing
                    End If
                    Exit Select
                Case NIType_Enum.ExclusionOfSoleOfficer
                    ' Exclusion of Sole Officer - stored on Indiana and Kentucky Quotes
                    'updated 11/27/2018
                    Dim INquote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuoteState.Indiana)
                    If INquote IsNot Nothing AndAlso INquote.ExclusionOfSoleProprietorRecords IsNot Nothing AndAlso INquote.ExclusionOfSoleProprietorRecords.HasItemAtIndex(NamedIndividualIndex) Then
                        Return INquote.ExclusionOfSoleProprietorRecords(NamedIndividualIndex)
                    Else
                        Dim KYQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuoteState.Kentucky)
                        If KYQuote IsNot Nothing Then
                            Return KYQuote.ExclusionOfSoleProprietorRecords(NamedIndividualIndex)
                        End If
                        Return Nothing
                    End If
                    Exit Select
                Case NIType_Enum.ExclusionOfSoleProprietor_IL 'added 11/27/2018
                    Dim ILquote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuoteState.Illinois)
                    If ILquote IsNot Nothing AndAlso ILquote.ExclusionOfSoleProprietorRecords_IL IsNot Nothing AndAlso ILquote.ExclusionOfSoleProprietorRecords_IL.HasItemAtIndex(NamedIndividualIndex) Then
                        Return ILquote.ExclusionOfSoleProprietorRecords_IL(NamedIndividualIndex)
                    Else
                        Return Nothing
                    End If
                    Exit Select
                Case NIType_Enum.RejectionOfCoverageEndorsement  ' added 5-2-2019 MGB
                    Dim KYQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuoteState.Kentucky)
                    If KYQuote IsNot Nothing AndAlso KYQuote.KentuckyRejectionOfCoverageEndorsementRecords IsNot Nothing AndAlso KYQuote.KentuckyRejectionOfCoverageEndorsementRecords.HasItemAtIndex(NamedIndividualIndex) Then
                        Return KYQuote.KentuckyRejectionOfCoverageEndorsementRecords(NamedIndividualIndex)
                    Else
                        Return Nothing
                    End If
                Case Else
                    Return Nothing
            End Select
        End Get
    End Property

    Dim nityp As NIType_Enum = NIType_Enum.NotSet
    Public Property NIType As NIType_Enum
        Get
            If hdnNIType.Value IsNot Nothing AndAlso hdnNIType.Value <> "" Then
                Return hdnNIType.Value
            Else
                Return NIType_Enum.NotSet
            End If
        End Get
        Set(value As NIType_Enum)
            hdnNIType.Value = value
            SetControlType()
        End Set
    End Property

    Public Enum NIType_Enum
        NotSet
        InclusionOfSoleProprietersEtc
        WaiverOfSubrogation
        ExclusionOfAmishWorkers
        ExclusionOfSoleOfficer
        ExclusionOfSoleProprietor_IL 'added 11/27/2018
        RejectionOfCoverageEndorsement  ' added 5/2/2019 MGB 
    End Enum

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return Me.NamedIndividualIndex
        End Get
    End Property

#End Region

#Region "Methods and Functions"

    Private Sub HandleError(ByVal RoutineName As String, ByVal ex As Exception)
        Dim str As String = RoutineName & ":  " & ex.Message
        If AppSettings("TestOrProd").ToUpper <> "PROD" Then lblMsg.Text = str Else Throw New Exception(ex.Message, ex)
    End Sub

    Private Sub SetControlType()
        trTypeRow.Attributes.Add("style", "display:none")
        trInfoRow.Attributes.Add("style", "display:none")

        Select Case NIType
            Case NIType_Enum.InclusionOfSoleProprietersEtc
                trTypeRow.Attributes.Add("style", "display:''")
                trInfoRow.Attributes.Add("style", "display:''")
                Exit Select
            Case NIType_Enum.WaiverOfSubrogation
                Exit Select
            Case NIType_Enum.ExclusionOfAmishWorkers
                Exit Select
            Case NIType_Enum.ExclusionOfSoleOfficer
                Exit Select
            Case NIType_Enum.ExclusionOfSoleProprietor_IL 'added 11/28/2018 for consistency
                Exit Select
            Case NIType_Enum.RejectionOfCoverageEndorsement 'added 5/2/2019 for consistency MGB
                Exit Select
            Case Else
                Exit Select
        End Select

        UpdateAccordHeader()

        Exit Sub
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkNew.ClientID)
        Me.VRScript.CreateConfirmDialog(Me.lnkDelete.ClientID, "Delete?")
    End Sub

    Public Overrides Sub LoadStaticData()
        If ddlType.Items.Count <= 0 Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddlType, QuickQuoteClassName.QuickQuoteInclusionExclusionScheduledItem, QuickQuotePropertyName.PositionTitleTypeId, SortBy.TextDescending, Me.Quote.LobType)
        End If
    End Sub

    Private Sub UpdateAccordHeader()
        Select Case NIType
            Case NIType_Enum.InclusionOfSoleProprietersEtc
                lblAccordHeader.Text = "Inclusion of Sole Proprietors... - #" & NamedIndividualIndex + 1.ToString
                Exit Select
            Case NIType_Enum.WaiverOfSubrogation
                lblAccordHeader.Text = "Waiver of Subrogation - #" & NamedIndividualIndex + 1.ToString
                Exit Select
            Case NIType_Enum.ExclusionOfAmishWorkers
                lblAccordHeader.Text = "Exclusion of Amish Workers - #" & NamedIndividualIndex + 1.ToString
                Exit Select
            Case NIType_Enum.ExclusionOfSoleOfficer
                lblAccordHeader.Text = "Exclusion of Sole Officer - #" & NamedIndividualIndex + 1.ToString
                Exit Select
            Case NIType_Enum.ExclusionOfSoleProprietor_IL 'added 11/28/2018
                lblAccordHeader.Text = "Exclusion of Sole Proprietors... IL - #" & NamedIndividualIndex + 1.ToString
                Exit Select
            Case NIType_Enum.RejectionOfCoverageEndorsement 'added 5/2/2019 MGB
                lblAccordHeader.Text = "Rejection of Coverage Endorsement - #" & NamedIndividualIndex + 1.ToString
                Exit Select
            Case Else
                lblAccordHeader.Text = "*Error*"
                Exit Select
        End Select
    End Sub

    Public Overrides Sub Populate()
        Dim err As String = Nothing

        Try
            LoadStaticData()
            ddlType.SelectedValue = "7"  ' Default is Sole Proprietor

            ClearInputFields()

            If MyNamedIndividual IsNot Nothing Then
                Select Case NIType
                    Case NIType_Enum.InclusionOfSoleProprietersEtc
                        Dim sp As QuickQuote.CommonObjects.QuickQuoteInclusionOfSoleProprietorRecord = MyNamedIndividual
                        If sp.Name.DisplayName.ToUpper.Contains("INCLUSION NAME") Then
                            txtName.Text = ""
                        Else
                            txtName.Text = sp.Name.DisplayName
                        End If
                        ddlType.SelectedValue = sp.PositionTitleTypeId
                        Exit Select
                    Case NIType_Enum.WaiverOfSubrogation
                        Dim ws As QuickQuote.CommonObjects.QuickQuoteWaiverOfSubrogationRecord = MyNamedIndividual
                        txtName.Text = ws.Name.DisplayName
                        Exit Select
                    Case NIType_Enum.ExclusionOfAmishWorkers
                        Dim aw As QuickQuote.CommonObjects.QuickQuoteExclusionOfAmishWorkerRecord = MyNamedIndividual
                        If aw.Name.DisplayName.ToUpper.Contains("EXCLUSION NAME") Then
                            txtName.Text = ""
                        Else
                            txtName.Text = aw.Name.DisplayName
                        End If
                        Exit Select
                    Case NIType_Enum.ExclusionOfSoleOfficer
                        Dim so As QuickQuote.CommonObjects.QuickQuoteExclusionOfSoleProprietorRecord = MyNamedIndividual
                        If so.Name.DisplayName.ToUpper.Contains("EXCLUSION NAME") Then
                            txtName.Text = ""
                        Else
                            txtName.Text = so.Name.DisplayName
                        End If
                        Exit Select
                    Case NIType_Enum.ExclusionOfSoleProprietor_IL 'added 11/28/2018
                        Dim so As QuickQuote.CommonObjects.QuickQuoteExclusionOfSoleProprietorRecord_IL = MyNamedIndividual
                        If so.Name.DisplayName.ToUpper.Contains("EXCLUSION NAME") Then
                            txtName.Text = ""
                        Else
                            txtName.Text = so.Name.DisplayName
                        End If
                        Exit Select
                    Case NIType_Enum.RejectionOfCoverageEndorsement  ' added 5-2-2019 MGB
                        Dim so As QuickQuote.CommonObjects.QuickQuoteKentuckyRejectionOfCoverageEndorsement = MyNamedIndividual
                        If so.Name.DisplayName.ToUpper.Contains("REJECTION NAME") Then
                            txtName.Text = ""
                        Else
                            txtName.Text = so.Name.DisplayName
                        End If
                        Exit Select
                End Select
            End If

            Me.PopulateChildControls()

            Exit Sub
        Catch ex As Exception
            HandleError("Populate", ex)
        End Try
    End Sub

    Public Overrides Function Save() As Boolean
        Dim cls As QuickQuote.CommonObjects.QuickQuoteClassification = Nothing
        Dim dia_id As String = Nothing
        Dim err As String = Nothing
        Dim pn As String = Nothing
        Dim pa As String = Nothing
        Dim fn As String = Nothing
        Dim mn As String = Nothing
        Dim ln As String = Nothing

        Try
            lblMsg.Text = "&nbsp;"


            ' This section saves the named individual information to the first quote that is eligible
            If MyNamedIndividual IsNot Nothing Then
                Select Case Me.NIType
                    Case NIType_Enum.InclusionOfSoleProprietersEtc
                        Dim NI As QuickQuote.CommonObjects.QuickQuoteInclusionOfSoleProprietorRecord = MyNamedIndividual
                        NI.Name.CommercialName1 = txtName.Text
                        NI.PositionTitleTypeId = ddlType.SelectedValue
                        NI.Name.TypeId = "2"
                        Exit Select
                    Case NIType_Enum.WaiverOfSubrogation
                        Dim NI As QuickQuote.CommonObjects.QuickQuoteWaiverOfSubrogationRecord = MyNamedIndividual
                        NI.Name.CommercialName1 = txtName.Text
                        NI.Name.TypeId = "2"
                        Exit Select
                    Case NIType_Enum.ExclusionOfAmishWorkers
                        Dim NI As QuickQuote.CommonObjects.QuickQuoteExclusionOfAmishWorkerRecord = MyNamedIndividual
                        NI.Name.CommercialName1 = txtName.Text
                        NI.Name.TypeId = "2"
                        Exit Select
                    Case NIType_Enum.ExclusionOfSoleOfficer
                        Dim NI As QuickQuote.CommonObjects.QuickQuoteExclusionOfSoleProprietorRecord = MyNamedIndividual
                        NI.Name.CommercialName1 = txtName.Text
                        NI.Name.TypeId = "2"
                        Exit Select
                    Case NIType_Enum.ExclusionOfSoleProprietor_IL 'added 11/28/2018
                        Dim NI As QuickQuote.CommonObjects.QuickQuoteExclusionOfSoleProprietorRecord_IL = MyNamedIndividual
                        NI.Name.CommercialName1 = txtName.Text
                        NI.Name.TypeId = "2"
                        Exit Select
                    Case NIType_Enum.RejectionOfCoverageEndorsement 'added 5/2/2019 MGB
                        Dim NI As QuickQuote.CommonObjects.QuickQuoteKentuckyRejectionOfCoverageEndorsement = MyNamedIndividual
                        NI.Name.CommercialName1 = txtName.Text
                        NI.Name.TypeId = "2"
                        Exit Select
                    Case Else
                        Throw New Exception("Unknown NIType")
                End Select
            End If

            Me.SaveChildControls()

            Return True
        Catch ex As Exception
            HandleError("Save", ex)
            Return False
        End Try
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Try
            MyBase.ValidateControl(valArgs)
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

            Select Case NIType
                Case NIType_Enum.InclusionOfSoleProprietersEtc
                    Me.ValidationHelper.GroupName = "Inclusion of Sole Proprietors... - #" & NamedIndividualIndex + 1.ToString
                    If ddlType.SelectedIndex < 0 Then
                        Me.ValidationHelper.AddError(ddlType, "Missing Type", accordList)
                    End If
                    Exit Select
                Case NIType_Enum.WaiverOfSubrogation
                    Me.ValidationHelper.GroupName = "Waiver of Subrogation - #" & NamedIndividualIndex + 1.ToString
                    Exit Select
                Case NIType_Enum.ExclusionOfAmishWorkers
                    Me.ValidationHelper.GroupName = "Exclusion of Amish Workers - #" & NamedIndividualIndex + 1.ToString
                    Exit Select
                Case NIType_Enum.ExclusionOfSoleOfficer
                    Me.ValidationHelper.GroupName = "Exclusion of Sole Officer - #" & NamedIndividualIndex + 1.ToString
                    Exit Select
                Case NIType_Enum.ExclusionOfSoleProprietor_IL 'added 11/28/2018
                    Me.ValidationHelper.GroupName = "Exclusion of Sole Proprietors... IL - #" & NamedIndividualIndex + 1.ToString
                    Exit Select
                Case NIType_Enum.RejectionOfCoverageEndorsement 'added 5/2/2019
                    Me.ValidationHelper.GroupName = "Rejection Of Coverage Endorsement - #" & NamedIndividualIndex + 1.ToString
                    Exit Select
            End Select

            If txtName.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtName, "Missing Name", accordList)
            End If

            Me.ValidateChildControls(valArgs)

            Exit Sub
        Catch ex As Exception
            HandleError("ValidateControls", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub ParseName(ByVal NameToParse As String, ByRef FName As String, ByRef LName As String, ByRef MName As String)
        Try
            FName = ""
            LName = ""
            MName = ""

            Dim nmparts As String() = NameToParse.Split(" ")
            Select Case nmparts.Count
                Case 1
                    ' 1 part - First Name
                    FName = NameToParse
                    Exit Select
                Case 2
                    ' 2 parts - First/Last name
                    FName = nmparts(0)
                    LName = nmparts(1)
                    Exit Select
                Case 3
                    ' 3 parts - First/Middle/Last name
                    FName = nmparts(0)
                    MName = nmparts(1)
                    LName = nmparts(2)
                    Exit Select
                Case Else
                    ' More than 3 parts - ???
                    FName = NameToParse
                    Exit Select
            End Select

            Exit Sub
        Catch ex As Exception
            HandleError("ParseName", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub ClearInputFields()
        Try
            txtName.Text = ""
            If ddlType.Items.Count > 0 Then
                ddlType.SelectedIndex = 0
            Else
                ddlType.SelectedIndex = -1
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("ClearInputFields", ex)
            Exit Sub
        End Try
    End Sub

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If hdnNIType.Value IsNot Nothing AndAlso hdnNIType.Value <> "" Then NIType = hdnNIType.Value

            If Not IsPostBack Then
                'Me.MainAccordionDivId = Me.divWCPWorkplace.ClientID
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("Page LOAD", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Try
            Me.Save_FireSaveEvent()
            Exit Sub
        Catch ex As Exception
            HandleError("Save", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub lnkNew_Click(sender As Object, e As EventArgs) Handles lnkNew.Click
        Try
            If Quote IsNot Nothing Then
                Select Case Me.NIType
                    Case NIType_Enum.InclusionOfSoleProprietersEtc
                        'updated 11/28/2018
                        Dim govStateQuote As QuickQuote.CommonObjects.QuickQuoteObject = Me.GoverningStateQuote
                        If govStateQuote IsNot Nothing Then
                            If govStateQuote.InclusionOfSoleProprietorRecords Is Nothing Then govStateQuote.InclusionOfSoleProprietorRecords = New List(Of QuickQuote.CommonObjects.QuickQuoteInclusionOfSoleProprietorRecord)
                            govStateQuote.InclusionOfSoleProprietorRecords.AddNew()
                            Save_FireSaveEvent(False)
                            Populate_FirePopulateEvent()
                            Save_FireSaveEvent(False)
                            RaiseEvent ChangeNIActiveAccordion(NIType_Enum.InclusionOfSoleProprietersEtc, NamedIndividualIndex)
                        End If
                        Exit Select
                    Case NIType_Enum.WaiverOfSubrogation
                        Dim govStateQuote As QuickQuote.CommonObjects.QuickQuoteObject = Me.GoverningStateQuote
                        If govStateQuote IsNot Nothing Then
                            If govStateQuote.WaiverOfSubrogationRecords Is Nothing Then govStateQuote.WaiverOfSubrogationRecords = New List(Of QuickQuote.CommonObjects.QuickQuoteWaiverOfSubrogationRecord)
                            govStateQuote.WaiverOfSubrogationRecords.AddNew()
                            Save_FireSaveEvent(False)
                            Populate_FirePopulateEvent()
                            Save_FireSaveEvent(False)
                            RaiseEvent ChangeNIActiveAccordion(NIType_Enum.WaiverOfSubrogation, NamedIndividualIndex)
                        End If
                        Exit Select
                    Case NIType_Enum.ExclusionOfAmishWorkers
                        'updated 11/28/2018
                        Dim INquote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuoteState.Indiana)
                        If INquote IsNot Nothing Then
                            If INquote.ExclusionOfAmishWorkerRecords Is Nothing Then INquote.ExclusionOfAmishWorkerRecords = New List(Of QuickQuote.CommonObjects.QuickQuoteExclusionOfAmishWorkerRecord)
                            INquote.ExclusionOfAmishWorkerRecords.AddNew()
                            Save_FireSaveEvent(False)
                            Populate_FirePopulateEvent()
                            Save_FireSaveEvent(False)
                            RaiseEvent ChangeNIActiveAccordion(NIType_Enum.ExclusionOfAmishWorkers, NamedIndividualIndex)
                        End If
                        Exit Select
                    Case NIType_Enum.ExclusionOfSoleOfficer
                        'updated 11/28/2018
                        Dim INquote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuoteState.Indiana)
                        If INquote IsNot Nothing Then
                            If INquote.ExclusionOfSoleProprietorRecords Is Nothing Then INquote.ExclusionOfSoleProprietorRecords = New List(Of QuickQuote.CommonObjects.QuickQuoteExclusionOfSoleProprietorRecord)
                            INquote.ExclusionOfSoleProprietorRecords.AddNew()
                            Save_FireSaveEvent(False)
                            Populate_FirePopulateEvent()
                            Save_FireSaveEvent(False)
                            RaiseEvent ChangeNIActiveAccordion(NIType_Enum.ExclusionOfSoleOfficer, NamedIndividualIndex)
                        End If
                        Exit Select
                    Case NIType_Enum.ExclusionOfSoleProprietor_IL 'added 11/28/2018
                        Dim ILquote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuoteState.Illinois)
                        If ILquote IsNot Nothing Then
                            If ILquote.ExclusionOfSoleProprietorRecords_IL Is Nothing Then ILquote.ExclusionOfSoleProprietorRecords_IL = New List(Of QuickQuote.CommonObjects.QuickQuoteExclusionOfSoleProprietorRecord_IL)
                            ILquote.ExclusionOfSoleProprietorRecords_IL.AddNew()
                            Save_FireSaveEvent(False)
                            Populate_FirePopulateEvent()
                            Save_FireSaveEvent(False)
                            RaiseEvent ChangeNIActiveAccordion(NIType_Enum.ExclusionOfSoleProprietor_IL, NamedIndividualIndex)
                        End If
                        Exit Select
                    Case NIType_Enum.RejectionOfCoverageEndorsement ' Added 5/31/19 MGB for KY WC
                        Dim KYquote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuoteState.Kentucky)
                        If KYquote IsNot Nothing Then
                            If KYquote.KentuckyRejectionOfCoverageEndorsementRecords Is Nothing Then KYquote.KentuckyRejectionOfCoverageEndorsementRecords = New List(Of QuickQuote.CommonObjects.QuickQuoteKentuckyRejectionOfCoverageEndorsement)
                            KYquote.KentuckyRejectionOfCoverageEndorsementRecords.AddNew()
                            Save_FireSaveEvent(False)
                            Populate_FirePopulateEvent()
                            Save_FireSaveEvent(False)
                            RaiseEvent ChangeNIActiveAccordion(NIType_Enum.RejectionOfCoverageEndorsement, NamedIndividualIndex)
                        End If
                        Exit Select
                    Case Else
                        Throw New Exception("Unknown NIType")
                End Select
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("lnkNew_Click", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        Try
            If MyNamedIndividual IsNot Nothing Then
                Select Case Me.NIType
                    Case NIType_Enum.InclusionOfSoleProprietersEtc
                        'updated 11/28/2018
                        Dim govStateQuote As QuickQuote.CommonObjects.QuickQuoteObject = Me.GoverningStateQuote
                        If govStateQuote IsNot Nothing Then
                            If govStateQuote.InclusionOfSoleProprietorRecords IsNot Nothing AndAlso govStateQuote.InclusionOfSoleProprietorRecords.HasItemAtIndex(NamedIndividualIndex) Then
                                govStateQuote.InclusionOfSoleProprietorRecords.RemoveAt(NamedIndividualIndex)
                                Populate()
                                Save_FireSaveEvent(False)
                                Populate_FirePopulateEvent()
                                RaiseEvent ChangeNIActiveAccordion(NIType_Enum.InclusionOfSoleProprietersEtc, govStateQuote.InclusionOfSoleProprietorRecords.Count - 1)
                            End If
                        End If
                        Exit Select
                    Case NIType_Enum.WaiverOfSubrogation
                        'updated 11/28/2018
                        Dim govStateQuote As QuickQuote.CommonObjects.QuickQuoteObject = Me.GoverningStateQuote
                        If govStateQuote IsNot Nothing Then
                            If govStateQuote.WaiverOfSubrogationRecords IsNot Nothing AndAlso govStateQuote.WaiverOfSubrogationRecords.HasItemAtIndex(NamedIndividualIndex) Then
                                govStateQuote.WaiverOfSubrogationRecords.RemoveAt(NamedIndividualIndex)
                                Populate()
                                Save_FireSaveEvent(False)
                                Populate_FirePopulateEvent()
                                RaiseEvent ChangeNIActiveAccordion(NIType_Enum.WaiverOfSubrogation, govStateQuote.WaiverOfSubrogationRecords.Count - 1)
                            End If
                        End If
                        Exit Select
                    Case NIType_Enum.ExclusionOfAmishWorkers
                        'updated 11/28/2018
                        Dim INquote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuoteState.Indiana)
                        If INquote IsNot Nothing Then
                            If INquote.ExclusionOfAmishWorkerRecords IsNot Nothing AndAlso INquote.ExclusionOfAmishWorkerRecords.HasItemAtIndex(NamedIndividualIndex) Then
                                INquote.ExclusionOfAmishWorkerRecords.RemoveAt(NamedIndividualIndex)
                                Populate()
                                Save_FireSaveEvent(False)
                                Populate_FirePopulateEvent()
                                RaiseEvent ChangeNIActiveAccordion(NIType_Enum.ExclusionOfAmishWorkers, INquote.ExclusionOfAmishWorkerRecords.Count - 1)
                            End If
                        End If
                        Exit Select
                    Case NIType_Enum.ExclusionOfSoleOfficer
                        'updated 11/28/2018
                        Dim INquote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuoteState.Indiana)
                        If INquote IsNot Nothing Then
                            If INquote.ExclusionOfSoleProprietorRecords IsNot Nothing AndAlso INquote.ExclusionOfSoleProprietorRecords.HasItemAtIndex(NamedIndividualIndex) Then
                                INquote.ExclusionOfSoleProprietorRecords.RemoveAt(NamedIndividualIndex)
                                Populate()
                                Save_FireSaveEvent(False)
                                Populate_FirePopulateEvent()
                                RaiseEvent ChangeNIActiveAccordion(NIType_Enum.ExclusionOfSoleOfficer, INquote.ExclusionOfSoleProprietorRecords.Count - 1)
                            End If
                        End If
                        Exit Select
                    Case NIType_Enum.ExclusionOfSoleProprietor_IL 'added 11/28/2018
                        Dim ILquote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuoteState.Illinois)
                        If ILquote IsNot Nothing Then
                            If ILquote.ExclusionOfSoleProprietorRecords_IL IsNot Nothing AndAlso ILquote.ExclusionOfSoleProprietorRecords_IL.HasItemAtIndex(NamedIndividualIndex) Then
                                ILquote.ExclusionOfSoleProprietorRecords_IL.RemoveAt(NamedIndividualIndex)
                                Populate()
                                Save_FireSaveEvent(False)
                                Populate_FirePopulateEvent()
                                RaiseEvent ChangeNIActiveAccordion(NIType_Enum.ExclusionOfSoleProprietor_IL, ILquote.ExclusionOfSoleProprietorRecords_IL.Count - 1)
                            End If
                        End If
                        Exit Select
                    Case NIType_Enum.RejectionOfCoverageEndorsement 'added 5/31/2019 for KY WC
                        Dim KYquote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuoteState.Kentucky)
                        If KYquote IsNot Nothing Then
                            If KYquote.KentuckyRejectionOfCoverageEndorsementRecords IsNot Nothing AndAlso KYquote.KentuckyRejectionOfCoverageEndorsementRecords.HasItemAtIndex(NamedIndividualIndex) Then
                                KYquote.KentuckyRejectionOfCoverageEndorsementRecords.RemoveAt(NamedIndividualIndex)
                                Populate()
                                Save_FireSaveEvent(False)
                                Populate_FirePopulateEvent()
                                RaiseEvent ChangeNIActiveAccordion(NIType_Enum.RejectionOfCoverageEndorsement, KYquote.KentuckyRejectionOfCoverageEndorsementRecords.Count - 1)
                            End If
                        End If
                        Exit Select
                    Case Else
                        Throw New Exception("Unknown NIType")
                End Select
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("lnkDelete_Click", ex)
            Exit Sub
        End Try
    End Sub

#End Region

End Class