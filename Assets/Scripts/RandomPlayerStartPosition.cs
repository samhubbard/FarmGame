using System;
using FishNet.Object;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FarmGame
{
    public class RandomPlayerStartPosition : NetworkBehaviour
    {
        private CharacterController _characterController;

        private void Start()
        {
            transform.position = new Vector3(Random.Range(1, 5), Random.Range(1, 5), 0);
        }
    }
}