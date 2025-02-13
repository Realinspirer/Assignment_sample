using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class Block_raycaster_manager : MonoBehaviour
{
    [SerializeField]Camera cam;
    [Tooltip("Grid Block/Tile layer")]
    [SerializeField] LayerMask block_layermask;
    [SerializeField] float raycast_distance = 60f;

    [Header("Enemy manager to manage enemies...")]
    [SerializeField] Enemy_manager enemy_manager;


    [HideInInspector]public Player_script current_player_script;
    
    
    //active element position for uicontroller
    public static Grid_tile_struct? active_element;

    //GridTile_individual_script current_selected;
    bool already_selected = false;
    
    void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void Update()
    {
        //checking if the static bool is true
        if (GridTile_individual_script.interactable)
        {
            //converting current cursor screen point to ray
            Ray cursor_ray = cam.ScreenPointToRay(Input_controller_script.mouse_position);

            RaycastHit raycast_data;

            //checking if raycast hit any collider in block_layermask
            var hit = Physics.Raycast(cursor_ray, out raycast_data, raycast_distance, block_layermask);

            active_element = null;

            if (hit)
            {

                var req_individual_sc = raycast_data.collider.GetComponent<GridTile_individual_script>();

                if (req_individual_sc != null)
                {

                    //for ui
                    active_element = req_individual_sc.Current_tile_detail;

                    //just making it pop 
                    req_individual_sc.Activate_block();


                    if (Input_controller_script.mouse_down && !already_selected)
                    {

                        GridTile_individual_script.interactable = false;


                        current_player_script.Move_to_position(req_individual_sc.Current_tile_detail, () =>
                        {
                            GridTile_individual_script.interactable = true;
                            enemy_manager.Follow_player();
                        });

                        already_selected = true;
                    }
                }
            }
        }

        if(!Input_controller_script.mouse_down)
        {
            already_selected = false;
        }
    }
}
