Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.VR.Common.Helpers.HOM
Imports IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper

Public Class ctl_CoverageAddress_HOM_App_Item
    Inherits VRControlBase

    'Added control 3/6/18 for HOM Upgrade MLW
    Public Property CoverageIndex As Int32
        Get
            If (ViewState("vs_coverageIndex") IsNot Nothing) Then
                Return CInt(ViewState("vs_coverageIndex"))
            End If
            Return -1
        End Get
        Set(value As Int32)
            ViewState("vs_coverageIndex") = value
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

    Public ReadOnly Property MyLocation As QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public Property MySectionIAppGapList As List(Of QuickQuoteSectionICoverage)
        Get
            Return IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper.GetAppGapSectionICoverages(Quote)
        End Get
        Set(value As List(Of QuickQuoteSectionICoverage))

        End Set
    End Property

    Public Property MySectionIIAppGapList As List(Of QuickQuoteSectionIICoverage)
        Get
            Return IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper.GetAppGapSectionIICoverages(Quote)
        End Get
        Set(value As List(Of QuickQuoteSectionIICoverage))

        End Set
    End Property

    Public Property MySectionIAndIIAppGapList As List(Of QuickQuoteSectionIAndIICoverage)
        Get
            Return IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper.GetAppGapSectionIAndIICoverages(Quote)
        End Get
        Set(value As List(Of QuickQuoteSectionIAndIICoverage))

        End Set
    End Property

    Public Property myCovList As List(Of Object)
        Get
            Dim myCov As Object
            Dim myList As New List(Of Object)
            For Each c As QuickQuoteSectionICoverage In MySectionIAppGapList
                myCov = c
                myList.Add(myCov)
            Next
            For Each c As QuickQuoteSectionIICoverage In MySectionIIAppGapList
                myCov = c
                myList.Add(myCov)
            Next
            For Each c As QuickQuoteSectionIAndIICoverage In MySectionIAndIIAppGapList
                myCov = c
                myList.Add(myCov)
            Next
            Return myList
        End Get
        Set(value As List(Of Object))

        End Set
    End Property

    Protected ReadOnly Property SectionType As HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType
        Get
            If Me.SectionCoverageIEnum <> QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.None Then
                Return HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionICoverage
            End If

            If Me.SectionCoverageIIEnum <> QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.None Then
                Return HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIICoverage
            End If

            If Me.SectionCoverageIAndIIEnum <> QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None Then
                Return HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIAndIICoverage
            End If
            Return HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.NotDefined
        End Get
    End Property

    Public Property SectionCoverageIEnum As QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType
        Get
            If Me.ViewState("vs_SectionCovIType") IsNot Nothing Then
                Return [Enum].Parse(GetType(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType), CInt(Me.ViewState("vs_SectionCovIType")))
            End If
            Return QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.None
        End Get
        Set(value As QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType)
            If Me.SectionType = SectionCoverage.QuickQuoteSectionCoverageType.NotDefined Then
                Me.ViewState("vs_SectionCovIType") = value
            End If
        End Set
    End Property

    Public Property SectionCoverageIIEnum As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType
        Get
            If Me.ViewState("vs_SectionCovIIType") IsNot Nothing Then
                Return [Enum].Parse(GetType(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType), CInt(Me.ViewState("vs_SectionCovIIType")))
            End If
            Return QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.None
        End Get
        Set(value As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType)
            If Me.SectionType = SectionCoverage.QuickQuoteSectionCoverageType.NotDefined Then
                Me.ViewState("vs_SectionCovIIType") = value
            End If
        End Set
    End Property

    Public Property SectionCoverageIAndIIEnum As QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType
        Get
            If Me.ViewState("vs_SectionCovIAndIIType") IsNot Nothing Then
                Return [Enum].Parse(GetType(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType), CInt(Me.ViewState("vs_SectionCovIAndIIType")))
            End If
            Return QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None
        End Get
        Set(value As QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)
            ' you need to have already set the SectionI&II Property and Liability Enums before setting this one
            If Me.SectionType = SectionCoverage.QuickQuoteSectionCoverageType.NotDefined Then
                Me.ViewState("vs_SectionCovIAndIIType") = value
            End If
        End Set
    End Property


    Public Property CoverageName As String
        Get
            If ViewState("vs_covname") Is Nothing Then
                ViewState("vs_covname") = "Unknown"
            End If
            Return ViewState("vs_covname").ToString()
        End Get
        Set(value As String)
            ViewState("vs_covname") = value
        End Set
    End Property

    Public Property MyAddAddressLinkClientId As String
        Get
            If ViewState("vs_MyAddAddressLinkClientId") Is Nothing Then
                ViewState("vs_MyAddAddressLinkClientId") = ""
            End If
            Return ViewState("vs_MyAddAddressLinkClientId")
        End Get
        Set(value As String)
            ViewState("vs_MyAddAddressLinkClientId") = value
        End Set
    End Property

    Public Property showOnAppGap As Boolean
        Get
            If ViewState("vs_showOnAppGap") Is Nothing Then
                ViewState("vs_showOnAppGap") = False
            End If
            Return ViewState("vs_showOnAppGap")
        End Get
        Set(value As Boolean)
            ViewState("vs_showOnAppGap") = value
        End Set
    End Property

    Public ReadOnly Property MySectionCoverage As HomAppGapCoveragesHelper
        Get
            Dim genericCov As HomAppGapCoveragesHelper = Nothing
            If Me.Quote.IsNotNull AndAlso Me.Quote.Locations.HasItemAtIndex(0) Then
                Select Case Me.SectionType
                    Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionICoverage
                        'Updated 8/23/18 for multi state MLW
                        'If myCovList.IsNotNull Then
                        If myCovList IsNot Nothing Then
                            Dim cov As QuickQuote.CommonObjects.QuickQuoteSectionICoverage = DirectCast(myCovList(CoverageIndex), QuickQuote.CommonObjects.QuickQuoteSectionICoverage)
                            If cov IsNot Nothing Then
                                genericCov = New HomAppGapCoveragesHelper(cov)
                            End If
                        End If
                    Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIICoverage
                        'Updated 8/23/18 for multi state
                        'If myCovList.IsNotNull Then
                        If myCovList IsNot Nothing Then
                            Dim cov As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = DirectCast(myCovList(CoverageIndex), QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)
                            If cov IsNot Nothing Then
                                genericCov = New HomAppGapCoveragesHelper(cov)
                            End If
                        End If
                    Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        'Updated 8/23/18 for multi state
                        'If myCovList.IsNotNull Then
                        If myCovList IsNot Nothing Then
                            Dim cov As QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage = DirectCast(myCovList(CoverageIndex), QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage)
                            If cov IsNot Nothing Then
                                genericCov = New HomAppGapCoveragesHelper(cov)
                            End If
                        End If
                    Case Else
                End Select
