using System.IO;
using UnityEngine;
#if UNITY_EDITOR  
#if ! UNITY_WEBPLAYER
using UnityEditor;
#endif
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngineEx;
using UnityEngineExJSON;

public class heroSprEditor : MonoBehaviour
{
    [SerializeField] Transform _heroModel;
    [SerializeField] Animator _heroAni;
    [SerializeField] Transform _heroWeapon;
    [SerializeField] Transform _heroWeaponAim;

    [SerializeField] AnimationClip[] _clips;
    Texture2D tex, texfinMatr;
    List<Color[]> pix = new List<Color[]>();
    Color[] pixels;
//192
    const int _sprSize = (int)(128 * 1.5f); const int _sprHalfSize = _sprSize >> 1;
    int numberOfColumns = 6, numberOfFrames = 30;//24;
    int largDim, maxDim;
int anglePart = 60;

    void OnEnable()
    {
        pixels = new Color[_sprSize * numberOfFrames * _sprSize];
        tex = new Texture2D(_sprSize, _sprSize, TextureFormat.ARGB32, false);
        texfinMatr = new Texture2D(numberOfColumns * _sprSize, _sprSize * numberOfFrames / numberOfColumns, TextureFormat.ARGB32, false);

        findTextureMaxDimenion();
    }

    public void MakeSprite() { Debug.Log("start makeSprite_"); StartCoroutine(makeSprite_()); }
    IEnumerator makeSprite_()
    {
        _heroModel.gameObject.SetActive(true);
        Camera.main.backgroundColor = new Color(0, 0, 0, 0);
        float aix = 45; int gap = 5;
        for (int t = -anglePart; t <= anglePart; t += gap)
        {
            pix.Clear();
            _heroModel.localEulerAngles = new Vector3(0, aix+t, 0);

            for (int i = 0; i < numberOfFrames; ++i)
            {
                _heroAni.speed = 0;
                _heroAni.SetLayerWeight(0, 1f);
                _heroAni.Play("walk", 0, (float)(i) / numberOfFrames);
                yield return new WaitForEndOfFrame();

                Vector2 scrPt = uis.WorldToCanvas_L(_heroModel.localPosition + new Vector3(0, 1, 0));
                tex.ReadPixels(new Rect((int)scrPt.x - _sprHalfSize, (int)scrPt.y - _sprHalfSize,
                                        (int)scrPt.x + _sprHalfSize, (int)scrPt.y + _sprHalfSize), 0, 0);
                tex.Apply();
                pix.Add(tex.GetPixels());
            }
            yield return new WaitForEndOfFrame();
            createFile((t + anglePart) / gap);
        }
        Debug.Log("done makeSprite_"); yield return null;
    }

    public bool invertSpriteY = true;
    void createFile(int axi_Idx)
    {
        int matrixRes = _sprSize * numberOfColumns;

        for (int i = 0; i < pixels.Length; i++)
        {
            int ind1 = i + 1;
            int imax1 = matrixRes;

            int j1 = (ind1 + imax1 - 1) / imax1;
            int i1 = ind1 - (j1 - 1) * imax1;

            int i2 = ((i1 - 1) / _sprSize) + 1;
            int j2 = ((j1 - 1) / _sprSize) + 1;

            int jinv2 = numberOfFrames / numberOfColumns - ((j1 - 1) / _sprSize) + 1;

            int ind2 = (j2 - 1) * numberOfColumns + i2;

            int indinv2 = (jinv2 - 2) * numberOfColumns + i2;

            int iof2 = _sprSize * (i2 - 1);
            int jof2 = _sprSize * (j2 - 1);

            int i3 = i1 - iof2;
            int j3 = j1 - jof2;

            int ind3 = (j3 - 1) * _sprSize + i3;

            if (invertSpriteY == false)
                pixels[i] = pix[ind2 - 1][ind3 - 1];
            else
                pixels[i] = pix[indinv2 - 1][ind3 - 1];
        }

        texfinMatr.SetPixels(pixels);
        texfinMatr.Apply();

        var bytes = texfinMatr.EncodeToPNG();
        FileIO.OnPrj.CreateFile_sprite("hero" + axi_Idx + ".png", bytes);
    }

