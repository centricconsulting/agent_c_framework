Imports System.Xml
Imports System.Xml.Serialization
Imports Diamond.Common.Objects
Imports Microsoft.VisualBasic
Imports DCO = Diamond.Common.Objects

Namespace QuickQuote.CommonObjects.Umbrella
    Public Class RecreationalVehicle
        Inherits TrackedPolicyItem
        Implements IXmlSerializable, IReconcilable

        Public Overrides Sub ConvertToDiamondItem(ByRef diaItem As InsTableObject, ByRef innerDiamondItemDeletedFlag As Boolean, ByRef innerDiamondItemAddedFlag As Boolean, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False)
            If diaItem Is Nothing Then
                diaItem = New DCO.Policy.RecreationalVehicle
            End If

            With DirectCast(diaItem, DCO.Policy.RecreationalVehicle)
                .DetailStatusCode = 1
                Integer.TryParse(Me.TypeId, .RecreationalBodyTypeId)
                Integer.TryParse(Me.NumberOfItems, .NumberOfItems)
            End With
        End Sub

        Public Overrides Function IsMatchForDiamondItem(diaItem As InsTableObject, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean
            Dim matched As Boolean = False
            Dim diaObj = DirectCast(diaItem, DCO.Policy.RecreationalVehicle)
            If diaObj IsNot Nothing Then
                If canUseItemNumberForPolicyItemReconcilliation Then
                    If Me.HasValidPolicyItemNumber AndAlso
                       _qqHelper.IsValidDiamondNum(diaObj.RecreationalVehicleNum) AndAlso
                       _qqHelper.IntegerForString(Me.PolicyItemNumber) = diaObj.RecreationalVehicleNum.Id Then
                        matched = True
                    End If
                Else
                    If _qqHelper.IntegerForString(Me.TypeId) = diaObj.RecreationalBodyTypeId AndAlso
                       Me.LinkNumber?.ToUpper() = diaObj.LinkNumber?.ToUpper() Then
                        matched = True
                    End If
                End If
            End If
            Return matched
        End Function

        Protected Overrides Function ReadXml_NextProperty(reader As XmlReader) As Boolean
            Dim found = MyBase.ReadXml_NextProperty(reader)
            If Not found Then
                found = True
                Select Case reader.Name
                    Case "RecreationalBodyTypeId"
                        Me.TypeId = reader.ReadElementContentAsString()
                    Case Else
                        found = False
                End Select
            End If
            Return found
        End Function
        Protected Overrides Sub WriteXml_Internal(writer As XmlWriter)
            MyBase.WriteXml_Internal(writer)
            With writer
                .WriteElementString("RecreationalBodyTypeId", Me.TypeId)
            End With
        End Sub
    End Class
End Namespace