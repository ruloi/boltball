using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Bolt.AdvancedTutorial
{
	
	public class PlayerMotor : MonoBehaviour
	{

		public struct State
		{
			public Vector3 position;
			public Vector3 velocity;
			public bool isGrounded;
			public int jumpFrames;
		}

		Rigidbody2D rig;

		State _state;

		[SerializeField]
		float skinWidth = 0.08f;

		[SerializeField]
		float gravityForce = -9.81f;

		[SerializeField]
		float jumpForce = +40f;

		[SerializeField]
		int jumpTotalFrames = 30;

		[SerializeField]
		float movingSpeed = 0.5f;

		[SerializeField]
		float maxVelocity = 2f;

		[SerializeField]
		Vector3 drag = new Vector3 (1f, 0f, 1f);

		[SerializeField]
		LayerMask layerMask;

		Vector3 sphere {
			get {
				Vector3 p;

				p = transform.position;
				p.y -= (skinWidth * 2);

				return p;
			}
		}

		Vector3 waist {
			get {
				Vector3 p;

				p = transform.position;

				return p;
			}
		}

		public bool jumpStartedThisFrame {
			get {
				return _state.jumpFrames == (jumpTotalFrames - 1);
			}
		}

		void Awake ()
		{
			rig = GetComponent<Rigidbody2D>();

			_state = new State ();
			_state.position = transform.localPosition;
		}

		public void SetState (Vector3 position, Vector3 velocity, bool isGrounded, int jumpFrames)
		{
			// assign new state
			_state.position = position;
			_state.velocity = velocity;
			_state.jumpFrames = jumpFrames;
			_state.isGrounded = isGrounded;

			// assign local position
			transform.localPosition = _state.position;
			rig.velocity = velocity;
		}

		void Move (Vector3 velocity)
		{
			bool isGrounded = true;

			_state.velocity = velocity;
			rig.velocity = _state.velocity;

			_state.position = transform.localPosition;
		}

		public State Move (bool forward, bool backward, bool left, bool right, bool jump, float yaw)
		{
			_state.isGrounded = true;
			jump = false;

			var moving = false;
			var movingDir = Vector3.zero;

			if (forward ^ backward) {
				movingDir.y = forward ? +1 : -1;
			}

			if (left ^ right) {
				movingDir.x = right ? +1 : -1;
			}

			if (movingDir.x != 0 || movingDir.y != 0) {
				moving = true;
				movingDir = Vector3.Normalize (movingDir);
			}

			_state.velocity += movingDir * movingSpeed;

			// clamp velocity
			_state.velocity = Vector3.ClampMagnitude (_state.velocity, maxVelocity);

			// apply movement
			Move (_state.velocity);

			// update position
			_state.position = transform.localPosition;

			// done
			return _state;
		}

		float ApplyDrag (float value, float drag)
		{
			if (value < 0) {
				return Mathf.Min (value + (drag * BoltNetwork.frameDeltaTime), 0f);
			} else if (value > 0) {
				return Mathf.Max (value - (drag * BoltNetwork.frameDeltaTime), 0f);
			}

			return value;
		}

		void DetectTunneling ()
		{
			
		}

		void OnDrawGizmos ()
		{
			if (Application.isPlaying) {
				Gizmos.color = _state.isGrounded ? Color.green : Color.red;
				//Gizmos.DrawWireSphere (sphere, _cc.radius);

				Gizmos.color = Color.magenta;
				//Gizmos.DrawLine (waist, waist + new Vector3 (0, -(_cc.height / 2f), 0));
			}
		}
	}
}