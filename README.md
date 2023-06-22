# WeatherService API Documentation

Base URL: https://weather-service-vbnet.azurewebsites.net/api/

## Models

### Geolocation
- `City` (String)
- `Region` (String)
- `Country` (String)
- `Latitude` (String)
- `Longitude` (String)

### CurrentWeather
- `Geolocation` (String)
- `TemperatureC` (Double)
- `TemperatureF` (Double)
- `Condition` (String)
- `Humidity` (Integer)
- `WindMph` (Double)
- `WindKph` (Double)

## Endpoints

### GeolocationController
- `GET /geolocation`: Returns geolocation of request's public IP.
- `POST /geolocation`: Returns geolocation for provided IP. Include IP in body.

### CurrentWeatherController
- `GET /currentweather`: Returns weather for request's public geolocation.
- `POST /currentweather`: Returns weather for provided geolocation. Include Geolocation in body.
