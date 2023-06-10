Imports System.Net.Http
Imports System.Threading.Tasks
Imports System.Dynamic
Imports Newtonsoft.Json
Imports Application.Models

Namespace Services

    Public Class WeatherService

        Private ReadOnly _weatherApiKey As String = ConfigurationManager.AppSettings("WeatherApi.Key")
        Private ReadOnly _httpClient As HttpClient
        Private ReadOnly _defaultGeolocation As New Geolocation With {
            .City = "Seattle",
            .Region = "Washington",
            .Country = "United States",
            .Latitude = Nothing,
            .Longitude = Nothing
        }

        Public Sub New(
            httpClient As HttpClient
        )
            _httpClient = httpClient
            Debug.WriteLine($"Weather API Key: {_weatherApiKey}")
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

            Dim apiUrl As String = $"http://api.weatherapi.com/v1/current.json?key={_weatherApiKey}&q={geolocationString}"

            Debug.WriteLine(apiUrl)

            Dim response As String = Await _httpClient.GetStringAsync(apiUrl)

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

        Private Async Function GetClientPublicIpAddressAsync() As Task(Of String)
            Dim clientPublicIpAddress As String = Await _httpClient.GetStringAsync("https://api.ipify.org")
            Return clientPublicIpAddress
        End Function

        Public Async Function GetGeolocationByPublicIpAddressAsync(Optional publicIpAddress As String = Nothing) As Task(Of Geolocation)

            Debug.WriteLine("GetGeoloctionByPublicIpAddressAsync")

            Dim apiUrl As String = "http://ip-api.com/json/"

            If publicIpAddress Is Nothing Then
                Try
                    publicIpAddress = Await GetClientPublicIpAddressAsync()
                Catch ex As Exception
                    Debug.WriteLine($"Error: {ex.Message}")
                    Return _defaultGeolocation
                End Try
            End If

            Dim requestBodyJson As String = JsonConvert.SerializeObject(
                    New With
                    {
                        .IpAddress = publicIpAddress
                    }
                )

            Dim content As New StringContent(requestBodyJson, Encoding.UTF8, "application/json")

            Dim response As HttpResponseMessage = Await _httpClient.PostAsync(apiUrl, content)

            If response.IsSuccessStatusCode Then

                Dim responseContent As String = response.Content.ReadAsStringAsync().Result

                Dim data As Object = JsonConvert.DeserializeObject(Of ExpandoObject)(responseContent)

                Debug.WriteLine(data.ToString())

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
                    Geolocation = _defaultGeolocation
                End Try

                Return geolocation
            Else
                Debug.WriteLine($"Error: {response.StatusCode}")
                Return _defaultGeolocation
            End If

        End Function

    End Class

End Namespace