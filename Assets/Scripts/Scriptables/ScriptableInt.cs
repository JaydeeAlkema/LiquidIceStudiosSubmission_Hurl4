using NaughtyAttributes;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Scriptables
{
	[CreateAssetMenu(fileName = "Scriptable Int", menuName = "ScriptableObjects/New Scriptable Int")]
	public class ScriptableInt : ScriptableObject
	{
		public int value;
		public bool resetOnDestroy = false;
		[ShowIf("resetOnDestroy")] public int startValue;

		private void OnDestroy()
		{
			Reset();
		}

		private void OnEnable()
		{
			Reset();
		}

		private void OnDisable()
		{
			Reset();
		}

		public void Reset()
		{
			value = startValue;
		}
	}
}