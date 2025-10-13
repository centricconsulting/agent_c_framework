Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.HOM
Imports QuickQuote.CommonObjects

Public Class ctl_Coverages_HOM_App_Section
    Inherits VRControlBase

    'Added 3/8/18 control for HOM Upgrade MLW
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
                'If _qqh.doUseNewVersionOfLOB(effectiveDate, Quote.LobType, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade) = True Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Get
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
    'Public Property SectionType As Type
    '    Get
    '        If ViewState("vs_sectionType") Is Nothing Then
    '            ViewState("vs_sectionType") = Nothing
    '        End If
    '        Return ViewState("vs_sectionType").GetType()
    '    End Get
    '    Set(value As Type)
    '        ViewState("vs_sectionType") = value.GetType()
    '    End Set
    'End Property

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


    'Public Property SectionICoverageType As QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType
    '    Get
    '        If (ViewState("vs_sectionICoverageType") IsNot Nothing) Then
    '            Return CInt(ViewState("vs_sectionICoverageType"))
    '        End If
    '        Return QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.None
    '    End Get
    '    Set(value As QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType)
    '        ViewState("vs_sectionICoverageType") = value
    '    End Set
    'End Property

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
                'SetCoverageDisplayProperties()
                '#If DEBUG Then
                '            Else
                '                Throw New Exception("You can not change the Section Type once it has been set.")
                '#End If
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
                'SetCoverageDisplayProperties()
                '#If DEBUG Then
                '            Else
                '                Throw New Exception("You can not change the Section Type once it has been set.")
                '#End If
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
                'SetCoverageDisplayProperties()
            End If
        End Set
    End Property

    'Private Sub SetCoverageDisplayProperties()
    '    Dim covProperties = SectionCoverage.GetCoverageDisplayProperties(Me.Quote, Me.MyLocation,
    '                                          Me.SectionType,
    '                                          Me.SectionCoverageIEnum,
    '                                          Me.SectionCoverageIIEnum,
    '                                          Me.SectionCoverageIAndIIEnum)
    '    'Me.MyDisplayType = covProperties.MyDisplayType
    '    Me.CoverageName = covProperties.Coveragename
    '    'Me.IncludedLimit = covProperties.IncludedLimit
    '    'Me.IsDefaultCoverage = covProperties.IsDefaultCoverage
    'End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Me.Quote IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then

                Me.VRScript.CreateConfirmDialog(Me.lnkClearCoverage.ClientID, "Clear?")
                Me.VRScript.StopEventPropagation(Me.lnkSaveCoverages.ClientID)

                If MyLocation Is Nothing Then
                    Me.VRScript.CreateAccordion(Me.divCoverageMain.ClientID, Nothing, "false")
                    divCoverageContent.Visible = False
                Else
                    divCoverageContent.Visible = True

                    'Dim isMultiple As Boolean = False
                    'If AppGapSectionIIIndex > 0 Then
                    '    Dim SectionCoverageIIEnum As QuickQuoteSectionIICoverage.HOM_SectionIICoverageType = MySectionIIAppGapList(AppGapSectionIIIndex).HOM_CoverageType
                    '    Dim previousCovSectionII As QuickQuoteSectionIICoverage.HOM_SectionIICoverageType = MySectionIIAppGapList(AppGapSectionIIIndex - 1).HOM_CoverageType
                    '    isMultiple = IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper.CurrentCoverageSameAsPrevious(SectionCoverageIIEnum, previousCovSectionII)
                    '    If isMultiple = True Then
                    '        'headerBar.Visible = False
                    '        lblHeader.Visible = False
                    '        lnkClearCoverage.Visible = False
                    '        lnkSaveCoverages.Visible = False
                    '    End If
                    'End If
                    'If isMultiple = True Then
                    '    'Me.VRScript.AddScriptLine("$(""#" + Me.divCoverageMain.ClientID + """).Attributes.Add('class', 'UI-accordion - Content ui-helper-reset ui-widget-content ui-corner-bottom ui-accordion-content-active');")
                    '    Me.VRScript.CreateAccordion(Me.divCoverageMain.ClientID, Me.hiddenCoverageAccordActive, "0")
                    'Else
                    Me.VRScript.CreateAccordion(Me.divCoverageMain.ClientID, Me.hiddenCoverageAccordActive, "0")
                    'End If

                    'If MySectionIAppGapList IsNot Nothing AndAlso MySectionIAppGapList.Count > 0 AndAlso CoverageIndex < MySectionIAppGapList.Count Then
                    '    Select Case MySectionICoverage.HOM_CoverageType
                    '        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises
                    '            Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                    '            VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)
                    '            txtDescription.Attributes.Add("onfocus", "this.select()")
                    '        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises
                    '            Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                    '            VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)
                    '            txtDescription.Attributes.Add("onfocus", "this.select()")
                    '    End Select
                    'End If

                    'Dim secIIAppGapMin = 0
                    ''Dim secIIAppGapMax = 0
                    'Dim secIIIndexMax = 0
                    'If MySectionIAppGapList IsNot Nothing AndAlso MySectionIAppGapList.Count > 0 Then
                    '    secIIAppGapMin = MySectionIAppGapList.Count
                    '    secIIIndexMax = MySectionIAppGapList.Count
                    'End If
                    'If MySectionIIAppGapList IsNot Nothing AndAlso MySectionIIAppGapList.Count > 0 Then
                    '    'secIIAppGapMax = MySectionIIAppGapList.Count
                    '    secIIIndexMax += MySectionIIAppGapList.Count
                    'End If

                    'If MySectionIIAppGapList IsNot Nothing AndAlso MySectionIIAppGapList.Count > 0 AndAlso CoverageIndex >= secIIAppGapMin AndAlso AppGapSectionIIIndex <= MySectionIIAppGapList.Count AndAlso CoverageIndex < secIIIndexMax Then
                    '    ''lblHeader.Text = IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper.GetAppGapCoverageName(MySectionIAppGapList(CoverageIndex))
                    '    Select Case MySectionIICoverage.HOM_CoverageType
                    '        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured
                    '            'no counting, no description
                    '        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther
                    '            Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                    '            VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)
                    '            txtDescription.Attributes.Add("onfocus", "this.select()")
                    '        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion
                    '            Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                    '            VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)
                    '            txtDescription.Attributes.Add("onfocus", "this.select()")
                    '        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres
                    '            Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                    '            VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)
                    '            txtDescription.Attributes.Add("onfocus", "this.select()")
                    '        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability 'IncidentalFarmersPersonalLiability-On Premises
                    '            Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                    '            VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)
                    '            txtDescription.Attributes.Add("onfocus", "this.select()")
                    '        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises
                    '            Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                    '            VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)
                    '            txtDescription.Attributes.Add("onfocus", "this.select()")
                    '        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence
                    '            Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                    '            VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)
                    '            txtDescription.Attributes.Add("onfocus", "this.select()")
                    '    End Select



                    'End If

                    'Dim secIAndIIAppGapMin = 0
                    ''Dim secIAndIIAppGapMax = 0
                    'If MySectionIAppGapList IsNot Nothing AndAlso MySectionIAppGapList.Count > 0 Then
                    '    secIAndIIAppGapMin = MySectionIAppGapList.Count
                    'End If
                    'If MySectionIIAppGapList IsNot Nothing AndAlso MySectionIIAppGapList.Count > 0 Then
                    '    secIAndIIAppGapMin += MySectionIIAppGapList.Count
                    '    'secIAndIIAppGapMax = MySectionIIAppGapList.Count
                    'End If
                    ''If MySectionIAndIIAppGapList IsNot Nothing AndAlso MySectionIAndIIAppGapList.Count > 0 Then
                    ''    secIAndIIAppGapMax = MySectionIAndIIAppGapList.Count
                    ''End If

                    'If MySectionIAndIIAppGapList IsNot Nothing AndAlso MySectionIAndIIAppGapList.Count > 0 AndAlso CoverageIndex >= secIAndIIAppGapMin AndAlso AppGapSectionIAndIIIndex <= MySectionIAndIIAppGapList.Count Then
                    '    Select Case MySectionIAndIICoverage.MainCoverageType
                    '        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence
                    '            'no counting, no description
                    '        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage
                    '            'no counting, no description
                    '        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.LossAssessment
                    '            'no counting, no description
                    '        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold
                    '            Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                    '            VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)
                    '            txtDescription.Attributes.Add("onfocus", "this.select()")
                    '        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures
                    '            Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                    '            VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)
                    '            txtDescription.Attributes.Add("onfocus", "this.select()")
                    '            Dim scriptDescCount2 As String = "CountDescLength(""" + txtDescription2.ClientID + """, """ + lblMaxCharCount2.ClientID + """, """ + hiddenMaxCharCount2.ClientID + """);"
                    '            VRScript.CreateJSBinding(Me.txtDescription2.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount2, True)
                    '            txtDescription2.Attributes.Add("onfocus", "this.select()")
                    '        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers
                    '            Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                    '            VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)
                    '            txtDescription.Attributes.Add("onfocus", "this.select()")
                    '        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement
                    '            'no counting, no description
                    '    End Select

                    'End If

                End If
            End If
        End If
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        If Me.Quote IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then

                'MyBase.ValidateControl(valArgs)

                'Me.ValidationHelper.GroupName = "Coverages"

                'Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

            End If
        End If

    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        If Me.Quote IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then

                Select Case Me.SectionType
                    Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionICoverage
                        ' Case GetType(QuickQuoteSectionICoverage)
                        Select Case Me.SectionCoverageIEnum
                            Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises
                                CoverageName = "Other Structures On the Residence Premises (HO 0448)"
                            Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises
                                CoverageName = "Specific Structures Away from Residence Premises<br />(HO 0492)"
                        End Select
                        'Case GetType(QuickQuoteSectionIICoverage)
                    Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIICoverage
                        Select Case Me.SectionCoverageIIEnum
                            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured
                                CoverageName = "Additional Residence - Occupied by Insured (N/A)"
                            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther
                                CoverageName = "Additional Residence - Rented to Others (HO 2470)"
                            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion
                                CoverageName = "Canine Liability Exclusion (HO 2477)"
                            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres
                                CoverageName = "Farm Owned and Operated By Insured: 0-100 Acres<br />(HO 2436)"
                            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability 'IncidentalFarmersPersonalLiability-On Premises
                                CoverageName = "Incidental Farming Personal Liability - On Premises<br />(HO 2472)"
                            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises
                                CoverageName = "Incidental Farming Personal Liability - Off Premises<br />(HO 2472)"
                            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence
                                CoverageName = "Permitted Incidental Occupancies Other Residence<br />(HO 2443)"
                        End Select
                    Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        'Case GetType(QuickQuoteSectionIAndIICoverage)
                        Select Case Me.SectionCoverageIAndIIEnum
                            Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence
                                CoverageName = "Additional Insured – Student Living Away from Residence<br />(HO 0527)"
                            Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage
                                CoverageName = "Assisted Living Care Coverage (HO 0459)"
                                'removed 3/16/18 - Loss Assessment no longer multiple, no longer capturing address MLW
                            'Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.LossAssessment
                            '    CoverageName = "Loss Assessment (HO 0435)"
                            Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold
                                CoverageName = "Other Members of Your Household (HO 0458)"
                            Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures
                                CoverageName = "Permitted Incidental Occupancies Residence Premises<br />(HO 0442)"
                            Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers
                                CoverageName = "Structures Rented To Others - Residence Premises<br />(HO 0440)"
                            Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement
                                CoverageName = "Trust Endorsement (HO 0615)"
                        End Select
                End Select

                Me.lblHeader.Text = Me.CoverageName
            End If
        End If

        'Me.ctl_CoverageAddress_HOM_App_Item.CoverageIndex = Me.CoverageIndex + 1
        'Me.ctl_CoverageAddress_HOM_App_Item.Populate()
        'Me.ctl_CoverageAddress_HOM_App_Item.InitFromExisting(Me)
    End Sub
    Public Overrides Function Save() As Boolean
        If Me.Quote IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then


            End If
        End If
        Return True
    End Function

    Protected Sub lnkSaveCoverages_Click(sender As Object, e As EventArgs) Handles lnkSaveCoverages.Click
        If Me.Quote IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then

            End If
        End If
    End Sub

    Protected Sub lnkClearCoverage_Click(sender As Object, e As EventArgs) Handles lnkClearCoverage.Click
        If Me.Quote IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then

            End If
        End If
    End Sub

End Class