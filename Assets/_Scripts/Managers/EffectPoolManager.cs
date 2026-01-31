using UnityEngine;
using System.Collections;

/// <summary>
/// Example: Effect Pool Manager (VFX, Particles, Blood, Etc.)
/// Quản lý pool riêng cho các hiệu ứng, particle, v.v.
/// </summary>
public class EffectPoolManager : MonoBehaviour
{
    public static EffectPoolManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Spawn một hiệu ứng (tự động return về pool sau duration)
    /// </summary>
    public GameObject SpawnEffect(string effectType, Vector3 position, float duration = 1f)
    {
        GameObject effect = GeneralObjectPoolManager.Instance.SpawnObject(effectType, position);
        if (effect != null)
        {
            StartCoroutine(ReturnEffectRoutine(effect, effectType, duration));
        }
        return effect;
    }

    public GameObject SpawnEffect(string effectType, Transform parent, float duration = 1f)
    {
        GameObject effect = GeneralObjectPoolManager.Instance.SpawnObject(effectType, parent);
        if (effect != null)
        {
            StartCoroutine(ReturnEffectRoutine(effect, effectType, duration));
        }
        return effect;
    }

    IEnumerator ReturnEffectRoutine(GameObject effect, string effectType, float duration)
    {
        yield return new WaitForSeconds(duration);
        GeneralObjectPoolManager.Instance.ReturnToPool(effect, effectType);
    }

    /// <summary>
    /// Spawn và tự động destroy (không return pool)
    /// Dùng cho hiệu ứng một lần không tái sử dụng
    /// </summary>
    public GameObject SpawnOneTimeEffect(string effectType, Vector3 position, float duration = 1f)
    {
        GameObject effect = GeneralObjectPoolManager.Instance.SpawnObject(effectType, position);
        if (effect != null)
        {
            // Kiểm tra nếu có ParticleSystem, dùng lifetime của nó
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
                duration = ps.main.duration;

            Destroy(effect, duration);
        }
        return effect;
    }
}
