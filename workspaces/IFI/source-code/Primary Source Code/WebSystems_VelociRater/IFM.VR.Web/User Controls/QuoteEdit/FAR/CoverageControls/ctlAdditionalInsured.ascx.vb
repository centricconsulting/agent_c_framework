Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.FarmLines

Public Class ctlAdditionalInsured
    Inherits VRControlBase

    Public Event RemoveStruct(index As Integer)

    Public Property FarmLocationIndex As Int32
        Get
            Return Session("sess_FarmLocationIndex")
        End Get
        Set(value As Int32)
            Session("sess_FarmLocationIndex") = value
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

    Public ReadOnly Property MyFarmLocation As List(Of QuickQuote.CommonObjects.QuickQuoteLocation)
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations
            End If
            Return Nothing
        End Get
    End Property

    Public Property RowNumber As Int32
        Get
            If ViewState("vs_rowNumber") Is Nothing Then
                ViewState("vs_rowNumber") = 0
            End If
            Return CInt(ViewState("vs_rowNumber"))
        End Get
        Set(value As Int32)
            ViewState("vs_rowNumber") = value
        End Set
    End Property

    Public ReadOnly Property PolicyHolderType() As String
        Get
            If Quote.Policyholder.Name.TypeId IsNot Nothing Then
                Return Quote.Policyholder.Name.TypeId
            Else
                Return "1"
            End If
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(ddlAIType, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAdditionalInterest, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)

        ' Filter AI Type List
        Dim newListItem As ListItem
        Dim tempAIType As DropDownList = New DropDownList()
        For Each item In ddlAIType.Items
            'If item.Text.StartsWith("ADDITIONAL INSURED", StringComparison.OrdinalIgnoreCase) Then
                newListItem = New ListItem(item.Text, item.Value)
                tempAIType.Items.Add(newListItem)
            'End If
        Next

        ddlAIType.Items.Clear()
        For Each item In tempAIType.Items
            newListItem = New ListItem(item.Text, item.Value)
            ddlAIType.Items.Add(newListItem)
        Next

        ' Remove AI types based on policy type
        If PolicyHolderType = "1" Then
            'Personal
            ddlAIType.Items.RemoveAt(1)
        Else
            'Commercial
            ddlAIType.Items.Clear()
            ddlAIType.Items.Add(New ListItem(tempAIType.Items(1).Text, tempAIType.Items(1).Value))
            ddlAIType.Items.Add(New ListItem(tempAIType.Items(4).Text, tempAIType.Items(4).Value))
            ddlAIType.Items.Add(New ListItem(tempAIType.Items(5).Text, tempAIType.Items(5).Value))
            ddlAIType.Items.Add(New ListItem(tempAIType.Items(6).Text, tempAIType.Items(6).Value))
            ddlAIType.Items.Add(New ListItem(tempAIType.Items(7).Text, tempAIType.Items(7).Value))
            ddlAIType.Items.Add(New ListItem(tempAIType.Items(8).Text, tempAIType.Items(8).Value))
        End If
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        Dim addlInsurd As QuickQuoteAdditionalInterest = Nothing

        If MyFarmLocation IsNot Nothing And MyFarmLocation(0).AdditionalInterests IsNot Nothing Then
            Try
                addlInsurd = MyFarmLocation(0).AdditionalInterests(RowNumber)
            Catch ex As Exception
            End Try

            'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlAIType, addlInsurd.TypeId)
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(ddlAIType, addlInsurd.TypeId, QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAdditionalInterest, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, addlInsurd.TypeId))
            txtAIBusiness.Text = addlInsurd.Name.CommercialName1

            If addlInsurd.Name.FirstName = "FIRST" Then
                txtAIFirstName.Text = ""
            Else
                txtAIFirstName.Text = addlInsurd.Name.FirstName
            End If

            If addlInsurd.Name.LastName = "LAST" Then
                txtAILastName.Text = ""
            Else
                txtAILastName.Text = addlInsurd.Name.LastName
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If MyFarmLocation IsNot Nothing AndAlso IsQuoteEndorsement() = False AndAlso IsQuoteReadOnly() = False Then
            If MyFarmLocation(0).AdditionalInterests IsNot Nothing Then
                Dim addlInsurd As QuickQuoteAdditionalInterest = Nothing

                Try
                    addlInsurd = MyFarmLocation(0).AdditionalInterests(RowNumber)
                Catch ex As Exception
                    addlInsurd = New QuickQuoteAdditionalInterest
                    MyFarmLocation(0).AdditionalInterests.Add(addlInsurd)
                End Try

                If addlInsurd IsNot Nothing Then


                    ' Matt A - 9-28-15 - Changed to not try to modify existing AIs if nothing has changed

                    If addlInsurd.ListId <> "" Then
                        'Existing from app side
                        If addlInsurd.Name.CommercialName1.Trim.ToUpper() <> txtAIBusiness.Text.Trim().ToUpper OrElse
                    addlInsurd.Name.FirstName.Trim.ToUpper() <> txtAIFirstName.Text.Trim.ToUpper() OrElse
                    addlInsurd.Name.LastName.Trim.ToUpper() <> txtAILastName.Text.Trim.ToUpper() OrElse
                    addlInsurd.TypeId.Trim() <> ddlAIType.SelectedValue.Trim() Then
                            ' can't assume that any of the existing info is still valid
                            addlInsurd.ListId = ""
                            If ddlAIType.SelectedValue <> "" Then
                                addlInsurd.TypeId = ddlAIType.SelectedValue
                            End If
                            addlInsurd.Name.CommercialName1 = txtAIBusiness.Text

                            If txtAIBusiness.Text <> "" Then
                                addlInsurd.Name.FirstName = ""
                                addlInsurd.Name.LastName = ""
                            Else
                                addlInsurd.Name.FirstName = txtAIFirstName.Text
                                addlInsurd.Name.LastName = txtAILastName.Text
                            End If

                            addlInsurd.Address.HouseNum = "123"
                            addlInsurd.Address.StreetName = "Any"
                            addlInsurd.Address.POBox = "" ' Matt A 9-28-2015
                            addlInsurd.Address.City = "Any"
                            addlInsurd.Address.State = "IN"
                            addlInsurd.Address.StateId = "16"
                            addlInsurd.Address.Zip = "11111"
                            addlInsurd.Address.County = "United States of America"
                        Else
                            addlInsurd.ListId = ""
                            ' If address is blank, then insert dummy data to prevent record from being deleted at rate
                            If addlInsurd.Address.HouseNum = "" AndAlso addlInsurd.Address.POBox = "" Then
                                addlInsurd.Address.HouseNum = "123"
                            End If

                            If addlInsurd.Address.StreetName = "" AndAlso addlInsurd.Address.POBox = "" Then
                                addlInsurd.Address.StreetName = "Any"
                            End If

                            If addlInsurd.Address.City = "" Then
                                addlInsurd.Address.City = "Any"
                            End If

                            addlInsurd.Address.State = "IN"
                            addlInsurd.Address.StateId = "16"

                            If addlInsurd.Address.Zip = "00000-0000" Then
                                addlInsurd.Address.Zip = "11111"
                            End If

                            addlInsurd.Address.County = "United States of America"
                            'addlInsurd.Address.POBox = "" ' Matt A 9-28-2015
                        End If
                    Else
                        'assume is new so default everything
                        If ddlAIType.SelectedValue <> "" Then
                            addlInsurd.TypeId = ddlAIType.SelectedValue
                        End If
                        addlInsurd.Name.CommercialName1 = txtAIBusiness.Text

                        If txtAIBusiness.Text <> "" Then
                            addlInsurd.Name.FirstName = ""
                            addlInsurd.Name.LastName = ""
                        Else
                            addlInsurd.Name.FirstName = txtAIFirstName.Text
                            addlInsurd.Name.LastName = txtAILastName.Text
                        End If

                        addlInsurd.Address.HouseNum = "123"
                        addlInsurd.Address.StreetName = "Any"
                        addlInsurd.Address.POBox = "" ' Matt A 9-28-2015
                        addlInsurd.Address.City = "Any"
                        addlInsurd.Address.State = "IN"
                        addlInsurd.Address.StateId = "16"
                        addlInsurd.Address.Zip = "11111"
                        addlInsurd.Address.County = "United States of America"
                    End If




                    ' When Additional Insured - Partners, Corporate Officers or Co-owners is selected, then Liability Coverage is added
                    'If ddlAIType.SelectedValue = "56" Then
                    Dim temp = MyFarmLocation(0).AdditionalInterests.FindAll(Function(p) p.TypeId = "56").Count
                    If MyFarmLocation(0).AdditionalInterests.FindAll(Function(p) p.TypeId = "56").Count > 0 Then
                        If MyFarmLocation(0).SectionIICoverages Is Nothing Then
                            MyFarmLocation(0).SectionIICoverages = New List(Of QuickQuoteSectionIICoverage)()
                        End If

                        ' Add as Optional Liability
                        If MyFarmLocation(0).SectionIICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Occupying_Residence_on_Premises).Count = 0 Then
                            Dim sectionIICoverage As New QuickQuoteSectionIICoverage()
                            sectionIICoverage.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Occupying_Residence_on_Premises
                            MyFarmLocation(0).SectionIICoverages.Add(sectionIICoverage)
                        End If
                    Else
                        ' Make sure that Optional Liability coverage is removed
                        If MyFarmLocation(0).SectionIICoverages IsNot Nothing Then
                            Dim optLiabCov As QuickQuoteSectionIICoverage = MyFarmLocation(0).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Occupying_Residence_on_Premises)
                            MyFarmLocation(0).SectionIICoverages.Remove(optLiabCov)
                        End If
                    End If

                    SaveChildControls()

                    Return True
                End If
            End If
        End If

        Return False
    End Function

    Protected Sub OnConfirm(sender As Object, e As EventArgs) Handles lnkDelete.Click
        Dim confirmValue As String = Request.Form("confirmValue")

        If confirmValue = "Yes" Then
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlAIType, "0")
            txtAIBusiness.Text = ""
            txtAILastName.Text = ""
            txtAIFirstName.Text = ""
            RaiseEvent RemoveStruct(RowNumber)
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = "Farm Additional Insured"
        Dim divAI As String = dvFarmAddlInsured.ClientID

        Dim valList = LocationCoverageValidator.ValidateFARLocation(Quote, MyLocationIndex, valArgs.ValidationType, RowNumber)

        If valList.Any() Then
            For Each v In valList
                ' *********************
                ' Base Policy Coverages
                ' *********************
                Select Case v.FieldId
                    Case LocationCoverageValidator.MissingAddlIns
                        ValidationHelper.Val_BindValidationItemToControl(ddlAIType, v, divAI, "0")
                    Case LocationCoverageValidator.MissingBusinessFirstLast
                        ValidationHelper.Val_BindValidationItemToControl(tblAI, v, divAI, "0")
                    Case LocationCoverageValidator.MissingLastName
                        ValidationHelper.Val_BindValidationItemToControl(txtAILastName, v, divAI, "0")
                    Case LocationCoverageValidator.MissingFirtName
                        ValidationHelper.Val_BindValidationItemToControl(txtAIFirstName, v, divAI, "0")
                End Select
            Next
        End If

        ValidateChildControls(valArgs)
        PopulateChildControls()
    End Sub
End Class