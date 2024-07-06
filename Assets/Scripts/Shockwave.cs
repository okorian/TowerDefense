using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shockwave : MonoBehaviour, ISubscriber<RestartGameSignal>, ISubscriber<ActivateShockwaveSignal>
{
    [SerializeField] LayerMask _enemyLayer;
    [SerializeField] GameObject _shockwaveEffect;
    [SerializeField] TextMeshProUGUI _text;

    int _cost;
    float _maxRadius;
    float _expansionSpeed;
    float _pushForce;
    float[] _currentRadius = { 0f, 0f, 0f, 0f, 0f };
    bool _isShockwaveActive;
    bool _isActive;
    GameController _gameController;

    private void Start()
    {
        _gameController = GameController.Instance;
        _maxRadius = 30f;
        _expansionSpeed = 15f;
        _pushForce = 5f;
        _cost = 50;
        _isActive = false;
        _isShockwaveActive = false;
        _text.text = $"{_cost} gold";
        Signalbus.Subscirbe<RestartGameSignal>(this);
        Signalbus.Subscirbe<ActivateShockwaveSignal>(this);
    }

    private void Update()
    {
        if (_isActive && Input.GetKeyDown(KeyCode.Space))
        {
            CastShockwave();
        }
    }

    public void CastShockwave()
    {
        if (_isActive && !_isShockwaveActive && _gameController.PayGold(_cost))
        {
            StartCoroutine(ActivateShockwave());
            if (_cost > int.MaxValue / 2)
            {
                _cost = int.MaxValue;
            }
            else
            {
                _cost *= 2;
            }
            _text.text = $"{_cost} gold";
            Signalbus.Fire<ShockwaveSignal>(new ShockwaveSignal());
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

    public void OnSignalReceived(RestartGameSignal signal)
    {
        _cost = 50;
        _text.text = $"{_cost} gold";
        _isActive = false;
        _isShockwaveActive = false;
    }

    public void OnSignalReceived(ActivateShockwaveSignal signal)
    {
        _isActive = true;
    }

    private void OnDestroy()
    {
        Signalbus.Unsubscribe<RestartGameSignal>(this);
    }
}
