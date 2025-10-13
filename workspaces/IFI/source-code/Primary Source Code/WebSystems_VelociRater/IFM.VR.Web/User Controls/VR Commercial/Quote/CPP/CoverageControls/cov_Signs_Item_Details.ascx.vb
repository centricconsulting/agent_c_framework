Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.PrimativeExtensions
Public Class cov_Signs_Item_Details
    Inherits VRControlBase

    Public Property LocationIndex As Integer
        Get
            If ViewState("vs_sidlocIndex") IsNot Nothing Then
                Return CInt(ViewState("vs_sidlocIndex"))
            End If
            Return 0
        End Get
        Set(value As Integer)
            ViewState("vs_sidlocIndex") = value
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
            If Session("si_selectedOptionList_" + Quote.Database_QuoteId) IsNot Nothing Then
                'Return selectedOptionList
                Return CType(Session("si_selectedOptionList_" + Quote.Database_QuoteId), List(Of String))
            End If
            Return New List(Of String)
        End Get
        Set(value As List(Of String))
            Session("si_selectedOptionList_" + Quote.Database_QuoteId) = value
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
                Dim Index As Integer = InputID.LastIndexOf("cov_Signs_Item_Details")
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

    Private ReadOnly Property SignCheckbox As CheckBox
        Get
            Dim ceBox As CheckBox = Parent.FindControl("chkApply")
            Return ceBox
        End Get
    End Property


    Public Event AddSignsPolicyItems(ThisState As QuickQuoteObject)

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If Me.MyBuilding IsNot Nothing Then
            If Me.MyBuilding.ScheduledSigns Is Nothing Then
                Me.MyBuilding.ScheduledSigns = New List(Of QuickQuoteScheduledSign)()
            End If
            If Me.MyBuilding.ScheduledSigns.Any() = False Then
                Me.MyBuilding.ScheduledSigns.Add(New QuickQuoteScheduledSign())
                Me.MyBuilding.ScheduledSigns(0).Limit = ""
                Me.MyBuilding.ScheduledSigns(0).Description = String.Format("Signs #{0}", Me.MyBuilding.ScheduledSigns.Count)
                initSelectedOptionList()
            End If


            Me.sidRepeater.DataSource = Me.MyBuilding.ScheduledSigns
            Me.sidRepeater.DataBind()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean

        Dim signList As New List(Of QuickQuoteScheduledSign)
        Dim chkApply As CheckBox = Parent.FindControl("chkApply")
        selectedOption = chkApply.Checked

        Me.MyBuilding.ScheduledSigns = signList

        If selectedOption Then
            For Each ri As RepeaterItem In sidRepeater.Items
                Dim sign As New QuickQuoteScheduledSign
                Dim txtLimit As TextBox = ri.FindControl("txtLimit")
                Dim txtDescription As TextBox = ri.FindControl("txtLocation")
                Dim location As LinkButton = ri.FindControl("btnDelete")
                Dim insideSign As CheckBox = ri.FindControl("chkInsideSign")
                Dim itemIndex = ri.ItemIndex

                'Using the delete button's command argument for Location and Building info to get MyBuilding
                setLocationIndexes(location.CommandArgument.ToString)

                sign.Limit = txtLimit.Text.Trim()
                sign.IsIndoor = insideSign.Checked
                sign.Description = txtDescription.Text.Trim()
                signList.Add(sign)
            Next
            Me.MyBuilding.ScheduledSigns = signList

            Dim stateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForLocation(Quote.Locations(LocationIndex))
            If signList.Count > 0 Then
                RaiseEvent AddSignsPolicyItems(stateQuote)
            End If
        End If


        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Me.ValidationHelper.GroupName = "Signs"
        Dim deductibleText As String = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FineArtsDeductibleId, GoverningStateQuote.SignsDeductibleId)
        Dim deductibleAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(deductibleText)
        Dim chkApply As CheckBox = Parent.FindControl("chkApply")

        For Each ri As RepeaterItem In sidRepeater.Items
            Dim txtLimit As TextBox = ri.FindControl("txtLimit")
            Dim LimitAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtLimit.Text)
            Dim txtLocation As TextBox = ri.FindControl("txtLocation")
            Dim ceValuation As DropDownList = ri.FindControl("ceValuation")

            '3.8.92
            If String.IsNullOrEmpty(txtLimit.Text) AndAlso chkApply.Checked Then
                Me.ValidationHelper.AddError("Missing Limit", Path + txtLimit.ClientID)
            End If
            '3.8.94
            If String.IsNullOrEmpty(txtLocation.Text) AndAlso chkApply.Checked Then
                Me.ValidationHelper.AddError("Missing  Description", Path + txtLocation.ClientID)
            End If
            ' 3.8.122
            If LimitAmount > 100000 AndAlso chkApply.Checked Then
                Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", Path + txtLimit.ClientID)
            End If
            ' 3.8.114
            If deductibleAmount >= LimitAmount AndAlso chkApply.Checked Then
                Me.ValidationHelper.AddError("Deductible amount selected is equal or greater than the Limit. Please adjust either value.", Path + txtLimit.ClientID)
            End If
        Next

        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub btnAddClick()
        If Me.Quote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Dim newItem As New QuickQuoteScheduledSign()
            newItem.Description = String.Format("Signs #{0}", Me.MyBuilding.ScheduledSigns.Count)
            Me.MyBuilding.ScheduledSigns.Add(newItem)
            Populate()
            Me.Save_FireSaveEvent(False)
        End If
    End Sub

    Private Sub btnDeleteClick(index, ItemCount)
        If Me.Quote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Me.MyBuilding.ScheduledSigns.RemoveAt(index)
            Populate()
            Me.Save_FireSaveEvent(False)
        End If
    End Sub

    Protected Sub sidRepeater_Add(source As Object, e As RepeaterCommandEventArgs) Handles sidRepeater.ItemCommand
        setLocationIndexes(e.CommandArgument.ToString)
        If e.CommandName = "lnkAdd" Then
            'ClearControl()
            btnAddClick()
        ElseIf e.CommandName = "lnkDelete" Then
            btnDeleteClick(e.Item.ItemIndex, sidRepeater.Items.Count)
        End If
    End Sub

    Private Sub sidRepeater_item(sender As Object, e As RepeaterItemEventArgs) Handles sidRepeater.ItemDataBound

        Dim txtLimit As TextBox = e.Item.FindControl("txtLimit")
        Dim txtLocation As TextBox = e.Item.FindControl("txtLocation")

        Dim chkApply As CheckBox = Parent.FindControl("chkApply")

        If (txtLimit IsNot Nothing) AndAlso (String.IsNullOrWhiteSpace(txtLimit.Text) = False) OrElse selectedOption Then
            chkApply.Checked = True
        End If


    End Sub

    Public Overrides Sub ClearControl()
        If MyBuilding IsNot Nothing Then
            Me.MyBuilding.ScheduledSigns = Nothing
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