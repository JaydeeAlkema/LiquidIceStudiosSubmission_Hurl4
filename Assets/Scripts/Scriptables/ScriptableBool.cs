using NaughtyAttributes;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Scriptables
{
	[CreateAssetMenu(fileName = "Scriptable Bool", menuName = "ScriptableObjects/New Scriptable Bool")]
	public class ScriptableBool : ScriptableObject
	{
		[SerializeField] private bool value;
		[SerializeField] private bool shouldReset = false;
		[SerializeField, ShowIf("shouldReset")] private bool startValue;

		public bool Value { get => value; set => this.value = value; }

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