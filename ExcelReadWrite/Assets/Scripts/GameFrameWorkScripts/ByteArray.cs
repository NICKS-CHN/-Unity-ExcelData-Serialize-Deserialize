using System;
using System.Text;
using UnityEngine;

public class ByteArray
{
    private byte[] _bytes;

    private bool _unCompressFlag;

    public ByteArray(byte[] bytes)
    {
        _bytes = bytes;
    }

    public byte[] bytes
    {
        get { return _bytes; }
    }

    public int Length
    {
        get { return _bytes.Length; }
    }

    public void UnCompress()
    {
        if (_unCompressFlag == false)
        {
            _unCompressFlag = true;
            _bytes = ZipLibUtils.Uncompress(_bytes);
        }
    }

    public void Compress()
    {
        _unCompressFlag = false;
        _bytes = ZipLibUtils.Compress(_bytes);
    }

    public string ToBase64String()
    {
        return Convert.ToBase64String(_bytes);
    }

    public Texture2D ToTexture2D()
    {
        if (_bytes != null)
        {
            var tex = new Texture2D(0, 0, TextureFormat.ARGB32, false);
            tex.LoadImage(_bytes);
            return tex;
        }

        return null;
    }

    public string ToUTF8String()
    {
        return Encoding.UTF8.GetString(_bytes);
    }

    public string ToEncodingString()
    {
        return Encoding.Default.GetString(_bytes);
    }

    #region CreateFrom
    public static ByteArray CreateFromUtf8(string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        return new ByteArray(bytes);
    }

    public static ByteArray CreateFromBase64(string str)
    {
        byte[] bytes = Convert.FromBase64String(str);
        return new ByteArray(bytes);
    }

    public static ByteArray CreateFromWWW(WWW www)
    {
        if (www != null && string.IsNullOrEmpty(www.error) && www.isDone)
        {
            return new ByteArray(www.bytes);
        }

        return null;
    }

    public static ByteArray Copy(ByteArray byteArray)
    {
        return new ByteArray(byteArray.bytes);
    }
    #endregion
}