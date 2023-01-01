using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JworkzNeosMod.Models
{
    internal struct FileId : IEquatable<FileId>, IComparable<FileId>
    {
        public const string UTF8_JSON_FILE_TYPE = "utf8_json";

        public string FileType { get; }

        public int FileLength { get; }

        public FileId(string fileType, int fileLength)
        {
            FileType = fileType;
            FileLength = fileLength;
        }

        public bool Equals(FileId other) => other.FileType == FileType && other.FileLength == other.FileLength;

        public int CompareTo(FileId other)
        {
            var fileTypeRes = FileType.CompareTo(other.FileType);
            var fileLength = FileLength.CompareTo(other.FileLength);

            return fileTypeRes == 0 ? fileLength : fileTypeRes;
        }

        public override bool Equals(object obj) => obj is FileId other && Equals(other);

        public override int GetHashCode()
        {
            var hash = FileLength.GetHashCode();
            return FileType == null ? hash : hash + FileType.GetHashCode();
        }

        public static bool operator ==(FileId a, FileId b) => a.Equals(b);

        public static bool operator !=(FileId a, FileId b) => !a.Equals(b);
    }
}
