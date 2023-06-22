Imports System.Net
Imports System.Net.Http
Imports System.Threading.Tasks
Imports System.Dynamic
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Application.Models

Namespace Services

    Public Class WeatherService

        Private ReadOnly _weatherApiKey As String
        Private ReadOnly _weatherApiUrl As String
        Private ReadOnly _geolocationApiUrl As String

        Private ReadOnly _httpClient As HttpClient

        Public Sub New(
            httpClient As HttpClient,
            weatherApiKey As String,
            weatherApiUrl As String,
            geolocationApiUrl As String
        )
            _httpClient = httpClient
            _weatherApiKey = weatherApiKey
            _weatherApiUrl = weatherApiUrl & _weatherApiKey
            _geolocationApiUrl = geolocationApiUrl
        End Sub

        Public Async Function GetCurrentWeatherByGeolocationAsync(Optional geolocation As Object = Nothing) As Task(Of Result(Of Object))

            Dim geolocationQuery As String = geolocation

            Debug.WriteLine(geolocationQuery)

            If geolocation Is Nothing Then
                Dim result = Await GetGeolocationByPublicIpAddressAsync()
                If TypeOf result.Data Is Geolocation Then
                    geolocation = CType(result.Data, Geolocation)
                    If geolocation.Latitude IsNot Nothing AndAlso geolocation.Longitude IsNot Nothing Then
                        geolocationQuery = $"{geolocation.Latitude},{geolocation.Longitude}"
                    End If
                Else
                    Return result
                End If
            End If

            Dim response As HttpResponseMessage = Await _httpClient.GetAsync($"{_weatherApiUrl}&q={geolocationQuery}")

            Dim content As String = Await response.Content.ReadAsStringAsync()

            Debug.WriteLine($"Weather API response: {content}")

            If Not response.IsSuccessStatusCode Then
                Dim message = JObject.Parse(content)
                Return New Result(Of Object) With {
                        .Success = False,
                        .Data = message
                    }
            End If

            Dim data As Object = JsonConvert.DeserializeObject(Of ExpandoObject)(content)

            Dim currentWeather As CurrentWeather
            Try
                currentWeather = PopulateCurrentWeatherObj(data)
            Catch ex As Exception
                Return New Result(Of Object) With {
                        .Success = False,
                        .Data = "CurrentWeather object could not be populated"
                    }
            End Try

            Return New Result(Of Object) With {
                        .Success = True,
                        .Data = currentWeather
                    }
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

        Public Async Function GetGeolocationByPublicIpAddressAsync() As Task(Of Result(Of Object))

            Dim publicIpAddress As String
            Try
                publicIpAddress = GetClientPublicIpAddress()
            Catch ex As Exception
                Return New Result(Of Object) With {
                        .Success = False,
                        .Data = "Unable to access IP Address data"
                    }
            End Try

            Debug.WriteLine($"Public Ip Address passed: {publicIpAddress}")

            Dim response As HttpResponseMessage = Await _httpClient.GetAsync($"{_geolocationApiUrl}{publicIpAddress}")
            Dim content As String = Await response.Content.ReadAsStringAsync()

            Debug.WriteLine($"Geolocation API response: {content}")

            If Not response.IsSuccessStatusCode Then

                Dim message = JObject.Parse(content)

                Return New Result(Of Object) With {
                       .Success = False,
                       .Data = message
                   }
            End If

            Dim data = JsonConvert.DeserializeObject(Of ExpandoObject)(content)

            Dim geolocation As Geolocation
            Try
                geolocation = PopulateGeolocationObj(data)
            Catch ex As Exception
                Return New Result(Of Object) With {
                .Success = False,
                .Data = "Geolocation object could not be populated"
            }
            End Try

            Return New Result(Of Object) With {
                .Success = True,
                .Data = geolocation
            }
        End Function

        Public Async Function GetGeolocationByPublicIpAddressAsync(publicIpAddress As String) As Task(Of Result(Of Object))

            Dim response As HttpResponseMessage = Await _httpClient.GetAsync($"{_geolocationApiUrl}{publicIpAddress}")
            Dim content As String = Await response.Content.ReadAsStringAsync()

            Debug.WriteLine($"Geolocation API response: {content}")

            If Not response.IsSuccessStatusCode Then

                Dim message = JObject.Parse(content)

                Return New Result(Of Object) With {
                       .Success = False,
                       .Data = message
                   }
            End If

            Dim data As Object = JsonConvert.DeserializeObject(Of ExpandoObject)(content)

            Dim geolocation As Geolocation
            Try
                geolocation = PopulateGeolocationObj(data)
            Catch ex As Exception
                Return New Result(Of Object) With {
                .Success = False,
                .Data = "Geolocation object could not be populated"
            }
            End Try

            Return New Result(Of Object) With {
                .Success = True,
                .Data = geolocation
            }
        End Function

        Private Function PopulateCurrentWeatherObj(data As Object) As CurrentWeather
            Dim currentWeather = New CurrentWeather() With {
                .Geolocation = New Geolocation()
            }

            If data.location?.name IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(data.location.name.ToString()) Then
                currentWeather.Geolocation.City = data.location.name
            End If

            If data.location?.region IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(data.location.region.ToString()) Then
                currentWeather.Geolocation.Region = data.location.region
            End If

            If data.location?.country IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(data.location.country.ToString()) Then
                currentWeather.Geolocation.Country = data.location.country
            End If

            If data.current?.temp_c IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(data.current.temp_c.ToString()) Then
                currentWeather.TemperatureC = data.current.temp_c
            End If

            If data.current?.temp_f IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(data.current.temp_f.ToString()) Then
                currentWeather.TemperatureF = data.current.temp_f
            End If

            If data.current?.condition?.text IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(data.current.condition.text.ToString()) Then
                currentWeather.Condition = data.current.condition.text
            End If

            If data.current?.humidity IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(data.current.humidity.ToString()) Then
                currentWeather.Humidity = data.current.humidity
            End If

            If data.current?.wind_mph IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(data.current.wind_mph.ToString()) Then
                currentWeather.WindMph = data.current.wind_mph
            End If

            If data.current?.wind_kph IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(data.current.wind_kph.ToString()) Then
                currentWeather.WindKph = data.current.wind_kph
            End If

            Return currentWeather
        End Function

        Private Function PopulateGeolocationObj(data As Object) As Geolocation
            Dim geolocation = New Geolocation()

            If data?.City IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(data.City.ToString()) Then
                geolocation.City = data.City
            End If

            If data?.regionName IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(data.regionName.ToString()) Then
                geolocation.Region = data.regionName
            End If

            If data?.country IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(data.country.ToString()) Then
                geolocation.Country = data.country
            End If

            If data?.lat IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(data.lat.ToString()) Then
                geolocation.Latitude = data.lat
            End If

            If data?.lon IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(data.lon.ToString()) Then
                geolocation.Longitude = data.lon
            End If

            Return geolocation
        End Function

    End Class

End Namespace