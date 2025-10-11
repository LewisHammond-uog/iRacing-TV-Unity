using System;
using TMPro;
using UnityEngine;


public class OverlayLapDelta : MonoBehaviour
{
	[SerializeField] private IPC ipc;
	[SerializeField] private GameObject enable;

	[SerializeField] private Color plusCol;
	[SerializeField] private Color minusCol;
	
	[Header("Imgs")]
	[SerializeField] private ImageSettings aheadHelmetImg;
	[SerializeField] private ImageSettings currentHelmetImg;
	[SerializeField] private ImageSettings behindHelmetImg;
	
	[Header("Names")]
	[SerializeField] private TMP_Text aheadName;
	[SerializeField] private TMP_Text currentName;
	[SerializeField] private TMP_Text prevName;

	[Header("Laps")] 
	[SerializeField] private TMP_Text[] lapCount;
	[SerializeField] private TMP_Text[] aheadLapDiff;
	[SerializeField] private TMP_Text[] currentLapDiff;
	[SerializeField] private TMP_Text[] behindLapDiff;

	private long indexLiveData;
	
	private void Update()
	{

		
		enable.SetActive(LiveData.Instance.isLapDeltaActive && !LiveData.Instance.isLiveSessionReplay && !LiveData.Instance.HasAnyBlockingCustomActive() && LiveData.Instance.liveDataControlPanel.masterOn  && !LiveData.Instance.liveDataIntro.show && !LiveData.Instance.liveDataRaceResult.show && ipc.isConnected && LiveData.Instance.isConnected );

		var data = LiveData.Instance.liveDataLapComp;

		if (indexLiveData != IPC.indexLiveData)
		{
			aheadHelmetImg.carIdx = data.aheadCarIdX;
			behindHelmetImg.carIdx = data.behindCarIdX;
			currentHelmetImg.carIdx = data.currentIdX;

			aheadName.text = data.aheadName;
			prevName.text = data.behindName;
			currentName.text = data.currentName;

			for (int i = 0; i < LiveData.Instance.liveDataLapComp.lapNums.Length; i++)
			{
				if (data.aheadCarIdX >= 0)
				{
					string aheadDiff = data.carAheadLastLapsDiff[i];
					aheadLapDiff[i].text = aheadDiff;
					SetColour(aheadLapDiff[i]);
				}

				if (data.behindCarIdX >= 0)
				{
					string behindText = data.carBehindLastLapsDiff[i];
					behindLapDiff[i].text = behindText;
					SetColour(behindLapDiff[i]);
				}

				currentLapDiff[i].text = data.thisCarLaps[i];
				lapCount[i].text = data.lapNums[i];
			}
			
			indexLiveData = IPC.indexLiveData;
		}
		
	}

	private void SetColour(TMP_Text tmp)
	{
		tmp.color = tmp.text.StartsWith("+") ? plusCol : minusCol;
	}
}
