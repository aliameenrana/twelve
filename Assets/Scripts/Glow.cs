using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GlowState
{
    None,
    StopGlow,
    Glow,
}

public enum GlowColor
{
    White,
    Green,
    Red
}

public class Glow : MonoBehaviour
{
    private GlowState state;
    private Image image;
    private bool isFadingOut = false;
    private bool isPulsating = false;

    private Color startColor;
    private Color endColor;

    private bool losingAlpha = false;

    private float timer = 0f;
    private float glowSpeed = 2f;

    private void OnEnable()
    {
        EventManager.StartListening(EventNames.OnStopGlow, OnStopGlow);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventNames.OnStopGlow, OnStopGlow);
    }

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        StateAction();
    }

    private void StateAction()
    {
        switch (state)
        {
            case GlowState.None:
                break;
            case GlowState.StopGlow:
                if (isFadingOut)
                {
                    timer += Time.deltaTime;
                    image.color = Color.Lerp(startColor, endColor, timer);
                    if (timer > 1f)
                    {
                        timer = 0f;
                        image.color = new Color(1f, 1f, 1f, 0f);
                        image.transform.localScale = Vector3.one;
                        state = GlowState.None;
                    }
                }
                break;
            case GlowState.Glow:
                if (isPulsating)
                {
                    timer += Time.deltaTime * glowSpeed;
                    if (losingAlpha)
                    {
                        image.color = Color.Lerp(startColor, endColor, Mathf.PingPong(timer, 1f));
                        image.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.75f, Mathf.PingPong(timer, 1f));
                    }
                    else
                    {
                        image.color = Color.Lerp(endColor, startColor, Mathf.PingPong(timer, 1f));
                        image.transform.localScale = Vector3.Lerp(Vector3.one * 0.75f, Vector3.one, Mathf.PingPong(timer, 1f));
                    }
                    
                    if (timer > 1f)
                    {
                        timer = 0f;
                        losingAlpha = !losingAlpha;
                    }
                }
                break;
        }
    }

    private void OnStopGlow(object userData)
    {
        FadeOut();
        OnStateUpdate(GlowState.StopGlow);
    }

    public void OnStateUpdate(GlowState glowState)
    {
        state = glowState;
    }

    private void BeginPulsating()
    {
        losingAlpha = true;
        endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        isPulsating = true;
        timer = 0f;
        state = GlowState.Glow;
    }

    private void FadeOut()
    {
        startColor = image.color;
        endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        isFadingOut = true;
        isPulsating = false;
        timer = 0f;
        state = GlowState.StopGlow;
    }

    public void BeginGlow(GlowColor color)
    {
        switch (color)
        {
            case GlowColor.White:
                startColor = Color.white;
                break;
            case GlowColor.Green:
                startColor = Color.green;
                break;
            case GlowColor.Red:
                startColor = Color.red;
                break;
        }
        BeginPulsating();
    }
}