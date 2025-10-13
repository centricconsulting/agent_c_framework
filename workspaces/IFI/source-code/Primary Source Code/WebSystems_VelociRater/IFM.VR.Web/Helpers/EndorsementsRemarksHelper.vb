Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

'Added 03/31/2021 for CAP Endorsements Task 52973 MLW

Namespace Helpers
    Public Class EndorsementsRemarksHelper

        Private _ddh As DevDictionaryHelper.DevDictionaryHelper
        Public Property ddh() As DevDictionaryHelper.DevDictionaryHelper
            Get
                If _ddh Is Nothing Then
                    _ddh = New DevDictionaryHelper.DevDictionaryHelper(Quote, DictionaryName)
                End If
                Return _ddh
            End Get
            Set(ByVal value As DevDictionaryHelper.DevDictionaryHelper)
                _ddh = value
            End Set
        End Property

        'Added 09/28/2021 for BOP Endorsements Task 61660 MLW
        Private _comDictItems As DevDictionaryHelper.AllCommercialDictionary
        Public ReadOnly Property ComDictItems As DevDictionaryHelper.AllCommercialDictionary
            Get
                If Quote IsNot Nothing Then
                    If _comDictItems Is Nothing Then
                        _comDictItems = New DevDictionaryHelper.AllCommercialDictionary(Quote)
                    End If
                End If
                Return _comDictItems
            End Get
        End Property

        Private _quote As QuickQuoteObject
        Public Property Quote() As QuickQuoteObject
            Get
                Return _quote
            End Get
            Set(ByVal value As QuickQuoteObject)
                _quote = value
            End Set
        End Property

        Private _dictName As String
        Public Property DictionaryName() As String
            Get
                Return _dictName
            End Get
            Set(ByVal value As String)
                _dictName = value
            End Set
        End Property

        Enum RemarksType
            'CAP
            Driver
            Vehicle
            AdditionalInterest
            'BOP
            AdditionalInterestGeneral
            Location 'Added 09/28/2021 for BOP Endorsements Task 61660 MLW
            AllBopRemarks
            AllCppRemarks
        End Enum

        'Us this to set up from scratch
        Public Sub New(ByRef quote As QuickQuoteObject, ByRef dictName As String)
            _quote = quote
            _dictName = dictName
        End Sub

        'Just pass your existing Dictionary in if you have one.
        Public Sub New(ByRef DevDictionary As DevDictionaryHelper.DevDictionaryHelper)
            _ddh = DevDictionary
            _quote = DevDictionary.Quote
        End Sub

        'Added 09/28/2021 for BOP Endorsements Task 61660 MLW
        Public Sub New(ByRef ComDictItems As DevDictionaryHelper.AllCommercialDictionary)
            _comDictItems = ComDictItems
            _ddh = ComDictItems.ddh
            _quote = ComDictItems.ddh.Quote
        End Sub

        Public Sub New(ByRef quote As QuickQuoteObject)
            _quote = quote
        End Sub

        'Call this with a RemarkType and let it do the work
        'Dim remarksBldr = New EndorsementsRemarksHelper(ddh)
        'Dim FinalRemarks as string = remarksBldr.UpdateRemarks(remarksBldr.RemarksType.Driver)
        Public Function UpdateRemarks(remarkType As RemarksType) As String
            Select Case remarkType
                Case RemarksType.Driver
                    Return GetDriverRemarks()
                Case RemarksType.Vehicle
                    Return GetVehicleRemarks()
                Case RemarksType.AdditionalInterest
                    Return GetAdditionalInterestRemarks()
                Case RemarksType.AdditionalInterestGeneral
                    Return GetAdditionalInterestGeneralRemarks()
                Case RemarksType.Location 'Added 09/28/2021 for BOP Endorsements Task 61660 MLW
                    Return GetLocationRemarks()
                Case RemarksType.AllBopRemarks
                    Return GetBopRemarks()
                Case RemarksType.AllCppRemarks
                    Return GetCppRemarks()

            End Select
            Return String.Empty
        End Function

