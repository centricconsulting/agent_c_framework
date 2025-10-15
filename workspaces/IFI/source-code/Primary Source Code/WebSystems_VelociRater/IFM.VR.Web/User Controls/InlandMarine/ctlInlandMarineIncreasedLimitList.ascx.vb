Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM

Public Class ctlInlandMarineIncreasedLimitList
    Inherits VRControlBase

    Public Event ToggleIncreasedLimits(show As Boolean)
    Public Event RemoveErrorBorder()
    Public Event RemoveFarmItem(rowNumber As Integer, form As String)

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
        End Get
    End Property

    Public Property InlandMarineType() As QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType
        Get
            Return ViewState("vs_InlandMarineType")
        End Get
        Set(ByVal value As QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType)
            ViewState("vs_InlandMarineType") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadStaticData()
            Populate()
        End If

        AttachCoverageControlEvents()
        'InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If MyLocation IsNot Nothing Then
            Try
                IMRepeater.DataSource = If(MyLocation?.InlandMarines IsNot Nothing, MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = InlandMarineType), Nothing)

                IMRepeater.DataBind()
                If IMRepeater.DataSource IsNot Nothing Then
                    FindChildVrControls()

                    For Each child In ChildVrControls
                        If TypeOf child Is ctlInlandMarineIncreasedLimit Then
                            Dim c As ctlInlandMarineIncreasedLimit = child
                            c.Populate()
                        End If
                    Next
                End If
            Catch ex As Exception

            End Try
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        For Each cntrl As RepeaterItem In IMRepeater.Items
            Dim increasedLimitControl As ctlInlandMarineIncreasedLimit = cntrl.FindControl("ctlInlandMarineIncreasedLimit")
            increasedLimitControl.InlandMarineType = InlandMarineType
        Next
        SaveChildControls()
        Return True
    End Function

    Protected Sub AttachCoverageControlEvents()
        For Each cntrl As RepeaterItem In IMRepeater.Items
            Dim increasedLimitControl As ctlInlandMarineIncreasedLimit = cntrl.FindControl("ctlInlandMarineIncreasedLimit")
            AddHandler increasedLimitControl.RemoveIMItem, AddressOf RemoveIMItem
        Next
    End Sub

    Private Sub RemoveIMItem(rowNumber As Integer)
        RaiseEvent RemoveFarmItem(rowNumber, InlandMarineType)
    End Sub

    Private Sub IMRepeater_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles IMRepeater.ItemDataBound
        Dim increasedLimitControl As ctlInlandMarineIncreasedLimit = e.Item.FindControl("ctlInlandMarineIncreasedLimit")
        increasedLimitControl.RowNumber = e.Item.ItemIndex
        increasedLimitControl.InlandMarineType = InlandMarineType
        increasedLimitControl.Populate()
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        For Each cntrl As RepeaterItem In IMRepeater.Items
            Dim increasedLimitControl As ctlInlandMarineIncreasedLimit = cntrl.FindControl("ctlInlandMarineIncreasedLimit")
            increasedLimitControl.InlandMarineType = InlandMarineType

            ' This may need to be a case statement
            'increasedLimitControl.ValidatorGroupName = "Jewelry"
        Next
        ValidateChildControls(valArgs)
    End Sub
End Class