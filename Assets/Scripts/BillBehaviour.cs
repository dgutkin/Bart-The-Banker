using UnityEngine;
using System.Collections;

public class BillBehaviour : MonoBehaviour {

	//0 = singleBill
	//1 = doubleBill
	//2 = singleStack
	//3 = doubleStack
	//4 = cashBriefcase
	public short billType;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {

		if (other.gameObject.CompareTag ("Player")) {
			Destroy (gameObject);

			switch(billType) {
			case 0:	
				other.SendMessage ("HitSingleBill", SendMessageOptions.DontRequireReceiver);
				break;
			case 1:
				other.SendMessage ("HitDoubleBill", SendMessageOptions.DontRequireReceiver);
				break;
			case 2:
				other.SendMessage ("HitSingleStack", SendMessageOptions.DontRequireReceiver);
				break;
			case 3:
				other.SendMessage ("HitDoubleStack", SendMessageOptions.DontRequireReceiver);
				break;
			case 4:
				other.SendMessage ("HitCashBriefcase", SendMessageOptions.DontRequireReceiver);
				break;
			}
		}

	}

}
