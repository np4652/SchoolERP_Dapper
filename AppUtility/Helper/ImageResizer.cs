using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Xml.Linq;

namespace AppUtility.Helper
{
    public class ImageResizer
    {
        private readonly long allowedFileSizeInByte;
        private readonly byte[] fileInBytes;
        private readonly string fileExtention;
        public ImageResizer(long allowedSize, byte[] fileInBytes, string fileExtention)
        {
            this.allowedFileSizeInByte = allowedSize;
            this.fileInBytes = fileInBytes;
            this.fileExtention = fileExtention;
        }

        public ImageResizer()
        {

        }
    }
}
