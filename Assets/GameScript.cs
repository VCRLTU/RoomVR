﻿using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

    private static float WALL_LEN = 2f;
    private static int LAYER_WALL = 8;
    private static int LAYER_MOVE = 9;

    GameObject itemHeld;
	Collider itemColider;
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
        if (Input.GetMouseButtonDown(0))
        {
            Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, (1 << LAYER_WALL) + (1 << LAYER_MOVE));
            if (hit.transform)
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
                        itemColider = itemHeld.GetComponent<Collider>();
                        itemHeld.layer = 0;
                    }
                    else if (hit.transform.tag == "Dupe")
                    {
                        holdingItem = true;
                        itemHeld = Instantiate(hit.collider.gameObject);
                        itemColider = itemHeld.GetComponent<Collider>();
                        itemHeld.tag = "Moveable";
                        itemHeld.layer = 0;
                    }
                }
                else
                {
                    itemHeld.layer = LAYER_MOVE;
                    holdingItem = false;
                    itemHeld = null;
                }
            }
        }
        else if (holdingItem)
        {
            Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, (1 << LAYER_WALL));
            itemHeld.transform.rotation = hit.transform.rotation;

            if (hit.point.x < 2.02f - itemColider.bounds.extents.x)     //Weird and long way to make sure the item held doesn't go half outside the walls (snap to the edges)
            {
                if (hit.point.x > -2.02f + itemColider.bounds.extents.x)
                {
                    if (hit.point.z < 2.02f - itemColider.bounds.extents.z)
                    {
                        if (hit.point.z > -2.02f + itemColider.bounds.extents.z)
                        {
                            itemHeld.transform.rotation = hit.transform.rotation;
                            itemHeld.transform.position = hit.point;
                        }
                        else
                        {
                            Vector3 point = hit.point;
                            point.z = -hit.collider.bounds.extents.z + itemColider.bounds.extents.z;
                            itemHeld.transform.position = point;
                        }
                    }
                    else
                    {
                        Vector3 point = hit.point;
                        point.z = hit.collider.bounds.extents.z - itemColider.bounds.extents.z;
                        itemHeld.transform.position = point;
                    }
                }
                else
                {
                    Vector3 point = hit.point;
                    point.x = -hit.collider.bounds.extents.x + itemColider.bounds.extents.x;
                    itemHeld.transform.position = point;
                }
            }
            else
            {
                Vector3 point = hit.point;
                point.x = hit.collider.bounds.extents.x - itemColider.bounds.extents.x;
                itemHeld.transform.position = point;
            }
            if (hit.point.y < 2f - itemColider.bounds.extents.y)
            {
                if (hit.point.y > 0f + itemColider.bounds.extents.y)
                {
                    //Do nothing

                }
                else
                {
                    print(hit.collider.bounds.extents.y);
                    Vector3 point = itemHeld.transform.position;
                    point.y = -hit.collider.bounds.extents.y + itemColider.bounds.extents.y + 1;
                    itemHeld.transform.position = point;
                }
            }
            else
            {
                Vector3 point = itemHeld.transform.position;
                point.y = hit.collider.bounds.extents.y - itemColider.bounds.extents.y + 1;
                itemHeld.transform.position = point;
            }
        }
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
