using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// NOTE: I learned this A* pathfinding algorithm and implemented it for my own.
// I still have not fully grasped it, but was still pretty successful at modifying it for my use case.
// As I have observed - A* algorithm is quite famous, and yeah- kinda wasted my time
// trying to create a new pathfinding method for a few hours.

public class Pathfinder : MonoBehaviour
{
    [Tooltip("Speed of block following")]
    [SerializeField] float speed = 5f;
    [SerializeField] LayerMask Grid_layermask;

    const int move_straight_cost = 10;
    const int move_diagonal_cost = 14;

    public Generate_grid_script generator_script;
    public Grid_tile_struct current_grid_position;

    
    protected Vector3 req_target;

    //the dictionary will just hold references to the main dictionary
    //no value types so performance impact will be minimal
    Dictionary<Grid_tile_struct, GridTile_individual_script> Instantiated_tiles;

    bool go_to_path;
    UnityAction afteraction;
    int path_index;

    [SerializeField]List<Grid_tile_struct> blocks_to_search;
    [SerializeField]List<Grid_tile_struct> searched_blocks;
    [SerializeField]List<Grid_tile_struct> final_path;


    int dist_travelled;
    int req_dist_travelled;

    private void Start()
    {
        Instantiated_tiles = generator_script.Instantiated_tiles;

        var req_grid = Instantiated_tiles[current_grid_position];
        req_grid.is_occupied = true;
        transform.parent = req_grid.transform;
    }

    public void Move_to_position(Grid_tile_struct tile_pos, UnityAction after_action = null, int max_dis = 0)
    {
        //for max distance
        dist_travelled = 0;
        req_dist_travelled = max_dis;

        Instantiated_tiles[current_grid_position].is_occupied = false;

        Find_path(current_grid_position, tile_pos);
        path_index = final_path.Count - 1;

        //go_to_path instructs the update to start going to a clicked path
        go_to_path = true;

        //afteraction is anything that needs to be called after the positioning is done
        afteraction = after_action;

    }


    private void Update()
    {
        if(go_to_path)
        {
            //if final_path is not null this will cycle through the items
            //and position itself accordingly
            if (final_path != null && final_path.Count > 0)
            {
                Vector3 target_pos = Instantiated_tiles[final_path[path_index]].transform.position;

                Vector3 actual_target_pos = new Vector3(target_pos.x, transform.position.y, target_pos.z);

                
                Vector3 prev_pos = transform.position;

                //making sure the position doesn't change on y axis
                transform.position = Vector3.MoveTowards(transform.position, actual_target_pos
                    , Time.deltaTime * speed);

                if (transform.position == actual_target_pos)
                {
                    path_index--;
                    //check if req dist to travel is greater than 0 and keeping it under.
                    if(req_dist_travelled > 0)
                    {
                        if(dist_travelled >= req_dist_travelled)
                        {
                            common_method();
                        }
                        dist_travelled++;
                    }
                    if(path_index < 0)
                    {
                        common_method();
                    }
                }
            }
            //if the final path is null or empty
            else
            {
                go_to_path = false;
                path_index = 0;
                afteraction?.Invoke();
                UI_controller.instance.No_path_found_activate();
            }
        }
    }


    //stuffs that will happen after the path is reached.
    void common_method()
    {
        go_to_path = false;
        current_grid_position = final_path[path_index + 1];
        path_index = 0;

        var req_grid = Instantiated_tiles[current_grid_position];
        req_grid.is_occupied = true;
        transform.parent = req_grid.transform;

        afteraction?.Invoke();
    }


