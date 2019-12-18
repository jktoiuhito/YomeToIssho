using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    private static BGMPlayer TheOnlyPlayer;

    void Awake()
    {
        if (TheOnlyPlayer == null)
        {
            TheOnlyPlayer = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this.gameObject);
    }
}
