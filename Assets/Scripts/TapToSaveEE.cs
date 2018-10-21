// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.InputModule.Utilities.Interactions;
using System.Collections.Generic;

/// <summary>
/// This class implements IInputClickHandler to handle the tap gesture.
/// It increases the scale of the object when tapped.
/// </summary>
/// 
public class TapToSaveEE : MonoBehaviour, IInputClickHandler
{

    public GameObject graspObject;

    public void Start()
    {
        //textBox.SetActive(false);
        //textBox = GameObject.Find("SceneContent");

    }

    public void Update()
    {
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("Cube2 was clicked!");

        foreach (Transform child in graspObject.transform)
        {
            if (child.name != "default")
            {
                print("Foreach loop: " + child);
                print(child.transform.position);
                print(child.transform.rotation);
            }
        }
           
    }
}