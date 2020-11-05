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
using Windows.Graphics.Printing.OptionDetails;
using Windows.Storage;
using Windows.UI.Xaml.Input;

namespace GoldstarrTrading.Classes
{

    //Todo: Try to read file content, both header and orderstruct

    [StructLayout(LayoutKind.Sequential)]
    struct Header
    {
        public int size;

        public int amount;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 120)]
        public string Name;
    }

    [StructLayout(LayoutKind.Sequential)]
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
            OrderStruct[] orderStruct = new OrderStruct[order.Count];
            for (int i = 0; i < order.Count; i++)
            {
                orderStruct[i].CustomerName = order[i].CustomerName;
                orderStruct[i].OrderedAmount = order[i].OrderedAmount;
                orderStruct[i].Amount = 4;
                orderStruct[i].OrderDate = DateTime.Now.ToString();
                orderStruct[i].ProductName = "Test product name";
                orderStruct[i].Supplier = "Test supp";

            }

            Header h = new Header();
            h.Name = typeof(OrderStruct).Name;
            h.size = Marshal.SizeOf(typeof(OrderStruct));
            h.amount = order.Count;

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


        public static byte[] ObjectToByteArray(OrderStruct[] obj, object h)
        {
            BinaryFormatter bf = new BinaryFormatter();
            //using (var ms = new MemoryStream())
            //{
            //    bf.Serialize(ms, h);
            //    //bf. Serialize(ms, obj);
            //    return ms.ToArray();
            //}

            using (var fs = new FileStream(file.Path, FileMode.Open, FileAccess.Write))
            {

                // Write header to file
                int Length = Marshal.SizeOf(h);
                byte[] Bytes = new byte[Length];

                IntPtr Handle = Marshal.AllocHGlobal(Length);

                Marshal.StructureToPtr(h, Handle, true);
                Marshal.Copy(Handle, Bytes, 0, Length);

                Marshal.FreeHGlobal(Handle);
                fs.Write(Bytes, 0, Length);

                Header header = ((Header)h);

                for (int i = 0; i < header.amount; i++)
                {
                    fs.Flush();
                    WriteStructArrayToFile(fs, obj[i], header.size);
                }
            }

            return new byte[2];
        }


        private static void WriteStructArrayToFile(FileStream fs, object theStruct, int length)
        {
            byte[] Bytes = new byte[length];

            length = Marshal.SizeOf(theStruct);

            IntPtr Handle = Marshal.AllocHGlobal(length);


            Marshal.StructureToPtr(theStruct, Handle, true);
            Marshal.Copy(Handle, Bytes, 0, length);

            Marshal.FreeHGlobal(Handle);
            //fs.Position = (long)oldLength;
            //fs.Seek(oldLength, SeekOrigin.Begin);
            fs.Write(Bytes, 0, length);
        }


        public static async void LoadAllDataFromFile(ViewModel vm)
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

                    // Always get header first
                    Header h = GetHeader(br);


                    Type type = Type.GetType(h.Name);

                    while (fs.Position < fs.Length)
                    {
                        switch (h.Name)
                        {
                            case "OrderStruct":
                                ReadOrderStruct(br, h, vm);
                                break;
                        }
                    
                    }

                    Debug.WriteLine("Hsd");

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static Header GetHeader(BinaryReader br )
        {
            int headerSize = Marshal.SizeOf(typeof(Header));
            byte[] headerBytes = new byte[headerSize];
            br.Read(headerBytes, 0, (int)headerSize);

            Header h = new Header();
            GCHandle handle = GCHandle.Alloc(headerBytes, GCHandleType.Pinned);
            try
            {
                h = (Header)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Header));
            }
            finally
            {
                handle.Free();
            }
            return h;
        }

        private static void ReadOrderStruct(BinaryReader br, Header h, ViewModel vm)
        {
            OrderStruct[] os = new OrderStruct[h.amount];
            for (int i = 0; i < h.amount; i++)
            {
                os[i] = ReadStructData<OrderStruct>(br, h);
            }

            AddOrderStructToList(os, vm);
        }

        private static void AddOrderStructToList(OrderStruct[] os, ViewModel vm)
        {
            for (int i = 0; i < os.Length; i++)
            {
                OrderModel om = new OrderModel
                {
                    CustomerName = os[i].CustomerName,
                    OrderedAmount = os[i].OrderedAmount,
                    OrderDate = DateTime.Parse(os[i].OrderDate)
                };
                MerchandiseModel mm = new MerchandiseModel
                {
                    Amount = os[i].Amount,
                    ProductName = os[i].ProductName,
                    Supplier = os[i].Supplier
                };

                om.Merch = mm;
                vm.Order.Add(om);
            }
        }


        private static T ReadStructData<T>(BinaryReader br, Header h )
        {
            //T os = default(T);

            int structSize = h.size;
            byte[] bytes = new byte[structSize];

            br.Read(bytes, 0, structSize);

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            
            T theStruct = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
           
            handle.Free();

            return theStruct;
        }

    }
}
/*
                   //MemoryStream ms = new MemoryStream(headerChar);

                   //h = (Header)bf.Deserialize(ms);


                   //int len = Marshal.SizeOf(obj);

                   //IntPtr i = Marshal.AllocHGlobal(len);

                   //Marshal.Copy(bytearray, 0, i, len);

                   //obj = Marshal.PtrToStructure(i, obj.GetType());

                   //Marshal.FreeHGlobal(i);

                    */