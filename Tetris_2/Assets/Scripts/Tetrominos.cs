using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrominos : MonoBehaviour {

	float fall = 0;
	private float fallSpeed;
	public bool allowRotation = true; //specify if we want to allow the tetromino to rotate
	public bool limitRotation = false; //used to limit the rotation of the tetromino to a 90 / -90 rotation 
	public int individualScore = 100;
	private float individualScoreTime;
	private float continuousVerticalSpeed = 0.05f; //The speed at which tetromino will move when the down button is held down
	private float continuousHorizontalSpeed = 0.1f; //the speed at which tetromino will move when the left or right buttons are held down
	private float buttonDownWaitMax = 0.2f; //how long to waif before the tetromino recognizes that a button is begin held down
	private float verticalTimer = 0;
	private float horizontalTimer = 0;
	private float buttonDownWaitTimerHorizontal = 0;
	private float buttonDownWaitTimerVertical = 0;
	private bool movedImmediateHorizontal = false;
	private bool movedImmediateVertical = false;

	public AudioClip moveSound; //sound for when tetrominos is moved
	public AudioClip rotateSound; //sound for when tetrominos is rotated
	public AudioClip landSound; //sound for when tetrominos land
	private AudioSource audioSource;
	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		fallSpeed = GameObject.Find("BG Canvas").GetComponent<Game>().fallSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		CheckUserInput();
		UpdateIndividualScore();
	}

	void UpdateIndividualScore(){
		if(individualScoreTime < 1){
			individualScoreTime += Time.deltaTime;
		}else{
			individualScoreTime = 0;
			individualScore = Mathf.Max(individualScore - 10,0); //impedir o valor de ser menor de 0
		}
	}
	//checks the user input
	void CheckUserInput(){
		/* this method checks the key that the player can press to manipulate the position of the tetromino
		 */
		if(Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow)){
			movedImmediateHorizontal = false;
			horizontalTimer = 0;
			buttonDownWaitTimerHorizontal = 0;
		}
		
		if(Input.GetKeyUp(KeyCode.DownArrow)){
			movedImmediateVertical = false;
			verticalTimer = 0;
			buttonDownWaitTimerVertical = 0;
		}

		if(Input.GetKey(KeyCode.RightArrow)){
			MoveRight();

		}
		if(Input.GetKey(KeyCode.LeftArrow)){
			MoveLeft();

		}
		if(Input.GetKeyDown(KeyCode.UpArrow)){
			Rotate();

		}
		if(Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed){
			MoveDown();
		}
	}
	void MoveLeft(){
		if(movedImmediateHorizontal){
			if(buttonDownWaitTimerHorizontal < buttonDownWaitMax){
				buttonDownWaitTimerHorizontal += Time.deltaTime;
				return;
			}

			if(horizontalTimer < continuousHorizontalSpeed){
				horizontalTimer += Time.deltaTime;
				return;
			}
		}
		if(!movedImmediateHorizontal){
			movedImmediateHorizontal = true;
		}
		horizontalTimer = 0;
		transform.position += new Vector3(-1,0,0);
		if(CheckIsValidPosition()){
			FindObjectOfType<Game>().UpdateGrid(this);
			PlayMoveAudio();
		}else{
			transform.position += new Vector3(1,0,0);
		}
		//print(transform.position);
	}

	void MoveRight(){
		if(movedImmediateHorizontal){
			if(buttonDownWaitTimerHorizontal < buttonDownWaitMax){
				buttonDownWaitTimerHorizontal += Time.deltaTime;
				return;
			}

			if(horizontalTimer < continuousHorizontalSpeed){
				horizontalTimer += Time.deltaTime;
				return;
			}
		}
		if(!movedImmediateHorizontal){
			movedImmediateHorizontal = true;
		}

		horizontalTimer = 0;
		transform.position += new Vector3(1,0,0);
		//check if the tetromino is at a valid position
		if(CheckIsValidPosition()){
			//if it is, call the UpdateGrid method which records this tetrominos new position
			FindObjectOfType<Game>().UpdateGrid(this);
			PlayMoveAudio();
		}else{
			transform.position += new Vector3(-1,0,0);
		}
		//print(transform.position);
	}

	void MoveDown(){
		if(movedImmediateVertical){
			if(buttonDownWaitTimerVertical < buttonDownWaitMax){
				buttonDownWaitTimerVertical += Time.deltaTime;
				return;
			}

			if(verticalTimer < continuousVerticalSpeed){
				verticalTimer += Time.deltaTime;
				return;
			}
		}
		if(!movedImmediateVertical){
			movedImmediateVertical = true;
		}
		verticalTimer = 0;
		transform.position += new Vector3(0,-1,0);
		if(CheckIsValidPosition()){
			FindObjectOfType<Game>().UpdateGrid(this);
			/*if(Input.GetKeyDown(KeyCode.DownArrow)){
				PlayMoveAudio();
			}*/
		}else{
			transform.position += new Vector3(0,1,0);
			FindObjectOfType<Game>().DeleteRow();
			//check if there any minos above the grid
			if(FindObjectOfType<Game>().CheckAboveGrid(this)){
				FindObjectOfType<Game>().GameOver();
			}
			//spawn the next piece
			PlayLandAudio();
			FindObjectOfType<Game>().SpawnNextTetromino();
			Game.currentScore += individualScore;
			enabled = false;
		}
		fall = Time.time;
	}
	
	void Rotate(){
		if(allowRotation){
				if(limitRotation){
					if(transform.rotation.eulerAngles.z >= 90){
						transform.Rotate(0,0,-90);
					}else{
						transform.Rotate(0,0,90);
					}
				}else{
					transform.Rotate(0,0,90);
				}
				if(CheckIsValidPosition()){
					FindObjectOfType<Game>().UpdateGrid(this);
					PlayRotateAudio();
				}else{
					if(limitRotation){
						if(transform.rotation.eulerAngles.z >= 90){
							transform.Rotate(0,0,-90);
						}else{
							transform.Rotate(0,0,90);
						}
					}else{
						transform.Rotate(0,0,-90);
					}
				}
			}
	}
	//plays audio clip when tetrominos is moved left, right or down
	void PlayMoveAudio(){
		audioSource.PlayOneShot(moveSound);
	}
	//plays audio clip when tetrominos is rotated
	void PlayRotateAudio(){
		audioSource.PlayOneShot(rotateSound);
	}
	//plays audio clip when tetrominos landed
	void PlayLandAudio(){
		audioSource.PlayOneShot(landSound);
	}

	bool CheckIsValidPosition(){
		foreach(Transform mino in transform){
			print(mino.position);
			Vector2 pos = FindObjectOfType<Game>().Round(mino.position);
			if(FindObjectOfType<Game>().CheckInsideGrid(pos) == false){
				print(pos);
				return false;
			}
			if(FindObjectOfType<Game>().GetTransformGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformGridPosition(pos).parent != transform){
				return false;
			}
		}
		return true;
	}
}
