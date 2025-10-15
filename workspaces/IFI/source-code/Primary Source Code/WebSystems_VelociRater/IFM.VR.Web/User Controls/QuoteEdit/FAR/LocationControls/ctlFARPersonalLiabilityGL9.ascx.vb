Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctlFARPersonalLiabilityGL9
    Inherits VRControlBase

#Region "Declarations"

    Private Structure MyRepeaterItemControls_structure
        Public tblPersonalLiabilityInfo As HtmlTable
        Public txtFirstName As TextBox
        Public txtSectionIIIndex As TextBox
        Public txtLastName As TextBox
        Public lbEditAddress As LinkButton
        Public lbDelete As LinkButton
        Public divAddressInfo As HtmlGenericControl
        Public tblAddress As HtmlTable
        Public txtStreetNumber As TextBox
        Public txtStreetName As TextBox
        Public txtAptSuiteNumber As TextBox
        Public txtZip As TextBox
        Public txtCity As TextBox
        Public ddCity As DropDownList
        Public ddState As DropDownList
        Public btnOK As Button
        Public btnCancel As Button
        Public txtCounty As TextBox
    End Structure

    Private Property AddNewRecord As Boolean
        Get
            If ViewState("vs_AddNewRecord") Is Nothing Then
                ViewState("vs_AddNewRecord") = False
            End If
            Return CBool(ViewState("vs_AddNewRecord"))
        End Get
        Set(value As Boolean)
            ViewState("vs_AddNewRecord") = value
        End Set
    End Property

    Private Property ControlIndexesWithValidationErrors As String
        Get
            Return ViewState("vs_ErrIndexes")
        End Get
        Set(value As String)
            ViewState("vs_ErrIndexes") = value
        End Set
    End Property

    Public Property MyLocationIndex As Integer
        Get
            Return ViewState("vs_MyLocationIndex")
        End Get
        Set(value As Integer)
            ViewState("vs_MyLocationIndex") = value
        End Set
    End Property

    Private Property MyLocation As QuickQuoteLocation
        Get
            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(MyLocationIndex) Then
                Return Quote.Locations(MyLocationIndex)
            Else
                Return Nothing
            End If
        End Get
        Set(value As QuickQuoteLocation)
            Quote.Locations(MyLocationIndex) = value
        End Set
    End Property

    Private ReadOnly Property QuoteHasInlandMarine As Boolean
        Get
            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                If Quote.Locations(0).InlandMarines IsNot Nothing Then
                    If Quote.Locations(0).InlandMarines.Count > 0 Then
                        Return True
                    Else
                        Return False
                    End If
                End If
            End If
            Return False
        End Get
    End Property

    Private ReadOnly Property QuoteHasRVWatercraft As Boolean
        Get
            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                If Quote.Locations(0).RvWatercrafts IsNot Nothing Then
                    If Quote.Locations(0).RvWatercrafts.Count > 0 Then
                        Return True
                    Else
                        Return False
                    End If
                End If
            End If
            Return False
        End Get
    End Property

    Public Event GL9Changed(ByVal ClearIMRV As Boolean)

#End Region

#Region "Methods and Functions"

