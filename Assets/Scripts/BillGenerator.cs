using UnityEngine;
using System.Collections;

public class BillGenerator : MonoBehaviour {

	public Transform[] billSpawnPoints;
	public GameObject bill;

	// Use this for initialization
	void Start () {
		
		SpawnBills ();

	}
	
	void SpawnBills() {

		for (int i = 0; i < billSpawnPoints.Length; i++) {
			
			Instantiate (bill, billSpawnPoints [i].position, Quaternion.identity);

		}

	}

}
