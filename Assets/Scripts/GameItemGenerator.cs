using UnityEngine;
using System.Collections;

public class GameItemGenerator : MonoBehaviour {

	public GameObject gavel;
	public GameObject bill;
	public GameObject box;

	private Vector2 originPosition;

	// Use this for initialization
	void Start () {
		originPosition = transform.position;
		originPosition += new Vector2 (5f, -3.72f);
		SpawnObjects ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	void SpawnObjects() {
		//Origin is Vector2 (2, 0), Floor is (7, -4.92)
		int blocksGenerated = 30;
		int[] blockMap = new int[30];

		//Box Positions
		//Random first block
		int firstRandom = Random.Range(0, 100);
		float blockHeight = 1.2f;
		float blockWidth = 1.2f;
		Vector2 prevBlockPos = originPosition;
		int prevBlockType;

		float blockType1 = originPosition.y;
		float blockType2 = originPosition.y + 3 * blockHeight;
		float blockType3 = originPosition.y + 4 * blockHeight;

		if (firstRandom < 70) {
			//Floor block (1)
			prevBlockPos.y = blockType1;
			prevBlockType = 1;
		} else if (firstRandom < 75) {
			//Head block (2)
			prevBlockPos.y = blockType2;
			prevBlockType = 2;
		} else {
			//Jump block (3)
			prevBlockPos.y = blockType3;
			prevBlockType = 3;
		}
		Instantiate (box, prevBlockPos, Quaternion.identity);
		Debug.Log (prevBlockPos.ToString());

		System.Random random = new System.Random ();
		int blocksMade = 0;
		int noBlock = 0;

		for(int i = 0; i < blocksGenerated; ++i) {
			int noBlockP = Random.Range (0, 100);

			//No Block
			if (noBlockP < 20 - noBlock * 5) {
				noBlock++;
				blockMap [i] = 0;
				continue;
			}

			//Make a block
			int rnd = random.Next(0, 100);
			switch (prevBlockType) {
			case 1:
				if (rnd < 33) {
					prevBlockPos += new Vector2 (Mathf.Max(5 * blockWidth, noBlock * blockWidth), 0);
					prevBlockType = 1;
				} else if (rnd < 66) {
					prevBlockPos += new Vector2 (Mathf.Max(4 * blockWidth, noBlock * blockWidth), 0);
					prevBlockType = 2;
				} else {
					prevBlockPos += new Vector2 (Mathf.Max (4 * blockWidth, noBlock * blockWidth), 0);
					prevBlockType = 3;
				}
				break;
			case 2:
				if (rnd < 40) {
					prevBlockPos += new Vector2 (Mathf.Max(3 * blockWidth, noBlock * blockWidth), 0);
					prevBlockType = 1;
				} else if (rnd < 60) {
					prevBlockPos += new Vector2 (Mathf.Max(1 * blockWidth, noBlock * blockWidth), 0);
					prevBlockType = 2;
				} else {
					prevBlockPos += new Vector2 (Mathf.Max (1 * blockWidth, noBlock * blockWidth), 0);
					prevBlockType = 3;
				}
				break;
			case 3:
				if (rnd < 60) {
					prevBlockPos += new Vector2 (Mathf.Max(2 * blockWidth, noBlock * blockWidth), 0);
					prevBlockType = 1;
				} else if (rnd < 80) {
					prevBlockPos += new Vector2 (Mathf.Max(1 * blockWidth, noBlock * blockWidth), 0);
					prevBlockType = 2;
				} else {
					prevBlockPos += new Vector2 (Mathf.Max (1 * blockWidth, noBlock * blockWidth), 0);
					prevBlockType = 3;
				}
				break;
			}

			switch (prevBlockType) {
			case 1:
				prevBlockPos.y = blockType1;
				break;
			case 2:
				prevBlockPos.y = blockType2;
				break;
			case 3: 
				prevBlockPos.y = blockType3;
				break;
			}

			Instantiate (box, prevBlockPos, Quaternion.identity);
			blocksMade++;
			noBlock = 0;
			blockMap [i] = prevBlockType;
			Debug.Log (i + ": BlockType" + prevBlockType + " Made At: " + prevBlockPos);
		}

		//Generate Bills

	}
		
}