#Region "VRControlBase Methods"

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Dim itemnum As Integer = 0
        Dim ErrIndexes As String = ""

        ControlIndexesWithValidationErrors = ""

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        If hdnCheckboxValue.Value = True Then
            For Each ri As RepeaterItem In rptPersonalLiabilityItems.Items
                itemnum += 1
                Dim FoundError As Boolean = False
                Me.ValidationHelper.GroupName = "Personal Liability Coverage (GL-9)"
                Dim riControls As MyRepeaterItemControls_structure = GetRepeaterControls(ri)

                ' First Name, Last Name, street, city, state, zip are all required
                If riControls.txtFirstName.Text.Trim = String.Empty Then
                    FoundError = True
                    Me.ValidationHelper.AddError(riControls.txtFirstName, "Missing First Name", accordList)
                End If

                If IsQuoteEndorsement() = False Then
                    If riControls.txtLastName.Text.Trim = String.Empty Then
                        FoundError = True
                        Me.ValidationHelper.AddError(riControls.txtLastName, "Missing Last Name", accordList)
                    End If
                    If riControls.txtStreetNumber.Text.Trim = String.Empty Then
                        FoundError = True
                        Me.ValidationHelper.AddError(riControls.txtStreetNumber, "Missing House Number", accordList)
                    End If
                    If riControls.txtStreetName.Text.Trim = String.Empty Then
                        FoundError = True
                        Me.ValidationHelper.AddError(riControls.txtStreetName, "Missing Street Name", accordList)
                    End If
                    If riControls.txtCity.Text.Trim = String.Empty Then
                        FoundError = True
                        Me.ValidationHelper.AddError(riControls.txtCity, "Missing City", accordList)
                    End If
                    If riControls.ddState.SelectedIndex <= 0 Then
                        FoundError = True
                        Me.ValidationHelper.AddError(riControls.ddState, "Missing State", accordList)
                    End If
                    If riControls.txtZip.Text.Trim = String.Empty Then
                        FoundError = True
                        Me.ValidationHelper.AddError(riControls.txtZip, "Missing Zip Code", accordList)
                    End If
                End If

                If FoundError Then
                    If ErrIndexes = "" Then
                        ErrIndexes += (itemnum - 1).ToString
                    Else
                        ErrIndexes += "," & (itemnum - 1).ToString
                    End If
                End If
            Next
        End If

        ControlIndexesWithValidationErrors = ErrIndexes

        'Me.ValidateChildControls(valArgs)  ' this guy has no child controls.  If you add any uncomment this line.

        Exit Sub
    End Sub

    Public Overrides Sub Populate()
        Dim SCtable As New List(Of IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.IFMSectionII_structure)
        Dim SCNdx As Integer = -1
        Dim dispNdx As Integer = -1
        Dim FoundAGL9 As Boolean = False
        Dim cnt As Integer = 0

        If MyLocation Is Nothing Then Exit Sub
        If MyLocation.SectionIICoverages Is Nothing Then MyLocation.SectionIICoverages = New List(Of QuickQuoteSectionIICoverage)

        chkPersonalLiabilityCoverage.Checked = False
        divPersonalLiabilityData.Attributes.Add("style", "display:none")

        'UpdateClearButtonConfirmation()

        ' Build the table of location GL9 coverages that we'll bind to the repeater
        SCtable = IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.GetAllLocationGL9s(MyLocation, cnt)
        If cnt > 0 Then FoundAGL9 = True

        If AddNewRecord Then
            ' If we are adding a new record, do it here.
            ' Add a blank record to the table we're binding to the repeater.
            Dim newDataItem As New IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.IFMSectionII_structure
            newDataItem.SectionIndex = -1
            newDataItem.SectionIIItem = New QuickQuoteSectionIICoverage()
            newDataItem.SectionIIItem.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9
            SCtable.Add(newDataItem)
        Else
            ' If we found a GL-9 coverage check the checkbox and display the data section
            If FoundAGL9 Then
                DisableAndCheckGL9Checkbox()
                divPersonalLiabilityData.Attributes.Add("style", "display:''")
            Else
                ' No Personal Liability (GL-9) coverages found, create a blank record for display
                Dim newTableItem As New IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.IFMSectionII_structure()
                Dim newSC As New QuickQuote.CommonObjects.QuickQuoteSectionIICoverage()
                newSC.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9
                newTableItem.SectionIIItem = newSC
                newTableItem.SectionIndex = -1
                SCtable.Add(newTableItem)
                EnableAndUncheckGL9Checkbox()
            End If
        End If

        ' Bind the data table to the repeater
        rptPersonalLiabilityItems.DataSource = SCtable
        rptPersonalLiabilityItems.DataBind()

        ' Update each repeater's delete button confirmations
        'UpdateRepeaterDeleteButtonConfirmations()
        'UpdateClearButtonConfirmation()

        Exit Sub
    End Sub

    Public Overrides Function Save() As Boolean
        Dim RepeaterNdx As Integer = -1
        Dim ndx As Integer = 0
        Dim MyCov As QuickQuoteSectionIICoverage = Nothing

        divPersonalLiabilityData.Attributes.Add("style", "display:none")

        If hdnCheckboxValue.Value = True Then
            For Each ri As RepeaterItem In rptPersonalLiabilityItems.Items
                RepeaterNdx += 1
                Dim riControls As MyRepeaterItemControls_structure = GetRepeaterControls(ri)
                Dim txt As String = ""

                MyCov = Nothing

                If IsNumeric(riControls.txtSectionIIIndex.Text) Then
                    ndx = CInt(riControls.txtSectionIIIndex.Text.ToString)
                Else
                    Throw New Exception("Personal Liability index is invalid")
                End If

                chkPersonalLiabilityCoverage.Checked = True

                ' UPDATE EACH COVERAGE
                If MyLocation Is Nothing Then Throw New Exception("MyLocation is nothing!!")
                If MyLocation.SectionIICoverages Is Nothing Then MyLocation.SectionIICoverages = New List(Of QuickQuoteSectionIICoverage)

                If riControls.txtSectionIIIndex.Text = "-1" Then
                    ' Empty record.  Create a new GL-9 on the object if no validation errors
                    Dim HasData As Boolean = GL9HasAnyValidFields(ri)
                    ' If no errors found on the current repeater item, save it
                    If HasData Then
                        MyLocation.SectionIICoverages.AddNew()
                        ndx = MyLocation.SectionIICoverages.Count - 1
                        riControls.txtSectionIIIndex.Text = ndx
                        MyCov = MyLocation.SectionIICoverages(ndx)
                        MyCov.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9
                    End If
                Else
                    If MyLocation.SectionIICoverages(ndx).CoverageType <> QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9 Then Throw New Exception("The coverage at Location(0).SectionCoverages(" & ndx.ToString & ") is not the expected coverage type of Personal Liability GL-9")
                    MyCov = MyLocation.SectionIICoverages(ndx)
                End If

                ' Copy the values from the page to the object
                If MyCov IsNot Nothing Then
                    If MyCov.Name Is Nothing Then MyCov.Name = New QuickQuoteName
                    If MyCov.Name.FirstName <> riControls.txtFirstName.Text Then MyCov.Name.FirstName = riControls.txtFirstName.Text
                    If MyCov.Name.LastName <> riControls.txtLastName.Text Then MyCov.Name.LastName = riControls.txtLastName.Text
                    If MyCov.Address Is Nothing Then MyCov.Address = New QuickQuoteAddress()
                    If MyCov.Address.HouseNum <> riControls.txtStreetNumber.Text Then MyCov.Address.HouseNum = riControls.txtStreetNumber.Text
                    If MyCov.Address.StreetName <> riControls.txtStreetName.Text Then MyCov.Address.StreetName = riControls.txtStreetName.Text
                    If MyCov.Address.City <> riControls.txtCity.Text Then MyCov.Address.City = riControls.txtCity.Text
                    If MyCov.Address.StateId <> riControls.ddState.SelectedValue Then MyCov.Address.StateId = riControls.ddState.SelectedValue
                    If MyCov.Address.Zip <> riControls.txtZip.Text Then MyCov.Address.Zip = riControls.txtZip.Text
                End If

                divPersonalLiabilityData.Attributes.Add("style", "display:''")
            Next
        Else
            ' Checkbox is not checked - Remove all Personal Liability coverages from the current location
            Dim scndx As Integer = -1
