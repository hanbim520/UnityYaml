/* Created by NavyZhang  
 * mail:710605420@qq.com
 * Welcome to exchange ideas
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class Buffer
{
    public int id;
    public byte[] data;
}

public class ThreadBuffer
{
    static object lock_obj = new object();
    static List<Buffer> BufferList = new List<Buffer>();
    static List<byte[]> BufferDataList = new List<byte[]>();

    public static byte[] GetBuffer()
    {
        int id = Thread.CurrentThread.ManagedThreadId;
        lock(lock_obj)
        {
            for(int i =0;i< BufferList.Count;++i)
            {
                if(BufferList[i].id == id)
                {
                    return BufferList[i].data;
                }
            }
        }
        Buffer buffer = new Buffer();
        buffer.id = id;
        buffer.data = AllocBuffer();

        lock(lock_obj)
        {
            BufferList.Add(buffer);
        }
        return buffer.data;
    }

    public static void ClearAllBuffer()
    {
        lock(lock_obj)
        {
            for(int i = 0;i < BufferList.Count;++i)
            {
                BufferDataList.Add(BufferList[i].data);
            }
            BufferDataList.Clear();
        }
    }

    public static void DestroyAllBuffer()
    {
        ClearAllBuffer();
        lock(lock_obj)
        {
            BufferDataList.Clear();
        }
    }

    static byte[] AllocBuffer()
    {
        byte[] buffer = null;
        lock(lock_obj)
        {
            if(BufferDataList.Count > 0)
            {
                int id = BufferDataList.Count - 1;
                buffer = BufferDataList[id];
                BufferDataList.RemoveAt(id);
            }
        }

        if(buffer == null)
        {
#if UNITY_EDITOR
            return new byte[50 * 1048576];
#else
            return new byte[5 * 1048576];
#endif
        }
        return buffer;
    }

    static void FreeBuffer(byte[] buffer)
    {
        if (buffer == null) return;
        lock(lock_obj)
        {
            BufferDataList.Add(buffer);
        }
    }
}
