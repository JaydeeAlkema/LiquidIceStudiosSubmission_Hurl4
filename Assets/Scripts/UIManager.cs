using Assets.Scripts.Scriptables;
using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
	public class UIManager : MonoBehaviour
	{
		[SerializeField, BoxGroup("Floating Dynamic Text")] private TextMeshPro playerOneDiscsCountText = default;
		[SerializeField, BoxGroup("Floating Dynamic Text")] private TextMeshPro playerTwoDiscsCountText = default;
		[SerializeField, BoxGroup("Floating Dynamic Text")] private ScriptableInt playerOneDiscsCount;
		[SerializeField, BoxGroup("Floating Dynamic Text")] private ScriptableInt playerTwoDiscsCount;

		[SerializeField, BoxGroup("Game Over UI")] private GameObject gameOverPanel = default;
		[SerializeField, BoxGroup("Game Over UI")] private TextMeshProUGUI winningPlayerText = default;

		[SerializeField, BoxGroup("Pause UI")] private GameObject pauseMenu = default;

		[SerializeField, BoxGroup("Scriptable Objects To Reset")] private List<ScriptableInt> scriptableInts = new List<ScriptableInt>();
		[SerializeField, BoxGroup("Scriptable Objects To Reset")] private List<ScriptableBool> scriptableBools = new List<ScriptableBool>();

		private PlayerInputs inputs = default;

		private void Awake()
		{
			inputs = new PlayerInputs();

			inputs.UI.Escape.performed += ToggleEscapeMenu;
		}

		private void OnEnable()
		{
			inputs.UI.Enable();
			inputs.UI.Escape.performed += ToggleEscapeMenu;
		}

		private void OnDisable()
		{
			inputs.UI.Disable();
			inputs.UI.Escape.performed -= ToggleEscapeMenu;
		}

		private void FixedUpdate()
		{
			UpdateDiscsCountText();
		}
		private void ToggleEscapeMenu(InputAction.CallbackContext context)
		{
			pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
			Time.timeScale = pauseMenu.activeInHierarchy ? 0 : 1;
		}

		#region Floating Dynamic Text
		public void UpdateDiscsCountText()
		{
			playerOneDiscsCountText.text = playerOneDiscsCount.value.ToString();
			playerTwoDiscsCountText.text = playerTwoDiscsCount.value.ToString();
		}
		#endregion

		#region Game Over
		/// <summary>
		/// Toggles the Game Over screen
		/// </summary>
		/// <param name="player"> Int of the winning player. </param>
		public void ToggleGameOverPanel(int player)
		{
			gameOverPanel.SetActive(!gameOverPanel.activeInHierarchy);
			if (player == 0)
			{
				winningPlayerText.text = $"Draw!";
			}
			else
			{
				winningPlayerText.text = $"Player {player} won!";
			}
		}
		public void RetryGame()
		{
			foreach (ScriptableInt scriptableInt in scriptableInts)
			{
				scriptableInt.Reset();
			}

			foreach (ScriptableBool scriptableBool in scriptableBools)
			{
				scriptableBool.Reset();
			}
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		public void BackToMainMenu()
		{
			SceneManager.LoadScene(0);
		}
		public void QuitGame()
		{
			Application.Quit();
		}
		#endregion
	}
}