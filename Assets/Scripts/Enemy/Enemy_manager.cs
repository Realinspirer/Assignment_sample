using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Obstacles and Enemy_manager can be inherited from a same class due to such similarities. Anyways.
public class Enemy_manager : MonoBehaviour
{
    //scriptable object used to place enemies
    [SerializeField] Obstacle_generator_scriptable Req_obstacle_placements;
    //generator script for blocks position
    [SerializeField] Generate_grid_script Generator_sc;
    //literally the enemy prefab
    [SerializeField] Enemy_script Enemy_prefab;
    //max dist the enemy can move
    [SerializeField] int max_dist_movement;
    
    


    //list to update all enemies
    List<Enemy_script> spawned_enemies = new();

    bool place_them_objects = false;
    int current_obstacle_reading_index = 0;

    public void Place_enemies()
    {
        place_them_objects = true;
    }


    private void Update()
    {
        if (place_them_objects)
        {
            if (current_obstacle_reading_index >= Req_obstacle_placements.Req_tiles.Count)
            {
                return;
            }
            Grid_tile_struct req_to_place_on = Req_obstacle_placements.Req_tiles[current_obstacle_reading_index];

            GridTile_individual_script grid_object = Generator_sc.Instantiated_tiles[req_to_place_on];

            Enemy_script enemy_instance = Instantiate(Enemy_prefab, grid_object.transform);
            enemy_instance.generator_script = Generator_sc;

            enemy_instance.current_grid_position = req_to_place_on;

            //setting the tiles/blocks as occupied
            grid_object.is_occupied = true;

            spawned_enemies.Add(enemy_instance);

            current_obstacle_reading_index++;
        }
    }

    int interactable_times_required;
    int current_interactable_count;

    public void Follow_player()
    {
        if (spawned_enemies == null || spawned_enemies.Count <= 0) return;

        //getting the neighbor blocks of the player
        var req_neighbor_sc = spawned_enemies[0].Get_neighbor_list(Player_script.instance.current_grid_position);


        //making the grid not interactable
        GridTile_individual_script.interactable = false;

        StartCoroutine(move_to_position_co(req_neighbor_sc));

        
    }


    IEnumerator move_to_position_co(List<Grid_tile_struct> req_neighbor_sc)
    {
        interactable_times_required = 0;
        int enemy_ind = 0;
        //just try to follow the neighboring blocks if possible, and if enemies are left then they will just stay
        if (req_neighbor_sc != null && req_neighbor_sc.Count > 0)
        {
            foreach (var item in req_neighbor_sc)
            {
                if (!Generator_sc.Instantiated_tiles[item].is_occupied)
                {
                    interactable_times_required++;
                    spawned_enemies[enemy_ind].Move_to_position(item, make_grid_interactable, max_dist_movement);
                    enemy_ind++;
                }

                if (enemy_ind >= spawned_enemies.Count)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
        }
        if(interactable_times_required <= 0)
        {
            make_grid_interactable();
        }
    }
    void make_grid_interactable()
    {
        current_interactable_count += 1;
        if(current_interactable_count >= interactable_times_required)
        {
            GridTile_individual_script.interactable = true;
        }
    }

}
