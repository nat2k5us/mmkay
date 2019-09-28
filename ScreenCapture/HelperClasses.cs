using System;
using System.Runtime.InteropServices;
namespace ScapLIB
{
    //Enums
    public enum ScapVideoFormats { Default, FLV1, H263P, MPEG2, MPEG4, MSMPEG4v2, MSMPEG4v3, RAW, WMV1, WMV2 }
    public enum ScapImageFormats { Bmp, PNG, Jpeg, Tiff, Gif }
    //Core Data class
    internal class Databank
    {
        public int CapNumber;
        public byte[] Buff;
        public Databank()
        {
        }
        public Databank(int Frame, byte[] ImageBuffer)
        {
            CapNumber = Frame;
            Buff = ImageBuffer;
        }
    }
    //File Writer Class
    unsafe internal class FileIO : IDisposable
    {
        //Constants
        private const uint GENERIC_W = 0x40000000;
        private const uint CREATE_ALWAYS = 2;
        private const int BlockS = 65536;
        //Variables
        private GCHandle GCHbuff;
        private IntPtr pHan;
        private void* pBuff;
        //Init Methods
        public FileIO()
        {
            pHan = IntPtr.Zero;
        }
        public FileIO(Array Buffer)
        {
            FixBuffer(Buffer);
            pHan = IntPtr.Zero;
        }
        //Disposal and Finalizer Methods
        protected void Dispose(bool disposing)
        {
            Close();
            ReleaseBuffer();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~FileIO()
        {
            Dispose(false);
        }
        //Buffer Methods
        public void FixBuffer(Array Buffer)
        {
            ReleaseBuffer();
            GCHbuff = GCHandle.Alloc(Buffer, GCHandleType.Pinned);
            IntPtr pAddr = Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0);
            pBuff = (void*)pAddr.ToPointer();
        }
        public void ReleaseBuffer()
        {
            if (GCHbuff.IsAllocated)
            {
                GCHbuff.Free();
            }
        }
        //Write Mehtods
        public int Write(int BytesToWrite, string Filename)
        {
            int NumberOfBytesWritten;
            pHan = Kernel.CreateFile(Filename, GENERIC_W, 0, 0, CREATE_ALWAYS, 0, 0);
            if (pHan == System.IntPtr.Zero)
            {
                return -3;
            }
            if (!Kernel.WriteFile(pHan, pBuff, BytesToWrite, &NumberOfBytesWritten, 0))
            {
                return -2;
            }
            if (!Close())
            {
                return -1;
            }
            return NumberOfBytesWritten;
        }
        //Close Method
        public bool Close()
        {
            bool Success = true;
            if (pHan != IntPtr.Zero)
            {
                Success = Kernel.CloseHandle(pHan);
                pHan = IntPtr.Zero;
            }
            return Success;
        }
    }
}
