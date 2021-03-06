﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using System.Collections;

using System;
using System.IO;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.InputModule.Utilities.Interactions;
using System.Collections.Generic;
namespace HoloToolkit.Unity
{
    //namespace Academy.HoloToolkit.Unity {
    public class GraspManager : Singleton<GraspManager>
    {
        public GameObject ee;
        public GameObject graspObject;
        private UniversalWebsocketClient wsc;
        private readonly string graspSaveTopic = "holocontrol/grasp";
        private readonly string placeSaveTopic = "holocontrol/place_save";
        public Boolean first_ee_generated = false;
        
        // Use this for initialization
        void Start()
        {
            Debug.Log("GraspManager Awake()");
            GameObject wso = GameObject.Find("WebsocketClient");
            graspObject.SetActive(false);

#if UNITY_EDITOR
            wsc = wso.GetComponent<WebsocketClient>();
            #else
                    wsc = wso.GetComponent<UWPWebSocketClient>();
            #endif
            wsc.Advertise(graspSaveTopic, "std_msgs/String");
            wsc.Advertise(placeSaveTopic, "std_msgs/String");
        }

        public void NewEE()
        {
            Debug.Log("Gripper Generated");

            if(!first_ee_generated) {
                graspObject.SetActive(true);
                ee.transform.parent = graspObject.transform;
                first_ee_generated = true;
            }
            else
            {
                Vector3 go = graspObject.transform.position;
                go.y += 0.5f;
                GameObject instance = (GameObject)Instantiate(ee, go, this.transform.rotation);
                instance.SetActive(true);
                instance.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                instance.transform.parent = graspObject.transform;
            }
        }

        public void SendGrasp()
        {
            Debug.Log("Grasp Sent");
            string message = "Grasp ";
            int number_of_grasp_points = 0;
            string grasp_points = "";
            Vector3 rosPos = new Vector3();
            Vector3 graspObjectLocation = new Vector3();
            Quaternion rosQuat = new Quaternion();

            foreach (Transform child in graspObject.transform)
            {
                if (child.name != "default")
                {
                    number_of_grasp_points += 1;
                    print("Foreach loop: " + child);
                    rosPos = GetCoords(child.transform.position); 
                    rosQuat = UnityToRosRotationAxisConversion(child.transform.rotation);
                    grasp_points += rosPos.x + " " + rosPos.y + " " + rosPos.z + " " +
                                    rosQuat.x + " " + rosQuat.y + " " + rosQuat.z + " " + rosQuat.w + "\n";
                }
            }

            graspObjectLocation = GetCoords(graspObject.transform.position);
            Debug.Log(grasp_points);
            message += graspObjectLocation.x + " " + graspObjectLocation.y + " " + graspObjectLocation.z + " " + number_of_grasp_points + "\n" + grasp_points;
            Debug.Log(message);
            wsc.Publish(graspSaveTopic, message);
            

        }

        // Update is called once per frame
        void Update()
        {
            if (StateManager.Instance.CurrentState != StateManager.State.GraspingState)
            {
                return;
            }

        }

        public Vector3 GetCoords(Vector3 unityInput)
        {
            Transform robotObjTransform = GameObject.Find("Movo").transform;
            Vector3 relativePos = robotObjTransform.InverseTransformPoint(unityInput);
            var x_coord = relativePos.z; // + StateManager.Instance.MovoROSStartPose.X;
            var y_coord = -relativePos.x; // + StateManager.Instance.MovoROSStartPose.Y;
            var z_coord = relativePos.y - StateManager.Instance.FloorY;
            x_coord += StateManager.Instance.MovoROSStartPose.X;
            y_coord += StateManager.Instance.MovoROSStartPose.Y;
            return new Vector3(x_coord, y_coord, z_coord);
        }

        //Convert 4D Unity quaternion to ROS quaternion
        Quaternion UnityToRosRotationAxisConversion(Quaternion qIn)
        {
            Quaternion temp = (new Quaternion(-qIn.w, qIn.y, qIn.x, -qIn.z));
            return temp;
        }
    }
}
