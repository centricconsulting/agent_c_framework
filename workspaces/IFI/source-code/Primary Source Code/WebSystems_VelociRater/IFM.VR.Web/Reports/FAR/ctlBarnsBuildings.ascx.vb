Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods

Public Class ctlBarnsBuildings
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

    Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject Implements IVRUI_P.Quote
        Get
            'Dim errCreateQSO As String = ""
            'If IsAppPageMode Then
            '    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.Farm, QuickQuoteXML.QuickQuoteSaveType.AppGap)
            'Else
            '    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.Farm)
            'End If

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

    Public Property LocationNumber As Int32
        Get
            If ViewState("vs_locationNumber") Is Nothing Then
                ViewState("vs_locationNumber") = 0
            End If
            Return CInt(ViewState("vs_locationNumber"))
        End Get
        Set(value As Int32)
            ViewState("vs_locationNumber") = value
        End Set
    End Property

    Public Property BuildingNumber As Int32
        Get
            If ViewState("vs_buildingNumber") Is Nothing Then
                ViewState("vs_buildingNumber") = 0
            End If
            Return CInt(ViewState("vs_buildingNumber"))
        End Get
        Set(value As Int32)
            ViewState("vs_buildingNumber") = value
        End Set
    End Property

    Public Property AllBuildingsObject() As Object
        Get
            Return ViewState("vs_AllBuildings")
        End Get
        Set(ByVal value As Object)
            ViewState("vs_AllBuildings") = value
        End Set
    End Property

    Public Property BldngCoverageObject() As Object
        Get
            Return ViewState("vs_BldngCoverage")
        End Get
        Set(ByVal value As Object)
            ViewState("vs_BldngCoverage") = value
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

    Public Property BuildingsLineCount() As Integer
        Get
            Return Session("sess_BuildingLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_BuildingLineCnt") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub LoadStaticData() Implements IVRUI_P.LoadStaticData

    End Sub

    Public Sub Populate() Implements IVRUI_P.Populate
        If AllBuildingsList IsNot Nothing Then
            Dim drBuilding = AllBuildingsList.Select("LocationNum='" & LocationNumber & "' And BuildingNum='" & BuildingNumber & "'")

            If drBuilding.Length > 0 Then
                lblAddress.Text = drBuilding(drBuilding.Length - 1)("LocationAddress")
                lblStructure.Text = drBuilding(drBuilding.Length - 1)("StructType")
                lblStructureDwellingContents.Text = drBuilding(drBuilding.Length - 1)("StructTypeDwellingContents") 'added 10/26/2020 (Interoperability)
                lblTypeData.Text = drBuilding(drBuilding.Length - 1)("BuildingType")
                lblConstructionData.Text = drBuilding(drBuilding.Length - 1)("Construction")
                lblBuildLimitData.Text = drBuilding(drBuilding.Length - 1)("Limit")
                lblBuildDwellingContentsLimitData.Text = drBuilding(drBuilding.Length - 1)("DwellingContentsLimit") 'added 10/26/2020 (Interoperability)
                lblDeductibleData.Text = drBuilding(drBuilding.Length - 1)("Deductible")
                lblBuildLimitPremData.Text = drBuilding(drBuilding.Length - 1)("Premium")
                lblBuildDwellingContentsPremData.Text = drBuilding(drBuilding.Length - 1)("DwellingContentsPremium") 'added 10/26/2020 (Interoperability)
                BuildingsLineCount += 7

                'added 10/26/2020 (Interoperability)
                If qqHelper.IsPositiveDecimalString(lblBuildDwellingContentsPremData.Text) = True Then
                    Me.BuildingDwellingContentsRow.Visible = True
                    BuildingsLineCount += 1
                Else
                    Me.BuildingDwellingContentsRow.Visible = False
                End If
            End If

            ' Coverage Table
            Dim dtBuildingCoverage As New DataTable
            dtBuildingCoverage.Columns.Add("CoverageName", System.Type.GetType("System.String"))
            dtBuildingCoverage.Columns.Add("CoverageLimit", System.Type.GetType("System.String"))
            dtBuildingCoverage.Columns.Add("CoveragePrem", System.Type.GetType("System.String"))
            BuildingsLineCount += 1

            Dim drBldngCoverage = BldngCoverageList.Select("LocationNum='" & LocationNumber & "' And BuildingNum='" & BuildingNumber & "'")
            If drBldngCoverage.Length > 0 Then
                For Each coverage In drBldngCoverage
                    Dim newRow As DataRow = dtBuildingCoverage.NewRow
                    newRow.Item("CoverageName") = coverage("CoverageName")
                    'If LocalQuickQuote.Locations(RowNumber).Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                    '    Dim lim As String = "N/A"
                    '    If lblBuildLimitData.Text.Trim <> "" AndAlso IsNumeric(lblBuildLimitData.Text) Then
                    '        If CDec(lblBuildLimitData.Text) < 300000 Then
                    '            lim = lblBuildLimitData.Text
                    '        Else
                    '            lim = "300,000"
                    '        End If
                    '    End If
                    '    newRow.Item("CoverageLimit") = lim
                    'Else
                    '    newRow.Item("CoverageLimit") = coverage("CoverageLimit")
                    'End If

                    If LocalQuickQuote.Locations(RowNumber).Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio AndAlso coverage("CoverageCodeId") = "70026" Then
                        Dim lim As String = "N/A"
                        If lblBuildLimitData.Text.Trim <> "" AndAlso IsNumeric(lblBuildLimitData.Text) Then
                            If CDec(lblBuildLimitData.Text) < 300000 Then
                                lim = lblBuildLimitData.Text
                            Else
                                lim = "300,000"
                            End If
                        End If
                        newRow.Item("CoverageLimit") = lim
                    Else
                        newRow.Item("CoverageLimit") = coverage("CoverageLimit")
                    End If
                    newRow.Item("CoveragePrem") = coverage("CoveragePrem")
                    dtBuildingCoverage.Rows.Add(newRow)
                    BuildingsLineCount += 1

                    If coverage("CoverageName") = "Loss of Income" Then
                        newRow = dtBuildingCoverage.NewRow
                        Dim lossSpacing As String = "&nbsp;"
                        For idx As Integer = 0 To 50
                            lossSpacing = lossSpacing & "&nbsp;"
                        Next
                        newRow.Item("CoverageName") = lossSpacing & "Period of Loss Extension - " & qqHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteIncomeLoss, QuickQuoteHelperClass.QuickQuotePropertyName.ExtendFarmIncomeOptionId, coverage("LossExt"))
                        dtBuildingCoverage.Rows.Add(newRow)
                        newRow = dtBuildingCoverage.NewRow
                        newRow.Item("CoverageName") = lossSpacing & "Coinsurance - " & qqHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteIncomeLoss, QuickQuoteHelperClass.QuickQuotePropertyName.CoinsuranceTypeId, coverage("CoIns"))
                        dtBuildingCoverage.Rows.Add(newRow)
                        dvBldngCoverage.Attributes.Add("style", "display:block;")
                        BuildingsLineCount += 3
                    End If
                Next
            Else
                dvBldngCoverage.Attributes.Add("style", "display:none;")
            End If

            dgBuildingCoverage.DataSource = dtBuildingCoverage
            dgBuildingCoverage.DataBind()
        End If
    End Sub

    Public Function Save() As Boolean Implements IVRUI_P.Save
        Return False
    End Function

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm

    End Sub
End Class