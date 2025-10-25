using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace DefaultNamespace.Country
{
	[CreateAssetMenu(fileName = "FlagRef", menuName = "Data/Flags", order = 0)]
	public class CountryFlagRef : ScriptableObject
	{
		[SerializeField] public SerializedDictionary<string, Sprite> codeToImg;

		public Sprite TryGetCountryCodeImg(string code)
		{
			if (codeToImg.TryGetValue(code, out Sprite s))
				return s;
			
			if (code.Contains("-"))
			{
				string codeAfterDash = code.Substring(code.IndexOf('-') + 1);
				if (codeToImg.TryGetValue(codeAfterDash, out s))
					return s;
			}

			// If all fails, try with dash removed (just in case)
			string codeNoDash = code.Replace("-", "");
			codeToImg.TryGetValue(codeNoDash, out s);
			return s; // will be null if not found
		}
	}
}