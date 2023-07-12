# Weather Service API Documentation

Base URL: `https://weather-service-vbnet.azurewebsites.net/api/`

RESTful API developed with VB.NET. Provides information about current weather conditions for a specified geolocation. 
The API obtains data based on either the public IP address from the request or a specified input.

## Geolocation
### 1. Public IP
- **Endpoint:** `GET /geolocation`
- **Description:** Returns the geolocation of the request's public IP.

### 2. Custom IP
- **Endpoint:** `GET /geolocation/{ipAddress}`
- **Description:** Returns the geolocation for a provided IP.

####  Response Payload
```json
{
	"City": "Naaldwijk",
	"Region": "South Holland",
	"Country": "Netherlands",
	"Latitude": "51.9981",
	"Longitude": "4.198"
}
```

## Current Weather
### 1. Public IP Geolocation
- **Endpoint:** `GET /currentweather`
- **Description:** Returns the current weather details of the request's public IP geolocation.

### 2. Custom Geolocation
- **Endpoint:** `GET /currentweather/{geolocation}`
- **Description:** Returns the current weather for a provided geolocation.

#### Response Payload
```json
{
	"Geolocation": {
		"City": "Warsaw",
		"Region": null,
		"Country": "Poland",
		"Latitude": null,
		"Longitude": null
	},
	"TemperatureC": 19.0,
	"TemperatureF": 66.2,
	"Condition": "Clear",
	"Humidity": 46,
	"WindMph": 4.3,
	"WindKph": 6.8
}
```

## Error Handling
In case of an error, the API will return a response with the following structure.

```json
{
	"Message": "<ERROR_MESSAGE>"
}
```
