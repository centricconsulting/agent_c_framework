Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager

Public Class ctlBarnsBuildingsList
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

    Private Const ClassName As String = "ctlBarnsBuildingsList"
    Private NumFormatNoCents As String = "###,###,##0"  ' No dollar sign 9/16/15 MGB
    Private NumFormatWithCents As String = "$###,###,###.00"
    'Dim BuildingTable = {(New With {.Address = "", .LocationIndex = "", .BuildingIndex = "", .Building = "", .BuildingType = "", .Construction = "", .Limit = "", .Deductible = "", .Premium = ""})}.Take(0).ToList()
    'Dim CoverageTable = {(New With {.LocationIndex = "", .BuildingIndex = "", .CoverageName = "", .Limit = "", .Premium = "", .LossExt = "", .CoIns = ""})}.Take(0).ToList()

    Public Event ToggleBuilding(state As Boolean)

    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

    Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject Implements IVRUI_P.Quote
        Get
            'Dim errCreateQSO As String = ""
            'If IsAppPageMode Then
            '    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.Farm, QuickQuoteXML.QuickQuoteSaveType.AppGap)
            'Else
            '    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.Farm)
            'End If
            Return New QuickQuote.CommonObjects.QuickQuoteObject
        End Get
    End Property

    Protected ReadOnly Property QuoteId As String Implements IVRUI_P.QuoteId
        Get
            If Request.QueryString("quoteid") IsNot Nothing Then
                Return Request.QueryString("quoteid")
            End If
            If Page.RouteData.Values("quoteid") IsNot Nothing Then
                Return Page.RouteData.Values("quoteid").ToString()
            End If
            Return ""
        End Get
    End Property

    Public Property LocalQuickQuote() As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            Return Session("sess_LocalQuickQuote")
        End Get
        Set(ByVal value As QuickQuote.CommonObjects.QuickQuoteObject)
            Session("sess_LocalQuickQuote") = value
        End Set
    End Property

    Public Property ValidationHelper As ControlValidationHelper Implements IVRUI_P.ValidationHelper
        Get
            If ViewState("vs_valHelp") Is Nothing Then
                ViewState("vs_valHelp") = New ControlValidationHelper
            End If
            Return DirectCast(ViewState("vs_valHelp"), ControlValidationHelper)
        End Get
        Set(value As ControlValidationHelper)
            ViewState("vs_valHelp") = value
        End Set
    End Property

    Public ReadOnly Property IsAppPageMode As Boolean
        Get
            If TypeOf Me.Page Is VR3FarmApp Then
                Return True
            End If
            Return False
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

    Public Property AllBuildingsList() As DataTable
        Get
            Return ViewState("vs_AllBuildings")
        End Get
        Set(ByVal value As DataTable)
            ViewState("vs_AllBuildings") = value
        End Set
    End Property

    Public Property BldngCoverageList() As DataTable
        Get
            Return ViewState("vs_BldngCoverage")
        End Get
        Set(ByVal value As DataTable)
            ViewState("vs_BldngCoverage") = value
        End Set
    End Property

    Public Property BuildNum() As Integer
        Get
            Return ViewState("vs_BuildNum")
        End Get
        Set(ByVal value As Integer)
            ViewState("vs_BuildNum") = value
        End Set
    End Property




    'Public ReadOnly Property LocationList() As List(Of QuickQuoteLocation)
    '    Get
    '        Return Session("sess_LocationList")
    '    End Get
    'End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Populate()
        End If
    End Sub

    Protected Sub ValidateForm() Implements IVRUI_P.ValidateForm

    End Sub

    Private Function CreateBuildingDataTable() As DataTable
        Dim dtBuilding As New DataTable
        dtBuilding.Columns.Add("LocationAddress", GetType(String))
        dtBuilding.Columns.Add("StructType", GetType(String))
        dtBuilding.Columns.Add("StructTypeDwellingContents", GetType(String)) 'added 10/26/2020 (Interoperability)
        dtBuilding.Columns.Add("BuildingType", GetType(String))
        dtBuilding.Columns.Add("Construction", GetType(String))
        dtBuilding.Columns.Add("Limit", GetType(String))
        dtBuilding.Columns.Add("DwellingContentsLimit", GetType(String)) 'added 10/26/2020 (Interoperability)
        dtBuilding.Columns.Add("Deductible", GetType(String))
        dtBuilding.Columns.Add("Premium", GetType(String))
        dtBuilding.Columns.Add("DwellingContentsPremium", GetType(String)) 'added 10/26/2020 (Interoperability)
        dtBuilding.Columns.Add("LocationNum", GetType(String))
        dtBuilding.Columns.Add("BuildingNum", GetType(String))

        Return dtBuilding
    End Function

    Private Function CreateBuildingCoverageDataTable() As DataTable
        Dim dtBldngCoverage As New DataTable
        dtBldngCoverage.Columns.Add("CoverageCodeId", GetType(String))
        dtBldngCoverage.Columns.Add("CoverageName", GetType(String))
        dtBldngCoverage.Columns.Add("CoverageLimit", GetType(String))
        dtBldngCoverage.Columns.Add("CoveragePrem", GetType(String))
        dtBldngCoverage.Columns.Add("LocationNum", GetType(String))
        dtBldngCoverage.Columns.Add("BuildingNum", GetType(String))
        dtBldngCoverage.Columns.Add("LossExt", GetType(String))
        dtBldngCoverage.Columns.Add("CoIns", GetType(String))
        dtBldngCoverage.Columns.Add("Description", GetType(String))

        Return dtBldngCoverage
    End Function

    Protected Sub Populate() Implements IVRUI_P.Populate
        dlBuilding.DataSource = Nothing
        AllBuildingsList = CreateBuildingDataTable()
        BldngCoverageList = CreateBuildingCoverageDataTable()
        Dim locationIdx = 0
        Dim farmBuildingNum = 1
        Dim terr As String = ""
        Dim txt As String = ""
        Dim prem As String = ""
        Dim BuildingIndex As Integer = -1
        Dim LocationIndex As Integer = -1

        'Dim BuildingTable = {(New With {.Address = "", .LocationIndex = "", .Building = "", .BuildingType = "", .Construction = "", .Limit = "", .Deductible = "", .Premium = ""})}.Take(0).ToList()
        'Dim CoverageTable = {(New With {.LocationIndex = "", .BuildingIndex = "", .CoverageName = "", .Limit = "", .Premium = "", .LossExt = "", .CoIns = ""})}.Take(0).ToList()

        If LocalQuickQuote IsNot Nothing AndAlso LocalQuickQuote.Locations IsNot Nothing Then
            'Dim index As Int32 = 1
            For Each Loc As QuickQuoteLocation In LocalQuickQuote.Locations
                LocationIndex += 1
                BuildingIndex = -1
                ''Updated 9/11/18 for multi state MLW - removed 9/20/19 since it is not needed because quoteForLoc is not being used. MLW
                'Dim SubQuotes = Me.qqHelper.MultiStateQuickQuoteObjects(Me.Quote)
                'If SubQuotes IsNot Nothing Then
                'Dim quoteForLoc As QuickQuoteObject = qqHelper.StateQuoteForLocation(SubQuotes, Loc, alwaysReturnQuoteIfPossibleOnNoMatch:=True) 'Removed 9/19/18 since it is not currently being used. May need to revisit since the commented code takes into account the VersionId. MLW
                For Each bld As QuickQuoteBuilding In Loc.Buildings
                    BuildingIndex += 1
                    ' Building Info
                    Dim address As String = FormatLocationAddress(Loc)
                    Dim construction As String = GetFarmConstructionTypeDescription(bld.ConstructionId)
                    Dim BuildingType As String = qqHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.FarmTypeId, bld.FarmTypeId)
                    'Dim structureType As String = GetFarmStructureTypeDescription(bld.FarmStructureTypeId)
                    Dim buildingname As String = GetFarmStructureTypeDescription(bld.FarmStructureTypeId)
                    'Added 5/21/18 for Bug 20408 MLW - shows building name and description
                    Dim buildingdesc As String = ""
                    buildingdesc = bld.Description
                    Dim buildingNameAndDesc As String = buildingname
                    If buildingdesc <> "" Then
                        buildingdesc = StrConv(buildingdesc, VbStrConv.ProperCase)
                        buildingNameAndDesc = buildingname & " - " & buildingdesc
                    End If
                    ' Changed deductible to use E deductible 7/21/2015
                    If Loc.TerritoryNumber IsNot Nothing AndAlso Loc.TerritoryNumber <> String.Empty Then terr = Loc.TerritoryNumber
                    Dim deductible As String = qqHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.E_Farm_DeductibleLimitId, bld.E_Farm_DeductibleLimitId)
                    If IsNumeric(deductible) Then
                        deductible = Format(CDec(deductible), NumFormatNoCents)
                    End If

                    'Updated 5/21/18 for Bug 20408 MLW - shows building name and description
                    'AllBuildingsList.Rows.Add(address, qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.FarmStructureTypeId, bld.FarmStructureTypeId),
                    '                                      qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.FarmTypeId, bld.FarmTypeId),
                    '                                      qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionId, bld.ConstructionId),
                    '                                      bld.E_Farm_Limit, qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.E_Farm_DeductibleLimitId, bld.E_Farm_DeductibleLimitId),
                    '                                      bld.E_Farm_QuotedPremium, Loc.LocationNum, farmBuildingNum.ToString())
                    'AllBuildingsList.Rows.Add(address, buildingNameAndDesc,
                    '                                      qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.FarmTypeId, bld.FarmTypeId),
                    '                                      qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionId, bld.ConstructionId),
                    '                                      bld.E_Farm_Limit, qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.E_Farm_DeductibleLimitId, bld.E_Farm_DeductibleLimitId),
                    '                                      bld.E_Farm_QuotedPremium, Loc.LocationNum, farmBuildingNum.ToString())

                    'AllBuildingsList.Rows.Add(address, buildingNameAndDesc,
                    '                                      qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.FarmTypeId, bld.FarmTypeId),
                    '                                      qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionId, bld.ConstructionId),
                    '                                      bld.E_Farm_Limit, qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.E_Farm_DeductibleLimitId, bld.E_Farm_DeductibleLimitId),
                    '                                      bld.E_Farm_QuotedPremium, LocationIndex.ToString, BuildingIndex.ToString)
                    'updated 10/26/2020 (Interoperability)
                    Dim dwellingContentsNameAndDesc As String = buildingname & " Contents"
                    If dwellingContentsNameAndDesc.Contains("Farm Dwelling") OrElse dwellingContentsNameAndDesc.Contains("Mobile Home Dwelling") OrElse dwellingContentsNameAndDesc.Contains("Outbuilding") Then
                        dwellingContentsNameAndDesc = "Household Contents"
                        If String.IsNullOrWhiteSpace(buildingdesc) = False Then
                            dwellingContentsNameAndDesc &= " - " & buildingdesc
                        End If
                    End If
                    AllBuildingsList.Rows.Add(address, buildingNameAndDesc, dwellingContentsNameAndDesc,
                                                          qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.FarmTypeId, bld.FarmTypeId),
                                                          qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionId, bld.ConstructionId),
                                                          bld.E_Farm_Limit, bld.HouseholdContentsLimit, qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.E_Farm_DeductibleLimitId, bld.E_Farm_DeductibleLimitId),
                                                          bld.E_Farm_QuotedPremium, bld.HouseholdContentsQuotedPremium, LocationIndex.ToString, BuildingIndex.ToString)

                    ' COVERAGES

                    ' (no optionalcoverages in this section)

                    ' OptionalCoverageEs
                    For Each Cov As QuickQuoteOptionalCoverageE In bld.OptionalCoverageEs
                        Dim limit As String = "N/A"
                        If Cov.IncreasedLimit IsNot Nothing AndAlso IsNumeric(Cov.IncreasedLimit) AndAlso CDec(Cov.IncreasedLimit) > 0 Then limit = Cov.IncreasedLimit
                        If IsNumeric(limit) Then limit = Format(CDec(limit), NumFormatNoCents)
                        'Removed 9/19/18 since it is not currently being used. May need to revisit since the commented code takes into account the VersionId. MLW
                        'Dim dscr As String = ""
                        'If Cov.Description IsNot Nothing AndAlso Cov.Description.Trim <> String.Empty Then
                        '    dscr = Cov.Description
                        'Else
                        '    ' Pull the caption from diamond
                        '    'TODO: Mary - ask Don - this isn't used?
                        '    dscr = GetCoverageCodeCaption(Cov.CoverageCodeId, stateQuote:=quoteForLoc) 'Updated 9/12/18 with new parameter, stateQuote, for multi state MLW
                        'End If

                        prem = Format(CDec(Cov.Premium), NumFormatWithCents)
                        'BldngCoverageList.Rows.Add(FarmSummaryHelper.GetCoverageCodeCaption(Cov.CoverageCodeId), If(qqHelper.IsZeroAmount(Cov.IncreasedLimit), "N/A", Cov.IncreasedLimit), If(qqHelper.IsZeroAmount(Cov.Premium), "Included", Cov.Premium), Loc.LocationNum, farmBuildingNum.ToString(), "", "", "")
                        BldngCoverageList.Rows.Add(Cov.CoverageCodeId, FarmSummaryHelper.GetCoverageCodeCaption(Cov.CoverageCodeId, LocalQuickQuote, Loc), If(qqHelper.IsZeroAmount(Cov.IncreasedLimit), "N/A", Cov.IncreasedLimit), If(qqHelper.IsZeroAmount(Cov.Premium), "Included", Cov.Premium), LocationIndex.ToString, BuildingIndex.ToString, "", "", "")
                    Next

                    ' Loss Income Coverage
                    Dim lossIncome = Loc.IncomeLosses.Find(Function(p) p.Description.Trim() = String.Format("LOC{0}BLD{1}", Loc.LocationNum, farmBuildingNum.ToString()))
                    If lossIncome IsNot Nothing Then
                        Dim covCodeId As String = ""
                        If lossIncome.Coverage IsNot Nothing Then
                            covCodeId = lossIncome.Coverage.CoverageCodeId
                        End If
                        BldngCoverageList.Rows.Add(covCodeId, "Loss of Income", If(qqHelper.IsZeroAmount(lossIncome.Limit), "N/A", lossIncome.Limit), If(qqHelper.IsZeroAmount(lossIncome.QuotedPremium), "Included", lossIncome.QuotedPremium), LocationIndex, BuildingIndex, lossIncome.ExtendFarmIncomeOptionId, lossIncome.CoinsuranceTypeId, lossIncome.Description)
                        'BldngCoverageList.Rows.Add("Loss of Income", If(qqHelper.IsZeroAmount(lossIncome.Limit), "N/A", lossIncome.Limit), If(qqHelper.IsZeroAmount(lossIncome.QuotedPremium), "Included", lossIncome.QuotedPremium), Loc.LocationNum, farmBuildingNum.ToString(), lossIncome.ExtendFarmIncomeOptionId, lossIncome.CoinsuranceTypeId, lossIncome.Description)
                        'CoverageTable.Add(New With {.LocationIndex = Loc.LocationNum, .BuildingIndex = index.ToString, .CoverageName = "Loss Income", .Limit = lossIncome.Limit, .Premium = lossIncome.QuotedPremium, .lossExt = lossIncome.ExtendFarmIncomeOptionId, .coIns = lossIncome.CoinsuranceTypeId})
                    End If

                    ' Heated Building Surcharge
                    If bld.HeatedBuildingSurchargeGasElectric OrElse bld.HeatedBuildingSurchargeOther Then
                        BldngCoverageList.Rows.Add("", "Heated Building Surcharge applies", "N/A", "Included", LocationIndex, BuildingIndex, "", "", "")
                        'BldngCoverageList.Rows.Add("Heated Building Surcharge applies", "N/A", "Included", Loc.LocationNum, farmBuildingNum.ToString(), "", "", "")
                        'CoverageTable.Add(New With {.LocationIndex = Loc.LocationNum, .BuildingIndex = index.ToString, .CoverageName = "Heated Building Surcharge Applies", .Limit = "N/A", .Premium = "", .lossExt = "", .CoIns = ""})
                    End If
                Next
            Next

            If AllBuildingsList.Rows.Count = 0 Then
                RaiseEvent ToggleBuilding(False)
            Else
                dlBuilding.DataSource = AllBuildingsList
                dlBuilding.DataBind()
                RaiseEvent ToggleBuilding(True)
            End If
        End If
    End Sub

    Private Sub dlAddlDwelling_ItemDataBound(sender As Object, e As DataListItemEventArgs) Handles dlBuilding.ItemDataBound
        If LocalQuickQuote IsNot Nothing AndAlso LocalQuickQuote.Locations IsNot Nothing Then
            Dim building As ctlBarnsBuildings = e.Item.FindControl("ctlBarnsBuildings")
            Dim currLoc As Integer = 1

            If (e.Item.ItemIndex - 1) < 0 Then
                BuildNum = 0
                Dim currBldg = AllBuildingsList.Rows(e.Item.ItemIndex)
                currLoc = Integer.Parse(currBldg("LocationNum"))
            Else
                Dim currBldg = AllBuildingsList.Rows(e.Item.ItemIndex)
                Dim prevBldg = AllBuildingsList.Rows(e.Item.ItemIndex - 1)
                Dim prevLoc = Integer.Parse(prevBldg("LocationNum"))
                currLoc = Integer.Parse(currBldg("LocationNum"))

                If currLoc.ToString() <> prevLoc.ToString() Then
                    BuildNum = 0
                Else
                    BuildNum += 1
                End If
            End If

            building.LocationNumber = currLoc
            building.BuildingNumber = BuildNum
            building.AllBuildingsList = AllBuildingsList
            building.BldngCoverageList = BldngCoverageList
            building.Populate()
        End If
    End Sub

    Protected Sub LoadStaticData() Implements IVRUI_P.LoadStaticData

    End Sub

    Protected Function Save() As Boolean Implements IVRUI_P.Save
        Return False
    End Function

    Private Function FormatLocationAddress(ByVal Loc As QuickQuoteLocation) As String
        Try
            Dim zip As String = Loc.Address.Zip
            If zip.Length > 5 Then
                zip = zip.Substring(0, 5)
            End If
            Return String.Format("{0} {1} {2} {3} {4} {5} {6}", Loc.Address.HouseNum, Loc.Address.StreetName, If(String.IsNullOrWhiteSpace(Loc.Address.ApartmentNumber) = False, "Apt# " + Loc.Address.ApartmentNumber, ""), Loc.Address.POBox, Loc.Address.City, Loc.Address.State, zip).Replace("  ", " ").Trim()
        Catch ex As Exception
            'HandleError(ClassName, "FormatLocationAddress", ex, lblMsg)
            Return ""
        End Try
    End Function

    Private Function GetFarmConstructionTypeDescription(ByVal FarmConstructionType_Id As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing
        Try
            conn = New SqlConnection(AppSettings("connDiamond"))
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT dscr FROM FarmConstructionType WHERE FarmConstructionType_Id = " & qqHelper.IntegerForString(FarmConstructionType_Id)
            rtn = cmd.ExecuteScalar()
            If rtn Is Nothing Then Throw New Exception("Returned Farm Construction Type value is nothing!")
            Return rtn.ToString
        Catch ex As Exception
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    Private Function GetFarmStructureTypeDescription(ByVal FarmStructureType_Id As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing
        Try
            conn = New SqlConnection(AppSettings("connDiamond"))
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT dscr FROM FarmStructureType WHERE FarmStructureType_Id = " & qqHelper.IntegerForString(FarmStructureType_Id)
            rtn = cmd.ExecuteScalar()
            If rtn Is Nothing Then Throw New Exception("Returned Farm Structure Type value is nothing!")
            Return rtn.ToString
        Catch ex As Exception
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    Private Function GetCoverageCodeCaption(ByVal CoverageCode_Id As String, Optional ByVal RemoveIncreasedLimitsText As Boolean = True, Optional ByVal stateQuote As QuickQuoteObject = Nothing) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim desc As String = ""
        Dim qqVersionId As String = ""
        Dim sql As String = ""

        Try
            ' Get the quickquote version id from the quote object
            'Updated 9/12/18 for multi state MLW
            'qqVersionId = Quote.VersionId
            If stateQuote IsNot Nothing Then
                qqVersionId = stateQuote.VersionId
            End If
            If qqHelper.IsPositiveIntegerString(qqVersionId) = False Then
                qqVersionId = Quote.VersionId
            End If

            If qqVersionId Is Nothing OrElse qqVersionId = String.Empty Then Throw New Exception("Error getting version id")

            ' Get the caption
            conn = New SqlConnection(AppSettings("connDiamond"))
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            '            cmd.CommandText = "SELECT coveragecodeversion_id, caption FROM CoverageCodeVersion WHERE CoverageCode_Id = " & CoverageCode_Id & " ORDER BY CoverageCodeVersion_id DESC"
            sql = "SELECT COALESCE(CCV.caption, CC.dscr) AS CoverageDescription, * FROM CoverageCode AS cc WITH (NOLOCK) "
            sql += "LEFT JOIN CoverageCodeVersion AS ccv WITH (NOLOCK) ON ccv.coveragecode_id = cc.coveragecode_id "
            sql += "AND ccv.version_id = " & qqHelper.IntegerForString(qqVersionId) & " WHERE cc.coveragecode_id = " & qqHelper.IntegerForString(CoverageCode_Id)
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)
            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Return ""

            desc = tbl(tbl.Rows.Count - 1)("caption").ToString()
            If RemoveIncreasedLimitsText Then desc = desc.Replace("Increased Limits", "")

            Return desc
        Catch ex As Exception
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            tbl.Dispose()
            da.Dispose()
        End Try
    End Function
End Class