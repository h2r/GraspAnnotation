﻿using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Academy.HoloToolkit.Unity;

namespace Academy.HoloToolkit.Unity {
    public class TFListener : MonoBehaviour {

        //private UWPWebSocketClient UWPwsc;
        //private WebsocketClient wsc;
        private UniversalWebsocketClient wsc;
        private string unityTopic = "holocontrol/unity_waypoint_pub";
        private string movoStateTopic = "holocontrol/ros_movo_state_pub";
        private string movoPoseTopic = "holocontrol/ros_movo_pose_pub";
        private string movoStateRequestTopic = "holocontrol/movo_state_request";
        private string movoPoseRequestTopic = "holocontrol/movo_pose_request";
        private bool currentlyNavigating = false;
        private bool hasPublishedWaypoints = false;
        private int frameCounter;
        private int frameCountStart;

        public float scale = 1f;
        // Use this for initialization
        void Start() {
            //if build
            GameObject wso = GameObject.Find("WebsocketClient");
#if UNITY_EDITOR
            wsc = wso.GetComponent<WebsocketClient>();
#else
        wsc = wso.GetComponent<UWPWebSocketClient>();
#endif
            wsc.Subscribe(movoStateTopic, "std_msgs/String", "none", 0);
            wsc.Subscribe(movoPoseTopic, "std_msgs/String", "none", 0);
            wsc.Advertise(unityTopic, "std_msgs/String");
            wsc.Advertise(movoStateRequestTopic, "std_msgs/String");
            wsc.Advertise(movoPoseRequestTopic, "std_msgs/String");
            Debug.Log("Subscribed!");
            currentlyNavigating = false;
            hasPublishedWaypoints = false;
            frameCounter = 0;
        }

        private string GetROSMessage(string topic_input) {
            string[] components = topic_input.Split(':');
            foreach (string component in components) {
                if (component.Contains("\"op\"")) {
                    var dataPair = component.Split('}');
                    var msg = dataPair[0].Trim(new Char[] { ' ', '"' });
                    return msg;
                }
            }
            return null;
        }

        private void PublishWaypoints() {
            Debug.Log("Publishing waypoints!");
            List<Waypoint> waypoints = WaypointManager.Instance.Waypoints;
            int num_waypoints = waypoints.Count;
            string coord_message = "";
            foreach (Waypoint waypoint in waypoints) {
                Vector2 waypoint_coords = waypoint.GetCoords();
                string coord_str = waypoint_coords[0].ToString() + "," + waypoint_coords[1].ToString();
                coord_message += coord_str + ";";
            }
            wsc.Publish(unityTopic, coord_message.TrimEnd(';'));
            Debug.Log("Published: " + coord_message);
            currentlyNavigating = true;
            hasPublishedWaypoints = true;
            StateManager.Instance.CurrentState = StateManager.State.NavigatingState;
            frameCountStart = frameCounter;
            if (StateManager.Instance.UnityDebugMode) {
                StateManager.Instance.MovoState = "navigating";
            }
        }

        public void UpdateMovoROSPose() {
            if (StateManager.Instance.UnityDebugMode) {
                StateManager.Instance.MovoROSPose = new Pose(0, 0, 0);
                return;
            }
            wsc.Publish(movoPoseRequestTopic, "True");
            string ros_msg = wsc.messages[movoPoseTopic];
            while (ros_msg == null) {
                Debug.Log("Waiting for message...");
                ros_msg = wsc.messages[movoPoseTopic];
            }
            Debug.Log(ros_msg);
            List<string> poseStr = new List<string>(GetROSMessage(ros_msg).Split(','));
            List<float> pose = new List<float> { Convert.ToSingle(poseStr[0]), Convert.ToSingle(poseStr[1]), Convert.ToSingle(poseStr[2]) };//poseStr.Cast<float>().ToList();
            Debug.Assert(pose.Count == 3);
            StateManager.Instance.MovoROSPose = new Pose(pose[0], pose[1], -pose[2]);
            //Debug.Log("MovoROSPose updated!");
        }

        private void UpdateMovoState() {
            if (StateManager.Instance.UnityDebugMode) {
                return;
            }
            //Debug.Log("Updating MovoState");
            wsc.Publish(movoStateRequestTopic, "True");
            StateManager.Instance.MovoState = GetROSMessage(wsc.messages[movoStateTopic]);
            Debug.Log(StateManager.Instance.MovoState);
        }

        void Update() {
            frameCounter++;
            try {
                UpdateMovoROSPose();
                UpdateMovoState();
            }
            catch {
                return;
            }

            if (StateManager.Instance.CurrentState != StateManager.State.NavigatingState) {
                return;
            }

            if (StateManager.Instance.MovoState == "standby" && hasPublishedWaypoints) {
                if (frameCounter - frameCountStart < 7) { // Give ROS enough time to receive waypoints
                    frameCounter++;
                    return;
                }
                frameCounter = 0;
                currentlyNavigating = false;
                hasPublishedWaypoints = false;
                // TODO: is this necessary?
                // ------------------------------------------------
                WaypointManager.Instance.InitializeWaypoints();
                // ------------------------------------------------
                StateManager.Instance.TransitionedToWaypointState = true;
                StateManager.Instance.CurrentState = StateManager.State.WaypointState;
            }
            else if (StateManager.Instance.MovoState == "standby" && !currentlyNavigating && !hasPublishedWaypoints) {
                PublishWaypoints();
            }
        }

        //Vector3 RosToUnityPositionAxisConversion(Vector3 rosIn) {
        //    return new Vector3(-rosIn.x, rosIn.z, -rosIn.y) * scale + StateManager.Instance.MovoOffset;// + robot.transform.position;	
        //}

        //Quaternion RosToUnityQuaternionConversion(Quaternion rosIn) {
        //    return new Quaternion(rosIn.x, -rosIn.z, rosIn.y, rosIn.w);
        //}


    }
}