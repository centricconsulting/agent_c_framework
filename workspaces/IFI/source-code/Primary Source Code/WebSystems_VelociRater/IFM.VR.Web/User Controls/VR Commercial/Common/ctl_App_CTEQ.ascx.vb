Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
'Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Public Class ctl_App_CTEQ
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
            If GoverningStateQuote() IsNot Nothing AndAlso GoverningStateQuote.ContractorsEquipmentScheduledItems IsNot Nothing AndAlso GoverningStateQuote.ContractorsEquipmentScheduledItems.HasItemAtIndex(Me.ItemIndex) Then
                Return GoverningStateQuote.ContractorsEquipmentScheduledItems(ItemIndex)
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
        Dim headerScript = $"ContractorEquipmentHeaderUpdate({Me.ItemIndex.ToString()},'{Me.lblAccordHeader.ClientID}','{Me.txtLimit.ClientID}','{Me.txtDescription.ClientID}')"
        Me.VRScript.AddScriptLine("$('#" + Me.txtLimit.ClientID + "').keyup(function(){ " + headerScript + "});")
        Me.VRScript.AddScriptLine("$('#" + Me.txtDescription.ClientID + "').keyup(function(){ " + headerScript + "});")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Private Sub PopulateLossPayeeDDL()
        '6/1/2017 note: similar logic in ctl_BOP_App_Building.ascx.vb
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
        Dim dsc As String = Nothing

        Try
            lblMsg.Text = "&nbsp;"
            ClearInputFields()

            If MyItem IsNot Nothing Then
                If MyItem.Description.Length < 20 Then
                    dsc = MyItem.Description
                Else
                    dsc = MyItem.Description.Substring(0, 20)
                End If
                'Updated 09/13/2021 for BOP Endorsement Task 63910 MLW
                If Not IsQuoteReadOnly() Then
                    PopulateLossPayeeDDL()
                End If
                'PopulateLossPayeeDDL()
                lblAccordHeader.Text = "Item #" & CInt(ItemIndex + 1).ToString & " $" & MyItem.Limit & " - " & dsc
                'ddlValuationMethod.SelectedValue = MyItem.ValuationMethodId
                txtLimit.Text = MyItem.Limit
                txtDescription.Text = MyItem.Description

                '6/7/2017 note on AIs: this control will just work w/ the specific Contractors Equipment AIs that it needs from the Building AIs; the full list of Building AIs is still in tact; these Contractors Equipment AIs are copied back and forth from the Building AIs in the QuickQuote library on Retrieval and Save
                '6/7/2017 note on ContractorsEquipmentScheduledItems (list) and ContractorsToolsEquipmentScheduled (total amount) properties: the QuickQuote library will now sync up both properties, so you should only need to use one or the other... ContractorsToolsEquipmentScheduled can be used from the Quote side to set the total amount, and ContractorsEquipmentScheduledItems should be the only one used from here (App side)

                'Updated 09/13/2021 for BOP Endorsement Task 63910 MLW
                If IsQuoteReadOnly() Then
                    trLossPayeeName.Visible = False
                    trLossPayeeType.Visible = False
                    trATIMA.Visible = False
                Else
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
            End If

                Me.PopulateChildControls()

            Exit Sub
        Catch ex As Exception
            HandleError("Populate", ex)
        End Try
    End Sub

    Public Overrides Function Save() As Boolean
        Dim dsc As String = Nothing
        Dim qqXml As New QuickQuoteXML()
        Dim diamondList As Diamond.Common.Objects.Policy.AdditionalInterestList = Nothing
        'Dim tot As Decimal = 0 'removed 6/7/2017 since we don't need to set ContractorsToolsEquipmentScheduled from the App side; everything is done in the list, and ContractorsToolsEquipmentScheduled will always reflect the total from there

        Try
            lblMsg.Text = "&nbsp;"
            If MyItem IsNot Nothing Then
                'MyItem.ValuationMethodId = ddlValuationMethod.SelectedValue
                MyItem.Limit = txtLimit.Text
                MyItem.Description = txtDescription.Text.ToMaxLength(249)

                ' TODO: Need to save Loss Payee info
                If ddlLossPayeeName.SelectedIndex > 0 Then
                    'Dim AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest = LoadAIInfoFromForm()
                    'qqXml.DiamondService_CreateAdditionalInterestList(AI, diamondList)
                    'If diamondList IsNot Nothing Then
                    '    MyItem.AdditionalInterests = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)
                    '    MyItem.AdditionalInterests.Add(AI)
                    'End If
                    'If diamondList IsNot Nothing Then
                    '    If IsNumeric(AI.ListId) Then
                    '        Me.txtAiId.Text = MyAdditionalInterest.ListId ' just in case you don't repopulate the form it will know it was already created
                    '        AdditionalInterstIdsCreatedThisSession.Add(CInt(MyAdditionalInterest.ListId))
                    '        RaiseEvent AIListChanged()
                    '    End If
                    'End If

                    'MyItem.AdditionalInterests = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInterest)
                    'MyItem.AdditionalInterests.Add(AI)

                    'added 6/7/2017
                    If MyItem.AdditionalInterests Is Nothing Then
                        MyItem.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                    End If
                    Dim aiToUse As QuickQuoteAdditionalInterest = Nothing
                    If MyItem.AdditionalInterests.Count = 0 Then
                        'QuickQuote.CommonMethods.QuickQuoteHelperClass.AddNewAdditionalInterestToList(MyItem.AdditionalInterests, ai:=aiToUse, listId:=Me.ddlLossPayeeName.SelectedValue, typeId:=Me.ddlLossPayeeType.SelectedValue) 'setting listId and typeId here may be redundant
                        aiToUse = New QuickQuoteAdditionalInterest
                        MyItem.AdditionalInterests.Add(aiToUse)
                    End If
                    If MyItem.AdditionalInterests.Count > 0 Then
                        If aiToUse Is Nothing Then
                            aiToUse = MyItem.AdditionalInterests(0)
                        End If
                        Dim isSameListId As Boolean = False
                        With aiToUse
                            If .ListId = Me.ddlLossPayeeName.SelectedValue Then
                                isSameListId = True
                            Else
                                isSameListId = False 'redundant
                            End If
                            .ListId = Me.ddlLossPayeeName.SelectedValue
                            .TypeId = Me.ddlLossPayeeType.SelectedValue
                            Select Case Me.ddlATIMA.SelectedValue
                                Case "3"
                                    .ATIMA = True
                                    .ISAOA = True
                                Case "1"
                                    .ATIMA = True
                                    .ISAOA = False
                                Case "2"
                                    .ISAOA = True
                                    .ATIMA = False
                                Case Else '0
                                    .ATIMA = False
                                    .ISAOA = False
                            End Select
                            'If isSameListId = False Then 'will just run every time
                            If Me.Quote IsNot Nothing AndAlso Me.Quote.AdditionalInterests IsNot Nothing AndAlso Me.Quote.AdditionalInterests.Count > 0 Then
                                Dim sourceAI As QuickQuoteAdditionalInterest = QuickQuoteHelperClass.QuickQuoteAdditionalInterestForListId(Me.Quote.AdditionalInterests, .ListId, cloneAI:=False, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First, matchType:=QuickQuoteHelperClass.TextMatchType.IntegerOrText_IgnoreCasing, returnNewIfNothing:=False)
                                If sourceAI IsNot Nothing Then
                                    QQHelper.CopyQuickQuoteAdditionalInterestNameAddressEmailsAndPhones(sourceAI, aiToUse)
                                End If
                            End If
                            'End If
                        End With
                    End If
                Else 'added 6/8/2017
                    QQHelper.DisposeAdditionalInterests(MyItem.AdditionalInterests)
                End If

                '6/7/2017 note on AIs: this control will just work w/ the specific Contractors Equipment AIs that it needs from the Building AIs; the full list of Building AIs is still in tact; these Contractors Equipment AIs are copied back and forth from the Building AIs in the QuickQuote library on Retrieval and Save
                '6/7/2017 note on ContractorsEquipmentScheduledItems (list) and ContractorsToolsEquipmentScheduled (total amount) properties: the QuickQuote library will now sync up both properties, so you should only need to use one or the other... ContractorsToolsEquipmentScheduled can be used from the Quote side to set the total amount, and ContractorsEquipmentScheduledItems should be the only one used from here (App side)

                'removed 6/7/2017 since we don't need to set ContractorsToolsEquipmentScheduled from the App side; everything is done in the list, and ContractorsToolsEquipmentScheduled will always reflect the total from there
                '' Check to see if we need to update the contractors equipment total limit - it can't be less than the total of all items
                'For Each ce As QuickQuote.CommonObjects.QuickQuoteContractorsEquipmentScheduledItem In Quote.ContractorsEquipmentScheduledItems
                '    If IsNumeric(ce.Limit) Then tot += CDec(ce.Limit)
                'Next
                'If IsNumeric(Quote.ContractorsToolsEquipmentScheduled) Then
                '    If CDec(Quote.ContractorsToolsEquipmentScheduled) < tot Then
                '        Quote.ContractorsToolsEquipmentScheduled = tot
                '    End If
                'Else
                '    Quote.ContractorsToolsEquipmentScheduled = tot
                'End If

                RaiseEvent ContractorsItemChanged(ItemIndex)
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
            'ddlValuationMethod.SelectedIndex = 0
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

    'added 6/7/2017
    Private Sub SetContractorsEquipmentAdditionalInterestFieldsFromAI(ByVal ceAI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest, Optional ByVal setLossPayeeName As Boolean = True)
        If ceAI IsNot Nothing Then
            If setLossPayeeName = True Then
                If QQHelper.IsPositiveIntegerString(ceAI.ListId) = True Then
                    If Me.ddlLossPayeeName.Items IsNot Nothing Then
                        If Me.ddlLossPayeeName.Items.Count > 0 AndAlso Me.ddlLossPayeeName.Items.FindByValue(ceAI.ListId) IsNot Nothing Then
                            Me.ddlLossPayeeName.SelectedValue = ceAI.ListId
                        Else
                            'add name to dropdown and set

                        End If
                    End If
                End If
            End If
            If QQHelper.IsPositiveIntegerString(ceAI.TypeId) = True Then
                If Me.ddlLossPayeeType.Items IsNot Nothing Then
                    If Me.ddlLossPayeeType.Items.Count > 0 AndAlso Me.ddlLossPayeeType.Items.FindByValue(ceAI.TypeId) IsNot Nothing Then
                        Me.ddlLossPayeeType.SelectedValue = ceAI.TypeId
                    Else
                        'add type to dropdown and set

                    End If
                End If
            End If
            If ceAI.ATIMA = True AndAlso ceAI.ISAOA = True Then
                Me.ddlATIMA.SelectedValue = "3"
            ElseIf ceAI.ATIMA = True Then
                Me.ddlATIMA.SelectedValue = "1"
            ElseIf ceAI.ISAOA = True Then
                Me.ddlATIMA.SelectedValue = "2"
            Else 'ElseIf ceAI.ATIMA = False AndAlso ceAI.ISAOA = False Then
                Me.ddlATIMA.SelectedValue = "0"
            End If
        End If
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
        RaiseEvent ContractorsItemChanged(ItemIndex)
    End Sub

#End Region


End Class