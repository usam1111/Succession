using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Itach.Succession
{
    public abstract class SectionBase : MonoBehaviour
    {
        /// <summary>
        /// セクションID
        /// </summary>
        [HideInInspector] public string id;

        /// <summary>
        /// 初期化
        /// </summary>
        public abstract SectionNode Initialize(string id);

        /// <summary>
        /// ノードを新規取得
        /// </summary>
        /// <param name="id"></param>
        /// <param name="section"></param>
        protected SectionNode CreateSectionNode(string id, SectionBase section)
        {
            this.id = id;
            return new SectionNode(id, section);
        }

        /// <summary>
        /// ロード処理
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator AtSectionLoad();

        /// <summary>
        /// 到着処理
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator AtSectionInit();

        /// <summary>
        /// 出発処理
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator AtSectionGoto();

        /// <summary>
        /// アンロード処理
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator AtSectionUnload();
    }
}
