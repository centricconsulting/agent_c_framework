Imports System.Xml
Imports System.Xml.Serialization
Imports Diamond.Common.Objects
Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods
Imports DCO = Diamond.Common.Objects

Namespace QuickQuote.CommonObjects.Umbrella
    ''' <summary>
    ''' object used to store miscellaneous liability information (on Umbrella)
    ''' </summary>
    ''' <remarks>typically found under PolicyInfo object (<see cref="PolicyInfo"/>) as a list</remarks>
    Public Class MiscellaneousLiability 'added for FUP/PUP
        Inherits TrackedPolicyItem
        Implements IXmlSerializable

        Public Overrides Sub ConvertToDiamondItem(ByRef diaItem As InsTableObject,  ByRef innerDiamondItemDeletedFlag As Boolean, ByRef innerDiamondItemAddedFlag As Boolean, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False)
                 If diaItem Is Nothing Then
                diaItem = New DCO.Policy.MiscellaneousLiability
            End If

            With DirectCast(diaItem, DCO.Policy.MiscellaneousLiability)
                .DetailStatusCode = 1
                Integer.TryParse(Me.TypeId, .MiscellaneousLiabilityTypeId)
                Integer.TryParse(Me.NumberOfItems, .NumberOfItems)
                .Description = Me.Description
            End With
        End Sub

        Public Overrides Function IsMatchForDiamondItem(diaItem As InsTableObject, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean
            Dim matched As Boolean = False
            Dim diaObj = DirectCast(diaItem, DCO.Policy.MiscellaneousLiability)
            If diaObj IsNot Nothing Then
                If canUseItemNumberForPolicyItemReconcilliation Then
                    If Me.HasValidPolicyItemNumber AndAlso
                       _qqHelper.IsValidDiamondNum(diaObj.MiscellaneousLiabilityNum) AndAlso
                       _qqHelper.IntegerForString(Me.PolicyItemNumber) = diaObj.MiscellaneousLiabilityNum.Id Then
                        matched = True
                    End If
                Else
                    If _qqHelper.IntegerForString(Me.TypeId) = diaObj.MiscellaneousLiabilityTypeId AndAlso
                       Me.LinkNumber?.ToUpper() = diaObj.LinkNumber?.ToUpper() Then
                        matched = True
                    End If
                End If
            End If
            Return matched
        End Function
    End Class
End Namespace
