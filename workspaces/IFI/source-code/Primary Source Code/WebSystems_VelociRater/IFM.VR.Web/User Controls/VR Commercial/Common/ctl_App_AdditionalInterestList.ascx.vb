Imports IFM.PrimativeExtensions
Public Class ctl_App_AdditionalInterestList
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hdnAccord, "0")
        Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccordList, "0")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkNew.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Private Sub AddAIEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim AICtl As ctl_App_AdditionalInterest = cntrl.FindControl("ctl_BOP_Additional_Interest")
            AddHandler AICtl.RemoveAi, AddressOf DeleteAddlInterest
            index += 1
        Next
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then
            If Quote.AdditionalInterests IsNot Nothing AndAlso Quote.AdditionalInterests.Count > 0 Then
                Me.Repeater1.DataSource = Me.Quote.AdditionalInterests
                Me.Repeater1.DataBind()

                Me.FindChildVrControls()

                Dim lIndex As Int32 = 0
                For Each cnt In Me.GatherChildrenOfType(Of ctl_App_AdditionalInterest)
                    cnt.AdditionalInterestIndex = lIndex
                    cnt.Populate()
                    lIndex += 1
                Next
            End If
        End If
    End Sub

    Private Sub DeleteAddlInterest(ByVal AIIndex As Integer)
        If Quote IsNot Nothing Then
            If Quote.AdditionalInterests IsNot Nothing Then
                If Quote.AdditionalInterests.HasItemAtIndex(AIIndex) Then
                    Quote.AdditionalInterests.RemoveAt(AIIndex)
                    If Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then 'added IF 2/15/2019; original logic in ELSE
                        'no save
                    ElseIf Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        Dim endorsementSaveError As String = ""
                        Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=endorsementSaveError, saveTypeView:=QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap)
                    Else
                        VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, Nothing, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.AppGap)
                    End If
                    Populate()
                    Save_FireSaveEvent(False)
                    Me.hdnAccordList.Value = (Me.Quote.AdditionalInterests.Count - 1).ToString
                End If
            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.MainAccordionDivId = Me.divMainList.ClientID
            Me.ListAccordionDivId = Me.divList.ClientID
            Me.hdnAccordList.Value = 0
        End If
        AddAIEvents()
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub lnkNew_Click(sender As Object, e As EventArgs) Handles lnkNew.Click
        ' Add additional Interest
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        If Quote.AdditionalInterests Is Nothing Then Quote.AdditionalInterests = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)
        Dim newAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest()
        'newAI.Description = "999"
        'newAI.Name.CommercialName1 = "New999"
        'newAI.Num = Quote.AdditionalInterests.Count + 1
        Quote.AdditionalInterests.Add(newAI)
        'Save_FireSaveEvent(False)
        Populate()
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Save_FireSaveEvent(False)
    End Sub
End Class