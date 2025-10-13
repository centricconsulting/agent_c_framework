Imports IFM.VR.Web.Helpers.FindAppliedAdditionalInterestList
Imports QuickQuote.CommonObjects

Public Class ctl_InlandMarineAssignedAI
    Inherits VRControlBase

    'Added 2/4/2022 for CPP Endorsements task 67310 MLW
    'Currently this control is only used on view only inland marine page.

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer ' New for accordion logic Matt A - 7/14/15
        Get
            Return Me.AdditionalInterestIndex
        End Get
    End Property

    Public Property AdditionalInterestIndex As Int32
        Get
            If ViewState("vs_interestNum") Is Nothing Then
                ViewState("vs_interestNum") = -1
            End If
            Return CInt(ViewState("vs_interestNum"))
        End Get
        Set(value As Int32)
            ViewState("vs_interestNum") = value
        End Set
    End Property

    Public Property MyAdditionalInterest As QuickQuoteAdditionalInterest
        Get
            If AdditionalInterestIndex > -1 AndAlso MyAppliedAiList IsNot Nothing AndAlso MyAppliedAiList.Count > 0 Then
                Dim myAIList = MyAppliedAiList(AdditionalInterestIndex)
                If myAIList.AI IsNot Nothing Then
                    Return myAIList.AI
                End If
            End If
            Return New QuickQuoteAdditionalInterest()
        End Get
        Set(value As QuickQuoteAdditionalInterest)
            If AdditionalInterestIndex > -1 AndAlso MyAppliedAiList IsNot Nothing AndAlso MyAppliedAiList.Count > 0 Then
                Dim myAIList = MyAppliedAiList(AdditionalInterestIndex)
                If myAIList.AI Is Nothing Then
                    myAIList.AI = New QuickQuoteAdditionalInterest
                End If
                myAIList.AI = value
            End If
        End Set
    End Property

    Public ReadOnly Property MyAppliedAiList As List(Of InlandMarineAIItem)
        Get
            Dim appAIs = New Helpers.FindAppliedAdditionalInterestList
            Return appAIs.FindAppliedInlandMarineAI(GoverningStateQuote)
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()

        If MyAdditionalInterest IsNot Nothing AndAlso IsQuoteReadOnly() Then

            LoadInlandMarineDDL()

            LoadPayeeDDLs()

            SetPersonalPropertyAdditionalInterestFieldsFromAI(MyAdditionalInterest)

            Dim expanderDescription As String = MyAdditionalInterest?.Description
            If String.IsNullOrWhiteSpace(expanderDescription) Then
                If MyAdditionalInterest.TypeId = "42" Then
                    expanderDescription = "FIRST MORTGAGEE"
                Else
                    expanderDescription = "N/A"
                End If
            End If


            Me.lblExpanderText.Text = "AI - " + expanderDescription
            If Me.lblExpanderText.Text.Length > 38 Then
                Me.lblExpanderText.Text = Me.lblExpanderText.Text.Substring(0, 38) + "..."
            End If

        End If
    End Sub

    Private Sub SetPersonalPropertyAdditionalInterestFieldsFromAI(ByVal ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest, Optional ByVal setLossPayeeName As Boolean = True)
        If ai IsNot Nothing Then
            If String.IsNullOrWhiteSpace(ai.Description) = False Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(ddlGAIDescriptions, ai.Description.ToUpper, ai.Description.ToUpper)
                Me.hdnAiDescription.Value = ai.Description.ToUpper
            End If
            If setLossPayeeName = True Then
                If QQHelper.IsPositiveIntegerString(ai.ListId) = True Then
                    If Me.ddlGAILimitLossPayeeName.Items IsNot Nothing Then
                        If Me.ddlGAILimitLossPayeeName.Items.Count > 0 AndAlso Me.ddlGAILimitLossPayeeName.Items.FindByValue(ai.ListId) IsNot Nothing Then
                            Me.ddlGAILimitLossPayeeName.SelectedValue = ai.ListId
                        Else
                            'add name to dropdown and set

                        End If
                    End If
                End If
            End If
            If QQHelper.IsPositiveIntegerString(ai.TypeId) = True Then
                If Me.ddlGAILimitLossPayeeType.Items IsNot Nothing Then
                    If Me.ddlGAILimitLossPayeeType.Items.Count > 0 AndAlso Me.ddlGAILimitLossPayeeType.Items.FindByValue(ai.TypeId) IsNot Nothing Then
                        Me.ddlGAILimitLossPayeeType.SelectedValue = ai.TypeId
                    Else
                        'add type to dropdown and set

                    End If
                End If
            End If
            If ai.ATIMA = True AndAlso ai.ISAOA = True Then
                Me.ddlGAILimitATMA.SelectedValue = "3"
            ElseIf ai.ATIMA = True Then
                Me.ddlGAILimitATMA.SelectedValue = "1"
            ElseIf ai.ISAOA = True Then
                Me.ddlGAILimitATMA.SelectedValue = "2"
            Else 'ElseIf ai.ATIMA = False AndAlso ai.ISAOA = False Then
                Me.ddlGAILimitATMA.SelectedValue = "0"
            End If

            If AdditionalInterestIndex > -1 AndAlso MyAppliedAiList IsNot Nothing AndAlso MyAppliedAiList.Count > 0 Then
                Dim myAIList = MyAppliedAiList(AdditionalInterestIndex)
                If myAIList IsNot Nothing Then
                    ddlIMType.SelectedValue = myAIList.InlandMarineType
                End If
            End If
        End If
    End Sub

    Private Sub LoadPayeeDDLs()

        ddlGAILimitLossPayeeName.Items.Clear()

        If Quote IsNot Nothing AndAlso Quote.AdditionalInterests IsNot Nothing Then
            For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In Quote.AdditionalInterests
                IFM.VR.Web.Helpers.WebHelper_Personal.AddDropDownValueIfMissing(ddlGAILimitLossPayeeName, ai.ListId, ai.Name.DisplayName)
            Next
        End If

    End Sub

    Private Sub LoadInlandMarineDDL()
        ddlIMType.Items.Clear()
        ddlIMType.Items.Add(New ListItem With {.Text = "", .Value = ""})
        ddlIMType.Items.Add(New ListItem With {.Text = InlandMarineTypeString.BuildersRisk, .Value = InlandMarineTypeString.BuildersRisk})
        ddlIMType.Items.Add(New ListItem With {.Text = InlandMarineTypeString.Computers, .Value = InlandMarineTypeString.Computers})
        ddlIMType.Items.Add(New ListItem With {.Text = InlandMarineTypeString.ContractorsScheduledEquipment, .Value = InlandMarineTypeString.ContractorsScheduledEquipment})
        ddlIMType.Items.Add(New ListItem With {.Text = InlandMarineTypeString.FineArts, .Value = InlandMarineTypeString.FineArts})
        ddlIMType.Items.Add(New ListItem With {.Text = InlandMarineTypeString.MotorTruckCargo, .Value = InlandMarineTypeString.MotorTruckCargo})
        ddlIMType.Items.Add(New ListItem With {.Text = InlandMarineTypeString.OwnersCargo, .Value = InlandMarineTypeString.OwnersCargo})
        ddlIMType.Items.Add(New ListItem With {.Text = InlandMarineTypeString.ScheduledPropertyFloater, .Value = InlandMarineTypeString.ScheduledPropertyFloater})
        ddlIMType.Items.Add(New ListItem With {.Text = InlandMarineTypeString.Signs, .Value = InlandMarineTypeString.Signs})
        ddlIMType.Items.Add(New ListItem With {.Text = InlandMarineTypeString.Transportation, .Value = InlandMarineTypeString.Transportation})
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)

    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

End Class