using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif
public static class CKUtil
{
    public static string ToString_Clip(this int _value)
    {
        string _temp = "";
        if (_value < 10)
            _temp += "<color=#B0B0B0>0</color>";
        if (_value == 0)
            _temp += "<color=#E44F4C>";
        _temp += _value.ToString();
        _temp += "</color>";
        return _temp;
    }

    public static string ToString_Currency(this int _value)
    {
        string _temp = "$" + _value.ToString();
        return _temp;
    }
    public static string ToString_Duration(this int _value, bool _full = false)
    {
        int minutes = _value / 60;
        int seconds = _value % 60;
        string _temp = "";
        if (minutes > 0 || _full)
        {
            if (minutes < 10) _temp += "0";
            _temp += minutes.ToString() + ":";
        }
        if (seconds < 10) _temp += "0";
        _temp += seconds.ToString();
        return _temp;
    }
    public static string ToString_Duration(this float _value, bool _full = false)
    {
        return Mathf.FloorToInt(_value).ToString_Duration(_full);
    }

    public static string ToString_Distance(this float _value)
    {
        string _temp;
        if (_value > 600)
        {
            _temp = (Mathf.Floor(_value / 10) / 100).ToString();
            _temp += "km";
        }
        else if (_value > 60)
        {
            _temp = (Mathf.Floor(_value / 10) * 10).ToString();
            _temp += "m";
        }
        else
        {
            _temp = (Mathf.Floor(_value)).ToString();
            _temp += "m";
        }
        return _temp;
    }
    public static string ToString_Percentage(this float _value)
    {
        string _temp;
        _temp = Mathf.Round(_value * 100).ToString();
        _temp += "%";
        return _temp;
    }

    public static Vector3 GetRandomPoint(this Bounds _bound, bool _3D = true)
    {
        Vector3 _pos = _bound.center;
        Vector3 _e = _bound.extents;
        _pos += new Vector3(UnityEngine.Random.Range(-1f, 1f) * _e.x, 0, UnityEngine.Random.Range(-1f, 1f) * _e.z);
        if (_3D)
            _pos.y += UnityEngine.Random.Range(-1f, 1f) * _e.y;
        else
            _pos.y -= _e.y;
        return _pos;
    }
    public static string ToSprite(this string _string)
    {
        _string = "<sprite name=\"" + _string + "\">";
        return _string;
    }

