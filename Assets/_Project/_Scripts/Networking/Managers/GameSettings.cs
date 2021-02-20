using UnityEngine;

[CreateAssetMenu(menuName = "Manager/GameSettings")]

public class GameSettings : ScriptableObject
{
    // Only Players on the Same GameVersion can play together
    [SerializeField] private string gameVersion = "0.0.0";
    public string GameVersion => gameVersion;

    // Currently used for debug to create random players
    [SerializeField] private string nickName = "Player";
    public string NickName
    {
        get
        {
            var value = Random.Range(0, 9999);
            return nickName + value.ToString();
        }
    }
}
