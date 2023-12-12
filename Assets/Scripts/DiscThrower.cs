using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
	public class DiscThrower : MonoBehaviour
	{

		[SerializeField, BoxGroup("Settings")] private AnimationCurve throwPowerCurve;
		[SerializeField, BoxGroup("Settings")] private float throwPowerMultiplier = 15;
		[SerializeField, BoxGroup("Settings")] private float rotationSpeed = 90f;

		[SerializeField, BoxGroup("References")] private List<GameObject> playerOneDiscs = new();
		[SerializeField, BoxGroup("References")] private List<GameObject> playerTwoDiscs = new();
		[Space]
		[SerializeField, BoxGroup("References")] private Transform playerOneStartPosition;
		[SerializeField, BoxGroup("References")] private GameObject playerOneStartPositionPowerIndicator;
		[SerializeField, BoxGroup("References")] private Transform playerTwoStartPosition;
		[SerializeField, BoxGroup("References")] private GameObject playerTwoStartPositionPowerIndicator;

		[SerializeField, ReadOnly, BoxGroup("Privates")] private int currentPlayer = 0;
		[SerializeField, ReadOnly, BoxGroup("Privates")] private int playerOneIndex = 0;
		[SerializeField, ReadOnly, BoxGroup("Privates")] private int playerTwoIndex = 0;
		[Space]
		[SerializeField, ReadOnly, BoxGroup("Privates")] private PlayerInputs inputs = default;
		[SerializeField, ReadOnly, BoxGroup("Privates")] private Transform currentSpawnPoint = null;
		[SerializeField, ReadOnly, BoxGroup("Privates")] private GameObject currentPowerIndicator = null;
		[Space]
		[SerializeField, ReadOnly, BoxGroup("Privates")] private float rot = 0;
		[Space]
		[SerializeField, ReadOnly, BoxGroup("Privates")] private bool isPoweringUp = false;
		[SerializeField, ReadOnly, BoxGroup("Privates")] private bool hasReleasedDisc = true;
		[SerializeField, ReadOnly, BoxGroup("Privates")] private float poweringUpTime = 0;
		[SerializeField, ReadOnly, BoxGroup("Privates")] private float powerCurveValue = 0;


		private void Awake()
		{
			Application.targetFrameRate = 60;

			inputs = new PlayerInputs();

			playerOneStartPosition.gameObject.SetActive(false);
			playerTwoStartPosition.gameObject.SetActive(false);

			inputs.UI.Reset.performed += ResetGame;

			EnableInputs();
		}


		private void Start()
		{
			GameLoop();
		}

		private void Update()
		{
			ReadDirectionalInputs();
			PowerUpDisc();
		}

		private void OnDisable()
		{
			DisableInputs();
		}

		private void ResetGame(InputAction.CallbackContext context)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		/// <summary>
		/// The main Game Loop. This handles setting the current player and indicators.
		/// </summary>
		private void GameLoop()
		{
			if (currentPlayer == 0) currentPlayer = Random.Range(1, 3);
			else currentPlayer = currentPlayer == 1 ? 2 : 1;

			if (currentPlayer == 1)
			{
				playerOneStartPosition.gameObject.SetActive(true);
				playerTwoStartPosition.gameObject.SetActive(false);
				currentSpawnPoint = playerOneStartPosition;
				currentPowerIndicator = playerOneStartPositionPowerIndicator;
			}
			else
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
			rot = Mathf.Clamp(rot, -90, 90);
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
			Debug.Log($"Released power at: {powerCurveValue}");

			GameObject discToThrow = currentPlayer == 1 ? playerOneDiscs[playerOneIndex] : playerTwoDiscs[playerTwoIndex];
			discToThrow.SetActive(true);

			float angle = currentSpawnPoint.transform.eulerAngles.z;
			angle += 90;
			float angleRad = angle * Mathf.Deg2Rad;
			Vector2 throwDirection = new(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
			discToThrow.GetComponent<Rigidbody2D>().velocity = powerCurveValue * throwPowerMultiplier * throwDirection;

			currentPowerIndicator.transform.localScale = new Vector3(1, 1, 1);
			currentSpawnPoint.transform.rotation = Quaternion.Euler(0, 0, 0);

			powerCurveValue = 0;
			poweringUpTime = 0f;
			rot = 0;

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
			inputs.UI.Reset.Enable();
		}
		private void DisableInputs()
		{
			inputs.Player.PlayerOne_PowerInput.Disable();
			inputs.Player.PlayerOne_DirectionKeys.Disable();
			inputs.Player.PlayerTwo_PowerInput.Disable();
			inputs.Player.PlayerTwo_DirectionKeys.Disable();
			inputs.UI.Reset.Disable();
		}
	}
}