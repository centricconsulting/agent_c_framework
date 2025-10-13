Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.ApplicantSearch
    Public Class ApplicantSearch

        Public Shared Function SearchApplicants(agencyId As String, firstName As String, lastName As String, zip As String, ssn As String) As List(Of QuickQuote.CommonObjects.QuickQuoteApplicant)

            Dim results As New List(Of QuickQuote.CommonObjects.QuickQuoteApplicant)

            Using conn As New System.Data.SqlClient.SqlConnection
                conn.ConnectionString = System.Configuration.ConfigurationManager.AppSettings("ConnQQ")
                conn.Open()
                Using cmd As New System.Data.SqlClient.SqlCommand
                    cmd.Connection = conn
                    cmd.CommandText = "[usp_QuickQuote_PersonalApplicantSearch_Matt]"
                    cmd.CommandType = CommandType.StoredProcedure

                    If String.IsNullOrWhiteSpace(agencyId) = False Then
                        cmd.Parameters.AddWithValue("@AgencyID", agencyId)
                    End If
                    If String.IsNullOrWhiteSpace(firstName) = False Then
                        cmd.Parameters.AddWithValue("@FirstName", firstName)
                    End If
                    If String.IsNullOrWhiteSpace(lastName) = False Then
                        cmd.Parameters.AddWithValue("@LastName", lastName)
                    End If
                    If String.IsNullOrWhiteSpace(zip) = False Then
                        cmd.Parameters.AddWithValue("@Zip", zip)
                    End If
                    If String.IsNullOrWhiteSpace(ssn) = False Then
                        cmd.Parameters.AddWithValue("@SSN", ssn)
                    End If

                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            While reader.Read()
                                Dim app As New QuickQuote.CommonObjects.QuickQuoteApplicant()

                                app.Name.FirstName = reader.GetStringIgnoreDBNull(0)
                                app.Name.MiddleName = reader.GetStringIgnoreDBNull(1)
                                app.Name.LastName = reader.GetStringIgnoreDBNull(2)
                                app.Name.SuffixName = reader.GetStringIgnoreDBNull(3)
                                app.Name.SexId = reader.GetIntAsStringIgnoreDBNull(4)
                                app.Name.BirthDate = reader.GetDateTimeAsShortDateStringIgnoreDBNull(5)
                                app.Name.TaxNumber = reader.GetStringIgnoreDBNull(6)

                                app.Emails = New List(Of QuickQuote.CommonObjects.QuickQuoteEmail)()
                                Dim email As New QuickQuote.CommonObjects.QuickQuoteEmail()
                                email.Address = reader.GetStringIgnoreDBNull(7)
                                app.Emails.Add(email)

                                app.Phones = New List(Of QuickQuote.CommonObjects.QuickQuotePhone)()
                                Dim phone As New QuickQuote.CommonObjects.QuickQuotePhone()
                                phone.Number = reader.GetStringIgnoreDBNull(8)
                                phone.Extension = reader.GetStringIgnoreDBNull(9)
                                phone.TypeId = reader.GetIntAsStringIgnoreDBNull(10)
                                app.Phones.Add(phone)

                                app.Address.HouseNum = reader.GetStringIgnoreDBNull(11)
                                app.Address.StreetName = reader.GetStringIgnoreDBNull(12)
                                app.Address.ApartmentNumber = reader.GetStringIgnoreDBNull(13)
                                app.Address.POBox = reader.GetStringIgnoreDBNull(14)
                                app.Address.Zip = reader.GetStringIgnoreDBNull(15)
                                app.Address.City = reader.GetStringIgnoreDBNull(16)
                                app.Address.StateId = reader.GetIntAsStringIgnoreDBNull(17)
                                app.Address.County = reader.GetStringIgnoreDBNull(18)

                                results.Add(app)
                            End While
                        End If
                    End Using
                End Using
            End Using
            Return results
        End Function

    End Class
End Namespace