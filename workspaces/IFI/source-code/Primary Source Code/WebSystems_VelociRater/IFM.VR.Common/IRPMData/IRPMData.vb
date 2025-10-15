Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.IRPMData

    Public Class IRPMData
        Private Shared qqConn As String = System.Configuration.ConfigurationManager.AppSettings("ConnQQ") ' "Server=ifmdiasqlQA;UID=ifmdsn;PWD=ifmdsn;Initial Catalog=QuickQuote;Max Pool Size=400;"

        Private Sub New()

        End Sub


        Public Shared Function GetIRPMData(list As List(Of QuickQuote.CommonObjects.QuickQuoteScheduledRating), LineOfBusiness As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType) As List(Of QuickQuote.CommonObjects.QuickQuoteScheduledRating)
            If list Is Nothing Then Return Nothing
            Select Case LineOfBusiness
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    Dim index = 0
                    Dim bopList = GetBOPValues()

                    For Each Item As QuickQuote.CommonObjects.QuickQuoteScheduledRating In list

                        'Item.Maximum = bopList(index).Maximum

                        Item.Maximum = (From ListItem In bopList
                                        Where Item.RiskCharacteristicTypeId = ListItem.RiskCharacteristicTypeId And
                                               Item.ScheduleRatingTypeId = ListItem.ScheduleRatingTypeId
                                        Select ListItem.Maximum).First()

                        'Item.Minimum = bopList(index).Minimum

                        Item.Minimum = (From ListItem In bopList
                                        Where Item.RiskCharacteristicTypeId = ListItem.RiskCharacteristicTypeId And
                                               Item.ScheduleRatingTypeId = ListItem.ScheduleRatingTypeId
                                        Select ListItem.Minimum).First()


                        'Item.Description = bopList(index).Description

                        Item.Description = (From ListItem In bopList
                                            Where Item.RiskCharacteristicTypeId = ListItem.RiskCharacteristicTypeId And
                                               Item.ScheduleRatingTypeId = ListItem.ScheduleRatingTypeId
                                            Select ListItem.Description).First()

                        index = index + 1
                    Next


                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                    Dim index = 0
                    Dim updateList = GetCAPValues()

                    ' Cap has a list with duplicated values: ScheduleRatingTypeID 1 and 2. We are removing the Type 2's 
                    '   because the questions are all same and IRPM results would be written to the same objects anyway. 
                    If IFM.VR.Common.Helpers.CAP.CAPScheduledCredit.CAPScheduledCreditEnabled = False Then
                        list.RemoveAll(Function(c) c.ScheduleRatingTypeId <> "1")
                    End If

                    For Each Item As QuickQuote.CommonObjects.QuickQuoteScheduledRating In list

                        Item.Maximum = (From ListItem In updateList
                                        Where Item.RiskCharacteristicTypeId = ListItem.RiskCharacteristicTypeId And
                                               Item.ScheduleRatingTypeId = ListItem.ScheduleRatingTypeId
                                        Select ListItem.Maximum).First()

                        Item.Minimum = (From ListItem In updateList
                                        Where Item.RiskCharacteristicTypeId = ListItem.RiskCharacteristicTypeId And
                                               Item.ScheduleRatingTypeId = ListItem.ScheduleRatingTypeId
                                        Select ListItem.Minimum).First()

                        Item.Description = (From ListItem In updateList
                                            Where Item.RiskCharacteristicTypeId = ListItem.RiskCharacteristicTypeId And
                                               Item.ScheduleRatingTypeId = ListItem.ScheduleRatingTypeId
                                            Select ListItem.Description).First()

                        index = index + 1
                    Next


                    Exit Select
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                    Dim index = 0
                    Dim wcpList = GetWCPValues()

                    ' Cap has a list with duplicated values: ScheduleRatingTypeID 1 and 2. We are removing the Type 2's 
                    '   because the questions are all same and IRPM results would be written to the same objects anyway. 
                    ' list.RemoveAll(Function(c) c.ScheduleRatingTypeId <> "1")

                    For Each Item As QuickQuote.CommonObjects.QuickQuoteScheduledRating In list

                        Item.Maximum = (From ListItem In wcpList
                                        Where Item.RiskCharacteristicTypeId = ListItem.RiskCharacteristicTypeId And
                                               Item.ScheduleRatingTypeId = ListItem.ScheduleRatingTypeId
                                        Select ListItem.Maximum).First()

                        Item.Minimum = (From ListItem In wcpList
                                        Where Item.RiskCharacteristicTypeId = ListItem.RiskCharacteristicTypeId And
                                               Item.ScheduleRatingTypeId = ListItem.ScheduleRatingTypeId
                                        Select ListItem.Minimum).First()

                        Item.Description = (From ListItem In wcpList
                                            Where Item.RiskCharacteristicTypeId = ListItem.RiskCharacteristicTypeId And
                                               Item.ScheduleRatingTypeId = ListItem.ScheduleRatingTypeId
                                            Select ListItem.Description).First()

                        index = index + 1
                    Next

                    Exit Select

                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                    Dim index = 0
                    Dim cglList = GetCGLValues()

                    ' Cap has a list with duplicated values: ScheduleRatingTypeID 1 and 2. We are removing the Type 2's 
                    '   because the questions are all same and IRPM results would be written to the same objects anyway. 
                    list.RemoveAll(Function(c) c.ScheduleRatingTypeId <> "6")

                    For Each Item As QuickQuote.CommonObjects.QuickQuoteScheduledRating In list

                        Item.Maximum = (From ListItem In cglList
                                        Where Item.RiskCharacteristicTypeId = ListItem.RiskCharacteristicTypeId And
                                               Item.ScheduleRatingTypeId = ListItem.ScheduleRatingTypeId
                                        Select ListItem.Maximum).First()

                        Item.Minimum = (From ListItem In cglList
                                        Where Item.RiskCharacteristicTypeId = ListItem.RiskCharacteristicTypeId And
                                               Item.ScheduleRatingTypeId = ListItem.ScheduleRatingTypeId
                                        Select ListItem.Minimum).First()

                        Item.Description = (From ListItem In cglList
                                            Where Item.RiskCharacteristicTypeId = ListItem.RiskCharacteristicTypeId And
                                               Item.ScheduleRatingTypeId = ListItem.ScheduleRatingTypeId
                                            Select ListItem.Description).First()

                        index = index + 1
                    Next


                    Exit Select

                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                    Dim index = 0
                    Dim cprList = GetCPRValues()

                    ' Cap has a list with duplicated values: ScheduleRatingTypeID 1 and 2. We are removing the Type 2's 
                    '   because the questions are all same and IRPM results would be written to the same objects anyway. 
                    ' list.RemoveAll(Function(c) c.ScheduleRatingTypeId <> "6")

                    For Each Item As QuickQuote.CommonObjects.QuickQuoteScheduledRating In list

                        Item.Maximum = (From ListItem In cprList
                                        Where Item.RiskCharacteristicTypeId = ListItem.RiskCharacteristicTypeId And
                                               Item.ScheduleRatingTypeId = ListItem.ScheduleRatingTypeId
                                        Select ListItem.Maximum).First()

                        Item.Minimum = (From ListItem In cprList
                                        Where Item.RiskCharacteristicTypeId = ListItem.RiskCharacteristicTypeId And
                                               Item.ScheduleRatingTypeId = ListItem.ScheduleRatingTypeId
                                        Select ListItem.Minimum).First()

                        Item.Description = (From ListItem In cprList
                                            Where Item.RiskCharacteristicTypeId = ListItem.RiskCharacteristicTypeId And
                                               Item.ScheduleRatingTypeId = ListItem.ScheduleRatingTypeId
                                            Select ListItem.Description).First()

                        index = index + 1
                    Next


                    Exit Select
                Case Else

                    Exit Select
            End Select

            Return list
        End Function

        Public Shared Function GetIRPMTotalMax(LineOfBusiness As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType) As String
            'Todo - Why is this a string? 
            If String.IsNullOrEmpty(LineOfBusiness.ToString) = False Then
                Select Case LineOfBusiness
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                        Dim bopList = GetBOPValues()
                        Return bopList(0).TotalMax
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                        Dim capList = GetCAPValues()
                        Return capList(0).TotalMax
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                        Dim wcpList = GetWCPValues()
                        Return wcpList(0).TotalMax
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                        Dim cglList = GetCGLValues()
                        Return cglList(0).TotalMax
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                        Dim cprList = GetCPRValues()
                        Return cprList(0).TotalMax
                        Exit Select
                    Case Else
                        Return 0
                        Exit Select
                End Select
            Else
                Return 0
            End If
        End Function

       Public Shared Function GetIRPMTotalMax(quote As QuickQuoteObject, LOB As QuickQuoteObject.QuickQuoteLobType) As String
            'Todo - Why is this a string? 
            If String.IsNullOrEmpty(LOB) = False Then
                Select Case LOB
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                        If IFM.VR.Common.Helpers.BopNewCoIRPMHelper.IsBopNewCoIRPMAvailable(quote) Then
                            Return "15"
                        Else
                            Dim bopList = GetBOPValues()
                            Return bopList(0).TotalMax
                        End If

                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                        Dim capList = GetCAPValues()
                        Return capList(0).TotalMax
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                        Dim wcpList = GetWCPValues()
                        Return wcpList(0).TotalMax
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                        If IFM.VR.Common.Helpers.NewCompanyIdHelper.IsNewCompany(quote) AndAlso quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                            Return "15"
                        Else
                            Return "25"
                        End If
                        'Dim cglList = GetCGLValues()
                        'Return cglList(0).TotalMax
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                        If IFM.VR.Common.Helpers.NewCompanyIdHelper.IsNewCompany(quote) AndAlso quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                            Return "15"
                        Else
                            Return "25"
                        End If
                        'Dim cprList = GetCPRValues()
                        'Return cprList(0).TotalMax 
                        Exit Select
                    Case Else
                        Return 0
                        Exit Select
                End Select
            Else
                Return 0
            End If
        End Function


        Public Shared Function GetIRPMTotalMin(LineOfBusiness As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType) As String
            'Todo - Why is this a string? 
            If String.IsNullOrEmpty(LineOfBusiness.ToString) = False Then
                Select Case LineOfBusiness
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                        Dim bopList = GetBOPValues()
                        Return bopList(0).TotalMin
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                        Dim capList = GetCAPValues()
                        Return capList(0).TotalMin
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                        Dim wcpList = GetWCPValues()
                        Return wcpList(0).TotalMin
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                        Dim cglList = GetCGLValues()
                        Return cglList(0).TotalMin
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                        Dim cprList = GetCPRValues()
                        Return cprList(0).TotalMin
                        Exit Select
                    Case Else
                        Return 0
                        Exit Select
                End Select
            Else
                Return 0
            End If
        End Function

        Public Shared Function GetIRPMTotalMin(quote As QuickQuoteObject, LOB As QuickQuoteObject.QuickQuoteLobType) As String
            'Todo - Why is this a string? 
            If String.IsNullOrEmpty(LOB) = False Then
                Select Case LOB
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                        If IFM.VR.Common.Helpers.BopNewCoIRPMHelper.IsBopNewCoIRPMAvailable(quote) Then
                            Return "-15"
                        Else
                            Dim bopList = GetBOPValues()
                            Return bopList(0).TotalMin
                        End If

                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                        Dim capList = GetCAPValues()
                        Return capList(0).TotalMin
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                        Dim wcpList = GetWCPValues()
                        Return wcpList(0).TotalMin
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                        If IFM.VR.Common.Helpers.NewCompanyIdHelper.IsNewCompany(quote) AndAlso quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                            Return "-15"
                        Else
                            Return "-25"
                        End If
                        'Dim cglList = GetCGLValues()
                        'Return cglList(0).TotalMin
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                        If IFM.VR.Common.Helpers.NewCompanyIdHelper.IsNewCompany(quote) AndAlso quote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                            Return "-15"
                        Else
                            Return "-25"
                        End If
                        'Dim cprList = GetCPRValues()
                        'Return cprList(0).TotalMin
                        Exit Select
                    Case Else
                        Return 0
                        Exit Select
                End Select
            Else
                Return 0
            End If
        End Function


        Public Shared Function GetBOPValues() As List(Of IRPMItem)
            Dim bopList As New List(Of IRPMItem)

            bopList.Add(New IRPMItem() With {
                .ItemIndex = 0,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Management/Cooperation",
                .RiskCharacteristicTypeId = "14",
                .ScheduleRatingTypeId = "4"
            })

            bopList.Add(New IRPMItem() With {
                .ItemIndex = 1,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Location",
                .RiskCharacteristicTypeId = "1",
                .ScheduleRatingTypeId = "4"
            })

            bopList.Add(New IRPMItem() With {
                .ItemIndex = 2,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Building Features",
                .RiskCharacteristicTypeId = "9",
                .ScheduleRatingTypeId = "4"
            })

            bopList.Add(New IRPMItem() With {
                .ItemIndex = 3,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Premises",
                .RiskCharacteristicTypeId = "2",
                .ScheduleRatingTypeId = "4"
            })

            bopList.Add(New IRPMItem() With {
                .ItemIndex = 4,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Employees",
                .RiskCharacteristicTypeId = "4",
                .ScheduleRatingTypeId = "4"
            })

            bopList.Add(New IRPMItem() With {
                .ItemIndex = 5,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Protection",
                .RiskCharacteristicTypeId = "12",
                .ScheduleRatingTypeId = "4"
            })

            bopList.Add(New IRPMItem() With {
                .ItemIndex = 6,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Catastrophic Hazards",
                .RiskCharacteristicTypeId = "15",
                .ScheduleRatingTypeId = "4"
            })

            bopList.Add(New IRPMItem() With {
                .ItemIndex = 7,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Management Experience",
                .RiskCharacteristicTypeId = "16",
                .ScheduleRatingTypeId = "4"
            })

            ' Allow Staff to have a higher total IRPM total than Agents.  MGB 11/20/17
            'If Helpers.QuickQuoteObjectHelper.IsStaff Then
            '    For Each i As IRPMItem In bopList
            '        'i.Minimum = -10
            '        'i.Maximum = 10
            '        i.TotalMin = -80
            '        i.TotalMax = 80
            '    Next
            'End If

            Return bopList
        End Function

        Public Shared Function GetCAPValues() As List(Of IRPMItem)
            Dim updateList As New List(Of IRPMItem)

            ' ScheduleRatingTypeID - Liability = 1, Physical Damage = 2
            updateList.Add(New IRPMItem() With {
                .ItemIndex = 0,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Management",
                .RiskCharacteristicTypeId = "5",
                .ScheduleRatingTypeId = "1"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 1,
                .Minimum = -15,
                .Maximum = 15,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Employees",
                .RiskCharacteristicTypeId = "4",
                .ScheduleRatingTypeId = "1"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 2,
                .Minimum = -15,
                .Maximum = 15,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Equipment",
                .RiskCharacteristicTypeId = "3",
                .ScheduleRatingTypeId = "1"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 3,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Safety Organization",
                .RiskCharacteristicTypeId = "13",
                .ScheduleRatingTypeId = "1"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 0,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Management",
                .RiskCharacteristicTypeId = "5",
                .ScheduleRatingTypeId = "2"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 1,
                .Minimum = -15,
                .Maximum = 15,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Employees",
                .RiskCharacteristicTypeId = "4",
                .ScheduleRatingTypeId = "2"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 2,
                .Minimum = -15,
                .Maximum = 15,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Equipment",
                .RiskCharacteristicTypeId = "3",
                .ScheduleRatingTypeId = "2"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 3,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Safety Organization",
                .RiskCharacteristicTypeId = "13",
                .ScheduleRatingTypeId = "2"
            })


            Return updateList
        End Function

        Public Shared Function GetWCPValues() As List(Of IRPMItem)
            Dim wcpList As New List(Of IRPMItem)

            wcpList.Add(New IRPMItem() With {
                .ItemIndex = 0,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -10,
                .TotalMax = 10,
                .Description = "Management/Cooperation",
                .RiskCharacteristicTypeId = "14",
                .ScheduleRatingTypeId = "4"
            })

            wcpList.Add(New IRPMItem() With {
                .ItemIndex = 1,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -10,
                .TotalMax = 10,
                .Description = "Premises",
                .RiskCharacteristicTypeId = "2",
                .ScheduleRatingTypeId = "4"
            })

            wcpList.Add(New IRPMItem() With {
                .ItemIndex = 2,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -10,
                .TotalMax = 10,
                .Description = "Equipment",
                .RiskCharacteristicTypeId = "3",
                .ScheduleRatingTypeId = "4"
            })

            wcpList.Add(New IRPMItem() With {
                .ItemIndex = 3,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -10,
                .TotalMax = 10,
                .Description = "Employees",
                .RiskCharacteristicTypeId = "4",
                .ScheduleRatingTypeId = "4"
            })

            wcpList.Add(New IRPMItem() With {
                .ItemIndex = 3,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -10,
                .TotalMax = 10,
                .Description = "Medical Facilities",
                .RiskCharacteristicTypeId = "19",
                .ScheduleRatingTypeId = "4"
            })

            wcpList.Add(New IRPMItem() With {
                .ItemIndex = 3,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -10,
                .TotalMax = 10,
                .Description = "Classificiation Peculiarities",
                .RiskCharacteristicTypeId = "17",
                .ScheduleRatingTypeId = "4"
            })


            Return wcpList
        End Function

        Public Shared Function GetCGLValues() As List(Of IRPMItem)
            Dim updateList As New List(Of IRPMItem)

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 0,
                .Minimum = -15,
                .Maximum = 15,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Management/Cooperation",
                .RiskCharacteristicTypeId = "14",
                .ScheduleRatingTypeId = "6"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 1,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Location",
                .RiskCharacteristicTypeId = "1",
                .ScheduleRatingTypeId = "6"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 2,
                .Minimum = -15,
                .Maximum = 15,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Premises",
                .RiskCharacteristicTypeId = "2",
                .ScheduleRatingTypeId = "6"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 3,
                .Minimum = -15,
                .Maximum = 15,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Equipment",
                .RiskCharacteristicTypeId = "3",
                .ScheduleRatingTypeId = "6"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 4,
                .Minimum = -15,
                .Maximum = 15,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Employees",
                .RiskCharacteristicTypeId = "4",
                .ScheduleRatingTypeId = "6"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 5,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Classificiation Peculiarities",
                .RiskCharacteristicTypeId = "17",
                .ScheduleRatingTypeId = "6"
            })


            Return updateList
        End Function

        Public Shared Function GetCPRValues() As List(Of IRPMItem)
            Dim updateList As New List(Of IRPMItem)

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 0,
                .Minimum = -18,
                .Maximum = 18,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Management",
                .RiskCharacteristicTypeId = "5",
                .ScheduleRatingTypeId = "4"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 1,
                .Minimum = -18,
                .Maximum = 18,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Location",
                .RiskCharacteristicTypeId = "1",
                .ScheduleRatingTypeId = "4"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 2,
                .Minimum = -20,
                .Maximum = 20,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Building Features",
                .RiskCharacteristicTypeId = "9",
                .ScheduleRatingTypeId = "4"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 3,
                .Minimum = -20,
                .Maximum = 20,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Premises and Equipment",
                .RiskCharacteristicTypeId = "24",
                .ScheduleRatingTypeId = "4"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 4,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Employees",
                .RiskCharacteristicTypeId = "4",
                .ScheduleRatingTypeId = "4"
            })

            updateList.Add(New IRPMItem() With {
                .ItemIndex = 5,
                .Minimum = -10,
                .Maximum = 10,
                .TotalMin = -25,
                .TotalMax = 25,
                .Description = "Protection",
                .RiskCharacteristicTypeId = "12",
                .ScheduleRatingTypeId = "4"
            })


            Return updateList
        End Function

        Public Class IRPMItem
            Inherits QuickQuote.CommonObjects.QuickQuoteScheduledRating
            Public Property ItemIndex As Short
            Public Property TotalMax As String
            Public Property TotalMin As String

        End Class

    End Class

End Namespace