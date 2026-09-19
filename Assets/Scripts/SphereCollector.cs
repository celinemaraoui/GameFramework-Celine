using UnityEngine;
using TMPro;

public class SphereCollector : MonoBehaviour
{

    private int Sphere = 0;
    public TextMeshProUGUI SphereText;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sphere"))
        {
            Sphere++;
            SphereText.text = "Sphere: " + Sphere.ToString();
            Debug.Log(Sphere);
            Destroy(other.gameObject);
        }
    }
}
