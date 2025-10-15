Imports System.Globalization
Imports System.IO

Public Class ctl_PF_ScheduledConstEquipment
    Inherits VRControlBase

    'Public pdfBytes As Byte()

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        Dim Ratings = Me.Quote.ContractorsEquipmentScheduledItems

        Select Case Me.Quote.LobType
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP

        End Select
        TitleQuoteNum.Text = Me.Quote.QuoteNumber
        PolicyHolderName.Text = Me.Quote.Policyholder.Name.CommercialName1
        If Not String.IsNullOrEmpty(Me.Quote?.Policyholder?.Name?.DoingBusinessAsName) Then
            DBANameElement.Visible = True
            DBAName.Text = Me.Quote.Policyholder.Name.DoingBusinessAsName
        End If



        Me.rptSchedConEquip.DataSource = Ratings
        Me.rptSchedConEquip.DataBind()
        'End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean

    End Function

    Protected Function FormatAsCurrency(ByVal money As Object) As String

        Dim NewMoney = 0
        Integer.TryParse(money.ToString, NumberStyles.AllowThousands, CultureInfo.CurrentCulture, NewMoney)
        Return NewMoney.ToString("c")
    End Function

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        Me.Populate()
        Dim filename As String
        Dim sb As New StringBuilder()
        Using sw As New StringWriter(sb)
            Using htw As New HtmlTextWriter(sw)
                MyBase.Render(htw)
                writer.Write(sb.ToString())

                Dim htmlBytes As Byte() = Encoding.UTF8.GetBytes(sw.ToString)
                If htmlBytes IsNot Nothing Then
                    filename = String.Format("ContractorsEquipment{0}.pdf", Me.Quote.QuoteNumber)
                    Dim filePath As String = Server.MapPath(Request.ApplicationPath) & "\Reports\" & filename & ".htm"
                    Dim fs As New FileStream(filePath, FileMode.Create)
                    fs.Write(htmlBytes, 0, htmlBytes.Length)
                    fs.Close()

                    If File.Exists(filePath) = True Then 'enclosed block in IF statement to make sure file exists
                        Dim status As String = ""
                        Dim pdfPath As String = Server.MapPath(Request.ApplicationPath) & "\Reports\" & filename & ".pdf"

                        Try
                            RunExecutable(Server.MapPath(Request.ApplicationPath) & "\Reports\wkhtmltopdf\wkhtmltopdf.exe", """" & filePath & """ """ & pdfPath & """", status)

                            System.IO.File.Delete(filePath)
                            If File.Exists(pdfPath) = True Then
                                Dim fs_pdf As New FileStream(pdfPath, FileMode.Open, FileAccess.Read)
                                Dim pdfBytes As Byte() = New Byte(fs_pdf.Length - 1) {}
                                fs_pdf.Read(pdfBytes, 0, System.Convert.ToInt32(fs_pdf.Length))
                                fs_pdf.Close()

                                If pdfBytes IsNot Nothing Then

                                    Dim proposalId As String = ""
                                    Dim errorMsg As String = ""
                                    Dim successfulInsert As Boolean = False

                                    If errorMsg = "" Then
                                        Response.Clear()
                                        Response.ContentType = "application/pdf"
                                        'Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("ContractorsEquipment{0}.pdf", Me.Quote.QuoteNumber))
                                        Response.BinaryWrite(pdfBytes)
                                        System.IO.File.Delete(pdfPath)
                                    End If
                                End If
                            End If
                        Catch ex As Exception

                        End Try
                    End If
                End If
            End Using
        End Using
    End Sub
    Public Sub RunExecutable(ByVal executable As String, ByVal arguments As String, ByRef status As String)
        'using same code as originally used in C:\Users\domin\Documents\Visual Studio 2005\WebSites\TestExecutableCall
       status = ""
        '*************** Feature Flags ***************
        Dim MaxWaitForExitMilliseconds As Integer = 1000
        Dim MaxTrials As Integer = 30
        Dim Trials As Integer = 0
        Dim chc = New CommonHelperClass
        If chc.ConfigurationAppSettingValueAsBoolean("Bug60875_Wkhtmltopdf") Then
            Dim WkArgs As String = "-q --javascript-delay 200" 'Required Defaults
            If ConfigurationManager.AppSettings("Wkhtmltopdf_RequiredArguments") IsNot Nothing Then
                WkArgs = chc.ConfigurationAppSettingValueAsString("Wkhtmltopdf_RequiredArguments")
            End If

            arguments = WkArgs & " " & arguments

            If ConfigurationManager.AppSettings("Wkhtmltopdf_WaitForExitMilliseconds_Per_Trial") IsNot Nothing Then
                MaxWaitForExitMilliseconds = chc.ConfigurationAppSettingValueAsInteger("Wkhtmltopdf_WaitForExitMilliseconds_Per_Trial")
            End If

            If ConfigurationManager.AppSettings("Wkhtmltopdf_MaxTrials") IsNot Nothing Then
                MaxTrials = chc.ConfigurationAppSettingValueAsInteger("Wkhtmltopdf_MaxTrials")
            End If

        End If

        Dim starter As ProcessStartInfo = New ProcessStartInfo(executable, arguments)
        starter.CreateNoWindow = True
        starter.RedirectStandardOutput = True
        starter.RedirectStandardError = True
        starter.UseShellExecute = False

        Dim process As Process = New Process()
        process.StartInfo = starter

        Dim compareTime As DateTime = DateAdd(DateInterval.Second, -5, Date.Now)

        process.Start()
        'updated to use variable

        If chc.ConfigurationAppSettingValueAsBoolean("Bug60875_Wkhtmltopdf") = False Then
            ' -----   Old Code  -----
            'process.WaitForExit(4000)
            'updated to use variable
            Dim waitForExitMilliseconds As Integer = 20000
            If ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_WaitForExitMilliseconds") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_WaitForExitMilliseconds").ToString <> "" AndAlso IsNumeric(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_WaitForExitMilliseconds").ToString) = True Then
                waitForExitMilliseconds = CInt(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_WaitForExitMilliseconds").ToString)
            End If
            process.WaitForExit(waitForExitMilliseconds)
            'process.WaitForExit()'never finished
            'process.WaitForExit(30000) 'updated to see if program can finish converting in that amount of time; wasn't creating file
            'process.WaitForExit(60000) 'trying double

            While process.HasExited = False
                If process.StartTime > compareTime Then
                    process.CloseMainWindow()
                    Try
                        process.Kill()
                    Catch ex As Exception
                        status = "<b>Kill failed-</b>"
                    End Try
                End If
            End While
        Else
            ' -----   New Code  -----
            While process.WaitForExit(MaxWaitForExitMilliseconds) = False
                If Trials = MaxTrials Then
                    process.CloseMainWindow()
                    Try
                        process.Kill()
                    Catch ex As Exception
                        status = "<b>Kill failed-</b>"
                    End Try
                End If
                Trials += 1
            End While
        End If

        Dim strOutput As String = process.StandardOutput.ReadToEnd
        Dim strError As String = process.StandardError.ReadToEnd

        If (process.ExitCode <> 0) Then
            status &= "Error<br><u>Output</u> - " & strOutput & "<br><u>Error</u> - " & strError

            'added 5/28/2013 for debugging on ifmwebtest (since it always works locally); seems to work okay after changing WaitForExit from 4000 to 10000 (4 seconds to 10 seconds)
            If ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_SendErrorEmail") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_SendErrorEmail").ToString <> "" AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_SendErrorEmail").ToString) = "TRUE" Then
                Dim eMsg As String = ""
                eMsg &= "<b>executable:</b>  " & executable
                eMsg &= "<br /><br />"
                eMsg &= "<b>arguments:</b>  " & arguments
                eMsg &= "<br /><br />"
                eMsg &= "<b>status:</b>  " & status
                QQHelper.SendEmail("ContractorEquipmentPdfConverter@indianafarmers.com", "chawley@indianafarmers.com", "Error Converting Summary to PDF", eMsg)
            End If
        Else
            'ShowError("Success")
            status &= "Success<br><u>Output</u> - " & strOutput & "<br><u>Error</u> - " & strError
        End If

        process.Close()
        process.Dispose()
        process = Nothing
        starter = Nothing
    End Sub
End Class