using UnityEngine;

public class SettingWindowManager : MonoBehaviour
{

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Cleanup()
    {

    }
}
