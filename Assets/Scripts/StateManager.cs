﻿using System.Collections.Generic;
using UnityEngine;

namespace Academy.HoloToolkit.Unity {
    public class StateManager : Singleton<StateManager> {
        public bool RobotCalibrated { get; set; }
        public bool TransitionedToWaypointState { get; set; }
        public enum State { CalibratingState, WaypointState, NavigatingState };
        public State CurrentState;
        public Pose MovoROSToUnityOffset { get; set; }
        public Pose MovoUnityToROSOffset { get; set; }
        public Pose MovoROSPose { get; set; }
        public Pose MovoStartPose { get; set; }
        public string MovoState;

        void Awake() {
            Debug.Log("Initialized StateManager");
            MovoROSToUnityOffset = null;
            MovoROSPose = null;
            RobotCalibrated = false;
            CurrentState = State.CalibratingState;
            TransitionedToWaypointState = false;
            MovoState = "standby";
        }

        private void ParseStates() {
            if (CurrentState == State.CalibratingState || CurrentState == State.WaypointState) {
                Utils.SetSpatialMapping(true);
            }
            else {
                Utils.SetSpatialMapping(false);
            }
            if (!RobotCalibrated && CurrentState != State.NavigatingState) {
                CurrentState = State.CalibratingState;
            }
            if (RobotCalibrated && CurrentState == State.CalibratingState) {
                Debug.Assert(MovoROSToUnityOffset != null);
                CurrentState = State.WaypointState;
            }
        }

        private void Update() {
            ParseStates();
            Debug.Assert(MovoState == "standby" || MovoState == "navigating");
        }
    }
}