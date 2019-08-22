using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Itach.Succession
{
    /// <summary>
    /// セクションの移動状態
    /// </summary>
    public enum State
    {
        /// <summary>
        /// 動いてない
        /// </summary>
        Idling = 0,

        /// <summary>
        /// 出発処理中
        /// </summary>
        Going = 1,

        /// <summary>
        /// アンロード処理中
        /// </summary>
        Unloading = 2,

        /// <summary>
        /// ロード処理中
        /// </summary>
        Loading = 3,

        /// <summary>
        /// 到着処理中
        /// </summary>
        Initializing = 4,
    }
}