scloop:
            ' CAH 09/21/2020 - This could be null
            If MyLocation.SectionIICoverages IsNot Nothing Then
                For Each SC As QuickQuoteSectionIICoverage In MyLocation.SectionIICoverages
                    scndx += 1
                    If SC.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9 Then
                        MyLocation.SectionIICoverages.Remove(SC)
                        GoTo scloop
                    End If
                Next
            End If
            'For Each SC As QuickQuoteSectionIICoverage In MyLocation.SectionIICoverages
            '    scndx += 1
            '    If SC.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9 Then
            '        MyLocation.SectionIICoverages.Remove(SC)
            '        GoTo scloop
            '    End If
            'Next

            ' Now add an empty GL-9 to the repeater so the UI will work correctly
            If MyLocation.SectionIICoverages Is Nothing Then MyLocation.SectionIICoverages = New List(Of QuickQuoteSectionIICoverage)
            Dim SCTable As New List(Of IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.IFMSectionII_structure)
            scndx = MyLocation.SectionIICoverages.Count    ' We're adding a record here so the new index will be the current 1-based count
            Dim newTableItem As New IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.IFMSectionII_structure()
            Dim newSC As New QuickQuote.CommonObjects.QuickQuoteSectionIICoverage()
            newSC.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9
            newTableItem.SectionIIItem = newSC
            newTableItem.SectionIndex = -1  ' blank records alway have a section ID of -1!!
            SCTable.Add(newTableItem)

            ' Now that we've removed all GL-9s from this location, rebind the repeater
            rptPersonalLiabilityItems.DataSource = SCTable
            rptPersonalLiabilityItems.DataBind()
        End If

        ' Since the data changed we need to update the confirmations on the Delete and Clear buttons
        UpdateRepeaterDeleteButtonConfirmations()
        UpdateClearButtonConfirmation()

        Return True
    End Function

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()

        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, hdnAccord, "0")
        Me.VRScript.StopEventPropagation(Me.lnkSavePersonalLiability.ClientID)

        ' The info panel for the GL-9 link
        lbPersonalLiabilityCoverage.Attributes.Add("onclick", "ShowPersLiabPopup('" & dvPersLiabPopup.ClientID & "'); return false;")

        ' The OK button on the personal liability popup
        btnPLOK.Attributes.Add("onclick", "ClosePersLiabPopup('" & dvPersLiabPopup.ClientID & "'); return false;")

        UpdateGL9CheckboxScript()

        ' Bindings for controls in the repeater items
        For Each ri As RepeaterItem In rptPersonalLiabilityItems.Items
            Dim divAddressInfo As HtmlGenericControl = ri.FindControl("divAddressInfo")
            Dim lbEdit As LinkButton = ri.FindControl("lbEditAddress")
            Dim txtZip As TextBox = ri.FindControl("txtZip")
            Dim txtCity As TextBox = ri.FindControl("txtCity")
            Dim ddCity As DropDownList = ri.FindControl("ddCity")
            Dim txtCounty As TextBox = ri.FindControl("txtCounty")
            Dim ddState As DropDownList = ri.FindControl("ddState")
            Dim lbDelete As LinkButton = ri.FindControl("lbDelete")

            If lbEdit IsNot Nothing Then VRScript.CreateJSBinding(lbEdit.ClientID, "onclick", "HandleEditAddressClicks('" & divAddressInfo.ClientID & "','" & lbEdit.ClientID & "');")
            If txtZip IsNot Nothing AndAlso ddCity IsNot Nothing AndAlso txtCity IsNot Nothing AndAlso txtCounty IsNot Nothing AndAlso ddState IsNot Nothing Then Me.VRScript.CreateJSBinding(txtZip, ctlPageStartupScript.JsEventType.onkeyup, "DoCityCountyLookup('" + txtZip.ClientID + "','" + ddCity.ClientID + "','" + txtCity.ClientID + "','" + txtCounty.ClientID + "','" + ddState.ClientID + "');")
        Next

        Exit Sub
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

