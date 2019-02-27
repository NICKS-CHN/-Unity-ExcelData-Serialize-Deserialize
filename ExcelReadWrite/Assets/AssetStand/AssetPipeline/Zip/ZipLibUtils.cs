using System;
using UnityEngine;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Core;

public class ZipLibUtils
{
    static ZipLibUtils()
    {
        ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
    }

    public static bool CompressFile(string source, string desc)
    {
        Func<byte[], byte[]> func = Compress;
        return _OperateFile(source, desc, func);
    }

    public static bool UnCompressFile(string source, string desc)
    {
        Func<byte[], byte[]> func = Uncompress;
        return _OperateFile(source, desc, func);
    }


    private static bool _OperateFile(string source, string desc, Func<byte[], byte[]> operation)
    {
        FileStream sourceFs = null;
        try
        {
            sourceFs = new FileStream(source, FileMode.Open);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }

        if (sourceFs != null)
        {
            byte[] bytes = new byte[sourceFs.Length];
            sourceFs.Read(bytes, 0, bytes.Length);
            sourceFs.Close();

            bytes = operation(bytes);
            try
            {
                FileStream descFs = new FileStream(desc, FileMode.OpenOrCreate);
                descFs.Write(bytes, 0, bytes.Length);
                descFs.Close();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        return true;
    }

    private static Deflater _deflater;
    private static Deflater GetDeflater()
    {
        if (_deflater == null)
            _deflater = new Deflater();
        else
            _deflater.Reset();

        return _deflater;
    }

    public static byte[] Compress(byte[] input)
    {
        if (input == null || input.Length == 0)
        {
            Debug.LogError("Compress error inputBytes Len = 0");
            return input;
        }

        // Create the compressor with highest level of compression  
        Deflater compressor = GetDeflater();
        compressor.SetLevel(Deflater.BEST_COMPRESSION);

        // Give the compressor the data to compress  
        compressor.SetInput(input);
        compressor.Finish();

        /* 
         * Create an expandable byte array to hold the compressed data. 
         * You cannot use an array that's the same size as the orginal because 
         * there is no guarantee that the compressed data will be smaller than 
         * the uncompressed data.
         */
        MemoryStream result = new MemoryStream(input.Length);

        // Compress the data  
        byte[] buffer = new byte[1024];
        while (!compressor.IsFinished)
        {
            int count = compressor.Deflate(buffer);
            result.Write(buffer, 0, count);
        }

        // Get the compressed data  
        return result.ToArray();
    }

    private static Inflater _inflater;
    private static Inflater GetInflater()
    {
        if (_inflater == null)
            _inflater = new Inflater();
        else
            _inflater.Reset();

        return _inflater;
    }

    public static byte[] Uncompress(byte[] input)
    {
        if (input == null || input.Length == 0)
        {
            Debug.LogError("Uncompress error inputBytes Len = 0");
            return input;
        }

        Inflater decompressor = GetInflater();
        decompressor.SetInput(input);

        // Create an expandable byte array to hold the decompressed data  
        MemoryStream result = new MemoryStream(input.Length);

        // Decompress the data  
        byte[] buffer = new byte[4096];
        while (!decompressor.IsFinished)
        {
            int count = decompressor.Inflate(buffer);
            result.Write(buffer, 0, count);
        }

        //#region 包解压测试代码
        //TotalUnCompressCount++;
        //string hint = "input:" + StringHelper.FormatBytes(input.Length) + "\noutput:" +
        //              StringHelper.FormatBytes(result.Length);
        //if (input.Length > result.Length)
        //{
        //    MaxErrorLength = Math.Max(MaxErrorLength, result.Length);
        //    ErrorCount++;
        //   Debug.LogError("<color=red>" + hint + "</color>");
        //}
        //else
        //{
        //   Debug.LogError(hint);
        //}
        //#endregion

        // Get the decompressed data  
        return result.ToArray();
    }

}

