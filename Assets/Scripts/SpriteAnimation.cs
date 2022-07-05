using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour
{
    public int _FrameRate = 30;
    public float _Speed = 1f;
    public bool _loop = true;
    private Image mImage = null;

    private Sprite[] mSprites = null;
    private float mTimePerFrame = 0f;
    private float mElapsedTime = 0f;
    private int mCurrentFrame = 0;
    private void Start()
    {
        mImage = GetComponent<Image>();
        enabled = false;
        LoadSpriteSheet();
    }

    private void LateUpdate()
    {
        mElapsedTime += Time.deltaTime * _Speed;
        if (mElapsedTime >= mTimePerFrame)
        {
            mElapsedTime = 0f;
            ++mCurrentFrame;
            SetSprite();
            if (mCurrentFrame >= mSprites.Length)
            {
                if (_loop)
                {
                    mCurrentFrame = 0;
                }
                else
                    enabled = false;
            }
        }
    }

    private void SetSprite()
    {
        if (mCurrentFrame >= 0 && mCurrentFrame < mSprites.Length)
            mImage.sprite = mSprites[mCurrentFrame];
    }

    void LoadSpriteSheet()
    {
        mSprites = Resources.LoadAll<Sprite>("scan-spritesheet");
        if (mSprites != null && mSprites.Length > 0)
        {
            mTimePerFrame = 1f / _FrameRate;
            Play();
        }
    }
    public void Play()
    {
        enabled = true;
    }
}

