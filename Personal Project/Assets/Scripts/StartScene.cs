﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Android;

/*
 * Class for scripting the events of the start scene.
 */
public class StartScene : MonoBehaviour
{
    //Variables
    private string userID = "";

    //GameObjects (UI)
    public TextMeshProUGUI idtext;
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        //code to detect if the app is running on computer (editor) or on the oculus device (which is android)
        //sets the value of boolean 'isonoculus' based on the result
        #if UNITY_EDITOR
            SceneData.isOnOculus = false;
            print("Editor");
        #elif UNITY_ANDROID
            SceneData.isOnOculus = true;
            print("Oculus");
        #endif

        //if no user id given yet, then set the value of userID to the result of the method generateRandomUserID which generates a random ID
        if (userID.Equals("") || userID == "")
        { 
            userID = generateRandomUserID();
        }

        //checks if the random id is unique
        if (FileManager.checkIfDirectoryExists(userID))
        {
            //if not unique then select a new one
            userID = generateRandomUserID();
        }

        //save the value of the user id in the scenedata class (temporary data storage class)
        SceneData.userID = userID;

        //set the UI text to display the users ID (so that the tester can note down the randomly generated ID that will be used to reference the participant from now onwards)
        idtext.SetText("User ID: " + userID);

        //uses the method generateSatNavOrder to randomly generate the order in which each satnav type will be tested, the array holding this order is saved in the scenedata class 
        SceneData.satNavOrder = generateSatNavOrder();
    }

    // Update is called once per frame
    void Update()
    {
        //checks the timer value and changes the value of the onscreen UI slider depending on it
        //this slider is used to show how long the user has pressed down the button to go to the next scene (as this requires a long press, not a short press)
        if(Controller.timer > 0)
        {
            slider.gameObject.SetActive(true);
            slider.maxValue = Controller.heldTime;
            slider.value = Controller.timer;
        }
        else
        {
            slider.gameObject.SetActive(false);
        }   
    }

    /*
     * Method for generating a random user ID in a defined format - returns a string
     */
    public string generateRandomUserID()
    {
        //initialised the string as "SUR", all ids will start with this to make it more human readable
        string id = "SUR";

        //loop 4 times and add a random integer between 0 & 9 to the id
        for (int i = 0; i < 4; i++)
        {
            id += Random.Range(0, 10);
        }

        //returns the string
        return id;
    }

    /*
     * Method for generating a random order in which the satnav types will be tested for that user.
     * This is so that they are tested over different scenes for different users, in order to reduce the likelihood that the scenes effect the results.
     */
    public int[] generateSatNavOrder()
    {
        //initialise the integer array 'order' to have 4 empty members
        int[] order = new int[4];

        //Initialising the members of the array so that a random number can replace it, I chose the number 10 to initialise the array members but any integer > 4 OR integer < 0 would be fine to use.
        //The members of the array need to be initialised as a different number to all of the options (0,1,2,3) so that when a number representing each satnav type is generated by the function, it is not equal to the initial value in the array and so initially replaces it.
        //if this wasn't the case and the array members were initialised to be 0, for example, then when the number 0 is randomly generated by the method it would not replace the member in the array and would default to being the last value which isn't what we want.
        order[0] = 10;
        order[1] = 10;
        order[2] = 10;
        order[3] = 10;

        //loops through the array
        for (int i = 0; i < 4; i++)
        {
            //initialised boolean isnew to false
            bool isNew = false;

            //while loop that loops until all satnav types are assigned in the order array
            while (!isNew)
            {
                //generates random integer between 0 & 3
                int number = Random.Range(0, 4);

                //set isnew to true, if the satnav type (where the numbers 0,1,2,3 denote a different satnav type) has already been added to the array then this boolean will be set to false and a new random number (random satnav type) will be generated
                isNew = true;

                //loops through array
                foreach (int n in order)
                {
                    //if a member of the array is equal to the value that was randomly generated then that satnav type has already been assigned in the array
                    if (number == n)
                    {
                        //so, set isnew to false
                        isNew = false;
                    }
                   
                }

                //if after looping through the order array there hasn't been a duplicate of the satnav type already in the array, then assign the satnav type to the member of the order array that is currently being picked
                if (isNew)
                {
                    order[i] = number;
                }
            }
        }

        //return the array once all satnav types have been assigned
        return order;
    }

}
