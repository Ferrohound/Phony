using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncWhitenoise : MonoBehaviour {
	
	public Material WhitenoiseMaterial;
	public Shader regularShader;
	
	float Step;
	float stage;
	int maxStage;
	
	float whiteNoise = 1;
	float XAmp;
	float YAmp;
	float ZAmp;
	float startAmp;
	
	float currTime = 0;
	float fadeTime = 5f;
	
	IEnumerator run;
	bool running = false;

	// Use this for initialization
	void Start () {
		whiteNoise = WhitenoiseMaterial.GetFloat("_TexBlend");
		XAmp = WhitenoiseMaterial.GetFloat("_AX");
		YAmp = WhitenoiseMaterial.GetFloat("_AY");
		ZAmp = WhitenoiseMaterial.GetFloat("_AZ");
		startAmp = ZAmp;
		stage = WhitenoiseMaterial.GetFloat("_Stage");
		maxStage = WhitenoiseMaterial.GetInt("_numStage");
		
		//Debug.Log(stage + " " + maxStage);
	}
	
	void Update()
	{
		if(Input.GetButtonDown("Fire1"))
			Play();
	}
	
	void Play()
	{	
		if(!running)
		{
			run = Normalize();
			StartCoroutine(run);
		}
	}
	
	//lerp all the amplitudes and frequencies
	IEnumerator Normalize()
	{
		if(stage>=0)
			stage-=1;
		
		currTime = 0;
		Debug.Log(stage);
		Debug.Log(maxStage);
		Debug.Log("Should be " + stage/maxStage);
		float newEnd = stage/maxStage;
		
		Debug.Log("New end " + newEnd);
		
		while(currTime<fadeTime)
		{
			currTime+=Time.deltaTime;
			//whiteNoise-=Time.deltaTime;
			whiteNoise = Mathf.Lerp(1, newEnd * startAmp, currTime/fadeTime);
			XAmp = Mathf.Lerp(0.5f, newEnd * startAmp, currTime/fadeTime);
			YAmp = Mathf.Lerp(0.5f, newEnd * startAmp, currTime/fadeTime);
			ZAmp = Mathf.Lerp(0.5f, newEnd * startAmp, currTime/fadeTime);
			
			WhitenoiseMaterial.SetFloat("_TexBlend", whiteNoise);
			WhitenoiseMaterial.SetFloat("_AX",XAmp);
			WhitenoiseMaterial.SetFloat("_AY", YAmp);
			WhitenoiseMaterial.SetFloat("_AZ", ZAmp);
			
			Debug.Log("LET ME OUT");
			
			yield return null;
		}
		
		Debug.Log("GOT OUT");
		WhitenoiseMaterial.SetFloat("_Stage", stage);
		
		//WhitenoiseMaterial.SetFloat("_TexBlend", 0);
		//WhitenoiseMaterial.SetFloat("_AX", 0);
		//WhitenoiseMaterial.SetFloat("_AY", 0);
		//WhitenoiseMaterial.SetFloat("_AZ", 0);
		
		//set the shader to the default one
		//WhitenoiseMaterial.shader = regularShader;
		running = false;
		yield return null;
	}
}
