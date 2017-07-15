using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Console Commands
/// </summary>
public class Console{
    private static World world;

    public static void SetWorld(World w){
        world = w;
    }
    public static string ReadCommand(string command){
        string answer = "Nothing entered";
        string[] parse = command.Split(' ');
        if (parse.Length != 0){
            answer = "Not enough tokens entered.";
            switch (parse[0]){
                case "getLightLevel":
                    answer = "Light Level: " + Weather.sunIntensity.ToString();
                    break;
                case "getTime":
                    answer = "Current Time: " + World.Time.ToString();
                    break;
                case "getTimeOfDay":
                    answer = Weather.dt.ToString();
                    break;
                case "getRealTime":
                    answer = "Current Time: " + World.RealTime.ToString();
                    break;
                case "setTime": //setTime <hour>:<minute>
                    if (parse.Length < 2) break;
                    parse = parse[1].Split(':'); if (parse.Length < 2) break;
                    answer = "Error reading data";
                    int a, b;
                    if (int.TryParse(parse[0], out a))
                    {
                        if (int.TryParse(parse[1], out b))
                        {
                            a = Mathf.Clamp(a, 0, 23);
                            b = Mathf.Clamp(b, 0, 59);
                            world.SetTime(a, b);
                            answer = "Time set to: " + a.ToString() + ":" + b.ToString();
                        }
                    }
                    break;
                case "spawn": //spawn <name> <number>
                    if (parse.Length < 3) break;

                    break;
                default:
                    Debug.Log("Command not found in system.");
                    break;
            }
        }
        return answer;
    }
}