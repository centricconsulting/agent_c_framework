Imports PdfSharp.Pdf
Imports PdfSharp.Drawing
Imports MigraDoc
Imports MigraDoc.Rendering
Imports MigraDoc.RtfRendering
Imports MigraDoc.DocumentObjectModel
Imports System.Diagnostics
Imports System.IO
Imports System.Configuration.ConfigurationManager
Imports System.Reflection

Namespace IFM.VR.Common

    Public Module ThirdPartyReporting

#Region "Declarations"
        Dim qqxml As New QuickQuote.CommonMethods.QuickQuoteXML

        Public Enum CreditReportSubject
            Applicant
            PolicyHolder1
            PolicyHolder2
            Driver
        End Enum

        Public Enum ReportType
            CLUE_Auto
            Clue_Home
            Credit_Applicant
            Credit_Driver
            Credit_Policyholder1
            Credit_Policyholder2
            MVR_Driver
            HOM_DFR_PCC
        End Enum

        Private Enum PPCReportType_Enum
            ADDRESS
            ZIPCODE
            NOMATCH
        End Enum

        Private Structure HOM_CLUE_AddtionalInfo_structure
            Public Quoteback As String
            Public Account As String
            Public DateOfOrder As String
            Public DateOfReceipt As String
            Public RealPropertyRefNumber As String
            Public CompanyName As String
            Public blob As String
        End Structure

        ' Common PDFSharp/MigraDoc objects
        Private CurrentPage As PdfPage = Nothing
        Private doc As PdfDocument = Nothing
        Private gfx As XGraphics = Nothing
        Private sec As Section = Nothing
        Private docRenderer As MigraDoc.Rendering.DocumentRenderer = Nothing
        Private font As XFont = Nothing
        Private tempdoc As Document = Nothing
        Dim col As Tables.Column = Nothing
        Dim row As Tables.Row = Nothing
        Dim cell As Tables.Cell = Nothing

        ' Common variables
        Private CurrentPageNum As Integer = 0
        Private NextTopPos As Decimal = 0
        Private i As Integer = 0
        Private rowcount As Integer = 0
        Private txt As String = Nothing
        Private MaxBodyLength As Decimal = 9.0
        Private NoDataText As String = "** No Data Found **"

#End Region

#Region "Methods and Functions"

#Region "PRIVATE subs and functions"

        ''' <summary>
        ''' Cleans up all report files older than 24 hours
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CleanupReportFiles()
            Dim rfiles() As String = Nothing
            Dim path As String = AppSettings("QuickQuote_ThirdPartyData_SavePath")
            Dim fi As FileInfo = Nothing

            Try
                If Not System.IO.Directory.Exists(path) Then Exit Sub

                ' Get a list of all files in the third party report folder
                rfiles = System.IO.Directory.GetFiles(path)
                If rfiles.Count <= 0 Then Exit Sub

                ' Loop through the files, delete all older than 24 hours
                For Each f As String In rfiles
                    fi = New FileInfo(f)
                    If DateDiff(DateInterval.Hour, DateTime.Now, fi.CreationTime) <= -24 Then fi.Delete()
                Next

                Exit Sub
            Catch ex As Exception
                Exit Sub
            End Try
        End Sub

#End Region

#Region "PUBLIC FUNCTIONS"

        ''' <summary>
        ''' PUBLIC FUNCTION
        ''' Returns a Protection Class Code Report for the passed Address
        ''' </summary>
        ''' <param name="qo"></param>
        ''' <param name="err"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function PERSONAL_HOM_DFR_GetPCCReport(ByVal qo As QuickQuote.CommonObjects.QuickQuoteObject, ByRef err As String, Optional ByVal asByteArray As Boolean = False) As Object
            Dim PCCData As String = ""
            Dim PCCErr As String = ""
            Dim PCCReportData As Diamond.Common.Objects.ThirdParty.ReportObjects.ISOPPC.ISOPPCReportData
            Dim pdffile As Object = Nothing
            Dim results As String = Nothing

            Try
                ' Call the service to get the MVR data
                PCCReportData = qqxml.GetPPCReportDataForQuote(qo, 0, results, PCCErr)

                ' Got Data? Errors?
                If PCCErr Is Nothing OrElse PCCErr = "" Then
                    ' No errors, check return data
                    If PCCReportData Is Nothing OrElse PCCReportData.Equals(New Diamond.Common.Objects.ThirdParty.ReportObjects.MVR.MVRReportData()) Then
                        Throw New Exception("PCC Report Object is nothing or empty")
                    End If
                    'If PCCData Is Nothing OrElse PCCData = "" Then
                    '    'Throw New Exception("No MVR Data returned")
                    'End If
                Else
                    ' PCCErr has a value - an error occurred
                    Throw New Exception(PCCErr)
                End If

                '**This code will execute only of we found MVR data
                ' Format and create PDF file
                If asByteArray Then
                    'return as a byte stream
                    pdffile = CreatePDFReport_Byte(qo, ReportType.HOM_DFR_PCC, PCCData, err, PCCReportData)
                Else
                    'return the file path
                    pdffile = CreatePDFReport(qo, ReportType.HOM_DFR_PCC, PCCData, err, PCCReportData)
                End If

                If err <> "" Then Throw New Exception(err)

                Return pdffile
            Catch ex As Exception
                err = ex.Message
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' PUBLIC FUNCTION
        ''' Returns a Motor Vehicle Report for the passed quote and driver
        ''' </summary>
        ''' <param name="qo"></param>
        ''' <param name="DriverNum"></param>
        ''' <param name="err"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function PERSONAL_AUTO_GetMVRReport(ByVal qo As QuickQuote.CommonObjects.QuickQuoteObject, ByVal DriverNum As String, ByRef err As String, Optional ByVal asByteArray As Boolean = False) As Object
            Dim MVRData As String = ""
            Dim MVRErr As String = ""
            Dim MVRReportData As Diamond.Common.Objects.ThirdParty.ReportObjects.MVR.MVRReportData = Nothing
            Dim pdffile As Object = Nothing

            Try
                ' Call the service to get the MVR data
                MVRReportData = qqxml.GetMvrReportDataForDriver(qo, DriverNum, MVRData, MVRErr)

                ' Got Data? Errors?
                If MVRErr Is Nothing OrElse MVRErr = "" Then
                    ' No errors, check return data
                    If MVRReportData Is Nothing OrElse MVRReportData.Equals(New Diamond.Common.Objects.ThirdParty.ReportObjects.MVR.MVRReportData()) Then
                        Throw New Exception("MVR Report Object is nothing or empty")
                    End If
                    If MVRData Is Nothing OrElse MVRData = "" Then
                        'Throw New Exception("No MVR Data returned")
                    End If
                Else
                    ' MVRErr has a value - an error occurred
                    Throw New Exception(MVRErr)
                End If

                '**This code will execute only of we found MVR data
                ' Format and create PDF file
                If asByteArray Then
                    'return as a byte stream
                    pdffile = CreatePDFReport_Byte(qo, ReportType.MVR_Driver, MVRData, err, MVRReportData)
                Else
                    'return the file path
                    pdffile = CreatePDFReport(qo, ReportType.MVR_Driver, MVRData, err, MVRReportData)
                End If

                If err <> "" Then Throw New Exception(err)

                Return pdffile
            Catch ex As Exception
                err = ex.Message
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' PUBLIC FUNCTION
        ''' Returns a CLUE report (Auto)
        ''' </summary>
        ''' <param name="qo"></param>
        ''' <param name="err"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function PERSONAL_AUTO_GetCLUEReport(ByVal qo As QuickQuote.CommonObjects.QuickQuoteObject, ByRef err As String, Optional AsByteArray As Boolean = False) As Object
            Dim CLUEData As String = ""
            Dim CLUEErr As String = ""
            Dim CLUEObj As List(Of Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.ClueAutoReportData)
            Dim pdffile As Object = Nothing
            Dim pdffiles As New List(Of Object)
            Dim newPDFDocument As New PdfDocument()
            Dim inputDocument As New PdfDocument()
            Dim newfilename As String = ""
            Dim pdfStream As New System.IO.MemoryStream()

            Try
                ' Call the service to get the MVR data
                'CLUEObj = qqxml.GetClueAutoReportDataForQuote(qo, CLUEData, CLUEErr)

                ' This method can return multiple report objects - BUG 3457
                CLUEObj = qqxml.GetClueAutoReportDataObjectsForQuote(qo, CLUEData, CLUEErr)

                ' Got Data? Errors?
                If CLUEErr Is Nothing OrElse CLUEErr = "" Then
                    ' No errors, check return data
                    If CLUEObj Is Nothing OrElse CLUEObj.Equals(New Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.ClueAutoReportData()) Then
                        ' No data
                        Throw New Exception("CLUE Object is nothing or empty")
                    End If
                    If CLUEData Is Nothing OrElse CLUEData = "" Then
                        'Throw New Exception("No CLUE Data returned")
                    End If
                Else
                    ' CLUEErr has a value - an error occurred
                    Throw New Exception(CLUEErr)
                End If

                '**This code will execute only of we found CLUE data
                ' Create each individual report
                For Each objCLUE As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.ClueAutoReportData In CLUEObj
                    pdffile = CreatePDFReport(qo, ReportType.CLUE_Auto, CLUEData, err, objCLUE)

                    If err <> "" Then Throw New Exception(err)

                    pdffiles.Add(pdffile)
                Next

                Select Case pdffiles.Count
                    Case 0
                        Return Nothing
                    Case Else
                        ' Combine the files into a single document
                        For Each fn As String In pdffiles
                            inputDocument = New PdfDocument()
                            inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(fn, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import)

                            Dim count As Integer = inputDocument.PageCount
                            For i As Integer = 0 To count - 1
                                Dim page As PdfPage = inputDocument.Pages(i)
                                newPDFDocument.AddPage(page)
                            Next
                        Next

                        ' Save as byte array or string  BUG 3457 MGB
                        If AsByteArray Then
                            ' byte array
                            newPDFDocument.Save(pdfStream)
                            Return pdfStream.GetBuffer()
                        Else
                            ' file name
                            newfilename = pdffiles(0).ToString()
                            Dim ndx As Integer = newfilename.IndexOf(".pdf")
                            newfilename = newfilename.Substring(0, ndx - 1) & "_COMBINED.pdf"
                            newPDFDocument.Save(newfilename)
                            Return newfilename
                        End If
                        Exit Select
                End Select
            Catch ex As Exception
                err = ex.Message
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' PUBLIC FUNCTION
        ''' Returns a CLUE report (Personal Home)
        ''' </summary>
        ''' <param name="qo"></param>
        ''' <param name="err"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function PERSONAL_HOME_GetCLUEReport(ByVal qo As QuickQuote.CommonObjects.QuickQuoteObject, ByRef err As String, Optional AsByteArray As Boolean = False) As Object
            Dim CLUEData As String = ""
            Dim CLUEErr As String = ""
            Dim CLUEObj As List(Of Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalProperty.ReportData)
            'Dim CLUEObj As List(Of Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.ClueAutoReportData)
            Dim pdffile As Object = Nothing
            Dim pdffiles As New List(Of Object)
            Dim newPDFDocument As New PdfDocument()
            Dim inputDocument As New PdfDocument()
            Dim newfilename As String = ""
            Dim pdfStream As New System.IO.MemoryStream()

            Try
                ' Call the service to get the MVR data
                'CLUEObj = qqxml.GetClueAutoReportDataForQuote(qo, CLUEData, CLUEErr)

                ' This method can return multiple report objects - BUG 3457
                CLUEObj = qqxml.GetCluePropertyReportDataObjectsForQuote(qo, CLUEData, CLUEErr)

                ' Got Data? Errors?
                If CLUEErr Is Nothing OrElse CLUEErr = "" Then
                    ' No errors, check return data
                    If CLUEObj Is Nothing OrElse CLUEObj.Equals(New Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.ClueAutoReportData()) Then
                        ' No data
                        Throw New Exception("CLUE Object is nothing or empty")
                    End If
                    If CLUEData Is Nothing OrElse CLUEData = "" Then
                        'Throw New Exception("No CLUE Data returned")
                    End If
                Else
                    ' CLUEErr has a value - an error occurred
                    Throw New Exception(CLUEErr)
                End If

                '**This code will execute only of we found CLUE data
                ' Create each individual report
                For Each objCLUE As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalProperty.ReportData In CLUEObj
                    pdffile = CreatePDFReport(qo, ReportType.Clue_Home, CLUEData, err, objCLUE)

                    If err <> "" Then Throw New Exception(err)

                    pdffiles.Add(pdffile)
                Next

                Select Case pdffiles.Count
                    Case 0
                        Return Nothing
                    Case Else
                        ' Combine the files into a single document
                        For Each fn As String In pdffiles
                            inputDocument = New PdfDocument()
                            inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(fn, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import)

                            Dim count As Integer = inputDocument.PageCount
                            For i As Integer = 0 To count - 1
                                Dim page As PdfPage = inputDocument.Pages(i)
                                newPDFDocument.AddPage(page)
                            Next
                        Next

                        ' Save as byte array or string  BUG 3457 MGB
                        If AsByteArray Then
                            ' byte array
                            newPDFDocument.Save(pdfStream)
                            Return pdfStream.GetBuffer()
                        Else
                            ' file name
                            newfilename = pdffiles(0).ToString()
                            Dim ndx As Integer = newfilename.IndexOf(".pdf")
                            newfilename = newfilename.Substring(0, ndx - 1) & "_COMBINED.pdf"
                            newPDFDocument.Save(newfilename)
                            Return newfilename
                        End If
                        Exit Select
                End Select
            Catch ex As Exception
                err = ex.Message
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' PUBLIC FUNCTION
        ''' Returns a credit report
        ''' Select the subject, pass in a populated quote object, and pass in the subject number (driver number etc) if required
        ''' </summary>
        ''' <param name="Subject"></param>
        ''' <param name="qo"></param>
        ''' <param name="err"></param>
        ''' <param name="SubjectNum"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCreditReport(ByVal Subject As CreditReportSubject, ByVal qo As QuickQuote.CommonObjects.QuickQuoteObject, ByRef err As String, Optional SubjectNum As String = Nothing, Optional asByteArray As Boolean = False) As Object
            Dim CreditData As String = Nothing
            Dim CRErr As String = ""
            Dim CRObj As New Diamond.Common.Objects.ThirdParty.ReportObjects.NCF.RecordGroup()
            Dim pdffile As Object = "" '5-22-14 Matt changed type from String to Object
            Dim rt As ReportType = Nothing

            Try
                Select Case Subject
                    Case CreditReportSubject.Applicant
                        CRObj = qqxml.GetCreditReportDataForApplicant(qo, SubjectNum, CreditData, CRErr)
                        'CRObj = qqxml.GetCreditReportDataForPolicyholder1(qo, CreditData, CRErr)
                        rt = ReportType.Credit_Applicant
                        Exit Select
                    Case CreditReportSubject.PolicyHolder1
                        CRObj = qqxml.GetCreditReportDataForPolicyholder1(qo, CreditData, CRErr)
                        rt = ReportType.Credit_Policyholder1
                        Exit Select
                    Case CreditReportSubject.PolicyHolder2
                        CRObj = qqxml.GetCreditReportDataForPolicyholder2(qo, CreditData, CRErr)
                        rt = ReportType.Credit_Policyholder2
                        Exit Select
                    Case CreditReportSubject.Driver
                        CRObj = qqxml.GetCreditReportDataForDriver(qo, SubjectNum, CreditData, CRErr)
                        rt = ReportType.Credit_Driver
                End Select

                ' Got Data? Errors?
                If CRErr Is Nothing OrElse CRErr = "" Then
                    ' No errors, check return data
                    If CRObj Is Nothing OrElse CRObj.Equals(New Diamond.Common.Objects.ThirdParty.ReportObjects.NCF.RecordGroup()) Then
                        Throw New Exception("Credit Report object is nothing or empty")
                    End If
                    If CreditData Is Nothing OrElse CreditData = "" Then
                        If CRObj.General.Gmessage IsNot Nothing AndAlso CRObj.General.Gmessage <> "" Then
                            CreditData = CRObj.General.Gmessage
                        Else
                            CreditData = "No Credit Data Returned"
                        End If
                    End If
                Else
                    ' MVRErr has a value - an error occurred
                    Throw New Exception(CRErr)
                End If

                '**This code will execute only of we found data
                ' Format and create PDF file
                If asByteArray Then '5-22-14 Matt
                    'return as a byte stream
                    pdffile = CreatePDFReport_Byte(qo, rt, CreditData, err, CRObj)
                Else
                    'return the file path
                    pdffile = CreatePDFReport(qo, rt, CreditData, err, CRObj)
                End If

                If err <> "" Then Throw New Exception(err)

                Return pdffile
            Catch ex As Exception
                err = ex.Message
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Creates the requested third party report as a file.
        ''' Calls the Appropriate 'Generate Report' function
        ''' Returns the file path if success, nothing if fail
        ''' **TPObj = Third Party Object - like the Diamond report objects
        ''' </summary>
        ''' <param name="qo"></param>
        ''' <param name="WhichReport"></param>
        ''' <param name="reportdata"></param>
        ''' <param name="err"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CreatePDFReport(ByVal qo As QuickQuote.CommonObjects.QuickQuoteObject, WhichReport As ReportType, ByVal reportdata As String, ByRef err As String, Optional TPObj As Object = Nothing) As String
            'Dim path As String = AppSettings("QuickQuote_ThirdPartyReportsFolder")
            Dim path As String = AppSettings("QuickQuote_ThirdPartyData_SavePath")
            Dim filename As String = ""
            Dim fname As String = ""
            Dim pdffilepath As String = ""
            Dim localpath As String = ""
            Dim timestamp As String = DateTime.Now.Year.ToString() & DateTime.Now.Month.ToString.PadLeft(2, "0") _
                                      & DateTime.Now.Day.ToString.PadLeft(2, "0") & DateTime.Now.Hour.ToString.PadLeft(2, "0") _
                                      & DateTime.Now.Second.ToString.PadLeft(2, "0") & DateTime.Now.Millisecond.ToString()

            Try
                ' First clean up the report folder
                CleanupReportFiles()

                doc = New PdfDocument()

                localpath = System.IO.Path.GetDirectoryName(path)
                If localpath IsNot Nothing AndAlso localpath.Trim() <> "" Then path = localpath
                If path.Substring(path.Length - 1, 1) <> "\" Then path = path & "\"

                ' Set the document title and begin building the filename
                ' Also check that a valid Third Party report object was passed if required
                Select Case WhichReport
                    Case ReportType.CLUE_Auto
                        doc.Info.Title = "Velocirater AUTO Quote CLUE Report"
                        fname = "CLUE_"
                        filename = path & "CLUE_AUTO_"
                        Exit Select
                    Case ReportType.Clue_Home
                        doc.Info.Title = "Velocirater HOME Quote CLUE Report"
                        fname = "CLUE_"
                        filename = path & "CLUE_HOME_"
                        Exit Select
                    Case ReportType.Credit_Driver
                        doc.Info.Title = "Velocirater AUTO Driver Credit Report"
                        fname = "CREDIT_DRIVER_"
                        filename = path & "CREDIT_DRIVER_"
                        If TPObj Is Nothing Then Throw New Exception("CreatePDFReport: Credit object required")
                        Exit Select
                    Case ReportType.Credit_Applicant
                        doc.Info.Title = "Velocirater HOME Applicant Credit Report"
                        fname = "CREDIT_PH1_"
                        filename = path & "CREDIT_PH1_"
                        If TPObj Is Nothing Then Throw New Exception("CreatePDFReport: Credit object required")
                        Exit Select
                    Case ReportType.Credit_Policyholder1
                        doc.Info.Title = "Velocirater AUTO Policyholder 1 Credit Report"
                        fname = "CREDIT_PH1_"
                        filename = path & "CREDIT_PH1_"
                        If TPObj Is Nothing Then Throw New Exception("CreatePDFReport: Credit object required")
                        Exit Select
                    Case ReportType.Credit_Policyholder2
                        doc.Info.Title = "Velocirater AUTO Policyholder 2 Credit Report"
                        fname = "CREDIT_PH2_"
                        filename = path & "CREDIT_PH2_"
                        If TPObj Is Nothing Then Throw New Exception("CreatePDFReport: Credit object required")
                        Exit Select
                    Case ReportType.Credit_Applicant
                        doc.Info.Title = "Velocirater HOME Applicant Credit Report"
                        fname = "CREDIT_APPLICANT_"
                        filename = path & "CREDIT_APPLICANT_"
                        If TPObj Is Nothing Then Throw New Exception("CreatePDFReport: Credit object required")
                        Exit Select
                    Case ReportType.MVR_Driver
                        doc.Info.Title = "Velocirater AUTO Driver MVR Report"
                        fname = "MVR_DRIVER_"
                        filename = path & "MVR_DRIVER_"
                        If TPObj Is Nothing Then Throw New Exception("CreatePDFReport: MVR Object Required")
                        Exit Select
                    Case Else
                        Throw New Exception("Unknown report type passed")
                End Select

                'added 6/18/2019; updated 6/19/2019 to just use policy # since report may not even be for the image being viewed
                Dim quoteOrPolNum As String = ""
                If qo IsNot Nothing Then
                    'If qo.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse qo.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    '    If String.IsNullOrWhiteSpace(qo.PolicyNumber) = False AndAlso Left(UCase(qo.PolicyNumber), 1) = "Q" AndAlso String.IsNullOrWhiteSpace(qo.QuoteNumber) = False Then
                    '        quoteOrPolNum = qo.QuoteNumber
                    '    End If
                    'Else
                    '    quoteOrPolNum = qo.QuoteNumber
                    'End If
                    'If String.IsNullOrWhiteSpace(quoteOrPolNum) = True Then
                    quoteOrPolNum = qo.PolicyNumber
                    'End If
                End If

                ' Finish building the file name
                'fname = fname & qo.QuoteNumber & "_" & timestamp & ".pdf"
                'updated 6/18/2019
                fname = fname & quoteOrPolNum & "_" & timestamp & ".pdf"
                filename = System.IO.Path.Combine(path, fname)

                ' Append the quote number to the internal document title
                'doc.Info.Title = doc.Info.Title & " (" & qo.QuoteNumber & ")"
                'updated 6/18/2019
                doc.Info.Title = doc.Info.Title & " (" & quoteOrPolNum & ")"

                ' Set the other info attributes for the PDF
                doc.Info.Author = "Indiana Farmers Mutual Insurance - Velocirater Quoting Tool"
                doc.Info.Subject = "Third Party Reporting"
                doc.Info.Keywords = ""

                ' THIS IS WHERE THE WORK IS DONE
                Select Case WhichReport
                    Case ReportType.CLUE_Auto
                        Generate_CLUE_Report_PPA(qo, reportdata, TPObj, err)
                        Exit Select
                    Case ReportType.Clue_Home
                        Generate_CLUE_Report_HOM(qo, reportdata, TPObj, err)
                        Exit Select
                    Case ReportType.Credit_Driver
                        Generate_QUOTE_CREDIT_Report(qo, reportdata, TPObj, err)
                        Exit Select
                    Case ReportType.Credit_Applicant
                        Generate_QUOTE_CREDIT_Report(qo, reportdata, TPObj, err)
                        Exit Select
                    Case ReportType.Credit_Policyholder1
                        Generate_QUOTE_CREDIT_Report(qo, reportdata, TPObj, err)
                        Exit Select
                    Case ReportType.Credit_Policyholder2
                        Generate_QUOTE_CREDIT_Report(qo, reportdata, TPObj, err)
                        Exit Select
                    Case ReportType.Credit_Applicant
                        Generate_QUOTE_CREDIT_Report(qo, reportdata, TPObj, err)
                        Exit Select
                    Case ReportType.MVR_Driver
                        Generate_DRIVER_MVR_Report(qo, reportdata, TPObj, err)
                        Exit Select
                    Case Else
                        Throw New Exception("Unknown report type passed")
                End Select

                If err <> "" Then Throw New Exception(err)

                doc.Save(filename)

                Return filename
            Catch ex As Exception
                err = ex.Message
                Return Nothing
            Finally
                If doc IsNot Nothing Then doc.Dispose()
            End Try
        End Function

        '5-22-14 Matt - I hated to add this but want to try a file-less method
        ''' <summary>
        ''' Creates the requested third party report as a stream.
        ''' Calls the Appropriate 'Generate Report' function
        ''' Returns the pdf file stream, nothing if fail
        ''' **TPObj = Third Party Object - like the Diamond report objects
        ''' </summary>
        ''' <param name="qo"></param>
        ''' <param name="WhichReport"></param>
        ''' <param name="reportdata"></param>
        ''' <param name="err"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CreatePDFReport_Byte(ByVal qo As QuickQuote.CommonObjects.QuickQuoteObject, WhichReport As ReportType, ByVal reportdata As String, ByRef err As String, Optional TPObj As Object = Nothing) As Byte()
            Dim pdfStream As New System.IO.MemoryStream()

            Try
                ' Initialize the document object
                doc = New PdfDocument()

                ' First clean up the report folder
                CleanupReportFiles()

                ' Set the document title and begin building the filename
                ' Also check that a valid Third Party report object was passed if required
                Select Case WhichReport
                    Case ReportType.CLUE_Auto
                        doc.Info.Title = "Velocirater Auto Quote CLUE Report"
                        Exit Select
                    Case ReportType.Clue_Home
                        doc.Info.Title = "Velocirater Home Quote CLUE Report"
                        Exit Select
                    Case ReportType.Credit_Driver
                        doc.Info.Title = "Velocirater Driver Credit Report"
                        If TPObj Is Nothing Then Throw New Exception("CreatePDFReport: Credit object required")
                        Exit Select
                    Case ReportType.Credit_Policyholder1
                        doc.Info.Title = "Velocirater Policyholder 1 Credit Report"
                        If TPObj Is Nothing Then Throw New Exception("CreatePDFReport: Credit object required")
                        Exit Select
                    Case ReportType.Credit_Policyholder2
                        doc.Info.Title = "Velocirater Policyholder 2 Credit Report"
                        If TPObj Is Nothing Then Throw New Exception("CreatePDFReport: Credit object required")
                        Exit Select
                    Case ReportType.Credit_Applicant
                        doc.Info.Title = "Velocirater Applicant Credit Report"
                        If TPObj Is Nothing Then Throw New Exception("CreatePDFReport: Credit object required")
                        Exit Select
                    Case ReportType.MVR_Driver
                        doc.Info.Title = "Velocirater Driver MVR Report"
                        If TPObj Is Nothing Then Throw New Exception("CreatePDFReport: MVR Object Required")
                        Exit Select
                    Case ReportType.HOM_DFR_PCC
                        doc.Info.Title = "Velocirater Public Protection Class Report"
                        If TPObj Is Nothing Then Throw New Exception("CreatePDFReport: PCC Object Required")
                        Exit Select
                    Case Else
                        Throw New Exception("Unknown report type passed")
                End Select

                'added 6/18/2019; updated 6/19/2019 to just use policy # since report may not even be for the image being viewed
                Dim quoteOrPolNum As String = ""
                If qo IsNot Nothing Then
                    'If qo.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse qo.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                    '    If String.IsNullOrWhiteSpace(qo.PolicyNumber) = False AndAlso Left(UCase(qo.PolicyNumber), 1) = "Q" AndAlso String.IsNullOrWhiteSpace(qo.QuoteNumber) = False Then
                    '        quoteOrPolNum = qo.QuoteNumber
                    '    End If
                    'Else
                    '    quoteOrPolNum = qo.QuoteNumber
                    'End If
                    'If String.IsNullOrWhiteSpace(quoteOrPolNum) = True Then
                    quoteOrPolNum = qo.PolicyNumber
                    'End If
                End If

                ' Append the quote number to the internal document title
                'doc.Info.Title = doc.Info.Title & " (" & qo.QuoteNumber & ")"
                'updated 6/18/2019
                doc.Info.Title = doc.Info.Title & " (" & quoteOrPolNum & ")"

                ' Set the other info attributes for the PDF
                doc.Info.Author = "Indiana Farmers Mutual Insurance - Velocirater Quoting Tool"
                doc.Info.Subject = "Third Party Reporting"
                doc.Info.Keywords = ""

                ' THIS IS WHERE THE WORK IS DONE
                Select Case WhichReport
                    Case ReportType.CLUE_Auto
                        Generate_CLUE_Report_PPA(qo, reportdata, TPObj, err)
                        Exit Select
                    Case ReportType.Clue_Home
                        Generate_CLUE_Report_HOM(qo, reportdata, TPObj, err)
                        Exit Select
                    Case ReportType.Credit_Driver
                        Generate_QUOTE_CREDIT_Report(qo, reportdata, TPObj, err)
                        Exit Select
                    Case ReportType.Credit_Policyholder1
                        Generate_QUOTE_CREDIT_Report(qo, reportdata, TPObj, err)
                        Exit Select
                    Case ReportType.Credit_Policyholder2
                        Generate_QUOTE_CREDIT_Report(qo, reportdata, TPObj, err)
                        Exit Select
                    Case ReportType.Credit_Applicant
                        Generate_QUOTE_CREDIT_Report(qo, reportdata, TPObj, err)
                        Exit Select
                    Case ReportType.MVR_Driver
                        Generate_DRIVER_MVR_Report(qo, reportdata, TPObj, err)
                        Exit Select
                    Case ReportType.HOM_DFR_PCC
                        Generate_HOM_DFR_PCC_Report(qo, reportdata, TPObj, err)
                        Exit Select
                    Case Else
                        Throw New Exception("Unknown report type passed")
                End Select

                If err <> "" Then Throw New Exception(err)

                doc.Save(pdfStream)

                Return pdfStream.GetBuffer()
            Catch ex As Exception
                err = ex.Message
                Return Nothing
            Finally
                If doc IsNot Nothing Then doc.Dispose()
            End Try
        End Function

        ''' <summary>
        ''' Parses the Additonal Info 'DataLine' field into it's individual values. HOM PERSONAL CLUE
        ''' </summary>
        ''' <param name="addlinfo"></param>
        ''' <param name="err"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Parse_HOM_CLUE_AdditionalInfo(ByVal addlinfo As String, ByRef err As String) As HOM_CLUE_AddtionalInfo_structure
            Dim AI As New HOM_CLUE_AddtionalInfo_structure()
            Dim parts() As String = Nothing
            Dim CleanParts As New List(Of String)
            Dim newstr As String = Nothing
            Dim holdstr As String = Nothing
            Dim ndx As Integer = -1
            Dim ndx2 As Integer = -1
            Dim LastInfoLine As Integer = -1

            Try
                parts = addlinfo.Split(vbCrLf)
                If parts.Count <= 1 Then Throw New Exception("Unable to parse additional Info")

                ' Find the last info section line
                For i As Integer = 0 To parts.Count - 1
                    Dim p As String = parts(i)
                    If p.Contains("----------") Then
                        LastInfoLine = i - 1
                        Exit For
                    End If
                Next
                If LastInfoLine < 0 Then Throw New Exception("Unable to determine last info line")

                ' Remove all white space and line feeds
                For z As Integer = 0 To parts.Length - 1
                    Dim p As String = parts(z)
                    newstr = ""
                    ' Do not remove formatting after the last info line
                    If z <= LastInfoLine Then
                        holdstr = p.Trim()
                    Else
                        holdstr = p
                    End If

                    If holdstr.Length > 0 Then
                        For i As Integer = 0 To holdstr.Length - 1
                            If holdstr.Substring(i, 1) <> vbCrLf AndAlso holdstr.Substring(i, 1) <> vbCr AndAlso holdstr.Substring(i, 1) <> vbLf Then
                                newstr = newstr & holdstr.Substring(i, 1)
                            End If
                        Next
                        CleanParts.Add(newstr)
                    End If
                Next

                ' Parse the data
                For i As Integer = 0 To LastInfoLine
                    Select Case i
                        Case 0  ' Quoteback line
                            AI.Quoteback = CleanParts(i).Substring(10).Trim()
                        Case 1  ' Account/Date of Order
                            ndx = CleanParts(i).IndexOf("Account:")
                            If ndx >= 0 Then
                                ndx2 = CleanParts(i).IndexOf("Date of Order:")
                                If ndx2 >= 0 Then
                                    AI.Account = CleanParts(i).Substring(ndx + 8, (ndx2 - 8) - 1).Trim()
                                    AI.DateOfOrder = CleanParts(i).Substring(ndx2 + 14).Trim()
                                Else
                                    AI.Account = CleanParts(i).Substring(ndx + 8).Trim()
                                    AI.DateOfOrder = ""
                                End If
                            End If
                        Case 2
                            ' Date of receipt is the second part of the string
                            ndx = CleanParts(i).IndexOf("Date of Receipt:")
                            If ndx >= 0 Then
                                AI.DateOfReceipt = CleanParts(i).Substring(ndx + 16).Trim()
                            End If
                            If ndx > 0 Then
                                AI.CompanyName = CleanParts(i).Substring(0, ndx - 1).Trim()
                            End If
                        Case 3  ' Real Property Reference #
                            ndx = CleanParts(i).IndexOf("Real Property Ref. #:")
                            If ndx >= 0 Then
                                AI.RealPropertyRefNumber = CleanParts(i).Substring(ndx + 21).Trim()
                            End If
                    End Select
                Next

                AI.blob = ""
                For i As Integer = LastInfoLine - 1 To CleanParts.Count - 1
                    AI.blob = AI.blob + CleanParts(i)
                    If i < CleanParts.Count - 1 Then AI.blob = AI.blob & vbCrLf
                Next

                Return AI
            Catch ex As Exception
                err = ex.Message
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Removes the phone number from the HOM CLUE SEARCH subjects
        ''' </summary>
        ''' <param name="txt"></param>
        ''' <param name="err"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function HOM_CLUE_Remove_PhoneNumberFromText(ByVal txt As String, ByRef err As String) As String
            Dim ndx As Integer = -1
            Try
                If txt.Trim = String.Empty Then Return txt
                ndx = txt.IndexOf("Telephone:")
                If ndx >= 0 Then
                    Return txt.Substring(0, ndx - 1)
                Else
                    Return txt
                End If
            Catch ex As Exception
                err = ex.Message
                Return txt
            End Try
        End Function

        ''' <summary>
        ''' Removes text from the Claim Hostory - Risk data
        ''' 1. Mortgagee and Loan#
        ''' </summary>
        ''' <param name="txt"></param>
        ''' <param name="err"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function HOM_CLUE_EditDataLine(ByVal txt As String, ByRef err As String) As String
            Dim ndx As Integer = -1

            Try
                If txt.Trim = String.Empty Then Return txt

                ' Remove the Mortgagee and Loan# fields from the text blob.
                ' They are the last 2 fields in the order of Mortgagee, Loan#
                ndx = txt.IndexOf("Mortgagee:")
                If ndx >= 0 Then
                    Return txt.Substring(0, ndx - 1)
                Else
                    Return txt
                End If
            Catch ex As Exception
                err = ex.Message
                Return txt
            End Try
        End Function

