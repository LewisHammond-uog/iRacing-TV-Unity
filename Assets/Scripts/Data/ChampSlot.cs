using TMPro;
using UnityEngine;

namespace Data
{
	public class ChampSlot : MonoBehaviour
	{
		[SerializeField] private TMP_Text posText;
		[SerializeField] private TMP_Text nameText;
		[SerializeField] private TMP_Text numText;
		[SerializeField] private TMP_Text pointsText;
		[SerializeField] private TMP_Text pointsDiffText;
		
		public void SetInfo(string positionOrdinal, string name, string number, int points, int pointsDiffToLeader, Color classColour)
		{
			posText.text = positionOrdinal;
			nameText.text = name;
			nameText.color = classColour;
			numText.text = number;
			pointsText.text = points.ToString();
			if (pointsDiffToLeader > 0)
			{
				pointsDiffText.text = $"-{pointsDiffToLeader.ToString()}";
			}
			else
			{
				pointsDiffText.text = "";
			}

		}
	}
}