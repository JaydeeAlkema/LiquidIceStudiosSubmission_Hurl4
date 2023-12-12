using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public class Slot : MonoBehaviour
	{
		[SerializeField] private Playfield playfield = null;
		[SerializeField] private int player = 0;

		private GameObject discInSlot = null;
		private Coroutine coroutine = null;

		void SetPlayfield(Playfield playfield)
		{
			this.playfield = playfield;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.name.Contains("Disc"))
			{
				Debug.Log($"{collision.name} entered {name}", this);
				discInSlot = collision.gameObject;
				coroutine = StartCoroutine(CountdownTillDiscIsStableInSlot());
			}
		}
		private void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.name.Contains("Disc"))
			{
				Debug.Log($"{collision.name} exited {name}", this);
				discInSlot = null;
				StopCoroutine(coroutine);
			}
		}

		private IEnumerator CountdownTillDiscIsStableInSlot()
		{
			yield return new WaitForSeconds(1);
			if (discInSlot == null) yield return null;
			if (discInSlot.GetComponentInParent<Rigidbody2D>().velocity != Vector2.zero) yield return null;

			player = discInSlot.name.Contains("P1") ? 1 : 2;
			Debug.Log($"<color=green>{discInSlot.name} now belongs to {name}</color>", this);
		}
	}
}
