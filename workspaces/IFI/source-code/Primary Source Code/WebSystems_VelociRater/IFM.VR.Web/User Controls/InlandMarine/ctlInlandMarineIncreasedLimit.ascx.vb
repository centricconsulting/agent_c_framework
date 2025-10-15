Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM

Public Class ctlInlandMarineIncreasedLimit
    Inherits VRControlBase

    Private ReadOnly Property defaultText As String
        Get
            Select Case Me.InlandMarineType
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry
                    Return "Jewelry"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault
                    Return "Jewelry in Vault"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Bicycles ' added 3-21-16 Matt for Comparative Rater project
                    Return "Bicycles"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Coins ' added 3-21-16 Matt for Comparative Rater project
                    Return "Coins"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.AntiquesWithBreakage
                    Return "Antiques - with breakage coverage"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.AntiquesWithoutBreakage
                    Return "Antiques - without breakage coverage"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithBreakage
                    Return "Collector Items Hobby - with breakage coverage"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithoutBreakage
                    Return "Collector Items Hobby - without breakage coverage"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras
                    Return "Cameras"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer
                    Return "Computers"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled
                    Return "Farm Machinery - Scheduled"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage
                    Return "Fine Arts - with breakage coverage"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage
                    Return "Fine Arts - without breakage coverage"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs
                    Return "Furs"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors
                    Return "Garden Tractors"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf
                    Return "Golfers Equipment"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GraveMarkers
                    Return "Grave Markers"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns
                    Return "Guns"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids
                    Return "Hearing Aids"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.MedicalItemsAndEquipment
                    Return "Medical Items and Equipment"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I
                    Return "Silverware"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineSportsEquipment
                    Return "Sports Equipment"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentNamedPerils
                    Return "Irrigation Equipment - Named Perils"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentSpecialCoverage
                    Return "Irrigation Equipment - Special Form"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Radios_CB
                    Return "Radios - CB"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Radios_FM
                    Return "Radios - FM"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.ReproductiveMaterialsNamedPerils
                    Return "Reproductive Materials - Named Perils"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.ReproductiveMaterialsSpecialCoverage
                    Return "Reproductive Materials - Special Form"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile
                    Return "Telephone - Car or Mobile"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment
                    Return "Tools and Equipment"
                Case QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional
                    Return "Musical Instrument (Non-Professional)"
            End Select
            Return ""
        End Get

    End Property
    Public Event RemoveIMItem(index As Integer)

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
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
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Dim scriptLimitData As String = "RoundIMValue100Validate(""" + txtIM_LimitData.ClientID + """, """ + ddlIM_Deductible.ClientID + """);"
        txtIM_LimitData.Attributes.Add("onblur", scriptLimitData)

        Dim scriptDescCount As String = "CountDescLength(""" + txtIM_Description.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
        'txtIM_Description.Attributes.Add("onkeyup", scriptDescCount) 'replaced with the line below Matt A 11/12/2016
        VRScript.CreateJSBinding(Me.txtIM_Description.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)

        Dim scriptSLCount As String = "CountDescLength(""" + txtIM_StorageLocation.ClientID + """, """ + lblIM_SL_MaxCharCount.ClientID + """, """ + hiddenSLMaxCharCount.ClientID + """);"
        VRScript.CreateJSBinding(Me.txtIM_StorageLocation.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptSLCount, True)

        txtIM_LimitData.Attributes.Add("onfocus", "this.select()")
        txtIM_Description.Attributes.Add("onfocus", "this.select()")
        txtIM_StorageLocation.Attributes.Add("onfocus", "this.select()")

    End Sub

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(ddlIM_Deductible, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        Dim inlandMarine As QuickQuoteInlandMarine = Nothing

        'Bug# 6257 - Set Max Characters
        hiddenMaxCharCount.Value = 250
        hiddenMaxCharCount.Value = 250 - Len(txtIM_Description.Text)
        lblMaxCharCount.Text = hiddenMaxCharCount.Value

        hiddenSLMaxCharCount.Value = 250
        hiddenSLMaxCharCount.Value = 250 - Len(txtIM_StorageLocation.Text)
        lblIM_SL_MaxCharCount.Text = hiddenSLMaxCharCount.Value

        If MyLocation.InlandMarines IsNot Nothing Then
            Dim inlandMarineList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = InlandMarineType)

            Try
                inlandMarine = inlandMarineList(RowNumber)
            Catch ex As Exception
            End Try

            If inlandMarine IsNot Nothing Then
                txtIM_LimitData.Text = inlandMarine.IncreasedLimit

                If inlandMarine.DeductibleLimitId = "" Or inlandMarine.DeductibleLimitId = "0" Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlIM_Deductible, inlandMarineList(0).DeductibleLimitId)
                Else
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlIM_Deductible, inlandMarine.DeductibleLimitId)
                End If

                If inlandMarine.Description.ToUpper() = defaultText.ToUpper() And IsOnAppPage Then  'Matt A 12-18-19 for app side
                    txtIM_Description.Text = ""
                Else
                    txtIM_Description.Text = inlandMarine.Description
                End If

                txtIM_StorageLocation.Text = inlandMarine.StorageLocation
            End If


            If inlandMarine.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GraveMarkers Then
                txtIM_StorageLocation.Visible = True
                lblIM_SL_MaxChar.Visible = True
                lblIM_SL_MaxCharCount.Visible = True
            Else
                txtIM_StorageLocation.Visible = False
                lblIM_SL_MaxChar.Visible = False
                lblIM_SL_MaxCharCount.Visible = False
            End If


            'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
            If Me.IsQuoteReadOnly Then
                Me.lblMaxCharCount.Visible = False
                Me.lblMaxChar.Visible = False
                Me.lnkDelete.Visible = False

                Me.lblIM_SL_MaxChar.Visible = False
                Me.lblIM_SL_MaxCharCount.Visible = False
            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Dim inlandMarine As QuickQuoteInlandMarine = Nothing

        If MyLocation.InlandMarines IsNot Nothing Then
            Dim inlandMarineList As List(Of QuickQuoteInlandMarine) = MyLocation.InlandMarines.FindAll(Function(p) p.InlandMarineType = InlandMarineType)

            Try
                inlandMarine = inlandMarineList(RowNumber)
            Catch ex As Exception
            End Try

            If inlandMarine IsNot Nothing Then
                inlandMarine.IncreasedLimit = txtIM_LimitData.Text
                inlandMarine.DeductibleLimitId = ddlIM_Deductible.SelectedValue
                inlandMarine.Description = txtIM_Description.Text

                If inlandMarine.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GraveMarkers Then
                    inlandMarine.StorageLocation = txtIM_StorageLocation.Text
                End If

                Return True
            End If
        End If

        Return False
    End Function

    Protected Sub OnConfirm(sender As Object, e As EventArgs) Handles lnkDelete.Click
        Dim confirmValue As String = Request.Form("confirmValue")

        If confirmValue = "Yes" Then
            Save_FireSaveEvent(False)
            txtIM_LimitData.Text = ""
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlIM_Deductible, "")
            txtIM_Description.Text = ""
            txtIM_StorageLocation.Text = ""
            RaiseEvent RemoveIMItem(RowNumber)
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = "Inland Marine - " + Me.defaultText
        Dim divIMLimit As String = dvIMLimit.ClientID

        'InlandMarineValidator.RowNumber = RowNumber
        'Dim valList = InlandMarineValidator.ValidateHOMInlandMarine(Me.Quote, valArgs.ValidationType, InlandMarineType)
        'updated 3/11/2021
        Dim valList = InlandMarineValidator.ValidateHOMInlandMarine(Me.Quote, valArgs.ValidationType, InlandMarineType, RowNumber)

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        If valList.Any() Then
            For Each v In valList
                ' *********************
                ' Base Policy Coverages
                ' *********************
                Select Case v.FieldId
                    Case InlandMarineValidator.IMLimitAmount
                        ValidationHelper.Val_BindValidationItemToControl(txtIM_LimitData, v, accordList)
                    Case InlandMarineValidator.IMDeductible
                        ValidationHelper.Val_BindValidationItemToControl(ddlIM_Deductible, v, accordList)
                    Case InlandMarineValidator.IMDescription
                        ValidationHelper.Val_BindValidationItemToControl(txtIM_Description, v, accordList)
                    'Case InlandMarineValidator.Single_Jewelry_Limit_Exceeded
                    '    ValidationHelper.Val_BindValidationItemToControl(txtIM_LimitData, v, accordList)
                    'Case InlandMarineValidator.Single_Jewelry_Appraisal_Needed
                    '    ValidationHelper.Val_BindValidationItemToControl(txtIM_LimitData, v, accordList)
                    'Case InlandMarineValidator.Single_JewelryVault_Limit_Exceeded
                    '    ValidationHelper.Val_BindValidationItemToControl(txtIM_LimitData, v, accordList)
                    'Case InlandMarineValidator.Single_JewelryVault_Appraisal_Needed
                    '    ValidationHelper.Val_BindValidationItemToControl(txtIM_LimitData, v, accordList)
                    Case InlandMarineValidator.Single_ArtsBreak_Limit_Exceeded
                        ValidationHelper.Val_BindValidationItemToControl(txtIM_LimitData, v, accordList)
                    Case InlandMarineValidator.Single_ArtsBreak_Doco_Needed
                        ValidationHelper.Val_BindValidationItemToControl(txtIM_LimitData, v, accordList)
                    Case InlandMarineValidator.Single_ArtsNoBreak_Limit_Exceeded
                        ValidationHelper.Val_BindValidationItemToControl(txtIM_LimitData, v, accordList)
                    Case InlandMarineValidator.Single_ArtsNoBreak_Doco_Needed
                        ValidationHelper.Val_BindValidationItemToControl(txtIM_LimitData, v, accordList)
                    Case InlandMarineValidator.Single_Fur_Limit_Exceeded
                        ValidationHelper.Val_BindValidationItemToControl(txtIM_LimitData, v, accordList)
                    Case InlandMarineValidator.Single_Fur_Doco_Needed
                        ValidationHelper.Val_BindValidationItemToControl(txtIM_LimitData, v, accordList)
                    Case InlandMarineValidator.Limit_LessThan_Deductible
                        ValidationHelper.Val_BindValidationItemToControl(txtIM_LimitData, v, accordList)
                    Case InlandMarineValidator.IMStorageLocation
                        ValidationHelper.Val_BindValidationItemToControl(txtIM_StorageLocation, v, accordList)
                End Select
            Next
        End If
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        txtIM_LimitData.Text = ""
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlIM_Deductible, "0")
        txtIM_Description.Text = defaultText
    End Sub
End Class