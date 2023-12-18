using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	/// <summary>
	/// Just a silly little class that spawns discs at random positions.
	/// Purely made to make the main menu a bit more interesting.
	/// </summary>
	public class RandomDiscSpawner : MonoBehaviour
	{
		[SerializeField, BoxGroup("References")] private List<Transform> spawnPoints = new();
		[SerializeField, BoxGroup("References")] private List<GameObject> discsToSpawn = new();

		[SerializeField, BoxGroup("Settings")] private float timeBetweenSpawns = 3;
		[SerializeField, BoxGroup("Settings")] private float timeBetweenResets = 7;
		[SerializeField, BoxGroup("Settings")] private int amountOfDiscsToSpawn = 42;

		[SerializeField, BoxGroup("Debugging"), ReadOnly] private int discIndex = 0;
		[SerializeField, BoxGroup("Debugging"), ReadOnly] private List<GameObject> spawnedDiscs = new();

		private void Start()
		{
			StartCoroutine(SpawnDiscsAtRandomSpawnPoints());
		}

		private IEnumerator SpawnDiscsAtRandomSpawnPoints()
		{
			while (spawnedDiscs.Count < amountOfDiscsToSpawn)
			{
				yield return new WaitForSeconds(timeBetweenSpawns);

				Transform randSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
				GameObject discToSpawn = discsToSpawn[discIndex];

				GameObject newDiscGO = Instantiate(discToSpawn, randSpawnPoint.transform.position, Quaternion.identity, transform);
				spawnedDiscs.Add(newDiscGO);

				discIndex = discIndex == 0 ? 1 : 0;
			}
			yield return new WaitForSeconds(timeBetweenResets);
			discIndex = 0;
			for (int i = spawnedDiscs.Count - 1; i >= 0; i--)
			{
				DestroyImmediate(spawnedDiscs[i]);
			}
			spawnedDiscs.Clear();
			StartCoroutine(SpawnDiscsAtRandomSpawnPoints());
		}
	}
}