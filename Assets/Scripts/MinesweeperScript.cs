using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MinesweeperScript : MonoBehaviour {
	private Field field;

	public int NUM = 9;
	public int MINES = 10;

	public GameObject cellPrefab;
	public GameObject mineCellPrefab;
	public RawImage smileImage;
	public Texture[] textures;
	public Texture[] mineCountTextures;

	private float cellWidth = 1.0f;
	private int DIM;
	private bool finished;

	private int openedTiles;
	private bool victory;

	private GameObject[,] cells;
	private Vector3 clickedCoord;

	private void drawTable(int num) {

		cellPrefab.transform.localScale = new Vector3(cellWidth, 0.1f, cellWidth);

		for (int row = 0; row < num; ++row) {
			for (int col = 0; col < num; ++col) {
				cells[row, col] = (GameObject) Instantiate (cellPrefab, new Vector3 (row * cellWidth, 0, col * cellWidth), Quaternion.identity);
			}
		}
	}


	// Use this for initialization
	void Start () {
		cells = new GameObject[NUM, NUM];

		reset ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!finished && Input.GetMouseButtonDown (0)) {
			RaycastHit hitInfo = new RaycastHit ();

			bool hit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo);
			if (hit) {
				GameObject selectedGO = hitInfo.transform.gameObject;

				if (selectedGO.tag == "CellTag") {
					clickedCoord = selectedGO.GetComponent<Transform> ().position;
					int x = (int)clickedCoord.x;
					int z = (int)clickedCoord.z;

					if (field.getStateOf (x, z) == Field.STATE.NOTVISITED) {
						if (field.isTileBomb (x, z)) {
							kaboom (x, z);
						}
					}
				}
			} 

		}
	}

	public void openTile(int x, int z, int minesAround) {
		openedTiles++;
		Debug.Log ("OPENED: " + openedTiles);
		cells [x, z].gameObject.GetComponent<MeshRenderer>().materials[0].SetTexture("_MainTex", mineCountTextures[minesAround]);

		if ((DIM - openedTiles) == MINES) {
			victory = true;
			finished = true;
			smileImage.texture = textures [1];
			Debug.Log ("DIM: " + DIM + " opened: " + openedTiles);
			Debug.Log ("Since " + (DIM - openedTiles) + " == " + MINES);
		}
	}

	private void kaboom(int x, int z) {
		cells[x,z] = (GameObject) Instantiate (mineCellPrefab, new Vector3 (x * cellWidth, 0, z * cellWidth), Quaternion.identity);
		finished = true;
		smileImage.texture = textures [2];
	}

	public void reset() {
		DIM = NUM * NUM;

		smileImage.texture = textures [0];

		foreach (GameObject cell in cells) {
			Destroy (cell);
		}

		field = new Field (NUM, MINES, this);
		finished = false;
		drawTable (NUM);

		openedTiles = 0;
		victory = false;
	}

	public void ExitApplication() {
		Application.Quit ();
	}
}
