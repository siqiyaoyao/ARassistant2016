using UnityEngine;
using System.Collections;

public class ButtonManager : MonoBehaviour
{
    public GameObject tryButton;
    public GameObject previewButton;
    public GameObject blackPlane;

    private enum State { dressed, preview };
    private State currentState = State.dressed;

    public int whichCloth = 0;
    private enum ClothState { none, t_shirt, dress };

    public Transform t_shirt;
    Vector3 oriPosTShirt;//初始坐标
    Vector3 camPosTShirt = new Vector3(-24, -300, 1800);//相机下的模型坐标

    public Transform dress;
    Vector3 oriPosDress;
    Vector3 camPosDress = new Vector3(-24,-10, 2000);

    // Use this for initialization
	void Start () {
        
        oriPosTShirt = t_shirt.position;
        oriPosDress = dress.position;
	}
	
	// Update is called once per frame
	void Update () {
	    switch(currentState)
        {
            case State.dressed:

                // adjust the position of the clothes
                if (Input.GetMouseButton(0))
                {
                    if(whichCloth == (int)ClothState.t_shirt)
                        t_shirt.position += new Vector3(Input.GetAxis("Mouse X") * 10, 0f, Input.GetAxis("Mouse Y") * 10);
                    else if(whichCloth == (int)ClothState.dress)
                        dress.position += new Vector3(Input.GetAxis("Mouse X") * 10, 0f, Input.GetAxis("Mouse Y") * 10);
                }

                //adjust the size of the clothes
                if (Input.GetAxis("Mouse ScrollWheel") > 0f) //zomm in
                {
                    if (whichCloth == (int)ClothState.t_shirt)
                    {
                        //if (t_shirt.localScale.y < 30f)
                        //{
                            t_shirt.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                        //}
                    }
                    else if(whichCloth == (int)ClothState.dress)
                    {
                        Debug.Log(dress.localScale);
                        //if(dress.localScale.y < 5f)
                        //{
                            dress.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                        //}
                    }
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f) //zomm out
                {
                    if (whichCloth == (int)ClothState.t_shirt)
                    {
                       // if (t_shirt.localScale.y > 0.5f)
                        //{
                            t_shirt.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                        //}
                    }
                    else if(whichCloth == (int)ClothState.dress)
                    {
                        //if(dress.localScale.y > 1f)
                        //{
                            dress.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                        //}
                    }
                }
                
                break;
            case State.preview: 
                // rotate
                if (whichCloth == (int)ClothState.t_shirt)
                    t_shirt.transform.Rotate(0, 1, 0);
                else if (whichCloth == (int)ClothState.dress)
                    dress.transform.Rotate(0, 1, 0);
                break;
        }
	}
    public void Try()
    {
        blackPlane.SetActive(false);
        currentState = State.dressed;
        if (whichCloth == (int)ClothState.t_shirt)
        {
            t_shirt.parent = GameObject.Find("TShirtTarget").transform;
            t_shirt.position = oriPosTShirt;
            t_shirt.rotation = Quaternion.Euler(new Vector3(270, 0, 180));//让坐标重置原点
            t_shirt.localScale = new Vector3(19.5f, 19.5f, 19.5f);
        }
        else if(whichCloth == (int)ClothState.dress)
        {
            dress.parent = GameObject.Find("DressTarget").transform;
            dress.position = oriPosDress;
            dress.rotation = Quaternion.Euler(new Vector3(270, 0, 180));
            dress.localScale = new Vector3(3.4f, 3.4f, 3.4f);
        }
    }
    public void Preview()
    {
        blackPlane.SetActive(true);
        currentState = State.preview;
        if (whichCloth == (int)ClothState.t_shirt)
        {
            t_shirt.parent = Camera.main.transform; //把模型移到相機底下
            t_shirt.localPosition = camPosTShirt;
            t_shirt.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            t_shirt.localScale = new Vector3(7000f, 7000f, 7000f);
        }
        else if(whichCloth == (int)ClothState.dress)
        {
            dress.parent = Camera.main.transform;
            dress.localPosition = camPosDress;
            dress.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            dress.localScale = new Vector3(500f, 500f, 500f);
        }
    }

}
