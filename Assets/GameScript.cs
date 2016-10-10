using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

    Color setter;

	// Use this for initialization
	void Start () 
	{
		GvrReticle reticle = GetComponentInChildren<GvrReticle>();

	}
	
	// Update is called once per frame
	void Update () 
	{
		RaycastHit hit;
		Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, (1<<8));
		if(Input.GetMouseButtonDown(0) && hit.transform && (setter != null))
		{
			MeshRenderer mesh;
			mesh = hit.collider.GetComponent<MeshRenderer>();
            mesh.material.color = setter;
		}

        transform.Rotate(new Vector3(Input.GetAxis("Vertical") * Time.deltaTime * 20, Input.GetAxis("Horizontal") * Time.deltaTime*20 , 0));
	}

    public void setRed()
    {
        setter = new Color(1, 0, 0);
    }

    public void setBlue()
    {
        setter = new Color(0, 0, 1);
    }

    public void setGreen()
    {
        setter = new Color(0, 1, 0);
    }
    public void setWhite()
    {
        setter = new Color(1, 1, 1);
        }

}
