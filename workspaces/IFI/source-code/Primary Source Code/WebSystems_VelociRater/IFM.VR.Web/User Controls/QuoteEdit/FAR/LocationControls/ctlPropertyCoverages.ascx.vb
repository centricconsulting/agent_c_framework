Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.FarmLines

Public Class ctlPropertyCoverages
    Inherits VRControlBase

    Public Property PropertyAccordionDivId As String
        Get
            If ViewState("vs_PropertyAccordionDivId_") IsNot Nothing Then
                Return ViewState("vs_PropertyAccordionDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_PropertyAccordionDivId_") = value
        End Set
    End Property

    Public Property CoverageAccordionDivId As String
        Get
            If ViewState("vs_CoverageAccordionDivId_") IsNot Nothing Then
                Return ViewState("vs_CoverageAccordionDivId_")
            End If
            Return ""
        End Get
        Set(value As String)
            ViewState("vs_CoverageAccordionDivId_") = value
        End Set
    End Property

    Public Property FarmLocationIndex As Int32
        Get
            Return Session("sess_FarmLocationIndex")
            'If ViewState("vs_farmLocationIndex") IsNot Nothing Then
            '    Return CInt(ViewState("vs_farmLocationIndex"))
            'End If
            'Return 0
        End Get
        Set(value As Int32)
            'ViewState("vs_farmLocationIndex") = value
            Session("sess_FarmLocationIndex") = value
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        PropertyAccordionDivId = dvFarmBuildingProperty.ClientID
        CoverageAccordionDivId = dvFarmBuildingCoverage.ClientID

        If Not IsPostBack Then
            LoadStaticData()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        VRScript.CreateAccordion(PropertyAccordionDivId, hiddenBuildingProperty, "0", False)
        VRScript.CreateAccordion(CoverageAccordionDivId, hiddenBuildingCoverage, "0", False)

        VRScript.StopEventPropagation(lnkClearProperty.ClientID, True)
        VRScript.StopEventPropagation(lnkSaveProperty.ClientID, True)
        VRScript.StopEventPropagation(lnkClearCoverage.ClientID, True)
        VRScript.StopEventPropagation(lnkSaveCoverage.ClientID, True)

        ' Toggle Contract Growers Limit
        Dim scriptGrowersLimit As String = "ToggleBldngCovLimit(""" + chkContractGrow.ClientID + """, """ + dvContractGrowLimit.ClientID + """, """ + ddlContractGrow.ClientID + """);"
        chkContractGrow.Attributes.Add("onclick", scriptGrowersLimit)

        ' Toggle Loss if Income Limit
        Dim scriptLossIncomeLimit As String = "ToggleBldngCovLimit(""" + chkLossIncome.ClientID + """, """ + dvLossIncomeLimit.ClientID + """, """ + txtLossIncomeLimit.ClientID + """);"
        chkLossIncome.Attributes.Add("onclick", scriptLossIncomeLimit)

        ' Toggle Suffocation of Livestock Limit
        Dim scriptLivestockLimit As String = "ToggleBldngCovLimit(""" + chkLivestock.ClientID + """, """ + dvLivestockLimit.ClientID + """, """ + txtLivestockLimit.ClientID + """);"
        chkLivestock.Attributes.Add("onclick", scriptLivestockLimit)

        ' Open Information Popup
        lbtnBuildtype.Attributes.Add("onclick", "InitBldTypePopupInfo('dvBldTypeInfoPopup', 'Building Type'); return false;")

        ' Close Information Popup
        btnBldTypeOK.Attributes.Add("onclick", "CloseBldTypePopupInfo('dvBldTypeInfoPopup'); return false;")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()

    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
    End Sub
End Class