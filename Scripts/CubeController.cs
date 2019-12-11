using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public int myX, myY;
    GameController myGameController;
    public bool active = false;
    public bool nextCube = false;

    // Start is called before the first frame update
    // Start is called before the first frame update
    void Start()
    {
        myGameController = GameObject.Find("GameControllerObject").GetComponent<GameController>();
    }//find object in scene called game controller, store game controller in my game controller

    // Update is called once per frame
    void Update()
    {

    }
    void OnMouseDown()
    {
        if (!nextCube)
        {
            myGameController.ProcessClick(gameObject, myX, myY, gameObject.GetComponent<Renderer>().material.color, active);
        }
    }
}