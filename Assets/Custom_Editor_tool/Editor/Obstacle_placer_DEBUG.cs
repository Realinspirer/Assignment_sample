using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Unity.Mathematics;

public class Obstacle_placer_DEBUG : EditorWindow
{
    
    Transform Temp_parent;

    GameObject Block_prefab;
    GameObject Obstacles_prefab;

    Obstacle_generator_scriptable req_scriptable_object;

    Dictionary<Grid_tile_struct, GameObject> temp_objects = new();

    Dictionary<Grid_tile_struct, GameObject> Obstacle_data = new();

    //change if required
    int x_count = 10;
    int y_count = 10;


    [MenuItem("Tools/Obstacle_placer")]
    public static void ShowWindow()
    {
        GetWindow(typeof(Obstacle_placer_DEBUG));
    }
    private void OnGUI()
    {
        GUILayout.Label("Obstacle Generator Tool");

        //temp parent to instantiate debug blocks
        Temp_parent = (Transform)EditorGUILayout.ObjectField("Any debug transform object", Temp_parent, typeof(Transform), true);

        //block/tile prefab
        Block_prefab = (GameObject)EditorGUILayout.ObjectField("Grid Cell/Block", Block_prefab, typeof(GameObject), false);

        //obstacles prefab
        Obstacles_prefab = (GameObject)EditorGUILayout.ObjectField("Obstacle prefab", Obstacles_prefab, typeof(GameObject), false);

        if (GUILayout.Button("Spawn Temp objects"))
        {
            //to spawn a temporary grid to place and check obstacles
            Spawn_temps();
        }
        if(GUILayout.Button("Destroy Temp objects"))
        {
            //to destroy all the placed temporary items
            Destroy_temps();
        }

        //creates a X by Y button grid
        for (int x = 0; x < x_count; x++)
        {
            GUILayout.BeginHorizontal();

            for (int y = 0; y < y_count; y++)
            {

                if (GUILayout.Button($"{x},{y}", "Button", GUILayout.Width(40), GUILayout.Height(40)))
                {
                    //just adds or disables obstacles
                    Enable_disable_thing(x , y);
                }
            }
            GUILayout.EndHorizontal();
        }

        req_scriptable_object = (Obstacle_generator_scriptable)
            EditorGUILayout.ObjectField("Obstacle Scriptable Object", req_scriptable_object, typeof(Obstacle_generator_scriptable), false);

        if (GUILayout.Button("Add to ScriptableObject"))
        {
            //adds the obstacle data to the Scriptable object
            Add_to_object();
        }


    }
    void Add_to_object()
    {
        if(req_scriptable_object.Req_tiles == null)
        {
            req_scriptable_object.Req_tiles = new();
        }

        EditorUtility.SetDirty(req_scriptable_object);
        req_scriptable_object.Req_tiles.Clear();

        foreach (var item in Obstacle_data)
        {
            req_scriptable_object.Req_tiles.Add(item.Key);
        }
    }

    void Enable_disable_thing(int x, int y)
    {
        try
        {
            Grid_tile_struct tile = new Grid_tile_struct() { Row = x, Col = y };
            if (Obstacle_data.ContainsKey(tile))
            {
                DestroyImmediate(Obstacle_data[tile]);
                Obstacle_data.Remove(tile);
            }
            else
            {
                GameObject req_block = temp_objects[tile];

                GameObject cloned_obj = Instantiate(Obstacles_prefab, req_block.transform);


                //add the obstacle pos to a list to add to the scriptable object
                Obstacle_data.Add(tile, cloned_obj);
            }
        }
        catch
        {
            Debug.LogError("Something went wrong, check if you spawned the blocks first.");
        }
    }
    
    void Destroy_temps()
    {
        foreach (var item in temp_objects)
        {
            DestroyImmediate(item.Value);
        }
        temp_objects.Clear();
        //also clear the obstacle data
        Obstacle_data.Clear();
    }
    void Spawn_temps()
    {
        //destroy prev objects
        Destroy_temps();

        var req_prefab_bounds = Block_prefab.GetComponent<MeshRenderer>().bounds.size;

        float x_position = 0;
        float z_position = 0;
        

        //structs are value type so this works -- 0
        Grid_tile_struct req_struct = new Grid_tile_struct();

        for (int row = 0; row <  x_count; row++)
        {
            
            req_struct.Row = row;

            for (int col = 0; col < y_count; col++)
            {
                req_struct.Col = col;

                //creating a grid block/tile
                var cloned_obj = Instantiate(Block_prefab, Temp_parent);

                //setting the cloned object's position so it doesn't phase into each other.
                cloned_obj.transform.localPosition = new Vector3(x_position, 0, z_position);

                //advancing the position according to the cube bounds
                x_position += req_prefab_bounds.x;

                //structs are value type so this works -- 1
                temp_objects.Add(req_struct, cloned_obj);
            }
            x_position = 0;

            //advancing the z position according to the cube bounds;
            z_position += req_prefab_bounds.z;
        }
    }
}
