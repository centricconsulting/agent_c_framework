Imports IFM.PrimativeExtensions
Public Class ctl_App_AdditionalPolicyholderList
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Private Sub AttachAPHControlEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim APHCtl As ctl_App_AdditionalPolicyholder = cntrl.FindControl("ctl_App_APH")
            AddHandler APHCtl.DeleteAddlPHRequested, AddressOf DeleteAPHRequested
            index += 1
        Next
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then

            Me.Repeater1.DataSource = Me.Quote.AdditionalPolicyholders
            Me.Repeater1.DataBind()

            Me.FindChildVrControls()

            Dim lIndex As Int32 = 0
            For Each cnt In Me.GatherChildrenOfType(Of ctl_App_AdditionalPolicyholder)
                cnt.AdditionalPolicyholderIndex = lIndex
                cnt.Populate()
                lIndex += 1
            Next
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.ListAccordionDivId = Me.divMainList.ClientID
        AttachAPHControlEvents()
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub btnAddAdditionalPolicyholder_Click(sender As Object, e As EventArgs) Handles btnAddAdditionalPolicyholder.Click
        ' Add additional policyholder
        If Quote.AdditionalPolicyholders Is Nothing Then Quote.AdditionalPolicyholders = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalPolicyholder)
        Dim newAP As New QuickQuote.CommonObjects.QuickQuoteAdditionalPolicyholder()
        newAP.Name.CommercialName1 = "New999"
        Quote.AdditionalPolicyholders.Add(newAP)
        Save_FireSaveEvent(False)
        Populate()
    End Sub

    Private Sub DeleteAPHRequested(ByVal APHIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.AdditionalPolicyholders IsNot Nothing Then
                If Quote.AdditionalPolicyholders.HasItemAtIndex(APHIndex) Then Quote.AdditionalPolicyholders.RemoveAt(APHIndex)
                Populate()
                Save_FireSaveEvent(False)
                Me.hdnAccord.Value = (Me.Quote.AdditionalPolicyholders.Count - 1).ToString
            End If
        End If
    End Sub

End Class