using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordInputRecordPlayer : ISwordInput
{
    public int currentRecordIndex = -1;
    public int recordsCount = 0;

    public KeyCode startPlayingKey = KeyCode.P;

    private List<IReadOnlyList<SwordInputRecorder.Frame>> records = new List<IReadOnlyList<SwordInputRecorder.Frame>>();

    private int currentFrameIndex = 0;

    private void Update()
    {
        if (Input.GetKeyDown(startPlayingKey) && records.Count > 0)
        {
            currentRecordIndex = (currentRecordIndex + 1)%records.Count;
            //Debug.Log($"Starting playing record nr. {currentRecordIndex}");
            resetRecord();
        }
    }
    private void FixedUpdate()
    {
        if (currentRecordIndex < 0 || currentRecordIndex >= recordsCount) return;
        if(++currentFrameIndex >= currentRecord.Count)
        {
            //Debug.Log($"Finished playing record {currentRecordIndex}. Replay duration: {Time.time - lastTimestamp}");
            resetRecord();
        }
    }

    private double lastTimestamp;
    private void resetRecord()
    {
        currentFrameIndex = -1;
        lastTimestamp = Time.timeAsDouble;
    }

    public void AddRecord(IReadOnlyList<SwordInputRecorder.Frame> toAdd)
    {
        records.Add(toAdd);
        recordsCount = records.Count;
    }

    private IReadOnlyList<SwordInputRecorder.Frame> currentRecord => currentRecordIndex<0?null: records[currentRecordIndex];
    private SwordInputRecorder.Frame? currentFrame => currentFrameIndex<0?null:currentRecord?[currentFrameIndex];

    public override Ray? GetInputRay() => currentFrame?.CursorRay is SerializableRay r ? transform.LocalToGlobal(r) : (Ray?)null;
    public override bool GetKey(KeyCode code) => currentFrame?.KeysPressed?.Contains(code)??false;
    public override bool GetKeyDown(KeyCode code) => GetKey(code) && (currentFrameIndex <= 0 || !currentRecord[currentFrameIndex - 1].KeysPressed.Contains(code));
    public override bool GetKeyUp(KeyCode code) => GetKey(code) && (currentFrameIndex >= (currentRecord.Count - 1) || !currentRecord[currentFrameIndex + 1].KeysPressed.Contains(code));
}
