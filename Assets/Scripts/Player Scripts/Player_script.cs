using UnityEngine;
using UnityEngine.Events;

public class Player_script : Pathfinder
{
    public static Player_script instance;

    private void Awake()
    {
        instance = this;
    }

}
