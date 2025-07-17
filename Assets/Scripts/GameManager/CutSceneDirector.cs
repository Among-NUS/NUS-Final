using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneDirector : MonoBehaviour
{
    public HeroActorBehaviour hero;
    public BossBehaviour boss;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayCutScene());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator PlayCutScene()
    {
        yield return WalkHeroTime(1f);
        boss.BossTurn();
        yield return BossFrightend();
        yield return BossJump();
        yield return new WaitForSeconds(1f);
        yield return WalkHeroTime(2.5f);
        boss.BossTurn();
        yield return WalkTogether(3f);



    }
    IEnumerator WalkHeroTime(float duration)
    {
        float t = 0;
        hero.MakeWalk();
        while (t < duration)
        {
            t += Time.deltaTime;
            if (hero.heroFacingRight) hero.transform.position += Vector3.right * hero.speed * Time.deltaTime;
            else hero.transform.position += Vector3.left * hero.speed * Time.deltaTime;
            yield return null;
        }
        hero.MakeStand();
        yield return null;

    }

    IEnumerator WalkBossTime(float duration)
    {
        float t = 0;
        boss.MakeWalk();
        while (t < duration)
        {
            t += Time.deltaTime;
            if (boss.bossFacingRight) boss.transform.position += Vector3.right * boss.speed * Time.deltaTime;
            else boss.transform.position += Vector3.left * boss.speed * Time.deltaTime;
            yield return null;
        }
        boss.MakeStand();
        yield return null;
    }

    IEnumerator HeroShoot()
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            yield return null;
        }
        hero.MakeShoot();
    }

    IEnumerator BossFrightend()
    {
        boss.BossAlert();
        float waitTime = 0f;
        while (waitTime < 0.5f)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }
        boss.BossUnAlert();
    }

    IEnumerator BossJump()
    {
        boss.Jump();
        float waitTime = 0f;
        while (waitTime < 0.5f)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator WalkTogether(float duration)
    {
        Coroutine heroWalk = StartCoroutine(WalkHeroTime(duration));
        Coroutine bossWalk = StartCoroutine(WalkBossTime(duration));
        yield return new WaitForSeconds(duration);
    }
}
