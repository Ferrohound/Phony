using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//need to put this in one script/one camera so clipping is a thing
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class Sin : MonoBehaviour {
	
	private Material _compositeMat;
	private Material _greyscaleMat;
	private Material _clipMat;
	
	//render textures, one for the original
	private static RenderTexture pre;
	private static RenderTexture sinned;
	private static RenderTexture clipped;
	
	RenderTexture middleMan;
	
	Shader sinShader;
	
	//transition shit
	float intensity = 0.0f;
	
	public float transitionTime = 3f;
	float currentTime = 0;
	IEnumerator playing;
	
	Camera camera;
	Camera camCopy;
	GameObject depthCamera=null;
	
	
	// Use this for initialization
	void Start () {
		depthCamera=new GameObject();
		depthCamera.AddComponent<Camera>();
		camCopy = depthCamera.GetComponent<Camera>();
		camCopy.enabled = false;
		
		camCopy.hideFlags=HideFlags.HideAndDontSave;
		camCopy.CopyFrom(camera);
		camCopy.cullingMask=1<<0;
		camCopy.clearFlags=CameraClearFlags.Depth;
		
		//lerp away
		Play();
	}
	
	void OnEnable()
	{
		//get the effect materials
		_compositeMat = new Material(Shader.Find("Hidden/SinComposite"));
		_greyscaleMat = new Material(Shader.Find("Hidden/Greyscale"));
		_clipMat = new Material(Shader.Find("Hidden/Clip"));
		
		//set up things for the prepass
		pre = new RenderTexture(Screen.width, Screen.height, 24);
		pre.antiAliasing = QualitySettings.antiAliasing;
		
		clipped = new RenderTexture(Screen.width, Screen.height, 24);
		
		//sinned is a smaller render texture to save on space
		//bitshifted right one, so half as big
		sinned = new RenderTexture(Screen.width/*>>1*/, Screen.height/*>>1*/, 24);
		
		camera = GetComponent<Camera>();
		
		//set replacement shader and global variables
		sinShader = Shader.Find("Hidden/Sin");
		
		//camera.SetReplacementShader(sinShader, "Sin");
		
		Shader.SetGlobalTexture("_SinPPTex", pre);
		Shader.SetGlobalTexture("_SinnedTex", sinned);
		Shader.SetGlobalTexture("_ClippedTex", clipped);
	}
	
	void OnPreRender()
	{
		//write sinned into sin
		camCopy.CopyFrom(camera);
		camCopy.targetTexture = sinned;
		//camCopy.transform.position=camera.transform.position;
		//camCopy.transform.rotation=camera.transform.rotation;
		//sin writes to the buffer
		//camCopy.SetReplacementShader(sinShader, "Sin");
		//camCopy.Render();
		//camCopy.RenderWithShader(sinShader, "Sin");
		
	}
	
	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		//camera.targetTexture = pre
		//Graphics.Blit(src, dst);
		//GL.Clear(false, true, Color.clear);
		//camera.RenderWithShader(sinShader);
		
		//camera turns its target texture to pre and blits the original image
		//into it
		camera.targetTexture = pre;
		Graphics.SetRenderTarget(pre);
		PrePass(src, pre);
		//set target back to screen
		camera.targetTexture = null;
		Composite(src, dst);
		
	}
	
	void PrePass(RenderTexture src, RenderTexture dst)
	{	
	
		camera.SetTargetBuffers(pre.colorBuffer, sinned.depthBuffer);
		Graphics.Blit(src, dst);
		camCopy.SetReplacementShader(sinShader, "Sin");
		camCopy.RenderWithShader(sinShader, "Sin");
		/*camera.targetTexture = sinned;
		Graphics.SetRenderTarget(sinned);
		
		GL.Clear(false, true, Color.clear);
		//Graphics.Blit(src, sinned);
		camera.RenderWithShader(sinShader, "Sin");
		Graphics.Blit(src, dst);*/
	}
	
	void Composite(RenderTexture src, RenderTexture dst)
	{
		_greyscaleMat.SetFloat("_Blend", intensity);
		middleMan = RenderTexture.GetTemporary(src.width, src.height);
		
		//grey middleman out then do the blit
		Graphics.SetRenderTarget(middleMan);
		Graphics.Blit(src, middleMan, _greyscaleMat);
		
		//add the sin on top of it via sin composition shader/mat
		Graphics.SetRenderTarget(dst);
		Graphics.Blit(middleMan, dst, _compositeMat);
		
		RenderTexture.ReleaseTemporary(middleMan);
	}
	
	//lerp the value of the greyscale
	void Play()
	{
		if (playing == null)
		{
			playing = Warp();
			StartCoroutine(playing);
		}
	}
	
	IEnumerator Warp()
	{
		currentTime = 0;
		intensity = _greyscaleMat.GetFloat("_Blend");
		while(currentTime<transitionTime)
		{
			currentTime += Time.deltaTime;
			intensity = Mathf.Lerp(0, 1, currentTime/transitionTime);
			//Debug.Log("Playing");
			yield return null;
		}
	}
	
	
}
