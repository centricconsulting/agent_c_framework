Imports System.Text.RegularExpressions
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.MultiState
Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.CGL

    Public Class ClassCodeHelper

        Public Enum SearchType
            Class_Code_Description = 0
            Class_Code_Description_Contains = 1
            Class_Code = 2
        End Enum

        Public Shared Function SearchDBForClassCodes(searchType As SearchType, searchtext As String, lob As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType, programTypeId As Int32) As List(Of ClassCodesearchResult)
            Dim results As New List(Of ClassCodesearchResult)

            Dim lineType As String = GetClassCodeLOBAbbrevFromQQOType(lob)


            Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("connQQ"))
                Using cmd As New System.Data.SqlClient.SqlCommand()
                    cmd.Connection = conn
                    conn.Open()
                    cmd.CommandText = "usp_CGL_Search_ClassCodes" '"assp_CLM_Search" 1-23-2013 created new proc to filter '*' and 'NA' from results
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.AddWithValue("@searchtype", CInt(searchType).ToString())
                    cmd.Parameters.AddWithValue("@searchstring", searchtext)
                    cmd.Parameters.AddWithValue("@programtype_id", programTypeId.ToString())
                    cmd.Parameters.AddWithValue("@linetype", lineType)
                    cmd.Parameters.AddWithValue("@editiondate", DateTime.Now)


                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            While reader.Read()
                                Dim cc As New ClassCodesearchResult()
                                cc.DescriptionID = reader.GetIntAsStringIgnoreDBNull(0)
                                cc.CodeId = reader.GetIntAsStringIgnoreDBNull(1)
                                cc.LOB = If(reader.IsDBNull(2) = False, reader.GetString(2), "")
                                cc.StateSpecific = If(reader.IsDBNull(3) = False, reader.GetString(3), "")
                                cc.Description = If(reader.IsDBNull(4) = False, reader.GetString(4), "")
                                cc.ClassCode = If(reader.IsDBNull(5) = False, reader.GetString(5), "")
                                cc.PMA = If(reader.IsDBNull(6) = False, reader.GetString(6), "")
                                cc.NOC = If(reader.IsDBNull(7) = False, reader.GetString(7), "")
                                cc.PremiumBase = If(reader.IsDBNull(8) = False, reader.GetString(8), "")
                                cc.RateGroup = If(reader.IsDBNull(9) = False, reader.GetString(9), "")
                                cc.ClassLimit = If(reader.IsDBNull(10) = False, reader.GetString(10), "")
                                cc.PremiumBaseShort = If(reader.IsDBNull(11) = False, reader.GetString(11), "")
                                cc.CPPMA = If(reader.IsDBNull(12) = False, reader.GetString(12), "")
                                cc.FootNoteType = If(reader.IsDBNull(13) = False, reader.GetString(13), "")
                                cc.FootNotes.Add(If(reader.IsDBNull(14) = False, reader.GetString(14), ""))
                                cc.CLMSubSectionID1 = If(reader.IsDBNull(15) = False, reader.GetInt32(15).ToString(), "")
                                cc.CLMRatingBaseID = If(reader.IsDBNull(16) = False, reader.GetInt32(16).ToString(), "")
                                cc.CLMSubSectionID2 = If(reader.IsDBNull(17) = False, reader.GetInt32(17).ToString(), "")
                                cc.Naics = If(reader.IsDBNull(18) = False, reader.GetString(18), "")
                                cc.SIC = If(reader.IsDBNull(19) = False, reader.GetString(19), "")

                                ' according to insuresoft if two classcodes are returned just ignore the second except to add its footnote text to the first instance
                                Dim alreadyExists As Boolean = False
                                For Each c As ClassCodesearchResult In results
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


        Public Shared Function SearchDBForClassCode(ByVal classCodeID As String, classcode As String, lob As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType, programTypeId As Int32) As List(Of ClassCodesearchResult)

            Dim lineType As String = GetClassCodeLOBAbbrevFromQQOType(lob)


            Dim results As New List(Of ClassCodesearchResult)
            Using conn As New System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings("connQQ"))
                Using cmd As New System.Data.SqlClient.SqlCommand()
                    cmd.Connection = conn
                    conn.Open()
                    cmd.CommandText = "usp_CGL_Get_ClassCode"
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.AddWithValue("@ClassCodeID", classCodeID)
                    cmd.Parameters.AddWithValue("@ClassCode", classcode)
                    cmd.Parameters.AddWithValue("@programtype_id", programTypeId)
                    cmd.Parameters.AddWithValue("@linetype", lineType)
                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            While reader.Read()
                                Dim cc As New ClassCodesearchResult()
                                cc.DescriptionID = If(reader.IsDBNull(0) = False, reader.GetInt32(0).ToString(), "")
                                cc.CodeId = If(reader.IsDBNull(1) = False, reader.GetInt32(1).ToString(), "")
                                cc.LOB = If(reader.IsDBNull(2) = False, reader.GetString(2), "")
                                cc.StateSpecific = If(reader.IsDBNull(3) = False, reader.GetString(3), "")
                                cc.Description = If(reader.IsDBNull(4) = False, reader.GetString(4), "")
                                cc.ClassCode = If(reader.IsDBNull(5) = False, reader.GetString(5), "")
                                cc.PMA = If(reader.IsDBNull(6) = False, reader.GetString(6), "")
                                cc.NOC = If(reader.IsDBNull(7) = False, reader.GetString(7), "")
                                cc.PremiumBase = If(reader.IsDBNull(8) = False, reader.GetString(8), "")
                                cc.RateGroup = If(reader.IsDBNull(9) = False, reader.GetString(9), "")
                                cc.ClassLimit = If(reader.IsDBNull(10) = False, reader.GetString(10), "")
                                cc.PremiumBaseShort = If(reader.IsDBNull(11) = False, reader.GetString(11), "")
                                cc.CPPMA = If(reader.IsDBNull(12) = False, reader.GetString(12), "")
                                cc.FootNoteType = If(reader.IsDBNull(13) = False, reader.GetString(13), "")
                                cc.FootNotes.Add(If(reader.IsDBNull(14) = False, reader.GetString(14), ""))
                                cc.CLMSubSectionID1 = If(reader.IsDBNull(15) = False, reader.GetInt32(15).ToString(), "")
                                cc.CLMRatingBaseID = If(reader.IsDBNull(16) = False, reader.GetInt32(16).ToString(), "")
                                'cc.CLMSubSectionID2 = If(reader.IsDBNull(17) = False, reader.GetString(17), "")
                                cc.Naics = If(reader.IsDBNull(17) = False, reader.GetString(17), "")
                                cc.SIC = If(reader.IsDBNull(18) = False, reader.GetString(18), "")
                                cc.PremRate = If(reader.IsDBNull(19) = False, reader.GetString(19), "")
                                cc.Prodrate = If(reader.IsDBNull(20) = False, reader.GetString(20), "")
                                ' according to insuresoft if two classcodes are returned just ignore the second except to add its footnote text to the first instance
                                Dim alreadyExists As Boolean = False
                                For Each c As ClassCodesearchResult In results
                                    If c.DescriptionID = cc.DescriptionID Then
                                        c.FootNotes.Add(cc.FootNote) ' add this footnote text to the existing
                                        alreadyExists = True
                                    End If

                                Next
                                If Not alreadyExists Then
                                    results.Add(cc)
                                End If
                            End While
                        End If
                    End Using

                End Using

            End Using
            Return results
        End Function

        Public Shared Function GetAllPolicyAndLocationClassCodes(ByVal topQuote As QuickQuoteObject) As List(Of QuickQuoteGLClassification)
            Dim list As New List(Of QuickQuoteGLClassification)
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If

                If topQuote.GLClassifications IsNot Nothing Then
                    For Each cc As QuickQuoteGLClassification In topQuote.GLClassifications
                        list.Add(cc)
                    Next
                End If

                If topQuote.Locations IsNot Nothing Then
                    For Each Loc As QuickQuoteLocation In topQuote.Locations
                        If Loc.GLClassifications IsNot Nothing Then
                            For Each cc As QuickQuoteGLClassification In Loc.GLClassifications
                                list.Add(cc)
                            Next
                        End If
                    Next
                End If
            End If
            Return list
        End Function

        Public Shared Function GetAllPolicyAndLocationClassCodes_NewToImage(ByVal topQuote As QuickQuoteObject) As List(Of QuickQuoteGLClassification)
            Dim list As New List(Of QuickQuoteGLClassification)
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If

                ' this is the new to image version of the above function
                Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass


                If topQuote.GLClassifications IsNot Nothing AndAlso topQuote.GLClassifications.Count > 0 Then
                    For Each qqGL As QuickQuoteGLClassification In topQuote.GLClassifications
                        If QQHelper.IsQuickQuoteGLClassificationNewToImage(qqGL, topQuote) Then
                            list.Add(qqGL)
                        End If
                    Next
                End If

                If topQuote.Locations IsNot Nothing Then
                    For Each Loc As QuickQuoteLocation In topQuote.Locations
                        If QQHelper.IsQuickQuoteLocationNewToImage(Loc, topQuote) AndAlso Loc.GLClassifications IsNot Nothing Then
                            For Each cc As QuickQuoteGLClassification In Loc.GLClassifications
                                list.Add(cc)
                            Next
                        End If
                    Next
                End If
            End If
            Return list
        End Function

        Public Shared Function AtleastOneGLClassCodeHasAnExposureGreaterThanZero(ByVal topQuote As QuickQuoteObject) As Boolean
            Dim exposure As Double = 0
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If topQuote.GLClassifications IsNot Nothing Then
                    For Each cc As QuickQuoteGLClassification In topQuote.GLClassifications
                        exposure += cc.PremiumExposure.TryToGetDouble
                    Next
                End If

                If topQuote.Locations IsNot Nothing Then
                    For Each Loc As QuickQuoteLocation In topQuote.Locations
                        If Loc.GLClassifications IsNot Nothing Then
                            For Each cc As QuickQuoteGLClassification In Loc.GLClassifications
                                exposure += cc.PremiumExposure.TryToGetDouble
                            Next
                        End If
                    Next
                End If

            End If
            Return exposure > 0
        End Function

        Public Shared Sub ConvertAllClassCodeSubline336ExcludedTo(ByVal topQuote As QuickQuoteObject)
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                Dim excluded As Boolean = False
                Dim sqf = IFM.VR.Common.Helpers.MultiState.General.SubQuotes(topQuote).FirstOrDefault()
                excluded = If(sqf IsNot Nothing, CBool(sqf.ProductsCompletedOperationsAggregateLimitId = "327"), False)

                If topQuote.GLClassifications IsNot Nothing Then
                    For Each cc2 As QuickQuoteGLClassification In topQuote.GLClassifications
                        cc2.Subline336_ExcludeProductsCompletedOperations = excluded
                    Next
                End If

                If topQuote.Locations IsNot Nothing Then
                    For Each Loc As QuickQuoteLocation In topQuote.Locations
                        If Loc.GLClassifications IsNot Nothing Then
                            For Each cc As QuickQuoteGLClassification In Loc.GLClassifications
                                cc.Subline336_ExcludeProductsCompletedOperations = excluded
                            Next
                        End If
                    Next
                End If
            End If
        End Sub


        Public Shared Function RemoveClassCodeByIndex(ByVal qso As QuickQuoteObject, ByVal index As Int32) As Boolean
            ' when ever you interact with these lists you have to be careful it must be done the same everywhere
            '  the way it displays must be the same every time

            Dim ccIndex As Int32 = 0
            If qso.GLClassifications IsNot Nothing Then
                For Each cc As QuickQuoteGLClassification In qso.GLClassifications
                    If ccIndex = index Then
                        qso.GLClassifications.Remove(cc)
                        Return True
                    End If
                    ccIndex += 1
                Next
            End If

            If qso.Locations IsNot Nothing Then
                Dim locationIndex As Int32 = 0
                For Each Loc As QuickQuoteLocation In qso.Locations
                    If Loc.GLClassifications IsNot Nothing Then
                        For Each cc As QuickQuoteGLClassification In Loc.GLClassifications
                            If ccIndex = index Then
                                Loc.GLClassifications.Remove(cc)
                                Return True
                            End If
                            ccIndex += 1
                        Next
                    End If
                    locationIndex += 1
                Next
            End If

            Return False
        End Function

        Public Shared Function GetClassCodeAtIndex(ByVal qso As QuickQuoteObject, ByVal getIndex As Int32) As QuickQuoteGLClassification
            Dim ccIndex As Int32 = 0

            If qso.GLClassifications IsNot Nothing Then
                For Each cc As QuickQuoteGLClassification In qso.GLClassifications
                    If ccIndex = getIndex Then
                        Return cc
                    End If
                    ccIndex += 1
                Next
            End If

            If qso.Locations IsNot Nothing Then
                Dim locationIndex As Int32 = 0
                For Each Loc As QuickQuoteLocation In qso.Locations
                    If Loc.GLClassifications IsNot Nothing Then
                        For Each cc As QuickQuoteGLClassification In Loc.GLClassifications
                            If ccIndex = getIndex Then
                                Return cc
                            End If
                            ccIndex += 1
                        Next
                    End If
                    locationIndex += 1
                Next
            End If
            Return Nothing
        End Function

        ' some of these functions are not very efficient or they do in three functions what could be done in one
        ' but having all the logic in these three functions instead of all over the classcode control where any change will require
        ' tracking down the multiple versions of this stuff offsets the inefficiency

        Public Shared Function ClassCodeAtIndexIsLocationAssigned(ByVal qso As QuickQuoteObject, ByVal getIndex As Int32) As Boolean
            Dim ccIndex As Int32 = 0

            If qso.GLClassifications IsNot Nothing Then
                For Each cc As QuickQuoteGLClassification In qso.GLClassifications
                    If ccIndex = getIndex Then
                        Return False
                    End If
                    ccIndex += 1
                Next
            End If

            If qso.Locations IsNot Nothing Then
                Dim locationIndex As Int32 = 0
                For Each Loc As QuickQuoteLocation In qso.Locations
                    If Loc.GLClassifications IsNot Nothing Then
                        For Each cc As QuickQuoteGLClassification In Loc.GLClassifications
                            If ccIndex = getIndex Then
                                Return True
                            End If
                            ccIndex += 1
                        Next
                    End If
                    locationIndex += 1
                Next
            End If
            Return False
        End Function

        Public Shared Function ClassCodeAtIndexIsAssignedToLocationNumber(ByVal qso As QuickQuoteObject, ByVal getIndex As Int32) As Int32
            Dim ccIndex As Int32 = 0

            If qso.GLClassifications IsNot Nothing Then
                For Each cc As QuickQuoteGLClassification In qso.GLClassifications
                    If ccIndex = getIndex Then
                        Return -1
                    End If
                    ccIndex += 1
                Next
            End If

            If qso.Locations IsNot Nothing Then
                Dim locationIndex As Int32 = 0
                For Each Loc As QuickQuoteLocation In qso.Locations
                    If Loc.GLClassifications IsNot Nothing Then
                        For Each cc As QuickQuoteGLClassification In Loc.GLClassifications
                            If ccIndex = getIndex Then
                                Return locationIndex
                            End If
                            ccIndex += 1
                        Next
                    End If
                    locationIndex += 1
                Next
            End If
            Return -1
        End Function


        Public Shared Function GetFootNote(ByVal classcode As String, ByVal programtypeid As Int32, ByVal lob As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType, Optional ByRef currentPremiumBaseShort As String = "") As String
            Dim footnoteText As String = ""
            Dim result As List(Of ClassCodesearchResult) = SearchDBForClassCode(Nothing, classcode, lob, programtypeid.ToString())
            If result.Any() Then
                ' according to insuresoft if you get back duplicates here you combine the footnotes
                If result.Count > 1 Then
                    For Each cc As ClassCodesearchResult In result
                        footnoteText += cc.FootNote
                        If String.IsNullOrWhiteSpace(currentPremiumBaseShort) = True AndAlso String.IsNullOrWhiteSpace(cc.PremiumBaseShort) = False Then
                            currentPremiumBaseShort = cc.PremiumBaseShort
                        End If
                    Next
                Else
                    footnoteText = result(0).FootNote
                    If String.IsNullOrWhiteSpace(currentPremiumBaseShort) = True AndAlso String.IsNullOrWhiteSpace(result(0).PremiumBaseShort) = False Then
                        currentPremiumBaseShort = result(0).PremiumBaseShort
                    End If
                End If
            End If
            Return footnoteText
        End Function

        Public Shared Function GetPMAfromFoootNote(ByVal footnoteText As String, ByVal classCodeNumber As String) As List(Of String)
            ' this is all based on an Email from insuresoft on 11/27/2012
            Dim pmaValues As New List(Of String)
            ' need to find all the class codes
            If Not String.IsNullOrWhiteSpace(footnoteText) Then
                Dim classCodes As MatchCollection = Regex.Matches(footnoteText, "\d{4}") ' cpr classcodes are always 4 digits
                For Each cc As Match In classCodes
                    If cc.ToString().Trim() = classCodeNumber.Trim() Then
                        ' then parse out PMA from subsequent text 
                        Dim pmaText As String = footnoteText.Substring(cc.Index + cc.Length, footnoteText.IndexOfAny(New Char() {CChar("."), CChar(";")}, cc.Index) - (cc.Index + cc.Length))
                        pmaText = pmaText.Replace(" and CPP PMA ", "")
                        pmaText = pmaText.Replace("CPP PMA ", "")
                        pmaText = pmaText.Replace(" or ", ",")
                        pmaText = pmaText.Trim()
                        If pmaText.StartsWith(",") Then
                            pmaText = pmaText.Remove(0, 1).Trim()
                        End If
                        If pmaText.Contains(",") Then
                            pmaValues = pmaText.Split(CChar(",")).ToList()
                        Else
                            pmaValues.Add(pmaText)
                        End If
                        Exit For ' only do it once
                    End If
                Next
            End If
            Return pmaValues
        End Function

        ' InsureSofts version of GetPMAfromFoootNote() above
        'Private Sub SetFootnoteText(ByVal footnoteText As String)
        '    'Dim tempText As String = String.Empty
        '    'tempText = tempText & "<body style=""font-size: 11px; font-family: 'Microsoft Sans Serif';"" leftmargin=""3px"" topmargin=""3px"" marginwidth=""3px"" marginheight=""3px"">"
        '    ''replace bullets with HTML for lists <li>
        '    ''Dim bulletString As String = "   ·"
        '    'Dim bulletString As String = " ï¿½"
        '    Dim done As Boolean = False
        '    Dim startPosition As Integer = 0
        '    Dim endPosition As Integer = 0
        '    'Dim eolStr As String = Nothing

        '    'While Not done
        '    '    startPosition = footnoteText.IndexOf(bulletString)
        '    '    If startPosition < 0 Then
        '    '        done = True
        '    '    Else
        '    '        eolStr = vbCrLf
        '    '        endPosition = footnoteText.IndexOf(eolStr, startPosition)
        '    '        If endPosition < 0 Then
        '    '            endPosition = footnoteText.IndexOf(bulletString, startPosition + 1)
        '    '            eolStr = String.Empty
        '    '        End If

        '    '        If endPosition < 0 Then
        '    '            footnoteText = footnoteText & "</li></ul>"
        '    '        Else
        '    '            footnoteText = footnoteText.Remove(endPosition, eolStr.Length)
        '    '            footnoteText = footnoteText.Insert(endPosition, "</li></ul>")
        '    '        End If
        '    '        footnoteText = footnoteText.Remove(startPosition, bulletString.Length)
        '    '        footnoteText = footnoteText.Insert(startPosition, "<ul style=""margin-top: 0px; margin-bottom: 0px""><li>")
        '    '    End If
        '    'End While

        '    ''Replace cbCrLF with <br/>
        '    'footnoteText = footnoteText.Replace(vbCrLf, "<br/>")
        '    'done = False
        '    Dim webText As String = String.Empty
        '    Dim pmaPosition As Integer = 0
        '    Dim pmaEndPosition As Integer = 0
        '    Dim pmaText As String = String.Empty
        '    'Replace class codes in text with html anchor links <a href="#CodeXXXX>XXXX</a>
        '    While Not done
        '        Dim tmp As Integer = 0
        '        startPosition = FindCodeString(footnoteText, endPosition)
        '        If startPosition >= 0 Then
        '            If endPosition > 0 Then
        '                Dim sCode As String = footnoteText.Substring(startPosition, endPosition - startPosition + 1)
        '                webText &= footnoteText.Substring(0, startPosition)
        '                'todo: the PMA parsing isn't done on the web
        '                pmaText = String.Empty
        '                pmaPosition = footnoteText.IndexOf("CPP PMA", endPosition)
        '                If pmaPosition > 0 AndAlso pmaPosition < (endPosition + 15) Then
        '                    pmaPosition += 8
        '                    pmaEndPosition = footnoteText.IndexOfAny(New Char() {"."c, ";"c}, pmaPosition)
        '                    If pmaEndPosition < 0 Then pmaEndPosition = footnoteText.Length
        '                    Dim pmaTrial As String = footnoteText.Substring(pmaPosition, pmaEndPosition - pmaPosition)
        '                    pmaText = pmaTrial.Replace(" or ", ", ")
        '                End If
        '                pmaText = pmaText.Replace(" ", "_")
        '                ' end PMA parsing
        '                footnoteText = footnoteText.Remove(0, endPosition + 1)
        '                webText &= "<a href= ""#Code" & sCode & "_" & pmaText & """>" & sCode & "</a>"
        '            End If
        '        Else
        '            webText = webText & footnoteText
        '            done = True
        '        End If
        '    End While

        '    tempText = tempText & webText
        '    tempText = tempText & "</body>"
        '    FootnotesWebBrowserControl.DocumentText = tempText
        '    'Have to wait for the web control to finish loading the document or it doesn't display
        '    While (FootnotesWebBrowserControl.ReadyState <> WebBrowserReadyState.Complete)
        '        Application.DoEvents()
        '    End While
        'End Sub

        Public Shared Function GetClassCodeLOBAbbrevFromQQOType(ByVal qqType As QuickQuoteObject.QuickQuoteLobType) As String
            Select Case qqType

                Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                    Return "GL"

                Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    'Return "CF"  ' CF DOESN' SEEM TO WORK WITH CPP, GL DOES
                    Return "GL"
                Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                    Return "CF"

                Case Else
                    Return ""
            End Select
        End Function

    End Class

    Public Class ClassCodesearchResult

        Public Property DescriptionID As String
        Public Property CodeId As String
        Public Property LOB As String
        Public Property StateSpecific As String
        Public Property Description As String
        Public Property ClassCode As String
        Public Property PMA As String
        Public Property NOC As String
        Public Property PremiumBase As String
        Public Property RateGroup As String
        Public Property ClassLimit As String
        Public Property PremiumBaseShort As String
        Public Property CPPMA As String
        Public Property FootNoteType As String
        Public ReadOnly Property FootNote As String
            Get
                Dim fn As String = ""
                For Each f As String In FootNotes
                    fn += f
                Next
                If String.IsNullOrWhiteSpace(fn) And ClassCode.Trim() = "*" Then
                    fn = "Rating and coding information should be determined from a more specific classification."
                End If
                Return fn
            End Get
        End Property

        Public Property CLMSubSectionID1 As String
        Public Property CLMRatingBaseID As String
        Public Property CLMSubSectionID2 As String
        Public Property Naics As String
        Public Property PremRate As String
        Public Property Prodrate As String
        Public Property SIC As String

        Private _footNotes As New List(Of String)
        Public ReadOnly Property FootNotes As List(Of String)
            Get
                Return _footNotes
            End Get
        End Property
    End Class

End Namespace
