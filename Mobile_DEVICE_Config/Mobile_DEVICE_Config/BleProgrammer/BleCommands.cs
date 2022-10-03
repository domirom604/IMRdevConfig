using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mobile_DEVICE_Config
{
    public class BleCommands
    {
        private byte[] KEY1 = new byte[] { 0x41, 0x49, 0x55, 0x54, 0x20, 0x57, 0x59, 0x50, 0x52, 0x5A, 0x45, 0x44, 0x5A, 0x41, 0x4D, 0x59 };
        private byte[] KEY2 = new byte[] { 0x20, 0x52, 0x5A, 0x45, 0x43, 0x5A, 0x59, 0x57, 0x49, 0x53, 0x54, 0x4F, 0x53, 0x43, 0x21, 0x21 };
        ConcurrentQueue<(long timeStamp, byte[] packet)> ReceivedBlePackets = new ConcurrentQueue<(long timeStamp, byte[] packet)>();
        public readonly AutoResetEvent StopEvent;
        // private string TXChar = "6c381cc6-cee5-4503-9a7b-98304df7e57b";
        // private string ControlChar = "2db15895-81f5-9eae-e14b-1f16dee0b58c";
        public static IReadOnlyList<ICharacteristic> ConnectionCharachteristics { get; set; }
        IReadOnlyList<ICharacteristic> charactersisticToChangeFirmware = null;
        CancellationToken ct;
        public BleCommands(CancellationToken ct)
        {
            this.ct = ct;
        }
        public void SetCancellationToken(CancellationToken ct)
        {
            this.ct = ct;
        }

        public static void SetConnectionParameters(IReadOnlyList<ICharacteristic> connectionCharachteristics)
        {
            if (connectionCharachteristics != null)
            {
                ConnectionCharachteristics = connectionCharachteristics;
            }
            else
            {
                ConnectionCharachteristics = null;
            }

        }

        public async Task<bool> FVBleCharacteristicConnection()
        {
            IReadOnlyList<ICharacteristic> charactersisticToChangeFirmware = ConnectionCharachteristics;
            if (ConnectionCharachteristics[0] != null && ConnectionCharachteristics[1] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
               
        }

        public async Task<bool> SendKey1Packet(CancellationToken ct)
        {
            bool isOk = true;
            await ConnectionCharachteristics[1].WriteAsync(KEY1); // sending key1  data to device
            //if (ct.IsCancellationRequested)
            //    return false;
            
            return isOk;
        }
        public async Task<bool> SendKey2Packet(CancellationToken ct)
        {
            bool isOk = true;
            await ConnectionCharachteristics[1].WriteAsync(KEY2); // sending key2 to device

            return isOk;
        }
        public async Task<bool> SendFileSettings(UInt32 fileSize, CancellationToken ct)
        {
            bool isOk = true;
            List<byte> ControlCharSend = new List<byte>();
            ControlCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt32(fileSize)));
            ControlCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0x050F)));
            ControlCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(0x00)));
            await ConnectionCharachteristics[1].WriteAsync(ControlCharSend.ToArray());
            return isOk;
        }
        public async Task<byte[]> SendFVUpgradeSettings()
        {
            bool isOk = true;
            byte[] arr = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            isOk = await SendControlBytes(arr);

            byte[] receivedData = { };
            if (ConnectionCharachteristics[1].CanRead)
            {
                receivedData = await ConnectionCharachteristics[1].ReadAsync();
            }
            receivedData[6] = 0;
            var val = receivedData;
            //var val = await Task.Run(() => WaitForResponse(ct));

            return val; // działa
        }
        public async Task<bool> SendFV(byte[] fvByte)
        {
            bool isOk = true;
            byte[] fvByteOne=new byte[100];
            byte[] fvByteTwo=new byte[fvByte.Length- fvByteOne.Length];

            //for (int i = 0; i < 100; i++)
            //{
            //    fvByteOne[i] = fvByte[i];
            //}

            Array.Copy(fvByte, 0, fvByteOne, 0, 100);
            Array.Copy(fvByte, 100, fvByteTwo, 0, 100);
            //for (int i = 100; i < fvByte.Length; i++)
            //{
            //    fvByteTwo[i - 100] = fvByte[i];
            //}

            
           await ConnectionCharachteristics[0].WriteAsync(fvByteOne);
           await ConnectionCharachteristics[0].WriteAsync(fvByteTwo);
            

            

            

            return isOk;
        }
        public async Task<byte[]> SendControlSettings(byte[] controlBytes)
        {
            bool isOk = true;
           
            await ConnectionCharachteristics[1].WriteAsync(controlBytes);
            

            controlBytes[6] = 0; //zakomentowane
          
            
            return controlBytes; 
        }

        private async Task<bool> SendControlBytes(byte[] controlBytes)
        {
            bool isOk = true;
            isOk =  await ConnectionCharachteristics[1].WriteAsync(controlBytes);
            return isOk;
        }


        public byte[] WaitForResponse(CancellationToken ct)
        {
            try
            {
                while (!StopEvent.WaitOne(1, false) && ReceivedBlePackets.Count <= 0)
                {
                    ct.ThrowIfCancellationRequested();
                    if (ReceivedBlePackets.Count <= 0) continue;

                }
                if (ReceivedBlePackets.TryDequeue(out (long timeStamp, byte[] packet) packet))
                {
                    return packet.packet;
                }
                return new byte[0];
            }
            catch (Exception ex)
            {
                return new byte[0];
            }
        }



    }
}
