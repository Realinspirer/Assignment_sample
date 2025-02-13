using UnityEngine;


//Obstacles and Enemy_manager can be inherited from a same class due to such similarities. Anyways.
public class Obstacle_manager : MonoBehaviour
{
    //scriptable object used to place obstacles
    [SerializeField] Obstacle_generator_scriptable Req_obstacle_placements;
    //generator script for blocks position
    [SerializeField] Generate_grid_script Generator_sc;
    //literally the obstacle prefab
    [SerializeField] GameObject Obstacle_prefab;

    bool place_them_objects = false;
    int current_obstacle_reading_index = 0;

    public void Place_objects()
    {
        place_them_objects = true;
    }


    //looping the objects creation in Update to not block game during instantiating, not that it would've made any difference.
    private void Update()
    {
        if (place_them_objects)
        {
            if(current_obstacle_reading_index >= Req_obstacle_placements.Req_tiles.Count)
            {
                return;
            }
            Grid_tile_struct req_to_place_on = Req_obstacle_placements.Req_tiles[current_obstacle_reading_index];

            GridTile_individual_script grid_object = Generator_sc.Instantiated_tiles[req_to_place_on];

            GameObject obstacle_instance = Instantiate(Obstacle_prefab, grid_object.transform);

            //setting the tiles/blocks as occupied
            grid_object.is_occupied = true;

            current_obstacle_reading_index++;
        }
    }
}
