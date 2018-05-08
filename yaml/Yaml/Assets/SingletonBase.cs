/* Created by NavyZhang  
 * mail:710605420@qq.com
 * Welcome to exchange ideas
 */
using UnityEngine;
using System.Collections;

namespace Common
{
    public abstract class SingletonBase<T>
        where T : SingletonBase<T>, new()
    {
        private static T _instance = new T();
        public static T Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