#Region "CAP Driver Specific"
        Private Function GetDriverRemarks() As String
            Dim RemarksText As String = String.Empty
            Dim AddDriverNames As List(Of String) = New List(Of String)
            Dim DelDriverNames As List(Of String) = New List(Of String)

            ProcessDriverList(AddDriverNames, DelDriverNames)

            If DelDriverNames.Count > 0 Then
                'RemarksText = String.Format("DELETED DRIVER{0} {1} AS A DRIVER FOR THIS POLICY.",
                '                            If(DelDriverNames.Count > 1, "S", String.Empty),
                '                            String.Join(", ", DelDriverNames))
                RemarksText = String.Format("DELETED {0} AS A DRIVER FOR THIS POLICY.",
                                            String.Join(", ", DelDriverNames))
            End If

            If DelDriverNames.Count > 0 AndAlso AddDriverNames.Count > 0 Then
                RemarksText &= " "
            End If

            If AddDriverNames.Count > 0 Then
                'RemarksText &= String.Format("ADDED DRIVER{0} {1} AS A DRIVER FOR THIS POLICY.",
                '                             If(AddDriverNames.Count > 1, "S", String.Empty),
                '                             String.Join(", ", AddDriverNames))
                RemarksText &= String.Format("ADDED {0} AS A DRIVER FOR THIS POLICY.",
                                             String.Join(", ", AddDriverNames))
            End If

            Return RemarksText
        End Function

        Private Sub ProcessDriverList(ByRef addList As List(Of String), ByRef delList As List(Of String))
            If ddh.GetDriverDictionary IsNot Nothing AndAlso ddh.GetDriverDictionary.Count > 0 Then
                For Each driver As DevDictionaryHelper.DriverInfo In ddh.GetDriverDictionary
                    If driver.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.addItem Then
                        addList.Add(driver.driverName)
                    ElseIf driver.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.deleteItem Then
                        delList.Add(driver.driverName)
                    End If
                Next
            End If
        End Sub
#End Region

#Region "CAP Vehicle Specific"
        Private Function GetVehicleRemarks() As String
            Dim RemarksText As String = String.Empty
            Dim AddVehicleInfoList As List(Of String) = New List(Of String)
            Dim DelVehiclesInfoList As List(Of String) = New List(Of String)

            ProcessVehicleList(AddVehicleInfoList, DelVehiclesInfoList)

            If DelVehiclesInfoList.Count > 0 Then
                RemarksText = String.Format("DELETED VEHICLE{0} {1} FROM THE POLICY. ALL COVERAGES ASSOCIATED WITH THE VEHICLE{0} HAVE BEEN DELETED.",
                                            If(DelVehiclesInfoList.Count > 1, "S", String.Empty),
                                            String.Join(", ", DelVehiclesInfoList))
            End If

            If DelVehiclesInfoList.Count > 0 AndAlso AddVehicleInfoList.Count > 0 Then
                RemarksText &= " "
            End If

            If AddVehicleInfoList.Count > 0 Then
                RemarksText &= String.Format("ADDED VEHICLE{0} {1} PER REVISED DECLARATIONS ATTACHED.",
                                             If(AddVehicleInfoList.Count > 1, "S", String.Empty),
                                             String.Join("; ", AddVehicleInfoList))
            End If

            Return RemarksText
        End Function

        Private Sub ProcessVehicleList(ByRef addList As List(Of String), ByRef delList As List(Of String))
            If ddh.GetVehicleDictionary IsNot Nothing AndAlso ddh.GetVehicleDictionary.Count > 0 Then
                For Each vehicle As DevDictionaryHelper.VehicleInfo In ddh.GetVehicleDictionary
                    If vehicle.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.addItem Then
                        Dim lastFourVin As String = vehicle.VIN
                        If Not String.IsNullOrWhiteSpace(vehicle.VIN) AndAlso vehicle.VIN.Length > 4 Then
                            lastFourVin = vehicle.VIN.Substring(vehicle.VIN.Length - 4).ToUpper()
                        End If
                        Dim addVehicleInfo As String = vehicle.Year + " " + vehicle.Make + " " + vehicle.Model + " " + lastFourVin
                        addList.Add(addVehicleInfo)
                        'addList.Add(vehicle.diaVehicleNumber)
                    ElseIf vehicle.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.deleteItem Then
                        Dim lastFourVin As String = vehicle.VIN
                        If Not String.IsNullOrWhiteSpace(vehicle.VIN) AndAlso vehicle.VIN.Length > 4 Then
                            lastFourVin = vehicle.VIN.Substring(vehicle.VIN.Length - 4).ToUpper()
                        End If
                        Dim delVehicleInfo As String = vehicle.Year + " " + vehicle.Make + " " + vehicle.Model + " " + lastFourVin
                        delList.Add(delVehicleInfo)
                        'delList.Add(vehicle.diaVehicleNumber)
                    End If
                Next
            End If
        End Sub
#End Region

