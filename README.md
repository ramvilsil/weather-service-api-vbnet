# WeatherService API Documentation

The WeatherService is a RESTful API developed using VB.NET, it provides information about current weather conditions for a specified geolocation. The API obtains data based on either the public IP address from the request or a specified geolocation string.

Base URL: `https://weather-service-vbnet.azurewebsites.net/api/`

## Models

### Geolocation
Represents a geographical location with the following fields:

- `City` (String): The city of the geographical location.
- `Region` (String): The region (e.g., state or province) of the geographical location.
- `Country` (String): The country of the geographical location.
- `Latitude` (String): The north-south position on the Earth's surface.
- `Longitude` (String): The east-west position on the Earth's surface.

Example response:

```json
{
    "City": "Paris",
    "Region": "Ile-de-France",
    "Country": "France",
    "Latitude": "48.8566",
    "Longitude": "2.3522"
}
```

### CurrentWeather
Represents the current weather conditions with the following fields:

- `Geolocation` (Geolocation): The geographical location for the weather data.
- `TemperatureC` (Double): The current temperature in Celsius.
- `TemperatureF` (Double): The current temperature in Fahrenheit.
- `Condition` (String): The current weather condition (e.g., clear, cloudy, rainy).
- `Humidity` (Integer): The current humidity level as a percentage.
- `WindMph` (Double): The wind speed in miles per hour.
- `WindKph` (Double): The wind speed in kilometers per hour.

Example response:

```json
{
    "Geolocation": {
        "City": "Paris",
        "Region": "Ile-de-France",
        "Country": "France",
        "Latitude": null,
        "Longitude": null
    },
    "TemperatureC": 19.0,
    "TemperatureF": 66.2,
    "Condition": "Sunny",
    "Humidity": 45,
    "WindMph": 5.0,
    "WindKph": 8.0
}
```

## Endpoints

### GeolocationController

- `GET /geolocation`: Returns the geolocation of the request's public IP. Returns a `Geolocation` object.
- `POST /geolocation`: Returns the geolocation for a provided IP. Include a `GeolocationRequest` object in the body of the request.

Example:

    ```
    curl -X POST https://weather-service-vbnet.azurewebsites.net/api/geolocation \
    -H "Content-Type: application/json" \
    -d '{ "IpAddress": "192.0.2.0" }'
    ```

### CurrentWeatherController

- `GET /currentweather`: Returns the weather for the request's public geolocation. Returns a `CurrentWeather` object.
- `POST /currentweather`: Returns the weather for a provided geolocation. Include a `CurrentWeatherRequest` object in the body of the request.

Example:

    ```
    curl -X POST https://weather-service-vbnet.azurewebsites.net/api/currentweather \
    -H "Content-Type: application/json" \
    -d '{ "Geolocation": "Paris, France" }'
    ```
