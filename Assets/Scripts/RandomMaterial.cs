using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterial : MonoBehaviour {

    public Material[] materials;

	void Start () {
        GenerateMaterial();
	}

    public void GenerateMaterial()
    {
        int r = Random.Range(0, materials.Length);
        GetComponent<Renderer>().material = materials[r];
    }
}
