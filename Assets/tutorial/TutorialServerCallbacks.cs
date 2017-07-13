using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "Game")]
public class TutorialServerCallbacks : Bolt.GlobalEventListener {
	void Awake() {
		TutorialPlayerObjectRegistry.CreateServerPlayer();
	}

	public override void Connected(BoltConnection arg) {
		TutorialPlayerObjectRegistry.CreateClientPlayer(arg);
	}

	public override void SceneLoadLocalDone(string map) {
		TutorialPlayerObjectRegistry.serverPlayer.Spawn();
	}

	public override void SceneLoadRemoteDone(BoltConnection connection) {
		TutorialPlayerObjectRegistry.GetTutorialPlayer(connection).Spawn();
	}
}