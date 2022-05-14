using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;

namespace FarmGame
{ 
    public class PredictionMotor : NetworkBehaviour
    {
        #region Data Types
        private struct MoveData
        {
            public float Horizontal;
            public float Vertical;

            public MoveData(float horizontal, float vertical)
            {
                Horizontal = horizontal;
                Vertical = vertical;
            }
        }

        private struct ReconcileData
        {
            public Vector2 Position;
            public Quaternion Rotation;

            public ReconcileData(Vector2 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
            }
        }
        #endregion

        public float MoveRate = 10f;
        private CharacterController characterController;
        private bool subscribed;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void SubscribeToTimeManager(bool subscribe)
        {
            if (TimeManager == null) return;
            if (subscribe == subscribed) return;

            subscribed = subscribe;

            if (subscribe)
            {
                TimeManager.OnTick += TimeManagerOnTick;
                TimeManager.OnPostTick += TimeManagerOnPostTick;
            }
            else
            {
                TimeManager.OnTick -= TimeManagerOnTick;
                TimeManager.OnPostTick -= TimeManagerOnPostTick;
            }
        }

        private void OnDestroy()
        {
            SubscribeToTimeManager(false);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            SubscribeToTimeManager(true);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            SubscribeToTimeManager(true);
        }

        private void TimeManagerOnTick()
        {
            if (IsOwner)
            {
                Reconciliation(default, false);

                MoveData data;
                GatherInputs(out data);
                
                Move(data, false);
            }

            if (IsServer)
            {
                Move(default, true);
            }
        }

        private void TimeManagerOnPostTick()
        {
            ReconcileData data = new ReconcileData(transform.position, transform.rotation);
            Reconciliation(data, true);
        }

        private void GatherInputs(out MoveData data)
        {
            data = default;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            
            if (horizontal == 0f && vertical == 0f) return;

            data = new MoveData(horizontal, vertical);
        }

        [Replicate]
        private void Move(MoveData data, bool asServer, bool replaying = false)
        {
            Vector2 movement = new Vector2(data.Horizontal, data.Vertical).normalized * MoveRate * Time.deltaTime;
            characterController.Move(movement);
        }

        [Reconcile]
        private void Reconciliation(ReconcileData data, bool asServer)
        {
            transform.position = data.Position;
            transform.rotation = data.Rotation;
        }
    }
}