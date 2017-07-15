using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/* ONLY ONE PER SCENE
 * This keeps track of all local settings
 * Also keeps track of all gravity sources
 * Time is recorded in 24 hour format. 
 * Currently one day is 24 minutes long (real time).
 * Maintains sun
 */

public class Weather{
    public enum DayTime{ //dunno if this is the best name for it
        Dawn, Morning, Noon, Afternoon, Dusk, Night
    }
    public enum Sky{
        Sunny, Rainy, Foggy
    }
    public class DTT{
        public DayTime dt;
        public float time;
        public float sunIntensity;
        public DTT() { }
        public DTT(DayTime DT, float TIME, float INT){
            dt = DT;
            time = TIME;
            sunIntensity = INT;
        }
    }
    /// <summary>
    /// Current time of day. Dawn, Morning, etc
    /// </summary>
    public static DayTime dt = DayTime.Dawn;
    public static float sunIntensity = 1.0f;
    public static Sky sky = Sky.Sunny;
    public static List<DTT> assocTime = new List<DTT>(); //start times

    public Weather(){
        if (assocTime.Count == 0){
            assocTime.Add(new DTT(DayTime.Dawn, 6.00f, 0.5f));
            assocTime.Add(new DTT(DayTime.Morning, 7.00f, 0.7f));
            assocTime.Add(new DTT(DayTime.Noon, 11.00f, 1.0f));
            assocTime.Add(new DTT(DayTime.Afternoon, 13.00f, 0.9f));
            assocTime.Add(new DTT(DayTime.Dusk, 18.00f, 0.6f));
            assocTime.Add(new DTT(DayTime.Night, 20.00f, 0f));
        }
    }

    public static void SetDayTime(){
        DayTime newdt = assocTime[assocTime.Count - 1].dt; //last item
        foreach (DTT dtt in assocTime){
            if (World.Time > dtt.time){
                newdt = dtt.dt;
            } else {
                break;
            }
        }
        dt = newdt;
    }
    public static void SetSunLevel(){
        int time = assocTime.FindIndex(x => x.dt == dt);
        int time2 = time + 1;
        if (time == assocTime.Count - 1){
            time2 = 0;
        }
        else if (time == 0) { return; /*There is an error*/}
        float a = (World.RealTime - assocTime[time].time) / (assocTime[time2].time - assocTime[time].time);
        sunIntensity = Mathf.Lerp(assocTime[time].sunIntensity, assocTime[time2].sunIntensity, a);
    }
    /// <summary>
    /// Whether the time is within a specified region.
    /// </summary>
    /// <param name="d1">Earlier DayTime</param>
    /// <param name="d2">Later DayTime</param>
    /// <param name="time">Time</param>
    /// <returns></returns>
    public static bool BetweenTime(DayTime d1, DayTime d2, float time){
        float a = assocTime.Find(x => x.dt == d1).time;
        float b = assocTime.Find(x => x.dt == d2).time;
        if (time >= a && time < b){
            return true;
        } else {
            return false;
        }
    }
}

public class World : MonoBehaviour{
    public bool outside;                                    //if the player is outside
    public List<Abr_Gravity> AllGravitySources;
    public UnityEngine.Light sun_primary;
    public static float GravitationalConstant = 100f;

    public static int Day { get { return weekday; } }
    public static float Time { get { return hours * 1.0f + minutes * 0.01f; } }
    public static float RealTime { get { return hours * 1f + minutes * 0.01f + seconds * 0.01f; } }
    public static int Minute { get { return minutes; } }
    public static int Hour { get { return hours; } }

    public static Weather weather;
    private static float time = 0f;
    private static int minutes = 0;
    private static float seconds = 0;
    private static int hours = 12;
    private static int weekday = 0;

    public static bool consoleUp = false;

    public AudioSource m_audio;
    private GameObject consoleWindow;
    private UnityEngine.UI.Text answer;

    //Calculation
    private Quaternion q;
    private Vector3 northPole; //used for sun direction calculation
    private float angleSun;

    //Has to be initialized before any start function is called
    void Awake(){
        AllGravitySources = new List<Abr_Gravity>();
    }
    void Start(){
        Initialize();
        UISetup();
    }
    void Update(){
        Tick();
    }

    void Tick(){
        seconds = UnityEngine.Time.time - time;
        if (seconds > 1f){
            minutes++;
            time += 1;
            if (minutes > 60){
                hours++;
                minutes = 0;
                if (hours > 24){
                    hours = 0;
                    weekday++;
                    if (weekday > 7){
                        weekday = 0;
                    }
                }
            }
            Weather.SetDayTime();
            if(outside) Weather.SetSunLevel();
        }
        if(outside) UpdateSky();
    }
    #region Gravity
    /// <summary>
    /// Returns the up vector of any point in space using the point gravity sources
    /// </summary>
    /// <param name="r">Rigidbody that needs an up vector</param>
    /// <returns></returns>
    public Vector3 GetGVector(Rigidbody r){
        Vector3 t = Vector3.zero;
        foreach (Abr_Gravity g in AllGravitySources)
        {
            t += g.getForce(r);
        }
        return -t.normalized;
    }
    /// <summary>
    /// Returns the vector of gravity at any point in space
    /// </summary>
    /// <param name="position">Position in world space</param>
    /// <param name="mass">Mass of rigidbody</param>
    /// <returns></returns>
    public Vector3 GetGVector(Vector3 position, float mass){
        Vector3 t = Vector3.zero;
        foreach (Abr_Gravity g in AllGravitySources){
            t += g.getForce(position, mass);
        }
        return t;
    }
    #endregion

    private void Initialize(){
        //get time from save file
        time = UnityEngine.Time.time;
        m_audio = GetComponent<AudioSource>();
        Console.SetWorld(this);
        sun_primary = GameObject.FindGameObjectWithTag("Sun").GetComponent<UnityEngine.Light>();
        northPole = new Vector3(-1f, 0.3f, 0f).normalized;
        weather = new Weather();
    }

    private void UpdateSky(){
        //days from 07:00 to 20:00

        angleSun = 180 * (RealTime - 7) / 13;
        angleSun = Mathf.Clamp(angleSun, 0, 180);
        sun_primary.transform.rotation = Quaternion.AngleAxis(-angleSun, northPole) * Quaternion.identity;

        //sun_primary.intensity = 5f;
    }

    private void UISetup(){
        consoleWindow = GameObject.Instantiate((GameObject)Resources.Load("ConsoleInput"));
        consoleWindow.SetActive(false);
        consoleWindow.transform.SetParent(GameObject.Find("Canvas").transform);
        consoleWindow.GetComponent<RectTransform>().anchoredPosition = new Vector2(-187, -113);
        foreach (UnityEngine.UI.Text c in consoleWindow.GetComponentsInChildren<UnityEngine.UI.Text>()){
            if (c.gameObject.name.Equals("Answer")){
                answer = c;
                break;
            }
        }
        var input = consoleWindow.GetComponent<UnityEngine.UI.InputField>();
        var se = new UnityEngine.UI.InputField.SubmitEvent();
        se.AddListener(SendCommand);
        input.onEndEdit = se;
    }
    #region Console
    public void SetTime(int Hours, int Minutes){
        hours = Hours; minutes = Minutes;
    }
    public void SendCommand(string c){
        Debug.Log(c);
        answer.text = Console.ReadCommand(c);
    }
    public void ChangeConsoleState(bool b){
        if (b){
            consoleUp = true;
            consoleWindow.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else {
            consoleUp = false;
            consoleWindow.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    #endregion
}