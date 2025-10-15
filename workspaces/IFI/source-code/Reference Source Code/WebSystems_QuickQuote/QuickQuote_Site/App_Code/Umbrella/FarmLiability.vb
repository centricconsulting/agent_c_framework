Imports System.Xml
Imports System.Xml.Serialization
Imports Diamond.Common.Objects
Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods
Imports DCO = Diamond.Common.Objects
Namespace QuickQuote.CommonObjects.Umbrella
    Public Class FarmLiability
        Inherits TrackedPolicyItem
        Implements IXmlSerializable, IReconcilable

        Public Property HerbicidesPesticidesUsed As String 'true
        Public Property AnnualReceiptsTypeId As String
            Get
                Return Me.TypeId
            End Get
            Set
                Me.TypeId = Value
            End Set
        End Property

#Region "IReconcilable"
        Public Overrides Sub CopyFrom(src As IReconcilable, Optional setItemNumbers As Boolean = True, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) Implements IReconcilable.CopyFrom
            MyBase.CopyFrom(src, canUseItemNumberForPolicyItemReconcilliation)
            With src.AsInstance(Of FarmLiability)
                Me.HerbicidesPesticidesUsed = .HerbicidesPesticidesUsed
                Me.AnnualReceiptsTypeId = .AnnualReceiptsTypeId
            End With
        End Sub

        Public Overrides Sub ConvertToDiamondItem(ByRef diaItem As InsTableObject, ByRef innerDiamondItemDeletedFlag As Boolean, ByRef innerDiamondItemAddedFlag As Boolean, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False)
            If diaItem Is Nothing Then
                diaItem = New DCO.Policy.FarmLiability
            End If

            With DirectCast(diaItem, DCO.Policy.FarmLiability)
                .DetailStatusCode = 1
                Integer.TryParse(Me.AnnualReceiptsTypeId, .AnnualReceiptsTypeId)
                Boolean.TryParse(Me.HerbicidesPesticidesUsed, .HerbicidesPesticidesUsed)
                Integer.TryParse(Me.NumberOfItems, .NumberOfItems)
            End With
        End Sub

        Public Overrides Function IsMatchForDiamondItem(diaItem As InsTableObject, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean
            Dim matched As Boolean = False
            Dim diaObj = DirectCast(diaItem, DCO.Policy.FarmLiability)
            If diaObj IsNot Nothing Then
                If canUseItemNumberForPolicyItemReconcilliation Then
                    If Me.HasValidPolicyItemNumber AndAlso
                       _qqHelper.IsValidDiamondNum(diaObj.FarmLiabilityNum) AndAlso
                       _qqHelper.IntegerForString(Me.PolicyItemNumber) = diaObj.FarmLiabilityNum.Id Then
                        matched = True
                    End If
                Else
                    If _qqHelper.IntegerForString(Me.AnnualReceiptsTypeId) = diaObj.AnnualReceiptsTypeId AndAlso
                       Me.LinkNumber?.ToUpper() = diaObj.LinkNumber?.ToUpper() Then
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
                    Case NameOf(Me.HerbicidesPesticidesUsed)
                        Me.HerbicidesPesticidesUsed = reader.ReadElementContentAsString()
                    Case NameOf(AnnualReceiptsTypeId)
                        Me.AnnualReceiptsTypeId = reader.ReadElementContentAsString()
                    Case Else
                        found = False
                End Select
            End If
            Return found
        End Function
        Protected Overrides Sub WriteXml_Internal(writer As XmlWriter)
            MyBase.WriteXml_Internal(writer)
            writer.WriteElementString(NameOf(Me.HerbicidesPesticidesUsed), Me.HerbicidesPesticidesUsed)
            writer.WriteElementString(NameOf(Me.AnnualReceiptsTypeId), Me.AnnualReceiptsTypeId)
        End Sub

        'Public Overrides Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
        '    MyBase.ReadXml(reader.ReadSubtree())
        '    reader.Read()
        '    If reader.IsEmptyElement Then Return
        '    With reader
        '        While .NodeType <> XmlNodeType.EndElement
        '            .Read()

        '            Select Case reader.Name
        '                Case NameOf(Me.HerbicidesPesticidesUsed)
        '                    Me.HerbicidesPesticidesUsed = reader.ReadElementContentAsString()
        '                Case NameOf(AnnualReceiptsTypeId)
        '                    Me.AnnualReceiptsTypeId = reader.ReadElementContentAsString()
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
        '    writer.WriteElementString(NameOf(Me.HerbicidesPesticidesUsed), Me.HerbicidesPesticidesUsed)
        '    writer.WriteElementString(NameOf(Me.AnnualReceiptsTypeId), Me.AnnualReceiptsTypeId)
        'End Sub
#End Region
    End Class
End Namespace
