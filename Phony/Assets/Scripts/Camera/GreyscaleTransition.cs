using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreyscaleTransition : MonoBehaviour {
public Shader shader;
	Material material;
	
	RenderTexture buffer;
	RenderTexture Output;
	
	float intensity = 0.0f;
	
	public float transitionTime = 3f;
	float currentTime = 0;
	IEnumerator playing;
	
	// Use this for initialization
	void Awake () {
		//create a new material
		material = new Material(shader);
		//testMat = new Material(testShad);
	}
	
	void Start()
	{
		//lerp away
		Play();
	}
	// Update is called once per frame
	void Update () {
		
	}
	
	//post processing
	//TO DO===============================================Buffer so only certain things go greyscale
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{	
		material.SetFloat("_Blend", intensity);
		Graphics.Blit(source, destination, material);
		return;
	}
	
	void OnPostRender()
	{
		RenderTexture.ReleaseTemporary(buffer);
		RenderTexture.ReleaseTemporary(Output);
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
		intensity = material.GetFloat("_Blend");
		while(currentTime<transitionTime)
		{
			currentTime += Time.deltaTime;
			intensity = Mathf.Lerp(0, 1, currentTime/transitionTime);
			//Debug.Log("Playing");
			yield return null;
		}
	}
}