using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
	/// <summary>
	/// The Disc Thrower class is responsible for keeping track which players turn it currently it is, and, ofcourse, handling the throwing part.
	/// </summary>
	public class DiscThrower : MonoBehaviour
	{
		[SerializeField, BoxGroup("Runtime")] private int currentPlayer = 0; // 0 means nobody is playing. Basically a fail-safe

		[SerializeField, BoxGroup("Settings")] private AnimationCurve throwPowerCurve;
		[SerializeField, BoxGroup("Settings")] private float rotationSpeed = 90f;

		[SerializeField, BoxGroup("References")] private GameObject playerOneDiscPrefab;
		[SerializeField, BoxGroup("References")] private GameObject playerTwoDiscPrefab;
		[Space]
		[SerializeField, BoxGroup("References")] private Transform playerOneStartPosition;
		[SerializeField, BoxGroup("References")] private GameObject playerOneStartPositionPowerIndicator;
		[SerializeField, BoxGroup("References")] private Transform playerTwoStartPosition;
		[SerializeField, BoxGroup("References")] private GameObject playerTwoStartPositionPowerIndicator;


		private PlayerInputs inputs = default;
		private Transform currentSpawnPoint = null;
		private GameObject currentPowerIndicator = null;
		private GameObject currentDisc = null;
		private float rot = 0;
		private bool isPoweringUp = false;
		float poweringUpTime = 0;
		float powerCurveValue = 0;

		private void Awake()
		{
			inputs = new PlayerInputs();

			playerOneStartPosition.gameObject.SetActive(false);
			playerTwoStartPosition.gameObject.SetActive(false);

			EnableInputs();
		}

		private void Start()
		{
			GameLoop();
		}

		private void Update()
		{
			ReadPlayerInputs();
			GetPower();
		}

		private void OnDisable()
		{
			DisableInputs();
		}

		private void GameLoop()
		{
			if (currentPlayer == 0) currentPlayer = Random.Range(1, 3);
			else currentPlayer = currentPlayer == 1 ? 2 : 1;

			currentDisc = currentPlayer == 1 ? playerOneDiscPrefab : playerTwoDiscPrefab;
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

		private void ReadPlayerInputs()
		{
			float directionInput = currentPlayer == 1 ? inputs.Player.PlayerOne_DirectionKeys.ReadValue<float>() : inputs.Player.PlayerTwo_DirectionKeys.ReadValue<float>();
			if (directionInput == 0) return;

			float rotationChange = -directionInput * rotationSpeed * Time.deltaTime;
			rot += rotationChange;
			rot = Mathf.Clamp(rot, -90, 90);
			currentSpawnPoint.rotation = Quaternion.Euler(0, 0, rot);
		}

		private void GetPower()
		{
			float powerInput = currentPlayer == 1 ? inputs.Player.PlayerOne_PowerInput.ReadValue<float>() : inputs.Player.PlayerTwo_PowerInput.ReadValue<float>();
			isPoweringUp = powerInput != 0;

			if (isPoweringUp)
			{
				poweringUpTime += Time.deltaTime;
				powerCurveValue = throwPowerCurve.Evaluate(poweringUpTime);
				//currentPowerIndicator.transform.localScale = new Vector3(1, currentPowerIndicator.transform.localScale.y + powerCurveValue, 1);
				Debug.Log(powerCurveValue);
			}
			else
			{
				currentPowerIndicator.transform.localScale = new Vector3(1, 1, 1f);
				powerCurveValue = throwPowerCurve.Evaluate(poweringUpTime);
				Debug.Log($"Released power at: {powerCurveValue}");
				powerCurveValue = 0;
				poweringUpTime = 0f;
			}

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