Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects

Public Class cov_FineArtsFloater_Item_Details
    Inherits VRControlBase

    Public Property LocationIndex As Integer
        Get
            If ViewState("vs_fadlocIndex") IsNot Nothing Then
                Return CInt(ViewState("vs_fadlocIndex"))
            End If
            Return 0
        End Get
        Set(value As Integer)
            ViewState("vs_fadlocIndex") = value
        End Set
    End Property

    Public Property BuildingIndex As Integer
        Get
            If ViewState("vs_fadbuildingIndex") IsNot Nothing Then
                Return CInt(ViewState("vs_fadbuildingIndex"))
            End If
            Return 0
        End Get
        Set(value As Integer)
            ViewState("vs_fadbuildingIndex") = value
        End Set
    End Property

    Public Property selectedOptionList As List(Of String)
        Get
            If Session("fa_selectedOptionList_" + Quote.Database_QuoteId) IsNot Nothing Then
                'Return selectedOptionList
                Return CType(Session("fa_selectedOptionList_" + Quote.Database_QuoteId), List(Of String))
            End If
            Return New List(Of String)
        End Get
        Set(value As List(Of String))
            Session("fa_selectedOptionList_" + Quote.Database_QuoteId) = value
        End Set
    End Property

    Private Property selectedOption As Boolean
        Get
            If selectedOptionList Is Nothing Then
                selectedOptionList = New List(Of String)
            End If
            If selectedOptionList.Count > 0 Then
                For Each item As String In selectedOptionList
                    If item = LocationIndex.ToString + "|" + BuildingIndex.ToString Then
                        Return True
                    End If
                Next
            End If
            Return False
        End Get
        Set(value As Boolean)
            If selectedOptionList Is Nothing Then
                selectedOptionList = New List(Of String)
            End If
            If value Then
                If selectedOptionList.Contains(LocationIndex.ToString + "|" + BuildingIndex.ToString) = False Then
                    Dim updatedList As List(Of String) = selectedOptionList
                    updatedList.Add(LocationIndex.ToString + "|" + BuildingIndex.ToString)
                    selectedOptionList = updatedList
                End If
            Else
                If selectedOptionList.Contains(LocationIndex.ToString + "|" + BuildingIndex.ToString) Then
                    Dim updatedList As List(Of String) = selectedOptionList
                    updatedList.Remove(LocationIndex.ToString + "|" + BuildingIndex.ToString)
                    selectedOptionList = updatedList
                End If
            End If
        End Set
    End Property

    Public Property Path As String
        Get
            If ViewState("vs_Path") IsNot Nothing Then
                Dim InputID = CStr(ViewState("vs_Path"))
                Dim Index As Integer = InputID.LastIndexOf("cov_FineArtsFloater_Item_Details")
                If (Index > 0) Then
                    InputID = InputID.Substring(0, Index)
                End If
                Return InputID
                'Return CStr(ViewState("vs_Path"))
            End If
            Return 0
        End Get
        Set(value As String)
            ViewState("vs_Path") = value
        End Set
    End Property

    Private ReadOnly Property MyBuilding As QuickQuoteBuilding
        Get
            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing Then
                If Quote.Locations.Count > LocationIndex Then
                    If Quote.Locations(LocationIndex).Buildings.Count > BuildingIndex Then
                        Return Quote.Locations(LocationIndex).Buildings(BuildingIndex)
                    End If
                End If
            End If
            Return Nothing
        End Get
    End Property

    Private Property LocationCheckbox As Boolean
        Get
            Dim ceBox As CheckBox = Parent.FindControl("chkApply")
            Return ceBox.Checked
        End Get
        Set(value As Boolean)
            Dim ceBox As CheckBox = Parent.FindControl("chkApply")
            ceBox.Checked = value
        End Set
    End Property

    Public Event AddFineArtsPolicyItems(ThisState As QuickQuoteObject)

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If Me.MyBuilding IsNot Nothing Then
            If Me.MyBuilding.FineArtsScheduledItems Is Nothing Then
                Me.MyBuilding.FineArtsScheduledItems = New List(Of QuickQuoteFineArtsScheduledItem)()
            End If
            If Me.MyBuilding.FineArtsScheduledItems.Any() = False Then
                Me.MyBuilding.FineArtsScheduledItems.Add(New QuickQuoteFineArtsScheduledItem())
                Me.MyBuilding.FineArtsScheduledItems(0).Limit = ""
                Me.MyBuilding.FineArtsScheduledItems(0).Description = String.Format("Fine Arts #{0}", Me.MyBuilding.FineArtsScheduledItems.Count)
                initSelectedOptionList()
            Else

            End If

            Me.fadRepeater.DataSource = Me.MyBuilding.FineArtsScheduledItems
            Me.fadRepeater.DataBind()

            Dim stateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForLocation(Quote.Locations(LocationIndex))
            txtTotalLimit.Text = stateQuote.FineArtsBuildingsTotalLimit

        End If

    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean

        Dim artList As New List(Of QuickQuoteFineArtsScheduledItem)
        Dim totalLimit As Double = 0.0
        Dim delete As LinkButton = New LinkButton()
        Dim chkApply As CheckBox = Parent.FindControl("chkApply")
        selectedOption = chkApply.Checked

        Me.MyBuilding.FineArtsScheduledItems = artList

        If selectedOption Then
            For Each ri As RepeaterItem In fadRepeater.Items
                Dim art As New QuickQuoteFineArtsScheduledItem
                Dim txtLimit As TextBox = ri.FindControl("txtLimit")
                Dim txtDescription As TextBox = ri.FindControl("txtLocation")
                delete = ri.FindControl("btnDelete")
                Dim LimitAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtLimit.Text)

                art.Limit = txtLimit.Text
                art.Description = txtDescription.Text
                artList.Add(art)

                totalLimit += LimitAmount
            Next

            'Using the delete button's command argument for Location and Building info to get MyBuilding
            setLocationIndexes(delete.CommandArgument.ToString)
            Me.MyBuilding.FineArtsScheduledItems = artList

            Dim stateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForLocation(Quote.Locations(LocationIndex))

            Dim allLimits = stateQuote.FineArtsBuildingsTotalLimit
            If stateQuote.FineArtsBreakageMarringOrScratching Then
                Dim Rate = CIMHelper.FineArtsRateTable.GetRateForLimit(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(allLimits))
                stateQuote.FineArtsRate = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Rate) > 0.0, (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Rate) + 0.1).ToString(), 0.0)
            Else
                stateQuote.FineArtsRate = CIMHelper.FineArtsRateTable.GetRateForLimit(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(allLimits))
            End If


            ' ----- Write HasPackagePart and Policysettings for Sub Quotes
            If artList.Count > 0 Then
                RaiseEvent AddFineArtsPolicyItems(stateQuote)
            End If
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Me.ValidationHelper.GroupName = "Fine Arts Floater"
        Dim deductibleText As String = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FineArtsDeductibleId, GoverningStateQuote.FineArtsDeductibleId)
        Dim deductibleAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(deductibleText)
        Dim chkApply As CheckBox = Parent.FindControl("chkApply")

        If chkApply.Checked Then
            For Each ri As RepeaterItem In fadRepeater.Items
                Dim txtLimit As TextBox = ri.FindControl("txtLimit")
                Dim LimitAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtLimit.Text)
                Dim txtLocation As TextBox = ri.FindControl("txtLocation")
                Dim ceValuation As DropDownList = ri.FindControl("ceValuation")
                If String.IsNullOrEmpty(txtLimit.Text) Then
                    Me.ValidationHelper.AddError("Missing Limit", Path + txtLimit.ClientID)
                End If
                If String.IsNullOrEmpty(txtLocation.Text) Then
                    Me.ValidationHelper.AddError("Missing  Description", Path + txtLocation.ClientID)
                End If
                ' 3.8.122
                If LimitAmount > 50000 And chkApply.Checked Then
                    Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", Path + txtLimit.ClientID)
                End If
                ' 3.8.114
                If deductibleAmount >= LimitAmount And chkApply.Checked Then
                    Me.ValidationHelper.AddError("Deductible amount selected is equal or greater than the Limit. Please adjust either value.", Path + txtLimit.ClientID)
                End If
            Next

        End If

        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub btnAddClick()
        If Me.Quote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Dim newItem As New QuickQuoteFineArtsScheduledItem()
            newItem.Description = String.Format("Fine Arts #{0}", Me.MyBuilding.FineArtsScheduledItems.Count)
            Me.MyBuilding.FineArtsScheduledItems.Add(newItem)

            'Me.MyBuilding.FineArtsScheduledItems.AddNew()
            Populate()
            Me.Save_FireSaveEvent(False)
        End If
    End Sub

    Private Sub btnDeleteClick(index)
        If Me.Quote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Me.MyBuilding.FineArtsScheduledItems.RemoveAt(index)
            Populate()
            Me.Save_FireSaveEvent(False)
        End If
    End Sub

    Protected Sub fadRepeater_Add(source As Object, e As RepeaterCommandEventArgs) Handles fadRepeater.ItemCommand
        setLocationIndexes(e.CommandArgument.ToString)
        If e.CommandName = "lnkAdd" Then
            'ClearControl()
            btnAddClick()
        ElseIf e.CommandName = "lnkDelete" Then
            btnDeleteClick(e.Item.ItemIndex)
        End If
    End Sub

    Private Sub fadRepeater_item(sender As Object, e As RepeaterItemEventArgs) Handles fadRepeater.ItemDataBound

        Dim txtLimit As TextBox = e.Item.FindControl("txtLimit")
        Dim txtLocation As TextBox = e.Item.FindControl("txtLocation")

        Dim chkApply As CheckBox = Parent.FindControl("chkApply")

        If (txtLimit IsNot Nothing) AndAlso (String.IsNullOrWhiteSpace(txtLimit.Text) = False) OrElse selectedOption Then
            chkApply.Checked = True
        End If

    End Sub

    Public Overrides Sub ClearControl()
        If Quote.Locations.HasItemAtIndex(LocationIndex) Then
            If MyBuilding IsNot Nothing Then
                Me.MyBuilding.FineArtsScheduledItems = New List(Of QuickQuoteFineArtsScheduledItem)()
            End If
            Dim stateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForLocation(Quote.Locations(LocationIndex))
            If stateQuote IsNot Nothing Then
                stateQuote.FineArtsRate = ""
            End If
        End If

        initSelectedOptionList()

        Dim chkApply As CheckBox = Parent.FindControl("chkApply")
        If LocationIndex = 0 And BuildingIndex = 0 Then
            chkApply.Checked = True
        End If

    End Sub

    Protected Sub setLocationIndexes(settings As String)
        Dim arrSettings As String()
        arrSettings = settings.Split("|")
        If arrSettings.Count = 2 Then
            LocationIndex = arrSettings(0)
            BuildingIndex = arrSettings(1)
        End If
    End Sub

    Protected Sub initSelectedOptionList()
        Dim updatedList As List(Of String) = selectedOptionList
        updatedList = New List(Of String)
        'updatedList.Add("0|0")
        selectedOptionList = updatedList
    End Sub

End Class