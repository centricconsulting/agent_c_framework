Imports QuickQuote.CommonObjects
Imports IFM.VR.Common.Helpers.MultiState
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers
    Public Class AdditionalInterest
        Public Id As Int32
        Public DisplayName As String
        Public CommercialName As String
        Public FirstName As String
        Public MiddleName As String
        Public LastName As String
        Public PhoneNumber As String
        Public PhoneExtension As String
        Public PhoneTypeId As Int32
        Public DisplayAddress As String
        Public Address_PoBox As String
        Public Address_AptNum As String
        Public Address_HouseNum As String
        Public Address_StreetName As String
        Public Address_City As String
        Public Address_Zip As String
        Public Address_State As String
        Public Address_StateId As Int32
        Public AgencyId As Int32
        Public SingleEntry As Boolean
        Public NameTypeId As Int32
        Public IsEditable As Boolean


        Public Shared Function GetAdditionalInterestByName(commericialName As String, firstname As String, middlename As String, lastname As String, agency_id As Int32) As List(Of AdditionalInterest)
            Dim results As New List(Of AdditionalInterest)
            Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("ConnQQ"))
                conn.Open()
                Using cmd As New System.Data.SqlClient.SqlCommand()
                    cmd.Connection = conn
                    cmd.Parameters.AddWithValue("@commercialname", commericialName)

                    cmd.Parameters.AddWithValue("@firstname", firstname)
                    cmd.Parameters.AddWithValue("@middlename", middlename)
                    cmd.Parameters.AddWithValue("@lastname", lastname)

                    cmd.Parameters.AddWithValue("@agencyId", agency_id)
                    cmd.CommandText = "usp_AdditionalInterest_Search"
                    cmd.CommandType = CommandType.StoredProcedure

                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            While reader.Read()
                                Dim additional As New AdditionalInterest()
                                additional.Id = reader.GetInt32(0)
                                additional.DisplayName = If(reader.IsDBNull(1) = False, reader.GetString(1).ToUpper(), "")
                                additional.FirstName = If(reader.IsDBNull(2) = False, reader.GetString(2).ToUpper(), "")
                                additional.MiddleName = If(reader.IsDBNull(3) = False, reader.GetString(3).ToUpper(), "")
                                additional.LastName = If(reader.IsDBNull(4) = False, reader.GetString(4).ToUpper(), "")
                                additional.CommercialName = If(reader.IsDBNull(5) = False, reader.GetString(5).ToUpper(), "")
                                additional.PhoneNumber = If(reader.IsDBNull(6) = False, reader.GetString(6).ToUpper(), "")
                                additional.PhoneExtension = If(reader.IsDBNull(7) = False, reader.GetInt32(7).ToString(), "")
                                additional.PhoneTypeId = If(reader.IsDBNull(8) = False, reader.GetInt32(8), 0)
                                additional.DisplayAddress = If(reader.IsDBNull(9) = False, reader.GetString(9).ToUpper(), "")
                                additional.Address_PoBox = If(reader.IsDBNull(10) = False, reader.GetString(10).ToUpper(), "")
                                additional.Address_AptNum = If(reader.IsDBNull(11) = False, reader.GetString(11).ToUpper(), "")
                                additional.Address_HouseNum = If(reader.IsDBNull(12) = False, reader.GetString(12).ToUpper(), "")
                                additional.Address_StreetName = If(reader.IsDBNull(13) = False, reader.GetString(13).ToUpper(), "")
                                additional.Address_City = If(reader.IsDBNull(14) = False, reader.GetString(14).ToUpper(), "")
                                additional.Address_Zip = If(reader.IsDBNull(15) = False, reader.GetString(15).ToUpper(), "")
                                additional.Address_StateId = If(reader.IsDBNull(16) = False, reader.GetInt32(16), 0)
                                additional.Address_State = If(reader.IsDBNull(17) = False, reader.GetString(17).ToUpper(), "")
                                additional.AgencyId = reader.GetInt32(18)
                                additional.SingleEntry = reader.GetBoolean(19)
                                additional.NameTypeId = If(reader.IsDBNull(20) = False, reader.GetInt32(20), 0)

                                If String.IsNullOrWhiteSpace(additional.DisplayName) Then
                                    If String.IsNullOrWhiteSpace(additional.CommercialName) = False Then
                                        additional.DisplayName = additional.CommercialName
                                    Else
                                        additional.DisplayName = additional.FirstName + " " + additional.LastName
                                    End If
                                End If

                                results.Add(additional)
                            End While
                        End If
                    End Using
                End Using
            End Using
            Return results
        End Function

        Public Shared Function HasIncompleteAi(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                Select Case topQuote.LobType
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal ' PPA
                        If topQuote.Vehicles IsNot Nothing Then
                            For Each v In topQuote.Vehicles
                                If v.AdditionalInterests IsNot Nothing Then
                                    For Each ai In v.AdditionalInterests
                                        If AiIsComplete(ai) = False Then
                                            Return True
                                        End If

                                    Next
                                End If
                            Next

                        End If
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal 'HOM
                        Dim AdditionalInterests = topQuote?.Locations(0)?.AdditionalInterests
                        If AdditionalInterests IsNot Nothing Then
                            For Each ai As QuickQuoteAdditionalInterest In AdditionalInterests
                                If AiIsComplete(ai) = False Then
                                    Return True
                                End If
                            Next
                        End If
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto  ' BOP, CAP
                        If topQuote.AdditionalInterests IsNot Nothing Then
                            For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In topQuote.AdditionalInterests
                                If Not AiIsComplete(ai) Then Return True
                            Next
                        End If
                        Exit Select
                End Select
            End If
            Return False
        End Function

        Public Shared Function AiIsComplete(ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest) As Boolean

            If ai IsNot Nothing Then
                ' MGB 8/13/19 AI Type must be validated!!
                If ai.TypeId Is Nothing OrElse ai.TypeId.Trim = String.Empty OrElse (Not IsNumeric(ai.TypeId)) Then Return False

                If ai.Name Is Nothing Then
                    Return False
                End If
                If ai.Address Is Nothing Then
                    Return False
                End If

                If String.IsNullOrWhiteSpace(ai.Name.CommercialName1) And String.IsNullOrWhiteSpace(ai.Name.FirstName) And String.IsNullOrWhiteSpace(ai.Name.LastName) Then
                    Return False

                Else

                    If String.IsNullOrWhiteSpace(ai.Name.CommercialName1) = False And (String.IsNullOrWhiteSpace(ai.Name.FirstName) = False Or String.IsNullOrWhiteSpace(ai.Name.LastName) = False) Then
                        Return False
                    Else
                        If String.IsNullOrWhiteSpace(ai.Name.CommercialName1) Then
                            ' must be first, middle, last
                            If String.IsNullOrWhiteSpace(ai.Name.FirstName) Or String.IsNullOrWhiteSpace(ai.Name.LastName) Then
                                Return False
                            End If

                        End If
                    End If

                End If

                If ai.Phones IsNot Nothing Then
                    If ai.Phones.Any() Then
                        If String.IsNullOrWhiteSpace(ai.Phones(0).Extension) = False AndAlso IFM.Common.InputValidation.CommonValidations.IsValidPhone(ai.Phones(0).Number) = False Then
                            Return False
                        End If
                        If String.IsNullOrWhiteSpace(ai.Phones(0).Extension) = False Then
                            If Integer.TryParse(ai.Phones(0).Extension, Nothing) = False Then
                                Return False
                            End If

                        End If
                    End If
                End If

                If ai.Address IsNot Nothing Then
                    If (String.IsNullOrWhiteSpace(ai.Address.HouseNum) Or String.IsNullOrWhiteSpace(ai.Address.StreetName)) And String.IsNullOrWhiteSpace(ai.Address.POBox) Then
                        If String.IsNullOrWhiteSpace(ai.Address.HouseNum) And String.IsNullOrWhiteSpace(ai.Address.StreetName) And String.IsNullOrWhiteSpace(ai.Address.POBox) Then
                            ' all missing
                            Return False
                        Else
                            ' test street num and street name
                            If (String.IsNullOrWhiteSpace(ai.Address.HouseNum)) And String.IsNullOrWhiteSpace(ai.Address.POBox) Then
                                Return False
                            Else
                                If (String.IsNullOrWhiteSpace(ai.Address.StreetName)) And String.IsNullOrWhiteSpace(ai.Address.POBox) Then
                                    Return False
                                End If
                            End If
                        End If
                    Else
                        If (String.IsNullOrWhiteSpace(ai.Address.HouseNum) = False Or String.IsNullOrWhiteSpace(ai.Address.StreetName) = False) And String.IsNullOrWhiteSpace(ai.Address.POBox) = False Then
                            'all are set
                            Return False
                        End If
                    End If

                    If String.IsNullOrWhiteSpace(ai.Address.Zip) Or ai.Address.Zip = "00000-0000" Or ai.Address.Zip = "00000" Then
                        Return False
                    Else
                        If IFM.Common.InputValidation.CommonValidations.IsValidZipCode(ai.Address.Zip) = False Then
                            Return False
                        End If
                    End If

                    If String.IsNullOrWhiteSpace(ai.Address.City) Then
                        Return False
                    End If

                    If String.IsNullOrWhiteSpace(ai.Address.State) Then
                        Return False
                    End If

                Else
                    Return False
                End If

                Return True
            End If
            Return False
        End Function

        Public Shared Sub RemoveIncompleteAis_Auto(topQuote As QuickQuote.CommonObjects.QuickQuoteObject)
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If topQuote.Vehicles IsNot Nothing Then
                    For Each v As QuickQuote.CommonObjects.QuickQuoteVehicle In topQuote.Vehicles
                        Dim removeList As New List(Of Int32)
                        Dim index As Int32 = 0
                        If v.AdditionalInterests IsNot Nothing Then
                            For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In v.AdditionalInterests
                                If IFM.VR.Common.Helpers.AdditionalInterest.AiIsComplete(ai) = False Then
                                    removeList.Add(index)
                                End If
                                index += 1
                            Next
                            removeList.Reverse() ' must do this to remove the proper indexes
                            For Each i In removeList
                                v.AdditionalInterests.RemoveAt(i)
                            Next
                        End If
                    Next
                End If
            End If
        End Sub

        Public Shared Sub RemoveIncompleteAIs_BOP_CAP(topQuote As QuickQuote.CommonObjects.QuickQuoteObject)
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If topQuote.AdditionalInterests IsNot Nothing Then
                    Dim removeList As New List(Of Int32)
                    Dim index As Int32 = 0
                    For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In topQuote.AdditionalInterests
                        If IFM.VR.Common.Helpers.AdditionalInterest.AiIsComplete(ai) = False Then
                            removeList.Add(index)
                        End If
                        index += 1
                    Next
                    removeList.Reverse() ' must do this to remove the proper indexes
                    For Each i In removeList
                        topQuote.AdditionalInterests.RemoveAt(i)
                    Next
                End If
            End If
        End Sub

        Public Shared Sub RemoveIncompleteAis_HOM(topQuote As QuickQuote.CommonObjects.QuickQuoteObject)
            Dim removeList As New List(Of Int32)
            Dim AdditionalInterests = topQuote?.Locations(0)?.AdditionalInterests
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If AdditionalInterests IsNot Nothing Then
                    For Each ai As QuickQuoteAdditionalInterest In AdditionalInterests
                        Dim index As Int32 = 0
                        If IFM.VR.Common.Helpers.AdditionalInterest.AiIsComplete(ai) = False Then
                            removeList.Add(index)
                        End If
                        index += 1
                        removeList.Reverse() ' must do this to remove the proper indexes

                    Next
                    For Each i In removeList
                        AdditionalInterests.RemoveAt(i)
                    Next
                End If
            End If
        End Sub

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>
        '''     Insure proper fake additional interests are present if necessary. Does not Save.
        ''' </summary>
        '''
        ''' <remarks>   Chhaw, 12/30/2019. </remarks>
        '''
        ''' <param name="Quote">    [in,out] The quote. </param>
        '''
        ''' <returns>   True if a change was made, false if not. For saving purposes. </returns>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Shared Function InsureProperFakeAI_AppSide(ByRef Quote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            Dim NeedsSaved As Boolean
            Dim AIRemoved As Boolean
            Select Case Quote?.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    If Quote?.Vehicles IsNot Nothing Then
                        For Each vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle In Quote.Vehicles
                            AIRemoved = False
                            If RemoveFakeAI_PPA(vehicle) Then
                                NeedsSaved = True
                                AIRemoved = True
                            End If
                            ' If I removed a FakeAI, I need to check to see if I need to add it back.
                            If AIRemoved AndAlso AddFakeAI_PPA(vehicle) Then
                                NeedsSaved = True
                            End If
                        Next
                    End If
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                    If Quote IsNot Nothing Then
                        If RemoveFakeAI_HOM(Quote) Then
                            NeedsSaved = True
                            AIRemoved = True
                        End If
                        ' If I removed a FakeAI, I need to check to see if I need to add it back.
                        If AIRemoved AndAlso AddFakeAI_HOM(Quote) Then
                            NeedsSaved = True
                        End If
                    End If
                    Exit Select
            End Select

            Return NeedsSaved
        End Function

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>
        '''     Removes ALL fake additional interests described on Quote by LOB. Does not Save.
        ''' </summary>
        '''
        ''' <remarks>   Chhaw, 12/30/2019. </remarks>
        '''
        ''' <param name="Quote">    [in,out] The quote. </param>
        '''
        ''' <returns>   True if a change was made, false if not. For saving purposes. </returns>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Shared Function RemoveAllFakeAIs(ByRef Quote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            Dim AIRemoved As Boolean
            Select Case Quote?.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    If Quote?.Vehicles IsNot Nothing Then
                        For Each vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle In Quote.Vehicles
                            If RemoveFakeAI_PPA(vehicle) Then
                                AIRemoved = True
                            End If
                        Next
                    End If
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                    If RemoveFakeAI_HOM(Quote) Then
                        AIRemoved = True
                    End If
                    Exit Select
            End Select
            Return AIRemoved
        End Function

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>
        '''     Adds a fake additional interest described by PPA Myvehicle if one is needed. Does not
        '''     Save.
        ''' </summary>
        '''
        ''' <remarks>   Chhaw, 12/30/2019. </remarks>
        '''
        ''' <param name="Myvehicle">    [in,out] The vehicle. </param>
        '''
        ''' <returns>   True if a change was made, false if not. </returns>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Shared Function AddFakeAI_PPA(ByRef Myvehicle As QuickQuoteVehicle) As Boolean
            ' Note that if there's already an AI on the vehicle do not create a fake AI.
            If Myvehicle?.AdditionalInterests Is Nothing OrElse Myvehicle?.AdditionalInterests?.Count = 0 Then
                Myvehicle.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                ' No AI's - create a fake one
                Dim FakeAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest()
                CreateFakeAI(FakeAI)
                Myvehicle.AdditionalInterests.Add(FakeAI)
                Return True
            End If
            Return False
        End Function

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Adds a fake additional interest described by Location(0) if one is needed. Does not
        '''     Save. </summary>
        '''
        ''' <remarks>   Chhaw, 02/24/2020. </remarks>
        '''
        ''' <param name="Quote">    [in,out] The quote. </param>
        '''
        ''' <returns>   True if a change was made, false if not. </returns>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Shared Function AddFakeAI_HOM(ByRef Quote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            Dim AdditionalInterests = Quote?.Locations(0)?.AdditionalInterests
            ' Note that if there's already an AI on the vehicle do not create a fake AI.
            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso (AdditionalInterests Is Nothing OrElse AdditionalInterests?.Count = 0) Then
                Quote.Locations(0).AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                ' No AI's - create a fake one
                Dim FakeAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest()
                CreateFakeAI(FakeAI, QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                Quote.Locations(0).AdditionalInterests.Add(FakeAI)
                Return True
            End If
            Return False
        End Function

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>
        '''     Removes the fake additional interest described by PPA Myvehicle. Does not Save.
        ''' </summary>
        '''
        ''' <remarks>   Chhaw, 12/30/2019. </remarks>
        '''
        ''' <param name="Myvehicle">    [in,out] The vehicle. </param>
        '''
        ''' <returns>   True if a change was made, false if not. </returns>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Shared Function RemoveFakeAI_PPA(ByRef Myvehicle As QuickQuoteVehicle) As Boolean
            Dim AIRemoved As Boolean
            If Myvehicle?.AdditionalInterests IsNot Nothing AndAlso Myvehicle?.AdditionalInterests?.Count > 0 Then
                Dim ndx As Integer = -1
                For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In Myvehicle.AdditionalInterests
                    ndx += 1
                    If IsFakeAI(ai) Then
                        Myvehicle.AdditionalInterests.RemoveAt(ndx)
                        AIRemoved = True
                        Exit For
                    End If
                Next
            End If
            Return AIRemoved
        End Function

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Removes the fake additional interest described by Location(0). Does not Save.. </summary>
        '''
        ''' <remarks>   Chhaw, 02/24/2020. </remarks>
        '''
        ''' <param name="Quote">    [in,out] The quote. </param>
        '''
        ''' <returns>   True if a change was made, false if not. </returns>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Shared Function RemoveFakeAI_HOM(ByRef Quote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            Dim AdditionalInterests = Quote?.Locations(0)?.AdditionalInterests
            Dim AIRemoved As Boolean
            If AdditionalInterests IsNot Nothing AndAlso AdditionalInterests.Count > 0 Then
                Dim ndx As Integer = -1
                For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In AdditionalInterests
                    ndx += 1
                    If IsFakeAI(ai) Then
                        AdditionalInterests.RemoveAt(ndx)
                        AIRemoved = True
                        Exit For
                    End If
                Next
            End If
            Return AIRemoved
        End Function


        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Query if Additional Interest is fake. </summary>
        '''
        ''' <remarks>   Chhaw, 01/02/2020. </remarks>
        '''
        ''' <param name="ai">   The additional interest. </param>
        '''
        ''' <returns>   True if fake AI, false if not. </returns>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Shared Function IsFakeAI(ByRef ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest) As Boolean
            If ai IsNot Nothing Then
                If ai.Name IsNot Nothing Then
                    If ai.Name.FirstName?.ToUpper = "FAKE" AndAlso ai.Name.LastName?.ToUpper = "AI" Then Return True
                End If
            End If
            Return False
        End Function

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Creates fake additional interest. </summary>
        '''
        ''' <remarks>   Chhaw, 01/03/2020. </remarks>
        '''
        ''' <param name="ai">   [in,out] The additional interest. </param>
        '''
        ''' <returns>   True if it succeeds, false if it fails. </returns>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Shared Sub CreateFakeAI(ByRef ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest, Optional quoteLob As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None)
            ' Create a fake AI with type of 'First Lienholder'
            Select Case quoteLob
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                    ai.TypeId = QuickQuoteAdditionalInterest.AdditionalInterestType.FirstMortgagee
                Case Else
                    ai.TypeId = QuickQuoteAdditionalInterest.AdditionalInterestType.FirstLienholder
            End Select
            ai.Name = New QuickQuoteName()
            ai.Name.BirthDate = "01/01/2000"
            ai.Name.FirstName = "Fake"
            ai.Name.LastName = "AI"
            ai.Address = New QuickQuoteAddress()
            ai.Address.HouseNum = "999"
            ai.Address.StreetName = "Fake Street"
            ai.Address.City = "FakeTown"
            ai.Address.StateId = "16"
            ai.Address.Zip = "99999"
        End Sub

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>   Gets a list of all quote vehicles that have a Fake AI by LOB</summary>
        '''
        ''' <remarks>   Chhaw, 01/06/2020. </remarks>
        '''
        ''' <param name="quote">    [in,out] The quote. </param>
        '''
        ''' <returns>   A list of quote vehicles that have a Fake AI by LOB. </returns>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Shared Function GetFakeAIItemList(ByRef quote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of Integer)
            Dim ItemsWithFakes As List(Of Integer) = New List(Of Integer)
            Select Case quote?.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    If quote?.Vehicles IsNot Nothing Then
                        For Each vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle In quote.Vehicles
                            If vehicle.AdditionalInterests IsNot Nothing Then
                                For Each ai In vehicle.AdditionalInterests
                                    If IsFakeAI(ai) Then
                                        Dim VehicleNumber As Integer
                                        If Int32.TryParse(vehicle.VehicleNum, VehicleNumber) Then
                                            ItemsWithFakes.Add(VehicleNumber)
                                            Exit For
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    End If
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                    If quote?.Locations(0) IsNot Nothing Then
                        If quote?.Locations(0).AdditionalInterests IsNot Nothing Then
                            For Each ai In quote?.Locations(0).AdditionalInterests
                                If IsFakeAI(ai) Then
                                    ItemsWithFakes.Add(ai.AgencyId)
                                End If
                            Next
                        End If
                    End If
                    Exit Select
            End Select
            Return ItemsWithFakes
        End Function

        '''////////////////////////////////////////////////////////////////////////////////////////
        ''' <summary>
        '''     Adds a fake AI if quote item (vehicle number for ex.) is contained in a passed array of
        '''     item numbers.
        ''' </summary>
        '''
        ''' <remarks>   Chhaw, 01/06/2020. </remarks>
        '''
        ''' <param name="quote">    [in,out] The quote. </param>
        ''' <param name="ItemsWithFakeAI">  String Array of items (ex. Vehicle Numbers) with Fake AI. </param>
        '''////////////////////////////////////////////////////////////////////////////////////////
        Public Shared Sub AddFakeAIByItem(ByRef quote As QuickQuote.CommonObjects.QuickQuoteObject, ItemsWithFakeAI As String())
            Select Case quote?.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    If quote?.Vehicles IsNot Nothing Then
                        For Each vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle In quote.Vehicles
                            If Array.IndexOf(ItemsWithFakeAI, vehicle.VehicleNum) <> -1 Then
                                AddFakeAI_PPA(vehicle)
                            End If
                        Next
                    End If
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                    If quote?.Locations(0) IsNot Nothing AndAlso ItemsWithFakeAI.IsNotNull AndAlso String.IsNullOrWhiteSpace(String.Join(".", ItemsWithFakeAI)) = False Then
                        AddFakeAI_HOM(quote)
                    End If
                    Exit Select
            End Select
        End Sub

    End Class
End Namespace