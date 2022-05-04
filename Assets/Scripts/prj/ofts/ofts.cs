using UnityEngine;

public class ofts : MonoBehaviour
{
    public static effParticles particles;
    
    void Awake()
    {
        particles = effParticles.Inst;
    }


}
