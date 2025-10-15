Imports System.Xml
Imports System.Xml.Serialization
Imports Diamond.Common.Objects
Imports Microsoft.VisualBasic
Imports DCO = Diamond.Common.Objects

Namespace QuickQuote.CommonObjects.Umbrella
    Public Class YouthfulOperator
        Inherits BasePolicyItem
        Implements IXmlSerializable, IReconcilable

        Public Property YouthfulOperatorCount As String
        Public Property YouthfulOperatorTypeId As String

#Region "IReconcilable"
        Public Overrides Function IsMatchFor(src As IReconcilable, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean Implements IReconcilable.IsMatchFor
            Dim matched = MyBase.IsMatchFor(src, canUseItemNumberForPolicyItemReconcilliation)
            Return matched AndAlso src.AsInstance(Of YouthfulOperator).YouthfulOperatorTypeId = Me.YouthfulOperatorTypeId
        End Function
        Public Overrides Sub CopyFrom(src As IReconcilable, Optional setItemNumbers As Boolean = True, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) Implements IReconcilable.CopyFrom
            MyBase.CopyFrom(src, setItemNumbers, canUseItemNumberForPolicyItemReconcilliation)
            With src.AsInstance(Of YouthfulOperator)
                Me.YouthfulOperatorCount = .YouthfulOperatorCount
                Me.YouthfulOperatorTypeId = .YouthfulOperatorTypeId
            End With
        End Sub
        
        Public Overrides Sub ConvertToDiamondItem(ByRef diaItem As InsTableObject,  ByRef innerDiamondItemDeletedFlag As Boolean, ByRef innerDiamondItemAddedFlag As Boolean, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False)
                 If diaItem Is Nothing Then
                diaItem = New DCO.Policy.YouthfulOperator
            End If

            With DirectCast(diaItem, DCO.Policy.YouthfulOperator)
                .DetailStatusCode = 1
                Integer.TryParse(Me.YouthfulOperatorTypeId, .YouthfulOperatorTypeId)
                Integer.tryparse(Me.YouthfulOperatorCount, .YouthfulOperatorCount)
            End With
        End Sub

        Public Overrides Function IsMatchForDiamondItem(diaItem As InsTableObject, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean
            Dim matched As Boolean = False
            Dim diaObj = DirectCast(diaItem, DCO.Policy.YouthfulOperator)
            If diaObj IsNot Nothing Then
                If canUseItemNumberForPolicyItemReconcilliation Then
                    If Me.HasValidPolicyItemNumber AndAlso
                       _qqHelper.IsValidDiamondNum(diaObj.YouthfulOperatorNum) AndAlso
                       _qqHelper.IntegerForString(Me.PolicyItemNumber) = diaObj.YouthfulOperatorNum.Id Then
                        matched = True
                    End If
                Else
                    If _qqHelper.IntegerForString(Me.YouthfulOperatorTypeId) = diaObj.YouthfulOperatorTypeId Then
                        matched = True
                    End If
                End If
            End If
            Return matched
        End Function
#End Region
#Region "IXmlSeriazable"
        Protected Overrides Function ReadXml_NextProperty(reader As XmlReader) As Boolean
            Dim found = MyBase.ReadXml_NextProperty(reader)
            If Not found Then
                found = True
                Select Case reader.Name
                    Case NameOf(Me.YouthfulOperatorCount)
                        Me.YouthfulOperatorCount = reader.ReadElementContentAsString()
                    Case NameOf(Me.YouthfulOperatorTypeId)
                        Me.YouthfulOperatorTypeId = reader.ReadElementContentAsString()
                    Case Else
                        found = False
                End Select
            End If
            Return found
        End Function
        Protected Overrides Sub WriteXml_Internal(writer As XmlWriter)
            MyBase.WriteXml_Internal(writer)

            writer.WriteElementString(NameOf(Me.YouthfulOperatorCount), Me.YouthfulOperatorCount)
            writer.WriteElementString(NameOf(Me.YouthfulOperatorTypeId), Me.YouthfulOperatorTypeId)
        End Sub

        'Public Overrides Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
        '    MyBase.ReadXml(reader.ReadSubtree())

        '    reader.Read()
        '    If reader.IsEmptyElement Then Return
        '    With reader
        '        While .NodeType <> XmlNodeType.EndElement
        '            .Read()

        '            Select Case reader.Name
        '                Case NameOf(Me.YouthfulOperatorCount)
        '                    Me.YouthfulOperatorCount = reader.ReadElementContentAsString()
        '                Case NameOf(Me.YouthfulOperatorTypeId)
        '                    Me.YouthfulOperatorTypeId = reader.ReadElementContentAsString()
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

        '    writer.WriteElementString(NameOf(Me.YouthfulOperatorCount), Me.YouthfulOperatorCount)
        '    writer.WriteElementString(NameOf(Me.YouthfulOperatorTypeId), Me.YouthfulOperatorTypeId)
        'End Sub
#End Region
    End Class
End Namespace