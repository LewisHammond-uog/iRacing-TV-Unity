using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Profiling;

namespace Data
{
	public class GoogleSheetsDataProvider : IDataProvider
	{
		private readonly string sheetId;
		private readonly string[] classSheets;
		private HttpClient httpClient;
		private List<IDriverData> latestData;

		private readonly CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
		{
			Delimiter = ",",
			BadDataFound = null,
			MissingFieldFound = null,
			IgnoreBlankLines = true,
			TrimOptions = TrimOptions.Trim
		};
		
		public GoogleSheetsDataProvider(string sheetId, string[] classSheets)
		{
			httpClient = new HttpClient();
			this.sheetId = sheetId;
			this.classSheets = classSheets;

			const int maxDrivers = 60;
			latestData = new List<IDriverData>(maxDrivers);
		}

		public async Task UpdateData()
		{
			latestData.Clear();
			
			var tasks = classSheets.Select(async sheet =>
			{
				string requestURI = GetSheetDownloadId(sheetId, sheet);
				var response = await httpClient.GetAsync(requestURI);
				return new { Sheet = sheet, Response = response };
			}).ToList();

			while (tasks.Count > 0)
			{
				var finishedTask = await Task.WhenAny(tasks);
				tasks.Remove(finishedTask);

				var response = await finishedTask; // contains both Sheet and Response

				Debug.Log($"Finished downloading sheet: {response.Sheet}");

				await using var contentStream = await response.Response.Content.ReadAsStreamAsync();
				using var reader = new StreamReader(contentStream);
				
				using var csv = new CsvReader(reader, csvConfig);

				await csv.ReadAsync();
				csv.ReadHeader();
				var headers = csv.HeaderRecord ?? Array.Empty<string>();

				int driverIdx = FindColumn(ref headers, "driver", "name");
				int carIdx    = FindColumn(ref headers, "car", "#", "car number");
				int pointsIdx = FindColumn(ref headers, "total", "points", "pts");

				while (await csv.ReadAsync())
				{
					if (csv.Parser.Record == null || string.Join("", csv.Parser.Record).Trim() == "")
						continue;

					if (driverIdx < 0 && carIdx < 0)
						continue;

					string driverName = driverIdx > 0 ? csv.GetField(driverIdx) : null;
					string driverNum = carIdx > 0 ? csv.GetField(carIdx) : null;
					
					if(string.IsNullOrWhiteSpace(driverName) || string.IsNullOrWhiteSpace(driverName))
						continue;
					
					int driverPoints = 0;
					if (pointsIdx > 0)
					{
						var ptsStr =  csv.GetField(pointsIdx);
						int.TryParse(ptsStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out driverPoints);
					}

					string driverClass = response.Sheet;
					
					latestData.Add(new DriverData(driverName, driverNum, driverPoints, driverClass));
				}
			}
			
			RefreshedDataReady?.Invoke(this, EventArgs.Empty);
			
			Debug.Log("Data Update Complete");
		}
	
		
		private static int FindColumn(ref string[] headers, params string[] candidates)
		{
			for (int i = 0; i < headers.Length; i++)
			{
				var h = (headers[i] ?? "").Trim().ToLowerInvariant();
				foreach (var cand in candidates)
				{
					if (h.Contains(cand.ToLowerInvariant()))
						return i;
				}
			}
			return -1;
		}

		public IList<IDriverData> TryGetAllDriverData()
		{
			try
			{
				return latestData;
			}
			finally
			{

			}

#pragma warning disable CS0162
			return null;
#pragma warning restore CS0162
		}

		public event EventHandler RefreshedDataReady;

		private string GetSheetDownloadId(string spreadSheetId, string sheetName)
		{
			return $"https://docs.google.com/spreadsheets/d/{spreadSheetId}/gviz/tq?tqx=out:csv&sheet={sheetName}";
		}
	}

	public interface IDataProvider
	{
		public Task UpdateData();
		public IList<IDriverData> TryGetAllDriverData();
		public event EventHandler RefreshedDataReady;
	}

	public class DriverData : IDriverData
	{
		public DriverData(string name, string carNum, int points, string driverClass)
		{
			this.DriverName = name;
			this.DriverCarNum = carNum;
			this.DriverPoints = points;
			this.DriverClass = driverClass;
		}
		
		public string DriverName { get; private set; }
		public int DriverPoints { get; private set;}
		public string DriverCarNum { get; private set; }
		public string DriverClass { get; private set;}
	}

	public interface IDriverData
	{
		public string DriverName { get; }
		public int DriverPoints { get; }
		public string DriverCarNum { get; }
		public string DriverClass { get; }
	}
}