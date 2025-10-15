Imports QuickQuote.CommonMethods 'added 5/19/2017

Partial Class DiamondQuickQuoteXmlSelecter_QQ
    Inherits System.Web.UI.Page

    Dim qqHelper As New QuickQuoteHelperClass

    Protected Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit
        Me.MasterPageFile = ConfigurationManager.AppSettings("DiamondQuickQuoteMaster")
    End Sub
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            Page.MaintainScrollPositionOnPostBack = True

            If qqHelper.IsHomeOfficeStaffUser = True Then
                Me.btnGetXml.Attributes.Add("onclick", "btnSubmit_Click(this, 'Processing...');") 'for disable button and server-side logic

                'added 2/28/2013
                Dim quoteId As String = ""
                Dim quoteXmlId As String = ""
                Dim xmlType As String = ""
                Dim validateQuoteIdForQuoteXml As Boolean = True 'defaults to False on Viewer page
                If Request.QueryString("DiamondQuickQuoteXmlString") IsNot Nothing AndAlso Request.QueryString("DiamondQuickQuoteXmlString").ToString <> "" Then
                    SplitDiamondQuickQuoteXmlString(Request.QueryString("DiamondQuickQuoteXmlString").ToString, quoteId, quoteXmlId, xmlType, validateQuoteIdForQuoteXml)
                Else
                    If Request.QueryString("quoteid") IsNot Nothing Then
                        quoteId = Request.QueryString("quoteid")
                    End If
                End If

                If quoteId <> "" AndAlso IsNumeric(quoteId) = True Then
                    Me.txtQuoteId.Text = quoteId
                End If
                If quoteXmlId <> "" AndAlso IsNumeric(quoteXmlId) = True Then
                    Me.txtQuoteXmlId.Text = quoteXmlId
                End If
                Me.cbValidateQuoteXmlId.Checked = validateQuoteIdForQuoteXml
                If xmlType <> "" Then
                    Select Case Replace(UCase(xmlType), " ", "") 'use UCASE and get rid of spaces
                        Case "QUOTE", "QUOTEREQUEST"
                            SetXmlTypeDropDownByType(QuickQuoteXML.QuickQuoteXmlType.Quote)
                        Case "RATEDQUOTE", "QUOTERESPONSE"
                            SetXmlTypeDropDownByType(QuickQuoteXML.QuickQuoteXmlType.RatedQuote)
                        Case "APPGAP", "APPGAPREQUEST", "APP", "APPREQUEST"
                            SetXmlTypeDropDownByType(QuickQuoteXML.QuickQuoteXmlType.AppGap)
                        Case "RATEDAPPGAP", "APPGAPRESPONSE", "RATEDAPP", "APPRESPONSE"
                            SetXmlTypeDropDownByType(QuickQuoteXML.QuickQuoteXmlType.RatedAppGap)
                    End Select
                End If

                If Me.txtQuoteId.Text <> "" Then 'added IF/ELSE 2/28/2013
                    btnGetXml_Click(Nothing, Nothing)
                Else
                    SetFocus(Me.txtQuoteId) 'was previously by itself
                End If
            Else
                ShowError("Only staff can use this page.")
                Me.btnGetXml.Enabled = False
            End If
        End If
    End Sub
    Private Sub ShowError(ByVal message As String, Optional ByVal redirect As Boolean = False, Optional ByVal redirectPage As String = "")
        message = Replace(message, "\", "\\")
        message = Replace(message, "<br>", "\n")
        message = Replace(message, vbCrLf, "\n")

        Dim strScript As String = "<script language=JavaScript>"
        strScript &= "alert(""" & message & """);"
        If redirect = True AndAlso redirectPage <> "" Then
            strScript &= " window.location.href='" & redirectPage & "';"
        End If
        strScript &= "</script>"

        Page.RegisterStartupScript("clientScript", strScript)

    End Sub

    Protected Sub btnGetXml_Click(sender As Object, e As System.EventArgs) Handles btnGetXml.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        'added 2/7/2013
        Me.XmlViewerLinkSection.Visible = False
        Me.XmlViewerLink.HRef = ""

        If Me.txtQuoteId.Text = "" Then
            ShowError("Please enter the quote id.")
            SetFocus(Me.txtQuoteId)
        ElseIf IsNumeric(Me.txtQuoteId.Text) = False Then
            ShowError("The quote id must be numeric.")
            SetFocus(Me.txtQuoteId)
        ElseIf Me.txtQuoteXmlId.Text <> "" AndAlso IsNumeric(Me.txtQuoteXmlId.Text) = False Then
            ShowError("If you enter the quote xml id, it must be numeric.")
            SetFocus(Me.txtQuoteXmlId)
        ElseIf Me.ddlXmlType.SelectedValue = "" Then
            ShowError("Please enter the xml type to load.")
            SetFocus(Me.ddlXmlType)
        Else
            Dim xmlType As String = ""
            Select Case Replace(UCase(Me.ddlXmlType.SelectedValue), " ", "") 'use UCASE and get rid of spaces
                Case "QUOTE", "QUOTEREQUEST"
                    xmlType = "Quote"
                Case "RATEDQUOTE", "QUOTERESPONSE"
                    xmlType = "RatedQuote"
                Case "APPGAP", "APPGAPREQUEST", "APP", "APPREQUEST" '2/28/2013 - added App and AppRequest
                    xmlType = "AppGap"
                Case "RATEDAPPGAP", "APPGAPRESPONSE", "RATEDAPP", "APPRESPONSE" '2/28/2013 - added App and AppRequest
                    xmlType = "RatedAppGap"
            End Select
            'added link logic 2/7/2013 (needs to use full path)
            Dim xmlViewerLink As String = ""
            If ConfigurationManager.AppSettings("QuickQuote_XmlViewer") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_XmlViewer").ToString <> "" Then
                xmlViewerLink = ConfigurationManager.AppSettings("QuickQuote_XmlViewer").ToString
                If xmlViewerLink.Contains("/") = False AndAlso xmlViewerLink.Contains("\") = False Then 'should have /, but may have \ if developer uses server path instead of web path
                    'assume the key has the relative path
                    xmlViewerLink = ResolveUrl(xmlViewerLink)
                End If
            Else
                xmlViewerLink = ResolveUrl("DiamondQuickQuoteXmlViewer.aspx")
            End If
            'OpenNewWindow("DiamondQuickQuoteXmlViewer.aspx?DiamondQuickQuoteXmlString=quoteId==" & Me.txtQuoteId.Text & "||quoteXmlId==" & Me.txtQuoteXmlId.Text & "||xmlType==" & xmlType)
            'added validateQuoteIdForQuoteXml 2/7/2013
            Dim validateQuoteIdForQuoteXml As String = ""
            If Me.cbValidateQuoteXmlId.Checked = True Then
                validateQuoteIdForQuoteXml = "Yes"
            Else
                validateQuoteIdForQuoteXml = "No"
            End If
            xmlViewerLink &= "?DiamondQuickQuoteXmlString=quoteId==" & Me.txtQuoteId.Text & "||quoteXmlId==" & Me.txtQuoteXmlId.Text & "||xmlType==" & xmlType & "||validateQuoteIdForQuoteXml==" & validateQuoteIdForQuoteXml

            'added 2/7/2013
            Me.XmlViewerLinkSection.Visible = True
            Me.XmlViewerLink.HRef = xmlViewerLink

            OpenNewWindow(xmlViewerLink)
        End If
    End Sub
    Private Sub OpenNewWindow(ByVal url As String)
        If url <> "" Then
            Dim strScript As String = "<script language=JavaScript>"
            'strScript &= " window.open('" & url & "');"
            strScript &= " window.open(""" & url & """);"
            strScript &= "</script>"

            Page.RegisterStartupScript("clientScript", strScript)
        End If
    End Sub
    'added 2/28/2013
    Private Sub SplitDiamondQuickQuoteXmlString(ByVal DiamondQuickQuoteXmlString As String, ByRef quoteId As String, ByRef quoteXmlId As String, ByRef xmlType As String, ByRef validateQuoteIdForQuoteXml As Boolean)
        '?DiamondQuickQuoteXmlString=quoteId==1||quoteXmlId==1||xmlType==Quote
        If DiamondQuickQuoteXmlString <> "" AndAlso DiamondQuickQuoteXmlString.Contains("==") = True Then
            Dim arNameValuePair As Array
            If DiamondQuickQuoteXmlString.Contains("||") = True Then
                'multiple values
                Dim arDecString As String()
                arDecString = Split(DiamondQuickQuoteXmlString, "||")
                For Each nameValuePair As String In arDecString
                    If nameValuePair.Contains("==") = True Then
                        arNameValuePair = Split(nameValuePair, "==")
                        Select Case UCase(arNameValuePair(0).ToString.Trim)
                            Case "QUOTEID"
                                quoteId = arNameValuePair(1).ToString.Trim
                            Case "QUOTEXMLID"
                                quoteXmlId = arNameValuePair(1).ToString.Trim
                            Case "XMLTYPE"
                                xmlType = arNameValuePair(1).ToString.Trim
                            Case "VALIDATEQUOTEIDFORQUOTEXML"
                                'If arNameValuePair(1).ToString.Trim <> "" AndAlso (UCase(arNameValuePair(1).ToString.Trim) = "YES" OrElse UCase(arNameValuePair(1).ToString.Trim) = "TRUE") Then
                                '    validateQuoteIdForQuoteXml = True
                                'Else
                                '    validateQuoteIdForQuoteXml = False
                                'End If
                                '2/28/2013 - changed to default to True if nothing is there; different than Viewer page
                                If arNameValuePair(1).ToString.Trim <> "" AndAlso (UCase(arNameValuePair(1).ToString.Trim) = "NO" OrElse UCase(arNameValuePair(1).ToString.Trim) = "FALSE") Then
                                    validateQuoteIdForQuoteXml = False
                                Else
                                    validateQuoteIdForQuoteXml = True
                                End If
                        End Select
                    End If
                Next
            ElseIf quoteId = "" AndAlso UCase(DiamondQuickQuoteXmlString).Contains("QUOTEID") = True Then
                arNameValuePair = Split(DiamondQuickQuoteXmlString, "==")
                quoteId = arNameValuePair(1).ToString.Trim
            End If
        End If
    End Sub
    Private Sub SetXmlTypeDropDownByType(ByVal xmlType As QuickQuoteXML.QuickQuoteXmlType)
        If xmlType <> Nothing Then
            Dim vals As New Generic.List(Of String)
            Select Case xmlType
                Case QuickQuoteXML.QuickQuoteXmlType.Quote
                    vals.Add("Quote")
                    vals.Add("QuoteRequest")
                Case QuickQuoteXML.QuickQuoteXmlType.RatedQuote
                    vals.Add("RatedQuote")
                    vals.Add("QuoteResponse")
                Case QuickQuoteXML.QuickQuoteXmlType.AppGap
                    vals.Add("AppGap")
                    vals.Add("AppGapRequest")
                    vals.Add("App")
                    vals.Add("AppRequest")
                Case QuickQuoteXML.QuickQuoteXmlType.RatedAppGap
                    vals.Add("RatedAppGap")
                    vals.Add("AppGapResponse")
                    vals.Add("RatedApp")
                    vals.Add("AppResponse")
            End Select
            MatchXmlTypeToDropDownValue(vals)
        End If
    End Sub
    Private Sub MatchXmlTypeToDropDownValue(ByVal vals As Generic.List(Of String))
        If vals IsNot Nothing AndAlso vals.Count > 0 AndAlso Me.ddlXmlType.Items IsNot Nothing AndAlso Me.ddlXmlType.Items.Count > 0 Then
            Dim foundMatch As Boolean = False
            For Each Val As String In vals
                'If Me.ddlXmlType.Items.FindByValue(Val) IsNot Nothing Then
                '    Me.ddlXmlType.Items.FindByValue(Val).Selected = True
                'End If
                For Each item As ListItem In Me.ddlXmlType.Items
                    If UCase(Replace(item.Value, " ", "")) = UCase(Replace(Val, " ", "")) Then
                        Me.ddlXmlType.SelectedValue = item.Value
                        foundMatch = True
                        Exit For
                    End If
                Next
                If foundMatch = True Then
                    Exit For
                End If
            Next
        End If
    End Sub
End Class
