Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctlFARAddlResidenceRentedToOthers
    Inherits VRControlBase

    Public Structure IFMSectionII_structure
        Public SectionIndex As Integer
        Public SectionIIItem As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage
    End Structure

    Private Structure MyRepeaterItemControls_structure
        Public tblAddlResiddenceInfo As HtmlTable
        Public txtDescription As TextBox
        Public txtSectionIIIndex As TextBox
        Public ddNumOfFamilies As DropDownList
        Public lbEditAddress As LinkButton
        Public lbDelete As LinkButton
        Public divAddressInfo As HtmlGenericControl
        Public tblAddress As HtmlTable
        Public btnCopyAddress As Button
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

    Private Property ControlIndexesWithAddressErrors As String
        Get
            Return ViewState("vs_ErrIndexes")
        End Get
        Set(value As String)
            ViewState("vs_ErrIndexes") = value
        End Set
    End Property

    Private Property AddNewRecord As Boolean
        Get
            If ViewState("vs_AddNewGL73Record") Is Nothing Then
                ViewState("vs_AddNewGL73Record") = False
            End If
            Return CBool(ViewState("vs_AddNewGL73Record"))
        End Get
        Set(value As Boolean)
            ViewState("vs_AddNewGL73Record") = value
        End Set
    End Property

    Public Property MyLocationIndex As Int32
        Get
            If ViewState("vs_locationIndex") IsNot Nothing Then
                Return CInt(ViewState("vs_locationIndex"))
            End If
            Return 0
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyFarmLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get         
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadStaticData()
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Dim itemnum As Integer = 0
        Dim ErrIndexes As String = ""

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        If Me.chkAddlResidenceRentedToOthers.Checked Then
            For Each ri As RepeaterItem In rptAddlResidenceItems.Items
                itemnum += 1
                Dim AddressError As Boolean = False
                'Me.ValidationHelper.GroupName = "Addl Residence Rented to Others #" & itemnum.ToString
                Me.ValidationHelper.GroupName = "Addl Residence Rented to Others"
                Dim riControls As MyRepeaterItemControls_structure = GetRepeaterControls(ri)

                ' Description, House number, street, city, state, zip are all required
                If riControls.txtDescription.Text.Trim = String.Empty Then Me.ValidationHelper.AddError(riControls.txtDescription, "Missing Description", accordList)
                If riControls.txtStreetNumber.Text.Trim = String.Empty Then
                    AddressError = True
                    Me.ValidationHelper.AddError(riControls.txtStreetNumber, "Missing House Number", accordList)
                End If
                If riControls.txtStreetName.Text.Trim = String.Empty Then
                    AddressError = True
                    Me.ValidationHelper.AddError(riControls.txtStreetName, "Missing Street Name", accordList)
                End If
                If riControls.txtCity.Text.Trim = String.Empty Then
                    AddressError = True
                    Me.ValidationHelper.AddError(riControls.txtCity, "Missing City", accordList)
                End If
                If riControls.ddState.SelectedIndex <= 0 Then
                    AddressError = True
                    Me.ValidationHelper.AddError(riControls.ddState, "Missing State", accordList)
                End If
                If riControls.txtZip.Text.Trim = String.Empty Then
                    AddressError = True
                    Me.ValidationHelper.AddError(riControls.txtZip, "Missing Zip Code", accordList)
                End If

                If AddressError Then
                    If ErrIndexes = "" Then
                        ErrIndexes += (itemnum - 1).ToString
                    Else
                        ErrIndexes += "," & (itemnum - 1).ToString
                    End If
                    ' Need to expand the address div if errors were found in it
                    CollapseOrExpandAddressSection(riControls.lbEditAddress, riControls.divAddressInfo, "Expand")
                End If
            Next
        End If

        ControlIndexesWithAddressErrors = ErrIndexes

        'Me.ValidateChildControls(valArgs)

        Exit Sub
    End Sub

    Public Overrides Sub Populate()
        Dim SCtable As New List(Of IFMSectionII_structure)
        Dim SCNdx As Integer = -1
        Dim dispNdx As Integer = -1
        Dim FoundOne As Boolean = False
        Dim FoundARealOne As Boolean = False

        'If Not Me.Visible Then Exit Sub 'Removed to allow for interop and endorsements since Diamond allows this without a dwelling
        If Quote Is Nothing Then Exit Sub
        'If Quote.Locations Is Nothing OrElse Quote.Locations.Count <= 0 Then Exit Sub
        'If Quote.Locations(0).SectionIICoverages Is Nothing Then Exit Sub
        If MyFarmLocation Is Nothing Then Exit Sub
        If MyFarmLocation.SectionIICoverages Is Nothing Then Exit Sub

        chkAddlResidenceRentedToOthers.Checked = False
        divAddlResidenceData.Attributes.Add("style", "display:none")

        ' Build the table of coverages
        'For Each SC As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage In Quote.Locations(0).SectionIICoverages
        For Each SC As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage In MyFarmLocation.SectionIICoverages
            SCNdx += 1
            If SC.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencesOrFarmsRentedtoOthers Then
                dispNdx += 1
                Dim SCTableItem As New IFMSectionII_structure()
                SCTableItem.SectionIndex = SCNdx
                SCTableItem.SectionIIItem = SC
                SCtable.Add(SCTableItem)
                If Not SCIsEmpty(SCTableItem.SectionIIItem) Then FoundARealOne = True
                FoundOne = True
            End If
        Next

        If AddNewRecord Then
            ' Add new record
            ' Add a blank record to the table we're binding to the repeater.
            Dim newDataItem As New IFMSectionII_structure()
            newDataItem.SectionIndex = -1
            newDataItem.SectionIIItem = New QuickQuoteSectionIICoverage()
            newDataItem.SectionIIItem.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencesOrFarmsRentedtoOthers
            SCtable.Add(newDataItem)
            chkAddlResidenceRentedToOthers.Checked = True
        Else
            ' If we found the coverage above, check the checkbox and display the data section
            If FoundOne Then
                If FoundARealOne Then
                    ' If we found a non-dummy record we need to check the box and open the data section
                    chkAddlResidenceRentedToOthers.Checked = True
                    divAddlResidenceData.Attributes.Add("style", "display:''")
                End If
            Else
                ' not adding a new record...
                ' NEW LOGIC - Does not create a dummy record on the quote object because that method is incompatible with endorsements. Task 52396  MGB
                ' No GL-73 coverages found, create a blank record for display (will be displayed when the checkbox is checked)
                Dim newTableItem As New IFMSectionII_structure()
                Dim newSC As New QuickQuote.CommonObjects.QuickQuoteSectionIICoverage()
                newSC.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencesOrFarmsRentedtoOthers
                newTableItem.SectionIIItem = newSC
                newTableItem.SectionIndex = -1
                SCtable.Add(newTableItem)
            End If
        End If

        rptAddlResidenceItems.DataSource = SCtable
        rptAddlResidenceItems.DataBind()

        Exit Sub
    End Sub

    Private Function GetRepeaterControls(ByVal ri As RepeaterItem) As MyRepeaterItemControls_structure
        Dim MyControls As New MyRepeaterItemControls_structure()

        MyControls.tblAddlResiddenceInfo = ri.FindControl("tblAddlResidenceInfo")
        MyControls.txtDescription = ri.FindControl("txtDescription")
        MyControls.txtSectionIIIndex = ri.FindControl("txtSectionIIIndex")
        MyControls.ddNumOfFamilies = ri.FindControl("ddNumOfFamilies")
        MyControls.lbEditAddress = ri.FindControl("lbEditAddress")
        MyControls.lbDelete = ri.FindControl("lbDelete")
        MyControls.divAddressInfo = ri.FindControl("divAddressInfo")
        MyControls.tblAddress = ri.FindControl("tblAddress")
        MyControls.btnCopyAddress = ri.FindControl("btnCopyAddress")
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

    Private Function SCIsEmpty(ByVal SC As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage) As Boolean
        If SC Is Nothing Then Return False
        If SC.Description = "" AndAlso
                SC.Address.HouseNum = "" AndAlso
                SC.Address.StreetName = "" AndAlso
                SC.Address.Zip = "" Then
            Return True
        End If
        Return False
    End Function

    Public Overrides Function Save() As Boolean
        Dim NDX As Integer = 0
        Dim GL9Cov As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = Nothing

        If Me.chkAddlResidenceRentedToOthers.Checked Then
            For Each ri As RepeaterItem In rptAddlResidenceItems.Items
                Dim riControls As MyRepeaterItemControls_structure = GetRepeaterControls(ri)

                GL9Cov = Nothing

                If IsNumeric(riControls.txtSectionIIIndex.Text) Then
                    NDX = CInt(riControls.txtSectionIIIndex.Text.ToString)
                Else
                    Throw New Exception("Addl Residence Coverage index is invalid")
                End If

                ' NEW LOGIC
                If riControls.txtSectionIIIndex.Text = "-1" Then
                    ' Empty record.  Create a new GL-73 on the object if no validation errors
                    Dim HasData As Boolean = GL73HasAnyValidFields(ri)
                    ' If no errors found on the current repeater item, save it
                    If HasData Then
                        'Quote.Locations(0).SectionIICoverages.AddNew()
                        'NDX = Quote.Locations(0).SectionIICoverages.Count - 1
                        MyFarmLocation.SectionIICoverages.AddNew()
                        NDX = MyFarmLocation.SectionIICoverages.Count - 1
                        riControls.txtSectionIIIndex.Text = NDX
                        'GL9Cov = Quote.Locations(0).SectionIICoverages(NDX)
                        GL9Cov = MyFarmLocation.SectionIICoverages(NDX)
                        GL9Cov.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencesOrFarmsRentedtoOthers

                        ' Copy the values from the page to the object
                        CopyGL73ValuesToObject(GL9Cov, riControls)
                    End If
                Else
                    ' UPDATE EACH EXISTING COVERAGE
                    ' Make sure there's a section II coverage at the specified index
                    'If Quote Is Nothing OrElse Quote.Locations Is Nothing OrElse (Not Quote.Locations.HasItemAtIndex(0)) OrElse Quote.Locations(0).SectionIICoverages Is Nothing OrElse (Not Quote.Locations(0).SectionIICoverages.HasItemAtIndex(NDX)) Then Throw New Exception("Addl Residence coverage not found at index " & NDX.ToString)
                    'If Quote.Locations(0).SectionIICoverages(NDX).CoverageType <> QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencesOrFarmsRentedtoOthers Then Throw New Exception("The coverage at Location(0).SectionCoverages(" & NDX.ToString & ") is not the expected coverage type of Additional Residence Rented to Others")
                    If Quote Is Nothing OrElse MyFarmLocation Is Nothing OrElse MyFarmLocation.SectionIICoverages Is Nothing OrElse (Not MyFarmLocation.SectionIICoverages.HasItemAtIndex(NDX)) Then Throw New Exception("Addl Residence coverage not found at index " & NDX.ToString)
                    If MyFarmLocation.SectionIICoverages(NDX).CoverageType <> QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencesOrFarmsRentedtoOthers Then Throw New Exception("The coverage at Location(" & MyLocationIndex & ").SectionCoverages(" & NDX.ToString & ") is not the expected coverage type of Additional Residence Rented to Others")

                    ' Copy the values from the page to the object
                    'Dim MyCov As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = Quote.Locations(0).SectionIICoverages(NDX)
                    Dim MyCov As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = MyFarmLocation.SectionIICoverages(NDX)
                    CopyGL73ValuesToObject(MyCov, riControls)
                End If
            Next
        Else
            ' Remove all Add'l Residence coverages
            Dim scndx As Integer = -1
