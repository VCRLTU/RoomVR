using UnityEngine;
using System.Collections;

public class ItemScript : MonoBehaviour {

	GameScript holder = null;
	bool triggered = false;
	int trigAmount = 0;


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
		trigAmount ++;
		triggered = true;
		if(holder)
			holder.IntersectTrue();
	}

	void OnTriggerExit() 
	{
		trigAmount --;
		if(!(trigAmount > 0))
		{
			triggered = false;
			if(holder)
				holder.IntersectFalse();	
		}
	}

	public bool isTriggered()
	{
		return triggered;
	}
}