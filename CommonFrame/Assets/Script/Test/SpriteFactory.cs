using UnityEngine;
using System.Collections;

using UnityEngine.UI ;



public class SpriteFactory : MonoBehaviour
{


	public  Object[] allSprit;




	// Use this for initialization
	void Start () {


        //	allSprit = Resources.LoadAll ("Number");
        LoadSprite("Number");

    }



	public   void  LoadSprite(string  name)
	{

		allSprit = Resources.LoadAll (name);
        for(int i = 0; i < allSprit.Length; i++)
        {
            GetSprite(i);
        }
		
	}

	public  GameObject  GetSprite(int  index)
	{

		GameObject tmpObj = new GameObject ("tmpGame");

		Image  tmpImage =   tmpObj.AddComponent <Image>();

		tmpImage.sprite = allSprit [index]  as Sprite;


		return  tmpObj;
	}




	//工厂 方法
	public  GameObject  GetImage(int index)
	{

		GameObject tmpObj = new GameObject ("tmpGame");

		Image  tmpImage =   tmpObj.AddComponent <Image>();

		tmpImage.sprite = allSprit [index]  as Sprite;


		return  tmpObj;
	}




	int  allIndex =0  ;

	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.A)) {
		

			allIndex++;

			GameObject tmpObj = GetImage (allIndex%9);

			tmpObj.transform.parent = transform;

			tmpObj.transform.position = new Vector3 (allIndex * 20, 20, 0);


		
		}
	}
}
