using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UI_controller : MonoBehaviour
{
    UIDocument current_doc;
    Label active_element_label;
    Label no_path_found_label;

    public static UI_controller instance;

    //created bool and clock to manage
    // I have previously created scripts that do late execution without needing these every time
    // with advaced features like special UID for each one/cancel the same..and more
    bool hide_no_path_found = false;
    float hiding_clock = 0;

    private void Awake()
    {
        instance = this;

        current_doc = GetComponent<UIDocument>();
        //getting label from uidocument.
        active_element_label = current_doc.rootVisualElement.Q<Label>("Active_element_label");
        no_path_found_label = current_doc.rootVisualElement.Q<Label>("No_path_found");
    }

    private void Update()
    {
        //checking nullable active_element
        if(Block_raycaster_manager.active_element == null)
        {
            active_element_label.text = $"Active Tile/Block [ Row : Column ] : (No block active)";
        }
        else 
        {
            var req_val = Block_raycaster_manager.active_element.Value;
            active_element_label.text = $"Active Tile/Block [ Row : Column ] : ({req_val.Row} , {req_val.Col})";
        }

        //hiding no path label if it's activated
        if (hide_no_path_found)
        {
            hiding_clock += Time.deltaTime;

            if(hiding_clock > 5)
            {
                hide_no_path_found = false;
                no_path_found_label.AddToClassList("hidden");
                
            }
        }
    }


    //showing the hidden path label
    public void No_path_found_activate()
    {
        hiding_clock = 0;
        hide_no_path_found = true;
        no_path_found_label.RemoveFromClassList("hidden");
    }
}
