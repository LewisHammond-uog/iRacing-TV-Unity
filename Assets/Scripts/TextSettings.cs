
using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class TextSettings : MonoBehaviour
{
	public string id = string.Empty;

	public bool allowResize = true;
	public bool allowChangeFontSettings = true;
	
	[NonSerialized] public RectTransform rectTransform;
	[NonSerialized] public TextMeshProUGUI text;

	[NonSerialized] public long indexSettings;
	[NonSerialized] public SettingsText settings;

	[NonSerialized] public GameObject border = null;
	[NonSerialized] public RectTransform border_RectTransform = null;

	[NonSerialized] public bool colorOverridden = false;

	public void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		text = GetComponent<TextMeshProUGUI>();
	}

	public void Update()
	{
		if ( border == null )
		{
			border = Instantiate( Border.border_GameObject );

			border.name = $"{transform.name} {Border.border_GameObject.name}";

			border.transform.SetParent( transform.parent );

			border.SetActive( true );

			border_RectTransform = border.GetComponent<RectTransform>();
			border_RectTransform.pivot = rectTransform.pivot;
		}

		if ( indexSettings != IPC.indexSettings )
		{
			indexSettings = IPC.indexSettings;

			if ( id == string.Empty )
			{
				enabled = false;
			}
			else
			{
				settings = Settings.overlay.GetTextSettingsData( id );

				if ( settings.fontIndex == SettingsText.FontIndex.None )
				{
					text.enabled = false;
				}
				else
				{
					text.enabled = true;

					if (allowChangeFontSettings)
					{
						text.fontSize = settings.fontSize;
						text.alignment = settings.alignment;
					}
					text.font = Fonts.GetFontAsset( settings.fontIndex );


					if ( !colorOverridden )
					{
						text.color = settings.tintColor;
					}

					text.overflowMode = ( settings.allowOverflow ) ? TextOverflowModes.Overflow : TextOverflowModes.Ellipsis;

					if (allowResize)
					{
						rectTransform.localPosition = new Vector2( settings.position.x, -settings.position.y );
						rectTransform.sizeDelta = settings.size;
					}


					border_RectTransform.localPosition = rectTransform.localPosition;
					border_RectTransform.sizeDelta = rectTransform.sizeDelta;

					if ( border_RectTransform.sizeDelta == Vector2.zero )
					{
						border_RectTransform.sizeDelta = new Vector2( 2, 2 );
					}
				}
			}
		}
	}

	public void SetColor( Color color )
	{
		colorOverridden = true;

		text.color = color;
	}
}
