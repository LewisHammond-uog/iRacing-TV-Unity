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

		private int prevPage = -1;
		
		private void Update()
		{
			if (LiveData.Instance != null && LiveData.Instance.champResultCurrentPage != prevPage)
			{
				int wantedPage = LiveData.Instance.champResultCurrentPage;
				
				if (currentPage != null)
				{
					currentPage.SetActive(false);
				}

				if (wantedPage > prevPage)
				{
					champDisplay.MoveNext(out currentPage);
				}
				else
				{
					champDisplay.MovePrev(out currentPage);
				}
				
				currentPage.SetActive(true);
				standingsText.text = $"Standings - {currentPage.GetComponent<StandingsPage>().className}";
			}

			prevPage = LiveData.Instance.champResultCurrentPage;


			/*
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
			}*/
		}
	}
}