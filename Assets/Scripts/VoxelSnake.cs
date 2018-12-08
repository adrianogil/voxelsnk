using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;

public enum VoxelSnakeDirection
{
    Up,
    Down,
    Left,
    Right,
    Forward,
    Backward
}

[RequireComponent(typeof(ImageSetter))]
public class VoxelSnake : MonoBehaviour
{
    [Header("World")]
    public int worldSizeX;

    public int worldSizeY;
    public int worldSizeZ;

    public int initialPlayerPositionX, initialPlayerPositionY, initialPlayerPositionZ;

    public float movementTime = 1.5f;

    [Header("Game Elements")]
    public Color backgroundColor;
    public Color playerColor;
    public Color foodColor;

    private ImageSetter mImageSetter;

    private int mCurrentX, mCurrentY, mCurrentZ;
    private int mDeltaX, mDeltaY, mDeltaZ;
    private int mLastX, mLastY, mLastZ;
    private int mTailX, mTailY, mTailZ;

    private Int3 mCurrent, mDelta, mLast, mTail, mLastDirection, mLastFoodPosition = -1 * Int3.one(), mWorldSize;

    private float lastMovementUpdate = 0f;

    public VoxelSnakeDirection currentSnakeDirection = VoxelSnakeDirection.Forward;

    public List<Int3> snakeBody;

    public bool shouldResetGridAtStart = false;

