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
using Windows.UI.Xaml.Documents;
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

    [StructLayout(LayoutKind.Sequential)]
    struct MerchandiseStruct
    {
        public int Amount;

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


            CachedFileManager.DeferUpdates(file);


            switch (typeof(T).Name)
            {
                case "OrderModel":
                    HandleOrderModel(collection as ObservableCollection<OrderModel>);
                    break;
                case "MerchandiseModel":
                    HandleMerchandiseModel(collection as ObservableCollection<MerchandiseModel>);
                    break;
            }

            await GetFileSaveStatus(); 

        }

        private static async Task GetFileSaveStatus()
        {
            Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
            if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
            {
                Debug.WriteLine("File " + file.Name + " was saved.");
            }
            else
            {
                Debug.WriteLine("File " + file.Name + " could not be saved.");
            }
        }

        private static void HandleMerchandiseModel(ObservableCollection<MerchandiseModel> merch)
        {
            MerchandiseStruct[] merchStruct = new MerchandiseStruct[merch.Count];
            for (int i = 0; i < merch.Count; i++)
            {
                merchStruct[i].Amount = merch[i].Amount;
                merchStruct[i].ProductName = merch[i].ProductName;
                merchStruct[i].Supplier = merch[i].Supplier;

            }

            Header h = new Header
            {
                Name = typeof(MerchandiseStruct).Name,
                size = Marshal.SizeOf(typeof(MerchandiseStruct)),
                amount = merch.Count
            };

            ObjectToByteArray(merchStruct, h);
        }


        private static void HandleOrderModel(ObservableCollection<OrderModel> order)
        {
            OrderStruct[] orderStruct = new OrderStruct[order.Count];
            MerchandiseModel tmpMerch;
            for (int i = 0; i < order.Count; i++)
            {
                tmpMerch = order[i].Merch;

                orderStruct[i].CustomerName = order[i].CustomerName;
                orderStruct[i].OrderedAmount = order[i].OrderedAmount;
                orderStruct[i].OrderDate = DateTime.Now.ToString();
                orderStruct[i].ProductName = tmpMerch.ProductName;
                orderStruct[i].Supplier = tmpMerch.Supplier;

            }

            Header h = new Header
            {
                Name = typeof(OrderStruct).Name,
                size = Marshal.SizeOf(typeof(OrderStruct)),
                amount = order.Count
            };

            ObjectToByteArray(orderStruct, h);
        }

        public static void ObjectToByteArray<T>(T[] obj, object h) where T : struct
        {
            using (var fs = new FileStream(file.Path, FileMode.Open, FileAccess.ReadWrite))
            {

                /*
                Assume we dont know the file layout
                Look for a matching header (name)
                Set header as start position
                Look for next header (header + headerSize + header. (size * amount)
                If new header is found, copy existing data to end of file, else, write to file
                Write new header + data
                Write the rest of the already existing data we saved
                 */


                // Find a header


                Header firstHeader = FindNextHeader(fs);
                bool eof = false;
                while (true)
                {

                    if (firstHeader.Name == ((Header)h).Name || eof )
                    {
                        Header nextHeader;
                        long oldPos = fs.Position;
                        // Advance position and look for next header
                        fs.Position += firstHeader.size * firstHeader.amount;
                        nextHeader = FindNextHeader(fs);

                        // New header was found
                        if (nextHeader.size > 0 && !eof)
                        {
                            // save remaining data
                            int remainingSize = (int)(fs.Length - fs.Position + Marshal.SizeOf(h));
                            byte[] remainingData = new byte[remainingSize];

                            fs.Position -= Marshal.SizeOf(h);
                            fs.Read(remainingData, 0, remainingSize);

                            // Revert fs.position to old position with negative offset for the header
                            fs.Position = oldPos - Marshal.SizeOf(h);

                            // Continue with normal write operation
                            WriteHeaderAndStructToFile(fs, h, obj);

                            // Write remaning data
                            fs.Write(remainingData, 0, remainingSize);
                            break;
                        }
                        else // No new header found
                        {
                            if (eof)
                            {
                                fs.Position = fs.Length;
                            }
                            // Continue with normal write operation
                            WriteHeaderAndStructToFile(fs, h, obj);
                            break;
                        }
                    }
                    else
                    {
                        fs.Position = (firstHeader.size * firstHeader.amount) + Marshal.SizeOf(h);
                        firstHeader = FindNextHeader(fs);
                        if (fs.Position == fs.Length)
                        {
                            eof = true;
                        }
                    }
                }

            }
        }

        private static void WriteHeaderAndStructToFile<T>(FileStream fs, object h, T[] obj)
        {
            // Write header to file
            int length = Marshal.SizeOf(h);
            byte[] bytes = new byte[length];

            IntPtr Handle = Marshal.AllocHGlobal(length);

            Marshal.StructureToPtr(h, Handle, true);
            Marshal.Copy(Handle, bytes, 0, length);

            Marshal.FreeHGlobal(Handle);
            fs.Write(bytes, 0, length);

            Header header = ((Header)h);

            for (int i = 0; i < header.amount; i++)
            {
                fs.Flush();
                WriteStructArrayToFile(fs, Handle, obj[i], header.size);
            }
        }

        private static Header FindNextHeader(FileStream fs)
        {
            Header h = new Header();
            BinaryReader br = new BinaryReader(fs, Encoding.UTF8);
            h = GetHeader(br);


            return h;
        }

        private static void WriteStructArrayToFile(FileStream fs, IntPtr handle, object theStruct, int length)
        {
            byte[] bytes = new byte[length];

            length = Marshal.SizeOf(theStruct);

            handle = Marshal.AllocHGlobal(length);

            Marshal.StructureToPtr(theStruct, handle, true);
            Marshal.Copy(handle, bytes, 0, length);

            Marshal.FreeHGlobal(handle);

            fs.Write(bytes, 0, length);
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

                    while (fs.Position < fs.Length - 668)
                    {
                        switch (h.Name)
                        {
                            case "OrderStruct":
                                ReadOrderStruct(br, h, vm);
                                break;
                        }
                    }

                    Debug.WriteLine("Loading Successfull");

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
            h.size = 0;
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
                    Amount = os[i].OrderedAmount,
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