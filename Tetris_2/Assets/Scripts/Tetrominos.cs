using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrominos : MonoBehaviour {

	float fall = 0;
	public float fallSpeed = 1;
	public bool allowRotation = true;
	public bool limitRotation = false;
	public int individualScore = 100;
	private float individualScoreTime;
	private float continuousVerticalSpeed = 0.05f; //The speed at which tetromino will move when the down button is held down
	private float continuousHorizontalSpeed = 0.1f; //the speed at which tetromino will move when the left or right buttons are held down
	private float buttonDownWaitMax = 0.2f; //how long to waif before the tetromino recognizes that a button is begin held down
	private float verticalTimer = 0;
	private float horizontalTimer = 0;
	private float buttonDownWaitTimer = 0;
	private bool movedImmediateHorizontal = false;
	private bool movedImmediateVertical = false;
	// Use this for initialization
	void Start () {
		
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

	void CheckUserInput(){
		if(Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.DownArrow)){
			horizontalTimer = 0;
			verticalTimer = 0;
			buttonDownWaitTimer = 0;
			movedImmediateHorizontal = false;
			movedImmediateVertical = false;
		}

		if(Input.GetKey(KeyCode.RightArrow)){
			if(movedImmediateHorizontal){
				if(buttonDownWaitTimer < buttonDownWaitMax){
					buttonDownWaitTimer += Time.deltaTime;
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
			if(CheckIsValidPosition()){
				FindObjectOfType<Game>().UpdateGrid(this);
			}else{
				transform.position += new Vector3(-1,0,0);
			}

		}else if(Input.GetKey(KeyCode.LeftArrow)){
			if(movedImmediateHorizontal){
				if(buttonDownWaitTimer < buttonDownWaitMax){
					buttonDownWaitTimer += Time.deltaTime;
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
			}else{
				transform.position += new Vector3(1,0,0);
			}

		}else if(Input.GetKeyDown(KeyCode.UpArrow)){
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
			
			

		}else if(Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed){
			if(movedImmediateVertical){
				if(buttonDownWaitTimer < buttonDownWaitMax){
					buttonDownWaitTimer += Time.deltaTime;
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
			}else{
				transform.position += new Vector3(0,1,0);
				FindObjectOfType<Game>().DeleteRow();

				if(FindObjectOfType<Game>().CheckAboveGrid(this)){
					FindObjectOfType<Game>().GameOver();
				}
				FindObjectOfType<Game>().SpawnNextTetromino();
				Game.currentScore += individualScore;
				enabled = false;
			}
			fall = Time.time;
		}
	}

	bool CheckIsValidPosition(){
		foreach(Transform mino in transform){
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
