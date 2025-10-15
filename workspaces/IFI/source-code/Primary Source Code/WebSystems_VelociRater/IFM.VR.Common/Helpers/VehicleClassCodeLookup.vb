Namespace IFM.VR.Common.Helpers

    Public Class VehicleClassCodeLookup

        Public Structure ClassCodeLookupFields_Structure
            Dim VehicleRatingTypeId As String
            Dim UseCodeId As String
            Dim OperatorTypeId As String
            Dim OperatorUseId As String
            Dim SizeId As String
            Dim RadiusId As String
            Dim SecondaryClassId As String
            Dim SecondaryClassTypeId As String
            Dim TrailerTypeId As String
            Dim DumpingOperations As Boolean
            Dim SeasonalFarmUse As Boolean
        End Structure

        Public Shared Function GetVehicleClassCode(Optional ByVal Yr As String = Nothing, Optional ByVal Make As String = Nothing, Optional ByVal Model As String = Nothing, Optional ByVal RatingId As String = Nothing, Optional ByVal UseCodeId As String = Nothing, Optional OperatorId As String = Nothing, Optional ByVal OperatorTypeId As String = Nothing, Optional ByVal SizeId As String = Nothing, Optional ByVal TrailerTypeId As String = Nothing, Optional ByVal RadiusId As String = Nothing, Optional ByVal SecondaryClassId As String = Nothing, Optional ByVal SecondaryClassTypeId As String = Nothing) As String
            Dim cc As String = Nothing
            Dim v As New Diamond.Common.Objects.Policy.Vehicle()
            Dim FleetRated As Boolean = False
            Dim DiamondValidation As New Diamond.Common.Objects.DiamondValidation()
            Dim ReplaceExistingCode As Boolean = False

            Try
                If Yr IsNot Nothing Then v.Year = Yr
                If Make IsNot Nothing Then v.Make = Make
                If Model IsNot Nothing Then v.Model = Model
                If intParamOk(RatingId) Then v.VehicleRatingTypeId = RatingId
                If intParamOk(UseCodeId) Then v.UseCodeTypeId = UseCodeId
                If intParamOk(OperatorId) Then v.OperatorTypeId = OperatorId
                If intParamOk(OperatorTypeId) Then v.OperatorUseTypeId = OperatorTypeId
                If intParamOk(SizeId) Then v.SizeTypeId = SizeId
                If intParamOk(TrailerTypeId) Then v.TrailerTypeId = TrailerTypeId
                If intParamOk(RadiusId) Then v.RadiusTypeId = RadiusId
                If intParamOk(SecondaryClassId) Then v.SecondaryClassTypeId = SecondaryClassId
                If intParamOk(SecondaryClassTypeId) Then v.SecondaryClassUsageTypeId = SecondaryClassTypeId

                ' Operator Type id's - use default values
                'v.OperatorTypeId = 1
                'v.OperatorUseTypeId = 3

                'cc = Diamond.C0044.Common.Library.Utility.ComputeClassCode(v, FleetRated, DiamondValidation, ReplaceExistingCode, False)
                'updated 12/14/2020 to accommodate switch from using C0044 assemblies to Diamond Product Extensions (updated w/ Interoperability project); note: AddHandler calls placed in global.asax Application_Start (global excluded on Publish in PublishProfile) and cleanup handled by CacheMonitorModule section in web.config
                'AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf Diamond.Common.Utility.ObjectCreator.UIAssemblyResolve
                'AddHandler AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve, AddressOf Diamond.Common.Utility.ObjectCreator.UIAssemblyResolve
                Dim c0044Utility As Object = QuickQuote.CommonMethods.QuickQuoteHelperClass.DiamondClass("Diamond.C0044.Common.Library", "Utility")
                'Dim c0044Utility As Object = Diamond.Common.Utility.ObjectCreator.CreateClassDynamically("Diamond.C0044.Common.Library", "Utility", False)
                If c0044Utility IsNot Nothing Then
                    cc = c0044Utility.ComputeClassCode(v, FleetRated, DiamondValidation, ReplaceExistingCode, False)
                Else 'added 12/16/2020 so it will still work locally if the assembly isn't in the bin folder and Diamond fails to generate it
