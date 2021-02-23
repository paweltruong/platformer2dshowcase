using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

/// <summary>
/// fade in /fade out effect on color's alpha
/// </summary>
public class Fader : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] float fadeFrameAlphaStep = 0.05f;
    [SerializeField] FadeType type;

    [SerializeField] bool fadeIn = true;
    [SerializeField] bool fadeOut;
    SpriteRenderer sr;
    SpriteShapeRenderer ssr;
    Image img;
    TextMeshProUGUI tmp;

    Color SpriteColor
    {
        get
        {
            switch (type)
            {
                case FadeType.SpriteRenderer:
                    return sr.color;
                case FadeType.SpriteShapeRenderer:
                    return ssr.color;
                case FadeType.UIImage:
                    return img.color;
                case FadeType.TMPText:
                    return tmp.color;
                default:
                    Debug.LogError("Fade type not set");
                    break;
            }
            return new Color();
        }
        set
        {
            switch (type)
            {
                case FadeType.SpriteRenderer:
                    sr.color = value;
                    break;
                case FadeType.SpriteShapeRenderer:
                    ssr.color = value;
                    break;
                case FadeType.UIImage:
                    img.color = value;
                    break;
                case FadeType.TMPText:
                    tmp.color = value;
                    break;
                default:
                    Debug.LogError("Fade type not set");
                    break;
            }
        }
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        ssr = GetComponent<SpriteShapeRenderer>();
        img = GetComponent<Image>();
        tmp = GetComponent<TextMeshProUGUI>();
        if (sr == null && ssr == null && img == null && tmp == null)
            Debug.LogError($"{nameof(Fader)} requires {nameof(SpriteRenderer)} or {nameof(SpriteShapeRenderer)} or {nameof(Image)} or {nameof(TextMeshProUGUI)}");
    }

    private void Update()
    {
        if (fadeIn)
        {
            switch (type)
            {
                case FadeType.SpriteRenderer:
                    {
                        var color = sr.color;
                        if (color.a < 1f)
                            sr.color = GetFadeInNextColor(color);
                    }
                    break;
                case FadeType.SpriteShapeRenderer:
                    {
                        var color = ssr.color;
                        if (color.a < 1f)
                            ssr.color = GetFadeInNextColor(color);
                    }
                    break;
                case FadeType.UIImage:
                    {
                        var color = img.color;
                        if (color.a < 1f)
                            img.color = GetFadeInNextColor(color);
                    }
                    break;
                case FadeType.TMPText:
                    {
                        var color = tmp.color;
                        if (color.a < 1f)
                            tmp.color = GetFadeInNextColor(color);
                    }
                    break;
                default:
                    Debug.LogError("Fade type not set");
                    break;
            }
        }
        else
        {
            switch (type)
            {
                case FadeType.SpriteRenderer:
                    {
                        var color = sr.color;
                        if (color.a > 0)
                            sr.color = GetFadeOutNextColor(color);
                    }
                    break;
                case FadeType.SpriteShapeRenderer:
                    {
                        var color = ssr.color;
                        if (color.a > 0)
                            ssr.color = GetFadeOutNextColor(color);
                    }
                    break;
                case FadeType.UIImage:
                    {
                        var color = img.color;
                        if (color.a > 0)
                            img.color = GetFadeOutNextColor(color);
                    }
                    break;
                case FadeType.TMPText:
                    {
                        var color = tmp.color;
                        if (color.a > 0)
                            tmp.color = GetFadeOutNextColor(color);
                    }
                    break;
                default:
                    Debug.LogError("Fade type not set");
                    break;
            }
        }
    }

    private Color GetFadeOutNextColor(Color color)
    {
        return new Color(color.r, color.g, color.b, Mathf.Clamp(color.a - fadeFrameAlphaStep, 0, 1f));
    }
    private Color GetFadeInNextColor(Color color)
    {
        return new Color(color.r, color.g, color.b, Mathf.Clamp(color.a + fadeFrameAlphaStep, 0, 1f));
    }

    public void FadeIn()
    {
        fadeIn = true;
        fadeOut = false;
    }
    public void FadeOut()
    {
        fadeIn = false;
        fadeOut = true;
    }
}
