Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.HOM
Imports IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper
Imports QuickQuote.CommonObjects

Public Class ctl_CoveragesAdditionalInterestsList_HOM_App
    Inherits VRControlBase

    'Added 3/22/18 control for HOM Upgrade MLW
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
            If _qqh.doUseNewVersionOfLOB(effectiveDate.ToString(), Quote.LobType, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade) = True Then
                'If _qqh.doUseNewVersionOfLOB(effectiveDate, Quote.LobType, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade) = True Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
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

    Public Property CoverageType As Int32
        Get
            If (ViewState("vs_coverageType") IsNot Nothing) Then
                Return CInt(ViewState("vs_coverageType"))
            End If
            Return -1
        End Get
        Set(value As Int32)
            ViewState("vs_coverageType") = value
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

    Public Property showAIOnAppGap As Boolean
        Get
            If ViewState("vs_showAIOnAppGap") Is Nothing Then
                ViewState("vs_showAIOnAppGap") = False
            End If
            Return ViewState("vs_showAIOnAppGap")
        End Get
        Set(value As Boolean)
            ViewState("vs_showAIOnAppGap") = value
        End Set
    End Property

    Public Property isFirstOfMultiple As Boolean
        Get
            If ViewState("vs_isFirstOfMultiple") Is Nothing Then
                ViewState("vs_isFirstOfMultiple") = False
            End If
            Return ViewState("vs_isFirstOfMultiple")
        End Get
        Set(value As Boolean)
            ViewState("vs_isFirstOfMultiple") = value
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

    Public Property MyAiTrusteeList As List(Of QuickQuoteAdditionalInterest)
        Get
            Dim AiTrusteeList As List(Of QuickQuoteAdditionalInterest) = New List(Of QuickQuoteAdditionalInterest)
            If MySectionCoverage.AdditionalInterests Is Nothing Then
                MySectionCoverage.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            End If
            For Each ai In MySectionCoverage.AdditionalInterests
                If ai.TypeId = "80" Then
                    AiTrusteeList.Add(ai)
                End If
            Next
            Return AiTrusteeList
        End Get
        Set(value As List(Of QuickQuoteAdditionalInterest))
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub


    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Me.Quote IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then


            End If
        End If
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        If Me.Quote IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then

                MyBase.ValidateControl(valArgs)
                Me.ValidationHelper.GroupName = Me.CoverageName
                Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
                ValidateChildControls(valArgs)

            End If
        End If
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        If Me.Quote IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then

                If MySectionCoverage IsNot Nothing Then
                    divAIList.Visible = True
                    If MySectionCoverage.AdditionalInterests IsNot Nothing Then
                        Me.Repeater1.DataSource = Me.MySectionCoverage.AdditionalInterests
                        Me.Repeater1.DataBind()
                        Me.FindChildVrControls()
                        Dim index As Int32 = 0
                        For Each c In Me.GatherChildrenOfType(Of ctl_CoveragesAdditionalInterests_HOM_App)
                            c.CoverageName = CoverageName
                            c.CoverageIndex = CoverageIndex
                            c.SectionCoverageIAndIIEnum = SectionCoverageIAndIIEnum
                            c.appGapAIStatusList = appGapAIStatusList
                            c.AdditionalInterestIndex = index
                            If appGapAIStatusList IsNot Nothing Then
                                showAIOnAppGap = appGapAIStatusList(index).ShowAIOnAppGap
                                isFirstOfMultiple = appGapAIStatusList(index).IsFirstOfMultiple 'for accordion header colors
                                c.showAIOnAppGap = showAIOnAppGap
                                c.isFirstOfMultiple = isFirstOfMultiple
                            Else
                                c.showAIOnAppGap = False
                                c.isFirstOfMultiple = False
                            End If
                            c.Populate()
                            index += 1
                        Next
                    End If
                End If

            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Quote IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                Me.SaveChildControls()
            End If
        End If

        Me.Populate()
        Return True
    End Function
End Class