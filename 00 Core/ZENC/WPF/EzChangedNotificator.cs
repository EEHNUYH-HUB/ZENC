using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows;

namespace ZENC.CORE.WPF
{

    public class EzChangedNotificator : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// UI에 현재 property의 값을 update 합니다.
        /// </summary>
        /// <param name="propertyName"></param>
        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// property가 변경된 경우에만 notification을 수행합니다.
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="target">The target to be swapped out, if different to the value parameter</param>
        /// <param name="value">The new value</param>
        /// <param name="changedProperties">A list of properties whose value may have been impacted by this change and whose PropertyChanged event should be raised</param>
        /// <returns>True if the value is changed, False otherwise</returns>
        protected virtual bool SetProperty<T>(ref T target, T value, params string[] changedProperties)
        {
            if (Equals(target, value))
            {
                return false;
            }

            target = value;

            foreach (string property in changedProperties)
            {
                OnPropertyChanged(property);
            }

            return true;
        }

        //// Minor check, however in cuttting and pasting a new property we can make errors that pass this test
        //// Oops, forget where I found these.. Suppose Sacha Barber introduced this test or at least blogged about it
        ///// <summary>
        ///// 추가 : 김병훈
        ///// debug시 사용, 프로퍼티 이름의 존재여부를 체크하는 메서드
        ///// </summary>
        ///// <param name="propertyName"></param>
        //[Conditional("DEBUG")]
        //private void checkIfPropertyNameExists(String propertyName)
        //{
        //    Type type = this.GetType();
        //    Debug.Assert(
        //      type.GetProperty(propertyName) != null,
        //      propertyName + "property does not exist on object of type : " + type.FullName);
        //}
    }
}
