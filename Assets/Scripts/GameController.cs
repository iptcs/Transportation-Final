using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject cubePrefab;
    Vector3 cubePosition;
    GameObject activeCube;
    public int gridX = 16, gridY = 9;
    int airplaneX, airplaneY, startX, startY;
    int depotX, depotY;
    GameObject[,] grid;
    bool airplaneActive;
    float turnLength, turnTimer;
    int airplaneCargo, airplaneCargoMax;
    int cargoGain;
    int score = 0;
    int moveY, moveX;
    int destinationX, destinationY;

    // Start is called before the first frame update
    void Start()
    {
        turnLength = 1.5f;
        turnTimer = turnLength;

        airplaneCargo = 0;
        airplaneCargoMax = 90;
        cargoGain = 10;

        grid = new GameObject[gridX, gridY];

        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                cubePosition = new Vector3(x * 2, y * 2, 0);
                grid[x,y] = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
                grid[x, y].GetComponent<CubeController>().myX = x;
                grid[x, y].GetComponent<CubeController>().myY = y;

            }
        }

        // airplane starts in upper left
        startX = 0;
        startY = gridY - 1;
        airplaneX = startX;
        airplaneY = startY;
        destinationX = airplaneX;
        destinationY = airplaneY;
        grid[airplaneX, airplaneY].GetComponent<Renderer>().material.color = Color.red;
        airplaneActive = false;
        depotX = gridX - 1;
        depotY = 0;
        grid[depotX, depotY].GetComponent<Renderer>().material.color = Color.black;

        moveX = 0;
        moveY = 0;
    }

    public void ProcessClick(GameObject clickedCube, int x, int y)
    {
        //print("Destination X is " + destinationX + " Detination Y is " + destinationY);
        if (x == airplaneX && y == airplaneY)
        {
            if (airplaneActive)
            {
                airplaneActive = false;
                clickedCube.transform.localScale /= 1.5f;
            }
            else
            {
                airplaneActive = true;
                clickedCube.transform.localScale *= 1.5f;
            }
        }

        else
        {
            if (airplaneActive)
            {
                destinationX = x;
                destinationY = y;
            }
        }
    }

    void DetectKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveY = -1;
            moveX = 0;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveY = 1;
            moveX = 0;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveX = 1;
            moveY = 0;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveX = -1;
            moveY = 0;
        }
    }

    void MoveAirplane()
    {
        if (airplaneActive)
        {
            // remove plane from old spot, turn depot black if needed
            if (airplaneX == depotX && airplaneY == depotY)
            {
                grid[depotX, depotY].GetComponent<Renderer>().material.color = Color.black;

            }
            else
            {
                grid[airplaneX, airplaneY].GetComponent<Renderer>().material.color = Color.white;
            }
            grid[airplaneX, airplaneY].transform.localScale /= 1.5f;


            // move plane to new spot
            airplaneX += moveX;
            airplaneY += moveY;
            if (airplaneX >= gridX)
            {
                airplaneX = gridX - 1;
            }
            else if (airplaneX < 0)
            {
                airplaneX = 0;
            }
            if (airplaneY >= gridY)
            {
                airplaneY = gridY - 1;
            }
            else if (airplaneY < 0)
            {
                airplaneY = 0;
            }
            grid[airplaneX, airplaneY].GetComponent<Renderer>().material.color = Color.red;
            grid[airplaneX, airplaneY].transform.localScale *= 1.5f;
            
        }
        moveX = 0;
        moveY = 0;
    }

    void SetDirection()
    {
        if (airplaneX > destinationX)
        {
            moveX = -1;
        }
        else if (airplaneX < destinationX)
        {
            moveX = 1;
        }
        if (airplaneY > destinationY)
        {
            moveY = -1;
        }
        else if (airplaneY < destinationY)
        {
            moveY = 1;
        }
    }

    void LoadCargo ()
    {
        if (airplaneX == startX && airplaneY == startY)
        {
            airplaneCargo += cargoGain;
            if (airplaneCargo > airplaneCargoMax)
            {
                airplaneCargo = airplaneCargoMax;
            }
        }
    }

    void DeliverCargo()
    {
        if (airplaneX == depotX && airplaneY == depotY)
        {
            score += airplaneCargo;
            airplaneCargo = 0;
        }
    }

// Update is called once per frame
    void Update()
    {
        //DetectKeyboardInput();
        if (Time.time > turnTimer)
        {
            if (airplaneActive && (destinationX != airplaneX || destinationY != airplaneY))
            {
                SetDirection();
            }
            MoveAirplane();
            //take a turn
            // see if it's in the upper left and give cargo
            LoadCargo ();
            DeliverCargo ();
            // check if its in the lowwer right and score points
            print("Cargo: " + airplaneCargo + "  Score: " + score);


            turnTimer += turnLength;
        }
    }
}