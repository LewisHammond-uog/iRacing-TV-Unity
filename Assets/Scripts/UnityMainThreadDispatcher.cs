using System;
using System.Collections.Concurrent;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
	private static readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();
	private static UnityMainThreadDispatcher _instance;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Initialize()
	{
		if (_instance == null)
		{
			var obj = new GameObject("UnityMainThreadDispatcher");
			_instance = obj.AddComponent<UnityMainThreadDispatcher>();
			DontDestroyOnLoad(obj);
		}
	}

	public static void Enqueue(Action action)
	{
		if (action == null) return;
		_actions.Enqueue(action);
	}

	private void Update()
	{
		while (_actions.TryDequeue(out var action))
		{
			action?.Invoke();
		}
	}
}