using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [SerializeField] LayerMask _enemyLayer;
    [SerializeField] GameObject _shockwaveEffect;
    [SerializeField] TextMeshProUGUI _text;
    float _maxRadius;
    float _expansionSpeed;
    float _pushForce;
    int _cost;
    float[] _currentRadius = { 0f, 0f, 0f, 0f, 0f } ;
    bool _isShockwaveActive = false;
    GameController _gameController;

    private void Start()
    {
        _gameController = GameController.Instance;
        _maxRadius = 30f;
        _expansionSpeed = 15f;
        _pushForce = 5f;
        _cost = 50;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ActivateShockwave());
        }
    }

    public void CastShockwave()
    {
        if (!_isShockwaveActive && _gameController.PayGold(_cost))
        {
            StartCoroutine(ActivateShockwave());
            _cost *= 2;
            _text.text = $"{_cost} g";
        }
    }

    IEnumerator ShowShockwave()
    {
        _shockwaveEffect.SetActive(true);
        yield return new WaitForSeconds(5);
        _shockwaveEffect.SetActive(false);
    }

    IEnumerator ActivateShockwave()
    {
        _isShockwaveActive = true;

        if (_shockwaveEffect != null)
        {
            StartCoroutine(ShowShockwave());
        }

        Signalbus.Fire<PlaySoundSignal>(new PlaySoundSignal() { clipName = "shockwave" });

        for (int i = 0; i < 5; i++)
        {
            StartCoroutine(ShockwaveOnly(i));
            yield return new WaitForSeconds(0.7f);
        }

        yield return new WaitForSeconds(5f);
        _isShockwaveActive = false;
        _currentRadius = new float[] { 0f, 0f, 0f, 0f, 0f };
    }

    IEnumerator ShockwaveOnly(int i)
    {
        while (_currentRadius[i] < _maxRadius)
        {
            _currentRadius[i] += _expansionSpeed * Time.deltaTime;

            Collider[] enemies = Physics.OverlapSphere(transform.position, _currentRadius[i], _enemyLayer);

            foreach (Collider enemyCollider in enemies)
            {
                Enemy enemy = enemyCollider.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.SetIsPush(true);

                    Transform enemyTransform = enemyCollider.transform;
                    Vector2 direction = enemyTransform.position - transform.position;
                    enemyTransform.position += (Vector3)(direction.normalized * _pushForce * Time.deltaTime);
                }
            }

            yield return null;
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if (_isShockwaveActive)
        {
            Gizmos.DrawWireSphere(transform.position, _currentRadius[0]);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, _maxRadius);
        }
    }
}