#Region "CAP Additional Interest Specific"
        Private Function GetAdditionalInterestRemarks() As String
            Dim RemarksText As String = String.Empty
            Dim AddAdditionalInterestLienholders As List(Of String) = New List(Of String)
            Dim DelAdditionalInterestLienholders As List(Of String) = New List(Of String)

            ProcessAdditionalInterestList(AddAdditionalInterestLienholders, DelAdditionalInterestLienholders)

            If DelAdditionalInterestLienholders.Count > 0 Then
                RemarksText = String.Format("DELETED {0}.",
                                            String.Join("; ", DelAdditionalInterestLienholders), "0")
            End If

            If DelAdditionalInterestLienholders.Count > 0 AndAlso AddAdditionalInterestLienholders.Count > 0 Then
                RemarksText &= " "
            End If

            If AddAdditionalInterestLienholders.Count > 0 Then
                'RemarksText &= String.Format("ADDED ADDITIONAL INTEREST{0} {1} AS AN ADDITIONAL INTEREST PER REVISED DECLARATIONS ATTACHED.",
                '                             If(AddAdditionalInterestLienholders.Count > 1, "S", String.Empty),
                '                             String.Join(", ", AddAdditionalInterestLienholders))
                RemarksText &= String.Format("ADDED {0} AS AN ADDITIONAL INTEREST PER REVISED DECLARATIONS ATTACHED.",
                                             String.Join(", ", AddAdditionalInterestLienholders))
            End If

            Return RemarksText
        End Function

        Private Sub ProcessAdditionalInterestList(ByRef addList As List(Of String), ByRef delList As List(Of String))
            If ddh.GetAdditionalInterestDictionary IsNot Nothing AndAlso ddh.GetAdditionalInterestDictionary.Count > 0 Then

                For Each additionalInterest As DevDictionaryHelper.AdditionalInterestInfo In ddh.GetAdditionalInterestDictionary
                    If additionalInterest.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.addItem Then
                        addList.Add(Trim(additionalInterest.Lienholder))
                    ElseIf additionalInterest.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.deleteItem Then
                        Dim delAIInfo As String = Trim(additionalInterest.Lienholder)
                        If additionalInterest.VehicleNumList IsNot Nothing AndAlso additionalInterest.VehicleNumList.Count > 0 Then
                            If additionalInterest.VehicleNumList = 0 Then
                                delAIInfo &= " FROM POLICY"
                            Else
                                delAIInfo &= String.Format(" APPLICABLE TO VEHICLE{0} {1}",
                                                           If(additionalInterest.VehicleNumList.Contains(","), "S", String.Empty),
                                                           additionalInterest.VehicleNumList)
                            End If
                        Else
                            delAIInfo &= " FROM POLICY"
                        End If
                        delList.Add(delAIInfo)
                    End If
                Next
            End If
        End Sub
#End Region

#Region "Commercial Section"
        Private Function GetBopRemarks() As String
            ComDictItems.GetAllCommercialDictionary()
            Return GetLocationRemarks() & GetAdditionalInterestGeneralRemarks() & GetAssignedAdditionalInterestGeneralRemarks()
        End Function

        Private Function GetCppRemarks() As String
            ComDictItems.GetAllCommercialDictionary()
            Return GetLocationRemarks() & GetAdditionalInterestGeneralRemarks() & GetAssignedAdditionalInterestGeneralRemarks() & GetInlandMarineGeneralRemarks()
        End Function

#Region "Commercial Location Specific"
        'Added 09/28/2021 for BOP Endorsements Task 61660 MLW
        Private Function GetLocationRemarks() As String
            Dim RemarksText As String = String.Empty
            Dim AddLocationInfoList As List(Of String) = New List(Of String)
            Dim DelLocationInfoList As List(Of String) = New List(Of String)

            ProcessLocationList(AddLocationInfoList, DelLocationInfoList)

            If DelLocationInfoList.Count > 0 Then
                RemarksText = String.Format("DELETED {0} FROM THIS POLICY. ALL COVERAGES FOR {1} HAVE BEEN REMOVED.",
                                            String.Join("; ", DelLocationInfoList), If(DelLocationInfoList.Count > 1, "THESE LOCATIONS", "THIS LOCATION"))
            End If

            If DelLocationInfoList.Count > 0 AndAlso AddLocationInfoList.Count > 0 Then
                RemarksText &= " "
            End If

            If AddLocationInfoList.Count > 0 Then
                RemarksText &= String.Format("ADDED {0} PER REVISED DECLARATIONS ATTACHED.",
                                             String.Join("; ", AddLocationInfoList))
            End If

            Return RemarksText & " "
        End Function

        Private Sub ProcessLocationList(ByRef addList As List(Of String), ByRef delList As List(Of String))
            If ComDictItems IsNot Nothing AndAlso ComDictItems.Locations IsNot Nothing AndAlso ComDictItems.Locations.Count > 0 Then
                For Each location As DevDictionaryHelper.Location In ComDictItems.Locations
                    If location.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.addItem Then
                        addList.Add(location.LocationAddress)
                    ElseIf location.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.deleteItem Then
                        delList.Add(location.LocationAddress)
                    End If
                Next
            End If
        End Sub
