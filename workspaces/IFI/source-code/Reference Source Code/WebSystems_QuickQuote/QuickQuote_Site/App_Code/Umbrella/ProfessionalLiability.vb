Imports System.Xml.Serialization
Imports Diamond.Common.Objects
Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods
Imports DCO = Diamond.Common.Objects

Namespace QuickQuote.CommonObjects.Umbrella
    ''' <summary>
    ''' object used to store professional liability information (on Umbrella)
    ''' </summary>
    ''' <remarks>typically found under QuickQuotePolicyInfo object (<see cref="QuickQuotePolicyInfo"/>) as a list</remarks>
    Public Class ProfessionalLiability
        Inherits TrackedPolicyItem
        Implements IXmlSerializable, IReconcilable

        Public Overrides Sub ConvertToDiamondItem(ByRef diaItem As InsTableObject,  ByRef innerDiamondItemDeletedFlag As Boolean, ByRef innerDiamondItemAddedFlag As Boolean, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False)
            If diaItem Is Nothing Then
                diaItem = New DCO.Policy.ProfessionalLiability
            End If

            With DirectCast(diaItem, DCO.Policy.ProfessionalLiability)
                .DetailStatusCode = 1
                Integer.TryParse(Me.TypeId, .ProfessionalLiabilityTypeId)
                Integer.TryParse(Me.NumberOfItems, .NumberOfItems)
            End With
        End Sub

        Public Overrides Function IsMatchForDiamondItem(diaItem As InsTableObject, Optional canUseItemNumberForPolicyItemReconcilliation As Boolean = False) As Boolean
            Dim matched As Boolean = False
            Dim diaObj = DirectCast(diaItem, DCO.Policy.ProfessionalLiability)
            If diaObj IsNot Nothing Then
                If canUseItemNumberForPolicyItemReconcilliation Then
                    If Me.HasValidPolicyItemNumber AndAlso
                       _qqHelper.IsValidDiamondNum(diaObj.ProfessionalLiabilityNum) AndAlso
                       _qqHelper.IntegerForString(Me.PolicyItemNumber) = diaObj.ProfessionalLiabilityNum.Id Then
                        matched = True
                    End If
                Else
                    If _qqHelper.IntegerForString(Me.TypeId) = diaObj.ProfessionalLiabilityTypeId AndAlso
                       Me.LinkNumber?.ToUpper() = diaObj.LinkNumber?.ToUpper() Then
                        matched = True
                    End If
                End If
            End If
            Return matched
        End Function
    End Class
End Namespace
