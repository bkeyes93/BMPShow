//  
// Copyright (c) Brandon P. Keyes. All rights reserved.  
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3.
// See LICENSE file in the project root for full license information.  
//  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Drawing;

namespace BMPShow
{
    class Program
    {
        static void Main(string[] args)
        {
            // Ensure correct number of arguments were used.
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: bmpshow.exe <input.bmp> <output.bin>");
                System.Environment.Exit(1);
            }

            string input_file = Path.GetFullPath(args[0]);
            string output_file = args[1];

            Bitmap bitmap = new Bitmap(input_file);

            // Create byte array to hold our data.
            byte[] data = new byte[bitmap.Width * bitmap.Height];
            int index = 0;
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    // Extract data from each pixel. 
                    Color pixel = bitmap.GetPixel(x, y);
                    data[index] = (byte)((pixel.R & 7) | ((pixel.G & 7) << 3) | ((pixel.B & 3) << 6));
                    index++;
                }
            }

            // Decrypt the data bytes.
            byte[] decrypted_data = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                byte current_byte = data[i];
                current_byte = rol(current_byte, 3);
                current_byte ^= (byte)((((i * 2) + 2) * 197) ^ (((i * 2) + 3) * 125));
                current_byte = ror(current_byte, 7);
                current_byte ^= (byte)((((i * 2) + 1) * 197) ^ (((i * 2) + 2) * 125));
                decrypted_data[i] = current_byte;
            }

            // Save decrypted data to file.
            File.WriteAllBytes(output_file, decrypted_data);
        }

        /// <summary>Bitwise circular rotation right.</summary>
        /// <param name="b"> The byte to be rotated.</param>
        /// <param name="r"> The number of times the bits will be rotated.</param> 
        /// <returns>The rotated byte.</returns>
        public static byte ror(byte b, int r)
        {
            for (int i = 0; i < r; i++)
            {
                byte b2 = (byte)((b & 1) * 128);
                b = (byte)((((int)b / 2) & 0xFF) + b2);
            }
            return b;
        }

        /// <summary> Bitwise circular rotation left.</summary>
        /// <param name="b"> The byte to be rotated.</param>
        /// <param name="r"> The number of times the bits will be rotated.</param> 
        /// <returns>The rotated byte.</returns>
        public static byte rol(byte b, int r)
        {
            for (int i = 0; i < r; i++)
            {
                byte b2 = (byte)((b & 0x80) / 128);
                b = (byte)(((b * 2) & 0xFF) + b2);
            }
            return b;
        }
    }
}
