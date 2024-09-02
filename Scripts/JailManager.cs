using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JailManager : MonoBehaviour
{
   [SerializeField]
   private const float pieceSapcing = 0.35f;
   public void ArrangePieces()
   {
        for(int i = 0; i < transform.childCount; i++) 
        {
            Transform child = transform.GetChild(i);
            child.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            if (i % 2 == 0)
                child.position = transform.position + new Vector3(0, 0, i * pieceSapcing);
            else 
                child.position = transform.position - new Vector3(0, 0, i * pieceSapcing);
                
        }
    }
}
