Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CGL
Imports QuickQuote.CommonObjects
Public Class ctl_CPP_ENDO_Liability_LocationList
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Property ActiveLocationIndex As String
        Get
            Return hdnAccord.Value
        End Get
        Set(value As String)
            hdnAccord.Value = value
        End Set
    End Property

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then
            ' Don't show the rate button when contractors enhancement is selected
            'Updated 12/21/18 for multi state MLW
            'If Quote.HasContractorsEnhancement Then
            If SubQuoteFirst.HasContractorsEnhancement Then
                Me.btnRate.Visible = False
            Else
                Me.btnRate.Visible = True
            End If
            Me.divLocationList.Visible = False
            If Me.Quote.Locations.IsLoaded() Then
                Me.divLocationList.Visible = True
                Me.Repeater1.DataSource = Me.Quote.Locations
                Me.Repeater1.DataBind()
                Me.FindChildVrControls()

                Dim index As Int32 = 0
                For Each Loc As ctl_CPP_ENDO_Liabilty_Location In Me.GatherChildrenOfType(Of ctl_CPP_ENDO_Liabilty_Location)
                    Loc.MyLocationIndex = index
                    'Loc.Populate()
                    index += 1
                Next
            Else
                'Me.divNewLocation.Visible = True
                Me.Repeater1.DataSource = Nothing
                Me.Repeater1.DataBind()
            End If

            Dim EndoCppHelper = New EndorsementsCppHelper(Quote)
            ClassCodeDeleteMessage.Visible = EndoCppHelper.ShowDeleteMessage
            ClassCodeAddMessage.Visible = EndoCppHelper.ShowAddMessage

        End If
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.ListAccordionDivId = Me.divLocationList.ClientID
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        If Not IsQuoteReadOnly() Then
            CGLMedicalExpensesExcludedClassCodesHelper.UpdateAndShowMessagesForMedicalExpensesDropdownForExcludedGLCodes(Quote, Me.Page)
        End If
        Session("CPPCPREventTrigger") = String.Empty
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click, btnRate.Click
        Me.Save_FireSaveEvent()
        If sender.Equals(btnRate) Then
            Session("CPPCPREventTrigger") = "RateButton"
            If Me.ValidationSummmary.HasErrors = False Then
                Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
            End If
        End If
    End Sub

End Class