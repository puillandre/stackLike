using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveRubble : MonoBehaviour {

    public TheStack theStack;

  
    private void OnTriggerEnter (Collider col) {
        Destroy (col.gameObject);
	}
}
