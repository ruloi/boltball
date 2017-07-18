using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using Bolt.AdvancedTutorial;

public class PlayerMovement : Bolt.EntityBehaviour<ITutorialPlayerState> {

	public int deaths = 0;

	private float maxVelocity = 2.3f;
	private float minusOne = -1f;
	public float accelerationRate = 500.0f;

	public SpriteRenderer sp;
	public SpriteRenderer shootAreaSp;

	public Vector2 startLeft,startRight;

    public bool canMove = true;

    public bool canShoot = true;

    public bool wasShooting = false;

    bool isShooting = false;

	float shootDistance = 0.9f;
	float shootForce = 7.0f;

	PlayerMotor _motor;

	const float MOUSE_SENSITIVITY = 2f;


	bool _forward;
	bool _backward;
	bool _left;
	bool _right;
	bool _jump;

	float _yaw;
	float _pitch;

	void Awake() {
		_motor = GetComponent<PlayerMotor>();
	}

	public override void Attached() {
		state.SetTransforms(state.Transform, transform);
		state.Velocity = GetComponent<Rigidbody2D>().velocity;
	}

	public override void SimulateController() {
		PollKeys(false);

		ITutorialPlayerCommandInput input = TutorialPlayerCommand.Create();

		input.Forward = _forward;
		input.Backward = _backward;
		input.Left = _left;
		input.Right = _right;
		input.Jump = _jump;
		input.Yaw = _yaw;
		input.Pitch = _pitch;

		entity.QueueInput(input);


	}

	public override void ExecuteCommand(Bolt.Command command, bool resetState) {
		TutorialPlayerCommand cmd = (TutorialPlayerCommand)command;

		if (resetState) {
			// we got a correction from the server, reset (this only runs on the client)
			Debug.Log("Correct");
			_motor.SetState(cmd.Result.Position, cmd.Result.Velocity, cmd.Result.IsGrounded, cmd.Result.JumpFrames);
		}
		else {
			// apply movement (this runs on both server and client)
			PlayerMotor.State motorState = _motor.Move(cmd.Input.Forward, cmd.Input.Backward, cmd.Input.Left, cmd.Input.Right, cmd.Input.Jump, cmd.Input.Yaw);

			// copy the motor state to the commands result (this gets sent back to the client)
			cmd.Result.Position = motorState.position;
			cmd.Result.Velocity = motorState.velocity;
			cmd.Result.IsGrounded = motorState.isGrounded;
			cmd.Result.JumpFrames = motorState.jumpFrames;

			if (cmd.Input.Jump)
			{
				if (!wasShooting)
				{
					wasShooting = true;
					//photonView.RPC("Shoot",PhotonTargets.All,null);
					Shoot();
				}
				ColorShooting(true);
			}
			else
			{
				wasShooting = false;
				ColorShooting(false);
			}
		}
	}

	void PollKeys(bool mouse) {
		_forward = Input.GetKey(KeyCode.W);
		_backward = Input.GetKey(KeyCode.S);
		_left = Input.GetKey(KeyCode.A);
		_right = Input.GetKey(KeyCode.D);
		_jump = Input.GetKeyDown(KeyCode.Space);
		/*
		if (mouse) {
			_yaw += (Input.GetAxisRaw("Mouse X") * MOUSE_SENSITIVITY);
			_yaw %= 360f;

			_pitch += (-Input.GetAxisRaw("Mouse Y") * MOUSE_SENSITIVITY);
			_pitch = Mathf.Clamp(_pitch, -85f, +85f);
		}
		*/

	}

	public void ProcessInput() {
		/*
		if( photonView.isMine == false && PhotonNetwork.connected == true )
		{
			return;
		}
		*/

		if (!canMove)
			return;

		float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
		float vertical = CrossPlatformInputManager.GetAxis("Vertical");
		bool shooting = false;


		if(Input.touchCount > 1){
			shooting = true;
		}else{
			shooting = false;
		}


		if(Input.GetKey(KeyCode.Space)){
			shooting = true;
		}else{
			shooting = false;
		}

		Rigidbody2D rig = GetComponent<Rigidbody2D>();

		Vector2 joystickVector = new Vector2(horizontal,vertical);
		joystickVector.Normalize();


		vertical = joystickVector.y * accelerationRate * Time.deltaTime;
		horizontal = joystickVector.x * accelerationRate * Time.deltaTime;

		rig.AddForce(new Vector2(horizontal,vertical));

		Vector2 newVelocity = new Vector2();
		newVelocity = rig.velocity;

		if(rig.velocity.x > maxVelocity){
			newVelocity.x = maxVelocity;
		}

		if(rig.velocity.y > maxVelocity){
			newVelocity.y = maxVelocity;
		}

		if(rig.velocity.x < maxVelocity * minusOne){
			newVelocity.x = maxVelocity * minusOne;
		}

		if(rig.velocity.y < maxVelocity * minusOne){
			newVelocity.y = maxVelocity * minusOne;
		}

		rig.velocity = newVelocity;


		// if (canShoot == true)
		// {

		//}

	}

