using UnityEngine;

[BoltGlobalBehaviour("Game")]
public class TutorialPlayerCallbacks : Bolt.GlobalEventListener {
	public override void SceneLoadLocalDone(string map) {
		// this just instantiates our player camera, 
		// the Instantiate() method is supplied by the BoltSingletonPrefab<T> class
		//PlayerCamera.Instantiate();
	}

	public override void ControlOfEntityGained(BoltEntity arg) {
		// this tells the player camera to look at the entity we are controlling
		//PlayerCamera.instance.SetTarget(arg);
	}
}