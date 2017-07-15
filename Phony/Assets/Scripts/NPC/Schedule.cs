using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

/*
	Schedule class
	<Schedule>
		<Days>
			<Day day="monday">
				<Tasks>
					<Task>
					</Task>
				</Tasks>
			</Day>
		</Days>
	</Schedule>
	
	would also have heuristics, ie)
	baker would get "bake" added to its schedule if the pantry has less than
	5 pieces of bread available
*/

[XmlRoot("Schedule")]
public class Schedule
{
	public enum Day {Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday};
	private S_Queue q;
	private float time;
	Task current;
	
	//initialize queues here
	public Schedule() {
		
	}
	
	//add an element to a given day
	public bool AddTask(string day, Task T)
	{
		//check if overlaps
		return true;
	}
	
	//remove an element
	public bool RemoveTask(Task task)
	{
		//check if even in there
		return true;
	}
	
	//execute the schedule system
	//tick time
	IEnumerator Run()
	{
		while(true)
		{
			time = World.Time; //changed. revisit if necessary
			/*if(current != null)
			{
				
			}*/
			yield return null;
		}
	}
	
	void Tick()
	{
		
	}
};


//queue base class
public class Base_Queue{
	public Base_Queue(){
		_numProcess = 0;
		_available = 8;
		//create space for one process
		//_processes = (Process**)calloc(_available, sizeof(Process*));
	}
	
	public virtual void push(Task T){return;}
	public virtual Task pop(){return new Task();}
	public bool empty(){ return _numProcess == 0; }
	public void print(){
		/*cout<<"[Q ";
		if(_numProcess==0){
			cout<<"empty";
		}
		else{
			for(int i=0;i<_numProcess - 1;i++){
				cout<<_processes[i]<<" ";
			}
			cout<<_processes[_numProcess - 1];
		}
		cout<<"]"<<endl;*/
		Debug.Log("F");
	}
	
	//percolate the tasks down the queue based on arrival time
	//========================================================================MOVE THIS LATER
	public int percolateFCFS(List<Task> Tasks, int AT, int a, int b){
		for(int i = 0; i < b + 1; ++i){
			if((Tasks[i])._arrival > AT){
				return i;
			}
		}
		return b + 1;
	}
	
	//percolate based on priority
	public int percolatePriority(List<Task> Tasks, int AT, int a, int b)
	{
		//percolate based on arrival time, then noodle it down based on its priority
		
		
		return b+1;
	}
	
	//number of processes, the available space and the data array
	int _numProcess;
	int _available;
	List<Task> Tasks;
};


//Schedule queue task


public class S_Queue : Base_Queue
{
	public S_Queue() : base()
	{
		
	}
	
	void Push(Task T)
	{
		
	}
	
	Task Pop()
	{
		
		return new Task();
	}
	
};


//if we're thinking about scheduling it more like memory, like blocks, then we'll probably
//need this
public struct Hour
{
	bool free;
	Task task;
};

//Task, equivalent to process in CPU Scheduling
//Tasks have an arrival time, duration, priority level, etc...
/*
	Should probably have associated coroutine to run?
	or just hash it, probably, almost definitely, okay guess that's what I'm doing then.
	ie) HashT["BuyBread"] = BuyBread();
*/
public struct Task
{
	public string _name;
	public char _ID;
	public int _arrival;
	public int _burst;
	public int _burstTime;
	public int _rem;
	public int _priority;
	public bool _used;
	public List<Hour> hours;
};

