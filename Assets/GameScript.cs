using UnityEngine;
using UnityEngine.UI;
using Gvr;
using System.Collections;
using System.Linq;

public class GameScript : MonoBehaviour
{

    private const float WALL_LEN = 2f;
	private const float WALL_HEIGHT = WALL_LEN;
    private const int LAYER_WALL = 8;
    private const int LAYER_MOVE = 9;
	private const int LAYER_BOARD = 10;
    private const float Y_ITEMOFFSET = 0.001f;
    private const int BLUE = 0;
    private const int RED = 1;
    private const int GREEN = 2;
    private const int WHITE = 3;
    private const int NUMBER_OF_COLORS = 4;
    private const float ALMOST_ONE = 0.99999999999999999999999999999999999999999999999999f;
	private const float OFFSET_FROM_WALL = 0.1f;
	private const float POINT_DEDUCTION = 0.9f;
	private const int MAX_DOUBLEABLE = 4;
	private const int NUM_SPECIAL_CASES = 4;
	private const int EQUAL = 0;
	private const int DIVIDE = 1;
	private const int DOUBLE = 2;
	private const int SETAMOUNT = 3;
    private const float UNSTICKER = 0.001f;
	

    private GameObject itemHeld;
    private Collider itemColider;
    private bool holdingItem = false;
    private bool colorSet = false;
    private bool itemIntersect = false;
    private Color setter;
    private string instructionText;
    private int level = 1;
    private bool multiplay = false;
    private bool placer = false;
	private bool startPlacer = false;
	private int levelScore;
	private int totalScore = 0;

	public GvrAudioSource PlaceAudio;
	public GvrAudioSource GrabAudio;
	public GvrAudioSource CantPlaceAudio;
	public GvrAudioSource MenuAudio;
	public GvrAudioSource VictoryAudio;
	public GvrAudioSource NotRightAudio;

    public GameScript script;
	public Text levelText;
	public Text pointsText;
	public GameObject buttonPrefab;
	public RectTransform ObjectPanel;
	public GameObject room;
	public GameObject singMultiMenu;
	public GameObject roleMenu;
	public GameObject seedMenu;
    public GameObject[] walls = new GameObject[4];
	private MeshRenderer[] meshes;
	public GameObject[] items;
    public Text[] buttons = new Text[3];
    public Button dropItemButton;
         

    private bool[] totalAmountFlags = {false, false};
	private int[][] winCond = new int[][] 
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
        /*buttons[0].enabled = false;
        buttons[1].enabled = false;
        buttons[2].enabled = false;
        */
		room.SetActive(false);

		buttonTest();
        Mute = false;
        ChangeSound(Mute);

