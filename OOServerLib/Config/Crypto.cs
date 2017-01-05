using System;
using System.Xml;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace OOServerLib.Config
{
    public class Crypto
    {
        private RSACryptoServiceProvider rsa;

        /// Loads the new object using an xml string that contains the key info
        public Crypto(string keyXml)
        {
            rsa = new RSACryptoServiceProvider();
            if (keyXml != null) rsa.FromXmlString(keyXml);
        }

        /// Encrypts the data using DES with A new IV and Key, then encrypts
        /// the IV and Key with RSA encryption.
        public EncryptedDataInfo Encrypt(string data)
        {
            string encIV;
            string encKey;
            string encData;

            ICryptoTransform encryptor;
            CryptoStream cStream;
            MemoryStream mStream = new MemoryStream();

            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.GenerateIV();
                des.GenerateKey();

                encIV = Convert.ToBase64String(rsa.Encrypt(des.IV, false));
                encKey = Convert.ToBase64String(rsa.Encrypt(des.Key, false));

                encryptor = des.CreateEncryptor(des.Key, des.IV);
                cStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write);

                byte[] bytesIn = Encoding.ASCII.GetBytes(data);

                cStream.Write(bytesIn, 0, bytesIn.Length);
                cStream.FlushFinalBlock();
                cStream.Close();

                byte[] bytesOut = mStream.ToArray();
                mStream.Close();

                encData = Convert.ToBase64String(bytesOut);

                return new EncryptedDataInfo(encData, encIV, encKey);
            }
            catch (Exception ex)
            {
                mStream.Close();
                throw ex;
            }
        }

        /// Decrypts data from a xml representation of an EncryptedDataInfo object
        public string Decrypt(string encryptedDataInfoXml)
        {
            return Decrypt(new EncryptedDataInfo(encryptedDataInfoXml));
        }

        /// Decrypts data from an EncryptedDataInfo object
        public string Decrypt(EncryptedDataInfo encryptedDataInfo)
        {
            return Decrypt(encryptedDataInfo.EncryptedData, encryptedDataInfo.EncryptedKey, encryptedDataInfo.EncryptedIV);
        }

        /// Decrypts the Key and IV using RSA and then decrypts the data using DES
        public string Decrypt(string encryptedData, string encryptedKey, string encryptedIV)
        {
            string returnValue;

            byte[] iv = rsa.Decrypt(Convert.FromBase64String(encryptedIV), false);
            byte[] key = rsa.Decrypt(Convert.FromBase64String(encryptedKey), false);
            byte[] data = Convert.FromBase64String(encryptedData);

            MemoryStream mStream = new MemoryStream(data);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            ICryptoTransform decryptor = des.CreateDecryptor(key, iv);

            CryptoStream cStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cStream);

            returnValue = reader.ReadToEnd();

            reader.Close();
            mStream.Close();
            cStream.Close();

            return returnValue;
        } // Crypto


        /// Container for encrypted data
        public class EncryptedDataInfo
        {
            private string encData;
            private string encIV;
            private string encKey;

            public EncryptedDataInfo(string encryptedData, string encryptedIV, string encryptedKey)
            {
                encData = encryptedData;
                encIV = encryptedIV;
                encKey = encryptedKey;
            }

            public EncryptedDataInfo(string encryptedDataInfoXml)
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(encryptedDataInfoXml);

                encData = xml.SelectSingleNode(@"/EncryptedDataInfo/EncryptedData").InnerText;
                encIV = xml.SelectSingleNode(@"/EncryptedDataInfo/EncryptedIV").InnerText;
                encKey = xml.SelectSingleNode(@"/EncryptedDataInfo/EncryptedKey").InnerText;
            }

            public string ToXml()
            {
                XmlDocument xml = new XmlDocument();
                XmlElement root = xml.CreateElement("EncryptedDataInfo");
                XmlElement data = xml.CreateElement("EncryptedData");
                XmlElement iv = xml.CreateElement("EncryptedIV");
                XmlElement key = xml.CreateElement("EncryptedKey");

                data.InnerText = encData;
                iv.InnerText = encIV;
                key.InnerText = encKey;

                root.AppendChild((XmlNode)data);
                root.AppendChild((XmlNode)iv);
                root.AppendChild((XmlNode)key);
                xml.AppendChild((XmlNode)root);

                return xml.OuterXml;
            }

            public override string ToString()
            {
                return ToXml();
            }

            public string EncryptedData { get { return encData; } }
            public string EncryptedIV { get { return encIV; } }
            public string EncryptedKey { get { return encKey; } }
        } //EncryptedDataInfo


        /// Used to create and save new keys
        public class RSAKeyCreator
        {
            private string publicKeyXml;
            private string privateKeyXml;

            public RSAKeyCreator()
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
                publicKeyXml = rsa.ToXmlString(false);
                privateKeyXml = rsa.ToXmlString(true);
            }

            public RSAKeyCreator(int keySize)
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
                publicKeyXml = rsa.ToXmlString(false);
                privateKeyXml = rsa.ToXmlString(true);
            }

            public void SavePublicKeyToFile(string filePath)
            {
                TextWriter writer = File.CreateText(filePath);
                writer.Write(publicKeyXml);
                writer.Flush();
                writer.Close();
            }

            public void SavePrivateKeyToFile(string filePath)
            {
                TextWriter writer = File.CreateText(filePath);
                writer.Write(privateKeyXml);
                writer.Flush();
                writer.Close();
            }

            public string PublicKeyXml { get { return publicKeyXml; } }
            public string PrivateKeyXml { get { return privateKeyXml; } }
        } // RSAKeyCreator
    }
}
