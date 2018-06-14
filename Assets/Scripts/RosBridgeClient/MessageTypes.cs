﻿/*
© Siemens AG, 2017-2018
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;

public static class MessageTypes {
    public static readonly Dictionary<string, Type> Dictionary = new Dictionary<string, Type>
    {
            { "geometry_msgs/Twist", typeof(GeometryTwist) },
            { "ros_reality_bridge_msgs/MoveitTarget", typeof(MoveitTarget)},
            { "std_msgs/String", typeof(StandardString) },
            { "geometry_msgs/Accel", typeof(GeometryAccel) },
            { "sensor_msgs/JointState", typeof(SensorJointStates) },
            { "geometry_msgs/Vector3", typeof(GeometryVector3) },
            { "sensor_msgs/Joy", typeof(SensorJoy) },
            { "nav_msgs/Odometry", typeof(NavigationOdometry) },
            { "std_msgs/Header", typeof(StandardHeader) },
            { "geometry_msgs/PoseWithCovariance",typeof(GeometryPoseWithCovariance) },
            { "geometry_msgs/TwistWithCovariance", typeof(GeometryTwistWithCovariance) },
            { "geometry_msgs/Pose", typeof(GeometryPose) },
            { "geometry_msgs/PoseStamped", typeof(GeometryPoseStamped) },
            { "geometry_msgs/Point", typeof(GeometryPoint) },
            { "geometry_msgs/Quaternion",typeof(GeometryQuaternion) },
            { "sensor_msgs/PointCloud2",typeof(SensorPointCloud2) },
            { "sensor_msgs/PointField", typeof(SensorPointField) },
            { "sensor_msgs/Image", typeof(SensorImage) },
            { "sensor_msgs/CompressedImage", typeof(SensorCompressedImage) },
            { "std_msgs/Time", typeof(StandardTime)    },
            { "nav_msgs/MapMetaData", typeof(NavigationMapMetaData) },
            { "nav_msgs/OccupancyGrid", typeof(NavigationOccupancyGrid)},
            { "moveit_msgs/DisplayTrajectory", typeof(MoveItDisplayTrajectory)},
            { "moveit_msgs/RobotTrajectory", typeof(MoveItRobotTrajectory)},
            { "trajectory_msgs/JointTrajectory", typeof(TrajectoryJointTrajectory)},
            { "trajectory_msgs/JointTrajectoryPoint",typeof(TrajectoryJointTrajectoryPoint)},
            { "trajectory_msgs/MultiDOFJointTrajectory", typeof(TrajectoryMultiDOFJointTrajectory)},
            { "trajectory_msgs/MultiDOFJointTrajectoryPoint", typeof(TrajectoryMulitDOFJointTrajectoryPoint)},
            { "geometry_msgs/Transform", typeof(GeometryTransform)},
            { "moveit_msgs/RobotState", typeof(MoveItRobotState)},
            { "sensor_msgs/MultiDOFJointState", typeof(SensorMultiDOFJointState)},
            { "geometry_msgs/Wrench", typeof(GeometryWrench)},
            { "moveit_msgs/AttachedCollisionObject", typeof(MoveItAttachedCollisionObject)},
            { "moveit_msgs/CollisionObject", typeof(MoveItCollisionObject)},
            { "object_recognition_msgs/ObjectType", typeof(ObjectRecognitionObjectType)},
            { "shape_msgs/SolidPrimitive", typeof(ShapeSolidPrivitive)},
            { "shape_msgs/Mesh", typeof(ShapeMesh)},
            { "shape_msgs/MeshTriangle", typeof(ShapeMeshTriangle)},
            { "shape_msgs/Plane", typeof(ShapePlane)}
        };
    public static string RosMessageType(Type messageType) {
        return Dictionary.FirstOrDefault(x => x.Value == messageType).Key;
    }
    public static Type MessageType(string rosMessageType) {
        Type messageType;
        Dictionary.TryGetValue(rosMessageType, out messageType);
        return messageType;
    }
}
