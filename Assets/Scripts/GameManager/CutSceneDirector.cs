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
        StartCoroutine(WalkHeroTime(10f));
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
}
