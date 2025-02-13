using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generate_grid_script : MonoBehaviour
{
    [Header("Grid/Block Generators")]
    [SerializeField] GridTile_individual_script Grid_prefab;
    [SerializeField] public int Grid_X_count;
    [SerializeField] public int Grid_Y_count;
    [SerializeField] Transform Req_parent;
    [Tooltip("Delay between each generation to create a smooth transition")]
    [SerializeField]float interval = 0.005f;
    [Tooltip("Parent_position change speed. Is arbitrary (or based on observation)")]
    [SerializeField] float parent_pos_speed = 6;


    [Header("Obstacle manager")]
    [SerializeField] Obstacle_manager obs_manager;

    [Header("Player manangers")]
    [SerializeField] Player_script Player_prefab;
    [SerializeField] Grid_tile_struct Player_spawn_loc;
    [SerializeField] Block_raycaster_manager Block_selection_manager;

    [Header("Enemy managers")]
    [SerializeField] Enemy_manager enemy_manager;



    Vector3 req_prefab_bounds;
    Vector3 parent_target_position;


    [HideInInspector] public Dictionary<Grid_tile_struct, GridTile_individual_script> Instantiated_tiles = new();


    private void Start()
    {
        //grid size to make a grid. For this to work the
        //parent objects of the grid need to be of Scale 1f
        req_prefab_bounds = Grid_prefab.GetComponent<MeshRenderer>().bounds.size; 

        //starting the initial grid creation
        StartCoroutine(Generate_initial_grid());

        //new parent_position to make sure the grid is in the middle of the screen
        parent_target_position = new Vector3(-(Grid_Y_count / 2) * req_prefab_bounds.x, 0, -(Grid_X_count / 2) * req_prefab_bounds.z); 
    }


    //Coroutine to generate blocks. Can be transformed
    //(with some changes) into just an Update() function if necessary
    IEnumerator Generate_initial_grid() 
    {
        float x_position = 0;
        float z_position = 0; 

        for (int row = 0; row < Grid_X_count; row++)
        {
            for(int col = 0; col < Grid_Y_count; col++)
            {
                //creating a grid block/tile
                var cloned_obj = Instantiate(Grid_prefab, Req_parent);

                //setting the cloned object's position so it doesn't phase into each other.
                cloned_obj.transform.localPosition = new Vector3(x_position, 0, z_position);

                //setting row and col
                cloned_obj.Set_row_col(row, col);

                //advancing the position according to the cube bounds
                x_position += req_prefab_bounds.x;


                //adding the tiles/blocks to the dictionary
                Instantiated_tiles.Add(new() { Row = row, Col = col }, cloned_obj);

                //also creates a non-blocking generation loop
                yield return new WaitForSeconds(interval);
            }
            x_position = 0;

            //advancing the z position according to the cube bounds;
            z_position += req_prefab_bounds.z; 
        }

        //place obstacles on the generated grid
        obs_manager.Place_objects();

        //place enemies
        enemy_manager.Place_enemies();

        //spawn player
        Place_player();
    }

    //bool checker to avoid useless calculation
    bool reposition_done = false;
    private void Update()
    {
        if (!reposition_done) 
        {
            //linearly moving parent to the target position;
            Req_parent.localPosition = Vector3.MoveTowards(Req_parent.localPosition, parent_target_position, parent_pos_speed * Time.deltaTime);


            if(Req_parent.localPosition == parent_target_position)
            {
                reposition_done = true;
            }
        }
    }


    void Place_player()
    {
        var player= Instantiate(Player_prefab, Req_parent);

        //setting the position above the block
        Vector3 spawn_block_position = Instantiated_tiles[Player_spawn_loc].transform.position;
        player.transform.position = new Vector3(spawn_block_position.x, spawn_block_position.y + req_prefab_bounds.y, spawn_block_position.z);
        player.current_grid_position = Player_spawn_loc;
        player.generator_script = this;
        Block_selection_manager.current_player_script = player;
        
    }
}
