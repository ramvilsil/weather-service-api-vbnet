Imports System.Web.Http
Imports System.Threading.Tasks
Imports Microsoft.Ajax.Utilities
Imports Application.Services
Imports Application.DTOs.Geolocation
Imports Application.Models

Namespace Controllers

    Public Class GeolocationController
        Inherits ApiController

        Private ReadOnly _weatherService As WeatherService

        Public Sub New(
            weatherService As WeatherService
        )
            _weatherService = weatherService
        End Sub

        <HttpGet>
        Public Async Function GetGeolocationByPublicIpAddressAsync() As Task(Of IHttpActionResult)
            Dim responsePayload As MethodResult(Of Object) = Await _weatherService.GetGeolocationByPublicIpAddressAsync()

            If Not responsePayload.Success Then
                Return BadRequest(responsePayload.ErrorMessage)
            End If

            Return Ok(responsePayload.Data)
        End Function

        <HttpGet>
        <Route("api/geolocation/{ipAddress}")>
        Public Async Function GetGeolocationBySpecificIpAddressAsync(ipAddress As String) As Task(Of IHttpActionResult)
            If ipAddress Is Nothing OrElse ipAddress.Trim() = String.Empty Then
                Return BadRequest("IpAddress required")
            End If

            Dim responsePayload As MethodResult(Of Object) = Await _weatherService.GetGeolocationByPublicIpAddressAsync(ipAddress)

            If Not responsePayload.Success Then
                Return BadRequest(responsePayload.ErrorMessage)
            End If

            Return Ok(responsePayload.Data)
        End Function

    End Class

End Namespace