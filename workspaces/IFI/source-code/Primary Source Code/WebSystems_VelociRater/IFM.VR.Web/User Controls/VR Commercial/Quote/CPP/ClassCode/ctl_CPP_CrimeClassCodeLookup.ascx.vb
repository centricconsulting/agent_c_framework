Imports IFM.PrimativeExtensions
Imports PopupMessageClass
Imports QuickQuote.CommonObjects

Public Class ctl_CPP_CrimeClassCodeLookup
    Inherits VRControlBase


    Public Property ChosenClassCode As QuickQuoteClassificationCode
        Get
            If ViewState("vs_chosenClassCode") IsNot Nothing Then
                Return CType(ViewState("vs_chosenClassCode"), QuickQuoteClassificationCode)
            End If
            Return New QuickQuoteClassificationCode()
        End Get
        Set(value As QuickQuoteClassificationCode)
            ViewState("vs_chosenClassCode") = value
        End Set
    End Property

    Public Property txtClassCodeId As String
        Get
            Return ViewState("vs_txtCCId")
        End Get
        Set(value As String)
            ViewState("vs_txtCCId") = value
        End Set
    End Property

    Public Property txtID As String
        Get
            Return ViewState("vs_lblId")
        End Get
        Set(value As String)
            ViewState("vs_lblId") = value
        End Set
    End Property

    Public Property ParentClassCodeTextboxId As String
        Get
            Return ViewState("vs_ParentCCTxtId")
        End Get
        Set(value As String)
            ViewState("vs_ParentCCTxtId") = value
        End Set
    End Property

    Public Property ParentDescriptionTextboxId As String
        Get
            Return ViewState("vs_ParentDescTxtId")
        End Get
        Set(value As String)
            ViewState("vs_ParentDescTxtId") = value
        End Set
    End Property

    Public Property ParentIDHdnId As String
        Get
            Return ViewState("vs_ParentIDHdnId")
        End Get
        Set(value As String)
            ViewState("vs_ParentIDHdnId") = value
        End Set
    End Property

    Public Property ParentPMAIDHdnId As String
        Get
            Return ViewState("vs_ParentPMAIDHdnId")
        End Get
        Set(value As String)
            ViewState("vs_ParentPMAIDHdnId") = value
        End Set
    End Property

    Public Property ParentGroupRateHdnId As String
        Get
            Return ViewState("vs_ParentGroupRateHdnId")
        End Get
        Set(value As String)
            ViewState("vs_ParentGroupRateHdnId") = value
        End Set
    End Property

    Public Property ParentClassLimitHdnId As String
        Get
            Return ViewState("vs_ParentClassLimitHdnId")
        End Get
        Set(value As String)
            ViewState("vs_ParentClassLimitHdnId") = value
        End Set
    End Property
    Public Property ParentYardRateHdnId As String
        Get
            Return ViewState("vs_ParentYardRateHdnId")
        End Get
        Set(value As String)
            ViewState("vs_ParentYardRateHdnId") = value
        End Set
    End Property

    Public Property ParentBCUseCodeRowId As String
        Get
            Return ViewState("vs_ParentBCUseCodeRowId")
        End Get
        Set(value As String)
            ViewState("vs_ParentBCUseCodeRowId") = value
        End Set
    End Property

    Public Property ParentBCUseCodeInfoRowId As String
        Get
            Return ViewState("vs_ParentBCUseCodeInfoRowId")
        End Get
        Set(value As String)
            ViewState("vs_ParentBCUseCodeInfoRowId") = value
        End Set
    End Property

    Public Property ParentBCUseCodeGroupIRowId As String
        Get
            Return ViewState("vs_ParentBCUseCodeGroupIRowId")
        End Get
        Set(value As String)
            ViewState("vs_ParentBCUseCodeGroupIRowId") = value
        End Set
    End Property

    Public Property ParentBCUseCodeGroupIIRowId As String
        Get
            Return ViewState("vs_ParentBCUseCodeGroupIIRowId")
        End Get
        Set(value As String)
            ViewState("vs_ParentBCUseCodeGroupIIRowId") = value
        End Set
    End Property

    Public Property ParentBICUseCodeRowId As String
        Get
            Return ViewState("vs_ParentBICUseCodeRowId")
        End Get
        Set(value As String)
            ViewState("vs_ParentBICUseCodeRowId") = value
        End Set
    End Property

    Public Property ParentBICUseCodeInfoRowId As String
        Get
            Return ViewState("vs_ParentBICUseCodeInfoRowId")
        End Get
        Set(value As String)
            ViewState("vs_ParentBICUseCodeInfoRowId") = value
        End Set
    End Property

    Public Property ParentBICUseCodeGroupIRowId As String
        Get
            Return ViewState("vs_ParentBICUseCodeGroupIRowId")
        End Get
        Set(value As String)
            ViewState("vs_ParentBICUseCodeGroupIRowId") = value
        End Set
    End Property

    Public Property ParentBICUseCodeGroupIIRowId As String
        Get
            Return ViewState("vs_ParentBICUseCodeGroupIIRowId")
        End Get
        Set(value As String)
            ViewState("vs_ParentBICUseCodeGroupIIRowId") = value
        End Set
    End Property

    Public Property ParentPPCUseCodeRowId As String
        Get
            Return ViewState("vs_ParentPPCUseCodeRowId")
        End Get
        Set(value As String)
            ViewState("vs_ParentPPCUseCodeRowId") = value
        End Set
    End Property

    Public Property ParentPPCUseCodeInfoRowId As String
        Get
            Return ViewState("vs_ParentPPCUseCodeInfoRowId")
        End Get
        Set(value As String)
            ViewState("vs_ParentPPCUseCodeInfoRowId") = value
        End Set
    End Property

    Public Property ParentPPCUseCodeGroupIRowId As String
        Get
            Return ViewState("vs_ParentPPCUseCodeGroupIRowId")
        End Get
        Set(value As String)
            ViewState("vs_ParentPPCUseCodeGroupIRowId") = value
        End Set
    End Property

    Public Property ParentPPCUseCodeGroupIIRowId As String
        Get
            Return ViewState("vs_ParentPPCUseCodeGroupIIRowId")
        End Get
        Set(value As String)
            ViewState("vs_ParentPPCUseCodeGroupIIRowId") = value
        End Set
    End Property

    Public Property ParentPPOUseCodeRowId As String
        Get
            Return ViewState("vs_ParentPPOUseCodeRowId")
        End Get
        Set(value As String)
            ViewState("vs_ParentPPOUseCodeRowId") = value
        End Set
    End Property

    Public Property ParentPPOUseCodeInfoRowId As String
        Get
            Return ViewState("vs_ParentPPOUseCodeInfoRowId")
        End Get
        Set(value As String)
            ViewState("vs_ParentPPOUseCodeInfoRowId") = value
        End Set
    End Property

    Public Property ParentPPOUseCodeGroupIRowId As String
        Get
            Return ViewState("vs_ParentPPOUseCodeGroupIRowId")
        End Get
        Set(value As String)
            ViewState("vs_ParentPPOUseCodeGroupIRowId") = value
        End Set
    End Property

    Public Property ParentPPOUseCodeGroupIIRowId As String
        Get
            Return ViewState("vs_ParentPPOUseCodeGroupIIRowId")
        End Get
        Set(value As String)
            ViewState("vs_ParentPPOUseCodeGroupIIRowId") = value
        End Set
    End Property

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote.IsNotNull Then
                Return Me.Quote.Locations.GetItemAtIndex(LocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public Property BuildingIndex As Int32
        Get
            Return ViewState.GetInt32("vs_BuildingIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_BuildingIndex") = value
        End Set
    End Property

    Private ReadOnly Property MyBuilding As QuickQuote.CommonObjects.QuickQuoteBuilding
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) AndAlso Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.HasItemAtIndex(Me.BuildingIndex) Then
                Return Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.GetItemAtIndex(Me.BuildingIndex)
            End If
            Return Nothing
        End Get

    End Property

    Public Event SelectedClassCodeChanged(ByVal ClassCode As String, ByVal Desc As String, ByVal DiaClass_Id As String)

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Dim pgmtype As String = ""
        If Not IsNumeric(Me.Quote.ProgramTypeId.IsNumeric) Then pgmtype = "60" Else pgmtype = Me.Quote.ProgramTypeId

        'Create CrimeClassCodeUiBinding
        'Me.VRScript.AddVariableLine("Cpr.UiBindings.push(new Cpr.BuildingClassCodeUiBinding(" & Me.BuildingIndex.ToString & "," & CInt(Me.Quote.LobType).ToString() & "," & pgmtype & ",'" & Me.ParentIDHdnId & "','" & Me.hdnDIAClass_Id.ClientID & "','" & Me.ParentClassCodeTextboxId & "','" & Me.txtClassCode.ClientID & "','" & Me.trYardRateRow.ClientID & "','" & Me.ddYardRate.ClientID & "','" & Me.ParentYardRateHdnId & "','" & Me.ParentDescriptionTextboxId & "','" & Me.txtDescription.ClientID & "','" & Me.btnApply.ClientID & "','" & Me.ParentPMAIDHdnId & "','" & Me.ddPMA.ClientID & "','" & Me.ParentGroupRateHdnId & "','" & Me.txtGroupRate.ClientID & "','" & Me.ParentClassLimitHdnId & "','" & Me.txtClassLimit.ClientID & "','" & divFootnote.ClientID & "','" & ParentBCUseCodeRowId & "','" & ParentBCUseCodeInfoRowId & "','" & ParentBCUseCodeGroupIRowId & "','" & ParentBCUseCodeGroupIIRowId & "','" & ParentBICUseCodeRowId & "','" & ParentBICUseCodeInfoRowId & "','" & ParentBICUseCodeGroupIRowId & "','" & ParentBICUseCodeGroupIIRowId & "','" & ParentPPCUseCodeRowId & "','" & ParentPPCUseCodeInfoRowId & "','" & ParentPPCUseCodeGroupIRowId & "','" & ParentPPCUseCodeGroupIIRowId & "','" & ParentPPOUseCodeRowId & "','" & ParentPPOUseCodeInfoRowId & "','" & ParentPPOUseCodeGroupIRowId & "','" & ParentPPOUseCodeGroupIIRowId & "','" & hdnClassCode.ClientID & "','" & hdnDescription.ClientID & "','" & hdnPMAID.ClientID & "','" & hdnPMAs.ClientID & "','" & hdnDIAClass_Id.ClientID & "','" & hdnGroupRate.ClientID & "','" & hdnClassLimit.ClientID & "','" & hdnFootNote.ClientID & "','" & trYardRateValidationRow.ClientID & "','" & trPMAValidationRow.ClientID & "','" & divCCInfo.ClientID & "','" & trFootnoteInfoRow.ClientID & "'));")
        Me.VRScript.AddVariableLine("Cpp.UiBindings.push(new Cpp.CrimeClassCodeUiBinding(" & CInt(Me.Quote.LobType).ToString() & "," & pgmtype & ",'" & Me.ParentIDHdnId & "','" & Me.hdnDIAClass_Id.ClientID & "','" & Me.ParentClassCodeTextboxId & "','" & Me.txtClassCode.ClientID & "','" & Me.ParentDescriptionTextboxId & "','" & Me.txtDescription.ClientID & "','" & Me.btnApply.ClientID & "','" & Me.ParentPMAIDHdnId & "','" & Me.ddPMA.ClientID & "','" & Me.ParentGroupRateHdnId & "','" & Me.txtGroupRate.ClientID & "','" & Me.ParentClassLimitHdnId & "','" & Me.txtClassLimit.ClientID & "','" & divFootnote.ClientID & "','" & ParentBCUseCodeRowId & "','" & ParentBCUseCodeInfoRowId & "','" & ParentBCUseCodeGroupIRowId & "','" & ParentBCUseCodeGroupIIRowId & "','" & ParentBICUseCodeRowId & "','" & ParentBICUseCodeInfoRowId & "','" & ParentBICUseCodeGroupIRowId & "','" & ParentBICUseCodeGroupIIRowId & "','" & ParentPPCUseCodeRowId & "','" & ParentPPCUseCodeInfoRowId & "','" & ParentPPCUseCodeGroupIRowId & "','" & ParentPPCUseCodeGroupIIRowId & "','" & ParentPPOUseCodeRowId & "','" & ParentPPOUseCodeInfoRowId & "','" & ParentPPOUseCodeGroupIRowId & "','" & ParentPPOUseCodeGroupIIRowId & "','" & hdnClassCode.ClientID & "','" & hdnDescription.ClientID & "','" & hdnPMAID.ClientID & "','" & hdnPMAs.ClientID & "','" & hdnDIAClass_Id.ClientID & "','" & hdnGroupRate.ClientID & "','" & hdnClassLimit.ClientID & "','" & hdnFootNote.ClientID & "','" & trPMAValidationRow.ClientID & "','" & divCCInfo.ClientID & "','" & trFootnoteInfoRow.ClientID & "'));")

        Me.VRScript.CreatePopupForm(Me.divMain.ClientID, "Crime Class Code Lookup", 750, 550, True, True, False, Me.txtFilterValue.ClientID, "")

        'Create Crime Version
        Me.VRScript.CreateJSBinding(Me.btnSearch.ClientID, "click", "VRClassCode.PerformCPPCrimeClassCodeLookup(" & CInt(Me.Quote.LobType).ToString() & "," & pgmtype & ",'#" & Me.ddlFilterBy.ClientID & "','#" & Me.txtFilterValue.ClientID & "','#" + Me.divResults.ClientID & "'); return false;")
        ''Apply Script when button
        Me.VRScript.CreateJSBinding(Me.btnApply.ClientID, "click", "Cpp.ApplyCrimeClassCode(0);")

        ' These variables will all be used by the Class Code selection script
        ' Hidden Fields
        Me.VRScript.AddVariableLine("var hdnCrimeClassCode = '" & hdnClassCode.ClientID & "';")
        Me.VRScript.AddVariableLine("var hdnCrimeDescription = '" & hdnDescription.ClientID & "';")
        Me.VRScript.AddVariableLine("var hdnCrimePMAID = '" & hdnPMAID.ClientID & "';")
        Me.VRScript.AddVariableLine("var hdnCrimeClassCodeID = '" & hdnDIAClass_Id.ClientID & "';")
        Me.VRScript.AddVariableLine("var hdnCrimeRateGroup = '" & hdnGroupRate.ClientID & "';")
        Me.VRScript.AddVariableLine("var hdnCrimeClassLimit = '" & hdnClassLimit.ClientID & "';")

        ' Input Controls
        Me.VRScript.AddVariableLine("var txtCrimeClassCodeId = '" & txtClassCode.ClientID & "';")
        Me.VRScript.AddVariableLine("var txtCrimeId = '" & txtDIA_Id.ClientID & "';")
        Me.VRScript.AddVariableLine("var txtCrimeDescriptionId = '" & txtDescription.ClientID & "';")
        Me.VRScript.AddVariableLine("var ddCrimePMAId = '" & ddPMA.ClientID & "';")
        Me.VRScript.AddVariableLine("var txtCrimeGroupRateId = '" & txtGroupRate.ClientID & "';")
        Me.VRScript.AddVariableLine("var txtCrimeClassLimitId = '" & txtClassLimit.ClientID & "';")
        Me.VRScript.AddVariableLine("var divCrimeFootNoteId = '" & divFootnote.ClientID & "';")

        ' Info div
        Me.VRScript.AddVariableLine("var divCCInfoId = '" & divCCInfo.ClientID & "';")

        ' Close routine
        Me.VRScript.AddVariableLine("function CloseCCLookupForm(){$('#" + Me.btnClose.ClientID + "').click();}")

        Exit Sub
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Hide()

        ' Clear the fields
        Me.txtFilterValue.Text = ""
        Me.ddlFilterBy.SelectedIndex = 0
        Me.txtClassCode.Text = ""
        Me.txtDescription.Text = ""
        If Me.ddPMA.Items.Count > 0 Then Me.ddPMA.SelectedIndex = 0 Else Me.ddPMA.SelectedIndex = -1
        Me.txtGroupRate.Text = ""
        Me.txtClassLimit.Text = ""
        Me.divFootnote.InnerHtml = ""

        PopulateChildControls()
    End Sub

    Public Sub Show()
        Populate()
        Me.Visible = True
    End Sub

    Public Sub Hide()
        Me.Visible = False
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Hide()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Hide()
    End Sub

    Private Sub btnApply_Click(sender As Object, e As EventArgs) Handles btnApply.Click
        ' WE NEED TO SAVE AND POPULATE HERE BEFORE HIDING THE FORM
        Save_FireSaveEvent(False)
        Populate_FirePopulateEvent()
        Hide()
    End Sub

    Public Sub setChosenClassCode()
        clearChosenClassCode()
        Dim classCodeItem As QuickQuoteClassificationCode = New QuickQuoteClassificationCode()
        classCodeItem.ClassCode = Me.txtClassCode.Text
        classCodeItem.ClassDescription = Me.txtDescription.Text
        If Me.ddPMA.SelectedItem IsNot Nothing Then
            classCodeItem.PMA = Me.ddPMA.SelectedItem.Text
        Else
            classCodeItem.PMA = Nothing
        End If
        classCodeItem.ClassLimit = Me.txtClassLimit.Text
        classCodeItem.RateGroup = Me.txtGroupRate.Text
        classCodeItem.ClassificationCodeNum = Me.txtDIA_Id.Text
        ChosenClassCode = classCodeItem
    End Sub

    Public Sub clearChosenClassCode()
        ChosenClassCode = New QuickQuoteClassificationCode()
        Me.txtClassCode.Text = Nothing
        Me.txtDescription.Text = Nothing
        Me.ddPMA.SelectedValue = Nothing
        Me.txtClassLimit.Text = Nothing
        Me.txtGroupRate.Text = Nothing
        Me.txtDIA_Id.Text = Nothing
    End Sub
End Class