Imports IFM.PrimativeExtensions

Public Class ctl_Summary_CGL
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion("divHomeQuoteSummary", HiddenField1, "0")
        Me.VRScript.StopEventPropagation(Me.lnkPrint.ClientID, False)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            If IsOnAppPage Then
                Me.lblMainAccord.Text = String.Format("App Summary - Effective Date: {0} - 12 month", Me.Quote.EffectiveDate)
            Else
                Me.lblMainAccord.Text = String.Format("Quote Summary - Effective Date: {0} - 12 month", Me.Quote.EffectiveDate)
            End If

            Me.lblPremiumMainAccord.Text = String.Format("{0:C2}", Me.Quote.TotalQuotedPremium)

            'Dim ph1Name As String = String.Format("{0} {1} {2} {3}", Me.Quote.Policyholder.Name.FirstName, Me.Quote.Policyholder.Name.MiddleName, Me.Quote.Policyholder.Name.LastName, Me.Quote.Policyholder.Name.SuffixName).Replace("  ", " ").Trim()
            Me.lblPhName.Text = Me.Quote.Policyholder.Name.DisplayName


            Me.lblQuoteNum.Text = Me.Quote.QuoteNumber

            Dim zip As String = Me.Quote.Policyholder.Address.Zip
            If zip.Length > 5 Then
                zip = zip.Substring(0, 5)
            End If

            'house num, street, apt, pobox, city, state, zip
            Me.lblPhAddress.Text = String.Format("{0} {1} {2} {3} {4} {5} {6}", Me.Quote.Policyholder.Address.HouseNum, Me.Quote.Policyholder.Address.StreetName, If(String.IsNullOrWhiteSpace(Me.Quote.Policyholder.Address.ApartmentNumber) = False, "Apt# " + Me.Quote.Policyholder.Address.ApartmentNumber, ""), Me.Quote.Policyholder.Address.POBox, Me.Quote.Policyholder.Address.City, Me.Quote.Policyholder.Address.State, zip).Replace("  ", " ").Trim()

            Me.lblEffectiveDate.Text = Me.Quote.EffectiveDate
            Me.lblExpirationDate.Text = Me.Quote.ExpirationDate

            Me.lblFullPremium.Text = String.Format("{0:C2}", Me.Quote.TotalQuotedPremium)

            PopulateGeneralInformation()


            Me.ctlQuoteSummaryActions.Populate()
        End If
    End Sub

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.UseRatedQuoteImage = True
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

    Private Sub WriteCell(sb As StringBuilder, cellText As String)
        sb.AppendLine("<td>")
        sb.AppendLine(cellText)
        sb.AppendLine("</td>")
    End Sub

    Private Sub WriteCell(sb As StringBuilder, cellText As String, styleText As String)
        sb.AppendLine("<td style=""" + styleText + """>")
        sb.AppendLine(cellText)
        sb.AppendLine("</td>")
    End Sub

    Private Sub WriteCell(sb As StringBuilder, cellText As String, styleText As String, cssclass As String)
        sb.AppendLine("<td style=""" + styleText + """ class=""" + cssclass + """>")
        sb.AppendLine(cellText)
        sb.AppendLine("</td>")
    End Sub


    Private Sub PopulateGeneralInformation()
        Dim html As New StringBuilder()
        'html.AppendLine("<div class=""hom_qs_Main_Sections"">")
        'html.AppendLine("<span class=""qs_section_headers"">Inland Marine</span>")
        html.AppendLine("<div class=""hom_qs_Sub_Sections"">")
        html.AppendLine("<table class=""hom_qa_table_shades"">")
        html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

        WriteCell(html, "Coverage")
        WriteCell(html, "Description")
        WriteCell(html, "Deductible", "", "qs_Grid_cell_premium")
        WriteCell(html, "Limit <sup>3</sup>", "", "qs_Grid_cell_premium")
        WriteCell(html, "Premium", "", "qs_Grid_cell_premium")
        html.AppendLine("</tr>")

        html.AppendLine("</table>")
        'html.AppendLine("</div>")
        'html.AppendLine("</div>")
        Me.tblGeneralInfo.Text = html.ToString()
    End Sub

End Class