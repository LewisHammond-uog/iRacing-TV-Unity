using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Properties;
using UnityEngine;

namespace DefaultNamespace
{
	public class OverlayYellowFlag : MonoBehaviour
	{
		[SerializeField] private GameObject enable;

		[SerializeField] private TMP_Text sectorText;

		private StringBuilder sb;

		private void Awake()
		{
			sb = new StringBuilder();
		}

		public void SetYellowIn(OverlayRaceStatus.SectorFlag sectors)
		{
			if (sectors == 0)
			{
				enable.SetActive(false);
				return;
			}

			// 1. Create a list to hold the sector labels that are set
			var activeSectors = new List<string>();
    
			// 2. Add the sector label to the list only if the flag is present
			if (sectors.HasFlag(OverlayRaceStatus.SectorFlag.Sector1))
			{
				activeSectors.Add("1");
			}
    
			if (sectors.HasFlag(OverlayRaceStatus.SectorFlag.Sector2))
			{
				activeSectors.Add("2");
			}
    
			if (sectors.HasFlag(OverlayRaceStatus.SectorFlag.Sector3))
			{
				activeSectors.Add("3");
			}
    
			// 3. Join the list elements with the desired divider
			// This automatically handles having one, two, or three sectors without extra logic.
			sb.Clear();
			sb.Append("Sector ");
			sb.Append(string.Join(",", activeSectors));

			sectorText.text = sb.ToString();
			enable.SetActive(true);
		}
	}
}