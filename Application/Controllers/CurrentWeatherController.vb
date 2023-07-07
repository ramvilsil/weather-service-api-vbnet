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
        Public Async Function GetCurrentWeatherByPublicGeolocationAsync() As Task(Of IHttpActionResult)
            Dim responsePayload As MethodResult(Of Object) = Await _weatherService.GetCurrentWeatherByGeolocationAsync()

            If Not responsePayload.Success Then
                Return BadRequest(responsePayload.ErrorMessage)
            End If

            Return Ok(responsePayload.Data)
        End Function

        <HttpGet>
        <Route("api/currentweather/{geolocation}")>
        Public Async Function GetCurrentWeatherBySpecificGeolocationAsync(geolocation As String) As Task(Of IHttpActionResult)
            If geolocation Is Nothing OrElse geolocation.Trim() = String.Empty Then
                Return BadRequest("Geolocation required")
            End If

            Dim responsePayload As MethodResult(Of Object) = Await _weatherService.GetCurrentWeatherByGeolocationAsync(geolocation)

            If Not responsePayload.Success Then
                Return BadRequest(responsePayload.ErrorMessage)
            End If

            Return Ok(responsePayload.Data)
        End Function

    End Class

End Namespace