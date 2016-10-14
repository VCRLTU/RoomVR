using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour {

    private static float WALL_LEN = 2f;
    private static int LAYER_WALL = 8;
    private static int LAYER_MOVE = 9;
	private static float Y_ITEMOFFSET = 0.001f;

    GameObject itemHeld;
	Collider itemColider;
    bool holdingItem = false;
    bool colorSet = false;
	bool itemIntersect = false;
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
						itemHeld.GetComponent<ItemScript>().setHolder(this);
                    }
                    else if (hit.transform.tag == "Dupe")
                    {
                        holdingItem = true;
                        itemHeld = Instantiate(hit.collider.gameObject);
                        itemColider = itemHeld.GetComponent<Collider>();
                        itemHeld.tag = "Moveable";
						itemHeld.layer = 0;
						itemHeld.GetComponent<ItemScript>().setHolder(this);
					}
                }
                else
                {
					if(itemIntersect == false)
					{
						itemHeld.transform.position = hit.point;
	                    itemHeld.layer = LAYER_MOVE;
						itemHeld.GetComponent<ItemScript>().removeHolder();
	                    holdingItem = false;
	                    itemHeld = null;
					}
				}
            }
        }
        else if (holdingItem)
        {
            Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, (1 << LAYER_WALL));
			if(hit.transform)
			{
				Vector3 point = hit.point;
				//Bad solution
				float y = hit.transform.rotation.eulerAngles.y;
				print(y);
				if( y > -1 && y <1)
					point.z = point.z - 0.01f;
				if( y > 89 && y < 91)
					point.x = point.x - 0.01f; 
				if( y > 179 && y < 181)
					point.z = point.z + 0.01f;
				if( y > 269 && y < 271)
					point.x = point.x + 0.01f; 

				//end of bad solution
				itemHeld.transform.rotation = hit.transform.rotation;
				//Weird and long way to make sure the item held doesn't go half outside the walls (snap to the edges)
	            if (hit.point.x < 2.02f - itemColider.bounds.extents.x)    
	            {
	                if (hit.point.x > -2.02f + itemColider.bounds.extents.x)
	                {
	                    if (hit.point.z < 2.02f - itemColider.bounds.extents.z)
	                    {
	                        if (hit.point.z > -2.02f + itemColider.bounds.extents.z)
	                        {
	                            itemHeld.transform.position = point;
	                        }
	                        else
	                        {
	                            point.z = -hit.collider.bounds.extents.z + itemColider.bounds.extents.z;
	                            itemHeld.transform.position = point;
	                        }
	                    }
	                    else
	                    {
	                        point.z = hit.collider.bounds.extents.z - itemColider.bounds.extents.z;
	                        itemHeld.transform.position = point;
	                    }
	                }
	                else
	                {
	                    point.x = -hit.collider.bounds.extents.x + itemColider.bounds.extents.x;
	                    itemHeld.transform.position = point;
	                }
	            }
	            else
	            {
	                point.x = hit.collider.bounds.extents.x - itemColider.bounds.extents.x;
	                itemHeld.transform.position = point;
	            }

				//Same as above but only for y axis
	            if (hit.point.y < 2f - itemColider.bounds.extents.y)		
	            {
	                if (hit.point.y > 0f + itemColider.bounds.extents.y)
	                {
	                    //Do nothing

	                }
	                else
	                {
	                    point.y = -hit.collider.bounds.extents.y + itemColider.bounds.extents.y + 1 + Y_ITEMOFFSET;
	                    itemHeld.transform.position = point;
	                }
	            }
	            else
	            {
					point.y = hit.collider.bounds.extents.y - itemColider.bounds.extents.y + 1 - Y_ITEMOFFSET;
	                itemHeld.transform.position = point;
	            }
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
	public void IntersectTrue ()
	{
		itemIntersect = true;
	}
	public void IntersectFalse ()
	{
		itemIntersect = false;
	}
}
