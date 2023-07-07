Imports System.Web.Http
Imports System.Threading.Tasks
Imports Microsoft.Ajax.Utilities
Imports Application.Services
Imports Application.DTOs.CurrentWeather
Imports Application.Models

Namespace Controllers

    Public Class CurrentWeatherController
        Inherits ApiController

        Private ReadOnly _weatherService As WeatherService

        Public Sub New(
            weatherService As WeatherService
        )
            _weatherService = weatherService
        End Sub

        <HttpGet>
        Public Async Function GetCurrentWeatherByPublicGeolocation() As Task(Of IHttpActionResult)
            Dim responsePayload As MethodResult(Of Object) = Await _weatherService.GetCurrentWeatherByGeolocationAsync()

            If Not responsePayload.Success Then
                Return BadRequest(responsePayload.ErrorMessage)
            End If

            Return Ok(responsePayload.Data)
        End Function

        <HttpGet>
        <Route("api/currentweather/bygeolocation")>
        Public Async Function GetCurrentWeatherBySpecificGeolocation(<FromBody> requestPayload As RequestPayloadDTO) As Task(Of IHttpActionResult)
            If requestPayload Is Nothing Then
                Return BadRequest("Invalid request")
            End If

            If requestPayload.Geolocation.IsNullOrWhiteSpace Then
                Return BadRequest("Geolocation required")
            End If

            Dim responsePayload As MethodResult(Of Object) = Await _weatherService.GetCurrentWeatherByGeolocationAsync(requestPayload.Geolocation)

            If Not responsePayload.Success Then
                Return BadRequest(responsePayload.ErrorMessage)
            End If

            Return Ok(responsePayload.Data)
        End Function

    End Class

End Namespace