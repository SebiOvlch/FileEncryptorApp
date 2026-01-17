using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


namespace FileEncryptor.Core.Services
{
    public class SteganoService
    {
        private const char END_CHAR = '\0';

        public static Bitmap EmbedText(Bitmap image, string text)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(text + END_CHAR);

            int byteIndex = 0;  
            int bitIndex = 0;   

            if (textBytes.Length * 8 > image.Width * image.Height * 3)
            {
                throw new Exception("The image is too small for the text!");
            }

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (byteIndex >= textBytes.Length)
                        return image;

                    Color pixel = image.GetPixel(x, y);

                    byte r = ProcessComponent(pixel.R, textBytes, ref byteIndex, ref bitIndex);
                    byte g = ProcessComponent(pixel.G, textBytes, ref byteIndex, ref bitIndex);
                    byte b = ProcessComponent(pixel.B, textBytes, ref byteIndex, ref bitIndex);

                    image.SetPixel(x, y, Color.FromArgb(pixel.A, r, g, b));
                }
            }
            return image;
        }

        public static string ExtractText(Bitmap image)
        {
            int currentByte = 0;
            int bitCount = 0;

            System.Collections.Generic.List<byte> messageBytes = new();

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);

                    foreach (byte colorComponent in new byte[] { pixel.R, pixel.G, pixel.B })
                    {
                        int extractedBit = colorComponent & 1;

                        currentByte = (currentByte << 1) | extractedBit;
                        bitCount++;

                        if (bitCount == 8)
                        {
                            if ((char)currentByte == END_CHAR)
                            {
                                return Encoding.UTF8.GetString(messageBytes.ToArray());
                            }

                            messageBytes.Add((byte)currentByte);

                            currentByte = 0;
                            bitCount = 0;
                        }
                    }
                }
            }
            return Encoding.UTF8.GetString(messageBytes.ToArray());
        }

        private static byte ProcessComponent(byte colorComponent, byte[] textBytes, ref int byteIndex, ref int bitIndex)
        {
            if (byteIndex >= textBytes.Length) return colorComponent;

            byte currentByte = textBytes[byteIndex];
            int bitToHide = (currentByte >> (7 - bitIndex)) & 1;

            byte newColor = (byte)((colorComponent & 0xFE) | bitToHide);

            bitIndex++;
            if (bitIndex == 8)
            {
                bitIndex = 0;
                byteIndex++;
            }

            return newColor;
        }
    }
}
