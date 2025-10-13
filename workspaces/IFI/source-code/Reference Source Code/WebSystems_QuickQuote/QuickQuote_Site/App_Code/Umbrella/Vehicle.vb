Imports System.Xml.Serialization
Imports Diamond.Common.Objects
Imports DCO = Diamond.Common.Objects

Namespace QuickQuote.CommonObjects.Umbrella
    Public Class Vehicle
        Inherits TrackedPolicyItem
        Implements IXmlSerializable, IReconcilable

        Public Overrides Sub ConvertToDiamondItem(ByRef diaItem As InsTableObject,  ByRef innerDiamondItemDeletedFlag As Boolean, ByRef innerDiamondItemAddedFlag As Boolean, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False)
            If diaItem Is Nothing Then
                diaItem = New DCO.Policy.Vehicle
            End If

            With DirectCast(diaItem, DCO.Policy.Vehicle)
                .DetailStatusCode = 1
                Integer.TryParse(Me.TypeId, .VehicleTypeId)
                Integer.TryParse(Me.NumberOfItems, .NumberOfItems)
            End With
        End Sub

        Public Overrides Function IsMatchForDiamondItem(diaItem As InsTableObject, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean
            Dim matched As Boolean = False
            Dim diaObj = DirectCast(diaItem, DCO.Policy.Vehicle)
            If diaObj IsNot Nothing Then
                If canUseItemNumberForPolicyItemReconcilliation Then
                    If Me.HasValidPolicyItemNumber AndAlso
                       _qqHelper.IsValidDiamondNum(diaObj.VehicleNum) AndAlso
                       _qqHelper.IntegerForString(Me.PolicyItemNumber) = diaObj.VehicleNum.Id Then
                        matched = True
                    End If
                Else
                    If _qqHelper.IntegerForString(Me.TypeId) = diaObj.VehicleTypeId AndAlso
                       Me.LinkNumber?.ToUpper() = diaObj.LinkNumber?.ToUpper() Then
                        matched = True
                    End If
                End If
            End If
            Return matched
        End Function
    End Class
End Namespace