#End Region

#Region "Commercial Assigned Additional Interest Specific"
        Private Function GetAssignedAdditionalInterestGeneralRemarks() As String
            Dim RemarksText As String = String.Empty
            Dim AddAAI As List(Of AssignedAiRemark) = New List(Of AssignedAiRemark)
            Dim DelAAI As List(Of AssignedAiRemark) = New List(Of AssignedAiRemark)

            ProcessAssignedAdditionalInterestGeneralList(AddAAI, DelAAI)

            If DelAAI.Count > 0 Then
                For Each AAIremark In DelAAI
                    RemarksText &= String.Format("DELETED {0} AS AN ADDITIONAL INTEREST FOR {1} AT {2} ON THIS POLICY.", AAIremark.LienHolderName, AAIremark.Description, AAIremark.Location)
                    RemarksText &= " "
                Next

            End If

            If AddAAI.Count > 0 Then
                For Each AAIremark In AddAAI
                    RemarksText &= String.Format("ADDED {0} AS AN ADDITIONAL INTEREST FOR {1} AT {2} ON THIS POLICY.", AAIremark.LienHolderName, AAIremark.Description, AAIremark.Location)
                    RemarksText &= " "
                Next
            End If

            Return RemarksText
        End Function

        Private Sub ProcessAssignedAdditionalInterestGeneralList(ByRef addList As List(Of AssignedAiRemark), ByRef delList As List(Of AssignedAiRemark))
            If ComDictItems IsNot Nothing AndAlso ComDictItems.AssignedAdditionalInterests IsNot Nothing AndAlso ComDictItems.AssignedAdditionalInterests.Count > 0 Then
                For Each appliedAI As DevDictionaryHelper.AssignedAI In ComDictItems.AssignedAdditionalInterests
                    If ComDictItems.Locations IsNot Nothing AndAlso ComDictItems.Locations.Count > 0 Then
                        If ComDictItems.Locations.FindAll(Function(x) x.LocationNumber = appliedAI.LocationNumber).Count > 0 Then
                            'Ignore
                            Continue For
                        End If
                    End If
                    If ComDictItems.AdditionalInterests IsNot Nothing AndAlso ComDictItems.AdditionalInterests.Count > 0 Then
                        If ComDictItems.AdditionalInterests.FindAll(Function(x) x.ListId = appliedAI.ListId).Count > 0 Then
                            'Ignore
                            Continue For
                        End If
                    End If

                    Dim LocNumber = appliedAI.LocationNumber.TryToGetInt32 + 1
                    Dim BuildNumber = appliedAI.BuildingNumber.TryToGetInt32 + 1
                    Dim LienHolderAI = Quote.AdditionalInterests.Find(Function(x) x.ListId = appliedAI.ListId)
                    Dim lienholder As String = LienHolderAI?.Name?.CommercialName1
                    If LienHolderAI?.Name?.TypeId <> "2" Then
                        lienholder = LienHolderAI?.Name?.DisplayName
                    End If
                    If String.IsNullOrWhiteSpace(lienholder) Then
                        lienholder = "LIENHOLDER"
                    End If
                    lienholder = lienholder.ToUpper

                    If appliedAI.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.addItem Then
                        addList.Add(New AssignedAiRemark(lienholder, appliedAI.Description, "LOCATION " & LocNumber.ToString & " / BUILDING " & BuildNumber.ToString))
                    ElseIf appliedAI.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.deleteItem Then
                        delList.Add(New AssignedAiRemark(lienholder, appliedAI.Description, "LOCATION " & LocNumber.ToString & " / BUILDING " & BuildNumber.ToString))
                    End If

                Next
            End If
        End Sub
#End Region

