using UnityEngine;

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

    private ImageSetter mImageSetter;

    private int mCurrentX, mCurrentY, mCurrentZ;
    private int mDeltaX, mDeltaY, mDeltaZ;
    private int mLastX, mLastY, mLastZ;
    private int mTailX, mTailY, mTailZ;

    private Int3 mCurrent, mDelta, mLast, mTail, mLastDirection;

    private float lastMovementUpdate = 0f;

    void Start ()
    {
        mImageSetter = GetComponent<ImageSetter>();

        mCurrent.x = initialPlayerPositionX;
        mCurrent.y = initialPlayerPositionY;
        mCurrent.z = initialPlayerPositionZ;

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

        mImageSetter.SetPixelColor(mCurrent.x + mCurrent.z*worldSizeX, mCurrent.y, playerColor);

        mLastDirection = new Int3(0, 0, 1);
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            mDelta = new Int3(0,0,1);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            mDelta = new Int3(0,0,-1);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            mDelta = new Int3(-1,0,0);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            mDelta = new Int3(1,0,0);
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            mDelta = new Int3(0,-1,0);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            mDelta = new Int3(0,1,0);
        }

        if (Time.time - lastMovementUpdate > movementTime)
        {
            UpdateMovement();
        }
    }

    void UpdateMovement()
    {
        // mCurrent = (mCurrent + mDelta) % mWorldSize;

        mImageSetter.SetPixelColor(mLastX + mLastZ*worldSizeX, mLastY, backgroundColor);
        mImageSetter.SetPixelColor(mCurrentX + mCurrentZ*worldSizeX, mCurrentY, playerColor);
    }

}