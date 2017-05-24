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

	public Text mineCountText;
	public Text timerCounter;

	private float cellWidth = 1.0f;
	private int DIM;
	private bool finished;

	private int openedTiles;
	private bool victory;

	private GameObject[,] cells;
	private Vector3 clickedCoord;
	private bool[,] isFlagged;
	private bool[,] isOpened;

	private float lastUpdate = 0.0f;
	public float timeBetweenUpdates = 1.0f;

	private void drawTable(int num) {

		cellPrefab.transform.localScale = new Vector3(cellWidth, 0.1f, cellWidth);

		for (int row = 0; row < num; ++row) {
			for (int col = 0; col < num; ++col) {
				cells[row, col] = (GameObject) Instantiate (cellPrefab, new Vector3 (row * cellWidth, 0, col * cellWidth), Quaternion.identity);
				isFlagged [row, col] = false;
				isOpened [row, col] = false;
			}
		}
	}


	// Use this for initialization
	void Start () {
		cells = new GameObject[NUM, NUM];

		reset ();
	}

	private void updateTimer() {
		int time = int.Parse(timerCounter.text);
		timerCounter.text = (time + 1).ToString ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!finished) {
			
			lastUpdate += Time.deltaTime;
			if (lastUpdate > timeBetweenUpdates) {
				updateTimer ();
				lastUpdate = 0.0f;
			}

			if (Input.GetMouseButtonDown (0)) {
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
			if (Input.GetMouseButtonDown (1)) {
				RaycastHit hitInfo = new RaycastHit ();

				bool hit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo);
				if (hit) {
					GameObject selectedGO = hitInfo.transform.gameObject;

					if (selectedGO.tag == "CellTag") {
						clickedCoord = selectedGO.GetComponent<Transform> ().position;
						int x = (int)clickedCoord.x;
						int z = (int)clickedCoord.z;
						if (!isOpened [x, z]) {
							Texture flagOrEmpty;
							int minesC = int.Parse(mineCountText.text);
							if (isFlagged [x, z]) {
								flagOrEmpty = textures [4];
								minesC++;
							} else {
								flagOrEmpty = textures [3];
								minesC--;
							}
							mineCountText.text = minesC.ToString ();
							isFlagged [x, z] = !isFlagged [x, z];
							cells [x, z].gameObject.GetComponent<MeshRenderer> ().materials [0].SetTexture ("_MainTex", flagOrEmpty);
						}
					}
				}
			}
		}
		if(Input.GetKeyDown(KeyCode.Space)) {
			reset();
		}
	}

	public void openTile(int x, int z, int minesAround) {
		isOpened [x, z] = true;
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
		cells [x, z].gameObject.GetComponent<MeshRenderer>().materials[0].SetTexture("_MainTex", textures[5]);
		finished = true;
		smileImage.texture = textures [2];
	}

	public void reset() {
		timerCounter.text = 0.ToString ();
		mineCountText.text = MINES.ToString ();
		DIM = NUM * NUM;
		isFlagged = new bool[NUM, NUM];
		isOpened = new bool[NUM, NUM];

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
