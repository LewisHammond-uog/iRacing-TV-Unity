
using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using UnityEngine;

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

	private static readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(LiveData));
	private  byte[] _buffer = new byte[65536]; // preallocate a reasonably large buffer (adjust size)
	private  MemoryStream _memoryStream = new MemoryStream();
	
	private readonly object _deserializeLock = new object(); // protect reuse of stream
	
	private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(LiveData));

public bool UpdateLiveData()
    {
        var index = memoryMappedViewAccessorLiveData.ReadInt64(0);
        if (index == indexLiveData)
            return false;

        if (!mutexLiveData.WaitOne(250))
            return false;

        try
        {
            var size = memoryMappedViewAccessorLiveData.ReadUInt32(8);

            // Ensure buffer is large enough
            if (_buffer.Length < size)
                Array.Resize(ref _buffer, (int)size);

            memoryMappedViewAccessorLiveData.ReadArray(12, _buffer, 0, (int)size);

            // Copy for background thread (avoid sharing mutable buffer)
            var payload = new byte[size];
            Buffer.BlockCopy(_buffer, 0, payload, 0, (int)size);
            
	        LiveData liveData;
	        lock (_deserializeLock)
	        {
	            _memoryStream.SetLength(0);
	            _memoryStream.Write(payload, 0, payload.Length);
	            _memoryStream.Position = 0;

	            using (var reader = XmlReader.Create(_memoryStream, new XmlReaderSettings
	            {
	                IgnoreWhitespace = true,
	                IgnoreComments = true,
	                DtdProcessing = DtdProcessing.Ignore
	            }))
	            {
	                liveData = (LiveData)_serializer.Deserialize(reader);
	            }
	        }


	        LiveData.Instance.Update(liveData);
	        StreamingTextures.CheckForUpdates();
                    


            indexLiveData = index;
            return true;
        }
        finally
        {
            mutexLiveData.ReleaseMutex();
        }
    }
}
