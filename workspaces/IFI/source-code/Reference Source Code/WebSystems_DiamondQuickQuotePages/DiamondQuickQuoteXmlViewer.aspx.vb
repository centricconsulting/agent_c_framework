Imports QuickQuote.CommonObjects 'added 5/19/2017
Imports QuickQuote.CommonMethods 'added 5/19/2017

Partial Class DiamondQuickQuoteXmlViewer_QQ
    Inherits System.Web.UI.Page

    Dim qqHelper As New QuickQuoteHelperClass
    Dim QQxml As New QuickQuoteXML

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            If qqHelper.IsHomeOfficeStaffUser = True Then
                Dim hasSufficientParams As Boolean = False
                Dim quoteId As String = ""
                Dim quoteXmlId As String = ""
                Dim xmlType As String = ""
                Dim validateQuoteIdForQuoteXml As Boolean = False 'added 2/7/2013
                If Request.QueryString("DiamondQuickQuoteXmlString") IsNot Nothing AndAlso Request.QueryString("DiamondQuickQuoteXmlString").ToString <> "" Then
                    SplitDiamondQuickQuoteXmlString(Request.QueryString("DiamondQuickQuoteXmlString").ToString, quoteId, quoteXmlId, xmlType, validateQuoteIdForQuoteXml)
                End If
                If quoteId <> "" AndAlso IsNumeric(quoteId) = True Then
                    LoadXml(quoteId, quoteXmlId, xmlType, validateQuoteIdForQuoteXml)
                Else
                    Me.lblMessage.Text = "The parameters passed to this page were insufficient."
                End If
            Else
                Me.lblMessage.Text = "Only staff can use this page."
            End If
            
        End If
    End Sub
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
                                If arNameValuePair(1).ToString.Trim <> "" AndAlso (UCase(arNameValuePair(1).ToString.Trim) = "YES" OrElse UCase(arNameValuePair(1).ToString.Trim) = "TRUE") Then
                                    validateQuoteIdForQuoteXml = True
                                Else
                                    validateQuoteIdForQuoteXml = False
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
    Private Sub LoadXml(ByVal quoteId As String, ByVal quoteXmlId As String, ByVal xmlType As String, ByVal validateQuoteIdForQuoteXml As Boolean)
        If quoteId <> "" AndAlso IsNumeric(quoteId) = True Then
            Dim qq As QuickQuoteObject = Nothing
            Dim errMsg As String = ""
            Dim strXml As String = ""
            Dim qqRateType As QuickQuoteXML.QuickQuoteXmlType = Nothing
            Dim quoteNumber As String = ""
            If xmlType <> "" Then
                'Select Case UCase(xmlType)
                '    Case "QUOTE"
                '        qqRateType = QuickQuoteXML.QuickQuoteXmlType.Quote
                '    Case "RATEDQUOTE"
                '        qqRateType = QuickQuoteXML.QuickQuoteXmlType.RatedQuote
                '    Case "APPGAP"
                '        qqRateType = QuickQuoteXML.QuickQuoteXmlType.AppGap
                '    Case "RATEDAPPGAP"
                '        qqRateType = QuickQuoteXML.QuickQuoteXmlType.RatedAppGap
                'End Select
                'updated 2/28/2013 to accept variations in text
                Select Case Replace(UCase(xmlType), " ", "") 'use UCASE and get rid of spaces
                    Case "QUOTE", "QUOTEREQUEST"
                        qqRateType = QuickQuoteXML.QuickQuoteXmlType.Quote
                    Case "RATEDQUOTE", "QUOTERESPONSE"
                        qqRateType = QuickQuoteXML.QuickQuoteXmlType.RatedQuote
                    Case "APPGAP", "APPGAPREQUEST", "APP", "APPREQUEST"
                        qqRateType = QuickQuoteXML.QuickQuoteXmlType.AppGap
                    Case "RATEDAPPGAP", "APPGAPRESPONSE", "RATEDAPP", "APPRESPONSE"
                        qqRateType = QuickQuoteXML.QuickQuoteXmlType.RatedAppGap
                End Select
            End If

            If quoteXmlId <> "" AndAlso IsNumeric(quoteXmlId) = True Then
                If qqRateType <> Nothing Then
                    QQxml.GetQuote(quoteId, quoteXmlId, qqRateType, qq, strXml, errMsg)
                    If strXml <> "" Then
                        '2/7/2013 - added quoteNumber from object
                        If qq IsNot Nothing Then
                            quoteNumber = qq.Database_LastAvailableQuoteNumber
                        End If
                        'added optional validation 2/7/2013
                        If validateQuoteIdForQuoteXml = True AndAlso (qq Is Nothing OrElse qq.Database_QuoteId <> qq.Database_XmlQuoteId) Then
                            Me.lblMessage.Text = "The entered quote xml id is not associated with the entered quote id."
                        Else
                            RenderXml(strXml, quoteId, quoteXmlId, xmlType, quoteNumber)
                        End If
                    ElseIf errMsg <> "" Then
                        Me.lblMessage.Text = errMsg
                    Else
                        Me.lblMessage.Text = "Nothing was returned for your parameters."
                    End If
                Else
                    'return last xml found
                    QQxml.GetQuote(quoteId, quoteXmlId, QuickQuoteXML.QuickQuoteXmlType.RatedAppGap, qq, strXml, errMsg)
                    xmlType = "RatedAppGap"
                    If strXml = "" Then
                        qq = Nothing
                        errMsg = ""
                        QQxml.GetQuote(quoteId, quoteXmlId, QuickQuoteXML.QuickQuoteXmlType.AppGap, qq, strXml, errMsg)
                        xmlType = "AppGap"
                        If strXml = "" Then
                            qq = Nothing
                            errMsg = ""
                            QQxml.GetQuote(quoteId, quoteXmlId, QuickQuoteXML.QuickQuoteXmlType.RatedQuote, qq, strXml, errMsg)
                            xmlType = "RatedQuote"
                            If strXml = "" Then
                                qq = Nothing
                                errMsg = ""
                                QQxml.GetQuote(quoteId, quoteXmlId, QuickQuoteXML.QuickQuoteXmlType.Quote, qq, strXml, errMsg)
                                xmlType = "Quote"
                            End If
                        End If
                    End If
                    If strXml <> "" Then
                        '2/7/2013 - added quoteNumber from object
                        If qq IsNot Nothing Then
                            quoteNumber = qq.Database_LastAvailableQuoteNumber
                        End If
                        'added optional validation 2/7/2013
                        If validateQuoteIdForQuoteXml = True AndAlso (qq Is Nothing OrElse qq.Database_QuoteId <> qq.Database_XmlQuoteId) Then
                            Me.lblMessage.Text = "The entered quote xml id is not associated with the entered quote id."
                        Else
                            RenderXml(strXml, quoteId, quoteXmlId, xmlType, quoteNumber)
                        End If
                    ElseIf errMsg <> "" Then
                        Me.lblMessage.Text = errMsg
                    Else
                        Me.lblMessage.Text = "Nothing was returned for your parameters."
                    End If
                End If
            Else 'omit quoteXmlId
                If qqRateType <> Nothing Then
                    QQxml.GetQuote(quoteId.ToString, qqRateType, qq, strXml, errMsg)
                    If strXml <> "" Then
                        '2/7/2013 - added quoteXmlId and quoteNumber from object
                        If qq IsNot Nothing Then
                            quoteXmlId = qq.Database_CurrentQuoteXmlId
                            quoteNumber = qq.Database_LastAvailableQuoteNumber
                        End If
                        'added optional validation 2/7/2013
                        If validateQuoteIdForQuoteXml = True AndAlso (qq Is Nothing OrElse qq.Database_QuoteId <> qq.Database_XmlQuoteId) Then
                            Me.lblMessage.Text = "The located quote xml record is not associated with the entered quote id; potential database problem."
                        Else
                            RenderXml(strXml, quoteId, quoteXmlId, xmlType, quoteNumber)
                        End If
                    ElseIf errMsg <> "" Then
                        Me.lblMessage.Text = errMsg
                    Else
                        Me.lblMessage.Text = "Nothing was returned for your parameters."
                    End If
                Else
                    'return last xml found
                    QQxml.GetQuote(quoteId, QuickQuoteXML.QuickQuoteXmlType.RatedAppGap, qq, strXml, errMsg)
                    xmlType = "RatedAppGap"
                    If strXml = "" Then
                        qq = Nothing
                        errMsg = ""
                        QQxml.GetQuote(quoteId, QuickQuoteXML.QuickQuoteXmlType.AppGap, qq, strXml, errMsg)
                        xmlType = "AppGap"
                        If strXml = "" Then
                            qq = Nothing
                            errMsg = ""
                            QQxml.GetQuote(quoteId, QuickQuoteXML.QuickQuoteXmlType.RatedQuote, qq, strXml, errMsg)
                            xmlType = "RatedQuote"
                            If strXml = "" Then
                                qq = Nothing
                                errMsg = ""
                                QQxml.GetQuote(quoteId, QuickQuoteXML.QuickQuoteXmlType.Quote, qq, strXml, errMsg)
                                xmlType = "Quote"
                            End If
                        End If
                    End If
                    If strXml <> "" Then
                        '2/7/2013 - added quoteXmlId and quoteNumber from object
                        If qq IsNot Nothing Then
                            quoteXmlId = qq.Database_CurrentQuoteXmlId
                            quoteNumber = qq.Database_LastAvailableQuoteNumber
                        End If
                        'added optional validation 2/7/2013
                        If validateQuoteIdForQuoteXml = True AndAlso (qq Is Nothing OrElse qq.Database_QuoteId <> qq.Database_XmlQuoteId) Then
                            Me.lblMessage.Text = "The located quote xml record is not associated with the entered quote id; potential database problem."
                        Else
                            RenderXml(strXml, quoteId, quoteXmlId, xmlType, quoteNumber)
                        End If
                    ElseIf errMsg <> "" Then
                        Me.lblMessage.Text = errMsg
                    Else
                        Me.lblMessage.Text = "Nothing was returned for your parameters."
                    End If
                End If
            End If
        End If
    End Sub
    Private Sub RenderXml(ByVal strXml As String, ByVal quoteId As String, ByVal quoteXmlId As String, ByVal xmlType As String, ByVal quoteNumber As String)
        If strXml <> "" Then
            If quoteId = "" Then
                quoteId = "UnspecifiedQuoteId"
            End If
            If quoteXmlId = "" Then
                quoteXmlId = "UnspecifiedQuoteXmlId"
            End If
            If xmlType = "" Then
                xmlType = "UnspecifiedXmlType"
            End If
            If quoteNumber = "" Then
                quoteNumber = "UnspecifiedQuoteNumber"
            End If
            Try
                Response.Clear() 'added 2/7/2013
                Dim time As String = String.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now)
                'Response.AddHeader("Content-disposition", "attachment; filename=" + String.Format("{0}-{1}-{2}-{3}.xml", quoteId, quoteXmlId, xmlType, time))
                'updated w/ quoteNumber 2/7/2013
                Response.AddHeader("Content-disposition", "attachment; filename=" + String.Format("{0}-{1}-{2}-{3}-{4}.xml", quoteNumber, quoteId, quoteXmlId, xmlType, time))
                Response.ContentType = "text/xml"
                Dim encoding As New System.Text.UTF8Encoding()
                Response.BinaryWrite(encoding.GetBytes(strXml))
                Response.End()
                '2/7/2013 - might update to use Flush since End may not work when opening the window w/ javascript; didn't make a difference
                'Response.Flush()
            Catch ex As Exception
                Me.lblMessage.Text = "An error was encountered while attempting to render the xml."
            End Try
        End If
    End Sub
End Class
