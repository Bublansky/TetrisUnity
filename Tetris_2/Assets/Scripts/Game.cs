using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour {

	public static int gridWidth = 10;
	public static int gridHeight = 22;
	public GameObject[] Tetrominos;
	public static Transform[,] grid = new Transform[gridWidth,gridHeight];

	public int scoreOneLine = 40;
    public int scoreTwoLine = 100;
    public int scoreThreeLine = 300;
    public int scoreFourLine = 1200;

	public int currentLevel = 0;
	private int numLinesCleared = 0;

	public float fallSpeed = 1.0f;

    private static int numberOfRowsThisTurn = 0;
    public static int currentScore = 0;
	
	private bool gameStarted = false;
	
    public Text hud_score;
	public Text hud_level;
	public Text hud_lines;

	private AudioSource audioSource;
	public AudioClip clearedLineSound;

	private GameObject previewTetromino;
	private GameObject nextTetromino;
	private Vector3 previewTetrominoPosition = new Vector3(11f,16f,-6.5f);
	// Use this for initialization
	void Start () {
		SpawnNextTetromino();
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateScore();
        UpdateUI();
		UpdateLevel();
		UpdateSpeed();
	}
	
	void UpdateLevel(){
		currentLevel = numLinesCleared / 10;
	}

	void UpdateSpeed(){
		fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
	}

	public void UpdateUI(){
        hud_score.text = currentScore.ToString();
		hud_level.text = currentLevel.ToString();
		hud_lines.text = numLinesCleared.ToString();
    }

	public bool CheckAboveGrid(Tetrominos tetromino){
		for(int x = 0; x < gridWidth; ++x){
			foreach(Transform mino in transform){
				Vector2 pos = Round(mino.position);
				if(pos.y > gridHeight - 1){
					return true;
				}
			}
		}
		return false;
	}

	public bool IsFullRow(int y){
		for(int x = 0; x < gridWidth; ++x){
			if(grid[x,y] == null){
				return false;
			}
		}
		numberOfRowsThisTurn++;
		return true;
	}

	public void DeleteMino(int y){
		for(int x = 0; x < gridWidth; ++x){
			Destroy(grid[x,y].gameObject);
			grid[x,y] = null;
		}
	}

	public void MoveRowDown(int y){
		for(int x = 0; x < gridWidth; ++x){
			if(grid[x,y] != null){
				grid[x,y-1] = grid[x,y];
				grid[x,y] = null;
				grid[x,y-1].position += new Vector3(0,-1,0);
			}
		}
	}

	public void MoveAllRowsDown(int y){
		for(int i = y; i < gridHeight; i++){
			MoveRowDown(i);
		}
	}

	public void DeleteRow(){
		for(int y = 0; y < gridHeight; y++){
			if(IsFullRow(y)){
				DeleteMino(y);
				MoveAllRowsDown(y+1);
				--y;
			}
		}
	}

	public void UpdateGrid(Tetrominos tetromino){
		for(int y = 0; y < gridHeight; ++y){
			for(int x = 0; x < gridWidth; ++x){
				if(grid[x,y] != null){
					if(grid[x,y].parent == tetromino.transform){
						grid[x,y] = null;
					}
				}
			}
		}
		foreach(Transform mino in tetromino.transform){
			Vector2 pos = Round(mino.position);

			if(pos.y < gridHeight){
				grid[(int)pos.x,(int)pos.y] = mino;
			}
		}
	}

	public Transform GetTransformGridPosition(Vector2 pos){
		if(pos.y > gridHeight - 1){
			return null;
		}else{
			return grid[(int)pos.x,(int)pos.y];
		}
	}
	
	public void SpawnNextTetromino(){
		if(!gameStarted){
			gameStarted = true;
			/* next tetromino */
			GameObject element = Tetrominos[Random.Range(0,Tetrominos.Length)];
			Vector3 spawnPos = new Vector3(5.0f,20f,-2f);
			Quaternion spawnRot = Quaternion.identity;
			nextTetromino = (GameObject)Instantiate(element,spawnPos,spawnRot);
			/* preview tetromino */
			GameObject elementP = Tetrominos[Random.Range(0,Tetrominos.Length)];
			Quaternion spawnRotP = Quaternion.identity;
			previewTetromino = (GameObject)Instantiate(elementP,previewTetrominoPosition,spawnRotP);

			previewTetromino.GetComponent<Tetrominos>().enabled = false;
		}else{
			previewTetromino.transform.localPosition = new Vector3(5.0f,20.0f,-2f);
			nextTetromino = previewTetromino;
			nextTetromino.GetComponent<Tetrominos>().enabled = true;

			GameObject element = Tetrominos[Random.Range(0,Tetrominos.Length)];
			Quaternion spawnRot = Quaternion.identity;
			previewTetromino = (GameObject)Instantiate(element,previewTetrominoPosition,spawnRot);
			previewTetromino.GetComponent<Tetrominos>().enabled = false;

		}
		
	}

	public bool CheckInsideGrid(Vector2 pos){
		Debug.Log("Pos.x: " + (int)pos.x);
		return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y > 0);
	}
	
	public Vector2 Round(Vector2 pos){
		return new Vector2(Mathf.Round(pos.x),Mathf.Round(pos.y));
	}

	public void GameOver(){
		SceneManager.LoadScene("GameOver");
	}

	public void UpdateScore(){
        if(numberOfRowsThisTurn > 0){
            if(numberOfRowsThisTurn == 1){
                ClearedOneLine();
            }else if(numberOfRowsThisTurn == 2){
                ClearedTwoLines();
            }else if(numberOfRowsThisTurn == 3){
                ClearedThreeLines();
            }else if(numberOfRowsThisTurn == 4){
                ClearedFourLines();
            }
            numberOfRowsThisTurn = 0;
			PlayLineClearSound();
        }
    }

    public void ClearedOneLine(){
        currentScore += scoreOneLine + (currentLevel * 20);
		numLinesCleared++;
    }

    public void ClearedTwoLines(){
        currentScore += scoreTwoLine + (currentLevel * 25);
		numLinesCleared += 2;
    }

    public void ClearedThreeLines(){
        currentScore += scoreThreeLine + (currentLevel * 30);;
		numLinesCleared += 3;
    }

    public void ClearedFourLines(){
        currentScore += scoreFourLine + (currentLevel * 35);;
		numLinesCleared += 4;
    }

	public void PlayLineClearSound(){
		audioSource.PlayOneShot(clearedLineSound);
	}
}
