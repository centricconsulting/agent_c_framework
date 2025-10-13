
Imports System.Configuration
Imports System.Data.SqlClient

Namespace IFM.VR.Common.Helpers.FARM
    Public Class QueryHelper

        'All in VR
        '   CoverageCodeVersion
        '   FarmConstructionType
        '       FarmStructureType 24 --duplicate
        '           CoverageCode -joins 25 --duplicate
        '   LocationAcreageType
        '   BusinessPursuitType
        '   CoverageCode -no joins 29
        '   AdditionalInterestType
        '       FarmStructureType 32 --duplicate
        '           CoverageCode -joins 33 --duplicate
        '   MotorType 




        Public Function GetCaptionFromCovCodeId(CoverageCode_Id As Integer) As String
            Using sproc As New SPManager("connDiamondReports", "usp_get_CaptionFromCovCodeId")
                sproc.AddStringParameter("@CoverageCode_Id", CoverageCode_Id)
                Dim Data = sproc.ExecuteSPQuery()
                Return Data.Rows(0).Item("caption").ToString
            End Using
        End Function


    End Class
End Namespace


