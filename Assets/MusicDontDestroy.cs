using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LAGS
{
    public class MusicDontDestroy : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
