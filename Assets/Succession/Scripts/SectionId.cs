using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Itach.Succession
{
    public class SectionId
    {
        public string path { get; private set; }
        public List<string> hierarchies { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="path">セクションのパス<br/>
        /// [good] /xx
        /// [good] /xx/xx
        /// [good] ./xx
        /// [good] ../xx
        /// [bad] xx
        /// [bad] /xx/
        /// </param>
        /// <param name="baseSectionId">相対パスを使う場合のベースとなるセクションID</param>
        public SectionId(string path = "/", SectionId baseSectionId = null)
        {
            // 相対パスを絶対パスに変換
            if (baseSectionId != null)
            {
                var _tempHierarchies = path.Split('/');
                var _baseDepth = Mathf.Max(baseSectionId.hierarchies.Count - 1, 0);
                var _additionPath = "";
                for (int i = 0; i < _tempHierarchies.Length; i++)
                {
                    switch (_tempHierarchies[i])
                    {
                        case "":
                            if (i == 0) _baseDepth = 0;
                            break;

                        case ".":
                            break;

                        case "..":
                            _baseDepth = Mathf.Max(_baseDepth - 1, 0);
                            break;

                        default:
                            _additionPath += "/" + _tempHierarchies[i];
                            break;
                    }
                }
                for (int i = 0; i < _baseDepth; i++)
                {
                    this.path += "/" + baseSectionId.hierarchies[i];
                }
                this.path += _additionPath;
            }
            else
            {
                this.path = path;
            }

            // 階層セット
            hierarchies = new List<string>(this.path.Split('/'));
            if (hierarchies.Count > 1 && hierarchies[0] == "")
            {
                hierarchies.RemoveAt(0);
            }
            if (this.path == "/"|| this.path == "")
            {
                hierarchies = new List<string>();
            }
        }

        /// <summary>
        /// 階層の深さを指定してパスを取得
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public string GetPathFromDepth(int depth)
        {
            string path = "";
            for (int i = 0; i < depth; i++)
            {
                path += "/" + hierarchies[i];
            }
            return path;
        }

        /// <summary>
        /// 何番目の階層までの親セクションが共通しているか<br/>
        /// /a/b<br/>
        /// /a/c<br/>
        /// の場合1
        /// </summary>
        /// <returns></returns>
        /// <param name="comparisonSectionId">比較対象となるセクションID</param>
        /// <returns></returns>
        public int GetSameParentDepth(SectionId comparisonSectionId)
        {
            int count = 0;
            for (int i = 0; i < hierarchies.Count; i++)
            {
                if (i >= comparisonSectionId.hierarchies.Count) break;
                if (hierarchies[i] != comparisonSectionId.hierarchies[i]) break;
                count++;
            }
            return count;
        }
    }
}