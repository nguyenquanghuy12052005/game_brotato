using System.Collections;
using UnityEngine;

internal sealed class GunLabBoss : MonoBehaviour
{
    private GunSkillTestArena arena;
    private Rigidbody2D bossBody, player;
    private EnemyController movement;
    private Sprite projectile;
    private Material material;

    internal void Setup(GunSkillTestArena owner, Rigidbody2D playerBody, Sprite shot, Material spriteMaterial)
    {
        arena = owner;
        player = playerBody;
        projectile = shot;
        material = spriteMaterial;
        bossBody = GetComponent<Rigidbody2D>();
        movement = GetComponent<EnemyController>();
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(1.45f);
        while (true)
        {
            if (!arena.PlayerStanding)
            {
                yield return null;
                continue;
            }
            yield return Fan();
            yield return new WaitForSeconds(0.75f);
            if (arena.PlayerStanding) yield return Dash();
            yield return new WaitForSeconds(0.85f);
        }
    }

    private IEnumerator Fan()
    {
        Vector2 origin = bossBody.position;
        Vector2 aim = (player.position - origin).normalized;
        var mark = Telegraph(origin, origin + aim * 7f, 0.13f);
        yield return new WaitForSeconds(0.62f);
        Destroy(mark.gameObject);
        if (!arena.PlayerStanding) yield break;
        for (int i = 0; i < 5; i++)
        {
            Vector2 heading = Quaternion.Euler(0, 0, (i - 2) * 13f) * aim;
            StartCoroutine(Orb(origin, heading));
        }
    }

    private IEnumerator Orb(Vector2 origin, Vector2 heading)
    {
        var obj = new GameObject("Boss fan projectile");
        obj.transform.SetParent(transform);
        obj.transform.position = origin;
        obj.transform.rotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.right, heading));
        var sprite = obj.AddComponent<SpriteRenderer>();
        sprite.sprite = projectile;
        sprite.sharedMaterial = material;
        sprite.color = new Color(1f, 0.34f, 0.17f);
        sprite.sortingOrder = 6;
        obj.transform.localScale = new Vector3(0.44f / projectile.bounds.size.x, 0.23f / projectile.bounds.size.y, 1f);
        Vector2 previous = origin;
        for (float elapsed = 0f; elapsed < 1.75f; elapsed += Time.deltaTime)
        {
            Vector2 next = origin + heading * (7.5f * elapsed);
            obj.transform.position = next;
            if (arena.PlayerStanding && DistanceSquared(player.position, previous, next) <= 0.33f * 0.33f)
            {
                arena.ReceiveBossAttack(player.position - heading, 12);
                break;
            }
            previous = next;
            yield return null;
        }
        Destroy(obj);
    }

    private IEnumerator Dash()
    {
        Vector2 start = bossBody.position;
        Vector2 heading = (player.position - start).normalized;
        if (heading.sqrMagnitude < 0.01f) yield break;
        const float duration = 0.48f;
        float speed = movement.MoveSpeed * 3.2f;
        Vector2 end = ClampToArena(start + heading * (speed * duration));
        var mark = Telegraph(start, end, 0.55f);
        yield return new WaitForSeconds(0.70f);
        Destroy(mark.gameObject);
        bool hit = false;
        for (float elapsed = 0f; elapsed < duration && arena.PlayerStanding; elapsed += Time.fixedDeltaTime)
        {
            Vector2 next = ClampToArena(bossBody.position + heading * (movement.MoveSpeed * 3.2f * Time.fixedDeltaTime));
            if (!hit && arena.PlayerStanding &&
                DistanceSquared(player.position, bossBody.position, next) <= 0.65f * 0.65f)
            {
                hit = true;
                arena.ReceiveBossAttack(bossBody.position, 20);
            }
            bossBody.MovePosition(next);
            yield return new WaitForFixedUpdate();
        }
    }

    private LineRenderer Telegraph(Vector2 start, Vector2 end, float width)
    {
        var obj = new GameObject("Boss attack warning");
        obj.transform.SetParent(transform);
        var line = obj.AddComponent<LineRenderer>();
        line.sharedMaterial = material;
        line.startColor = new Color(1f, 0.2f, 0.12f, 0.75f);
        line.endColor = new Color(1f, 0.5f, 0.15f, 0.25f);
        line.widthMultiplier = width;
        line.sortingOrder = 4;
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        return line;
    }

    private static Vector2 ClampToArena(Vector2 position) =>
        new Vector2(Mathf.Clamp(position.x, -7.1f, 7.1f), Mathf.Clamp(position.y, -4f, 4f));

    private static float DistanceSquared(Vector2 point, Vector2 from, Vector2 to)
    {
        Vector2 segment = to - from;
        float t = Mathf.Clamp01(Vector2.Dot(point - from, segment) / Mathf.Max(segment.sqrMagnitude, 0.0001f));
        return (point - (from + segment * t)).sqrMagnitude;
    }
}
