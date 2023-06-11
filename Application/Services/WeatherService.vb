Imports System.Net.Http
Imports System.Threading.Tasks
Imports System.Dynamic
Imports Microsoft.Ajax.Utilities
Imports Newtonsoft.Json
Imports Application.Models
Imports System.Net

Namespace Services

    Public Class WeatherService

        Private ReadOnly _weatherApiKey As String = ConfigurationManager.AppSettings("WeatherApi.Key")
        Private ReadOnly _weatherApiUrl As String = $"http://api.weatherapi.com/v1/current.json?key={_weatherApiKey}"
        Private ReadOnly _geolocationApiUrl As String = "http://ip-api.com/json/"

        Private ReadOnly _defaultGeolocation As New Geolocation With {
            .City = "Seattle",
            .Region = "Washington",
            .Country = "United States",
            .Latitude = Nothing,
            .Longitude = Nothing
        }

        Private ReadOnly _httpClient As HttpClient
        Public Sub New(
            httpClient As HttpClient
        )
            _httpClient = httpClient
        End Sub

        Private Function ConvertObjectToStringWithoutNulls(obj As Object) As String
            Dim settings As New JsonSerializerSettings
            settings.NullValueHandling = NullValueHandling.Ignore

            Return JsonConvert.SerializeObject(obj, settings)
        End Function

        Public Async Function GetCurrentWeatherByGeolocationAsync(Optional geolocation As Geolocation = Nothing) As Task(Of CurrentWeather)

            If geolocation Is Nothing Then
                geolocation = Await GetGeolocationByPublicIpAddressAsync()
            End If

            Dim geolocationString As String

            If geolocation.Latitude Is Nothing Or geolocation.Longitude Is Nothing Then
                geolocationString = $"{geolocation.City}, {geolocation.Region}, {geolocation.Country}"
            Else
                geolocationString = $"{geolocation.Latitude},{geolocation.Longitude}"
            End If

            Dim response As String = Await _httpClient.GetStringAsync(_weatherApiUrl + $"&q={geolocationString}")

            Dim data As Object = JsonConvert.DeserializeObject(Of ExpandoObject)(response)

            Try
                Dim currentWeather As New CurrentWeather With
                {
                    .Geolocation = $"{data.location.name}, {data.location.region}, {data.location.country}",
                    .TemperatureC = data.current.temp_c,
                    .TemperatureF = data.current.temp_f,
                    .Condition = data.current.condition.text,
                    .Humidity = data.current.humidity,
                    .WindMph = data.current.wind_mph,
                    .WindKph = data.current.wind_kph
                }
                Return currentWeather
            Catch ex As Exception
                Debug.WriteLine($"Error: {ex.Message}")
                Return Nothing
            End Try

        End Function

        Private Function GetClientPublicIpAddress() As String
            Dim request = HttpContext.Current.Request
            Dim userHostAddress = request.UserHostAddress

            Try
                Dim hostIpAddress = IPAddress.Parse(userHostAddress)
                If hostIpAddress.AddressFamily = System.Net.Sockets.AddressFamily.InterNetworkV6 Then
                    Dim ipv4Address As IPAddress = hostIpAddress.MapToIPv4()
                    Return ipv4Address.ToString()
                Else
                    Return hostIpAddress.ToString()
                End If
            Catch
                Return userHostAddress
            End Try
        End Function

        Public Async Function GetGeolocationByPublicIpAddressAsync(Optional publicIpAddress As String = Nothing) As Task(Of Geolocation)

            If publicIpAddress Is Nothing Then
                Debug.WriteLine($"No public ip address passed")
                Try
                    publicIpAddress = GetClientPublicIpAddress()
                Catch ex As Exception
                    Debug.WriteLine($"Error: {ex.Message}")
                    Return _defaultGeolocation
                End Try
            End If

            Dim response As String = Await _httpClient.GetStringAsync(_geolocationApiUrl + publicIpAddress)

            If response.IsNullOrWhiteSpace Then
                Debug.WriteLine($"Invalid API response")
                Return _defaultGeolocation
            End If

            Dim data As Object = JsonConvert.DeserializeObject(Of ExpandoObject)(response)

            Dim geolocation
            Try
                geolocation = New Geolocation With {
                        .City = data.city,
                        .Region = data.regionName,
                        .Country = data.country,
                        .Latitude = data.lat,
                        .Longitude = data.lon
                    }
            Catch ex As Exception
                Debug.WriteLine($"Error: {ex.Message}")
                geolocation = _defaultGeolocation
            End Try

            Return geolocation

        End Function

    End Class

End Namespace