Imports System.Web.Http
Imports System.Threading.Tasks
Imports Microsoft.Ajax.Utilities
Imports Application.Services
Imports Application.DTOs

Namespace Controllers

    Public Class GeolocationController
        Inherits ApiController

        Private ReadOnly _weatherService As WeatherService

        Public Sub New(weatherService As WeatherService)
            _weatherService = weatherService
        End Sub

        <HttpGet>
        Public Async Function GetGeolocationByPublicIpAddress() As Task(Of IHttpActionResult)
            Dim response = Await _weatherService.GetGeolocationByPublicIpAddressAsync()
            Return Ok(response)
        End Function

        <HttpPost>
        Public Async Function GetGeolocationBySpecificIpAddress(<FromBody> request As GeolocationRequest) As Task(Of IHttpActionResult)
            If request Is Nothing OrElse Not ModelState.IsValid Then
                Return BadRequest("Invalid request parameters")
            End If

            Dim ipAddress As String = request.IpAddress

            If ipAddress.IsNullOrWhiteSpace Then
                Return BadRequest("Empty request parameters")
            End If

            Dim response = Await _weatherService.GetGeolocationByPublicIpAddressAsync(ipAddress)
            Return Ok(response)
        End Function

    End Class

End Namespace