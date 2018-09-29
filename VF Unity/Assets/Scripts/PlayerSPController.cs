using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum StepStates {
	standBy,
	calibrating,
	noStep,
	step,
	maxStep, 
	stop
}

public class PlayerSPController : MonoBehaviour {
	public SharpConect spReader;

    [SerializeField]
    StepStates []statesArray = new StepStates[10];

    [Header("Configuration")]
    [SerializeField]
    int arrayStatesSize = 10;

	public int sensorCount = 1;//TODO: Funcionalidad multi sensor real!!
	public int maxOffset = 10, minOffset = 5;
	public bool autoCalibrate = true;
	public float calibrateTime = 2 ;
	public float ignoreCalibrationTime= 0.5f ;
	[Range(0f,1f)]
	public float sensitivity;
	[Range(0f,2f)]
	public float gravityFactor;
	[Range(0.2f,0.6f)]
	public float stepVelocitiReferenceValue;
	[Range(0.4f,1f)]
	public float maxStateTimeReferenceValue;

	[Header("Debug Info:")]
	public float noStepDeltaTime;
	public float stepDeltaTime;
	public float maxStepDeltaTime;
	public int maxWeightFootValue;
	public int minWeightFootValue ;
	public int currentMagnitude;
	[Range(0f,1f)]
	public float currentSpeed;

	public static PlayerSPController instance;

	//Walk Speed
	Animator anim;
	static float _walkSpeed;
	public static float walkSpeed {
		get {
			return _walkSpeed;
		}
		set {
			//Suavizado de varianza de la velocidad
			//_walkSpeed = Mathf.Clamp(value, _walkSpeed / 2f, _walkSpeed * 2f + 0.1f);
			_walkSpeed = value;
		}
				
	}

	//Current State
	StepStates _currentState;
	StepStates currentState {
		get { 
			return _currentState;
		}
		set {
			//Reset times 
			WalkSpeedControl (_currentState, value);
			_currentState = value;

		}
	}
		
	void Awake () {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();

		currentState = StepStates.standBy;
		maxWeightFootValue = maxOffset;
		minWeightFootValue = minOffset;

        InitializeArray();
	}

	// Update is called once per frame
	void FixedUpdate () {

		//Magic
		WalkSystem ();

		//Gravity and Clamp
		walkSpeed -= Time.fixedDeltaTime * gravityFactor; 
		walkSpeed = Mathf.Clamp (walkSpeed, 0, 1);

		//Time states control
		IncreaseCurrentDeltaTime ();

        //Ad Current State to array
        AddToArrayStates();

        //TODO: Parche animator.
        if (anim != null)
			anim.SetFloat ("Speed", walkSpeed);

		//TODO: PArche FPS!
		UnityStandardAssets.Characters.FirstPerson.FirstPersonController fps = GetComponent <UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
		if (fps != null)
			fps.vertical = walkSpeed;

	}

    void InitializeArray ()
    {
        for (int i = 0; i < arrayStatesSize; i++)
        {
            statesArray[i] = StepStates.standBy;
        }

    }

    void AddToArrayStates ()
    {
        for (int i = 0; i < arrayStatesSize-1; i++)
        {
            statesArray[i+1] = statesArray[i];             
        }
        statesArray[0] = currentState;
    }

    int GetContigousState()
    {
        int count = 0;

        return count;
    }

    //Delta time states increase (At Update)
    void IncreaseCurrentDeltaTime (){
		//Max Step Time
		if (currentState == StepStates.maxStep) {
			maxStepDeltaTime = Mathf.Clamp (Time.fixedDeltaTime + maxStepDeltaTime, 0, maxStateTimeReferenceValue);
			return;
		}

		//Step Time
		if (currentState == StepStates.step) {
			stepDeltaTime = Mathf.Clamp (Time.fixedDeltaTime + stepDeltaTime, 0, maxStateTimeReferenceValue);
			return;
		}

		//No Step Time
		if (currentState == StepStates.noStep) {
			noStepDeltaTime = Mathf.Clamp (Time.fixedDeltaTime + noStepDeltaTime, 0, maxStateTimeReferenceValue);
			return;
		}
	}

	//*** WALK SPEEED CONTROL ***
	void WalkSpeedControl(StepStates previousState, StepStates currentState) {

		//No Step
		if (previousState == StepStates.noStep && currentState != StepStates.noStep) {
			walkSpeed += (stepVelocitiReferenceValue - noStepDeltaTime) * instance.sensitivity;

			noStepDeltaTime = 0;
			return;
		}

		//MaxStep
		if (previousState == StepStates.maxStep && currentState != StepStates.maxStep) {
			walkSpeed += (stepVelocitiReferenceValue - maxStepDeltaTime) * instance.sensitivity;

			maxStepDeltaTime = 0;
			return;
		}

		//Step
		if (previousState == StepStates.step && currentState != StepStates.step) {
			walkSpeed += (stepVelocitiReferenceValue - stepDeltaTime) * instance.sensitivity;
			stepDeltaTime = 0;
			return;
		}

		//Jump
		if (previousState == StepStates.maxStep && currentState == StepStates.noStep) {
			UnityStandardAssets.Characters.FirstPerson.FirstPersonController fps = GetComponent <UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
			if (fps != null)
				fps.m_Jump = true;
		}

//		//STOP
//		if (currentState == stepStates.stop) {
//			walkSpeed -= Time.fixedDeltaTime * gravityFactor; 
//			return;
//		}

	}

