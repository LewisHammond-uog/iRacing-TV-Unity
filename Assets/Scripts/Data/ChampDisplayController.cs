using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Data
{
	public class ChampDisplayController : MonoBehaviour
	{
		[SerializeField] ChampDisplay champDisplay;
		private GameObject currentPage;

		[SerializeField] private TMP_Text standingsText;
		
		private void Update()
		{
			if (Keyboard.current.pageUpKey.wasPressedThisFrame)
			{
				Debug.Log("Next Champ");
				if (currentPage != null)
				{
					currentPage.SetActive(false);
				}
				
				champDisplay.MoveNext(out currentPage);
				currentPage.SetActive(true);

				standingsText.text = $"Standings - {currentPage.GetComponent<StandingsPage>().className}";
			}
			
			if (Keyboard.current.pageDownKey.wasPressedThisFrame)
			{
				Debug.Log("Prev Champ");
				if (currentPage != null)
				{
					currentPage.SetActive(false);
				}
				
				champDisplay.MovePrev(out currentPage);
				currentPage.SetActive(true);
				
				standingsText.text = $"Standings - {currentPage.GetComponent<StandingsPage>().className}";
			}
		}
	}
}