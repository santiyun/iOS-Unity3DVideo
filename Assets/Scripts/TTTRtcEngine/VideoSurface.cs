using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoSurface : MonoBehaviour {
	public uint UserID = 0;
	public bool IsMe = false;
	public int TextureWidth = 0;
	public int TextureHeight = 0;

	void Start () {

	}
	
	void Update () {
		if (IsMe) {
			transform.Rotate (new Vector3 (0, 0.5f, 0));
		}

		Renderer theRenderer = GetComponent<Renderer> ();

		// 创建纹理
		if (theRenderer.material.mainTexture == null) {
			if (TextureWidth > 0 && TextureHeight > 0) {
				#if UNITY_IPHONE
				Texture2D texture = new Texture2D (TextureWidth, TextureHeight, TextureFormat.BGRA32, false);
				#else
				Texture2D texture = new Texture2D (TextureWidth, TextureHeight, TextureFormat.RGBA32, false);
				#endif
				theRenderer.material.mainTexture = texture;
			}
		}

		// 更新纹理
		if (theRenderer.material.mainTexture != null && theRenderer.material.mainTexture is Texture2D) {
			Texture2D tex = theRenderer.material.mainTexture as Texture2D;
			int texId = (int)tex.GetNativeTexturePtr ();
			TTTRtcEngine.IRtcEngine rtcEngine = TTTRtcEngine.IRtcEngine.QueryEngine();
			rtcEngine.UpdateTexture (texId, UserID, TextureWidth, TextureHeight);
		}
	}

	// 删除纹理
	public void RemoveTexture() {
		Renderer render = GetComponent<Renderer> ();
		if (render.material.mainTexture != null && render.material.mainTexture is Texture2D) {
			render.material.mainTexture = null;
			TextureWidth = 0;
			TextureHeight = 0;
		}
	}
}
