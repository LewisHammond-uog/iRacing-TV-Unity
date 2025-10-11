
using System;
using System.Xml.Serialization;

[Serializable]
public class LiveData
{
	public const int MaxNumDrivers = 64;
	public const int MaxNumClasses = 8;
	public const int MaxNumCustom = 6;

	public static LiveData Instance { get; private set; }

	public bool isConnected = false;

	public string systemMessage = string.Empty;

	public LiveDataSteamVr liveDataSteamVr = new();
	public LiveDataControlPanel liveDataControlPanel = new();
	public LiveDataDriver[] liveDataDrivers = new LiveDataDriver[ MaxNumDrivers ];
	public LiveDataRaceStatus liveDataRaceStatus = new();
	public LiveDataLeaderboard[] liveDataLeaderboards = null;
	public LiveDataRaceResult liveDataRaceResult = new();
	public LiveDataVoiceOf liveDataVoiceOf = new();
	public LiveDataChyron liveDataChyron = new();
	public LiveDataBattleChyron liveDataBattleChyron = new();
	public LiveDataSubtitle liveDataSubtitle = new();
	public LiveDataIntro liveDataIntro = new();
	public LiveDataStartLights liveDataStartLights = new();
	public LiveDataTrackMap liveDataTrackMap = new();
	public LiveDataPitLane liveDataPitLane = new();
	//public LiveDataHud liveDataHud = new();
	//public LiveDataTrainer liveDataTrainer = new();
	//public LiveDataWebcamStreaming liveDataWebcamStreaming = new();

	public LiveDataCustom[] liveDataCustom = new LiveDataCustom[ MaxNumCustom ];

	
	public bool isLiveSessionReplay = false;
	public int champResultCurrentPage = 0;
	
		
	public LiveDataLapComp liveDataLapComp = new(); 



	public string seriesLogoTextureUrl = string.Empty;
	public string trackLogoTextureUrl = string.Empty;
	public string trackTextureUrl = string.Empty;

	[XmlIgnore] public bool isLapDeltaActive = false;

	public bool HasAnyBlockingCustomActive()
	{
		if (Instance.liveDataControlPanel.customLayerOn == null)
		{
			return false;
		}

		isLapDeltaActive = false;

		for (int index = 0; index < LiveData.Instance.liveDataControlPanel.customLayerOn.Length; index++)
		{
			//Exclude 4
			if (index == 4)
			{
				isLapDeltaActive = LiveData.Instance.liveDataControlPanel.customLayerOn[index];
				continue;
			}
			
			bool isOn = LiveData.Instance.liveDataControlPanel.customLayerOn[index];
			if (isOn)
			{
				return true;
			}
		}

		return false;
	}

	static LiveData()
	{
		Instance = new LiveData();
	}

	private LiveData()
	{
		Instance = this;
	}

	public void Update( LiveData liveData )
	{
		liveDataSteamVr = liveData.liveDataSteamVr;
		liveDataControlPanel = liveData.liveDataControlPanel;
		liveDataDrivers = liveData.liveDataDrivers;
		liveDataRaceStatus = liveData.liveDataRaceStatus;
		liveDataLeaderboards = liveData.liveDataLeaderboards;
		liveDataRaceResult = liveData.liveDataRaceResult;
		liveDataVoiceOf = liveData.liveDataVoiceOf;
		liveDataChyron = liveData.liveDataChyron;
		liveDataBattleChyron = liveData.liveDataBattleChyron;
		liveDataSubtitle = liveData.liveDataSubtitle;
		liveDataIntro = liveData.liveDataIntro;
		liveDataStartLights = liveData.liveDataStartLights;
		liveDataTrackMap = liveData.liveDataTrackMap;
		liveDataPitLane = liveData.liveDataPitLane;
		liveDataCustom = liveData.liveDataCustom;

		seriesLogoTextureUrl = liveData.seriesLogoTextureUrl;
		trackLogoTextureUrl = liveData.trackLogoTextureUrl;
	}
}
