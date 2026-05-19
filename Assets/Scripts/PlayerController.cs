using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float clampX = 8.5f;

    public float dashDuration = 0.5f;
    public float dashCooldown = 3f;
    private float dashTimer = 0f;
    private bool isDashing = false;

    public float speedBoostMultiplier = 2.5f;
    public float speedBoostDuration = 1f;
    public float speedBoostCooldown = 5f;

    public float clearRadius = 3f;
    public float clearCooldown = 7f;

    public TextMeshProUGUI skillText;

    private float currentSpeedMultiplier = 1f;
    private float speedBoostTimer = 0f;
    private float clearTimer = 0f;
    private bool isSpeedBoosting = false;

    private SpriteRenderer sr;
    private Color originalColor;

    public float SpeedBoostCooldownRatio =>
        speedBoostTimer > 0f ? speedBoostTimer / speedBoostCooldown : 0f;
    public float ClearCooldownRatio =>
        clearTimer > 0f ? clearTimer / clearCooldown : 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    void Update()
    {
        HandleCooldowns();
        HandleMovement();
        HandleDash();
        HandleSpeedBoost();
        HandleClear();
        UpdateSkillUI();
    }

    void HandleCooldowns()
    {
        if (speedBoostTimer > 0f) speedBoostTimer -= Time.deltaTime;
        if (clearTimer > 0f) clearTimer -= Time.deltaTime;
        if (dashTimer > 0f) dashTimer -= Time.deltaTime;
    }

    void HandleMovement()
    {
        float input = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input = 1f;

        float speed = moveSpeed * currentSpeedMultiplier;
        float newX = transform.position.x + input * speed * Time.deltaTime;
        newX = Mathf.Clamp(newX, -clampX, clampX);

        transform.position = new Vector3(newX, transform.position.y, 0f);
    }

    void HandleDash()
    {
        if (!Keyboard.current.spaceKey.wasPressedThisFrame) return;
        if (dashTimer > 0f) return;
        if (isDashing) return;

        StartCoroutine(DashCoroutine());
    }

    IEnumerator DashCoroutine()
    {
        isDashing = true;
        sr.color = new Color(1f, 1f, 0f, 0.7f);

        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Poop"),
            true
        );

        yield return new WaitForSeconds(dashDuration);

        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Poop"),
            false
        );

        sr.color = originalColor;
        isDashing = false;
        dashTimer = dashCooldown;
    }

    void HandleSpeedBoost()
    {
        if (!Keyboard.current.eKey.wasPressedThisFrame) return;
        if (speedBoostTimer > 0f) return;
        if (isSpeedBoosting) return;

        StartCoroutine(SpeedBoostCoroutine());
    }

    IEnumerator SpeedBoostCoroutine()
    {
        isSpeedBoosting = true;
        currentSpeedMultiplier = speedBoostMultiplier;

        yield return new WaitForSeconds(speedBoostDuration);

        currentSpeedMultiplier = 1f;
        isSpeedBoosting = false;
        speedBoostTimer = speedBoostCooldown;
    }

    void HandleClear()
    {
        if (!Keyboard.current.qKey.wasPressedThisFrame) return;
        if (clearTimer > 0f) return;

        StartCoroutine(ShowClearRadius());

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, clearRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Poop"))
                hit.GetComponent<Poop>()?.ReturnToPool();
        }

        clearTimer = clearCooldown;
    }

    IEnumerator ShowClearRadius()
    {
        GameObject circle = new GameObject("ClearEffect");
        circle.transform.position = transform.position;
        LineRenderer lr = circle.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.loop = true;
        lr.widthMultiplier = 0.1f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(0f, 1f, 1f, 0.8f);
        lr.endColor = new Color(0f, 1f, 1f, 0f);

        int segments = 40;
        lr.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;
            float x = transform.position.x + Mathf.Cos(angle) * clearRadius;
            float y = transform.position.y + Mathf.Sin(angle) * clearRadius;
            lr.SetPosition(i, new Vector3(x, y, 0));
        }

        yield return new WaitForSeconds(0.3f);
        Destroy(circle);
    }

    void UpdateSkillUI()
    {
        if (skillText == null) return;

        var spaceStr = isDashing
            ? "Space(Invincible): Active!"
            : dashTimer > 0f
            ? $"Space(Invincible): {dashTimer:F1}s"
            : "Space(Invincible): Ready";

        var eStr = isSpeedBoosting
            ? "E(Boost): Active!"
            : speedBoostTimer > 0f
            ? $"E(Boost): {speedBoostTimer:F1}s"
            : "E(Boost): Ready";

        var qStr = clearTimer > 0f
            ? $"Q(Clear): {clearTimer:F1}s"
            : "Q(Clear): Ready";

        skillText.text = $"{spaceStr}\n{eStr}\n{qStr}";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Poop")) return;
        other.GetComponent<Poop>()?.ReturnToPool();
        GameManager.Instance?.GameOver();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, clearRadius);
    }
}