scloop:
            'For Each SC As QuickQuoteSectionIICoverage In Quote.Locations(0).SectionIICoverages
            For Each SC As QuickQuoteSectionIICoverage In MyFarmLocation.SectionIICoverages
                scndx += 1
                If SC.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencesOrFarmsRentedtoOthers Then
                    'Quote.Locations(0).SectionIICoverages.Remove(SC)
                    MyFarmLocation.SectionIICoverages.Remove(SC)
                    GoTo scloop
                End If
            Next
        End If

        Return True
    End Function

    ''' <summary>
    ''' Returns true if the passed repeater item has any values in it's input fields
    ''' </summary>
    ''' <param name="ri"></param>
    ''' <returns></returns>
    Private Function GL73HasAnyValidFields(ByVal ri As RepeaterItem) As Boolean
        If ri Is Nothing Then Return False
        Dim riControls As New MyRepeaterItemControls_structure
        riControls = GetRepeaterControls(ri)

        If riControls.txtDescription.Text.Trim <> "" Then Return True
        If riControls.txtSectionIIIndex.Text.Trim <> "" Then Return True
        If riControls.ddNumOfFamilies.SelectedIndex >= 0 Then Return True
        If riControls.txtStreetNumber.Text.Trim <> "" Then Return True
        If riControls.txtStreetName.Text.Trim <> "" Then Return True
        If riControls.txtAptSuiteNumber.Text.Trim <> "" Then Return True
        If riControls.txtZip.Text.Trim <> "" Then Return True
        If riControls.txtCity.Text.Trim <> "" Then Return True
        If riControls.ddState.SelectedIndex >= 0 Then Return True
        If riControls.txtCounty.Text.Trim <> "" Then Return True

        Return False
    End Function

    Private Sub CopyGL73ValuesToObject(ByRef SC2Coverage As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage, ByVal riControls As MyRepeaterItemControls_structure)
        If SC2Coverage.NumberOfFamilies <> riControls.ddNumOfFamilies.SelectedValue Then SC2Coverage.NumberOfFamilies = riControls.ddNumOfFamilies.SelectedValue
        If SC2Coverage.Description <> riControls.txtDescription.Text Then SC2Coverage.Description = riControls.txtDescription.Text
        If SC2Coverage.Address Is Nothing Then SC2Coverage.Address = New QuickQuoteAddress()
        If SC2Coverage.Address.HouseNum <> riControls.txtStreetNumber.Text Then SC2Coverage.Address.HouseNum = riControls.txtStreetNumber.Text
        If SC2Coverage.Address.StreetName <> riControls.txtStreetName.Text Then SC2Coverage.Address.StreetName = riControls.txtStreetName.Text
        If SC2Coverage.Address.ApartmentNumber <> riControls.txtAptSuiteNumber.Text Then SC2Coverage.Address.ApartmentNumber = riControls.txtAptSuiteNumber.Text
        If SC2Coverage.Address.City <> riControls.txtCity.Text Then SC2Coverage.Address.City = riControls.txtCity.Text
        If SC2Coverage.Address.StateId <> riControls.ddState.SelectedValue Then SC2Coverage.Address.StateId = riControls.ddState.SelectedValue
        If SC2Coverage.Address.Zip <> riControls.txtZip.Text Then SC2Coverage.Address.Zip = riControls.txtZip.Text

        Exit Sub
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateJSBinding(chkAddlResidenceRentedToOthers.ClientID, "onclick", "HandleAddlResidenceCheckboxClicks('" & chkAddlResidenceRentedToOthers.ClientID & "','" & divAddlResidenceData.ClientID & "');")

        For Each ri As RepeaterItem In rptAddlResidenceItems.Items
            Dim divAddressInfo As HtmlGenericControl = ri.FindControl("divAddressInfo")
            Dim lbEdit As LinkButton = ri.FindControl("lbEditAddress")
            Dim txtZip As TextBox = ri.FindControl("txtZip")
            Dim txtCity As TextBox = ri.FindControl("txtCity")
            Dim ddCity As DropDownList = ri.FindControl("ddCity")
            Dim txtCounty As TextBox = ri.FindControl("txtCounty")
            Dim ddState As DropDownList = ri.FindControl("ddState")

            If lbEdit IsNot Nothing Then VRScript.CreateJSBinding(lbEdit.ClientID, "onclick", "HandleEditAddressClicks('" & divAddressInfo.ClientID & "','" & lbEdit.ClientID & "');")
            If txtZip IsNot Nothing AndAlso ddCity IsNot Nothing AndAlso txtCity IsNot Nothing AndAlso txtCounty IsNot Nothing AndAlso ddState IsNot Nothing Then Me.VRScript.CreateJSBinding(txtZip, ctlPageStartupScript.JsEventType.onkeyup, "DoCityCountyLookup('" + txtZip.ClientID + "','" + ddCity.ClientID + "','" + txtCity.ClientID + "','" + txtCounty.ClientID + "','" + ddState.ClientID + "');")
        Next
        
        Exit Sub
    End Sub

    ''' <summary>
    ''' Handles events coming from repeater controls
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub rpt_ItemCommand(sender As Object, e As RepeaterCommandEventArgs) Handles rptAddlResidenceItems.ItemCommand
        Dim lb As LinkButton = Nothing
        Dim txtIndex As TextBox = e.Item.FindControl("txtSectionIIIndex")
        If txtIndex Is Nothing Then Throw New Exception("Index textbox not found!")
        If Not IsNumeric(txtIndex.Text) Then Throw New Exception("Index is not numeric!")
        Dim Ndx As Integer = CInt(txtIndex.Text)

        Select Case e.CommandName
            Case "EDIT"     ' Edit linkbutton
                Exit Select
            Case "DELETE"   ' Delete linkbutton
                Dim err As String = Nothing
                'If Quote Is Nothing OrElse Quote.Locations Is Nothing OrElse Not Quote.Locations.HasItemAtIndex(0) Then Throw New Exception("Quote or location is nothing or there is no location at index 0")
                'If Quote.Locations(0).SectionIICoverages Is Nothing OrElse Not Quote.Locations(0).SectionIICoverages.HasItemAtIndex(Ndx) Then Throw New Exception("There is no Section II coverage at the specified index")
                If Quote Is Nothing OrElse Quote.Locations Is Nothing OrElse MyFarmLocation Is Nothing Then Throw New Exception("Quote or location is nothing or there is no location at location index " & MyLocationIndex)
                If MyFarmLocation.SectionIICoverages Is Nothing OrElse Not MyFarmLocation.SectionIICoverages.HasItemAtIndex(Ndx) Then Throw New Exception("There is no Section II coverage at the specified index")

                'Quote.Locations(0).SectionIICoverages.RemoveAt(Ndx)
                MyFarmLocation.SectionIICoverages.RemoveAt(Ndx)
                IFM.VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(QuoteId, Quote, err)  ' since we removed a coverage we need to save
                Me.Populate()
                Exit Select
            Case "OK"       ' Address OK linkbutton - SAVE
                Save_FireSaveEvent()
                Exit Select
            Case "CANCEL"   ' Address CANCEL linkbutton
                Dim MyRI As RepeaterItem = rptAddlResidenceItems.Items(e.Item.ItemIndex)
                Dim riControls As MyRepeaterItemControls_structure = GetRepeaterControls(MyRI)
                If riControls.txtSectionIIIndex Is Nothing OrElse (Not IsNumeric(riControls.txtSectionIIIndex.Text)) Then Exit Sub
                Dim SectionIIIndex As Integer = CInt(riControls.txtSectionIIIndex.Text)

                If riControls.txtSectionIIIndex.Text = "-1" Then
                    ' New Record - Don't do anything except repopulate which will only populate existing records, which will get rid of the new one.
                    AddNewRecord = False
                    Populate()
                Else
                    ' Existing record - replace any user-entered values with the original record values
                    'Dim GL9 As QuickQuoteSectionIICoverage = Quote.Locations(0).SectionIICoverages(SectionIIIndex)
                    Dim GL9 As QuickQuoteSectionIICoverage = MyFarmLocation.SectionIICoverages(SectionIIIndex)
                    CopyGL73ValuesToObject(GL9, riControls)
                    chkAddlResidenceRentedToOthers.Checked = True
                End If

                Exit Select
            Case "CopyAddress"
                'perform a save first in case they enter a location address or change the location address without saving first
                Save_FireSaveEvent(False)
                If MyFarmLocation IsNot Nothing AndAlso MyFarmLocation.Address IsNot Nothing Then
                    Dim MyRI As RepeaterItem = rptAddlResidenceItems.Items(e.Item.ItemIndex)
                    Dim riControls As MyRepeaterItemControls_structure = GetRepeaterControls(MyRI)
                    riControls.txtStreetNumber.Text = MyFarmLocation.Address.HouseNum
                    riControls.txtStreetName.Text = MyFarmLocation.Address.StreetName
                    riControls.txtAptSuiteNumber.Text = MyFarmLocation.Address.ApartmentNumber
                    riControls.txtCity.Text = MyFarmLocation.Address.City
                    riControls.ddState.SelectedValue = MyFarmLocation.Address.StateId
                    riControls.txtZip.Text = MyFarmLocation.Address.Zip
                    riControls.txtCounty.Text = MyFarmLocation.Address.County
                End If
            Case Else
                Exit Sub
        End Select
    End Sub

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

    Private Sub rptAddlResidenceItems_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptAddlResidenceItems.ItemDataBound
        Dim SectionIICov As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = Nothing
        Dim EmptyRecord As Boolean = False
        Dim HasAddressError As Boolean = False
        Dim errindexes() As String = Nothing

        Dim riControls As MyRepeaterItemControls_structure = GetRepeaterControls(e.Item)

        If ControlIndexesWithAddressErrors IsNot Nothing Then errindexes = ControlIndexesWithAddressErrors.Split(",")

        If riControls.ddNumOfFamilies Is Nothing OrElse riControls.txtDescription Is Nothing OrElse riControls.txtSectionIIIndex Is Nothing Then Exit Sub

        ' Load the state dropdown
        QQHelper.LoadStaticDataOptionsDropDown(riControls.ddState, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.StateId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)

        Dim SCDataRecord As IFMSectionII_structure = e.Item.DataItem
        SectionIICov = SCDataRecord.SectionIIItem

        If SectionIICov IsNot Nothing Then
            EmptyRecord = SCIsEmpty(SectionIICov)

            ' Populate general coverage information if this is not a empty record
            If Not EmptyRecord Then
                If riControls.txtDescription IsNot Nothing Then riControls.txtDescription.Text = SectionIICov.Description
                If SectionIICov.NumberOfFamilies IsNot Nothing AndAlso SectionIICov.NumberOfFamilies <> "" AndAlso IsNumeric(SectionIICov.NumberOfFamilies) Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(riControls.ddNumOfFamilies, SectionIICov.NumberOfFamilies)
                Else
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(riControls.ddNumOfFamilies, "1")
                End If
            End If
            riControls.txtSectionIIIndex.Text = SCDataRecord.SectionIndex.ToString  ' Always populate the index, including empty records

            ' Clear the input fields
            riControls.txtStreetNumber.Text = ""
            riControls.txtStreetName.Text = ""
            riControls.txtAptSuiteNumber.Text = ""
            riControls.txtZip.Text = ""
            riControls.txtCity.Text = ""
            riControls.ddState.SelectedIndex = 0

            ' Only populate the address info if this is not a empty record
            If (Not EmptyRecord) AndAlso (SectionIICov.Address IsNot Nothing) Then
                riControls.txtStreetNumber.Text = SectionIICov.Address.HouseNum
                riControls.txtStreetName.Text = SectionIICov.Address.StreetName
                riControls.txtAptSuiteNumber.Text = SectionIICov.Address.ApartmentNumber
                riControls.txtZip.Text = SectionIICov.Address.Zip
                riControls.txtCity.Text = SectionIICov.Address.City
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(riControls.ddState, SectionIICov.Address.StateId)
            End If

            ' If this item has an address error don't collapse the address section
            CollapseOrExpandAddressSection(riControls.lbEditAddress, riControls.divAddressInfo, "Collapse")
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
                divAddlResidenceData.Attributes.Add("style", "display:''")
                CollapseOrExpandAddressSection(riControls.lbEditAddress, riControls.divAddressInfo, "Expand")
                riControls.txtDescription.Focus()
                AddNewRecord = False
            End If

        End If

        Exit Sub
    End Sub

    Public Overrides Sub ClearControl()
        chkAddlResidenceRentedToOthers.Checked = False
        divAddlResidenceData.Attributes.Add("style", "display:none")
    End Sub

    Private Sub lbAddNewResidence_Click(sender As Object, e As EventArgs) Handles lbAddNewResidence.Click
        AddNewRecord = True
        Populate()

        Exit Sub
    End Sub
End Class