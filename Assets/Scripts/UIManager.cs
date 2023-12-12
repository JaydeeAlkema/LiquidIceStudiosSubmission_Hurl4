using Assets.Scripts.Scriptables;
using NaughtyAttributes;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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

		private void FixedUpdate()
		{
			UpdateDiscsCountText();
		}

		#region Floating Dynamic Text
		public void UpdateDiscsCountText()
		{
			playerOneDiscsCountText.text = playerOneDiscsCount.Value.ToString();
			playerTwoDiscsCountText.text = playerTwoDiscsCount.Value.ToString();
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
			winningPlayerText.text = $"Player {player} won!";
		}
		public void RetryGame()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		public void QuitGame()
		{
			Application.Quit();
		}
		#endregion
	}
}