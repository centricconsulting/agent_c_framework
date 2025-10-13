Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods 'added 8/19/2014

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store static data element
    ''' </summary>
    ''' <remarks>used w/ Static Data xml file(s)</remarks>
    <Serializable()> _
    Public Class QuickQuoteStaticDataElement 'added 8/15/2014
        Public Property NameValuePair As QuickQuoteStaticDataNameValuePair = Nothing
        Public Property Attributes As List(Of QuickQuoteStaticDataAttribute) = Nothing

        'added 8/18/2014; not needed here as much as for QuickQuoteStaticDataAttribute
        Public Property nvp_name As String 'changed 8/19/2014 from name to be consistent w/ value prop (nvp_value)
            Get
                If NameValuePair IsNot Nothing Then
                    Return NameValuePair.Name
                Else
                    Return String.Empty
                End If
            End Get
            Set(value As String)
                If NameValuePair Is Nothing Then
                    NameValuePair = New QuickQuoteStaticDataNameValuePair
                End If
                NameValuePair.Name = value
            End Set
        End Property
        Public Property nvp_value As String 'changed 8/19/2014 from value to avoid issues w/ setter having same param name
            Get
                If NameValuePair IsNot Nothing Then
                    Return NameValuePair.Value
                Else
                    Return String.Empty
                End If
            End Get
            Set(value As String)
                If NameValuePair Is Nothing Then
                    NameValuePair = New QuickQuoteStaticDataNameValuePair
                End If
                NameValuePair.Value = value
            End Set
        End Property
        'added 8/19/2014
        Public Property nvp_propertyName As QuickQuoteHelperClass.QuickQuotePropertyName
            Get
                Dim pName As QuickQuoteHelperClass.QuickQuotePropertyName = Nothing
                If System.Enum.TryParse(Of QuickQuoteHelperClass.QuickQuotePropertyName)(nvp_name, pName) = False Then
                    pName = QuickQuoteHelperClass.QuickQuotePropertyName.None
                End If
                Return pName
            End Get
            Set(value As QuickQuoteHelperClass.QuickQuotePropertyName)
                If value <> Nothing AndAlso value <> QuickQuoteHelperClass.QuickQuotePropertyName.None Then
                    nvp_name = System.Enum.GetName(GetType(QuickQuoteHelperClass.QuickQuotePropertyName), value)
                End If
            End Set
        End Property
    End Class
End Namespace
