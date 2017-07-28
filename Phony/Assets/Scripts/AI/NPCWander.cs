using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class NPCWander : MonoBehaviour {
	
	public Transform target;
	public Transform curTarget;
	public Transform[] route;
	
	private UnityEngine.AI.NavMeshAgent nav;
	
	public bool wander = true;
	public bool patrol = false;
	bool onPatrol = false;
	public bool moveTowards = false;
	public bool move = true;
	public bool following = false;
	
	public float moveSpeed = 2f;
	
	Animator animator;

	// Use this for initialization
	void Start () {
		nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
		nav.baseOffset = 0;
		animator = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	//play animation if velocity is 0, otherwise don't
	void Update () {
		//animator.SetFloat("MoveSpeed", Vector3.Magnitude(nav.velocity)/nav.speed);	
		//Debug.Log(Vector3.Magnitude(nav.velocity));
		//Debug.Log("SHT");
		
		if(move && !Dialogue.running)
		{
			//nav.Resume();
            //use isStoppe instead
			if(wander)
			{
				moveTowards = false;
				patrol = false;
				StartCoroutine(Wander());
			}
			if(moveTowards)
			{
				patrol = false;
				wander = false;
				moveTo(curTarget);
			}
			if(patrol && !onPatrol)
			{
				wander = false;
				moveTowards = false;
				StartCoroutine(Patrol());
			}
		}
		else
		{
			animator.SetFloat("MoveSpeed", 0f);
			//nav.Stop();
            //use isStopped instead
		}
	}
	
	void FixedUpdate()
	{
		animator.SetFloat("MoveSpeed", (float) Vector3.Magnitude(nav.velocity));
	}
	
	IEnumerator Wander()
	{
		do
		{
			//Debug.Log("NEXT");
			Vector3 newTarget = Random.onUnitSphere * 5;
			newTarget = new Vector3(newTarget.x + transform.position.x,
				transform.position.y, transform.position.z + newTarget.z);
			
			yield return StartCoroutine(movePos(newTarget));
			yield return new WaitForSeconds(3f);
		} while(wander);
	}
	
	IEnumerator Patrol()
	{
		//Debug.Log("Time to Patrol");
		onPatrol = true;
		
		do
		{
			foreach(Transform t in route)
			{
				//Debug.Log("Let's go!");
				yield return StartCoroutine(movePos(t));
				yield return new WaitForSeconds(3f);
			}
		} while (patrol);
		
		onPatrol = false;
		
	}
	
	//rewrite this mess
	IEnumerator movePos(Transform target)
	{
		curTarget = target;
		float distance = Vector3.Distance (curTarget.position, transform.position);
		//Debug.Log("The distance is...");
		//Debug.Log(distance);
		while(distance>1f)
		{
			//Debug.Log("Patroling");
			if(!Dialogue.running)
			{
				moveTo(curTarget);
				distance = Vector3.Distance (curTarget.position, transform.position);
				//Debug.Log(Vector3.Magnitude(nav.velocity));
			}
			yield return null;
		}
	}
	
	void moveTo(Transform location)
	{		
		if(curTarget == null)
			curTarget = location;
		
		Vector3 dir = curTarget.position - transform.position;
		//Vector2 direction = curTarget.position.xy;
		Vector3 moveDirection = new Vector3 (dir.x, 0, dir.y);
		float distance = Vector3.Distance (curTarget.position, transform.position);
		float navDistance = Vector3.Distance (nav.destination, transform.position);
		
		if (distance >= 10f) {
			nav.speed = 2f;
			nav.angularSpeed = 240f;
		}
		else{
			nav.speed = Mathf.Lerp(.75f, 2f,  distance / 10f);
			nav.angularSpeed = Mathf.Lerp(120f, 240f,  distance / 10f);
		}
		
		nav.destination = curTarget.position + moveDirection;
		
		//set this to be wander later but for now just copy dog
	}
	
	IEnumerator movePos(Vector3 target)
	{
		float distance = Vector3.Distance (target, transform.position);
		//Debug.Log("The distance is...");
		//Debug.Log(distance);
		while(distance>1f)
		{
			if(!Dialogue.running)
			{
				//Debug.Log("Patroling");
				moveTo(target);
				distance = Vector3.Distance (target, transform.position);
				animator.SetFloat("MoveSpeed", (float) Vector3.Magnitude(nav.velocity));
				//Debug.Log(Vector3.Magnitude(nav.velocity));
			}
			yield return null;
			
		}
	}
	
	public void startFollow(GameObject target)
	{
		StartCoroutine(Follow(target));
	}
	
	public void startMoveTo(Transform target)
	{
		StartCoroutine(movePos(target));
	}
	
	public IEnumerator Follow(GameObject target)
	{
		float distance = Vector3.Distance (target.transform.position, transform.position);
		//Debug.Log("going to the ball!");
		//move to the thing
		while( following )
		{
			//Debug.Log("Distance is..." + distance);
			if(!Dialogue.running && distance > 3f)
			{
				moveTo(target.transform.position);
			}
			distance = Vector3.Distance (target.transform.position, transform.position);
			
			if(!following)
			{
				yield break;
			}
			
			yield return null;
		}
	}
	
	void moveTo(Vector3 location)
	{
		Vector3 dir = location - transform.position;
		//Vector2 direction = curTarget.position.xy;
		Vector3 moveDirection = new Vector3 (dir.x, 0, dir.y);
		float distance = Vector3.Distance (location, transform.position);
		float navDistance = Vector3.Distance (nav.destination, transform.position);
		
		if (distance >= 10f) {
			nav.speed = 2f;
			nav.angularSpeed = 240f;
		}
		else{
			nav.speed = Mathf.Lerp(.75f, 2f,  distance / 10f);
			nav.angularSpeed = Mathf.Lerp(120f, 240f,  distance / 10f);
		}
		
		nav.destination = location + moveDirection;
		//set this to be wander later but for now just copy dog
	}
}
