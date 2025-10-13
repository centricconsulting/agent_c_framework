Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.FarmLines

Public Class ctlIrrigationEquip
    Inherits VRControlBase

    Public Event RemoveItem(rowNumber As Integer, optCov As QuickQuote.CommonObjects.QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType, persPropCov As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType)

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

    'Private _persPropType As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType
    'Public Property PersPropType() As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType
    '    Get
    '        Return _persPropType
    '    End Get
    '    Set(ByVal value As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType)
    '        _persPropType = value
    '    End Set
    'End Property

    Public Property PersPropType() As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType
        Get
            Return ViewState("vs_PersPropType")
        End Get
        Set(ByVal value As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType)
            ViewState("vs_PersPropType") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadStaticData()
            Populate()
        End If

        AttachCoverageControlEvents()
        PersPropType = QuickQuoteScheduledPersonalPropertyCoverage.QuickQuoteScheduledPersonalPropertyCoverageType.Farm_F_Irrigation_Equipment
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Quote IsNot Nothing Then
            Try
                'Updated 11/06/2019 for Bug 33751 MLW - SubQuoteFirst to GoverningStateQuote
                'Updated 9/10/18 for multi state MLW - Quote to SubQuoteFirst
                'If SubQuoteFirst IsNot Nothing AndAlso SubQuoteFirst.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                If GoverningStateQuote() IsNot Nothing AndAlso GoverningStateQuote.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                    'PersPropRepeater.DataSource = SubQuoteFirst.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = PersPropType)
                    PersPropRepeater.DataSource = GoverningStateQuote.ScheduledPersonalPropertyCoverages.FindAll(Function(p) p.CoverageType = PersPropType)

                    If PersPropRepeater.DataSource IsNot Nothing Then
                        PersPropRepeater.DataBind()
                        FindChildVrControls()

                        For Each child In ChildVrControls
                            If TypeOf child Is ctlFarmScheduledLimit Then
                                Dim c As ctlFarmScheduledLimit = child
                                c.Populate()
                            End If
                        Next
                    End If
                End If
            Catch ex As Exception

            End Try
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        For Each cntrl As RepeaterItem In PersPropRepeater.Items
            Dim increasedLimitControl As ctlFarmScheduledLimit = cntrl.FindControl("ctlFarmScheduledLimit")
            increasedLimitControl.PersPropType = PersPropType
        Next

        SaveChildControls()
        Return True
    End Function

    Protected Sub AttachCoverageControlEvents()
        For Each cntrl As RepeaterItem In PersPropRepeater.Items
            Dim increasedLimitControl As ctlFarmScheduledLimit = cntrl.FindControl("ctlFarmScheduledLimit")
            AddHandler increasedLimitControl.RemovePersPropItem, AddressOf RemovePersPropItem
        Next
    End Sub

    Private Sub RemovePersPropItem(rowNumber As Integer)
        RaiseEvent RemoveItem(rowNumber, QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.None, PersPropType)
    End Sub

    Private Sub IMRepeater_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles PersPropRepeater.ItemDataBound
        Dim increasedLimitControl As ctlFarmScheduledLimit = e.Item.FindControl("ctlFarmScheduledLimit")
        increasedLimitControl.RowNumber = e.Item.ItemIndex
        increasedLimitControl.PersPropType = PersPropType
        increasedLimitControl.Populate()
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        For Each cntrl As RepeaterItem In PersPropRepeater.Items
            Dim increasedLimitControl As ctlFarmScheduledLimit = cntrl.FindControl("ctlFarmScheduledLimit")
            increasedLimitControl.PersPropType = PersPropType
            increasedLimitControl.ValidatorGroupName = "Irrigation Equipment"
        Next
        ValidateChildControls(valArgs)
    End Sub
End Class