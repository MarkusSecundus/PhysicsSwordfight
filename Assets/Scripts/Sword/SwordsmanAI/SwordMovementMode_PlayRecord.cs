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
    /// <summary>
    /// Sword control mode that replays prerecorded moves. Used by <see cref="SwordsmanAI"/>.
    /// </summary>
    public class SwordMovementMode_PlayRecord : SwordMovement.Module
    {
        /// <summary>
        /// Set of movement records to play for each usecase
        /// </summary>
        public IDictionary<SwordRecordUsecase, SwordMovementRecord[]> Records { protected get; set; }
        /// <summary>
        /// Speed of replay
        /// </summary>
        public float PlaySpeed = 1f;

        /// <summary>
        /// Currently active usecase - the one whose corresponding set of records is being played
        /// </summary>
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
        SwordRecordUsecase _currentUsecase = (SwordRecordUsecase)(-43);


        Shuffler<SwordMovementRecord> recordsRandomizer;

        /// <summary>
        /// Inits the module to <see cref="SwordRecordUsecase.Attack"/> and starts playing
        /// </summary>
        protected override void OnStart()
        {
            CurrentUsecase = SwordRecordUsecase.Attack;
            StartPlaying();
        }
        /// <summary>
        /// Replays a frame of current record
        /// </summary>
        /// <param name="delta">Time elapsed from last <see cref="OnFixedUpdate(float)"/></param>
        public override void OnFixedUpdate(float delta)
        {
            PlayCurrentMovementSteps(delta);
        }




        System.Random rand = new System.Random();
        SwordMovementRecord currentlyPlaying = null;

        SwordMovementRecord.Frame[] currentSegment => currentlyPlaying.Loop.Frames;
        int currentFrameIndex = -2;

        void StartPlaying()
        {
            if (Records[CurrentUsecase].IsNullOrEmpty()) currentlyPlaying = null;
            else currentlyPlaying = recordsRandomizer.Next();
            currentFrameIndex = -1;
        }



        double deltaLeftower = 0f;
        void PlayCurrentMovementSteps(double delta)
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