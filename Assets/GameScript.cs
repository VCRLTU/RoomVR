using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

    GameObject itemHeld;
    bool holdingItem = false;
    bool colorSet = false;
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
        if (hit.transform)
        {
            if (Input.GetMouseButtonDown(0))
            {
                print(hit.transform.ToString());
                if (!holdingItem)
                {
                    if (colorSet && hit.transform.tag == "Paintable")
                    {
                        MeshRenderer mesh;
                        mesh = hit.collider.GetComponent<MeshRenderer>();
                        mesh.material.color = setter;
                    }
                    else if (hit.transform.tag == "Moveable")
                    {
                        holdingItem = true;
                        itemHeld = hit.transform.gameObject;
                        itemHeld.layer = 0;
                        print("move");
                    }
                }
                else
                {
                    itemHeld.layer = 8;
                    holdingItem = false;
                    itemHeld = null;
                }
            }
            else if (holdingItem)
            {
                itemHeld.transform.rotation = hit.transform.rotation;
                itemHeld.transform.position = hit.point;
            }
        }
        transform.Rotate(new Vector3(Input.GetAxis("Vertical") * Time.deltaTime * 20, Input.GetAxis("Horizontal") * Time.deltaTime*20 , 0));
	}

    public void setRed()
    {
        setter = new Color(1, 0, 0);
        colorSet = true;
    }

    public void setBlue()
    {
        setter = new Color(0, 0, 1);
        colorSet = true;
    }

    public void setGreen()
    {
        setter = new Color(0, 1, 0);
        colorSet = true;
    }
    public void setWhite()
    {
        setter = new Color(1, 1, 1);
        colorSet = true;
    }

}
