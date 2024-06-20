using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    GameController _gameController;
    private void Start()
    {
        _gameController = GameController.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            _gameController.EarnGold(other.gameObject.GetComponent<Enemy>().GetBounty() / 2);
            Destroy(other.gameObject);
        };
    }
}
