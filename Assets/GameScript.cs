using UnityEngine;
using System.Collections;
using System.Linq;

public class GameScript : MonoBehaviour
{

    private const float WALL_LEN = 2f;
    private const int LAYER_WALL = 8;
    private const int LAYER_MOVE = 9;
    private const float Y_ITEMOFFSET = 0.001f;
    private const int BLUE = 0;
    private const int RED = 1;
    private const int GREEN = 2;
    private const int WHITE = 3;

    private GameObject itemHeld;
    private Collider itemColider;
    private bool holdingItem = false;
    private bool colorSet = false;
    private bool itemIntersect = false;
    private Color setter;

    public GameObject Wall1;
    private MeshRenderer W1mesh;
    public GameObject Wall2;
    private MeshRenderer W2mesh;
    public GameObject Wall3;
    private MeshRenderer W3mesh;
    public GameObject Wall4;
    private MeshRenderer W4mesh;

    bool[] winFlags = { false };
    int[][] winCond = new int[][]
    {
        new int[] {0,1,1,0}
    };
    // Use this for initialization
    void Start()
    {
        W1mesh = Wall1.GetComponent<MeshRenderer>();
        W2mesh = Wall2.GetComponent<MeshRenderer>();
        W3mesh = Wall3.GetComponent<MeshRenderer>();
        W4mesh = Wall4.GetComponent<MeshRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (holdingItem)
        {
            Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, (1 << LAYER_WALL));
            if (hit.transform)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (itemIntersect == false)
                    {
                        float y = hit.transform.rotation.eulerAngles.y;
                        Vector3 point = itemHeld.transform.position;
                        print(y);
                        if (y > -1 && y < 1)
                            point.z = point.z + 0.01f;
                        if (y > 89 && y < 91)
                            point.x = point.x + 0.01f;
                        if (y > 179 && y < 181)
                            point.z = point.z - 0.01f;
                        if (y > 269 && y < 271)
                            point.x = point.x - 0.01f;

                        itemHeld.transform.position = point;
                        itemHeld.layer = LAYER_MOVE;
                        itemHeld.GetComponent<ItemScript>().removeHolder();
                        holdingItem = false;
                        itemHeld = null;
                    }
                    CheckWin();
                }
                else
                {
                    Vector3 point = hit.point;
                    //Bad solution
                    float y = hit.transform.rotation.eulerAngles.y;
                    print(y);
                    if (y > -1 && y < 1)
                        point.z = point.z - 0.01f;
                    if (y > 89 && y < 91)
                        point.x = point.x - 0.01f;
                    if (y > 179 && y < 181)
                        point.z = point.z + 0.01f;
                    if (y > 269 && y < 271)
                        point.x = point.x + 0.01f;

                    //end of bad solution
                    itemHeld.transform.rotation = hit.transform.rotation;
                    //Weird and long way to make sure the item held doesn't go half outside the walls (snaps to the edges)
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
        else //not holding item
        {
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
                            CheckWin();
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
    public void IntersectTrue()
    {
        itemIntersect = true;
    }
    public void IntersectFalse()
    {
        itemIntersect = false;
    }

	private int getColorNum(MeshRenderer mesh)
	{
		Color col = W1mesh.material.color;
		if (col.b == 1 && col.g != 1)
			return BLUE;
		else if (col.r == 1 && col.g != 1)
			return RED;
		else if (col.g == 1 && col.r != 1)
			return GREEN;
        else if (col.r == 1 && col.g == 1 && col.b == 1)
            return WHITE;
        else
            return 666;
	}

	public void CheckWin()
    {
		int[] currentCol = {getColorNum(W1mesh), getColorNum(W2mesh), getColorNum(W3mesh), getColorNum(W4mesh)};
		if(winFlags[0])
		{
			int blue = winCond[0][BLUE];
			int red = winCond[0][RED];
			int green = winCond[0][GREEN];
			int white = winCond[0][WHITE];

			for (int i = 0; i < currentCol.Length; i++) 
			{
				if(currentCol[i] == BLUE)
					blue --;
				else if (currentCol[i] == RED)
					red --;
				else if (currentCol[i] == GREEN)
					green --;
				else if (currentCol[i] == WHITE)
					white --;
				if (blue + red + green  + white == 0)
				{
					print("Win");
				}
			}
		}
		else
		{
			if(Enumerable.SequenceEqual(currentCol, winCond[0]))
			{
				print("win");
			}
		}
    }
}