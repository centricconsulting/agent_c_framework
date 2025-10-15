Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services

Namespace Services.Common
    Public Module Common
        Delegate Function pMethod(req As DC.Services.Messages.RequestBase) As DC.Services.Messages.ResponseBase
        Public Const outFilePath As String = "c:\projects\tempfile\"
        Public ReadOnly Property Token As DCS.DiamondSecurityToken
            Get
                Return DCS.Proxies.ProxyBase.DiamondSecurityToken
            End Get
        End Property
        Public Sub SetDiamondToken(token As DCS.DiamondSecurityToken)
            DCS.Proxies.ProxyBase.DiamondSecurityToken = token
        End Sub
        ''' <summary>
        ''' Base diamond proxy call with error handling
        ''' </summary>
        ''' <param name="m"></param>
        ''' <param name="req"></param>
        ''' <param name="e">Exception bubbled from proxy call, will include any proxy validation error messages.</param>
        ''' <param name="dv">Reference to diamond validation object for debugging and error checking</param>
        ''' <returns></returns>
        ''' <remarks>Common call for Diamond Services</remarks>
        Public Function RunDiamondService(ByRef m As pMethod, req As DC.Services.Messages.RequestBase,
                                          Optional ByRef e As System.Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As DC.Services.Messages.ResponseBase
            Dim res As DCS.Messages.ResponseBase = Nothing
            Try
                res = m(req)
                dv = res.DiamondValidation
            Catch ex As Exception
                e = ex
            End Try
            'TODO: Add XML dumps for service calls and for error's
            Return res
        End Function

        Public Function BuildUniqueFilePath(BaseName) As String
            Return outFilePath + UniqueFileName(BaseName) + ".xml"
        End Function

        Public Function UniqueFileName(BaseName As String) As String
            Return BaseName + "_" + Date.Now.ToString("yyyy_MM_dd.HH_mm_ss")
        End Function

        Public Function DiamondImageDump(img As DCO.Policy.Image, Optional filePath As String = "") As Boolean
#If DEBUG Then
            Try
                If String.IsNullOrWhiteSpace(filePath) Then
                    img.DumpToFile(outFilePath + "imageOut." + Date.Now.ToString("yyyy_MM_dd.HH_mm_ss") + ".xml")
                Else
                    img.DumpToFile(filePath)
                End If
                Return True
            Catch ex As Exception
                Return False
            End Try
#End If
            Return True
        End Function
    End Module
End Namespace

