Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods
Imports IFM.VR.Common.Helpers.HOM
Imports IFM.VR.Common.Helpers.HOM.SectionCoverage
Imports QuickQuote.CommonObjects

Public MustInherit Class ctlSectionCoverageControlBase
    Inherits VRControlBase

    'Select Case Me.CurrentFormTypeId
    'Case HO2
    'Case HO3, HO3_15
    'Case HO4
    'Case HO6
    'Case ML2, ML4
    'End Select

    Public Property IsDefaultCoverage As Boolean
        Get
            If ViewState("vs_isDefaultCoverage") Is Nothing Then
                ViewState("vs_isDefaultCoverage") = False
            End If
            Return CBool(ViewState("vs_isDefaultCoverage"))
        End Get
        Set(value As Boolean)
            ViewState("vs_isDefaultCoverage") = value
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

    ''' <summary>
    ''' Any of the simple coverages never use this property. It is used for coverages that have addresses. Each address is
    ''' a instance of the section coverage of a specific type. So when working with lists this index is needed.
    ''' </summary>
    ''' <returns></returns>
    Public Property CoverageIndex As Int32
        Get
            If ViewState("vs_Covindex") Is Nothing Then
                ViewState("vs_Covindex") = 0
            End If
            Return CInt(ViewState("vs_Covindex"))
        End Get
        Set(value As Int32)
            ViewState("vs_Covindex") = value
        End Set
    End Property


    Protected Property MyDisplayType As DisplayType
        Get
            If Me.ViewState("vs_MyDisplayType") IsNot Nothing Then
                Return [Enum].Parse(GetType(DisplayType), CInt(Me.ViewState("vs_MyDisplayType")))
            End If
            Return DisplayType.justCheckBox
        End Get
        Set(value As DisplayType)
            Me.ViewState("vs_MyDisplayType") = value
        End Set
    End Property

    Public Property IncludedLimit As String
        Get
            If ViewState("vs_IncludedLimit") Is Nothing Then
                ViewState("vs_IncludedLimit") = ""
            End If
            Return ViewState("vs_IncludedLimit").ToString()
        End Get
        Set(value As String)
            ViewState("vs_IncludedLimit") = value
        End Set
    End Property

    Public ReadOnly Property CurrentFormTypeId As Int32
        Get
            'If Me.MyLocation.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
            If Me.MyLocation IsNot Nothing Then
                If Int32.TryParse(Me.MyLocation.FormTypeId, Nothing) Then
                    Return CInt(Me.MyLocation.FormTypeId)
                End If
            End If
            Return -1
        End Get
    End Property

    Protected ReadOnly Property SectionType As SectionCoverage.QuickQuoteSectionCoverageType
        Get
            If Me.SectionCoverageIEnum <> QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.None Then
                Return SectionCoverage.QuickQuoteSectionCoverageType.SectionICoverage
            End If

            If Me.SectionCoverageIIEnum <> QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.None Then
                Return SectionCoverage.QuickQuoteSectionCoverageType.SectionIICoverage
            End If

            If Me.SectionCoverageIAndIIEnum <> QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None Then
                Return SectionCoverage.QuickQuoteSectionCoverageType.SectionIAndIICoverage
            End If
            Return SectionCoverage.QuickQuoteSectionCoverageType.NotDefined
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
                SetCoverageDisplayProperties()
                '#If DEBUG Then
                '            Else
                '                Throw New Exception("You can not change the Section Type once it has been set.")
                '#End If
            End If

        End Set
    End Property

    'Added 5/16/2022 for task 74106 MLW
    Public Property AssociatedSectionICoverageEnum As QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType
        Get
            If Me.ViewState("vs_AssociatedSectionICoverageEnum") IsNot Nothing Then
                Return [Enum].Parse(GetType(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType), CInt(Me.ViewState("vs_AssociatedSectionICoverageEnum")))
            End If
            Return QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.None
        End Get
        Set(value As QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType)
            If Me.SectionType = SectionCoverage.QuickQuoteSectionCoverageType.NotDefined Then
                Me.ViewState("vs_AssociatedSectionICoverageEnum") = value
                SetCoverageDisplayProperties()
                '#If DEBUG Then
                '            Else
                '                Throw New Exception("You can not change the Section Type once it has been set.")
                '#End If
            End If

        End Set
    End Property

    'Added 5/16/2022 for task 74106 MLW
    Public Sub SetSectionICoverageEnumAndAssociate(ByVal covType As QuickQuoteSectionICoverage.HOM_SectionICoverageType, ByVal assocType As QuickQuoteSectionICoverage.HOM_SectionICoverageType)
        If Me.SectionType = SectionCoverage.QuickQuoteSectionCoverageType.NotDefined Then
            Me.ViewState("vs_SectionCovIType") = covType
            Me.ViewState("vs_AssociatedSectionICoverageEnum") = assocType
            SetCoverageDisplayProperties()
            '#If DEBUG Then
            '            Else
            '                Throw New Exception("You can not change the Section Type once it has been set.")
            '#End If
        End If
    End Sub

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
                SetCoverageDisplayProperties()
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
                SetCoverageDisplayProperties()
            End If
        End Set
    End Property

    Public Property SectionCoverageIAndIIEnum_Property As QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIIPropertyCoverageType
        Get
            If Me.ViewState("vs_SectionCovIAndIIType_property") IsNot Nothing Then
                Return [Enum].Parse(GetType(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIIPropertyCoverageType), CInt(Me.ViewState("vs_SectionCovIAndIIType_property")))
            End If
            Return QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIIPropertyCoverageType.None
        End Get
        Set(value As QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIIPropertyCoverageType)
            Me.ViewState("vs_SectionCovIAndIIType_property") = value
        End Set
    End Property

    Public Property SectionCoverageIAndIIEnum_Liability As QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIILiabilityCoverageType
        Get
            If Me.ViewState("vs_SectionCovIAndIIType_Liability") IsNot Nothing Then
                Return [Enum].Parse(GetType(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIILiabilityCoverageType), CInt(Me.ViewState("vs_SectionCovIAndIIType_Liability")))
            End If
            Return QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIILiabilityCoverageType.None
        End Get
        Set(value As QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIILiabilityCoverageType)
            Me.ViewState("vs_SectionCovIAndIIType_Liability") = value
            SetCoverageDisplayProperties()
        End Set
    End Property




    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing Then
                Return Me.Quote.Locations.GetItemAtIndex(0)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property MySectionCoverage As SectionCoverage
        Get
            Dim genericCov As SectionCoverage = Nothing
            'If Me.Quote.IsNotNull AndAlso Me.Quote.Locations.HasItemAtIndex(0) Then 'Updated 8/22/18 for multi-state MLW
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(0) Then
                Select Case Me.SectionType
                    Case SectionCoverage.QuickQuoteSectionCoverageType.SectionICoverage
                        'If MyLocation.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                        If MyLocation IsNot Nothing Then
                            'If MyLocation.SectionICoverages.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                            If MyLocation.SectionICoverages IsNot Nothing Then
                                Dim cov = (From sc In MyLocation.SectionICoverages Where sc.HOM_CoverageType = Me.SectionCoverageIEnum Select sc).GetItemAtIndex(Me.CoverageIndex)
                                'cov = QQHelper.QuickQuoteSectionICoverageForType(MyLocation.SectionICoverages, Me.SectionCoverageIEnum, returnNewIfNothing:=False)
                                'QQHelper.RemoveQuickQuoteSectionICoveragesForType(MyLocation.SectionICoverages, Me.SectionCoverageIEnum)
                                'If cov.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                                If cov IsNot Nothing Then
                                    genericCov = New SectionCoverage(cov)
                                End If
                            End If
                        End If
                    Case SectionCoverage.QuickQuoteSectionCoverageType.SectionIICoverage
                        'If MyLocation.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                        If MyLocation IsNot Nothing Then
                            'If MyLocation.SectionIICoverages.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                            If MyLocation.SectionIICoverages IsNot Nothing Then
                                Dim cov = (From sc In MyLocation.SectionIICoverages Where sc.HOM_CoverageType = Me.SectionCoverageIIEnum Select sc).GetItemAtIndex(Me.CoverageIndex)
                                'If cov.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                                If cov IsNot Nothing Then
                                    genericCov = New SectionCoverage(cov)
                                End If
                            End If
                        End If
                    Case SectionCoverage.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        'If MyLocation.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                        If MyLocation IsNot Nothing Then
                            'If MyLocation.SectionIAndIICoverages.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                            If MyLocation.SectionIAndIICoverages IsNot Nothing Then
                                Dim cov = (From sc In MyLocation.SectionIAndIICoverages Where sc.MainCoverageType = Me.SectionCoverageIAndIIEnum Select sc).GetItemAtIndex(Me.CoverageIndex)
                                'If cov.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                                If cov IsNot Nothing Then
                                    genericCov = New SectionCoverage(cov)
                                End If
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

    Public ReadOnly Property MySubSectionCoverages As List(Of SectionCoverage)
        Get
            Dim genericCov As New List(Of SectionCoverage)
            If Me.CoverageIndex = 0 Then ' if not then this guy is a sub coverage itself so it has no sub coverages
                'If Me.Quote.IsNotNull AndAlso Me.Quote.Locations.HasItemAtIndex(0) Then 'Updated 8/22/18 for multi-state MLW
                If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(0) Then
                    Select Case Me.SectionType
                        Case SectionCoverage.QuickQuoteSectionCoverageType.SectionICoverage
                            'If MyLocation.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                            If MyLocation IsNot Nothing Then
                                'If MyLocation.SectionICoverages.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                                If MyLocation.SectionICoverages IsNot Nothing Then
                                    Dim cov = (From sc In MyLocation.SectionICoverages Where sc.HOM_CoverageType = Me.SectionCoverageIEnum Select sc).FirstOrDefault()
                                    'If cov.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                                    If cov IsNot Nothing Then
                                        For Each c In (From sc In MyLocation.SectionICoverages Where sc.HOM_CoverageType = Me.SectionCoverageIEnum Select sc)
                                            If Not c.Equals(cov) Then
                                                genericCov.Add(New SectionCoverage(c))
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        Case SectionCoverage.QuickQuoteSectionCoverageType.SectionIICoverage
                            'If MyLocation.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                            If MyLocation IsNot Nothing Then
                                'If MyLocation.SectionIICoverages.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                                If MyLocation.SectionIICoverages IsNot Nothing Then
                                    Dim cov = (From sc In MyLocation.SectionIICoverages Where sc.HOM_CoverageType = Me.SectionCoverageIIEnum Select sc).FirstOrDefault()
                                    'If cov.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                                    If cov IsNot Nothing Then
                                        For Each c In (From sc In MyLocation.SectionIICoverages Where sc.HOM_CoverageType = Me.SectionCoverageIIEnum Select sc)
                                            If Not c.Equals(cov) Then
                                                genericCov.Add(New SectionCoverage(c))
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        Case SectionCoverage.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                            'If MyLocation.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                            If MyLocation IsNot Nothing Then
                                'If MyLocation.SectionIAndIICoverages.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                                If MyLocation.SectionIAndIICoverages IsNot Nothing Then
                                    Dim cov = (From sc In MyLocation.SectionIAndIICoverages Where sc.MainCoverageType = Me.SectionCoverageIAndIIEnum Select sc).FirstOrDefault()
                                    'If cov.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                                    If cov IsNot Nothing Then
                                        For Each c In (From sc In MyLocation.SectionIAndIICoverages Where sc.MainCoverageType = Me.SectionCoverageIAndIIEnum Select sc)
                                            If Not c.Equals(cov) Then
                                                genericCov.Add(New SectionCoverage(c))
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        Case Else
                    End Select
                End If
            End If
            Return genericCov
        End Get
    End Property

    Public Sub InitFromExisting(existingControl As ctlSectionCoverageControlBase)
        Me.SectionCoverageIEnum = existingControl.SectionCoverageIEnum
        Me.SectionCoverageIIEnum = existingControl.SectionCoverageIIEnum
        Me.SectionCoverageIAndIIEnum = existingControl.SectionCoverageIAndIIEnum
    End Sub

    Public Sub CreateMySectionCoverage()
        Select Case Me.SectionType
            Case SectionCoverage.QuickQuoteSectionCoverageType.SectionICoverage
                Dim newCov = New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
                newCov.HOM_CoverageType = Me.SectionCoverageIEnum
                'Added 4/12/18 for HOM Upgrade MLW - Bug 26099
                If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    If Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises Then
                        'need to clear out the IN default selected state value for HOM Upgrade for coverages with multiple locations
                        newCov.Address.StateId = "0"
                    End If
                End If

                'add it
                Me.MyLocation.SectionICoverages.CreateIfNull()
                Me.MyLocation.SectionICoverages.Add(newCov)
            Case SectionCoverage.QuickQuoteSectionCoverageType.SectionIICoverage
                Dim newCov = New QuickQuote.CommonObjects.QuickQuoteSectionIICoverage()
                newCov.HOM_CoverageType = Me.SectionCoverageIIEnum
                'Added 4/12/18 for HOM Upgrade MLW - Bug 26099
                If (Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    Select Case Me.SectionCoverageIIEnum
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured,
                             QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther,
                             QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres,
                             QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises,
                             QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence
                            'need to clear out the IN default selected state value for HOM Upgrade for coverages with multiple locations
                            newCov.Address.StateId = "0"
                    End Select
                End If
                'add it
                Me.MyLocation.SectionIICoverages.CreateIfNull()
                Me.MyLocation.SectionIICoverages.Add(newCov)
            Case SectionCoverage.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                Dim newCov = New QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage()
                newCov.MainCoverageType = Me.SectionCoverageIAndIIEnum
                'newCov.LiabilityCoverageType = Me.SectionCoverageIAndIIEnum
                'newCov.PropertyCoverageType = Me.SectionCoverageIAndIIEnum
                'add it
                Me.MyLocation.SectionIAndIICoverages.CreateIfNull()
                Me.MyLocation.SectionIAndIICoverages.Add(newCov)
        End Select
    End Sub

    Public Sub DeleteMySectionCoverage()
        Select Case Me.SectionType
            Case SectionCoverage.QuickQuoteSectionCoverageType.SectionICoverage
                'find my coverage and its index
                'If MyLocation.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                If MyLocation IsNot Nothing Then
                    'If MyLocation.SectionICoverages.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                    If MyLocation.SectionICoverages IsNot Nothing Then
                        Dim cov = (From sc In MyLocation.SectionICoverages Where sc.HOM_CoverageType = Me.SectionCoverageIEnum Select sc).GetItemAtIndex(Me.CoverageIndex)
                        'If cov.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                        If cov IsNot Nothing Then
                            Me.MyLocation.SectionICoverages.Remove(cov)
                        End If
                    End If
                End If
            Case SectionCoverage.QuickQuoteSectionCoverageType.SectionIICoverage
                'If MyLocation.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                If MyLocation IsNot Nothing Then
                    'If MyLocation.SectionIICoverages.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                    If MyLocation.SectionIICoverages IsNot Nothing Then
                        Dim cov As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = Nothing
                        Select Case Me.SectionCoverageIIEnum
                            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.SpecialEventCoverage
                                ' SPECIAL EVENT COVERAGE - can have multiples
                                Dim s2Index As Integer = -1
                                cov = GetNewSpecialEventCoverage(s2Index)
                                Exit Select
                            Case Else
                                ' ALL OTHER COVERAGES (other that special event)
                                cov = (From sc In MyLocation.SectionIICoverages Where sc.HOM_CoverageType = Me.SectionCoverageIIEnum Select sc).GetItemAtIndex(Me.CoverageIndex)
                                Exit Select
                        End Select

                        If cov IsNot Nothing Then
                            Me.MyLocation.SectionIICoverages.Remove(cov)
                        End If

                        'Dim cov = (From sc In MyLocation.SectionIICoverages Where sc.HOM_CoverageType = Me.SectionCoverageIIEnum Select sc).GetItemAtIndex(Me.CoverageIndex)
                        ''If cov.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                        'If cov IsNot Nothing Then
                        '    Me.MyLocation.SectionIICoverages.Remove(cov)
                        'End If
                    End If
                End If

            Case SectionCoverage.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                'If MyLocation.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                If MyLocation IsNot Nothing Then
                    'If MyLocation.SectionIAndIICoverages.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                    If MyLocation.SectionIAndIICoverages IsNot Nothing Then
                        Dim cov = (From sc In MyLocation.SectionIAndIICoverages Where sc.MainCoverageType = Me.SectionCoverageIAndIIEnum Select sc).GetItemAtIndex(Me.CoverageIndex)
                        'If cov.IsNotNull Then 'Updated 8/22/18 for multi-state MLW
                        If cov IsNot Nothing Then
                            Me.MyLocation.SectionIAndIICoverages.Remove(cov)
                        End If
                    End If
                End If
        End Select
    End Sub

    Private Sub SetCoverageDisplayProperties()
        Dim covProperties = SectionCoverage.GetCoverageDisplayProperties(Me.Quote, Me.MyLocation,
                                              Me.SectionType,
                                              Me.SectionCoverageIEnum,
                                              Me.SectionCoverageIIEnum,
                                              Me.SectionCoverageIAndIIEnum,
                                              AssociatedSectionICoverageEnum:=Me.AssociatedSectionICoverageEnum)
        Me.MyDisplayType = covProperties.MyDisplayType
        Me.CoverageName = covProperties.Coveragename
        Me.IncludedLimit = covProperties.IncludedLimit
        Me.IsDefaultCoverage = covProperties.IsDefaultCoverage
    End Sub

    ''' <summary>
    ''' Retrieves the special event coverage that was entered on this endorsement.
    ''' Returns the index of the sectionIIcoverage in the ByRef argument
    ''' </summary>
    ''' <returns></returns>
    Public Function GetNewSpecialEventCoverage(ByRef SectionIIIndex As Integer) As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage
        Dim sc2Covs As New List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)
        Dim ndx As Integer = -1

        SectionIIIndex = -1

        If Quote IsNot Nothing AndAlso MyLocation IsNot Nothing Then
            If MyLocation.SectionIICoverages IsNot Nothing AndAlso MyLocation.SectionIICoverages.Count > 0 Then
                sc2Covs = MyLocation.SectionIICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.SpecialEventCoverage)
                If sc2Covs IsNot Nothing AndAlso sc2Covs.Count > 0 Then
                    For Each sc2 As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage In sc2Covs
                        If QQHelper.IsQuickQuoteSectionIICoverageNewToImage(sc2, Quote) Then
                            ' Get the index of the coverage in the section II list
                            For Each s2 As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage In MyLocation.SectionIICoverages
                                ndx += 1
                                If s2.Equals(sc2) Then
                                    SectionIIIndex = ndx
                                End If
                            Next
                            Return sc2
                        End If
                    Next
                End If
            End If
        End If
        Return Nothing
    End Function

    Public Function GetSpecialEventCheckboxLabelText()
        Dim txt As String = "Special Event Coverage (HOM 1005)"
        Dim cnt As Integer = 0

        ' Get the number of special event coverages on the policy that are not new
        Dim covs As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage) = MyLocation.SectionIICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.SpecialEventCoverage)
        If covs IsNot Nothing AndAlso covs.Count > 0 Then
            For Each cvrg As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage In covs
                If Not QQHelper.IsQuickQuoteSectionIICoverageNewToImage(cvrg, Quote) Then
                    cnt += 1
                End If
            Next
        End If

        If cnt > 0 Then
            txt += " - " & cnt.ToString() & " previously added event(s) scheduled"
        End If

        Return txt
    End Function

End Class

