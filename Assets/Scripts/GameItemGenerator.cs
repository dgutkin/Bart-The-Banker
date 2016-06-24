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
		for(int i = 0; i < 20; ++i) {
			int randomX = Random.Range (50, 100);
			int randomY = Random.Range (-43, 0);

			if (randomY > -40 && randomY < -10) {
				Vector2 billPosition = originPosition + new Vector2 (randomX/10, randomY/10);
				Instantiate (bill, billPosition, Quaternion.identity);
				continue;
			}

			Vector2 randomPosition = originPosition + new Vector2 (randomX/10, randomY/10);
			Debug.Log (randomY);

			Instantiate (gavel, randomPosition, Quaternion.identity);

			originPosition.x = randomPosition.x;

		}
	}
		
}