    public static void Shuffle<T>(this IList<T> ts)
    {
        ts.Shuffle(new Unity.Mathematics.Random());
    }
    public static void Shuffle<T>(this IList<T> ts, Unity.Mathematics.Random Random_Seeded)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random_Seeded.NextInt(count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
    public static T GetRandom<T>(this IList<T> ts)
    {
        if (ts.Count == 0)
        {
            Debug.LogWarning("List Empty");
            return default(T);
        }
        return ts.GetRandom(new Unity.Mathematics.Random(0x6E624EB7u));
    }
    public static T GetRandom<T>(this IList<T> ts, Unity.Mathematics.Random Random_Seeded)
    {
        int _ran = Random_Seeded.NextInt(0, ts.Count);
        return ts[_ran];
    }

    public static T[] Reverse<T>(this T[] ts)
    {
        T[] _temp = new T[ts.Length];
        int j = ts.Length - 1;
        for (int i = 0; i < ts.Length; i++)
        {
            _temp[i] = ts[j];
            j--;
        }
        return _temp;
    }
    public static Mesh Clone(this Mesh m)
    {
        Mesh _m = new Mesh()
        {
            vertices = m.vertices,
            triangles = m.triangles,
            normals = m.normals,
            tangents = m.tangents,
            bounds = m.bounds,
            uv = m.uv,
        };
        return _m;
    }
    public static bool IsValid(this Bounds _bounds, int _dimReq = 2)
    {
        int _dim = 0;
        if (_bounds.extents.x > 0.01f)
            _dim++;
        if (_bounds.extents.y > 0.01f)
            _dim++;
        if (_bounds.extents.z > 0.01f)
            _dim++;
        return _dim >= _dimReq;
    }

    public static void PlayClip(this Animator _anim, string _name)
    {
        _anim.Play(_name);
    }

    public static bool FollowObject(this RectTransform _holder, Canvas C_canvas, Transform _target)
    {
        return _holder.FollowObject(C_canvas, _target.position);
    }
    public static bool FollowObject(this RectTransform _holder, Canvas C_canvas, Vector3 _target)
    {
        Vector3 _tarPos = Camera.main.WorldToScreenPoint(_target);

        _tarPos /= C_canvas.scaleFactor;

        Vector2 _size = _holder.sizeDelta / 2;
        Vector2 _xBounds = new Vector2(_size.x, (Screen.width / C_canvas.scaleFactor) - _size.x);
        Vector2 _yBounds = new Vector2(_size.y, (Screen.height / C_canvas.scaleFactor) - _size.y);

        bool _offscreen =
            _tarPos.x < _xBounds.x ||
            _tarPos.x > _xBounds.y ||
            _tarPos.y < _yBounds.x ||
            _tarPos.y > _yBounds.y;

        _tarPos.x = Mathf.Clamp(_tarPos.x, _xBounds.x, _xBounds.y);
        _tarPos.y = Mathf.Clamp(_tarPos.y, _yBounds.x, _yBounds.y);


        if (_tarPos.z < 0)
        {
            Vector2 _checker = _tarPos;
            _checker.x = 1 - Mathf.Abs(_checker.x / (Screen.width / 2) - 1);
            _checker.y = 1 - Mathf.Abs(_checker.y / (Screen.height / 2) - 1);
            if (_checker.x >= _checker.y)
            {
                if (_tarPos.x > Screen.width / 2) _tarPos.x = _xBounds.y;
                else _tarPos.x = _xBounds.x;
            }
            else
            {
                if (_tarPos.y > Screen.height / 2) _tarPos.y = _yBounds.y;
                else _tarPos.y = _yBounds.x;
            }
            _offscreen = true;
        }

        _holder.anchoredPosition = _tarPos;
        return _offscreen;
    }
    public static void DeleteChildren (this Transform _transform, bool _immediate = false)
    {
        for (int i = _transform.childCount - 1; i >= 0; i--)
        {
            if (_immediate) GameObject.DestroyImmediate(_transform.GetChild(i).gameObject);
            else GameObject.Destroy(_transform.GetChild(i).gameObject);
        }
    }
    public static bool Check(this LayerMask _mask, int _layer)
    {
        return (_mask & (1 << _layer)) != 0;
    }
#if UNITY_EDITOR
    public enum SaveTextureFileFormat { PNG, JPG, EXR, TGA};
    static public bool SaveToFile(this Texture source,
                                         string filePath,
                                         System.Action<bool, Texture2D> done = null,
                                         int width = -1,
                                         int height = -1,
                                         SaveTextureFileFormat fileFormat = SaveTextureFileFormat.PNG,
                                         int jpgQuality = 95,
                                         bool asynchronous = false)
    {
        // check that the input we're getting is something we can handle:
        if (!(source is Texture2D || source is RenderTexture))
        {
            Debug.LogError("Unsupported Type");
            done?.Invoke(false, null);
            return false;
        }

        // use the original texture size in case the input is negative:
        if (width < 0 || height < 0)
        {
            width = source.width;
            height = source.height;
        }

        // resize the original image:
        var resizeRT = RenderTexture.GetTemporary(width, height, 0);
        Graphics.Blit(source, resizeRT);

        // create a native array to receive data from the GPU:
        var narray = new NativeArray<byte>(width * height * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        // request the texture data back from the GPU:
        var request = AsyncGPUReadback.RequestIntoNativeArray(ref narray, resizeRT, 0, (AsyncGPUReadbackRequest request) =>
        {
            // if the readback was successful, encode and write the results to disk
            if (!request.hasError)
            {
                NativeArray<byte> encoded;

                switch (fileFormat)
                {
                    case SaveTextureFileFormat.EXR:
                        encoded = ImageConversion.EncodeNativeArrayToEXR(narray, resizeRT.graphicsFormat, (uint)width, (uint)height);
                        break;
                    case SaveTextureFileFormat.JPG:
                        encoded = ImageConversion.EncodeNativeArrayToJPG(narray, resizeRT.graphicsFormat, (uint)width, (uint)height, 0, jpgQuality);
                        break;
                    case SaveTextureFileFormat.TGA:
                        encoded = ImageConversion.EncodeNativeArrayToTGA(narray, resizeRT.graphicsFormat, (uint)width, (uint)height);
                        break;
                    default:
                        encoded = ImageConversion.EncodeNativeArrayToPNG(narray, resizeRT.graphicsFormat, (uint)width, (uint)height);
                        break;
                }

                System.IO.File.WriteAllBytes(filePath, encoded.ToArray());
                encoded.Dispose();
            }
                narray.Dispose();

        });

        if (!asynchronous)
            request.WaitForCompletion();
        // notify the user that the operation is done, and its outcome.
        AssetDatabase.Refresh();
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
        texture.alphaIsTransparency = true;
        done?.Invoke(!request.hasError, texture);
        return !request.hasError;
    }
#endif
}
