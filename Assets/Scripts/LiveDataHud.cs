﻿
using System;

using UnityEngine;

[Serializable]
public class LiveDataHud
{
	public string fuel = string.Empty;
	public Color fuelColor = Color.white;

	public string lapsToLeader = string.Empty;

	public string rpm = string.Empty;
	public Color rpmColor = Color.white;

	public string speed = string.Empty;

	public string gear = string.Empty;

	public string gapTimeFront = string.Empty;
	public Color gapTimeInFrontColor = Color.white;

	public string gapTimeBack = string.Empty;
	public Color gapTimeInBackColor = Color.white;

	public string speechToText = string.Empty;

	public bool showLeftSpotterIndicator = false;
	public bool showRightSpotterIndicator = false;
}
