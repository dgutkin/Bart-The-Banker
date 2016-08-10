using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameItemGenerator : MonoBehaviour {

	public GameObject gavel;
	public GameObject bill;
	public GameObject deathBlock;
	public GameObject taxBlock;
	public GameObject cop;

	private Vector2 originPosition;
	private Vector2 lastItemPosition;
	private Camera cam;
	private int offSetX = 9;
	private int blocksPerTile = 15;

	float[,] probTable = new float[10,10];

	float blockHeight = 1.2f;
	float blockWidth = 1.2f;

	float midBlockHeightFactor = 1;
	float highBlockHeightFactor = 4;
	float skyBlockHeightFactor = 5;

	int beforePreviousBlockType = 0;
	int previousBlockType = 0;
	int currentBlockType = 0;

	void Awake() {
		cam = Camera.main;
	}

	// Use this for initialization
	void Start () {
		originPosition = transform.position;
		originPosition += new Vector2 (5f, -3.72f);
		lastItemPosition = originPosition;

		probTable = LoadProbTable ();
	}
	
	// Update is called once per frame
	void Update () {

		// tiling of game items
		// get position of last game item and if camera.x exceeds it then call spawnobjects2 again
		if (cam.transform.position.x >= lastItemPosition.x - offSetX) {
			SpawnObjects2 ();
		}
	
	}

	/*Dictionary<string, Dictionary<string, float> > loadProbTable() {
		var reader = new StreamReader(File.OpenRead(@"../probTable.csv"));
		Dictionary<string, Dictionary<string, float> > probTable = new Dictionary<string, Dictionary<string, float> > ();

		var line = reader.ReadLine();
		var keys = line.Split(',');
		while (!reader.EndOfStream)
		{
			line = reader.ReadLine();
			var values = line.Split(',');

			for (int i = 1; i < keys.Length; ++i) {
				probTable [values [0]] [keys [i]] = values [i];
			}
				
		}
		return probTable;
	}*/

	float[,] LoadProbTable() {
		// probTable csv saved in Assets folder of the project
		StreamReader reader = new StreamReader(File.OpenRead(@Application.dataPath + "/probTable.csv"));
		float[,] probTable = new float[10,10];

		int i = 0;
		string line = reader.ReadLine(); // read first line that contains labels before looping
		while (!reader.EndOfStream)
		{
			line = reader.ReadLine();
			string[] values = line.Split(',');

			int cumu = 0;
			for (int j = 0; j < values.Length-1; ++j) {
				cumu += int.Parse(values[j+1]);
				probTable [i,j] = cumu;
			}
			++i;
		}
		return probTable;
	}

	void SpawnObjects2() {

		//float[,] probTable = new float[10,10];
		// fill in the table with cumulative transition probabilities
		//probTable = LoadProbTable();
		Vector2 blockPosition = lastItemPosition;
		Vector2 billPosition = lastItemPosition;

		bool createBill = false;
		int billFrequency = 20; // percent of the time a bill appears in a column slot

		// in fixed length loop (level)
		for (int blocks = 0; blocks < blocksPerTile; blocks++) { // destroy previous game items to save space

			int randomNumber = Random.Range(0,100);
			for (int i = 0; i < 10; i++) {
				if (randomNumber > probTable [previousBlockType,i]) {
					currentBlockType = i+1;
				}
			}

			if (((beforePreviousBlockType == 1 || beforePreviousBlockType == 5) &&
				(previousBlockType == 0 || previousBlockType == 4) &&
				(currentBlockType == 1 || currentBlockType == 5)) ||
				((beforePreviousBlockType == 1 || beforePreviousBlockType == 5) &&
					(previousBlockType == 0 || previousBlockType == 4) &&
					(currentBlockType == 2 || currentBlockType == 6)) ||
				((beforePreviousBlockType == 2 || beforePreviousBlockType == 6) &&
					(previousBlockType == 0 || previousBlockType == 4) &&
					(currentBlockType == 1 || currentBlockType == 5))) {
				// prevent undoable scenarios
				blocks = blocks - 1;
				continue;
			}

			int randomNumber2 = Random.Range (0, 100); // max exclusive
			createBill = false;
			billFrequency += 5;  // increase probability for every turn to achieve consitency
			if (randomNumber2 > (100-billFrequency)) {
				createBill = true;
				billFrequency = 20;
			}

			blockPosition = new Vector2 (blockPosition.x, originPosition.y); // reset the height but keep distance

			switch (currentBlockType) {
			case 0: // no block
				blockPosition += new Vector2 (blockWidth, 0);
				int randomNumber3 = Random.Range (0, 2);

				if (createBill) {
					if (randomNumber3 == 0) { // half probability having to jump for bill instead of running into it
						billPosition = blockPosition;
					} else {
						billPosition = blockPosition + new Vector2 (0, midBlockHeightFactor * blockHeight);
					}
					Instantiate (bill, billPosition, Quaternion.identity);
					// half probability of instantiating block position above
				}
				break;
			case 1: // floor death block
				blockPosition += new Vector2 (blockWidth, 0);
				GameObject floorBlock = Instantiate (deathBlock, blockPosition, Quaternion.identity) as GameObject;
				Destroy (floorBlock, 30.0f);
				billPosition = blockPosition + new Vector2 (0, midBlockHeightFactor * blockHeight);
				if (createBill) {
					Instantiate (bill, billPosition, Quaternion.identity);
				}
				break;
			case 2: // mid death block
				blockPosition += new Vector2 (blockWidth, midBlockHeightFactor * blockHeight);
				GameObject midBlock = Instantiate (deathBlock, blockPosition, Quaternion.identity) as GameObject;
				Destroy (midBlock, 30.0f);
				billPosition = blockPosition - new Vector2 (0, midBlockHeightFactor * blockHeight);
				if (createBill) {
					Instantiate (bill, billPosition, Quaternion.identity);
				}
				break;
			case 3: // high death blocks
				blockPosition += new Vector2(blockWidth, highBlockHeightFactor * blockHeight);
				GameObject highBlock = Instantiate(deathBlock, blockPosition, Quaternion.identity) as GameObject;
				Destroy (highBlock, 30.0f);
				break;
			case 4: // sky death blocks
				blockPosition += new Vector2(blockWidth, skyBlockHeightFactor * blockHeight);
				GameObject skyBlock = Instantiate(deathBlock, blockPosition, Quaternion.identity) as GameObject;
				Destroy (skyBlock, 30.0f);
				break;
			case 5: // floor tax block
				blockPosition += new Vector2(blockWidth, 0);
				GameObject floorTax = Instantiate(taxBlock, blockPosition, Quaternion.identity) as GameObject;
				Destroy (floorTax, 30.0f);
				// change transform being initiated
				break;
			case 6: // mid tax block
				blockPosition += new Vector2(blockWidth, midBlockHeightFactor * blockHeight);
				GameObject midTax = Instantiate(taxBlock, blockPosition, Quaternion.identity) as GameObject;
				Destroy (midTax, 30.0f);
				break;
			case 7: // high tax block
				blockPosition += new Vector2(blockWidth, highBlockHeightFactor * blockHeight);
				GameObject highTax = Instantiate(taxBlock, blockPosition, Quaternion.identity) as GameObject;
				Destroy (highTax, 30.0f);
				break;
			case 8: // cop
				blockPosition += new Vector2 (blockWidth * 2, 0);
				GameObject copDude = Instantiate (cop, blockPosition, Quaternion.identity) as GameObject;
				blockPosition += new Vector2 (blockWidth * 2, 0); // create more space after the cop
				Destroy (copDude, 30.0f);
				break;
			case 9: // judge
				blockPosition += new Vector2(blockWidth, 0);
				break;
			}

			beforePreviousBlockType = previousBlockType;
			previousBlockType = currentBlockType;
			currentBlockType = 0;

		}

		lastItemPosition = blockPosition;
			
	}
		
	void SpawnObjects() {
		//Origin is Vector2 (2, 0), Floor is (7, -4.92)
		int blocksGenerated = 30;
		int[] blockMap = new int[blocksGenerated];

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

		//Bill positions
		Vector2 billPosition = originPosition;
		float billPos1 = originPosition.y + 2 * blockHeight;
		float billPos2 = originPosition.y;
		float billPos3 = originPosition.y + blockHeight;


		if (firstRandom < 70) {
			//Floor block (1)
			prevBlockPos.y = blockType1;
			prevBlockType = 1;
			billPosition.y = billPos1;
		} else if (firstRandom < 75) {
			//Head block (2)
			prevBlockPos.y = blockType2;
			prevBlockType = 2;
			billPosition.y = billPos2;
		} else {
			//Jump block (3)
			prevBlockPos.y = blockType3;
			prevBlockType = 3;
			billPosition.y = billPos3;
		}

		Instantiate (deathBlock, prevBlockPos, Quaternion.identity);
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

				//Bill
				int noBlockBillP = Random.Range(0, 10);
				if (noBlockBillP < 6) {
					billPosition.x = originPosition.x + i * blockWidth;
					billPosition.y = billPos2;
					Instantiate (bill, billPosition, Quaternion.identity);
				}
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

			Instantiate (deathBlock, prevBlockPos, Quaternion.identity);
			blocksMade++;
			noBlock = 0;
			blockMap [i] = prevBlockType;
			Debug.Log (i + ": BlockType" + prevBlockType + " Made At: " + prevBlockPos);

			//Simple bill gen
			int billP = Random.Range (0, 10);
			if (billP < 6) {
				billPosition.x = prevBlockPos.x;
				switch (prevBlockType) {
				case 1:
					billPosition.y = billPos1;
					break;
				case 2:
					billPosition.y = billPos2;
					break;
				case 3:
					billPosition.y = billPos3;
					break;
				}
				Instantiate (bill, billPosition, Quaternion.identity);
			}
		}

		//Generate Bills

	}
		
}
