using System;
using System.Security.Cryptography;
using System.IO;


namespace Mobile_DEVICE_Config
{
    [Serializable]
    public class AES
    {
        #region ExecuteEncryption
        /// <summary>
        /// Runs AES encryption process for the given data with the given parameters
        /// </summary>
        /// <param name="cipherMode">Cipher mode</param>
        /// <param name="cryptKey">Key used for encryption</param>
        /// <param name="IV">Initialization vector</param>
        /// <param name="Data">Data to be encrypted; cipher data after encryption process</param>
        static void ExecuteEncryption(CipherMode cipherMode, byte[] cryptKey, byte[] IV, ref byte[] Data)
        {
            Rijndael RijndaelAlg = Rijndael.Create();
            RijndaelAlg.Mode = cipherMode;
            RijndaelAlg.Padding = PaddingMode.Zeros;

            MemoryStream fStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(fStream,
                RijndaelAlg.CreateEncryptor(cryptKey, IV),
                CryptoStreamMode.Write);

            cStream.Write(Data, 0, Data.Length);

            cStream.FlushFinalBlock();
            Data = fStream.ToArray();
        }
        #endregion

        #region EncryptDecryptByteTable
        /// <summary>
        /// Encrypt/Decrypt data in CTR mode with the specified parameters
        /// </summary>
        /// <param name="IV">Initialization vector</param>
        /// <param name="ctrlByte">WP_PDU_CTRL</param>
        /// <param name="dataLength">WP_LEN</param>
        /// <param name="cryptKey">Key</param>
        /// <param name="dataToEncrypt">Data</param>        
        /// <param name="serialNumberBytes">Serial Number bytes</param>
        public static void EncryptDecryptByteTable(UInt32 IV, byte ctrlByte, UInt16 dataLen, byte[] serialNumberBytes, byte[] dataToEncrypt, byte[] cryptKey)
        {
            int dataToEncryptLen = dataToEncrypt.Length;
            byte[] AES_state = new byte[16];

            //block encryption
            for (int i = 0; i < dataToEncryptLen;)
            {
                // preparation of the counter block
                AES_state[0] = ctrlByte;//WP_CTRL
                Buffer.BlockCopy(BitConverter.GetBytes(dataLen), 0, AES_state, 1, 2);//WP_LEN
                Buffer.BlockCopy(serialNumberBytes, 0, AES_state, 3, 4);//WP_SN
                Buffer.BlockCopy(BitConverter.GetBytes(IV), 0, AES_state, 7, 4);//WP_IV
                Buffer.BlockCopy(BitConverter.GetBytes(0), 0, AES_state, 11, 3);//WP_UNUSED
                Buffer.BlockCopy(BitConverter.GetBytes((ushort)(i / 16)), 0, AES_state, 14, 2);//BLOCK_NR

                //counter block encryption
                ExecuteEncryption(CipherMode.ECB, cryptKey, new byte[16], ref AES_state);

                // xor the message with the encrypted counter block
                for (int j = 0; j < 16 && i < dataToEncryptLen;)
                    dataToEncrypt[i++] ^= AES_state[j++];
            }
        }
        #endregion
        #region EncryptDecryptByteTable
        /// <summary>
        /// Encrypt/Decrypt data in CTR mode with the specified parameters
        /// </summary>
        /// <param name="IV">Initialization vector</param>
        /// <param name="ctrlByte">WP_PDU_CTRL</param>
        /// <param name="dataLength">WP_LEN</param>
        /// <param name="cryptKey">Key</param>
        /// <param name="dataToEncrypt">Data</param>        
        /// <param name="serialNumberBytes">Serial Number bytes</param>
        public static void EncryptDecryptByteTable(byte[] IV, byte ctrlByte, UInt16 dataLen, byte[] serialNumberBytes, byte[] dataToEncrypt, byte[] cryptKey)
               
        {
            int dataToEncryptLen = dataToEncrypt.Length;
            byte[] AES_state = new byte[16];

            //szyfrowanie bloków
            for (int i = 0; i < dataToEncryptLen;)
            {
                //przygotowanie bloku licznika
                AES_state[0] = ctrlByte;//WP_CTRL
                Buffer.BlockCopy(BitConverter.GetBytes(dataLen), 0, AES_state, 1, 2);//WP_LEN
                Buffer.BlockCopy(serialNumberBytes, 0, AES_state, 3, 4);//WP_SN
                Buffer.BlockCopy(IV, 0, AES_state, 7, 4);//WP_IV
                Buffer.BlockCopy(BitConverter.GetBytes(0), 0, AES_state, 11, 3);//WP_UNUSED
                Buffer.BlockCopy(BitConverter.GetBytes((ushort)(i / 16)), 0, AES_state, 14, 2);//BLOCK_NR

                //szyfrowanie bloku licznika
                ExecuteEncryption(CipherMode.ECB, cryptKey, new byte[16], ref AES_state);

                //xorowanie wiadomosci z zaszyfrowanym blokiem licznika
                for (int j = 0; j < 16 && i < dataToEncryptLen;)
                    dataToEncrypt[i++] ^= AES_state[j++];
            }
        }
        #endregion
        #region CalculateMAC4B
        /// <summary>
        /// Calculates 4 bytes long Message Authentication Code for the given data
        /// </summary>
        /// <param name="dataToAuthenticate">Data to authenticate</param>
        /// <param name="cryptKey">Cryptographic key</param>
        /// <param name="MAC">Calculated MAC</param>
        public static void CalculateMAC4B(byte[] dataToAuthenticate, byte[] cryptKey, byte[] MAC)
        {
            //przygotowanie klucza MAC_KEY
            byte[] MAC_Key = new byte[16];
            ExecuteEncryption(CipherMode.ECB, cryptKey, new byte[16], ref MAC_Key);

            //zaszyfrowanie blokow w trybie CBC
            ExecuteEncryption(CipherMode.CBC, MAC_Key, new byte[16], ref dataToAuthenticate);

            //MAC stanowia 4 pierwsze bajty ostatniego zaszyfrowanego bloku
            Buffer.BlockCopy(dataToAuthenticate, dataToAuthenticate.Length - 16, MAC, 0, 4);

        }
        #endregion
    }
}
