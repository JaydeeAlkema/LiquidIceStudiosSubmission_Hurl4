using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
	public class UIManager : MonoBehaviour
	{
		[SerializeField, BoxGroup("Game Over UI")] private GameObject gameOverPanel = default;
		[SerializeField, BoxGroup("Game Over UI")] private TextMeshProUGUI winningPlayerText = default;

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

		public void BackToMainMenu()
		{
			SceneManager.LoadScene("Main Menu");
		}

		public void QuitGame()
		{
			Application.Quit();
		}
		#endregion
	}
}