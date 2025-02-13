using UnityEngine;


[System.Serializable]
public struct Grid_tile_struct
{
    public int Col;
    public int Row;

    public Grid_tile_struct(int x, int y)
    {
        Row = x; 
        Col = y;
    }
}

public class GridTile_individual_script : MonoBehaviour
{

    [Header("On hover pop animation")]
    [SerializeField] float hover_Y_position = 0.5f;
    [SerializeField] float hover_speed = 5f;






    //can be disabled from other scripts
    public static bool interactable = true;

    bool supposed_to_be_active = false;
    Vector3 req_hover_pos;
    Vector3 def_pos;


    //required for pathfinding
    [HideInInspector]public Grid_tile_struct Current_tile_detail;
    [HideInInspector]public int f_cost;
    [HideInInspector]public int g_cost;
    [HideInInspector]public int h_cost;

    [HideInInspector]public GridTile_individual_script camefrom;

    [HideInInspector] public bool is_occupied = false;

    public void Calculate_f_cost()
    {
        f_cost = g_cost + f_cost;
    }
    public void Set_row_col(int X, int Y)
    {
        Current_tile_detail.Row = X;
        Current_tile_detail.Col = Y;
    }

    private void Update()
    {
        var current_pos = transform.localPosition;
        
        //active_anim_enabled is a bool that can be disabled if necessary (if player is moving)
        if (interactable && supposed_to_be_active)
        {
            if (current_pos.y != hover_Y_position)
            {
                transform.localPosition = Vector3.MoveTowards(current_pos, req_hover_pos, hover_speed * Time.deltaTime);
            }
        }
        else
        {
            if (current_pos.y != 0)
            {
                transform.localPosition = Vector3.MoveTowards(current_pos, def_pos, hover_speed * Time.deltaTime);
            }
        }

        //setting the bool false every frame but only at the end
        //(before this line the bool can be true due to Activate_block method)
        supposed_to_be_active = false;
    }
    public void Activate_block()
    {
        //setting the def and hover position. can be set elsewhere
        var local_pos = transform.localPosition;

        def_pos = new Vector3(local_pos.x, 0, local_pos.z);
        req_hover_pos = new Vector3(local_pos.x, hover_Y_position, local_pos.z);

        //if raycast is there this will set the bool true and it will be true till the end of Update every frame.
        supposed_to_be_active = true;
    }
}
