using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameItemGenerator : MonoBehaviour {

	public GameObject singleBill;
	public GameObject doubleBill;
	public GameObject singleStack;
	public GameObject doubleStack;
	public GameObject cashBriefcase;

	public GameObject deathBlock;
	public GameObject taxBlock;
	public GameObject cop;

	private Vector2 originPosition;
	private Vector2 lastItemPosition;
	private Camera cam;
	private int offSetX = 9;
	private int blocksPerTile = 15;
	private int[] _billProbabilities = new int[5]{90, 10, 0, 0, 0}; //Single Bill, Double Bill, Single Stack, Double Stack, Briefcase

	private float[,] probTable = new float[10,10];

	private float blockHeight = 1.2f;
	private float blockWidth = 1.2f;

	private float midBlockHeightFactor = 1;
	private float highBlockHeightFactor = 4;
	private float skyBlockHeightFactor = 5;

	private int _beforePreviousBlockType = 0;
	private int _previousBlockType = 0;
	private int _currentBlockType = 0;

	private int _yIndex = 0;

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
			SpawnObjects3 ();
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

	//Checks for an bad sequence, true if it is bad, false if not
	bool CheckBadSequence() {
		if (((_beforePreviousBlockType == 1 || _beforePreviousBlockType == 5) &&
		    	(_previousBlockType == 0 || _previousBlockType == 4) &&
		    	(_currentBlockType == 1 || _currentBlockType == 5)) ||
		    ((_beforePreviousBlockType == 1 || _beforePreviousBlockType == 5) &&
		    	(_previousBlockType == 0 || _previousBlockType == 4) &&
		    	(_currentBlockType == 2 || _currentBlockType == 6)) ||
		    ((_beforePreviousBlockType == 2 || _beforePreviousBlockType == 6) &&
		    	(_previousBlockType == 0 || _previousBlockType == 4) &&
		    	(_currentBlockType == 1 || _currentBlockType == 5))) {
			return true;
		}

		return false;
	}

	void SpawnObjects3() {

		Vector2 blockPosition = lastItemPosition;
		Vector2 billPosition = lastItemPosition;

		int[] blocksGenerated = new int[blocksPerTile];
		List<List<int> > billSpawnpoints = new List<List<int> > ();

		// in fixed length loop (level)
		for (int blocks = 0; blocks < blocksPerTile; blocks++) { // destroy previous game items to save space

			int randomNumber = Random.Range(0,100);
			for (int i = 0; i < 10; i++) {
				if (randomNumber > probTable [_previousBlockType,i]) {
					_currentBlockType = i+1;
				}
			}

			//Check for bad sequence to skip
			if(CheckBadSequence()) {
				blocks = blocks - 1;
				continue;
			}

			blockPosition = new Vector2 (blockPosition.x, originPosition.y); // reset the height but keep distance
			blocksGenerated[blocks] = _currentBlockType;

			//Generate block and note down the trivial bill spawn points
			switch (_currentBlockType) {
			case 0: // no block
				blockPosition += new Vector2 (blockWidth, 0);
				billSpawnpoints.Add(new List<int>{1, 2});
				break;
			case 1: // floor death block
				blockPosition += new Vector2 (blockWidth, 0);
				GameObject floorBlock = Instantiate (deathBlock, blockPosition, Quaternion.identity) as GameObject;
				Destroy (floorBlock, 30.0f);
				billSpawnpoints.Add(new List<int>{2, 3});
				break;
			case 2: // mid death block
				blockPosition += new Vector2 (blockWidth, midBlockHeightFactor * blockHeight);
				GameObject midBlock = Instantiate (deathBlock, blockPosition, Quaternion.identity) as GameObject;
				Destroy (midBlock, 30.0f);
				billSpawnpoints.Add(new List<int>{1});
				break;
			case 3: // high death blocks
				blockPosition += new Vector2(blockWidth, highBlockHeightFactor * blockHeight);
				GameObject highBlock = Instantiate(deathBlock, blockPosition, Quaternion.identity) as GameObject;
				Destroy (highBlock, 30.0f);
				billSpawnpoints.Add(new List<int>{1, 2});
				break;
			case 4: // sky death blocks
				blockPosition += new Vector2(blockWidth, skyBlockHeightFactor * blockHeight);
				GameObject skyBlock = Instantiate(deathBlock, blockPosition, Quaternion.identity) as GameObject;
				Destroy (skyBlock, 30.0f);
				billSpawnpoints.Add(new List<int>{1, 2});
				break;
			case 5: // floor tax block
				blockPosition += new Vector2(blockWidth, 0);
				GameObject floorTax = Instantiate(taxBlock, blockPosition, Quaternion.identity) as GameObject;
				Destroy (floorTax, 30.0f);
				billSpawnpoints.Add(new List<int>{2, 3});
				break;
			case 6: // mid tax block
				blockPosition += new Vector2(blockWidth, midBlockHeightFactor * blockHeight);
				GameObject midTax = Instantiate(taxBlock, blockPosition, Quaternion.identity) as GameObject;
				Destroy (midTax, 30.0f);
				billSpawnpoints.Add(new List<int>{1});
				break;
			case 7: // high tax block
				blockPosition += new Vector2(blockWidth, highBlockHeightFactor * blockHeight);
				GameObject highTax = Instantiate(taxBlock, blockPosition, Quaternion.identity) as GameObject;
				Destroy (highTax, 30.0f);
				billSpawnpoints.Add(new List<int>{1, 2});
				break;
			case 8: // cop
				blockPosition += new Vector2 (blockWidth * 2, 0);
				GameObject copDude = Instantiate (cop, blockPosition, Quaternion.identity) as GameObject;
				blockPosition += new Vector2 (blockWidth * 2, 0); // create more space after the cop
				Destroy (copDude, 30.0f);
				billSpawnpoints.Add(new List<int>{1, 2, 3});
				break;
			case 9: // judge
				blockPosition += new Vector2(blockWidth, 0);
				billSpawnpoints.Add(new List<int>{1});
				break;
			}

			_beforePreviousBlockType = _previousBlockType;
			_previousBlockType = _currentBlockType;
			_currentBlockType = 0;

		}

		lastItemPosition = blockPosition;

		//Generate bills
		//Find all possible non-trivial spawn points (sequence of 3) and revise impossible sequences
		for (int i = 0; i < blocksGenerated.Length - 2; ++i) {
			//Create string encoded sequence
			string sequence = blocksGenerated[i].ToString() + blocksGenerated[i+1].ToString() + blocksGenerated[i+2].ToString();

			switch (sequence) {
			case "202": //Mid, Empty, Mid
				billSpawnpoints[i+1] = new List<int>{};
				break;
			case "000": //Empty, Empty, Empty
			case "040": //Empty, Sky, Empty
			case "404": //Sky, Empty, Sky
			case "400": //Sky, Empty, Empty
			case "004": //Empty, Empty, Sky
				billSpawnpoints[i+1] = new List<int>{1, 2, 3};
				break;
			}
		}

		//Set last 2 columns to invalid spawns
		billSpawnpoints [billSpawnpoints.Count - 1] = new List<int>{ };
		billSpawnpoints [billSpawnpoints.Count - 2] = new List<int>{ };

		//Place bills into spawnpoints by probability
		int billFrequency = 30; // percent of the time a bill appears in a column slot
		for (int i = 0; i < billSpawnpoints.Count; ++i) {
			//Skip if no possible spawn in the column slot
			if (billSpawnpoints [i].Count == 0) {
				billFrequency += 5;
				continue;
			}

			//Generate random number
			int randomNumber2 = Random.Range (0, 100);

			// increase probability for every turn to achieve consistency, ie. see pseudo random distribution
			billFrequency += 5;  

			//Check if Bill will be generated 
			if (randomNumber2 > (100 - billFrequency)) {
				//Scale probabilities
				AdjustBillProbabilities ();

				//Get a bill from bill type distribution
				int rng = Random.Range (0, 100);
				int billType = 0;

				int cumu = 0;
				for (int j = 0; j < _billProbabilities.Length; ++j) {
					cumu += _billProbabilities [j];
					if (rng < cumu) {
						billType = j;
						break;
					}

				}

				//RNG the location in the current column slot (yIndex)
				int rng2 = Random.Range (0, billSpawnpoints [i].Count);

				//Reset the height but keep distance
				billPosition = new Vector2 (billPosition.x, originPosition.y); 

				//Spawn bill
				switch (billSpawnpoints [i] [rng2]) {
				case 1: //Floor position
					billPosition += new Vector2 (blockWidth, 0);
					break;
				case 2: //Mid position
					billPosition += new Vector2 (blockWidth, midBlockHeightFactor * blockHeight);
					break;
				case 3: //High-1 position
					billPosition += new Vector2 (blockWidth, (highBlockHeightFactor - 1) * blockHeight);
					break;
				}

				switch(billType) {
				case 0:	//Single bill
					Instantiate (singleBill, billPosition, Quaternion.identity);
					break;
				case 1: //Double bill
					Instantiate (doubleBill, billPosition, Quaternion.identity);
					break;
				case 2: //Single stack
					Instantiate (singleStack, billPosition, Quaternion.identity);
					break;
				case 3: //Double stack
					Instantiate (doubleStack, billPosition, Quaternion.identity);
					break;
				case 4: //Briefcase
					Instantiate (cashBriefcase, billPosition, Quaternion.identity);
					break;
				}

				//Reset probability
				billFrequency = 30;
			} else {
				billPosition += new Vector2 (blockWidth, 0);
			}

			_yIndex++;
		}
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
				if (randomNumber > probTable [_previousBlockType,i]) {
					_currentBlockType = i+1;
				}
			}

			if (((_beforePreviousBlockType == 1 || _beforePreviousBlockType == 5) &&
					(_previousBlockType == 0 || _previousBlockType == 4) &&
					(_currentBlockType == 1 || _currentBlockType == 5)) ||
				((_beforePreviousBlockType == 1 || _beforePreviousBlockType == 5) &&
					(_previousBlockType == 0 || _previousBlockType == 4) &&
					(_currentBlockType == 2 || _currentBlockType == 6)) ||
				((_beforePreviousBlockType == 2 || _beforePreviousBlockType == 6) &&
					(_previousBlockType == 0 || _previousBlockType == 4) &&
					(_currentBlockType == 1 || _currentBlockType == 5))) {
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

			switch (_currentBlockType) {
			case 0: // no block
				blockPosition += new Vector2 (blockWidth, 0);
				int randomNumber3 = Random.Range (0, 2);

				if (createBill) {
					if (randomNumber3 == 0) { // half probability having to jump for bill instead of running into it
						billPosition = blockPosition;
					} else {
						billPosition = blockPosition + new Vector2 (0, midBlockHeightFactor * blockHeight);
					}
					Instantiate (singleBill, billPosition, Quaternion.identity);
					// half probability of instantiating block position above
				}
				break;
			case 1: // floor death block
				blockPosition += new Vector2 (blockWidth, 0);
				GameObject floorBlock = Instantiate (deathBlock, blockPosition, Quaternion.identity) as GameObject;
				Destroy (floorBlock, 30.0f);
				billPosition = blockPosition + new Vector2 (0, midBlockHeightFactor * blockHeight);
				if (createBill) {
					Instantiate (singleBill, billPosition, Quaternion.identity);
				}
				break;
			case 2: // mid death block
				blockPosition += new Vector2 (blockWidth, midBlockHeightFactor * blockHeight);
				GameObject midBlock = Instantiate (deathBlock, blockPosition, Quaternion.identity) as GameObject;
				Destroy (midBlock, 30.0f);
				billPosition = blockPosition - new Vector2 (0, midBlockHeightFactor * blockHeight);
				if (createBill) {
					Instantiate (singleBill, billPosition, Quaternion.identity);
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

			_beforePreviousBlockType = _previousBlockType;
			_previousBlockType = _currentBlockType;
			_currentBlockType = 0;
		}

		lastItemPosition = blockPosition;
			
	}

	void AdjustBillProbabilities() {
		if (_yIndex > 500) {
			return;
		}

		//Adjust probabilities
		switch(_yIndex) {
		case 50:
			_billProbabilities [0] = 60;
			_billProbabilities [1] = 30;
			_billProbabilities [2] = 10;
			_billProbabilities [3] = 0;
			_billProbabilities [4] = 0;
			break;
		case 100:
			_billProbabilities [0] = 35;
			_billProbabilities [1] = 35;
			_billProbabilities [2] = 20;
			_billProbabilities [3] = 10;
			_billProbabilities [4] = 0;
			break;
		case 200:
			_billProbabilities [0] = 25;
			_billProbabilities [1] = 30;
			_billProbabilities [2] = 25;
			_billProbabilities [3] = 15;
			_billProbabilities [4] = 5;
			break;
		case 300:
			_billProbabilities [0] = 15;
			_billProbabilities [1] = 20;
			_billProbabilities [2] = 35;
			_billProbabilities [3] = 20;
			_billProbabilities [4] = 10;
			break;
		case 500:
			_billProbabilities [0] = 10;
			_billProbabilities [1] = 15;
			_billProbabilities [2] = 20;
			_billProbabilities [3] = 20;
			_billProbabilities [4] = 15;
			break;
		}
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
					Instantiate (singleBill, billPosition, Quaternion.identity);
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
				Instantiate (singleBill, billPosition, Quaternion.identity);
			}
		}

		//Generate Bills

	}
		
}
