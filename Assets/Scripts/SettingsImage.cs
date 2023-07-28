﻿
using System;

using UnityEngine;

[Serializable]
public class SettingsImage
{
	public enum ImageType
	{
		None,
		ImageFile,
		SeriesLogo,
		CarNumber,
		Car,
		Helmet,
		Driver
	}

	public ImageType imageType = ImageType.None;

	public string filePath = string.Empty;

	public Vector2 position = Vector2.zero;
	public Vector2 size = Vector2.zero;
	public Color tintColor = Color.white;
	public Vector4 border = Vector4.zero;
	public Vector2 frameSize = Vector2.zero;
	public int frameCount = 1;
	public float animationSpeed = 10;
	public bool tilingEnabled = false;
}
