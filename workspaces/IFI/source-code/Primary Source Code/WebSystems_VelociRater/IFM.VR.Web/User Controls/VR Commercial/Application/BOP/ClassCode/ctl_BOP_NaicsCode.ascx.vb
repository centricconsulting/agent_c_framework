Imports Diamond.Business.ThirdParty.ISO.PassportV2.CoverageVerifier.Objects
Imports Diamond.Common.StaticDataManager.Objects.VersionData
Imports Diamond.Web.BaseControls
Imports IFM.VR.Common.Helpers
Imports IFM.VR.Common.Helpers.BOP
Imports IFM.VR.Common.Helpers.CL

Public Class ctl_BOP_NaicsCode
    Inherits VRControlBase

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        ctl_BOP_NaicsCodeLookup.NaicsCodeId = txtNaicsCode.ClientID
        ctl_BOP_NaicsCodeLookup.NaicsDescriptionId = txtNaicsDescription.ClientID
        Me.ctl_BOP_NaicsCodeLookup.Hide()
        PopulateNaicsFields()
    End Sub

    Private Sub PopulateNaicsFields()
        If Quote IsNot Nothing Then
            Dim dictNaicsCode As String = Quote.GetDevDictionaryItem("", "NAICS")
            Dim dictNaicsDescription As String = Quote.GetDevDictionaryItem("", "NAICSdescription")

            If Not String.IsNullOrWhiteSpace(dictNaicsCode) Then
                txtNaicsCode.Text = dictNaicsCode
                If Not String.IsNullOrWhiteSpace(dictNaicsDescription) Then
                    txtNaicsDescription.Text = dictNaicsDescription
                Else
                    txtNaicsDescription.Text = NaicsCodeHelper.LookUpNaicsDescription(dictNaicsCode)

                End If
                HideNaicsFind()
                Return
            End If

            txtNaicsCode.Text = Quote.Policyholder.Name.NAICS
            txtNaicsDescription.Text = NaicsCodeHelper.LookUpNaicsDescription(Quote.Policyholder.Name.NAICS)
            ShowNaicsFind()
        End If

    End Sub

    Private Sub HideNaicsFind()
        btnNaicsSearch.Visible = False
    End Sub
    Private Sub ShowNaicsFind()
        btnNaicsSearch.Visible = True
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.AddScriptLine("$(""#NAICSSection"").accordion({collapsible: false, heightStyle: ""content""});")
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
        Me.VRScript.AddScriptLine("ifm.vr.ui.SingleElementDisable(['" & txtNaicsCode.ClientID & "']);", onlyAllowOnce:=True)
        Me.VRScript.AddScriptLine("ifm.vr.ui.SingleElementDisable(['" & txtNaicsDescription.ClientID & "']);", onlyAllowOnce:=True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsNewCo() Then
            Me.Visible = False
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing AndAlso Quote.Policyholder IsNot Nothing Then

            Quote.Policyholder.Name.NAICS = Me.txtNaicsCode.Text

        End If
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        ValidationHelper.GroupName = "NAICS Code"

        If IsNewCo() = True Then
            If String.IsNullOrWhiteSpace(Me.txtNaicsCode.Text) Then
                Me.ValidationHelper.AddError(btnNaicsSearch, "Missing NAICS Code", accordList)
            End If
        End If

        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New VRValidationArgs(Me.DefaultValidationType)))
    End Sub
    Private Sub btnClassCodeLookup_Click(sender As Object, e As EventArgs) Handles btnNaicsSearch.Click
        Me.ctl_BOP_NaicsCodeLookup.Show()
    End Sub
End Class