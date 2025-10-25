using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace DefaultNamespace.Country
{
	[CreateAssetMenu(fileName = "FlagRef", menuName = "Data/Flags", order = 0)]
	public class CountryFlagRef : ScriptableObject
	{
		[SerializeField] public SerializedDictionary<string, Sprite> codeToImg;
	}
}