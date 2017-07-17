using UnityEngine;
using System.Collections;

public class BallMovement :  Bolt.EntityBehaviour<IBallState>
{
	private float maxVelocity = 30.0f;
	private float minusOne = -1f;

	public Vector2 startPos;

	public override void Attached()
	{
		state.SetTransforms(state.Transform, transform);
	}


    // Update is called once per frame
    public void FixedUpdate () {
		Vector2 newVelocity = new Vector2();
		Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
		newVelocity = GetComponent<Rigidbody2D>().velocity;

		if(rigidbody.velocity.x > maxVelocity){
			newVelocity.x = maxVelocity;
		}

		if(rigidbody.velocity.y > maxVelocity){
			newVelocity.y = maxVelocity;
		}

		if(rigidbody.velocity.x < maxVelocity * minusOne){
			newVelocity.x = maxVelocity * minusOne;
		}

		if(rigidbody.velocity.y < maxVelocity * minusOne){
			newVelocity.y = maxVelocity * minusOne;
		}

		rigidbody.velocity = newVelocity;
	}

	public void OnSyncedTriggerEnter(Collision2D other) {
		if (other.gameObject.tag == "GoalLeft") {
			//GameManager.AddScoreLeft();
		}
		if (other.gameObject.tag == "GoalRight") {
			//GameManager.AddScoreRight();
		}
	}

	public void Respawn(){

      //  TrueSyncManager.SyncedStartCoroutine(ResetBall());
       
        
		
	}


	public void Shoot(Vector2 dir, float force){
		GetComponent<Rigidbody2D>().AddForce(dir * force,ForceMode2D.Impulse);
	}

    IEnumerator ResetBall()
    {
		Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        yield return 1.0f;
		rigidbody.velocity = Vector2.zero;
        
        yield return 3.0f;
		rigidbody.position = startPos;
    }

}

