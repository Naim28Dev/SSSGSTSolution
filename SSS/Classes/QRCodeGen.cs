using System.Drawing;
using System.IO;

namespace SSS
{
    class QRCodeGen
    {
        public static byte[] GetQRCode(string strQRCode)
        {
            QRCoder.QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
            QRCoder.QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(strQRCode, QRCoder.QRCodeGenerator.ECCLevel.Q);
            using (Bitmap bitMap = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }
    }
}
