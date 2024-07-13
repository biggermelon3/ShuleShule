using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public GameObject blockRemoveParticle;

    private void Start()
    {
        EventManager.onBlockRemoved.AddListener(RemoveBlock);
    }

    private void RemoveBlock(Vector3 pos, Color c)
    {
        GameObject particleToSpawn = blockRemoveParticle.gameObject;
        var particleMainModule = particleToSpawn.GetComponent<ParticleSystem>().main;
        particleMainModule.startColor = c;
        Instantiate(particleToSpawn);
        particleToSpawn.transform.position = pos;
    }
}
