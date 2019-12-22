using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Corekit.Extensions;

namespace Toolkit.WPF.Sample
{
    internal class FilterTextBoxWindowViewModel : INotifyPropertyChanged
    {
        public IEnumerable<FileInfo> FileEntries { get; set; }

        public string SelectedUnit { 
            get => this._SelectedUnit;
            set
            {
                if (this.SetProperty(ref this._SelectedUnit, value))
                {
                    this.SetProperty(nameof(this.FileEntries), this.GetFileInfos());
                }
            }
        }

        public IEnumerable<string> FileSizeSelection => new List<string>()
        {
            "Byte",
            "KiB",
            "MiB",
            "GiB",
            "TiB"
        };

        public FilterTextBoxWindowViewModel()
        {
            this.SetProperty(nameof(this.FileEntries), this.GetFileInfos());
        }

        public IEnumerable<FileInfo> GetFileInfos()
        {
            var path = System.Reflection.Assembly.GetEntryAssembly().Location;
            var dir = Path.GetDirectoryName(path);
            return Directory.EnumerateFileSystemEntries(dir, "*", SearchOption.TopDirectoryOnly)
                .Select(i => new FileInfo(i))
                .ToList();
        }

        private string _SelectedUnit;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
