Imports System.Xml
Imports System.Xml.Schema
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports DCO = Diamond.Common.Objects

Namespace QuickQuote.CommonObjects.Umbrella
    Public Class Driver
        Inherits LinkedPolicyItem
        Implements IXmlSerializable, IReconcilable

        Public Property Name As New Name
        Public Property NameAddressSourceId As String

#Region "IReconcilable"
        Public Overrides Sub CopyFrom(src As IReconcilable, Optional setItemNumbers As Boolean = True, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) Implements IReconcilable.CopyFrom
            MyBase.CopyFrom(src, setItemNumbers, canUseItemNumberForPolicyItemReconcilliation)
            With src.AsInstance(Of Driver)
                Me.NameAddressSourceId = .NameAddressSourceId
                
                Me.Name.InitIfNothing.CopyFrom(.Name)
            End With
        End Sub

        Public Overrides Function IsMatchFor(src As IReconcilable, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean Implements IReconcilable.IsMatchFor
            Dim matched = MyBase.IsMatchFor(src, canUseItemNumberForPolicyItemReconcilliation)

            Return matched AndAlso src.AsInstance(Of Driver).Name.IsMatchFor(Me.Name)
        End Function

        Public Overrides Sub ConvertToDiamondItem(ByRef diaItem As DCO.InsTableObject, ByRef innerDiamondItemDeletedFlag As Boolean, ByRef innerDiamondItemAddedFlag As Boolean, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False)
            If diaItem Is Nothing Then
                diaItem = New DCO.Policy.Driver
            End If

            With DirectCast(diaItem, DCO.Policy.Driver)
                Me.Name.ConvertToDiamondItem(.Name, innerDiamondItemDeletedFlag, innerDiamondItemAddedFlag)
                If Integer.TryParse(Me.NameAddressSourceId, .NameAddressSourceId) = False Then
                    .NameAddressSourceId = Diamond.Common.Enums.NameAddressSource.Driver
                End If
            End With
        End Sub

        Public Overrides Function IsMatchForDiamondItem(diaItem As DCO.InsTableObject, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean
            Dim matched As Boolean = False
            Dim diaObj = DirectCast(diaItem, DCO.Policy.Driver)
            If diaObj IsNot Nothing Then
                If canUseItemNumberForPolicyItemReconcilliation Then
                    If Me.HasValidPolicyItemNumber AndAlso
                       _qqHelper.IsValidDiamondNum(diaObj.DriverNum) AndAlso
                       _qqHelper.IntegerForString(Me.PolicyItemNumber) = diaObj.DriverNum.Id Then
                        matched = True
                    End If
                Else
                    If Me.Name.IsMatchForDiamondItem(diaObj.Name) Then
                        matched = True
                    End If
                End If
            End If
            Return matched
        End Function
#End Region
#Region "IXmlSerializable"
        Protected Overrides Function ReadXml_NextProperty(reader As XmlReader) As Boolean
            Dim found = MyBase.ReadXml_NextProperty(reader)

            If Not found Then
                found = True
                Select Case reader.Name
                    Case NameOf(Name)
                        Dim xmlSer As New XmlSerializer(GetType(Name))
                        Me.Name = xmlSer.Deserialize(reader)

                    Case NameOf(Me.NameAddressSourceId)
                        Me.NameAddressSourceId = reader.ReadElementContentAsString()
                    Case Else
                        found = False
                End Select
            End If
            Return found
        End Function

        Protected Overrides Sub WriteXml_Internal(writer As XmlWriter)
            MyBase.WriteXml_Internal(writer)

            If Me.Name IsNot Nothing Then
                Dim xmlSer As New XmlSerializer(GetType(Name))
                xmlSer.Serialize(writer, Me.Name)
            End If
            writer.WriteElementString(NameOf(Me.NameAddressSourceId), Me.NameAddressSourceId)
        End Sub

        'Public Overrides Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
        '    MyBase.ReadXml(reader.ReadSubtree())

        '    reader.Read()
        '    If reader.IsEmptyElement Then Return
        '    With reader
        '        While .NodeType <> XmlNodeType.EndElement
        '            .Read()

        '            Select Case reader.Name
        '                Case NameOf(Address)
        '                    Dim xmlSer As New XmlSerializer(GetType(Name))
        '                    Me.Name = xmlSer.Deserialize(reader.ReadSubtree())

        '                Case NameOf(Me.NameAddressSourceId)
        '                    Me.NameAddressSourceId = reader.ReadElementContentAsString()
        '                Case Else

        '            End Select
        '            .ReadEndElement()
        '            .MoveToContent()
        '        End While
        '    End With
        'End Sub

        'Public Overrides Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml
        '    MyBase.WriteXml(writer)
        '    If Me.Name IsNot Nothing Then
        '        Dim xmlSer As New XmlSerializer(GetType(Name))
        '        xmlSer.Serialize(writer, Me.Name)
        '    End If
        '    writer.WriteElementString(NameOf(Me.NameAddressSourceId), Me.NameAddressSourceId)
        'End Sub
#End Region
    End Class

End Namespace