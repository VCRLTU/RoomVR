using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Physics.Raycast(transform.position, transform.forward, out hit);
			print(hit.collider.transform.ToString());
			MeshRenderer mesh;
			mesh = hit.collider.GetComponent<MeshRenderer>();
			print(mesh.material.color.ToString());
			mesh.material.color = new Color(1f, 2f, 3f);
		}
	}
}
