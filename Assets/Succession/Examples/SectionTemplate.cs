using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Itach.Succession;

public class SectionTemplate : SectionBase
{
    // 初期設定
    public override SectionNode Initialize(string id)
    {
        return base.CreateSectionNode(id, this);
    }
    //-------------------------------------------------------------------------
    // ロード処理
    public override IEnumerator AtSectionLoad()
    {
        yield break;
    }
    //-------------------------------------------------------------------------
    // 到着処理
    public override IEnumerator AtSectionInit()
    {
        yield break;
    }
    //-------------------------------------------------------------------------
    // 出発処理
    public override IEnumerator AtSectionGoto()
    {
        yield break;
    }
    //-------------------------------------------------------------------------
    // アンロード処理
    public override IEnumerator AtSectionUnload()
    {
        yield break;
    }
    //-------------------------------------------------------------------------
}
