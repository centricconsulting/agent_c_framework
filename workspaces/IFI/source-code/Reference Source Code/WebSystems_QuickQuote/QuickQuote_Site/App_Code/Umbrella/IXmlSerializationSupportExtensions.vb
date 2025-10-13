Imports System.Runtime.CompilerServices
Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects.Umbrella
    Module IXmlSerializationSupportextensions
        <Extension>
        Public Sub SerializeList(Of T As BasePolicyItem)(ByRef obj As BasePolicyItem, list As List(Of T), listName As String, writer As XmlWriter)
            With writer
                .WriteStartElement(listName)
                Dim xmlSer = New XmlSerializer(GetType(T))
                If list IsNot Nothing Then
                    For Each item In list
                        xmlSer.Serialize(writer, item)
                    Next
                End If
                .WriteEndElement()
            End With
        End Sub
        <Extension>
        Public Function DeserializeList(Of T As BasePolicyItem)(ByRef obj As BasePolicyItem, listName As String, reader As XmlReader) As List(Of T)
            Dim retval As New List(Of T)

            With reader
                If Not .IsEmptyElement Then
                    .ReadStartElement(listName)
                    Dim xmlSer = New XmlSerializer(GetType(T))
                    Dim item As T
                    While .IsStartElement()
                        'If we have an attribute that changes the serialization element name, this would fail
                        'If .Name.Equals(GetType(T).Name) = False Then
                        '    Continue While
                        'End If
                        item = xmlSer.Deserialize(reader)
                        retval.Add(item)
                    End While
                    If reader.NodeType = XmlNodeType.EndElement Then
                        .ReadEndElement()
                    Else
                        .Read()
                    End If
                Else
                    .Skip() 'less error prone, but equivalent to a .Read() in this case
                End If
            End With

            Return retval
        End Function
    End Module
End Namespace