    public void ResetVoxelGridToBackgroundColor()
    {
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                for (int z = 0; z < worldSizeZ; z++)
                {
                    mImageSetter.SetPixelColor(x+z*worldSizeX, y, backgroundColor, false);
                }
            }
        }
    }

    void Start ()
    {
        mImageSetter = GetComponent<ImageSetter>();

        mCurrent.x = initialPlayerPositionX;
        mCurrent.y = initialPlayerPositionY;
        mCurrent.z = initialPlayerPositionZ;

        if (shouldResetGridAtStart)
            ResetVoxelGridToBackgroundColor();

        // Set initial player position
        mImageSetter.SetPixelColor(mCurrent.x + mCurrent.z*worldSizeX, mCurrent.y, playerColor);

        mLastDirection = new Int3(0, 0, 1);

        // mCurrent = new Int3(mCurrentX, mCurrentY, mCurrentZ);
        mWorldSize = new Int3(worldSizeX, worldSizeY, worldSizeZ);

        snakeBody = new List<Int3>();
        snakeBody.Add(mCurrent);

        PlaceFoodAtRandom();
    }

    // Update is called once per frame
    void Update ()
    {
        if (Time.time - lastMovementUpdate > movementTime)
        {
            if (mCurrent == mLastFoodPosition)
            {
                PlaceFoodAtRandom();
                snakeBody.Add(snakeBody[snakeBody.Count-1]);
            }

            UpdateAI();
            UpdateMovement();
            lastMovementUpdate = Time.time;
        }
    }

    Int3 GetDeltaByDirection(VoxelSnakeDirection dir)
    {
        if (dir == VoxelSnakeDirection.Forward)
        {
            return new Int3(0,0,1);
        }
        else if (dir == VoxelSnakeDirection.Backward)
        {
            return new Int3(0,0,-1);
        }
        else if (dir == VoxelSnakeDirection.Left)
        {
            return new Int3(-1,0,0);
        }
        else if (dir == VoxelSnakeDirection.Right)
        {
            return new Int3(1,0,0);
        }
        else if (dir == VoxelSnakeDirection.Down)
        {
            return new Int3(0,-1,0);
        }
        else if (dir == VoxelSnakeDirection.Up)
        {
            return new Int3(0,1,0);
        }

        // Up
        return new Int3(0,1,0);
    }

    void UpdateMovement()
    {
        mDelta = GetDeltaByDirection(currentSnakeDirection);

        SetVoxelColor(snakeBody[snakeBody.Count-1], backgroundColor);
        mCurrent = (mCurrent + mDelta) % mWorldSize;

        Int3 tmp1 = snakeBody[0];
        Int3 tmp2 = snakeBody[0];

        for (int i = 0; i < snakeBody.Count; i++)
        {
            if (i == 0)
            {
                snakeBody[i] = mCurrent;
            }
            else {
                tmp1 = snakeBody[i];
                snakeBody[i] = tmp2;
                tmp2 = tmp1;
            }
            SetVoxelColor(snakeBody[i], playerColor);
        }

        // mImageSetter.SetPixelColor(mLastX + mLastZ*worldSizeX, mLastY, backgroundColor);
        // mImageSetter.SetPixelColor(mCurrentX + mCurrentZ*worldSizeX, mCurrentY, playerColor);
    }



    void UpdateAI()
    {
        float bestUtilityValue = int.MaxValue;
        VoxelSnakeDirection bestDirection = VoxelSnakeDirection.Up;

        int utilityValue = 0;
        Int3 delta;

        Vector4 vBackgroundColor = backgroundColor;
        Vector4 vFoodColor = foodColor;

        foreach(VoxelSnakeDirection dir in Enum.GetValues(typeof(VoxelSnakeDirection)))
        {
            delta = GetDeltaByDirection(dir);

            Int3 nextPos = (mCurrent+delta)%mWorldSize;
            utilityValue = mLastFoodPosition.ManhattanDistance(nextPos);
            Vector4 vNextPosVoxelColor = GetVoxelColor(nextPos);

            if ((vBackgroundColor - vNextPosVoxelColor).sqrMagnitude > 0.001f &&
                (vFoodColor - vNextPosVoxelColor).sqrMagnitude > 0.001f)
            {
                utilityValue = int.MaxValue;
            }

            if (utilityValue < bestUtilityValue)
            {
                bestUtilityValue = utilityValue;
                bestDirection = dir;
            }
        }

        currentSnakeDirection = bestDirection;
    }

    public void PlaceFoodAtRandom()
    {
        mWorldSize = new Int3(worldSizeX, worldSizeY, worldSizeZ);

        if (mLastFoodPosition != (-1*Int3.one()))
        {
            Debug.Log("GilLog - VoxelSnake::PlaceFoodAtRandom " + GetVoxelColor(mLastFoodPosition));
            Vector4 vColor = GetVoxelColor(mLastFoodPosition);
            Vector4 fColor = foodColor;
            if ((vColor - fColor).sqrMagnitude < 0.001F)
            {
                Debug.Log("GilLog - VoxelSnake::PlaceFoodAtRandom - Lets reset food color in last pos");
                SetVoxelColor(mLastFoodPosition, backgroundColor);
            }
        }

        int max_steps = 100;
        int current_steps = 0;

        Vector4 vBackgroundColor = backgroundColor;
        Vector4 vCurrentPosColor;
        do
        {
            mLastFoodPosition = Int3.random(Int3.zero(), mWorldSize);
            vCurrentPosColor = GetVoxelColor(mLastFoodPosition);
            current_steps++;
        }
        while((vBackgroundColor - vCurrentPosColor).sqrMagnitude > 0.001 && current_steps < max_steps);
        SetVoxelColor(mLastFoodPosition, foodColor);
    }

    public void SetVoxelColor(Int3 pos, Color color)
    {
        mImageSetter.SetPixelColor(pos.x + pos.z*worldSizeX, pos.y, color);
    }

    public Color GetVoxelColor(Int3 pos)
    {
        return mImageSetter.GetPixelColor(pos.x + pos.z*worldSizeX, pos.y);
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(VoxelSnake))]
public class VoxelSnakeEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        VoxelSnake editorObj = target as VoxelSnake;

        if (editorObj == null) return;

        if (GUILayout.Button("Place Food At Random"))
        {
            editorObj.PlaceFoodAtRandom();
        }

        if (GUILayout.Button("Reset Grid"))
        {
            editorObj.ResetVoxelGridToBackgroundColor();
        }
    }

}
#endif