	float CurrentDeltaTime() {
		if (currentState == StepStates.noStep) 			
			return noStepDeltaTime;

		if (currentState == StepStates.maxStep) 			
			return maxStepDeltaTime;
		
		if (currentState == StepStates.step) 			
			return stepDeltaTime;

		return 0;
	}


	int WalkSystem () {
		int[] magnitudes = new int[sensorCount];

		//Getting magnitudes from stream reader dividing one matrix data at the vertical mid in 'sensorCount' magnitudes
		magnitudes[0] = spReader.GetValue();

		// ** DEBUGS **
		print ("Current State: " + currentState.ToString ());
		//Inspector Debugs
		currentMagnitude = magnitudes[0];
		currentSpeed = walkSpeed;

		//AutoOn STAND BY
		if (currentState == StepStates.standBy ) {
			if (magnitudes [sensorCount-1] > 0) {
				if (autoCalibrate)
					currentState = StepStates.calibrating;
				else
					currentState = StepStates.step;
			}
			else
				return 0;
		}

		//Auto Calibrating
		//TODO: guardar calibrado y que solo lo haga la 1ª vez
		if (currentState == StepStates.calibrating) {

			//We ignore first values
			if (ignoreCalibrationTime <= 0) {		
				
				//Calibrado de mínimos
				if (magnitudes [sensorCount-1] < minWeightFootValue) {
					minWeightFootValue = magnitudes [sensorCount-1];
				}

				//Calibrado de máximos
				if (magnitudes [sensorCount-1] > maxWeightFootValue) {
					maxWeightFootValue = magnitudes [sensorCount-1];
				}
						
				calibrateTime -= Time.fixedDeltaTime;
			} else {
				ignoreCalibrationTime -= Time.fixedDeltaTime;
			}


			if (calibrateTime <= 0) {
				currentState = StepStates.step;
				//event!!!
			} else {
				return 0;
			}
		}

		//Step
		if(currentState == StepStates.step) {
			//To max step
			if (magnitudes [sensorCount-1] < minWeightFootValue + minOffset){
				currentState = StepStates.noStep;
				//event!!!
				return 0;
			}
			//To no step
			if(magnitudes [sensorCount-1] > maxWeightFootValue - maxOffset) {
				currentState = StepStates.maxStep;
				//event!!!
				return 0;
			}
		}

		//No Step
		if(currentState == StepStates.noStep) {
			//To step
			if (magnitudes [sensorCount-1] > minWeightFootValue + minOffset){
				currentState = StepStates.step;
				return 0;
			}
		}

		//Max Step
		if(currentState == StepStates.maxStep) {
			//To step
			if (magnitudes [sensorCount-1] < maxWeightFootValue - maxOffset){
				currentState = StepStates.step;
				//event!!!
				return 0;
			}
			//To no step
			if (magnitudes [sensorCount-1] < minWeightFootValue + minOffset){
				currentState = StepStates.noStep;
				//event!!!
				return 0;
			}
		}

		if (currentState == StepStates.stop) {
			if(magnitudes [sensorCount-1] > maxWeightFootValue - maxOffset) {
				WalkSpeedControl (currentState, currentState);
				currentState = StepStates.maxStep;
				return 0;
			}
			return 0;
		}

		//Long Step  = slown down = stop
		if (noStepDeltaTime == maxStateTimeReferenceValue || maxStepDeltaTime == maxStateTimeReferenceValue || stepDeltaTime == maxStateTimeReferenceValue ) {
			//currentState = stepStates.stop;	
			//event!!
			return 0;
		} 

		//Jump
//		if (ground && magnitudes [0] < minWeightFootValue && magnitudes [sensorCount-1] < minWeightFootValue ) {
//			ground = false;
//			anim.SetTrigger ("Jump");
//			return true;
//		}

	/*	//Landing
		if (!ground && magnitudes [0] > 50 && magnitudes [sensorCount-1] > 50) {
			ground = true;
		}


		//RigthDown
		if (magnitudes [sensorCount-1] > maxWeightFootValue){
			anim.SetBool ("RigthUp", false);
			//return true;
		}

		//RigthUp
		if (magnitudes [sensorCount-1] < minWeightFootValue){
			anim.SetBool ("RigthUp", true);
			//return true;
		}

	

		//LeftUp
		if (magnitudes [0] < minWeightFootValue) {
			anim.SetBool ("LeftUp", true);
			//return true;
		}

		//LeftDown
		if (magnitudes [0] > maxWeightFootValue){
			anim.SetBool ("LeftUp", false);
			//return true;
		}



		*/
		return 0;
	}


}
