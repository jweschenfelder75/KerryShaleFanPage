using KerryShaleFanPage.Shared.Security;

namespace KerryShaleFanPage.Server.Interfaces.Security
{
    public interface ISecurityService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public string EncryptData(string plainText);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <returns></returns>
        public string DecryptData(string encryptedText);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="plainText"></param>
        /// <param name="deleteExistingFile"></param>
        /// <returns></returns>
        public bool EncryptFile(string filePath, string plainText, bool deleteExistingFile = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string? DecryptFile(string filePath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="plainText"></param>
        /// <param name="deleteExistingFile"></param>
        /// <returns></returns>
        public bool EncryptDataAndFile(string filePath, string plainText, bool deleteExistingFile = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string DecryptFileAndData(string filePath);
    }
}
