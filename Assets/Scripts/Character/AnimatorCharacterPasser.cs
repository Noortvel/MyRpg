using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRpg
{
    [RequireComponent(typeof(Character))]
    public class AnimatorCharacterPasser : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;
        private Character character;
        void Awake()
        {
            character = GetComponent<Character>();
        }
        void Update()
        {
            animator.SetFloat("SpeedRatio", 
                character.navMeshAgent.velocity.magnitude / character.navMeshAgent.speed);
        }
    }
}