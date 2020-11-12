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

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string OrderDate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string CustomerName;

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

    [StructLayout(LayoutKind.Sequential)]
    struct CustomerStruct
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string Address;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Phone;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Email;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct SupplierStruct
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string Address;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Phone;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct AssociateStruct
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string Address;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Phone;
    }

    static class FileManager
    {
        static StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        static StorageFolder storageFolder2 = ApplicationData.Current.LocalFolder;
        static StorageFile file;
        static StorageFile tmpFile;

        const string FileName = "Data.group4";
        const string FileName2 = "Data2.group4";

        public static async Task<Task> SaveToFile<T>(ObservableCollection<T> collection)
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
                tmpFile = await storageFolder2.GetFileAsync(FileName2);
            }
            catch (FileNotFoundException)
            {
                tmpFile = await storageFolder2.CreateFileAsync(FileName2);
            }

            CachedFileManager.DeferUpdates(file);
            CachedFileManager.DeferUpdates(tmpFile);

            switch (typeof(T).Name)
            {
                case "OrderModel":
                    HandleOrderModel(collection as ObservableCollection<OrderModel>);
                    break;
                case "MerchandiseModel":
                    HandleMerchandiseModel(collection as ObservableCollection<MerchandiseModel>);
                    break;
                case "CustomerModel":
                    HandleCustomerModel(collection as ObservableCollection<CustomerModel>);
                    break;
                case "SupplierModel":
                    HandleSupplierModel(collection as ObservableCollection<SupplierModel>);
                    break;
            }

            return GetFileSaveStatus(); 
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
            Windows.Storage.Provider.FileUpdateStatus status2 = await CachedFileManager.CompleteUpdatesAsync(tmpFile);
            if (status2 == Windows.Storage.Provider.FileUpdateStatus.Complete)
            {
                Debug.WriteLine("File " + tmpFile.Name + " was saved.");
            }
            else
            {
                Debug.WriteLine("File " + tmpFile.Name + " could not be saved.");
            }
        }

        //private static void HandleAssociateModel<T>(ObservableCollection<T> associateCollection) where T : AssociateModel
        //{
        //    AssociateStruct[] assStruct = new AssociateStruct[associateCollection.Count];
        //    for (int i = 0; i < associateCollection.Count; i++)
        //    {
        //        assStruct[i].Name = associateCollection[i].Name;
        //        assStruct[i].Address = associateCollection[i].Address;
        //        assStruct[i].Phone = associateCollection[i].Phone;
        //    }

        //    Header h = new Header
        //    {
        //        Name = typeof(T).Name,
        //        size = Marshal.SizeOf(typeof(AssociateStruct)),
        //        amount = associateCollection.Count
        //    };

        //    ObjectToByteArray(assStruct, h);
        //}

        
        private static void HandleSupplierModel(ObservableCollection<SupplierModel> supp)
        {
            SupplierStruct[] suppStruct = new SupplierStruct[supp.Count];
            for (int i = 0; i < supp.Count; i++)
            {
                suppStruct[i].Name = supp[i].Name;
                suppStruct[i].Address = supp[i].Address;
                suppStruct[i].Phone = supp[i].Phone;
            }

            Header h = new Header
            {
                Name = typeof(SupplierStruct).Name,
                size = Marshal.SizeOf(typeof(SupplierStruct)),
                amount = supp.Count
            };

            ObjectToByteArray(suppStruct, h);
        }

        private static void HandleCustomerModel(ObservableCollection<CustomerModel> cust)
        {
            CustomerStruct[] custStruct = new CustomerStruct[cust.Count];
            for (int i = 0; i < cust.Count; i++)
            {
                custStruct[i].Name = cust[i].Name;
                custStruct[i].Address = cust[i].Address;
                custStruct[i].Phone = cust[i].Phone;
                custStruct[i].Email = cust[i].Email;
            }

            Header h = new Header
            {
                Name = typeof(CustomerStruct).Name,
                size = Marshal.SizeOf(typeof(CustomerStruct)),
                amount = cust.Count
            };

            ObjectToByteArray(custStruct, h);
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
            string pending = "";
            for (int i = 0; i < order.Count; i++)
            {
                tmpMerch = order[i].Merch;

                orderStruct[i].CustomerName = order[i].CustomerName;
                orderStruct[i].OrderedAmount = order[i].OrderedAmount;
                orderStruct[i].OrderDate = DateTime.Now.ToString();
                orderStruct[i].ProductName = tmpMerch.ProductName;
                orderStruct[i].Supplier = tmpMerch.Supplier;
                if (order[0].IsPendingOrder)
                {
                    pending = "Pending";
                }
            }
            if (order.Count == 0)
            {
                pending = "Pending";
            }

            Header h = new Header
            {
                Name = typeof(OrderStruct).Name + pending,
                size = Marshal.SizeOf(typeof(OrderStruct)),
                amount = order.Count
            };

            ObjectToByteArray(orderStruct, h);
        }

        public static async void ObjectToByteArray<T>(T[] obj, object h) where T : struct
        {
            bool remove = false;
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
                    if (((Header)h).Name == "OrderStructPending")
                    {
                        if (((Header)h).amount == 0)
                        {
                            await tmpFile.DeleteAsync();
                        }
                        else
                        {
                            using (var fs2 = new FileStream(tmpFile.Path, FileMode.Truncate, FileAccess.ReadWrite))
                            {
                                // Write all data
                                WriteHeaderAndStructToFile(fs2, h, obj);
                            }
                        }
                        break;
                    }
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

                            if (((Header)h).amount != 0)
                            {
                                // Continue with normal write operation
                                WriteHeaderAndStructToFile(fs, h, obj);

                            }

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
                            else
                            {
                                fs.Position = oldPos - Marshal.SizeOf(h);
                            }

                            if (((Header)h).Name != "OrderStructPending")
                            {
                                // Continue with normal write operation
                                WriteHeaderAndStructToFile(fs, h, obj);
                                break;

                            }
                        }
                    }
                    else // Matching header not found, keep looking
                    {
                        fs.Position += (firstHeader.size * firstHeader.amount);
                        firstHeader = FindNextHeader(fs);
                        if (fs.Position >= fs.Length)
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
                tmpFile = await storageFolder.GetFileAsync(FileName2);
            }
            catch (FileNotFoundException)
            {
                tmpFile = await storageFolder.CreateFileAsync(FileName2);

            }

            try
            {
                using (var fs = new FileStream(file.Path, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader br = new BinaryReader(fs, Encoding.UTF8);

                    while (fs.Position < fs.Length)
                    {
                        // Always get header first
                        Header h = GetHeader(br);
                        switch (h.Name)
                        {
                            case "OrderStruct":
                                ReadOrderStruct(br, h, vm, false);
                                break;
                            case "MerchandiseStruct":
                                ReadMerchandiseStruct(br, h, vm);
                                break;
                            case "CustomerStruct":
                                ReadCustomerStruct(br, h, vm);
                                break;
                            case "SupplierStruct":
                                ReadSupplierStruct(br, h, vm);
                                break;
                        }
                    }

                    Debug.WriteLine("Loading Successfull");

                }

                using (var fs2 = new FileStream(tmpFile.Path, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader br = new BinaryReader(fs2, Encoding.UTF8);
                    Header h = GetHeader(br);
                    if ( h.Name == "OrderStructPending")
                    {
                        ReadOrderStruct(br, h, vm, true);
                    }
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

        // Not used anymore
        /*
        private static void ReadAssociateStruct(BinaryReader br, Header h, ViewModel vm) 
        {
            AssociateStruct[] asc = new AssociateStruct[h.amount];
            for (int i = 0; i < h.amount; i++)
            {
                asc[i] = ReadStructData<AssociateStruct>(br, h);
            }

            if (h.Name == "CustomerModel")
                AddAssociateStructToList<CustomerModel>(asc, vm.CustomerList);
            else if (h.Name == "SupplierModel")
                AddAssociateStructToList<SupplierModel>(asc, vm.Supplier);
        }

        private static void AddAssociateStructToList<T>(AssociateStruct[] cs, ObservableCollection<T> vmList) where T : AssociateModel
        {
            for (int i = 0; i < cs.Length; i++)
            {
                T cm = (T)Activator.CreateInstance(typeof(T));

                cm.Name = cs[i].Name;
                cm.Address = cs[i].Address;
                cm.Phone = cs[i].Phone;

                vmList.Add(cm);
            }
        }
         */

        private static void ReadSupplierStruct(BinaryReader br, Header h, ViewModel vm)
        {
            SupplierStruct[] ss = new SupplierStruct[h.amount];
            for (int i = 0; i < h.amount; i++)
            {
                ss[i] = ReadStructData<SupplierStruct>(br, h);
            }
            AddSupplierStructToList(ss, vm);
        }

        private static void AddSupplierStructToList(SupplierStruct[] cs, ViewModel vm)
        {
            for (int i = 0; i < cs.Length; i++)
            {
                SupplierModel sm = new SupplierModel
                {
                    Name = cs[i].Name,
                    Address = cs[i].Address,
                    Phone = cs[i].Phone
                };

                vm.Supplier.Add(sm);
            }
        }

        private static void ReadCustomerStruct(BinaryReader br, Header h, ViewModel vm)
        {
            CustomerStruct[] cs = new CustomerStruct[h.amount];
            for (int i = 0; i < h.amount; i++)
            {
                cs[i] = ReadStructData<CustomerStruct>(br, h);
            }
            AddCustomerStructToList(cs, vm);
        }

        private static void AddCustomerStructToList(CustomerStruct[] cs, ViewModel vm)
        {
            for (int i = 0; i < cs.Length; i++)
            {
                CustomerModel cm = new CustomerModel
                {
                    Name = cs[i].Name,
                    Address = cs[i].Address,
                    Phone = cs[i].Phone,
                    Email = cs[i].Email
                };

                vm.CustomerList.Add(cm);
            }
        }
         

        private static void ReadMerchandiseStruct(BinaryReader br, Header h, ViewModel vm)
        {
            MerchandiseStruct[] ms = new MerchandiseStruct[h.amount];
            for (int i = 0; i < h.amount; i++)
            {
                ms[i] = ReadStructData<MerchandiseStruct>(br, h);
            }
            AddMerchandiseStructToList(ms, vm);
        }

        private static void AddMerchandiseStructToList(MerchandiseStruct[] ms, ViewModel vm)
        {
            for (int i = 0; i < ms.Length; i++)
            {
                MerchandiseModel mm = new MerchandiseModel
                {
                    ProductName = ms[i].ProductName,
                    Amount = ms[i].Amount,
                    Supplier = ms[i].Supplier
                };
               
                vm.ObsMerch.Add(mm);
            }
        }

        private static void ReadOrderStruct(BinaryReader br, Header h, ViewModel vm, bool pending)
        {
            OrderStruct[] os = new OrderStruct[h.amount];
            for (int i = 0; i < h.amount; i++)
            {
                os[i] = ReadStructData<OrderStruct>(br, h);
            }

            AddOrderStructToList(os, vm, pending);
        }

        private static void AddOrderStructToList(OrderStruct[] os, ViewModel vm, bool pending)
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
                if (pending)
                {
                    om.IsPendingOrder = true;
                    vm.PendingOrder.Add(om);
                }
                else
                {
                    vm.Order.Add(om);
                }
            }
        }

        private static T ReadStructData<T>(BinaryReader br, Header h )
        {
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