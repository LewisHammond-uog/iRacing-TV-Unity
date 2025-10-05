using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace Data
{
	public class ChampDisplay : MonoBehaviour
	{
		[SerializeField] private SerializedDictionary<string, Color> classToColor;
		[SerializeField] private Transform pagesTransform;
		[SerializeField] private GameObject slotPrefab;
		[SerializeField] private GameObject pagePrefab;
		
		[SerializeField, Min(1)] private int resultsPerPage = 10;

		private LinkedList<GameObject> allPages;
		private LinkedListNode<GameObject> currentPage = null;

		private void OnEnable()
		{
			allPages = new LinkedList<GameObject>();
		}

		public void CreateClass(string carClass, IEnumerable<IDriverData> sortedResults)
		{
			var currentPage = CreatePage(carClass);

			int slotsThisPage = 0;
			int pos = 1;
			IDriverData classLeader = null;
			foreach (IDriverData driver in sortedResults)
			{
				classLeader ??= driver;
				int pointsGapToLeader = classLeader.DriverPoints - driver.DriverPoints;
				
				var slot = Instantiate(slotPrefab).GetComponent<ChampSlot>();
				slot.transform.localScale = Vector3.one;

				slot.SetInfo(GetPositionStringOrdinal(pos++), driver.DriverName, driver.DriverCarNum, driver.DriverPoints, pointsGapToLeader, classToColor[carClass]);
				AddToPage(currentPage, slot);

				if (++slotsThisPage >= resultsPerPage)
				{
					currentPage = CreatePage(carClass);
					allPages.AddLast(currentPage);
				}
			}
		}

		public void MoveNext(out GameObject newCurrentPage)
		{
			if (currentPage == null)
			{
				currentPage = allPages.First;
				newCurrentPage = currentPage.Value;
				return;
			}

			if (currentPage.Next != null)
			{
				currentPage = currentPage.Next;
				newCurrentPage = currentPage.Value;
				return;
			}

			newCurrentPage = currentPage.Value;
		}
		
		public void MovePrev(out GameObject newCurrentPage)
		{
			if (currentPage == null)
			{
				currentPage = allPages.First;
				newCurrentPage = currentPage.Value;
				return;
			}

			if (currentPage.Previous != null)
			{
				currentPage = currentPage.Previous;
				newCurrentPage = currentPage.Value;
				return;
			}

			newCurrentPage = currentPage.Value;
		}

		private string GetPositionStringOrdinal(int pos)
		{
			switch (pos)
			{
				case > 3:
					return $"{pos}th";
				case 1:
					return "1st";
				case 2:
					return "2nd";
				case 3:
					return "3rd";
			}

			return "E";
		}

		private GameObject CreatePage(string className)
		{
			var obj = Instantiate(pagePrefab, pagesTransform, false);
			obj.transform.localScale = Vector3.one;
			allPages.AddLast(obj);
			
			obj.SetActive(false);

			obj.GetComponent<StandingsPage>().className = className;
			
			return obj;
		}

		private void AddToPage(GameObject page, ChampSlot slot)
		{
			slot.gameObject.transform.SetParent(page.transform, false);
		}
	}
}