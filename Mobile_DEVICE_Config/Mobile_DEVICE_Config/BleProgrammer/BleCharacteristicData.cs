using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mobile_DEVICE_Config
{
    public enum Endianness : byte
    {
        LittleEndian,
        BigEndian
    }

    class BleCharacteristicData
    {
        public ushort SinglePacketLen { get; set; }
        public ushort FirmwarePacketLen { get; set; }
        public ushort CharNbr1 { get; set; }
        public ushort CharConfirmation1 { get; set; }
        public ushort CharCRC1 { get; set; }
        public ushort CharNbr2 { get; set; }
        public ushort CharConfirmation2 { get; set; }
        public ushort CharCRC2 { get; set; }
        public bool SetValues(byte[] byteArr)
        {
            bool setOk = true;
            try
            {
                this.SinglePacketLen = BitConverter.ToUInt16(byteArr, 0);
                this.FirmwarePacketLen = BitConverter.ToUInt16(byteArr, 2);
                this.CharNbr1 = BitConverter.ToUInt16(byteArr, 4);
                this.CharConfirmation1 = BitConverter.ToUInt16(byteArr, 6);
                this.CharCRC1 = BitConverter.ToUInt16(byteArr, 8);
                this.CharNbr2 = BitConverter.ToUInt16(byteArr, 10);
                this.CharConfirmation2 = BitConverter.ToUInt16(byteArr, 12);
                this.CharCRC2 = BitConverter.ToUInt16(byteArr, 14);

            }
            catch { setOk = false; }
            return setOk;
        }
    }
    #region CRC-CCITT (0xFFFF)
    public class CRC
    {


        static ushort[] CRC16_CCITT_TAB = new ushort[]
        {
            0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7,
            0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef,
            0x1231, 0x0210, 0x3273, 0x2252, 0x52b5, 0x4294, 0x72f7, 0x62d6,
            0x9339, 0x8318, 0xb37b, 0xa35a, 0xd3bd, 0xc39c, 0xf3ff, 0xe3de,
            0x2462, 0x3443, 0x0420, 0x1401, 0x64e6, 0x74c7, 0x44a4, 0x5485,
            0xa56a, 0xb54b, 0x8528, 0x9509, 0xe5ee, 0xf5cf, 0xc5ac, 0xd58d,
            0x3653, 0x2672, 0x1611, 0x0630, 0x76d7, 0x66f6, 0x5695, 0x46b4,
            0xb75b, 0xa77a, 0x9719, 0x8738, 0xf7df, 0xe7fe, 0xd79d, 0xc7bc,
            0x48c4, 0x58e5, 0x6886, 0x78a7, 0x0840, 0x1861, 0x2802, 0x3823,
            0xc9cc, 0xd9ed, 0xe98e, 0xf9af, 0x8948, 0x9969, 0xa90a, 0xb92b,
            0x5af5, 0x4ad4, 0x7ab7, 0x6a96, 0x1a71, 0x0a50, 0x3a33, 0x2a12,
            0xdbfd, 0xcbdc, 0xfbbf, 0xeb9e, 0x9b79, 0x8b58, 0xbb3b, 0xab1a,
            0x6ca6, 0x7c87, 0x4ce4, 0x5cc5, 0x2c22, 0x3c03, 0x0c60, 0x1c41,
            0xedae, 0xfd8f, 0xcdec, 0xddcd, 0xad2a, 0xbd0b, 0x8d68, 0x9d49,
            0x7e97, 0x6eb6, 0x5ed5, 0x4ef4, 0x3e13, 0x2e32, 0x1e51, 0x0e70,
            0xff9f, 0xefbe, 0xdfdd, 0xcffc, 0xbf1b, 0xaf3a, 0x9f59, 0x8f78,
            0x9188, 0x81a9, 0xb1ca, 0xa1eb, 0xd10c, 0xc12d, 0xf14e, 0xe16f,
            0x1080, 0x00a1, 0x30c2, 0x20e3, 0x5004, 0x4025, 0x7046, 0x6067,
            0x83b9, 0x9398, 0xa3fb, 0xb3da, 0xc33d, 0xd31c, 0xe37f, 0xf35e,
            0x02b1, 0x1290, 0x22f3, 0x32d2, 0x4235, 0x5214, 0x6277, 0x7256,
            0xb5ea, 0xa5cb, 0x95a8, 0x8589, 0xf56e, 0xe54f, 0xd52c, 0xc50d,
            0x34e2, 0x24c3, 0x14a0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405,
            0xa7db, 0xb7fa, 0x8799, 0x97b8, 0xe75f, 0xf77e, 0xc71d, 0xd73c,
            0x26d3, 0x36f2, 0x0691, 0x16b0, 0x6657, 0x7676, 0x4615, 0x5634,
            0xd94c, 0xc96d, 0xf90e, 0xe92f, 0x99c8, 0x89e9, 0xb98a, 0xa9ab,
            0x5844, 0x4865, 0x7806, 0x6827, 0x18c0, 0x08e1, 0x3882, 0x28a3,
            0xcb7d, 0xdb5c, 0xeb3f, 0xfb1e, 0x8bf9, 0x9bd8, 0xabbb, 0xbb9a,
            0x4a75, 0x5a54, 0x6a37, 0x7a16, 0x0af1, 0x1ad0, 0x2ab3, 0x3a92,
            0xfd2e, 0xed0f, 0xdd6c, 0xcd4d, 0xbdaa, 0xad8b, 0x9de8, 0x8dc9,
            0x7c26, 0x6c07, 0x5c64, 0x4c45, 0x3ca2, 0x2c83, 0x1ce0, 0x0cc1,
            0xef1f, 0xff3e, 0xcf5d, 0xdf7c, 0xaf9b, 0xbfba, 0x8fd9, 0x9ff8,
            0x6e17, 0x7e36, 0x4e55, 0x5e74, 0x2e93, 0x3eb2, 0x0ed1, 0x1ef0
        };

        public static uint GenCRC16_CCITT(byte[] aTab)
        {
            uint tbl_idx;
            uint u16crc = 0xFFFF;

            for (ushort a = 0; a < aTab.Length; a++)
            {
                tbl_idx = ((u16crc >> 8) ^ aTab[a]) & 0xFF;
                u16crc = ((u16crc << 8) ^ CRC16_CCITT_TAB[tbl_idx]) & 0xFFFF;
            }

            return u16crc;
        }
        public static UInt16 get_crc_ccit(UInt16 crc, UInt16 data)
        {
            uint i;

            /* move to most significant bit */
            data <<= 8;

            /* for each bit in the character... */
            for (i = 8; i > 0; i--)
            {

                /* calculate */
                if (((data ^ crc) & 0x8000) != 0)
                    crc = (UInt16)((crc << 1) ^ 0x1021);
                else
                    crc <<= 1;

                /* next bit please */
                data <<= 1;
            }
            return crc;
        }

        public static UInt16 ChecksumCRC(byte[] aTab)
        {
            uint a;
            UInt16 iCrc;

            for (a = 0, iCrc = 0; a != aTab.Length; a++)
            {
                iCrc = get_crc_ccit(iCrc, aTab[a]);
            }
            return iCrc;
        }
    }
    #endregion
    public static partial class Extensions
    {

        #region string.ToBytes()
        public static byte[] ToBytes(this string value, Endianness endianness)
        {
            string tmp = value.Replace("0x", "");
            if (tmp.Length % 2 != 0) tmp += "F";
            byte[] bytes = new byte[tmp.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                if (endianness == Endianness.LittleEndian)
                    bytes[i] = Convert.ToByte(tmp.Substring(i * 2, 2), 16);
                if (endianness == Endianness.BigEndian)
                    bytes[bytes.Length - i - 1] = Convert.ToByte(tmp.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
        #endregion

        #region byte[].ToHexString()
        public static string ToHexString(this byte[] bytes, Endianness endianess)
        {
            return bytes.ToHexString(false, endianess);
        }
        public static string ToHexString(this byte[] bytes, bool withPrefix0x, Endianness endianess)
        {
            string result = string.Empty;
            if (withPrefix0x)
                result += "0x";
            foreach (byte b in bytes)
            {
                if (endianess == Endianness.LittleEndian)
                    result += string.Format("{0:X2}", b);
                if (endianess == Endianness.BigEndian)
                    result = result.Insert(0, string.Format("{0:X2}", b));
            }
            return result;
        }
        #endregion
    }

    #region CRC_32
    public class CRC_32
    {

        public static UInt32 CountCrc(ArrayList array)
        {
            byte[] buf = (byte[])array.ToArray(typeof(byte));
            return CountCrc(buf);
        }
        public static UInt32 CountCrc(byte[] bytes)
        {
            uint crc = 0xFFFFFFFF;

            int steps = 0;
            if (bytes.Length % 4 != 0)
            {
                //uzupełnienie 0;
                byte[] tmpTbl = new byte[bytes.Length + 4 - bytes.Length % 4];
                Array.Copy(bytes, tmpTbl, bytes.Length);
                bytes = tmpTbl;
            }
            steps = bytes.Length / 4;
            for (int i = 0; i < steps; i++)
            {
                UInt32 data = BitConverter.ToUInt32(bytes, i * 4);
                crc = crc ^ data;

                for (int j = 0; j < 32; j++)
                {
                    if ((crc & 0x80000000) == 0x80000000)
                        crc = (crc << 1) ^ 0x04C11DB7; // Polynomial used in STM32
                    else
                        crc = (crc << 1);
                }
            }
            return crc;
        }
        public static UInt32 CountCrcZEROinit(byte[] bytes)
        {
            uint crc = 0;

            int steps = 0;
            if (bytes.Length % 4 != 0)
            {
                //uzupełnienie 0;
                byte[] tmpTbl = new byte[bytes.Length + 4 - bytes.Length % 4];
                Array.Copy(bytes, tmpTbl, bytes.Length);
                bytes = tmpTbl;
            }
            steps = bytes.Length / 4;
            for (int i = 0; i < steps; i++)
            {
                UInt32 data = BitConverter.ToUInt32(bytes, i * 4);
                crc = crc ^ data;

                for (int j = 0; j < 32; j++)
                {
                    if ((crc & 0x80000000) == 0x80000000)
                        crc = (crc << 1) ^ 0x04C11DB7; // Polynomial used in STM32
                    else
                        crc = (crc << 1);
                }
            }
            return crc;
        }
    }
    #endregion
}