#End Region

#Region "GENERATE methods"

        ''' <summary>
        ''' Generate HOM/DFR PCC report pdf
        ''' </summary>
        ''' <param name="qo"></param>
        ''' <param name="reportdata"></param>
        ''' <param name="PCCObj"></param>
        ''' <param name="err"></param>
        ''' <remarks></remarks>
        Private Sub Generate_HOM_DFR_PCC_Report(ByVal qo As QuickQuote.CommonObjects.QuickQuoteObject, ByVal reportdata As String, ByVal PCCObj As Diamond.Common.Objects.ThirdParty.ReportObjects.ISOPPC.ISOPPCReportData, ByRef err As String)
            Dim page As PdfPage = Nothing
            Dim NumDataLines As Integer = 0

            ' Paragraphs
            Dim paraTITLE As Paragraph = Nothing
            Dim paraRESULTSHEADER As Paragraph = Nothing
            Dim paraFOOTERLeft As New Paragraph()
            Dim paraFOOTERRight As New Paragraph()
            Dim RIGHT_FOOTERS As New List(Of Paragraph)

            ' Tables
            Dim tblREQUEST As Tables.Table = Nothing   ' Request Info table - Four Columns
            Dim tblREQUEST2 As Tables.Table = Nothing   ' Request Info table - Two Columns
            Dim tblRESULTS As Tables.Table = Nothing    ' Results Info table
            Dim tblDETAILRESULT As Tables.Table = Nothing ' Detail results table
            Dim DETAILRESULTSTABLES As New List(Of Tables.Table) ' All detail results tables
            Dim tblCurrent As Tables.Table = Nothing

            Dim rowheight As MigraDoc.DocumentObjectModel.Unit = Unit.FromInch(0.33)
            Dim rowheight_slim As MigraDoc.DocumentObjectModel.Unit = Unit.FromInch(0.2)
            Dim DetailSectionTotalHeight As Integer = 0
            Dim DetailSectionHeightEach As Integer = 1
            Dim PageCount As Integer = 0
            Dim DetailCount As Integer = 0
            Dim DetailRowsPerPage_FirstPage As Integer = 4
            Dim DetailRowsPerPage_AfterFirstPage As Integer = 5
            Dim PPCReportType As PPCReportType_Enum = Nothing

            Try
                ' There are three distinct report layouts depending on what Return Source (match type)
                ' comes back from PPC:
                '   * NO MATCH - Only the request data section is printed
                '   * ADDRESS MATCH - Single page REQUEST/RESPONSE report
                '   * ZIP CODE LEVEL MATCH - Dynamic report with detail for each PPC in the zip code.
                ' No other Return Sources will be recognized.

                ' Set the report type
                If PCCObj.ReturnSource IsNot Nothing AndAlso PCCObj.ReturnSource.Trim <> String.Empty Then
                    Select Case PCCObj.ReturnSource.ToUpper.Trim
                        Case "ADDRESS"
                            PPCReportType = PPCReportType_Enum.ADDRESS
                            Exit Select
                        Case "ZIP CODE LEVEL MATCH"
                            PPCReportType = PPCReportType_Enum.ZIPCODE
                            Exit Select
                        Case "NO MATCHES"
                            PPCReportType = PPCReportType_Enum.NOMATCH
                            Exit Select
                        Case Else
                            Throw New Exception("Unsupported Return Source value: '" & PCCObj.ReturnSource)
                    End Select
                Else
                    Throw New Exception("PPC Return Source is null or empty!  Unable to create report.")
                End If

                ' Set up the page & graphics adapter
                page = doc.AddPage()
                gfx = XGraphics.FromPdfPage(page)
                gfx.MUH = PdfFontEncoding.Unicode
                gfx.MFEH = PdfFontEmbedding.Default
                font = New XFont("Verdana", 13, XFontStyle.Bold)

                ' Set up the document Body
                tempdoc = New Document()
                sec = tempdoc.AddSection()
                sec.PageSetup.StartingNumber = 1

                ' *********************
                ' TITLE SECTION
                ' *********************
                paraTITLE = sec.AddParagraph("LOCATION PPC ORDER")
                paraTITLE.Format.Font.Name = "Verdana"
                paraTITLE.Format.Font.Size = 12
                paraTITLE.Format.Font.Bold = True
                paraTITLE.Format.Alignment = ParagraphAlignment.Center
                paraTITLE.Format.Borders.Width = 0.5
                paraTITLE.Format.Borders.Color = Colors.Black
                paraTITLE.Format.Borders.Style = BorderStyle.Single

                ' *********************
                ' REQUEST DATA SECTION 1
                ' *********************
                tblREQUEST = New Tables.Table
                tblREQUEST.Format.Alignment = ParagraphAlignment.Center
                tblREQUEST.Format.Font.Name = "Verdana"
                tblREQUEST.Format.Font.Size = 9

                ' Column 0 (label column 1)
                col = tblREQUEST.AddColumn(Unit.FromInch(2))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Font.Bold = True
                col.Format.Font.Italic = True
                ' column 1 (data column 1)
                col = tblREQUEST.AddColumn(Unit.FromInch(2))
                col.Format.Alignment = ParagraphAlignment.Left
                ' Column 2 (label column 2)
                col = tblREQUEST.AddColumn(Unit.FromInch(2))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Font.Bold = True
                col.Format.Font.Italic = True
                ' column 3 (data column 2)
                col = tblREQUEST.AddColumn(Unit.FromInch(2))
                col.Format.Alignment = ParagraphAlignment.Left

                ' FIRST ROW - Order Date, Response Date
                row = tblREQUEST.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                row.Height = rowheight
                cell = row.Cells(0)
                cell.AddParagraph("Order Date:")
                cell = row.Cells(1)
                cell.AddParagraph(PCCObj.OrderDate)  ' Order Date
                cell = row.Cells(2)
                cell.AddParagraph("Response Date:")
                cell = row.Cells(3)
                cell.AddParagraph(PCCObj.ResponseDate)   ' Response Date
                ' SECOND ROW - Order Time, Response Time
                row = tblREQUEST.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                row.Height = rowheight
                cell = row.Cells(0)
                cell.AddParagraph("Order Time:")
                cell = row.Cells(1)
                cell.AddParagraph(PCCObj.OrderTime)  ' Order Time
                cell = row.Cells(2)
                cell.AddParagraph("Response Time:")
                cell = row.Cells(3)
                cell.AddParagraph(PCCObj.ResponseTime)   ' Response Time

                tblREQUEST.SetEdge(0, 0, 4, 2, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                sec.Add(tblREQUEST)

                ' *********************
                ' REQUEST DATA SECTION 2
                ' *********************
                tblREQUEST2 = New Tables.Table
                tblREQUEST2.Format.Alignment = ParagraphAlignment.Center
                tblREQUEST2.Format.Font.Name = "Verdana"
                tblREQUEST2.Format.Font.Size = 9

                ' Column 0 (label column 1)
                col = tblREQUEST2.AddColumn(Unit.FromInch(2))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Font.Bold = True
                col.Format.Font.Italic = True
                ' column 1 (data column 1)
                col = tblREQUEST2.AddColumn(Unit.FromInch(6))
                col.Format.Alignment = ParagraphAlignment.Left

                ' FIRST ROW Row - Risk Address
                row = tblREQUEST2.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                row.Height = rowheight
                cell = row.Cells(0)
                cell.AddParagraph("Risk Address:")
                cell = row.Cells(1)
                cell.AddParagraph(PCCObj.RiskAddress)  ' Risk Address
                ' SECOND ROW - Return Source
                row = tblREQUEST2.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                row.Height = rowheight
                cell = row.Cells(0)
                cell.AddParagraph("Return Source:")
                cell = row.Cells(1)
                cell.AddParagraph(PCCObj.ReturnSource)  ' Return Source

                tblREQUEST2.SetEdge(0, 0, 2, 2, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                sec.Add(tblREQUEST2)

                '****************************
                ' RESULTS HEADER SECTION
                '****************************
                paraRESULTSHEADER = sec.AddParagraph("RESULTS")
                paraRESULTSHEADER.Format.Font.Name = "Verdana"
                paraRESULTSHEADER.Format.Font.Size = 12
                paraRESULTSHEADER.Format.Font.Bold = True
                paraRESULTSHEADER.Format.Alignment = ParagraphAlignment.Center
                paraRESULTSHEADER.Format.Borders.Width = 0.5
                paraRESULTSHEADER.Format.Borders.Color = Colors.Black
                paraRESULTSHEADER.Format.Borders.Style = BorderStyle.Single

                '***************************************
                ' RESULTS DATA SECTION - ADDRESS MATCH
                '***************************************
                tblRESULTS = New Tables.Table
                tblRESULTS.Format.Alignment = ParagraphAlignment.Center
                tblRESULTS.Format.Font.Name = "Verdana"
                tblRESULTS.Format.Font.Size = 9

                ' Column 0 (label column)
                col = tblRESULTS.AddColumn(Unit.FromInch(4))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Font.Bold = True
                col.Format.Font.Italic = True

                ' column 1 (data column)
                col = tblRESULTS.AddColumn(Unit.FromInch(4))
                col.Format.Alignment = ParagraphAlignment.Left

                Select Case PPCReportType
                    Case PPCReportType_Enum.ADDRESS
                        ' FIRST ROW - Public Protection Class
                        row = tblRESULTS.AddRow()
                        row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        row.Height = rowheight
                        cell = row.Cells(0)
                        cell.AddParagraph("Public Protection Class at Risk:")
                        cell = row.Cells(1)
                        cell.AddParagraph(PCCObj.PublicProtectionClass)  ' Protection Class

                        ' SECOND ROW - Alternative PPCs
                        row = tblRESULTS.AddRow()
                        row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        row.Height = rowheight
                        cell = row.Cells(0)
                        cell.AddParagraph("Alternative PPC(s):")
                        cell = row.Cells(1)
                        cell.AddParagraph(PCCObj.AlternativePPC)  ' Alternative PPC

                        ' THIRD ROW - Fire Protection Area
                        row = tblRESULTS.AddRow()
                        row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        row.Height = rowheight
                        cell = row.Cells(0)
                        cell.AddParagraph("Fire Protection Area:")
                        cell = row.Cells(1)
                        cell.AddParagraph(PCCObj.FireProtectionArea)  ' Fire Protection Area

                        ' FOURTH ROW - Driving Distance to FD
                        row = tblRESULTS.AddRow()
                        row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        row.Height = rowheight
                        cell = row.Cells(0)
                        cell.AddParagraph("Driving Distance to Responding Fire Station:")
                        cell = row.Cells(1)
                        cell.AddParagraph(PCCObj.DrivingDistance)  ' Driving Distance to FD

                        ' FIFTH ROW - Driving Distance to FD
                        row = tblRESULTS.AddRow()
                        row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        row.Height = rowheight
                        cell = row.Cells(0)
                        cell.AddParagraph("Responding Fire Station:")
                        cell = row.Cells(1)
                        cell.AddParagraph(PCCObj.RespondingFireStation)  ' Responding Fire Station

                        ' SIXTH ROW - Statistical Placement Indicator
                        row = tblRESULTS.AddRow()
                        row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        row.Height = rowheight
                        cell = row.Cells(0)
                        cell.AddParagraph("Statistical Placement Indicator:")
                        cell = row.Cells(1)
                        cell.AddParagraph(PCCObj.StatisticalPlacementIndicator)  ' Statistical Placement Indicator

                        ' SEVENTH ROW - Subscription Indicator
                        row = tblRESULTS.AddRow()
                        row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        row.Height = rowheight
                        cell = row.Cells(0)
                        cell.AddParagraph("Subscription Indicator:")
                        cell = row.Cells(1)
                        cell.AddParagraph(PCCObj.SubscriptionIndicator)  ' Subscription Indicator

                        tblRESULTS.SetEdge(0, 0, 2, 7, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                        sec.Add(tblRESULTS)
                        Exit Select
                    Case PPCReportType_Enum.ZIPCODE
                        ' ** First two rows are the non-repeating header section TBLRESULTS
                        ' FIRST ROW - Predominant PPC
                        row = tblRESULTS.AddRow()
                        row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        row.Height = rowheight
                        cell = row.Cells(0)
                        cell.AddParagraph("Predominant PPC:")
                        cell = row.Cells(1)
                        cell.AddParagraph(PCCObj.PredominantPPC)

                        ' SECOND ROW - Predominant PPC Percent
                        row = tblRESULTS.AddRow()
                        row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        row.Height = rowheight
                        cell = row.Cells(0)
                        cell.AddParagraph("Percent:")
                        cell = row.Cells(1)
                        If IsNumeric(PCCObj.PredominantPPCPercentage) Then
                            cell.AddParagraph(FormatPercent(PCCObj.PredominantPPCPercentage, 2))
                        Else
                            cell.AddParagraph(PCCObj.PredominantPPCPercentage)
                        End If

                        tblRESULTS.SetEdge(0, 0, 2, 2, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                        sec.Add(tblRESULTS)

                        ' ** This section repeats for each PPC record
                        For Each rec As Diamond.Common.Objects.ThirdParty.ReportObjects.ISOPPC.ZipRecord In PCCObj.ZipRecords
                            DetailCount += 1

                            ' Create First page table
                            If DetailCount = 1 Then
                                tblDETAILRESULT = New Tables.Table
                                tblDETAILRESULT.Format.Alignment = ParagraphAlignment.Center
                                tblDETAILRESULT.Format.Font.Name = "Verdana"
                                tblDETAILRESULT.Format.Font.Size = 9

                                ' Column 0 (label column)
                                col = tblDETAILRESULT.AddColumn(Unit.FromInch(4))
                                col.Format.Alignment = ParagraphAlignment.Left
                                col.Format.Font.Bold = True
                                col.Format.Font.Italic = True

                                ' column 1 (data column)
                                col = tblDETAILRESULT.AddColumn(Unit.FromInch(4))
                                col.Format.Alignment = ParagraphAlignment.Left

                                tblCurrent = tblDETAILRESULT
                            End If

                            ' FIRST DETAIL ROW - PPC
                            row = tblCurrent.AddRow()
                            row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                            row.Height = rowheight
                            cell = row.Cells(0)
                            cell.AddParagraph("PPC:")
                            cell = row.Cells(1)
                            cell.AddParagraph(rec.PPCVal)

                            ' SECOND DETAIL ROW - Percent
                            row = tblCurrent.AddRow()
                            row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                            row.Height = rowheight
                            cell = row.Cells(0)
                            cell.AddParagraph("Percent:")
                            cell = row.Cells(1)
                            If IsNumeric(rec.PPCPercentage) Then
                                cell.AddParagraph(FormatPercent(rec.PPCPercentage, 2))
                            Else
                                cell.AddParagraph(rec.PPCPercentage)
                            End If

                            ' THIRD DETAIL ROW - Fire Protection Area
                            row = tblCurrent.AddRow()
                            row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                            row.Height = rowheight
                            cell = row.Cells(0)
                            cell.AddParagraph("Fire Protection Area:")
                            cell = row.Cells(1)
                            cell.AddParagraph(rec.FireDistrictName)

                            ' FOURTH DETAIL ROW - County
                            row = tblCurrent.AddRow()
                            row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                            row.Height = rowheight
                            cell = row.Cells(0)
                            cell.AddParagraph("County:")
                            cell = row.Cells(1)
                            cell.AddParagraph(rec.PPCCountyName)

                            ' FIFTH DETAIL ROW - Empty Row
                            row = tblCurrent.AddRow()
                            row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                            row.Height = rowheight
                            cell = row.Cells(0)
                            cell.AddParagraph("")
                            cell = row.Cells(1)
                            cell.AddParagraph("")

                            If PageCount = 0 Then
                                If DetailCount = DetailRowsPerPage_FirstPage Then
                                    PageCount += 1
                                    tblCurrent.SetEdge(0, 0, 2, DetailRowsPerPage_FirstPage, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                                    DETAILRESULTSTABLES.Add(tblCurrent)

                                    tblDETAILRESULT = New Tables.Table
                                    tblDETAILRESULT.Format.Alignment = ParagraphAlignment.Center
                                    tblDETAILRESULT.Format.Font.Name = "Verdana"
                                    tblDETAILRESULT.Format.Font.Size = 9

                                    ' Column 0 (label column)
                                    col = tblDETAILRESULT.AddColumn(Unit.FromInch(4))
                                    col.Format.Alignment = ParagraphAlignment.Left
                                    col.Format.Font.Bold = True
                                    col.Format.Font.Italic = True

                                    ' column 1 (data column)
                                    col = tblDETAILRESULT.AddColumn(Unit.FromInch(4))
                                    col.Format.Alignment = ParagraphAlignment.Left

                                    tblCurrent = tblDETAILRESULT

                                    DetailCount = 0
                                End If
                            Else
                                If DetailCount = DetailRowsPerPage_AfterFirstPage Then
                                    PageCount += 1
                                    tblCurrent.SetEdge(0, 0, 2, DetailRowsPerPage_AfterFirstPage, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                                    DETAILRESULTSTABLES.Add(tblCurrent)

                                    tblDETAILRESULT = New Tables.Table
                                    tblDETAILRESULT.Format.Alignment = ParagraphAlignment.Center
                                    tblDETAILRESULT.Format.Font.Name = "Verdana"
                                    tblDETAILRESULT.Format.Font.Size = 9

                                    ' Column 0 (label column)
                                    col = tblDETAILRESULT.AddColumn(Unit.FromInch(4))
                                    col.Format.Alignment = ParagraphAlignment.Left
                                    col.Format.Font.Bold = True
                                    col.Format.Font.Italic = True

                                    ' column 1 (data column)
                                    col = tblDETAILRESULT.AddColumn(Unit.FromInch(4))
                                    col.Format.Alignment = ParagraphAlignment.Left

                                    tblCurrent = tblDETAILRESULT

                                    DetailCount = 0
                                End If
                            End If
                        Next

                        For Each t As Tables.Table In DETAILRESULTSTABLES
                            sec.Add(t)
                        Next

                        Exit Select
                    Case PPCReportType_Enum.NOMATCH
                        ' FIRST ROW - Empty Row
                        row = tblRESULTS.AddRow()
                        row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        row.Height = rowheight
                        cell = row.Cells(0)
                        cell.AddParagraph("")
                        cell = row.Cells(1)
                        cell.AddParagraph("")

                        tblRESULTS.SetEdge(0, 0, 2, 1, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                        sec.Add(tblRESULTS)
                        Exit Select
                End Select

                ' Left FOOTER - Always the same
                paraFOOTERLeft = sec.AddParagraph()
                paraFOOTERLeft.AddText(DateTime.Now.ToLongDateString)
                paraFOOTERLeft.Format.Font.Name = "Verdana"
                paraFOOTERRight.Format.Font.Size = 9
                paraFOOTERLeft.Format.Alignment = ParagraphAlignment.Left

                Select Case PPCReportType
                    Case PPCReportType_Enum.ADDRESS, PPCReportType_Enum.NOMATCH
                        ' Right Footer - only one page
                        paraFOOTERRight = sec.AddParagraph()
                        paraFOOTERRight.AddText("Page 1 of 1")
                        paraFOOTERRight.Format.Font.Name = "Verdana"
                        paraFOOTERRight.Format.Font.Size = 9
                        paraFOOTERRight.Format.Alignment = ParagraphAlignment.Right
                        Exit Select
                    Case PPCReportType_Enum.ZIPCODE
                        ' Right footer - changes on each page
                        Dim ParaFOOTER_Right As New Paragraph
                        For i As Integer = 1 To PageCount
                            ParaFOOTER_Right = Nothing
                            ParaFOOTER_Right = sec.AddParagraph()
                            ParaFOOTER_Right.AddText("Page " & i & " of " & PageCount)
                            ParaFOOTER_Right.Format.Font.Name = "Verdana"
                            ParaFOOTER_Right.Format.Font.Size = 9
                            ParaFOOTER_Right.Format.Alignment = ParagraphAlignment.Right
                            RIGHT_FOOTERS.Add(ParaFOOTER_Right)
                        Next
                        Exit Select
                End Select

                ' ADD ALL THE FORMATTED OBJECTS TO THE DOCUMENT
                docRenderer = New Rendering.DocumentRenderer(tempdoc)
                docRenderer.PrepareDocument()

                ' ****************
                ' RENDERING
                ' ****************
                ' Render the objects. You can render tables, shapes etc the same way.
                ' Common to all PPC reports: Title, Request Data
                NextTopPos = 0.25
                docRenderer.RenderObject(gfx, XUnit.FromInch(1.25), XUnit.FromInch(NextTopPos), XUnit.FromInch(6), paraTITLE)
                NextTopPos = NextTopPos + 0.25
                docRenderer.RenderObject(gfx, XUnit.FromInch(1), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), tblREQUEST)
                NextTopPos += 0.66
                docRenderer.RenderObject(gfx, XUnit.FromInch(1), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), tblREQUEST2)
                NextTopPos = NextTopPos + 1

                ' Report-Type specific
                Select Case PPCReportType
                    Case PPCReportType_Enum.ADDRESS
                        ' Address - render results header, footers
                        docRenderer.RenderObject(gfx, XUnit.FromInch(1.25), XUnit.FromInch(NextTopPos), XUnit.FromInch(6), paraRESULTSHEADER)
                        NextTopPos = NextTopPos + 0.25
                        docRenderer.RenderObject(gfx, XUnit.FromInch(1), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), tblRESULTS)
                        docRenderer.RenderObject(gfx, XUnit.FromInch(1), XUnit.FromInch(10.25), XUnit.FromInch(4), paraFOOTERLeft)
                        docRenderer.RenderObject(gfx, XUnit.FromInch(5), XUnit.FromInch(10.25), XUnit.FromInch(2), paraFOOTERRight)
                        Exit Select
                    Case PPCReportType_Enum.NOMATCH
                        ' No Match - render footers only
                        docRenderer.RenderObject(gfx, XUnit.FromInch(1), XUnit.FromInch(10.25), XUnit.FromInch(4), paraFOOTERLeft)
                        docRenderer.RenderObject(gfx, XUnit.FromInch(5), XUnit.FromInch(10.25), XUnit.FromInch(2), paraFOOTERRight)
                        Exit Select
                    Case PPCReportType_Enum.ZIPCODE
                        ' Zip Code Match - Render paginated document
                        Dim CurrentPage As Integer = 0
                        Dim index As Integer = 0
                        For Each t As Tables.Table In DETAILRESULTSTABLES
                            CurrentPage += 1
                            index += 1
                            If CurrentPage = 1 Then
                                ' First page result & detail tables displayed below header section
                                docRenderer.RenderObject(gfx, XUnit.FromInch(1.25), XUnit.FromInch(NextTopPos), XUnit.FromInch(6), paraRESULTSHEADER)
                                NextTopPos = NextTopPos + 0.25
                                docRenderer.RenderObject(gfx, XUnit.FromInch(1), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), tblRESULTS)
                                NextTopPos = NextTopPos + 1
                                docRenderer.RenderObject(gfx, XUnit.FromInch(1), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), t)
                            Else
                                ' Pages after first use the whole page for header & detail
                                NextTopPos = 0.25
                                docRenderer.RenderObject(gfx, XUnit.FromInch(1.25), XUnit.FromInch(NextTopPos), XUnit.FromInch(6), paraTITLE)
                                NextTopPos = 0.7
                                docRenderer.RenderObject(gfx, XUnit.FromInch(1), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), t)
                            End If

                            ' All pages get footers
                            ' Left footer - always the same
                            docRenderer.RenderObject(gfx, XUnit.FromInch(1), XUnit.FromInch(10.25), XUnit.FromInch(4), paraFOOTERLeft)

                            ' Right Footer - Dynamic
                            docRenderer.RenderObject(gfx, XUnit.FromInch(5), XUnit.FromInch(10.25), XUnit.FromInch(2), RIGHT_FOOTERS(CurrentPage - 1))

                            If index < PageCount Then CreateNewPageBreak(err)
                        Next
                        Exit Select
                End Select

                Exit Sub
            Catch ex As Exception
                err = ex.Message
                Exit Sub
            End Try
        End Sub

        ''' <summary>
        ''' Generate credit report pdf
        ''' </summary>
        ''' <param name="qo"></param>
        ''' <param name="reportdata"></param>
        ''' <param name="CredObj"></param>
        ''' <param name="err"></param>
        ''' <remarks></remarks>
        Private Sub Generate_QUOTE_CREDIT_Report(ByVal qo As QuickQuote.CommonObjects.QuickQuoteObject, ByVal reportdata As String, ByVal CredObj As Diamond.Common.Objects.ThirdParty.ReportObjects.NCF.RecordGroup, ByRef err As String)
            Dim page As PdfPage = Nothing
            Dim NumDataLines As Integer = -1
            Dim DataSectionHeightInches As Decimal = 2

            ' Paragraphs
            Dim paraTITLE As Paragraph = Nothing
            Dim paraDATA As Paragraph = Nothing
            Dim paraSEARCHHeader As Paragraph = Nothing
            Dim paraSUBJHeader As Paragraph = Nothing
            Dim paraFooter1 As Paragraph = Nothing
            Dim paraFooter2 As Paragraph = Nothing
            Dim paraFooter3 As Paragraph = Nothing

            ' Tables
            Dim tblHDR As Tables.Table = Nothing
            Dim tblSRCH As Tables.Table = Nothing
            Dim tblSUBJ As Tables.Table = Nothing

            Try
                page = doc.AddPage()
                gfx = XGraphics.FromPdfPage(page)
                gfx.MUH = PdfFontEncoding.Unicode
                gfx.MFEH = PdfFontEmbedding.Default
                font = New XFont("Verdana", 13, XFontStyle.Bold)

                ' Document Body
                tempdoc = New Document()
                sec = tempdoc.AddSection()
                'sec.PageSetup.BottomMargin = Unit.FromInch(2)
                'sec.PageSetup.OddAndEvenPagesHeaderFooter = False
                sec.PageSetup.StartingNumber = 1

                ' FOOTER - THIS BULLSHIT DOES NOT WORK
                'footer = sec.Footers.Primary
                'paraFooter = New Paragraph()
                'paraFooter.AddTab()
                'paraFooter.AddText("Prepared by: NATIONAL CREDIT FILE SYSTEM")
                'paraFooter.AddLineBreak()
                'paraFooter.AddImage("C:\temp\IFMLogo.jpg")
                'paraFooter.Format.Alignment = ParagraphAlignment.Center

                'sec.Footers.Primary.Add(paraFooter)
                'sec.Footers.EvenPage.Add(paraFooter.Clone())

                'footer.Add(paraFooter)
                'sec.Footers.Primary = footer
                'sec.Footers.Primary.Add(paraFooter)
                'sec.Footers.Primary.Add(paraFooter)

                ' Title section
                paraTITLE = sec.AddParagraph("NATIONAL CREDIT FILE REPORT - " & qo.PolicyNumber) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
                paraTITLE.Format.Font.Name = "Verdana"
                paraTITLE.Format.Font.Size = 12
                paraTITLE.Format.Font.Bold = True
                paraTITLE.Format.Alignment = ParagraphAlignment.Center

                ' Divider underneath the title
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(0.5), page.Width, Unit.FromInch(0.5))

                ' *******************
                ' HEADER SECTION
                ' *******************
                tblHDR = New Tables.Table
                tblHDR.Borders.Width = 0
                tblHDR.Format.Font.Name = "Verdana"
                tblHDR.Format.Font.Size = 8

                ' Column 0 (label column 1)
                col = tblHDR.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Right
                col.Format.Font.Bold = True
                ' column 1 (data column 1)
                col = tblHDR.AddColumn(Unit.FromInch(4))
                col.Format.Alignment = ParagraphAlignment.Left
                ' Column 2 (label column 2)
                col = tblHDR.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Right
                col.Format.Font.Bold = True
                ' column 3 (data column 2)
                col = tblHDR.AddColumn(Unit.FromInch(2.5))
                col.Format.Alignment = ParagraphAlignment.Left

                ' First Row - Quoteback/Date of Receipt
                row = tblHDR.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("Quoteback:")
                cell = row.Cells(1)
                cell.AddParagraph(CredObj.General.Quoteback)  ' QUOTEBACK
                cell = row.Cells(2)
                cell.AddParagraph("Date of Receipt:")
                cell = row.Cells(3)
                cell.AddParagraph(CredObj.General.ReceiptDate)   ' RECEIPT DATE
                ' Second Row - Account/Time
                row = tblHDR.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("Account:")
                cell = row.Cells(1)
                cell.AddParagraph(CredObj.General.Account)  ' ACCOUNT
                cell = row.Cells(2)
                cell.AddParagraph("Time:")
                cell = row.Cells(3)
                cell.AddParagraph(CredObj.General.Time)  ' TIME
                ' Third Row - Requestor/NCF Ref
                row = tblHDR.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("Requestor:")
                cell = row.Cells(1)
                cell.AddParagraph(CredObj.General.Requestor)  ' REQUESTOR
                cell = row.Cells(2)
                cell.AddParagraph("NCF Ref.#:")
                cell = row.Cells(3)
                cell.AddParagraph(CredObj.General.ClueReferenceNo)  ' REF #

                tblHDR.SetEdge(0, 0, 4, 3, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                sec.Add(tblHDR)

                ' Divider between header section and data section
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(1), page.Width, Unit.FromInch(1))

                ' ****************
                ' DATA SECTION
                ' ****************
                ' Determine how much space is needed for the data section
                ' Count the number of lines based on carriage returns and line feeds
                For i As Integer = 0 To reportdata.Length - 1
                    If reportdata.Substring(i, 1) = vbLf _
                        OrElse reportdata.Substring(i, 1) = vbCrLf Then
                        'OrElse reportdata.Substring(i, 1) = vbCr _
                        NumDataLines += 1
                    End If
                Next

                ' Every 10 lines = 1.3 inches
                If NumDataLines < 10 Then
                    DataSectionHeightInches = 1
                Else
                    DataSectionHeightInches = (NumDataLines / 10) * 1.3
                End If

                paraDATA = sec.AddParagraph()
                paraDATA.Format.Alignment = ParagraphAlignment.Left
                paraDATA.Format.Font.Name = "Terminal"
                paraDATA.Format.Font.Size = 7
                paraDATA.Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black
                paraDATA.AddLineBreak()
                paraDATA.AddText(reportdata)

                ' ************************************
                ' SEARCH INFORMATION SECTION
                ' ************************************
                paraSEARCHHeader = sec.AddParagraph("SEARCH INFORMATION")
                paraSEARCHHeader.Format.Alignment = ParagraphAlignment.Center
                paraSEARCHHeader.Format.Font.Name = "Verdana"
                paraSEARCHHeader.Format.Font.Size = 8
                paraSEARCHHeader.Format.Font.Bold = True
                paraSEARCHHeader.Format.Borders.Color = Colors.Black
                paraSEARCHHeader.Format.Borders.Style = BorderStyle.Single

                tblSRCH = New Tables.Table
                tblSRCH.Borders.Width = 0
                tblSRCH.Format.Font.Name = "Verdana"
                tblSRCH.Format.Font.Size = 8

                ' Column 0 (label column 1)
                col = tblSRCH.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Right
                col.Format.Font.Bold = True
                ' column 1 (data column 1)
                col = tblSRCH.AddColumn(Unit.FromInch(4))
                col.Format.Alignment = ParagraphAlignment.Left
                ' Column 2 (label column 2)
                col = tblSRCH.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Right
                col.Format.Font.Bold = True
                ' column 3 (data column 2)
                col = tblSRCH.AddColumn(Unit.FromInch(2.5))
                col.Format.Alignment = ParagraphAlignment.Left

                ' First Row - Name/SSN
                row = tblSRCH.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("Name:")
                cell = row.Cells(1)
                cell.AddParagraph(CredObj.Search.SearchSubject)  ' NAME
                cell = row.Cells(2)
                cell.AddParagraph("SSN:")
                cell = row.Cells(3)
                cell.AddParagraph(CredObj.Search.SearchSSN)   ' SSN
                ' Second Row - Address/Order Type
                row = tblSRCH.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("Address:")
                cell = row.Cells(1)
                cell.AddParagraph(CredObj.Search.SearchAddress)  ' ADDRESS
                cell = row.Cells(2)
                cell.AddParagraph("Order Type:")
                cell = row.Cells(3)
                cell.AddParagraph(CredObj.Search.OrderType)  ' ORDER TYPE
                ' Third Row - former address
                row = tblSRCH.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("Former Address:")
                cell = row.Cells(1)
                cell.AddParagraph(CredObj.Search.SearchFormerAddress)  ' FORMER ADDRESS

                tblSRCH.SetEdge(0, 0, 4, 3, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                sec.Add(tblSRCH)

                ' ************************************
                ' SUBJECT IDENTIFICATION SECTION
                ' ************************************
                paraSUBJHeader = sec.AddParagraph("SUBJECT INFORMATION")
                paraSUBJHeader.Format.Alignment = ParagraphAlignment.Center
                paraSUBJHeader.Format.Font.Name = "Verdana"
                paraSUBJHeader.Format.Font.Size = 8
                paraSUBJHeader.Format.Font.Bold = True
                paraSUBJHeader.Format.Borders.Color = Colors.Black
                paraSUBJHeader.Format.Borders.Style = BorderStyle.Single

                tblSUBJ = New Tables.Table
                tblSUBJ.Borders.Width = 0
                tblSUBJ.Format.Font.Name = "Verdana"
                tblSUBJ.Format.Font.Size = 8

                ' Column 0 (label column 1)
                col = tblSUBJ.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Right
                col.Format.Font.Bold = True
                ' column 1 (data column 1)
                col = tblSUBJ.AddColumn(Unit.FromInch(4))
                col.Format.Alignment = ParagraphAlignment.Left
                ' Column 2 (label column 2)
                col = tblSUBJ.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Right
                col.Format.Font.Bold = True
                ' column 3 (data column 2)
                col = tblSUBJ.AddColumn(Unit.FromInch(2.5))
                col.Format.Alignment = ParagraphAlignment.Left

                ' First Row - Name/SSN
                row = tblSUBJ.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("Name:")
                cell = row.Cells(1)
                cell.AddParagraph(CredObj.Subject.SubName)  ' NAME
                cell = row.Cells(2)
                cell.AddParagraph("SSN:")
                cell = row.Cells(3)
                cell.AddParagraph(CredObj.Subject.SubSSN)   ' SSN
                ' Second Row - AKA
                row = tblSUBJ.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("A/K/A:")
                cell = row.Cells(1)
                cell.AddParagraph(CredObj.Subject.AName)  ' AKA
                ' Third Row - Address/RptDt
                row = tblSUBJ.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("Address:")
                cell = row.Cells(1)
                cell.AddParagraph(CredObj.Subject.SubAddress)  '  ADDRESS
                cell = row.Cells(2)
                cell.AddParagraph("RptDt:")
                cell = row.Cells(3)
                cell.AddParagraph(CredObj.Subject.Rptdate1)  ' RptDt1
                ' Fourth row Fmr Add/RptDt
                row = tblSUBJ.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("Fmr Add:")
                cell = row.Cells(1)
                cell.AddParagraph(CredObj.Subject.FromAddress)  ' FORMER ADDRESS
                cell = row.Cells(2)
                cell.AddParagraph("RptDt:")
                cell = row.Cells(3)
                cell.AddParagraph(CredObj.Subject.Rptdate2)  ' RptDt2
                ' Fifth row Fmr Add/RptDt
                row = tblSUBJ.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("Fmr Add:")
                cell = row.Cells(1)
                cell.AddParagraph(CredObj.Subject.FromAddress1)  ' FORMER ADDRESS 2
                cell = row.Cells(2)
                cell.AddParagraph("RptDt:")
                cell = row.Cells(3)
                cell.AddParagraph(CredObj.Subject.Rptdate3)  ' RptDt3
                ' Sixth row DOB/Sex
                row = tblSUBJ.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("DOB/AGE:")
                cell = row.Cells(1)
                cell.AddParagraph(CredObj.Subject.DOB)  ' DOB
                cell = row.Cells(2)
                cell.AddParagraph("Sex:")
                cell = row.Cells(3)
                cell.AddParagraph("")  ' TODO: Sex?
                ' Seventh Row - Dependents
                row = tblSUBJ.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("Dependents:")
                cell = row.Cells(1)
                cell.AddParagraph("")  ' TODO: Dependents?

                tblSUBJ.SetEdge(0, 0, 4, 3, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                sec.Add(tblSUBJ)

                ' Divider between the page body and the footer section
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(9.9), page.Width, Unit.FromInch(9.9))

                ' FOOTER PARAGRAPHS (3)
                paraFooter1 = sec.AddParagraph()
                paraFooter1.AddText("Prepared by: NATIONAL CREDIT FILE SYSTEM")
                paraFooter1.AddLineBreak()
                paraFooter1.AddText("LexisNexis, Inc")
                paraFooter1.AddLineBreak()
                paraFooter1.AddLineBreak()
                paraFooter1.AddLineBreak()
                paraFooter1.AddLineBreak()
                paraFooter1.AddLineBreak()
                paraFooter1.AddText("NCF is a Service Mark of LexisNexis Asset Company.  All Rights Reserved")
                paraFooter1.Format.Alignment = ParagraphAlignment.Center
                paraFooter1.Format.Font.Name = "Verdana"
                paraFooter1.Format.Font.Size = 8
                paraFooter1.Format.Font.Color = Colors.Black

                paraFooter2 = sec.AddParagraph()
                paraFooter2.AddFormattedText("If you have questions, contact:", TextFormat.Underline)
                paraFooter2.AddLineBreak()
                paraFooter2.AddText("LexisNexis Technical Support")
                paraFooter2.AddLineBreak()
                paraFooter2.AddText("P.O. Box 105179")
                paraFooter2.AddLineBreak()
                paraFooter2.AddText("Atlanta, GA 30348-5179")
                paraFooter2.AddLineBreak()
                paraFooter2.AddText("Telephone: 1-800-456-6432")
                paraFooter2.Format.Alignment = ParagraphAlignment.Left
                paraFooter2.Format.Font.Name = "Verdana"
                paraFooter2.Format.Font.Size = 8
                paraFooter2.Format.Font.Color = Colors.Black

                paraFooter3 = sec.AddParagraph()
                paraFooter3.AddFormattedText("Refer Consumers to:", TextFormat.Underline)
                paraFooter3.AddLineBreak()
                paraFooter3.AddText("LexisNexis Consumer Center")
                paraFooter3.AddLineBreak()
                paraFooter3.AddText("P.O. Box 105108")
                paraFooter3.AddLineBreak()
                paraFooter3.AddText("Atlanta, GA 30348-5108")
                paraFooter3.AddLineBreak()
                paraFooter3.AddText("Telephone: 1-800-456-6004")
                paraFooter3.AddLineBreak()
                paraFooter3.AddText("http://www.consumersdisclosure.com")
                paraFooter3.Format.Alignment = ParagraphAlignment.Left
                paraFooter3.Format.Font.Name = "Verdana"
                paraFooter3.Format.Font.Size = 8
                paraFooter3.Format.Font.Color = Colors.Black

                ' ADD ALL THE FORMATTED OBJECTS TO THE DOCUMENT
                docRenderer = New Rendering.DocumentRenderer(tempdoc)
                docRenderer.PrepareDocument()

                ' Render the objects. You can render tables, shapes etc the same way.
                NextTopPos = 0.25
                docRenderer.RenderObject(gfx, 0, XUnit.FromInch(NextTopPos), page.Width, paraTITLE)
                NextTopPos = NextTopPos + 0.26
                docRenderer.RenderObject(gfx, XUnit.Zero, XUnit.FromInch(NextTopPos), page.Width, tblHDR)
                NextTopPos = NextTopPos + 0.49
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), paraDATA)
                NextTopPos = NextTopPos + DataSectionHeightInches
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), paraSEARCHHeader)
                NextTopPos = NextTopPos + 0.27
                docRenderer.RenderObject(gfx, XUnit.Zero, XUnit.FromInch(NextTopPos), page.Width, tblSRCH)
                NextTopPos = NextTopPos + 0.5
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), paraSUBJHeader)
                NextTopPos = NextTopPos + 0.23
                docRenderer.RenderObject(gfx, XUnit.Zero, XUnit.FromInch(NextTopPos), page.Width, tblSUBJ)
                NextTopPos = NextTopPos + 6
                ' The footers are always going to be in the same place on the page
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(10), page.Width, paraFooter1)
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(10), XUnit.FromInch(4), paraFooter2)
                docRenderer.RenderObject(gfx, XUnit.FromInch(6.3), XUnit.FromInch(10), XUnit.FromInch(2.5), paraFooter3)

                ' This is how to draw an image on the page
                'Dim img As XImage = XImage.FromFile("C:\temp\IFMLogo.jpg")
                'gfx.DrawImage(img, Unit.FromInch(0.5), Unit.FromInch(7), Unit.FromInch(2), Unit.FromInch(0.5))

                Exit Sub
            Catch ex As Exception
                err = ex.Message
                Exit Sub
            End Try
        End Sub

        ''' <summary>
        ''' Generate driver MVR report pdf
        ''' </summary>
        ''' <param name="qo"></param>
        ''' <param name="reportdata"></param>
        ''' <param name="MVRObj"></param>
        ''' <param name="err"></param>
        ''' <remarks></remarks>
        Private Sub Generate_DRIVER_MVR_Report(ByVal qo As QuickQuote.CommonObjects.QuickQuoteObject, ByVal reportdata As String, ByVal MVRObj As Diamond.Common.Objects.ThirdParty.ReportObjects.MVR.MVRReportData, ByRef err As String)
            Dim page As PdfPage = Nothing
            Dim NumDataLines As Integer = 0
            Dim MiscSectionHeightInches As Decimal = 0
            Dim DrivingSectionHeightInches As Decimal = 0

            ' Paragraphs
            Dim paraTITLE As Paragraph = Nothing
            Dim paraTITLE2 As Paragraph = Nothing
            Dim paraSUBJHDR As Paragraph = Nothing
            Dim paraRPTHDR As Paragraph = Nothing
            Dim paraLICHDR As Paragraph = Nothing
            Dim paraDRVHDR As Paragraph = Nothing
            Dim paraMISC As Paragraph = Nothing
            Dim paraMISCHDR As Paragraph = Nothing
            Dim paraFOOTERLeft As New Paragraph()
            '            Dim paraFOOTERRight As New Paragraph()

            ' Tables
            Dim tblSUBJ As Tables.Table = Nothing   ' Subject table
            Dim tblRPT As Tables.Table = Nothing    ' Report table
            Dim tblLIC As Tables.Table = Nothing    ' License Table
            Dim tblDRV As Tables.Table = Nothing    ' Driver table

            Dim rowheight As MigraDoc.DocumentObjectModel.Unit = Unit.FromInch(0.33)
            Dim rowheight_slim As MigraDoc.DocumentObjectModel.Unit = Unit.FromInch(0.2)

            Try
                CurrentPageNum = 1

                rowcount = -1
                page = doc.AddPage()
                gfx = XGraphics.FromPdfPage(page)
                gfx.MUH = PdfFontEncoding.Unicode
                gfx.MFEH = PdfFontEmbedding.Default
                font = New XFont("Verdana", 13, XFontStyle.Bold)

                ' Document Body
                tempdoc = New Document()
                sec = tempdoc.AddSection()
                sec.PageSetup.StartingNumber = 1

                ' *********************
                ' TITLE SECTION
                ' *********************
                paraTITLE = sec.AddParagraph("MOTOR VEHICLE REPORT")
                paraTITLE.Format.Font.Name = "Verdana"
                paraTITLE.Format.Font.Size = 10
                paraTITLE.Format.Font.Bold = True
                paraTITLE.Format.Alignment = ParagraphAlignment.Center
                paraTITLE2 = sec.AddParagraph("DRIVER RECORD INFORMATION obtained by LexisNexis Inc on customer's behalf from the motor vehicle records of the state/province of")
                paraTITLE2.AddLineBreak()
                paraTITLE2.AddText("IN")
                paraTITLE2.AddLineBreak()
                paraTITLE2.AddText("identification of driver is based on information submitted")
                paraTITLE2.Format.Font.Name = "Verdana"
                paraTITLE2.Format.Font.Size = 8
                paraTITLE2.Format.Font.Bold = False
                paraTITLE2.Format.Alignment = ParagraphAlignment.Center

                ' Divider underneath the title
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(0.5), page.Width, Unit.FromInch(0.5))

                '****************************
                ' SUBJECT INFORMATION
                '****************************
                paraSUBJHDR = sec.AddParagraph("SUBJECT INFORMATION")
                paraSUBJHDR.Format.Alignment = ParagraphAlignment.Center
                paraSUBJHDR.Format.Font.Name = "Verdana"
                paraSUBJHDR.Format.Font.Size = 6
                paraSUBJHDR.Format.Font.Bold = True
                'paraSUBJHDR.Format.Borders.Color = Colors.Black
                'paraSUBJHDR.Format.Borders.Style = BorderStyle.None

                tblSUBJ = New Tables.Table
                tblSUBJ.Format.Alignment = ParagraphAlignment.Center
                tblSUBJ.Borders.Width = 0
                tblSUBJ.Format.Font.Name = "Verdana"
                tblSUBJ.Format.Font.Size = 6
                tblSUBJ.Borders.Color = Colors.Black
                tblSUBJ.Borders.Style = BorderStyle.Single
                tblSUBJ.Borders.Width = 0.5
                tblSUBJ.Borders.Bottom.Style = BorderStyle.Single
                tblSUBJ.Borders.Bottom.Width = 0.5
                tblSUBJ.Borders.Bottom.Color = Colors.Black
                tblSUBJ.Borders.Top.Style = BorderStyle.Single
                tblSUBJ.Borders.Top.Width = 0.5
                tblSUBJ.Borders.Top.Color = Colors.Black
                tblSUBJ.Borders.Left.Style = BorderStyle.Single
                tblSUBJ.Borders.Left.Width = 0.5
                tblSUBJ.Borders.Left.Color = Colors.Black
                tblSUBJ.Borders.Right.Style = BorderStyle.Single
                tblSUBJ.Borders.Right.Width = 0.5
                tblSUBJ.Borders.Right.Color = Colors.Black

                ' Column 0 (label column 1)
                col = tblSUBJ.AddColumn(Unit.FromInch(1))
                col.Format.Alignment = ParagraphAlignment.Right
                col.Format.Font.Bold = True
                ' column 1 (data column 1)
                col = tblSUBJ.AddColumn(Unit.FromInch(1.75))
                col.Format.Alignment = ParagraphAlignment.Left
                ' Column 2 (label column 2)
                col = tblSUBJ.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Right
                col.Format.Font.Bold = True
                ' column 3 (data column 2)
                col = tblSUBJ.AddColumn(Unit.FromInch(1.25))
                col.Format.Alignment = ParagraphAlignment.Left
                ' Column 4 (label column 3)
                col = tblSUBJ.AddColumn(Unit.FromInch(1))
                col.Format.Alignment = ParagraphAlignment.Right
                col.Format.Font.Bold = True
                ' column 5 (data column 3)
                col = tblSUBJ.AddColumn(Unit.FromInch(1.75))
                col.Format.Alignment = ParagraphAlignment.Left

                ' First Row - Name, Drivers License, Sex
                row = tblSUBJ.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                row.Height = rowheight
                cell = row.Cells(0)
                cell.AddParagraph("Name")
                cell = row.Cells(1)
                cell.AddParagraph(MVRObj.GeneralReport.Name)  ' Name
                cell = row.Cells(2)
                cell.AddParagraph("Driver License Number")
                cell = row.Cells(3)
                cell.AddParagraph(MVRObj.GeneralReport.DriverLicenseNumber)   ' Drivers License Number
                cell = row.Cells(4)
                cell.AddParagraph("Sex")
                cell = row.Cells(5)
                cell.AddParagraph(MVRObj.GeneralReport.Sex)   ' Sex
                ' Second Row - Address, SSN, Height/Weight
                row = tblSUBJ.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                row.Height = rowheight
                cell = row.Cells(0)
                cell.AddParagraph("Address")
                cell = row.Cells(1)
                ' Format Address
                txt = ""
                If MVRObj.GeneralReport.Street.Trim() <> "" OrElse MVRObj.GeneralReport.City.Trim <> "" OrElse MVRObj.GeneralReport.State <> "" Then
                    If MVRObj.GeneralReport.Street.Trim() <> "" AndAlso MVRObj.GeneralReport.City.Trim <> "" AndAlso MVRObj.GeneralReport.State <> "" Then
                        txt = MVRObj.GeneralReport.Street.Trim() & " " & MVRObj.GeneralReport.City.Trim() & ", " & MVRObj.GeneralReport.State.Trim()
                    ElseIf MVRObj.GeneralReport.Street.Trim() <> "" AndAlso MVRObj.GeneralReport.City.Trim() <> "" Then
                        txt = MVRObj.GeneralReport.Street.Trim() & ", " & MVRObj.GeneralReport.City.Trim()
                    ElseIf MVRObj.GeneralReport.City.Trim() <> "" AndAlso MVRObj.GeneralReport.State.Trim() <> "" Then
                        txt = MVRObj.GeneralReport.City.Trim() & ", " And MVRObj.GeneralReport.State.Trim()
                    ElseIf MVRObj.GeneralReport.Street.Trim() <> "" Then
                        txt = MVRObj.GeneralReport.Street.Trim()
                    ElseIf MVRObj.GeneralReport.City.Trim <> "" Then
                        txt = MVRObj.GeneralReport.City.Trim()
                    ElseIf MVRObj.GeneralReport.State <> "" Then
                        txt = MVRObj.GeneralReport.State.Trim()
                    End If
                End If
                cell.AddParagraph(txt)  ' Address
                cell = row.Cells(2)
                cell.AddParagraph("Social Security Number")
                cell = row.Cells(3)
                cell.AddParagraph(MVRObj.GeneralReport.SSN)  ' SSN
                cell = row.Cells(4)
                cell.AddParagraph("Height/Weight")
                cell = row.Cells(5)
                txt = "na / na"
                If MVRObj.GeneralReport.Height.Trim() <> "" OrElse MVRObj.GeneralReport.Weight.Trim <> "" Then
                    If MVRObj.GeneralReport.Height.Trim() <> "" AndAlso MVRObj.GeneralReport.Weight.Trim() <> "" Then
                        txt = MVRObj.GeneralReport.Height.Trim() & " / " & MVRObj.GeneralReport.Weight.Trim()
                    ElseIf MVRObj.GeneralReport.Height.Trim() <> "" Then
                        txt = MVRObj.GeneralReport.Height.Trim() & " / na"
                    Else
                        txt = "na / " & MVRObj.GeneralReport.Weight.Trim()
                    End If
                End If
                cell.AddParagraph(txt)  ' Height/Weight
                ' Third Row - AKA, Eye/Hair color, Birthdate
                row = tblSUBJ.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                row.Height = rowheight
                cell = row.Cells(0)
                cell.AddParagraph("A.K.A")
                cell = row.Cells(1)
                cell.AddParagraph("")  ' AKA
                cell = row.Cells(2)
                cell.AddParagraph("Eye/Hair Color")
                cell = row.Cells(3)
                txt = "na / na"
                If MVRObj.GeneralReport.Eyes.Trim() <> "" OrElse MVRObj.GeneralReport.Hair.Trim <> "" Then
                    If MVRObj.GeneralReport.Eyes.Trim() <> "" AndAlso MVRObj.GeneralReport.Hair.Trim() <> "" Then
                        txt = MVRObj.GeneralReport.Eyes.Trim() & " / " & MVRObj.GeneralReport.Hair.Trim()
                    ElseIf MVRObj.GeneralReport.Eyes.Trim() <> "" Then
                        txt = MVRObj.GeneralReport.Eyes.Trim() & " / na"
                    Else
                        txt = "na / " & MVRObj.GeneralReport.Hair.Trim()
                    End If
                End If
                cell.AddParagraph(txt)  ' Eye/Hair color
                cell = row.Cells(4)
                cell.AddParagraph("Birth Date")
                cell = row.Cells(5)
                cell.AddParagraph(MVRObj.GeneralReport.DataOfBirth)  ' Birth Date

                tblSUBJ.SetEdge(0, 0, 6, 3, Tables.Edge.Box, BorderStyle.Single, 0.5, Colors.Black)
                sec.Add(tblSUBJ)

                ' Divider between header section and data section
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(1), page.Width, Unit.FromInch(1))

                '****************************
                ' REPORT INFORMATION
                '****************************
                paraRPTHDR = sec.AddParagraph("REPORT INFORMATION")
                paraRPTHDR.Format.Alignment = ParagraphAlignment.Center
                paraRPTHDR.Format.Font.Name = "Verdana"
                paraRPTHDR.Format.Font.Size = 6
                paraRPTHDR.Format.Font.Bold = True
                'paraRPTHDR.Format.Borders.Color = Colors.Black
                'paraRPTHDR.Format.Borders.Style = BorderStyle.None

                tblRPT = New Tables.Table
                tblRPT.Borders.Width = 0
                tblRPT.Format.Font.Name = "Verdana"
                tblRPT.Format.Font.Size = 6
                tblRPT.Borders.Color = Colors.Black
                tblRPT.Borders.Style = BorderStyle.Single
                tblRPT.Borders.Width = 0.5

                ' Column 0 (label column 1)
                col = tblRPT.AddColumn(Unit.FromInch(1.25))
                col.Format.Alignment = ParagraphAlignment.Right
                col.Format.Font.Bold = True
                ' column 1 (data column 1)
                col = tblRPT.AddColumn(Unit.FromInch(2.875))
                col.Format.Alignment = ParagraphAlignment.Left
                ' Column 2 (label column 2)
                col = tblRPT.AddColumn(Unit.FromInch(1.25))
                col.Format.Alignment = ParagraphAlignment.Right
                col.Format.Font.Bold = True
                ' column 3 (data column 2)
                col = tblRPT.AddColumn(Unit.FromInch(2.875))
                col.Format.Alignment = ParagraphAlignment.Left

                ' First Row - Report Date, System Use
                row = tblRPT.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                row.Height = rowheight
                cell = row.Cells(0)
                cell.AddParagraph("Report Date")
                cell = row.Cells(1)
                cell.AddParagraph(MVRObj.GeneralReport.ReportDate)  ' Report Date
                cell = row.Cells(2)
                cell.AddParagraph("System Use")
                cell = row.Cells(3)
                cell.AddParagraph(MVRObj.GeneralReport.SystemUse)   ' System Use
                ' Second Row - Account Number/DMV Account Number
                row = tblRPT.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                row.Height = rowheight
                cell = row.Cells(0)
                cell.AddParagraph("Account Number")
                cell = row.Cells(1)
                cell.AddParagraph(MVRObj.GeneralReport.AccountNumber)  ' Account Number
                cell = row.Cells(2)
                cell.AddParagraph("DMV Account Number")
                cell = row.Cells(3)
                cell.AddParagraph(MVRObj.GeneralReport.DMVNumber)  ' DMV Account Number
                ' Third Row - Quoteback
                row = tblRPT.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                row.Height = rowheight
                cell = row.Cells(0)
                cell.AddParagraph("Quoteback")
                cell = row.Cells(1)
                cell.AddParagraph(MVRObj.GeneralReport.QuoteBack)  ' Quoteback

                tblRPT.SetEdge(0, 0, 4, 3, Tables.Edge.Box, BorderStyle.Single, 0.5, Colors.Black)
                sec.Add(tblRPT)

                '****************************
                ' DRIVER LICENSE INFORMATION
                '****************************
                paraLICHDR = sec.AddParagraph("DRIVER LICENSE INFORMATION")
                paraLICHDR.Format.Alignment = ParagraphAlignment.Center
                paraLICHDR.Format.Font.Name = "Verdana"
                paraLICHDR.Format.Font.Size = 6
                paraLICHDR.Format.Font.Bold = True
                'paraLICHDR.Format.Borders.Color = Colors.Black
                'paraLICHDR.Format.Borders.Style = BorderStyle.None

                tblLIC = New Tables.Table
                tblLIC.Borders.Width = 0
                tblLIC.Format.Font.Name = "Verdana"
                tblLIC.Format.Font.Size = 6
                tblLIC.Borders.Color = Colors.Black
                tblLIC.Borders.Style = BorderStyle.Single
                tblLIC.Borders.Width = 0.5

                ' Column 0 (Class)
                col = tblLIC.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Font.Bold = True
                ' column 1 (Issued)
                col = tblLIC.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Font.Bold = True
                ' Column 2 (Expires)
                col = tblLIC.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Font.Bold = True
                ' column 3 (Status)
                col = tblLIC.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Font.Bold = True
                ' column 4 (Restrictions)
                col = tblLIC.AddColumn(Unit.FromInch(2.25))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Font.Bold = True

                ' Label Row
                row = tblLIC.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                row.Height = rowheight_slim
                cell = row.Cells(0)
                cell.AddParagraph("Class")
                cell = row.Cells(1)
                cell.AddParagraph("Issued")
                cell = row.Cells(2)
                cell.AddParagraph("Expires")
                cell = row.Cells(3)
                cell.AddParagraph("Status")
                cell = row.Cells(4)
                cell.AddParagraph("Restrictions")

                ' Data Row
                row = tblLIC.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                row.Height = rowheight_slim
                cell = row.Cells(0)
                cell.AddParagraph(MVRObj.DriverLicense.LicenseClass)   ' Class
                cell = row.Cells(1)
                cell.AddParagraph(MVRObj.DriverLicense.Issued)   ' Issued
                cell = row.Cells(2)
                cell.AddParagraph(MVRObj.DriverLicense.Expires)   ' Expires
                cell = row.Cells(3)
                cell.AddParagraph(MVRObj.DriverLicense.Status)   ' Status
                cell = row.Cells(4)
                cell.AddParagraph(MVRObj.DriverLicense.Restrictions)   ' Restrictions

                tblLIC.SetEdge(0, 0, 5, 2, Tables.Edge.Box, BorderStyle.Single, 0.5, Colors.Black)
                sec.Add(tblLIC)

                '****************************
                ' MISC AND STATE INFORMATION
                '****************************
                paraMISCHDR = sec.AddParagraph()
                paraMISCHDR.Format.Alignment = ParagraphAlignment.Center
                paraMISCHDR.Format.Font.Name = "Verdana"
                paraMISCHDR.Format.Font.Size = 6
                paraMISCHDR.Format.Font.Bold = True
                'paraMISCHDR.Format.Borders.Color = Colors.Black
                'paraMISCHDR.Format.Borders.Style = BorderStyle.None
                paraMISCHDR.AddText("MISCELLANEOUS AND STATE SPECIFIC INFORMATION")
                paraMISC = sec.AddParagraph()
                paraMISC.Format.Alignment = ParagraphAlignment.Left
                paraMISC.Format.Font.Name = "Terminal"
                paraMISC.Format.Font.Size = 6
                paraMISC.Format.Font.Color = MigraDoc.DocumentObjectModel.Colors.Black
                paraMISC.Format.Borders.Color = Colors.Black
                paraMISC.Format.Borders.Width = 0.5
                paraMISC.AddLineBreak()
                paraMISC.AddText(MVRObj.MiscellaneousAndStateSpecificInfo.Miscellaneous)

                ' Determine the height of the MISC section
                ' Determine how much space is needed for the data section
                ' Count the number of lines based on carriage returns and line feeds
                For i As Integer = 0 To MVRObj.MiscellaneousAndStateSpecificInfo.Miscellaneous.Length - 1
                    If MVRObj.MiscellaneousAndStateSpecificInfo.Miscellaneous.Substring(i, 1) = vbLf _
                        OrElse MVRObj.MiscellaneousAndStateSpecificInfo.Miscellaneous.Substring(i, 1) = vbCrLf Then
                        NumDataLines += 1
                    End If
                Next

                ' Every 10 lines = 1.25 inches
                If NumDataLines < 10 Then
                    MiscSectionHeightInches = 1
                Else
                    MiscSectionHeightInches = (NumDataLines / 10) * 1.25
                End If

                '****************************
                ' DRIVING RECORD
                '****************************
                paraDRVHDR = sec.AddParagraph("DRIVING RECORD")
                paraDRVHDR.Format.Alignment = ParagraphAlignment.Center
                paraDRVHDR.Format.Font.Name = "Verdana"
                paraDRVHDR.Format.Font.Size = 6
                paraDRVHDR.Format.Font.Bold = True
                'paraDRVHDR.Format.Borders.Color = Colors.Black
                'paraDRVHDR.Format.Borders.Style = BorderStyle.None

                tblDRV = New Tables.Table
                tblDRV.Borders.Width = 0
                tblDRV.Format.Font.Name = "Verdana"
                tblDRV.Format.Font.Size = 6
                tblDRV.Borders.Color = Colors.Black
                tblDRV.Borders.Style = BorderStyle.Single
                tblDRV.Borders.Width = 0.5

                ' Column 0 (Type)
                col = tblDRV.AddColumn(Unit.FromInch(0.75))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Font.Bold = True
                ' column 1 (Violation Date)
                col = tblDRV.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Font.Bold = True
                ' Column 2 (Conviction Date)
                col = tblDRV.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Font.Bold = True
                ' column 3 (Description)
                col = tblDRV.AddColumn(Unit.FromInch(2))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Font.Bold = True
                ' column 4 (Viol/Conv Code)
                col = tblDRV.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Font.Bold = True
                ' column 5 (Points)
                col = tblDRV.AddColumn(Unit.FromInch(1))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Font.Bold = True

                ' Label Row
                row = tblDRV.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                row.Height = rowheight_slim
                cell = row.Cells(0)
                cell.AddParagraph("Type")
                cell = row.Cells(1)
                cell.AddParagraph("Violation Date")
                cell = row.Cells(2)
                cell.AddParagraph("Conviction Date")
                cell = row.Cells(3)
                cell.AddParagraph("Description")
                cell = row.Cells(4)
                cell.AddParagraph("Viol/Conv Code")
                cell = row.Cells(5)
                cell.AddParagraph("Points")

                ' Data Rows
                If MVRObj.DrivingRecords IsNot Nothing AndAlso MVRObj.DrivingRecords.Count > 0 Then
                    rowcount = 0
                    ' Create a row for each driving record
                    For Each drv As Diamond.Common.Objects.ThirdParty.ReportObjects.MVR.DrivingRecords In MVRObj.DrivingRecords
                        rowcount += 1
                        row = tblDRV.AddRow()
                        row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        row.Height = rowheight_slim
                        cell = row.Cells(0)
                        cell.AddParagraph(drv.Type)   ' Type
                        cell = row.Cells(1)
                        cell.AddParagraph(drv.ViolationDate)   ' Violation Date
                        cell = row.Cells(2)
                        cell.AddParagraph(drv.ConvictionDate)   ' Conviction Date
                        cell = row.Cells(3)
                        cell.AddParagraph(drv.Description)   ' Description
                        cell = row.Cells(4)
                        cell.AddParagraph(drv.Violation)   ' Viol/Conv Code
                        cell = row.Cells(5)
                        cell.AddParagraph(drv.Points)   ' Points
                    Next
                Else
                    ' No records
                    rowcount += 1
                    row = tblDRV.AddRow()
                    row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                    row.Height = rowheight_slim
                    cell = row.Cells(0)
                    cell.AddParagraph("")   ' Type
                    cell = row.Cells(1)
                    cell.AddParagraph("**No Data")   ' Violation Date
                    cell = row.Cells(2)
                    cell.AddParagraph("")   ' Conviction Date
                    cell = row.Cells(3)
                    cell.AddParagraph("**No Data")   ' Description
                    cell = row.Cells(4)
                    cell.AddParagraph("")   ' Viol/Conv Code
                    cell = row.Cells(5)
                    cell.AddParagraph("")   ' Points
                End If

                ' Determine the height of the driving record section based on the number of rows in the table
                ' Each row uses .2 inch in height
                DrivingSectionHeightInches = 0.2 * rowcount

                tblDRV.SetEdge(0, 0, 6, rowcount, Tables.Edge.Box, BorderStyle.Single, 0.5, Colors.Black)
                sec.Add(tblDRV)

                ' FOOTERS
                ' Left
                paraFOOTERLeft = sec.AddParagraph()
                paraFOOTERLeft.AddText(DateTime.Now.ToShortDateString() & " " & DateTime.Now.ToShortTimeString())
                paraFOOTERLeft.Format.Font.Name = "Verdana"
                paraFOOTERLeft.Format.Font.Size = 5
                paraFOOTERLeft.Format.Alignment = ParagraphAlignment.Left

                ' Right
                'paraFOOTERRight = sec.AddParagraph()
                'paraFOOTERRight.AddText("Page " & CurrentPageNum.ToString())
                'paraFOOTERRight.Format.Font.Name = "Verdana"
                'paraFOOTERRight.Format.Font.Size = 5
                'paraFOOTERRight.Format.Alignment = ParagraphAlignment.Right

                ' ADD ALL THE FORMATTED OBJECTS TO THE DOCUMENT
                docRenderer = New Rendering.DocumentRenderer(tempdoc)
                docRenderer.PrepareDocument()

                ' Render the objects. You can render tables, shapes etc the same way.
                NextTopPos = 0.25
                docRenderer.RenderObject(gfx, 0, XUnit.FromInch(NextTopPos), page.Width, paraTITLE)
                NextTopPos = NextTopPos + 0.25
                docRenderer.RenderObject(gfx, 0, XUnit.FromInch(NextTopPos), page.Width, paraTITLE2)
                NextTopPos = NextTopPos + 0.5
                docRenderer.RenderObject(gfx, 0, XUnit.FromInch(NextTopPos), page.Width, paraSUBJHDR)
                NextTopPos = NextTopPos + 0.125
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8.25), tblSUBJ)
                NextTopPos = NextTopPos + 1.125
                docRenderer.RenderObject(gfx, 0, XUnit.FromInch(NextTopPos), page.Width, paraRPTHDR)
                NextTopPos = NextTopPos + 0.125
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), tblRPT)
                NextTopPos = NextTopPos + 1.125
                docRenderer.RenderObject(gfx, 0, XUnit.FromInch(NextTopPos), page.Width, paraLICHDR)
                NextTopPos = NextTopPos + 0.125
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), tblLIC)
                NextTopPos = NextTopPos + 0.625
                CheckForMVRPageBreak(paraFOOTERLeft, err, MiscSectionHeightInches)
                docRenderer.RenderObject(gfx, 0, XUnit.FromInch(NextTopPos), page.Width, paraMISCHDR)
                NextTopPos = NextTopPos + 0.125
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8.25), paraMISC)
                NextTopPos = NextTopPos + MiscSectionHeightInches
                CheckForMVRPageBreak(paraFOOTERLeft, err, DrivingSectionHeightInches)
                docRenderer.RenderObject(gfx, 0, XUnit.FromInch(NextTopPos), page.Width, paraDRVHDR)
                NextTopPos = NextTopPos + 0.125
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), tblDRV)
                'NextTopPos = NextTopPos + DrivingSectionHeightInches

                DrawMVRFooter(paraFOOTERLeft, err)
                'docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(10.75), XUnit.FromInch(2), paraFOOTERLeft)
                'docRenderer.RenderObject(gfx, XUnit.FromInch(7), XUnit.FromInch(10.75), XUnit.FromInch(1), paraFOOTERRight)

                'docRenderer.RenderObject(gfx, 0, XUnit.FromInch(0.25), page.Width, paraTITLE)
                'docRenderer.RenderObject(gfx, 0, XUnit.FromInch(0.5), page.Width, paraTITLE2)
                'docRenderer.RenderObject(gfx, 0, XUnit.FromInch(1), page.Width, paraSUBJHDR)
                'docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(1.125), XUnit.FromInch(8.25), tblSUBJ)
                'docRenderer.RenderObject(gfx, 0, XUnit.FromInch(2.25), page.Width, paraRPTHDR)
                'docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(2.375), XUnit.FromInch(8), tblRPT)
                'docRenderer.RenderObject(gfx, 0, XUnit.FromInch(3.5), page.Width, paraLICHDR)
                'docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(3.625), XUnit.FromInch(8), tblLIC)
                'docRenderer.RenderObject(gfx, 0, XUnit.FromInch(4.25), page.Width, paraMISCHDR)
                'docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(4.375), XUnit.FromInch(8.25), paraMISC)
                'docRenderer.RenderObject(gfx, 0, XUnit.FromInch(5), page.Width, paraDRVHDR)
                'docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(5.125), XUnit.FromInch(8), tblDRV)
                'docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(10), XUnit.FromInch(3), paraFOOTERLeft)

                'docRenderer.RenderObject(gfx, 0, XUnit.FromInch(10.0), page.Width, paraFooter1)
                'docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(10.0), XUnit.FromInch(4), paraFooter2)
                'docRenderer.RenderObject(gfx, XUnit.FromInch(6.3), XUnit.FromInch(10.0), XUnit.FromInch(2.5), paraFooter3)

                ' This is how to draw an image on the page
                'Dim img As XImage = XImage.FromFile("C:\temp\IFMLogo.jpg")
                'gfx.DrawImage(img, Unit.FromInch(0.5), Unit.FromInch(7), Unit.FromInch(2), Unit.FromInch(0.5))

                Exit Sub
            Catch ex As Exception
                err = ex.Message
                Exit Sub
            End Try
        End Sub

        ''' <summary>
        ''' Generate CLUE report pdf for Personal Auto
        ''' </summary>
        ''' <param name="qo"></param>
        ''' <param name="reportdata"></param>
        ''' <param name="CLUEObj"></param>
        ''' <param name="err"></param>
        ''' <remarks></remarks>
        Private Sub Generate_CLUE_Report_PPA(ByVal qo As QuickQuote.CommonObjects.QuickQuoteObject, ByVal reportdata As String, ByVal CLUEObj As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.ClueAutoReportData, ByRef err As String)
            Dim RecapSectionHeightInches As Decimal = 0
            Dim MsgSectionHeightInches As Decimal = 0
            Dim ClaimHistorySectionHeightInches As Decimal = 0
            Dim PRCSectionHeightInches As Decimal = 0
            Dim VehSearchResultsSectionHeightInches As Decimal = 0
            Dim VehicleListSectionHeight As Decimal = 0
            Dim PADSectionHeightInches As Decimal = 0
            Dim DataFound As Boolean = False
            Dim z As Integer = -1

            ' Constants
            Const RecapItemHeightInches As Decimal = 0.25
            Const MsgItemHeightInches As Decimal = 0.2
            Const ClaimHistoryItemHeightInches As Decimal = 1.2
            Const PRCItemHeightInches As Decimal = 1.3
            Const VehSearchResultsItemHeightInches As Decimal = 1.3
            Const PADItemHeightInches As Decimal = 1
            'Const DataSectionHeightInches As Decimal = 2

            ' Paragraphs
            Dim paraTITLE As Paragraph = Nothing
            Dim paraRECAP As Paragraph = Nothing
            Dim paraMSGHDR As Paragraph = Nothing
            Dim paraMSG As Paragraph = Nothing
            Dim paraSEARCHSECTIONHEADER As Paragraph = Nothing
            Dim paraSUBJECTHEADERS As List(Of Paragraph) = Nothing
            Dim paraPRIORHEADER As Paragraph = Nothing
            Dim paraVEHSEARCHHEADER As Paragraph = Nothing
            Dim paraVEHICLEHEADER As Paragraph = Nothing
            Dim newSUBJHEADER As Paragraph = Nothing
            Dim paraORDERDATE As Paragraph = Nothing
            Dim paraCLAIMHISTHEADER As Paragraph = Nothing
            Dim paraPRCHEADER As Paragraph = Nothing
            Dim paraPADHEADER As Paragraph = Nothing
            Dim paraFooterCenter As Paragraph = Nothing
            Dim paraFooterLeft As Paragraph = Nothing
            Dim paraFooterRight As Paragraph = Nothing

            ' Tables
            Dim tblRPTINFO As Tables.Table = Nothing
            Dim tblSUBJ As List(Of Tables.Table) = Nothing
            Dim newSUBJtable As Tables.Table = Nothing
            Dim tblVEH As Tables.Table = Nothing
            Dim tblCLAIMHIST As Tables.Table = Nothing
            Dim tblPRIOR As Tables.Table = Nothing
            Dim tblVEHSEARCH As Tables.Table = Nothing
            Dim tblPRC As Tables.Table = Nothing
            Dim tblPAD As Tables.Table = Nothing

            Try
                CurrentPageNum = 1
                CurrentPage = doc.AddPage()
                gfx = XGraphics.FromPdfPage(CurrentPage)
                gfx.MUH = PdfFontEncoding.Unicode
                gfx.MFEH = PdfFontEmbedding.Default
                font = New XFont("Verdana", 13, XFontStyle.Bold)

                ' Document Body
                tempdoc = New Document()
                sec = tempdoc.AddSection()
                sec.PageSetup.StartingNumber = 1

                ' *******************
                ' TITLE SECTION
                ' *******************
                paraTITLE = sec.AddParagraph("C.L.U.E. Inc.")
                paraTITLE.Format.Font.Name = "Verdana"
                paraTITLE.Format.Font.Size = 10
                paraTITLE.Format.Font.Bold = True
                paraTITLE.Format.Alignment = ParagraphAlignment.Center
                paraTITLE.AddLineBreak()
                paraTITLE.AddText("C.L.U.E. - COMPREHENSIVE LOSS UNDERWRITING EXCHANGE")

                ' Divider underneath the title
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(0.5), CurrentPage.Width, Unit.FromInch(0.5))

                paraORDERDATE = sec.AddParagraph("Date of Order: " & CLUEObj.GeneralReportInfo.OrderDate)
                paraORDERDATE.Format.Font.Name = "Verdana"
                paraORDERDATE.Format.Font.Size = 8
                paraORDERDATE.Format.Alignment = ParagraphAlignment.Left

                ' ********************
                ' REPORT INFO SECTION
                ' ********************
                tblRPTINFO = New Tables.Table
                tblRPTINFO.Borders.Width = 0
                tblRPTINFO.Format.Font.Name = "Verdana"
                tblRPTINFO.Format.Font.Size = 8

                col = Nothing
                row = Nothing

                ' TOTAL TABLE WIDTH = 7.75 INCHES
                ' Column 0 (Label Column 1)
                col = tblRPTINFO.AddColumn(Unit.FromInch(1.25))
                col.Format.Alignment = ParagraphAlignment.Left
                ' column 1 (Data Column 1)
                col = tblRPTINFO.AddColumn(Unit.FromInch(3))
                col.Format.Alignment = ParagraphAlignment.Left
                ' Column 2 (Label Column 2)
                col = tblRPTINFO.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Left
                ' column 3 (Data Column 2)
                col = tblRPTINFO.AddColumn(Unit.FromInch(2))
                col.Format.Alignment = ParagraphAlignment.Left

                ' Row 1
                row = tblRPTINFO.AddRow()
                cell = row.Cells(0)
                cell.AddParagraph("Quoteback:")
                cell = row.Cells(1)
                cell.AddParagraph(CLUEObj.GeneralReportInfo.Quoteback)
                cell = row.Cells(2)
                cell.AddParagraph("Date of Order:")
                cell = row.Cells(3)
                cell.AddParagraph(CLUEObj.GeneralReportInfo.OrderDate)
                ' Row 2
                row = tblRPTINFO.AddRow()
                cell = row.Cells(0)
                cell.AddParagraph("Account:")
                cell = row.Cells(1)
                cell.AddParagraph(CLUEObj.GeneralReportInfo.Account)
                cell = row.Cells(2)
                cell.AddParagraph("Date of Receipt:")
                cell = row.Cells(3)
                cell.AddParagraph(CLUEObj.GeneralReportInfo.ReceiptDate)
                ' Row 3
                row = tblRPTINFO.AddRow()
                cell = row.Cells(0)
                cell.AddParagraph("Requestor:")
                cell = row.Cells(1)
                cell.AddParagraph(CLUEObj.GeneralReportInfo.Requestor)
                cell = row.Cells(2)
                cell.AddParagraph("C.L.U.E. Ref #:")
                cell = row.Cells(3)
                cell.AddParagraph(CLUEObj.GeneralReportInfo.ClueReferenceNo)

                tblRPTINFO.SetEdge(0, 0, 4, 3, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                sec.Add(tblRPTINFO)

                ' *******************
                ' RECAP SECTION
                ' *******************
                paraRECAP = sec.AddParagraph()
                paraRECAP.Format.Font.Name = "Verdana"
                paraRECAP.Format.Font.Size = 8
                paraRECAP.Format.Alignment = ParagraphAlignment.Left
                paraRECAP.AddText("RECAP:")
                z = -1
                If CLUEObj.Recap IsNot Nothing AndAlso CLUEObj.Recap.DataItems IsNot Nothing AndAlso CLUEObj.Recap.DataItems.Count > 0 Then
                    For Each recap As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.RecapDataItem In CLUEObj.Recap.DataItems
                        z += 1
                        paraRECAP.AddLineBreak()
                        If recap.Vehicle IsNot Nothing AndAlso recap.Vehicle.Trim() <> "" Then
                            txt = RemoveExtraLineBreaksFromText(recap.Vehicle, err)
                            If err <> "" Then Throw New Exception(err)
                            paraRECAP.AddText(txt)
                            If z < CLUEObj.Recap.DataItems.Count - 1 Then paraRECAP.AddLineBreak()
                        ElseIf recap.Subject IsNot Nothing AndAlso recap.Subject.Trim() <> "" Then
                            txt = RemoveExtraLineBreaksFromText(recap.Subject, err)
                            If err <> "" Then Throw New Exception(err)
                            paraRECAP.AddText(txt)
                            If z < CLUEObj.Recap.DataItems.Count - 1 Then paraRECAP.AddLineBreak()
                        End If
                    Next
                    ' Calculate the height of the recap section based on number of items
                    RecapSectionHeightInches = CLUEObj.Recap.DataItems.Count * RecapItemHeightInches
                Else
                    paraRECAP.AddLineBreak()
                    paraRECAP.AddText(NoDataText)
                    ' Calculate the height of the recap section based on number of items
                    RecapSectionHeightInches = RecapItemHeightInches
                End If

                ' *******************
                ' MESSAGES SECTION
                ' *******************
                paraMSGHDR = sec.AddParagraph()
                paraMSGHDR.Format.Font.Name = "Verdana"
                paraMSGHDR.Format.Font.Size = 8
                paraMSGHDR.Format.Alignment = ParagraphAlignment.Center
                paraMSGHDR.AddText("MESSAGES")
                paraMSG = sec.AddParagraph()
                paraMSG.Format.Font.Name = "Verdana"
                paraMSG.Format.Font.Size = 8
                paraMSG.Format.Alignment = ParagraphAlignment.Left
                ' Check to make sure there's data in the messages
                DataFound = False
                If CLUEObj.Messages IsNot Nothing AndAlso CLUEObj.Messages.Count > 0 Then
                    For Each msg As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.Messages In CLUEObj.Messages
                        If msg.Message <> "" Then
                            DataFound = True
                            Exit For
                        End If
                    Next
                Else
                    DataFound = False
                End If

                If DataFound AndAlso CLUEObj.Messages IsNot Nothing AndAlso CLUEObj.Messages.Count > 0 Then
                    For Each msg As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.Messages In CLUEObj.Messages
                        paraMSG.AddLineBreak()
                        paraMSG.AddText(msg.Message)
                    Next
                    MsgSectionHeightInches = CLUEObj.Messages.Count * MsgItemHeightInches
                Else
                    paraMSG.AddText(NoDataText)
                    MsgSectionHeightInches = MsgItemHeightInches
                End If

                ' *******************
                ' SEARCH SECTION
                ' *******************
                paraSEARCHSECTIONHEADER = sec.AddParagraph()
                paraSEARCHSECTIONHEADER.Format.Font.Name = "Verdana"
                paraSEARCHSECTIONHEADER.Format.Font.Size = 8
                paraSEARCHSECTIONHEADER.Format.Alignment = ParagraphAlignment.Center
                paraSEARCHSECTIONHEADER.AddText("SEARCH REQUEST")

                ' *******************
                ' Subjects
                ' *******************
                If CLUEObj.SearchRequest Is Nothing OrElse CLUEObj.SearchRequest.Subjects Is Nothing OrElse CLUEObj.SearchRequest.Subjects.Count <= 0 Then Throw New Exception("So Subject Data Found")

                ' Generate a subject section and header for each subject
                paraSUBJECTHEADERS = New List(Of Paragraph)
                tblSUBJ = New List(Of Tables.Table)
                i = -1
                For Each subj As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.SearchRequestSubject In CLUEObj.SearchRequest.Subjects
                    i += 1
                    ' Set up the subject header
                    newSUBJHEADER = sec.AddParagraph()
                    newSUBJHEADER.Format.Font.Name = "Verdana"
                    newSUBJHEADER.Format.Font.Size = 8
                    newSUBJHEADER.Format.Borders.Color = Colors.Black
                    newSUBJHEADER.Format.Borders.Width = 1
                    newSUBJHEADER.Format.Alignment = ParagraphAlignment.Center
                    newSUBJHEADER.AddText("Subject #" & (i + 1).ToString())

                    ' Set up the subject table
                    newSUBJtable = New Tables.Table
                    newSUBJtable.Borders.Width = 0
                    newSUBJtable.Format.Font.Name = "Verdana"
                    newSUBJtable.Format.Font.Size = 8
                    ' Column 0 (labels column)
                    col = newSUBJtable.AddColumn(Unit.FromInch(1.5))
                    col.Format.Alignment = ParagraphAlignment.Right
                    ' column 1 (data column 1)
                    col = newSUBJtable.AddColumn(Unit.FromInch(2.25))
                    col.Format.Alignment = ParagraphAlignment.Left
                    ' Column 2 (data column 2)
                    col = newSUBJtable.AddColumn(Unit.FromInch(2.25))
                    col.Format.Alignment = ParagraphAlignment.Right
                    ' column 3 (data column 3)
                    col = newSUBJtable.AddColumn(Unit.FromInch(2.25))
                    col.Format.Alignment = ParagraphAlignment.Left

                    ' First Row - Name
                    row = newSUBJtable.AddRow()
                    row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                    cell = row.Cells(0)
                    cell.AddParagraph("Name:")
                    cell = row.Cells(1)
                    cell.AddParagraph(CLUEObj.SearchRequest.Subjects(i).Name)  ' Name
                    ' Second Row - Address Line 1
                    row = newSUBJtable.AddRow()
                    row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                    cell = row.Cells(0)
                    cell.AddParagraph("Address:")
                    cell = row.Cells(1)
                    cell.AddParagraph(CLUEObj.SearchRequest.Subjects(i).AddressLine1)  ' Address Line 1
                    ' Third Row - Address Line 2
                    row = newSUBJtable.AddRow()
                    row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                    cell = row.Cells(0)
                    cell.AddParagraph("")
                    cell = row.Cells(1)
                    cell.AddParagraph(CLUEObj.SearchRequest.Subjects(i).AddressLine2)  ' Address Line 2
                    ' Fourth Row - DOB, Sex, SSN
                    row = newSUBJtable.AddRow()
                    row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                    cell = row.Cells(0)
                    cell.AddParagraph("DOB:")
                    cell = row.Cells(1)
                    cell.AddParagraph(CLUEObj.SearchRequest.Subjects(i).PriorDateOfBirth)  ' Address Line 2
                    cell = row.Cells(2)
                    cell.AddParagraph("Sex: " & CLUEObj.SearchRequest.Subjects(i).PriorGender)  ' Sex
                    cell = row.Cells(3)
                    cell.AddParagraph("SSN: " & CLUEObj.SearchRequest.Subjects(i).PriorTaxNumber)  ' SSN
                    ' Fifth Row - D/L#, State
                    row = newSUBJtable.AddRow()
                    row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                    cell = row.Cells(0)
                    cell.AddParagraph("D/L#:")
                    cell = row.Cells(1)
                    cell.AddParagraph(CLUEObj.SearchRequest.Subjects(i).PriorDriversLicenseNumber)  ' Drivers License #
                    cell = row.Cells(2)
                    cell.AddParagraph("State: " & CLUEObj.SearchRequest.Subjects(i).PriorDriversLicenseState)  ' Drivers license state

                    newSUBJtable.SetEdge(0, 0, 4, 5, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                    sec.Add(newSUBJtable)

                    tblSUBJ.Add(newSUBJtable)
                    paraSUBJECTHEADERS.Add(newSUBJHEADER)
                Next

                ' ****************
                ' PRIOR SECTION
                ' ****************
                ' Header
                paraPRIORHEADER = sec.AddParagraph("PRIOR")
                paraPRIORHEADER.Format.Font.Name = "Verdana"
                paraPRIORHEADER.Format.Font.Size = 8
                paraPRIORHEADER.Format.Font.Bold = False
                paraPRIORHEADER.Format.Alignment = ParagraphAlignment.Center
                paraPRIORHEADER.Format.Borders.Color = Colors.Black
                paraPRIORHEADER.Format.Borders.Width = 1
                ' Data
                tblPRIOR = New Tables.Table
                tblPRIOR.Borders.Width = 0
                tblPRIOR.Format.Font.Name = "Verdana"
                tblPRIOR.Format.Font.Size = 8
                ' Column 0 (labels column 1)
                col = tblPRIOR.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Right
                ' column 1 (data column 1)
                col = tblPRIOR.AddColumn(Unit.FromInch(2.5))
                col.Format.Alignment = ParagraphAlignment.Left
                ' Column 2 (labels column 2)
                col = tblPRIOR.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Left
                ' column 3 (data column 2)
                col = tblPRIOR.AddColumn(Unit.FromInch(2.5))
                col.Format.Alignment = ParagraphAlignment.Left

                ' First Row
                row = tblPRIOR.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("Address:")
                cell = row.Cells(1)
                cell.AddParagraph("")  ' Address
                cell = row.Cells(2)
                cell.AddParagraph("Policy: " & " " & "Type: " & " " & "No: ")
                ' Second Row
                row = tblPRIOR.AddRow()
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("")
                cell = row.Cells(1)
                cell.AddParagraph("")  ' Address Line 2?
                cell = row.Cells(2)
                cell.AddParagraph("Company: ")
                cell = row.Cells(3)
                cell.AddParagraph("")   ' Company

                tblPRIOR.SetEdge(0, 0, 4, 2, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                sec.Add(tblPRIOR)

                ' ****************
                ' VEHICLES SECTION
                ' ****************
                ' Header
                paraVEHICLEHEADER = sec.AddParagraph("VEHICLES")
                paraVEHICLEHEADER.Format.Font.Name = "Verdana"
                paraVEHICLEHEADER.Format.Font.Size = 8
                paraVEHICLEHEADER.Format.Font.Bold = False
                paraVEHICLEHEADER.Format.Alignment = ParagraphAlignment.Center
                paraVEHICLEHEADER.Format.Borders.Color = Colors.Black
                paraVEHICLEHEADER.Format.Borders.Width = 1

                If CLUEObj.SearchRequest.Vehicles Is Nothing OrElse CLUEObj.SearchRequest.Vehicles Is Nothing OrElse CLUEObj.SearchRequest.Vehicles.Count <= 0 Then Throw New Exception("No Vehicles found!")
                tblVEH = New Tables.Table
                tblVEH.Borders.Width = 0
                tblVEH.Format.Font.Name = "Verdana"
                tblVEH.Format.Font.Size = 8

                col = Nothing
                row = Nothing

                ' Column 0 (Vehicle Number)
                col = tblVEH.AddColumn(Unit.FromInch(1.25))
                col.Format.Alignment = ParagraphAlignment.Left
                ' column 1 (VIN)
                col = tblVEH.AddColumn(Unit.FromInch(1.75))
                col.Format.Alignment = ParagraphAlignment.Left
                ' Column 2 (Year)
                col = tblVEH.AddColumn(Unit.FromInch(1.75))
                col.Format.Alignment = ParagraphAlignment.Left
                ' column 3 (Make)
                col = tblVEH.AddColumn(Unit.FromInch(1.75))
                col.Format.Alignment = ParagraphAlignment.Left
                ' column 4 (Model)
                col = tblVEH.AddColumn(Unit.FromInch(1.75))
                col.Format.Alignment = ParagraphAlignment.Left

                ' Create a table row for each vehicle
                rowcount = 0
                For Each veh As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.SearchRequestVehicle In CLUEObj.SearchRequest.Vehicles
                    rowcount += 1
                    row = tblVEH.AddRow()
                    row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                    cell = row.Cells(0)
                    cell.AddParagraph(veh.VehicleNumDscr)
                    cell = row.Cells(1)
                    cell.AddParagraph(veh.VIN)
                    cell = row.Cells(2)
                    cell.AddParagraph(veh.Year)
                    cell = row.Cells(3)
                    cell.AddParagraph(veh.Make)
                Next

                tblVEH.SetEdge(0, 0, 4, rowcount, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                sec.Add(tblVEH)

                ' Calculate the height size of the vehicle list
                VehicleListSectionHeight = rowcount * 0.25

                ' ************************************
                ' REPORTED CLAIM HISTORY SECTION
                ' ************************************
                ' Header
                txt = "Reported Loss History with identification information that is underlined may not apply to this risk and should be verified prior to use.  This report is not a recommendation.  Subscriber should independently determine what action, if any, to take."
                paraCLAIMHISTHEADER = sec.AddParagraph()
                paraCLAIMHISTHEADER.Format.Font.Name = "Verdana"
                paraCLAIMHISTHEADER.Format.Font.Size = 8
                paraCLAIMHISTHEADER.Format.Alignment = ParagraphAlignment.Center
                paraCLAIMHISTHEADER.AddText("REPORTED CLAIM HISTORY")
                paraCLAIMHISTHEADER.AddLineBreak()
                paraCLAIMHISTHEADER.AddLineBreak()
                paraCLAIMHISTHEADER.AddText(txt)
                paraCLAIMHISTHEADER.Format.Font.Name = "Verdana"
                paraCLAIMHISTHEADER.Format.Font.Size = 8
                paraCLAIMHISTHEADER.Format.Font.Color = Colors.Black

                ' Data
                tblCLAIMHIST = New Tables.Table()
                tblCLAIMHIST.Borders.Width = 0
                tblCLAIMHIST.Format.Font.Name = "Verdana"
                tblCLAIMHIST.Format.Font.Size = 8
                tblCLAIMHIST.LeftPadding = 0
                tblCLAIMHIST.RightPadding = 0

                col = Nothing
                row = Nothing
                rowcount = 0
                ' Column 0 (Subj/Veh, Date/Age)
                col = tblCLAIMHIST.AddColumn(Unit.FromInch(0.75))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Borders.Right.Style = BorderStyle.Single
                ' column 1 (CLUE File #, Claim Number, Policy type & Company, Policy Number, Driver)
                col = tblCLAIMHIST.AddColumn(Unit.FromInch(4.75))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Borders.Right.Style = BorderStyle.Single
                ' Column 2 (Claim Type)
                col = tblCLAIMHIST.AddColumn(Unit.FromInch(0.75))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Borders.Right.Style = BorderStyle.Single
                ' column 3 (Amount Paid)
                col = tblCLAIMHIST.AddColumn(Unit.FromInch(0.75))
                col.Format.Alignment = ParagraphAlignment.Left

                ' Header Row
                row = tblCLAIMHIST.AddRow()
                rowcount += 1
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                row.HeightRule = Tables.RowHeightRule.Auto
                row.TopPadding = Unit.FromInch(0)
                row.BottomPadding = Unit.FromInch(0)
                row.Format.Borders.Bottom.Style = BorderStyle.Single

                cell = row.Cells(0)
                cell.AddParagraph("Subj/Veh" & vbCrLf & "Date/Age")
                cell = row.Cells(1)
                cell.AddParagraph("--CLUE File # ----   ---- Claim Number ---   ----- PolicyType & Company--" & vbCrLf & "---Policy Number---    ---- Driver-----")
                cell = row.Cells(2)
                cell.AddParagraph("Claim" & vbCrLf & "Type")
                cell = row.Cells(3)
                cell.AddParagraph("Amount" & vbCrLf & "Paid")

                ' Data rows
                z = -1
                If CLUEObj.ReportedClaimHistories IsNot Nothing AndAlso CLUEObj.ReportedClaimHistories.Count > 0 Then
                    For Each hist As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.ReportedClaimHistory In CLUEObj.ReportedClaimHistories
                        ' We need to know the number of detail lines from the onset
                        Dim NumDetailLines As Integer = 0
                        If hist.SubClaims Is Nothing OrElse hist.SubClaims.Trim() = "" Then
                            NumDetailLines = 4
                        Else
                            NumDetailLines = hist.SubClaims.Trim.Split(vbCrLf).Count + 1
                        End If

                        z += 1
                        rowcount += 1
                        row = tblCLAIMHIST.AddRow()
                        row.HeightRule = Tables.RowHeightRule.Auto
                        'row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        row.VerticalAlignment = Tables.VerticalAlignment.Top
                        row.BottomPadding = Unit.FromInch(0)
                        If z < CLUEObj.ReportedClaimHistories.Count - 1 Then row.Format.Borders.Bottom.Style = BorderStyle.Single
                        ' Column 0 - Subj/Veh  Date/Age
                        cell = row.Cells(0)
                        If hist.DriverDetail.Trim() = "" AndAlso hist.DateAgeInfo.Trim() = "" Then
                            cell.AddParagraph("")
                        Else
                            If hist.DriverDetail.Trim() <> "" AndAlso hist.DateAgeInfo.Trim() <> "" Then
                                txt = hist.DriverDetail.Trim() & vbCrLf & hist.DateAgeInfo.Trim()
                                cell.AddParagraph(AddPaddingLinesToText(txt, NumDetailLines, err))
                            ElseIf hist.DriverDetail <> "" Then
                                cell.AddParagraph(AddPaddingLinesToText(hist.DriverDetail.Trim(), NumDetailLines, err))
                            Else
                                cell.AddParagraph(AddPaddingLinesToText(hist.DateAgeInfo.Trim(), NumDetailLines, err))
                            End If
                        End If
                        cell.Format.Borders.Right.Style = BorderStyle.Single
                        ' Column 1- Details
                        cell = row.Cells(1)
                        'cell.Format.LeftIndent = Unit.FromInch(0.125)
                        If hist.SubClaims Is Nothing OrElse hist.SubClaims.Trim() = "" Then
                            cell.AddParagraph(AddPaddingLinesToText("", NumDetailLines, err))
                            'cell.AddParagraph("" & vbCrLf & "" & vbCrLf & "" & vbCrLf & "" & vbCrLf & "" & vbCrLf & "" & vbCrLf & "" & vbCrLf & "" & vbCrLf & vbCrLf & "" & vbCrLf)
                        Else
                            cell.AddParagraph(hist.SubClaims.Trim())
                        End If
                        'If hist.ClueFileNumber.Trim() = "" AndAlso hist.ClaimNumber.Trim() = "" AndAlso hist.PolicyType.Trim() = "" AndAlso hist.Company.Trim() = "" AndAlso hist.DriverDetail.Trim() = "" Then
                        '    cell.AddParagraph("" & vbCrLf & "" & vbCrLf & "" & vbCrLf & "" & vbCrLf)
                        'Else
                        '    txt = FormatClaimHistoryItemData(hist, err)
                        '    If err <> "" Then Throw New Exception(err)
                        '    cell.AddParagraph(txt)
                        'End If
                        ' Column 2 - Claim Type
                        cell = row.Cells(2)
                        txt = RemoveExtraLineBreaksFromText(hist.ClaimType, err)
                        If err <> "" Then Throw New Exception(err)
                        'txt = AddPaddingLinesToText(txt, 4, err)
                        txt = AddPaddingLinesToText(txt, NumDetailLines, err)
                        If err <> "" Then Throw New Exception(err)
                        cell.AddParagraph(txt)
                        ' Column 3 - Amount Paid
                        cell = row.Cells(3)
                        txt = RemoveExtraLineBreaksFromText(hist.AmountPaid, err)
                        If err <> "" Then Throw New Exception(err)
                        'txt = AddPaddingLinesToText(txt, 4, err)
                        txt = AddPaddingLinesToText(txt, NumDetailLines, err)
                        If err <> "" Then Throw New Exception(err)
                        cell.AddParagraph(txt)
                    Next
                Else
                    ' No data found
                    rowcount += 1
                    row = tblCLAIMHIST.AddRow()
                    row.HeightRule = Tables.RowHeightRule.Auto
                    row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                    row.BottomPadding = Unit.FromInch(0)
                    'row.Format.Borders.Bottom.Style = BorderStyle.Single
                    cell = row.Cells(0)
                    cell.AddParagraph("")

                    cell.Format.Borders.Top.Style = BorderStyle.Single
                    cell = row.Cells(1)
                    cell.AddParagraph(NoDataText)
                    cell.Format.Borders.Right.Style = BorderStyle.Single
                    cell = row.Cells(2)
                    cell.AddParagraph("")
                    cell.Format.Borders.Right.Style = BorderStyle.Single
                    cell = row.Cells(3)
                    cell.AddParagraph("")
                End If

                If CLUEObj.ReportedClaimHistories IsNot Nothing AndAlso CLUEObj.ReportedClaimHistories.Count <= 0 Then
                    ClaimHistorySectionHeightInches = 1
                Else
                    ' Calculate the size of this section - account for the header row
                    ClaimHistorySectionHeightInches = (0.3 + ((rowcount - 1) * ClaimHistoryItemHeightInches))
                End If

                tblCLAIMHIST.SetEdge(0, 0, 4, rowcount, Tables.Edge.Box, BorderStyle.Single, 1, Colors.Black)
                sec.Add(tblCLAIMHIST)

                ' ************************************
                ' VEHICLE SEARCH RESULTS SECTION
                ' Added data section 7/15/14 Bug 3467
                ' ************************************
                ' Header
                txt = "Vehicle claims imported in this section may have occured prior to the applicant / policy holder owning the vehicle.  For these claims, ownership of the vehicle at the time of the loss shoud be verified."
                paraVEHSEARCHHEADER = sec.AddParagraph()
                paraVEHSEARCHHEADER.Format.Font.Name = "Verdana"
                paraVEHSEARCHHEADER.Format.Font.Size = 8
                paraVEHSEARCHHEADER.Format.Alignment = ParagraphAlignment.Center
                paraVEHSEARCHHEADER.AddText("VEHICLE SEARCH RESULTS (Claims Not Reported Above)")
                paraVEHSEARCHHEADER.AddLineBreak()
                paraVEHSEARCHHEADER.AddLineBreak()
                paraVEHSEARCHHEADER.AddText(txt)
                paraVEHSEARCHHEADER.Format.Font.Name = "Verdana"
                paraVEHSEARCHHEADER.Format.Font.Size = 8
                paraVEHSEARCHHEADER.Format.Font.Color = Colors.Black

                ' Data
                tblVEHSEARCH = New Tables.Table()
                tblVEHSEARCH.Borders.Width = 0
                tblVEHSEARCH.Format.Font.Name = "Verdana"
                tblVEHSEARCH.Format.Font.Size = 8
                tblVEHSEARCH.LeftPadding = 0
                tblVEHSEARCH.RightPadding = 0

                col = Nothing
                row = Nothing
                rowcount = 0
                ' Column 0 (Subj/Veh, Date/Age)
                col = tblVEHSEARCH.AddColumn(Unit.FromInch(0.75))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Borders.Right.Style = BorderStyle.Single
                ' column 1 (CLUE File #, Claim Number, Policy type & Company, Policy Number, Driver)
                col = tblVEHSEARCH.AddColumn(Unit.FromInch(4.75))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Borders.Right.Style = BorderStyle.Single
                ' Column 2 (Claim Type)
                col = tblVEHSEARCH.AddColumn(Unit.FromInch(0.75))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Borders.Right.Style = BorderStyle.Single
                ' column 3 (Amount Paid)
                col = tblVEHSEARCH.AddColumn(Unit.FromInch(0.75))
                col.Format.Alignment = ParagraphAlignment.Left

                ' Header Row
                row = tblVEHSEARCH.AddRow()
                row.HeightRule = Tables.RowHeightRule.Auto
                row.TopPadding = Unit.FromInch(0)
                row.BottomPadding = Unit.FromInch(0)
                row.Format.Borders.Bottom.Style = BorderStyle.Single
                rowcount += 1
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("Subj/Veh" & vbCrLf & "Date/Age")
                cell = row.Cells(1)
                cell.AddParagraph("--CLUE File # ----   ---- Claim Number ---   ----- PolicyType & Company--" & vbCrLf & "---Policy Number---    ---- Driver-----")
                cell = row.Cells(2)
                cell.AddParagraph("Claim" & vbCrLf & "Type")
                cell = row.Cells(3)
                cell.AddParagraph("Amount" & vbCrLf & "Paid")

                ' Data rows
                z = -1
                If CLUEObj.VehicleSearchResults IsNot Nothing AndAlso CLUEObj.VehicleSearchResults.Count > 0 Then
                    For Each vsr As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.VehicleSearchResult In CLUEObj.VehicleSearchResults
                        Dim NumVehSearchLines As Integer = 0
                        If vsr.SubClaims Is Nothing OrElse vsr.SubClaims.Trim = "" Then
                            NumVehSearchLines = 4
                        Else
                            NumVehSearchLines = GetNumVehSearchLines(vsr, err) + 1
                        End If
                        z += 1
                        rowcount += 1
                        row = tblVEHSEARCH.AddRow()
                        row.HeightRule = Tables.RowHeightRule.Auto
                        'row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        row.VerticalAlignment = Tables.VerticalAlignment.Top
                        row.BottomPadding = Unit.FromInch(0)
                        If z < CLUEObj.VehicleSearchResults.Count - 1 Then row.Format.Borders.Bottom.Style = BorderStyle.Single
                        ' Column 0 - Date/Age Info
                        cell = row.Cells(0)
                        If vsr.DateAgeInfo.Trim() = "" Then
                            cell.AddParagraph(AddPaddingLinesToText("", NumVehSearchLines, err))
                        Else
                            txt = RemoveExtraLineBreaksFromText(vsr.DateAgeInfo.Trim(), err)
                            cell.AddParagraph(AddPaddingLinesToText(txt, NumVehSearchLines, err))
                        End If
                        cell.Format.Borders.Top.Style = BorderStyle.Single

                        ' Column 1 - Data Detail
                        cell = row.Cells(1)
                        If vsr.SubClaims IsNot Nothing AndAlso vsr.SubClaims.Trim <> "" Then
                            cell.AddParagraph(AddPaddingLinesToText(vsr.SubClaims.Trim(), NumVehSearchLines, err))
                            cell.Format.Borders.Top.Style = BorderStyle.Single
                        Else
                            cell.AddParagraph(AddPaddingLinesToText("", NumVehSearchLines, err))
                        End If

                        ' Column 3 - Claim Type
                        cell = row.Cells(2)
                        txt = RemoveExtraLineBreaksFromText(vsr.ClaimType.Trim(), err)
                        If err <> "" Then Throw New Exception(err)
                        txt = AddPaddingLinesToText(txt, NumVehSearchLines, err)
                        'If err <> "" Then Throw New Exception(err)
                        cell.AddParagraph(txt)
                        cell.Format.Borders.Top.Style = BorderStyle.Single

                        ' Column 3 - Amount Paid
                        cell = row.Cells(3)
                        txt = RemoveExtraLineBreaksFromText(vsr.AmountPaid.Trim(), err)
                        If err <> "" Then Throw New Exception(err)
                        txt = AddPaddingLinesToText(txt, NumVehSearchLines, err)
                        'If err <> "" Then Throw New Exception(err)
                        cell.AddParagraph(txt)
                        cell.Format.Borders.Top.Style = BorderStyle.Single
                    Next
                Else
                    ' No data found
                    rowcount += 1
                    row = tblVEHSEARCH.AddRow()
                    row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                    row.HeightRule = Tables.RowHeightRule.Auto
                    row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                    row.BottomPadding = Unit.FromInch(0)
                    'row.Format.Borders.Bottom.Style = BorderStyle.Single
                    cell = row.Cells(0)
                    cell.AddParagraph("")

                    cell.Format.Borders.Top.Style = BorderStyle.Single
                    cell = row.Cells(1)
                    cell.AddParagraph(NoDataText)
                    cell.Format.Borders.Top.Style = BorderStyle.Single
                    cell = row.Cells(2)
                    cell.AddParagraph("")
                    cell.Format.Borders.Top.Style = BorderStyle.Single
                    cell = row.Cells(3)
                    cell.AddParagraph("")
                    cell.Format.Borders.Top.Style = BorderStyle.Single
                End If

                tblVEHSEARCH.SetEdge(0, 0, 4, rowcount, Tables.Edge.Box, BorderStyle.Single, 1, Colors.Black)
                sec.Add(tblVEHSEARCH)

                If CLUEObj.VehicleSearchResults IsNot Nothing AndAlso CLUEObj.VehicleSearchResults.Count <= 0 Then
                    VehSearchResultsSectionHeightInches = 1
                Else
                    ' calculate the size of the vehicle search results section - factor in header row
                    VehSearchResultsSectionHeightInches = (0.3 + ((rowcount - 1) * VehSearchResultsItemHeightInches))
                End If

                ' ************************************
                ' POSSIBLE RELATED CLAIMS SECTION
                ' Added data section 7/15/14 Bug 3467
                ' ************************************
                ' Header
                paraPRCHEADER = sec.AddParagraph()
                paraPRCHEADER.Format.Alignment = ParagraphAlignment.Center
                paraPRCHEADER.Format.Font.Name = "Verdana"
                paraPRCHEADER.Format.Font.Size = 8
                paraPRCHEADER.Format.Font.Color = Colors.Black
                paraPRCHEADER.AddText("POSSIBLE RELATED CLAIMS (Based on Address)")

                ' Data Table
                tblPRC = New Tables.Table()
                tblPRC.Borders.Width = 0
                tblPRC.Format.Font.Name = "Verdana"
                tblPRC.Format.Font.Size = 8
                tblPRC.LeftPadding = 0
                tblPRC.RightPadding = 0

                col = Nothing
                row = Nothing
                rowcount = 0
                ' Column 0 (Date/Age Info)
                col = tblPRC.AddColumn(Unit.FromInch(0.75))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Borders.Right.Style = BorderStyle.Single
                ' column 1 (Detail)
                col = tblPRC.AddColumn(Unit.FromInch(4.75))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Borders.Right.Style = BorderStyle.Single
                ' column 2 (Claim Type)
                col = tblPRC.AddColumn(Unit.FromInch(0.75))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Borders.Right.Style = BorderStyle.Single
                ' Column 3 (Amount Paid)
                col = tblPRC.AddColumn(Unit.FromInch(0.75))
                col.Format.Alignment = ParagraphAlignment.Left

                ' Header Row
                row = tblPRC.AddRow()
                rowcount += 1
                row.HeightRule = Tables.RowHeightRule.Auto
                row.TopPadding = Unit.FromInch(0)
                row.BottomPadding = Unit.FromInch(0)
                row.Format.Borders.Bottom.Style = BorderStyle.Single
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph(" " & vbCrLf & "Date/Age")
                cell = row.Cells(1)
                cell.AddParagraph("--CLUE File # ----   ---- Claim Number ---   ----- PolicyType & Company--" & vbCrLf & "---Policy Number---    ---- Driver-----")
                cell = row.Cells(2)
                cell.AddParagraph("Claim" & vbCrLf & "Type")
                cell = row.Cells(3)
                cell.AddParagraph("Amount" & vbCrLf & "Paid")

                z = -1
                If CLUEObj.PossibleRelatedClaims IsNot Nothing AndAlso CLUEObj.PossibleRelatedClaims.Count > 0 Then
                    For Each prc As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.PossibleRelatedClaim In CLUEObj.PossibleRelatedClaims
                        Dim NumPRCLines As Integer = 0
                        If prc.SubClaims Is Nothing OrElse prc.SubClaims.Trim = "" Then
                            NumPRCLines = 4
                        Else
                            NumPRCLines = GetNumPRCLines(prc, err) + 1
                        End If
                        z += 1
                        row = tblPRC.AddRow()
                        rowcount += 1
                        row.HeightRule = Tables.RowHeightRule.Auto
                        row.TopPadding = Unit.FromInch(0)
                        row.BottomPadding = Unit.FromInch(0)
                        If z < CLUEObj.PossibleRelatedClaims.Count - 1 Then row.Format.Borders.Bottom.Style = BorderStyle.Single
                        row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        ' Date/Age Info
                        cell = row.Cells(0)
                        txt = RemoveExtraLineBreaksFromText(prc.DateAgeInfo, err)
                        If err <> "" Then Throw New Exception(err)
                        txt = AddPaddingLinesToText(txt, NumPRCLines, err)
                        If err <> "" Then Throw New Exception(err)
                        cell.AddParagraph(txt)
                        ' Data Detail
                        cell = row.Cells(1)
                        txt = RemoveExtraLineBreaksFromText(prc.SubClaims.Trim(), err)
                        If err <> "" Then Throw New Exception(err)
                        txt = AddPaddingLinesToText(txt, NumPRCLines, err)
                        If err <> "" Then Throw New Exception(err)
                        cell.AddParagraph(txt)
                        ' Claim Type
                        cell = row.Cells(2)
                        txt = RemoveExtraLineBreaksFromText(prc.ClaimType, err)
                        If err <> "" Then Throw New Exception(err)
                        txt = AddPaddingLinesToText(txt, NumPRCLines, err)
                        If err <> "" Then Throw New Exception(err)
                        cell.AddParagraph(txt)
                        ' Amount Paid
                        cell = row.Cells(3)
                        txt = RemoveExtraLineBreaksFromText(prc.AmountPaid, err)
                        If err <> "" Then Throw New Exception(err)
                        txt = AddPaddingLinesToText(txt, NumPRCLines, err)
                        If err <> "" Then Throw New Exception(err)
                        cell.AddParagraph(txt)
                    Next
                Else
                    ' No data found
                    rowcount += 1
                    row = tblPRC.AddRow()
                    row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                    row.HeightRule = Tables.RowHeightRule.Auto
                    row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                    row.BottomPadding = Unit.FromInch(0)
                    'row.Format.Borders.Bottom.Style = BorderStyle.Single
                    cell = row.Cells(0)
                    cell.AddParagraph("")

                    cell.Format.Borders.Top.Style = BorderStyle.Single
                    cell = row.Cells(1)
                    cell.AddParagraph(NoDataText)
                    cell.Format.Borders.Top.Style = BorderStyle.Single
                    cell = row.Cells(2)
                    cell.AddParagraph("")
                    cell.Format.Borders.Top.Style = BorderStyle.Single
                    cell = row.Cells(3)
                    cell.AddParagraph("")
                    cell.Format.Borders.Top.Style = BorderStyle.Single
                End If

                tblPRC.SetEdge(0, 0, 4, rowcount, Tables.Edge.Box, BorderStyle.Single, 1, Colors.Black)
                sec.Add(tblPRC)

                If CLUEObj.PossibleRelatedClaims IsNot Nothing AndAlso CLUEObj.PossibleRelatedClaims.Count <= 0 Then
                    PRCSectionHeightInches = 1
                Else
                    ' Calculate the height of the section - factor in the header row
                    PRCSectionHeightInches = (0.3 + ((rowcount - 1) * PRCItemHeightInches))
                End If

                ' ************************************
                ' POSSIBLE ADDITIONAL DRIVERS SECTION
                ' ************************************
                ' Header
                txt = "Additional driver may not reside in this household or be associated with insured.  This information should be independently verified prior to use.  This report is not a recommendation."
                paraPADHEADER = sec.AddParagraph()
                paraPADHEADER.Format.Font.Name = "Verdana"
                paraPADHEADER.Format.Font.Size = 8
                paraPADHEADER.Format.Alignment = ParagraphAlignment.Center
                paraPADHEADER.AddText("POSSIBLE ADDITIONAL DRIVERS")
                paraPADHEADER.AddLineBreak()
                paraPADHEADER.AddLineBreak()
                paraPADHEADER.AddText(txt)
                paraPADHEADER.Format.Font.Name = "Verdana"
                paraPADHEADER.Format.Font.Size = 8
                paraPADHEADER.Format.Font.Color = Colors.Black

                ' Data
                tblPAD = New Tables.Table()
                tblPAD.Borders.Width = 0
                tblPAD.Format.Font.Name = "Verdana"
                tblPAD.Format.Font.Size = 8
                tblPAD.LeftPadding = 0
                tblPAD.RightPadding = 0

                col = Nothing
                row = Nothing
                rowcount = 0
                ' Column 0 (Data)
                col = tblPAD.AddColumn(Unit.FromInch(8))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Borders.Right.Style = BorderStyle.Single

                ' Header Row
                row = tblPAD.AddRow()
                rowcount += 1
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                cell = row.Cells(0)
                cell.AddParagraph("Name    Driv. License #      SSN               Type" & vbCrLf & "DOB     Exp Date      Iss Date" & vbCrLf & "Restrict")
                cell.Format.Borders.Right.Style = BorderStyle.Single

                ' Data rows
                If CLUEObj.PossibleAdditionalDrivers IsNot Nothing AndAlso CLUEObj.PossibleAdditionalDrivers.Count > 0 Then
                    For Each pad As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.AdditionalDriver In CLUEObj.PossibleAdditionalDrivers
                        rowcount += 1
                        row = tblPAD.AddRow()
                        row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        cell = row.Cells(0)
                        cell.AddParagraph(pad.DataLine)
                        cell.Format.Borders.Top.Style = BorderStyle.Single
                    Next
                Else
                    rowcount += 1
                    row = tblPAD.AddRow()
                    row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                    cell = row.Cells(0)
                    cell.AddParagraph(NoDataText)
                    cell.Format.Borders.Top.Style = BorderStyle.Single
                End If

                tblPAD.SetEdge(0, 0, 1, rowcount, Tables.Edge.Box, BorderStyle.Single, 1, Colors.Black)
                sec.Add(tblPAD)

                ' calculate the size of the section - factor in header row
                PADSectionHeightInches = (0.5 + ((rowcount - 1) * PADItemHeightInches))

                ' FOOTER PARAGRAPHS (3)
                ' This one is the header (first) and footer (last) line of the footer section
                paraFooterCenter = sec.AddParagraph()
                paraFooterCenter.AddText(CLUEObj.PreparedBySection.FirstLine)
                paraFooterCenter.AddLineBreak()
                paraFooterCenter.AddText("LexisNexis, Inc")
                paraFooterCenter.AddLineBreak()
                paraFooterCenter.AddLineBreak()
                paraFooterCenter.AddLineBreak()
                paraFooterCenter.AddLineBreak()
                paraFooterCenter.AddLineBreak()
                paraFooterCenter.AddText(CLUEObj.PreparedBySection.LastLine)
                paraFooterCenter.Format.Alignment = ParagraphAlignment.Center
                paraFooterCenter.Format.Font.Name = "Verdana"
                paraFooterCenter.Format.Font.Size = 8
                paraFooterCenter.Format.Font.Color = Colors.Black

                ' Left Paragraph
                ' The prepared data lines are in the ClueObj.PreparedBySection.LeftColumnItems collection
                ' Loop through them and build the left footer paragraph
                paraFooterLeft = sec.AddParagraph()
                If CLUEObj.PreparedBySection.LeftColumnItems Is Nothing OrElse CLUEObj.PreparedBySection.LeftColumnItems.Count <= 0 Then Throw New Exception("Left Footer Info is missing")
                i = -1
                For Each ft As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.PreparedLeftColumnDataItem In CLUEObj.PreparedBySection.LeftColumnItems
                    i += 1
                    If i = 0 Then
                        ' The first line gets underlined
                        paraFooterLeft.AddFormattedText(ft.DataLine, TextFormat.Underline)
                    Else
                        paraFooterLeft.AddText(ft.DataLine)
                    End If
                    paraFooterLeft.AddLineBreak()
                Next
                paraFooterLeft.Format.Alignment = ParagraphAlignment.Left
                paraFooterLeft.Format.Font.Name = "Verdana"
                paraFooterLeft.Format.Font.Size = 8
                paraFooterLeft.Format.Font.Color = Colors.Black

                ' Right Paragraph
                ' The prepared data lines are in the ClueObj.PreparedBySection.LeftColumnItems collection
                ' Loop through them and build the right footer paragraph
                If CLUEObj.PreparedBySection.RightColumnItems Is Nothing OrElse CLUEObj.PreparedBySection.RightColumnItems.Count <= 0 Then Throw New Exception("Right Footer Info is missing")
                paraFooterRight = sec.AddParagraph()
                i = -1
                For Each ft As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.PreparedRightColumnDataItem In CLUEObj.PreparedBySection.RightColumnItems
                    i += 1
                    If i = 0 Then
                        ' The first line gets underlined
                        paraFooterRight.AddFormattedText(ft.DataLine, TextFormat.Underline)
                    Else
                        paraFooterRight.AddText(ft.DataLine)
                    End If
                    paraFooterRight.AddLineBreak()
                Next
                paraFooterRight.Format.Alignment = ParagraphAlignment.Left
                paraFooterRight.Format.Font.Name = "Verdana"
                paraFooterRight.Format.Font.Size = 8
                paraFooterRight.Format.Font.Color = Colors.Black

                ' ************************************************
                ' !!! R  E  N  D  E  R  !!!                    ***
                ' ************************************************
                ' ADD ALL THE FORMATTED OBJECTS TO THE DOCUMENT
                ' ************************************************
                docRenderer = New Rendering.DocumentRenderer(tempdoc)
                docRenderer.PrepareDocument()

                ' RENDER TITLE SECTION
                NextTopPos = 0.125
                docRenderer.RenderObject(gfx, 0, XUnit.FromInch(NextTopPos), CurrentPage.Width, paraTITLE)
                NextTopPos = NextTopPos + 0.5
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), paraORDERDATE)

                ' RENDER REPORT INFO SECTION
                NextTopPos = NextTopPos + 0.25
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(NextTopPos), XUnit.FromInch(7.75), tblRPTINFO)
                ' Divider below report info section
                NextTopPos = NextTopPos + 0.5
                gfx.DrawLine(XPens.Black, 0, XUnit.FromInch(NextTopPos), CurrentPage.Width, XUnit.FromInch(NextTopPos))

                ' RENDER RECAP SECTION
                NextTopPos = NextTopPos + 0.1
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), paraRECAP)
                NextTopPos = NextTopPos + RecapSectionHeightInches

                ' RENDER MESSAGES SECTION
                NextTopPos = NextTopPos + 0.25
                CheckForCLUEPageBreak("PPA", paraFooterCenter, paraFooterLeft, paraFooterRight, err)
                docRenderer.RenderObject(gfx, 0, XUnit.FromInch(NextTopPos), XUnit.FromInch(8.25), paraMSGHDR)
                NextTopPos = NextTopPos + 0.15
                ' Divider under messages header
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(NextTopPos), CurrentPage.Width, Unit.FromInch(NextTopPos))
                NextTopPos = NextTopPos + 0.05
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), paraMSG)
                NextTopPos = NextTopPos + MsgSectionHeightInches

                ' RENDER SEARCH SECTION
                ' Header
                CheckForCLUEPageBreak("PPA", paraFooterCenter, paraFooterLeft, paraFooterRight, err)
                docRenderer.RenderObject(gfx, 0, XUnit.FromInch(NextTopPos), CurrentPage.Width, paraSEARCHSECTIONHEADER)
                NextTopPos = NextTopPos + 0.15
                ' Divider under search section header
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(NextTopPos), CurrentPage.Width, Unit.FromInch(NextTopPos))

                ' Search Subjects
                i = -1
                For Each subjtbl As Tables.Table In tblSUBJ
                    i += 1
                    NextTopPos = NextTopPos + 0.1
                    CheckForCLUEPageBreak("PPA", paraFooterCenter, paraFooterLeft, paraFooterRight, err)
                    docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), paraSUBJECTHEADERS(i))
                    NextTopPos = NextTopPos + 0.15
                    docRenderer.RenderObject(gfx, 0, XUnit.FromInch(NextTopPos), CurrentPage.Width, subjtbl)
                    'If i < tblSUBJ.Count - 1 Then NextTopPos = NextTopPos + 0.75
                    NextTopPos = NextTopPos + 0.6
                Next

                ' RENDER PRIOR SECTION
                CheckForCLUEPageBreak("PPA", paraFooterCenter, paraFooterLeft, paraFooterRight, err)
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), paraPRIORHEADER)
                NextTopPos = NextTopPos + 0.15
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), tblPRIOR)
                NextTopPos = NextTopPos + 0.25

                ' RENDER VEHICLES SECTION
                CheckForCLUEPageBreak("PPA", paraFooterCenter, paraFooterLeft, paraFooterRight, err, VehicleListSectionHeight)
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), paraVEHICLEHEADER)
                NextTopPos = NextTopPos + 0.25
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), tblVEH)
                NextTopPos = NextTopPos + VehicleListSectionHeight

                ' RENDER REPORTED CLAIM HISTORY SECTION
                CheckForCLUEPageBreak("PPA", paraFooterCenter, paraFooterLeft, paraFooterRight, err, ClaimHistorySectionHeightInches)
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), paraCLAIMHISTHEADER)
                ' Divider underneath the title
                NextTopPos = NextTopPos + 0.2
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(NextTopPos), CurrentPage.Width, Unit.FromInch(NextTopPos))
                NextTopPos = NextTopPos + 0.45
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.75), XUnit.FromInch(NextTopPos), XUnit.FromInch(7), tblCLAIMHIST)

                ' RENDER VEHICLE SEARCH RESULTS SECTION
                NextTopPos = NextTopPos + ClaimHistorySectionHeightInches
                CheckForCLUEPageBreak("PPA", paraFooterCenter, paraFooterLeft, paraFooterRight, err, VehSearchResultsSectionHeightInches)
                NextTopPos = NextTopPos + 0.2
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), paraVEHSEARCHHEADER)
                ' Divider underneath the title
                NextTopPos = NextTopPos + 0.2
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(NextTopPos), CurrentPage.Width, Unit.FromInch(NextTopPos))
                NextTopPos = NextTopPos + 0.45
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.75), XUnit.FromInch(NextTopPos), XUnit.FromInch(7), tblVEHSEARCH)

                ' RENDER POSSIBLE RELATED CLAIMS SECTION
                NextTopPos = NextTopPos + VehSearchResultsSectionHeightInches
                CheckForCLUEPageBreak("PPA", paraFooterCenter, paraFooterLeft, paraFooterRight, err, PRCSectionHeightInches)
                NextTopPos = NextTopPos + 0.2
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), paraPRCHEADER)
                ' Divider underneath the title
                NextTopPos = NextTopPos + 0.2
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(NextTopPos), CurrentPage.Width, Unit.FromInch(NextTopPos))
                NextTopPos = NextTopPos + 0.2
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.75), XUnit.FromInch(NextTopPos), XUnit.FromInch(7), tblPRC)

                ' RENDER POSSIBLE ADDITIONAL DRIVERS SECTION
                NextTopPos = NextTopPos + PRCSectionHeightInches
                CheckForCLUEPageBreak("PPA", paraFooterCenter, paraFooterLeft, paraFooterRight, err, PADSectionHeightInches)
                NextTopPos = NextTopPos + 0.2
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), paraPADHEADER)
                ' Divider underneath the title
                NextTopPos = NextTopPos + 0.2
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(NextTopPos), CurrentPage.Width, Unit.FromInch(NextTopPos))
                NextTopPos = NextTopPos + 0.45
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), tblPAD)

                ' This will draw the footers on the last page
                DrawCLUEFooter("PPA", paraFooterCenter, paraFooterLeft, paraFooterRight, err)

                Exit Sub
            Catch ex As Exception
                err = ex.Message
                Exit Sub
            End Try
        End Sub

        ''' <summary>
        ''' Generate CLUE report pdf for Home Personal
        ''' </summary>
        ''' <param name="qo"></param>
        ''' <param name="reportdata"></param>
        ''' <param name="CLUEObj"></param>
        ''' <param name="err"></param>
        ''' <remarks></remarks>
        Private Sub Generate_CLUE_Report_HOM(ByVal qo As QuickQuote.CommonObjects.QuickQuoteObject, ByVal reportdata As String, ByVal CLUEObj As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalProperty.ReportData, ByRef err As String)
            Dim ClaimHistoryRISKSectionHeightInches As Decimal = 0
            Dim ClaimHistorySUBJSectionHeightInches As Decimal = 0
            Dim DataFound As Boolean = False
            Dim z As Integer = -1

            ' Constants
            Const ClaimHistoryItemHeightInches As Decimal = 1.2

            ' Paragraphs
            Dim paraTITLE As Paragraph = Nothing
            Dim paraSEARCHSECTIONHEADER As Paragraph = Nothing
            Dim paraCLAIMHISTHEADERRISK As Paragraph = Nothing
            Dim paraCLAIMHISTHEADERSUBJ As Paragraph = Nothing
            Dim paraADDLINFOHEADER As Paragraph = Nothing
            Dim paraADDLINFO As Paragraph = Nothing
            Dim paraINQUIRYHEADER As Paragraph = Nothing
            Dim paraINQUIRYHISTORY As Paragraph = Nothing

            ' Other than first page footer
            Dim paraFooterCenter As Paragraph = Nothing
            Dim paraFooterLeft As Paragraph = Nothing
            Dim paraFooterRight As Paragraph = Nothing

            ' First page footer
            'Dim paraFooterCenter1 As Paragraph = Nothing
            'Dim paraFooterLeft1 As Paragraph = Nothing
            'Dim paraFooterRight1 As Paragraph = Nothing
            'Dim paraFooterCenterBottom1 As Paragraph = Nothing

            ' Tables
            Dim tblRECAP As Tables.Table = Nothing
            Dim tblRPTINFO As Tables.Table = Nothing
            Dim tblCLAIMHISTRISK As Tables.Table = Nothing
            Dim tblCLAIMHISTSUBJ As Tables.Table = Nothing
            Dim tblSEARCH As Tables.Table = Nothing
            Dim tblADDLINFO_INFO As Tables.Table = Nothing

            Dim FormattedText As FormattedText = Nothing
            Dim ai As HOM_CLUE_AddtionalInfo_structure = Nothing
            Dim txt As String = Nothing

            Try
                CurrentPageNum = 1
                CurrentPage = doc.AddPage()
                gfx = XGraphics.FromPdfPage(CurrentPage)
                gfx.MUH = PdfFontEncoding.Unicode
                gfx.MFEH = PdfFontEmbedding.Default
                font = New XFont("Verdana", 13, XFontStyle.Bold)

                ' Document Body
                tempdoc = New Document()
                sec = tempdoc.AddSection()
                sec.PageSetup.StartingNumber = 1

                ' *******************
                ' TITLE SECTION
                ' *******************
                paraTITLE = sec.AddParagraph("C.L.U.E. - COMPREHENSIVE LOSS UNDERWRITING EXCHANGE")
                paraTITLE.Format.Font.Name = "Verdana"
                paraTITLE.Format.Font.Size = 10
                paraTITLE.Format.Font.Bold = True
                paraTITLE.Format.Alignment = ParagraphAlignment.Center
                paraTITLE.AddLineBreak()
                paraTITLE.AddText("PERSONAL PROPERTY SYSTEM")

                ' Divider underneath the title
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(0.5), CurrentPage.Width, Unit.FromInch(0.5))

                ' ********************
                ' REPORT INFO SECTION
                ' ********************
                tblRPTINFO = New Tables.Table
                tblRPTINFO.Borders.Width = 0
                tblRPTINFO.Format.Font.Name = "Verdana"
                tblRPTINFO.Format.Font.Size = 8

                col = Nothing
                row = Nothing

                ' TOTAL TABLE WIDTH = 7.75 INCHES
                ' Column 0 (Label Column 1)
                col = tblRPTINFO.AddColumn(Unit.FromInch(1.25))
                col.Format.Alignment = ParagraphAlignment.Left
                ' column 1 (Data Column 1)
                col = tblRPTINFO.AddColumn(Unit.FromInch(3))
                col.Format.Alignment = ParagraphAlignment.Left
                ' Column 2 (Label Column 2)
                col = tblRPTINFO.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Left
                ' column 3 (Data Column 2)
                col = tblRPTINFO.AddColumn(Unit.FromInch(2))
                col.Format.Alignment = ParagraphAlignment.Left

                ' Row 1
                row = tblRPTINFO.AddRow()
                cell = row.Cells(0)
                cell.AddParagraph("Quoteback:")
                cell = row.Cells(1)
                cell.AddParagraph(CLUEObj.GeneralReport.QuoteBack)
                cell = row.Cells(2)
                cell.AddParagraph("Date of Order:")
                cell = row.Cells(3)
                cell.AddParagraph(CLUEObj.GeneralReport.OrderDate)
                ' Row 2
                row = tblRPTINFO.AddRow()
                cell = row.Cells(0)
                cell.AddParagraph("Account:")
                cell = row.Cells(1)
                cell.AddParagraph(CLUEObj.GeneralReport.AccountNumber & " " & CLUEObj.GeneralReport.Company)
                cell = row.Cells(2)
                cell.AddParagraph("Date of Receipt:")
                cell = row.Cells(3)
                cell.AddParagraph(CLUEObj.GeneralReport.ReceiptDate)
                ' Row 3
                row = tblRPTINFO.AddRow()
                cell = row.Cells(0)
                cell.AddParagraph("Requestor:")
                cell = row.Cells(1)
                cell.AddParagraph(CLUEObj.GeneralReport.Requestor)
                cell = row.Cells(2)
                cell.AddParagraph("C.L.U.E. Ref #:")
                cell = row.Cells(3)
                cell.AddParagraph(CLUEObj.GeneralReport.ClueReference)

                tblRPTINFO.SetEdge(0, 0, 4, 3, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                sec.Add(tblRPTINFO)

                ' *******************
                ' RECAP SECTION
                ' *******************
                z = 0

                tblRECAP = New Tables.Table
                tblRECAP.Borders.Width = 0
                tblRECAP.Format.Font.Name = "Verdana"
                tblRECAP.Format.Font.Size = 8

                col = Nothing
                row = Nothing

                ' Column 0 (Label)
                col = tblRECAP.AddColumn(Unit.FromInch(1.25))
                col.Format.Alignment = ParagraphAlignment.Left
                ' column 1 (data)
                col = tblRECAP.AddColumn(Unit.FromInch(5.75))
                col.Format.Alignment = ParagraphAlignment.Left

                row = tblRECAP.AddRow()
                cell = row.Cells(0)
                cell.AddParagraph("RECAP:")
                cell = row.Cells(1)
                cell.AddParagraph(CLUEObj.GeneralReport.Recap)

                tblRECAP.SetEdge(0, 0, 2, 1, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                sec.Add(tblRECAP)

                ' *******************
                ' SEARCH SECTION
                ' *******************
                paraSEARCHSECTIONHEADER = sec.AddParagraph()
                paraSEARCHSECTIONHEADER.Format.Font.Name = "Verdana"
                paraSEARCHSECTIONHEADER.Format.Font.Size = 8
                paraSEARCHSECTIONHEADER.Format.Alignment = ParagraphAlignment.Center
                paraSEARCHSECTIONHEADER.AddText("SEARCH REQUEST")

                If CLUEObj.SearchRequest Is Nothing Then Throw New Exception("No Search Request Data found")

                tblSEARCH = New Tables.Table
                tblSEARCH.Borders.Width = 0
                tblSEARCH.Format.Font.Name = "Verdana"
                tblSEARCH.Format.Font.Size = 8

                col = Nothing
                row = Nothing

                ' Column 0 (Label)
                col = tblSEARCH.AddColumn(Unit.FromInch(1.25))
                col.Format.Alignment = ParagraphAlignment.Left
                ' column 1 (data)
                col = tblSEARCH.AddColumn(Unit.FromInch(5.75))
                col.Format.Alignment = ParagraphAlignment.Left

                ' Subject 1
                row = tblSEARCH.AddRow()
                cell = row.Cells(0)
                cell.AddParagraph(AddPaddingLinesToText("Subject 1 Name:", 3, err))
                If err <> "" Then Throw New Exception(err)
                cell = row.Cells(1)
                txt = RemoveExtraLineBreaksFromText(CLUEObj.SearchRequest.Subject1, err)
                If err <> "" Then Throw New Exception(err)
                ' Don't remove the telephone MGB 11/18/14
                'txt = HOM_CLUE_Remove_PhoneNumberFromText(txt, err)
                'If err <> "" Then Throw New Exception(err)
                cell.AddParagraph(txt)

                ' Blank Row
                row = tblSEARCH.AddRow()

                ' Subject 2
                row = tblSEARCH.AddRow()
                cell = row.Cells(0)
                cell.AddParagraph(AddPaddingLinesToText("Subject 2 Name:", 3, err))
                If err <> "" Then Throw New Exception(err)
                cell = row.Cells(1)
                txt = RemoveExtraLineBreaksFromText(CLUEObj.SearchRequest.Subject2, err)
                If err <> "" Then Throw New Exception(err)
                ' Don't remove the telephone MGB 11/18/14
                'txt = HOM_CLUE_Remove_PhoneNumberFromText(txt, err)
                'If err <> "" Then Throw New Exception(err)
                cell.AddParagraph(txt)

                ' Blank Row
                row = tblSEARCH.AddRow()

                ' Risk Address
                row = tblSEARCH.AddRow()
                cell = row.Cells(0)
                cell.AddParagraph(AddPaddingLinesToText("Risk Address:", 2, err))
                If err <> "" Then Throw New Exception(err)
                cell = row.Cells(1)
                cell.AddParagraph(RemoveExtraLineBreaksFromText(CLUEObj.SearchRequest.RiskAddress, err))
                If err <> "" Then Throw New Exception(err)

                ' Blank Row
                row = tblSEARCH.AddRow()

                ' Mailing Address
                row = tblSEARCH.AddRow()
                cell = row.Cells(0)
                cell.AddParagraph(AddPaddingLinesToText("Mailing Address:", 2, err))
                If err <> "" Then Throw New Exception(err)
                cell = row.Cells(1)
                cell.AddParagraph(RemoveExtraLineBreaksFromText(CLUEObj.SearchRequest.MailingAddress, err))
                If err <> "" Then Throw New Exception(err)

                tblSEARCH.SetEdge(0, 0, 2, 4, Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                sec.Add(tblSEARCH)

                ' ************************************
                ' REPORTED CLAIM HISTORY SECTION - RISK
                ' ************************************
                ' Header
                txt = "Reported Loss History with identification information that is underlined may not apply to this risk and should be verified prior to use.  This report is not a recommendation.  Subscriber should independently determine what action, if any, to take."
                paraCLAIMHISTHEADERRISK = sec.AddParagraph()
                paraCLAIMHISTHEADERRISK.Format.Font.Name = "Verdana"
                paraCLAIMHISTHEADERRISK.Format.Font.Size = 8
                paraCLAIMHISTHEADERRISK.Format.Alignment = ParagraphAlignment.Center
                paraCLAIMHISTHEADERRISK.AddText("REPORTED CLAIM HISTORY FOR RISK")
                paraCLAIMHISTHEADERRISK.AddLineBreak()
                paraCLAIMHISTHEADERRISK.AddLineBreak()
                paraCLAIMHISTHEADERRISK.AddText(txt)
                paraCLAIMHISTHEADERRISK.Format.Font.Name = "Verdana"
                paraCLAIMHISTHEADERRISK.Format.Font.Size = 8
                paraCLAIMHISTHEADERRISK.Format.Font.Color = Colors.Black

                ' Data
                tblCLAIMHISTRISK = New Tables.Table()
                tblCLAIMHISTRISK.Borders.Width = 0
                tblCLAIMHISTRISK.Format.Font.Name = "Verdana"
                tblCLAIMHISTRISK.Format.Font.Size = 8
                tblCLAIMHISTRISK.LeftPadding = 0
                tblCLAIMHISTRISK.RightPadding = 0

                col = Nothing
                row = Nothing
                rowcount = 0
                ' Column 0 (Subj/Veh, Date/Age)
                col = tblCLAIMHISTRISK.AddColumn(Unit.FromInch(1.75))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Borders.Right.Style = BorderStyle.Single
                ' column 1 (CLUE File #, Claim Number, Policy type & Company, Policy Number, Driver)
                col = tblCLAIMHISTRISK.AddColumn(Unit.FromInch(3.75))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Borders.Right.Style = BorderStyle.Single
                ' Column 2 (Claim Type)
                col = tblCLAIMHISTRISK.AddColumn(Unit.FromInch(0.75))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Borders.Right.Style = BorderStyle.Single
                ' column 3 (Amount Paid)
                col = tblCLAIMHISTRISK.AddColumn(Unit.FromInch(0.75))
                col.Format.Alignment = ParagraphAlignment.Left

                ' Header Row
                row = tblCLAIMHISTRISK.AddRow()
                rowcount += 1
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                row.HeightRule = Tables.RowHeightRule.Auto
                row.TopPadding = Unit.FromInch(0)
                row.BottomPadding = Unit.FromInch(0)
                row.Format.Borders.Bottom.Style = BorderStyle.Single

                cell = row.Cells(0)
                cell.AddParagraph("Claim" & vbCrLf & "Date/Age" & vbCrLf & vbCrLf)
                cell = row.Cells(1)
                cell.AddParagraph("--CLUE File # ----   ---- AM BEST # ---   ----- Claim Number --" & vbCrLf & "--- Policy Type & Company --- ---Policy Number---" & vbCrLf & "--- Insured/Risk Address -------------")
                cell = row.Cells(2)
                cell.AddParagraph("Cause" & vbCrLf & "of Loss" & vbCrLf & vbCrLf)
                cell = row.Cells(3)
                cell.AddParagraph("Amount" & vbCrLf & "Paid" & vbCrLf & vbCrLf)

                ' Data rows
                z = -1
                If CLUEObj.RCHForRisk IsNot Nothing AndAlso CLUEObj.RCHForRisk.Count > 0 Then
                    For Each hist As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalProperty.ReportedClaimHistoryForRisk In CLUEObj.RCHForRisk
                        ' We need to know the number of detail lines from the onset
                        Dim NumDetailLines As Integer = 0
                        If hist.claimHistoryRisk Is Nothing OrElse hist.claimHistoryRisk.Trim() = "" Then
                            NumDetailLines = 4
                        Else
                            txt = RemoveExtraLineBreaksFromText(hist.claimHistoryRisk, err)
                            If err <> "" Then Throw New Exception(err)
                            txt = HOM_CLUE_EditDataLine(txt, err)
                            If err <> "" Then Throw New Exception(err)
                            NumDetailLines = txt.Split(vbCrLf).Count
                        End If
                        rowcount += 1

                        z += 1
                        'rowcount += 1
                        row = tblCLAIMHISTRISK.AddRow()
                        row.HeightRule = Tables.RowHeightRule.Auto
                        'row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        row.VerticalAlignment = Tables.VerticalAlignment.Top
                        row.BottomPadding = Unit.FromInch(0)
                        If z < CLUEObj.RCHForRisk.Count - 1 Then row.Format.Borders.Bottom.Style = BorderStyle.Single

                        ' Column 0 - Claim Date/Age
                        cell = row.Cells(0)
                        txt = RemoveExtraLineBreaksFromText(hist.dateage.Trim(), err)
                        If err <> "" Then Throw New Exception(err)
                        txt = AddPaddingLinesToText(txt, NumDetailLines, err)
                        If err <> "" Then Throw New Exception(err)
                        cell.AddParagraph(txt)
                        cell.Format.Borders.Right.Style = BorderStyle.Single

                        ' Column 1- Details
                        cell = row.Cells(1)
                        If hist.claimHistoryRisk Is Nothing OrElse hist.claimHistoryRisk.Trim() = "" Then
                            txt = AddPaddingLinesToText("", NumDetailLines, err)
                            If err <> "" Then Throw New Exception(err)
                            cell.AddParagraph(txt)
                        Else
                            txt = RemoveExtraLineBreaksFromText(hist.claimHistoryRisk.Trim(), err)
                            If err <> "" Then Throw New Exception(err)
                            txt = HOM_CLUE_EditDataLine(txt, err)
                            If err <> "" Then Throw New Exception(err)
                            txt = AddPaddingLinesToText(txt, NumDetailLines, err)
                            If err <> "" Then Throw New Exception(err)
                            cell.AddParagraph(txt)
                        End If

                        ' Column 2 - Cause of Loss
                        cell = row.Cells(2)
                        txt = RemoveExtraLineBreaksFromText(hist.Cause.Trim(), err)
                        If err <> "" Then Throw New Exception(err)
                        txt = AddPaddingLinesToText(txt, NumDetailLines, err)
                        If err <> "" Then Throw New Exception(err)
                        cell.AddParagraph(txt)

                        ' Column 3 - Amount Paid
                        cell = row.Cells(3)
                        txt = RemoveExtraLineBreaksFromText(hist.Amount.Trim(), err)
                        If err <> "" Then Throw New Exception(err)
                        txt = AddPaddingLinesToText(txt, NumDetailLines, err)
                        If err <> "" Then Throw New Exception(err)
                        cell.AddParagraph(txt)
                    Next
                Else
                    ' No data found
                    rowcount += 1
                    row = tblCLAIMHISTRISK.AddRow()
                    row.HeightRule = Tables.RowHeightRule.Auto
                    row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                    row.BottomPadding = Unit.FromInch(0)
                    'row.Format.Borders.Bottom.Style = BorderStyle.Single
                    cell = row.Cells(0)
                    cell.AddParagraph("")

                    cell.Format.Borders.Top.Style = BorderStyle.Single
                    cell = row.Cells(1)
                    cell.AddParagraph(NoDataText)
                    cell.Format.Borders.Right.Style = BorderStyle.Single
                    cell = row.Cells(2)
                    cell.AddParagraph("")
                    cell.Format.Borders.Right.Style = BorderStyle.Single
                    cell = row.Cells(3)
                    cell.AddParagraph("")
                End If

                If CLUEObj.RCHForRisk IsNot Nothing AndAlso CLUEObj.RCHForRisk.Count <= 0 Then
                    ClaimHistoryRISKSectionHeightInches = 1
                Else
                    ' Calculate the size of this section - account for the header row
                    If rowcount = 1 Then
                        ClaimHistoryRISKSectionHeightInches = (1.3 * ClaimHistoryItemHeightInches)
                    Else
                        ClaimHistoryRISKSectionHeightInches = (0.3 + ((rowcount - 1) * ClaimHistoryItemHeightInches))
                    End If
                End If

                tblCLAIMHISTRISK.SetEdge(0, 0, 4, rowcount, Tables.Edge.Box, BorderStyle.Single, 1, Colors.Black)
                sec.Add(tblCLAIMHISTRISK)

                ' ************************************
                ' REPORTED CLAIM HISTORY SECTION - SUBJECT
                ' ************************************
                ' Header
                txt = "Reported Loss History with identification information that is underlined may not apply to this risk and should be verified prior to use.  This report is not a recommendation.  Subscriber should independently determine what action, if any, to take."
                paraCLAIMHISTHEADERSUBJ = sec.AddParagraph()
                paraCLAIMHISTHEADERSUBJ.Format.Font.Name = "Verdana"
                paraCLAIMHISTHEADERSUBJ.Format.Font.Size = 8
                paraCLAIMHISTHEADERSUBJ.Format.Alignment = ParagraphAlignment.Center
                paraCLAIMHISTHEADERSUBJ.AddText("REPORTED CLAIM HISTORY FOR SUBJECT")
                paraCLAIMHISTHEADERSUBJ.AddLineBreak()
                paraCLAIMHISTHEADERSUBJ.AddLineBreak()
                paraCLAIMHISTHEADERSUBJ.AddText(txt)
                paraCLAIMHISTHEADERSUBJ.Format.Font.Name = "Verdana"
                paraCLAIMHISTHEADERSUBJ.Format.Font.Size = 8
                paraCLAIMHISTHEADERSUBJ.Format.Font.Color = Colors.Black

                ' Data
                tblCLAIMHISTSUBJ = New Tables.Table()
                tblCLAIMHISTSUBJ.Borders.Width = 0
                tblCLAIMHISTSUBJ.Format.Font.Name = "Verdana"
                tblCLAIMHISTSUBJ.Format.Font.Size = 8
                tblCLAIMHISTSUBJ.LeftPadding = 0
                tblCLAIMHISTSUBJ.RightPadding = 0

                col = Nothing
                row = Nothing
                rowcount = 0
                ' Column 0 (Subj/Veh, Date/Age)
                col = tblCLAIMHISTSUBJ.AddColumn(Unit.FromInch(1.75))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Borders.Right.Style = BorderStyle.Single
                ' column 1 (CLUE File #, Claim Number, Policy type & Company, Policy Number, Driver)
                col = tblCLAIMHISTSUBJ.AddColumn(Unit.FromInch(3.75))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Borders.Right.Style = BorderStyle.Single
                ' Column 2 (Claim Type)
                col = tblCLAIMHISTSUBJ.AddColumn(Unit.FromInch(0.75))
                col.Format.Alignment = ParagraphAlignment.Left
                col.Format.Borders.Right.Style = BorderStyle.Single
                ' column 3 (Amount Paid)
                col = tblCLAIMHISTSUBJ.AddColumn(Unit.FromInch(0.75))
                col.Format.Alignment = ParagraphAlignment.Left

                ' Header Row
                row = tblCLAIMHISTSUBJ.AddRow()
                rowcount += 1
                row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                row.HeightRule = Tables.RowHeightRule.Auto
                row.TopPadding = Unit.FromInch(0)
                row.BottomPadding = Unit.FromInch(0)
                row.Format.Borders.Bottom.Style = BorderStyle.Single

                cell = row.Cells(0)
                cell.AddParagraph("Claim" & vbCrLf & "Date/Age" & vbCrLf & vbCrLf)
                cell = row.Cells(1)
                cell.AddParagraph("--CLUE File # ----   ---- AM BEST # ---   ----- Claim Number --" & vbCrLf & "--- Policy Type & Company --- ---Policy Number---" & vbCrLf & "--- Insured/Risk Address -------------")
                cell = row.Cells(2)
                cell.AddParagraph("Cause" & vbCrLf & "of Loss" & vbCrLf & vbCrLf)
                cell = row.Cells(3)
                cell.AddParagraph("Amount" & vbCrLf & "Paid" & vbCrLf & vbCrLf)

                ' Data rows
                z = -1
                If CLUEObj.RCHForSubject IsNot Nothing AndAlso CLUEObj.RCHForSubject.Count > 0 Then
                    For Each hist As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalProperty.ReportedClaimHistoryForSubject In CLUEObj.RCHForSubject
                        ' We need to know the number of detail lines from the onset
                        Dim NumDetailLines As Integer = 0
                        If hist.claimHistoryRisk Is Nothing OrElse hist.claimHistoryRisk.Trim() = "" Then
                            NumDetailLines = 4
                        Else
                            txt = RemoveExtraLineBreaksFromText(hist.claimHistoryRisk, err)
                            If err <> "" Then Throw New Exception(err)
                            txt = HOM_CLUE_EditDataLine(txt, err)
                            If err <> "" Then Throw New Exception(err)
                            NumDetailLines = txt.Split(vbCrLf).Count + 1
                        End If
                        rowcount += 1

                        z += 1
                        'rowcount += 1
                        row = tblCLAIMHISTSUBJ.AddRow()
                        row.HeightRule = Tables.RowHeightRule.Auto
                        'row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                        row.VerticalAlignment = Tables.VerticalAlignment.Top
                        row.BottomPadding = Unit.FromInch(0)
                        If z < CLUEObj.RCHForSubject.Count - 1 Then row.Format.Borders.Bottom.Style = BorderStyle.Single

                        ' Column 0 - Claim Date/Age
                        cell = row.Cells(0)
                        txt = RemoveExtraLineBreaksFromText(hist.dateage.Trim(), err)
                        If err <> "" Then Throw New Exception(err)
                        txt = AddPaddingLinesToText(txt, NumDetailLines, err)
                        If err <> "" Then Throw New Exception(err)
                        cell.AddParagraph(txt)
                        cell.Format.Borders.Right.Style = BorderStyle.Single

                        ' Column 1- Details
                        cell = row.Cells(1)
                        If hist.claimHistoryRisk Is Nothing OrElse hist.claimHistoryRisk.Trim() = "" Then
                            txt = AddPaddingLinesToText("", NumDetailLines, err)
                            If err <> "" Then Throw New Exception(err)
                            cell.AddParagraph(txt)
                        Else
                            txt = RemoveExtraLineBreaksFromText(hist.claimHistoryRisk.Trim(), err)
                            If err <> "" Then Throw New Exception(err)
                            txt = HOM_CLUE_EditDataLine(txt, err)
                            If err <> "" Then Throw New Exception(err)
                            txt = AddPaddingLinesToText(txt, NumDetailLines, err)
                            If err <> "" Then Throw New Exception(err)
                            cell.AddParagraph(txt)
                        End If

                        ' Column 2 - Cause of Loss
                        cell = row.Cells(2)
                        txt = RemoveExtraLineBreaksFromText(hist.Cause.Trim(), err)
                        If err <> "" Then Throw New Exception(err)
                        txt = AddPaddingLinesToText(txt, NumDetailLines, err)
                        If err <> "" Then Throw New Exception(err)
                        cell.AddParagraph(txt)

                        ' Column 3 - Amount Paid
                        cell = row.Cells(3)
                        txt = RemoveExtraLineBreaksFromText(hist.Amount.Trim(), err)
                        If err <> "" Then Throw New Exception(err)
                        txt = AddPaddingLinesToText(txt, NumDetailLines, err)
                        If err <> "" Then Throw New Exception(err)
                        cell.AddParagraph(txt)
                    Next
                Else
                    ' No data found
                    rowcount += 1
                    row = tblCLAIMHISTSUBJ.AddRow()
                    row.HeightRule = Tables.RowHeightRule.Auto
                    row.VerticalAlignment = Tables.VerticalAlignment.Bottom
                    row.BottomPadding = Unit.FromInch(0)
                    'row.Format.Borders.Bottom.Style = BorderStyle.Single
                    cell = row.Cells(0)
                    cell.AddParagraph("")

                    cell.Format.Borders.Top.Style = BorderStyle.Single
                    cell = row.Cells(1)
                    cell.AddParagraph(NoDataText)
                    cell.Format.Borders.Right.Style = BorderStyle.Single
                    cell = row.Cells(2)
                    cell.AddParagraph("")
                    cell.Format.Borders.Right.Style = BorderStyle.Single
                    cell = row.Cells(3)
                    cell.AddParagraph("")
                End If

                If CLUEObj.RCHForSubject IsNot Nothing AndAlso CLUEObj.RCHForSubject.Count <= 0 Then
                    ClaimHistorySUBJSectionHeightInches = 1
                Else
                    ' Calculate the size of this section - account for the header row
                    If rowcount = 1 Then
                        ClaimHistorySUBJSectionHeightInches = (1.3 * ClaimHistoryItemHeightInches)
                    Else
                        ClaimHistorySUBJSectionHeightInches = (0.3 + ((rowcount - 1) * ClaimHistoryItemHeightInches))
                    End If
                End If

                tblCLAIMHISTSUBJ.SetEdge(0, 0, 4, rowcount, Tables.Edge.Box, BorderStyle.Single, 1, Colors.Black)
                sec.Add(tblCLAIMHISTSUBJ)

                ' ************************************
                ' ADDITIONAL INFORMATION SECTION
                ' ************************************
                ' Header
                paraADDLINFOHEADER = sec.AddParagraph()
                paraADDLINFOHEADER.Format.Font.Name = "Verdana"
                paraADDLINFOHEADER.Format.Font.Size = 8
                paraADDLINFOHEADER.Format.Alignment = ParagraphAlignment.Center
                paraADDLINFOHEADER.AddText("ADDITIONAL INFORMATION")

                ' Info section table
                tblADDLINFO_INFO = New Tables.Table
                tblADDLINFO_INFO.Borders.Width = 0
                tblADDLINFO_INFO.Format.Font.Name = "Verdana"
                tblADDLINFO_INFO.Format.Font.Size = 8
                col = Nothing
                row = Nothing

                ' Column 0 (Label 1)
                col = tblADDLINFO_INFO.AddColumn(Unit.FromInch(1.5))
                col.Format.Alignment = ParagraphAlignment.Left
                ' column 1 (Data 1)
                col = tblADDLINFO_INFO.AddColumn(Unit.FromInch(5.5))
                col.Format.Alignment = ParagraphAlignment.Left
                '' Column 2 (Label 2)
                'col = tblADDLINFO_INFO.AddColumn(Unit.FromInch(1.25))
                'col.Format.Alignment = ParagraphAlignment.Left
                '' Column 3 (Data 2)
                'col = tblADDLINFO_INFO.AddColumn(Unit.FromInch(1.25))
                'col.Format.Alignment = ParagraphAlignment.Left

                ai = Nothing
                If CLUEObj.AdditionalInformation IsNot Nothing AndAlso CLUEObj.AdditionalInformation.DataLine IsNot Nothing AndAlso CLUEObj.AdditionalInformation.DataLine.Trim() <> String.Empty Then
                    ai = Parse_HOM_CLUE_AdditionalInfo(CLUEObj.AdditionalInformation.DataLine, err)
                    If err <> "" Then Throw New Exception(err)
                    If ai.Equals(New HOM_CLUE_AddtionalInfo_structure()) Then Throw New Exception("Parse AI failed")

                    ' Line 1 - Quoteback
                    row = tblADDLINFO_INFO.AddRow()
                    cell = row.Cells(0)
                    cell.AddParagraph("Quoteback:")
                    cell = row.Cells(1)
                    cell.AddParagraph(ai.Quoteback)
                    ' Line 2 - Account
                    row = tblADDLINFO_INFO.AddRow()
                    cell = row.Cells(0)
                    cell.AddParagraph("Account:")
                    cell = row.Cells(1)
                    cell.AddParagraph(ai.Account)
                    ' Line 3 - Date of Order
                    row = tblADDLINFO_INFO.AddRow()
                    cell = row.Cells(0)
                    cell.AddParagraph("Date of Order:")
                    cell = row.Cells(1)
                    cell.AddParagraph(ai.DateOfOrder)
                    ' Line 4 - Date of Receipt
                    row = tblADDLINFO_INFO.AddRow()
                    cell = row.Cells(0)
                    cell.AddParagraph("Date of Receipt:")
                    cell = row.Cells(1)
                    cell.AddParagraph(ai.DateOfReceipt)
                    ' Line 5 - Company
                    row = tblADDLINFO_INFO.AddRow()
                    cell = row.Cells(0)
                    cell.AddParagraph("Company")
                    cell = row.Cells(1)
                    cell.AddParagraph(ai.CompanyName)
                    ' Line 6 - Real Property Ref #
                    row = tblADDLINFO_INFO.AddRow()
                    cell = row.Cells(0)
                    cell.AddParagraph("Real Property Ref. #:")
                    cell = row.Cells(1)
                    cell.AddParagraph(ai.RealPropertyRefNumber)
                Else
                    row = tblADDLINFO_INFO.AddRow()
                    cell = row.Cells(0)
                    cell.AddParagraph(NoDataText)
                End If
                ' CAH B51631 - From Hard 6 to a Row Count - rows could be 1 line if no data found.
                tblADDLINFO_INFO.SetEdge(0, 0, 2, tblADDLINFO_INFO.Rows.Count(), Tables.Edge.Box, BorderStyle.None, 0, Colors.Black)
                sec.Add(tblADDLINFO_INFO)

                ' Data Blob
                paraADDLINFO = sec.AddParagraph()
                paraADDLINFO.Format.Font.Name = "Verdana"
                paraADDLINFO.Format.Font.Size = 8
                If CLUEObj.AdditionalInformation Is Nothing OrElse CLUEObj.AdditionalInformation.DataLine Is Nothing OrElse CLUEObj.AdditionalInformation.DataLine.Trim = String.Empty Then
                    paraADDLINFO.AddText(NoDataText)
                Else
                    paraADDLINFO.AddText(ai.blob)
                End If

                ' *******************
                ' SEARCH SECTION
                ' *******************
                ' Header
                paraINQUIRYHEADER = sec.AddParagraph()
                paraINQUIRYHEADER.Format.Font.Name = "Verdana"
                paraINQUIRYHEADER.Format.Font.Size = 8
                paraINQUIRYHEADER.Format.Alignment = ParagraphAlignment.Center
                paraINQUIRYHEADER.AddText("INQUIRY HISTORY")

                paraINQUIRYHISTORY = sec.AddParagraph()
                paraINQUIRYHISTORY.Format.Font.Name = "Verdana"
                paraINQUIRYHISTORY.Format.Font.Size = 8
                paraINQUIRYHISTORY.Format.Alignment = ParagraphAlignment.Left
                If CLUEObj.InquiryHistory IsNot Nothing AndAlso CLUEObj.InquiryHistory.DataLine IsNot Nothing AndAlso CLUEObj.InquiryHistory.DataLine.Trim <> String.Empty Then
                    paraINQUIRYHISTORY.AddText(CLUEObj.InquiryHistory.DataLine)
                Else
                    paraINQUIRYHISTORY.AddText(NoDataText)
                End If

                ' ************************************
                ' FOOTER PARAGRAPHS (3)
                ' ************************************
                ' The first page has a different header than the rest  - NO, THEY ARE ALL THE SAME AFTER BRD UPDATE MGB 11/18/14
                ' FIRST PAGE HEADER
                ' Center Paragraph
                'paraFooterCenter1 = sec.AddParagraph()
                'FormattedText = paraFooterCenter1.AddFormattedText("------------------- Prepared By: COMPREHENSIVE LOSS UNDERWRITING EXCHANGE ------------------")
                'FormattedText.Bold = True
                'paraFooterCenter1.AddLineBreak()
                'FormattedText = paraFooterCenter1.AddFormattedText("LexisNexis Inc")
                'FormattedText.Bold = True
                'paraFooterCenter1.Format.Alignment = ParagraphAlignment.Center
                'paraFooterCenter1.Format.Font.Name = "Verdana"
                'paraFooterCenter1.Format.Font.Size = 6
                'paraFooterCenter1.Format.Font.Color = Colors.Black
                'paraFooterCenter1.Format.Borders.Top.Style = BorderStyle.Single
                'paraFooterCenter1.Format.Borders.Bottom.Style = BorderStyle.Single
                'paraFooterCenter1.Format.Borders.Left.Style = BorderStyle.Single
                'paraFooterCenter1.Format.Borders.Right.Style = BorderStyle.Single
                'paraFooterCenter1.Format.Borders.Top.Width = 1
                'paraFooterCenter1.Format.Borders.Bottom.Width = 1
                'paraFooterCenter1.Format.Borders.Left.Width = 1
                'paraFooterCenter1.Format.Borders.Right.Width = 1
                'paraFooterCenter1.Format.Borders.Top.Color = Colors.Black
                'paraFooterCenter1.Format.Borders.Bottom.Color = Colors.Black
                'paraFooterCenter1.Format.Borders.Left.Color = Colors.Black
                'paraFooterCenter1.Format.Borders.Right.Color = Colors.Black

                'paraFooterCenterBottom1 = sec.AddParagraph()
                'FormattedText = paraFooterCenterBottom1.AddFormattedText("'C.L.U.E.' is a registered trademark of ChoicePoint Asset Company")
                'paraFooterCenterBottom1.Format.Alignment = ParagraphAlignment.Center
                'paraFooterCenterBottom1.Format.Font.Name = "Verdana"
                'paraFooterCenterBottom1.Format.Font.Size = 6
                'paraFooterCenterBottom1.Format.Font.Color = Colors.Black

                '' Left Paragraph
                '' The prepared data lines are in the ClueObj.PreparedBySection.LeftColumnItems collection
                '' Loop through them and build the left footer paragraph
                'paraFooterLeft1 = sec.AddParagraph()
                'paraFooterLeft1.Format.Alignment = ParagraphAlignment.Left
                'paraFooterLeft1.Format.Font.Name = "Verdana"
                'paraFooterLeft1.Format.Font.Size = 6
                'paraFooterLeft1.Format.Font.Color = Colors.Black
                'FormattedText = paraFooterLeft1.AddFormattedText("ChoicePoint Inc., Atlanta, GA")
                'FormattedText.Underline = Underline.Single
                'FormattedText.Bold = True
                'paraFooterLeft1.AddLineBreak()
                'txt = "ChoicePoint Insurance Consumer Center" & vbCrLf & "Atlanta, GA 30348-5108"
                'paraFooterLeft1.AddText(txt)

                '' Right Paragraph
                '' The prepared data lines are in the ClueObj.PreparedBySection.LeftColumnItems collection
                '' Loop through them and build the right footer paragraph
                'paraFooterRight1 = sec.AddParagraph()
                'paraFooterRight1.Format.Alignment = ParagraphAlignment.Left
                'paraFooterRight1.Format.Font.Name = "Verdana"
                'paraFooterRight1.Format.Font.Size = 6
                'paraFooterRight1.Format.Font.Color = Colors.Black
                'FormattedText = paraFooterRight1.AddFormattedText("For additional information contact:")
                'FormattedText.Underline = Underline.Single
                'FormattedText.Bold = True
                'paraFooterRight1.AddLineBreak()
                'txt = "P.O. Box 105108" & vbCrLf & "Telephone: 1-800-456-6004"
                'paraFooterRight1.AddText(txt)

                ' FOOTER FOR ALL PAGES EXCEPT THE FIRST - NO THEY ARE ALL THE SAME AFTER BRD UPDATE MGB 11/18/14
                'This one is the header (first) and footer (last) line of the footer section
                paraFooterCenter = sec.AddParagraph()
                FormattedText = paraFooterCenter.AddFormattedText("------------------- Prepared By: COMPREHENSIVE LOSS UNDERWRITING EXCHANGE ------------------")
                FormattedText.Bold = True
                paraFooterCenter.AddLineBreak()
                FormattedText = paraFooterCenter.AddFormattedText("LexisNexis Inc., Atlanta, GA")
                FormattedText.Bold = True
                '
                paraFooterCenter.AddLineBreak()
                FormattedText = paraFooterCenter.AddFormattedText("'C.L.U.E.' is a registered trademark of LexisNexis Asset Company.  All Rights Reserved.")
                '
                paraFooterCenter.Format.Alignment = ParagraphAlignment.Center
                paraFooterCenter.Format.Font.Name = "Verdana"
                paraFooterCenter.Format.Font.Size = 6
                paraFooterCenter.Format.Font.Color = Colors.Black
                paraFooterCenter.Format.Borders.Top.Style = BorderStyle.Single
                paraFooterCenter.Format.Borders.Bottom.Style = BorderStyle.Single
                paraFooterCenter.Format.Borders.Left.Style = BorderStyle.Single
                paraFooterCenter.Format.Borders.Right.Style = BorderStyle.Single
                paraFooterCenter.Format.Borders.Top.Width = 1
                paraFooterCenter.Format.Borders.Bottom.Width = 1
                paraFooterCenter.Format.Borders.Left.Width = 1
                paraFooterCenter.Format.Borders.Right.Width = 1
                paraFooterCenter.Format.Borders.Top.Color = Colors.Black
                paraFooterCenter.Format.Borders.Bottom.Color = Colors.Black
                paraFooterCenter.Format.Borders.Left.Color = Colors.Black
                paraFooterCenter.Format.Borders.Right.Color = Colors.Black

                ' Left Paragraph
                ' The prepared data lines are in the ClueObj.PreparedBySection.LeftColumnItems collection
                ' Loop through them and build the left footer paragraph
                paraFooterLeft = sec.AddParagraph()
                paraFooterLeft.Format.Alignment = ParagraphAlignment.Left
                paraFooterLeft.Format.Font.Name = "Verdana"
                paraFooterLeft.Format.Font.Size = 6
                paraFooterLeft.Format.Font.Color = Colors.Black
                FormattedText = paraFooterLeft.AddFormattedText("If you have questions contact:")
                FormattedText.Underline = Underline.Single
                FormattedText.Bold = True
                paraFooterLeft.AddLineBreak()
                txt = "LexisNexis Techincal Support" & vbCrLf & "P.O. Box 105179" & vbCrLf & "Atlanta, GA 30348-5179" & vbCrLf & "Telephone: 1-800-456-6432"
                paraFooterLeft.AddText(txt)

                ' Right Paragraph
                ' The prepared data lines are in the ClueObj.PreparedBySection.LeftColumnItems collection
                ' Loop through them and build the right footer paragraph
                paraFooterRight = sec.AddParagraph()
                paraFooterRight.Format.Alignment = ParagraphAlignment.Left
                paraFooterRight.Format.Font.Name = "Verdana"
                paraFooterRight.Format.Font.Size = 6
                paraFooterRight.Format.Font.Color = Colors.Black
                FormattedText = paraFooterRight.AddFormattedText("Refer consumers to:")
                FormattedText.Underline = Underline.Single
                FormattedText.Bold = True
                paraFooterRight.AddLineBreak()
                txt = "LexisNexis Consumer Center" & vbCrLf & "P.O. Box 105108" & vbCrLf & "Atlanta, GA 30348-5108" & vbCrLf & "Telephone: 1-800-456-6004" & vbCrLf & "http://www.consumerdisclosure.com"
                paraFooterRight.AddText(txt)

                '' ************************************************
                '' !!! R  E  N  D  E  R  !!!                    ***
                '' ************************************************
                '' ADD ALL THE FORMATTED OBJECTS TO THE DOCUMENT
                '' ************************************************
                docRenderer = New Rendering.DocumentRenderer(tempdoc)
                docRenderer.PrepareDocument()

                ' RENDER TITLE SECTION
                NextTopPos = 0.125
                docRenderer.RenderObject(gfx, 0, XUnit.FromInch(NextTopPos), CurrentPage.Width, paraTITLE)
                NextTopPos = NextTopPos + 0.5

                ' RENDER REPORT INFO SECTION
                'NextTopPos = NextTopPos + 0.25
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(NextTopPos), XUnit.FromInch(7.75), tblRPTINFO)
                ' Divider below report info section
                NextTopPos = NextTopPos + 0.5
                gfx.DrawLine(XPens.Black, 0, XUnit.FromInch(NextTopPos), CurrentPage.Width, XUnit.FromInch(NextTopPos))

                ' RENDER RECAP SECTION
                NextTopPos = NextTopPos + 0.1
                'docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), paraRECAP)
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(NextTopPos), XUnit.FromInch(7), tblRECAP)
                NextTopPos = NextTopPos + 0.25

                ' RENDER SEARCH SECTION
                ' Header
                'If CurrentPageNum = 1 Then
                '    CheckForPageBreak("HOM", paraFooterCenter1, paraFooterLeft1, paraFooterRight1, err, 0, paraFooterCenterBottom1)
                'Else
                CheckForCLUEPageBreak("HOM", paraFooterCenter, paraFooterLeft, paraFooterRight, err, 0)
                'End If
                docRenderer.RenderObject(gfx, 0, XUnit.FromInch(NextTopPos), CurrentPage.Width, paraSEARCHSECTIONHEADER)
                NextTopPos = NextTopPos + 0.15
                ' Divider under search section header
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(NextTopPos), CurrentPage.Width, Unit.FromInch(NextTopPos))
                NextTopPos = NextTopPos + 0.15
                'docRenderer.RenderObject(gfx, 0, XUnit.FromInch(NextTopPos), CurrentPage.Width, paraSEARCH)
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(NextTopPos), XUnit.FromInch(7), tblSEARCH)
                NextTopPos = NextTopPos + 1.75

                ' RENDER REPORTED CLAIM HISTORY SECTION - RISK
                'If CurrentPageNum = 1 Then
                '    CheckForPageBreak("HOM", paraFooterCenter1, paraFooterLeft1, paraFooterRight1, err, ClaimHistoryRISKSectionHeightInches, paraFooterCenterBottom1)
                'Else
                CheckForCLUEPageBreak("HOM", paraFooterCenter, paraFooterLeft, paraFooterRight, err, ClaimHistoryRISKSectionHeightInches)
                'End If
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), paraCLAIMHISTHEADERRISK)
                ' Divider underneath the title
                NextTopPos = NextTopPos + 0.2
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(NextTopPos), CurrentPage.Width, Unit.FromInch(NextTopPos))
                NextTopPos = NextTopPos + 0.45
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.75), XUnit.FromInch(NextTopPos), XUnit.FromInch(7), tblCLAIMHISTRISK)

                ' RENDER REPORTED CLAIM HISTORY SECTION - SUBJECT
                NextTopPos = NextTopPos + ClaimHistoryRISKSectionHeightInches
                'If CurrentPageNum = 1 Then
                '    CheckForPageBreak("HOM", paraFooterCenter1, paraFooterLeft1, paraFooterRight1, err, ClaimHistorySUBJSectionHeightInches, paraFooterCenterBottom1)
                'Else
                CheckForCLUEPageBreak("HOM", paraFooterCenter, paraFooterLeft, paraFooterRight, err, ClaimHistorySUBJSectionHeightInches)
                'End If
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(NextTopPos), XUnit.FromInch(8), paraCLAIMHISTHEADERSUBJ)
                ' Divider underneath the title
                NextTopPos = NextTopPos + 0.2
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(NextTopPos), CurrentPage.Width, Unit.FromInch(NextTopPos))
                NextTopPos = NextTopPos + 0.45
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.75), XUnit.FromInch(NextTopPos), XUnit.FromInch(7), tblCLAIMHISTSUBJ)

                ' Hard Page Break after CLAIM HISTORY sections
                NextTopPos = 15
                'If CurrentPageNum = 1 Then
                '    CheckForPageBreak("HOM", paraFooterCenter1, paraFooterLeft1, paraFooterRight1, err, 0, paraFooterCenterBottom1)
                'Else
                CheckForCLUEPageBreak("HOM", paraFooterCenter, paraFooterLeft, paraFooterRight, err, 0)
                'End If

                ' RENDER ADDITIONAL INFO SECTION
                ' Header
                docRenderer.RenderObject(gfx, 0, XUnit.FromInch(NextTopPos), CurrentPage.Width, paraADDLINFOHEADER)
                NextTopPos = NextTopPos + 0.15
                ' Divider under search section header
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(NextTopPos), CurrentPage.Width, Unit.FromInch(NextTopPos))
                NextTopPos = NextTopPos + 0.15
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(NextTopPos), XUnit.FromInch(7), tblADDLINFO_INFO)
                NextTopPos = NextTopPos + 0.8
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(NextTopPos), XUnit.FromInch(7), paraADDLINFO)
                NextTopPos = NextTopPos + 2.5

                ' RENDER INQUIRY HISTORY SECTION
                'If CurrentPageNum = 1 Then
                '    CheckForPageBreak("HOM", paraFooterCenter1, paraFooterLeft1, paraFooterRight1, err, 0, paraFooterCenterBottom1)
                'Else
                CheckForCLUEPageBreak("HOM", paraFooterCenter, paraFooterLeft, paraFooterRight, err, 0)
                'End If
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(NextTopPos), XUnit.FromInch(7.5), paraINQUIRYHEADER)
                NextTopPos = NextTopPos + 0.15
                gfx.DrawLine(XPens.Black, 0, Unit.FromInch(NextTopPos), CurrentPage.Width, Unit.FromInch(NextTopPos))
                NextTopPos = NextTopPos + 0.15
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(NextTopPos), XUnit.FromInch(7.5), paraINQUIRYHISTORY)

                ' This will draw the footers on the last page
                DrawCLUEFooter("HOM", paraFooterCenter, paraFooterLeft, paraFooterRight, err)

                Exit Sub
            Catch ex As Exception
                err = ex.Message
                Exit Sub
            End Try
        End Sub

        Private Function GetNumVehSearchLines(ByVal vsi As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.VehicleSearchResult, ByRef err As String) As Integer
            Dim cnt1 As Integer = 0
            Dim cnt2 As Integer = 0
            Dim cnt3 As Integer = 0
            Dim cnt4 As Integer = 0
            Dim highval As Integer = 0

            Try
                ' Get the maximum number of lines of all of the vehicle search data
                cnt1 = 0
                If vsi.DateAgeInfo IsNot Nothing AndAlso vsi.DateAgeInfo.Trim() <> "" Then cnt1 = vsi.DateAgeInfo.Trim.Split(vbCrLf).Count
                cnt2 = 0
                If vsi.ClaimType IsNot Nothing AndAlso vsi.ClaimType.Trim() <> "" Then cnt2 = vsi.ClaimType.Trim.Split(vbCrLf).Count
                cnt3 = 0
                If vsi.AmountPaid IsNot Nothing AndAlso vsi.AmountPaid.Trim() <> "" Then cnt3 = vsi.AmountPaid.Trim.Split(vbCrLf).Count
                cnt4 = 0
                If vsi.SubClaims IsNot Nothing AndAlso vsi.SubClaims.Trim() <> "" Then cnt4 = vsi.SubClaims.Trim.Split(vbCrLf).Count

                highval = cnt1
                If cnt2 > highval Then highval = cnt2
                If cnt3 > highval Then highval = cnt3
                If cnt4 > highval Then highval = cnt4

                Return highval
            Catch ex As Exception
                err = ex.Message
                Return 0
            End Try
        End Function

        Private Function GetNumPRCLines(ByVal prc As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.PossibleRelatedClaim, ByRef err As String) As Integer
            Dim cnt1 As Integer = 0
            Dim cnt2 As Integer = 0
            Dim cnt3 As Integer = 0
            Dim cnt4 As Integer = 0
            Dim highval As Integer = 0

            Try
                ' Get the maximum number of lines of all of the vehicle search data
                cnt1 = 0
                If prc.DateAgeInfo IsNot Nothing AndAlso prc.DateAgeInfo.Trim() <> "" Then cnt1 = prc.DateAgeInfo.Trim.Split(vbCrLf).Count
                cnt2 = 0
                If prc.ClaimType IsNot Nothing AndAlso prc.ClaimType.Trim() <> "" Then cnt2 = prc.ClaimType.Trim.Split(vbCrLf).Count
                cnt3 = 0
                If prc.AmountPaid IsNot Nothing AndAlso prc.AmountPaid.Trim() <> "" Then cnt3 = prc.AmountPaid.Trim.Split(vbCrLf).Count
                cnt4 = 0
                If prc.SubClaims IsNot Nothing AndAlso prc.SubClaims.Trim() <> "" Then cnt4 = prc.SubClaims.Trim.Split(vbCrLf).Count

                highval = cnt1
                If cnt2 > highval Then highval = cnt2
                If cnt3 > highval Then highval = cnt3
                If cnt4 > highval Then highval = cnt4

                Return highval
            Catch ex As Exception
                err = ex.Message
                Return 0
            End Try
        End Function

        Private Function FormatClaimHistoryItemData(ByVal histitem As Diamond.Common.Objects.ThirdParty.ReportObjects.CLUEPersonalAuto.ReportedClaimHistory, ByRef err As String) As String
            Dim newstr As String = ""
            Try
                'cell.AddParagraph(hist.ClueFileNumber & " " & hist.ClaimNumber & vbCrLf & hist.PolicyType & "/" & hist.Company & vbCrLf & hist.DriverDetail)
                If histitem.ClueFileNumber.Trim() <> "" Then newstr = newstr & histitem.ClueFileNumber.Trim() & vbCrLf
                If histitem.ClaimNumber.Trim() <> "" Then newstr = newstr & histitem.ClaimNumber.Trim() & vbCrLf
                If histitem.PolicyType.Trim() <> "" Then newstr = newstr & histitem.PolicyType.Trim() & vbCrLf
                If histitem.Company.Trim() <> "" Then newstr = newstr & histitem.Company.Trim() & vbCrLf
                If histitem.DriverDetail.Trim() <> "" Then newstr = newstr & histitem.DriverDetail.Trim()

                Return newstr.Trim()
            Catch ex As Exception
                err = ex.Message
                Return ""
            End Try
        End Function

        ''' <summary>
        ''' Pass in the data item and the number of lines desired and this function will add extra line breaks so
        ''' that the data item has the specified number of lines.
        '''
        ''' We need this because when drawing gridlines on a pdf table all cells must have the same number of rows or the
        ''' grid lines will not draw correctly
        ''' </summary>
        ''' <param name="DataItem"></param>
        ''' <param name="TotNumLines"></param>
        ''' <param name="err"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AddPaddingLinesToText(DataItem As String, ByVal TotNumLines As Integer, ByRef err As String) As String
            Dim newstr As String = ""
            Dim work() As String = Nothing

            Try
                If DataItem.Trim() = "" Then
                    'If the item is an empty string return x number of blank lines
                    For i As Integer = 1 To TotNumLines - 1
                        newstr = newstr & vbCrLf
                    Next
                    Return newstr
                Else
                    work = DataItem.Split(vbCrLf)
                    ' work didn't split, return x number blank lines
                    If work Is Nothing OrElse work.Count = 0 Then
                        For i As Integer = 1 To TotNumLines
                            newstr = newstr & vbCrLf
                        Next
                        Return newstr
                    End If
                    ' The item already has at least the number of lines desired, just return the input
                    If work.Count >= TotNumLines Then Return DataItem
                    ' Item has less than the desired number of lines, add the required lines and return the new string
                    For i As Integer = 1 To (TotNumLines - work.Count)
                        newstr = newstr & "" & vbCrLf
                    Next
                    newstr = DataItem + newstr
                    'newstr = newstr & DataItem
                    Return newstr
                End If
            Catch ex As Exception
                err = ex.Message
                Return ""
            End Try
        End Function

        ''' <summary>
        ''' This will remove any extraneous line breaks from the passed data item.
        ''' ie: if the only thing on a data line is a crlf then the line will be discarded.
        ''' Only lines with actual data will be kept.
        '''
        ''' This will essentially shrink any formatted displayable data down to it's smallest possible height
        ''' </summary>
        ''' <param name="DataItem"></param>
        ''' <param name="err"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function RemoveExtraLineBreaksFromText(DataItem As String, ByRef err As String) As String
            Dim newstr As String = ""
            Dim work() As String = Nothing
            Try
                If DataItem = "" Then Return ""
                work = DataItem.Split(vbCrLf)

                For Each s As String In work
                    ' Also remove invalid date values
                    If s.Trim() <> "" AndAlso s.Trim() <> "01/01/1900 12:00:00 AM" Then newstr = newstr & s.Trim() & vbCrLf
                Next

                Return newstr.Trim()
            Catch ex As Exception
                err = ex.Message
                Return ""
            End Try
        End Function

        ''' <summary>
        ''' Creates a page break on the passed document and initializes the graphics object
        ''' </summary>
        ''' <param name="err"></param>
        ''' <remarks></remarks>
        Private Sub CreateNewPageBreak(ByRef err As String)
            Try
                ' create a new page
                CurrentPage = doc.AddPage()
                gfx = XGraphics.FromPdfPage(CurrentPage)
                gfx.MUH = PdfFontEncoding.Unicode
                gfx.MFEH = PdfFontEmbedding.Default

                CurrentPageNum += 1

                Exit Sub
            Catch ex As Exception
                err = ex.Message
                Exit Sub
            End Try
        End Sub

        Private Sub DrawCLUEFooter(ByVal LOB As String, ByRef CenterFooter As Paragraph, ByRef LeftFooter As Paragraph, RightFooter As Paragraph, ByRef err As String)
            Try

                Select Case LOB.ToUpper()
                    Case "PPA"
                        ' Divider above the footer section
                        gfx.DrawLine(XPens.Black, 0, Unit.FromInch(9.98), CurrentPage.Width, Unit.FromInch(9.98))

                        docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(10), CurrentPage.Width, CenterFooter)
                        docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(10.125), XUnit.FromInch(4), LeftFooter)
                        docRenderer.RenderObject(gfx, XUnit.FromInch(6.3), XUnit.FromInch(10.125), XUnit.FromInch(2.5), RightFooter)

                        Exit Select
                    Case "HOM"
                        docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(9.85), XUnit.FromInch(8), CenterFooter)
                        docRenderer.RenderObject(gfx, XUnit.FromInch(0.125), XUnit.FromInch(10.25), XUnit.FromInch(3), LeftFooter)
                        docRenderer.RenderObject(gfx, XUnit.FromInch(6.75), XUnit.FromInch(10.25), XUnit.FromInch(3), RightFooter)

                        Exit Select
                    Case Else
                        Exit Sub
                End Select

                Exit Sub
            Catch ex As Exception
                err = ex.Message
                Exit Sub
            End Try
        End Sub

        Private Sub CheckForCLUEPageBreak(ByVal LOB As String, ByVal paraFooterCenter As Paragraph, ByVal paraFooterLeft As Paragraph, ByVal paraFooterRight As Paragraph, ByRef err As String, Optional ByVal NextSectionHeight As Decimal = 0)
            Try
                Select Case NextSectionHeight
                    Case 0 ' Check for page break - do not factor in the next section height
                        If NextTopPos >= MaxBodyLength Then
                            ' Draw the footer on the current page
                            DrawCLUEFooter(LOB, paraFooterCenter, paraFooterLeft, paraFooterRight, err)
                            ' Create the new page
                            CreateNewPageBreak(err)
                            If err <> "" Then Throw New Exception(err)
                            NextTopPos = 0.25
                        Else
                            NextTopPos = NextTopPos + 0.25
                        End If
                    Case Else ' Check for page break - factor in the next section height
                        If (NextTopPos + NextSectionHeight) >= MaxBodyLength Then
                            ' Draw the footer on the current page
                            DrawCLUEFooter(LOB, paraFooterCenter, paraFooterLeft, paraFooterRight, err)
                            ' Create the new page
                            CreateNewPageBreak(err)
                            If err <> "" Then Throw New Exception(err)
                            NextTopPos = 0.25
                        Else
                            NextTopPos = NextTopPos + 0.25
                        End If
                End Select
                Exit Sub
            Catch ex As Exception
                err = ex.Message
                Exit Sub
            End Try
        End Sub

        Private Sub CheckForMVRPageBreak(ByVal paraFooterLeft As Paragraph, ByRef err As String, Optional ByVal NextSectionHeight As Decimal = 0)
            Try
                Select Case NextSectionHeight
                    Case 0 ' Check for page break - do not factor in the next section height
                        If NextTopPos >= MaxBodyLength Then
                            ' Draw the footer on the current page
                            DrawMVRFooter(paraFooterLeft, err)
                            ' Create the new page
                            CreateNewPageBreak(err)
                            If err <> "" Then Throw New Exception(err)
                            NextTopPos = 0.25
                        Else
                            NextTopPos = NextTopPos + 0.25
                        End If
                    Case Else ' Check for page break - factor in the next section height
                        If (NextTopPos + NextSectionHeight) >= MaxBodyLength Then
                            ' Draw the footer on the current page
                            DrawMVRFooter(paraFooterLeft, err)
                            ' Create the new page
                            CreateNewPageBreak(err)
                            If err <> "" Then Throw New Exception(err)
                            NextTopPos = 0.25
                        Else
                            NextTopPos = NextTopPos + 0.25
                        End If
                End Select
                Exit Sub
            Catch ex As Exception
                err = ex.Message
                Exit Sub
            End Try
        End Sub

        Private Sub DrawMVRFooter(ByRef LeftFooter As Paragraph, ByRef err As String)
            Try
                docRenderer.RenderObject(gfx, XUnit.FromInch(0.25), XUnit.FromInch(10.75), XUnit.FromInch(2), LeftFooter)

                Exit Sub
            Catch ex As Exception
                err = ex.Message
                Exit Sub
            End Try
        End Sub

#End Region

#End Region

    End Module

End Namespace