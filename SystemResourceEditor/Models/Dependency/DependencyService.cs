using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace System.Resource.Models.Dependency
{
    interface DependencyEntity : INotifyPropertyChanged
    {
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e);
    }


    internal class DependencyService
    {
        /// <summary>
        /// 登録
        /// </summary>
        public void Register(ref DependencyEntity entity)
        {
            entity.PropertyChanged += this.OnPropertyChanged;
            this.PropertyChanged += entity.OnPropertyChanged;
        }

        /// <summary>
        /// 登録解除
        /// </summary>
        public void Unregister(ref DependencyEntity entity)
        {
            this.PropertyChanged -= entity.OnPropertyChanged;
            entity.PropertyChanged -= this.OnPropertyChanged;
        }

        /// <summary>
        /// プロパティ変更通知
        /// </summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
