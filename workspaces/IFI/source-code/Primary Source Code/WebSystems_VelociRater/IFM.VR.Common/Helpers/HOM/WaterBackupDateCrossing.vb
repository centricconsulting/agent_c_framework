Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

'Added 6/7/2022 for task 74147 MLW
Namespace IFM.VR.Common.Helpers.HOM
    Public Class WaterBackupDateCrossing
        Public Shared Sub UpdateHPEE1020WaterBackup(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum)
            Dim MyLocation As QuickQuoteLocation = Quote.Locations(0)
            Dim hasPlusEnhancement1020 = MyLocation.SectionICoverages.FindAll(Function(c) c.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownersPlusEnhancementEndorsement1020).Any()
            If hasPlusEnhancement1020 then
                Select Case CrossDirection
                    Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                        'Add water backup to HPEE 1020
                        Dim hasWaterBackup = MyLocation.SectionICoverages.FindAll(Function(c) c.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.BackupSewersAndDrains).Any()
                        If NOT hasWaterBackup then
                            Dim newWBU As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
                            newWBU.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.BackupSewersAndDrains
                            newWBU.Address = New QuickQuoteAddress()
                            newWBU.IncludedLimit = "5,000"
                            newWBU.Address.StateId = "0"
                            MyLocation.SectionICoverages.CreateIfNull()
                            MyLocation.SectionICoverages.Add(newWBU)
                        End if
                    Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                        'remove water backup from HPEE 1020
                        For Each sc As QuickQuote.CommonObjects.QuickQuoteSectionICoverage In MyLocation.SectionICoverages
                            If sc.CoverageCodeId = "144" Then
                                MyLocation.SectionICoverages.Remove(sc)
                                Exit For
                            End If
                        Next
                End Select             
            End If
        End Sub
    End Class

End Namespace
