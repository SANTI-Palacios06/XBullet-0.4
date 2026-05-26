using UnityEngine;

public class LemonColor : MonoBehaviour
{
    // Color LemonColor equivalente a #EEE0A3
    public Color lemonColor = new Color(0.933f, 0.878f, 0.639f);

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = lemonColor;
        }
    }
}
