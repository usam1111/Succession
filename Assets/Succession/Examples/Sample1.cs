using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Itach.Succession;

public class Sample1 : MonoBehaviour
{
    public Succession suc;

    void Start()
    {
        suc = new Succession(this);

        // 各セクションを登録
        var a1 = suc.Add(GetComponent<SectionA>().Initialize("a1"));
        var b1 = a1.Add(GetComponent<SectionB>().Initialize("b1"));
        var b2 = a1.Add(GetComponent<SectionB>().Initialize("b2"));
        var c1 = b2.Add(GetComponent<SectionC>().Initialize("c1"));
        var a2 = suc.Add(GetComponent<SectionA>().Initialize("a2"));

        // 全セクションへのパスを出力
        suc.OutputAllSectionPath();

        // セクション移動
        suc.Goto("/a1");
    }

    // OnClick：セクション移動
    public void OnGoto(string path)
    {
        // セクション移動
        suc.Goto(path);
    }
}
