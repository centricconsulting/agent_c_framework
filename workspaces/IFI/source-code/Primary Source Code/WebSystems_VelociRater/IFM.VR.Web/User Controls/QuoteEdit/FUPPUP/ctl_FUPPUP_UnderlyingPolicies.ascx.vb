Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports IFM.VR.Common.Helpers.MultiState.General
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctl_FUPPUP_UnderlyingPolicies
    Inherits VRControlBase

#Region "Declarations"
    Public ReadOnly UmbrellaDictionaryName As String = "UmbrellaUnderlyingDetails"



#End Region

#Region "Methods and Functions"

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(divUmbrellaUnderlyingPolicies.ClientID, hdnAccordGenInfo, "0")

        Me.VRScript.StopEventPropagation(Me.lnkSaveGeneralInfo.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkClearGeneralInfo.ClientID)
    End Sub


    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()

        If Quote IsNot Nothing Then
            'Do Sorting and Filtering on List
            'Determine which should be shown
            ctl_FUPPUP_UnderlyingPolicy_Item_hom.LobCategoryText = "Homeowners Policy or Quote"
            ctl_FUPPUP_UnderlyingPolicy_Item_hom.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal
            ctl_FUPPUP_UnderlyingPolicy_Item_ppa.LobCategoryText = "Personal Auto Policy or Quote"
            ctl_FUPPUP_UnderlyingPolicy_Item_ppa.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal
            ctl_FUPPUP_UnderlyingPolicy_Item_far.LobCategoryText = "Farm Policy or Quote"
            ctl_FUPPUP_UnderlyingPolicy_Item_far.LobType = QuickQuoteObject.QuickQuoteLobType.Farm
            ctl_FUPPUP_UnderlyingPolicy_Item_dfr.LobCategoryText = "Dwelling Fire Policy or Quote"
            ctl_FUPPUP_UnderlyingPolicy_Item_dfr.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
            ctl_FUPPUP_UnderlyingPolicy_Item_wcp.LobCategoryText = "Workers Comp Policy or Quote"
            ctl_FUPPUP_UnderlyingPolicy_Item_wcp.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
            'ctl_FUPPUP_UnderlyingPolicy_Item_wcp.LobCategoryMsg = "We require minimum limits of 500/500/500 when quoting an umbrella."
            ctl_FUPPUP_UnderlyingPolicy_Item_cap.LobCategoryText = "Commercial Auto Policy or Quote"
            ctl_FUPPUP_UnderlyingPolicy_Item_cap.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto
            ctl_FUPPUP_UnderlyingPolicy_Item_cap.LobCategoryMsg = "Note: All youthful drivers need to be entered on the Commercial Auto policy to receive a correct rate."

            If SubQuoteFirst.ProgramTypeId = "4" Then
                'personal 
                ctl_FUPPUP_UnderlyingPolicy_Item_far.Visible = False
                'ctl_FUPPUP_UnderlyingPolicy_Item_dfr.Visible = False
                ctl_FUPPUP_UnderlyingPolicy_Item_wcp.Visible = False
                ctl_FUPPUP_UnderlyingPolicy_Item_cap.Visible = False

            Else
                'farm
                ctl_FUPPUP_UnderlyingPolicy_Item_hom.Visible = False

                If Quote.Policyholder.Name.TypeId = "2" Then
                    'commercial
                Else
                    'personal
                End If
            End If


        End If
        PopulateChildControls()
    End Sub

    Public Overrides Function Save() As Boolean

        If Me.Quote IsNot Nothing Then

        End If
        SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        MyBase.ValidateControl(valArgs)

        Me.ValidationHelper.GroupName = "Policy Level Coverages"

        ValidateChildControls(valArgs)

        Exit Sub
    End Sub


#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkSaveGeneralInfo.Click
        Me.Save_FireSaveEvent()
        Populate()
    End Sub

    Public Sub VerifyPolicies()
        If ChildVrControls.Any() = False Then
            FindChildVrControls()
        End If
        For Each control As VRControlBase In Me.ChildVrControls
            If TypeOf control Is ctl_FUPPUP_UnderlyingPolicy_Item Then
                Dim item As ctl_FUPPUP_UnderlyingPolicy_Item = DirectCast(control, ctl_FUPPUP_UnderlyingPolicy_Item)
                item.VerifyPolicy()
            End If
        Next

    End Sub

    Protected Sub lnkClearGeneralInfo_Click(sender As Object, e As EventArgs) Handles lnkClearGeneralInfo.Click
        ClearControl()
        Populate()
    End Sub

    Public Overrides Sub ClearControl()
        GoverningStateQuote.UnderlyingPolicies = New List(Of Umbrella.QuickQuoteUnderlyingPolicy)
        Dim ddh = New DevDictionaryHelper.DevDictionaryHelper(Quote, UmbrellaDictionaryName)
        ddh.ClearMasterDictionaryValue()
    End Sub

#End Region

End Class