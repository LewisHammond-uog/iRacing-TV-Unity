using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace DefaultNamespace.Country
{
	[CreateAssetMenu(fileName = "FlagRef", menuName = "Data/Flags", order = 0)]
	public class CountryFlagRef : ScriptableObject
	{
		[SerializeField] public SerializedDictionary<string, Sprite> codeToImg;
		[SerializeField] public Sprite defaultSprite;
		
		private readonly Dictionary<string, string> translations = new Dictionary<string, string>()
		{
			{"nir", "gb-nir"},
			{"eng", "gb-eng"},
			{"sct", "gb-sct"},
			{"ct", "es-ct"},
			{"ga", "es-ga"},
			{"pv", "es-pv"}
		};

		public Sprite TryGetCountryCodeImg(string code)
		{
			code = code.ToLower();
			if (codeToImg.TryGetValue(code, out Sprite s))
				return s;
			
			if (code.Contains("-"))
			{
				string codeAfterDash = code.Substring(code.IndexOf('-') + 1);
				if (codeToImg.TryGetValue(codeAfterDash, out s))
					return s;
			}

			if (translations.TryGetValue(code, out string fixedCode))
			{
				if (codeToImg.TryGetValue(fixedCode, out s))
					return s;
			}
			
			
			return defaultSprite; // will be null if not found
		}
	}
}