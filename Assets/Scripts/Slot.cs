using System;
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
				//Debug.Log($"{collision.name} entered {name}", this);
				discInSlot = collision.gameObject;
				checkingForCorrectvelocity = true;
				coroutine = StartCoroutine(CountdownTillDiscIsStableInSlot());
				Debug.Log(discInSlot.GetComponentInParent<Rigidbody2D>().velocity.magnitude);
			}
		}
		private void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.name.Contains("Disc"))
			{
				//Debug.Log($"{collision.name} exited {name}", this);
				discInSlot = null;
				checkingForCorrectvelocity = false;
				StopCoroutine(coroutine);
			}
		}

		private IEnumerator CountdownTillDiscIsStableInSlot()
		{
			Rigidbody2D rb2d = discInSlot.GetComponentInParent<Rigidbody2D>();
			while (checkingForCorrectvelocity == true)
			{
				float velocityM = rb2d.velocity.magnitude;
				if (discInSlot == null) yield return null;
				if (Mathf.Approximately(velocityM, 0f)) checkingForCorrectvelocity = false;

				//Debug.Log($"Velocity check: {velocityM}", this);
				yield return new WaitForSeconds(0.1f);
			}
			if (discInSlot != null)
			{
				string discName = discInSlot.transform.parent.name;
				string[] discNameSplit = discName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				player = discNameSplit[1] == "P1" ? 1 : 2;
				Debug.Log($"<color=green>{discName} now belongs to {name}</color>", this);

				playfield.Wincheck(player);
				yield return null;
			}
		}
	}
}
