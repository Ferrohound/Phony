using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffectController : MonoBehaviour
{
	
	[SerializeField]
	private Vingette _vingette;
	[SerializeField]
	private AnimationCurve _innerVingette;
	[SerializeField]
	private AnimationCurve _outerVingette;
	[SerializeField]
	private AnimationCurve _fov;
	[SerializeField]
	private AnimationCurve _saturation;
	[SerializeField]
	private AnimationCurve _timeScale;
	
	//match up multiple cameras
	[SerializeField]
	private Camera[] _cameras;
	
	public AudioSource _audio;
	
	private IEnumerator playing;
	
	void Awake()
	{
		if(_audio == null)
			_audio = GetComponent<AudioSource>();
	}
	
	void Play()
	{
		if(playing == null)
		{
			playing = Warp();
			StartCoroutine(playing);
		}
	}
	
	IEnumerator Warp()
	{
		_audio.PlayOneShot(_audio.clip);
		
		for (float t = 0; t < 1.0f; t += Time.unscaledDeltaTime * 1.2f)
		{
			for (int i = 0; i < _cameras.Length; i++)
			{
				_cameras[i].fieldOfView = _fov.Evaluate(t);
			}
			_vingette.MinRadius = _innerVingette.Evaluate(t);
			_vingette.MaxRadius = _outerVingette.Evaluate(t);
			_vingette.Saturation = _saturation.Evaluate(t);
			Time.timeScale = _timeScale.Evaluate(t);

			//for jumping between worlds/cameras
			/*if (t > _swapTime && !_swapTiggered)
			{
				_swapTiggered = true;
				_twinCameras.SwapCameras();
			}*/

			yield return null;
		}
		
		_vingette.MinRadius = _innerVingette.Evaluate(1.0f);
		_vingette.MaxRadius = _outerVingette.Evaluate(1.0f);
		
		playing = null;
	}
	
}