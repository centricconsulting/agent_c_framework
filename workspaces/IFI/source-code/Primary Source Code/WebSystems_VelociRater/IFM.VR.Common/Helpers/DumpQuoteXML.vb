Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers
    Public Class DumpQuoteXML
        ''' <summary>
        ''' Writes current QuickQuoteObject XML to a file for review.  Good for
        ''' before and after checks
        ''' </summary>
        ''' <param name="Quote">QuickQuoteObject to be saved</param>
        ''' <param name="name">Unique name to add to file name</param>
        ''' <example>IFM.VR.Common.Helpers.DumpQuoteXML.DumpQuoteXML(Quote, "Location")</example>
        Public Shared Sub DumpQuoteXML(ByRef Quote As QuickQuoteObject, Optional name As String = "")
            Dim QQxml As QuickQuoteXML = New QuickQuoteXML()
            Dim xmlDoc As System.Xml.XmlDocument = New System.Xml.XmlDocument()
            Dim IsTest = System.Configuration.ConfigurationManager.AppSettings("TestOrProd").ToUpper() = "TEST"
            Dim SpecialName As String = String.Empty
            If String.IsNullOrWhiteSpace(name) = False Then
                SpecialName = name & "_"
            End If
            Dim filepath = System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DiamondXmlsSaveDirectory") & "\" & "QuoteImage_" & SpecialName & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml"
            Try
                If IsTest Then
                    QQxml.BuildXml(Quote, xmlDoc)
                    xmlDoc.Save(filepath)
                End If
            Catch ex As Exception
                'This is code for development so if it fails, swallow the error
            End Try

        End Sub

    End Class
End Namespace
