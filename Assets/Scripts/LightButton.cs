using UnityEngine;

public class LightButton : MonoBehaviour
{
    [SerializeField]
    GameObject[] lights;

    public void ToggleLight()
    {
        foreach (var light in lights)
        {
            light.GetComponent<Light>().enabled = !light.GetComponent<Light>().enabled;
        }
    }
}
