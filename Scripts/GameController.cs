using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    float Timer = 60f;  //timer for the length of the game
    int gridX = 8; // represent the grid of cubes
    int gridY = 5;
    float turnDuration = 2.0f; //how many seconds pass before the new turn
    int NextTurn = 0; // number of turns
    GameObject[,] cubeGrid; //2d array, a collection of game objects
    GameObject NextCube;
    Color[] MyColor = { Color.blue, Color.red, Color.green, Color.yellow, Color.magenta }; // an array for the color randomization
    bool EndOfGame; //conditions for winning for losing
    Vector3 cubePosition;
    int score = 10;
    GameObject activeCube = null;
    int rainbowPoints = 5;
    int monoPoints = 0;
    public Text ScoreText;
    public Text nextCubeText;
    public Text timerText;
    public GameObject cubePrefab;
    bool gameOver = false;

    void CreateGrid()
    {
        cubeGrid = new GameObject[gridX, gridY];

        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                cubePosition = new Vector3(x * 2, y * 2, 0);
                cubeGrid[x, y] = Instantiate(cubePrefab, cubePosition, Quaternion.identity);//make cube, tell grid to store it in array
                cubeGrid[x, y].GetComponent<CubeController>().myX = x;
                cubeGrid[x, y].GetComponent<CubeController>().myY = y;
            }
        }
    }
     
    void GenerateNextCube()//  generates a random colored cube in the Next Cube position
    {
        if(NextCube == null)//there is no cube in the next cube area
        {
            NextCube = Instantiate(cubePrefab,  new Vector3(18, 4, 0), Quaternion.identity); //new cube in next cube area
            NextCube.GetComponent<Renderer>().material.color = MyColor[Random.Range(0, MyColor.Length)];
            NextCube.GetComponent<CubeController>().nextCube = true;
        }
    }

    void EndGame (bool win)
    { 
        if (win)
        {
            nextCubeText.text = "You Win!" +"\n" + "YEEHAWW!!!";
        }
        else
        {
            nextCubeText.text = "You lose" + "\n" + "wanna try again?";
        }
        Destroy(NextCube);//just in case the next cube still exists
        NextCube = null; // process keyboard input doesnt work if nextcube is null

        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                cubeGrid[x, y].GetComponent<CubeController>().nextCube = true;// make all cubes think they are next cube which are now all disabled           }
            }   
        }
        
    }

    void SetCubeColor (GameObject myCube, Color color)
    {
        if (myCube == null)
        {
            gameOver = true;//no available Cubes 
        }
        else
        {//chosen cube is now NextCube color
            myCube.GetComponent<Renderer>().material.color = color;//set new color
            Destroy(NextCube);
            NextCube = null;
        }
    }

    GameObject ChooseWhiteCube(List<GameObject> whiteCubes)
    {
        if (whiteCubes.Count == 0)//no white cubes left in the row
        {
            return null;
        }
        return whiteCubes[Random.Range(0, whiteCubes.Count)];//random white cube is selected from the list
    }

    GameObject FindAvailableCube(int y)
    {
        List<GameObject> whiteCubes = new List<GameObject>();//a list of white cubes

        for (int x = 0; x < gridX; x++)
        {
            if (cubeGrid[x, y].GetComponent<Renderer>().material.color == Color.white)
            {
                whiteCubes.Add(cubeGrid[x, y]);
            }
        }
        return ChooseWhiteCube(whiteCubes);
    }
    GameObject FindAvailableCube()
    {
        List<GameObject> whiteCubes = new List<GameObject>();//a list of white cubes without the y 
        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                if (cubeGrid[x, y].GetComponent<Renderer>().material.color == Color.white)
                {
                    whiteCubes.Add(cubeGrid[x, y]);
                }
            }
        }
        return ChooseWhiteCube(whiteCubes);
    } 

    void PlaceNextCube(int y)
    {
        GameObject WhiteCube = FindAvailableCube(y);
        //theres an available cube
        SetCubeColor(WhiteCube, NextCube.GetComponent<Renderer>().material.color);
    } 

    void CheckKeyboardInput()
    {
        int numKeyPressed = 0;

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            numKeyPressed = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            numKeyPressed = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            numKeyPressed = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            numKeyPressed = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            numKeyPressed = 5;
        }
        if (NextCube != null && numKeyPressed != 0)//if we still have a cube, put it in the row you chose
        {
            PlaceNextCube(numKeyPressed - 1);//grid array based on 0 index, so its -1 to actually line up 
        }
    }

    void SpawnBlackCube()// if turn passes and no button was pressed, a random white cube turns black and the next cube is destroyed
    {
        GameObject WhiteCube = FindAvailableCube();
        SetCubeColor(WhiteCube, Color.black); //a white cube is set to black
    }

    public void ProcessClick(GameObject clickedCube, int x, int y, Color cubeColor, bool active)
    {
        if (cubeColor != Color.white && cubeColor != Color.black )
        {
            if (active)//this cube is already active
            {
                clickedCube.transform.localScale /= 1.5f;
                clickedCube.GetComponent<CubeController>().active = false;
                activeCube = null; //now it isnt active
            }
            else//not active
            {
                if (activeCube != null)
                {
                    activeCube.transform.localScale /= 1.5f;//deactive previously clicked cube
                    activeCube.GetComponent<CubeController>().active = false;
                }
                //active the new colored cube
                clickedCube.transform.localScale *= 1.5f;
                clickedCube.GetComponent<CubeController>().active = true;
                activeCube = clickedCube;
            }
        }

        else if (cubeColor == Color.white)//click on white cube
        {
            int xDistance = clickedCube.GetComponent<CubeController>().myX - activeCube.GetComponent<CubeController>().myX;//check the distance if active and clicked cube
            int yDistance = clickedCube.GetComponent<CubeController>().myY - activeCube.GetComponent<CubeController>().myY;

            if ( Mathf.Abs (yDistance) == 1 || Mathf.Abs (xDistance) == 1)//is the distance of the clicked cube 1 unit away in any direction, is it close
            {   //clicked cube is now the active cube
                clickedCube.GetComponent<Renderer>().material.color = activeCube.GetComponent<Renderer>().material.color;
                clickedCube.transform.localScale *= 1.5f;
                clickedCube.GetComponent<CubeController>().active = true;
                    
                //old active cube is now white
                activeCube.GetComponent<Renderer>().material.color = Color.white;
                activeCube.transform.localScale /= 1.5f;
                activeCube.GetComponent<CubeController>().active = false;

                activeCube = clickedCube;//this is last so we remember which one the original cube was and change it from active to clicked officially
            }
        }
    } 

   bool CheckRainbowPlus (int x, int y)
   {
        Color a = cubeGrid[x, y].GetComponent<Renderer>().material.color;
        Color b = cubeGrid[x+1, y].GetComponent<Renderer>().material.color;
        Color c = cubeGrid[x-1, y].GetComponent<Renderer>().material.color;
        Color d = cubeGrid[x, y+1].GetComponent<Renderer>().material.color;
        Color e = cubeGrid[x, y-1].GetComponent<Renderer>().material.color;

        if (a == Color.white || a == Color.black ||
            b == Color.white || b == Color.black ||
            c == Color.white || c == Color.black ||
            d == Color.white || d == Color.black ||
            e == Color.white || e == Color.black )
            //no rainbow plus with white or black cubes
        {
            return false;
        }

        if (a != b && a != c && a != d && a != e &&    //if every color is different
           b != c && b != d && b != e &&
           c != d && c != e &&
           d != e)
        {
            return true;
        }
        else
        {
            return false;
        }
   }

    bool CheckMonoPlus(int x, int y)
    {
        if(cubeGrid[x, y].GetComponent<Renderer>().material.color != Color.black &&
           cubeGrid[x, y].GetComponent<Renderer>().material.color != Color.white &&
           cubeGrid[x, y].GetComponent<Renderer>().material.color == cubeGrid[x + 1, y].GetComponent<Renderer>().material.color &&
           cubeGrid[x, y].GetComponent<Renderer>().material.color == cubeGrid[x - 1, y].GetComponent<Renderer>().material.color &&
           cubeGrid[x, y].GetComponent<Renderer>().material.color == cubeGrid[x, y + 1].GetComponent<Renderer>().material.color &&
           cubeGrid[x, y].GetComponent<Renderer>().material.color == cubeGrid[x, y - 1].GetComponent<Renderer>().material.color)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void MakeBlackPlus (int x, int y)
    {
        if (x == 0 || y == 0 || x == gridX - 1 || y == gridY - 1)//error check that the cubes are not out of bounds
        {
            return;// returns nothing, stop excecuting, the method is done
        }
        cubeGrid[x, y].GetComponent<Renderer>().material.color = Color.black;
        cubeGrid[x+1, y].GetComponent<Renderer>().material.color = Color.black;
        cubeGrid[x-1, y].GetComponent<Renderer>().material.color = Color.black;
        cubeGrid[x, y+1].GetComponent<Renderer>().material.color = Color.black;
        cubeGrid[x, y-1].GetComponent<Renderer>().material.color = Color.black;

        if (activeCube != null && activeCube.GetComponent <Renderer> (). material.color == Color.black)
        {  //if an acgive cube was invlolved in the plus, deactivea it
            activeCube.transform.localScale /= 1.5f;
            activeCube.GetComponent<CubeController>().active = false;
            activeCube = null;
        }
    }

    void Score()
    {
        for (int x = 1; x < gridX - 1; x++)
        {
            for( int y = 1; y < gridY - 1; y++)
            {
                if (CheckRainbowPlus(x, y))
                {
                    score += rainbowPoints;
                    MakeBlackPlus(x, y);
                }
                if (CheckMonoPlus(x, y))
                {
                    score += monoPoints;
                    MakeBlackPlus(x, y);
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
        GenerateNextCube();
    }

    // Update is called once per frame
    void Update()
    {
       
       if (gameOver)
       {
            if (score > 0)//game ends with a score more than 0
            {
                EndGame(true);
            }
            else
            {
                EndGame(false);
            }

       }
       else if (Timer < 0)
       {
            gameOver = true;
       }
       else if (Time.time < 60)//if there is time in the game keep playing
       {
            Timer -= Time.deltaTime;
            timerText.text = "Time Remaining: " + Timer.ToString("F2");
            CheckKeyboardInput();
            Score();
            if (Time.time > turnDuration * NextTurn)
            {
                NextTurn++;

                if (NextCube != null)
                {
                    score -= 1;
                    if (score < 0)
                    {
                        score = 0;
                    }
                    SpawnBlackCube();
                }
                GenerateNextCube();
            }
            ScoreText.text = "Score : " + score;
       }
       
    }
} 