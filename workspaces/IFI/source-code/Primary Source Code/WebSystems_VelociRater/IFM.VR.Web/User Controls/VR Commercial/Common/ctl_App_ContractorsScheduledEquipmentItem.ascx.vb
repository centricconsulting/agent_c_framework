Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
'Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctl_App_ContractorsScheduledEquipmentItem
    Inherits VRControlBase

#Region "Declarations"

    Public Property ItemIndex As Int32
        Get
            Return ViewState.GetInt32("vs_ContractorsItemIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_ContractorsItemIndex") = value
        End Set
    End Property

    Private ReadOnly Property MyItem As QuickQuote.CommonObjects.QuickQuoteContractorsEquipmentScheduledItem
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.ContractorsEquipmentScheduledItems IsNot Nothing AndAlso Me.Quote.ContractorsEquipmentScheduledItems.HasItemAtIndex(Me.ItemIndex) Then
                Return Me.Quote.ContractorsEquipmentScheduledItems(ItemIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return Me.ItemIndex
        End Get
    End Property

    Public Event ContractorsItemDeleteRequested(ByVal ItemIndex As Integer)
    Public Event ContractorsItemChanged(ByVal ItemIndex As Integer)

#End Region

#Region "Methods and Functions"

    Private Sub HandleError(ByVal RoutineName As String, ByVal ex As Exception)
        Dim str As String = RoutineName & ":  " & ex.Message
        If AppSettings("TestOrProd").ToUpper <> "PROD" Then lblMsg.Text = str Else Throw New Exception(ex.Message, ex)
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.CreateConfirmDialog(Me.lnkDelete.ClientID, "Remove?")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Private Sub PopulateLossPayeeDDL()
        Dim li As New ListItem()
        ddlLossPayeeName.Items.Clear()
        li.Text = "N/A"
        li.Value = ""
        ddlLossPayeeName.Items.Add(li)
        If Quote IsNot Nothing AndAlso Quote.AdditionalInterests IsNot Nothing Then
            For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In Quote.AdditionalInterests
                li = New ListItem
                li.Text = ai.Name.DisplayName
                li.Value = ai.ListId
                ddlLossPayeeName.Items.Add(li)
            Next
        End If
    End Sub

    Public Overrides Sub Populate()
        Dim err As String = Nothing

        Try
            lblMsg.Text = "&nbsp;"
            ClearInputFields()

            If MyItem IsNot Nothing Then
                PopulateLossPayeeDDL()
                lblAccordHeader.Text = "Item #" & CInt(ItemIndex + 1).ToString & " " & MyItem.Limit
                ddlValuationMethod.SelectedValue = MyItem.ValuationMethodId
                txtLimit.Text = MyItem.Limit
                txtDescription.Text = MyItem.Description
                If MyItem.AdditionalInterests IsNot Nothing AndAlso MyItem.AdditionalInterests.Count > 0 Then
                    Dim AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest = MyItem.AdditionalInterests(0)
                    ddlLossPayeeName.SelectedValue = AI.ListId
                    ddlLossPayeeType.SelectedValue = AI.TypeId
                    If AI.ATIMA = False AndAlso AI.ISAOA = False Then
                        ddlATIMA.SelectedValue = 0
                    ElseIf AI.ATIMA = True AndAlso AI.ISAOA = False Then
                        ddlATIMA.SelectedValue = 1
                    ElseIf AI.ATIMA = False AndAlso AI.ISAOA = True Then
                        ddlATIMA.SelectedValue = 2
                    ElseIf AI.ATIMA = True AndAlso AI.ISAOA = True Then
                        ddlATIMA.SelectedValue = 3
                    End If
                End If
            End If

            Me.PopulateChildControls()

            Exit Sub
        Catch ex As Exception
            HandleError("Populate", ex)
        End Try
    End Sub

    Public Overrides Function Save() As Boolean
        Dim qqXml As New QuickQuoteXML()
        Dim diamondList As Diamond.Common.Objects.Policy.AdditionalInterestList = Nothing

        Try
            lblMsg.Text = "&nbsp;"
            If MyItem IsNot Nothing Then
                MyItem.ValuationMethodId = ddlValuationMethod.SelectedValue
                MyItem.Limit = txtLimit.Text
                MyItem.Description = txtDescription.Text
                If ddlLossPayeeName.SelectedIndex > 0 Then
                    Dim AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest = LoadAIInfoFromForm()
                    qqXml.DiamondService_CreateAdditionalInterestList(AI, diamondList)
                    If diamondList IsNot Nothing Then
                        MyItem.AdditionalInterests = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)
                        MyItem.AdditionalInterests.Add(AI)
                    End If
                    'If diamondList IsNot Nothing Then
                    '    If IsNumeric(AI.ListId) Then
                    '        Me.txtAiId.Text = MyAdditionalInterest.ListId ' just in case you don't repopulate the form it will know it was already created
                    '        AdditionalInterstIdsCreatedThisSession.Add(CInt(MyAdditionalInterest.ListId))
                    '        RaiseEvent AIListChanged()
                    '    End If
                    'End If

                    'MyItem.AdditionalInterests = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)
                    'MyItem.AdditionalInterests.Add(AI)
                End If
            End If

            Me.SaveChildControls()

            Return True
        Catch ex As Exception
            HandleError("Save", ex)
            Return False
        End Try
    End Function

    Private Function LoadAIInfoFromForm() As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest
        Dim NewAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest()

        If Me.Quote IsNot Nothing AndAlso Quote.AdditionalInterests IsNot Nothing Then
            For Each AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In Quote.AdditionalInterests
                If AI.Name.DisplayName.ToUpper = ddlLossPayeeName.SelectedItem.Text.ToUpper Then      'Return AI
                    NewAI.Name = AI.Name
                    NewAI.Address = AI.Address
                    NewAI.Description = txtDescription.Text
                    NewAI.TypeId = ddlLossPayeeType.SelectedValue
                    Select Case ddlATIMA.SelectedValue
                        Case 0
                            NewAI.ATIMA = False
                            NewAI.ISAOA = False
                        Case 1
                            NewAI.ATIMA = True
                            NewAI.ISAOA = False
                        Case 2
                            NewAI.ATIMA = False
                            NewAI.ISAOA = True
                        Case 3
                            NewAI.ATIMA = True
                            NewAI.ISAOA = True
                    End Select
                    Return NewAI
                End If
            Next
        End If

        Return Nothing
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Try
            MyBase.ValidateControl(valArgs)
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
            Me.ValidationHelper.GroupName = "Contractors Scheduled Items #" & ItemIndex + 1

            If txtLimit.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtLimit, "Missing Limit", accordList)
            Else
                If Not IsNumeric(txtLimit.Text) OrElse CInt(txtLimit.Text) <= 0 Then
                    Me.ValidationHelper.AddError(txtLimit, "Limit is invalid", accordList)
                End If
            End If

            If txtDescription.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtDescription, "Missing Description", accordList)
            End If

            Me.ValidateChildControls(valArgs)

            Exit Sub
        Catch ex As Exception
            HandleError("ValidateControls", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub ClearInputFields()
        Try
            ddlValuationMethod.SelectedIndex = 0
            txtLimit.Text = ""
            txtDescription.Text = ""
            ddlLossPayeeName.SelectedIndex = 0
            ddlLossPayeeType.SelectedIndex = 0
            ddlATIMA.SelectedIndex = 0

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
            If Not IsPostBack Then
                'Me.MainAccordionDivId = Me.divBuildingClassification.ClientID
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

    Private Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        RaiseEvent ContractorsItemDeleteRequested(ItemIndex)
    End Sub

#End Region


End Class