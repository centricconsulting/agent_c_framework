Imports Diamond.Common.Objects.Claims.DC.Waterstreet
Imports Diamond.Common.Objects.Policy
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.CGL
    Public Class ClassCodeAssignmentHelper
        Private Shared _ClassCodeAssignmentSettings As NewFlagItem
        Public Shared ReadOnly Property ClassCodeAssignmentSettings() As NewFlagItem
            Get
                If _ClassCodeAssignmentSettings Is Nothing Then
                    _ClassCodeAssignmentSettings = New NewFlagItem("VR_CPP_CGL_ClassCodeAssignment_Settings")
                End If
                Return _ClassCodeAssignmentSettings
            End Get
        End Property

        Const ClassCodeAssignmentMsg As String = "General liability classifications with a premium basis of area must be assigned to a specific location address. Please confirm or update the addresses for these classes to ensure accurate premium calculation."
        Public Shared Function ClassCodeAssignmentEnabled() As Boolean
            Return ClassCodeAssignmentSettings.EnabledFlag
        End Function

        Public Shared Function ClassCodeAssignmentEffDate() As Date
            Return ClassCodeAssignmentSettings.GetStartDateOrDefault("3/15/2025")
        End Function

        Public Shared Sub UpdateClassCodeAssignment(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Change Class Code Assigment to Location, show message
                    Dim NeedsWarningMessage As Boolean = False
                    Dim hasMoreThanOneLocationOnQuote As Boolean = False
                    'see if the policy level class code(s) has the Premium Exposure Description that begins with "Area". Full description would be "Area, Products/Completed Operations". Sometimes this comes back truncated at Opera or Op. Do not know if there are more variations. 
                    'if so, move it to location level: Quote.Locations(LocationIndex).GLClassifications(ClassCodeIndex)
                    If Quote IsNot Nothing Then
                        If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 1 Then
                            'There is no rate/premium change if only one location on the quote, so no need to message user for one location
                            hasMoreThanOneLocationOnQuote = True
                        End If
                        If Quote.GLClassifications IsNot Nothing AndAlso Quote.GLClassifications.Count > 0 Then
                            Dim i As Integer = 0
                            Dim PolicyLevelGLClassificationsIndexesToRemove As List(Of Integer) = New List(Of Integer)
                            For Each glc As QuickQuoteGLClassification In Quote.GLClassifications
                                If Left(glc.PremiumBase, 4).ToUpper() = "AREA" Then
                                    'Move to Location level GL Classification
                                    If Quote.Locations(0).GLClassifications Is Nothing Then Quote.Locations(0).GLClassifications = New List(Of QuickQuote.CommonObjects.QuickQuoteGLClassification)
                                    Quote.Locations(0).GLClassifications.Add(glc)
                                    'Add index to list of Policy Level GLClassification indexes to remove
                                    PolicyLevelGLClassificationsIndexesToRemove.Add(i)
                                    'Set message flag, but only show message if quote also has more than one location
                                    NeedsWarningMessage = True
                                End If
                                i += 1
                            Next
                            'Remove from Policy level GL Classification
                            If PolicyLevelGLClassificationsIndexesToRemove IsNot Nothing AndAlso PolicyLevelGLClassificationsIndexesToRemove.Count > 0 Then
                                PolicyLevelGLClassificationsIndexesToRemove.Reverse()
                                For Each glcIndex In PolicyLevelGLClassificationsIndexesToRemove
                                    If Quote.GLClassifications.HasItemAtIndex(glcIndex) Then
                                        Quote.GLClassifications.RemoveAt(glcIndex)
                                    End If
                                Next
                            End If
                        End If
                    End If
                    If NeedsWarningMessage AndAlso hasMoreThanOneLocationOnQuote AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(ClassCodeAssignmentMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No Change
            End Select
        End Sub

        Public Shared Function IsClassCodeAssignmentAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, ClassCodeAssignmentSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
