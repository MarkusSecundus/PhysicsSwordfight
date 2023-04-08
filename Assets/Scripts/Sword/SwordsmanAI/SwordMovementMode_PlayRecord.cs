using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwordMovementMode_PlayRecord : SwordMovement.Submodule
{
    public IDictionary<SwordRecordUsecase, IReadOnlyList<SwordMovementRecord>> Records { protected get; set; }
    public float PlaySpeed = 1f;

    private SwordRecordUsecase _currentUsecase = SwordRecordUsecase.Idle;
        public SwordRecordUsecase CurrentUsecase { get=>_currentUsecase; set {
            if (_currentUsecase == value) return;
            _currentUsecase = value;
            StartPlaying();
        } }



    protected override void OnStart(bool wasForced=false)
    {
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
        currentlyPlaying = rand.NextElement(Records[CurrentUsecase]);
        currentFrameIndex = -1;
    }



    double deltaLeftower = 0f;
    private void PlayCurrentMovementSteps(double delta)
    {
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
