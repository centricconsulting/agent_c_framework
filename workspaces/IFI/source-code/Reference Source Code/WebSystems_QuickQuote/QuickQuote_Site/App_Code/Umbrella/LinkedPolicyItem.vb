Imports System.Xml
Imports System.Xml.Serialization

Namespace QuickQuote.CommonObjects.Umbrella
    Public MustInherit Class LinkedPolicyItem
        Inherits BasePolicyItem
        Implements IXmlSerializable, IReconcilable
        Public Property LinkNumber As String
        Public Property DisplayNum As String

#Region "IReconcilable"
        Public Overrides Sub CopyFrom(src As IReconcilable, Optional setItemNumbers As Boolean = True, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) Implements IReconcilable.CopyFrom
            MyBase.CopyFrom(src, canUseItemNumberForPolicyItemReconcilliation)
            With src.AsInstance(Of LinkedPolicyItem)
                Me.DisplayNum = .DisplayNum
                Me.LinkNumber = .LinkNumber
            End With
        End Sub

        Public Overrides Function IsMatchFor(src As IReconcilable, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean
            Dim matched = MyBase.IsMatchFor(src, canUseItemNumberForPolicyItemReconcilliation)

            Return matched AndAlso Me.LinkNumber = src.AsInstance(Of LinkedPolicyItem).LinkNumber
        End Function
#End Region

#Region "IXmlSerializable"
        Protected Overrides Function ReadXml_NextProperty(reader As XmlReader) As Boolean
            Dim found = MyBase.ReadXml_NextProperty(reader)

            If Not found Then
                found = True
                Select Case reader.Name
                    Case NameOf(Me.LinkNumber)
                        Me.LinkNumber = reader.ReadElementContentAsString()
                    Case $"{Me.GetType().Name}{NameOf(DisplayNum)}"
                        Me.DisplayNum = reader.ReadElementContentAsString()
                    Case Else
                        found = False
                End Select
            End If

            Return found
        End Function

        Protected Overrides Sub WriteXml_Internal(writer As XmlWriter)
            MyBase.WriteXml_Internal(writer)

            writer.WriteElementString(NameOf(Me.LinkNumber), Me.LinkNumber)
            'writer.WriteElementString($"{Me.GetType().Name}{NameOf(Me.DisplayNum)}", Me.DisplayNum)
        End Sub
        'Public Overrides Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
        '    MyBase.ReadXml(reader.ReadSubtree())

        '    reader.Read()
        '    If reader.IsEmptyElement Then Return
        '    With reader
        '        Dim myType = Me.GetType()
        '        While .NodeType <> XmlNodeType.EndElement
        '            .Read()

        '            Select Case reader.Name
        '                Case NameOf(Me.LinkNumber)
        '                    Me.LinkNumber = reader.ReadElementContentAsString()
        '                Case $"{myType}{NameOf(DisplayNum)}"
        '                    Me.DisplayNum = reader.ReadElementContentAsString()
        '                Case Else

        '            End Select
        '            .ReadEndElement()
        '            .MoveToContent()
        '        End While
        '    End With
        '    reader.ReadEndElement()
        'End Sub

        'Public Overrides Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml
        '    MyBase.WriteXml(writer)
        '    writer.WriteElementString(NameOf(Me.LinkNumber), Me.LinkNumber)
        '    writer.WriteElementString($"{Me.GetType()}{NameOf(Me.DisplayNum)}", Me.DisplayNum)
        'End Sub
#End Region
    End Class
End Namespace