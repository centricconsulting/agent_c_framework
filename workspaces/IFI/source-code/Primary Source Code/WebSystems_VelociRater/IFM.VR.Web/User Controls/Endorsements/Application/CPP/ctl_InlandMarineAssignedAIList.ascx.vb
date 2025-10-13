Imports QuickQuote.CommonObjects

'Added 2/4/2022 for CPP Endorsements task 67310 MLW
'Currently this control is only used on view only inland marine page.

Public Class ctl_InlandMarineAssignedAIList
    Inherits VRControlBase

    Public ReadOnly Property MyAiList As List(Of InlandMarineAIItem)
        Get
            Dim appAIs = New Helpers.FindAppliedAdditionalInterestList
            Return appAIs.FindAppliedInlandMarineAI(GoverningStateQuote)
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadStaticData()
        End If
        Me.MainAccordionDivId = Me.divIMAssignedAdditionalInterests.ClientID
        Me.ListAccordionDivId = Me.divIMAssignedAdditionalInterestItems.ClientID
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.Repeater1.DataSource = Nothing
        If Me.Quote IsNot Nothing AndAlso IsQuoteReadOnly() Then
            Me.Repeater1.DataSource = MyAiList
            Me.Repeater1.DataBind()
            If MyAiList IsNot Nothing AndAlso MyAiList.Any() Then
                Me.lblHeader.Text = String.Format("Assigned Additional Interests ({0})", MyAiList.Count)
                Me.divIMAssignedAdditionalInterestItems.Visible = True
                Me.FindChildVrControls()
                Dim index As Int32 = 0
                For Each child In Me.ChildVrControls
                    If TypeOf child Is ctl_InlandMarineAssignedAI Then
                        Dim c As ctl_InlandMarineAssignedAI = CType(child, ctl_InlandMarineAssignedAI)
                        c.AdditionalInterestIndex = index
                        c.Populate()
                        index += 1
                    End If
                Next
            Else
                Select Case Me.Quote.LobType
                    Case Else
                        Me.lblHeader.Text = "Assigned Additional Interests (0)"
                End Select
                Me.divIMAssignedAdditionalInterestItems.Visible = False
            End If
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Me.Quote IsNot Nothing Then
            If MyAiList IsNot Nothing AndAlso MyAiList.Any() Then
                Me.VRScript.CreateAccordion(Me.divIMAssignedAdditionalInterests.ClientID, hiddenIMAdditionalInterest, "false")
            Else
                Me.VRScript.CreateAccordion(Me.divIMAssignedAdditionalInterests.ClientID, hiddenIMAdditionalInterest, "false", True)
            End If
        End If

        Me.VRScript.CreateAccordion(Me.divIMAssignedAdditionalInterestItems.ClientID, hiddenIMAdditionalInterestItems, "0")
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)

    End Sub

    Public Overrides Function Save() As Boolean

        Return True
    End Function

End Class

Public Class InlandMarineAIItem
    Property InlandMarineType As String
    Property AiIndex As Int32
    Property CeAiIndex As Int32
    Property AI As QuickQuoteAdditionalInterest

    Public Sub New(inlandMarineType, aiIndex, ceAiIndex, ai)
        Me.InlandMarineType = inlandMarineType
        Me.AiIndex = aiIndex
        Me.CeAiIndex = ceAiIndex
        Me.AI = ai
    End Sub

End Class