#End Region

    Private Function GL9HasAnyValidFields(ByVal ri As RepeaterItem) As Boolean
        If ri Is Nothing Then Return False
        Dim riControls As New MyRepeaterItemControls_structure
        riControls = GetRepeaterControls(ri)

        If riControls.txtFirstName.Text.Trim <> "" OrElse riControls.txtLastName.Text.Trim <> "" OrElse riControls.txtStreetNumber.Text.Trim <> "" _
            OrElse riControls.txtStreetName.Text.Trim <> "" OrElse riControls.txtZip.Text.Trim <> "" OrElse riControls.txtCity.Text.Trim <> "" Then
            Return True
        End If

        Return False
    End Function

    Private Sub SetCheckbox(ByVal Checked As Boolean)
        If Checked Then
            chkPersonalLiabilityCoverage.Checked = True
            hdnCheckboxValue.Value = True
        Else
            chkPersonalLiabilityCoverage.Checked = False
            hdnCheckboxValue.Value = False
        End If
    End Sub

    Private Sub EnableAndUncheckGL9Checkbox()
        SetCheckbox(False)
        chkPersonalLiabilityCoverage.Attributes.Remove("disabled")
        Exit Sub
    End Sub

    Private Sub DisableAndCheckGL9Checkbox()
        SetCheckbox(True)
        chkPersonalLiabilityCoverage.Attributes.Add("disabled", "true")
    End Sub

    Private Sub UpdateGL9CheckboxScript()
        Dim rvExists As Boolean = QuoteHasRVWatercraft()
        Dim imExists As Boolean = QuoteHasInlandMarine()
        Dim FirstGL9AddressDiv As HtmlGenericControl = Nothing
        Dim FirstGL9HideShowAddressButton As LinkButton = Nothing
        Dim LastLocationWithGL9 As Boolean = IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.LocationIsTheOnlyLocationWithGL9(Quote, MyLocationIndex)

        ' Get the first repeater item controls
        If rptPersonalLiabilityItems.Items.Count > 0 Then
            Dim FirstGL9Controls As MyRepeaterItemControls_structure = GetRepeaterControls(rptPersonalLiabilityItems.Items(0))
            FirstGL9AddressDiv = FirstGL9Controls.divAddressInfo
            FirstGL9HideShowAddressButton = FirstGL9Controls.lbEditAddress

            ' Handle GL-9 checkbox clicks
            Me.VRScript.CreateJSBinding(chkPersonalLiabilityCoverage.ClientID, "onclick", "HandleGL9PersonalLiabilityCheckboxClicks('" & chkPersonalLiabilityCoverage.ClientID & "','" & divPersonalLiabilityData.ClientID & "','" & FirstGL9AddressDiv.ClientID & "','" & FirstGL9HideShowAddressButton.ClientID & "','" & rvExists.ToString & "','" & imExists.ToString & "','" & LastLocationWithGL9.ToString & "','" & hdnCheckboxValue.ClientID & "');")
        End If
    End Sub

    Private Function GetRepeaterControls(ByVal ri As RepeaterItem) As MyRepeaterItemControls_structure
        Dim MyControls As New MyRepeaterItemControls_structure()

        MyControls.tblPersonalLiabilityInfo = ri.FindControl("tblPersonalLiabilityInfo")
        MyControls.txtFirstName = ri.FindControl("txtFirstName")
        MyControls.txtSectionIIIndex = ri.FindControl("txtSectionIIIndex")
        MyControls.txtLastName = ri.FindControl("txtLastName")
        MyControls.lbEditAddress = ri.FindControl("lbEditAddress")
        MyControls.lbDelete = ri.FindControl("lbDelete")
        MyControls.divAddressInfo = ri.FindControl("divAddressInfo")
        MyControls.tblAddress = ri.FindControl("tblAddress")
        MyControls.txtStreetNumber = ri.FindControl("txtStreetNumber")
        MyControls.txtStreetName = ri.FindControl("txtStreetName")
        MyControls.txtAptSuiteNumber = ri.FindControl("txtAptSuiteNumber")
        MyControls.txtZip = ri.FindControl("txtZip")
        MyControls.txtCity = ri.FindControl("txtCity")
        MyControls.ddCity = ri.FindControl("ddCity")
        MyControls.ddState = ri.FindControl("ddState")
        MyControls.btnOK = ri.FindControl("btnOK")
        MyControls.btnCancel = ri.FindControl("btnCancel")
        MyControls.txtCounty = ri.FindControl("txtCounty")

        Return MyControls
    End Function

    Private Sub CollapseOrExpandAddressSection(ByRef LBEdit As LinkButton, ByRef AddressDiv As HtmlGenericControl, ByVal CollapseOrExpand As String)
        If LBEdit Is Nothing OrElse AddressDiv Is Nothing OrElse CollapseOrExpand Is Nothing OrElse (CollapseOrExpand.Trim = String.Empty) Then Exit Sub

        Select Case CollapseOrExpand.Substring(0, 1).ToUpper()
            Case "C"    ' Collapse
                AddressDiv.Attributes.Add("style", "display:none")
                LBEdit.Text = "Edit Address"
                Exit Select
            Case "E"    ' Expand
                AddressDiv.Attributes.Add("style", "display:''")
                LBEdit.Text = "Hide Address"
                Exit Select
            Case Else
                Exit Sub
        End Select
    End Sub

    ''' <summary>
    ''' Called externally when we update the GL-9's.  We need to update the text on the confirmations that are displayed when the Clear and Delete buttons are clicked
    ''' </summary>
    Public Sub UpdateGL9Confirmations()
        UpdateClearButtonConfirmation()
        UpdateRepeaterDeleteButtonConfirmations()
        UpdateGL9CheckboxScript()
        Exit Sub
    End Sub

    ''' <summary>
    ''' Updates the confirmation dialogs on the item delete links.  The text will change depending on how many GL9's are on the quote 
    ''' and whether the quote has Inland Marine and/or RV/Watercraft items
    ''' </summary>
    Private Sub UpdateRepeaterDeleteButtonConfirmations()
        Dim locGL9Count As Integer = IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.NumberOfGL9sOnALocation(MyLocation)
        Dim txt As String = ""

        For Each ri As RepeaterItem In rptPersonalLiabilityItems.Items
            Dim riControls As New MyRepeaterItemControls_structure

            riControls = GetRepeaterControls(ri)
            riControls.lbDelete.OnClientClick = Nothing

            If (Not RepeaterItemIsABlankRecord(riControls)) Then
                ' Set the Delete link confirmation text.  When this is the last GL-9 on the quote (not location) and the quote also has Inland Marine or 
                ' RV/Watercraft we need to warn the user that removing the final GL-9 will also drop any Inland Marine or RV/Watercraft items.
                If IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.NumberOfGL9sOnAQuote(Quote) = 1 Then
                    ' Last GL-9 on the quote
                    txt = ""
                    If QuoteHasInlandMarine AndAlso QuoteHasRVWatercraft Then
                        txt = "Deleting this coverage will also delete all Inland Marine and RV/Watercraft coverages and information on this quote. Are you sure you want to delete this item?"
                    ElseIf QuoteHasInlandMarine Then
                        txt = "Deleting this coverage will also delete all Inland Marine coverages and information on this quote. Are you sure you want to delete this item?"
                    ElseIf QuoteHasRVWatercraft Then
                        txt = "Deleting this coverage will also delete all RV/Watercraft coverages and information on this quote. Are you sure you want to delete this item?"
                    Else
                        ' No IM or RV
                        txt = "Are you sure you want to delete this item?"
                    End If
                Else
                    ' Not the last GL-9 on the quote
                    txt = "Are you sure you want to delete this item?"
                End If
                riControls.lbDelete.OnClientClick = "return confirm('" & txt & "');"
            End If
        Next
    End Sub

    ''' <summary>
    ''' Updates the confirmation text on the clear link.  The text will change depending on how many GL9's are on the quote 
    ''' and whether the quote has Inland Marine and/or RV/Watercraft items
    ''' </summary>
    Private Sub UpdateClearButtonConfirmation()
        ' Clear link
        Dim txt As String = Nothing

        If IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.NumberOfGL9sOnALocation(MyLocation) > 0 Then
            If IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.LocationIsTheOnlyLocationWithGL9(Quote, MyLocationIndex) Then
                ' This is the only location with GL-9's
                If QuoteHasInlandMarine AndAlso QuoteHasRVWatercraft Then
                    txt = "Deleting this coverage from this location will also delete all Inland Marine and RV/Watercraft coverages and information on this quote. Are you sure you want to delete this item?"
                ElseIf QuoteHasInlandMarine Then
                    txt = "Deleting this coverage from this location will also delete all Inland Marine coverages and information on this quote. Are you sure you want to delete this item?"
                ElseIf QuoteHasRVWatercraft Then
                    txt = "Deleting this coverage from this location will also delete all RV/Watercraft coverages and information on this quote. Are you sure you want to delete this item?"
                Else
                    ' No IM or RV
                    txt = "Are you sure you want to delete all GL-9s from this location?"
                End If
            Else
                ' Not the only location with a GL-9
                txt = "Are you sure you want to delete all GL-9s from this location?"
            End If
            If txt IsNot Nothing Then VRScript.CreateConfirmDialog(lnkClearPersonalLiability.ClientID, txt)
        End If
        Exit Sub
    End Sub

    ''' If the section index text is -1 it's a blank record.
    ''' Returns true if it's a blank record, false if not.
    ''' </summary>
    ''' <param name="riControls"></param>
    ''' <returns></returns>
    Private Function RepeaterItemIsABlankRecord(ByVal riControls As MyRepeaterItemControls_structure) As Boolean
        If riControls.txtSectionIIIndex.Text = "-1" Then Return True Else Return False
    End Function

    Private Sub CheckForIMAndRVDelete(ByRef Deleted As Boolean)
        Deleted = False
        ' If there are no real GL-9's left on the quote after deleting, remove all 
        ' Inland Marine and RV/Watercraft from the quote.  The user was warned about this via dialog
        ' when they clicked the delete or clear buttons.
        If IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.NumberOfGL9sOnAQuote(Quote) = 0 Then
            If SubQuoteFirst IsNot Nothing Then
                SubQuoteFirst.Locations(0).InlandMarines = Nothing
                SubQuoteFirst.Locations(0).RvWatercrafts = Nothing
                Deleted = True
            End If
        End If
    End Sub
