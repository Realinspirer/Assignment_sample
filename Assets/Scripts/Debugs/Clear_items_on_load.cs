using UnityEngine;



//just a debug child clearer. Incase forgot to clear gameobject by the obstacle generator tool
public class Clear_items_on_load : MonoBehaviour
{
    private void Start()
    {
        while(transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
