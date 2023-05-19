using Betauer.Core;
using Betauer.Core.Nodes;
using Godot;

namespace Betauer.Camera; 

public static class CameraExtensions {
    
    public static Godot.NodePath CameraTransformerNodeName = "CameraTransformer";
    public static StringName CameraId = "__CameraId";
    public static StringName RT2DId = "__RemoteTransformer2D_Id";

    public static RemoteTransform2D Follow(this Camera2D camera, Node node) {
        var cameraPath = camera.GetPath().ToString();
        var rt2d = node.GetNodeOrNull<RemoteTransform2D>(CameraTransformerNodeName);
        if (rt2d == null) {
            rt2d = new RemoteTransform2D { Name = CameraTransformerNodeName.ToString() };
            node.AddChild(rt2d);
        } else {
            // camera is already following this RT2D
            if (rt2d.RemotePath == cameraPath) {
                // Rewrite values just in case
                rt2d.LinkObject(CameraId, camera);
                camera.LinkObject(RT2DId, rt2d);
                return rt2d;
            }
            // If RT2D is followed by other random camera, remove the link
            ClearFollowerCamera(rt2d);
        }
        camera.StopFollowing(); // If camera is following other RT2D, remove the link
        
        // Make camera following the RT2D
        rt2d.RemotePath = cameraPath;
        rt2d.LinkObject(CameraId, camera);
        camera.LinkObject(RT2DId, rt2d);
        return rt2d;
    }
    
    public static void StopFollowing(this Camera2D camera, bool forever = false) {
        if (camera.HasMeta(RT2DId)) {
            if (camera.GetObjectLinked<RemoteTransform2D>(RT2DId) is RemoteTransform2D rt2d) {
                if (rt2d.HasMeta(CameraId) && rt2d.GetObjectLinked<Camera2D>(CameraId) == camera) {
                    if (forever) {
                        rt2d.RemoveFromParent();
                        rt2d.Free();
                    } else {
                        rt2d.RemotePath = null;
                        rt2d.RemoveMeta(CameraId);
                    }
                } else {
                    // The RT2D was not linked to a camara at all, or it was linked to another camera
                }
            }
            camera.RemoveMeta(RT2DId);
        }
    }

    public static void ClearFollowerCamera(this Node node, bool forever = false) {
        var rt2d = node.GetNodeOrNull<RemoteTransform2D>(CameraTransformerNodeName);
        rt2d?.ClearFollowerCamera();
        if (forever) {
            rt2d?.RemoveFromParent();
            rt2d?.Free();
        }
    } 

    public static void ClearFollowerCamera(this RemoteTransform2D rt2d) {
        rt2d.RemotePath = null;
        if (rt2d.HasMeta(CameraId)) {
            if (rt2d.GetObjectLinked<Camera2D>(CameraId) is Camera2D camera) {
                if (camera.HasMeta(RT2DId) && camera.GetObjectLinked<RemoteTransform2D>(RT2DId) == rt2d) {
                    camera.RemoveMeta(RT2DId);
                } else {
                    // The Camara was not linked, or it was linked to another RT2D
                }
            }
            rt2d.RemoveMeta(CameraId);
        }
    }
}