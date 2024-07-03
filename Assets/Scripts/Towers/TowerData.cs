using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "Tower Defense/Tower")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public int[] dmg;
    public int[] price;
    public float[] range;
    public float[] attackSpeed;
    public Sprite towerSprite;
    public GameObject towerPrefab;
    public ProjectileFactory projectileFactory;
    public bool isFire;
    public bool isIce;
    public bool isGold;
}
