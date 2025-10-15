Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.HOM
Imports IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper
Imports QuickQuote.CommonObjects

Public Class ctl_Coverages_HOM_App
    Inherits VRControlBase

    'Added 2/27/18 control for HOM Upgrade MLW
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

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            'Updated 8/23/18 for multi state MLW
            'If Me.Quote.IsNotNull AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations.GetItemAtIndex(0)
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


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If MyLocation IsNot Nothing Then
            'If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                If Not IsPostBack Then
                    Me.MainAccordionDivId = Me.divCoveragesMain.ClientID
                    'hiddenCoveragesAccordActive.Value = "0" 'expand
                    hiddenCoveragesAccordActive.Value = "false" 'collapse
                    hiddenHasAppGapCoverages.Value = hasAppGapCoveragesAvailable(Quote)
                End If
            End If
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)

            If MyLocation Is Nothing OrElse ((MySectionIAppGapList Is Nothing OrElse MySectionIAppGapList.Count = 0) AndAlso (MySectionIIAppGapList Is Nothing OrElse MySectionIIAppGapList.Count = 0) AndAlso (MySectionIAndIIAppGapList Is Nothing OrElse MySectionIAndIIAppGapList.Count = 0)) OrElse hiddenHasAppGapCoverages.Value = False Then
                divCoveragesContent.Visible = False
                hiddenCoveragesAccordActive.Value = "false" 'collapse
            Else
                divCoveragesContent.Visible = True
                hiddenCoveragesAccordActive.Value = "0" 'expand
            End If
            Me.VRScript.CreateAccordion(Me.divCoveragesMain.ClientID, Me.hiddenCoveragesAccordActive, "0")
        Else
            divCoveragesMain.Visible = False
            divCoveragesContent.Visible = False
        End If
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            MyBase.ValidateControl(valArgs)
            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Public Overrides Sub Populate()
        If Me.MyLocation IsNot Nothing Then
            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                LoadStaticData()
                Me.lblMainAccord.Text = String.Format("COVERAGES")

                If MySectionIAppGapList IsNot Nothing Then
                    Me.divSectionI.Visible = True
                    Me.covSectionIRepeater.DataSource = MySectionIAppGapList
                    Me.covSectionIRepeater.DataBind()
                Else
                    Me.divSectionI.Visible = False
                    Me.covSectionIRepeater.DataSource = Nothing
                    Me.covSectionIRepeater.DataBind()
                End If

                If MySectionIIAppGapList IsNot Nothing Then
                    Me.divSectionII.Visible = True
                    Me.covSectionIIRepeater.DataSource = MySectionIIAppGapList
                    Me.covSectionIIRepeater.DataBind()
                Else
                    Me.divSectionII.Visible = False
                    Me.covSectionIIRepeater.DataSource = Nothing
                    Me.covSectionIIRepeater.DataBind()
                End If

                If MySectionIAndIIAppGapList IsNot Nothing Then
                    Me.divSectionIandII.Visible = True
                    Me.covSectionIAndIIRepeater.DataSource = MySectionIAndIIAppGapList
                    Me.covSectionIAndIIRepeater.DataBind()
                Else
                    Me.divSectionIandII.Visible = False
                    Me.covSectionIAndIIRepeater.DataSource = Nothing
                    Me.covSectionIAndIIRepeater.DataBind()
                End If

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

                Me.FindChildVrControls() ' finds the just added controls do to the binding
                Dim index As Int32 = 0
                For Each child In Me.ChildVrControls
                    If TypeOf child Is ctl_Coverages_HOM_App_Item Then 'for multiple
                        Dim c As ctl_Coverages_HOM_App_Item = child 'for multiple
                        c.CoverageIndex = index
                        If myList(index).GetType Is GetType(QuickQuoteSectionICoverage) Then
                            c.SectionCoverageIEnum = myList(index).HOM_CoverageType
                            c.showOnAppGap = IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper.GetAppGapStatus(Quote, myList(index), QuickQuoteSectionCoverageType.SectionICoverage, c.SectionCoverageIEnum, c.SectionCoverageIIEnum, c.SectionCoverageIAndIIEnum)
                            c.isFirstAppGapCoverage = IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper.GetAppGapFirstCoverageStatus(Quote, index, myList, myList(index), QuickQuoteSectionCoverageType.SectionICoverage, c.SectionCoverageIEnum, c.SectionCoverageIIEnum, c.SectionCoverageIAndIIEnum)
                        ElseIf myList(index).GetType Is GetType(QuickQuoteSectionIICoverage) Then
                            c.SectionCoverageIIEnum = myList(index).HOM_CoverageType
                            c.showOnAppGap = IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper.GetAppGapStatus(Quote, myList(index), QuickQuoteSectionCoverageType.SectionIICoverage, c.SectionCoverageIEnum, c.SectionCoverageIIEnum, c.SectionCoverageIAndIIEnum)
                            c.isFirstAppGapCoverage = IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper.GetAppGapFirstCoverageStatus(Quote, index, myList, myList(index), QuickQuoteSectionCoverageType.SectionIICoverage, c.SectionCoverageIEnum, c.SectionCoverageIIEnum, c.SectionCoverageIAndIIEnum)
                        ElseIf myList(index).GetType Is GetType(QuickQuoteSectionIAndIICoverage) Then
                            c.SectionCoverageIAndIIEnum = myList(index).MainCoverageType 'myList(index).GetType
                            c.showOnAppGap = IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper.GetAppGapStatus(Quote, myList(index), QuickQuoteSectionCoverageType.SectionIAndIICoverage, c.SectionCoverageIEnum, c.SectionCoverageIIEnum, c.SectionCoverageIAndIIEnum)
                            c.isFirstAppGapCoverage = IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper.GetAppGapFirstCoverageStatus(Quote, index, myList, myList(index), QuickQuoteSectionCoverageType.SectionIAndIICoverage, c.SectionCoverageIEnum, c.SectionCoverageIIEnum, c.SectionCoverageIAndIIEnum)
                            'If myList(index).MainCoverageType = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence OrElse myList(index).MainCoverageType = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage OrElse myList(index).MainCoverageType = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement Then
                            If myList(index).MainCoverageType = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage OrElse myList(index).MainCoverageType = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement Then
                                c.appGapAIStatusList = IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper.GetAppGapAdditionalInterestStatusList(Quote, index, myList(index), QuickQuoteSectionCoverageType.SectionIAndIICoverage, c.SectionCoverageIAndIIEnum)
                            End If
                        End If
                        c.Populate()
                        index += 1
                    End If
                Next

            End If
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            Me.SaveChildControls()
        End If
        Return True
    End Function

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            Save_FireSaveEvent(New IFM.VR.Web.VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        End If
    End Sub

End Class