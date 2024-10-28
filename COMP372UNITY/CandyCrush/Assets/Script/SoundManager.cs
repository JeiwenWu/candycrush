using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource destroyNoise;

    public void PlayDestroyNoise()
    {
        destroyNoise.Play();
    }

}