    void findTextureMaxDimenion()
    {
        if (_sprSize * numberOfColumns > _sprSize * numberOfFrames / numberOfColumns)
        {
            largDim = _sprSize * numberOfColumns;
        }
        else
        {
            largDim = _sprSize * numberOfFrames / numberOfColumns;
        }

        if (largDim <= 32)
        {
            maxDim = 32;
        }
        else if ((largDim > 32) && (largDim <= 64))
        {
            maxDim = 64;
        }
        else if ((largDim > 64) && (largDim <= 128))
        {
            maxDim = 128;
        }
        else if ((largDim > 128) && (largDim <= 256))
        {
            maxDim = 256;
        }
        else if ((largDim > 256) && (largDim <= 512))
        {
            maxDim = 512;
        }
        else if ((largDim > 512) && (largDim <= 1024))
        {
            maxDim = 1024;
        }
        else if ((largDim > 1024) && (largDim <= 2048))
        {
            maxDim = 2048;
        }
        else if ((largDim > 2048) && (largDim <= 4096))
        {
            maxDim = 4096;
        }

        else
        {
            print("Maximum resolution size is exceded! Resize texture and try again");
            maxDim = 1024;
        }

        Debug.Log(" atlas Size " + maxDim);
    }



    public void MakeAimPoses() { Debug.Log("start makeAimPoses_"); StartCoroutine(makeAimPoses_()); }
    IEnumerator makeAimPoses_()
    {
        JSONObject jsRoot = new JSONObject(); // for return jsRoot.Print()
        JSONObject jsProj = NewJSONObj.With(jsK.Proj, jsRoot, JSONObject.Type.OBJECT);

        JSONObject jsAimgaps = NewJSONObj.With(jsK.AimGaps, jsProj, JSONObject.Type.ARRAY);

        _heroModel.gameObject.SetActive(true);
        Camera.main.backgroundColor = new Color(0, 0, 0, 0);
        float aix = 45;  int gap = 5; 
        for (int t = -anglePart; t <= anglePart; t += gap)
        {
            pix.Clear();
            _heroModel.localEulerAngles = new Vector3(0, aix+t, 0);

            int idxDir = (t + anglePart) / gap;
            for (int i = 0; i < numberOfFrames; ++i)
            {
                _heroAni.speed = 0;
                _heroAni.SetLayerWeight(0, 1f);
                _heroAni.Play("walk", 0, (float)(i) / numberOfFrames);
                yield return null;
                yield return new WaitForEndOfFrame();
                ///*
                JSONObject jsO = NewJSONObj.With(jsAimgaps, JSONObject.Type.ARRAY);
                jsO.Add(idxDir);
                jsO.Add(i);
                jsO.Add(_heroWeaponAim.position.x - _heroModel.position.x);
                jsO.Add(_heroWeaponAim.position.z - _heroModel.position.z);
                Vector3 gapDir = (_heroWeaponAim.position - _heroWeapon.position).normalized;
                jsO.Add(gapDir.x);
                jsO.Add(gapDir.z);
                //*/
            }

            yield return new WaitForEndOfFrame();
            /*
            JSONObject jsO = NewJSONObj.With(jsAimgaps, JSONObject.Type.ARRAY);
            jsO.Add(idxDir);
            jsO.Add(_heroWeaponAim.position.x - _heroModel.position.x);
            jsO.Add(_heroWeaponAim.position.z - _heroModel.position.z);

            gapDir = (_heroWeaponAim.position - _heroWeapon.position).normalized;
            JSONObject js1 = NewJSONObj.With(jsAimdirs, JSONObject.Type.ARRAY);
            js1.Add(idxDir);
            js1.Add(gapDir.x);
            js1.Add(gapDir.z);
            */
        }

        FileIO.OnPrj.CreateFile_prjss(jsRoot.Print(), "txt");

        jsRoot.Clear();
        Debug.Log("done makeAimPoses_"); yield return new WaitForEndOfFrame();
    }

}