        meshes = new MeshRenderer[walls.Length];
		for(int i = 0; i < walls.Length; i++)
		{
			meshes[i] = walls[i].GetComponent<MeshRenderer>();
		}
    }

    // Update is called once per frame
    void Update()
    {
		if(!(multiplay && !placer))
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
							Vector3 point = hit.transform.InverseTransformDirection(itemHeld.transform.position);
							point.z = point.z + OFFSET_FROM_WALL;
							point = hit.transform.TransformDirection(point);

							itemHeld.transform.parent = hit.transform;
	                        itemHeld.transform.position = point;
	                        itemHeld.layer = LAYER_MOVE;
	                        itemHeld.GetComponent<ItemScript>().removeHolder();
	                        holdingItem = false;
	                        itemHeld = null;

							PlaceAudio.transform.position = point;
							PlaceAudio.Play();
	                    }
						else
						{
							CantPlaceAudio.transform.position = hit.point;
							CantPlaceAudio.Play();
						}
	                }
	                else //If not clicking
	                {
	                    Vector3 point = hit.point;
						Vector3 localPoint = hit.transform.InverseTransformDirection(point);
						localPoint.z = localPoint.z - OFFSET_FROM_WALL;

	                    itemHeld.transform.rotation = hit.transform.rotation;

						//To make item stick to the edges
						Vector3 localExtent = hit.transform.InverseTransformDirection(itemColider.bounds.extents);
						localExtent.x = Mathf.Abs(localExtent.x);

						if(localPoint.x > WALL_LEN - localExtent.x)
							localPoint.x = WALL_LEN - (localExtent.x + UNSTICKER);
						else if(localPoint.x < -WALL_LEN + localExtent.x)
							localPoint.x = -WALL_LEN + (localExtent.x + UNSTICKER);
						if(localPoint.y > WALL_HEIGHT - localExtent.y)
							localPoint.y = WALL_HEIGHT - (localExtent.y + UNSTICKER); 
						else if(localPoint.y < 0 + localExtent.y)
							localPoint.y = 0 + (localExtent.y + UNSTICKER);
						itemHeld.transform.position = hit.transform.TransformDirection(localPoint);

	                    
					} // end of If not clicking
	            }
	        }
	        else //not holding item
	        {
	            if (Input.GetMouseButtonDown(0))
	            {
	                /*instruction = Random.Range(0, 5);
	                wallColor = Random.Range(1, 3);
	                if (instruction > 0 && wallColor == 1)
	                    text.text = "Måla " + instruction + "väggar " + wallColor;
					*/
					Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, (1 << LAYER_WALL) + (1 << LAYER_MOVE) + (1 << LAYER_BOARD));
	                if (hit.transform)
	                {
	                    if (!holdingItem)
	                    {
	                        if (colorSet && hit.transform.tag == "Paintable")
	                        {
	                            MeshRenderer mesh = hit.collider.GetComponent<MeshRenderer>();
	                            mesh.material.color = setter;
	                        }
	                        else if (hit.transform.tag == "Moveable")
	                        {
	                            holdingItem = true;
	                            itemHeld = hit.transform.gameObject;
	                            itemColider = itemHeld.GetComponent<Collider>();
	                            itemHeld.layer = 0;
	                            itemHeld.GetComponent<ItemScript>().setHolder(this);
								itemHeld.transform.parent = null;
								GrabAudio.transform.position = hit.point;
								GrabAudio.Play();
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

		bool correctItems = true; //negate if fault found

		for (int i = 1; i < winCond.Length && correctItems && correctCol; i++)
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
			Victory();
		}
		else
		{
			levelScore = Mathf.FloorToInt(levelScore * POINT_DEDUCTION);
			NotRightAudio.Play();
            for (int i = 0; i < winCond.Length; i++)
            {
                string pr = i + ": ";
                for (int j = 0; j < winCond[i].Length; j++)
                    pr += winCond[i][j] +", ";
                print(pr);
            }
		}
    }

	private void clearLevel()
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

	}

	private void Victory()
	{
		
		VictoryAudio.Play();
        if (level == 4)
        {
            EndGame();
        }
        else
        {
            levelUp(level + 1);
            int change = 1;
            if (multiplay)
                change = (placer ? 2 : 0);

            totalScore += levelScore * change;
            pointsText.text = "Poäng: " + totalScore;

        }
    }

    private void newInstructions()
    {
        levelScore = 1000 + 400 * (level - 1);
        instructionText = "";
        //function for changing the instructions.
        int numItems = 0;
        if (level == 1)
        {
            numItems = 1;
        }
        else 
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
            for (int i = 0; i < winCond[0].Length; i++)
            {
                int colNumb = winCond[0][i];
                if (colNumb > 0)
                {
                    string colName = getColName(i);
                    instructionText = instructionText + "Vi vill ha " + colNumb + " " + colName + "a väggar \n";
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
                instructionText = instructionText + "Vägg " + (i + 1) + " ska vara " + colName + " \n";

            }
        }

        for (int i = 1; i < numItems + 1; i++)
        {
            float chance = Random.Range(0, 3 + level);
            bool specialCase = chance > 2f;
            if (specialCase && i > 1)
            {
                string caseStr = "";
                string putWhere = "";
                string takeWhere = "";
                int takeIndex = 0;
                int putIndex;
                int index = Mathf.FloorToInt(Random.Range(1, i - 1 + ALMOST_ONE));

                if (totalAmountFlags[index])
                {
                    takeWhere = " totalt";
                }
                else
                {
                    for (int j = 0; j < winCond[index].Length; j++)
                    {
                        if (winCond[index][j] != 0)
                        {
                            takeIndex = j;
                            break;
                        }
                    }
                    takeWhere = " på vägg " + takeIndex;
                }
                int wallIndex;
                bool totalAmount = Random.value > 0.5f;
                if (totalAmount)
                {
                    putWhere = "totalt";
                    winCond[i] = new int[1];
                    putIndex = 0;
                }
                else
                {
                    wallIndex = Mathf.FloorToInt(Random.Range(0, walls.Length - 1 + ALMOST_ONE));
                    putWhere = " på vägg " + (wallIndex+1);
                    winCond[i] = new int[4];
                    for (int j = 0; j < winCond[i].Length; j++)
                        winCond[i][j] = 0;
                    putIndex = wallIndex;
                }
                int determiner = Mathf.FloorToInt(Random.Range(0, NUM_SPECIAL_CASES - 1 + ALMOST_ONE));


                int amount = winCond[index][takeIndex];

                bool set = false;
                while (!set)                                //The editor is fucking up the indentation :((( cba to fight it.
                    switch (determiner)
                    {
                        case EQUAL:
                            winCond[i][putIndex] = winCond[index][takeIndex];
                            caseStr = "lika många " + items[i - 1].name + " som " + items[index - 1].name;
                            set = true;
                            break;

                        case DIVIDE:
                            if (amount % 2 == 0 && amount >= 2)
                            {
                                winCond[i][putIndex] = winCond[index][takeIndex] / 2;

                                caseStr = "hälften så många " + items[i - 1].name + " som " + items[index - 1].name;
                                set = true;
                            }
                            else if (amount < MAX_DOUBLEABLE)
                                determiner = DOUBLE;
                            else
                                determiner = (Random.value < 0.5f ? EQUAL : SETAMOUNT);
                            break;

                        case DOUBLE:
                            if (amount <= MAX_DOUBLEABLE)
                            {
                                winCond[i][putIndex] = winCond[index][takeIndex] * 2;
                                caseStr = "dubbelt så många " + items[i - 1].name + " som " + items[index - 1].name;
                                set = true;
                            }
                            else if (amount % 2 == 0)
                                determiner = DIVIDE;
                            else
                                determiner = (Random.value < 0.5f ? EQUAL : SETAMOUNT);
                            break;
                        case SETAMOUNT:
                            bool positive = Random.value < 0.5f;
                            int maxChange = positive ? 3 : amount;
                            int change = Mathf.FloorToInt(Random.Range(0, maxChange + ALMOST_ONE));
                            winCond[i][putIndex] = positive ? winCond[index][takeIndex] + change : winCond[index][takeIndex] - change;
                            caseStr = change + (positive ? " fler " : " färre ") + items[i - 1].name + " än " + items[index - 1].name;
                            set = true;
                            break;
                    }

                instructionText = instructionText + "Vi vill " + putWhere + " ha " + caseStr + takeWhere + "\n";
            }
            else
            {
                totalAmountFlags[i] = true;
                int num = Mathf.FloorToInt(Random.Range(1, 5));
                winCond[i] = new int[1] { num };
                instructionText = instructionText + "Vi vill ha " + num + " " + items[i - 1].name + "\n";
            }
        }
        print(instructionText);
        if (multiplay)
        {
            multiplayPopulate();
            if (placer)
                instructionText = "Du skall placera objekt som din vän säger åt dig";
            else
            {
                instructionText = "Du skall berätta för din vän hur rummet ser ut\n\nTips:\n";
                if (totalAmountFlags[0])
                    instructionText = instructionText + "Färger: Specifik vägg\n";
                else
                    instructionText = instructionText + "Färger: Totalt\n";
                for (int i = 1; i < totalAmountFlags.Length; i++)
                    instructionText = instructionText + items[i - 1].name + ": " + (totalAmountFlags[i] ? "Totalt" : "Specifik vägg") + "\n";
            }
        }
        setNewInstructions(instructionText);

    }


    private string getColName(int i)
    {
        switch (i)
        {
            case BLUE:
                return "blå";
            case RED:
                return "röd";
            case GREEN:
                return "grön";
            case WHITE:
                return "vit";
            default:
                return "If you see this, something is wrong.";
        }
    }
	private Color getColor(int i)
	{
		switch(i)
		{
		case BLUE:
			return new Color(0, 0, 1);
		case RED: 
			return new Color(1, 0, 0);
		case GREEN:
			return new Color(0, 1, 0);
		case WHITE:
			return new Color(1, 1, 1);
		default:
			throw new UnityException ();	
		}
	}

	//Tries to place an item on a wall 5 times, before giving up and reducing win condition instead. If no wall is given, one will be randomized.
	private void placeItem(int i, int wallIndex = -1)	
	{
		bool wallSet = wallIndex != -1;
		GameObject wall;
		GameObject item = Instantiate(items[i-1]);
		Collider collider = item.GetComponent<Collider>();
		bool set = false;
		int failiours = 0;
		while(!set)
		{
			float sideway = Random.Range(-WALL_LEN + collider.bounds.extents.x, WALL_LEN - collider.bounds.extents.x);
			float up = Random.Range(0 + collider.bounds.extents.y, WALL_HEIGHT - collider.bounds.extents.y);
			if(!wallSet)
				wall = walls[Mathf.FloorToInt(Random.Range(0, walls.Length - 1 + ALMOST_ONE))];
			else
				wall = walls[wallIndex];		//If wall is known we don't actually have to set it each loop, but it makes for a neater if statement (that the compiler doens't complain about)
			item.transform.rotation = wall.transform.rotation;
			Vector3 move = wall.transform.TransformDirection(sideway, up, WALL_LEN);

			if(!Physics.CheckBox(move, collider.bounds.extents, item.transform.rotation, (1<<LAYER_MOVE) + (1<<LAYER_BOARD))) //Kinda works....
			{
				set = true;
				item.transform.position = move;
				item.transform.parent = wall.transform;
			}

			else if(failiours > 5)
			{
				set = true;
				Destroy(item);
				int conditionIndex = (!wallSet ? 0 : wallIndex);
				winCond[i][conditionIndex] = winCond[i][conditionIndex] - 1;
			}
			else
				failiours ++;
		}
	}

	private void multiplayPopulate()
	{
		if(totalAmountFlags[0])
        {
            int[] cols = (int[]) winCond[0].Clone();
            foreach (MeshRenderer mesh in meshes)
			{
				bool set = false;
				while(!set)
				{
					int index = Mathf.FloorToInt(Random.Range(0, NUMBER_OF_COLORS-1+ALMOST_ONE));
					if(cols[index] > 0)
					{
						cols[index] = cols[index] -1;
                        mesh.material.color = getColor(index);
						set = true;
					}
				}
			}
        }
        else
		{
			for(int i = 0; i < winCond[0].Length; i++)
			{
				meshes[i].material.color = getColor(winCond[0][i]);
			}
		}
        for (int i = 1; i < winCond.Length; i++)
		{
			if(totalAmountFlags[i])
			{
				for (int count = winCond[i][0]; count > 0; count--)
				{
					placeItem(i);
				}
			}
			else
			{
				for (int wallIndex = 0; wallIndex < winCond[i].Length; wallIndex ++)
				{
					for (int count = winCond[i][wallIndex]; count > 0; count--)
					{
						placeItem(i, wallIndex);
					}
				}
			}
            if (placer)
			{
				clearLevel();
            }
        }
	}

    private void EndGame()
    {

    }

	public void initReady()
	{
		newInstructions();
		room.SetActive(true);
    }

	public void multiplayerMode(bool enabled)
	{
		multiplay = enabled;
		singMultiMenu.SetActive(false);
		if(multiplay)
			roleMenu.SetActive(true);
		else
			initReady();
	}
	public void multiplayerRole(bool isPlacer)
	{
		this.placer = isPlacer;
		startPlacer = isPlacer;
		roleMenu.SetActive(false);
		seedMenu.SetActive(true);
	}
	public void seed(int seed)
	{
		Random.InitState(seed);
		seedMenu.SetActive(false);
		initReady();
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
	public void levelUp(int num)
	{
		clearLevel();
		
		level = num;
		levelText.text = "Nivå: " + level;
		placer = (num % 2 == 0) ^ startPlacer;  //Switch roles every round (Beware! Functionality overlap between levelUp and newInstructions regarding placers and objects in scene. Cba to fix :)
        newInstructions();                      //kinda doesn't matter now tho :D
    }

    public void setNewInstructions(string winInstructrions)
	{
		instr.text = winInstructrions;
	}

	public void menuSound()
	{
		if(!Mute)
			MenuAudio.Play();
	}

	public void ChangeSound(bool Mute)
	{
		if (Mute == false)
		{
			// AudioListener.pause = false;
			AudioText.text = "Ljud På";
			AudioListener.volume = 1;
		}
		else
		{
			// AudioListener.pause = true;
			AudioText.text = "Ljud Av";
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

    public void dropItem()
    {
        if (holdingItem)
        {
            holdingItem = false;
            Destroy(itemHeld);
            itemHeld = null;
        }
    }

	private void buttonTest()
	{
        //int width = 200;
        //int height = 50;
        //int distance = -200;
        int posY = 325;
		int maxColumn = 2;
		int maxRows = 6;
		int columns = 1;
		bool set = false;
		int workingLength = items.Length;
		while (!set)
		{
			if(workingLength > maxRows * maxColumn)
			{
				workingLength = maxRows * maxColumn;
			}
			if ( columns == 1)
			{
				columns = 4;
				set = true;
			}
			else if(workingLength % columns == 0 && workingLength/columns <= maxRows)
			{
				set = true;
			}
			else
			{
				columns --;
			}
		}
		for(int i = 0; i < workingLength; i++)
		{
           
                Vector2 pos = new Vector2(0, posY=posY-100);
                //pos.x = pos.x + (width + distance/2) * (i % columns);
                //pos.y = pos.y - (height + distance/2) * Mathf.Floor((i / columns)) ;
                GameObject button = Instantiate(buttonPrefab);
                RectTransform rect = button.GetComponent<RectTransform>();
                Text text = button.GetComponent<Text>();
                text.text = items[i].name;
                button.transform.SetParent(ObjectPanel, false);
                rect.anchoredPosition = pos;

                int tmpInt = i;
                Button btn = button.GetComponent<Button>();
                btn.onClick.AddListener(() => buttonClick(tmpInt));
		}
	}

	private void buttonClick(int i)
	{
		if(!holdingItem)
		{
			holdingItem = true;
			itemHeld = Instantiate(items[i]);
			itemColider = itemHeld.GetComponent<Collider>();
			itemHeld.tag = "Moveable";
			itemHeld.layer = 0;
			itemHeld.GetComponent<ItemScript>().setHolder(this);
			itemHeld.transform.parent = null;
			GrabAudio.Play();
		}
    }
}