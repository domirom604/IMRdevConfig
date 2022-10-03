using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mobile_DEVICE_Config
{
    public class ProgrammerBle
    {
        bool FVCharacteristicConnected = false;
        bool cancTock = false;
        CancellationToken ct;
        bool program = true;
        BleCharacteristicData settings;
        
        public BleCommands Commander;

        int progressValue;
        

        public ProgrammerBle(CancellationToken ct)
        {
            this.ct = ct;
      
            settings = new BleCharacteristicData();
            Commander = new BleCommands(ct);
        }
        public void SetCancellationToken(bool cancelation)
        {
            cancTock = cancelation;
            //Commander.SetCancellationToken(ct);
        }

        public async Task<bool> UpdateFV(byte[] file)
        {
            try
            {
                bool startProgramming = true;

                startProgramming = await Commander.FVBleCharacteristicConnection();

                if (startProgramming)
                {
                    FVCharacteristicConnected = true;

                    
                    if (!await InitConnection(Convert.ToUInt32(file.Length)))
                        return false;
                   
                    if (!await FVUpgradeSettings())
                        return false;
                   
                    if (!await FVByteSend(file))
                        return false;
                    

                    return true;
                }
                else
                {
               
                    return false;
                }
            }
            finally
            {
               

            }
        }

        public async Task<bool> InitConnection(UInt32 fileSize)
        {

            bool isConnectionOk = true;
            isConnectionOk = await Commander.SendKey1Packet(ct);
            isConnectionOk = await Commander.SendKey2Packet(ct);
            isConnectionOk = await Commander.SendFileSettings(fileSize, ct);
            return isConnectionOk;


        }

        public async Task<bool> FVUpgradeSettings()
        {
            byte[] response = await Commander.SendFVUpgradeSettings();
            settings.SetValues(response);
            
            return true;
        }

        public async Task<bool> FVByteSend(byte[] fileByte)
        {
            
            ushort counter = 0;
            ushort maxByteCount = settings.FirmwarePacketLen;
            ushort byteCountForCRC = 200;  // settings.SinglePacketLen;
            byteCountForCRC = Convert.ToUInt16(maxByteCount);
            List<byte> tmpFileByte = new List<byte>(fileByte);
            double packets_to_send = Math.Ceiling(fileByte.Length / Convert.ToDouble(maxByteCount));
            bool isLastPacket = false;
            int packet_at_moment=0;
            //double valuelOfProg;
           

            if (fileByte != null && fileByte.Length > 0)
            {
                List<byte> contentForCRC = new List<byte>();
                byte[] content;
                while (program)
                {
                    if (cancTock == false)
                    {
                        program = true;
                    }
                    else
                    {
                        program = false;
                    }

                    //valuelOfProg = (packet_at_moment / packets_to_send)*100;
                    

                    if (fileByte.Length <= maxByteCount)
                        isLastPacket = true;

                    if (isLastPacket)
                    {
                        content = fileByte;
                        fileByte = fileByte.Skip(maxByteCount).ToArray();
                    }
                    else
                    {
                        if (contentForCRC.Count + maxByteCount > byteCountForCRC)
                        {
                            content = fileByte.Take(byteCountForCRC - contentForCRC.Count).ToArray();
                            fileByte = fileByte.Skip(byteCountForCRC - contentForCRC.Count).ToArray();
                        }
                        else
                        {
                            content = fileByte.Take(maxByteCount).ToArray();
                            fileByte = fileByte.Skip(maxByteCount).ToArray();
                            
                        }
                    }
                    contentForCRC.AddRange(content);

                    await Commander.SendFV(content);
                    


                    if (contentForCRC.Count == byteCountForCRC || isLastPacket)
                    {
                        uint crc = CRC.ChecksumCRC(contentForCRC.ToArray());
                        
                        
                        settings.CharNbr1 = counter;
                        settings.CharCRC1 = Convert.ToUInt16(crc);
                        settings.CharConfirmation1 = 0xFF;


                        List<byte> ControlCharSend = new List<byte>();
                        ControlCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(settings.SinglePacketLen)));
                        ControlCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(settings.FirmwarePacketLen)));
                        ControlCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(settings.CharNbr1)));
                        ControlCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(settings.CharConfirmation1)));
                        ControlCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(settings.CharCRC1)));
                        ControlCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(settings.CharNbr2)));
                        ControlCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(settings.CharConfirmation2)));
                        ControlCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(settings.CharCRC2)));

                        byte[] val = await Commander.SendControlSettings(ControlCharSend.ToArray());

                        
                        settings.SetValues(val);
                        if (settings.CharConfirmation1 == 0x0000 && settings.CharNbr1 == counter)
                        {
                            
                            counter++;
                            if (fileByte.Length == 0)
                                break;
                            tmpFileByte = tmpFileByte.Skip(contentForCRC.Count).ToList();
                            contentForCRC = new List<byte>();
                            packet_at_moment++;
                            //update.Showing_Progress_Information();

                        }
                        if (settings.CharConfirmation1 == 0x000F && settings.CharNbr1 == counter)
                        {
                            fileByte = new List<byte>(tmpFileByte).ToArray();
                            contentForCRC = new List<byte>();
                        }
                    }




                }
                if (program)
                {
                    settings.CharConfirmation1 = 0xAA;
                    settings.CharConfirmation2 = 0xAA;
                    List<byte> ControlFinishCharSend = new List<byte>();
                    ControlFinishCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(settings.SinglePacketLen)));
                    ControlFinishCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(settings.FirmwarePacketLen)));
                    ControlFinishCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(settings.CharNbr1)));
                    ControlFinishCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(settings.CharConfirmation1)));
                    ControlFinishCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(settings.CharCRC1)));
                    ControlFinishCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(settings.CharNbr2)));
                    ControlFinishCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(settings.CharConfirmation2)));
                    ControlFinishCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(settings.CharCRC2)));

                    await Commander.SendControlSettings(ControlFinishCharSend.ToArray());

                    Firmware.SetProgress = 100;

                    settings.CharConfirmation1 = 0x00;
                    settings.CharConfirmation2 = 0x00;
                    List<byte> Response = GenerateFrame(settings);

                }
                else
                {
                    settings.CharConfirmation1 = 0x55;
                    settings.CharConfirmation2 = 0x55;
                    List<byte> ControlFinishCharSend = GenerateFrame(settings);

                    await Commander.SendControlSettings(ControlFinishCharSend.ToArray());
                    //var valRes = await Task.Run(() => publisher.GetPacket());
                    settings.CharConfirmation1 = 0x00;
                    settings.CharConfirmation2 = 0x00;

                    List<byte> Response = GenerateFrame(settings);

                    cancTock = false;
                    program = true;
                    //tokenSource.Cancel();
                    //sBProgram.Enabled = true;
                }

            }
            return true;
        }

        private List<byte> GenerateFrame(BleCharacteristicData set)
        {
            List<byte> ControlFinishCharSend = new List<byte>();
            ControlFinishCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(set.SinglePacketLen)));
            ControlFinishCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(set.FirmwarePacketLen)));
            ControlFinishCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(set.CharNbr1)));
            ControlFinishCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(set.CharConfirmation1)));
            ControlFinishCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(set.CharCRC1)));
            ControlFinishCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(set.CharNbr2)));
            ControlFinishCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(set.CharConfirmation2)));
            ControlFinishCharSend.AddRange(BitConverter.GetBytes(Convert.ToUInt16(set.CharCRC2)));
            return ControlFinishCharSend;
        }


    }
}
