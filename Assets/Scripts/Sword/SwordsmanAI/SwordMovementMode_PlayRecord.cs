using MarkusSecundus.PhysicsSwordfight.Sword;
using MarkusSecundus.PhysicsSwordfight.Sword.Recording;
using MarkusSecundus.PhysicsSwordfight.Utils.Randomness;
using MarkusSecundus.Utils.Datastructs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace MarkusSecundus.PhysicsSwordfight.Sword.AI
{
    public class SwordMovementMode_PlayRecord : SwordMovement.Module
    {
        public IDictionary<SwordRecordUsecase, SwordMovementRecord[]> Records { protected get; set; }
        public float PlaySpeed = 1f;

        private SwordRecordUsecase _currentUsecase = (SwordRecordUsecase)(-43);
        public SwordRecordUsecase CurrentUsecase
        {
            get => _currentUsecase; set
            {
                if (_currentUsecase == value) return;
                _currentUsecase = value;
                recordsRandomizer = new Shuffler<SwordMovementRecord>(rand, Records[_currentUsecase], 2);
                StartPlaying();
            }
        }


        private Shuffler<SwordMovementRecord> recordsRandomizer;

        protected override void OnStart()
        {
            CurrentUsecase = SwordRecordUsecase.Generic;
            StartPlaying();
        }
        public override void OnFixedUpdate(float delta)
        {
            PlayCurrentMovementSteps(delta);
        }




        private System.Random rand = new System.Random();
        private SwordMovementRecord currentlyPlaying = null;

        private SwordMovementRecord.Frame[] currentSegment => currentlyPlaying.Loop.Frames;
        private int currentFrameIndex = -2;

        private void StartPlaying()
        {
            if (Records[CurrentUsecase].IsEmpty()) currentlyPlaying = null;
            else currentlyPlaying = recordsRandomizer.Next();
            currentFrameIndex = -1;
        }



        double deltaLeftower = 0f;
        private void PlayCurrentMovementSteps(double delta)
        {
            if (currentlyPlaying == null) return;
            delta *= PlaySpeed;
            deltaLeftower += delta;

            while (true)
            {
                if (++currentFrameIndex >= currentSegment.Length)
                {
                    StartPlaying();
                    deltaLeftower = 0f;
                    break;
                }
                var currentFrame = currentSegment[currentFrameIndex];
                if (currentFrame.DeltaTime > deltaLeftower)
                {
                    --currentFrameIndex;
                    break;
                }
                Script.MoveSword(currentFrame.Value.ToCommand(Script.SwordWielder.transform));
                deltaLeftower -= currentFrame.DeltaTime;
            }
        }
    }
}