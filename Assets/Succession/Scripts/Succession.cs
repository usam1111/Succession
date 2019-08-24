using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Itach.Succession
{
    public class Succession
    {
        /// <summary>
        /// 出発地（移動完了時に更新される）
        /// </summary>
        public SectionId departedSectionId { get; private set; } = new SectionId();

        /// <summary>
        /// 現在地（移動中の場合は移動処理が行われているセクション）
        /// </summary>
        public SectionId currentSectionId { get; private set; } = new SectionId();

        /// <summary>
        /// 目的地（移動開始時に更新される）
        /// </summary>
        public SectionId destinedSectionId { get; private set; } = new SectionId();

        /// <summary>
        /// 予約目的地（移動中に使用）
        /// </summary>
        private SectionId reservedSectionId = new SectionId();

        /// <summary>
        /// セクションの移動状態
        /// </summary>
        public State state { get; private set; } = State.Idling;

        /// <summary>
        /// セクション間を移動している状態か
        /// </summary>
        public bool isMoving => (state != State.Idling);

        /// <summary>
        /// セクション移動開始イベント
        /// </summary>
        public event EventHandler processStart;

        /// <summary>
        /// セクション移動終了イベント
        /// </summary>
        public event EventHandler processComplete;

        private List<SectionNode> nodes = new List<SectionNode>();
        private Coroutine gotoManageCoroutine;
        private bool isCoroutineBreak;

        /// <summary>
        /// コンストラクタの呼び出し元
        /// </summary>
        private MonoBehaviour rootObject;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="gameObject"></param>
        public Succession(MonoBehaviour rootObject)
        {
            this.rootObject = rootObject;
        }


        /// <summary>
        /// ノードを追加
        /// </summary>
        /// <param name="node"></param>
        public SectionNode Add(SectionNode node)
        {
            nodes.Add(node);
            return node;
        }

        /// <summary>
        /// セクションを移動する
        /// </summary>
        /// <param name="path">セクションのパス<br/>
        /// [good] /xx
        /// [good] /xx/xx
        /// [good] ./xx
        /// [good] ../xx
        /// [bad] xx
        /// [bad] /xx/
        /// </param>
        public void Goto(string path)
        {
            var tempSectionId = new SectionId(path, departedSectionId);

            // パスが存在しない
            if (!IsExistPath(tempSectionId.path)) return;

            // セクションを移動中
            if (isMoving)
            {
                // 目的地が違う
                if (tempSectionId.path != destinedSectionId.path)
                {
                    reservedSectionId = tempSectionId;
                }
                return;
            }

            // 現在のパスと同じ
            if (tempSectionId.path == currentSectionId.path) return;

            
            destinedSectionId = tempSectionId;
            reservedSectionId = null;

            gotoManageCoroutine = rootObject.StartCoroutine(GotoManage(departedSectionId));
        }


        /// <summary>
        /// セクション移動のコルーチン
        /// </summary>
        /// <param name="lastDepartedSectionId">移動中に使用する出発地</param>
        /// <param name="prevState">目的地変更による再実行前のセクション移動状態</param>
        /// <returns></returns>
        private IEnumerator GotoManage(SectionId lastDepartedSectionId, State prevState = State.Idling)
        {
            state = State.Going;

            // 現在の階層
            int nowDepth = lastDepartedSectionId.hierarchies.Count - 1;
            // 共通している親セクションの階層
            int sameParentDepth = lastDepartedSectionId.GetSameParentDepth(destinedSectionId);
            // 目的地の階層
            int destinedDepth = Mathf.Max(destinedSectionId.hierarchies.Count - 1, 0);
            bool isDestination = false;
            SectionNode node;

            // 移動開始イベント発信
            processStart?.Invoke(this, EventArgs.Empty);

            // Goto
            if (prevState == State.Idling || prevState == State.Initializing)
            {
                node = GetNode(lastDepartedSectionId, nowDepth);
                if (node != null)
                {
                    currentSectionId = new SectionId(lastDepartedSectionId.GetPathFromDepth(nowDepth + 1));
                    node.section.id = node.id;
                    var e = node.section.AtSectionGoto();
                    while (e.MoveNext()) yield return e.Current;
                    // 目的地変更予約がある
                    if (reservedSectionId != null)
                    {
                        CheckChangeDestination(lastDepartedSectionId, nowDepth + 1);
                        yield return null;
                    }
                }
            }
            else
            {
                yield return null;
            }

            // Unload
            state = State.Unloading;
            while (nowDepth >= sameParentDepth)
            {
                isDestination = (sameParentDepth > destinedDepth) && (nowDepth == destinedDepth);
                if (isDestination) break;
                node = GetNode(lastDepartedSectionId, nowDepth--);
                if (node != null)
                {
                    currentSectionId = new SectionId(lastDepartedSectionId.GetPathFromDepth(nowDepth + 2));
                    node.section.id = node.id;
                    var e = node.section.AtSectionUnload();
                    while (e.MoveNext()) yield return e.Current;
                    // 目的地変更予約がある
                    if (reservedSectionId != null)
                    {
                        CheckChangeDestination(lastDepartedSectionId, nowDepth + 1);
                        yield return null;
                    }
                }
            }

            // Load
            state = State.Loading;
            while (nowDepth <= destinedDepth)
            {
                node = GetNode(destinedSectionId, ++nowDepth);
                if (node != null)
                {
                    currentSectionId = new SectionId(destinedSectionId.GetPathFromDepth(nowDepth + 1));
                    node.section.id = node.id;
                    var e = node.section.AtSectionLoad();
                    while (e.MoveNext()) yield return e.Current;
                    // 目的地変更予約がある
                    if (reservedSectionId != null)
                    {
                        CheckChangeDestination(destinedSectionId, nowDepth + 1);
                        yield return null;
                    }
                }
            }

            // Init
            state = State.Initializing;
            node = GetNode(destinedSectionId, destinedDepth);
            if (node != null)
            {
                currentSectionId = destinedSectionId;
                node.section.id = node.id;
                var e = node.section.AtSectionInit();
                while (e.MoveNext()) yield return e.Current;
                // 目的地変更予約がある
                if (reservedSectionId != null)
                {
                    CheckChangeDestination(destinedSectionId, destinedDepth + 1);
                    yield return null;
                }
            }

            state = State.Idling;

            // 移動終了イベント発信
            processComplete?.Invoke(this, EventArgs.Empty);

            departedSectionId
                = currentSectionId
                    = destinedSectionId;
        }

        /// <summary>
        /// 目的地が変更されていたらセクション移動コルーチン再実行
        /// </summary>
        /// <param name="movingSectionId"></param>
        /// <param name="hierarchyDepth"></param>
        private void CheckChangeDestination(SectionId movingSectionId, int hierarchyDepth)
        {
            // 新たな出発地
            var lastDepartedSectionId = new SectionId(movingSectionId.GetPathFromDepth(hierarchyDepth));

            // 新たな目的地
            destinedSectionId = reservedSectionId;
            reservedSectionId = null;

            // GotoManage をやり直し
            rootObject.StopCoroutine(gotoManageCoroutine);
            gotoManageCoroutine = rootObject.StartCoroutine(GotoManage(lastDepartedSectionId, state));
        }

        /// <summary>
        /// パスからセクションを取得
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public SectionBase GetSection(string path)
        {
            List<string> pathList = GetPathList();
            foreach (var item in pathList)
            {
                if (item == path)
                {
                    var secId = new SectionId(path);
                    return GetNode(secId, secId.hierarchies.Count - 1).section;
                }
            }
            return null;
        }

        /// <summary>
        /// 全セクションへのパスを出力
        /// </summary>
        public void OutputAllSectionPath()
        {
            List<string> pathList = GetPathList();
            foreach (var item in pathList)
            {
                Debug.Log("[All section path] : " + item);
            }
        }

        /// <summary>
        /// パスが存在するか調べる
        /// </summary>
        /// <returns></returns>
        private bool IsExistPath(string path)
        {
            if (path == "") return true;

            List<string> pathList = GetPathList();
            foreach (var item in pathList)
            {
                if (item == path) return true;
            }
            return false;
        }

        /// <summary>
        /// 存在するパス全てを返す
        /// </summary>
        /// <returns></returns>
        private List<string> GetPathList()
        {
            List<string> pathList = new List<string>();
            foreach (var node in nodes)
            {
                node.AddPathList("/", pathList);
            }
            return pathList;
        }

        /// <summary>
        /// ノードを取得
        /// </summary>
        /// <param name="secId"></param>
        /// <param name="targetDepth">何番目の階層か（0~）</param>
        /// <returns></returns>
        private SectionNode GetNode(SectionId secId, int targetDepth)
        {
            foreach (var node in nodes)
            {
                var sec = node.GeNode(secId, targetDepth, 0);
                if (sec != null) return sec;
            }
            return null;
        }
    }
}
