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
        StartCoroutine(HeroShoot());
    }

    // Update is called once per frame
    void Update()
    {

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
}
