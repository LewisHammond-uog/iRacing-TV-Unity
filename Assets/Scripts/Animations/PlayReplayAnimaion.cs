using System;
using EasyTransition;
using UnityEngine;

public class PlayReplayAnimaion : MonoBehaviour
{
	[SerializeField] private TransitionSettings transitionReplay;
	[SerializeField] private TransitionSettings transitionLive;

	private void OnEnable()
	{
		TransitionManager.Instance().Transition(transitionReplay, 0f);
	}

	private void OnDisable()
	{
		TransitionManager.Instance().Transition(transitionLive, 0f);
	}
}
