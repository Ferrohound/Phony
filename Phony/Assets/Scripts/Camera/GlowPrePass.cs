using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class GlowPrePass : MonoBehaviour {

	//two render textures that the camera renders to
	private static RenderTexture Pre;
	private static RenderTexture Blurred;
	
	private Material _blurMat;
	
	
	void OnEnable()
	{
		//create the textures
		Pre = new RenderTexture(Screen.width, Screen.height, 24);
		Pre.antiAliasing = QualitySettings.antiAliasing;
		//blurred is the screen's width bitshifted/dividedby2
		Blurred = new RenderTexture(Screen.width>>1, Screen.height>>1, 0);
		
		//get the camera
		Camera camera = GetComponent<Camera>();
		Shader glowShader = Shader.Find("Hidden/GlowReplace");
		//set the camera's target rendertexture to the prepass
		camera.targetTexture = Pre;
		//set it so that whenever a shader has the "Glows" tag, it renders the glow
		//colour to the rendertexture instead
		camera.SetReplacementShader(glowShader, "Glows");
		
		//set global textures so everything has access
		Shader.SetGlobalTexture("_GlowPrePassTex", Pre);
		Shader.SetGlobalTexture("_GlowBlurredTex", Blurred);
		
		//blur material for post-processing
		//blur simply blurs whatever it gets
		_blurMat = new Material(Shader.Find("Hidden/Blur"));
		_blurMat.SetVector("_BlurSize", new Vector2(Blurred.texelSize.x * 1.5f,
			Blurred.texelSize.y * 1.5f));
		
	}
	
	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		//blit the current view buffer to the PrePass RenderTexture
		Graphics.Blit(src, dst);
		
		//set the new target to the blur and clear it
		Graphics.SetRenderTarget(Blurred);
		
		//clear depth, clear colour, BG colour
		GL.Clear(false, true, Color.clear);
		Graphics.Blit(src, Blurred);
		
		//blur the hell out of it
		for(int i=0; i<4; i++)
		{
			RenderTexture tmp = RenderTexture.GetTemporary(Blurred.width, Blurred.height);
			
			//the fourth argument determines which pass to do
			//by default, it's -1 which is all of them, but we're doing vertical, then horizontal
			Graphics.Blit(Blurred, tmp, _blurMat, 0);
			Graphics.Blit(tmp, Blurred, _blurMat, 1);
			RenderTexture.ReleaseTemporary(tmp);
		}
	}
}
