using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Mobile_DEVICE_Config
{
    class WAN3
    {
        public static byte package_sent_preamble = 0xA2;
        public static byte  package_sent_preambleValue = 162;
        public static byte package_receive_preamble = 0xA0;
        public static byte package_receive_preambleValue = 160;
        public static byte[] headerAndDataBytes = new byte[1];
        public static byte[] headerAndDataBytesCopy = new byte[1];
        protected static byte[] datagramLayerBytes = null;
        private static byte ReferenceNumber = 0;
        private static  Random random = new Random();

        public static ObservableCollection<OMBname> selectedOMB { get; set; }

        public static void SetSelectedOMBparameter(ObservableCollection<OMBname> SelectedOMB)
        {

            selectedOMB = SelectedOMB;

        }

        #region WP_PDU_ENCRYPTION region
        public static byte[] WP_PDU_ENCRYPTION(byte[] packetBytes, int preambleToEncryptData, int ivValue, int count, bool getOrSet, string serial_number, string cryptography_key, bool AcionListSet )
        {
            byte[] serialNumberbytes = StringToByteTable(serial_number, true);
            byte[] cryptKeybytes = StringToByteTable(cryptography_key, false);
            byte[] MAC = new byte[4];


            int mac_length = 4;
            int serilNumber_length = 4;
            int wpIV_quantity = 1;

            int preamble = preambleToEncryptData + 128;
            byte preambleToEncrypt = Convert.ToByte(preamble);

            string[] ombInTotalSeparatable;
            string[] dataTable = new string[count];
            if (getOrSet == true) // means set
            {
                count = count * 2;
                if (AcionListSet==true)
                {
                    count = 6;
                }
                ombInTotalSeparatable = new string[count];

                //dataTable = valuesSelectedToString();
                int pointer = 0;
                int pointerTwo = 0;
                string[] valueToSetInHex = valuesSelectedToString(AcionListSet);

                
               
                    for (int i = 0; i < count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            ombInTotalSeparatable[i] = ConversionStringHexValueToSetable(selectedOMB[pointerTwo].hexOfOMB);
                            pointerTwo++;
                        }
                        else
                        {

                            ombInTotalSeparatable[i] = valueToSetInHex[pointer];
                            pointer++;
                        }


                    }
                
                if(AcionListSet==true)
                {
                     ombInTotalSeparatable = ombInTotalSeparatable.Where((source, index) => index != 2).ToArray();
                     ombInTotalSeparatable = ombInTotalSeparatable.Where((source, index) => index != 3).ToArray();
                    List<int> list = new List<int>();
                    for (int i =1; i< ombInTotalSeparatable.Length; i++)
                    {
                        if(ombInTotalSeparatable[i]=="")
                        {
                            list.Add(i);
                        }
                    }
                    int[] array = list.ToArray();
                    for (int i = 0; i<array.Length;i++)
                    {
                        ombInTotalSeparatable = ombInTotalSeparatable.Where((source, index) => index != array[i]).ToArray();
                        if(i+1< array.Length)
                        {
                            array[i + 1] = array[i+1] - 1;
                        }
                        else
                        {
                            ombInTotalSeparatable = ombInTotalSeparatable.Where((source, index) => index != 2).ToArray();
                        }
                       
                    }

                }
                
            }

            else
            {
                ombInTotalSeparatable = new string[count];
                for (int i = 0; i < count; i++)
                {
                    ombInTotalSeparatable[i] = selectedOMB[i].hexOfOMB;

                }

            }
            byte[] allOMBinBytes = new byte [1];

            if (AcionListSet==true)
            {
                ombInTotalSeparatable[1] = ombInTotalSeparatable[1].Substring(0, 2);
                string ombInTotal = String.Concat(ombInTotalSeparatable);
               
                int packetBytesNumber= (ombInTotal.Length-4)/2;
                
                string hexPacketBytesNumber = packetBytesNumber.ToString("X");
                string partA = ombInTotal.Substring(0, 4);
                string partB = ombInTotal.Substring(4,( packetBytesNumber*2));

                string ombInTotaltwo =  partA+ "0"+ hexPacketBytesNumber+partB  ;
                allOMBinBytes = StringToByteTable(ombInTotaltwo, false);
                headerAndDataBytes[0] = preambleToEncrypt;
                Array.Resize<byte>(ref headerAndDataBytes, (allOMBinBytes.Length) + 1);// (count*2) +1

                for (int i = 1; i < (allOMBinBytes.Length) + 1; i++)
                {
                    headerAndDataBytes[i] = allOMBinBytes[i - 1];

                }
                
                headerAndDataBytesCopy = allOMBinBytes;
            }
            else
            {
                string ombInTotal = String.Concat(ombInTotalSeparatable); // od tą w dół do poprawy do seta!!!!!

                allOMBinBytes = StringToByteTable(ombInTotal, false);
                headerAndDataBytes[0] = preambleToEncrypt;
                Array.Resize<byte>(ref headerAndDataBytes, (allOMBinBytes.Length) + 1);// (count*2) +1

                for (int i = 1; i < (allOMBinBytes.Length) + 1; i++)
                {
                    headerAndDataBytes[i] = allOMBinBytes[i - 1];

                }
               

                headerAndDataBytesCopy = allOMBinBytes;
            }
            
            int package_length = mac_length + serilNumber_length + wpIV_quantity + allOMBinBytes.Length;
            UInt16 dataLen = Convert.ToUInt16(package_length);
            UInt32 iV = Convert.ToUInt32(ivValue);
            int kk = 1;


            AES.EncryptDecryptByteTable(iV, package_sent_preamble, dataLen, serialNumberbytes, headerAndDataBytes, cryptKeybytes);

            ArrayList pduArrayTmp = new ArrayList();
            ArrayList pduArray = new ArrayList();

            byte[] tmpHdrBytes = new byte[11];
            byte[] hdrBytes = new byte[11];
            byte[] table = intIntoTwoByteHex(dataLen);
            byte[] tableTwo = intIntoForByteHex(ivValue);
            int j = 0;
            int k = 0;
            int l = 0;
            for (int i = 0; i < tmpHdrBytes.Length; i++)
            {

                if (i < 1) { tmpHdrBytes[i] = package_sent_preambleValue; }
                if (i > 0 && i < 3)
                {
                    tmpHdrBytes[i] = table[j];
                    j += 1;
                }

                if (i > 2 && i < 7)
                {
                    tmpHdrBytes[i] = serialNumberbytes[k];
                    k += 1;
                }

                if (i > 6 && i < 10)
                {
                    tmpHdrBytes[i] = tableTwo[l];
                    l += 1;
                }

            }

            hdrBytes = tmpHdrBytes;




            pduArrayTmp.AddRange(tmpHdrBytes);

            pduArrayTmp.AddRange(headerAndDataBytes);
            byte[] dataToMacAnalyze = (byte[])pduArrayTmp.ToArray(typeof(byte));

            AES.CalculateMAC4B(dataToMacAnalyze, cryptKeybytes, MAC);

            hdrBytes[3] = MAC[0];
            hdrBytes[4] = MAC[1];
            hdrBytes[5] = MAC[2];
            hdrBytes[6] = MAC[3];

            pduArray.AddRange(hdrBytes);
            pduArray.AddRange(headerAndDataBytes);


            pduArray.AddRange(BitConverter.GetBytes(CCITT.Crc(pduArray)));

            packetBytes = (byte[])pduArray.ToArray(typeof(byte));
            return packetBytes;
        }
        #endregion

        #region WD_PDU_ENCRYPTION region
        public static byte[] WD_PDU_ENCRYPTION(byte[] packetBytes)
        {
            byte newSmartgas = 254;
            byte constantPreamble = 10;
            ReferenceNumber = (byte)random.Next(127);
            int packetCount = 1;
            //int numberOfIEpacket = 2;
            int dataLengthToSend = packetBytes.Length;
            byte byteDataLengthToSend = Convert.ToByte(dataLengthToSend);
            byte[] hdrBytes = new byte[2];
            hdrBytes[0] = newSmartgas;
            hdrBytes[1] = constantPreamble;
            byte[] IE_nextPacket;
            ArrayList wd_pduArrayTmp = new ArrayList();

            wd_pduArrayTmp.AddRange(hdrBytes);

            for (int i = 0; i <= packetCount; i++)
            {
                // 2 paket count zamienić na wartość chwilową 
                IE_nextPacket = Encode_IE(ReferenceNumber, packetCount, packetCount, i, byteDataLengthToSend, packetBytes);
                wd_pduArrayTmp.AddRange(IE_nextPacket);
            }

            datagramLayerBytes = (byte[])wd_pduArrayTmp.ToArray(typeof(byte));


            return datagramLayerBytes;
        }
        #endregion

        public static byte[] Try_Get_Sn()
        {
            ReferenceNumber = (byte)random.Next(127);

           
            string hexString = "0500030001018203008104000C7F";

            byte[] senddata = new byte[hexString.Length / 2];
            for (int index = 0; index < senddata.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                senddata[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            senddata[3] = ReferenceNumber;

            return senddata;
        }

        #region encoding frame
        public static byte[] Encode_IE(byte ReferenceNumber, int packetCount, int packetCounteachiteration, int numberOfIE, byte byteDataLengthToSend, byte[] packetBytes)
        {
            byte WD_HL = Convert.ToByte(numberOfIE);
            byte constanThree = 3;
            byte packetCountByte = Convert.ToByte(packetCount);
            byte packetCounteachiterationByte = Convert.ToByte(packetCounteachiteration);
            ArrayList encodedArray = new ArrayList();
            int doubledReferenceNumber = 2 * Convert.ToInt32(ReferenceNumber);
            byte ReferenceNumberMultiplyed = Convert.ToByte(doubledReferenceNumber);
            if (numberOfIE == 0)
            {
                encodedArray.Add(WD_HL);
                encodedArray.Add(constanThree);
                encodedArray.Add(ReferenceNumberMultiplyed);
                encodedArray.Add(packetCountByte); // musi tu być stała max wartość n
                encodedArray.Add(packetCountByte); //  musi tu być wartość od 1 do n
            }

            if (numberOfIE == 1)
            {
                // musi tu być poćwiartowany pakiet (byte[] packetBytes) żeby dobrze obliczyć CRC
                ushort DatagramCRC = CCITT.Crc(packetBytes, byteDataLengthToSend);
                byte[] datagramCrc = BitConverter.GetBytes(DatagramCRC);
                encodedArray.Add(WD_HL);
                encodedArray.Add(constanThree);
                encodedArray.Add(byteDataLengthToSend);
                encodedArray.Add(datagramCrc[0]);
                encodedArray.Add(datagramCrc[1]);
            }
            return (byte[])encodedArray.ToArray(typeof(byte));
        }
        #endregion

        #region decoding frame
        public static byte[] Decode_frame(byte[] recivedData , string serial_number, string cryptography_key)
        {
            int hdrBytesNumber = 11;
            byte[] dataToEncrypt = null;
            recivedData = recivedData.Skip(1).ToArray();
            byte[] recivedDataWithoutCRC = new byte[recivedData.Length - 2];
            Buffer.BlockCopy(recivedData, 0, recivedDataWithoutCRC, 0, recivedDataWithoutCRC.Length);

            byte[] MAC = new byte[4];
            byte[] serialNumberbytes = StringToByteTable(serial_number, true);
            byte[] cryptKeybytes = StringToByteTable(cryptography_key, false);

            AES.CalculateMAC4B(recivedDataWithoutCRC, cryptKeybytes, MAC);

            int package_length = recivedDataWithoutCRC[1];

            //ivValue = recivedDataWithoutCRC[7];
            byte[] arrayToConvertIV = { 0, 0, 0, 0, };
            if(recivedDataWithoutCRC.Length<=10)
            {
               
                return dataToEncrypt;
            }
            arrayToConvertIV[0] = recivedDataWithoutCRC[7];
            arrayToConvertIV[1] = recivedDataWithoutCRC[8];
            arrayToConvertIV[2] = recivedDataWithoutCRC[9];
            arrayToConvertIV[3] = recivedDataWithoutCRC[10];

            UInt32 iV = byteTableToInt(arrayToConvertIV);
            dataToEncrypt = recivedDataWithoutCRC.Skip(hdrBytesNumber).ToArray();

            UInt16 dataLen = Convert.ToUInt16(package_length);

            AES.EncryptDecryptByteTable(iV, package_receive_preambleValue, dataLen, serialNumberbytes, dataToEncrypt, cryptKeybytes);

            return dataToEncrypt;
        }
        #endregion

        public static byte[] StringToByteTable(string hexString, bool reverse)
        {

            byte[] senddata = new byte[hexString.Length / 2];
            for (int index = 0; index < senddata.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                senddata[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            if (reverse == true) // only serial number array must be reverse
            {
                int i = 0;
                int j = senddata.Length - 1;
                while (i < j)
                {
                    var temp = senddata[i];
                    senddata[i] = senddata[j];
                    senddata[j] = temp;
                    i++;
                    j--;
                }
            }

            return senddata;
        }

        public static string[] valuesSelectedToString(bool ActionListSet)
        {
            int count = 0;
            count = selectedOMB.Count();
            string[] stringTable = new string[count];

            if (ActionListSet == true)
            {
                for (int i = 0; i < count; i++)
                {
                    stringTable[i] = selectedItemToBigEndianHexValue(selectedOMB[i].omb_values, typeof(int));
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    stringTable[i] = selectedItemToBigEndianHexValue(selectedOMB[i].omb_values, selectedOMB[i].type);
                }
            }

               

            return stringTable;
        }

        public static string ConversionStringHexValueToSetable(string stringToConversion)
        {
            string afterConversion = "";
            char[] array = stringToConversion.ToCharArray();
            array[0] = '1';

            afterConversion = new string(array);
            return afterConversion;
        }

        public static UInt32 byteTableToInt(byte[] table)
        {
            UInt32 WP_IV = 0;
            int[] tablee = new int[table.Length];
            string[] stringTable = new string[table.Length];
            string convert = BitConverter.ToString(table);
            convert = convert.Replace("-", string.Empty);
            char[] array = convert.ToCharArray();
            char[] arrayInBigEndian = new char[array.Length];

            int pointerVariable = array.Length;
            int value = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (value % 2 == 0)
                {
                    arrayInBigEndian[pointerVariable - 2] = array[i];
                    value++;
                }
                else
                {
                    arrayInBigEndian[pointerVariable - 1] = array[i];
                    value++;
                    pointerVariable -= 2;
                }
                if (pointerVariable < 0)
                {
                    break;
                }



            }
            string prefixedHex = new string(arrayInBigEndian);
            prefixedHex = String.Format("0x{0:X}", prefixedHex);
            int wp_iv = Convert.ToInt32(prefixedHex, 16);
            WP_IV = Convert.ToUInt32(wp_iv);
            return WP_IV;
        }

        public static byte[] intIntoTwoByteHex(int value)
        {
            var bytes = new byte[2];
            bytes[0] = (byte)value;
            bytes[1] = (byte)(value >> 8);

            return bytes;
        }
        public static byte[] intIntoForByteHex(int value)
        {
            var bytes = new byte[4];
            bytes[0] = (byte)value;
            bytes[1] = (byte)(value >> 8);
            bytes[2] = (byte)(value >> 8);
            bytes[3] = (byte)(value >> 8);
            return bytes;
        }

        public static  string selectedItemToBigEndianHexValue(object SelectedValue, Type typeOfSelection)
        {
            string text = "";
            string valueToString = "";
            if (typeOfSelection == typeof(bool))
            {
                // must be added type od data and mac!!!!
                if (SelectedValue == "True")
                {
                    text = "01";
                }
                else
                {
                    text = "00";
                }

            }
            if (typeOfSelection == typeof(byte))
            {
                // conversion byte to hex in big endian
                //valueToString = SelectedValue.ToString();
                // one means 00
                text = stringToBigEndianString(SelectedValue, 1);

            }
            if (typeOfSelection == typeof(int))
            {
                // conversion int to hex in big endian
                //valueToString = SelectedValue.ToString();
                // 4 means 00 00 00 00
                text = stringToBigEndianString(SelectedValue, 4);

            }

            return text;
        }

        public static string stringToBigEndianString(object SelectedValue, int bytesNumber)
        {
            string hexValue = "";
            if (SelectedValue=="")
            {
                return hexValue;
            }
            int valueToConvert = Convert.ToInt32(SelectedValue);
            hexValue = valueToConvert.ToString("X");
            char[] array = hexValue.ToCharArray();
            if (array.Length % 2 != 0)
            {
                Array.Resize<char>(ref array, array.Length + 1);

                char last = array[array.Length - 1];
                for (int i = array.Length - 1; i > 0; i--)
                {
                    array[i] = array[i - 1];
                }
                array[0] = '0';


            }
            string afterConversion = "";
            char[] arrayInBigEndian = new char[array.Length];

            int pointerVariable = array.Length;
            int value = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (value % 2 == 0)
                {
                    arrayInBigEndian[pointerVariable - 2] = array[i];
                    value++;
                }
                else
                {
                    arrayInBigEndian[pointerVariable - 1] = array[i];
                    value++;
                    pointerVariable -= 2;
                }
                if (pointerVariable < 0)
                {
                    break;
                }



            }
            int siezeOfarRAY = arrayInBigEndian.Length;
            if (siezeOfarRAY / 2 != bytesNumber)
            {
                int bytesToAdd = bytesNumber - (arrayInBigEndian.Length / 2);
                Array.Resize<char>(ref arrayInBigEndian, siezeOfarRAY + (bytesToAdd * 2));
                for (int i = siezeOfarRAY; i < arrayInBigEndian.Length; i++)
                {
                    arrayInBigEndian[i] = '0';
                }
            }
            afterConversion = new string(arrayInBigEndian);
            //string convert = BitConverter.ToString(table);
            //convert = convert.Replace("-", string.Empty);

            return afterConversion;
        }
    }

}

   

