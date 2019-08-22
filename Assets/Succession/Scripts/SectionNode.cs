using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Itach.Succession
{
    public class SectionNode
    {
        public string id;
        public SectionBase section;
        private List<SectionNode> children = new List<SectionNode>();

        public SectionNode(string id, SectionBase section)
        {
            this.id = id;
            this.section = section;
        }

        /// <summary>
        /// ノードを追加
        /// </summary>
        /// <param name="node"></param>
        public SectionNode Add(SectionNode node)
        {
            children.Add(node);
            return node;
        }

        /// <summary>
        /// パスのリストを作成
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathList"></param>
        public void AddPathList(string path, List<string> pathList)
        {
            pathList.Add(path + id);

            if (children.Count > 0)
            {
                foreach (var child in children)
                {
                    child.AddPathList(path + id + "/", pathList);
                }
            }
        }

        /// <summary>
        /// ノードを取得
        /// </summary>
        /// <param name="secId"></param>
        /// <param name="targetDepth"></param>
        /// <param name="nowDepth"></param>
        /// <returns></returns>
        public SectionNode GeNode(SectionId secId, int targetDepth, int nowDepth)
        {
            if (nowDepth >= secId.hierarchies.Count) return null;

            if (secId.hierarchies[nowDepth] == id)
            {
                if (nowDepth == targetDepth)
                {
                    return this;
                }
                else
                {
                    nowDepth++;
                    SectionNode anySec = null;
                    foreach (var child in children)
                    {
                        anySec = child.GeNode(secId, targetDepth, nowDepth);
                        if (anySec != null) break;
                    }
                    return anySec;
                }
            }
            else
            {
                return null;
            }
        }
    }
}