#Region "Commercial Additional Interest Specific"
        Private Function GetAdditionalInterestGeneralRemarks() As String
            Dim RemarksText As String = String.Empty
            Dim AddAdditionalInterestLienholders As List(Of String) = New List(Of String)
            Dim DelAdditionalInterestLienholders As List(Of String) = New List(Of String)

            ProcessAdditionalInterestGeneralList(AddAdditionalInterestLienholders, DelAdditionalInterestLienholders)

            If DelAdditionalInterestLienholders.Count > 0 Then
                RemarksText = String.Format("DELETED {0} AS AN ADDITIONAL INTEREST ON THIS POLICY.",
                                             String.Join(", ", DelAdditionalInterestLienholders))
            End If

            If DelAdditionalInterestLienholders.Count > 0 AndAlso AddAdditionalInterestLienholders.Count > 0 Then
                RemarksText &= " "
            End If

            If AddAdditionalInterestLienholders.Count > 0 Then
                RemarksText &= String.Format("ADDED {0} AS AN ADDITIONAL INTEREST PER REVISED DECLARATIONS ATTACHED.",
                                             String.Join(", ", AddAdditionalInterestLienholders))
            End If

            Return RemarksText & " "
        End Function

        Private Sub ProcessAdditionalInterestGeneralList(ByRef addList As List(Of String), ByRef delList As List(Of String))
            Dim GoToNextAi As Boolean
            If ComDictItems IsNot Nothing AndAlso ComDictItems.AdditionalInterests IsNot Nothing AndAlso ComDictItems.AdditionalInterests.Count > 0 Then
                For Each AI As DevDictionaryHelper.AddInterest In ComDictItems.AdditionalInterests
                    If ComDictItems.AssignedAdditionalInterests IsNot Nothing AndAlso ComDictItems.AssignedAdditionalInterests.Count > 0 Then
                        Dim FoundAssignedAIList = ComDictItems.AssignedAdditionalInterests.FindAll(Function(x) x.ListId = AI.ListId)
                        If FoundAssignedAIList.Count > 0 Then
                            If ComDictItems.Locations IsNot Nothing AndAlso ComDictItems.Locations.Count > 0 Then
                                For Each FoundAssignedAI In FoundAssignedAIList
                                    If ComDictItems.Locations.FindAll(Function(x) x.LocationNumber = FoundAssignedAI.LocationNumber).Count > 0 Then
                                        'Ignore
                                        GoToNextAi = True
                                        Exit For
                                    End If
                                Next
                                'Resume with next AI
                                If GoToNextAi Then Continue For
                            End If
                        End If
                    End If
                    If AI.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.addItem Then
                        addList.Add(AI.LienholderName)
                    ElseIf AI.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.deleteItem Then
                        delList.Add(AI.LienholderName)
                    End If
                Next
            End If
        End Sub
#End Region

#Region "Commercial Inland Marine Specific"
        Private Function GetInlandMarineGeneralRemarks() As String
            Dim RemarksText As String = String.Empty
            Dim AddEquipment As List(Of String) = New List(Of String)
            Dim DelEquipment As List(Of String) = New List(Of String)
            ProcessInlandMarineGeneralList(AddEquipment, DelEquipment)

            If DelEquipment.Count > 0 Then
                RemarksText = String.Format("DELETED {0} FROM THIS POLICY. COVERAGE HAS BEEN REMOVED.",
                                             String.Join(", ", DelEquipment))
            End If

            If DelEquipment.Count > 0 AndAlso AddEquipment.Count > 0 Then
                RemarksText &= " "
            End If

            If AddEquipment.Count > 0 Then
                RemarksText &= String.Format("ADDED {0} TO THE CONTRACTORS EQUIPMENT SCHEDULE PER REVISED DECLARATIONS ATTACHED.",
                                             String.Join(", ", AddEquipment))
            End If

            Return (RemarksText & " ").ToUpper
        End Function

        Private Sub ProcessInlandMarineGeneralList(ByRef addList As List(Of String), ByRef delList As List(Of String))
            Dim AiList As New List(Of String)

            For Each AI In ComDictItems.AdditionalInterests
                AiList.Add(AI.ListId.Trim)
            Next

            If ComDictItems IsNot Nothing AndAlso ComDictItems.IMContractorsEquip IsNot Nothing AndAlso ComDictItems.IMContractorsEquip.Count > 0 Then
                For Each Item As DevDictionaryHelper.InlandMarineCoverageWithPremium In ComDictItems.IMContractorsEquip

                    If AiList.Contains(Item.AIListId) Then
                        'Is covered by AI, so ignore for remarks
                        Continue For
                    End If

                    'Add to List for remark generation
                    If Item.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.addItem Then
                        addList.Add(Item.Description)
                    ElseIf Item.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.deleteItem Then
                        delList.Add(Item.Description)
                    Else
                        delList.Add(Item.Description)
                    End If
                Next
            End If
        End Sub
#End Region
#End Region



    End Class

    Class AssignedAiRemark
        Property LienHolderName As String
        Property Description As String
        Property Location As String

        Sub New()

        End Sub

        Sub New(LienHolderName, Description, Location)
            Me.LienHolderName = LienHolderName
            Me.Description = Description
            Me.Location = Location
        End Sub
    End Class

End Namespace