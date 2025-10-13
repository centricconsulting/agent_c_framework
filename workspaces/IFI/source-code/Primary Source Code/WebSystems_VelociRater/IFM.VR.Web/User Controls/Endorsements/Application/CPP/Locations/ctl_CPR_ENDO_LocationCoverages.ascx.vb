Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Helpers.CPP
Imports IFM.VR.Common.Helpers.CPR
Imports QuickQuote.CommonObjects
Imports IFM.VR.Common.Helpers

Public Class ctl_CPR_ENDO_LocationCoverages
    Inherits VRControlBase

    Public Property MyLocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_locationIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote.IsNotNull Then
                Return Me.Quote.Locations.GetItemAtIndex(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return MyLocationIndex
        End Get
    End Property

    Protected ReadOnly Property isWindHailEnabled As Boolean
        Get
            If LocationWindHailHelper.IsLocationWindHailAvailable(Quote) Then
                Return True
            Else
                Dim CHC = New CommonHelperClass
                Return CHC.ConfigurationAppSettingValueAsBoolean("VR_CPP_Endo_WindHailEnabled", False)
            End If
        End Get
    End Property

    Public ReadOnly Property LocationPropertyDeductibleClientID As String
        Get
            Return Me.ddLocationPropertyDeductible.ClientID
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkClear.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.CreateAccordion(Me.divCPRLocationCoverages.ClientID, hdnAccord, 0)

        Me.VRScript.CreateJSBinding(chkEquipmentBreakdown, ctlPageStartupScript.JsEventType.onchange, "Cpr.LocOrPIOCoverageCheckboxChanged('EQB','" & chkEquipmentBreakdown.ClientID & "');")

        VRScript.AddVariableLine("var LocWindHailDeductibleId = '" & ddWindHailDeductible.ClientID & "';")

        If isWindHailEnabled Then
            If LocationWindHailHelper.IsLocationWindHailAvailable(Quote) = False Then
                Me.VRScript.FakeDisableSingleElement(Me.ddWindHailDeductible)
            End If
        End If
        Dim BlanketDeductibleId = BlanketHelper_CPR_CPP.GetBlanketDeductibleID(SubQuoteFirst)
        VRScript.AddVariableLine("var BlanketDeductibleId = '" & BlanketDeductibleId & "';")

    End Sub

    Public Overrides Sub LoadStaticData()
        If isWindHailEnabled Then
            If ddWindHailDeductible.Items Is Nothing OrElse ddWindHailDeductible.Items.Count <= 0 Then
                'QQHelper.LoadStaticDataOptionsDropDown(Me.ddWindHailDeductible, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.WindHailDeductibleLimitId, SortBy.None, Me.Quote.LobType)
                QQHelper.LoadStaticDataOptionsDropDown(Me.ddWindHailDeductible, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.WindHailDeductibleLimitId, SortBy.None, Me.Quote.LobType)
            End If
        End If
        If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
            If ddLocationPropertyDeductible.Items Is Nothing OrElse ddLocationPropertyDeductible.Items.Count <= 0 Then
                QQHelper.LoadStaticDataOptionsDropDown(Me.ddLocationPropertyDeductible, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId, SortBy.None, Me.Quote.LobType)
            End If
        End If
        Dim remove500K As ListItem = ddLocationPropertyDeductible.Items.FindByValue("8")
        If remove500K IsNot Nothing Then
            Me.ddLocationPropertyDeductible.Items.Remove(remove500K)
        End If
    End Sub

    Protected Sub HandlePropertyClear()
        Me.lblAccordHeader.Text = "Location"
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()

        If Quote IsNot Nothing Then
            If MyLocation IsNot Nothing Then
                ' WIND/HAIL

                'Dim AllPreExistingItems = New DevDictionaryHelper.AllPreExistingItems()
                'AllPreExistingItems.GetAllPreExistingInDevDictionary(Quote)
                'If AllPreExistingItems.PreExisting_Locations.isPreExistingLocationByLocationObject(MyLocation) Then
                '    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddWindHailDeductible, MyLocation.WindHailDeductibleLimitId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.WindHailDeductibleLimitId, Me.Quote.LobType)
                'Else
                '    'If not Preexisting then default to location 1
                '    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddWindHailDeductible, Me.Quote.Locations.GetItemAtIndex(0).WindHailDeductibleLimitId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.WindHailDeductibleLimitId, Me.Quote.LobType)
                'End If
                If isWindHailEnabled Then
                    tblWindHail.Visible = True
                    SetFromValue(ddWindHailDeductible, MyLocation.WindHailDeductibleLimitId, "0")
                Else
                    tblWindHail.Visible = False
                End If

                If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                    Me.lblAccordHeader.Text = "Location Coverage Options"
                    Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
                    If endorsementsPreexistHelper.IsPreexistingLocation(MyLocation) = False Then
                        tblLocationPropertyDeductible.Attributes.Add("style", "display:''")
                        SetFromValue(ddLocationPropertyDeductible, MyLocation.DeductibleId, "9")
                    Else
                        tblLocationPropertyDeductible.Attributes.Add("style", "display:none")
                    End If
                Else
                    Me.lblAccordHeader.Text = "Optional Location Coverages"
                    tblLocationPropertyDeductible.Attributes.Add("style", "display:none")
                End If

                ' EQUIPMENT BREAKDOWN
                ' Certain risk grades can make the quote ineligible for Equipment Breakdown coverage 
                If RiskGradeEligibleForEquipmentBreakdown() Then
                    ' ELIGIBLE - enable checkbox, hide info row, show the value
                    chkEquipmentBreakdown.Enabled = True
                    trEquipmentBreakDownInfoRow.Attributes.Add("style", "display:none")
                    If MyLocation.EquipmentBreakdownDeductibleId <> "" Then
                        ' Deductible is set - has Equipment Breakdown
                        chkEquipmentBreakdown.Checked = True
                    Else
                        ' Deductible not set - does not have equipment breakdown
                        chkEquipmentBreakdown.Checked = False
                    End If
                Else
                    ' NOT ELIGIBLE - disable checkbox, show info row
                    chkEquipmentBreakdown.Checked = False
                    chkEquipmentBreakdown.Enabled = False
                    trEquipmentBreakDownInfoRow.Attributes.Add("style", "display:''")
                End If
                If WindHailDefaultingHelper.IsWindHailDefaultingAvailable(Quote) Then '0=NA
                    Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
                    If endorsementsPreexistHelper.IsPreexistingLocation(MyLocation) = False Then
                        DefaultWindHailDropDown()
                    End If
                End If
            End If
        End If
        Me.PopulateChildControls()
    End Sub



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        Dim chc = New CommonHelperClass
        Dim EQBrkDEfaultLogicEnabled As Boolean = chc.ConfigurationAppSettingValueAsBoolean("VR_CPP_CPR_EnableEquipmentBreakdownDeductibleDefaultLogic")

        ' Wind/Hail deductible
        If isWindHailEnabled Then
            If Quote IsNot Nothing AndAlso MyLocation IsNot Nothing Then
                MyLocation.WindHailDeductibleLimitId = ddWindHailDeductible.SelectedValue
            End If
        End If

        ' Location deductible

        If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
            If Quote IsNot Nothing AndAlso MyLocation IsNot Nothing Then
                MyLocation.DeductibleId = ddLocationPropertyDeductible.SelectedValue
            End If
        End If

        If Not EQBrkDEfaultLogicEnabled Then
            ' Set the Equipment Breakdown value - Only uses this old logic when the feature flag above is set to false or does not exist
            If chkEquipmentBreakdown.Checked AndAlso MyLocation IsNot Nothing Then
                MyLocation.EquipmentBreakdownDeductibleId = QQHelper.DefaultMBREquipmentBreakdownDeductibleId().ToString
            Else
                MyLocation.EquipmentBreakdownDeductibleId = ""
            End If
        End If

        ' Location info that's based on the first building on the location
        If MyLocation IsNot Nothing Then
            Dim hasBlanket As Boolean = False
            If SubQuoteFirst.HasBlanketBuilding OrElse SubQuoteFirst.HasBlanketBuildingAndContents OrElse SubQuoteFirst.HasBlanketBusinessIncome OrElse SubQuoteFirst.HasBlanketContents Then
                hasBlanket = True
                MyLocation.DeductibleId = Me.Quote.Locations(0).DeductibleId
                MyLocation.CauseOfLossTypeId = Me.Quote.Locations(0).CauseOfLossTypeId
                MyLocation.CoinsuranceTypeId = Me.Quote.Locations(0).CoinsuranceTypeId
                MyLocation.ValuationMethodTypeId = Me.Quote.Locations(0).ValuationMethodTypeId
            End If

            If MyLocation.Buildings.HasItemAtIndex(0) Then
                MyLocation.EarthquakeApplies = MyLocation.Buildings(0).EarthquakeApplies

                If hasBlanket = False OrElse QQHelper.IsPositiveIntegerString(MyLocation.CauseOfLossTypeId) = False Then
                    If QQHelper.IsPositiveIntegerString(MyLocation.Buildings(0).CauseOfLossTypeId) = True Then
                        If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                            MyLocation.DeductibleId = Me.ddLocationPropertyDeductible.SelectedValue
                        Else
                            MyLocation.DeductibleId = MyLocation.Buildings(0).DeductibleId
                        End If
                        MyLocation.CauseOfLossTypeId = MyLocation.Buildings(0).CauseOfLossTypeId
                        MyLocation.CoinsuranceTypeId = MyLocation.Buildings(0).CoinsuranceTypeId
                        MyLocation.ValuationMethodTypeId = MyLocation.Buildings(0).ValuationMethodId
                    ElseIf QQHelper.IsPositiveIntegerString(MyLocation.Buildings(0).PersPropCov_CauseOfLossTypeId) = True Then
                        If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                            MyLocation.DeductibleId = Me.ddLocationPropertyDeductible.SelectedValue
                        Else
                            MyLocation.DeductibleId = MyLocation.Buildings(0).PersPropCov_DeductibleId
                        End If
                        MyLocation.CauseOfLossTypeId = MyLocation.Buildings(0).PersPropCov_CauseOfLossTypeId
                        MyLocation.CoinsuranceTypeId = MyLocation.Buildings(0).PersPropCov_CoinsuranceTypeId
                        MyLocation.ValuationMethodTypeId = MyLocation.Buildings(0).PersPropCov_ValuationId
                    ElseIf QQHelper.IsPositiveIntegerString(MyLocation.Buildings(0).PersPropOfOthers_CauseOfLossTypeId) = True Then
                        If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                            MyLocation.DeductibleId = Me.ddLocationPropertyDeductible.SelectedValue
                        Else
                            MyLocation.DeductibleId = MyLocation.Buildings(0).PersPropOfOthers_DeductibleId
                        End If
                        MyLocation.CauseOfLossTypeId = MyLocation.Buildings(0).PersPropOfOthers_CauseOfLossTypeId
                        MyLocation.CoinsuranceTypeId = MyLocation.Buildings(0).PersPropOfOthers_CoinsuranceTypeId
                        MyLocation.ValuationMethodTypeId = MyLocation.Buildings(0).PersPropOfOthers_ValuationId
                    ElseIf QQHelper.IsPositiveIntegerString(MyLocation.Buildings(0).BusinessIncomeCov_CauseOfLossTypeId) = True Then
                        'MyLocation.DeductibleId = MyLocation.Buildings(0).BusinessIncomeCov_DeductibleId
                        MyLocation.CauseOfLossTypeId = MyLocation.Buildings(0).BusinessIncomeCov_CauseOfLossTypeId
                        MyLocation.CoinsuranceTypeId = MyLocation.Buildings(0).BusinessIncomeCov_CoinsuranceTypeId
                        'MyLocation.ValuationMethodTypeId = MyLocation.Buildings(0).BusinessIncomeCov_ValuationId
                    End If
                End If
            End If
        End If

        ' Set the location deductible and the equipment breakdown deductibles.
        ' On Endorsements we only want to do this when the location is new.
        ' Only perform this logic if the EQBrkDefaultLogicEnabled flag is set to true.
        Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
        Dim dedId As String = ""

        If (Me.IsQuoteEndorsement = True AndAlso endorsementsPreexistHelper.IsPreexistingLocation(MyLocationIndex) = False) OrElse Me.IsQuoteEndorsement() = False Then
            If MyLocation IsNot Nothing Then
                If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                    dedId = Me.ddLocationPropertyDeductible.SelectedValue
                Else
                    dedId = GetLocationDeductibleId()
                End If
            End If
            MyLocation.DeductibleId = dedId
        End If

        If EQBrkDEfaultLogicEnabled Then
            If (Me.IsQuoteEndorsement = True AndAlso endorsementsPreexistHelper.IsPreexistingLocation(MyLocationIndex) = False) OrElse Me.IsQuoteEndorsement() = False Then
                ' Get the location deductible - Calculates based on the location's building's coverages

                ' Set the Equipment Breakdown and it's sub-coverage deductible values,
                ' uses the same deductible we just set on the location to above.
                If chkEquipmentBreakdown.Checked AndAlso MyLocation IsNot Nothing Then
                    'MyLocation.EquipmentBreakdownDeductibleId = QQHelper.DefaultMBREquipmentBreakdownDeductibleId().ToString
                    MyLocation.EquipmentBreakdownDeductibleId = dedId
                    MyLocation.EquipmentBreakdown_MBR_SpoilageDeductibleId = dedId
                    MyLocation.EquipmentBreakdown_MBR_PollutantCleanupRemovalDeductibleId = dedId
                    MyLocation.EquipmentBreakdown_MBR_RefrigerantContaminationDeductibleId = dedId
                Else
                    MyLocation.EquipmentBreakdownDeductibleId = ""
                    MyLocation.EquipmentBreakdown_MBR_SpoilageDeductibleId = ""
                    MyLocation.EquipmentBreakdown_MBR_PollutantCleanupRemovalDeductibleId = ""
                    MyLocation.EquipmentBreakdown_MBR_RefrigerantContaminationDeductibleId = ""
                End If
            End If
        End If

        Me.SaveChildControls()
        Return True
    End Function

    ''' <summary>
    ''' Determines the location deductible based on the building, personal property, 
    ''' and personal property of others coverages.  Prioritizes these coverages, 
    ''' highest to lowest:
    '''     - Building Coverage
    '''     - Personal Property Coverage
    '''     - Personal Property of Others Coverage
    '''     
    ''' Note that this deductible will also be applied to Equipment breakdown and it's
    ''' sub-coverages.
    ''' </summary>
    ''' <returns></returns>
    Private Function GetLocationDeductibleId() As String
        Dim dedId As String = ""
        Dim bldDedValue As Integer = 0
        Dim ppDedValue As Integer = 0
        Dim ppoDedValue As Integer = 0

        If MyLocation.Buildings IsNot Nothing AndAlso MyLocation.Buildings.Count > 0 Then
            ' BUILDING COVERAGE LOOP
            For Each bld As QuickQuote.CommonObjects.QuickQuoteBuilding In MyLocation.Buildings
                ' Check if the building has building coverage, if so set the variables if needed
                If bld.DeductibleId.IsNullEmptyorWhitespace = False AndAlso IsNumeric(bld.DeductibleId) Then
                    If IsNumeric(bld.Deductible) Then
                        If CInt(bld.Deductible) > bldDedValue Then
                            ' Only update the variables if the deductible is greater than the 
                            ' stored value.
                            bldDedValue = CInt(bld.Deductible)
                            dedId = bld.DeductibleId
                        End If
                    End If
                End If
            Next

            ' If there were no building coverages, and thus no deductible set yet, 
            ' check for Personal Property coverage and attempt to set the deductible 
            ' from there.
            ' PERSONAL PROPERTY LOOP
            If bldDedValue = 0 Then
                For Each bld As QuickQuote.CommonObjects.QuickQuoteBuilding In MyLocation.Buildings
                    ' Check if the building has personal property coverage, if so set the variables if needed
                    If bld.PersPropCov_DeductibleId.IsNullEmptyorWhitespace = False AndAlso IsNumeric(bld.PersPropCov_DeductibleId) Then
                        If IsNumeric(bld.PersPropCov_Deductible) Then
                            If CInt(bld.PersPropCov_Deductible) > ppDedValue Then
                                ' Only update the variables if the deductible is greater than the 
                                ' stored value.
                                ppDedValue = CInt(bld.PersPropCov_Deductible)
                                dedId = bld.PersPropCov_DeductibleId
                            End If
                        End If
                    End If
                Next
            End If

            ' If there were no building or personal property coverages, and thus no
            ' deductible set yet, check for Personal Property Of Others coverage and
            ' attempt to set the deductible from there.
            ' PERSONAL PROPERTY OF OTHERS LOOP
            If bldDedValue = 0 AndAlso ppDedValue = 0 Then
                For Each bld As QuickQuote.CommonObjects.QuickQuoteBuilding In MyLocation.Buildings
                    ' Check if the building has building coverage, if so set the variables if needed
                    If bld.PersPropOfOthers_DeductibleId.IsNullEmptyorWhitespace = False AndAlso IsNumeric(bld.PersPropOfOthers_DeductibleId) Then
                        If IsNumeric(bld.PersPropOfOthers_Deductible) Then
                            If CInt(bld.PersPropOfOthers_Deductible) > ppoDedValue Then
                                ' Only update the variables if the deductible is greater than the 
                                ' stored value.
                                ppoDedValue = CInt(bld.PersPropOfOthers_Deductible)
                                dedId = bld.PersPropOfOthers_DeductibleId
                            End If
                        End If
                    End If
                Next
            End If

            ' The deductible ID will be set if there are any coverages on the building.  If no coverages
            ' have been set yet we need to default the deductible.
            Dim dedVal As Integer = 0
            If dedId = "" Then
                dedId = "9"  ' default to 500 changed to 1000 (id 9) per task 62836
                dedVal = 1000
            Else
                dedVal = bldDedValue + ppDedValue + ppoDedValue
            End If
        End If

        Return dedId
    End Function

    Private Function RiskGradeEligibleForEquipmentBreakdown() As Boolean
        Dim ccs As String = "97220, 59977, 59975, 59973, 59970, 59941, 59923, 59915, 59914, 59892, 59889, 59806, 59798, 59790, 59784, 59783, 59782, 59781, 59701, 59695, 59661, 59660, 59058, 59057, 58922, 58904, 58903, 58813, 58713, 58663, 58627, 58575, 58561, 58560, 58559, 58532, 58009, 57999, 57998, 57625, 57600, 57572, 57401, 57257, 57202, 56919, 56918, 56917, 56916, 56915, 56913, 56912, 56911, 56910, 56900, 56808, 56807, 56806, 56805, 56690, 56654, 56653, 56652, 56651, 56650, 56391, 56390, 56171, 56042, 56041, 55802, 55649, 55648, 55647, 55013, 55012, 55011, 55010, 54444, 53803, 53425, 53403, 53333, 53271, 53229, 53147, 52744, 52619, 52581, 52547, 52505, 52469, 52467, 52440, 52435, 52433, 52432, 52401, 52137, 51999, 51941, 51927, 51926, 51900, 51889, 51767, 51734, 51703, 51702, 51666, 51625, 51613, 51500, 51401, 51400, 51380, 51370, 51333, 51330, 51255, 51254, 51253, 51252, 51251, 51250, 51224, 51222, 51221, 51220, 51211, 51206, 51205, 51201, 51116, 51001, 56170, 59932, 59931, 59867, 59223, 58837, 58756, 58056, 57690, 56920, 56567, 55426, 53905, 53904, 53903, 53902, 53901, 53077, 52876, 52343, 52342, 52341, 52076, 52075, 51970, 51919, 51909, 51857, 51856, 51855, 51854, 51853, 51852, 51851, 51850, 51833, 51790, 51241, 51240, 50045, 15733, 59482, 59481, 56980, 56427, 55715, 55214, 51959, 51958, 51957, 51956, 51934, 51877, 51869, 51809, 51808, 51600, 50017, 50017, 50015, 50010, 59751, 59750, 59257, 59005, 58759, 58096, 58095, 58058, 58057, 58020, 58010, 56040, 55918, 55718, 55717, 55597, 52402, 51554, 51553, 51340, 49239, 99303, 53121, 98710, 98555, 98430, 98429, 98428, 98427, 98162, 98161, 98160, 98159, 98158, 98157, 98156, 98155, 98154, 98153, 98151, 98150, 98003, 98002, 97111, 95358, 95357, 95306, 95305, 95233, 92453, 92445, 92102, 92101, 92055, 92054, 92053, 91210, 59985, 59984, 59947, 59647, 59537, 59189, 59188, 58873, 58503, 58302, 58301, 57726, 57611, 54077, 54012, 52967, 52150, 51552, 51551, 51550, 51230, 47147, 47146, 44010, 43991, 43990, 43946, 43945, 43822"
        Dim ccarray As String() = ccs.Split(",")

        ' Default to show all the specific rates rows with the checkboxes checked
        chkEquipmentBreakdown.Enabled = True

        ' Get the quote's risk grade class code
        Dim cc As String = GetRiskGradeClassCode(Quote.RiskGradeLookupId)

        ' If the quote risk grade class code is one in the list above then disable and uncheck equipment breakdown
        For Each c As String In ccarray
            If cc = c.Trim Then
                Return False
            End If
        Next

        ' Risk grade class code not in list - eligible for EB
        Return True
    End Function
    Private Function GetRiskGradeClassCode(ByVal riskId As String) As String
        'Dim conn As New System.Data.SqlClient.SqlConnection
        'Dim cmd As New System.Data.SqlClient.SqlCommand
        'Dim rtn As Object = Nothing

        Try
            Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
            Dim spm As New IFM.VR.Common.Helpers.SPManager("connDiamondReports", "usp_Get_RiskGradeLookup")
            spm.AddIntegerParamater("@Id", qqh.IntegerForString(riskId))
            Dim tbl As DataTable = spm.ExecuteSPQuery()

            If tbl IsNot Nothing AndAlso tbl.Rows.Count > 0 Then
                Return tbl.Rows(0)("glclasscode").ToString()
            Else
                Return ""
            End If

            'conn.ConnectionString = System.Configuration.ConfigurationManager.AppSettings("connDiamond")
            'conn.Open()
            'cmd.Connection = conn
            'cmd.CommandType = CommandType.Text
            'cmd.CommandText = "SELECT glclasscode FROM RiskGradeLookup WHERE riskgradelookup_id = " & riskId
            'rtn = cmd.ExecuteScalar()
            'If rtn IsNot Nothing Then Return rtn.ToString() Else Return ""
        Catch ex As Exception
            Return ""
        Finally
            'If conn.State = ConnectionState.Open Then conn.Close()
            'conn.Dispose()
            'cmd.Dispose()
        End Try
    End Function


    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    Protected Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
        If isWindHailEnabled Then
            If ddWindHailDeductible.Items IsNot Nothing AndAlso ddWindHailDeductible.Items.Count > 0 Then
                ddWindHailDeductible.SelectedIndex = 0
            End If
        End If
        If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
            If ddLocationPropertyDeductible.Items IsNot Nothing AndAlso ddLocationPropertyDeductible.Items.Count > 0 Then
                ddLocationPropertyDeductible.SelectedValue = "9" ' 9 = 1,000
            End If
        End If

        chkEquipmentBreakdown.Checked = False
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Sub DefaultWindHailDropDown()
        If MyLocation.Buildings IsNot Nothing AndAlso MyLocation.Buildings.Count > 0 Then
            For Each b As QuickQuoteBuilding In MyLocation.Buildings
                If b IsNot Nothing AndAlso Not WindHailDefaultingHelper.CheckCPRCPPExemptCodes(b) Then
                    If IsNullEmptyorWhitespace(MyLocation.WindHailDeductibleLimitId) = False Then
                        Dim removeNA As ListItem = Nothing
                        If b.OwnerOccupiedPercentageId <> "" AndAlso (b.OwnerOccupiedPercentageId = "30" OrElse b.OwnerOccupiedPercentageId = "31") Then
                            If Me.ddWindHailDeductible.SelectedValue = "0" Then '0 = N/A
                                SetFromValue(ddWindHailDeductible, MyLocation.WindHailDeductibleLimitId, "32")
                            End If
                            removeNA = ddWindHailDeductible.Items.FindByValue("0")
                            If removeNA IsNot Nothing Then
                                Me.ddWindHailDeductible.Items.Remove(removeNA)
                            End If
                        End If
                        Exit For
                    End If
                End If
            Next
        End If
    End Sub

End Class