#If DEBUG Then
                    '''TODO: needs to extract to a reusable function -- possibly combining with the above.
                    Dim Diamond_C044_Common_Libary As System.Reflection.Assembly
                    Diamond_C044_Common_Libary = System.Reflection.Assembly.Load("Diamond.C0044.Common.Library")

                    If Diamond_C044_Common_Libary IsNot Nothing Then

                        Dim Utility As Object
                        Utility = Diamond_C044_Common_Libary.CreateInstance("Diamond.C0044.Common.Library.Utility")
                        cc = Utility?.ComputeClassCode(v, FleetRated, DiamondValidation, ReplaceExistingCode, False)
                    End If
#End If
                End If

                If Not String.IsNullOrWhiteSpace(cc) Then
                    Return (cc)
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Private Shared Function intParamOk(ByVal p As String) As Boolean
            If p IsNot Nothing AndAlso p.Trim <> String.Empty AndAlso IsNumeric(p) Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Takes a class code and looks up the class code 'buildup' fields
        ''' Returns a Diamond.Common.Objects.Policy.Vehicle object that should be populated with the data after the call
        ''' </summary>
        ''' <param name="ClassCode"></param>
        ''' <remarks></remarks>
        Public Shared Function ReverseClassCodeLookup(ByVal ClassCode As String) As ClassCodeLookupFields_Structure
            Dim cc As New ClassCodeLookupFields_Structure()
            Dim v As New Diamond.Common.Objects.Policy.Vehicle()

            Try
                'Diamond.C0044.Common.Library.Utility.SetVehicleValuesFromClassCode(ClassCode, v)
                'updated 12/14/2020 to accommodate switch from using C0044 assemblies to Diamond Product Extensions (updated w/ Interoperability project); note: AddHandler calls placed in global.asax Application_Start (global excluded on Publish in PublishProfile) and cleanup handled by CacheMonitorModule section in web.config
                'AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf Diamond.Common.Utility.ObjectCreator.UIAssemblyResolve
                'AddHandler AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve, AddressOf Diamond.Common.Utility.ObjectCreator.UIAssemblyResolve
                Dim c0044Utility As Object = QuickQuote.CommonMethods.QuickQuoteHelperClass.DiamondClass("Diamond.C0044.Common.Library", "Utility")
                If c0044Utility IsNot Nothing Then
                    c0044Utility.SetVehicleValuesFromClassCode(ClassCode, v)
                Else 'added 12/16/2020 so it will still work locally if the assembly isn't in the bin folder and Diamond fails to generate it
#If DEBUG Then
                    '''TODO: needs to extract to a reusable function -- possibly combining with the above.
                    Dim Diamond_C044_Common_Libary As System.Reflection.Assembly
                    Diamond_C044_Common_Libary = System.Reflection.Assembly.Load("Diamond.C0044.Common.Library")

                    Dim Utility As Object
                    Utility = Diamond_C044_Common_Libary.CreateInstance("Diamond.C0044.Common.Library.Utility")
                    cc = Utility.SetVehicleValuesFromClassCode(ClassCode, v)
#End If
                End If
                If v.Equals(New Diamond.Common.Objects.Policy.Vehicle()) Then Return Nothing
                If v.VehicleRatingTypeId = 0 Then Return Nothing

                cc.DumpingOperations = v.UsedInDumping
                cc.OperatorTypeId = v.OperatorTypeId
                cc.OperatorUseId = v.OperatorUseTypeId
                cc.RadiusId = v.RadiusTypeId
                If v.FarmUseCodeTypeId = "3" Then
                    cc.SeasonalFarmUse = True
                Else
                    cc.SeasonalFarmUse = False
                End If
                cc.SecondaryClassId = v.SecondaryClassTypeId
                cc.SecondaryClassTypeId = v.SecondaryClassUsageTypeId
                cc.SizeId = v.SizeTypeId
                cc.TrailerTypeId = v.TrailerTypeId
                cc.UseCodeId = v.UseCodeTypeId
                cc.VehicleRatingTypeId = v.VehicleRatingTypeId

                Return cc
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

    End Class

End Namespace
