Imports System.Xml
Imports System.Xml.Serialization
Imports Diamond.Common.Objects
Imports Microsoft.VisualBasic
Imports DCO = Diamond.Common.Objects
Namespace QuickQuote.CommonObjects.Umbrella
    Public Class Watercraft
        Inherits TrackedPolicyItem
        Implements IXmlSerializable, IReconcilable

        Public Property Horsepower As String
        Public Property Length As String


        Public Overrides Sub ConvertToDiamondItem(ByRef diaItem As InsTableObject, ByRef innerDiamondItemDeletedFlag As Boolean, ByRef innerDiamondItemAddedFlag As Boolean, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False)
            If diaItem Is Nothing Then
                diaItem = New DCO.Policy.Watercraft
            End If

            With DirectCast(diaItem, DCO.Policy.Watercraft)
                .DetailStatusCode = 1
                Integer.TryParse(Me.TypeId, .WatercraftTypeId)
                Integer.TryParse(Me.NumberOfItems, .NumberOfItems)
                Integer.TryParse(Me.Horsepower, .Horsepower)
                .Length = Me.Length
            End With
        End Sub

        Public Overrides Sub CopyFrom(src As IReconcilable, Optional setItemNumbers As Boolean = True, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) Implements IReconcilable.CopyFrom
            MyBase.CopyFrom(src, canUseItemNumberForPolicyItemReconcilliation)
            With src.AsInstance(Of Watercraft)
                'nothing to do here ¯\_(ツ)_/¯
                Me.Horsepower = .Horsepower
                Me.Length = Length
            End With
        End Sub

        Public Overrides Function IsMatchForDiamondItem(diaItem As InsTableObject, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean
            Dim matched As Boolean = False
            Dim diaObj = DirectCast(diaItem, DCO.Policy.Watercraft)
            If diaObj IsNot Nothing Then
                If canUseItemNumberForPolicyItemReconcilliation Then
                    If Me.HasValidPolicyItemNumber AndAlso
                       _qqHelper.IsValidDiamondNum(diaObj.WatercraftNum) AndAlso
                       _qqHelper.IntegerForString(Me.PolicyItemNumber) = diaObj.WatercraftNum.Id Then
                        matched = True
                    End If
                Else
                    If _qqHelper.IntegerForString(Me.TypeId) = diaObj.WatercraftTypeId AndAlso
                       Me.LinkNumber?.ToUpper() = diaObj.LinkNumber?.ToUpper() Then
                        matched = True
                    End If
                End If
            End If
            Return matched
        End Function
#Region "IXmlSerializable"
        Protected Overrides Function ReadXml_NextProperty(reader As XmlReader) As Boolean
            Dim found = MyBase.ReadXml_NextProperty(reader)
            If Not found Then
                found = True
                Select Case reader.Name

                    Case NameOf(Me.Horsepower)
                        Me.Horsepower = reader.ReadElementContentAsString()
                    Case NameOf(Me.Length)
                        Me.Length = reader.ReadElementContentAsString()
                    Case Else
                        found = False
                End Select
            End If
            Return found
        End Function
        Protected Overrides Sub WriteXml_Internal(writer As XmlWriter)
            MyBase.WriteXml_Internal(writer)
            With writer
                .WriteElementString(NameOf(Me.Horsepower), Me.Horsepower)
                .WriteElementString(NameOf(Me.Length), Me.Length)
            End With
        End Sub

        'Public Overrides Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
        '    MyBase.ReadXml(reader.ReadSubtree())

        '    reader.Read()
        '    If reader.IsEmptyElement Then Return
        '    With reader
        '        While .NodeType <> XmlNodeType.EndElement
        '            .Read()

        '            Select Case reader.Name

        '                Case NameOf(Me.Horsepower)
        '                    Me.Horsepower = reader.ReadElementContentAsString()
        '                Case NameOf(Me.Length)
        '                    Me.Length = reader.ReadElementContentAsString()
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

        '    With writer
        '        .WriteElementString(NameOf(Me.Horsepower), Me.Horsepower)
        '        .WriteElementString(NameOf(Me.Length), Me.Length)
        '    End With
        'End Sub
#End Region
    End Class
End Namespace
