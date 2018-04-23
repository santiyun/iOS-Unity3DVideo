using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TTTRtcEngine;

public class HelloUnity : MonoBehaviour {
	private string AppID = "a967ac491e3acf92eed5e1b5ba641ab7";
	private uint MyUserID = (uint)Random.Range (1, 1000000);
	//private VIDEO_PROFILE VideoProfile = VIDEO_PROFILE._120P;
	private VIDEO_PROFILE VideoProfile = VIDEO_PROFILE._360P;

	public IRtcEngine RtcEngine;
	public readonly Dictionary<GameObject, uint> UsersDictionary = new Dictionary<GameObject, uint> ();

	public HelloUnity() {
		RtcEngine = null;
	}

	public IRtcEngine loadEngine() {
		if (!ReferenceEquals(RtcEngine, null)) {
			return RtcEngine;
		}

		// 初始化TTTRtcEngine
		RtcEngine = IRtcEngine.GetEngine (AppID);

//		// enable log
//		RtcEngine.SetLogFilter (LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);

		return RtcEngine;
	}

	public void unloadEngine() {
		if (!ReferenceEquals(RtcEngine, null)) {
			IRtcEngine.Destroy ();
			RtcEngine = null;
		}
	}

	public void joinChannel(string channel) {
		if (ReferenceEquals(RtcEngine, null))
			return;

		RtcEngine.OnFirstLocalVideoFrame = OnFirstLocalVideoFrame;
		RtcEngine.OnUserJoined = OnUserJoined;
		RtcEngine.OnFirstRemoteVideoFrameDecoded = OnFirstRemoteVideoFrameDecoded; 
		RtcEngine.OnUserOffline = OnUserOffline;

		RtcEngine.EnableVideo();
		RtcEngine.SetVideoProfile (VideoProfile, true);
		RtcEngine.JoinChannel(channel, MyUserID);
	}

	public void leaveChannel() {
		UsersDictionary.Clear ();

		if (!ReferenceEquals(RtcEngine, null)) {
			RtcEngine.LeaveChannel();
		}
	}

	private GameObject getAvailableGameObject() {
		foreach (KeyValuePair<GameObject, uint> kvp in UsersDictionary) {
			if (kvp.Value == 0) {
				return kvp.Key;
			}
		}
		return null;
	}

	private GameObject getGameObjectWithUserID(uint uid) {
		foreach (KeyValuePair<GameObject, uint> kvp in UsersDictionary) {
			if (kvp.Value == uid) {
				return kvp.Key;
			}
		}
		return null;
	}

	private void getLocalTextureSize(bool swapWidthAndHeight, ref int width, ref int height) {
		switch (VideoProfile) {
		case VIDEO_PROFILE._120P:
			width  = 160;
			height = 120;
			break;
		case VIDEO_PROFILE._180P:
			width  = 320;
			height = 180;
			break;
		case VIDEO_PROFILE._240P:
			width  = 320;
			height = 240;
			break;
		case VIDEO_PROFILE._360P:
			width  = 640;
			height = 360;
			break;
		case VIDEO_PROFILE._480P:
			width  = 640;
			height = 480;
			break;
		case VIDEO_PROFILE._720P:
			width  = 1280;
			height = 720;
			break;
		case VIDEO_PROFILE._1080P:
			width  = 1920;
			height = 1080;
			break;
		}

		if (swapWidthAndHeight) {
			int temp = width;
			width = height;
			height = temp;
		}
	}

	public void OnHelloUnitySceneLoaded() {
		UsersDictionary.Clear ();
		UsersDictionary.Add (GameObject.Find ("Cylinder"), MyUserID);
		UsersDictionary.Add (GameObject.Find ("Cube1"), 0);
		UsersDictionary.Add (GameObject.Find ("Cube2"), 0);
	}

	private void OnFirstLocalVideoFrame(uint uid, int width, int height, int elapsed) {
		GameObject go = getGameObjectWithUserID (MyUserID);
		VideoSurface vs = go.GetComponent<VideoSurface>();
		vs.UserID = MyUserID;
		vs.IsMe = true;
		//getLocalTextureSize (true, ref vs.TextureWidth, ref vs.TextureHeight);
		vs.TextureWidth = width;
		vs.TextureHeight = height;
	}

	private void OnUserJoined(uint uid, int elapsed) {
		Debug.LogFormat ("OnUserJoined: {0}", uid);
	}

	private void OnFirstRemoteVideoFrameDecoded(uint uid, int width, int height, int elapsed) {
		GameObject go = getAvailableGameObject ();
		if (ReferenceEquals(go, null)) {
			return;
		}
		UsersDictionary [go] = uid;

		VideoSurface vs = go.GetComponent<VideoSurface>();
		vs.enabled = true;
		vs.UserID = uid;
		vs.TextureWidth = width;
		vs.TextureHeight = height;
	}

	private void OnUserOffline(uint uid, USER_OFFLINE_REASON reason) {
		GameObject go = getGameObjectWithUserID(uid);
		if (!ReferenceEquals (go, null)) {
			UsersDictionary [go] = 0;

			VideoSurface vs = go.GetComponent<VideoSurface> ();
			vs.UserID = 0;
			vs.RemoveTexture ();
			vs.enabled = false;
		}
	}
}
