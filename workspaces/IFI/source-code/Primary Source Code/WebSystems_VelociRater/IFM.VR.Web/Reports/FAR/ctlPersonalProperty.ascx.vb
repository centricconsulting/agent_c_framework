Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods

Public Class ctlPersonalProperty
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

    Public Property PeakSeasonNumber As Int32
        Get
            If ViewState("vs_peakSeasonNumber") Is Nothing Then
                ViewState("vs_peakSeasonNumber") = 0
            End If
            Return CInt(ViewState("vs_peakSeasonNumber"))
        End Get
        Set(value As Int32)
            ViewState("vs_peakSeasonNumber") = value
        End Set
    End Property

    Public Property PersPropList() As DataTable
        Get
            Return ViewState("vs_PersProp")
        End Get
        Set(ByVal value As DataTable)
            ViewState("vs_PersProp") = value
        End Set
    End Property

    Public Property PeakSeasonList() As DataTable
        Get
            Return ViewState("vs_PeakSeason")
        End Get
        Set(ByVal value As DataTable)
            ViewState("vs_PeakSeason") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub LoadStaticData() Implements IVRUI_P.LoadStaticData

    End Sub

    Public Sub Populate() Implements IVRUI_P.Populate
        If PersPropList IsNot Nothing Then
            Dim drPersProp = PersPropList.Select("PersPropRowNum='" & RowNumber & "'")
            If drPersProp.Length > 0 Then
                If drPersProp(drPersProp.Length - 1)("CoverageName") <> "Farm_F_G_Extra_Expense" Then
                    lblCoverage.Text = GetCoverageName(drPersProp(drPersProp.Length - 1)("CoverageName"))
                    ' Select added 03/19/2020 for B39333 CAH
                    Select Case lblCoverage.Text
                        Case "Farm Machinery - Special Coverage - Coverage G" 'Updated 8/28/2019 for Bug 39727 MLW
                            lblCoverageLimit.Text = ""
                            lblCoverageDesc.Text = drPersProp(drPersProp.Length - 1)("Description")
                            lblEarthquake.Text = If(drPersProp(drPersProp.Length - 1)("Earthquake"), "Earthquake", "")
                        Case "Property in Transit"
                            lblCoverageLimit.Text = "5,000"
                            lblCoverageDesc.Text = drPersProp(drPersProp.Length - 1)("Description")
                            lblEarthquake.Text = If(drPersProp(drPersProp.Length - 1)("Earthquake"), "Earthquake", "")
                        Case "Suffocation of Livestock or Poultry - Cattle", "Suffocation of Livestock or Poultry - Poultry", "Suffocation of Livestock or Poultry - Equine", "Suffocation of Livestock or Poultry - Swine"
                            lblCoverageDesc.Text = drPersProp(drPersProp.Length - 1)("Description")
                            lblEarthquake.Text = drPersProp(drPersProp.Length - 1)("Earthquake")
                            lblCoverageLimit.Text = drPersProp(drPersProp.Length - 1)("Limit")
                        Case Else
                            lblCoverageLimit.Text = drPersProp(drPersProp.Length - 1)("Limit")
                            lblCoverageDesc.Text = drPersProp(drPersProp.Length - 1)("Description")
                            lblEarthquake.Text = If(drPersProp(drPersProp.Length - 1)("Earthquake"), "Earthquake", "")
                    End Select

                    lblDeductible.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.Farm_F_and_G_DeductibleLimitId, drPersProp(drPersProp.Length - 1)("Deductible"))
                    lblCoveragePrem.Text = drPersProp(drPersProp.Length - 1)("Premium")
                End If
            End If
        End If

        ctlPeakSeasons.RowNumber = RowNumber
        ctlPeakSeasons.PeakSeasonList = PeakSeasonList
    End Sub

    Private Function GetCoverageName(coverage As String) As String
        Dim coverageName As String = Nothing

        Select Case coverage
            Case "Farm_4_H_and_FFAAnimals"
                coverageName = "Farm 4H and FFA Animals"
            Case "Farm_Sheep"
                coverageName = "Sheep Additional Perils"
            Case "LocationF_DescribedMachinery"
                coverageName = "Farm Machinery - Described"
            Case "FarmMachineryDescribed_OpenPerils"
                coverageName = "Farm Machinery - Described - Open Perils"
            Case "Farm_F_Irrigation_Equipment"
                coverageName = "Irrigation"
            Case "Livestock"
                coverageName = "Livestock"
            Case "MiscellaneousFarmPersonalProperty"
                coverageName = "Miscellaneous Farm Personal Property"
            Case "Farm_Rented_or_borrowed_Equipment"
                coverageName = "Rented or Borrowed Equipment"
            Case "LocationF_MachineryNotDescribed"
                coverageName = "Farm Machinery - Not Described"
            Case "Farm_F_Grain"
                coverageName = "Grain in Buildings"
            Case "GrainintheOpen"
                coverageName = "Grain in the Open"
            Case "Farm_F_Hay_in_Barn"
                coverageName = "Hay in Buildings"
            Case "Farm_F_Hay_in_the_Open"
                coverageName = "Hay in the Open"
            Case "ReproductiveMaterials"
                coverageName = "Reproductive Equipment"
            Case "Farm_Suffocation_of_Livestock"
                coverageName = "Suffocation of Livestock"
            Case "UnschedBlnkt" 'Added 8/28/2019 for Bug 39727 MLW              
                coverageName = "Unscheduled Farm Personal Property"
            Case "Farm_Property_in_Transit"
                coverageName = "Property in Transit"
            Case "Farm_Suffocation_of_Livestock_Cattle"
                coverageName = "Suffocation of Livestock or Poultry - Cattle"
            Case "Farm_Suffocation_of_Livestock_Poultry"
                coverageName = "Suffocation of Livestock or Poultry - Poultry"
            Case "Farm_Suffocation_of_Livestock_Equine"
                coverageName = "Suffocation of Livestock or Poultry - Equine"
            Case "Farm_Suffocation_of_Livestock_Swine"
                coverageName = "Suffocation of Livestock or Poultry - Swine"
            Case Else 'Updated 8/28/2019 for Bug 39727 MLW
                coverageName = "Farm Machinery - Special Coverage - Coverage G"
        End Select

        Return coverageName
    End Function

    Public Function Save() As Boolean Implements IVRUI_P.Save
        Return False
    End Function

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm

    End Sub
End Class