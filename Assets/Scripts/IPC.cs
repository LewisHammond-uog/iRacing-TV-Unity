
using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class IPC : MonoBehaviour
{
	public const int MAX_MEMORY_MAPPED_FILE_SIZE = 1 * 1024 * 1024;

	[NonSerialized] public Mutex mutexSettings;
	[NonSerialized] public Mutex mutexLiveData;

	[NonSerialized] public MemoryMappedFile memoryMappedFileSettings;
	[NonSerialized] public MemoryMappedFile memoryMappedFileLiveData;

	[NonSerialized] public MemoryMappedViewAccessor memoryMappedViewAccessorSettings;
	[NonSerialized] public MemoryMappedViewAccessor memoryMappedViewAccessorLiveData;

	[NonSerialized] public static long indexSettings = 0;
	[NonSerialized] public static long indexLiveData = 0;

	[NonSerialized] public bool isConnected = false;
	[NonSerialized] public long lastUpdateMilliseconds = 0;
	[NonSerialized] public Stopwatch stopwatch;

	[NonSerialized] public Task updateStreamingTexturesTask;

	public void Awake()
	{
		mutexSettings = new Mutex( false, Program.MutexNameSettings );
		mutexLiveData = new Mutex( false, Program.MutexNameLiveData );

		memoryMappedFileSettings = MemoryMappedFile.CreateOrOpen( Program.IpcNameSettings, MAX_MEMORY_MAPPED_FILE_SIZE );
		memoryMappedFileLiveData = MemoryMappedFile.CreateOrOpen( Program.IpcNameLiveData, MAX_MEMORY_MAPPED_FILE_SIZE );

		memoryMappedViewAccessorSettings = memoryMappedFileSettings.CreateViewAccessor( 0, 0, MemoryMappedFileAccess.Read );
		memoryMappedViewAccessorLiveData = memoryMappedFileLiveData.CreateViewAccessor( 0, 0, MemoryMappedFileAccess.Read );

		stopwatch = new Stopwatch();

		stopwatch.Start();
	}

	public void Update()
	{
		var settingsUpdated = UpdateSettings();
		var liveDataUpdated = UpdateLiveData();

		if ( !settingsUpdated && !liveDataUpdated )
		{
			if ( ( stopwatch.ElapsedMilliseconds - lastUpdateMilliseconds > 1000 ) )
			{
				isConnected = false;
			}
		}
		else
		{
			isConnected = true;
			lastUpdateMilliseconds = stopwatch.ElapsedMilliseconds;
		}
	}

	public void OnDestroy()
	{
		memoryMappedViewAccessorSettings.Dispose();
		memoryMappedViewAccessorLiveData.Dispose();

		memoryMappedFileSettings.Dispose();
		memoryMappedFileLiveData.Dispose();

		mutexSettings.Dispose();
		mutexLiveData.Dispose();
	}

	public bool UpdateSettings()
	{
		var index = memoryMappedViewAccessorSettings.ReadInt64( 0 );

		if ( index != indexSettings )
		{
			var signalReceived = mutexSettings.WaitOne( 250 );

			if ( signalReceived )
			{
				var size = memoryMappedViewAccessorSettings.ReadUInt32( 8 );

				var buffer = new byte[ size ];

				memoryMappedViewAccessorSettings.ReadArray( 12, buffer, 0, buffer.Length );

				mutexSettings.ReleaseMutex();

				var xmlSerializer = new XmlSerializer( typeof( SettingsOverlay ) );

				var memoryStream = new MemoryStream( buffer );

				Settings.overlay = (SettingsOverlay) xmlSerializer.Deserialize( memoryStream );

				indexSettings = index;

				return true;
			}
		}

		return false;
	}

	private static XmlSerializer serializer = new XmlSerializer(typeof(LiveData));
	private static readonly MemoryStream memoryStream = new MemoryStream();
	
	public bool UpdateLiveData()
	{
		var index = memoryMappedViewAccessorLiveData.ReadInt64( 0 );

		if (index != indexLiveData)
		{
			var signalReceived = mutexLiveData.WaitOne(1);

			if (signalReceived)
			{
				var size = memoryMappedViewAccessorLiveData.ReadUInt32(8);
				var buffer = new byte[size];

				memoryMappedViewAccessorLiveData.ReadArray(12, buffer, 0, buffer.Length);
				mutexLiveData.ReleaseMutex();

				// Reset and write to memory stream properly
				memoryStream.SetLength(0);             // clear previous data
				memoryStream.Write(buffer, 0, buffer.Length);
				memoryStream.Position = 0;             // <<=== rewind!

				var serializer = new XmlSerializer(typeof(LiveData));
				var liveData = (LiveData)serializer.Deserialize(memoryStream);

				LiveData.Instance.Update(liveData);
				StreamingTextures.CheckForUpdates();

				indexLiveData = index;
				return true;
			}
		}

		return false;
	}
}