	public void Update () {
		PollKeys(true);

	}

	public void ColorShooting(bool active = true){
		Color c;
		if(active)
			c = Color.white;
		else
			c = Color.black;
		
		c.a = 0.5f;
		shootAreaSp.color = c;

	}

	/*
	public override void OnSyncedUpdate () {
		FP vertical = TrueSyncInput.GetFP (0);
		FP horizontal = TrueSyncInput.GetFP (1);
		//Debug.Log(vertical + " # " + horizontal);
		vertical *= 1125000 * TrueSyncManager.DeltaTime;
		horizontal *= 1125000 * TrueSyncManager.DeltaTime;



		tsRigidBody2D.velocity = new Vector2(horizontal*maxVelocity,vertical*maxVelocity);

	}
	*/

	public void SetColor(){
		if(IsLeftSide()){
			sp.color = Color.blue;
		}else{
			sp.color = Color.red;
		}
		/*
		if(this.localOwner.Id == this.owner.Id){
			sp.color = Color.yellow;
		}
		*/
	}

	public bool IsLeftSide(){



		return false;
		//Debug.Log("IDS: " + localOwner.Id + " # " + owner.Id + " # " + PhotonNetwork.masterClient.ID);
		//return (owner.Id == PhotonNetwork.masterClient.ID);
		//	return true;
		//Debug.Log("LEFT:  " +  PhotonNetwork.isMasterClient);
		//return PhotonNetwork.isMasterClient;
	}

	public void Start () {
		
		//StartCoroutine(CanMoveRoutine());
		//GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		SetColor();

		if(IsLeftSide()){
			transform.position = startLeft;
		}else{
			transform.position = startRight;
		}

        //StateTracker.AddTracking(this);

    }

	public void Respawn() {
		Debug.Log("RESPAWN");
		//tsRigidBody2D.velocity = Vector2.zero;
		canMove = false;
        //canShoot = false;

		StartCoroutine(CanMoveRoutine());
		//tsRigidBody2D.velocity = Vector2.zero;

		/*if(IsLeftSide()){
			tsTransform2D.position = startLeft;
		}else{
			tsTransform2D.position = startRight;
		}*/

		
	}

    IEnumerator CanMoveRoutine()
    {
		Rigidbody2D rig = GetComponent<Rigidbody2D>();

        yield return 1.0f;
        Debug.Log("MOVE FALSE");
        canMove = false;
        Debug.Log("VELOCIDAD 0");
		rig.velocity = Vector2.zero;
        yield return 3.0f;
        Debug.Log("NUEVAS POSI");
        if (IsLeftSide())
        {
			transform.position = startLeft;
        }
        else
        {
			transform.position = startRight;
        }
        yield return 1.0f;
        Debug.Log("MOVER TRUE");
        canMove = true;
    }

    /* IEnumerator CanMoveRoutine(){
		canMove = false;
        canShoot = false;
		yield return (FP)3.0f;
		canMove = true;
        canShoot = true;
	}*/

    void OnGUI() {
		//GUI.Label (new Rect(10, 100 + 30 * owner.Id, 300, 30), "player: " + owner.Id + ", deaths: " + deaths);
	}

	//[PunRPC]
	public void Shoot(){
		
		//Debug.Log("SHOOTERINO");
		BallMovement bc = FindObjectOfType<BallMovement>();
		//Debug.Log("Distance: " + Vector2.Distance(tsTransform2D.position,bc.tsTransform2D.position));
		if(shootDistance > Vector2.Distance(transform.position,bc.transform.position)){
			Vector2 shootDir = bc.transform.position - transform.position;
			shootDir.Normalize();

			//bc.photonView.RPC("Shoot",PhotonTargets.AllViaServer,shootDir,shootForce);

			bc.GetComponent<Rigidbody2D>().AddForce(shootDir * shootForce,ForceMode2D.Impulse);
		}
	}



	public void OnSyncedTriggerEnter(Collision2D other) {
		Debug.Log("TRIGGER ENTER");
	}

	public void ShootPressed(){
		isShooting = true;
	}



}