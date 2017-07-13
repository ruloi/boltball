using UnityEngine;
using System.Collections.Generic;

public class TutorialPlayerObject {
	public BoltEntity character;
	public BoltConnection connection;

	public bool isServer {
		get { return connection == null; }
	}

	public bool isClient {
		get { return connection != null; }
	}

	public void Spawn() {
		if (!character) {
			character = BoltNetwork.Instantiate(BoltPrefabs.PlayerHax);

			if (isServer) {
				character.TakeControl();
			} else {
				character.AssignControl(connection);
			}
		}

		// teleport entity to a random spawn position
		character.transform.position = RandomPosition();
	}

	Vector3 RandomPosition()
	{
		float x = Random.Range(-3f, +3f);
		float y = Random.Range(-3f, +3f);
		return new Vector3(x, y, 0);
	}
}