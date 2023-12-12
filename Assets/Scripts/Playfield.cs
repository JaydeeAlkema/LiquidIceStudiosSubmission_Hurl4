using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public class Playfield : MonoBehaviour
	{
		[SerializeField, BoxGroup("Settings")] private int rows = 7;
		[SerializeField, BoxGroup("Settings")] private int columns = 6;

		[SerializeField, BoxGroup("References")] private GameObject slotPrefab = default;
		[SerializeField, BoxGroup("References")] private UIManager uiManager = default; // Im choosing not to use a singleton pattern, since the ui manager lives in the scene permenantly.

		private GameObject[,] grid;
		private List<GameObject> slotsInScene = new();

		private bool gameOver = false;

		private void Start()
		{
			InitializeGrid();
		}

		[Button]
		private void InitializeGrid()
		{
			ClearGrid();
			grid = new GameObject[rows, columns];

			for (int row = 0; row < rows; row++)
			{
				for (int col = 0; col < columns; col++)
				{
					Vector2 position = new(transform.position.x + col, transform.position.y + row);
					GameObject slot = Instantiate(slotPrefab, position, Quaternion.identity, transform);
					slot.name = "Slot_" + row + "_" + col;
					grid[row, col] = slot;
					slotsInScene.Add(slot);
					slot.GetComponent<Slot>().SetPlayfield(this);
				}
			}
		}

		private void ClearGrid()
		{
			Transform[] children = GetComponentsInChildren<Transform>();
			for (int c = children.Length - 1; c >= 0; c--)
			{
				if (children[c] == null) continue;
				if (children[c].name == transform.name) continue;

				DestroyImmediate(children[c].gameObject);
			}
		}

		public void Wincheck(int player)
		{
			for (int row = rows - 1; row >= 0; row--)
			{
				for (int colum = columns - 1; colum >= 0; colum--)
				{
					if (gameOver) return;
					if (CheckForWin(row, colum, player))
					{
						gameOver = true;
						uiManager.ToggleGameOverPanel(player);
					}
				}
			}
		}

		private bool CheckForWin(int row, int col, int player)
		{
			// Check horizontally
			if (CheckLine(row, col, 0, 1, player)) return true;

			// Check vertically
			if (CheckLine(row, col, 1, 0, player)) return true;

			// Check diagonally (bottom-left to top-right)
			if (CheckLine(row, col, 1, 1, player)) return true;

			// Check diagonally (bottom-right to top-left)
			if (CheckLine(row, col, 1, -1, player)) return true;

			return false;
		}

		private bool CheckLine(int startRow, int startCol, int rowIncrement, int colIncrement, int player)
		{
			int count = 0;

			for (int i = -3; i <= 3; i++)
			{
				int row = startRow + i * rowIncrement;
				int col = startCol + i * colIncrement;

				if (IsInGrid(row, col) && grid[row, col].GetComponent<Slot>().GetPlayer() == player)
				{
					count++;

					if (count == 4)
					{
						return true;
					}
				}
				else
				{
					count = 0;
				}
			}

			return false;
		}

		private bool IsInGrid(int row, int col)
		{
			return row >= 0 && row < rows && col >= 0 && col < columns;
		}
	}
}
