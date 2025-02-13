using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Obstacle_generator_scriptable", menuName = "Scriptable Objects/Obstacle_generator_scriptable")]
public class Obstacle_generator_scriptable : ScriptableObject
{
    public List<Grid_tile_struct> Req_tiles = new();
}
