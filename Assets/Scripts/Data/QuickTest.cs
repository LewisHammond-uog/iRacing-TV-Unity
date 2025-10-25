using System;
using UnityEngine;

public class QuickTest : MonoBehaviour
{
	[SerializeField] private int carIdX;

	private void Update()
	{
		GetComponent<ImageSettings>().carIdx = carIdX;
	}
}
