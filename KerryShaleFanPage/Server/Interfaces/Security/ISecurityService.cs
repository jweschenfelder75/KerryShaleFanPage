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
    }
}
