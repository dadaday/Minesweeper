using UnityEngine;
using System.Collections;

public class Field {
	private int NUM;
	private int MINES;
	private int[,] fieldArr;

	public Field(int n, int m) {
		NUM = n;
		MINES = m;
		fieldArr = new int[NUM, NUM];

		for (int x = 0; x < NUM; x++) {
			for (int y = 0; y < NUM; y++) {
				fieldArr [x, y] = 0;
			}
		}

		int count = MINES;

		while (count > 0) {
			int x = Random.Range (0, NUM);
			int z = Random.Range (0, NUM);

			if (fieldArr [x, z] == 0) {
				fieldArr [x, z] = -1;
				count--;
				Debug.Log ("MINIFIED " + x + "-" + z);
			}
		}
	}


	public bool isTileBomb(int x, int z) {
		if (fieldArr [x, z] == -1) {
			return true;
		}
		return false;
	}
}
