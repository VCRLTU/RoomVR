using UnityEngine;
using System.Collections;

public class ItemScript : MonoBehaviour {

	GameScript holder = null;


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
		if(holder)
			holder.IntersectTrue();
	}

	void OnTriggerExit() 
	{
		if(holder)
			holder.IntersectFalse();	
	}
}