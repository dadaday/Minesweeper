using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Field {
	private int NUM;
	private int MINES;
	private bool[,] fieldArr;

	private MinesweeperScript mScript;

	public enum STATE {
		NOTVISITED,
		VISITED,
		FINISHED
	};

	private List<List<int>> graph;

	private STATE[,] states;

	public Field(int n, int m, MinesweeperScript mS) {
		NUM = n;
		MINES = m;
		fieldArr = new bool[NUM, NUM];
		graph = new List<List<int>> ();
		states = new STATE[NUM,NUM];

		mScript = mS;

		for (int x = 0; x < NUM; x++) {
			for (int z = 0; z < NUM; z++) {
				fieldArr [x, z] = false;
				states [x, z] = STATE.NOTVISITED;

				List<int> temp = new List<int> ();

				if (x < (NUM-1)) {
					temp.Add ((x+1) * NUM + z);	
				}

				if (x > 0) {
					temp.Add ((x-1) * NUM + z);	
				}
				if (z > 0) {
					temp.Add (x * NUM + (z-1));	
				}
				if (z < (NUM-1)) {
					temp.Add (x * NUM + (z+1));	
				}

				graph.Insert(x * NUM + z, temp);
			}
		}

		int count = MINES;

		while (count > 0) {
			int x = Random.Range (0, NUM);
			int z = Random.Range (0, NUM);

			if (!fieldArr [x, z]) {
				fieldArr [x, z] = true;
				count--;
				Debug.Log ("MINIFIED " + x + "-" + z);
			}
		}
	}


	public bool isTileBomb(int x, int z) {
		if (fieldArr [x, z]) {
			return true;
		} else {
			dfs (x, z);
		}
		return false;
	}

	private void dfs(int x, int z) {
		states [x, z] = STATE.VISITED;
		if (fieldArr [x, z]) {
			Debug.Log (x + "|" + z + " is mined. SKIPPING");
			states [x, z] = STATE.FINISHED;
			return;
		}

		int mines = countMines (x, z);

		Debug.Log ("x: " + x + " z: " + z + " surrounded by " + mines + " mines");

		if (mines == 0) {
			foreach (int vertex in graph[x*NUM + z]) {
				int vertX = vertex / NUM;
				int vertZ = vertex % NUM;
				if (states [vertX, vertZ] == STATE.NOTVISITED) {
				
					dfs (vertX, vertZ);
				}
			}
		}

		mScript.openTile (x, z, mines);
		states [x, z] = STATE.FINISHED;
	}

	public STATE getStateOf(int x, int z) {
		return states [x, z];
	}

	private int countMines(int x, int z) {
		int minesAround = 0;

		if (x < (NUM-1)) {
			minesAround += (fieldArr [x + 1, z]) ? 1 : 0;

			if (z > 0) {
				minesAround += (fieldArr [x + 1, z - 1]) ? 1 : 0;
			}
			if (z < (NUM-1)) {
				minesAround += (fieldArr [x + 1, z + 1]) ? 1 : 0;
			}
		}
		if (x > 0) {
			minesAround += (fieldArr [x - 1, z]) ? 1 : 0;
			if (z > 0) {
				minesAround += (fieldArr [x - 1, z - 1]) ? 1 : 0;
			}
			if (z < (NUM-1)) {
				minesAround += (fieldArr [x - 1, z + 1]) ? 1 : 0;
			}
		}

		if (z > 0) {
			minesAround += (fieldArr [x, z - 1]) ? 1 : 0;
		}
		if (z < (NUM-1)) {
			minesAround += (fieldArr [x, z + 1]) ? 1 : 0;
		}

		return minesAround;
	}
}
