using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    [SerializeField] private GameObject weaponSocket;
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject weaponSheath;

    [SerializeField] private GameObject offhandSocket;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject backSocket;

    GameObject currentWeaponInHand;
    GameObject currentOffhandItem;
    GameObject currentWeaponInSheath;
    GameObject currentWeaponOnBack;

    void Start()
    {
        currentWeaponInSheath = Instantiate(weapon, weaponSheath.transform);
        currentWeaponOnBack = Instantiate(shield, backSocket.transform);
    }

    public void DrawWeapon()
    {
        currentWeaponInHand = Instantiate(weapon, weaponSocket.transform);
        Destroy(currentWeaponInSheath);

        currentOffhandItem = Instantiate(shield, offhandSocket.transform);
        Destroy(currentWeaponOnBack);
    }

    public void SheatheWeapon()
    {
        currentWeaponInSheath = Instantiate(weapon, weaponSheath.transform);
        Destroy(currentWeaponInHand);

        currentWeaponOnBack = Instantiate(shield, backSocket.transform);
        Destroy(currentOffhandItem);
    }
}
