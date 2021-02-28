using UnityEngine;

[System.Serializable]
public class Powerup : MonoBehaviour, IConsumable
{
    [SerializeField]
    private string _name;

    private bool _isUsed = false;

    public Powerup(string name)
    {
        this._name = name;
    }

    public void Use()
    {
        print($"{_name} used!");
    }

    public bool IsUsed()
    {
        return _isUsed;
    }

    public void SetUsed(bool used)
    {
        this._isUsed = used;
    }
}
