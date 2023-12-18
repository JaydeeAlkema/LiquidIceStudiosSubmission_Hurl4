using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	public class Slot : MonoBehaviour
	{
		[SerializeField] private Playfield playfield = null;
		[SerializeField] private int player = 0;

		private GameObject discInSlot = null;
		private Coroutine coroutine = null;
		private bool checkingForCorrectvelocity = false;

		public void SetPlayfield(Playfield playfield)
		{
			this.playfield = playfield;
		}

		public int GetPlayer()
		{
			return player;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.name.Contains("Disc"))
			{
				discInSlot = collision.gameObject;
				checkingForCorrectvelocity = true;
				coroutine = StartCoroutine(CountdownTillDiscIsStableInSlot());
			}
		}
		private void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.name.Contains("Disc"))
			{
				discInSlot = null;
				checkingForCorrectvelocity = false;
				StopCoroutine(coroutine);
			}
		}

		// Could have probably done this with a timer in the Update method. But using a coroutine felt better in this case.
		private IEnumerator CountdownTillDiscIsStableInSlot()
		{
			Rigidbody2D rb2d = discInSlot.GetComponentInParent<Rigidbody2D>();
			while (checkingForCorrectvelocity == true)
			{
				if (discInSlot == null) yield return null;

				float velocityM = rb2d.velocity.magnitude;
				if (Mathf.Approximately(velocityM, 0f)) checkingForCorrectvelocity = false;

				yield return new WaitForSeconds(0.1f);
			}
			if (discInSlot != null)
			{
				string discName = discInSlot.transform.parent.name;
				string[] discNameSplit = discName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				player = discNameSplit[1] == "P1" ? 1 : 2;

				playfield.Wincheck(player);
				yield return null;
			}
		}
	}
}
