using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Xaml.Input;

namespace GoldstarrTrading.Classes
{
    [StructLayout(LayoutKind.Sequential)]
    struct Header
    {
        public int size;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Name;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct OrderStruct
    {
        public int OrderedAmount;
        public int Amount;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string OrderDate;


        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string CustomerName;



        // Merch

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string ProductName;


        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Supplier;


    }

    static class FileManager
    {
        static StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        static StorageFile file;

        const string FileName = "Data.group4";

        public static async void SaveToFile<T>(ObservableCollection<T> collection)
        {
            try
            {
                file = await storageFolder.GetFileAsync(FileName);
            }
            catch (FileNotFoundException)
            {
                file = await storageFolder.CreateFileAsync(FileName);

            }

            Header h = new Header();
            h.Name = typeof(T).Name;
            //h.size = Marshal.SizeOf(collection);



            //OrderModel om = (OrderModel)Convert.ChangeType(collection, typeof(OrderModel));

            //string s = new string(h.Name);
            //Debug.WriteLine(s);

            switch (h.Name)
            {
                case "OrderModel":
                    HandleOrderModel(collection as ObservableCollection<OrderModel>);
                    break;
            }

            


           // byte[] buffer = ObjectToByteArray(collection);


            //BytesToFile(buffer, h);

            //await FileIO.WriteBytesAsync(file, buffer);
        }

        private static void HandleOrderModel(ObservableCollection<OrderModel> order)
        {
            OrderStruct orderStruct = new OrderStruct();
            for (int i = 0; i < order.Count; i++)
            {
                orderStruct.CustomerName = order[i].CustomerName;
                orderStruct.OrderedAmount = order[i].OrderedAmount;
                orderStruct.Amount = 4;
                orderStruct.OrderDate = "1111";
                orderStruct.ProductName = "Test product name";
                orderStruct.Supplier = "Test supp";

            }

            Header h = new Header();
            h.Name = "OrderModel";
            h.size = Marshal.SizeOf(orderStruct);

            byte[] buffer = ObjectToByteArray(orderStruct, h);
            //BytesToFile(buffer);
        }


        private static void BytesToFile(byte[] buffer)
        {
            try
            {
                using(var fs = new FileStream(file.Path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    BinaryWriter bw = new BinaryWriter(fs, Encoding.UTF8);
                    bw.Write(buffer, 0, buffer.Length);
                    //fs.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public static byte[] ObjectToByteArray(object obj, object h)
        {
            BinaryFormatter bf = new BinaryFormatter();
            //using (var ms = new MemoryStream())
            //{
            //    bf.Serialize(ms, h);
            //    //bf. Serialize(ms, obj);
            //    return ms.ToArray();
            //}

            using (var fs = new FileStream(file.Path, FileMode.Append, FileAccess.Write))
            {

                int Length = Marshal.SizeOf(h);
                byte[] Bytes = new byte[Length];

                IntPtr Handle = Marshal.AllocHGlobal(Length);

                Marshal.StructureToPtr(h, Handle, true);
                Marshal.Copy(Handle, Bytes, 0, Length);

                Marshal.FreeHGlobal(Handle);
                fs.Write(Bytes, 0, Length);
                int oldLength = Length;
                fs.Flush();

                Length = Marshal.SizeOf(obj);
                Bytes = new byte[Length];
                Handle = Marshal.AllocHGlobal(Length);


                Marshal.StructureToPtr(obj, Handle, true);
                Marshal.Copy(Handle, Bytes, 0, Length);

                Marshal.FreeHGlobal(Handle);
                //fs.Position = (long)oldLength;
                //fs.Seek(oldLength, SeekOrigin.Begin);
                fs.Write(Bytes, 0, Length);
            }

            return new byte[2];
        }
        public static async void LoadAllDataFromFile()
        {

            try
            {
                file = await storageFolder.GetFileAsync(FileName);
            }
            catch (FileNotFoundException)
            {
                file = await storageFolder.CreateFileAsync(FileName);

            }

            try
            {
                using (var fs = new FileStream(file.Path, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader br = new BinaryReader(fs, Encoding.UTF8);
                    BinaryFormatter bf = new BinaryFormatter();


                    int headerSize = Marshal.SizeOf(typeof(Header));
                    byte[] headerChar = new byte[headerSize];
                    br.Read(headerChar, 0, (int)headerSize);

                    Header h = new Header();

                    MemoryStream ms = new MemoryStream(headerChar);

                    //h = (Header)bf.Deserialize(ms);


                    //int len = Marshal.SizeOf(obj);

                    //IntPtr i = Marshal.AllocHGlobal(len);

                    //Marshal.Copy(bytearray, 0, i, len);

                    //obj = Marshal.PtrToStructure(i, obj.GetType());

                    //Marshal.FreeHGlobal(i);

                    GCHandle handle = GCHandle.Alloc(headerChar, GCHandleType.Pinned);
                    try
                    {
                        h = (Header)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Header));
                    }
                    finally
                    {
                        handle.Free();
                    }



                    Debug.WriteLine("Hsd");

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
