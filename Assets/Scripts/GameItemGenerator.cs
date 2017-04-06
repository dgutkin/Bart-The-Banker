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

	public GameObject trapCage;
	public GameObject taxPaper;
	public GameObject cop;

	public GameObject heart;

	private Camera _cam;
	private Vector2 _originPosition;
	private Vector2 _lastItemPosition;

	private float[,] _probTable = new float[10,10];

	private int _offsetXTiling = 9;
	private int _obstaclesPerTile = 15;

	private float _obstacleHeight = 1.3f;
	private float _obstacleWidth = 1.4f;
	private float _obstacleWidthCopScalingFactor = 2f;
	private float _secondsUntilDestroy = 30f;

	private float _midObstacleHeightFactor = 1;
	private float _highObstacleHeightFactor = 4;
	private float _skyObstacleHeightFactor = 5;

	private int _beforePreviousObstacleType = 0;
	private int _previousObstacleType = 0;
	private int _currentObstacleType = 0;

	//Single Bill, Double Bill, Single Stack, Double Stack, Cash Briefcase
	private int[] _billProbabilities = new int[5]{90, 10, 0, 0, 0};

	private int _heartFrequency = 2;
	private int _yIndex = 0;

	void Awake() {
		
		_cam = Camera.main;

	}

	// Use this for initialization
	void Start () {
		
		_originPosition = transform.position;
		_originPosition += new Vector2 (5f, -3.72f);
		_lastItemPosition = _originPosition;
		_probTable = LoadProbTable ();

	}
	
	// Update is called once per frame
	void Update () {

		// tiling of game items
		// get position of last game item and if camera.x exceeds it then respawn items
		if (_cam.transform.position.x >= _lastItemPosition.x - _offsetXTiling) {
			
			SpawnObjectsNew ();

		}
	
	}

	// Import the table of transition probabilites for the game item generation
	float[,] LoadProbTable() {
		
		#if UNITY_EDITOR

			StreamReader reader = new StreamReader(File.OpenRead(Application.dataPath + "/probTable.csv"));

			float[,] _probTable = new float[10,10];

			int i = 0;
			string line = reader.ReadLine(); // read first line that contains labels before looping
			while (!reader.EndOfStream) {
			
				line = reader.ReadLine();
				string[] values = line.Split(',');

				int cumprob = 0;
				for (int j = 0; j < values.Length-1; ++j) {
					cumprob += int.Parse(values[j+1]);
					_probTable [i,j] = cumprob;
				}
				++i;
			}
			return _probTable;

		#endif

		#if UNITY_ANDROID || UNITY_IOS

			TextAsset textAsset = Resources.Load<TextAsset>("probTable");

			string text = textAsset.text;
			string[] lines = text.Split("\n"[0]);

			for (int k = 1; k < lines.Length; ++k) { // start at the second line of csv to skip titles
			
				string[] values = lines[k].Split(","[0]);
				int cumprob = 0;
				for (int j = 0; j < values.Length-1; ++j) {
					cumprob += int.Parse(values[j+1]);
					_probTable [k-1,j] = cumprob;
				}

			}
			return _probTable;

		#endif

	}

	//Check for an unplayable three-item sequences, true if bad, otherwise false
	bool CheckBadSequence() {

		// low trap/low tax + nothing/sky trap + low trap/low tax
		// low trap/low tax + nothing/sky trap + mid trap/mid tax
		// mid trap/low tax + nothing/sky trap + low trap/low tax

		bool bad = ((_beforePreviousObstacleType == 1 || _beforePreviousObstacleType == 5) &&
		           (_previousObstacleType == 0 || _previousObstacleType == 4) &&
		           (_currentObstacleType == 1 || _currentObstacleType == 5)) 
					||
		           ((_beforePreviousObstacleType == 1 || _beforePreviousObstacleType == 5) &&
		           (_previousObstacleType == 0 || _previousObstacleType == 4) &&
		           (_currentObstacleType == 2 || _currentObstacleType == 6)) 
					||
		           ((_beforePreviousObstacleType == 2 || _beforePreviousObstacleType == 6) &&
		           (_previousObstacleType == 0 || _previousObstacleType == 4) &&
		           (_currentObstacleType == 1 || _currentObstacleType == 5));

		return bad;

	}

	void SpawnObjectsNew() {

		Vector2 obstaclePosition = _lastItemPosition;

		int[] obstaclesGenerated = new int[_obstaclesPerTile];
		List<List<int>> billSpawnpoints = new List<List<int>> ();

		// in a fixed length loop per tile
		for (int obstacles = 0; obstacles < _obstaclesPerTile; obstacles++) {

			// randomly choose the obstacle type
			int randomNumber = Random.Range(0,100);
			for (int i = 0; i < 9; i++) {
				if (randomNumber > _probTable[_previousObstacleType,i]) {
					_currentObstacleType = i+1;
				}
			}

			//Check for bad sequence and if true then skip
			if(CheckBadSequence()) {
				obstacles = obstacles - 1;
				continue;
			}

			obstaclePosition = new Vector2 (obstaclePosition.x, _originPosition.y); // reset the height but keep distance
			obstaclesGenerated[obstacles] = _currentObstacleType;

			// Generate obstacle and note down the trivial bill spawn points
			// Destroy obstacles 30 seconds after they spawn
			switch (_currentObstacleType) {
			case 0: // no obstacle
				obstaclePosition += new Vector2 (_obstacleWidth, 0);
				billSpawnpoints.Add(new List<int>{1, 2});
				break;
			case 1: // floor trap
				obstaclePosition += new Vector2 (_obstacleWidth, 0);
				GameObject floorTrap = Instantiate (trapCage, obstaclePosition, Quaternion.identity) as GameObject;
				Destroy (floorTrap, _secondsUntilDestroy);
				billSpawnpoints.Add(new List<int>{2, 3});
				break;
			case 2: // mid trap
				obstaclePosition += new Vector2 (_obstacleWidth, _midObstacleHeightFactor * _obstacleHeight);
				GameObject midTrap = Instantiate (trapCage, obstaclePosition, Quaternion.identity) as GameObject;
				Destroy (midTrap, _secondsUntilDestroy);
				billSpawnpoints.Add(new List<int>{1});
				break;
			case 3: // high trap
				obstaclePosition += new Vector2(_obstacleWidth, _highObstacleHeightFactor * _obstacleHeight);
				GameObject highTrap = Instantiate(trapCage, obstaclePosition, Quaternion.identity) as GameObject;
				Destroy (highTrap, _secondsUntilDestroy);
				billSpawnpoints.Add(new List<int>{1, 2});
				break;
			case 4: // sky trap
				obstaclePosition += new Vector2(_obstacleWidth, _skyObstacleHeightFactor * _obstacleHeight);
				GameObject skyTrap = Instantiate(trapCage, obstaclePosition, Quaternion.identity) as GameObject;
				Destroy (skyTrap, _secondsUntilDestroy);
				billSpawnpoints.Add(new List<int>{1, 2});
				break;
			case 5: // floor tax
				obstaclePosition += new Vector2(_obstacleWidth, 0);
				GameObject floorTax = Instantiate(taxPaper, obstaclePosition, Quaternion.identity) as GameObject;
				Destroy (floorTax, _secondsUntilDestroy);
				billSpawnpoints.Add(new List<int>{2, 3});
				break;
			case 6: // mid tax
				obstaclePosition += new Vector2(_obstacleWidth, _midObstacleHeightFactor * _obstacleHeight);
				GameObject midTax = Instantiate(taxPaper, obstaclePosition, Quaternion.identity) as GameObject;
				Destroy (midTax, _secondsUntilDestroy);
				billSpawnpoints.Add(new List<int>{1});
				break;
			case 7: // high tax
				obstaclePosition += new Vector2(_obstacleWidth, _highObstacleHeightFactor * _obstacleHeight);
				GameObject highTax = Instantiate(taxPaper, obstaclePosition, Quaternion.identity) as GameObject;
				Destroy (highTax, _secondsUntilDestroy);
				billSpawnpoints.Add(new List<int>{1, 2});
				break;
			case 8: // cop
				obstaclePosition += new Vector2 (_obstacleWidth * _obstacleWidthCopScalingFactor, 0);
				Vector2 copPosition = obstaclePosition + new Vector2 (0, 0.2f); // adjust for the height of the cop so he stands on ground
				GameObject copDude = Instantiate (cop, copPosition, Quaternion.identity) as GameObject;
				obstaclePosition += new Vector2 (_obstacleWidth * _obstacleWidthCopScalingFactor, 0); // create more space after the cop
				Destroy (copDude, _secondsUntilDestroy);
				billSpawnpoints.Add(new List<int>{1, 2, 3});
				billSpawnpoints.Add(new List<int>{1, 2, 3});
				billSpawnpoints.Add(new List<int>{1, 2, 3});
				billSpawnpoints.Add(new List<int>{1, 2, 3});
				break;
			}

			// Update obstacle trackers
			_beforePreviousObstacleType = _previousObstacleType;
			_previousObstacleType = _currentObstacleType;
			_currentObstacleType = 0;

		}

		SpawnBillsAndLives(obstaclesGenerated, billSpawnpoints);

		_lastItemPosition = new Vector2(obstaclePosition.x, _obstacleHeight);

	}

	void SpawnBillsAndLives(int[] obstaclesGenerated, List<List<int>> billSpawnpoints) {

		//Find all possible non-trivial spawn points (sequence of 3) and revise impossible sequences
		for (int i = 0; i < obstaclesGenerated.Length - 2; ++i) {
			//Create string encoded sequence
			string sequence = obstaclesGenerated[i].ToString() + 
				obstaclesGenerated[i+1].ToString() + 
				obstaclesGenerated[i+2].ToString();

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

		Vector2 billPosition = _lastItemPosition;

		//Place bills into spawnpoints by probability
		int billFrequency = 30; // percent of the time a bill appears in a column slot
		for (int i = 0; i < billSpawnpoints.Count; ++i) {
			//Skip if no possible spawn in the column slot
			if (billSpawnpoints [i].Count == 0) {
				billFrequency += 5;
				continue;
			}

			//Generate random number
			int randomNumber = Random.Range (0, 100);

			// Spawn Heart
			if (randomNumber < _heartFrequency) {
				//RNG the location in the current column slot (yIndex)
				int heartRNG = Random.Range (0, billSpawnpoints [i].Count);

				//Reset the height but keep distance
				billPosition = new Vector2 (billPosition.x, _originPosition.y); 

				//Spawn Heart
				switch (billSpawnpoints [i] [heartRNG]) {
				case 1: //Floor position
					billPosition += new Vector2 (_obstacleWidth, 0);
					break;
				case 2: //Mid position
					billPosition += new Vector2 (_obstacleWidth, _midObstacleHeightFactor * _obstacleHeight);
					break;
				case 3: //High-1 position
					billPosition += new Vector2 (_obstacleWidth, (_highObstacleHeightFactor - 1) * _obstacleHeight);
					break;
				}
				Instantiate (heart, billPosition, Quaternion.identity);

				continue;
			}

			// increase probability for every turn to achieve consistency, ie. see pseudo random distribution
			billFrequency += 5;  

			//Check if Bill will be generated 
			if (randomNumber > (100 - billFrequency)) {
				//Scale probabilities based on current progress
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
				int rngLocation = Random.Range (0, billSpawnpoints [i].Count);

				//Reset the height but keep distance
				billPosition = new Vector2 (billPosition.x, _originPosition.y); 

				//Spawn bill
				Debug.Log(billSpawnpoints[i][rngLocation].ToString());
				switch (billSpawnpoints [i] [rngLocation]) {
				case 1: //Floor position
					billPosition += new Vector2 (_obstacleWidth, 0);
					break;
				case 2: //Mid position
					billPosition += new Vector2 (_obstacleWidth, _midObstacleHeightFactor * _obstacleHeight);
					break;
				case 3: //High-1 position
					billPosition += new Vector2 (_obstacleWidth, (_highObstacleHeightFactor - 1) * _obstacleHeight);
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
				billPosition += new Vector2 (_obstacleWidth, 0);
			}

			_yIndex++;
		}

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
			_billProbabilities [2] = 30;
			_billProbabilities [3] = 30;
			_billProbabilities [4] = 15;
			break;
		}
	}
				
}