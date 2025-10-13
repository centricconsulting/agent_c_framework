Namespace IFM.VR.Common.QuoteSearch

    Public Class ClientSearch
        Private Shared qqConn As String = System.Configuration.ConfigurationManager.AppSettings("ConnQQ") ' "Server=ifmdiasqlQA;UID=ifmdsn;PWD=ifmdsn;Initial Catalog=QuickQuote;Max Pool Size=400;"

        Public Enum ClientSearchType
            personal = 1
            commercial = 2
        End Enum

        Public Shared Function SearchClients(searchType As ClientSearchType, ByVal agencyID As Int32, ByVal displayname As String, ByVal firstname As String, lastname As String, city As String, zipcode As String, ssn As String) As List(Of QuickQuote.CommonObjects.QuickQuoteClient)
            Dim results As New List(Of QuickQuote.CommonObjects.QuickQuoteClient)
            Try
                Using conn As New System.Data.SqlClient.SqlConnection(qqConn)
                    conn.Open()
                    Using cmd As New System.Data.SqlClient.SqlCommand()
                        cmd.CommandTimeout = 15000 ' added 11-11-14 if it is not done in 3 seconds it is too late
                        cmd.Connection = conn
                        cmd.CommandText = "[usp_QuickQuote_PersonalClientSearch_Matt]"
                        cmd.CommandType = CommandType.StoredProcedure

                        cmd.Parameters.AddWithValue("@ClientNameTypeId", CInt(searchType)) ' Matt A - 2-27-15

                        If agencyID > 0 Then
                            cmd.Parameters.AddWithValue("@AgencyID", agencyID)
                        End If

                        If String.IsNullOrWhiteSpace(displayname.Trim()) = False Then
                            cmd.Parameters.AddWithValue("@DisplayName", displayname)
                        End If

                        If searchType = ClientSearchType.personal Then
                            If String.IsNullOrWhiteSpace(firstname.Trim()) = False Then
                                cmd.Parameters.AddWithValue("@FirstName", firstname)
                            End If

                            If String.IsNullOrWhiteSpace(lastname.Trim()) = False Then
                                cmd.Parameters.AddWithValue("@LastName", lastname)
                            End If
                        Else
                            If String.IsNullOrWhiteSpace(firstname.Trim()) = False Then
                                cmd.Parameters.AddWithValue("@CommercialName1", firstname)
                            End If
                        End If

                        If String.IsNullOrWhiteSpace(city.Trim()) = False Then
                            cmd.Parameters.AddWithValue("@City", city)
                        End If

                        If String.IsNullOrWhiteSpace(zipcode.Trim()) = False Then
                            cmd.Parameters.AddWithValue("@Zip", zipcode)
                        End If

                        If String.IsNullOrWhiteSpace(ssn.Trim()) = False Then
                            cmd.Parameters.AddWithValue("@SSN", ssn)
                        End If

                        Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                            If reader.HasRows Then
                                While reader.Read()
                                    Dim result As New QuickQuote.CommonObjects.QuickQuoteClient()
                                    result.ClientId = If(reader.IsDBNull(0) = False, reader.GetInt32(0).ToString(), "")

                                    'Need to get CommercialName1, DBA, EntityTypeId

                                    result.Name = New QuickQuote.CommonObjects.QuickQuoteName()
                                    result.Name.DisplayName = If(reader.IsDBNull(2) = False, reader.GetString(2), "")
                                    result.Name.FirstName = If(reader.IsDBNull(3) = False, reader.GetString(3), "")
                                    result.Name.MiddleName = If(reader.IsDBNull(4) = False, reader.GetString(4), "")
                                    result.Name.LastName = If(reader.IsDBNull(5) = False, reader.GetString(5), "")

                                    result.Name2 = New QuickQuote.CommonObjects.QuickQuoteName()
                                    result.Name2.DisplayName = If(reader.IsDBNull(6) = False, reader.GetString(6), "")
                                    result.Name2.FirstName = If(reader.IsDBNull(7) = False, reader.GetString(7), "")
                                    result.Name2.MiddleName = If(reader.IsDBNull(8) = False, reader.GetString(8), "")
                                    result.Name2.LastName = If(reader.IsDBNull(9) = False, reader.GetString(9), "")

                                    result.Name.BirthDate = If(reader.IsDBNull(10) = False, reader.GetDateTime(10).ToString(), "")
                                    result.Name.TaxNumber = If(reader.IsDBNull(11) = False, reader.GetString(11), "")
                                    result.Name.TaxTypeId = If(reader.IsDBNull(12) = False, reader.GetInt32(12).ToString(), "")

                                    result.Name2.BirthDate = If(reader.IsDBNull(13) = False, reader.GetDateTime(13).ToString(), "")
                                    result.Name2.TaxNumber = If(reader.IsDBNull(14) = False, reader.GetString(14), "")
                                    result.Name2.TaxTypeId = If(reader.IsDBNull(15) = False, reader.GetInt32(15).ToString(), "")

                                    result.Name.SexId = If(reader.IsDBNull(16) = False, reader.GetInt32(16).ToString(), "")
                                    result.Name2.SexId = If(reader.IsDBNull(17) = False, reader.GetInt32(17).ToString(), "")

                                    'result.PrimaryPhone is readonly
                                    result.Phones = New List(Of QuickQuote.CommonObjects.QuickQuotePhone)
                                    Dim phone1 As New QuickQuote.CommonObjects.QuickQuotePhone()
                                    phone1.Number = If(reader.IsDBNull(18) = False, reader.GetString(18).ToString(), "") '10/7/2017 note: always home phone from storedProc (phoneTypeId 1)
                                    'phone1.PhoneId = If(reader.IsDBNull(19) = False, reader.GetInt32(19).ToString(), "")
                                    'updated 10/7/2017
                                    phone1.TypeId = If(reader.IsDBNull(19) = False, reader.GetInt32(19).ToString(), "") '10/7/2017 note: always home phone from storedProc (phoneTypeId 1)
                                    result.Phones.Add(phone1)

                                    'result.PrimaryEmail is readonly
                                    result.Emails = New List(Of QuickQuote.CommonObjects.QuickQuoteEmail)()
                                    Dim email1 As New QuickQuote.CommonObjects.QuickQuoteEmail
                                    email1.Address = If(reader.IsDBNull(20) = False, reader.GetString(20).ToString(), "") '10/7/2017 note: always home email from storedProc (emailTypeId 1)
                                    'email1.EmailId = If(reader.IsDBNull(21) = False, reader.GetInt32(21).ToString(), "")
                                    'updated 10/7/2017
                                    email1.TypeId = If(reader.IsDBNull(21) = False, reader.GetInt32(21).ToString(), "") '10/7/2017 note: always home email from storedProc (emailTypeId 1)
                                    result.Emails.Add(email1)

                                    result.Address = New QuickQuote.CommonObjects.QuickQuoteAddress()
                                    result.Address.AddressNum = If(reader.IsDBNull(22) = False, reader.GetInt32(22).ToString(), "")
                                    result.Address.HouseNum = If(reader.IsDBNull(23) = False, reader.GetString(23), "")
                                    result.Address.StreetName = If(reader.IsDBNull(24) = False, reader.GetString(24), "")
                                    result.Address.POBox = If(reader.IsDBNull(25) = False, reader.GetString(25), "")
                                    result.Address.ApartmentNumber = If(reader.IsDBNull(26) = False, reader.GetString(26), "")
                                    result.Address.City = If(reader.IsDBNull(27) = False, reader.GetString(27), "")
                                    result.Address.Zip = If(reader.IsDBNull(28) = False, reader.GetString(28), "")

                                    result.Address.County = If(reader.IsDBNull(29) = False, reader.GetString(29), "")
                                    result.Address.DisplayAddress = If(reader.IsDBNull(30) = False, reader.GetString(30), "")
                                    result.Address.State = If(reader.IsDBNull(31) = False, reader.GetString(31), "")
                                    result.Address.StateId = If(reader.IsDBNull(32) = False, reader.GetInt32(32), "")

                                    result.Name.SuffixName = If(reader.IsDBNull(33) = False, reader.GetString(33).Replace(".", ""), "")
                                    result.Name2.SuffixName = If(reader.IsDBNull(34) = False, reader.GetString(34).Replace(".", ""), "")

                                    result.Name.CommercialName1 = If(reader.IsDBNull(35) = False, reader.GetString(35), "")
                                    result.Name.EntityTypeId = If(reader.IsDBNull(36) = False, reader.GetInt32(36).ToString(), "")
                                    result.Name.TypeId = If(reader.IsDBNull(37) = False, reader.GetInt32(37).ToString(), "")

                                    'added 10/7/2017
                                    result.Name.DateBusinessStarted = If(reader.IsDBNull(38) = False, reader.GetDateTime(38).ToString(), "")
                                    'result.Name.YearsOfExperience = If(reader.IsDBNull(39) = False, reader.GetInt32(39).ToString(), "") 'invalid cast (tinyint type); GetInt16 didn't work either
                                    result.Name.YearsOfExperience = If(reader.IsDBNull(39) = False, reader.GetByte(39).ToString(), "")
                                    result.Name.DescriptionOfOperations = If(reader.IsDBNull(40) = False, reader.GetString(40), "")

                                    If reader.IsDBNull(41) = False Then
                                        If result.Phones Is Nothing Then
                                            result.Phones = New List(Of QuickQuote.CommonObjects.QuickQuotePhone)
                                        End If
                                        Dim busPhone As New QuickQuote.CommonObjects.QuickQuotePhone
                                        busPhone.Number = reader.GetString(41).ToString()
                                        busPhone.TypeId = "2"
                                        result.Phones.Add(busPhone)
                                    End If
                                    If reader.IsDBNull(42) = False Then
                                        If result.Phones Is Nothing Then
                                            result.Phones = New List(Of QuickQuote.CommonObjects.QuickQuotePhone)
                                        End If
                                        Dim cellPhone As New QuickQuote.CommonObjects.QuickQuotePhone
                                        cellPhone.Number = reader.GetString(42).ToString()
                                        cellPhone.TypeId = "4"
                                        result.Phones.Add(cellPhone)
                                    End If

                                    If reader.IsDBNull(43) = False Then
                                        If result.Emails Is Nothing Then
                                            result.Emails = New List(Of QuickQuote.CommonObjects.QuickQuoteEmail)
                                        End If
                                        Dim busEmail As New QuickQuote.CommonObjects.QuickQuoteEmail
                                        busEmail.Address = reader.GetString(43).ToString()
                                        busEmail.TypeId = "2"
                                        result.Emails.Add(busEmail)
                                    End If

                                    '10/7/2017 note: new properties to get info for 1st phone/email found w/o drilling into list
                                    'result.FirstEmailAddress
                                    'result.FirstEmailTypeId
                                    'result.FirstPhoneNumber
                                    'result.FirstPhoneTypeId
                                    'result.FirstPhoneExt
                                    'result.FirstPhoneNumberWithExtension

                                    '44 = Client#1 PrimaryPhone (1st one found in specific order for nameType): for bus - Bus/Cell/Home order; for pers - Home/Cell/Bus order
                                    '45 = Client#1 PrimaryPhoneTypeId (1st one found in specific order for nameType): for bus - Bus/Cell/Home order; for pers - Home/Cell/Bus order
                                    '46 = Client#1 PrimaryEmail (1st one found in specific order for nameType): for bus - Bus/Home order; for pers - Home/Bus order
                                    '47 = Client#1 PrimaryEmailTypeId (1st one found in specific order for nameType): for bus - Bus/Home order; for pers - Home/Bus order

                                    'Added 2/22/2022 for bug 63511 MLW
                                    result.Name.OtherLegalEntityDescription = If(reader.IsDBNull(48) = False, reader.GetString(48), "")

                                    result.Address.Other = If(reader.IsDBNull(49) = False, reader.GetString(49), "")

                                    results.Add(result)
                                End While
                            End If
                        End Using

                    End Using
                End Using
            Catch ex As Exception
#If DEBUG Then
                Debugger.Break()
#End If
            End Try

            Return results
        End Function
    End Class

End Namespace