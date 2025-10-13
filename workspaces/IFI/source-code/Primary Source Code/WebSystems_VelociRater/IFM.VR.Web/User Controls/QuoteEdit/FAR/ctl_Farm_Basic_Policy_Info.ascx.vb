Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports QuickQuote.CommonObjects.QuickQuoteObject
Imports IFM.VR.Web.ctlPageStartupScript
Imports IFM.PrimativeExtensions

Public Class ctl_Farm_Basic_Policy_Info
    Inherits VRControlBase

    Public ReadOnly Property isFarmCopy() As Boolean
        Get
            Dim FInfoStatus As Boolean
            If Boolean.TryParse(QQDevDictionary_GetItem("showFarmInfo"), FInfoStatus) Then
                If FInfoStatus Then
                    Return True
                End If
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property ShouldProcessFarmCopy() As Boolean
        Get
            If isFarmCopy Then
                If SubQuoteFirst?.ProgramTypeId = "6" AndAlso Me.Quote.Policyholder.Name.TypeId.Equals("1") Then
                    If Me.Quote?.Locations(0)?.FormTypeId?.EqualsAny("15", "16", "17", "18") Then
                        Return True
                    End If
                End If
            End If
            Return False
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadStaticData()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreatePopUpWindow_jQueryUi_Dialog("dibHobbyFarmInfoPopup", "Hobby Farm", 750, 460, False, True, False, "lnkHobbyFarmPopup")
        Me.VRScript.CreatePopUpWindow_jQueryUi_Dialog("divFarmFormTypeHelp", "Policy Types", 550, 360, False, True, False, "lnkPopupPolicyType")
        Me.VRScript.CreatePopUpWindow_jQueryUi_Dialog("divHobbyFarmHelp", "Hobby Farm Guidelines", 450, 260, False, True, False, Me.chkIsHobbyFarm.ClientID, hddnDivFarmGuidelines.ClientID)

        Me.VRScript.CreateJSBinding(btnCancel.ClientID, JsEventType.onclick, "var c = confirm('Cancel and remove quote?'); if (c){$('input[type=submit]').hide();return true;}else{return false;}")
    End Sub

    Public Overrides Sub LoadStaticData()
        'QQHelper.LoadStaticDataOptionsDropDown(ddPolicyType, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.ProgramTypeId, SortBy.None, Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()
        Me.Visible = False
        If IsPostBack = False AndAlso Me.Quote IsNot Nothing Then
            If Me.Quote.LobType = QuickQuoteLobType.Farm Then
                If String.IsNullOrWhiteSpace(Me.Quote.Policyholder.Name.TypeId) OrElse ShouldProcessFarmCopy() OrElse ShouldProcessFarmStartNewQuoteForClient() Then
                    Me.Visible = True
                    Me.ShowPopup()
                    Me.radioListCommOrPersonal.SelectedValue = Me.Quote.Policyholder.Name.TypeId
                    'Necessary? Removed below because it doesn't do anything - CAH 20200506
                    'If Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                    '    If Me.Quote.Locations(0).Buildings IsNot Nothing AndAlso Me.Quote.Locations(0).Buildings.Any() Then
                    '        ' has a building
                    '        ' no need for any loading logic because this won't be shown on copied quotes
                    '    End If
                    'End If
                    '
                    'CAH B34547
                    If isFarmCopy Then
                        If SubQuoteFirst?.Locations?.Any() Then
                            Dim Loc = SubQuoteFirst.Locations(0)
                            Me.chkIsHobbyFarm.Checked = Loc.FarmTypeHobby

                            If Loc.FarmTypeDairy = True Then
                                Me.radioListFarmActivity.SelectedValue = "1"
                            ElseIf Loc.FarmTypeFieldCrops = True Then
                                Me.radioListFarmActivity.SelectedValue = "2"
                            ElseIf Loc.FarmTypeFruits = True Then
                                Me.radioListFarmActivity.SelectedValue = "3"
                            ElseIf Loc.FarmTypeGreenhouses = True Then
                                Me.radioListFarmActivity.SelectedValue = "4"
                            ElseIf Loc.FarmTypeHorse = True Then
                                Me.radioListFarmActivity.SelectedValue = "5"
                            ElseIf Loc.FarmTypeLivestock = True Then
                                Me.radioListFarmActivity.SelectedValue = "6"
                            ElseIf Loc.FarmTypePoultry = True Then
                                Me.radioListFarmActivity.SelectedValue = "7"
                            ElseIf Loc.FarmTypeSwine = True Then
                                Me.radioListFarmActivity.SelectedValue = "8"
                            ElseIf Loc.FarmTypeVegetables = True Then
                                Me.radioListFarmActivity.SelectedValue = "9"
                            End If
                            rdoPolicyType.SelectedValue = SubQuoteFirst.ProgramTypeId
                        End If
                    End If
                    Me.Visible = True
                    If Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Indiana OrElse Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois Then
                        Me.divHobbyFarm.Attributes.Add("style", "display:none;")
                    End If
                End If
                'Added to adjust javascript for Copy - CAH B34547
                VRScript.AddVariableLine("var isFarmCopy = " + isFarmCopy.ToString().ToLower() + ";")
            End If
        End If
    End Sub

    Private Function ShouldProcessFarmStartNewQuoteForClient() As Boolean
        Dim showBasicInfo As Boolean = False
        If SubQuoteFirst?.ProgramTypeId = "" AndAlso String.IsNullOrWhiteSpace(Me.Quote.Policyholder.Name.TypeId) = False Then
            showBasicInfo = True
        End If
        Return showBasicInfo
    End Function

    Public Overrides Function Save() As Boolean
        If Me.Quote IsNot Nothing AndAlso Me.Visible Then

            If Me.Quote.Locations Is Nothing Then
                Me.Quote.Locations = New List(Of QuickQuoteLocation)()
            End If
            Dim loc As QuickQuoteLocation = Nothing
            If Me.Quote.Locations.Any() = False Then
                loc = New QuickQuoteLocation()
                Me.Quote.Locations.Add(loc)
            End If
            loc = Me.Quote.Locations(0)

            If Me.Quote.Policyholder Is Nothing Then
                Me.Quote.Policyholder = New QuickQuotePolicyholder()
            End If

            If Quote.Policyholder.Name Is Nothing Then
                Quote.Policyholder.Name = New QuickQuoteName
            End If

            Dim startNewQuoteForClient As Boolean = ShouldProcessFarmStartNewQuoteForClient()
            Dim origPHTypeId = Me.Quote.Policyholder.Name.TypeId

            Me.Quote.Policyholder.Name.TypeId = If(Me.radioListCommOrPersonal.SelectedIndex > -1, Me.radioListCommOrPersonal.SelectedValue, "")

            Dim newPHTypeId = Me.Quote.Policyholder.Name.TypeId
            If startNewQuoteForClient AndAlso origPHTypeId <> newPHTypeId Then
                'need to set commercial name to personal name, personal name to commercial name
                Select Case newPHTypeId
                    Case "1"
                        'need to set commercial name to personal
                        If GoverningStateQuote IsNot Nothing AndAlso GoverningStateQuote.Applicants IsNot Nothing AndAlso GoverningStateQuote.Applicants.Count > 0 Then
                            Quote.Policyholder.Name.FirstName = GoverningStateQuote.Applicants(0).Name.FirstName
                            Quote.Policyholder.Name.MiddleName = GoverningStateQuote.Applicants(0).Name.MiddleName
                            Quote.Policyholder.Name.LastName = GoverningStateQuote.Applicants(0).Name.LastName
                        End If
                        Quote.Policyholder.Name.CommercialName1 = "" 'Otherwise this throws a validation error "Must be a personal name but you have a commercial name"               
                    Case "2"
                        'need to set personal to commercial
                        If GoverningStateQuote.Applicants Is Nothing Then
                            GoverningStateQuote.Applicants = New List(Of QuickQuoteApplicant)()
                        End If
                        If GoverningStateQuote.Applicants.Count <= 0 Then
                            GoverningStateQuote.Applicants.AddNew()
                        End If
                        GoverningStateQuote.Applicants(0).Name.FirstName = Quote.Policyholder.Name.FirstName
                        GoverningStateQuote.Applicants(0).Name.MiddleName = Quote.Policyholder.Name.MiddleName
                        GoverningStateQuote.Applicants(0).Name.LastName = Quote.Policyholder.Name.LastName
                        GoverningStateQuote.Applicants(0).Name.CommercialName1 = "" 'not copying policyholder name to commercial name, leave blank and have them enter. This is similar to how the liability coverage type drop down changing from personal to commercial works on ctlFarmPolicyCoverages.ascx.vb.
                        Quote.Policyholder.Name.FirstName = "" 'Otherwise gives validation error "Must be a commercial name but you have a personal name"
                        Quote.Policyholder.Name.MiddleName = ""
                        Quote.Policyholder.Name.LastName = ""
                        Quote.Policyholder.Name.SexId = "" 'Otherwise gives validation error "Invalid Gender"
                End Select
            End If

            'Updated 9/6/18 for multi state MLW
            'If Quote.Policyholder.Name.TypeId = "2" Then
            '    Quote.HasEPLI = True
            'End If
            ''Me.Quote.PolicyTypeId = Me.ddPolicyType.SelectedValue
            'If chkIsHobbyFarm.Checked Then
            '    Quote.ProgramTypeId = "6"
            'Else
            '    Me.Quote.ProgramTypeId = rdoPolicyType.SelectedValue ' Matt A 5-12-15
            'End If
            'If rdoPolicyType.SelectedValue <> "7" Then 'SOM - can be set on quote side under Policy Level Coverages
            '    Me.Quote.LiabilityOptionId = Quote.Policyholder.Name.TypeId ' Matt A 4-14-16  BUG 6380
            'End If
            If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                    If Quote.Policyholder.Name.TypeId = "2" Then
                        sq.HasEPLI = True
                    End If
                    'Me.Quote.PolicyTypeId = Me.ddPolicyType.SelectedValue
                    If chkIsHobbyFarm.Checked Then
                        sq.ProgramTypeId = "6"
                    Else
                        sq.ProgramTypeId = rdoPolicyType.SelectedValue ' Matt A 5-12-15
                    End If
                    sq.LiabilityOptionId = Quote.Policyholder.Name.TypeId ' Matt A 4-14-16  BUG 6380
                Next
            End If


            loc.ProgramTypeId = rdoPolicyType.SelectedValue ' needed on policy level and on first location

            If isFarmCopy = False Then
                loc.FormTypeId = "13" ' na = 13,  SOM-1 Basic Form = 19, SOM-2 Broad Form = 20
            End If


            ClearFarmingActivity(loc) ' clear all before you try to set one
            loc.FarmTypeHobby = Me.chkIsHobbyFarm.Checked
            If loc.FarmTypeHobby Then
                loc.HobbyFarmCredit = True
                loc.ProgramTypeId = "6"
            Else
                Dim farmActivity As String = If(Me.radioListFarmActivity.SelectedIndex > -1, Me.radioListFarmActivity.SelectedValue, "")
                loc.HobbyFarmCredit = False
                loc.ProgramTypeId = rdoPolicyType.SelectedValue
                Select Case Me.radioListFarmActivity.SelectedValue
                    Case "1"
                        loc.FarmTypeDairy = True
                    Case "2"
                        loc.FarmTypeFieldCrops = True
                    Case "3"
                        loc.FarmTypeFruits = True
                    Case "4"
                        loc.FarmTypeGreenhouses = True
                    Case "5"
                        loc.FarmTypeHorse = True
                    Case "6"
                        loc.FarmTypeLivestock = True
                    Case "7"
                        loc.FarmTypePoultry = True
                    Case "8"
                        loc.FarmTypeSwine = True
                    Case "9"
                        loc.FarmTypeVegetables = True
                    Case Else

                End Select
            End If

            ' if comm add the first applicant because it will be required anyway
            'Updated 9/6/18 for multi state MLW
            'If Me.Quote.Policyholder.Name.TypeId = "2" AndAlso (Me.Quote.Applicants Is Nothing OrElse Me.Quote.Applicants.Any() = False) Then
            If Me.Quote.Policyholder.Name.TypeId = "2" AndAlso Me.GoverningStateQuote IsNot Nothing AndAlso (Me.GoverningStateQuote.Applicants Is Nothing OrElse Me.GoverningStateQuote.Applicants.Any() = False) Then
                'Updated 9/6/18 for multi state MLW
                'If Me.Quote.Applicants Is Nothing Then
                '    Me.Quote.Applicants = New List(Of QuickQuoteApplicant)()
                'End If
                'If Me.Quote.Applicants.Any() = False Then
                '    Me.Quote.Applicants.Add(New QuickQuoteApplicant())
                'End If
                If Me.GoverningStateQuote.Applicants Is Nothing Then
                    Me.GoverningStateQuote.Applicants = New List(Of QuickQuoteApplicant)()
                End If
                If Me.GoverningStateQuote.Applicants.Any() = False Then
                    Me.GoverningStateQuote.Applicants.Add(New QuickQuoteApplicant())
                End If
            End If

            QQDevDictionary_RemoveItem("showFarmInfo")

            If Me.Quote.QuoteTransactionType = QuickQuoteTransactionType.ReadOnlyImage Then 'added IF 2/15/2019; original logic in ELSE
                'no save
            ElseIf Me.Quote.QuoteTransactionType = QuickQuoteTransactionType.EndorsementQuote Then
                Dim endorsementSaveError As String = ""
                Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=endorsementSaveError)
            Else
                IFM.VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(Me.QuoteId, Me.Quote, Nothing, QuickQuoteXML.QuickQuoteSaveType.Quote)
            End If
        End If
        Return True
    End Function

    Private Sub ClearFarmingActivity(loc As QuickQuoteLocation)
        loc.FarmTypeBees = False
        loc.FarmTypeDairy = False
        loc.FarmTypeFeedLot = False
        loc.FarmTypeFieldCrops = False
        loc.FarmTypeFlowers = False
        loc.FarmTypeFruits = False
        loc.FarmTypeFurbearingAnimals = False
        loc.FarmTypeGreenhouses = False
        loc.FarmTypeHobby = False
        loc.FarmTypeHorse = False
        loc.FarmTypeLivestock = False
        loc.FarmTypeMushrooms = False
        loc.FarmTypeNurseryStock = False
        loc.FarmTypeNuts = False
        loc.FarmTypePoultry = False
        loc.FarmTypeSod = False
        loc.FarmTypeSwine = False
        loc.FarmTypeTobacco = False
        loc.FarmTypeTurkey = False
        loc.FarmTypeVegetables = False
        loc.FarmTypeVineyards = False
        loc.FarmTypeWorms = False
    End Sub

    Private Function LocationHasFarmingActivitySelected(loc As QuickQuoteLocation)
        Dim types As New List(Of Boolean)
        types.Add(loc.FarmTypeBees)
        types.Add(loc.FarmTypeDairy)
        types.Add(loc.FarmTypeFeedLot)
        types.Add(loc.FarmTypeFieldCrops)
        types.Add(loc.FarmTypeFlowers)
        types.Add(loc.FarmTypeFruits)
        types.Add(loc.FarmTypeFurbearingAnimals)
        types.Add(loc.FarmTypeGreenhouses)
        types.Add(loc.FarmTypeHobby)
        types.Add(loc.FarmTypeHorse)
        types.Add(loc.FarmTypeLivestock)
        types.Add(loc.FarmTypeMushrooms)
        types.Add(loc.FarmTypeNurseryStock)
        types.Add(loc.FarmTypeNuts)
        types.Add(loc.FarmTypePoultry)
        types.Add(loc.FarmTypeSod)
        types.Add(loc.FarmTypeSwine)
        types.Add(loc.FarmTypeTobacco)
        types.Add(loc.FarmTypeTurkey)
        types.Add(loc.FarmTypeVegetables)
        types.Add(loc.FarmTypeVineyards)
        types.Add(loc.FarmTypeWorms)
        Return (From t In types Where t = True Select t).Any()
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

    End Sub

    Public Sub ShowPopup()

        If DirectCast(Me.Page.Master, VelociRater).AgencyID > 0 Then
            Me.VRScript.CreatePopupForm("divBasicInfoPopup", "Basic Policy Information", 520, 570, True, True, False, Me.chkIsHobbyFarm.ClientID, String.Empty)
        Else
            Me.VRScript.AddScriptLine("alert('Choose an agency before creating a new quote.');")
        End If

    End Sub

    Protected Sub btnContinue_Click(sender As Object, e As EventArgs) Handles btnContinue.Click
        Me.Save()
        Response.Redirect(Request.RawUrl, True)
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Helpers.WebHelper_Personal.RemoveQuoteIdFromSessionHistory(Session, QuoteId)
        Dim qqx As New QuickQuoteXML()
        Dim errMsg As String = Nothing
        qqx.ArchiveOrUnarchiveQuote(QuoteId, QuickQuoteXML.QuickQuoteArchiveType.Archive, errMsg)
        Response.Redirect("MyVelocirater.aspx", True)
    End Sub

    'CAH B34547
    Public Sub HidePopup()
        Me.Visible = False
        VRScript.AddScriptLine("$('#divASP_Popups').hide();")
    End Sub
End Class