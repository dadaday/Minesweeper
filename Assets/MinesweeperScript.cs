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

	private float cellWidth = 1.0f;
	private int DIM;
	private bool finished;

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
		DIM = NUM * NUM;
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
					Debug.Log ("x: " + x + " z: " + z);
					Destroy (cells [x, z]);
					if (field.isTileBomb (x, z)) {
						kaboom (x, z);
					}
				}
			} 

		}
	}

	private void kaboom(int x, int z) {
		cells[x,z] = (GameObject) Instantiate (mineCellPrefab, new Vector3 (x * cellWidth, 0, z * cellWidth), Quaternion.identity);
		finished = true;
		smileImage.texture = textures [2];
	}

	public void reset() {
		smileImage.texture = textures [0];

		foreach (GameObject cell in cells) {
			Destroy (cell);
		}

		field = new Field (NUM, MINES);
		finished = false;
		drawTable (NUM);
	}

	public void ExitApplication() {
		Application.Quit ();
	}
}
