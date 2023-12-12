using NaughtyAttributes;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Scriptables
{
	[CreateAssetMenu(fileName = "Scriptable Int", menuName = "ScriptableObjects/New Scriptable Int")]
	public class ScriptableInt : ScriptableObject
	{
		[SerializeField] private int value;
		[SerializeField] private bool shouldReset = false;
		[SerializeField, ShowIf("shouldReset")] private int startValue;

		public int Value { get => value; set => this.value = value; }


		private void OnDisable()
		{
			if (shouldReset) value = startValue;
		}

		private void OnDestroy()
		{
			if (shouldReset) value = startValue;
		}
	}
}