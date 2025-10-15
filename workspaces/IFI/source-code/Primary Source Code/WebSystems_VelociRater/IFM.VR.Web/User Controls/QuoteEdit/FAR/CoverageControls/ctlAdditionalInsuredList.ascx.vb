Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.FarmLines

Public Class ctlAdditionalInsuredList
    Inherits VRControlBase

    Public Event HideAdditionalInsuredInfo()

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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadStaticData()
            Populate()
        End If

        AttachCoverageControlEvents()
    End Sub

    Protected Sub AttachCoverageControlEvents()
        For Each cntrl As RepeaterItem In aiRepeater.Items
            Dim additionalInsuredControl As ctlAdditionalInsured = cntrl.FindControl("ctlAdditionalInsured")
            AddHandler additionalInsuredControl.RemoveStruct, AddressOf RemoveAdditionalInsured
        Next
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If MyFarmLocation IsNot Nothing Then
            If MyFarmLocation(0).AdditionalInterests IsNot Nothing Then
                Try
                    aiRepeater.DataSource = MyFarmLocation(0).AdditionalInterests

                    If aiRepeater.DataSource IsNot Nothing Then
                        aiRepeater.DataBind()
                        FindChildVrControls()

                        For Each child In ChildVrControls
                            If TypeOf child Is ctlAdditionalInsured Then
                                Dim c As ctlAdditionalInsured = child
                                c.Populate()
                            End If
                        Next

                        If MyFarmLocation(0).AdditionalInterests.Count <= 0 Then
                            RaiseEvent HideAdditionalInsuredInfo()
                        End If
                    End If
                Catch ex As Exception

                End Try
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        SaveChildControls()
        Return True
    End Function

    Private Sub RemoveAdditionalInsured(rowNumber As Integer)
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

        If MyFarmLocation IsNot Nothing And MyFarmLocation(0).AdditionalInterests IsNot Nothing Then
            Dim addlIntrst As QuickQuoteAdditionalInterest = MyFarmLocation(0).AdditionalInterests(rowNumber)

            If addlIntrst.TypeId = "56" Then
                Dim additionalInsuredList As List(Of QuickQuoteSectionIICoverage) = MyFarmLocation(0).SectionIICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Occupying_Residence_on_Premises)
                Dim addlInsurd As QuickQuoteSectionIICoverage = additionalInsuredList(0)

                MyFarmLocation(0).SectionIICoverages.Remove(addlInsurd)
            End If

            MyFarmLocation(0).AdditionalInterests.Remove(addlIntrst)

            Populate()
            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        End If
    End Sub

    Private Sub AdditionalInsured_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles aiRepeater.ItemDataBound
        Dim addlInsrControl As ctlAdditionalInsured = e.Item.FindControl("ctlAdditionalInsured")
        addlInsrControl.RowNumber = e.Item.ItemIndex
        addlInsrControl.Populate()
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidateChildControls(valArgs)
    End Sub
End Class