    //A* algorithm (grid)
    void Find_path(Grid_tile_struct start_pos, Grid_tile_struct end_pos)
    {
        blocks_to_search = new() { start_pos };
        searched_blocks = new();
        final_path = new();


        foreach (var item in Instantiated_tiles)
        {
            item.Value.g_cost = int.MaxValue;
            item.Value.Calculate_f_cost();
            item.Value.camefrom = null;
        }

        GridTile_individual_script startcell = Instantiated_tiles[start_pos];
        startcell.g_cost = 0;
        startcell.h_cost = get_distance(start_pos, end_pos);
        startcell.Calculate_f_cost();


        while(blocks_to_search.Count > 0)
        {
            Grid_tile_struct current_search = get_lowest_fcost_item(blocks_to_search);

            if (current_search.Equals(end_pos))
            {
                //reached
                final_path = Calculate_path(end_pos);
                return;
            }


            blocks_to_search.Remove(current_search);
            searched_blocks.Add(current_search);

            foreach (var item in Get_neighbor_list(current_search))
            {
                if (searched_blocks.Contains(item))
                {
                    continue;
                }
                

                GridTile_individual_script current_sc = Instantiated_tiles[current_search];
                GridTile_individual_script neighbor_sc = Instantiated_tiles[item];
                if (neighbor_sc.is_occupied)
                {
                    searched_blocks.Add(item);
                    continue;
                }

                int ten_g_cost = current_sc.g_cost + get_distance(current_search, item);
                if(ten_g_cost < neighbor_sc.g_cost)
                {
                    neighbor_sc.camefrom = current_sc;
                    neighbor_sc.g_cost = ten_g_cost;
                    neighbor_sc.h_cost = get_distance(item, end_pos);
                    neighbor_sc.Calculate_f_cost();

                    if (!blocks_to_search.Contains(item))
                    {
                        blocks_to_search.Add(item);
                    }
                }
            }

        }
    }

    List<Grid_tile_struct> Calculate_path(Grid_tile_struct endpos)
    {
        List<Grid_tile_struct> to_return = new();
        to_return.Add(endpos);

        GridTile_individual_script current = Instantiated_tiles[endpos];
        while(current.camefrom != null)
        {
            to_return.Add(current.camefrom.Current_tile_detail);
            current = current.camefrom;
        }

        return to_return;
    }

    public List<Grid_tile_struct> Get_neighbor_list(Grid_tile_struct current_path)
    {
        List<Grid_tile_struct> return_list = new();

        if(current_path.Row - 1 >= 0)
        {
            return_list.Add(new(current_path.Row - 1, current_path.Col));

            //commented to avoid diagonal movement
            //if (current_path.Col - 1 >= 0)
            //{
            //    return_list.Add(new(current_path.Row - 1, current_path.Col - 1));
            //}
            //if (current_path.Col + 1 >= 0)
            //{
            //    return_list.Add(new(current_path.Row - 1, current_path.Col + 1));
            //}
        }

        if (current_path.Row + 1 < generator_script.Grid_X_count)
        {
            return_list.Add(new(current_path.Row + 1, current_path.Col));

            //commented to avoid diagonal movement
            //if (current_path.Col - 1 >= 0)
            //{
            //    return_list.Add(new(current_path.Row + 1, current_path.Col - 1));
            //}
            //if (current_path.Col + 1 >= 0)
            //{
            //    return_list.Add(new(current_path.Row + 1, current_path.Col + 1));
            //}
        }
        if (current_path.Col - 1 >= 0) return_list.Add(new(current_path.Row, current_path.Col - 1));
        if(current_path.Col + 1 < generator_script.Grid_Y_count) return_list.Add(new(current_path.Row, current_path.Col + 1));

        return return_list;
    }

    Grid_tile_struct get_lowest_fcost_item(List<Grid_tile_struct> path_list)
    {
        GridTile_individual_script lowest_f_cost = Instantiated_tiles[path_list[0]];

        for (int i = 1; i < path_list.Count; i++)
        {
            if(Instantiated_tiles[path_list[i]].f_cost < lowest_f_cost.f_cost)
            {
                lowest_f_cost = Instantiated_tiles[path_list[i]];
            }
        }
        return lowest_f_cost.Current_tile_detail;
    }
    int get_distance(Grid_tile_struct pos1, Grid_tile_struct pos2)
    {
        int xdist = Mathf.Abs(pos1.Row -  pos2.Row);
        int ydist = Mathf.Abs(pos1.Col -  pos2.Col);

        int rem = Mathf.Abs(xdist - ydist);

        return Mathf.Min(xdist, ydist) * move_diagonal_cost + rem * move_straight_cost;
    }
}
