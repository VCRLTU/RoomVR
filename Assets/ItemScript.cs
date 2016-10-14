using UnityEngine;
using System.Collections;

public class ItemScript : MonoBehaviour {

	GameScript holder;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}



	public void setHolder( GameScript gs)
	{
		holder = gs;
	}

	public void removeHolder ()
	{
		holder = null;
	}

	void OnTriggerEnter() 
	{
		holder.IntersectTrue();
	}

	void OnTriggerExit() 
	{
		holder.IntersectFalse();	
	}
}