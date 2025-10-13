Imports System.Web.Services.Description
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.FARM
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Public Class ctl_FarBuildingList
    Inherits VRControlBase

    Public Property ActiveBuildingPane As String
        Get
            Return Me.hiddenActiveBuilding.Value
        End Get
        Set(value As String)
            Me.hiddenActiveBuilding.Value = value
        End Set
    End Property

    Public Property MyLocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_locationIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing Then
                Return Me.Quote.Locations.GetItemAtIndex(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Me.divNewBuilding.Visible Then
            Me.VRScript.CreateAccordion(Me.divNewBuilding.ClientID, Nothing, "false", True)
        Else
            If Me.divBuildingList.Visible Then
                Me.VRScript.CreateAccordion(Me.divBuildingList.ClientID, hiddenActiveBuilding, "0")
            End If
        End If

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.divNewBuilding.Visible = False
        Me.divBuildingList.Visible = False
        'Updated 9/10/18 for multi state MLW - Quote to SubQuoteFirst
        If SubQuoteFirst IsNot Nothing Then
            If SubQuoteFirst.ProgramTypeId.Trim() <> "8" Then
                If Me.MyLocation IsNot Nothing Then
                    If Me.MyLocation.Buildings.IsLoaded Then
                        Me.divBuildingList.Visible = True
                        Me.Repeater1.DataSource = Me.MyLocation.Buildings
                        Me.Repeater1.DataBind()
                        Me.FindChildVrControls() ' finds the just added controls do to the binding
                        Dim index As Int32 = 0
                        For Each c As ctl_FarBuilding In Me.GatherChildrenOfType(Of ctl_FarBuilding)()
                            c.MyBuildingIndex = index
                            c.MyLocationIndex = Me.MyLocationIndex
                            c.Populate()
                            index += 1
                        Next
                    Else
                        Me.divNewBuilding.Visible = True
                    End If

                    Dim activePane As Int32 = 0 'usually will do nothing but will make sure that after removing an item the active pane gets adjusted
                    If Int32.TryParse(Me.ActiveBuildingPane, activePane) Then
                        Do While activePane > Me.MyLocation.Buildings.Count() - 1
                            activePane -= 1
                            Me.hiddenActiveBuilding.Value = (activePane).ToString()
                        Loop
                    End If

                End If
            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.ListAccordionDivId = Me.divBuildingList.ClientID
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Buildings"
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Sub CreateNewBuilding()
        If Me.Quote IsNot Nothing AndAlso Me.MyLocation IsNot Nothing Then
            Me.MyLocation.Buildings.AddNew()

            ' Default Cosmetic Damage Exclusion to Checked on creation
            Dim MyBuilding As QuickQuoteBuilding
            Dim cov = New QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE() With {.CoverageType = QuickQuote.CommonObjects.QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Cosmetic_Damage_Exclusion_Coverage_E}
            cov.IncreasedLimit = String.Empty
            MyBuilding = Me.MyLocation.Buildings.LastOrDefault

            If cov.CoverageType = QuickQuoteOptionalCoverageE.QuickQuoteOptionalCoverageEType.Cosmetic_Damage_Exclusion_Coverage_E Then
                If CosmeticDamageExHelper.IsCosmeticDamageExAvailable(Quote) AndAlso MyBuilding IsNot Nothing Then
                    If MyBuilding.OptionalCoverageEs Is Nothing Then
                        MyBuilding.OptionalCoverageEs = New List(Of QuickQuoteOptionalCoverageE)
                    End If
                    MyBuilding.OptionalCoverageEs.Add(cov)
                End If
            End If

            Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

            Me.Populate()
            Me.ActiveBuildingPane = (Me.MyLocation.Buildings.Count() - 1).ToString()
            Dim lastBuilding = Me.GatherChildrenOfType(Of ctl_FarBuilding)().LastOrDefault() ' added 10-1-2015
            If lastBuilding IsNot Nothing Then
                Dim lastBuilding_PropertyControl = lastBuilding.GatherChildrenOfType(Of ctl_FarmBuilding_Property).FirstOrDefault()
                If lastBuilding_PropertyControl IsNot Nothing Then
                    lastBuilding.OpenAllParentAccordionsOnNextLoad(lastBuilding_PropertyControl.ScrollToControlId)
                End If
            End If
            lastBuilding.NewBuilding = "True"
        End If

    End Sub


    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        For Each ri As RepeaterItem In Repeater1.Items
            Dim ctlBLD As ctl_FarBuilding = ri.FindControl("ctl_FarBuilding")
            If ctlBLD IsNot Nothing Then ctlBLD.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
        Next
        Exit Sub
    End Sub
End Class