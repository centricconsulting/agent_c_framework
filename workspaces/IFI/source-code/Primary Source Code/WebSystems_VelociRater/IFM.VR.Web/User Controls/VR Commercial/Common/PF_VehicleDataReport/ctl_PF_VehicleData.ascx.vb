Imports System.IO

Public Class ctl_PF_VehicleData
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()

        TitleQuoteNum.Text = Me.Quote.QuoteNumber
        PolicyHolderName.Text = Me.Quote.Policyholder.Name.CommercialName1
        If Not String.IsNullOrEmpty(Me.Quote?.Policyholder?.Name?.DoingBusinessAsName) Then
            DBANameElement.Visible = True
            DBAName.Text = Me.Quote.Policyholder.Name.DoingBusinessAsName
        End If

        If Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Count > 0 Then
            'Me.VehiclesRow.Visible = True
            'Me.VehiclesHeaderSpacerRow.Visible = True 'added 2/19/2013


            Dim dt As New DataTable
            dt.Columns.Add("VehicleNum", System.Type.GetType("System.String"))
            dt.Columns.Add("Year", System.Type.GetType("System.String"))
            dt.Columns.Add("Make", System.Type.GetType("System.String"))
            dt.Columns.Add("Model", System.Type.GetType("System.String"))
            dt.Columns.Add("CostNew", System.Type.GetType("System.String"))
            dt.Columns.Add("Class", System.Type.GetType("System.String"))
            dt.Columns.Add("LiabPrem", System.Type.GetType("System.String"))
            dt.Columns.Add("MedPrem", System.Type.GetType("System.String"))
            dt.Columns.Add("CompDed", System.Type.GetType("System.String"))
            dt.Columns.Add("CompPrem", System.Type.GetType("System.String"))
            dt.Columns.Add("CollDed", System.Type.GetType("System.String"))
            dt.Columns.Add("CollPrem", System.Type.GetType("System.String"))
            dt.Columns.Add("Rntl", System.Type.GetType("System.String"))
            dt.Columns.Add("Tow", System.Type.GetType("System.String"))
            dt.Columns.Add("TotlPrem", System.Type.GetType("System.String"))
            dt.Columns.Add("Terr#", System.Type.GetType("System.String"))

            Dim vCounter As Integer = 0
            For Each v As QuickQuote.CommonObjects.QuickQuoteVehicle In Quote.Vehicles
                vCounter += 1
                Dim newRow As DataRow = dt.NewRow
                newRow.Item("VehicleNum") = vCounter.ToString
                newRow.Item("Year") = v.Year
                newRow.Item("Make") = v.Make
                newRow.Item("Model") = v.Model
                newRow.Item("CostNew") = v.CostNew
                newRow.Item("Class") = v.ClassCode
                newRow.Item("LiabPrem") = If(v.HasLiability_UM_UIM = True, v.Liability_UM_UIM_QuotedPremium, "NS")
                newRow.Item("MedPrem") = If(v.HasMedicalPayments = True, v.MedicalPaymentsQuotedPremium, "NS")
                newRow.Item("CompDed") = If(v.HasComprehensive = True, v.ComprehensiveDeductible, "NS")
                newRow.Item("CompPrem") = If(v.HasComprehensive = True, v.ComprehensiveQuotedPremium, "NS")
                newRow.Item("CollDed") = If(v.HasCollision = True, v.CollisionDeductible, "NS")
                newRow.Item("CollPrem") = If(v.HasCollision = True, v.CollisionQuotedPremium, "NS")
                newRow.Item("Rntl") = If(v.HasRentalReimbursement = True, v.RentalReimbursementQuotedPremium, "NS")
                newRow.Item("Tow") = If(v.HasTowingAndLabor = True, v.TowingAndLaborQuotedPremium, "NS")
                newRow.Item("TotlPrem") = v.PremiumFullTerm
                newRow.Item("Terr#") = v.TerritoryNum
                dt.Rows.Add(newRow)
            Next

            Me.dgrdVehicles.DataSource = dt
            Me.dgrdVehicles.DataBind()
        Else
            'Me.VehiclesRow.Visible = False
            'Me.VehiclesHeaderSpacerRow.Visible = False 'added 2/19/2013
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
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
                    filename = String.Format("VehicleData-{0}.pdf", Me.Quote.QuoteNumber)
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
        Dim waitForExitMilliseconds As Integer = 4000
        If ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_WaitForExitMilliseconds") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_WaitForExitMilliseconds").ToString <> "" AndAlso IsNumeric(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_WaitForExitMilliseconds").ToString) = True Then
            waitForExitMilliseconds = CInt(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_WaitForExitMilliseconds").ToString)
        End If
        process.WaitForExit(waitForExitMilliseconds)

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