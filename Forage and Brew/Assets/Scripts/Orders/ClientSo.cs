using UnityEngine;

[CreateAssetMenu(fileName = "D_Client", menuName = "Clients/ClientSo")]
public class ClientSo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Color AssociatedColor { get; private set; }
}
