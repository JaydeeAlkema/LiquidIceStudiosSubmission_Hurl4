using Assets.Scripts.Scriptables;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public class DiscThrower : MonoBehaviour
	{

		[SerializeField, BoxGroup("Settings")] private AnimationCurve throwPowerCurve;
		[SerializeField, BoxGroup("Settings")] private float throwPowerMultiplier = 15;
		[SerializeField, BoxGroup("Settings")] private float rotationSpeed = 90f;
		[SerializeField, BoxGroup("Settings")] private float startRotation = 0;
		[SerializeField, BoxGroup("Settings")] private Vector2 rotationClamps = new(-90, 90);

		[SerializeField, BoxGroup("References")] private List<GameObject> playerOneDiscs = new();
		[SerializeField, BoxGroup("References")] private List<GameObject> playerTwoDiscs = new();
		[Space]
		[SerializeField, BoxGroup("References")] private Transform playerOneStartPosition;
		[SerializeField, BoxGroup("References")] private GameObject playerOneStartPositionPowerIndicator;
		[SerializeField, BoxGroup("References")] private Transform playerTwoStartPosition;
		[SerializeField, BoxGroup("References")] private GameObject playerTwoStartPositionPowerIndicator;
		[SerializeField, BoxGroup("References")] private ScriptableInt playerOneDiscsCount;
		[SerializeField, BoxGroup("References")] private ScriptableInt playerTwoDiscsCount;
		[Space]
		[SerializeField, BoxGroup("References")] private ScriptableBool isGameOver = default;
		[SerializeField, BoxGroup("References")] private UIManager uiManager = default; // Im choosing not to use a singleton pattern, since the ui manager lives in the scene permenantly.

		[SerializeField, ReadOnly, BoxGroup("Privates")] private int currentPlayer = 0;
		[SerializeField, ReadOnly, BoxGroup("Privates")] private int playerOneIndex = 0;
		[SerializeField, ReadOnly, BoxGroup("Privates")] private int playerTwoIndex = 0;
		[Space]
		[SerializeField, ReadOnly, BoxGroup("Privates")] private Transform currentSpawnPoint = null;
		[SerializeField, ReadOnly, BoxGroup("Privates")] private GameObject currentPowerIndicator = null;
		[Space]
		[SerializeField, ReadOnly, BoxGroup("Privates")] private float rot = 0;
		[Space]
		[SerializeField, ReadOnly, BoxGroup("Privates")] private bool isPoweringUp = false;
		[SerializeField, ReadOnly, BoxGroup("Privates")] private bool hasReleasedDisc = true;
		[SerializeField, ReadOnly, BoxGroup("Privates")] private float poweringUpTime = 0;
		[SerializeField, ReadOnly, BoxGroup("Privates")] private float powerCurveValue = 0;

		private PlayerInputs inputs = default;

		private void Awake()
		{
			inputs = new PlayerInputs();

			playerOneStartPosition.gameObject.SetActive(false);
			playerTwoStartPosition.gameObject.SetActive(false);

			rot = startRotation;

			EnableInputs();
		}

		private void Start()
		{
			GameLoop();
		}

		private void Update()
		{
			if (isGameOver.value == true) return;

			ReadDirectionalInputs();
			PowerUpDisc();
		}

		private void OnDisable()
		{
			DisableInputs();
		}

		/// <summary>
		/// The main Game Loop. This handles setting the current player and indicators.
		/// </summary>
		private void GameLoop()
		{
			if (isGameOver.value == true) return;
			if (currentPlayer == 0) currentPlayer = Random.Range(1, 3);
			else currentPlayer = currentPlayer == 1 ? 2 : 1;

			if (currentPlayer == 1)
			{
				playerOneStartPosition.gameObject.SetActive(true);
				playerTwoStartPosition.gameObject.SetActive(false);
				currentSpawnPoint = playerOneStartPosition;
				currentPowerIndicator = playerOneStartPositionPowerIndicator;
			}
			else if (currentPlayer == 2)
			{
				playerOneStartPosition.gameObject.SetActive(false);
				playerTwoStartPosition.gameObject.SetActive(true);
				currentSpawnPoint = playerTwoStartPosition;
				currentPowerIndicator = playerTwoStartPositionPowerIndicator;
			}
		}

		/// <summary>
		/// Reads the directional inputs of both player 1 and player 2.
		/// This rotates the spawn point indicators.
		/// </summary>
		private void ReadDirectionalInputs()
		{
			float directionInput = currentPlayer == 1 ? inputs.Player.PlayerOne_DirectionKeys.ReadValue<float>() : inputs.Player.PlayerTwo_DirectionKeys.ReadValue<float>();
			if (directionInput == 0) return;

			float rotationChange = -directionInput * rotationSpeed * Time.deltaTime;
			rot += rotationChange;

			rot = Mathf.Clamp(rot, rotationClamps.x, rotationClamps.y);

			currentSpawnPoint.rotation = Quaternion.Euler(0, 0, rot);
		}

		/// <summary>
		/// Reads the power up inputs of both player 1 and player 2.
		/// </summary>
		private void PowerUpDisc()
		{
			float powerInput = currentPlayer == 1 ? inputs.Player.PlayerOne_PowerInput.ReadValue<float>() : inputs.Player.PlayerTwo_PowerInput.ReadValue<float>();
			isPoweringUp = powerInput != 0;

			if (isPoweringUp)
			{
				poweringUpTime += Time.deltaTime;
				powerCurveValue = throwPowerCurve.Evaluate(poweringUpTime);
				currentPowerIndicator.transform.localScale = new Vector3(1, 1 + (powerCurveValue * 4), 1);
				//Debug.Log(powerCurveValue);

				hasReleasedDisc = false;
			}
			if (!isPoweringUp && !hasReleasedDisc)
			{
				ReleaseDiscWithPower();
			}
		}

		/// <summary>
		/// Releases a disc with the power value that get set in the "PowerUpDisc" method.
		/// After releasing the disc, this method also resets a lot of values so the next player can have their turn.
		/// </summary>
		private void ReleaseDiscWithPower()
		{
			powerCurveValue = throwPowerCurve.Evaluate(poweringUpTime);
			//Debug.Log($"Released power at: {powerCurveValue}");

			GameObject discToThrow = currentPlayer == 1 ? playerOneDiscs[playerOneIndex] : playerTwoDiscs[playerTwoIndex];
			discToThrow.SetActive(true);

			float angle = currentSpawnPoint.transform.eulerAngles.z;
			angle += 90;
			float angleRad = angle * Mathf.Deg2Rad;
			Vector2 throwDirection = new(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
			discToThrow.GetComponent<Rigidbody2D>().velocity = powerCurveValue * throwPowerMultiplier * throwDirection;

			currentPowerIndicator.transform.localScale = new Vector3(1, 1, 1);
			currentSpawnPoint.transform.rotation = Quaternion.Euler(0, 0, startRotation);

			powerCurveValue = 0;
			poweringUpTime = 0f;
			rot = startRotation;

			switch (currentPlayer)
			{
				case 1:
					playerOneDiscsCount.value--;
					break;

				case 2:
					playerTwoDiscsCount.value--;
					break;

				default:
					break;
			}
			if (playerOneDiscsCount.value == 0 && playerTwoDiscsCount.value == 0)
			{
				isGameOver.value = true;
				uiManager.ToggleGameOverPanel(0);
			}
			if (currentPlayer == 1) playerOneIndex++;
			else if (currentPlayer == 2) playerTwoIndex++;

			hasReleasedDisc = true;
			GameLoop();
		}

		private void EnableInputs()
		{
			inputs.Player.PlayerOne_PowerInput.Enable();
			inputs.Player.PlayerOne_DirectionKeys.Enable();
			inputs.Player.PlayerTwo_PowerInput.Enable();
			inputs.Player.PlayerTwo_DirectionKeys.Enable();
		}
		private void DisableInputs()
		{
			inputs.Player.PlayerOne_PowerInput.Disable();
			inputs.Player.PlayerOne_DirectionKeys.Disable();
			inputs.Player.PlayerTwo_PowerInput.Disable();
			inputs.Player.PlayerTwo_DirectionKeys.Disable();
		}
	}
}