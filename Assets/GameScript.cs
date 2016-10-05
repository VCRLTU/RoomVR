using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		GvrReticle reticle = GetComponentInChildren<GvrReticle>();

	}
	
	// Update is called once per frame
	void Update () 
	{
		RaycastHit hit;
		Physics.Raycast(transform.position, transform.forward, out hit);
		print(hit.collider.GetType().ToString());
		if(Input.GetMouseButtonDown(0))
		{
			print(hit.collider.transform.ToString());
			MeshRenderer mesh;
			mesh = hit.collider.GetComponent<MeshRenderer>();
			print(mesh.material.color.ToString());
			mesh.material.color = new Color(1f, 2f, 3f);
		}
	}
}
