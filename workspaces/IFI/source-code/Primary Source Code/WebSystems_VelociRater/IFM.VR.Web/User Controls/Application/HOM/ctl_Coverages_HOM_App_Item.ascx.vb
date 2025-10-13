Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.HOM
Imports IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper
Imports QuickQuote.CommonObjects

Public Class ctl_Coverages_HOM_App_Item
    Inherits VRControlBase

    'Added 2/27/18 control for HOM Upgrade MLW
    Dim _qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Dim _chc As New CommonHelperClass
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

    Public Property MySectionIAppGapList As List(Of QuickQuoteSectionICoverage)
        Get
            'Dim sectionIAppGapList As List(Of QuickQuoteSectionICoverage) = New List(Of QuickQuoteSectionICoverage)
            'sectionIAppGapList = IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper.GetAppGapSectionICoverages(Quote)
            'Return sectionIAppGapList
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
            If Me.SectionType = SectionCoverage.QuickQuoteSectionCoverageType.NotDefined Then
                Me.ViewState("vs_SectionCovIAndIIType") = value
            End If
        End Set
    End Property

    Public ReadOnly Property MySectionCoverage As HomAppGapCoveragesHelper
        Get
            Dim genericCov As HomAppGapCoveragesHelper = Nothing
            'Updated 8/23/18 for multi state MLW
            'If Me.Quote.IsNotNull AndAlso Me.Quote.Locations.HasItemAtIndex(0) Then
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(0) Then
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
                        'Updated 8/23/18 for multi state MLW
                        'If myCovList.IsNotNull Then
                        If myCovList IsNot Nothing Then
                            Dim cov As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = DirectCast(myCovList(CoverageIndex), QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)
                            If cov IsNot Nothing Then
                                genericCov = New HomAppGapCoveragesHelper(cov)
                            End If
                        End If
                    Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        'Updated 8/23/18 for multi state MLW
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


    Public Property myCovList As List(Of Object)
        Get
            Dim myCov As Object
            Dim myList As New List(Of Object)
            If MySectionIAppGapList IsNot Nothing Then
                For Each c As QuickQuoteSectionICoverage In MySectionIAppGapList
                    myCov = c
                    myList.Add(myCov)
                Next
            End If
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

    Public Property isFirstAppGapCoverage As Boolean
        Get
            If ViewState("vs_isFirstAppGapCoverage") Is Nothing Then
                ViewState("vs_isFirstAppGapCoverage") = False
            End If
            Return ViewState("vs_isFirstAppGapCoverage")
        End Get
        Set(value As Boolean)
            ViewState("vs_isFirstAppGapCoverage") = value
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


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Me.Quote IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                If showOnAppGap = True Then
                    Me.VRScript.CreateConfirmDialog(Me.lnkClearCoverage.ClientID, "Clear?")
                    Me.VRScript.StopEventPropagation(Me.lnkSaveCoverages.ClientID)

                    If MyLocation Is Nothing Then
                        Me.VRScript.CreateAccordion(Me.divCoverageMain.ClientID, Nothing, "false")
                        divCoverageContent.Visible = False
                        divAdditionalInterest.Visible = False
                    Else
                        divCoverageContent.Visible = True
                        divAdditionalInterest.Visible = True
                        Me.VRScript.CreateAccordion(Me.divCoverageMain.ClientID, Me.hiddenCoverageAccordActive, "0")

                        If isFirstAppGapCoverage = False Then
                            lblHeader.Visible = True
                            lblHeader.Text = ""
                            Me.VRScript.AddScriptLine("$(""#" + divCoverageMain.ClientID + """).accordion({icons: false});")
                            Me.VRScript.AddScriptLine("$(""#" + Me.headerBar.ClientID + """).css({'border-left':'1px solid #EEE', 'border-right':'1px solid #EEE', 'border-top':'1px solid #EEE', 'border-bottom':'1px solid #DDD', 'background':'#EEE'});")
                            Me.VRScript.AddScriptLine("$(""#" + Me.lnkClearCoverage.ClientID + """).css({'color':'#333', 'text-decoration':'none'});")
                            Me.VRScript.AddScriptLine("$(""#" + Me.lnkSaveCoverages.ClientID + """).css({'color':'#333', 'text-decoration':'none'});")
                            Me.VRScript.AddScriptLine("$(""#" + Me.lblHeader.ClientID + """).addClass('transparent-header-label');")
                        End If

                        'for description field max char count
                        Select Case Me.SectionType
                            Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionICoverage
                                Select Case Me.SectionCoverageIEnum
                                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures,
                                         QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises
                                        'Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
                                        'Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises,
                                        '     QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises
                                        Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                                            VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)
                                            txtDescription.Attributes.Add("onfocus", "this.select()")
                                    End Select
                            Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIICoverage
                                Select Case Me.SectionCoverageIIEnum
                                    Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther,
                                         QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion,
                                         QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres,
                                         QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability,
                                         QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises,
                                         QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence
                                        Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                                        VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)
                                        txtDescription.Attributes.Add("onfocus", "this.select()")
                                End Select
                            Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                                Select Case Me.SectionCoverageIAndIIEnum
                                    Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold,
                                         QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers
                                        Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                                        VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)
                                        txtDescription.Attributes.Add("onfocus", "this.select()")
                                    Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures
                                        'Matt A Bug 26340 - Made the character count max 125 - to do this needed to add new parameter to function CountDescLength in VrAll.js
                                        Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """,125);"
                                        VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)
                                        txtDescription.Attributes.Add("onfocus", "this.select()")
                                        'If MySectionCoverage IsNot Nothing Then
                                        '    If IsNullEmptyorWhitespace(MySectionCoverage.BuildingLimit) Then
                                        '    Else
                                        Dim scriptDescCount2 As String = "CountDescLength(""" + txtDescription2.ClientID + """, """ + lblMaxCharCount2.ClientID + """, """ + hiddenMaxCharCount2.ClientID + """,125);"
                                        VRScript.CreateJSBinding(Me.txtDescription2.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount2, True)
                                        txtDescription2.Attributes.Add("onfocus", "this.select()")
                                        '    End If
                                        'End If
                                End Select
                        End Select

                        End If
                        Else
                    lblHeader.Visible = False
                    lblHeader.Text = ""
                    lnkClearCoverage.Visible = False
                    lnkSaveCoverages.Visible = False
                End If
            End If
        End If
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        If Me.Quote IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                If showOnAppGap = True Then
                    MyBase.ValidateControl(valArgs)
                    Me.ValidationHelper.GroupName = Me.CoverageName
                    Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

                    If MySectionCoverage IsNot Nothing Then
                        Dim valItems = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AppGapCoveragesValidator.ValidateAppGapCoverage(Quote, MySectionCoverage, CoverageIndex, valArgs.ValidationType)
                        If valItems.Any() Then
                            For Each v In valItems
                                Select Case v.FieldId
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AppGapCoveragesValidator.Description
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDescription.ClientID, v, accordList)
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.AppGapCoveragesValidator.BuildingDescription
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDescription2.ClientID, v, accordList)
                                End Select
                            Next
                        End If

                        Select Case Me.SectionType
                            Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionICoverage
                                Select Case Me.SectionCoverageIEnum
                                    Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises
                                        'CoverageName = "Specific Structures Away from Residence Premises<br />(HO 0492)"
                                        ctl_CoverageAddress_HOM_App_Item.CoverageName = Me.CoverageName
                                        ctl_CoverageAddress_HOM_App_Item.ValidateControl(valArgs)
                                End Select
                            Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIICoverage
                                Select Case Me.SectionCoverageIIEnum
                                    Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured,
                                     QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther,
                                     QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres,
                                     QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises,
                                     QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence
                                        ctl_CoverageAddress_HOM_App_Item.CoverageName = Me.CoverageName
                                        ctl_CoverageAddress_HOM_App_Item.ValidateControl(valArgs)
                                End Select
                            Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                                Select Case Me.SectionCoverageIAndIIEnum
                                    Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence
                                        ctl_CoverageAddress_HOM_App_Item.CoverageName = Me.CoverageName
                                        ctl_CoverageAddress_HOM_App_Item.ValidateControl(valArgs)
                                    Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage,
                                     QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement
                                        ctl_CoveragesAdditionalInterestsList_HOM_App.CoverageName = Me.CoverageName
                                        ctl_CoveragesAdditionalInterestsList_HOM_App.ValidateControl(valArgs)
                                End Select
                        End Select
                    End If
                End If
            End If
        End If

    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        If Me.Quote IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                If MySectionCoverage IsNot Nothing Then
                    If showOnAppGap = True Then
                        divAdditionalInterest.Visible = False
                        divAppSpecialText.Visible = False 'Added 11/19/2019 for bug 27734 MLW
                        Select Case Me.SectionType
                            Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionICoverage
                                Select Case Me.SectionCoverageIEnum
                                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures
                                        'Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
                                        'Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises
                                        CoverageName = "Other Structures On the Residence Premises (HO 0448)"
                                        divDescription.Visible = True
                                        If MySectionCoverage.Description <> "OTHER STRUCTURE" Then
                                            txtDescription.Text = MySectionCoverage.Description
                                        End If
                                        divAddress.Visible = False
                                    Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises
                                        CoverageName = "Specific Structures Away from Residence Premises<br />(HO 0492)"
                                        divDescription.Visible = True
                                        If Left(MySectionCoverage.Description, 11) = "STRUCTURE #" Then
                                        Else
                                            txtDescription.Text = MySectionCoverage.Description
                                        End If
                                        divAddress.Visible = True
                                        Me.ctl_CoverageAddress_HOM_App_Item.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoverageAddress_HOM_App_Item.SectionCoverageIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoverageAddress_HOM_App_Item.myCovList = myCovList
                                        Me.ctl_CoverageAddress_HOM_App_Item.Populate()
                                End Select
                            Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIICoverage
                                Select Case Me.SectionCoverageIIEnum
                                    Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured
                                        'Updated 6/13/2019 for Bug 31870 MLW
                                        'CoverageName = "Additional Residence - Occupied by Insured (N/A)"
                                        CoverageName = "Other Insured Location Occupied by Insured"
                                        divDescription.Visible = False
                                        divAddress.Visible = True
                                        Me.ctl_CoverageAddress_HOM_App_Item.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoverageAddress_HOM_App_Item.SectionCoverageIIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoverageAddress_HOM_App_Item.myCovList = myCovList
                                        Me.ctl_CoverageAddress_HOM_App_Item.Populate()
                                    Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther
                                        CoverageName = "Additional Residence - Rented to Others (HO 2470)"
                                        divDescription.Visible = True
                                        If Left(MySectionCoverage.Description, 22) <> "ADDITIONAL RESIDENCE #" Then
                                            txtDescription.Text = MySectionCoverage.Description
                                        End If
                                        divAddress.Visible = True
                                        Me.ctl_CoverageAddress_HOM_App_Item.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoverageAddress_HOM_App_Item.SectionCoverageIIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoverageAddress_HOM_App_Item.myCovList = myCovList
                                        Me.ctl_CoverageAddress_HOM_App_Item.Populate()
                                    Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion
                                        CoverageName = "Canine Liability Exclusion (HO 2477)"
                                        divName.Visible = True
                                        lblName.Text = "Canine Name"
                                        txtName.Enabled = False
                                        txtName.BackColor = Drawing.Color.WhiteSmoke 'LightGray
                                        txtName.ForeColor = Drawing.Color.Black
                                        divDescription.Visible = True
                                        txtName.Text = MySectionCoverage.Name.FirstName
                                        If Left(MySectionCoverage.Description, 8) <> "CANINE #" Then
                                            txtDescription.Text = MySectionCoverage.Description
                                        End If
                                        divAddress.Visible = False
                                        'Added 11/19/2019 for bug 27734 MLW
                                        divAppSpecialText.Visible = True
                                        'NOTE: if you change the text in this label, then you need to check/change the jQuery that checks for certain text in the label's div that only shows this message for the last one for multiple canine entries - in VrHomeLine.js at top
                                        lblAppSpecialText.Text = "<br />Form 2477 10 17 (Canine Liability Exclusion Endorsement) must be completed, signed and returned to Underwriting <span style='color:red;'>AND approved by your underwriter</span> before this policy can be issued. Please click <a href='" + System.Configuration.ConfigurationManager.AppSettings("HOM_Help_CanineForm") + "'  target='_blank' style='color:blue;font-weight:bold;'>here</a> for the form."
                                    Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres
                                        CoverageName = "Farm Owned and Operated By Insured: 0-100 Acres<br />(HO 2446)"
                                        divDescription.Visible = True
                                        If Left(MySectionCoverage.Description, 6) <> "FARM #" Then
                                            txtDescription.Text = MySectionCoverage.Description
                                        End If
                                        divAddress.Visible = True
                                        Me.ctl_CoverageAddress_HOM_App_Item.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoverageAddress_HOM_App_Item.SectionCoverageIIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoverageAddress_HOM_App_Item.myCovList = myCovList
                                        Me.ctl_CoverageAddress_HOM_App_Item.Populate()
                                    Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability
                                        CoverageName = "Incidental Farming Personal Liability - On Premises<br />(HO 2472)"
                                        divDescription.Visible = True
                                        If MySectionCoverage.Description <> "PERSONAL LIABILITY" Then
                                            txtDescription.Text = MySectionCoverage.Description
                                        End If
                                        divAddress.Visible = False
                                    Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises
                                        CoverageName = "Incidental Farming Personal Liability - Off Premises<br />(HO 2472)"
                                        divDescription.Visible = True
                                        If Left(MySectionCoverage.Description, 23) <> "OFF PREMISES LOCATION #" Then
                                            txtDescription.Text = MySectionCoverage.Description
                                        End If
                                        divAddress.Visible = True
                                        Me.ctl_CoverageAddress_HOM_App_Item.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoverageAddress_HOM_App_Item.SectionCoverageIIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoverageAddress_HOM_App_Item.myCovList = myCovList
                                        Me.ctl_CoverageAddress_HOM_App_Item.Populate()
                                    Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence
                                        CoverageName = "Permitted Incidental Occupancies Other Residence<br />(HO 2443)"
                                        divDescription.Visible = True
                                        If Left(MySectionCoverage.Description, 24) <> "INCIDENTAL OCCUPANCIES #" Then
                                            txtDescription.Text = MySectionCoverage.Description
                                        End If
                                        divAddress.Visible = True
                                        Me.ctl_CoverageAddress_HOM_App_Item.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoverageAddress_HOM_App_Item.SectionCoverageIIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoverageAddress_HOM_App_Item.myCovList = myCovList
                                        Me.ctl_CoverageAddress_HOM_App_Item.Populate()
                                End Select
                            Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                                Select Case Me.SectionCoverageIAndIIEnum
                                    Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence
                                        CoverageName = "Additional Insured – Student Living Away from Residence<br />(HO 0527)"
                                        'Updated 4/30/18 for Bug 26102 MLW
                                        'divAddress.Visible = False
                                        'divCoverageMain.Visible = False
                                        'divCoverageContent.Visible = False
                                        'divAdditionalInterest.Visible = True
                                        'Me.ctl_CoveragesAdditionalInterestsList_HOM_App.CoverageIndex = Me.CoverageIndex
                                        'Me.ctl_CoveragesAdditionalInterestsList_HOM_App.SectionCoverageIAndIIEnum = MySectionCoverage.CoverageType
                                        'Me.ctl_CoveragesAdditionalInterestsList_HOM_App.CoverageName = CoverageName
                                        'Me.ctl_CoveragesAdditionalInterestsList_HOM_App.appGapAIStatusList = appGapAIStatusList
                                        'Me.ctl_CoveragesAdditionalInterestsList_HOM_App.Populate()
                                        divName.Visible = True
                                        divDescription.Visible = True
                                        lblName.Text = "Name of Student"
                                        txtName.Enabled = False
                                        txtName.BackColor = Drawing.Color.WhiteSmoke
                                        txtName.ForeColor = Drawing.Color.Black
                                        txtDescription.Enabled = False
                                        txtDescription.BackColor = Drawing.Color.WhiteSmoke
                                        txtDescription.ForeColor = Drawing.Color.Black
                                        lblDescription.Text = "Name of School"
                                        txtName.Text = MySectionCoverage.AdditionalInterests(0).Name.CommercialName1
                                        txtDescription.Text = MySectionCoverage.AdditionalInterests(0).Description
                                        lblMaxChar.Visible = False
                                        divAddress.Visible = True
                                        Me.ctl_CoverageAddress_HOM_App_Item.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoverageAddress_HOM_App_Item.SectionCoverageIAndIIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoverageAddress_HOM_App_Item.myCovList = myCovList
                                        Me.ctl_CoverageAddress_HOM_App_Item.AdditionalInterestIndex = 0 'will always have only 1 for this coverage type
                                        Me.ctl_CoverageAddress_HOM_App_Item.Populate()
                                    Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage
                                        CoverageName = "Assisted Living Care Coverage (HO 0459)"
                                        'Updated 4/30/18 for Bug 26102 MLW
                                        divAddress.Visible = False
                                        divCoverageMain.Visible = False
                                        divCoverageMain.Visible = False
                                        divAdditionalInterest.Visible = True
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.SectionCoverageIAndIIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.CoverageName = CoverageName
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.appGapAIStatusList = appGapAIStatusList
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.Populate()
                                    Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold
                                        CoverageName = "Other Members of Your Household (HO 0458)"
                                        divName.Visible = True
                                        divDescription.Visible = True
                                        lblName.Text = "Member Name"
                                        txtName.Enabled = False
                                        txtName.BackColor = Drawing.Color.WhiteSmoke
                                        txtName.ForeColor = Drawing.Color.Black
                                        Dim memberFirstName As String = MySectionCoverage.Name.FirstName
                                        Dim memberMiddleName As String = MySectionCoverage.Name.MiddleName
                                        Dim memberLastName As String = MySectionCoverage.Name.LastName
                                        Dim memberSuffixName As String = MySectionCoverage.Name.SuffixName
                                        Dim memberName As String = Nothing
                                        If memberFirstName IsNot Nothing AndAlso memberFirstName <> "" Then
                                            memberName = memberFirstName
                                        End If
                                        If memberMiddleName IsNot Nothing AndAlso memberMiddleName <> "" Then
                                            memberName += " " + memberMiddleName
                                        End If
                                        If memberLastName IsNot Nothing AndAlso memberLastName <> "" Then
                                            memberName += " " + memberLastName
                                        End If
                                        If memberSuffixName IsNot Nothing AndAlso memberSuffixName <> "" Then
                                            memberName += " " + memberSuffixName
                                        End If
                                        txtName.Text = memberName
                                        If Left(MySectionCoverage.Description, 14) <> "OTHER MEMBER #" Then
                                            txtDescription.Text = MySectionCoverage.Description
                                        End If
                                        divAddress.Visible = False
                                    Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures
                                        CoverageName = "Permitted Incidental Occupancies Residence Premises<br />(HO 0442)"
                                        divDescription.Visible = True
                                        lblDescription.Text = "*Business Description"

                                        Dim txtBusinessDescr As String = Nothing
                                        Dim txtBuildingDescr As String = Nothing
                                        If MySectionCoverage.Description.Contains(vbNewLine) Then
                                            Dim txtDescrSplit As Array = Split(MySectionCoverage.Description, vbNewLine)
                                            txtBusinessDescr = txtDescrSplit(0)
                                            txtBuildingDescr = txtDescrSplit(1)
                                        Else
                                            txtBusinessDescr = MySectionCoverage.Description
                                            txtBuildingDescr = ""
                                        End If

                                        If txtBusinessDescr <> "BUSINESS" Then
                                            txtDescription.Text = txtBusinessDescr
                                        End If

                                        If _chc.NumericStringComparison(MySectionCoverage.BuildingLimit, CommonHelperClass.ComparisonOperators.GreaterThan, 0) Then
                                            divDescription2.Visible = True
                                            lblDescription2.Text = "Other Structures<br />*Building Description"
                                            If txtBuildingDescr <> "BUILDING" Then
                                                txtDescription2.Text = txtBuildingDescr
                                            End If
                                        End If

                                        divAddress.Visible = False
                                    Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers
                                        CoverageName = "Structures Rented To Others - Residence Premises<br />(HO 0440)"
                                        divDescription.Visible = True
                                        If MySectionCoverage.Description <> "STRUCTURE" Then
                                            txtDescription.Text = MySectionCoverage.Description
                                        End If
                                        divAddress.Visible = False
                                    Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement
                                        CoverageName = "Trust Endorsement (HO 0615)"
                                        divAddress.Visible = False
                                        divCoverageMain.Visible = False
                                        divCoverageMain.Visible = False
                                        divAdditionalInterest.Visible = True
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.SectionCoverageIAndIIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.CoverageName = CoverageName
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.appGapAIStatusList = appGapAIStatusList
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.Populate()
                                End Select
                        End Select

                        Me.lblHeader.Text = Me.CoverageName
                    End If
                End If
            End If
        End If

    End Sub
    Public Overrides Function Save() As Boolean
        If Me.Quote IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                If MySectionCoverage IsNot Nothing Then
                    If showOnAppGap = True Then
                        Select Case Me.SectionType
                            Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionICoverage
                                Select Case Me.SectionCoverageIEnum
                                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures
                                        'Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
                                        'Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises
                                        If txtDescription.Text = "OTHER STRUCTURE" Then
                                            txtDescription.Text = ""
                                        End If
                                        MySectionCoverage.Description = txtDescription.Text
                                    Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises
                                        If Left(txtDescription.Text, 11) = "STRUCTURE #" Then
                                            txtDescription.Text = ""
                                        End If
                                        MySectionCoverage.Description = txtDescription.Text
                                        Me.ctl_CoverageAddress_HOM_App_Item.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoverageAddress_HOM_App_Item.SectionCoverageIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoverageAddress_HOM_App_Item.myCovList = myCovList
                                        Me.ctl_CoverageAddress_HOM_App_Item.Save()
                                End Select
                            Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIICoverage
                                Select Case Me.SectionCoverageIIEnum
                                    Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured
                                        'CoverageName = "Additional Residence - Occupied by Insured (N/A)"
                                        Me.ctl_CoverageAddress_HOM_App_Item.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoverageAddress_HOM_App_Item.SectionCoverageIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoverageAddress_HOM_App_Item.myCovList = myCovList
                                        Me.ctl_CoverageAddress_HOM_App_Item.Save()
                                    Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther
                                        'CoverageName = "Additional Residence - Rented to Others (HO 2470)"
                                        If Left(txtDescription.Text, 22) = "ADDITIONAL RESIDENCE #" Then
                                            txtDescription.Text = ""
                                        End If
                                        MySectionCoverage.Description = txtDescription.Text
                                        Me.ctl_CoverageAddress_HOM_App_Item.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoverageAddress_HOM_App_Item.SectionCoverageIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoverageAddress_HOM_App_Item.myCovList = myCovList
                                        Me.ctl_CoverageAddress_HOM_App_Item.Save()
                                    Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion
                                        If Left(txtDescription.Text, 8) = "CANINE #" Then
                                            txtDescription.Text = ""
                                        End If
                                        MySectionCoverage.Description = txtDescription.Text
                                    Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres
                                        If Left(txtDescription.Text, 6) = "FARM #" Then
                                            txtDescription.Text = ""
                                        End If
                                        MySectionCoverage.Description = txtDescription.Text
                                        Me.ctl_CoverageAddress_HOM_App_Item.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoverageAddress_HOM_App_Item.SectionCoverageIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoverageAddress_HOM_App_Item.myCovList = myCovList
                                        Me.ctl_CoverageAddress_HOM_App_Item.Save()
                                    Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability
                                        'CoverageName = "Incidental Farming Personal Liability - On Premises<br />(HO 2472)"
                                        If txtDescription.Text = "PERSONAL LIABILITY" Then
                                            txtDescription.Text = ""
                                        End If
                                        MySectionCoverage.Description = txtDescription.Text
                                    Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises
                                        If Left(txtDescription.Text, 23) = "OFF PREMISES LOCATION #" Then
                                            txtDescription.Text = ""
                                        End If
                                        MySectionCoverage.Description = txtDescription.Text
                                        Me.ctl_CoverageAddress_HOM_App_Item.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoverageAddress_HOM_App_Item.SectionCoverageIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoverageAddress_HOM_App_Item.myCovList = myCovList
                                        Me.ctl_CoverageAddress_HOM_App_Item.Save()
                                    Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence
                                        If Left(txtDescription.Text, 24) = "INCIDENTAL OCCUPANCIES #" Then
                                            txtDescription.Text = ""
                                        End If
                                        MySectionCoverage.Description = txtDescription.Text
                                        Me.ctl_CoverageAddress_HOM_App_Item.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoverageAddress_HOM_App_Item.SectionCoverageIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoverageAddress_HOM_App_Item.myCovList = myCovList
                                        Me.ctl_CoverageAddress_HOM_App_Item.Save()
                                End Select
                            Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                                Select Case Me.SectionCoverageIAndIIEnum
                                    Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence
                                        'Added 5/3/18 for Bug 26102 MLW
                                        If SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence Then
                                            MySectionCoverage.AdditionalInterests = Nothing
                                            MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                                            Dim dummyItem As New QuickQuoteAdditionalInterest
                                            dummyItem.Name.CommercialName1 = Me.txtName.Text
                                            dummyItem.Description = Me.txtDescription.Text
                                            dummyItem.Address.StateId = "63" 'Need to initially save to blank, will save again below - added 4/30/18 for Bug 26344 MLW      
                                            dummyItem.TypeId = "81" 'Relative
                                            MySectionCoverage.AdditionalInterests.Add(dummyItem)
                                        End If
                                        Me.ctl_CoverageAddress_HOM_App_Item.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoverageAddress_HOM_App_Item.SectionCoverageIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoverageAddress_HOM_App_Item.myCovList = myCovList
                                        Me.ctl_CoverageAddress_HOM_App_Item.AdditionalInterestIndex = 0 'will always only have 1 item for this coverage type
                                        Me.ctl_CoverageAddress_HOM_App_Item.Save()
                                    Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.SectionCoverageIAndIIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.CoverageName = CoverageName
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.appGapAIStatusList = appGapAIStatusList
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.Save()
                                    Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold
                                        If Left(txtDescription.Text = "", 14) = "OTHER MEMBER #" Then
                                            txtDescription.Text = ""
                                        End If
                                        MySectionCoverage.Description = txtDescription.Text
                                    Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures
                                        'CoverageName = "Permitted Incidental Occupancies Residence Premises<br />(HO 0442)"
                                        'Business description and building description both use the description field
                                        If txtDescription.Text.ToUpper() = "BUSINESS" Then
                                            txtDescription.Text = ""
                                        End If
                                        If txtDescription2.Text.ToUpper() = "BUILDING" Then
                                            txtDescription2.Text = ""
                                        End If
                                        MySectionCoverage.Description = (txtDescription.Text & vbNewLine & txtDescription2.Text).ToMaxLength(250) ' Matt A - Bug 26340
                                    Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers
                                        'CoverageName = "Structures Rented To Others - Residence Premises<br />(HO 0440)"
                                        If txtDescription.Text = "STRUCTURE" Then
                                            txtDescription.Text = ""
                                        End If
                                        MySectionCoverage.Description = txtDescription.Text
                                    Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.CoverageIndex = Me.CoverageIndex
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.SectionCoverageIAndIIEnum = MySectionCoverage.CoverageType
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.CoverageName = CoverageName
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.appGapAIStatusList = appGapAIStatusList
                                        Me.ctl_CoveragesAdditionalInterestsList_HOM_App.Save()
                                End Select
                        End Select


                    End If
                End If
            End If
        End If
        Me.Populate()
        Return True
    End Function

    Protected Sub lnkSaveCoverages_Click(sender As Object, e As EventArgs) Handles lnkSaveCoverages.Click
        If Me.Quote IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            End If
        End If
    End Sub

    Protected Sub lnkClearCoverage_Click(sender As Object, e As EventArgs) Handles lnkClearCoverage.Click
        If Me.Quote IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                txtDescription.Text = ""
                txtDescription2.Text = ""
                Me.ClearChildControls()
                Me.LockTree()
            End If
        End If
    End Sub

End Class