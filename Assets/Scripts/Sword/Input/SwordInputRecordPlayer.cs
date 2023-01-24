using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordInputRecordPlayer : ISwordInput
{
    public IReadOnlyList<IReadOnlyList<SwordInputRecorder.FrameImage>> records;



    private int currentRecordIndex = 0, currentFrameIndex = 0;
    private void FixedUpdate()
    {
    }



    private SwordInputRecorder.FrameImage? currentFrame => records[currentRecordIndex][currentFrameIndex];

    public override Ray? GetInputRay() => currentFrame==null?(Ray?)null:transform.LocalToGlobal(currentFrame.Value.InputRayLocal.Value);
    public override bool GetKey(KeyCode code) => currentFrame?.Keys?.Contains(code)??false;
    public override bool GetKeyDown(KeyCode code) => currentFrame?.KeysDown?.Contains(code) ?? false;
    public override bool GetKeyUp(KeyCode code) => currentFrame?.KeysUp?.Contains(code) ?? false;
}