#If DEBUG Then
                If genericCov Is Nothing Then
                    If 1 = 1 Then

                    End If
                End If
#End If
            End If

            Return genericCov
        End Get
    End Property

    Public Property AdditionalInterestIndex As Int32
        Get
            If (ViewState("vs_additionalInterestIndex") IsNot Nothing) Then
                Return CInt(ViewState("vs_additionalInterestIndex"))
            End If
            Return 0
        End Get
        Set(value As Int32)
            ViewState("vs_additionalInterestIndex") = value
        End Set
    End Property

    Public Property appGapAIStatusList As List(Of appGapAIStatusListItem)
        Get
            Return ViewState("vs_appGapAIStatusList")
        End Get
        Set(value As List(Of appGapAIStatusListItem))
            ViewState("vs_appGapAIStatusList") = value
        End Set
    End Property

    Public Property MyAiTrustList As List(Of QuickQuoteAdditionalInterest)
        Get
            Dim AiTrustList As List(Of QuickQuoteAdditionalInterest) = New List(Of QuickQuoteAdditionalInterest)
            If MySectionCoverage.AdditionalInterests Is Nothing Then
                MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            End If
            For Each ai In MySectionCoverage.AdditionalInterests
                If ai.TypeId = "79" Then
                    AiTrustList.Add(ai)
                End If
            Next
            Return AiTrustList
        End Get
        Set(value As List(Of QuickQuoteAdditionalInterest))
        End Set
    End Property



    Dim _qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Protected ReadOnly Property HomeVersion As String
        Get
            Dim effectiveDate As DateTime
            If Me.Quote IsNot Nothing Then
                If Me.Quote.EffectiveDate IsNot Nothing AndAlso Me.Quote.EffectiveDate <> String.Empty Then
                    effectiveDate = Me.Quote.EffectiveDate
                Else
                    effectiveDate = Now()
                End If
            Else
                effectiveDate = Now()
            End If
            If _qqh.doUseNewVersionOfLOB(Quote, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade) = True Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Get
    End Property


    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onkeyup, "DoCityCountyLookup('" + Me.txtZipCode.ClientID + "','" + Me.ddCityName.ClientID + "','" + Me.txtCityName.ClientID + "','" + Me.txtCounty.ClientID + "','" + Me.ddStateAbbrev.ClientID + "');")
        End If
    End Sub

    Public Overrides Sub LoadStaticData()
        If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddStateAbbrev, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.None, Me.Quote.LobType)
        End If
    End Sub

    Public Overrides Sub Populate()
        If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            LoadStaticData()
            If SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence OrElse SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage OrElse SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement Then
                If MySectionCoverage IsNot Nothing AndAlso MySectionCoverage.AdditionalInterests IsNot Nothing AndAlso MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address IsNot Nothing Then
                    lblAdditionalInterests.Visible = True
                    Select Case SectionCoverageIAndIIEnum
                        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence
                            lblAdditionalInterests.Text = "School Address:"
                        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage
                            lblAdditionalInterests.Text = "Residency Address:"
                        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement
                            If MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).TypeId = "79" Then
                                lblAdditionalInterests.Text = "Trust Address:"
                            Else
                                lblAdditionalInterests.Text = "Trustee Address:"
                            End If
                    End Select
                    Me.txtStreetNum.Text = MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.HouseNum
                    Me.txtStreetName.Text = MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.StreetName
                    Me.txtAptSuite.Text = MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.ApartmentNumber
                    Me.txtZipCode.Text = MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.Zip.ToString().ToMaxLength(5)
                    Me.txtCityName.Text = MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.City
                    If MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.StateId = "63" Then
                        MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.StateId = "0"
                    End If
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddStateAbbrev, MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.StateId)
                    Me.txtCounty.Text = MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.County
                End If
            Else
                If MySectionCoverage IsNot Nothing AndAlso MySectionCoverage.Address IsNot Nothing Then
                    Me.txtStreetNum.Text = MySectionCoverage.Address.HouseNum
                    Me.txtStreetName.Text = MySectionCoverage.Address.StreetName
                    Me.txtAptSuite.Text = MySectionCoverage.Address.ApartmentNumber
                    Me.txtZipCode.Text = MySectionCoverage.Address.Zip.ToString().ToMaxLength(5)
                    Me.txtCityName.Text = MySectionCoverage.Address.City
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddStateAbbrev, MySectionCoverage.Address.StateId)
                    Me.txtCounty.Text = MySectionCoverage.Address.County
                End If
            End If

            If Me.SectionCoverageIIEnum = QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured Then
                'CoverageName = "Additional Residence - Occupied by Insured (N/A)"
                divBar.Visible = False
            End If

            If Me.txtStreetNum.Text.Trim() = "NEED STREET #" Then
                Me.txtStreetNum.Text = ""
            End If
            If Me.txtStreetName.Text.Trim() = "NEED STREET NAME" Then
                Me.txtStreetName.Text = ""
            End If
            If Me.txtAptSuite.Text.Trim() = "NEED IF APPLICABLE" Then
                Me.txtAptSuite.Text = ""
            End If
            If Me.txtZipCode.Text.Trim() = "00001" Then
                Me.txtZipCode.Text = ""
            End If
            If Me.txtCityName.Text.Trim() = "NEED CITY" Then
                Me.txtCityName.Text = ""
            End If
            If Me.ddStateAbbrev.SelectedValue = "999" OrElse Me.ddStateAbbrev.SelectedValue = "63" Then
                Me.ddStateAbbrev.SelectedValue = "0"
            End If
            If Me.txtCounty.Text.Trim() = "NEED COUNTY" Then
                Me.txtCounty.Text = ""
            End If
        End If
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            If myCovList IsNot Nothing Then
                If SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence OrElse SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage OrElse SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement Then

                    If MySectionCoverage IsNot Nothing AndAlso MySectionCoverage.AdditionalInterests IsNot Nothing AndAlso MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address IsNot Nothing Then

                        'Added 5/3/18 for HOM Upgrade - needed in order to save addresses listed under AI without deleting the AI - MLW
                        MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).OverwriteAdditionalInterestListInfoForDiamondId = True

                        If Me.txtStreetNum.Text.Trim() = "NEED STREET #" Then
                            MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.HouseNum = ""
                        Else
                            MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.HouseNum = Me.txtStreetNum.Text
                        End If
                        MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.StreetName = Me.txtStreetName.Text
                        MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.ApartmentNumber = Me.txtAptSuite.Text
                        If Me.txtZipCode.Text = "00001" OrElse Me.txtZipCode.Text = "00001-0000" Then
                            MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.Zip = Me.txtZipCode.Text
                        Else
                            MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.Zip = Me.txtZipCode.Text
                        End If
                        MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.City = Me.txtCityName.Text
                        MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.StateId = ddStateAbbrev.SelectedValue
                        MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address.County = Me.txtCounty.Text
                    End If
                Else
                    If MySectionCoverage IsNot Nothing AndAlso MySectionCoverage.Address IsNot Nothing Then
                        If Me.txtStreetNum.Text.Trim() = "NEED STREET #" Then
                            MySectionCoverage.Address.HouseNum = ""
                        Else
                            MySectionCoverage.Address.HouseNum = Me.txtStreetNum.Text
                        End If
                        MySectionCoverage.Address.StreetName = Me.txtStreetName.Text
                        MySectionCoverage.Address.ApartmentNumber = Me.txtAptSuite.Text
                        If Me.txtZipCode.Text = "00001" OrElse Me.txtZipCode.Text = "00001-0000" Then
                            MySectionCoverage.Address.Zip = ""
                        Else
                            MySectionCoverage.Address.Zip = Me.txtZipCode.Text
                        End If
                        MySectionCoverage.Address.City = Me.txtCityName.Text
                        MySectionCoverage.Address.StateId = Me.ddStateAbbrev.SelectedValue
                        MySectionCoverage.Address.County = Me.txtCounty.Text
                    End If
                End If
            End If
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then


            MyBase.ValidateControl(valArgs)
            Me.ValidationHelper.GroupName = Me.CoverageName
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

            If MySectionCoverage IsNot Nothing Then
                If SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence OrElse SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage OrElse SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement Then
                    If MySectionCoverage IsNot Nothing AndAlso MySectionCoverage.AdditionalInterests IsNot Nothing AndAlso MySectionCoverage.AdditionalInterests(AdditionalInterestIndex).Address IsNot Nothing Then
                        Dim valItems = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AdditionalInterestAddressValidation(MySectionCoverage.AdditionalInterests(AdditionalInterestIndex), valArgs.ValidationType, MySectionCoverage, Quote)

                        If valItems.Any() Then
                            For Each v In valItems
                                Select Case v.FieldId
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AddressStreetNumber
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordList)
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AddressStreetName
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetName, v, accordList)
                                'Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AddressAptNumber
                                '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtAptSuite, v, accordList)
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AddressCity
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCityName, v, accordList)
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AddressState
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddStateAbbrev, v, accordList)
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AddressSatetNotIndiana
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddStateAbbrev, v, accordList)
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AddressZipCode
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtZipCode, v, accordList)
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AIRelativeValidator.AddressCountyID
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCounty, v, accordList)
                                End Select
                            Next
                        End If
                    End If
                Else
                    Dim valItems = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AppGapCoveragesValidator.ValidateAppGapCoverage(Quote, MySectionCoverage, CoverageIndex, valArgs.ValidationType)
                    If valItems.Any() Then
                        For Each v In valItems
                            Select Case v.FieldId
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AppGapCoveragesValidator.AddressStreetNumber
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordList)
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AppGapCoveragesValidator.AddressStreetName
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetName, v, accordList)
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AppGapCoveragesValidator.AddressAptNumber
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtAptSuite, v, accordList)
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AppGapCoveragesValidator.AddressCity
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCityName, v, accordList)
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AppGapCoveragesValidator.AddressState
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddStateAbbrev, v, accordList)
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AppGapCoveragesValidator.AddressSatetNotIndiana
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddStateAbbrev, v, accordList)
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AppGapCoveragesValidator.AddressZipCode
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtZipCode, v, accordList)
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AppGapCoveragesValidator.AddressCountyID
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCounty, v, accordList)
                            End Select
                        Next
                    End If
                End If
            End If


            ' This copies the validation item up to its parent so they don't have to be in a sperate group
            'Updated 8/23/18 for multi state
            'If Me.ParentVrControl.IsNotNull Then
            If Me.ParentVrControl IsNot Nothing Then
                Me.ParentVrControl.ValidationHelper.InsertFromOtherValidationHelper(Me.ValidationHelper)
            End If
            ' END OF COPY LOGIC

        End If
    End Sub

    Public Overrides Sub ClearControl()
        If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            Me.txtAptSuite.Text = ""
            Me.txtCityName.Text = ""
            Me.txtCounty.Text = ""
            Me.txtStreetName.Text = ""
            Me.txtStreetNum.Text = ""
            Me.txtZipCode.Text = ""
            Me.ddStateAbbrev.SelectedIndex = -1
            Me.LockTree()
        End If
    End Sub

    Protected Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            Me.Save_FireSaveEvent(True)
        End If
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            Me.ClearControl()
            Me.LockTree()
        End If
    End Sub

End Class