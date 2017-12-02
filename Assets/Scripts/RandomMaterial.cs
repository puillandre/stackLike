using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterial : MonoBehaviour {

    public Material[] materials;

	private int materialID;
	private int newMaterialID;

	private bool needNewMaterial = false;

	void Start () {
        GenerateMaterial();
	}

    public void GenerateMaterial()
    {
        int r = Random.Range(0, materials.Length);
        GetComponent<Renderer>().material = materials[r];
		materialID = r;
    }

	void Update()
	{
		if (needNewMaterial)
		{
			needNewMaterial = false;
			GetComponent<Renderer>().material = materials[newMaterialID];
		}
	}

	public void SetMaterialID(int id)
	{
		needNewMaterial = true;
		newMaterialID = id;
	}

	public int getMaterialID()
	{
		return (materialID);
	}
}
