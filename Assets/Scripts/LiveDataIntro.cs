
using System;

[Serializable]
public class LiveDataIntro
{
	public bool show = true;

	public LiveDataIntroDriver[] liveDataIntroDrivers = new LiveDataIntroDriver[ LiveData.MaxNumDrivers ];
}
