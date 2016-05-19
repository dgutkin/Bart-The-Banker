using UnityEngine;
using System.Collections;

public class GavelGenerator : MonoBehaviour {

	public Transform[] gavelSpawnPoints;
	public GameObject gavel;

	// Use this for initialization
	void Start () {
		GenerateGavelPositions ();
		SpawnGavels ();
	}

	void GenerateGavelPositions() {
		
	}
	
	void SpawnGavels() {

		for (int i = 0; i < gavelSpawnPoints.Length; i++) {
			Instantiate (gavel, gavelSpawnPoints [i].position, Quaternion.identity);
		}

	}

}
