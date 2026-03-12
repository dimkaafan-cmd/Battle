using System;
using System.Collections;
using Scripts.Data;
using Scripts.View;
using TMPro;
using UnityEngine;

// только визуал юнита.
[RequireComponent(typeof(Renderer))]
public class UnitView : MonoBehaviour
{
    [SerializeField] private GameObject mutationEffectPrefab;
    [SerializeField] private TextMeshPro   mutationLabel;
    [SerializeField] private UnitHealthView unitHealthView;

    private Renderer _renderer;
    private Color    _currentColor;

    public void Initialize(UnitIdentity identity)
    {
        _renderer     = GetComponent<Renderer>();
        _currentColor = unitHealthView.GetCurrentColor();

        if (mutationLabel != null)
            mutationLabel.text = string.Empty;
        unitHealthView?.UpdateHealth(1f, _currentColor);
    }

    //при получении урона — можно добавить вспышку
    public void OnDamaged(int currentHP, int maxHP)
    {
        // Опционально: кратковременная вспышка белым
        StopAllCoroutines();
        StartCoroutine(DamageFlash());
        unitHealthView?.UpdateHealth((float)currentHP/maxHP, _currentColor);
    }

    /// <summary>Визуальный эффект мутации: смешение цвета + рост масштаба.</summary>
    public void PlayMutationEffect(UnitIdentity victimIdentity, int mutationCount)
    {
        // Смешиваем цвет с цветом жертвы (20% сдвиг за каждую мутацию)
        Color victimColor = ColorFromIdentity(victimIdentity);
        _currentColor     = Color.Lerp(_currentColor, victimColor, 0.2f);
        _renderer.material.color = _currentColor;

        // Небольшой рост (макс ~+25% при 5 стеках)
        transform.localScale *= 1.05f;

        // Счётчик мутаций
        if (mutationLabel != null)
            mutationLabel.text = mutationCount > 0 ? $"{mutationCount}" : string.Empty;

        // VFX-эффект (если назначен prefab)
        if (mutationEffectPrefab != null)
        {
            var vfx = Instantiate(mutationEffectPrefab,
                                  transform.position + Vector3.up,
                                  Quaternion.identity);
            Destroy(vfx, 1f);
        }
    }

    /// <summary>Анимация смерти: сжатие + вызов callback по завершении.</summary>
    public void PlayDeathAnimation(Action onComplete)
    {
        StartCoroutine(DeathRoutine(onComplete));
    }

    private IEnumerator DamageFlash()
    {
        _renderer.material.color = Color.white;
        yield return new WaitForSeconds(0.08f);
        _renderer.material.color = _currentColor;
    }

    private IEnumerator DeathRoutine(Action onComplete)
    {
        Vector3 startScale = transform.localScale;
        float   t          = 0f;
        float   duration   = 0.35f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t / duration);
            yield return null;
        }

        onComplete?.Invoke();
    }

    private static Color ColorFromIdentity(UnitIdentity identity)
    {
        return identity.color switch
        {
            UnitColor.Red   => new Color(0.85f, 0.20f, 0.20f),
            UnitColor.Green => new Color(0.20f, 0.75f, 0.30f),
            UnitColor.Blue  => new Color(0.20f, 0.40f, 0.85f),
            _               => Color.white
        };
    }
}
