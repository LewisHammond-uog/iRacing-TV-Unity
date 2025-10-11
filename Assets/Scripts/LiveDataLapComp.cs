using System;

[Serializable]
public class LiveDataLapComp
{
	public string[] carAheadLastLapsDiff = new string[5];
	public string[] carBehindLastLapsDiff= new string[5];
	public string[] thisCarLaps= new string[5];
	public string[] lapNums= new string[5];

	public int aheadCarIdX = 0;
	public string aheadName = string.Empty;

	public int currentIdX = 0;
	public string currentName = string.Empty;

	public int behindCarIdX = 0;
	public string behindName = string.Empty;

}