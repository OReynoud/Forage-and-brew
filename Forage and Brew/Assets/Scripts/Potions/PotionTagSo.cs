using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_PotionTag", menuName = "Potions/PotionTagSo")]
public class PotionTagSo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public List<PotionTagSo> InducedTags { get; private set; }
}
