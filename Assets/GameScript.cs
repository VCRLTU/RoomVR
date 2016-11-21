using UnityEngine;
using UnityEngine.UI;
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
    private const int NUMBER_OF_COLORS = 4;
    private const float ALMOST_ONE = 0.99999999999999999999999999999999999999999999999999f;
	

    private GameObject itemHeld;
    private Collider itemColider;
    private bool holdingItem = false;
    private bool colorSet = false;
    private bool itemIntersect = false;
    private Color setter;
    string instructionText;
    private int level = 2;


    public GameObject[] walls = new GameObject[4];
	private MeshRenderer[] meshes;
	public GameObject[] items;

	bool[] totalAmountFlags = {false, false};
	int[][] winCond = new int[][] 
	{
		new int[] {BLUE, RED , RED , BLUE},
		new int[] {1, 0, 0, 0}
	};

    public Text instr;
    public Text AudioText;
    public TextMesh text;
    private bool Mute;
	int instruction = 0;
	int wallColor = 0;

    // Use this for initialization
    void Start()
    {
        Mute = false;
        ChangeSound(Mute);

        meshes = new MeshRenderer[walls.Length];
		for(int i = 0; i < walls.Length; i++)
		{
			meshes[i] = walls[i].GetComponent<MeshRenderer>();
		}
        newInstructions();
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
                        if (y > -1 && y < 1)
                            point.z = point.z + 0.01f;
                        if (y > 89 && y < 91)
                            point.x = point.x + 0.01f;
                        if (y > 179 && y < 181)
                            point.z = point.z - 0.01f;
                        if (y > 269 && y < 271)
                            point.x = point.x - 0.01f;

						itemHeld.transform.parent = hit.transform;
                        itemHeld.transform.position = point;
                        itemHeld.layer = LAYER_MOVE;
                        itemHeld.GetComponent<ItemScript>().removeHolder();
                        holdingItem = false;
                        itemHeld = null;
                    }
                    CheckWin();
                }
                else //If not clicking
                {
                    Vector3 point = hit.point;
                    //Bad solution
                    float y = hit.transform.rotation.eulerAngles.y;
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
                instruction = Random.Range(0, 5);
                wallColor = Random.Range(1, 3);
                if (instruction > 0 && wallColor == 1)
                    text.text = "Måla " + instruction + "väggar " + wallColor;

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
							itemHeld.transform.parent = null;
                        }
                        else if (hit.transform.tag == "Dupe") //Placeholder
                        {
                            holdingItem = true;
                            itemHeld = Instantiate(hit.collider.gameObject);
                            itemColider = itemHeld.GetComponent<Collider>();
                            itemHeld.tag = "Moveable";
                            itemHeld.layer = 0;
                            itemHeld.GetComponent<ItemScript>().setHolder(this);
							itemHeld.transform.parent = null;
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

	public void CheckWin()
    {
		bool correctCol = false;
		int[] currentCol = new int[meshes.Length];

		for (int i = 0; i < meshes.Length; i++)
		{
			Color col = meshes[i].material.color;
			if (col.b == 1 && col.g != 1)
				currentCol[i] = BLUE;
			else if (col.r == 1 && col.g != 1)
				currentCol[i] = RED;
			else if (col.g == 1 && col.r != 1)
				currentCol[i] = GREEN;
			else if (col.r == 1 && col.g == 1 && col.b == 1)
				currentCol[i] = WHITE;
			else 
				throw new UnityException();
		}
		if(totalAmountFlags[0])
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
					correctCol = true;
				}
			}
		}
		else
		{
			if(Enumerable.SequenceEqual(currentCol, winCond[0]))
			{
				correctCol = true;
			}
		}

		//if(correctCol) 	// If colours aren't correct, we don't need to check anything else. But commented for now for debugging
		//{
			bool correctItems = true; //negate if fault found

			for (int i = 1; i < winCond.Length && correctItems; i++)
			{
				if(totalAmountFlags[i]) //If we're only looking at totals
				{
					int amount = winCond[i][0];
					foreach (GameObject wall in walls)
					{
						foreach (Transform child in wall.transform)
						{
							if(child.name == items[i-1].name + "(Clone)")
								amount --;
						}
					}
					if(amount != 0)
						correctItems = false;
				}
				else  //If looking at specific amount of items at each wall
				{
					for (int j = 0; j < winCond[i].Length && correctItems; j++)
					{
						int amount = winCond[i][j];
						foreach (Transform child in walls[j].transform)
						{
							if(child.name == items[i-1].name + "(Clone)")
								amount --;
						}
						if(amount != 0)
							correctItems = false;
					}
				}
			}
			if (correctCol && correctItems)
			{
				print("Win");
				Victory();
			}
			else
			{
				print("not win " + correctCol.ToString() + " and " + correctItems.ToString());
			}
		//}
    }

	private void Victory()
	{
		foreach (GameObject wall in walls)
		{
			foreach (Transform child in wall.transform)
			{
				Destroy(child.gameObject);
			}
		}

		Color white = new Color(1,1,1);

		foreach (MeshRenderer mesh in meshes)
		{
			mesh.material.color = white;
		}
		text.text = "Du vinner!";
		levelUp();
		newInstructions();
	}

	private void levelUp()
	{
		//function for leveling up. Should probably be merged with Victory, but good for showing the concept right now
	}

    public void setNewInstructions(string winInstructrions)
    {
        instr.text = winInstructrions;
    }

    public void ChangeSound(bool Mute)
    {
        if (Mute == false)
        {
            // AudioListener.pause = false;
            AudioText.text = "Audio On";
            AudioListener.volume = 1;
        }
        else
        {
            // AudioListener.pause = true;
            AudioText.text = "Audio Off";
            AudioListener.volume = 0;
        }
    }

    public void toggleSound()
    {
        if (Mute == true)
        {
            Mute = false;
            ChangeSound(Mute);
        }
        else
        {
            Mute = true;
            ChangeSound(Mute);
        }
    }

    private void newInstructions()
    {
        //function for changing the instructions.
        int numItems = 0;
        if (level == 1)
        {
            numItems = 1;
        }
        else if (level == 2)
        {
            numItems = Mathf.FloorToInt(Random.Range(1f, items.Length + ALMOST_ONE));
        }

        totalAmountFlags = new bool[numItems + 1];
        winCond = new int[numItems + 1][];

        bool totalColor = Random.Range(0f, 2f) > 1f;
        totalAmountFlags[0] = totalColor;
        winCond[0] = new int[4];
        if (totalColor)
        {
            for (int i = 0; i < 4; i++)
            {
                int index = Mathf.FloorToInt(Random.Range(0f, NUMBER_OF_COLORS - 1 + ALMOST_ONE));
                winCond[0][index] = winCond[0][index] + 1;
            }
            string colText = "";
            for (int i = 0; i < winCond[0].Length; i++)
            {
                int colNumb = winCond[0][i];
                if (colNumb > 0)
                {
                    string colName = getColName(i);
                    instructionText = instructionText + "Vi vill ha " + colNumb + " " + colName + "a väggar. \n";
                }
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                int colour = Mathf.FloorToInt(Random.Range(0f, NUMBER_OF_COLORS - 1 + ALMOST_ONE));
                winCond[0][i] = colour;
                string colName = getColName(colour);
                instructionText = instructionText + "Vägg " + (i + 1) + " ska vara " + colName + ". \n";
            }
        }

        for (int i = 1; i < numItems + 1; i++)
        {
            totalAmountFlags[i] = true;
            //bool specialCase = Random.Range(0f, 2f) > 1f;
            //if (specialCase)
            //{
            int num = Mathf.FloorToInt(Random.Range(1, 10));
            winCond[i] = new int[1] { num };
            instructionText = instructionText + "Vi vill ha " + num + " " + items[i - 1].name + "\n";
            //}
        }
        print(instructionText);
        setNewInstructions(instructionText);
    }

    private string getColName(int i)
    {
        switch (i)
        {
            case BLUE:
                return "blå";
                break;
            case RED:
                return "röd";
                break;
            case GREEN:
                return "grön";
                break;
            case WHITE:
                return "vit";
                break;
            default:
                return "YOU FUCKED UP SON";
                break;
        }
    }
}