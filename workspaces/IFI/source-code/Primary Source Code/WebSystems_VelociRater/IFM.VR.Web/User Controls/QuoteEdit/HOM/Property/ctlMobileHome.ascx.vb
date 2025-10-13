Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.PrimativeExtensions

Public Class ctlMobileHome
    Inherits VRControlBase

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

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(MyLocationIndex) Then
                Return Me.Quote.Locations(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.MobileHomeDiv.ClientID
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(MainAccordionDivId, HiddenField1, "0")
        Me.VRScript.CreateConfirmDialog(Me.lnkClearMobileHome.ClientID, "Clear Mobile Home?")
        Me.VRScript.StopEventPropagation(Me.lnkSaveMobileHome.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddlFoundation, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.FoundationTypeId, SortBy.None, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddlTieDown, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.MobileHomeTieDownTypeId, SortBy.None, Me.Quote.LobType)
        QQHelper.LoadStaticDataOptionsDropDown(Me.ddlSkirting, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.MobileHomeSkirtTypeId, SortBy.None, Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()
        If Me.MyLocation IsNot Nothing Then
            Me.LoadStaticData()
            Select Case Me.MyLocation.FormTypeId
                Case "6", "7"
                    Me.Visible = True
                    ddlTieDown.SetFromValue(Me.MyLocation.MobileHomeTieDownTypeId)
                    ddlSkirting.SetFromValue(Me.MyLocation.MobileHomeSkirtTypeId)
                    ddlFoundation.SetFromValue(Me.MyLocation.FoundationTypeId)
                    Me.txtMake.Text = Me.MyLocation.MobileHomeMake
                    Me.txtModel.Text = Me.MyLocation.MobileHomeModel
                    Me.txtVin.Text = Me.MyLocation.MobileHomeVIN
                Case "22", "25"
                    If Me.MyLocation.StructureTypeId = "2" Then
                        Me.Visible = True
                        ddlTieDown.SetFromValue(Me.MyLocation.MobileHomeTieDownTypeId)
                        ddlSkirting.SetFromValue(Me.MyLocation.MobileHomeSkirtTypeId)
                        ddlFoundation.SetFromValue(Me.MyLocation.FoundationTypeId)
                        Me.txtMake.Text = Me.MyLocation.MobileHomeMake
                        Me.txtModel.Text = Me.MyLocation.MobileHomeModel
                        Me.txtVin.Text = Me.MyLocation.MobileHomeVIN
                    Else
                        Me.Visible = False
                    End If
                Case Else
                    Me.Visible = False
            End Select
            'Me.Visible = Me.MyLocation.FormTypeId = "6" Or Me.MyLocation.FormTypeId = "7"
            'If Me.MyLocation.FormTypeId = "6" Or Me.MyLocation.FormTypeId = "7" Then
            '    ddlTieDown.SetFromValue(Me.MyLocation.MobileHomeTieDownTypeId)
            '    ddlSkirting.SetFromValue(Me.MyLocation.MobileHomeSkirtTypeId)
            '    ddlFoundation.SetFromValue(Me.MyLocation.FoundationTypeId)
            '    Me.txtMake.Text = Me.MyLocation.MobileHomeMake
            '    Me.txtModel.Text = Me.MyLocation.MobileHomeModel
            '    Me.txtVin.Text = Me.MyLocation.MobileHomeVIN
            'End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Select Case Me.MyLocation.FormTypeId
            Case "1", "2", "3", "4", "5", "23", "24", "26" 'updated 11/15/17 with 23, 24, 26 for HOM Upgrade MLW
                Me.MyLocation.FoundationTypeId = 2
            Case "6", "7"
                'MOBILE HOME
                Me.MyLocation.MobileHomeTieDownTypeId = ddlTieDown.SelectedValue
                Me.MyLocation.MobileHomeSkirtTypeId = ddlSkirting.SelectedValue
                Me.MyLocation.FoundationTypeId = Me.ddlFoundation.SelectedValue

                Me.MyLocation.MobileHomeMake = Me.txtMake.Text
                Me.MyLocation.MobileHomeModel = Me.txtModel.Text
                Me.MyLocation.MobileHomeVIN = Me.txtVin.Text
            Case "22", "25" 'added 11/15/17 for HOM Upgrade MLW
                'HO 0002 and HO 0004 - HOME AND MOBILE HOME
                If Me.MyLocation.StructureTypeId = "2" Then
                    Me.MyLocation.MobileHomeTieDownTypeId = ddlTieDown.SelectedValue
                    Me.MyLocation.MobileHomeSkirtTypeId = ddlSkirting.SelectedValue
                    Me.MyLocation.FoundationTypeId = Me.ddlFoundation.SelectedValue

                    Me.MyLocation.MobileHomeMake = Me.txtMake.Text
                    Me.MyLocation.MobileHomeModel = Me.txtModel.Text
                    Me.MyLocation.MobileHomeVIN = Me.txtVin.Text
                Else
                    Me.MyLocation.FoundationTypeId = 2
                End If
        End Select
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Property Mobile Home"
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Dim valList = LocationMobileHomeValidator.ValidateHOMMobileHome(Me.Quote, Me.MyLocationIndex, valArgs.ValidationType)
        If valList.Any() Then
            For Each v In valList
                Select Case v.FieldId
                    Case LocationMobileHomeValidator.LocationTieDown
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddlTieDown, v, accordList)
                    Case LocationMobileHomeValidator.LocationSkirting
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddlSkirting, v, accordList)
                    Case LocationMobileHomeValidator.LocationFoundationType
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddlFoundation, v, accordList)
                End Select
            Next
        End If
    End Sub

    Protected Sub lnkClearMobileHome_Click(sender As Object, e As EventArgs) Handles lnkClearMobileHome.Click
        Me.ClearControl()
        'force edit mode so they have to save at some point before leaving
        Me.LockTree()
    End Sub

    Public Overrides Sub ClearControl()
        ddlTieDown.SetFromValue("")
        ddlSkirting.SetFromValue("")
        ddlFoundation.SetFromValue("")
        Me.txtMake.Text = ""
        Me.txtModel.Text = ""
        Me.txtVin.Text = ""

        MyBase.ClearControl()
    End Sub

    Protected Sub lnkSaveMobileHome_Click(sender As Object, e As EventArgs) Handles lnkSaveMobileHome.Click
        Me.Save_FireSaveEvent(True)
    End Sub
End Class