#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.PersonalLiabilitySection.ClientID
        If Not IsPostBack Then
            LoadStaticData()
        End If
    End Sub

    Private Sub lbAddNewPersonalLiability_Click(sender As Object, e As EventArgs) Handles lbAddNewPersonalLiability.Click
        Dim cnt As Integer = 0

        AddNewRecord = True
        Populate()

        Exit Sub
    End Sub

    Private Sub lnkSavePersonalLiability_Click(sender As Object, e As EventArgs) Handles lnkSavePersonalLiability.Click
        Save_FireSaveEvent()
        Populate()
    End Sub

    Private Sub lnkClearPersonalLiability_Click(sender As Object, e As EventArgs) Handles lnkClearPersonalLiability.Click
        Dim chg As Boolean = False
        Dim IMRVDeleted As Boolean = False

restart:
        If MyLocation Is Nothing OrElse MyLocation.SectionIICoverages Is Nothing OrElse MyLocation.SectionIICoverages.Count = 0 Then
            If chg Then
                ' No sectionII coverages left. re-populate.
                CheckForIMAndRVDelete(IMRVDeleted)
                EnableAndUncheckGL9Checkbox()
                Save_FireSaveEvent(False)
                RaiseEvent GL9Changed(IMRVDeleted)
            End If
            Exit Sub
        End If
        For Each sc As QuickQuoteSectionIICoverage In MyLocation.SectionIICoverages
            If sc.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9 Then
                MyLocation.SectionIICoverages.Remove(sc)
                chg = True
                GoTo restart
            End If
        Next
        If chg Then
            ' No sectionII coverages left. re-populate.
            CheckForIMAndRVDelete(IMRVDeleted)
            EnableAndUncheckGL9Checkbox()
            Save_FireSaveEvent(False)
            RaiseEvent GL9Changed(IMRVDeleted)
        End If
    End Sub

    ''' <summary>
    ''' Handles events coming from repeater controls
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub rpt_ItemCommand(sender As Object, e As RepeaterCommandEventArgs) Handles rptPersonalLiabilityItems.ItemCommand
        Dim lb As LinkButton = Nothing
        Dim txtIndex As TextBox = e.Item.FindControl("txtSectionIIIndex")
        If txtIndex Is Nothing Then Throw New Exception("Index textbox not found!")
        If Not IsNumeric(txtIndex.Text) Then Throw New Exception("Index is not numeric!")
        Dim Ndx As Integer = CInt(txtIndex.Text)
        Dim IMRVDeleted As Boolean = False

        Select Case e.CommandName
            Case "EDIT"     ' Edit linkbutton
                Exit Select
            Case "DELETE"   ' Delete linkbutton
                Dim err As String = Nothing
                If MyLocation Is Nothing Then Throw New Exception("MyLocation is nothing!")
                If MyLocation.SectionIICoverages Is Nothing OrElse Not MyLocation.SectionIICoverages.HasItemAtIndex(Ndx) Then Throw New Exception("There is no Section II coverage at the specified index")

                MyLocation.SectionIICoverages.RemoveAt(Ndx)

                CheckForIMAndRVDelete(IMRVDeleted)

                If IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.NumberOfGL9sOnALocation(MyLocation) = 0 Then EnableAndUncheckGL9Checkbox()
                Populate()
                Save_FireSaveEvent()
                RaiseEvent GL9Changed(IMRVDeleted)

                Exit Select
            Case "OK"       ' Address OK linkbutton - SAVE
                Save_FireSaveEvent()
                ' Don't populate if the item had validation errors - Populate will wipe out the new item
                If ControlIndexesWithValidationErrors Is Nothing OrElse ControlIndexesWithValidationErrors = "" Then
                    Populate()
                End If
                Exit Select
            Case "CANCEL"   ' Address CANCEL linkbutton
                Dim MyRI As RepeaterItem = rptPersonalLiabilityItems.Items(e.Item.ItemIndex)
                Dim riControls As MyRepeaterItemControls_structure = GetRepeaterControls(MyRI)
                If riControls.txtSectionIIIndex Is Nothing OrElse (Not IsNumeric(riControls.txtSectionIIIndex.Text)) Then Exit Sub
                Dim SectionIIIndex As Integer = CInt(riControls.txtSectionIIIndex.Text)

                If riControls.txtSectionIIIndex.Text = "-1" Then
                    ' New Record - Don't do anything except repopulate which will only populate existing records, which will get rid of the new one.
                    AddNewRecord = False
                    Populate()
                Else
                    ' Existing record - replace any user-entered values with the original record values
                    Dim GL9 As QuickQuoteSectionIICoverage = MyLocation.SectionIICoverages(SectionIIIndex)
                    riControls.txtFirstName.Text = GL9.Name.FirstName
                    riControls.txtLastName.Text = GL9.Name.LastName
                    riControls.txtStreetNumber.Text = GL9.Address.HouseNum
                    riControls.txtStreetName.Text = GL9.Address.StreetName
                    riControls.txtAptSuiteNumber.Text = GL9.Address.ApartmentNumber
                    riControls.txtCity.Text = GL9.Address.City
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(riControls.ddState, GL9.Address.StateId)
                    riControls.txtZip.Text = GL9.Address.Zip
                    DisableAndCheckGL9Checkbox()
                End If

                Exit Select
            Case Else
                Exit Sub
        End Select
    End Sub

    Private Sub rptPersonalLiabilityItems_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptPersonalLiabilityItems.ItemDataBound
        Dim SectionIICov As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = Nothing
        Dim HasAddressError As Boolean = False
        Dim errindexes() As String = Nothing
        Dim txt As String = ""

        Dim riControls As MyRepeaterItemControls_structure = GetRepeaterControls(e.Item)

        If ControlIndexesWithValidationErrors IsNot Nothing Then errindexes = ControlIndexesWithValidationErrors.Split(",")

        If riControls.txtFirstName Is Nothing OrElse riControls.txtLastName Is Nothing OrElse riControls.txtSectionIIIndex Is Nothing Then Exit Sub

        ' Load the state dropdown
        QQHelper.LoadStaticDataOptionsDropDown(riControls.ddState, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.StateId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)

        Dim SCDataRecord As IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.IFMSectionII_structure = e.Item.DataItem
        SectionIICov = SCDataRecord.SectionIIItem

        If SectionIICov IsNot Nothing Then
            riControls.txtSectionIIIndex.Text = SCDataRecord.SectionIndex.ToString  ' Always populate the index!

            ' Clear the input fields
            riControls.txtFirstName.Text = ""
            riControls.txtLastName.Text = ""
            riControls.txtStreetNumber.Text = ""
            riControls.txtStreetName.Text = ""
            riControls.txtAptSuiteNumber.Text = ""
            riControls.txtZip.Text = ""
            riControls.txtCity.Text = ""
            riControls.ddState.SelectedIndex = 0

            If SectionIICov.Name Is Nothing Then SectionIICov.Name = New QuickQuoteName
            If SectionIICov.Address Is Nothing Then SectionIICov.Address = New QuickQuoteAddress

            If riControls.txtSectionIIIndex.Text = "-1" Then
                ' Empty record.  No confirmation needed on the delete link
                ' Don't populate the controls if it's an empty record
                riControls.lbDelete.OnClientClick = Nothing
            Else
                riControls.txtFirstName.Text = SectionIICov.Name.FirstName
                riControls.txtLastName.Text = SectionIICov.Name.LastName
                riControls.txtStreetNumber.Text = SectionIICov.Address.HouseNum
                riControls.txtStreetName.Text = SectionIICov.Address.StreetName
                riControls.txtAptSuiteNumber.Text = SectionIICov.Address.ApartmentNumber
                riControls.txtZip.Text = SectionIICov.Address.Zip
                riControls.txtCity.Text = SectionIICov.Address.City
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(riControls.ddState, SectionIICov.Address.StateId)
            End If

            ' Collapse the address section by default
            CollapseOrExpandAddressSection(riControls.lbEditAddress, riControls.divAddressInfo, "Collapse")

            ' If this item has error expand the address section
            If errindexes IsNot Nothing AndAlso errindexes.Count > 0 Then
                Dim HasError As Boolean = False
                For Each ei As String In errindexes
                    If IsNumeric(ei) Then
                        Dim intEi As Integer = CInt(ei)
                        If intEi = e.Item.ItemIndex Then
                            HasError = True
                            Exit For
                        End If
                    End If
                Next
                If HasError Then CollapseOrExpandAddressSection(riControls.lbEditAddress, riControls.divAddressInfo, "Expand")
            End If

            ' Always open the address section on a new item 
            If AddNewRecord AndAlso riControls.txtSectionIIIndex.Text = "-1" Then
                divPersonalLiabilityData.Attributes.Add("style", "display:''")
                CollapseOrExpandAddressSection(riControls.lbEditAddress, riControls.divAddressInfo, "Expand")
                riControls.txtFirstName.Focus()
                AddNewRecord = False
                DisableAndCheckGL9Checkbox()
            End If
        End If

        Exit Sub
    End Sub

#End Region


End Class