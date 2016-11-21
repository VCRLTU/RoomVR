using UnityEngine;
using System.Collections;

public class ItemScript : MonoBehaviour {

	GameScript holder = null;
	bool triggered = false;


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
		triggered = true;
		if(holder)
			holder.IntersectTrue();
	}

	void OnTriggerExit() 
	{
		triggered = false;
		if(holder)
			holder.IntersectFalse();	
	}
	public bool isTriggered()
	{
		return triggered;
	}
}