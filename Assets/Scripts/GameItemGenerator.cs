using UnityEngine;
using System.Collections;

public class GameItemGenerator : MonoBehaviour {

	public GameObject gavel;
	public GameObject bill;

	private Vector2 originPosition;

	// Use this for initialization
	void Start () {
		originPosition = transform.position;
		SpawnObjects ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	void SpawnObjects() {
		//Gavel Position
		for (int i = 0; i < 20; i++) {
			int randomX = Random.Range (40, 70);
			int randomY = Random.Range (1, 3);

			if (randomY == 1) {
				randomY = 15;
			} else {
				randomY = 30;
			}

			Vector2 randomPosition = originPosition + new Vector2 (randomX/10, randomY/10);
			Debug.Log (randomY);

			Instantiate (gavel, randomPosition, Quaternion.identity);

			originPosition.x = randomPosition.x;

		}

		/*for (int i = 0; i < billSpawnPoints.Length; i++) {
			int randomX = Random.Range (2, 5);
			int randomY = Random.Range (2, 5);
			Instantiate (gavel, billSpawnPoints [i].position, Quaternion.identity);
		}*/
	}
}
