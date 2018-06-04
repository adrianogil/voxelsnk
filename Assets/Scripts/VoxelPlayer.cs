using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ImageSetter))]
public class VoxelPlayer : MonoBehaviour
{

	[Header("World")] 
	public int worldSizeX;

	public int worldSizeY;
	public int worldSizeZ;

	public int initialPlayerPositionX, initialPlayerPositionY, initialPlayerPositionZ;

	[Header("Game Elements")] 
	public Color backgroundColor;
	public Color playerColor;
	
	private ImageSetter mImageSetter;

	private int mCurrentX, mCurrentY, mCurrentZ;
	private int mLastX, mLastY, mLastZ;

	void Start ()
	{
		mImageSetter = GetComponent<ImageSetter>();

		mCurrentX = initialPlayerPositionX;
		mCurrentY = initialPlayerPositionY;
		mCurrentZ = initialPlayerPositionZ;

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
		
		mImageSetter.SetPixelColor(mCurrentX + mCurrentZ*worldSizeX, mCurrentY, playerColor);
	}

	// Use this for initialization

	// Update is called once per frame
	void Update ()
	{
		mLastX = mCurrentX;
		mLastY = mCurrentY;
		mLastZ = mCurrentZ;
		
		if (Input.GetKeyUp(KeyCode.W))
		{
			mCurrentZ = (mCurrentZ + 1) % worldSizeZ;
		}
		if (Input.GetKeyUp(KeyCode.S))
		{
			mCurrentZ = (worldSizeZ + mCurrentZ - 1) % worldSizeZ;
		}
		if (Input.GetKeyUp(KeyCode.A))
		{
			mCurrentX = (worldSizeX + mCurrentX - 1) % worldSizeX;
		}
		if (Input.GetKeyUp(KeyCode.D))
		{
			mCurrentX = (mCurrentX + 1) % worldSizeX;
		}
		if (Input.GetKeyUp(KeyCode.Q))
		{
			mCurrentY = (worldSizeY + mCurrentY - 1) % worldSizeY;
		}
		if (Input.GetKeyUp(KeyCode.E))
		{
			mCurrentY = (mCurrentY + 1) % worldSizeY;
		}

		if (mLastX != mCurrentX ||
		    mLastY != mCurrentY ||
		    mLastZ != mCurrentZ)
		{
			mImageSetter.SetPixelColor(mLastX + mLastZ*worldSizeX, mLastY, backgroundColor);
			mImageSetter.SetPixelColor(mCurrentX + mCurrentZ*worldSizeX, mCurrentY, playerColor);	
		}
	}
}
