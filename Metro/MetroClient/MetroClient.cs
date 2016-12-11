﻿using System;
using System.Linq;
using MetroClient.Models;
using MetroUtility;
using Newtonsoft.Json.Linq;

namespace MetroClient
{
	public sealed class MetroClient : WebServiceClient
	{
		public MetroClient(Uri baseUri)
			: base(baseUri)
		{
		}

		public RouteDto GetRoute(string id)
		{
			return ToRouteDto(GetJson($"routes/{id}"));
		}

		public StopDto GetStop(string stopId)
		{
			return ToStopDto(GetJson($"stops/{stopId}"));
		}

		public StopsDto GetStops(string routeId)
		{
			return ToStopsDto(GetJson($"routes/{routeId}/stops"));
		}


		// TODO: Remove unused code.
		// (The scope of the project made it look like this information would be useful, but the API doesn't offer very meaningful information for this Vehicles object.
		public VehiclesDtos GetVehicles(string routeId)
		{
			return ToVehichlesDto(GetJson($"routes/{routeId}/vehicles"));
		}

		public PredictionsDto GetPredictions(string routeId, string stopId)
		{
			return ToPredictionsDto(GetJson($"routes/{routeId}/stops/{stopId}/predictions/"));
		}

		public TravelInformationDto GetTravelInformationDto(string routeId, string departureStopId, string arrivalStopId)
		{
			// I can't find a working example of a messasge object, and their trip making seems defunct so, I just made this up.
			return new TravelInformationDto
			{
				Message = "Looks like the bus driver is having a bad day today...",
				TravelDurationMinutes = 30
			};
		}

		private static RouteDto ToRouteDto(JObject route)
		{
			return new RouteDto
			{
				BackgroundColor = route["bg_color"].ToString(),
				DisplayName = route["display_name"].ToString(),
				ForegroundColor = route["fg_color"].ToString(),
				Id = int.Parse(route["id"].ToString())
			};
		}

		private static StopDto ToStopDto(JToken stop)
		{
			// Some of these values I would expect to be required values. Better documentation from the API would allow me to know which of these fields are required, and which are optional.  If I owned the API, I would let exceptions fly whenever a field is missing that is expected.
			return new StopDto
			{
				Id = stop["id"] == null
					? (int?) null
					: int.Parse(stop["id"].ToString()),
				DisplayName = stop["display_name"].ToString(),
				Latitude = stop["latitude"] == null
					? (double?) null
					: double.Parse(stop["latitude"].ToString() ),
				Longitude = stop["longitude"] == null
					? (double?) null
					: double.Parse(stop["longitude"].ToString())
			};
		}

		private static VehicleDto ToVehicleDto(JToken vehicle)
		{
			return new VehicleDto
			{
				Id = int.Parse(vehicle["id"].ToString()),
				Heading = int.Parse(vehicle["heading"].ToString()),
				RunId = vehicle["run_id"]?.ToString() ?? "",
				Predictable = ToBool(vehicle["predictable"].ToString()),
				RouteId =  vehicle["route_id"] == null
					? (int?) null
					: int.Parse(vehicle["route_id"].ToString()),
				SecondsSinceReport = int.Parse(vehicle["seconds_since_report"].ToString()),
				Latitude = double.Parse(vehicle["latitude"].ToString()),
				Longitude = double.Parse(vehicle["longitude"].ToString())
			};
		}

		private static PredictionDto ToPredictionDto(JToken prediction)
		{
			return new PredictionDto
			{
				BlockId = prediction["block_id"].ToString(),
				RunId = prediction["run_id"].ToString(),
				RouteId = int.Parse(prediction["route_id"].ToString()),
				IsDeparting = ToBool(prediction["is_departing"].ToString()),
				Minutes = int.Parse(prediction["minutes"].ToString()),
				Seconds = int.Parse(prediction["seconds"].ToString())
			};
		}

		private static StopsDto ToStopsDto(JObject stops)
		{
			return new StopsDto
			{
				Stops = stops["items"].Select(ToStopDto).EmptyIfNull()
			};
		}

		private static VehiclesDtos ToVehichlesDto(JObject vehicles)
		{
			return new VehiclesDtos
			{
				Vehicles = vehicles["items"].Select(ToVehicleDto).EmptyIfNull()
			};
		}

		private static PredictionsDto ToPredictionsDto(JObject predictions)
		{
			return new PredictionsDto
			{
				Predictions = predictions["items"].Select(ToPredictionDto).EmptyIfNull()
			};
		}

		private static bool ToBool(string value)
		{
			return value == "true";
		}
	}
}
