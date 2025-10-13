Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects
Imports System.Configuration.ConfigurationManager

Namespace IFM.VR.Common.Helpers.CPP

    Public Class CPPCrimeClassCodeHelper
        Public Shared Function GetClassCodes(SearchTypeid As String, searchString As String, lob As String, ProgramTypeId As String) As List(Of CPPCrimeClassCodeLookupResult)
            Dim results As New List(Of CPPCrimeClassCodeLookupResult)
            'Dim conn2 As New System.Data.SqlClient.SqlConnection()
            'Dim cmd2 As New System.Data.SqlClient.SqlCommand()
            'Dim da As New System.Data.SqlClient.SqlDataAdapter()
            'Dim tblPMA As New System.Data.DataTable()



            Using conn As New System.Data.SqlClient.SqlConnection(AppSettings("connQQ"))
                Using cmd As New System.Data.SqlClient.SqlCommand()
                    conn.Open()
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandText = "usp_CGL_Search_ClassCodes"
                    ' Note:  The stored procedure expects 1 for contains, 2 for starts with & 3 for class code

                    cmd.Parameters.AddWithValue("@searchtype", SearchTypeid)
                    cmd.Parameters.AddWithValue("@searchstring", searchString)
                    cmd.Parameters.AddWithValue("@programtype_id", ProgramTypeId)
                    cmd.Parameters.AddWithValue("@linetype", lob)
                    cmd.Parameters.AddWithValue("@editiondate", DateTime.Now)

                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            While reader.Read()
                                Dim cc As New CPPCrimeClassCodeLookupResult()
                                cc.DescriptionID = If(reader.IsDBNull(0) = False, reader.GetInt32(0).ToString(), "")
                                cc.CodeId = If(reader.IsDBNull(1) = False, reader.GetInt32(1).ToString(), "")
                                cc.LOB = If(reader.IsDBNull(2) = False, reader.GetString(2), "")
                                cc.StateSpecific = If(reader.IsDBNull(3) = False, reader.GetString(3), "")
                                cc.Description = If(reader.IsDBNull(4) = False, reader.GetString(4), "")
                                ' escape special characters in the description
                                cc.Description = cc.Description.Replace("""", "\" & """")
                                cc.Description = cc.Description.Replace("'", "\'")
                                cc.ClassCode = If(reader.IsDBNull(5) = False, reader.GetString(5), "")
                                cc.PMA = If(reader.IsDBNull(6) = False, reader.GetString(6), "")
                                cc.NOC = If(reader.IsDBNull(7) = False, reader.GetString(7), "")
                                cc.PremiumBase = If(reader.IsDBNull(8) = False, reader.GetString(8), "")
                                cc.RateGroup = If(reader.IsDBNull(9) = False, reader.GetString(9), "")
                                cc.ClassLimit = If(reader.IsDBNull(10) = False, reader.GetString(10), "")
                                cc.PremiumBaseShort = If(reader.IsDBNull(11) = False, reader.GetString(11), "")
                                cc.CPPMA = If(reader.IsDBNull(12) = False, reader.GetString(12), "")

                                'SOMETIMES multiple PMAs will be in the PMA field as a comma-delimted string.  MGB 2-21-18
                                If cc.CPPMA IsNot Nothing AndAlso cc.CPPMA.Trim <> "" AndAlso cc.CPPMA.Contains(",") Then
                                    ' Comma-delimted string found. Parse the PMAS out of the delimited PMA.
                                    Dim pieces() As String = cc.CPPMA.Split(",")
                                    If pieces.Count > 0 Then
                                        cc.PMAs = New List(Of String)
                                        For Each s As String In pieces
                                            cc.PMAs.Add(s.Trim)
                                        Next
                                    End If
                                End If

                                'LOOK UP PMA'S - Loads multiple PMA's into the PMAs list field if there are more than one
                                Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
                                Dim spm As New SPManager("connDiamondReports", "usp_Get_PMAs")
                                spm.AddIntegerParamater("@CodeID", qqh.IntegerForString(cc.CodeId))
                                Dim tbl As DataTable = spm.ExecuteSPQuery()
                                If tbl IsNot Nothing AndAlso tbl.Rows IsNot Nothing AndAlso tbl.Rows.Count > 1 Then
                                    cc.PMAs = New List(Of String)
                                    For Each d As DataRow In tbl.Rows
                                        Dim val As String = d("pma").ToString()
                                        val = val.Replace(",", "**")  ' Replace all embedded commas with special characters so it doesn't mess up the splitter later
                                        cc.PMAs.Add(val)
                                    Next
                                End If

                                'conn2.ConnectionString = AppSettings("connQQ")
                                'conn2.Open()
                                'cmd2.Connection = conn2
                                'cmd2.CommandType = CommandType.Text
                                'cmd2.CommandText = "SELECT pma from Diamond.dbo.CLMDescription WHERE code_id = " & cc.CodeId & " AND (pma is not null AND pma <> '') AND lob = 'CP' AND effective_date < '" & DateTime.Now.ToShortDateString & "' AND expiration_date > '" & DateTime.Now.ToShortDateString() & "'"
                                'tblPMA = New DataTable()
                                'da.SelectCommand = cmd2
                                'da.Fill(tblPMA)
                                'If tblPMA IsNot Nothing AndAlso tblPMA.Rows IsNot Nothing AndAlso tblPMA.Rows.Count > 1 Then
                                '    cc.PMAs = New List(Of String)
                                '    For Each d As DataRow In tblPMA.Rows
                                '        Dim val As String = d("pma").ToString()
                                '        val = val.Replace(",", "**")  ' Replace all embedded commas with special characters so it doesn't mess up the splitter later
                                '        cc.PMAs.Add(val)
                                '    Next
                                'End If
                                ' END OF PMA LOOKUP

                                cc.FootNoteType = If(reader.IsDBNull(13) = False, reader.GetString(13), "")
                                ' Escape special characters in the footnote text
                                Dim fnText As String = ""
                                If Not reader.IsDBNull(14) Then fnText = reader.GetString(14)
                                fnText = fnText.Replace("""", "\" & """")
                                fnText = fnText.Replace("'", "\'")
                                cc.FootNotes.Add(fnText)
                                'cc.FootNotes.Add(If(reader.IsDBNull(14) = False, reader.GetString(14), ""))
                                cc.CLMSubSectionID1 = If(reader.IsDBNull(15) = False, reader.GetInt32(15).ToString(), "")
                                cc.CLMRatingBaseID = If(reader.IsDBNull(16) = False, reader.GetInt32(16).ToString(), "")
                                cc.CLMSubSectionID2 = If(reader.IsDBNull(17) = False, reader.GetInt32(17).ToString(), "")
                                cc.Naics = If(reader.IsDBNull(18) = False, reader.GetString(18), "")
                                cc.SIC = If(reader.IsDBNull(19) = False, reader.GetString(19), "")

                                ' according to insuresoft if two classcodes are returned just ignore the second except to add its footnote text to the first instance
                                Dim alreadyExists As Boolean = False
                                For Each c As CPPCrimeClassCodeLookupResult In results
                                    If c.DescriptionID = cc.DescriptionID Then
                                        c.FootNotes.Add(cc.FootNote) ' add this footnote text to the existing
                                        alreadyExists = True
                                        Exit For
                                    End If
                                Next
                                If Not alreadyExists Then 'And cc.ClassCode.Trim() <> "*" Then '1-21-2013 added "*" check
                                    results.Add(cc)
                                End If

                            End While
                        End If
                    End Using
                End Using
            End Using

            Return results
        End Function
    End Class

    Public Class CPPCrimeClassCodeLookupResult

        Public Property DescriptionID As String
        Public Property CodeId As String
        Public Property LOB As String
        Public Property StateSpecific As String
        Public Property Description As String
        Public Property ClassCode As String
        Public Property PMA As String
        Public Property PMAs As List(Of String)
        Public Property NOC As String
        Public Property PremiumBase As String
        Public Property RateGroup As String
        Public Property ClassLimit As String
        Public Property PremiumBaseShort As String
        Public Property CPPMA As String
        Public Property FootNoteType As String
        'Public ReadOnly Property FootNote As String
        '    Get
        '        Dim fn As String = ""
        '        For Each f As String In FootNotes
        '            fn += f
        '        Next
        '        If String.IsNullOrWhiteSpace(fn) And ClassCode.Trim() = "*" Then
        '            fn = Keys_VR_CGL.ClassCodeInvalidClassCodeVerbiage
        '        End If
        '        Return fn
        '    End Get
        'End Property

        Public Property CLMSubSectionID1 As String
        Public Property CLMRatingBaseID As String
        Public Property CLMSubSectionID2 As String
        Public Property Naics As String
        Public Property PremRate As String
        Public Property Prodrate As String
        Public Property SIC As String

        Public ReadOnly Property FootNote As String
            Get
                Dim fn As String = ""
                For Each f As String In FootNotes
                    fn += f
                Next
                If String.IsNullOrWhiteSpace(fn) And ClassCode.Trim() = "*" Then
                    fn = AppSettings("ClassCodeInvalidClassCodeVerbiage")
                End If
                Return fn
            End Get
        End Property

        Private _footNotes As New List(Of String)
        Public ReadOnly Property FootNotes As List(Of String)
            Get
                Return _footNotes
            End Get
        End Property

        'Public Property DescriptionID As String
        'Public Property CodeId As String
        'Public Property LOB As String
        'Public Property StateSpecific As String
        'Public Property Description As String
        'Public Property ClassCode As String
        'Public Property PMA As String
        'Public Property PMAs As List(Of String)
        'Public Property NOC As String
        'Public Property PremiumBase As String
        'Public Property RateGroup As String
        'Public Property ClassLimit As String
        'Public Property PremiumBaseShort As String
        'Public Property CPPMA As String
        'Public Property FootNoteType As String
        'Public Property CLMSubSectionID1 As String
        'Public Property CLMRatingBaseID As String
        'Public Property CLMSubSectionID2 As String
        'Public Property NAICS As String
        'Public Property SIC As String
        'Public ReadOnly Property FootNote As String
        '    Get
        '        Dim fn As String = ""
        '        For Each f As String In FootNotes
        '            fn += f
        '        Next
        '        If String.IsNullOrWhiteSpace(fn) And ClassCode.Trim() = "*" Then
        '            fn = AppSettings("ClassCodeInvalidClassCodeVerbiage")
        '        End If
        '        Return fn
        '    End Get
        'End Property
        'Private _footNotes As New List(Of String)
        'Public ReadOnly Property FootNotes As List(Of String)
        '    Get
        '        Return _footNotes
        '    End Get
        'End Property


    End Class

End Namespace
