Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects


Namespace IFM.VR.Common.Helpers.CPR

    Public Class CPR_CPP_BlanketRatingMessagingHelper

        Private Shared needToshowMessage As Boolean = False

        Public Shared Function CheckForAllBuildingAndPropertiesBlanket(ByRef Quote As QuickQuoteObject) As Boolean

            Dim countBuildingBlanket As Integer = 0
            Dim countPersonalProperty As Integer = 0
            Dim countPersonalPropertyOthers As Integer = 0
            Dim blanketType As String = Quote.BlanketRatingOptionId
            Dim HasBuildingAndContents As Boolean = False

            If Quote IsNot Nothing AndAlso Quote.Locations.IsLoaded() Then
                For Each l As QuickQuoteLocation In Quote.Locations
                    If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                        For Each b As QuickQuoteBuilding In l.Buildings
                            If blanketType = "1" Then
                                countBuildingBlanket += HasBuildingWithBlanket(b)
                                countPersonalProperty += HasPersonalPropertyBlanket(b)
                                countPersonalPropertyOthers += HasPersonalPropertyOthersBlanket(b)
                                HasBuildingAndContents = ((countBuildingBlanket >= 1) AndAlso (countPersonalProperty >= 1 OrElse countPersonalPropertyOthers >= 1))
                                If HasBuildingAndContents Then
                                    Exit For
                                End If
                            End If
                            If blanketType = "2" Then
                                countBuildingBlanket += HasBuildingWithBlanket(b)
                                If countBuildingBlanket > 1 Then
                                    Exit For
                                End If
                            End If
                            If blanketType = "3" Then
                                countPersonalProperty += HasPersonalPropertyBlanket(b)
                                countPersonalPropertyOthers += HasPersonalPropertyOthersBlanket(b)
                                If (countPersonalProperty > 0 AndAlso countPersonalPropertyOthers > 0) OrElse countPersonalProperty > 1 OrElse countPersonalPropertyOthers > 1 Then
                                    Exit For
                                End If
                            End If
                        Next
                    End If
                Next
            End If

            Select Case blanketType
                Case "1"  ' Building & Contents
                    If Not HasBuildingAndContents Then
                        needToshowMessage = True
                    Else
                        needToshowMessage = False
                    End If
                Case "2" ' Building Only
                    If countBuildingBlanket <2 Then
                        needToshowMessage= True
                    Else
                        needToshowMessage= False
                    End If
                Case "3"  ' Contents Only
                    If Not ((countPersonalProperty > 0 AndAlso countPersonalPropertyOthers > 0) OrElse countPersonalProperty > 1 OrElse countPersonalPropertyOthers > 1) Then
                        needToshowMessage = True
                    Else
                        needToshowMessage = False
                    End If
                Case "0"
                    needToshowMessage = False
            End Select

            Return needToshowMessage
        End Function

        Public Shared Function HasBuildingWithBlanket(b As QuickQuoteBuilding) As Integer
            Dim hasBuildingBlanket As Integer = 0
            If b IsNot Nothing Then
                If b.IsBuildingValIncludedInBlanketRating Then
                    hasBuildingBlanket = 1
                End If
            End If
            Return hasBuildingBlanket
        End Function

        Public Shared Function HasPersonalPropertyBlanket(b As QuickQuoteBuilding) As Integer
            Dim hasPersonalProperty As Integer = 0
            If b IsNot Nothing Then
                If b.PersPropCov_IncludedInBlanketCoverage Then
                    hasPersonalProperty = 1
                End If
            End If
            Return hasPersonalProperty
        End Function

        Public Shared Function HasPersonalPropertyOthersBlanket(b As QuickQuoteBuilding) As Integer
            Dim hasPersonalPropertyOthers As Integer = 0
            If b IsNot Nothing Then
                If b.PersPropOfOthers_IncludedInBlanketCoverage Then
                    hasPersonalPropertyOthers = 1
                End If
            End If
            Return hasPersonalPropertyOthers
        End Function

    End Class
End Namespace
