Imports System.Web.Http
Imports System.Threading.Tasks
Imports Microsoft.Ajax.Utilities
Imports Application.Services
Imports Application.DTOs
Imports Application.Models

Namespace Controllers

    Public Class CurrentWeatherController
        Inherits ApiController

        Private ReadOnly _weatherService As WeatherService

        Public Sub New(weatherService As WeatherService)
            _weatherService = weatherService
        End Sub

        <HttpGet>
        Public Async Function GetCurrentWeatherByPublicGeolocation() As Task(Of IHttpActionResult)
            Dim response As Result(Of Object) = Await _weatherService.GetCurrentWeatherByGeolocationAsync()

            If Not response.Success Then
                Return BadRequest(response.Data.ToString)
            End If

            Return Ok(response.Data)
        End Function

        <HttpPost>
        Public Async Function GetCurrentWeatherBySpecificGeolocation(<FromBody> request As CurrentWeatherRequest) As Task(Of IHttpActionResult)
            If request Is Nothing OrElse Not ModelState.IsValid Then
                Return BadRequest("Invalid request parameters")
            End If

            Dim geolocation As String = request.Geolocation

            If geolocation.IsNullOrWhiteSpace Then
                Return BadRequest("Empty request parameters")
            End If

            Dim response As Result(Of Object) = Await _weatherService.GetCurrentWeatherByGeolocationAsync(geolocation)

            If Not response.Success Then
                Return BadRequest(response.Data.ToString)
            End If

            Return Ok(response.Data)
        End Function

    End Class

End Namespace